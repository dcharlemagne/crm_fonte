using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin.itbc_solicitacaodebeneficio
{
    public class ManagerPreEvent : PluginBase
    {
        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            SolicitacaoBeneficioService ServiceSolicitacaoBeneficio = new SolicitacaoBeneficioService(context.OrganizationName, context.IsExecutingOffline, adminService);

            switch (EnumConverter<MessageName>(context.MessageName.ToLower()))
            {
                #region Create

                case MessageName.Create:

                    var e = context.GetContextEntity();
                    var solicitacaoBeneficioTargetCreate = e.Parse<SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline);

                    ValidaValorAcao(ServiceSolicitacaoBeneficio, solicitacaoBeneficioTargetCreate);
                    AtualizaDataLimiteAposCriacao(ServiceSolicitacaoBeneficio, ref solicitacaoBeneficioTargetCreate, ref e);

                    AtualizaInformacoesPortfolio(ServiceSolicitacaoBeneficio, solicitacaoBeneficioTargetCreate, ref e);
                    AtualizaCodicaoPagamento(ServiceSolicitacaoBeneficio, solicitacaoBeneficioTargetCreate, ref e);
                    AtualizaDataValidade(ServiceSolicitacaoBeneficio, solicitacaoBeneficioTargetCreate, ref e);
                    //AtualizaSituacaoInrregular(ServiceSolicitacaoBeneficio, solicitacaoBeneficioTargetCreate, ref e);
                    AtualizaTrimestreCompetencia(ServiceSolicitacaoBeneficio, solicitacaoBeneficioTargetCreate, ref e);
                    AtualizaValores(ServiceSolicitacaoBeneficio, ref solicitacaoBeneficioTargetCreate, ref e);

                    ValidaValorAprovado(ServiceSolicitacaoBeneficio, solicitacaoBeneficioTargetCreate);
                    AtualizaNome(ServiceSolicitacaoBeneficio, ref solicitacaoBeneficioTargetCreate, ref e);

                    break;

                #endregion

                #region Update

                case MessageName.Update:

                    var entityMerge = context.PreEntityImages["imagem"];
                    var entityTargetUpdate = (Entity)context.InputParameters["Target"];

                    var solicitacaoPreUpdate = entityMerge.Parse<SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    foreach (var item in context.GetContextEntity().Attributes)
                    {
                        entityMerge.Attributes[item.Key] = item.Value;
                    }

                    var solicitacaoBeneficioMergeUpdate = entityMerge.Parse<SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline, adminService);



                    ValidaTrocaStatus(solicitacaoBeneficioMergeUpdate, solicitacaoPreUpdate);
                    ValidaValorAcao(ServiceSolicitacaoBeneficio, solicitacaoBeneficioMergeUpdate);

                    ServiceSolicitacaoBeneficio.ValidaDataParametrizadaParaConclusao(solicitacaoBeneficioMergeUpdate);

                    #region SOLICITAÇÃO DE BENEFICIO

                    if (solicitacaoBeneficioMergeUpdate.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                        || solicitacaoBeneficioMergeUpdate.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise
                        || solicitacaoBeneficioMergeUpdate.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada
                        || solicitacaoBeneficioMergeUpdate.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente
                        || solicitacaoBeneficioMergeUpdate.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.ComprovantesValidacao
                        || solicitacaoBeneficioMergeUpdate.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.ComprovacaoConcluida)
                    {
                        var tempSolicBen = ServiceSolicitacaoBeneficio.RecalculaValoresNaAlteracaoDeStatus(solicitacaoBeneficioMergeUpdate, solicitacaoPreUpdate);

                        if (tempSolicBen.ValorPago.HasValue)
                        {
                            entityTargetUpdate.Attributes["itbc_valorpago"] = new Money(tempSolicBen.ValorPago.Value);
                            solicitacaoBeneficioMergeUpdate.ValorPago = tempSolicBen.ValorPago;
                        }

                        if (tempSolicBen.ValorSolicitado.HasValue)
                        {
                            entityTargetUpdate.Attributes["itbc_valorsolicitado"] = new Money(tempSolicBen.ValorSolicitado.Value);
                            solicitacaoBeneficioMergeUpdate.ValorSolicitado = tempSolicBen.ValorSolicitado;
                        }

                        if (tempSolicBen.ValorAbater.HasValue)
                        {
                            entityTargetUpdate.Attributes["itbc_valoraabater"] = new Money(tempSolicBen.ValorAbater.Value);
                            solicitacaoBeneficioMergeUpdate.ValorAbater = tempSolicBen.ValorAbater;
                        }

                        if (tempSolicBen.ValorAprovado.HasValue)
                        {
                            entityTargetUpdate.Attributes["itbc_valoraprovado"] = new Money(tempSolicBen.ValorAprovado.Value);
                            solicitacaoBeneficioMergeUpdate.ValorAprovado = tempSolicBen.ValorAprovado;
                        }
                    }

                    #endregion

                    ValidaValorAprovado(ServiceSolicitacaoBeneficio, solicitacaoBeneficioMergeUpdate);
                    AtualizaNome(ServiceSolicitacaoBeneficio, ref solicitacaoBeneficioMergeUpdate, ref entityTargetUpdate);

                    break;

                #endregion

                #region SetStateDynamicEntity

                case MessageName.SetStateDynamicEntity:

                    if (!context.InputParameters.Contains("EntityMoniker") || !(context.InputParameters["EntityMoniker"] is EntityReference))
                        throw new ArgumentException("(CRM) SetStateDynamicEntity não contém EntityMoniker.");

                    EntityReference eSolicitacao = (EntityReference)context.InputParameters["EntityMoniker"];
                    OptionSetValue state = (OptionSetValue)context.InputParameters["State"];
                    OptionSetValue status = (OptionSetValue)context.InputParameters["Status"];

                    if (eSolicitacao.Id != Guid.Empty && state.Value == (int)Domain.Enum.SolicitacaoBeneficio.State.Inativo)
                    {
                        throw new ArgumentException("(CRM) Não é possível inativar uma solicitação!");
                    }
                    break;

                    #endregion
            }
        }

        private void AtualizaInformacoesPortfolio(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            var portfolioRepresentante = ServiceSolicitacaoBeneficio.ObterPortfolioRepresentante(solicitacaoBeneficio);
            if (portfolioRepresentante != null)
            {
                e.Attributes["itbc_assistadmvendasid"] = new EntityReference()
                {
                    Id = portfolioRepresentante.AssistentedeAdministracaodeVendas.Id,
                    LogicalName = SDKore.Crm.Util.Utility.GetEntityName<Usuario>(),
                    Name = portfolioRepresentante.AssistentedeAdministracaodeVendas.Name
                };
                e.Attributes["itbc_supervisorid"] = new EntityReference()
                {
                    Id = portfolioRepresentante.SupervisordeVendas.Id,
                    LogicalName = SDKore.Crm.Util.Utility.GetEntityName<Usuario>(),
                    Name = portfolioRepresentante.SupervisordeVendas.Name
                };
                e.Attributes["itbc_karepresentanteresponsvel"] = new EntityReference()
                {
                    Id = portfolioRepresentante.KeyAccountRepresentante.Id,
                    LogicalName = SDKore.Crm.Util.Utility.GetEntityName<Contato>(),
                    Name = portfolioRepresentante.KeyAccountRepresentante.Name
                };
            }
        }

        private void AtualizaCodicaoPagamento(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            if (e.Attributes.Contains("itbc_formapagamentoid"))
            {
                var condicaoPagamento = ServiceSolicitacaoBeneficio.ObterCondicaoDePagamento(solicitacaoBeneficio);
                if (condicaoPagamento != null)
                {
                    e.Attributes["itbc_condicaopagamentoid"] = new EntityReference()
                    {
                        Id = condicaoPagamento.ID.Value,
                        LogicalName = SDKore.Crm.Util.Utility.GetEntityName(condicaoPagamento),
                        Name = condicaoPagamento.Nome
                    };
                }
            }
        }

        private void AtualizaDataValidade(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            DateTime dataValidade = ServiceSolicitacaoBeneficio.ObterDataValidadeCriacao(solicitacaoBeneficio);

            if (dataValidade != DateTime.MinValue)
            {
                e.Attributes["itbc_datavalidade"] = dataValidade;
            }
        }

        /*
         * Removido do código. Essa verificação será feita futuramente
         * private void AtualizaSituacaoInrregular(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            bool situacaoInregular = ServiceSolicitacaoBeneficio.TemSituacaoInregularVMC(solicitacaoBeneficio);
            e["itbc_solicitacao_irregularidades"] = situacaoInregular;

            
            if (situacaoInregular)
            {
                e["itbc_situacao_irregular"] = "Solicitação de VMC acima do percentual determinado pela Intelbras.";
            }
             
        }
        */

        private void AtualizaTrimestreCompetencia(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            if (!e.Contains("itbc_trimestrecompetencia"))
            {
                e.Attributes["itbc_trimestrecompetencia"] = ServiceSolicitacaoBeneficio.ObterTrimestreCompetencia();
            }
        }

        private void AtualizaValores(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, ref SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            decimal? valorAbater = ServiceSolicitacaoBeneficio.ObterValorAbater(solicitacaoBeneficio);
            if (valorAbater.HasValue)
            {
                e.Attributes["itbc_valoraabater"] = new Money(valorAbater.Value);
                solicitacaoBeneficio.ValorAbater = valorAbater;
            }

            var retornoValores = ServiceSolicitacaoBeneficio.RecalculaValoresNaAlteracaoDeStatus(solicitacaoBeneficio, solicitacaoBeneficio);
            if (retornoValores.ValorAprovado.HasValue)
            {
                e.Attributes["itbc_valoraprovado"] = new Money(retornoValores.ValorAprovado.Value);
                solicitacaoBeneficio.ValorAprovado = retornoValores.ValorAprovado;
            }
        }

        private void ValidaValorAcao(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, SolicitacaoBeneficio solicitacaoBeneficio)
        {
            ServiceSolicitacaoBeneficio.ValidaValorAcao(solicitacaoBeneficio);
        }

        private void ValidaValorAprovado(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, SolicitacaoBeneficio solicitacaoBeneficio)
        {
            ServiceSolicitacaoBeneficio.ValidaValorAprovado(solicitacaoBeneficio);
        }

        private void ValidaTrocaStatus(SolicitacaoBeneficio target, SolicitacaoBeneficio preImage)
        {
            if (target.StatusSolicitacao.HasValue)
            {
                if (target.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada)
                {
                    if (preImage.StatusSolicitacao.Value == (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                    {
                        throw new ArgumentException("(CRM) A solicitação só pode ser cancelada, o status já está finalizado!");
                    }
                }
            }
        }

        private void AtualizaNome(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, ref SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            if (!e.Attributes.Contains("itbc_name") || solicitacaoBeneficio.TipoPriceProtection.HasValue)
            {
                string nome = ServiceSolicitacaoBeneficio.ObterNome(solicitacaoBeneficio);

                e.Attributes["itbc_name"] = nome;
                solicitacaoBeneficio.Nome = nome;
            }
        }

        private void AtualizaDataLimiteAposCriacao(SolicitacaoBeneficioService ServiceSolicitacaoBeneficio, ref SolicitacaoBeneficio solicitacaoBeneficio, ref Entity e)
        {
            bool criadaNoPrazo = ServiceSolicitacaoBeneficio.CriadaAposDataLimite(solicitacaoBeneficio);

            e.Attributes["itbc_criada_apos_data_limite"] = criadaNoPrazo;
            solicitacaoBeneficio.CriadaAposDataLimite = criadaNoPrazo;
        }

    }
}
