using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcamentodaunidade")]
    public class OrcamentodaUnidade : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidade() { }
        
        public OrcamentodaUnidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public OrcamentodaUnidade(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcamentodaunidadeid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_orcamentoparanovosprodutos")]
        public decimal? OrcamentoParaNovosProdutos { get; set; }

        [LogicalAttribute("itbc_orcamentonaoalocado")]
        public decimal? OrcamentoNaoAlocado { get; set; }

        [LogicalAttribute("itbc_OrcamentoRealizado")]
        public decimal? OrcamentoRealizado { get; set; }

        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        [LogicalAttribute("itbc_dataultimageracao")]
        public DateTime? DataUltimaGeracao { get; set; }

        [LogicalAttribute("itbc_OrcamentoGeradoPor")]
        public Lookup OrcamentoGeradoPor { get; set; }

        [LogicalAttribute("itbc_OrcamentoPlanejado")]
        public decimal? OrcamentoPlanejado { get; set; }

        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_niveldoorcamento")]
        public int? NiveldoOrcamento { get; set; }

        [LogicalAttribute("itbc_Integradopor")]
        public String Integradopor { get; set; }

        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }

        [LogicalAttribute("statecode")]
        public int? StateCode { get; set; }

        [LogicalAttribute("statuscode")]
        public int? StatusCode { get; set; }

        [LogicalAttribute("itbc_razaodostatusoramentomanual")]
        public int? RazaoStatusOramentoManual { get; set; }

        [LogicalAttribute("itbc_mensagem_processamento")]
        public string MensagemProcessamento { get; set; }

        private UnidadeNegocio _UnidadeNegocio = null;
        public UnidadeNegocio UnidadeNegocio
        {
            get
            {
                if (_UnidadeNegocio == null)
                    _UnidadeNegocio = RepositoryService.UnidadeNegocio.Retrieve(this.UnidadedeNegocio.Id);

                return _UnidadeNegocio;
            }
        }

        #endregion
    }
}

