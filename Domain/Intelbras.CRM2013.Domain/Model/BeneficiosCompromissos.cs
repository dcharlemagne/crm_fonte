using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_benefecomp")]
    public class BeneficiosCompromissos : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public BeneficiosCompromissos() { }

        public BeneficiosCompromissos(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public BeneficiosCompromissos(string organization, bool isOffline, object provider)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_benefecompid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_perfilid")]
        public Lookup Perfil { get; set; }

        [LogicalAttribute("itbc_benefdoprogid")]
        public Lookup Beneficio { get; set; }

        [LogicalAttribute("itbc_compdoprogid")]
        public Lookup Compromisso { get; set; }

        [LogicalAttribute("itbc_status1beneficio")]
        public Lookup Status1Beneficio { get; set; }

        [LogicalAttribute("itbc_status1compromisso")]
        public Lookup Status1Compromisso { get; set; }

        [LogicalAttribute("itbc_status2beneficio")]
        public Lookup Status2Beneficio { get; set; }

        [LogicalAttribute("itbc_status2compromisso")]
        public Lookup Status2Compromisso { get; set; }

        [LogicalAttribute("itbc_status3beneficio")]
        public Lookup Status3Beneficio { get; set; }

        [LogicalAttribute("itbc_status3compromisso")]
        public Lookup Status3Compromisso { get; set; }

        [LogicalAttribute("itbc_status4beneficio")]
        public Lookup Status4Beneficio { get; set; }

        [LogicalAttribute("itbc_status4compromisso")]
        public Lookup Status4Compromisso { get; set; }
        
        #endregion
    }
}
