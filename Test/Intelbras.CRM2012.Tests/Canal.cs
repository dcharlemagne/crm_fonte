using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using NUnit.Framework;
using SDKore.DomainModel;
using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class Canal : Base
    {
        [Test]
        public void AdesaBeneficios()
        {

           // var Conta = Intelbras.CRM2013.Domain.Servicos.RepositoryService.Conta(this.OrganizationName, this.IsOffline).ObterPor(new Guid("A345F1D7-62B0-E311-9207-00155D013D19"));

        }

        [Test]
        public void ListarContasParticipantesMAtrizEFilialTeste()
        {
            var contaRepository = RepositoryFactory.GetRepository<IConta<Conta>>(this.OrganizationName, this.IsOffline);

            var contas = contaRepository.ListarContasParticipantesMAtrizEFilial();
        }

        [Test]
        public void ListarContasParticipantesTeste()
        {
            var contaRepository = RepositoryFactory.GetRepository<IConta<Conta>>(this.OrganizationName, this.IsOffline);

            var unidadeDeNegocio = new Guid("");
            //var contas = contaRepository.ListarContasParticipantes(, null);
            
        }
    }
}
