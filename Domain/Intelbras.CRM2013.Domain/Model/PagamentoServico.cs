using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_pagamento_servico")]
    public class PagamentoServico : DomainBase
    {
        private RepositoryService RepositoryService { get; set; }

        public PagamentoServico() { }

        public PagamentoServico(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PagamentoServico(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        [LogicalAttribute("new_tipo_pagamento")]
        public int? Tipo { get; set; }
        [LogicalAttribute("new_pagamento_servico_ocorrenciaid")]
        public Lookup OcorrenciaId { get; set; }
        public Ocorrencia Ocorrencia { get; set; }
        [LogicalAttribute("new_valor_nf")]
        public decimal? Valor { get; set; }

        [LogicalAttribute("itbc_tipo_pagamentoid")]
        public Lookup TipoPagamentoId { get; set; }

        private TipoPagamento _TipoPagamento = null;
        public TipoPagamento TipoPagamento
        {
            get
            {
                if (TipoPagamentoId != null && _TipoPagamento == null && this.Id != Guid.Empty)
                    _TipoPagamento = (new CRM2013.Domain.Servicos.RepositoryService()).TipoPagamento.Retrieve(this.TipoPagamentoId.Id);
                return _TipoPagamento;
            }
            set { _TipoPagamento = value; }
        }
        [LogicalAttribute("new_pagamento_servicoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_quantidade")]
        public decimal? Quantidade { get; set; }

        [LogicalAttribute("new_nf_pagamento")]
        public String NumeroNotaFiscal { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        [LogicalAttribute("new_data_pagamento")]
        public DateTime? DataPagamento { get; set; }

        [LogicalAttribute("new_observacao")]
        public String Observacao { get; set; } 
    }
}