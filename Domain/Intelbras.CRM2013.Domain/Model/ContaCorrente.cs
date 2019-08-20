using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("account")] //não existe entidade no CRM, somente a classe
    public class ContaCorrente : DomainBase
    {

        private string numero = "";
        private string agencia = "";
        private string banco = "";

        public string Banco
        {
            get { return banco; }
        }

        public string Agencia
        {
            get { return agencia; }
        }

        public string Numero
        {
            get { return numero; }
        }

        private RepositoryService RepositoryService { get; set; }

        public ContaCorrente() { }

        public ContaCorrente(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ContaCorrente(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
    }
}
