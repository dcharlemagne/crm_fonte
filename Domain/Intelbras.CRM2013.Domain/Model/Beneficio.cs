using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_beneficio")]
    public class Beneficio : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Beneficio() { }

        public Beneficio(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }


        public Beneficio(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_beneficioid")]
            public Guid? ID { get; set; }

            [LogicalAttribute("itbc_name")]
            public String Nome { get; set; }

            [LogicalAttribute("itbc_codigo")]
            public int? Codigo { get; set; }

            [LogicalAttribute("itbc_beneficioativo")]
            public int? BeneficioAtivo { get; set; }

            [LogicalAttribute("itbc_passivel_solicitacao")]
            public Boolean? PassivelDeSolicitacao { get; set; }

            [LogicalAttribute("itbc_possuicontroledecontacorrente")]
            public int? PossuiControleContaCorrente { get; set; }
            //[LogicalAttribute("itbc_validade")]
            //public DateTime? Validade { get; set; }

        #endregion
    }
}
