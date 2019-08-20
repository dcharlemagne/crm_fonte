using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_valor_servico")]
    public class ValorDoServico : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ValorDoServico() { }

        public ValorDoServico(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ValorDoServico(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        decimal valor = decimal.MinValue;
        public decimal Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        #endregion

        #region Métodos



        #endregion
    }
}