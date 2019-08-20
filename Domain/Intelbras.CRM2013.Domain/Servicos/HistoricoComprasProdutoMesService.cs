using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasProdutoMesService
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

        public HistoricoComprasProdutoMesService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public HistoricoComprasProdutoMesService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos


        public void RetornoDWHistoricoCompraProdutoMes(int ano, int trimestre)
        {
            DataTable dtHistoricoCompraProduto = RepositoryService.HistoricoComprasProdutoMes.ListarPor(ano.ToString(), trimestre.ToString());

            foreach (DataRow item in dtHistoricoCompraProduto.Rows)
            {
                int productNumber = 0;
                if (!Int32.TryParse(item["CD_item"].ToString(), out productNumber))
                    continue;

                Product produto = RepositoryService.Produto.ObterPor(item["CD_item"].ToString());

                if (produto == null)
                    continue;


                HistoricoDeComprasPorProdutoMes historicoCompraProdutoMes = RepositoryService.HistoricoComprasProdutoMes
                    .ObterPor(trimestre, ano, produto.ID.Value, (int)item["CD_Mes"]);

                if (historicoCompraProdutoMes != null)
                {
                    historicoCompraProdutoMes.Valor = decimal.Parse(item["Valor"].ToString());
                    historicoCompraProdutoMes.quantidade = decimal.Parse(item["Qtd"].ToString());

                    HistoricoDeComprasPorProduto hCompraProduto = RepositoryService.HistoricoComprasProduto
                        .ObterPor(historicoCompraProdutoMes.trimestre, historicoCompraProdutoMes.ano, historicoCompraProdutoMes.produto.Id);

                    if (hCompraProduto != null)
                        historicoCompraProdutoMes.HistoricoProduto = new Lookup(hCompraProduto.ID.Value, "");

                    RepositoryService.HistoricoComprasProdutoMes.Update(historicoCompraProdutoMes);
                }
                else
                {
                    HistoricoDeComprasPorProdutoMes hsCompProdutoMes = new HistoricoDeComprasPorProdutoMes(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);
                    hsCompProdutoMes.produto = new Lookup(produto.ID.Value, "");
                    hsCompProdutoMes.quantidade = decimal.Parse(item["Qtd"].ToString());
                    hsCompProdutoMes.Valor = decimal.Parse(item["Valor"].ToString());
                    hsCompProdutoMes.mes = int.Parse(item["CD_Mes"].ToString());
                    hsCompProdutoMes.ano = ano;
                    hsCompProdutoMes.trimestre = trimestre;
                    hsCompProdutoMes.Nome = produto.Codigo.ToString() + " - " + Helper.ConverterMesEnumParaExtenso(item["CD_Mes"].ToString()) + " - " + ano;

                    RepositoryService.HistoricoComprasProdutoMes.Create(hsCompProdutoMes);
                }
            }
        }

        #endregion
    }
}
