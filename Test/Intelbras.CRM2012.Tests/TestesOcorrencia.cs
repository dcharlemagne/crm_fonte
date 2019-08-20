using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System.DirectoryServices.AccountManagement;
using System.ServiceModel.Description;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    [TestFixture]
    public class TestesOcorrencia : Base
    {
        [TestMethod]
        public void TestCalculoSLAPorOcorrencia()
        {
            var service = new Domain.Servicos.OcorrenciaService(this.OrganizationName, this.IsOffline);
            // new Guid("8C69D160-7F68-E711-80CB-0050568DED44")
            var ocorrenciaTest = (new Domain.Servicos.RepositoryService()).Ocorrencia.ObterPor("OCOR-11603-R8F2L6");
            service.Ocorrencia = ocorrenciaTest;

            service.Atualizar();
        }

        [TestMethod]
        public void TestEnvioAenxosOcorrencia()
        {
            var service = new Domain.Servicos.OcorrenciaService(this.OrganizationName, this.IsOffline);
            // new Guid("8C69D160-7F68-E711-80CB-0050568DED44")
            var ocorrenciaTest = (new Domain.Servicos.RepositoryService()).Ocorrencia.ObterPor("OCOR-917798-N8T5H0");
            var result = service.IntegracaoBarramento(ocorrenciaTest);            
        }

        [TestMethod]
        public void TestEnvioOcorrenciaASTEC()
        {
            var service = new Domain.Servicos.OcorrenciaService(this.OrganizationName, this.IsOffline);
            // new Guid("8C69D160-7F68-E711-80CB-0050568DED44")
            var ocorrenciaTest = (new Domain.Servicos.RepositoryService()).Ocorrencia.ObterPor("OCOR-918578-M2D7N1");
            var result = service.IntegracaoBarramento(ocorrenciaTest);
        }
    }
}
