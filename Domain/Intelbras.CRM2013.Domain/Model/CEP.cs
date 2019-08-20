using Intelbras.CRM2013.Domain.Servicos;
using SDKore.Crm.Util;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("customeraddress")]
    public class EnderecoCEP : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public EnderecoCEP() { }

        public EnderecoCEP(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public EnderecoCEP(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public bool? CidadeZonaFranca { get; set; }
        public int? CodigoIBGE { get; set; }
        public string Endereco { get; set; }
        public string Estado { get; set; }
        public string NomeCidade { get; set; }
        public string Pais { get; set; }
        public string UF { get; set; }
    }
}
