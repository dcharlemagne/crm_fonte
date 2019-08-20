using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    public class Telefone : DomainBase
    {
        private string celular = "";
        public string Celular
        {
            get { return celular; }
            set { celular = value; }
        }

        public string RamalFax
        {
            get { return ramalFax; }
            set { ramalFax = value; }
        }
        private string ramalFax = "";

        private string fax = "";
        public string Fax
        {
            get { return fax; }
            set { fax = value; }
        }

        private string ramal = "";
        public string Ramal
        {
            get { return ramal; }
            set { ramal = value; }
        }

        private string numero = "";
        public string Numero
        {
            get { return numero; }
            set { numero = value; }
        }

        private RepositoryService RepositoryService { get; set; }

        public Telefone() { }

        public Telefone(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Telefone(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


    }
}
