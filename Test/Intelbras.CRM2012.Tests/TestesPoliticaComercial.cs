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
using System.Xml.Linq;
using Intelbras.CRM2013.DAL;
using System.IO;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class TestesPoliticaComercial : Base
    {

        [Test]
        public void testarPortfolio()
        {
            Domain.Model.Portfolio portfolio = new Intelbras.CRM2013.Domain.Servicos.PortfolioService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("0F7A6441-AEF0-E311-9420-00155D013D39"));
            Domain.Model.Product produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("08BC0401-BEF0-E311-9420-00155D013D39"));
            Domain.Model.ProdutoPortfolio ProdPortfolio= new Domain.Model.ProdutoPortfolio(this.OrganizationName, this.IsOffline);
            ProdPortfolio.Produto = new Lookup(produto.ID.Value, "");
            ProdPortfolio.Portfolio = new Lookup(portfolio.ID.Value,"");

            new Intelbras.CRM2013.Domain.Servicos.PortfolioService(this.OrganizationName, this.IsOffline).VerificaVinculoProdutoVsProdutoPortifolio(ProdPortfolio);

            new Intelbras.CRM2013.Domain.Servicos.PortfolioService(this.OrganizationName, this.IsOffline).VerificaVinculoPortifolio(ProdPortfolio);
        }
        [Test]
        public void testarDuplicidade()
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.PoliticaComercial politica = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(organizationName, false).ObterPor(new Guid("039F0F9A-56B1-E411-BFBC-00155D013E80"));

            List<Guid> lstPoliticaEstado = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(this.OrganizationName, this.IsOffline).ListarEstadosDaPoliticaComercial(politica.ID.Value);
            
            List<Guid> lstPoliticaCanais = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(this.OrganizationName, this.IsOffline).ListarCanaisDaPoliticaComercial(politica.ID.Value);

            bool resposta,resposta2;
            if (lstPoliticaEstado.Count > 0)
                resposta = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(this.OrganizationName, this.IsOffline).VerificarDuplicidadePoliticaRegistros(politica, lstPoliticaEstado, "estado",false);
            if (lstPoliticaCanais.Count > 0)
                resposta2 = new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(this.OrganizationName, this.IsOffline).VerificarDuplicidadePoliticaRegistros(politica, lstPoliticaCanais, "conta",false);

            if (lstPoliticaEstado.Count == 0 && lstPoliticaCanais.Count == 0)
                new Intelbras.CRM2013.Domain.Servicos.PoliticaComercialService(this.OrganizationName, this.IsOffline).VerificarExistenciaPoliticaComercial(politica);
        }

    }
}
