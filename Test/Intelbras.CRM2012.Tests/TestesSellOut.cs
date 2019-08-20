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

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class TestesSellOut : Base
    {
        [Test]
        public void TestarProdutoSellOut()
        {
            string resposta = string.Empty;
            var xmlroot = new XDocument(
            new XDeclaration("1.0", "utf-8", "no"),
            new XElement("Distribuidor",
                new XElement("Iddistribuidorcrm","E53CBD6F-8E9D-E311-888D-00155D013E2F"),
                new XElement("Iddistribuidorerp","321321"),
                new XElement("Statuscode","3"),
                new XElement("Statecode","2")
                //new XElement("Criado_em","30/06/2014 14:10:00")
                ));
            //string xml = "<Statuscode>3</Statuscode><Statecode>2</Statecode><Criado_em xsi:nil=\"true\" /></Distribuidor>";
            //SellOutTestWS.SelloutWS test = new SellOutTestWS.SelloutWS();
            //string respost = string.Empty;
            //bool resp = test.EnvioSellout("usuario", "senha", "xml", out resposta);

            string xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString(); ;

            var resultado = new Domain.Servicos.SellOutService(this.OrganizationName,
            this.IsOffline).MudarStatusDistribuidor("usuario", "senha", xml, out resposta);

        }

        [Test]
        public void TestarSellOut()
        {
            //4457b34c-b1c8-e411-bfbc-00155d013e80
            Guid arquivoGuid;
            string GuidEntidade = "4457b34c-b1c8-e411-bfbc-00155d013e80";
            int status = 993520001;
            string resposta;
            
            try
            {
                if (!Guid.TryParse(GuidEntidade, out arquivoGuid))
                    throw new ArgumentException("Guid em formato inválido.");

                Domain.Model.ArquivoDeSellOut arquivoSellout = new Domain.Model.ArquivoDeSellOut(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);
                arquivoSellout.Status = status;
                arquivoSellout.ID = arquivoGuid;
                arquivoSellout.DataDeProcessamento = DateTime.Now;
                arquivoSellout = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeSellOutServices(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false).Persistir(arquivoSellout);
                if (arquivoSellout == null)
                    throw new ArgumentException("Não foi possível atualizar o status do SellOut");

                resposta = "";
            }
            catch (Exception e)
            {
                resposta = e.Message;
            }
        }
    }
}
