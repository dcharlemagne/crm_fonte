using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    public class TestesMunicipio : Base
    {
        [TestMethod]
        public void Teste_ObterIbgeViewModelPor_codigoIbge()
        {
            int codigoIbge = 1;

            var IbgeViewModel = new Domain.Servicos.RepositoryService(OrganizationName, IsOffline)
                                                   .Municipio.ObterIbgeViewModelPor(codigoIbge);
        }
    }
}