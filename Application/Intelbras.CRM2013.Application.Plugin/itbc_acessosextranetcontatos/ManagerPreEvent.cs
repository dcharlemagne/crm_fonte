using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Reflection;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.itbc_acessosextranetcontatos
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            //if (context.GetStage() != Stage.PreValidation)
            //    return;
            var e = context.GetContextEntity();
            var _service = new Intelbras.CRM2013.Domain.Servicos.AcessoExtranetContatoService(context.OrganizationName, context.IsExecutingOffline, null);
            var sAcessoExtranetContato = new AcessoExtranetContatoService(context.OrganizationName, context.IsExecutingOffline);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                #region Create

                case Domain.Enum.Plugin.MessageName.Create:

                    Domain.Model.AcessoExtranetContato acessoExtranetContatoEmail = e.Parse<Domain.Model.AcessoExtranetContato>(context.OrganizationName, context.IsExecutingOffline, userService);

                    Contato contatoObj = new Intelbras.CRM2013.Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline).BuscaContato(acessoExtranetContatoEmail.Contato.Id);

                    if (!string.IsNullOrEmpty(contatoObj.Email1))
                    {
                        Guid idContatoEmail = new RepositoryService().AcessoExtranetContato.VerificarEmail(contatoObj.Email1);
                        if (idContatoEmail != Guid.Empty && idContatoEmail != contatoObj.Id)
                        {
                            throw new ArgumentException(string.Format("(CRM) Duplicidade encontrada, existe outro contato com acesso a extranet com o mesmo e-mail: [{0}].", contatoObj.Email1));
                        }
                        else
                        {
                            if (new Intelbras.CRM2013.Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline).ContatoPossuiTodosCamposParaIntegracao(contatoObj))
                            {
                                contatoObj.IntegrarNoBarramento = true;
                                contatoObj.Update(contatoObj);
                            }
                            else
                            {
                                throw new ArgumentException("(CRM) Acesso não pode ser criado pois Contato não possui todos os campos para integração, favor completar o cadastro do contato.");
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("(CRM) Acesso não pode ser criado pois Contato não possui e-mail cadastrado, favor completar o cadastro do contato.");
                    }

                        break;
                #endregion
                #region Update
                case Domain.Enum.Plugin.MessageName.Update:

                    var entityTargetUpdate = (Entity)context.PreEntityImages["imagem"];

                    Domain.Model.AcessoExtranetContato acessoExtranetContatoEmailUpdate = entityTargetUpdate.Parse<Domain.Model.AcessoExtranetContato>(context.OrganizationName, context.IsExecutingOffline, userService);

                    Contato contatoObjUpdate = new Intelbras.CRM2013.Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline).BuscaContato(acessoExtranetContatoEmailUpdate.Contato.Id);

                    if (!string.IsNullOrEmpty(contatoObjUpdate.Email1))
                    {
                        Guid idContatoEmail = new RepositoryService().AcessoExtranetContato.VerificarEmail(contatoObjUpdate.Email1);
                        if (idContatoEmail != Guid.Empty && idContatoEmail != contatoObjUpdate.Id)
                        {
                            throw new ArgumentException(string.Format("(CRM) Duplicidade encontrada, existe outro contato com acesso a extranet com o mesmo e-mail: [{0}].", contatoObjUpdate.Email1));
                        }
                        else
                        {
                            if (new Intelbras.CRM2013.Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline).ContatoPossuiTodosCamposParaIntegracao(contatoObjUpdate))
                            {
                                contatoObjUpdate.IntegrarNoBarramento = true;
                                contatoObjUpdate.Update(contatoObjUpdate);
                            }
                            else
                            {
                                throw new ArgumentException("(CRM) Acesso não pode ser criado pois Contato não possui todos os campos para integração, favor completar o cadastro do contato.");
                            }
                        }
                    }else
                    {
                        throw new ArgumentException("(CRM) Acesso não pode ser criado pois Contato não possui e-mail cadastrado, favor completar o cadastro do contato.");
                    }

                    break;

                #endregion

                #region SetStateDynamicEntity

                case Domain.Enum.Plugin.MessageName.SetStateDynamicEntity:

                    var entityMerge = context.PreEntityImages["imagem"];
                    Domain.Model.AcessoExtranetContato acessoExtranetContato = entityMerge.Parse<Domain.Model.AcessoExtranetContato>(context.OrganizationName, context.IsExecutingOffline, userService);

                    if (acessoExtranetContato.Status == 0)
                    {
                        Contato contato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline).BuscaContato(acessoExtranetContato.Contato.Id);
                        Guid idContatoEmail = new RepositoryService().AcessoExtranetContato.VerificarEmail(contato.Email1);
                        if (idContatoEmail != Guid.Empty && idContatoEmail != contato.Id)
                        {
                            throw new ArgumentException(string.Format("(CRM) Duplicidade encontrada, existe outro contato com acesso a extranet com o mesmo e-mail: [{0}].", contato.Email1));
                        }else
                        {
                            contato.IntegrarNoBarramento = true;
                            contato.Update(contato);
                        }

                        acessoExtranetContato.Status = 1;
                        sAcessoExtranetContato.IntegracaoBarramento(acessoExtranetContato);
                        e.Attributes["itbc_acaocrm"] = true;
                    }
                    break;

                    #endregion
            }
        }
    }
}


