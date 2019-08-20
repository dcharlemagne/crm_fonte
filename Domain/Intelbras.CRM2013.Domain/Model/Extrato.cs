using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_extrato_pagamento_ocorrencia")]
    public class Extrato : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Extrato() { }

        public Extrato(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Extrato(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private List<LancamentoAvulsoDoExtrato> lancamentoAvulsoDoExtrato = null;
        [LogicalAttribute("new_extrato")]
        public string Numero { get; set; }

        private Anotacao anexo = null;
        public Anotacao Anexo
        {
            get { return anexo; }
            set { anexo = value; }
        }
        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        [LogicalAttribute("new_nota_fiscal")]
        public string NumeroNotaFiscal { get; set; }
        [LogicalAttribute("new_data_recebimento_nf")]
        public DateTime? DataRecebimentoNotaFiscal { get; set; }
        [LogicalAttribute("new_data_prevista_pagamento")]
        public DateTime? DataPrevistaDePagamento { get; set; }
        [LogicalAttribute("new_data_envio_nf")]
        public DateTime? DataEnvioNotaFiscal { get; set; }
        [LogicalAttribute("new_posto_servicoid")]
        public Lookup AutorizadaId { get; set; }
        private Model.Conta _autorizado = null;
        public Model.Conta Autorizada
        {
            get
            {
                if (_autorizado == null && AutorizadaId != null)
                    _autorizado = RepositoryService.Conta.Retrieve(AutorizadaId.Id);
                return _autorizado;
            }
            set { _autorizado = value; }
        }
        [LogicalAttribute("new_valor_extrato")]
        public Decimal? ValorTotal { get; set; }
        [LogicalAttribute("new_valor_lancamento")]
        public Decimal? ValorDosLancamentosAvulsos { get; set; }
        [LogicalAttribute("new_valor_ordem_servico")]
        public Decimal? ValorOS { get; set; }

        public List<LancamentoAvulsoDoExtrato> LancamentoAvulso
        {
            get
            {
                if (this.Id == Guid.Empty) return new List<LancamentoAvulsoDoExtrato>();
                if (null == lancamentoAvulsoDoExtrato) lancamentoAvulsoDoExtrato = (new RepositoryService(OrganizationName, IsOffline)).LancamentoAvulsoDoExtrato.ListarPor(this);

                return lancamentoAvulsoDoExtrato;
            }
        }    

        #endregion

        #region Métodos

        public void AtualizarValor()
        {
            if (this.Id == Guid.Empty)
                return;

            this.ValorOS = 0;
            this.ValorDosLancamentosAvulsos = 0;

            List<Ocorrencia> lista = RepositoryService.Ocorrencia.ListarPor(this, null);
            
            foreach (Ocorrencia ocorrencia in lista)
                if (ocorrencia.ValorServico.HasValue && ocorrencia.ValorServico.Value != decimal.MinValue)
                    this.ValorOS += ocorrencia.ValorServico.Value;

            foreach (LancamentoAvulsoDoExtrato item in this.LancamentoAvulso)
                if (item.Valor.HasValue)
                    this.ValorDosLancamentosAvulsos += item.Valor.Value;

            this.ValorTotal = this.ValorDosLancamentosAvulsos.Value + this.ValorOS.Value;
        }

        #endregion
    }
}