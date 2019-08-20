using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_orcamentodocanal")]
    public class OrcamentoPorCanal : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentoPorCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public OrcamentoPorCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_orcamentodocanalid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_OcramentoParaNovosProdutos")]
        public decimal? OrcamentoParaNovosProdutos { get; set; }

        [LogicalAttribute("itbc_UltAtualizacaoIntegracao")]
        public DateTime? UltAtualizacaoIntegracao { get; set; }

        [LogicalAttribute("itbc_CanalId")]
        public Lookup Canal { get; set; }

        private Conta _Conta = null;
        public Conta Conta
        {
            get
            {
                if (_Conta == null && Canal.Id != null && Canal.Id != Guid.Empty)
                    _Conta = RepositoryService.Conta.Retrieve(Canal.Id);

                return _Conta;
            }
        }

        [LogicalAttribute("itbc_trimestre")]
        public int? Trimestre { get; set; }

        [LogicalAttribute("itbc_OrcamentoporTrimestredaUnidadeId")]
        public Lookup OrcamentoporTrimestredaUnidade { get; set; }

        [LogicalAttribute("itbc_orcamentorealizado")]
        public decimal? OrcamentoRealizado { get; set; }

        [LogicalAttribute("itbc_orcamentoplanejado")]
        public decimal? OrcamentoPlanejado { get; set; }

        [LogicalAttribute("itbc_Ano")]
        public int? Ano { get; set; }

        [LogicalAttribute("itbc_unidadedenegocio")]
        public Lookup UnidadedeNegocio { get; set; }

        [LogicalAttribute("itbc_IntegradoPor")]
        public String IntegradoPor { get; set; }

        [LogicalAttribute("itbc_usuariointegracao")]
        public String UsuarioIntegracao { get; set; }
        #endregion
    }
}
