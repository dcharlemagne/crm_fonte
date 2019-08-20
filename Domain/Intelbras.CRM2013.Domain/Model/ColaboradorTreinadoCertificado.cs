using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_colaboradorestreincert")]
    public class ColaboradorTreinadoCertificado : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ColaboradorTreinadoCertificado() { }

        public ColaboradorTreinadoCertificado(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ColaboradorTreinadoCertificado(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("itbc_colaboradorestreincertid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_accountid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_contactid")]
        public Lookup Contato { get; set; }

        //[LogicalAttribute("itbc_datatreincert")]
        //public DateTime DataTreinamento { get; set; }

        [LogicalAttribute("itbc_dataconclusao")]
        public DateTime? DataConclusao { get; set; }

        [LogicalAttribute("itbc_validade")]
        public DateTime? DataValidade { get; set; }

        [LogicalAttribute("itbc_idmatricula")]
        public Int32? IdMatricula { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        [LogicalAttribute("itbc_treinamcertifid")]
        public Lookup TreinamentoCertificado { get; set; }

        [LogicalAttribute("statuscode")]
        public Int32? StatusAprovacao { get; set; }

        #endregion
    }
}
