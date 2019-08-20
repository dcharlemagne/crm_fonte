using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    public class TestesSefaz : Base
    {
        [TestMethod]
        public void Teste_Enviar_MSG0164()
        {
            var msg0164 = new Domain.Integracao.MSG0164(OrganizationName, IsOffline);
            var sefazViewModel = new Domain.ViewModels.SefazViewModel()
            {
                CNPJ = "07844260000192",
                UF = "SC"
            };

            var resltado = msg0164.Enviar(sefazViewModel);
        }
    }
}