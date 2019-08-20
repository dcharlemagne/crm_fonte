using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    //[LogicalEntity("")] não existe entidade no CRM, somente a classe
    public class DebitoFidelidade: DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public DebitoFidelidade() { }

        public DebitoFidelidade(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public DebitoFidelidade(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private int valor = int.MinValue;
        private int situacao = int.MinValue;
        private string guidParticipante = string.Empty;
        private DateTime dataCriacao = DateTime.MinValue;
        private Guid resgate = Guid.Empty;
        private Guid credito = Guid.Empty;

        public int Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        public int Situacao
        {
            get { return situacao; }
            set { situacao = value; }
        }

        public string GuidParticpante
        {
            get { return guidParticipante; }
            set { guidParticipante = value; }
        }

        public DateTime DataCriacao
        {
            get { return dataCriacao; }
            set { dataCriacao = value; }
        }

        public Guid Resgate
        {
            get { return resgate; }
            set { resgate = value; }
        }

        public Guid Credito
        {
            get { return credito; }
            set { credito = value; }
        }

        #endregion

        #region Métodos


        #endregion
    }
}
