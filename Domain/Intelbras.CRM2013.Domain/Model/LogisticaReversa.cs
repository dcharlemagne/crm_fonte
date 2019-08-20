using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_extrato_logistica_reversa")]
    public class LogisticaReversa : DomainBase
    {
        private string motivo, justificativa;
        private List<Product> produtosDaLogisticaReversa = null;

        [LogicalAttribute("new_serie_nota_fiscal")]
        public string SerieNotaFiscal { get; set; }
        [LogicalAttribute("new_numero_nota_fiscal")]
        public string NumeroNotaFiscal { get; set; }
        
        public List<Product> ProdutosDaLogisticaReversa
        {
            get { return produtosDaLogisticaReversa; }
            set { produtosDaLogisticaReversa = value; }
        }

        public string Justificativa
        {
            get { return justificativa; }
            set { justificativa = value; }
        }
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        public string Motivo
        {
            get { return motivo; }
            set { motivo = value; }
        }
        [LogicalAttribute("new_data_recebimento")]
        public DateTime? DataFinalizacao { get; set; }
        [LogicalAttribute("createdon")]
        public DateTime? DataDeCriacao { get; set; }
        [LogicalAttribute("new_data_nota_fiscal")]
        public DateTime? DataNotaFiscal { get; set; }
        [LogicalAttribute("statuscode")]
        public int? StatusCode { get; set; }
        [LogicalAttribute("new_posto_autorizadoid")]
        public Lookup PostoAutorizadoId { get; set; }


        private RepositoryService RepositoryService { get; set; }

        public LogisticaReversa() { }

        public LogisticaReversa(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public LogisticaReversa(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

    }
}
