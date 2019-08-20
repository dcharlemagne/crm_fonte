using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    //[LogicalEntity("")] não existe entidade no CRM, somente a classe
    public class CreditoFidelidade: DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CreditoFidelidade() { }

        public CreditoFidelidade(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public CreditoFidelidade(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        private int valor = int.MinValue;
        private int valorDisponivel = int.MinValue;
        private string guidParticipante = string.Empty;
        private DateTime dataVencimento = DateTime.MinValue;
        private Guid pontosParticipante = Guid.Empty;

        public int Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        public int ValorDisponivel
        {
            get { return valorDisponivel; }
            set { valorDisponivel = value; }
        }

        public string GuidParticpante
        {
            get { return guidParticipante; }
            set { guidParticipante = value; }
        }

        public DateTime DataVencimento
        {
            get { return dataVencimento; }
            set { dataVencimento = value; }
        }

        public Guid PontosParticipante
        {
            get { return pontosParticipante; }
            set { pontosParticipante = value; }
        }

        #endregion

        #region Métodos


        #endregion
    }
}
