using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    public class TestesProdutoKit
    {
        private RepositoryService RepositoryService = new RepositoryService();

        [TestMethod]
        public void ProdutoKit_ListarPorProduto_codigoProdutoPai()
        {
            string codigoProdutoPai = "4612026";
            List<Domain.Model.ProdutoKit> lista = RepositoryService.ProdutoKit.ListarPorProdutoPai(codigoProdutoPai);

            Assert.IsFalse(lista.Count > 0, "Nenhum registro encontrado para o Kit.");
        }
    }
}
