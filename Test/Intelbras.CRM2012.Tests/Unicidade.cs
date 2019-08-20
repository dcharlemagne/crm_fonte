using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class Unicidade : Base
    {
        public Unicidade()
        {
            SDKore.DomainModel.RepositoryFactory.SetTag(this.OrganizationName);
        }

        #region Lista PSD

        [Test]
        public void ListaPSD_SemNenhumRegistroParaUnidade()
        {
            var psd = new Domain.Model.ListaPrecoPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.UnidadeNegocio = new SDKore.DomainModel.Lookup { Id = new Guid("04F29237-1F9E-E311-888D-00155D013E2E"), Name = "INET" };
            psd.DataInicio = DateTime.Today.AddDays(-35);
            psd.DataFim = DateTime.Today;

            var resultado = new Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd,new List<Guid>());
            if (resultado)
                Assert.Fail("Já existe Lista PSD com unidade e datas informadas.");

        }

        [Test]
        public void ListaPSD_ValidarUnidadeICORP_Inicio_Fim_Entre_Janeiro_Junho()
        {
            var psd = new Domain.Model.ListaPrecoPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.UnidadeNegocio = new SDKore.DomainModel.Lookup { Id = new Guid("00F29237-1F9E-E311-888D-00155D013E2E"), Name = "ICORP" };
            psd.DataInicio = DateTime.Today.AddDays(-35);
            psd.DataFim = DateTime.Today;

            var resultado = new Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd, new List<Guid>());
            Assert.AreEqual(resultado, false, "Já existe Lista PSD com unidade e datas informadas.");
        }

        [Test]
        public void ListaPSD_ValidarUnidadeICORP_Inicio_Entre_Janeiro_Junho()
        {
            var psd = new Domain.Model.ListaPrecoPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.UnidadeNegocio = new SDKore.DomainModel.Lookup { Id = new Guid("00F29237-1F9E-E311-888D-00155D013E2E"), Name = "ICORP" };
            psd.DataInicio = DateTime.Today.AddDays(-35);
            psd.DataFim = DateTime.Parse("01/07/2014", System.Globalization.CultureInfo.CreateSpecificCulture("pt-Br"));

            var resultado = new Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd, new List<Guid>());
            Assert.AreEqual(resultado, false, "Já existe Lista PSD com unidade e datas informadas.");
        }

        [Test]
        public void ListaPSD_ValidarUnidadeICORP_Fim_Entre_Janeiro_Junho()
        {
            var psd = new Domain.Model.ListaPrecoPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.UnidadeNegocio = new SDKore.DomainModel.Lookup { Id = new Guid("00F29237-1F9E-E311-888D-00155D013E2E"), Name = "ICORP" };
            psd.DataInicio = DateTime.Today.AddYears(-1);
            psd.DataFim = DateTime.Parse("01/06/2014", System.Globalization.CultureInfo.CreateSpecificCulture("pt-Br"));

            var resultado = new Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd, new List<Guid>());
            Assert.AreEqual(resultado, false, "Já existe Lista PSD com unidade e datas informadas.");
        }

        [Test]
        public void ListaPSD_ValidarUnidadeICORP_Inicio_Fim_Apos_Junho()
        {
            var psd = new Domain.Model.ListaPrecoPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.UnidadeNegocio = new SDKore.DomainModel.Lookup { Id = new Guid("00F29237-1F9E-E311-888D-00155D013E2E"), Name = "ICORP" };
            psd.DataInicio = DateTime.Parse("01/07/2014", System.Globalization.CultureInfo.CreateSpecificCulture("pt-Br"));
            psd.DataFim = DateTime.Parse("30/07/2014", System.Globalization.CultureInfo.CreateSpecificCulture("pt-Br"));

            var resultado = new Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd, new List<Guid>());
            Assert.AreEqual(resultado, false, "Já existe Lista PSD com unidade e datas informadas.");
        }

        [Test]
        public void ListaPSD_ValidarAtualizacao()
        {
            var psd = new Domain.Model.ListaPrecoPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.ID = new Guid("62A30090-5DFD-E311-91F9-00155D013E44");
            psd.UnidadeNegocio = new SDKore.DomainModel.Lookup { Id = new Guid("00F29237-1F9E-E311-888D-00155D013E2E"), Name = "ICORP" };
            psd.DataInicio = DateTime.Parse("01/05/2014", System.Globalization.CultureInfo.CreateSpecificCulture("pt-Br"));
            psd.DataFim = DateTime.Parse("30/07/2014", System.Globalization.CultureInfo.CreateSpecificCulture("pt-Br"));

            var resultado = new Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd, new List<Guid>());
            Assert.AreEqual(resultado, false, "Já existe Lista PSD com unidade e datas informadas.");
        }

        #endregion

        #region Produto Lista PSD

        [Test]
        public void ProdutoListaPSD_Novo_Produto_Unico()
        {
            var psd = new Domain.Model.ProdutoListaPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.PSD = new SDKore.DomainModel.Lookup { Id = new Guid("62A30090-5DFD-E311-91F9-00155D013E44") }; //Teste Salles
            psd.Produto = new SDKore.DomainModel.Lookup { Id = new Guid("1B31BD54-ABDC-E311-88A2-00155D013E44") };
            psd.PSDControlado = true;

            //var resultado = null; //new Domain.Servicos.ProdutoListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd);
            //if (resultado)
            //    Assert.Fail("Erro. O produto não deveria existir.");
            //else
            //    Assert.Pass("Scuesso.");

        }

        [Test]
        public void ProdutoListaPSD_PRoduto_Existente()
        {
            var psd = new Domain.Model.ProdutoListaPSDPPPSCF(this.OrganizationName, this.IsOffline);
            psd.PSD = new SDKore.DomainModel.Lookup { Id = new Guid("62A30090-5DFD-E311-91F9-00155D013E44") }; //Teste Salles
            psd.Produto = new SDKore.DomainModel.Lookup { Id = new Guid("5A360BBD-90DC-E311-88A2-00155D013E44") };
            psd.PSDControlado = true;

            //var resultado = null; //new Domain.Servicos.ProdutoListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(psd);
            //if (resultado)
            //    Assert.Pass("Scuesso.");
            //else
            //    Assert.Fail("Erro. O produto deveria existir.");

        }


        //

        #endregion
    }
}
