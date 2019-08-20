using Intelbras.CRM2013.Domain.Servicos;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    public class VerbaVMC : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public VerbaVMC(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public VerbaVMC(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        public string CodigoConta { get; set; }
        public string CNPJ { get; set; }
        public decimal Verba { get; set; }
        public string Trimestre { get; set; }
        public DateTime Validade { get; set; }
    }
}
