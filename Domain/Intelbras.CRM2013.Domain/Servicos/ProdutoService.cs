using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ProdutoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        public Product Persistir(Product produto)
        {
            Product TmpProduto = RepositoryService.Produto.ObterPor(produto.Codigo);

            if (TmpProduto != null)
            {
                //Controle Ativa - Inativa
                int? statusAltera = produto.RazaoStatus;
                int? stateAltera = produto.Status;
                produto.Status = TmpProduto.Status;
                produto.Status = TmpProduto.RazaoStatus;

                produto.ID = TmpProduto.ID;
                produto.Id = TmpProduto.ID.Value;
                RepositoryService.Produto.Update(produto);

                produto.Status = stateAltera;
                produto.RazaoStatus = statusAltera;

                if (!TmpProduto.RazaoStatus.Equals(produto.RazaoStatus) && produto.RazaoStatus != null)
                    this.MudarStatus(TmpProduto.ID.Value, produto.Status.Value);

                return TmpProduto;
            }
            else
                produto.ID = RepositoryService.Produto.Create(produto);
            return produto;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Produto.AlterarStatus(id, status);
        }

        public void DescontinuarProduto(Product produto)
        {
            try
            {
                #region Descontinua Produto das Politicas Comerciais

                List<ProdutoPoliticaComercial> lstProdPolCom = RepositoryService.ProdutoPoliticaComercial.ListarPorProduto((Guid)produto.ID);

                foreach (ProdutoPoliticaComercial prodPolCom in lstProdPolCom)
                {
                    RepositoryService.ProdutoPoliticaComercial.AlterarStatus((Guid)prodPolCom.ID, (int)Enum.ProdutoPoliticaComercial.Status.Descontinuado);
                }

                #endregion

                #region Descontinua Produto dos Portfolios

                List<ProdutoPortfolio> lstProdPort = RepositoryService.ProdutoPortfolio.ListarPorProduto((Guid)produto.ID);

                foreach (ProdutoPortfolio prodPort in lstProdPort)
                {
                    RepositoryService.ProdutoPortfolio.AlterarStatus((Guid)prodPort.ID, (int)Enum.ProdutoPortfolio.Status.Descontinuado);
                }

                #endregion

                #region Descontinua Produto dos Treinamentos

                List<ProdutoTreinamento> lstProdTrei = RepositoryService.ProdutoTreinamento.ListarPorProduto((Guid)produto.ID);

                foreach (ProdutoTreinamento prodPort in lstProdTrei)
                {
                    RepositoryService.ProdutoTreinamento.AlterarStatus((Guid)prodPort.ID, (int)Enum.ProdutoTreinamento.Status.Descontinuado);
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) " + ex.Message);
            }

        }

        public List<ProdutoPortfolio> ProdutosPortfolio(Conta canal, Guid? classificacaoId, Guid? unidadeNegocioId)
        {
            Guid canalId = canal.ID.Value;

            List<ProdutoPortfolio> lstProdutosPortfolio = new List<ProdutoPortfolio>();
            List<Portfolio> lstPortfolio = new List<Portfolio>();
            List<Portfolio> lstPortfolioCrosseling = new List<Portfolio>();
            List<int> lstTipoCanal = new List<int>();

            #region Consulta a entidade “Categoria do Canal” [itbc_categoriasdocanal] onde o campo Canal [itbc_canalid] é igual o canal enviado,  obtém a Classificação [itbc_classificacaoid] e as Unidades de Negócio [itbc_businessunit] do Canal nos registros encontrados

            List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canalId, unidadeNegocioId, classificacaoId, null, null);

            List<Guid> lstUN = new List<Guid>();

            foreach (CategoriasCanal catCanal in lstCatCanal)
            {
                if (!lstUN.Contains(catCanal.UnidadeNegocios.Id))
                    lstUN.Add(catCanal.UnidadeNegocios.Id);
            }

            #endregion

            #region Obtém todos os Produtos do Portfolio cujo Portfolio relacionado tenha Tipo igual a Normal e a Unidade de Negócio igual as do Canal
            lstTipoCanal.Add((int)Enum.Portfolio.Tipo.Normal);
            #endregion

            #region Caso o Canal seja exclusivo (se o atributo itbc_exclusividade for igual a “Sim”), obtém todos os Produtos do Portfolio cujo Portfolio relacionado tenho o campo Tipo igual a Exclusivo e a Unidade de Negócio é igual as do Canal.

            if (canal.Exclusividade == true)
            {
                lstTipoCanal.Add((int)Enum.Portfolio.Tipo.Exclusivo);
            }

            #endregion

            #region Une as três lista acima em uma lista de produtos do portfólio.
            //Portifolio relacionados da Unidade
            //Se não tiver unidade de negocios e/ou tipoCanal, retorna 0.
            if (lstUN.Count > 0)
            {
                lstPortfolio = RepositoryService.Portfolio.ListarPor(lstUN, lstTipoCanal, classificacaoId);

                if (RepositoryService.Classificacao.Retrieve(classificacaoId.Value).AcessaSolucoes)
                {
                    List<Portfolio> lstPortfolioSolucao = RepositoryService.Portfolio.ListarPor(lstUN, (int)Enum.Portfolio.Tipo.Solucao, classificacaoId);

                    foreach (Portfolio item in lstPortfolioSolucao)
                    {
                        //new
                        lstPortfolio.Add(item);

                    }
                }
            }
            else
                return lstProdutosPortfolio;

            #endregion

            if (RepositoryService.Classificacao.Retrieve(classificacaoId.Value).AcessaCrossSelling)
            {
                lstPortfolioCrosseling = RepositoryService.Portfolio.ListarPorCrosseling(lstUN, (int)Enum.ProdutoPortfolio.Tipo.CrossSelling, classificacaoId);
                foreach (Portfolio item in lstPortfolioCrosseling)
                {
                    if (item.Tipo.Equals((int)Enum.ProdutoPortfolio.Tipo.CrossSelling))
                    {
                        //new
                        List<ProdutoPortfolio> lsta = RepositoryService.ProdutoPortfolio.ListarPor((Guid)item.ID, null);
                        foreach (var item2 in lsta)
                        {
                            item2.PortfolioTipo = item.Tipo.Value;
                            item2.CanalId = canal.ID.Value;
                        }

                        lstProdutosPortfolio.AddRange(lsta);
                    }
                }
            }

            //Busca segmento do canal 
            List<Segmento> lstSegmentoCanal = RepositoryService.Segmento.ListarPorContaSegmento((Guid)canalId);

            // Se a lista de segmento for vazia, então não tem relacionamento com produto Normal e Solução
            if (lstSegmentoCanal.Count > 0)
            {
                List<Guid> lstSegmentoCanalId = new List<Guid>();
                {
                    foreach (var segmentocanal in lstSegmentoCanal)
                    {
                        lstSegmentoCanalId.Add(segmentocanal.ID.Value);
                    }
                }

                foreach (Portfolio portfolio in lstPortfolio)
                {

                    List<ProdutoPortfolio> lsta = RepositoryService.ProdutoPortfolio.ListarPor((Guid)portfolio.ID, lstSegmentoCanalId);
                    foreach (var item in lsta)
                    {
                        item.PortfolioTipo = portfolio.Tipo.Value;
                        item.CanalId = canal.ID.Value;
                    }

                    lstProdutosPortfolio.AddRange(lsta);
                }
            }

            return lstProdutosPortfolio;
        }

        public List<Model.ProdutoPortfolio> filtrarProdutoPortfolioPorShowroom(List<Model.ProdutoPortfolio> lstProdutosPortfolio)
        {
            List<Model.ProdutoPortfolio> tmpLstProdutosPortfolio = new List<ProdutoPortfolio>();

            foreach (Model.ProdutoPortfolio prodPort in lstProdutosPortfolio)
            {
                tmpLstProdutosPortfolio.Add(prodPort);

                Product produto = RepositoryService.Produto.ObterPor(prodPort.Produto.Id);

                if (produto.Showroom != true)
                {
                    tmpLstProdutosPortfolio.Remove(prodPort);
                }

            }
            return tmpLstProdutosPortfolio;
        }

        public Model.Product BuscaPorCodigo(int productNumber)
        {
            return RepositoryService.Produto.ObterPor(productNumber);
        }

        public Product BuscaPorCodigo(string productNumber)
        {
            return RepositoryService.Produto.ObterPor(productNumber);
        }

        public List<Product> BuscaPorCodigo(string[] productNumbers)
        {
            return RepositoryService.Produto.ObterPor(productNumbers);
        }

        public Product ExtrairPorCodigo(string productNumber, List<Product> listaProdutos)
        {
            foreach (var item in listaProdutos)
                if (item.Codigo.ToLower() == productNumber.ToLower())
                    return item;

            return null;
        }

        public Product ObterPor(Guid productId)
        {
            return RepositoryService.Produto.ObterPor(productId);
        }

        public List<Product> ListarProduto(List<Guid> lstProdudcId)
        {
            return RepositoryService.Produto.ListarProdutos(lstProdudcId);
        }

        public List<ProdutoTreinamento> ProdutoTreinamentoListarTodos()
        {
            return RepositoryService.ProdutoTreinamento.ListarTodos();
        }

        public List<TreinamentoCanal> TreinamentoCanalListarTodos()
        {
            return RepositoryService.TreinamentoCanal.ListarTodos();
        }


        public List<Intelbras.Message.Helper.Entities.ProdutoR1> ListarPSDPPPSCF(Guid unidadeNegocioId, Guid moedaId, Guid estadoId, Guid? produtoId)
        {
            ProdutoListaPSDPPPSCF prodTemp = null;
            //Localizar na entidade ""Lista PMA"" registros que possuam os mesmos valores de ""Unidade de Negócio"" e ""Estado""  
            //(entidade  N:N relacionada) e que sejam válidas para a data da solicitação (verificar ""Data de Início"" e ""Data de Término"").
            List<ListaPreco> lstListaPreco = RepositoryService.ListaPreco.ListarPor(Enum.ListaPreco.Tipo.PMA, estadoId, unidadeNegocioId);
            List<ListaPrecoPSDPPPSCF> lstListaPsdPpPscf = RepositoryService.ListaPrecoPSD.ListarPor(unidadeNegocioId, estadoId);

            //Caso não exista localizar os que possuam o mesmo valor de ""Unidade de Negócio"".
            if (lstListaPsdPpPscf == null || lstListaPsdPpPscf.Count == 0)
            {
                lstListaPsdPpPscf = RepositoryService.ListaPrecoPSD.ListarPor(unidadeNegocioId, null);
            }

            List<Intelbras.Message.Helper.Entities.ProdutoR1> lstPollux = new List<Intelbras.Message.Helper.Entities.ProdutoR1>();

            if (lstListaPsdPpPscf != null && lstListaPsdPpPscf.Count > 0)
            {
                foreach (var itemLstPreco in lstListaPsdPpPscf)
                {
                    //if (hasPrice)
                    //    break;
                    var item = RepositoryService.ProdutoListaPSD.ListarPor(itemLstPreco.ID.Value, produtoId);
                    List<ProdutoListaPSDPPPSCF> lstItems = RepositoryService.ProdutoListaPSD.ListarPor(itemLstPreco.ID.Value, produtoId);
                    if (lstItems.Count > 0)
                    {

                        List<Guid> lstCodigoProd = new List<Guid>();
                        //Carregar lista de Guid produto - Para consulta em bloco
                        foreach (var itemCodProd in lstItems)
                        {
                            if (itemCodProd.Produto != null)
                                lstCodigoProd.Add(itemCodProd.Produto.Id);
                        }

                        List<Product> lstProdutosRetorno = RepositoryService.Produto.ListarProdutos(lstCodigoProd);



                        foreach (var prodItem in lstProdutosRetorno)
                        {
                            Intelbras.Message.Helper.Entities.ProdutoR1 produtoRetorno = new Message.Helper.Entities.ProdutoR1();
                            produtoRetorno.CodigoProduto = prodItem.Codigo;

                            if (prodItem.Nome.Length > 60)
                                throw new ArgumentException("(CRM) Nome do produto é maior que o permitido: " + prodItem.Nome);
                            else
                                produtoRetorno.NomeProduto = prodItem.Nome;

                            #region Atribuir Valores PSD, PP e PSCF
                            prodTemp = lstItems.Where(x => x.Produto.Id == prodItem.ID.Value).FirstOrDefault();

                            if (prodTemp.ValorPSD.HasValue)
                                produtoRetorno.ValorPSD = prodTemp.ValorPSD.Value;
                            else
                                produtoRetorno.ValorPSD = 0;

                            if (prodTemp.ValorPP.HasValue)
                                produtoRetorno.ValorPP = prodTemp.ValorPP.Value;
                            else
                                produtoRetorno.ValorPP = 0;

                            if (prodTemp.ValorPSCF.HasValue)
                                produtoRetorno.Valor = prodTemp.ValorPSCF.Value;
                            else
                                produtoRetorno.Valor = 0;

                            if (prodTemp.ControlaPSD.HasValue)
                                produtoRetorno.ControlaPSD = prodTemp.ControlaPSD;
                            else
                                produtoRetorno.Valor = 0;
                            #endregion

                            lstPollux.Add(produtoRetorno);
                        }

                    }

                }
            }

            return lstPollux;

        }

        public List<Intelbras.Message.Helper.Entities.ProdutoItemPSD> ListarPSD0087(Guid unidadeNegocioId, Guid moedaId, Guid estadoId, Guid? produtoId)
        {

            ListaPrecoPSDPPPSCF lstListaPreco = RepositoryService.ListaPrecoPSD.ObterPor((Guid)estadoId, unidadeNegocioId);

            if (lstListaPreco == null)
            {
                lstListaPreco = RepositoryService.ListaPrecoPSD.ObterPor(null, unidadeNegocioId);
            }

            List<Intelbras.Message.Helper.Entities.ProdutoItemPSD> lstPollux = new List<Intelbras.Message.Helper.Entities.ProdutoItemPSD>();

            if (lstListaPreco != null)
            {
                List<ProdutoListaPSDPPPSCF> lstItems = RepositoryService.ProdutoListaPSD.ListarPor(lstListaPreco.ID.Value, produtoId);
                if (lstItems.Count > 0)
                {
                    List<Guid> lstCodigoProd = new List<Guid>();
                    //Carregar lista de Guid produto - Para consulta em bloco
                    foreach (var itemCodProd in lstItems)
                    {
                        if (itemCodProd.Produto != null)
                            lstCodigoProd.Add(itemCodProd.Produto.Id);
                    }

                    List<Product> lstProdutosRetorno = RepositoryService.Produto.ListarProdutos(lstCodigoProd);


                    foreach (var prodItem in lstProdutosRetorno)
                    {
                        Intelbras.Message.Helper.Entities.ProdutoItemPSD produtoRetorno = new Message.Helper.Entities.ProdutoItemPSD();
                        ProdutoListaPSDPPPSCF prodListaPsdTmp = new ProdutoListaPSDPPPSCF(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

                        prodListaPsdTmp = lstItems.Where(x => x.Produto.Id == prodItem.ID.Value).FirstOrDefault();

                        produtoRetorno.CodigoProduto = prodItem.Codigo;
                        if (prodItem.Nome.Length > 60)
                            throw new ArgumentException("(CRM) Nome do produto é maior que o permitido: " + prodItem.Nome);
                        else
                            produtoRetorno.NomeProduto = prodItem.Nome;

                        produtoRetorno.PSDObrigatorio = prodListaPsdTmp.PSDControlado.Value;

                        if (prodListaPsdTmp.ValorPSD.HasValue)
                            produtoRetorno.Valor = prodListaPsdTmp.ValorPSD.Value;
                        else
                            produtoRetorno.Valor = 0;

                        if (lstListaPreco.FatorPrecoPredatorio.HasValue && produtoRetorno.Valor.HasValue)
                            produtoRetorno.PrecoPredatorio = lstListaPreco.FatorPrecoPredatorio * produtoRetorno.Valor.Value;
                        else
                            produtoRetorno.PrecoPredatorio = 0;


                        lstPollux.Add(produtoRetorno);
                    }
                }

            }

            return lstPollux;

        }

        public List<Product> ListarPMA(Guid estadoId, List<string> codigosProduto = null)
        {

            List<Product> lstRetorno = new List<Product>();

            if (codigosProduto != null)
            {
                List<Product> lstProdutos = RepositoryService.Produto.ListarPor(codigosProduto);

                if (lstProdutos != null)
                {
                    foreach (var itemProduto in lstProdutos)
                    {
                        List<ListaPreco> lstListaPreco = RepositoryService.ListaPreco.ListarPor(Enum.ListaPreco.Tipo.PMA, (Guid)estadoId, itemProduto.UnidadeNegocio.Id);
                        bool hasPrice = false;

                        if (lstListaPreco != null && lstListaPreco.Count > 0)
                        {
                            foreach (var itemLstPreco in lstListaPreco)
                            {
                                if (hasPrice)
                                    break;
                                List<ItemListaPreco> lstItems = RepositoryService.ItemListaPreco.ListarPor((Guid)itemLstPreco.ID, null);
                                if (lstItems.Count > 0)
                                {
                                    foreach (var itemProdutoPMA in lstItems)
                                    {
                                        if (itemProdutoPMA.ProdutoID.Id == itemProduto.ID.Value)
                                        {
                                            itemProduto.CustoAtual = itemProdutoPMA.Valor;
                                            hasPrice = true;
                                            break;
                                        }
                                        else
                                            itemProduto.CustoAtual = 0;
                                    }
                                }
                                else
                                    itemProduto.CustoAtual = 0;
                            }
                        }
                        else
                        {
                            itemProduto.CustoAtual = 0;
                        }
                    }
                    lstRetorno = lstProdutos;
                }
            }

            return lstRetorno;
        }

        public List<Product> ListarPSD(Intelbras.Message.Helper.MSG0087 objPollux)
        {

            Estado estado = RepositoryService.Estado.ListarPor(objPollux.Estados.First()).First();

            List<string> codigosProdutos = new List<string>();

            codigosProdutos.Add(objPollux.CodigoMensagem);

            return ListarPSD((Guid)estado.ID, codigosProdutos);
        }

        public List<Product> ListarPSD(Guid estadoId, List<string> codigosProduto = null)
        {

            List<Product> lstRetorno = new List<Product>();

            if (codigosProduto != null)
            {
                List<Product> lstProdutos = RepositoryService.Produto.ListarPor(codigosProduto);

                if (lstProdutos != null)
                {
                    foreach (var itemProduto in lstProdutos)
                    {
                        ProdutoListaPSDPPPSCF prodPSD = RepositoryService.ProdutoListaPSD.ObterPor(itemProduto.ID.Value, estadoId, itemProduto.UnidadeNegocio.Id);

                        if (prodPSD != null)
                            itemProduto.CustoAtual = prodPSD.ValorPSD;
                        else
                            itemProduto.CustoAtual = 0;
                    }
                    lstRetorno = lstProdutos;
                }
            }

            return lstRetorno;
        }

        public List<Product> ListarParaMeta(Guid unidadeNegocioId)
        {
            var listaGrupoEstoque = new GrupoEstoqueService(RepositoryService).ListarGrupoEstoqueParaMeta();
            var listaGuidGrupoEstoque = listaGrupoEstoque.Select(x => x.ID.Value).ToArray();

            var listaProdutos = RepositoryService.Produto.ListarParaMeta(unidadeNegocioId, listaGuidGrupoEstoque);



            return listaProdutos;
        }

        public List<Product> ListarPor(Guid? unidadeNegocioId, Guid? familiaComercialId, Guid? familiaMaterialId, Guid? familiaProdutoId)
        {
            List<Product> lstRetorno = RepositoryService.Produto.ListarPor(unidadeNegocioId, familiaComercialId, familiaMaterialId, familiaProdutoId);

            return lstRetorno;
        }

        public List<Product> ListarPorUnidadeNegocio(Guid unidadeNegocioId)
        {
            List<Product> lstRetorno = RepositoryService.Produto.ListarPorUnidadeNegocio(unidadeNegocioId);

            return lstRetorno;
        }

        public List<Product> ListarProdutosPorCodigo(List<String> lstProductNumber, bool apenasAtivos = true, params string[] columns)
        {
            return RepositoryService.Produto.ListarPor(lstProductNumber, apenasAtivos, columns);
        }

        #region PoliticaComercial

        public List<PrecoProduto> ListarPor(List<PrecoProduto> lstObj)
        {
            Conta conta = RepositoryService.Conta.Retrieve(lstObj[0].ContaId);
            Classificacao classificacao = null;
            if (conta.Classificacao != null)
                classificacao = RepositoryService.Classificacao.ObterPor(conta.Classificacao.Id);

            #region Listas Cache


            List<PoliticaComercial> lstPoliticaComercial = RepositoryService.PoliticaComercial.ListarPoliticasComClassificacao();

            List<FamiliaPoliticaComercial> lstFamiliaPoliticaComercial = RepositoryService.FamiliaPoliticaComercial.ListarTodas();

            List<PoliticaComercial> lstPoliticaComercialXEstado = new List<PoliticaComercial>();

            if (!String.IsNullOrEmpty(conta.Endereco1Estadoid.Id.ToString()))
                lstPoliticaComercialXEstado = RepositoryService.PoliticaComercial.ListarPorEstado(new Guid(conta.Endereco1Estadoid.Id.ToString()));


            List<PoliticaComercial> lstPoliticaComEspecifica = RepositoryService.PoliticaComercial.ListarPorCanal(conta.ID.Value);


            List<Estabelecimento> lstEstabelecimentos = RepositoryService.Estabelecimento.ListarTodos();


            #endregion
            List<PrecoProduto> lstRetornoPreco = new List<PrecoProduto>();

            foreach (var itemPrecoProduto in lstObj)
            {

                double fatorPolitica = 0;
                double fatorProdPolitica = 0;
                decimal precoBase = 0;
                double fatorFamilia = 0;
                DateTime DataValidade = DateTime.Now;
                Product prod = null;

                if (itemPrecoProduto.Produto != null)
                    prod = itemPrecoProduto.Produto;//RepositoryService.Produto.ObterPor(item.ProdutoId);
                else
                    prod = RepositoryService.Produto.Retrieve(itemPrecoProduto.ProdutoId);

                if (prod == null)
                {
                    itemPrecoProduto.ValorProduto = 0;
                    itemPrecoProduto.ValorComDesconto = 0;
                    itemPrecoProduto.PercentualDesconto = 0;
                    itemPrecoProduto.PercentualPossivelDesconto = 0;
                    itemPrecoProduto.DataValidade = DateTime.Now;
                    itemPrecoProduto.Retorno = "O Produto: " + itemPrecoProduto.CodigoProduto + " - não cadastrado no Crm.";
                    lstRetornoPreco.Add(itemPrecoProduto);
                    continue;
                }

                if (prod.Moeda != null)
                {
                    itemPrecoProduto.Moeda = prod.Moeda.Name;
                }

                itemPrecoProduto.CodigoProduto = prod.Codigo;

                if (prod.CustoAtual == null ||
                    !prod.CustoAtual.HasValue ||
                    prod.CustoAtual.Value == 0)
                {
                    itemPrecoProduto.ValorProduto = 0;
                    itemPrecoProduto.ValorComDesconto = 0;
                    itemPrecoProduto.PercentualDesconto = 0;
                    itemPrecoProduto.PercentualPossivelDesconto = 0;
                    itemPrecoProduto.DataValidade = DateTime.Now;
                    itemPrecoProduto.Retorno = "O Produto - " + itemPrecoProduto.ProdutoId.ToString() + " - não foi precificado.";
                    lstRetornoPreco.Add(itemPrecoProduto);
                    continue;
                }
                precoBase = prod.CustoAtual.Value;


                if (conta == null || conta.Classificacao == null)
                {
                    itemPrecoProduto.ValorProduto = 0;
                    itemPrecoProduto.ValorComDesconto = 0;
                    itemPrecoProduto.PercentualDesconto = 0;
                    itemPrecoProduto.PercentualPossivelDesconto = 0;
                    itemPrecoProduto.DataValidade = DateTime.Now;
                    itemPrecoProduto.Retorno = "O Canal - " + itemPrecoProduto.ContaId.ToString() + " - não encontrada no CRM.";
                    lstRetornoPreco.Add(itemPrecoProduto);
                    continue;
                }

                if (prod.UnidadeNegocio == null)
                {
                    itemPrecoProduto.ValorProduto = 0;
                    itemPrecoProduto.ValorComDesconto = 0;
                    itemPrecoProduto.PercentualDesconto = 0;
                    itemPrecoProduto.PercentualPossivelDesconto = 0;
                    itemPrecoProduto.DataValidade = DateTime.Now;
                    itemPrecoProduto.Retorno = "Este Canal não atua com a Unidade de Negócio deste Produto.";
                    lstRetornoPreco.Add(itemPrecoProduto);
                    continue;
                }

                Estabelecimento Estabelecimento = lstEstabelecimentos.Where(x => x.Codigo.Value == itemPrecoProduto.codEstabelecimento).FirstOrDefault<Estabelecimento>();

                if (Estabelecimento == null)
                {
                    itemPrecoProduto.ValorProduto = 0;
                    itemPrecoProduto.ValorComDesconto = 0;
                    itemPrecoProduto.PercentualDesconto = 0;
                    itemPrecoProduto.PercentualPossivelDesconto = 0;
                    itemPrecoProduto.DataValidade = DateTime.Now;
                    itemPrecoProduto.Retorno = "Estabelecimento do produto não encontrado.";
                    lstRetornoPreco.Add(itemPrecoProduto);
                    continue;
                }

                ConfiguracaoBeneficio configBenef = RepositoryService.ConfiguracaoBeneficio.ObterPorProduto(prod.ID);

                if (configBenef != null)
                {
                    itemPrecoProduto.CalcularRebate = configBenef.CalcularRebate;
                    itemPrecoProduto.RebateAntecipado = configBenef.AnteciparRebate;
                    itemPrecoProduto.PercentualRebateAntecipado = configBenef.PercRebateAntecipado / 100;
                }
                else
                {
                    itemPrecoProduto.CalcularRebate = true;
                    itemPrecoProduto.RebateAntecipado = false;
                    itemPrecoProduto.PercentualRebateAntecipado = 0;
                }

                CanalVerde canalVerdeExist = RepositoryService.CanalVerde.ObterParaCalculo((Guid)conta.ID, prod.FamiliaProduto.Id);

                if (canalVerdeExist != null)
                {
                    itemPrecoProduto.PercentualPossivelDesconto = 0;
                    FamiliaProduto famProdObj = RepositoryService.FamiliaProduto.ObterPor(prod.FamiliaProduto.Id);
                    if (famProdObj != null && famProdObj.DescontoVerdeHabilitado)
                    {
                        itemPrecoProduto.PercentualDesconto = famProdObj.PercentualDescontoVerde;
                    }
                    else
                    {
                        itemPrecoProduto.PercentualDesconto = 0;
                    }
                }
                else
                {
                    itemPrecoProduto.PercentualDesconto = 0;
                    FamiliaProduto famProdObj = RepositoryService.FamiliaProduto.ObterPor(prod.FamiliaProduto.Id);
                    if (famProdObj != null && famProdObj.DescontoVerdeHabilitado)
                    {
                        itemPrecoProduto.PercentualPossivelDesconto = famProdObj.PercentualDescontoVerde;
                    }
                    else
                    {
                        itemPrecoProduto.PercentualPossivelDesconto = 0;
                    }
                }

                //TODO - Especificação diferente ITBES0012 
                itemPrecoProduto.PrecoAlterado = false;

                PoliticaComercial politicaComercial = new PoliticaComercial(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                if (itemPrecoProduto.TipoCrossSelling)
                {


                    //Estado -Cache
                    politicaComercial = lstPoliticaComercialXEstado.Where(x => x.Estabelecimento.Id == Estabelecimento.ID.Value)
                                                                                                .Where(x => x.TipoDePolitica.Value == (int)Domain.Enum.PoliticaComercial.TipoDePolitica.CrossSelling)
                                                                                                .Where(x => x.Classificacao.Id == classificacao.ID.Value)
                                                                                                .Where(x => x.UnidadeNegocio.Id == prod.UnidadeNegocio.Id).FirstOrDefault<PoliticaComercial>();

                    if (politicaComercial == null)
                    {
                        politicaComercial = lstPoliticaComercial.Where(x => x.TipoDePolitica.Value == (int)Domain.Enum.PoliticaComercial.TipoDePolitica.CrossSelling)
                                                                              .Where(x => x.Estabelecimento.Id == Estabelecimento.ID.Value)
                                                                              .Where(x => x.Classificacao != null && x.Classificacao.Id == classificacao.ID.Value)
                                                                              .Where(x => x.UnidadeNegocio.Id == prod.UnidadeNegocio.Id).FirstOrDefault<PoliticaComercial>();
                    }
                }
                else //Caso não se CrossSeling
                {
                    //Especifica -Cache
                    politicaComercial = lstPoliticaComEspecifica.Where(x => x.Estabelecimento.Id == Estabelecimento.ID.Value)
                                                                                        .Where(x => x.UnidadeNegocio.Id == prod.UnidadeNegocio.Id).FirstOrDefault<PoliticaComercial>();

                    if (politicaComercial == null)
                    {
                        //Estado -Cache
                        politicaComercial = lstPoliticaComercialXEstado.Where(x => x.Estabelecimento.Id == Estabelecimento.ID.Value)
                                                                                                    .Where(x => x.TipoDePolitica.Value == (int)Domain.Enum.PoliticaComercial.TipoDePolitica.Normal)
                                                                                                    .Where(x => x.Classificacao.Id == classificacao.ID.Value)
                                                                                                    .Where(x => x.UnidadeNegocio.Id == prod.UnidadeNegocio.Id).FirstOrDefault<PoliticaComercial>();
                    }

                    if (politicaComercial == null)
                    {
                        politicaComercial = lstPoliticaComercial.Where(x => x.TipoDePolitica.Value == (int)Domain.Enum.PoliticaComercial.TipoDePolitica.Normal)
                                                                              .Where(x => x.Estabelecimento.Id == Estabelecimento.ID.Value)
                                                                              .Where(x => x.Classificacao != null && x.Classificacao.Id == classificacao.ID.Value)
                                                                              .Where(x => x.UnidadeNegocio.Id == prod.UnidadeNegocio.Id).FirstOrDefault<PoliticaComercial>();
                    }
                }




                ProdutoPoliticaComercial prodPoliticaComercial = null;
                if (politicaComercial != null)
                {
                    prodPoliticaComercial = RepositoryService.ProdutoPoliticaComercial.ObterPor(politicaComercial.ID.Value, prod.ID.Value, itemPrecoProduto.Quantidade);

                    if (prodPoliticaComercial != null)
                    {

                        if (prodPoliticaComercial.Fator.HasValue)
                        {
                            fatorProdPolitica = prodPoliticaComercial.Fator.Value;
                            DataValidade = prodPoliticaComercial.DataFimVigencia.Value;
                            itemPrecoProduto.QuantidadeFinal = prodPoliticaComercial.QtdFinal;
                        }
                    }
                    else
                    {
                        FamiliaPoliticaComercial familiaPoliticaComercial = lstFamiliaPoliticaComercial
                                                                            .Where(x => x.PoliticaComercial.Id == politicaComercial.ID.Value)
                                                                            .Where(x => x.FamiliaComercialInicial.Codigo.CompareTo(itemPrecoProduto.codFamiliaComl) <= 0)
                                                                            .Where(x => x.FamiliaComercialFinal.Codigo.CompareTo(itemPrecoProduto.codFamiliaComl) >= 0)
                                                                            .Where(x => x.QtdInicial <= itemPrecoProduto.Quantidade)
                                                                            .Where(x => x.QtdFinal >= itemPrecoProduto.Quantidade).FirstOrDefault<FamiliaPoliticaComercial>();

                        if (familiaPoliticaComercial != null)
                        {
                            fatorFamilia = (double)familiaPoliticaComercial.Fator;
                            itemPrecoProduto.NomePoliticaComercial = politicaComercial.Nome;
                            DataValidade = familiaPoliticaComercial.DataFinal;
                            itemPrecoProduto.QuantidadeFinal = familiaPoliticaComercial.QtdFinal;
                        }
                        else if (politicaComercial.Fator.HasValue)
                        {
                            //Setado valor default em 999999999 para caso for a política da capa.
                            itemPrecoProduto.QuantidadeFinal = 999999999;
                            fatorPolitica = politicaComercial.Fator.Value;
                            if (politicaComercial.DataFim.HasValue)
                                DataValidade = politicaComercial.DataFim.Value;
                        }
                        else
                        {
                            itemPrecoProduto.ValorProduto = 0;
                            itemPrecoProduto.ValorComDesconto = 0;
                            itemPrecoProduto.PercentualDesconto = 0;
                            itemPrecoProduto.Retorno = "Valor do fator da politica comercial não preenchido.";
                            lstRetornoPreco.Add(itemPrecoProduto);
                            continue;
                        }
                    }

                    itemPrecoProduto.NomePoliticaComercial = politicaComercial.Nome;
                    itemPrecoProduto.PoliticaComercialID = politicaComercial.ID.Value;


                }

                itemPrecoProduto.CustoProduto = prod.CustoAtual.Value;
                itemPrecoProduto.DataValidade = DataValidade;

                if (fatorProdPolitica > 0)
                {
                    precoBase = precoBase * (decimal)fatorProdPolitica;
                    itemPrecoProduto.ValorProduto = precoBase;
                }
                else if (fatorFamilia > 0)
                {
                    precoBase = precoBase * (decimal)fatorFamilia;
                    itemPrecoProduto.ValorProduto = precoBase;
                }
                else if (fatorPolitica > 0)
                {
                    precoBase = precoBase * (decimal)fatorPolitica;
                    itemPrecoProduto.ValorProduto = precoBase;
                }
                else
                {
                    if (itemPrecoProduto.TipoCrossSelling)
                        itemPrecoProduto.ValorProduto = precoBase;
                    itemPrecoProduto.NomePoliticaComercial = "Sem Política Comercial";
                }

                itemPrecoProduto.ValorBase = precoBase;

                decimal valorComDesconto = itemPrecoProduto.ValorProduto;
                if (configBenef != null && configBenef.AnteciparRebate == true)
                {
                    valorComDesconto = itemPrecoProduto.ValorProduto - (itemPrecoProduto.ValorProduto * ((decimal)configBenef.PercRebateAntecipado / 100));
                }

                if (itemPrecoProduto.PercentualDesconto > 0)
                {
                    valorComDesconto = (decimal)(valorComDesconto - (valorComDesconto * itemPrecoProduto.PercentualDesconto));
                }


                itemPrecoProduto.ValorComDesconto = valorComDesconto;

                lstRetornoPreco.Add(itemPrecoProduto);
            }

            return lstRetornoPreco;
        }

        public PrecoProduto ObterPrecoProduto(Conta conta, Product produto)
        {
            var lstPrecoProduto = new List<PrecoProduto>();
            lstPrecoProduto.Add(new PrecoProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
            {
                //codEstabelecimento
                //codFamiliaComl
                CodigoProduto = produto.Codigo,
                //codUnidade
                ContaId = conta.ID.Value,
                Produto = produto,


            });

            return null;
        }

        #endregion

        #region ProdutoPorfolioCompleto
        //Produto portfolio funcionando completamente , a versao que esta nessa classe foi alterada conforme o Kledir pediu, estou guardando esta para ter referencia no rollout4(que sera utilizado)

        //public List<ProdutoPortfolio> ProdutosPortfolio(Guid canalId, Enum.ListaProdutos.Tipo? tipoPesquisa)
        //{
        //    //List<FiltroProduto> lstFiltroProdutos = new List<FiltroProduto>();
        //    List<ProdutoPortfolio> lstProdutosPortfolio = new List<ProdutoPortfolio>();
        //    List<Portfolio> lstPortfolio = new List<Portfolio>();
        //    List<int> lstTipoCanal = new List<int>();

        //    #region Obtém o Canal, fazendo uma consulta na entidade de Contas [Account] pelo ID do canal passado por parâmetro de acordo com o passo anterior.

        //    Conta canal = RepositoryService.Conta.ObterPor(canalId);

        //    #endregion

        //    #region Consulta a entidade “Categoria do Canal” [itbc_categoriasdocanal] onde o campo Canal [itbc_canalid] é igual o canal obtido no passo 2,  obtém a Classificação [itbc_classificacaoid] e as Unidades de Negócio [itbc_businessunit] do Canal nos registros encontrados

        //    List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canalId, null, null, null, null);

        //    List<Guid> lstUN = new List<Guid>();

        //    foreach (CategoriasCanal catCanal in lstCatCanal)
        //    {
        //        if (!lstUN.Contains(catCanal.UnidadeNegocios.Id))
        //            lstUN.Add(catCanal.UnidadeNegocios.Id);
        //    }

        //    #endregion

        //    #region Obtém todos os Produtos do Portfolio cujo Portfolio relacionado tenha Tipo igual a Box Mover e a Unidade de Negócio igual as do Canal

        //    lstTipoCanal.Add((int)Enum.Portfolio.Tipo.BoxMover);

        //    #endregion

        //    #region Caso o Canal seja exclusivo (se o atributo itbc_exclusividade for igual a “Sim”), obtém todos os Produtos do Portfolio cujo Portfolio relacionado tenho o campo Tipo igual a Exclusivo e a Unidade de Negócio é igual as do Canal.

        //    if (canal.Exclusividade == true)
        //    {
        //        lstTipoCanal.Add((int)Enum.Portfolio.Tipo.Exclusivo);
        //    }

        //    #endregion

        //    #region Caso a Classificação do Canal seja Relacional ou VAD, Obtém todos os Produtos do Portfolio cujo Portfolio relacionado tenha o campo Tipo igual a VAD e a Unidade de Negócio é igual as do Canal.

        //    if (canal.Classificacao.Name == Enum.Conta.Classificacao.Dist_VAD || canal.Classificacao.Name == Enum.Conta.Classificacao.Rev_Rel)
        //    {
        //        lstTipoCanal.Add((int)Enum.Portfolio.Tipo.VAD);
        //    }

        //    #endregion

        //    #region Une as três lista acima em uma lista de produtos do portfólio.

        //    lstPortfolio = RepositoryService.Portfolio.ListarPor(lstUN, lstTipoCanal);

        //    foreach (Portfolio portfolio in lstPortfolio)
        //    {
        //        lstProdutosPortfolio.AddRange(RepositoryService.ProdutoPortfolio.ListarPor((Guid)portfolio.ID));
        //    }

        //    #endregion

        //    #region Para cada Produto obtido na lista acima, verifica quais possuem atributo “Requere certificação” igual a “Sim”.

        //    foreach (ProdutoPortfolio prodPort in lstProdutosPortfolio.Where(x => x.ExigeTreinamento == true).ToList())
        //    {
        //        //Para cada um que requere certificação, localiza a lista de Treinamentos/Certificações requeridas na entidade Produtos x Treinamento/Certificação. 
        //        List<ProdutoTreinamento> lstProdTreinamento = RepositoryService.ProdutoTreinamento.ListarPorProduto(prodPort.Produto.Id);
        //        foreach (ProdutoTreinamento prodTreinamento in lstProdTreinamento)
        //        {
        //            List<TreinamentoCanal> lstTreinamentoCanal = RepositoryService.TreinamentoCanal.ListarPor((Guid)prodTreinamento.Treinamento.Id, (Guid)canalId);

        //            //Para cada Treinamento encontrado, verifica quais estão com Status igual a “Não cumprido” na entidade Treinamentos do Canal (de mesmo Canal e Treinamento obtidos).
        //            if (lstTreinamentoCanal.Count(x => x.StatusCompromisso.Name == Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido) > 0)
        //            {
        //                //Remove da lista de produtos do portfólio do passo 8, todos os produtos encontrados sem treinamento no passo 9 (remove da lista, não da base do CRM).
        //                lstProdutosPortfolio.Remove(prodPort);
        //            }
        //        }
        //    }

        //    #endregion

        //    if (tipoPesquisa != null)
        //    {
        //        switch (tipoPesquisa)
        //        {
        //            case Enum.ListaProdutos.Tipo.PMA:

        //                //Consulta no Canal [Account] qual o UF [itbc_address1_stateorprovince] e Obtém a Lista de Preço [pricelevel] relacionada com o Estado do Canal através da tabela de relacionamento N pra N (itbc_pricelevel_itbc_estado).
        //                //Obtém todos os Produtos[product] da Lista de Preço encontrada no passo 4, através de uma consulta na entidade de Item da Lista de Preço [productpricelevel] cuja Lista de Preço relacionada seja do tipo “Lista PMA”.

        //                List<ListaPreco> lstListaPreco = RepositoryService.ListaPreco.ListarPor(Enum.ListaPreco.Tipo.PMA, canal.Endereco1Estadoid.Id);

        //                if (lstListaPreco.Count > 0)
        //                {
        //                    foreach (ProdutoPortfolio prodPort in lstProdutosPortfolio.ToList())
        //                    {
        //                        if (RepositoryService.ItemListaPreco.ListarPor((Guid)lstListaPreco[0].ID, prodPort.Produto.Id).Count() < 1)
        //                        {
        //                            lstProdutosPortfolio.Remove(prodPort);
        //                            continue;
        //                        }

        //                        prodPort.CodigoProduto = RepositoryService.Produto.ObterPor(prodPort.Produto.Id).Codigo;

        //                    }
        //                }
        //                else
        //                {
        //                    lstProdutosPortfolio.Clear();
        //                }

        //                break;

        //            case Enum.ListaProdutos.Tipo.Showroom:

        //                foreach (ProdutoPortfolio prodPort in lstProdutosPortfolio.ToList())
        //                {
        //                    Product produto = RepositoryService.Produto.ObterPor(prodPort.Produto.Id);

        //                    if (produto.Showroom != true)
        //                    {
        //                        lstProdutosPortfolio.Remove(prodPort);
        //                        continue;
        //                    }

        //                    prodPort.CodigoProduto = produto.Codigo;
        //                }
        //                break;

        //        }
        //    }

        //    return lstProdutosPortfolio;

        //}
        #endregion

        #region Service para Carga de Produtos do Sell Out
        public List<Model.Product> ListarTodosProdutos(ref int pagina, int contagem, out bool moreRecords)
        {
            return RepositoryService.Produto.ListarTodosProdutos(ref pagina, contagem, out moreRecords);
        }

        public List<Model.Product> ListarTodosProdutos()
        {
            return RepositoryService.Produto.ListarTodosProdutos();
        }

        public void EnviarValorPSD()
        {
            RepositoryService.Produto.EnviarValorPSD();
        }

        #endregion

        public string IntegracaoBarramento(Product objProduto)
        {
            Domain.Integracao.MSG0088 msgProduto = new Domain.Integracao.MSG0088(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgProduto.Enviar(objProduto);
        }

        public void ProdutoAcumulaOutroProduto(ref List<Product> lstProdutos)
        {
            List<Product> lstProdutosPage = new List<Product>();
            lstProdutosPage.AddRange(lstProdutos.ToArray());

            foreach (Model.Product item in lstProdutosPage)
            {
                if (item.AcumulaOutroProduto.HasValue && item.AcumulaOutroProduto.Value)
                {
                    Product produtosubstituto = RepositoryService.Produto.ProdutoSubstituto(item.ID.Value);
                    if (produtosubstituto != null)
                    {
                        lstProdutos.Remove(item);
                        lstProdutos.Add(produtosubstituto);
                    }
                }
            }
        }

        public void DataAlteracaoPVC(Product mProduto, Product mProdutoOld)
        {
            decimal vlrCustoOld = 0;
            decimal vlrCustoNew = 0;

            if (!mProduto.DataUltAlteracaoPVC.HasValue)
            {
                if (mProduto.CustoAtual.HasValue)
                    vlrCustoNew = mProduto.CustoAtual.Value;

                if (mProdutoOld.CustoAtual.HasValue)
                    vlrCustoOld = mProdutoOld.CustoAtual.Value;

                if (vlrCustoNew != vlrCustoOld)
                {
                    mProduto.DataUltAlteracaoPVC = DateTime.Now.Date.AddHours(3);
                    RepositoryService.Produto.Update(mProduto);
                }

            }
        }

        public void ValidarQuantidadeMultipla(Product produto, decimal quantidadeMultipla)
        {
            if (!produto.QuantidadeMultiplaProduto.HasValue)
            {
                throw new ArgumentException("(CRM) Quantidade Multipla não cadastrada para o produto: " + produto.Codigo);
            }

            bool quantidadeMultiplaCorreta = (quantidadeMultipla % produto.QuantidadeMultiplaProduto.Value) == 0;

            if (!quantidadeMultiplaCorreta)
            {
                throw new ArgumentException(string.Format("(CRM) Quantidade incorreta para o produto: {0}, a quantidade solicitada deverá ser um múltiplo de: {1}",
                                                produto.Codigo,
                                                produto.QuantidadeMultiplaProduto.Value));
            }
        }


        public void GravaLogPSD(string Idprodutoerp, int qtderro, string nomeArquivo, int linha, int linha2)
        {
            string erros = "";

            var excel = new ClosedXML.Excel.XLWorkbook();
            var ws = excel.Worksheets.Add("MaiorValorPSD");

            ws.Cell(linha, 1).Value = "Produto";
            ws.Cell(linha, 2).Value = "Sucesso";
            ws.Cell(linha, 3).Value = "Erro";

            string data = DateTime.Now.ToString();


            //Excel - Coluna Conta
            ws.Cell(linha2, 1).Value = Idprodutoerp;
            try
            {
                //Excel - Coluna Sucesso
                ws.Cell(linha2, 2).Value = "Sim";
            }
            catch (Exception ex)
            {
                SDKore.Helper.Error.Create("Problemas ao enviar maior valor PSD " + Idprodutoerp + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                erros += "<b> - </b> Problemas ao enviar maior valor PSD, produto [" + Idprodutoerp + "] <b> Erro: </b>" + ex.Message + "<br />";
                qtderro++;
                //Excel - Coluna Erro
                ws.Cell(linha2, 3).Value = ex.Message;
            }
            excel.SaveAs("c:\\temp\\" + nomeArquivo);
        }
        
        public void EnviaEmailRegistroMaiorValorPSD(int qtdtotal, int qtdcerto, int qtderro, string nomeArquivo)
        {

            #region Recupera valor do parâmetro global para buscar o grupo do email.
            var parametroGlobalEmail = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.EmailGrupoIC);
            #endregion

            string textoEmail = "<b> Início da rotina: </b>" + DateTime.Now + " <br />";
            textoEmail += "<b> Linhas processadas: </b>" + qtdtotal + "<br />";
            textoEmail += "<b> Linhas corretas: </b>" + qtdcerto + "<br />";
            textoEmail += "<b> Linhas com erros: </b>" + qtderro + "<br />";
            textoEmail += "<b> Rotina finalizada: </b>" + DateTime.Now + " <br />";

            RepositoryService.Email.EnviaEmailComLogdeRotinas(textoEmail, "Rotina: Enviar maior valor PSD", nomeArquivo, Convert.ToString(parametroGlobalEmail.Valor));
        }        
    }
}


