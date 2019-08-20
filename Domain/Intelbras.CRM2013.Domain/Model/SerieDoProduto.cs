using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    public class SerieDoProduto : DomainBase
    {
        private string numeroDeSerie = "";
        private string celula = "";
        private bool corporativo = false;
        private Fatura notaFiscal = null;
        private Product produto = null;
        private string ordem = "";
        private DateTime? dataFabricacaoProduto = null;
        private string numeroPedido = "";
        public string DescricaoDaMensagemDeIntegracao { get; set; }
        public string Ordem
        {
            get { return ordem; }
            set { ordem = value; }
        }

        public Product Produto
        {
            get { return produto; }
            set { produto = value; }
        }

        public Fatura NotaFiscal
        {
            get { return notaFiscal; }
            set { notaFiscal = value; }
        }

        public DateTime? DataFabricacaoProduto
        {
            get { return dataFabricacaoProduto; }
            set { dataFabricacaoProduto = value; }
        }

        public bool Corporativo
        {
            get { return corporativo; }
            set { corporativo = value; }
        }

        public string Celula
        {
            get { return celula; }
            set { celula = value; }
        }

        public string NumeroDeSerie
        {
            get { return numeroDeSerie; }
            set { numeroDeSerie = value; }
        }

        public string Descricao { get; set; }

        private RepositoryService RepositoryService { get; set; }

        public SerieDoProduto(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SerieDoProduto(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        public string NumeroPedido
        {
            get { return numeroPedido; }
            set { numeroPedido = value; }
        }

    }
}
