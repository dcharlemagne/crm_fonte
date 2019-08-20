using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PortfolioService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PortfolioService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public PortfolioService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public Portfolio Persistir(Portfolio portfolio)
        {
            Portfolio tmpPortfolio = RepositoryService.Portfolio.ObterPor(portfolio.ID.Value);

            if (tmpPortfolio != null)
            {
                portfolio.ID = tmpPortfolio.ID;
                RepositoryService.Portfolio.Update(portfolio);
                return tmpPortfolio;
            }
            else
                portfolio.ID = RepositoryService.Portfolio.Create(portfolio);
            return portfolio;
        }


        public Portfolio ObterPor(Guid portfolioId)
        {
            return RepositoryService.Portfolio.ObterPor(portfolioId);
        }

        public void VerificaDuplicidadeClassificadores(Portfolio Portfolio)
        {

            if (Portfolio.Tipo.Value == (int)Domain.Enum.Portfolio.Tipo.Solucao)
            {
                //Verifica se a classificacao tem acesso a solucoes
                if (!RepositoryService.Classificacao.ObterPor(Portfolio.Classificacao.Id).AcessaSolucoes)
                {
                    throw new ArgumentException("(CRM) Classificação do Porfolio não possui acesso a soluções");
                }             
            }
            else  if (Portfolio.Tipo.Value == (int)Domain.Enum.Portfolio.Tipo.CrossSelling)
            {
                //Verifica se a classificacao tem acesso a cross selling
                if (!RepositoryService.Classificacao.ObterPor(Portfolio.Classificacao.Id).AcessaCrossSelling)
                {
                    throw new ArgumentException("(CRM) Classificação do Porfolio não possui acesso a Cross Selling");
                }
            }

            else
            {
                if (RepositoryService.Portfolio.ListarPor(Portfolio.UnidadeNegocio.Id, Portfolio.Tipo.Value, Portfolio.Classificacao.Id, Portfolio.ID.Value).Count > 0)
                    throw new ArgumentException("(CRM) Não é possível realizar a operação: Já existe um portfólio com as mesmas características. Verificar os registros existentes.");
            }

        }

        public void VerificaVinculoProdutoVsProdutoPortifolio(ProdutoPortfolio ProdPortfolio)
        {
            if (ProdPortfolio.Produto == null ||
                ProdPortfolio.Portfolio == null)
                throw new ArgumentException("(CRM) O Produto e o Portifolio devem ser informados.");

            Portfolio portfolio = RepositoryService.Portfolio.Retrieve(ProdPortfolio.Portfolio.Id);
            Product produto = RepositoryService.Produto.Retrieve(ProdPortfolio.Produto.Id);

            if (portfolio.Tipo == (int)Domain.Enum.Portfolio.Tipo.Solucao)
            {
                if (portfolio.UnidadeNegocio.Id == produto.UnidadeNegocio.Id)
                    throw new ArgumentException("(CRM) Para portfólios do tipo Solução a únidade de negócios do produto não pode ser igual a do portfólio.");
            }
            else
            {
                if (portfolio.UnidadeNegocio.Id != produto.UnidadeNegocio.Id)
                    throw new ArgumentException("(CRM) O Produto e o Portfolio não estão na mesma Unidade de Negócio.");
            }
        }
        public void VerificaProdutoDuplicado(ProdutoPortfolio ProdPortfolio)
        {
            Product produto = RepositoryService.Produto.Retrieve(ProdPortfolio.Produto.Id);

            var produtos = RepositoryService.ProdutoPortfolio.ListarPor(ProdPortfolio.Portfolio.Id, null);
            foreach (var p in produtos)
            {
                if (p.Produto.Id == produto.ID)
                {
                    throw new ArgumentException("(CRM) O produto " + produto.Nome + " ja existe neste portifolio. Favor selecionar outro produto.");
                }
            }
        }

        public void VerificaCrossSellingSolucao(ProdutoPortfolio ProdPortfolio)
        {
            List<Portfolio> lPortfolio = new List<Portfolio>();
            bool existe = false;
            var portfolio = RepositoryService.Portfolio.Retrieve(ProdPortfolio.Portfolio.Id);

            if (portfolio.Tipo == (int)Domain.Enum.Portfolio.Tipo.CrossSelling || portfolio.Tipo == (int)Domain.Enum.Portfolio.Tipo.Exclusivo)
            {
                lPortfolio = RepositoryService.Portfolio.ListarPor(portfolio.UnidadeNegocio.Id, (int)Domain.Enum.Portfolio.Tipo.Normal, portfolio.Classificacao.Id);

                if (lPortfolio.Count > 0)
                {
                    var produtos = RepositoryService.ProdutoPortfolio.ListarPor(lPortfolio.FirstOrDefault().ID.Value, null);

                    foreach (var p in produtos)
                    {
                        if (p.Produto.Id == ProdPortfolio.Produto.Id)
                        {
                            existe = true;
                            break;
                        }
                    }
                }

                if (!existe)
                {
                    throw new ArgumentException("(CRM) Para inserir esse produto ele precisa fazer parte de um portfólio do tipo Normal da mesma classificação e unidade de negócio.");
                }
            }
            else if (portfolio.Tipo == (int)Domain.Enum.Portfolio.Tipo.Solucao)
            {
                lPortfolio = RepositoryService.Portfolio.ListarPorProduto(ProdPortfolio.Produto.Id);

                if (lPortfolio.Count > 0)
                {
                    foreach (var port in lPortfolio)
                    {
                        if (port.UnidadeNegocio.Id != portfolio.UnidadeNegocio.Id)
                        {
                            var portAux = RepositoryService.Portfolio.ListarPor(port.UnidadeNegocio.Id, (int)Domain.Enum.Portfolio.Tipo.Normal, port.Classificacao.Id);

                            if (portAux.Count > 0)
                            {
                                var produtos = RepositoryService.ProdutoPortfolio.ListarPor(portAux.FirstOrDefault().ID.Value, null);

                                foreach (var p in produtos)
                                {
                                    if (p.Produto.Id == ProdPortfolio.Produto.Id)
                                    {
                                        existe = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!existe)
                {
                    throw new ArgumentException("(CRM) Para inserir esse produto ele precisa fazer parte de um portfólio do tipo Normal da mesma classificação e diferente unidade de negócio.");
                }
            }            
        }
        public void VerificaVinculoPortifolio(ProdutoPortfolio ProdPortfolio)
        {
            if (ProdPortfolio.StateCode.HasValue)
            {
                if (ProdPortfolio.StateCode.Value == (int)Domain.Enum.ProdutoPortfolio.StateCode.Inativo)
                {
                    return;
                }
            }

            if (ProdPortfolio.Produto == null || ProdPortfolio.Portfolio == null)
            {
                throw new ArgumentException("(CRM) O Produto e o Portifolio devem ser informados.");
            }

            Portfolio Portfolio = RepositoryService.Portfolio.Retrieve(ProdPortfolio.Portfolio.Id);

            if (!Portfolio.Tipo.HasValue)
            {
                throw new ArgumentException("(CRM) Tipo do portfólio é obrigatório");
            }

            if (Portfolio.Tipo == (int)Enum.Portfolio.Tipo.Solucao)
            {
                if (Portfolio.Classificacao == null)
                    throw new ArgumentException("(CRM) Classificação do portfólio é obrigatório.");

                if (Portfolio.UnidadeNegocio == null)
                    throw new ArgumentException("(CRM) Unidade de negócio do portfólio é obrigatório.");
            }

            List<Portfolio> listaPortfolio = RepositoryService.Portfolio.ListarPorProduto(ProdPortfolio.Produto.Id);
            listaPortfolio.Remove(Portfolio);

            foreach (var item in listaPortfolio)
            {
                if (!item.Tipo.HasValue)
                    throw new ArgumentException("(CRM) Foi encontrado um registro inconsistente(Com o tipo de portfólio vazio).Contate o administrador do sistema.");

                if (item.Tipo.Value == (int)Enum.Portfolio.Tipo.Solucao)
                {
                    if (item.Classificacao == null)
                        throw new ArgumentException("(CRM) Foi encontrado um registro inconsistente(Com a classificação vazia).Contate o administrador do sistema.");

                    if (Portfolio.UnidadeNegocio == null)
                        throw new ArgumentException("(CRM) Foi encontrado um registro inconsistente(Com a unidade de negócio vazia).Contate o administrador do sistema.");
                }

                if (Portfolio.Tipo.Value == (int)Enum.Portfolio.Tipo.CrossSelling || Portfolio.Tipo.Value == (int)Enum.Portfolio.Tipo.Solucao)
                {
                    if (item.Tipo.Value == (int)Enum.Portfolio.Tipo.CrossSelling || item.Tipo.Value == (int)Enum.Portfolio.Tipo.Solucao)
                    {
                        if (Portfolio.UnidadeNegocio == null || item.UnidadeNegocio == null)
                            throw new ArgumentException("(CRM) Foi encontrado um registro inconsistente(Com a unidade de negócio vazio). Contate o administrador do sistema.");                 
                    }
                }                
            }
        }
        public void VerificaVinculoPortifolioCrossSelling(ProdutoPortfolio ProdPortfolio)
        {
            
            Portfolio Portfolio = RepositoryService.Portfolio.Retrieve(ProdPortfolio.Portfolio.Id);
            
            List<Portfolio> listaPortfolio = RepositoryService.Portfolio.ListarPorProduto(ProdPortfolio.Produto.Id);
            listaPortfolio.Remove(Portfolio);

            foreach (var item in listaPortfolio)
            {              
                if (item.Tipo.Value == (int)Enum.Portfolio.Tipo.CrossSelling)
                {   
                    throw new ArgumentException("(CRM) Para excluir esse produto primeiro você tem que exclui-lo do portifolio de crossseling.");
                }                
            }
        }
        public void VerificaVinculoPortifolioSolucao(ProdutoPortfolio ProdPortfolio)
        {

            Portfolio Portfolio = RepositoryService.Portfolio.Retrieve(ProdPortfolio.Portfolio.Id);

            List<Portfolio> listaPortfolio = RepositoryService.Portfolio.ListarPorProduto(ProdPortfolio.Produto.Id);
            listaPortfolio.Remove(Portfolio);

            foreach (var item in listaPortfolio)
            {
                if (item.Tipo.Value == (int)Enum.Portfolio.Tipo.Solucao)
                {
                    throw new ArgumentException("(CRM) Para excluir esse produto primeiro você tem que exclui-lo do portifolio de Solução.");
                }
            }
        }

        public List<Portfolio> VerificaPortfolio(Conta canal, Guid? classificacaoId, Guid? unidadeNegocioId)
        {
            Guid canalId = canal.ID.Value;
            List<Portfolio> lstPortfolio = new List<Portfolio>();
            List<int> lstTipoCanal = new List<int>();
            List<CategoriasCanal> lstCatCanal = RepositoryService.CategoriasCanal.ListarPor(canalId, unidadeNegocioId, classificacaoId, null, null);

            List<Guid> lstUN = new List<Guid>();

            foreach (CategoriasCanal catCanal in lstCatCanal)
            {
                if (!lstUN.Contains(catCanal.UnidadeNegocios.Id))
                    lstUN.Add(catCanal.UnidadeNegocios.Id);
            }
            
            lstTipoCanal.Add((int)Enum.Portfolio.Tipo.Normal);
            
            if (canal.Exclusividade == true)
            {
                lstTipoCanal.Add((int)Enum.Portfolio.Tipo.Exclusivo);
            }

            

            
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
            
            return lstPortfolio;
        }

        public void VerificaVinculoProdutoVsProdutoPortifolioAlteracao(ProdutoPortfolio ProdPortfolio, ProdutoPortfolio ProdPortfolioPre)
        {
            if (ProdPortfolio.StateCode.HasValue)
            {
                if (ProdPortfolio.StateCode.Value == (int)Domain.Enum.ProdutoPortfolio.StateCode.Inativo)
                {
                    return;
                }
            }

            if (ProdPortfolioPre.Produto == null)
                throw new ArgumentException("(CRM) Erro de dados: O campo Produto obrigatório não preenchido em Portifolio do produto");

            if (ProdPortfolioPre.Portfolio == null)
                throw new ArgumentException("(CRM) Erro de dados: O campo Portfolio obrigatório não preenchido em Portifolio do produto");

            if (ProdPortfolio.Produto == null)
                ProdPortfolio.Produto = new Lookup(ProdPortfolioPre.Produto.Id, ProdPortfolioPre.Produto.Type);

            if (ProdPortfolio.Portfolio == null)
                ProdPortfolio.Portfolio = new Lookup(ProdPortfolioPre.Portfolio.Id, ProdPortfolioPre.Portfolio.Type);

            Domain.Model.Portfolio portfolio = RepositoryService.Portfolio.Retrieve(ProdPortfolio.Portfolio.Id);
            Domain.Model.Product produto = RepositoryService.Produto.Retrieve(ProdPortfolio.Produto.Id);

            if (portfolio == null)
                throw new ArgumentException("(CRM) Portfólio não encontrado");

            if (produto == null)
                throw new ArgumentException("(CRM) Produto não encontrado");

            if (portfolio.Tipo == (int)Domain.Enum.Portfolio.Tipo.Solucao)
            {
                if (portfolio.UnidadeNegocio.Id == produto.UnidadeNegocio.Id)
                    throw new ArgumentException("(CRM) Para portfólios do tipo Solução a únidade de negócios do produto não pode ser igual a do portfólio.");
            }
            else
            {
                if (portfolio.UnidadeNegocio.Id != produto.UnidadeNegocio.Id)
                    throw new ArgumentException("(CRM) O Produto e o Portfolio não estão na mesma Unidade de Negócio.");
            }
        }

        public void CriaTreinamentoeCertificacaoDoCanal(ProdutoTreinamento ProdPortifolio)
        {
            //procura em qual produtos do portifolio está
            if (ProdPortifolio.Produto != null && ProdPortifolio.NroMinimoProf.Value > 0)
            {
                List<ProdutoPortfolio> lstProdutoPorfolio = RepositoryService.ProdutoPortfolio.ListarPorProduto(ProdPortifolio.Produto.Id);

                foreach (ProdutoPortfolio _ProdutoPorfolio in lstProdutoPorfolio)
                {
                    Portfolio _Portfolio = RepositoryService.Portfolio.ObterPor(_ProdutoPorfolio.Portfolio.Id);

                    List<CategoriasCanal> lstCategoriasCanal = RepositoryService.CategoriasCanal.ListarPor(null, _Portfolio.UnidadeNegocio.Id, null, null, null);

                    foreach (CategoriasCanal _CategoriasCanal in lstCategoriasCanal)
                    {
                        TreinamentoCanal treinamentocanal = new TreinamentoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        treinamentocanal.Canal = new Lookup(_CategoriasCanal.Canal.Id, "account");
                        treinamentocanal.Treinamento = new Lookup(ProdPortifolio.Treinamento.Id, "");
                        treinamentocanal.Nome = _CategoriasCanal.Canal.Name + " - " + ProdPortifolio.Treinamento.Name;
                        treinamentocanal.DataLimite = DateTime.Now.AddMonths(1);

                        //consulta os profissionais certificados do canal.
                        List<ColaboradorTreinadoCertificado> lstColaboradorTreinadoCertificado = RepositoryService.ColaboradorTreinadoCertificado.ListarPor(_CategoriasCanal.Canal.Id, ProdPortifolio.Treinamento.Id);

                        if (lstColaboradorTreinadoCertificado.Count >= ProdPortifolio.NroMinimoProf.Value)
                            treinamentocanal.StatusCompromisso = new Lookup(RepositoryService.StatusCompromissos.ObterPor("Cumprido").ID.Value, "");
                        else
                            treinamentocanal.StatusCompromisso = new Lookup(RepositoryService.StatusCompromissos.ObterPor("Não Cumprido").ID.Value, "");

                        treinamentocanal.ID = RepositoryService.TreinamentoCanal.Create(treinamentocanal);

                    }
                }
            }
        }

        public void AtualizaTreinamentoeCertificacaoDoCanal(ProdutoTreinamento ProdPortifolio)
        {
            //procura em qual produtos do portifolio está
            if (ProdPortifolio.Produto != null && ProdPortifolio.NroMinimoProf.Value > 0)
            {
                List<ProdutoPortfolio> lstProdutoPorfolio = RepositoryService.ProdutoPortfolio.ListarPorProduto(ProdPortifolio.Produto.Id);

                foreach (ProdutoPortfolio _ProdutoPorfolio in lstProdutoPorfolio)
                {
                    if (_ProdutoPorfolio.Product.ExigeTreinamento.HasValue)
                    {
                        Portfolio _Portfolio = RepositoryService.Portfolio.ObterPor(_ProdutoPorfolio.Portfolio.Id);

                        List<CategoriasCanal> lstCategoriasCanal = RepositoryService.CategoriasCanal.ListarPor(null, _Portfolio.UnidadeNegocio.Id, null, null, null);

                        foreach (CategoriasCanal _CategoriasCanal in lstCategoriasCanal)
                        {
                            List<ColaboradorTreinadoCertificado> lstColaboradorTreinadoCertificadoDoCanal = RepositoryService.ColaboradorTreinadoCertificado.ListarPor(_CategoriasCanal.Canal.Id, ProdPortifolio.Treinamento.Id);

                            List<TreinamentoCanal> lstTreinamentoCanal = RepositoryService.TreinamentoCanal.ListarPor(ProdPortifolio.Treinamento.Id, _CategoriasCanal.Canal.Id);

                            foreach (TreinamentoCanal _treinamentocanal in lstTreinamentoCanal)
                            {
                                if (lstColaboradorTreinadoCertificadoDoCanal.Count >= ProdPortifolio.NroMinimoProf.Value)
                                    _treinamentocanal.StatusCompromisso = new Lookup(RepositoryService.StatusCompromissos.ObterPor("Cumprido").ID.Value, "");
                                else
                                    _treinamentocanal.StatusCompromisso = new Lookup(RepositoryService.StatusCompromissos.ObterPor("Não Cumprido").ID.Value, "");

                                RepositoryService.TreinamentoCanal.Update(_treinamentocanal);
                            }

                        }
                    }
                }
            }
        }

        public List<Portfolio> ListPortifolios(Guid unidadeNegocioId, Guid classificacaoId )
        {
            return RepositoryService.Portfolio.ListarPor(unidadeNegocioId, null, classificacaoId);
        }
    }
}
