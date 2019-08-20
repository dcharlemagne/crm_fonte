using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.Message.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SDKore.Configuration;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;


namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    public class TestesLeandro : Base
    {
        

        
        [Test]
        public void TestarMsgAcessoExtranetCreate()
        {
            CompromissosDoCanal comp = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(this.OrganizationName, this.IsOffline).BuscarPorGuid(new Guid("AF9198F9-530C-E411-9420-00155D013D39"));

            new CompromissosDoCanalService(this.OrganizationName, this.IsOffline).AtualizarBeneficiosECompromissosCascata(comp);
        }



        



    }
}
