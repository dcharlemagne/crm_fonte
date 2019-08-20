using System;
using NUnit.Framework;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Domain;
using System.Web;
using System.Web.Services;
using Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using System.Collections.Generic;
using SDKore.Configuration;
using System.Text;
using System.Security.Cryptography;
using Intelbras.CRM2013.Domain.Model;
using System.ServiceModel;
using System.Xml.Linq;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class TestesBeneficioDoCanalService : Base
    {
        [Test]
        public void TesteAdesaoAoProgramaTodosOsCanais()
        {
            var beneficioDoCanalService = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, this.IsOffline);

            beneficioDoCanalService.AdesaoAoProgramaTodosOsCanais();
        }

        [Test]
        public void TesteDescredenciamentoAoPrograma()
        {
            var beneficioDoCanalService = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, this.IsOffline);
            var canalService = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline);

            var canal = canalService.BuscaConta(new Guid("E333FE1D-A60E-E411-9408-00155D013D38"));

            beneficioDoCanalService.DescredenciamentoAoPrograma(canal);
        }

        
    }
}