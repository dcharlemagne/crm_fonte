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
    public class TestesChange038 : Base
    {

        
        [Test]
        public void TesteMsg0086()
        {
           

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Brasil,SC</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0086</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0086>");
            sb.AppendLine("      <Estado>Brasil,SC</Estado>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>      ");
            sb.AppendLine("    </MSG0086>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }


        [Test]
        public void TesteMsg0087()
        {


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Brasil,SC</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0087</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0087>");
            sb.AppendLine("      <Estado>Brasil,SC</Estado>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <CodigoUnidadeNegocio>SEC</CodigoUnidadeNegocio>");
            sb.AppendLine("    </MSG0087>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }


        [Test]
        public void TesteMsg0141()
        {


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>d66a88b1-bc0d-e411-9420-00155d013d39</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>numero operacao</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0141</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0141>");
            sb.AppendLine("      <CodigoConta>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("      <PassivelSolicitacao>true</PassivelSolicitacao>");
            sb.AppendLine("      <PossuiControleContaCorrente>993520000</PossuiControleContaCorrente>");
            sb.AppendLine("    </MSG0141>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

    }
}
