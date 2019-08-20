using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_configuracaodebeneficio")]
    public class ConfiguracaoBeneficio : DomainBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public ConfiguracaoBeneficio() { }

        public ConfiguracaoBeneficio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ConfiguracaoBeneficio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_configuracaodebeneficioid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_percrebateantecipado")]
        public Decimal? PercRebateAntecipado { get; set; }

        [LogicalAttribute("itbc_anteciparrebate")]
        public Boolean AnteciparRebate { get; set; }

        [LogicalAttribute("itbc_calcularrebate")]
        public Boolean? CalcularRebate { get; set; }

        [LogicalAttribute("itbc_vigenciafinal")]
        public DateTime VigenciaFinal { get; set; }

        [LogicalAttribute("itbc_vigenciainicial")]
        public DateTime VigenciaInicial { get; set; }

        [LogicalAttribute("itbc_produtoid")]
        public Lookup Produto { get; set; }

        #endregion
    }
}
