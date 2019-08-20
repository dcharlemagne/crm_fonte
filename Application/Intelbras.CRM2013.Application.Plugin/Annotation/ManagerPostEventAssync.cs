using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.Annotation
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
                    #region Create
                    case Domain.Enum.Plugin.MessageName.Create:

                        if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                        {
                            var entidade = (Entity)context.InputParameters["Target"];
                            Anexo anexo = entidade.Parse<Anexo>(context.OrganizationName, context.IsExecutingOffline);

                            var ocorrenciaService = new OcorrenciaService(context.OrganizationName, context.IsExecutingOffline, service);
                            OcorrenciaBase ocorrenciaBase = ocorrenciaService.BuscaOcorrenca(anexo.EntidadeRelacionada.Id);

                            if (ocorrenciaService.IsContractBradesco(ocorrenciaBase) && ocorrenciaBase.ReplicadoBradesco == false && ocorrenciaBase.Id == anexo.EntidadeRelacionada.Id)
                            {
                                /*Está condição serve para realizar uma nova tentatica de criação do incidente no HPSM caso 
                                 * tenha algum problema de integração durante a execução do create Incidente*/
                                if (ocorrenciaBase.OsCliente == null)
                                {
                                    trace.Trace("Inicio integração HPSM - Operação: REGISTRAR INCIDENTE - posCreate");
                                    ocorrenciaBase.TipoOperacao = "REGISTRAR INCIDENTE";
                                    ocorrenciaService.IntegracaoBradesco(ocorrenciaBase);
                                    trace.Trace("Fim integração HPSM - Operação: REGISTRAR INCIDENTE - posCreate");
                                }

                                if (anexo.NomeArquivos == null && anexo.Texto != "")
                                {
                                    trace.Trace("Inicio integração HPSM - Operação: REGISTRAR INFORMACAO COMPLEMENTAR - posCreate");
                                    ocorrenciaBase.TipoOperacao = "REGISTRAR INFORMACAO COMPLEMENTAR";
                                    ocorrenciaService.IntegracaoBradesco(ocorrenciaBase, anexo);
                                    trace.Trace("Fim integração HPSM - Operação: REGISTRAR INFORMACAO COMPLEMENTAR - posCreate");
                                }
                            }
                        }
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "annotation", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }

        }
    }
}