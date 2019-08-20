using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class HistoricoComprasCanalProdutoMesService
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

        public HistoricoComprasCanalProdutoMesService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public HistoricoComprasCanalProdutoMesService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Repositórios De Domínio


        #endregion

        #region Métodos

        public void RetornoDWHistoricoCompraCanalProdutoMes(int ano, int trimestre)
        {
            DataTable dtHistoricoCompraCanalProdutoMes = RepositoryService.HistoricoComprasCanalProdutoMes.ListarPor(ano.ToString(), trimestre.ToString());
            foreach (DataRow item in dtHistoricoCompraCanalProdutoMes.Rows)
            {
                int productNumber = 0;
                if (!Int32.TryParse(item["CD_item"].ToString(), out productNumber))
                    continue;

                Product produto = RepositoryService.Produto.ObterPor(item["CD_item"].ToString());

                if (produto == null)
                    continue;

                Conta canal = RepositoryService.Conta.Retrieve(new Guid(item["CD_guid"].ToString()));

                if (canal == null)
                    continue;

                HistoricoComprasCanalProdutoMes historicoComprasCanalProdutoMes = RepositoryService.HistoricoComprasCanalProdutoMes
                    .ObterPor(trimestre, (int)item["CD_Mes"], ano, produto.ID.Value, canal.ID.Value);

                if (historicoComprasCanalProdutoMes != null)
                {
                    historicoComprasCanalProdutoMes.Valor = decimal.Parse(item["Valor"].ToString());
                    historicoComprasCanalProdutoMes.Quantidade = decimal.Parse(item["Qtd"].ToString());


                    if (produto.UnidadeNegocio != null && canal.ID.HasValue)
                    {
                        HistoricoCompraCanal hsCanal = RepositoryService.HistoricoComprasCanal.ObterPor(produto.UnidadeNegocio.Id, trimestre, ano, canal.ID.Value);
                        if (hsCanal != null)
                            historicoComprasCanalProdutoMes.HistoricoCanal = new Lookup(hsCanal.ID.Value, "");
                    }
                    RepositoryService.HistoricoComprasCanalProdutoMes.Update(historicoComprasCanalProdutoMes);
                }
                else
                {
                    HistoricoComprasCanalProdutoMes hsCompCanalProdutoMes = new HistoricoComprasCanalProdutoMes(this.RepositoryService.NomeDaOrganizacao, this.RepositoryService.IsOffline);
                    hsCompCanalProdutoMes.Produto = new Lookup(produto.ID.Value, "");
                    hsCompCanalProdutoMes.Canal = new Lookup(canal.ID.Value, "");
                    hsCompCanalProdutoMes.Quantidade = decimal.Parse(item["Qtd"].ToString());
                    hsCompCanalProdutoMes.Valor = decimal.Parse(item["Valor"].ToString());
                    hsCompCanalProdutoMes.Mes = int.Parse(item["CD_Mes"].ToString());
                    hsCompCanalProdutoMes.Ano = ano;
                    hsCompCanalProdutoMes.Trimestre = trimestre;
                    hsCompCanalProdutoMes.Nome = produto.Codigo.ToString() + " - " + Helper.ConverterMesEnumParaExtenso(item["CD_Mes"].ToString()) + " - " + ano;

                    if (produto.UnidadeNegocio != null && canal.ID.HasValue)
                    {
                        HistoricoCompraCanal hsCanal = RepositoryService.HistoricoComprasCanal.ObterPor(produto.UnidadeNegocio.Id, trimestre, ano, canal.ID.Value);
                        if (hsCanal != null)
                            hsCompCanalProdutoMes.HistoricoCanal = new Lookup(hsCanal.ID.Value, "");
                    }

                    RepositoryService.HistoricoComprasCanalProdutoMes.Create(hsCompCanalProdutoMes);
                }
            }
        }

        #endregion
    }
}
