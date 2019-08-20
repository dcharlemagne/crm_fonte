using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Application.Plugin.new_diagnostico_ocorrencia
{
    public class ManagerPostEventAsync : IPlugin
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
                            OcorrenciaService ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service);
                            ocorrenciaService.Ocorrencia = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Ocorrencia.Retrieve(diagnosticoCreate.OcorrenciaId.Id);
                            ocorrenciaService.AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados();
                            //this.VerificaIntervencao(diagnosticoCreate, context, service);
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Update:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];
                            Domain.Model.Diagnostico diagnosticoUpdate = entidade.Parse<Domain.Model.Diagnostico>(context.OrganizationName, context.IsExecutingOffline, service);
                            OcorrenciaService ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service);
                            ocorrenciaService.Ocorrencia = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Ocorrencia.Retrieve(diagnosticoUpdate.OcorrenciaId.Id);
                            ocorrenciaService.AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados();
                            //this.VerificaIntervencao(diagnosticoUpdate, context, service);
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.Delete:

                        if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PreEntityImages["imagem"];
                            Domain.Model.Diagnostico diagnosticoUpdate = entidade.Parse<Domain.Model.Diagnostico>(context.OrganizationName, context.IsExecutingOffline, service);
                            OcorrenciaService ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service);
                            ocorrenciaService.Ocorrencia = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Ocorrencia.Retrieve(diagnosticoUpdate.OcorrenciaId.Id);
                            ocorrenciaService.AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados();
                        }

                        break;

                    case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                        if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity)
                        {
                            var entidade = context.PostEntityImages["imagem"];
                            Domain.Model.Diagnostico diagnosticoUpdate = entidade.Parse<Domain.Model.Diagnostico>(context.OrganizationName, context.IsExecutingOffline, service);
                            Diagnostico diagnostico = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Diagnostico.Retrieve(diagnosticoUpdate.Id);
                            OcorrenciaService ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service);
                            ocorrenciaService.Ocorrencia = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Ocorrencia.Retrieve(diagnostico.OcorrenciaId.Id);
                            ocorrenciaService.AlterarStatusDaOcorrenciaParaOMenorStatusDosDiagnosticosRelacionados();
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

        //private void VerificaIntervencao(Domain.Model.Diagnostico diagnostico, IPluginExecutionContext context, object service)
        //{
        //    if (context.Depth == 1)
        //    {

        //        //Se o contexto do pubin não carregou o campo da ocorrencia do diagnóstico, não verifica a intervenção
        //        if (diagnostico.OcorrenciaId == null)
        //            return;
        //        if (diagnostico.Ocorrencia == null)
        //            diagnostico.Ocorrencia = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Ocorrencia.Retrieve(diagnostico.OcorrenciaId.Id);
        //        if (new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Intervencao.ListarPor(diagnostico.Ocorrencia).Count > 0)
        //            return;

        //        var statusOcorrencia = (StatusDaOcorrencia)diagnostico.Ocorrencia.RazaoStatus.Value;
        //        if (statusOcorrencia != StatusDaOcorrencia.Aguardando_Analise && statusOcorrencia != StatusDaOcorrencia.Aguardando_Peça)
        //            return;

        //        if (diagnostico.ProdutoId != null && diagnostico.Produto == null)
        //            diagnostico.Produto = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Produto.Retrieve(diagnostico.ProdutoId.Id);

        //        if (diagnostico.GeraTroca.HasValue && diagnostico.GeraTroca.Value
        //            && diagnostico.QuantidadeSolicitada.Value > 0 && diagnostico.Produto != null && diagnostico.Produto.IntervencaoTecnica != null && diagnostico.Produto.IntervencaoTecnica.Value)
        //        {
        //            this.CriarIntervencaoTecnica(diagnostico, "Produto do diagnostico esta em Intervenção Técnica.", context, service);
        //            return;
        //        }

        //        LinhaComercial linhaComercial = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).LinhaComercial.ObterPor(diagnostico.Ocorrencia.Produto);

        //        if (linhaComercial == null)
        //            return;

        //        if (linhaComercial.NumeroDeDiasParaReincidencia.HasValue && linhaComercial.NumeroDeDiasParaReincidencia.Value >= 0)
        //        {

        //            DateTime dataCriacaoReincidente = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Ocorrencia.ObterDataDeCriacaoDoReincidentePorDiagnostico(diagnostico.Id);
        //            if (dataCriacaoReincidente != DateTime.MinValue)
        //                if (linhaComercial.NumeroDeDiasParaReincidencia.HasValue && dataCriacaoReincidente.AddDays(linhaComercial.NumeroDeDiasParaReincidencia.Value) >= diagnostico.Ocorrencia.CriadoEm)
        //                {
        //                    CriarIntervencaoTecnica(diagnostico, "Intervenção Técnica por reincidente.", context, service);
        //                    return;
        //                }
        //        }

        //        List<Domain.Model.Diagnostico> diagnosticos = new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Diagnostico.ListarPor(diagnostico.Ocorrencia);

        //        int quantidade = 0;
        //        foreach (Diagnostico item in diagnosticos)
        //            if (item.GeraTroca.HasValue && item.QuantidadeSolicitada.HasValue && item.GeraTroca.Value && item.QuantidadeSolicitada.Value > 0)
        //                quantidade += item.QuantidadeSolicitada.Value;

        //        if (linhaComercial.NumeroDeItensParaReincidencia.HasValue && linhaComercial.NumeroDeItensParaReincidencia.Value > 0 && quantidade >= linhaComercial.NumeroDeItensParaReincidencia)
        //        {
        //            CriarIntervencaoTecnica(diagnostico, "Intervenção Técnica por quantidade de itens.", context, service);
        //            return;
        //        }
        //    }
        //}

        //private void CriarIntervencaoTecnica(Domain.Model.Diagnostico diagnostico, string mensagem, IPluginExecutionContext context, object service)
        //{
        //    IntervencaoTecnica intervencao = new IntervencaoTecnica()
        //    {
        //        OcorrenciaId = diagnostico.OcorrenciaId,
        //        Nome = string.Format("{0} - {1}", diagnostico.Produto.CodigoEms, mensagem),
        //        RazaoStatus = 1
        //    };

        //    new Domain.Servicos.RepositoryService(context.OrganizationName, context.IsExecutingOffline, service).Intervencao.Create(intervencao);
        //}
    }
}
