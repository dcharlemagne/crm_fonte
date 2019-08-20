using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Application.Plugin.AssociateEvent
{
    public class AssociateEvent : IPlugin
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
                    case Domain.Enum.Plugin.MessageName.Associate:
                            if ((context.InputParameters.Contains("Relationship")) && (context.InputParameters.Contains("RelatedEntities") && 
                                context.InputParameters["RelatedEntities"] is EntityReferenceCollection))
                            {
                                var targetEntity = (EntityReference)context.InputParameters["Target"];
                                var relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;

                                switch (((Relationship)context.InputParameters["Relationship"]).SchemaName.ToString())
                                {
                                    case "itbc_itbc_psdid_itbc_estado":

                                        List<Guid> lstEstados = new List<Guid>();
                                        Domain.Model.ListaPrecoPSDPPPSCF lstPSD = new Intelbras.CRM2013.Domain.Servicos.ListaPSDService(context.OrganizationName,context.IsExecutingOffline,service).ObterPor(targetEntity.Id);
                                        if(lstPSD == null)
                                            throw new ArgumentException("Lista PSD não encontrada!");

                                        foreach (EntityReference entidade in relatedEntities)
                                        {
                                            lstEstados.Add(entidade.Id);
                                        }
                                        if (new Intelbras.CRM2013.Domain.Servicos.ListaPSDService(context.OrganizationName, context.IsExecutingOffline, service).ValidarExistencia(lstPSD,lstEstados))
                                            throw new ArgumentException("Registro duplicado!Operação cancelada.");
                                        break;

                                    //Caso Lista PMA x Estado
                                    case "itbc_pricelevel_itbc_estado":
                                        List<Guid> lstEstadosPMA = new List<Guid>();
                                        Domain.Model.ListaPreco listaPMA = new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(context.OrganizationName, context.IsExecutingOffline, service).ObterPor(targetEntity.Id);
                                        if (listaPMA == null)
                                            throw new ArgumentException("Lista PMA não encontrada!");

                                        foreach (EntityReference entidade in relatedEntities)
                                        {
                                            lstEstadosPMA.Add(entidade.Id);
                                        }
                                        if (new Intelbras.CRM2013.Domain.Servicos.ListaPrecoService(context.OrganizationName, context.IsExecutingOffline, service).ValidarExistencia(listaPMA, lstEstadosPMA))
                                            throw new ArgumentException("Registro duplicado!Operação cancelada.");
                                        break;

                                        //Ligar conta com politica comercial
                                    case "itbc_itbc_politicacomercial_account":
                                        List<Guid> lstCanais = new List<Guid>();

                                        //Impedir duplicidade politica comercial- conta
                                        Domain.Model.PoliticaComercial politicaComercial = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).ObterPor(targetEntity.Id);

                                        
                                            //Só pode vincular quando for Aplicar politica para !=  perfil de canais
                                            if (politicaComercial.AplicarPoliticaPara.HasValue && politicaComercial.AplicarPoliticaPara.Value == (int)Domain.Enum.PoliticaComercial.AplicarPolíticaPara.PerfilDeCanais)
                                                throw new ArgumentException("(CRM)Não é possível realizar a operação: A política comercial é para perfis de canais.Não é possível vincular um canal");

                                            //Só pode vincular quando Tipo da politica for diferente de cross-selling
                                            if (politicaComercial.TipoDePolitica.HasValue && politicaComercial.TipoDePolitica.Value == (int)Domain.Enum.PoliticaComercial.TipoDePolitica.CrossSelling)
                                                throw new ArgumentException("(CRM)Não é possível realizar a operação: A política comercial é para cross selling.Não é possível vincular um canal");
                                            
                                            //Só verifica duplicidade se o registro estiver ativo
                                            if (politicaComercial.Status.HasValue && politicaComercial.Status.Value == (int)Domain.Enum.PoliticaComercial.Status.Ativo)
                                            {
                                                foreach (EntityReference entidade in relatedEntities)
                                                {
                                                    lstCanais.Add(entidade.Id);
                                                }
                                                if (true == new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).VerificarDuplicidadePoliticaRegistros(politicaComercial, lstCanais, "conta",true))
                                                    throw new ArgumentException("(CRM)Não é possível realizar a operação: O canal informado já está vinculado à outra política comercial com o mesmo tipo, aplicação, estabelecimento, unidade de negócio e data de vigência");
                                            }
                                        break;
                                    //Ligar Estado com politica comercial
                                    case "itbc_itbc_politicacomercial_itbc_estado":
                                        List<Guid> lstEstadosPoliticaComercial = new List<Guid>();

                                        //Impedir duplicidade politica comercial- Estado
                                        Domain.Model.PoliticaComercial _mPoliticaComercial = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).ObterPor(targetEntity.Id);

                                        //Só verifica duplicidade se o registro estiver ativo
                                        if (_mPoliticaComercial.Status.HasValue && _mPoliticaComercial.Status.Value == (int)Domain.Enum.PoliticaComercial.Status.Ativo)
                                        {
                                            foreach (EntityReference entidade in relatedEntities)
                                            {
                                                lstEstadosPoliticaComercial.Add(entidade.Id);
                                            }
                                            if (true == new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(context.OrganizationName, context.IsExecutingOffline, service).VerificarDuplicidadePoliticaRegistros(_mPoliticaComercial, lstEstadosPoliticaComercial, "estado",false))
                                                throw new ArgumentException("(CRM)Não é possível realizar a operação: O estado informado já está vinculado à outra política comercial com o mesmo tipo, aplicação, estabelecimento, unidade de negócio e data de vigência");
                                        }
                                        break;

                                }
                                
                            }
                        break;
                }
            }
            catch (Exception ex)
            {
                trace.Trace(String.Format("EXCEPTION PLUGIN {0} {1} [{2}]", context.MessageName.ToLower(), "AssociateEvent", DateTime.Now));
                trace.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}
