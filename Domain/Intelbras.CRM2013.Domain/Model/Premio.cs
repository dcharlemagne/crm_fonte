using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_premio_fidelidade")]
    public class Premio: DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public Premio() { }

        public Premio(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Premio(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private Guid grupoPremio;
        private int pontosNecessarios;
        private string caminhoImagem;

        public Guid GrupoPremio
        {
            get { return grupoPremio; }
            set { grupoPremio = value; }
        }

        public int PontosNecessarios
        {
            get { return pontosNecessarios; }
            set { pontosNecessarios = value; }
        }

        public string CaminhoImagem
        {
            get { return caminhoImagem; }
            set { caminhoImagem = value; }
        }

        #endregion
    }
}
