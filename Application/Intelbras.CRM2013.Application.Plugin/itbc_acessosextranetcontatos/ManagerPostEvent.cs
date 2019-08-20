using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;
using Model = Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Application.Plugin.itbc_acessosextranetcontatos
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var e = context.GetContextEntity();
            var mAcessoExtranetContato = e.Parse<Model.AcessoExtranetContato>(context.OrganizationName, context.IsExecutingOffline);
            var sAcessoExtranetContato = new AcessoExtranetContatoService(context.OrganizationName, context.IsExecutingOffline);

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:

                    switch (context.GetStage())
                    {
                        case Stage.PreOperation:
                            sAcessoExtranetContato.PreCreate(mAcessoExtranetContato);
                            break;

                        case Stage.PostOperation:
                            if (mAcessoExtranetContato.Contato == null)
                            {
                                throw new ArgumentNullException("Campo contato obrigatório.");
                            }

                            if (mAcessoExtranetContato.IntegrarNoPlugin.HasValue && !mAcessoExtranetContato.IntegrarNoPlugin.Value)
                            {
                                string xmlResposta = new Domain.Servicos.AcessoExtranetContatoService(context.OrganizationName,
                                context.IsExecutingOffline, sAcessoExtranetContato).IntegracaoBarramento(mAcessoExtranetContato);
                            }

                            var acessoKonvivaService = new AcessoKonvivaService(context.OrganizationName, context.IsExecutingOffline, userService);
                            acessoKonvivaService.CriarAcessoKonvivaPadrao(mAcessoExtranetContato.Contato.Id);
                            break;
                    }
                    break;

                #endregion

                #region Update

                case MessageName.Update:

                    var AcessoExtranetPost = ((Entity)context.PostEntityImages["imagem"]).Parse<Domain.Model.AcessoExtranetContato>(context.OrganizationName, context.IsExecutingOffline, sAcessoExtranetContato);
                    var preAcessoExtranet = (context.PreEntityImages["imagem"]).Parse<Model.AcessoExtranetContato>(context.OrganizationName, context.IsExecutingOffline, userService);
                    int dep = context.Depth;

                    switch (context.GetStage())
                    {
                        case Stage.PostOperation:

                            if ((AcessoExtranetPost.IntegrarNoPlugin.HasValue && !AcessoExtranetPost.IntegrarNoPlugin.Value) && AcessoExtranetPost.Status == preAcessoExtranet.Status)
                            {
                                string xmlResposta = new Domain.Servicos.AcessoExtranetContatoService(context.OrganizationName,
                                context.IsExecutingOffline, sAcessoExtranetContato).IntegracaoBarramento(AcessoExtranetPost);
                            }

                            if (AcessoExtranetPost.Status.HasValue)
                            {
                                if (AcessoExtranetPost.Status != preAcessoExtranet.Status)
                                {
                                    var deParaService = new DeParaDeUnidadeDoKonvivaService(context.OrganizationName, context.IsExecutingOffline, userService);
                                    var acessoKonvivaService = new AcessoKonvivaService(context.OrganizationName, context.IsExecutingOffline, userService);
                                    var acessKonviva = acessoKonvivaService.ObterPorContato(AcessoExtranetPost.Contato.Id, (Domain.Enum.StateCode)AcessoExtranetPost.Status);

                                    if (AcessoExtranetPost.Status == (int)Domain.Enum.AcessoExtranetContatos.StateCode.Ativo)
                                    {
                                        var contato = new ContatoService(context.OrganizationName, context.IsExecutingOffline, userService).BuscaContato(AcessoExtranetPost.Contato.Id);

                                        if (contato.AssociadoA != null && contato.PapelCanal != (int)Domain.Enum.Contato.PapelNoCanal.Representante)
                                        {
                                            var canal = new ContaService(context.OrganizationName, context.IsExecutingOffline, userService).BuscaConta(contato.AssociadoA.Id);
                                            acessKonviva = deParaService.ObterUnidadeKonvivaDeParaCom(acessKonviva, canal, null);
                                        }
                                        else {
                                            acessKonviva = deParaService.ObterUnidadeKonvivaDeParaCom(acessKonviva, null, contato);
                                        }
                                    }

                                    if (acessKonviva != null)
                                    {
                                        if (acessKonviva.ID.HasValue)
                                        {
                                            acessoKonvivaService.MudarStatus(acessKonviva.ID.Value, AcessoExtranetPost.Status.Value);
                                        }
                                        acessKonviva.Status = null; acessKonviva.Status = null;
                                        acessoKonvivaService.Persistir(acessKonviva);
                                    }
                                    
                                }
                            }

                            break;
                    }
                    break;

                #endregion
            }
        }
    }
}