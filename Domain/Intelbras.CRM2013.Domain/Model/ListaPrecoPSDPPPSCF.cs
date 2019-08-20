using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_psdid")]
    public class ListaPrecoPSDPPPSCF : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ListaPrecoPSDPPPSCF() { }

        public ListaPrecoPSDPPPSCF(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public ListaPrecoPSDPPPSCF(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("itbc_psdidid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_businessunit")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_data_fim")]
        public DateTime? DataFim { get; set; }

        [LogicalAttribute("itbc_data_inicio")]
        public DateTime? DataInicio { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("itbc_fatorparaprecopredatorio")]
        public decimal? FatorPrecoPredatorio { get; set; }

        #endregion

    }
}
