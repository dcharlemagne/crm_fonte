using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_segurodaconta")]
    public class SeguroConta : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SeguroConta() { }

        public SeguroConta(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SeguroConta(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_segurodacontaid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_contaid")]
        public Lookup Conta { get; set; }

        [LogicalAttribute("itbc_modalidade")]
        public String Modalidade { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        //[LogicalAttribute("itbc_seguros")]
        //public Lookup Seguro { get; set; }

        [LogicalAttribute("itbc_valorsegurado")]
        public Decimal? ValorSegurado { get; set; }

        [LogicalAttribute("itbc_vencimento")]
        public DateTime? Vencimento { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }


        #endregion
    }
}
