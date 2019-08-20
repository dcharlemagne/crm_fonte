using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_solicitacaoxunidades")]
    public class SolicitacaoXUnidades : DomainBase
    {
        #region Construtores

        public SolicitacaoXUnidades() { }

        public SolicitacaoXUnidades(string organization, bool isOffline)
            : base(organization, isOffline)
        {
        }

        public SolicitacaoXUnidades(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_solicitacaoxunidadesid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_solicitacaoid")]
        public Lookup Solicitacao { get; set; }

        [LogicalAttribute("itbc_unidadesid")]
        public Lookup Unidades { get; set; }
        public UnidadeNegocio UnidadeNegocio
        {
            get
            {
                UnidadeNegocio unidadeNegocio = null;
                if (this.Unidades.Id != Guid.Empty)
                    unidadeNegocio = (new Domain.Servicos.RepositoryService()).UnidadeNegocio.ObterPor(this.Unidades.Id);
                return unidadeNegocio;
            }

            set { }
        }


        #endregion
    }
}