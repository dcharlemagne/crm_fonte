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


namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class TestesIntegracaoCrm4 : Base
    {

        
        [Test]
        public void IntegrarConta()
        {
           

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            Domain.Model.Conta contaTeste = new Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline).BuscaConta(new Guid("14C0A4F9-4A4E-E411-9424-00155D013D3A"));

            //var retorno = new Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline).IntegracaoCrm4(contaTeste.CpfCnpj, contaTeste.CodigoMatriz, contaTeste.ID.Value.ToString());

        }


        [Test]
        public void IntegrarContato()
        {

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            //Domain.Model.Contato contatoTeste = new Domain.Servicos.ContatoService(this.OrganizationName, this.IsOffline).BuscaContato(new Guid("14C0A4F9-4A4E-E411-9424-00155D013D3A"));

            //var retorno = new Domain.Servicos.ContatoService(this.OrganizationName, this.IsOffline).IntegracaoCrm4("121.203.781-26", "teste");

        
        }



        

    }
}
