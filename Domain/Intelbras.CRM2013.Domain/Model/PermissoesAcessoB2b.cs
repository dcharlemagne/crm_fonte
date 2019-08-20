using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_permissao_usuario_b2b")]
    public class PermissoesAcessoB2b : DomainBase
    {
        #region Contrutores

        private RepositoryService RepositoryService { get; set; }

        public PermissoesAcessoB2b() { }

        public PermissoesAcessoB2b(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public PermissoesAcessoB2b(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private Contato representante = null;
        private Contato contato = null;
        private UnidadeNegocio unidadeDeNegocio = null;

        public Contato Representante
        {
            get { return representante; }
            set { representante = value; }
        }

        public Contato Contato
        {
            get { return contato; }
            set { contato = value; }
        }

        public UnidadeNegocio UnidadeDeNegocio
        {
            get { return unidadeDeNegocio; }
            set { unidadeDeNegocio = value; }
        }

        #endregion

    }
}
