using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_resgate_premio_fidelidade")]
    public class ResgateDePremios: DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ResgateDePremios() { }

        public ResgateDePremios(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ResgateDePremios(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private Guid participante;
        private Guid premio;
        private int pontos;
        private DateTime dataResgate;

        public Guid Participante
        {
            get { return participante; }
            set { participante = value; }
        }

        public Guid Premio
        {
            get { return premio; }
            set { premio = value; }
        }

        public int Pontos
        {
            get { return pontos; }
            set { pontos = value; }
        }

        public DateTime DataResgate
        {
            get { return dataResgate; }
            set { dataResgate = value; }
        }

        #endregion

        #region Métodos


        #endregion
    }
}
