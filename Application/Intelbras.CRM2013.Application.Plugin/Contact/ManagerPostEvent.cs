using System;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Application.Plugin.Contact
{
    public class ManagerPostEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (context.GetMessageName())
            {
                case PluginBase.MessageName.Update:

                    if (context.PostEntityImages.Contains("imagem") && context.PostEntityImages["imagem"] is Entity
                        && context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                    {
                        var ContatoPre = ((Entity)context.PreEntityImages["imagem"]).Parse<Contato>(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var ContatoPos = ((Entity)context.PostEntityImages["imagem"]).Parse<Contato>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        var contatoService = new ContatoService(context.OrganizationName, context.IsExecutingOffline, adminService);
                        var contaService   = new ContaService(context.OrganizationName, context.IsExecutingOffline, adminService);

                        //Atualização de Treinamento
                        new BeneficioDoCanalService(context.OrganizationName, context.IsExecutingOffline, adminService).MudarEmpresa(ContatoPos);

                        #region Integração

                        if (context.Depth > 1)
                        {
                            return;
                        }

                        //Valida se contato tem acesso a Extranet e está sem e-mail
                        if (string.IsNullOrEmpty(ContatoPos.Email1))
                        {
                            ValidaEmailAcessoEXtranet(ContatoPos);
                        }

                        if (!ContatoPos.IntegrarNoPlugin && contatoService.ContatoPossuiTodosCamposParaIntegracao(ContatoPos) && (bool)ContatoPos.IntegrarNoBarramento)
                        {
                            Guid idContatoEmail = new RepositoryService().AcessoExtranetContato.VerificarEmail(ContatoPos.Email1);
                            if (idContatoEmail != Guid.Empty && idContatoEmail != ContatoPos.Id)
                            {
                                throw new ArgumentException(string.Format("(CRM) Duplicidade encontrada, existe outro contato com acesso a extranet com o mesmo e-mail: [{0}].", ContatoPos.Email1));
                            }
                            string xmlResposta = contatoService.IntegracaoBarramento(ContatoPos);
                        }

                        //Caso ele mude a associação tem que resetar o perfil do Konviva
                        //E consequentemente mudar a Unidade do Konviva
                        if (ContatoPre.AssociadoA == null && ContatoPos.AssociadoA != null
                                || (ContatoPre.AssociadoA != null && ContatoPos.AssociadoA != null
                                    && ContatoPre.AssociadoA.Id != ContatoPos.AssociadoA.Id)
                                || ContatoPre.AssociadoA != null && ContatoPos.AssociadoA == null)
                            {
                                Guid? guidTmp = ContatoPos.AssociadoA == null ? Guid.Empty : ContatoPos.AssociadoA.Id;
                                guidTmp = guidTmp == Guid.Empty ? null : guidTmp;

                                var acessoKonvivaService = new AcessoKonvivaService(context.OrganizationName, context.IsExecutingOffline, adminService);
                                var acessoExtranetContatoService = new AcessoExtranetContatoService(context.OrganizationName, context.IsExecutingOffline, adminService);

                                acessoKonvivaService.MudarCanal(ContatoPos, guidTmp);
                                acessoExtranetContatoService.MudarCanal(ContatoPos.ID.Value, guidTmp, ContatoPos.IntegrarNoPlugin);
                                if(ContatoPos.AssociadoA == null)
                                {
                                    contatoService.AlteraTipoRelacao(ContatoPos);
                                }
                            }


                            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                            {
                                var entidade = context.GetContextEntity();
                                Object papelCanalTmp = null;
                                if (entidade.Attributes.Contains("itbc_papelnocanal") && entidade.Attributes.TryGetValue("itbc_papelnocanal", out papelCanalTmp) && papelCanalTmp != null)
                                {
                                    if (ContatoPos.AssociadoA == null || entidade.GetAttributeValue<OptionSetValue>("itbc_papelnocanal").Value == (int)Intelbras.CRM2013.Domain.Enum.Contato.PapelNoCanal.Representante)
                                    {
                                        var postImage = (Entity)context.PostEntityImages["imagem"];
                                        postImage = UpdateImage(postImage, entidade);
                                        ContatoPos = postImage.Parse<Contato>(context.OrganizationName, context.IsExecutingOffline, adminService);

                                        var aKService = new AcessoKonvivaService(context.OrganizationName, context.IsExecutingOffline, adminService);
                                        AcessoKonviva acessoKonviva = aKService.ObterPorContato(ContatoPos.ID.Value, Domain.Enum.StateCode.Ativo);

                                        if (acessoKonviva != null)
                                        {
                                            acessoKonviva = new DeParaDeUnidadeDoKonvivaService(context.OrganizationName, context.IsExecutingOffline, adminService).ObterUnidadeKonvivaDeParaCom(acessoKonviva, null, ContatoPos);

                                            aKService.Persistir(acessoKonviva);
                                        }
                                    }
                                }
                            }

                            if ((ContatoPos.Email1 != ContatoPre.Email1) && ContatoPos.AcessoAoPortal == true && ContatoPos.Login != null)
                            {
                                if (ContatoPre.Email1 == string.Empty)
                                    (new RepositoryService(context.OrganizationName, context.IsExecutingOffline)).Contato.EnviaEmailAcessoPortalCorporativo(ContatoPos);
                                (new RepositoryService(context.OrganizationName, context.IsExecutingOffline)).Contato.UpdateEmailFBA(ContatoPos);
                            }

                            //new Domain.Services.PortalFidelidade().RemoverUsuarioDoSharepoint(contato);
                            //if (contato.ParticipaFidelidade.HasValue && contato.ParticipaFidelidade.Value)
                            //{
                            //    new Domain.Services.PortalFidelidade().AdicionarAoGrupoSharePoint(contato);
                            //}

                            #endregion
                        }

                        break;
                }
            }

            private void ValidaEmailAcessoEXtranet(Contato contatoUpdate)
            {
                var acessoExtranetContato = new RepositoryService().AcessoExtranetContato.ListarAcessosAtivosPorContato(contatoUpdate.Id);

                if (acessoExtranetContato.Count > 0)
                {
                    throw new ArgumentException("(CRM) O e-mail é obrigatório pois este contato possui acesso à Extranet. Se você deseja remover o e-mail do contato, primeiro inative o acesso à extranet do contato.");
                }
            }
        }
    }
