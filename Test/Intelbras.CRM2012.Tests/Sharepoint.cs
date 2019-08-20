using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NUnit.Framework;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class Sharepoint : Base
    {
        public Sharepoint()
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
        public void ExcluiProdutosPolitica()
        {        


             //Consulta todas as contas
            List<Domain.Model.PoliticaComercial> lstPoliticas = repositorioPoliticaComercial.ListarPorTipoNull();

            foreach (Domain.Model.PoliticaComercial item in lstPoliticas)
            {
                List<Domain.Model.ProdutoPoliticaComercial> lstContas = repositorioProdutoPolitica.ListarPor(item.ID.Value);
                foreach (Domain.Model.ProdutoPoliticaComercial item2 in lstContas)
                {
                    try
                    {
                        repositorioProdutoPolitica.Delete(item2.ID.Value);
                    }
                    catch {}
                }
            } 
        }


        [Test]
        public void CriarDiretorio()
        {
            Guid guidId = new Guid("00D3F289-C048-E411-93F5-00155D013E70");

            new Domain.Servicos.SharepointServices(this.OrganizationName, this.IsOffline).CriarDiretorio<Domain.Model.SolicitacaoBeneficio>("SolicitacaoBeneficio", guidId);
        
        }
    }
}
