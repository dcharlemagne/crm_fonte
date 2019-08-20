using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("new_tabela_preco")]
    public class TabelaDePreco : DomainBase
    {

        private Model.Conta cliente;
        private UnidadeNegocio unidadeDeNegocio;
        private string codigoDaTabelaDePreco;
        private Categoria categoria;
        private int tabelaEspecifica;
        private Representante representante;
        private CondicaoPagamento condicaoDePagamento;

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        public CondicaoPagamento CondicaoDePagamento
        {
            get { return condicaoDePagamento; }
            set { condicaoDePagamento = value; }
        }

        public Representante Representante
        {
            get { return representante; }
            set { representante = value; }
        }

        public int TabelaEspecifica
        {
            get { return tabelaEspecifica; }
            set { tabelaEspecifica = value; }
        }

        public Categoria Categoria
        {
            get { return categoria; }
            set { categoria = value; }
        }

        public string CodigoDaTabelaDePreco
        {
            get { return codigoDaTabelaDePreco; }
            set { codigoDaTabelaDePreco = value; }
        }

        public UnidadeNegocio UnidadeDeNegocio
        {
            get { return unidadeDeNegocio; }
            set { unidadeDeNegocio = value; }
        }

        public Model.Conta Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }

        #region Contrutores

        private RepositoryService RepositoryService { get; set; }

        public TabelaDePreco() { }

        public TabelaDePreco(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public TabelaDePreco(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion
    }
}
