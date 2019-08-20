using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_metas")]
    public class MetadaUnidade : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MetadaUnidade() { }

        public MetadaUnidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public MetadaUnidade(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_metasid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_metaplanejada")]
        public decimal? MetaPlanejada { get; set; }
        
        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }
        
        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }
        
        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }
        
        [LogicalAttribute("itbc_MetaGeradaPorId")]
        public Lookup MetaGeradaPor { get; set; }
        
        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }
        
        [LogicalAttribute("itbc_transactioncurrencyid")]
        public decimal? Moeda { get; set; }
        
        [LogicalAttribute("itbc_DataUltimaGeracao")]
        public DateTime? DataUltimaGeracao { get; set; }
        
        [LogicalAttribute("itbc_businessunit")]
        public Lookup UnidadedeNegocios { get; set; }
        
        [LogicalAttribute("itbc_RazodoStatusMetaKARepresentante")]
        public int? RazaodoStatusMetaKARepresentante { get; set; }
        
        [LogicalAttribute("itbc_MetaRealizada")]
        public decimal? MetaRealizada { get; set; }
        
        [LogicalAttribute("itbc_mensagem_processamento")]
        public string MensagemdeProcessamento { get; set; }
        
        [LogicalAttribute("itbc_NiveldaMeta")]
        public int? NiveldaMeta { get; set; }
        
        [LogicalAttribute("itbc_razodostatusmetasupervisor")]
        public int? RazaodoStatusMetaSupervisor { get; set; }
        
        [LogicalAttribute("itbc_razaodostatusmetamanual")]
        public int? RazaodoStatusMetaManual { get; set; }
        
        [LogicalAttribute("itbc_UsuarioIntegracao")]
        public String UsuarioIntegracao { get; set; }

        [LogicalAttribute("itbc_businessunit")]
        public Lookup OrcamentodaUnidade { get; set; }

        [LogicalAttribute("statecode")]
        public int? StateCode { get; set; }

        [LogicalAttribute("statuscode")]
        public int? StatusCode { get; set; }

        private UnidadeNegocio _UnidadeNegocio = null;
        public UnidadeNegocio UnidadedeNegocio
        {
            get
            {
                if (_UnidadeNegocio == null)
                    _UnidadeNegocio = RepositoryService.UnidadeNegocio.Retrieve(this.UnidadedeNegocios.Id);

                return _UnidadeNegocio;
            }
        }
        //[LogicalAttribute("itbc_metasid")]
        //public Guid? ID { get; set; }

        //[LogicalAttribute("itbc_Ano")]
        //public int? Ano { get; set; }

        //[LogicalAttribute("itbc_metaPlanejada")]
        //public decimal? MetaPlanejada { get; set; }

        //[LogicalAttribute("itbc_businessunit")]
        //public Lookup UnidadedeNegocios { get; set; }

        //[LogicalAttribute("itbc_name")]
        //public String Nome { get; set; }

        //[LogicalAttribute("itbc_MetaGeradaPorId")]
        //public Lookup MetaGeradaPor { get; set; }

        //[LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        //public DateTime? UltAtualizacaoIntegracao { get; set; }

        //[LogicalAttribute("itbc_transactioncurrencyid")]
        //public decimal? Moeda { get; set; }

        //[LogicalAttribute("itbc_DataUltimaGeracao")]
        //public DateTime? DataUltimaGeracao { get; set; }

        //[LogicalAttribute("itbc_NiveldaMeta")]
        //public int? NiveldaMeta { get; set; }

        //[LogicalAttribute("itbc_MetaRealizada")]
        //public decimal? MetaRealizada { get; set; }

        //[LogicalAttribute("itbc_OrcamentodaUnidadeId")]
        //public Lookup OrcamentodaUnidade { get; set; }

        //[LogicalAttribute("itbc_IntegradoPor")]
        //public String IntegradoPor { get; set; }

        //[LogicalAttribute("itbc_UsuarioIntegracao")]
        //public String UsuarioIntegracao { get; set; }

        //[LogicalAttribute("itbc_razodostatusmetakarepresentante")]
        //public int? RazaoStatusMetaKARepresentante { get; set; }

        //[LogicalAttribute("itbc_razaodostatusmetamanual")]
        //public int? RazaoStatusMetaManual { get; set; }

        //[LogicalAttribute("itbc_razodostatusmetasupervisor")]
        //public int? RazaoStatusMetaSupervisor { get; set; }

        //[LogicalAttribute("itbc_mensagem_processamento")]
        //public string MensagemProcessamento { get; set; }

      
        #endregion
    }
}