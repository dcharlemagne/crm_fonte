using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Sellout;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Domain.Integracao;

namespace Intelbras.CRM2013.Application.Plugin.itbc_solicitacaodebeneficio
{
    public class ManagerPostEventAssync : PluginBase
    {
        private readonly string NameFileLog = "log-calculo-price.txt";
        private readonly string NameFileTable = "calculo-price.csv";

        protected override void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService)
        {
            switch (Util.Utilitario.ConverterEnum<Domain.Enum.Plugin.MessageName>(context.MessageName))
            {
                #region Create

                case Domain.Enum.Plugin.MessageName.Create:

                    var entidade = (Entity)context.InputParameters["Target"];
                    var solicitacaoBeneficio = entidade.Parse<Domain.Model.SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline);

                    new SharepointServices(context.OrganizationName, context.IsExecutingOffline, adminService)
                        .CriarDiretorio<Domain.Model.SolicitacaoBeneficio>(solicitacaoBeneficio.Nome, solicitacaoBeneficio.ID.Value);

                    break;

                #endregion

                #region Update

                case Domain.Enum.Plugin.MessageName.Update:

                    if (context.Depth > 1)
                    {
                        return;
                    }

                    if (context.PreEntityImages.Contains("imagem") && context.PreEntityImages["imagem"] is Entity)
                    {
                        var ServiceSolicitacaoBeneficio = new SolicitacaoBeneficioService(context.OrganizationName, context.IsExecutingOffline, adminService);

                        var entityMerge = context.PostEntityImages["imagem"];
                        var solicMerge = entityMerge.Parse<Domain.Model.SolicitacaoBeneficio>(context.OrganizationName, context.IsExecutingOffline, adminService);

                        ServiceSolicitacaoBeneficio.CriarSolicitacaoComProdutosCancelados(solicMerge);

                        if (solicMerge.TipoPriceProtection.HasValue && solicMerge.TipoPriceProtection == (int)Domain.Enum.SolicitacaoBeneficio.TipoPriceProtection.Autorizacao)
                        {
                            if (solicMerge.StatusCalculoPriceProtection.HasValue && solicMerge.StatusCalculoPriceProtection == (int)Domain.Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calcular)
                            {
                                // Instanciando objetos utilizados para buscas
                                var ServiceProdSolicitacao = new ProdutosdaSolicitacaoService(context.OrganizationName, context.IsExecutingOffline, adminService);
                                var ServiceProd = new ProdutoService(context.OrganizationName, context.IsExecutingOffline, adminService);
                                var ServiceSellout = new SellOutService(context.OrganizationName, context.IsExecutingOffline);
                                var ServiceFatura = new FaturaService(context.OrganizationName, context.IsExecutingOffline);
                                var ServiceConta = new ContaService(context.OrganizationName, context.IsExecutingOffline);
                                var ServiceUnidade = new UnidadeNegocioService(context.OrganizationName, context.IsExecutingOffline);
                                var ServiceFamilia = new FamiliaComercialService(context.OrganizationName, context.IsExecutingOffline);
                                var ServiceEstabelecimento = new EstabelecimentoService(context.OrganizationName, context.IsExecutingOffline);
                                var ServiceArquivo = new ArquivoService(context.OrganizationName, context.IsExecutingOffline);


                                //trocar status para calculando
                                solicMerge.StatusCalculoPriceProtection = (int)Domain.Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calculando;
                                ServiceSolicitacaoBeneficio.Persistir(solicMerge);

                                // Obtendo dados para busca de quantidade de itens vendidos no Sellout
                                var lstProdSolic = ServiceProdSolicitacao.ListarPorSolicitacaoAtivos(solicMerge.ID.Value);
                                if (lstProdSolic.Count > 0)
                                {
                                    var lstTmpFilter = lstProdSolic.Select(c => (Guid)c.Produto.Id).ToList();

                                    var lstObjProd = ServiceProd.ListarProduto(lstTmpFilter);
                                    var listFilterSelloutWS = lstObjProd.Select(c => c.Codigo).ToList();
                                    var listFilterFatura = lstObjProd.Select(c => c.ID).ToList();

                                    var strListFilter = String.Join("','", listFilterSelloutWS);

                                    DateTime dataIni = (DateTime)solicMerge.DataCriacao;
                                    dataIni = dataIni.AddMonths(-1);
                                    DateTime dataFim = new DateTime(dataIni.Year, dataIni.Month, DateTime.DaysInMonth(dataIni.Year, dataIni.Month), 23, 59, 59);
                                    dataIni = dataIni.AddMonths(-2);
                                    dataIni = new DateTime(dataIni.Year, dataIni.Month, 1, 0, 0, 0);

                                    //Lista de total de unidades reportadas pelo sellout nos ultimos 90 dias, divididos por 2, para os produtos solicitados.
                                    var listValueSellout = ServiceSellout.listarContagemVenda(dataIni, dataFim, solicMerge.Canal.Id, strListFilter);

                                    //TODO - Determinar data referencia dos 6 meses para as faturas
                                    var listProdFaturas = ServiceFatura.listarContagemVendaPrice(solicMerge.Canal.Id, listFilterFatura, lstObjProd);

                                    //Ajuste de valor de quantidade
                                    foreach (var ajusteValuesTmp in listValueSellout)
                                    {
                                        var prodTmp = lstObjProd.Find(x => x.Codigo == ajusteValuesTmp.CodigoProdutoERP);
                                        if (prodTmp != null)
                                        {
                                            var prodSolicTmp = lstProdSolic.Find(x => x.Produto.Id == prodTmp.ID);
                                            if (prodSolicTmp != null)
                                            {
                                                if (prodSolicTmp.QuantidadeAjustada.HasValue)
                                                {
                                                    ajusteValuesTmp.TotalUnidades = (decimal)prodSolicTmp.QuantidadeAjustada;
                                                }
                                                else
                                                {
                                                    if (ajusteValuesTmp.TotalUnidades > prodSolicTmp.QuantidadeSolicitada)
                                                    {
                                                        ajusteValuesTmp.TotalUnidades = (decimal)prodSolicTmp.QuantidadeSolicitada;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //Busca de preços atuais
                                    var listPrecoProduto = new List<PrecoProduto>();
                                    foreach (var objSolicProd in lstProdSolic)
                                    {
                                        PrecoProduto precoProduto = new PrecoProduto(context.OrganizationName, context.IsExecutingOffline);
                                        Product objProd = lstObjProd.Find(x => x.ID == objSolicProd.Produto.Id);

                                        precoProduto.CodigoProduto = objProd.Codigo;
                                        precoProduto.ProdutoId = objProd.ID.Value;
                                        precoProduto.Produto = objProd;

                                        var contaObjTmp = ServiceConta.BuscaConta(solicMerge.Canal.Id);
                                        List<ProdutoPortfolio> lstProdutoPortifolio = ServiceProd.ProdutosPortfolio(contaObjTmp, contaObjTmp.Classificacao.Id, solicMerge.UnidadedeNegocio.Id);
                                        ProdutoPortfolio produtoPortfolioTmp = lstProdutoPortifolio.Find(x => x.Produto.Id == objProd.ID.Value);

                                        Estabelecimento estTmp = ServiceEstabelecimento.BuscaEstabelecimento(objSolicProd.Estabelecimento.Id);
                                        precoProduto.codEstabelecimento = (int)estTmp.Codigo;

                                        var unidadeTmp = ServiceUnidade.BuscaUnidadeNegocio(solicMerge.UnidadedeNegocio.Id);
                                        precoProduto.codUnidade = unidadeTmp.ChaveIntegracao;
                                        var famComTmp = ServiceFamilia.ObterPor(objProd.FamiliaComercial.Id);
                                        precoProduto.codFamiliaComl = famComTmp.Codigo;

                                        precoProduto.tipoPortofolio = produtoPortfolioTmp.PortfolioTipo;

                                        precoProduto.ContaId = solicMerge.Canal.Id;
                                        precoProduto.ValorProduto = 0;
                                        precoProduto.Quantidade = (int)objSolicProd.QuantidadeSolicitada;

                                        listPrecoProduto.Add(precoProduto);
                                    }
                                    listPrecoProduto = ServiceProd.ListarPor(listPrecoProduto);

                                    MSG0138 msg0138 = new MSG0138(context.OrganizationName, context.IsExecutingOffline);
                                    List<ValorProdutoICMSViewModel> lstPrecoProdICMS = msg0138.Enviar(lstObjProd, lstProdSolic, listPrecoProduto, solicMerge);

                                    List<ProdutoFaturaViewModel> lstProdFatGenerate = new List<ProdutoFaturaViewModel>();

                                    List<string> errorNoSellout = new List<string>();
                                    foreach (var objProdSolic in lstProdSolic)
                                    {
                                        Product objProd = lstObjProd.Find(x => x.ID == objProdSolic.Produto.Id);
                                        QtdProdutoSellout objQtdSellout = listValueSellout.Find(x => x.CodigoProdutoERP == objProd.Codigo);

                                        if (objQtdSellout == null || objQtdSellout.TotalUnidades <= 0)
                                        {
                                            string strError = "Produto: " + objProd.Codigo + " " + objProd.Nome + " | Nenhuma entrada de sellout para o produto";
                                            errorNoSellout.Add(strError);
                                        }
                                    }

                                    string urlSharepoint = string.Empty;
                                    var retUrlSharepoint = ServiceArquivo.ObterUrlArquivo(solicMerge.ID.ToString(), context.OrganizationName, out urlSharepoint);

                                    if (errorNoSellout.Count == 0)
                                    {
                                        foreach (var objProdSolic in lstProdSolic)
                                        {
                                            Product objProd = lstObjProd.Find(x => x.ID == objProdSolic.Produto.Id);
                                            QtdProdutoSellout objQtdSellout = listValueSellout.Find(x => x.CodigoProdutoERP == objProd.Codigo);
                                            objProdSolic.QuantidadeAprovada = 0;
                                            objProdSolic.ValorTotalAprovado = 0;
                                            objProdSolic.ValorTotal = 0;

                                            PrecoProduto objPrecoProd = listPrecoProduto.Find(x => x.Produto.ID == objProdSolic.Produto.Id);
                                            ValorProdutoICMSViewModel objValProdICMS = lstPrecoProdICMS.Find(x => x.CodigoProduto == objProd.Codigo);

                                            var lstProdFatLocal = listProdFaturas.Where(x => x.CodigoProduto == objProd.Codigo).OrderByDescending(x => x.DataEmissaoFatura).ToList();

                                            foreach (var objProdFaturas in lstProdFatLocal)
                                            {
                                                if (objQtdSellout.TotalUnidades == 0)
                                                {
                                                    objProdFaturas.QtdCalculo = 0;
                                                }
                                                else if (objProdFaturas.QtdFatura >= objQtdSellout.TotalUnidades)
                                                {
                                                    objProdFaturas.QtdCalculo = objQtdSellout.TotalUnidades;
                                                }
                                                else
                                                {
                                                    objProdFaturas.QtdCalculo = objProdFaturas.QtdFatura;
                                                }

                                                objProdFaturas.SaldoDiferenca = objProdFaturas.PrecoFatura - objValProdICMS.PrecoLiquido;

                                                if (objProdFaturas.SaldoDiferenca > 0)
                                                {
                                                    objProdFaturas.ValorIPIProduto = objProd.PercentualIPI;
                                                    objProdFaturas.PrecoCalculoAtual = objValProdICMS.PrecoLiquido;
                                                    objProdFaturas.TotalDiferenca = objProdFaturas.SaldoDiferenca * objProdFaturas.QtdCalculo;
                                                    objProdFaturas.TotalComIPI = objProdFaturas.TotalDiferenca + (objProdFaturas.TotalDiferenca * (objProd.PercentualIPI / 100));

                                                    objProdSolic.QuantidadeAprovada += objProdFaturas.QtdCalculo;
                                                    if (objProdFaturas.QtdFatura >= objQtdSellout.TotalUnidades)
                                                    {
                                                        objQtdSellout.TotalUnidades = 0;
                                                    }
                                                    else
                                                    {
                                                        objQtdSellout.TotalUnidades -= objProdFaturas.QtdFatura;
                                                    }

                                                    if (objProdSolic.ValorTotalAprovado.HasValue)
                                                    {
                                                        objProdSolic.ValorTotalAprovado += objProdFaturas.TotalComIPI;
                                                    }
                                                    else
                                                    {
                                                        objProdSolic.ValorTotalAprovado = objProdFaturas.TotalComIPI;
                                                    }

                                                    if (objProdSolic.ValorTotal.HasValue)
                                                    {
                                                        objProdSolic.ValorTotal += objProdFaturas.TotalComIPI;
                                                    }
                                                    else
                                                    {
                                                        objProdSolic.ValorTotal = objProdFaturas.TotalComIPI;
                                                    }

                                                }
                                                else if (objProdFaturas.SaldoDiferenca <= 0)
                                                {
                                                    objProdFaturas.ValorIPIProduto = objProd.PercentualIPI;
                                                    objProdFaturas.PrecoCalculoAtual = objValProdICMS.PrecoLiquido;
                                                }
                                                lstProdFatGenerate.Add(objProdFaturas);
                                            }

                                            if (objProdSolic.QuantidadeAprovada > 0 && objProdSolic.ValorTotalAprovado > 0)
                                            {
                                                var dateTime = DateTime.Now;
                                                ServiceArquivo.CriarExcelRecalculo(lstProdFatGenerate, dateTime.ToString("dd/MM/yyyy"), urlSharepoint, NameFileTable, context.OrganizationName, context.IsExecutingOffline);

                                                ServiceProdSolicitacao.Persistir(objProdSolic);
                                            }
                                            else if (lstProdFatLocal.Count > 0 && objProdSolic.ValorTotalAprovado <= 0)
                                            {
                                                string strError = "Produto: " + objProd.Codigo + " " + objProd.Nome + " | Nenhuma entrada nas Faturas do Canal com diferença positiva de preço para o Produto";
                                                errorNoSellout.Add(strError);
                                            }
                                            else
                                            {
                                                string strError = "Produto: " + objProd.Codigo + " " + objProd.Nome + " | Nenhuma entrada nas Faturas do Canal para o Produto";
                                                errorNoSellout.Add(strError);
                                            }
                                        }

                                        if (errorNoSellout.Count == 0)
                                        {
                                            solicMerge.StatusCalculoPriceProtection = (int)Domain.Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calculado;
                                            ServiceSolicitacaoBeneficio.Persistir(solicMerge);
                                        }
                                        else
                                        {
                                            ServiceArquivo.CriarArquivosErros(errorNoSellout, urlSharepoint, NameFileLog);
                                            solicMerge.StatusCalculoPriceProtection = (int)Domain.Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.ErroCalcular;
                                            ServiceSolicitacaoBeneficio.Persistir(solicMerge);
                                        }

                                    }
                                    else
                                    {
                                        ServiceArquivo.CriarArquivosErros(errorNoSellout, urlSharepoint, NameFileLog);
                                        solicMerge.StatusCalculoPriceProtection = (int)Domain.Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.ErroCalcular;
                                        ServiceSolicitacaoBeneficio.Persistir(solicMerge);
                                    }
                                }
                                else
                                {
                                    solicMerge.StatusCalculoPriceProtection = (int)Domain.Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calculado;
                                    ServiceSolicitacaoBeneficio.Persistir(solicMerge);
                                }
                            }
                        }
                    }

                    break;

                    #endregion
            }
        }
    }
}
