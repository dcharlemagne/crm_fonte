using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;
using SDKore.DomainModel;
using System.Reflection;


namespace Intelbras.CRM2013.Application.Plugin.itbc_acessoaokonviva
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            //if (context.GetStage() != Stage.PreValidation)
            //    return;
            var e = context.GetContextEntity();
            var _service = new Intelbras.CRM2013.Domain.Servicos.AcessoKonvivaService(context.OrganizationName, context.IsExecutingOffline, null);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                case Domain.Enum.Plugin.MessageName.Create:
                    {

                        Domain.Model.AcessoKonviva mAcessoKonviva = e.Parse<Domain.Model.AcessoKonviva>(context.OrganizationName, context.IsExecutingOffline, userService);

                        //Adicionado verificação preEvent pra poder mudar o nome da entidade
                        if (mAcessoKonviva.Contato == null)
                            throw new ArgumentException("(CRM) Contato obrigatório");

                        Domain.Model.Contato mContato = new Intelbras.CRM2013.Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline).BuscaContato(mAcessoKonviva.Contato.Id);

                        if (mContato == null)
                            throw new ArgumentException("(CRM) Contato não encontrado");

                        if (!new AcessoExtranetContatoService(context.OrganizationName, context.IsExecutingOffline, userService).ValidarExistenciaAcessoExtranet(mAcessoKonviva.Contato.Id))
                            throw new ApplicationException("(CRM) Contato não possui acesso na extranet");

                        if (!_service.ValidarTipoAcesso(mAcessoKonviva))
                            throw new ApplicationException("(CRM) Usuários sem vínculo com a Intelbras não podem criar Acesso no Konviva com Perfil Administrador");

                        //Já validamos se o contato existe mesmo na service com o método validarTipoAcesso
                        e.Attributes["itbc_name"] = mContato.NomeCompleto;

                        if (!e.Attributes.Contains("itbc_acaocrm") || e.GetAttributeValue<bool>("itbc_acaocrm") == false)
                        {
                            context.SharedVariables.Add("IntegraKonviva", true);
                        }
                        e.Attributes.Remove("itbc_acaocrm");

                        break;
                    }
                case Domain.Enum.Plugin.MessageName.Update:
                    {
                        Guid contatoId;
                        //Domain.Model.AcessoKonviva acessoKonvivaImagem = ((Entity)context.PreEntityImages["imagem"]).Parse<Domain.Model.AcessoKonviva>(context.OrganizationName, context.IsExecutingOffline, context.UserId);
                        Domain.Model.AcessoKonviva acessoKonvivaImagem = ((Entity)context.GetContextEntityMerge("imagem")).Parse<Domain.Model.AcessoKonviva>(context.OrganizationName, context.IsExecutingOffline, context.UserId);                        
                        Domain.Model.AcessoKonviva mAcessoKonvivaUpdate = e.Parse<Domain.Model.AcessoKonviva>(context.OrganizationName, context.IsExecutingOffline, userService);

                        //Adicionado verificação preEvent pra poder mudar o nome da entidade
                        if (acessoKonvivaImagem.Contato == null && mAcessoKonvivaUpdate.Contato == null)
                            throw new ArgumentException("(CRM) Contato obrigatório");

                        if (mAcessoKonvivaUpdate.Contato == null)
                            contatoId = acessoKonvivaImagem.Contato.Id;
                        else
                            contatoId = mAcessoKonvivaUpdate.Contato.Id;

                        Domain.Model.Contato mContatoUpdate = new Intelbras.CRM2013.Domain.Servicos.ContatoService(context.OrganizationName, context.IsExecutingOffline).BuscaContato(contatoId);

                        if (mContatoUpdate == null)
                            throw new ArgumentException("(CRM) Contato não encontrado");

                        if (!_service.ValidarTipoAcesso(acessoKonvivaImagem))
                        {
                            throw new ApplicationException("(CRM) Usuários sem vínculo com a Intelbras não podem criar Acesso no Konviva com Perfil Administrador");
                        }

                        //Já validamos se o contato existe mesmo na service validarTipoAcesso
                        e.Attributes["itbc_name"] = mContatoUpdate.NomeCompleto;

                        // SharedVariable para a Integração
                        if (!e.Attributes.Contains("itbc_acaocrm") || e.GetAttributeValue<bool>("itbc_acaocrm") == false)
                        {
                            context.SharedVariables.Add("IntegraKonviva", true);
                        }
                        e.Attributes.Remove("itbc_acaocrm");
                        break;
                    }
            }
        }
    }
}


