using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Application.Plugin.itbc_produtosdasolicitacao
{
    public class ManagerPreEvent : PluginBase
    {
        private RepositoryService repoService = null;

        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            var produtosdaSolicitacaoService = new ProdutosdaSolicitacaoService(context.OrganizationName, context.IsExecutingOffline, userService);
            repoService = new RepositoryService(context.OrganizationName, context.IsExecutingOffline, userService);

            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                #region Create

                case Domain.Enum.Plugin.MessageName.Create:
                    {
                        var targetCreate = (Entity)context.InputParameters["Target"];

                        AtualizarValorAprovado(ref targetCreate);

                        var produtosdaSolicitacaoCreate = targetCreate.Parse<ProdutosdaSolicitacao>(context.OrganizationName, context.IsExecutingOffline, userService);

                        produtosdaSolicitacaoService.ValidaCamposObrigatorios(produtosdaSolicitacaoCreate);
                        AtualizaEstabelecimento(produtosdaSolicitacaoService, ref produtosdaSolicitacaoCreate, ref targetCreate);
                        ValidarDuplicidade(produtosdaSolicitacaoService, produtosdaSolicitacaoCreate);

                        ProdutoPortfolio produtoPortfolio1;
                        produtosdaSolicitacaoService.ValidaIntegridadeDados(produtosdaSolicitacaoCreate, out produtoPortfolio1);


                        AtualizaValores(produtosdaSolicitacaoService, repoService, ref produtosdaSolicitacaoCreate, ref targetCreate);

                        break;
                    }

                #endregion

                #region Update

                case Domain.Enum.Plugin.MessageName.Update:
                    var targetUpdate = context.GetContextEntity();
                    var preImageUpdate = context.PreEntityImages["imagem"];
                    var produtoSolicitacaoMergeUpdate = preImageUpdate.Parse<ProdutosdaSolicitacao>(context.OrganizationName, context.IsExecutingOffline, adminService);
                    SolicitacaoBeneficio solicBenef = repoService.SolicitacaoBeneficio.ObterPor(produtoSolicitacaoMergeUpdate.SolicitacaoBeneficio.Id, 0);

                    if (solicBenef.TipoPriceProtection != (int)Domain.Enum.SolicitacaoBeneficio.TipoPriceProtection.Autorizacao)
                    {
                        AtualizarValorAprovado(ref targetUpdate);
                    }

                    var produtoSolicitacaoTargetUpdate = targetUpdate.Parse<ProdutosdaSolicitacao>(context.OrganizationName, context.IsExecutingOffline, userService);

                    foreach (var item in targetUpdate.Attributes)
                    {
                        preImageUpdate.Attributes[item.Key] = item.Value;
                    }

                    if (produtoSolicitacaoMergeUpdate.State.HasValue && produtoSolicitacaoMergeUpdate.State.Value == (int)Domain.Enum.ProdutoSolicitacao.Status.Inativo)
                    {
                        break;
                    }

                    produtosdaSolicitacaoService.ValidaCamposObrigatorios(produtoSolicitacaoMergeUpdate);
                    ValidaAlteracaoProduto(produtoSolicitacaoTargetUpdate, produtoSolicitacaoMergeUpdate);
                    produtosdaSolicitacaoService.VerificaStatusDaSolicitacao(produtoSolicitacaoMergeUpdate, produtoSolicitacaoTargetUpdate.IntegradoDe);
                    AtualizaEstabelecimento(produtosdaSolicitacaoService, ref produtoSolicitacaoMergeUpdate, ref targetUpdate);

                    //ProdutoPortfolio produtoPortfolio;
                    //produtosdaSolicitacaoService.ValidaIntegridadeDados(produtoSolicitacaoMergeUpdate, out produtoPortfolio);
                    //AtualizaValores(produtosdaSolicitacaoService, repoService, ref produtoSolicitacaoMergeUpdate, ref targetUpdate);

                    solicBenef = repoService.SolicitacaoBeneficio.Retrieve(produtoSolicitacaoMergeUpdate.SolicitacaoBeneficio.Id);

                    ValidarDuplicidade(produtosdaSolicitacaoService, produtoSolicitacaoMergeUpdate);

                    break;
                #endregion

                #region Delete

                case Domain.Enum.Plugin.MessageName.Delete:

                    var preImageDelete = context.PreEntityImages["imagem"];
                    var produtoSolicitacaoDelete = preImageDelete.Parse<ProdutosdaSolicitacao>(context.OrganizationName, context.IsExecutingOffline, adminService);

                    var ServiceProdutosSolicitacao = new ProdutosdaSolicitacaoService(context.OrganizationName, context.IsExecutingOffline, userService);
                    produtosdaSolicitacaoService.VerificaStatusDaSolicitacao(produtoSolicitacaoDelete, "");

                    break;

                    #endregion
            }
        }

        private void ValidarDuplicidade(ProdutosdaSolicitacaoService produtosdaSolicitacaoService, ProdutosdaSolicitacao produtosdaSolicitacao)
        {
            if (produtosdaSolicitacaoService.ExisteDuplicidade(produtosdaSolicitacao))
            {
                throw new ArgumentException("(CRM) O Produto da Solicitação [" + produtosdaSolicitacao.Nome
                    + "] está duplicado, não pode haver mais de um Produto da Solicitação com o mesmo produto e nota fiscal.");
            }
        }

        private void ValidaAlteracaoProduto(ProdutosdaSolicitacao produtoSolicitacaoTarget, ProdutosdaSolicitacao produtoSolicitacaoImage)
        {
            if (produtoSolicitacaoTarget.Produto != null && produtoSolicitacaoImage.Produto != null)
            {
                if (produtoSolicitacaoTarget.Produto.Id != produtoSolicitacaoImage.Produto.Id)
                {
                    throw new ArgumentException("(CRM) Não é permitido alterar o campo Produto no Produto da Solicitação!");
                }
            }
        }

        private void AtualizaEstabelecimento(ProdutosdaSolicitacaoService produtosdaSolicitacaoService, ref ProdutosdaSolicitacao produtosdaSolicitacao, ref Entity e)
        {
            if (!e.Attributes.Contains("itbc_estabelecimentoid"))
            {
                var produtoEstabelecimento = produtosdaSolicitacaoService.ObterEstabelecimento(produtosdaSolicitacao);

                if (produtoEstabelecimento == null || produtoEstabelecimento.Estabelecimento == null)
                {
                    throw new ArgumentException("(CRM) Estabelecimento não encontrado para o produto:  " + produtosdaSolicitacao.Nome + ".");
                }

                e.Attributes.Add("itbc_estabelecimentoid", new EntityReference("itbc_estabelecimento", produtoEstabelecimento.Estabelecimento.Id));
                produtosdaSolicitacao.Estabelecimento = produtoEstabelecimento.Estabelecimento;
            }
        }
        private void AtualizaValores(ProdutosdaSolicitacaoService produtosdaSolicitacaoService, RepositoryService repoService, ref ProdutosdaSolicitacao produtosdaSolicitacao, ref Entity e)
        {

            if (e.Attributes.Contains("itbc_faturaid") || e.Attributes.Contains("itbc_qtdaprovada") || e.Attributes.Contains("itbc_productid"))
            {
                decimal[] valoresCalculados = CalculoValorUnitarioETotal(produtosdaSolicitacao, repoService);
                if (valoresCalculados != null)
                {
                    var solicBenef = repoService.SolicitacaoBeneficio.Retrieve(produtosdaSolicitacao.SolicitacaoBeneficio.Id);

                    if (solicBenef.TipoPriceProtection == null || solicBenef.TipoPriceProtection != (int)Intelbras.CRM2013.Domain.Enum.SolicitacaoBeneficio.TipoPriceProtection.Autorizacao)
                    {
                        e.Attributes["itbc_valorunitario"] = new Money(valoresCalculados[0]);
                        e.Attributes["itbc_valortotal"] = new Money(valoresCalculados[1]);
                        e.Attributes["itbc_valorunitarioaprovado"] = new Money(valoresCalculados[0]);
                        e.Attributes["itbc_valortotalaprovado"] = new Money(valoresCalculados[1]);

                        produtosdaSolicitacao.ValorUnitario = valoresCalculados[0];
                        produtosdaSolicitacao.ValorUnitarioAprovado = valoresCalculados[0];
                        produtosdaSolicitacao.ValorTotal = valoresCalculados[1];
                        produtosdaSolicitacao.ValorTotalAprovado = valoresCalculados[1];
                    }
                }
            }
        }

        public decimal[] CalculoValorUnitarioETotal(ProdutosdaSolicitacao produtoSolicitacao, RepositoryService repoService)
        {
            decimal[] valoresCalculados = { 0, 0 };
            decimal valorUnitario = 0;

            SolicitacaoBeneficio solicBen = repoService.SolicitacaoBeneficio.Retrieve(produtoSolicitacao.SolicitacaoBeneficio.Id);
            Beneficio beneficio = repoService.Beneficio.ObterPor(solicBen.BeneficioPrograma.Id);
            Product produto = repoService.Produto.Retrieve(produtoSolicitacao.Produto.Id);
            UnidadeNegocio unidadeNegocio = new UnidadeNegocio(repoService) { ID = solicBen.UnidadedeNegocio.Id, Nome = solicBen.UnidadedeNegocio.Name };
            Conta conta = repoService.Conta.Retrieve(solicBen.Canal.Id);
            Classificacao classificacao = new Classificacao(conta.OrganizationName, conta.IsOffline) { ID = conta.Classificacao.Id, Nome = conta.Classificacao.Name };
            Categoria categoria = repoService.Categoria.ObterPor(conta.Categoria.Id);

            if (beneficio.Codigo != (int)Domain.Enum.BeneficiodoPrograma.Codigos.StockRotation)
            {
                Estabelecimento estabelecimento = repoService.Estabelecimento.ObterPor(produtoSolicitacao.Estabelecimento.Id);

                var lstPrecoProduto = new List<PrecoProduto>();
                lstPrecoProduto.Add(new PrecoProduto(repoService.NomeDaOrganizacao, repoService.IsOffline)
                {
                    codEstabelecimento = estabelecimento.Codigo.Value,
                    CodigoProduto = produto.Codigo,
                    ContaId = solicBen.Canal.Id,
                    Produto = produto,
                    TipoCrossSelling = false,
                    Quantidade = Convert.ToInt32(produtoSolicitacao.QuantidadeAprovada.Value)
                });

                if (conta.Classificacao.Name != "Revendas")
                {
                    var precoProduto = new ProdutoService(repoService).ListarPor(lstPrecoProduto).First();

                    if (precoProduto == null || precoProduto.ValorProduto <= 0)
                    {
                        throw new ArgumentException("(CRM) Não foi possível calcular o preço do produto [" + produto.Nome + "]. Verifique política comercial");
                    }
                    valorUnitario = precoProduto.ValorProduto;
                }
                else
                {
                    ListaPrecoPSDPPPSCF lstListaPreco = repoService.ListaPrecoPSD.ObterPor(conta.Endereco1Estadoid.Id, produto.UnidadeNegocio.Id);
                    if (lstListaPreco == null)
                    {
                        throw new ArgumentException("(CRM) Não foi possível encontrar uma Lista de Preço(PSD) para o Estado [" + conta.Endereco1Estadoid.Name + "] e Unidade de Negócio [" + produto.UnidadeNegocio.Name + "]");
                    }
                    var precoProduto = repoService.ProdutoListaPSD.ListarPor(lstListaPreco.ID.Value, produto.ID).First();
                    if (precoProduto == null || precoProduto.ValorPSD <= 0)
                    {
                        throw new ArgumentException("(CRM) Não foi possível calcular o preço do produto [" + produto.Nome + "]. Verifique a Lista PSD para o Estado [" + conta.Endereco1Estadoid.Name + "]");
                    }
                    valorUnitario = precoProduto.ValorPSD.Value;
                }

            }
            else
            {
                ProdutoFatura prodFatura = repoService.ProdutoFatura.ObterObtemPorNotaFiscal(produtoSolicitacao.Produto.Id, produtoSolicitacao.Fatura.Id);

                if (prodFatura != null)
                {
                    valorUnitario = prodFatura.ValorLiquido.Value;
                }
                else
                {
                    throw new ArgumentException("(CRM) Produto não localizado na nota fiscal informada.");
                }
            }

            valorUnitario = CalcularDescontoValorSolicitado(valorUnitario, beneficio, unidadeNegocio, classificacao, categoria, repoService);
            valorUnitario = decimal.Round(valorUnitario, 2);
            valoresCalculados[0] = valorUnitario;
            valoresCalculados[1] = valorUnitario * produtoSolicitacao.QuantidadeAprovada.Value;

            return valoresCalculados;
        }

        private decimal CalcularDescontoValorSolicitado(decimal valor, Domain.Model.Beneficio beneficio, UnidadeNegocio unidadeNegocio, Classificacao classificacao, Categoria categoria, RepositoryService repoService)
        {
            decimal? fator = ObterPercentualDescontoValorSolicitado(beneficio, unidadeNegocio, classificacao, categoria, repoService);

            if (fator.HasValue)
            {
                valor = valor - ((valor * fator.Value) / 100);
            }

            return valor;
        }

        private decimal? ObterPercentualDescontoValorSolicitado(Beneficio beneficio, UnidadeNegocio unidadeNegocio, Classificacao classificacao, Categoria categoria, RepositoryService repoService)
        {
            decimal? fator = null;

            switch (beneficio.Codigo)
            {
                case (int)Domain.Enum.BeneficiodoPrograma.Codigos.Showroom:
                    var fatorShowRoom = repoService.ParametroGlobal.ObterPor((int)Domain.Enum.TipoParametroGlobal.PercentualDescontoShowRoom, unidadeNegocio.ID.Value, classificacao.ID.Value, categoria.ID, null, null, beneficio.ID, null);

                    if (fatorShowRoom == null)
                    {
                        throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Show Room não localizado para Unidade de Negócio [" + unidadeNegocio.Nome + "]");
                    }

                    fator = Convert.ToDecimal(fatorShowRoom.Valor);

                    break;

                case (int)Domain.Enum.BeneficiodoPrograma.Codigos.Backup:
                    var fatorBackup = repoService.ParametroGlobal.ObterPor((int)Domain.Enum.TipoParametroGlobal.PercentualDescontoBackup, unidadeNegocio.ID.Value, classificacao.ID.Value, categoria.ID, null, null, beneficio.ID, null);

                    if (fatorBackup == null)
                    {
                        throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Backup não localizado para Unidade de Negócio [" + unidadeNegocio.Nome + "]");
                    }

                    fator = Convert.ToDecimal(fatorBackup.Valor);

                    break;
            }

            return fator;
        }

        private void AtualizarValorAprovado(ref Entity e)
        {
            if (e.Contains("itbc_qtdsolicitado"))
            {
                if (e.Attributes.Contains("itbc_qtdaprovada"))
                {
                    e.Attributes.Remove("itbc_qtdaprovada");
                }

                e.Attributes.Add("itbc_qtdaprovada", e.Attributes["itbc_qtdsolicitado"]);
            }
        }
    }

}
