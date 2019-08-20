using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasProdutoService
    {
        #region Atributos

        private static bool _isOffline = false;
        public static bool IsOffline
        {
            get { return _isOffline; }
            set { _isOffline = value; }
        }

        private static string _nomeDaOrganizacao = "";
        public static string NomeDaOrganizacao
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeDaOrganizacao))
                    _nomeDaOrganizacao = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

                return _nomeDaOrganizacao;
            }
            set { _nomeDaOrganizacao = value; }
        }

        public static object Provider { get; set; }

        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public HistoricoComprasProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public HistoricoComprasProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos


        public void RetornoDWHistoricoCompraProduto(int ano, int trimestre)
        {
            DataTable dtHistoricoCompraProduto = RepositoryService.HistoricoComprasProduto.ListarPor(ano.ToString(), trimestre.ToString());

            foreach (DataRow item in dtHistoricoCompraProduto.Rows)
            {
                int productNumber = 0;
                if (!Int32.TryParse(item["CD_item"].ToString(), out productNumber))
                    continue;

                Product produto = RepositoryService.Produto.ObterPor(item["CD_item"].ToString());

                if (produto == null)
                    continue;

                HistoricoDeComprasPorProduto historicoCompraProduto = RepositoryService.HistoricoComprasProduto.ObterPor(trimestre, ano, produto.ID.Value);

                if (historicoCompraProduto != null)
                {
                    historicoCompraProduto.Valor = decimal.Parse(item["Valor"].ToString());
                    historicoCompraProduto.quantidade = decimal.Parse(item["Qtd"].ToString());

                    if (produto.UnidadeNegocio != null && produto.SubfamiliaProduto != null && produto.Segmento != null && produto.FamiliaProduto != null)
                    {
                        HistoricoComprasSubfamilia hSubFamilia = RepositoryService.HistoricoComprasSubFamilia
                            .ObterPor(produto.UnidadeNegocio.Id, produto.SubfamiliaProduto.Id, produto.Segmento.Id, produto.FamiliaProduto.Id, ano, trimestre);

                        if (hSubFamilia != null)
                            historicoCompraProduto.SubFamilia = new Lookup(hSubFamilia.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasProduto.Update(historicoCompraProduto);
                }
                else
                {
                    HistoricoDeComprasPorProduto hsCompProduto = new HistoricoDeComprasPorProduto(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);
                    hsCompProduto.produto = new Lookup(produto.ID.Value, "");
                    hsCompProduto.quantidade = decimal.Parse(item["Qtd"].ToString());
                    hsCompProduto.Valor = decimal.Parse(item["Valor"].ToString());
                    hsCompProduto.ano = ano;
                    hsCompProduto.trimestre = trimestre;
                    hsCompProduto.Nome = produto.Codigo.ToString() + " - " + ano;

                    if (produto.UnidadeNegocio != null && produto.SubfamiliaProduto != null && produto.Segmento != null && produto.FamiliaProduto != null)
                    {
                        HistoricoComprasSubfamilia hSubFamilia = RepositoryService.HistoricoComprasSubFamilia
                            .ObterPor(produto.UnidadeNegocio.Id, produto.SubfamiliaProduto.Id, produto.Segmento.Id, produto.FamiliaProduto.Id, ano, trimestre);

                        if(hSubFamilia != null)
                            hsCompProduto.SubFamilia = new Lookup(hSubFamilia.ID.Value,"");
                    }

                    RepositoryService.HistoricoComprasProduto.Create(hsCompProduto);
                }
            }
        }

        #endregion
    }
}