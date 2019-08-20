using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_representante")]
    public class Representante : DomainBase
    {

        private RepositoryService RepositoryService { get; set; }

        public Representante() { }

        public Representante(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Representante(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        private int codigoRepresentante = int.MinValue;
        private string descricaoRepresentante = string.Empty;

        [LogicalAttribute("new_name")]
        public String Nome { get; set; }
        public int CodigoRepresentante
        {
            get { return codigoRepresentante; }
            set { codigoRepresentante = value; }
        }
        public string DescricaoRepresentante
        {
            get { return descricaoRepresentante; }
            set { descricaoRepresentante = value; }
        }

    }
}
