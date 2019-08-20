using NUnit.Framework;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    class ExcluiProdutosPolitica : Base
    {

        public ExcluiProdutosPolitica()
        {
            SDKore.DomainModel.RepositoryFactory.SetTag(this.OrganizationName);
        }

        private Domain.IRepository.IConta<Domain.Model.Conta> _repositorioConta;
        private Domain.IRepository.IConta<Domain.Model.Conta> RepositorioConta
        {
            get
            {
                if (_repositorioConta == null)
                {
                    _repositorioConta = SDKore.DomainModel.RepositoryFactory.Instance.Container.Resolve<Domain.IRepository.IConta<Domain.Model.Conta>>();
                    _repositorioConta.SetOrganization(this.OrganizationName);
                    _repositorioConta.SetIsOffline(this.IsOffline);
                }
                return _repositorioConta;
            }
        }


        private Domain.IRepository.IProdutoPoliticaComercial<Domain.Model.ProdutoPoliticaComercial> _repositorioProdutoPolitica;
        private Domain.IRepository.IProdutoPoliticaComercial<Domain.Model.ProdutoPoliticaComercial> repositorioProdutoPolitica
        {
            get
            {
                if (_repositorioProdutoPolitica == null)
                {
                    _repositorioProdutoPolitica = SDKore.DomainModel.RepositoryFactory.Instance.Container.Resolve<Domain.IRepository.IProdutoPoliticaComercial<Domain.Model.ProdutoPoliticaComercial>>();
                    _repositorioProdutoPolitica.SetOrganization(this.OrganizationName);
                    _repositorioProdutoPolitica.SetIsOffline(this.IsOffline);
                }
                return _repositorioProdutoPolitica;
            }
        }

        private Domain.IRepository.IPoliticaComercial<Domain.Model.PoliticaComercial> _repositorioPoliticaComercial;
        private Domain.IRepository.IPoliticaComercial<Domain.Model.PoliticaComercial> repositorioPoliticaComercial
        {
            get
            {
                if (_repositorioPoliticaComercial == null)
                {
                    _repositorioPoliticaComercial = SDKore.DomainModel.RepositoryFactory.Instance.Container.Resolve<Domain.IRepository.IPoliticaComercial<Domain.Model.PoliticaComercial>>();
                    _repositorioPoliticaComercial.SetOrganization(this.OrganizationName);
                    _repositorioPoliticaComercial.SetIsOffline(this.IsOffline);
                }
                return _repositorioPoliticaComercial;
            }
        }

        private Domain.IRepository.ISharePointDocumentLocation<Domain.Model.SharePointDocumentLocation> _repositorioSharePointDocumentLocation;
        private Domain.IRepository.ISharePointDocumentLocation<Domain.Model.SharePointDocumentLocation> RepositorioSharePointDocumentLocation
        {
            get
            {
                if (_repositorioSharePointDocumentLocation == null)
                {
                    _repositorioSharePointDocumentLocation = SDKore.DomainModel.RepositoryFactory.Instance.Container.Resolve<Domain.IRepository.ISharePointDocumentLocation<Domain.Model.SharePointDocumentLocation>>();
                    _repositorioSharePointDocumentLocation.SetOrganization(this.OrganizationName);
                    _repositorioSharePointDocumentLocation.SetIsOffline(this.IsOffline);
                }
                return _repositorioSharePointDocumentLocation;
            }
        }

        [Test]
        public void TestaExcluiProdutosPolitica()
        {           
            List<Domain.Model.PoliticaComercial> lstPoliticas = repositorioPoliticaComercial.ListarPorTipoNull();

            foreach (Domain.Model.PoliticaComercial item in lstPoliticas)
            {
                List<Domain.Model.ProdutoPoliticaComercial> lstPoliticasComercial = repositorioProdutoPolitica.ListarPor(item.ID.Value);
                foreach (Domain.Model.ProdutoPoliticaComercial item2 in lstPoliticasComercial)
                {
                    try
                    {
                        repositorioProdutoPolitica.Delete(item2.ID.Value);
                    }
                    catch { }
                }

                repositorioPoliticaComercial.Delete(item.ID.Value);

            }
        }

    }
}
