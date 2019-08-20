using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcamentoportrimestredaunidade")]
    public class OrcamentodaUnidadeporTrimestre : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeporTrimestre(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        public OrcamentodaUnidadeporTrimestre(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcamentoportrimestredaunidadeid")]
        public Guid? ID { get; set; }



        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }



        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }



        [LogicalAttribute("itbc_orcamentoparanovosprodutos")]
        public decimal? OrcamentoparanovosProdutos { get; set; }



        [LogicalAttribute("itbc_orcamentonaoalocado")]
        public decimal? OrcamentonaoAlocado { get; set; }



        [LogicalAttribute("new_OrcamentoporUnidadeId")]
        public Lookup OrcamentoporUnidade { get; set; }



        [LogicalAttribute("itbc_UnidadedeNegocioId")]
        public Lookup UnidadedeNegocio { get; set; }



        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }



        [LogicalAttribute("itbc_OrcamentoRealizado")]
        public decimal? OrcamentoRealizado { get; set; }



        [LogicalAttribute("itbc_orcamentoplanejado")]
        public decimal? OrcamentoPlanejado { get; set; }



        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }



        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }



        [LogicalAttribute("itbc_usuariointegracao")]
        public String UsuarioIntegracao { get; set; }



        #endregion
    }
}

