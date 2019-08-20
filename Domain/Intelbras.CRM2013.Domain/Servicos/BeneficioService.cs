using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class BeneficioService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public BeneficioService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public BeneficioService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public BeneficioService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region
        private BeneficioDoCanalService _ServiceBeneficioDoCanal = null;
        private BeneficioDoCanalService ServiceBeneficioDoCanal
        {
            get
            {
                if (_ServiceBeneficioDoCanal == null)
                    _ServiceBeneficioDoCanal = new BeneficioDoCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

                return _ServiceBeneficioDoCanal;
            }
        }
        #endregion

        #region Métodos

        public void AtivarOrDesativado(Beneficio mBeneficio)
        {
            bool status = (mBeneficio.BeneficioAtivo.HasValue ? (mBeneficio.BeneficioAtivo.Value.Equals((int)Enum.BeneficiodoPrograma.BeneficioAtivo.sim) ? true : false) : false);
            ServiceBeneficioDoCanal.AtivaOuDesativaTodos(mBeneficio.ID.Value, status);
        }

        public Beneficio ObterPor(Guid beneficioId)
        {
            return RepositoryService.Beneficio.ObterPor(beneficioId);
        }

        #endregion
    }
}
