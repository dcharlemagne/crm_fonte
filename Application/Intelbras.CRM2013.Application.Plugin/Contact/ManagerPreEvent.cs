using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using SDKore.Helper;
using System;

namespace Intelbras.CRM2013.Application.Plugin.Contact
{
    public class ManagerPreEvent : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);

            try
            {
                switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(Context.MessageName))
                {
                    #region Create

                    case Domain.Enum.Plugin.MessageName.Create:

                        var e = (Entity)Context.InputParameters["Target"];
                        e.Attributes["itbc_integraintelbraspontua"] = false;

                        Domain.Model.Contato Contato = e.Parse<Domain.Model.Contato>(Context.OrganizationName, Context.IsExecutingOffline, service);
                        var ContatoServiceCreate = new Domain.Servicos.ContatoService(Context.OrganizationName, Context.IsExecutingOffline, service);
                        //var ContatoService = new Domain.Servicos.ContatoService(Context.OrganizationName, Context.IsExecutingOffline, service).ValidarDadosContato(Contato);
                        
                        //valida se o contato integra ou não no IntelbrasPontua
                        ValidaIntegraPontua(e, ContatoServiceCreate, Contato, ref Context, service);

                        if(Contato.CpfCnpj != null)
                        {
                            e.Attributes["new_cpf"] = Contato.CpfCnpj.GetOnlyNumbers();
                            e.Attributes["new_cpf_sem_mascara"] = Contato.CpfCnpj.GetOnlyNumbers();
                            e.Attributes["itbc_cpfoucnpj"] = Contato.CpfCnpj.InputMask();
                        }
                       
                        break;

                    #endregion

                    #region Update

                    case Domain.Enum.Plugin.MessageName.Update:

                        var entityTargetUpdate = (Entity)Context.InputParameters["Target"];
                        var ContatoService = new Domain.Servicos.ContatoService(Context.OrganizationName, Context.IsExecutingOffline, service);
                        var contatoUpdate = entityTargetUpdate.Parse<Domain.Model.Contato>(Context.OrganizationName, Context.IsExecutingOffline, service);

                        //valida se o contato integra ou não no IntelbrasPontua
                        ValidaIntegraPontua(entityTargetUpdate, ContatoService, contatoUpdate, ref Context, service);

                        if (contatoUpdate.CpfCnpj == null)
                        {
                            var contato = new Domain.Servicos.ContatoService(Context.OrganizationName, Context.IsExecutingOffline, service).BuscaContato(contatoUpdate.ID.Value);

                            entityTargetUpdate.Attributes["new_cpf"] = contato.CpfCnpj.GetOnlyNumbers();
                            entityTargetUpdate.Attributes["new_cpf_sem_mascara"] = contato.CpfCnpj.GetOnlyNumbers();
                            entityTargetUpdate.Attributes["itbc_cpfoucnpj"] = contato.CpfCnpj.InputMask();
                        }
                        else
                        {
                            entityTargetUpdate.Attributes["new_cpf"] = contatoUpdate.CpfCnpj.GetOnlyNumbers();
                            entityTargetUpdate.Attributes["new_cpf_sem_mascara"] = contatoUpdate.CpfCnpj.GetOnlyNumbers();
                            entityTargetUpdate.Attributes["itbc_cpfoucnpj"] = contatoUpdate.CpfCnpj.InputMask();
                        }
                        if ((contatoUpdate.Imagem != null) && (!contatoUpdate.IntegrarNoPlugin))
                        {
                            string xmlResposta = ContatoService.IntegracaoFotoBarramento(contatoUpdate);
                        }
                        break;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                string mensagem = SDKore.Helper.Error.Handler(ex);
                throw new InvalidPluginExecutionException(mensagem);
            }
        }

        

        private void ValidaIntegraPontua(Entity entityTargetUpdate, ContatoService ContatoService, Contato contatoUpdate, ref IPluginExecutionContext Context, IOrganizationService service)
        {
            ContatoService.ValidarDadosContato(contatoUpdate);

            var contato = ContatoService.BuscaContato(contatoUpdate.Id);
            if (contato != null)
            {
                if (contato.AssociadoA != null)
                {
                    //Pode estar vinculado à contatos
                    var contaAssociada = new Domain.Servicos.ContaService(Context.OrganizationName, Context.IsExecutingOffline, service).BuscaConta(contato.AssociadoA.Id);
                    if (contaAssociada != null)
                    {
                        var IntegraPontua = (new Domain.Servicos.BeneficioDoCanalService(Context.OrganizationName, Context.IsExecutingOffline, service).validaIntegraPontuaFielo(contaAssociada, null));
                        entityTargetUpdate.Attributes["itbc_integraintelbraspontua"] = IntegraPontua;
                    }
                }
                else if (entityTargetUpdate.Attributes.Contains("parentcustomerid"))
                {
                    if (entityTargetUpdate.Attributes["parentcustomerid"] != null)
                    {
                        var contaAssociada = new Domain.Servicos.ContaService(Context.OrganizationName, Context.IsExecutingOffline, service).BuscaConta(((EntityReference)entityTargetUpdate.Attributes["parentcustomerid"]).Id);
                        if (contaAssociada != null)
                        {
                            var IntegraPontua = (new Domain.Servicos.BeneficioDoCanalService(Context.OrganizationName, Context.IsExecutingOffline, service).validaIntegraPontuaFielo(contaAssociada, null));
                            entityTargetUpdate.Attributes["itbc_integraintelbraspontua"] = IntegraPontua;
                        }
                    }
                    else
                    {
                        entityTargetUpdate.Attributes["itbc_integraintelbraspontua"] = false;
                    }
                }
                else
                {
                    entityTargetUpdate.Attributes["itbc_integraintelbraspontua"] = false;
                }
            }
        }
    }
}