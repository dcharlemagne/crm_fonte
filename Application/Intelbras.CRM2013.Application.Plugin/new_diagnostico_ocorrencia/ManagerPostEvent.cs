using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.Plugin.new_diagnostico_ocorrencia
{
    public class ManagerPostEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
                {
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            Domain.Model.Diagnostico diagnosticoCreate = entidade.Parse<Domain.Model.Diagnostico>(context.OrganizationName, context.IsExecutingOffline, service);
                            this.VerificaIntervencao(diagnosticoCreate, context);
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];
                            Domain.Model.Diagnostico diagnosticoUpdate = entidade.Parse<Domain.Model.Diagnostico>(context.OrganizationName, context.IsExecutingOffline, service);
                            this.VerificaIntervencao(diagnosticoUpdate, context);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "new_diagnostico_ocorrencia", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        private void VerificaIntervencao(Domain.Model.Diagnostico diagnostico, IPluginExecutionContext context)
        {
            if (context.Depth == 1)
            {
                //Controle de Id de ocorrência
                if (diagnostico.Ocorrencia == null && diagnostico.OcorrenciaId != null)
                    diagnostico.Ocorrencia = (new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Ocorrencia.Retrieve(diagnostico.OcorrenciaId.Id));
                
                if (diagnostico.Ocorrencia == null)
                {
                    return;
                }

                if (new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Intervencao.ListarPor(diagnostico.Ocorrencia).Count > 0)
                {
                    return;
                }

                var statusOcorrencia = (StatusDaOcorrencia)diagnostico.Ocorrencia.RazaoStatus.Value;
                if (statusOcorrencia != StatusDaOcorrencia.Aguardando_Analise && statusOcorrencia != StatusDaOcorrencia.Aguardando_Peça)
                {
                    return;
                }

                if (diagnostico.GeraTroca.HasValue
                    && diagnostico.GeraTroca.Value
                    && diagnostico.QuantidadeSolicitada > 0
                    && diagnostico.Produto != null
                    && diagnostico.Produto.IntervencaoTecnica != null
                    && diagnostico.Produto.IntervencaoTecnica.Value)
                {
                    this.CriarIntervencaoTecnica(diagnostico, "Produto do diagnóstico está em Intervenção Técnica.", context);
                    return;
                }


                LinhaComercial linhaComercial = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).LinhaComercial.ObterPor(diagnostico.Ocorrencia.Produto);

                if (linhaComercial == null)
                    return;

                if (linhaComercial.NumeroDeDiasParaReincidencia <= 0)
                    return;


                DateTime dataCriacaoReincidente = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Ocorrencia.ObterDataDeCriacaoDoReincidentePorDiagnostico(diagnostico.Id);
                if (dataCriacaoReincidente != DateTime.MinValue)
                    if (linhaComercial.NumeroDeDiasParaReincidencia.HasValue && dataCriacaoReincidente.AddDays(linhaComercial.NumeroDeDiasParaReincidencia.Value) >= diagnostico.Ocorrencia.CriadoEm)
                    {
                        CriarIntervencaoTecnica(diagnostico, "Intervenção Técnica por reincidente.", context);
                        return;
                    }


                List<Domain.Model.Diagnostico> diagnosticos = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Diagnostico.ListarPor(diagnostico.Ocorrencia);

                int quantidade = 0;
                foreach (Diagnostico item in diagnosticos)
                    if (item.GeraTroca.HasValue && item.GeraTroca.Value && item.QuantidadeSolicitada > 0)
                        quantidade += item.QuantidadeSolicitada.Value;

                if (linhaComercial.NumeroDeItensParaReincidencia > 0 && quantidade >= linhaComercial.NumeroDeItensParaReincidencia)
                {
                    CriarIntervencaoTecnica(diagnostico, "Intervenção Técnica por quantidade de itens.", context);
                    return;
                }
            }
        }

        private void CriarIntervencaoTecnica(Domain.Model.Diagnostico diagnostico, string mensagem, IPluginExecutionContext context)
        {
            IntervencaoTecnica intervencao = new IntervencaoTecnica()
            {
                OcorrenciaId = diagnostico.OcorrenciaId,
                Nome = string.Format("{0} - {1}", diagnostico.Produto.CodigoEms, mensagem),
                RazaoStatus = 1
            };

            new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline).Intervencao.Create(intervencao);
        }
    }
}
