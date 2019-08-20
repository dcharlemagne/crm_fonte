using Intelbras.CRM2013.Application.AtualizarAtendimentoChat.Model;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Intelbras.Message.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using NUnit.Framework;
using SDKore.Configuration;
using SDKore.Crm;
using SDKore.DomainModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Web.Services;
using System.Xml.Linq;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;


namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    public class Testes : Base
    {
        [TestMethod]
        public void TestBuscarProprietario()
        {
            var proprietario = (new CRM2013.Domain.Servicos.RepositoryService()).Usuario.BuscarProprietario("opportunity", "opportunityid", new Guid("6DC36630-261A-E711-80C2-0050568DB649"));
            if (proprietario != null)
            {
                var id = proprietario.Id.ToString();
                var nome = proprietario.NomeCompleto;
            }
        }

        [TestMethod]
        public void TestListarProdutosDeTabelaEspecificaPor()
        {
            var produtos = (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ListarProdutosDeTabelaEspecificaPor("3145", "Seg Eletronica - canais", "550", "101", "TEST1");
        }

        [TestMethod]
        public void DetalhesDaNotaFiscal()
        {
            var service = new PortalB2BNotaFiscal.NotaFiscalSoapClient();
            var detalhes = service.DetalhesDaNotaFiscal("104", "1", "0619380");
        }

        [TestMethod]
        public void ListarPorRevenda()
        {
            var lista2 = new CRM2013.Domain.Servicos.RepositoryService().HistoricoDistribuidor.ListarPorRevendaComDataFim(new Guid("939bdb3f-c330-e611-9430-0050568d63ab"), (DateTime.Parse("2017-10-18 3:00:00 AM")), (DateTime.Parse("2018-01-01 3:00:00 AM")));

            var lista = new CRM2013.Domain.Servicos.RepositoryService().HistoricoDistribuidor.ListarPorRevendaSemDataFim(new Guid("d07de600-814a-e511-941c-00155d014524"), (DateTime.Parse("2017-11-11 3:00:00 AM")));

        }

        [TestMethod]
        public void ListarRelacionamentoUnidadeNegocioBenef()
        {

            var lista = new CRM2013.Domain.Servicos.RepositoryService().UnidadeNegocio.ObterRelacionamentoUnidadeNegocioBenef("ACE");
        }

        public void CancelarDaNotaFiscal()
        {
            new Domain.Servicos.PedidoService(OrganizationName, IsOffline).CancelarNotaFiscal("123456", "1", "104");
        }

        [TestMethod]
        public void ObterOutraContaPorCpfCnpj()
        {
            new Domain.Servicos.ContaService(OrganizationName, IsOffline).BuscaOutraContaPorCpfCnpj("11.755.926/0001-03", new Guid("61BFFAAE-3F56-E111-9A58-00155D013801"), 993520001);
        }

        [TestMethod]
        public void ConsultaOS()
        {
            string[] status = { "200035" };
            Conta conta = new CRM2013.Domain.Servicos.ContaService(OrganizationName, IsOffline).BuscaConta(new Guid("E0B4423B-E7F8-E411-9418-00155D01421E"));
            string[] origem = { "200004", "200006" };

            var lista = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarPorStatus(DateTime.Parse("2017-07-04"), DateTime.Parse("2017-07-11"), status, conta, origem);
        }


        [TestMethod]
        public void ObtemReduntanteASTEC()
        {
            string ocorrencia = "5AFA3780-4F0C-E811-80CF-005056AA4874";
            Ocorrencia ocorrencias = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrencia));
            new CRM2013.Domain.Servicos.RepositoryService().Ocorrencia.ObtemReduntanteASTEC(ocorrencias);
        }

        [TestMethod]
        public void BuscarCategoriaDoCanal()
        {
            RepositoryService repoService = new RepositoryService("CRM2013H", false);
            var teste = repoService.AcessoKonviva.Retrieve(new Guid(""));
            //CategoriaCanalService servico = new CategoriaCanalService("CRM2013D", false);
            //ContaService serviceConta = new ContaService("CRM2013D", false);
            //servico.ObterCategoriaPrincipalDoCanal(serviceConta.BuscaConta(new Guid("0C33C6F8-D800-E411-9420-00155D013D39")));

            //AcessoKonvivaService acKonvivaService = new AcessoKonvivaService("CRM2013H", false);

            //var teste = acKonvivaService.ObterPorContato(new Guid("8D355D95-E977-E511-9415-0050568D4EB2"));

            //DeParaDeUnidadeDoKonvivaService deParaService = new DeParaDeUnidadeDoKonvivaService("CRM2013H", false);
            //var teste = deParaService.ObterUnidadeKonvivaDeParaCom(null, new Contato("CRM2013H", false)
            //{
            //    PapelCanal = 993520005,
            //    TipoRelacao = 993520000,
            //    ID = new Guid("8D355D95-E977-E511-9415-0050568D4EB2")
            //});
        }
        [TestMethod]
        public void ConsultaConta()
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarPorCampos("225263", "", "", "", "", new Guid("7648A738-D76C-E711-80CB-0050568DED44"));
        }

        [TestMethod]
        public void ConsultaOcorrencia()
        {
            string[] status = { "1", "2", "3" };
            string[] statusDiagnostico = { "1", "2", "3" };
            Conta conta = new Conta();
            string[] origem = { "1", "2", "3" };

            (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarPorStatusDiagnostico(DateTime.Now, DateTime.Now, status, statusDiagnostico, conta, origem);
        }

        //[TestMethod]
        //public void RepOcorrencia()
        //{
        //    Conta conta = new Conta();
        //    conta.Id = new Guid("e0b4423b-e7f8-e411-9418-00155d01421e");
        //    Estabelecimento estabelecimento = new Estabelecimento();

        //    (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.GerarLogisticaReversa(conta, estabelecimento);
        //}

        [TestMethod]
        public void Konviva()
        {

            string msg = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<MENSAGEM>
  <CABECALHO>
    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>
    <NumeroOperacao>PSD DISTRIBUIDOR IACCS - DF</NumeroOperacao>
    <CodigoMensagem>MSG0087</CodigoMensagem>
  </CABECALHO>
  <CONTEUDO>
    <MSG0087>
      <CodigoListaPreco>0d7dd5e7-78c2-e411-942c-00155d013d5d</CodigoListaPreco>
      <CodigoUnidadeNegocio>ACE</CodigoUnidadeNegocio>
      <Estados>
        <Estado>Brasil,DF</Estado>
      </Estados>
      <DataInicioVigencia>2015-08-01</DataInicioVigencia>
      <DataFinalVigencia>2015-10-01</DataFinalVigencia>
      <Situacao>0</Situacao>
      <ListaPrecoItens>
        <ListaPrecoItem>
          <CodigoProduto>4565205</CodigoProduto>
          <ValorPSD>180.3000</ValorPSD>
          <PSDControlado>true</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563815</CodigoProduto>
          <ValorPSD>1348.2500</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007607</CodigoProduto>
          <ValorPSD>29.9900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4561800</CodigoProduto>
          <ValorPSD>28.6900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007159</CodigoProduto>
          <ValorPSD>222.4000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007160</CodigoProduto>
          <ValorPSD>217.9700</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563806</CodigoProduto>
          <ValorPSD>191.4000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007304</CodigoProduto>
          <ValorPSD>74.3400</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4564808</CodigoProduto>
          <ValorPSD>85.2100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007305</CodigoProduto>
          <ValorPSD>246.9400</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4564800</CodigoProduto>
          <ValorPSD>173.4500</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563802</CodigoProduto>
          <ValorPSD>129.4100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4395010</CodigoProduto>
          <ValorPSD>3.9000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563804</CodigoProduto>
          <ValorPSD>185.8700</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563805</CodigoProduto>
          <ValorPSD>193.6200</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563807</CodigoProduto>
          <ValorPSD>194.7200</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4564809</CodigoProduto>
          <ValorPSD>17.9900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563814</CodigoProduto>
          <ValorPSD>1912.1300</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810012</CodigoProduto>
          <ValorPSD>1249.9000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810013</CodigoProduto>
          <ValorPSD>673.7900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810014</CodigoProduto>
          <ValorPSD>444.0400</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810015</CodigoProduto>
          <ValorPSD>369.4100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810016</CodigoProduto>
          <ValorPSD>279.2300</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810017</CodigoProduto>
          <ValorPSD>79.2100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810021</CodigoProduto>
          <ValorPSD>54.2200</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007159</CodigoProduto>
          <ValorPSD>222.4000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007160</CodigoProduto>
          <ValorPSD>217.9700</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007304</CodigoProduto>
          <ValorPSD>74.3400</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007305</CodigoProduto>
          <ValorPSD>246.9400</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4007607</CodigoProduto>
          <ValorPSD>29.9900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4395010</CodigoProduto>
          <ValorPSD>3.9000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4561800</CodigoProduto>
          <ValorPSD>28.6900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563802</CodigoProduto>
          <ValorPSD>129.4100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563804</CodigoProduto>
          <ValorPSD>185.8700</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563805</CodigoProduto>
          <ValorPSD>193.6200</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563806</CodigoProduto>
          <ValorPSD>191.4000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563807</CodigoProduto>
          <ValorPSD>194.7200</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563814</CodigoProduto>
          <ValorPSD>1912.1300</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4563815</CodigoProduto>
          <ValorPSD>1348.2500</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4564800</CodigoProduto>
          <ValorPSD>173.4500</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4564808</CodigoProduto>
          <ValorPSD>85.2100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4564809</CodigoProduto>
          <ValorPSD>17.9900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810012</CodigoProduto>
          <ValorPSD>1249.9000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810013</CodigoProduto>
          <ValorPSD>673.7900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810014</CodigoProduto>
          <ValorPSD>444.0400</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810015</CodigoProduto>
          <ValorPSD>369.4100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810016</CodigoProduto>
          <ValorPSD>279.2300</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810017</CodigoProduto>
          <ValorPSD>79.2100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810021</CodigoProduto>
          <ValorPSD>54.2200</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4820000</CodigoProduto>
          <ValorPSD>16.7400</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4820001</CodigoProduto>
          <ValorPSD>45.2300</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4820002</CodigoProduto>
          <ValorPSD>61.4900</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4820003</CodigoProduto>
          <ValorPSD>73.7200</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4820004</CodigoProduto>
          <ValorPSD>125.5100</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
        <ListaPrecoItem>
          <CodigoProduto>4810024</CodigoProduto>
          <ValorPSD>76.7000</ValorPSD>
          <PSDControlado>false</PSDControlado>
          <ValorPP>0.0000</ValorPP>
          <ValorPSCF>0.0000</ValorPSCF>
        </ListaPrecoItem>
      </ListaPrecoItens>
    </MSG0087>
  </CONTEUDO>
</MENSAGEM>";

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, msg, out teste);

        }

        [Test]
        public void aTesteCategoriasDesativar()
        {

            CategoriasCanal cat = new Intelbras.CRM2013.Domain.Servicos.CategoriaCanalService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("AF9198F9-530C-E411-9420-00155D013D39"));

            new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, this.IsOffline).AdesaoAoProgramaRemoveCategoria(cat);

        }

        [Test]
        public void aTesteCategoriasAtivar()
        {

            CategoriasCanal cat = new Intelbras.CRM2013.Domain.Servicos.CategoriaCanalService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("5C9E970A-5F81-E411-8E33-00155D013E80"));

            new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(this.OrganizationName, this.IsOffline).AdesaoAoProgramaNovaCategoria(cat);

        }

        [Test]
        public void aTesteConsultaPSD()
        {
            ListaPrecoPSDPPPSCF PsdPreco = new Intelbras.CRM2013.Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("F8FD3355-087F-E411-8E33-00155D013E80"));
            List<Guid> lstEstados = new List<Guid>();
            bool resposta = new Intelbras.CRM2013.Domain.Servicos.ListaPSDService(this.OrganizationName, this.IsOffline).ValidarExistencia(PsdPreco, lstEstados);

        }
        [Test]
        public void CriarPastaSharePoint()
        {

            //new Intelbras.CRM2013.Domain.Servicos.SharepointServices(OrganizationName, IsOffline).CriarDiretorioSharepoint();

        }
        [Test]
        public void PortfolioKARepresentante()
        {
            Domain.Model.Usuario imagemPre = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("B56E2663-7CEE-E311-9407-00155D013D38"));
            Domain.Model.Usuario usuario = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("B56E2663-7CEE-E311-9407-00155D013D38"));
            usuario.CodigoSupervisorEMS = "11";
            usuario.CodigoAssistenteComercial = 13;


            //Verificamos se o campo nao esta sendo preenchido pela primeira vez,pois ai nao teria nada para atualizar
            if (!String.IsNullOrEmpty(imagemPre.CodigoSupervisorEMS) || imagemPre.CodigoAssistenteComercial.HasValue)
            {
                List<Domain.Model.PortfoliodoKeyAccountRepresentantes> lstPortfKARepre = new Intelbras.CRM2013.Domain.Servicos.PortfoliodoKeyAccountRepresentantesService(this.OrganizationName, this.IsOffline).ListarPortfolioKARepresentantes(imagemPre.CodigoAssistenteComercial, imagemPre.CodigoSupervisorEMS);
                foreach (Domain.Model.PortfoliodoKeyAccountRepresentantes portfolioKARepresentante in lstPortfKARepre)
                {
                    new Intelbras.CRM2013.Domain.Servicos.PortfoliodoKeyAccountRepresentantesService(this.OrganizationName, this.IsOffline).IntegracaoBarramento(portfolioKARepresentante);
                }
            }
        }
        [Test]
        public void testeSharepoint()
        {
            MSG0114 tst = new MSG0114(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0114")
            {
                TipoObjeto = "itbc_solicitacaodebeneficio",
                CodigoEntidade = "7127d84b-583e-e411-803a-00155d013e44"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);

        }
        [Test]
        public void Konviva103()
        {
            MSG0103 tst = new MSG0103(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0103")
            {
                CodigoTreinamento = "CCAL",
                ModalidadeTreinamento = "modalidade teste",
                NomeTreinamento = "treinamento teste",
                IdentificadorTreinamento = 2,
                CategoriaTreinamento = "",
                Situacao = 0
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);

        }

        [Test]
        public void Konviva104()
        {
            MSG0104 tst = new MSG0104(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0104")
            {
                CodigoConta = "d66a88b1-bc0d-e411-9420-00155d013d39",
                IdentificadorTurma = 2,
                DataInicio = DateTime.Now,
                DataTermino = DateTime.Now,
                NomeTurma = "Turma de teste",
                Situacao = 0,
                Proprietario = "6de6c975-3aeb-e311-9407-00155d013d38",
                TipoProprietario = "systemuser"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);

        }

        [Test]
        public void EnviarKonviva105()
        {
            Intelbras.CRM2013.Domain.Model.AcessoKonviva acess = new AcessoKonviva(this.OrganizationName, this.IsOffline);
            acess.Conta = new SDKore.DomainModel.Lookup(new Guid("d66a88b1-bc0d-e411-9420-00155d013d39"), "");
            acess.Contato = new SDKore.DomainModel.Lookup(new Guid("1325b7ee-82ed-e311-9420-00155d013d39"), "");
            acess.ID = new Guid("0E77858D-D374-E411-8E33-00155D013E80");
            acess.Nome = "Gabriel Dias Junckes";
            acess.PerfilAdministrador = false;
            acess.PerfilAluno = true;
            acess.PerfilAnalista = false;
            acess.PerfilAutor = false;
            acess.PerfilGestor = false;
            acess.PerfilInstrutor = false;
            acess.PerfilModerador = false;
            acess.PerfilMonitor = false;
            acess.PerfilTutor = false;
            acess.Status = 0;
            acess.Status = 1;
            acess.UnidadeKonviva = new SDKore.DomainModel.Lookup(new Guid("6B6BAE1C-4864-E411-A6A3-00155D013D51"), "");
            string resposta = new Intelbras.CRM2013.Domain.Integracao.MSG0105(this.OrganizationName, this.IsOffline).Enviar(acess);

        }

        [Test]
        public void EnviarKonviva105Gabriel()
        {
            MSG0105 tst = new MSG0105(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0105")
            {
                //CodigoAcessoKonviva = "51e421e9-237a-e411-8e33-00155d013e80",
                NomeAcessoKonviva = "Teste kon",
                CodigoContato = "3d7ae870-2d0c-e411-9420-00155d013d39",
                //CodigoConta = "d66a88b1-bc0d-e411-9420-00155d013d39",
                PerfilAluno = true,
                PerfilGestor = true,
                PerfilAutor = false,
                PerfilAdministrador = false,
                PerfilModerador = false,
                PerfilMonitor = false,
                PerfilInstrutor = true,
                PerfilAnalista = false,
                PerfilTutor = false,
                Situacao = 0,
                Proprietario = "6de6c975-3aeb-e311-9407-00155d013d38",
                TipoProprietario = "systemuser"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void Konviva105()
        {
            MSG0105 tst = new MSG0105(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0105")
            {
                CodigoAcessoKonviva = "51e421e9-237a-e411-8e33-00155d013e80",
                NomeAcessoKonviva = "Gabriel Dias Junckes",
                CodigoContato = "1325b7ee-82ed-e311-9420-00155d013d39",
                CodigoConta = "d66a88b1-bc0d-e411-9420-00155d013d39",
                PerfilAluno = true,
                PerfilGestor = false,
                PerfilAutor = false,
                PerfilAdministrador = false,
                PerfilModerador = false,
                PerfilMonitor = false,
                PerfilInstrutor = false,
                PerfilAnalista = false,
                PerfilTutor = false,
                Situacao = 0,
                Proprietario = "6de6c975-3aeb-e311-9407-00155d013d38",
                TipoProprietario = "systemuser"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);

        }

        [Test]
        public void Konviva106()
        {
            MSG0106 tst = new MSG0106(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0106")
            {

                DataConclusao = DateTime.Parse("2014-12-03"),
                DataValidade = DateTime.Parse("2015-03-03"),
                StatusAproveitamento = 1,
                IdentificadorMatricula = 380,
                IdentificadorTreinamento = 99,
                CodigoContato = "b538e466-097b-e411-8e33-00155d013e80",
                //CodigoConta = "d66a88b1-bc0d-e411-9420-00155d013d39",
                Situacao = 0,
                Proprietario = "259A8E4F-15E9-E311-9420-00155D013D39",
                TipoProprietario = "systemuser"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);

        }

        [Test]
        public void msg160Teste()
        {
            MSG0160 tst = new MSG0160(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0160")
            {

            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        //[Test]
        //public void msg159Teste() Mudar Willer - Alterar pra busca seguindo alógica de múltiplos itens.
        //{
        //    MSG0159 tst = new MSG0159(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0159")
        //    {
        //        CodigoBeneficioCanal = "0a6bfdc1-e33d-e411-9421-00155d013d39",
        //        VerbaReembolsada = (decimal)0.0000,
        //        VerbaCalculadaPeriodoAtual = (decimal)7929.4400

        //    };
        //    Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
        //    String teste = String.Empty;

        //    integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        //}
        [Test]
        public void testaContaKC()
        {
            try
            {
                var t = new CRM2013.Domain.Servicos.ContaService(OrganizationName, IsOffline).BuscaConta(new Guid("20EE3D76-E7CA-E311-BB3D-00155D013E44"));


            }
            catch (FaultException<Microsoft.Xrm.Sdk.DiscoveryServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.DiscoveryServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.DiscoveryServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
        }

        [Test]
        public void TestarPortadorCreate()
        {
            MSG0024 tst = new MSG0024(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0024")
            {
                CodigoPortador = 444,
                Nome = "jacob2",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        [Test]
        public void TestarPortadorUpdate()
        {
            MSG0024 tst = new MSG0024(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0024")
            {
                CodigoPortador = 2223,
                Nome = "PortadorTeste3",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarIndiceCreate()
        {
            MSG0046 tst = new MSG0046(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0046")
            {
                ChaveIntegracao = "3333",
                Indice = (decimal)33.33,
                Nome = "Indice Guilhermino",
                NumeroDias = "30",
                TabelaFinanciamento = 33333,
                Situacao = 0
            };

            string tst4 = "<?xml version=\"1.0\"?><MENSAGEM xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">  <CABECALHO>    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>    <NumeroOperacao>3</NumeroOperacao>    <CodigoMensagem>MSG0046</CodigoMensagem>    <LoginUsuario />  </CABECALHO>  <CONTEUDO>    <MSG0046>      <ChaveIntegracao>1;4</ChaveIntegracao>      <Indice>3.0</Indice>      <Nome>3</Nome>      <NumeroDias>4</NumeroDias>      <TabelaFinanciamento>1</TabelaFinanciamento>      <Situacao>0</Situacao>    </MSG0046>  </CONTEUDO></MENSAGEM>";


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst4, out teste);
        }

        [Test]
        public void TestarIndiceUpdate()
        {
            MSG0046 tst = new MSG0046(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0046")
            {
                ChaveIntegracao = "123123",
                Indice = (decimal)10.11,
                Nome = "Indice Messi",
                NumeroDias = "11",
                TabelaFinanciamento = 1231231234,
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        [Test]
        public void TestarMsgAcessoExtranetCreate()
        {
            //MSG0119 tst = new MSG0119(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0119")
            //{
            //    CodigoAcesso = "4E4A55CC-C3C0-E311-B194-00155D013E44",
            //    Situacao = 0,
            //    Nome = "Messi Esteve aqui",
            //    TipoAcesso = "1A002B53-5DA5-E311-888D-00155D013E2E",
            //    Contato = "29935831-2A9E-E311-888D-00155D013E2E",
            //    PerfilAcesso = "B6724984-949D-E311-888D-00155D013E2E",
            //    DataValidade = new DateTime(2014, 10, 01),
            //    Conta = "08F14E0E-4B9E-E311-888D-00155D013E2E",
            //    Proprietario = "BEE55B63-9AAE-E311-9207-00155D013D19",
            //    TipoProprietario = "systemuser"
            //};

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Acesso Básico para Matheus Barkmeyer</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0119</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>Claudiney</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0119>");
            sb.AppendLine("      <Nome>Acesso Básico para Matheus Barkmeyer</Nome>");
            sb.AppendLine("      <TipoAcesso>4c77db98-70ed-e311-9407-00155d013d38</TipoAcesso>");
            sb.AppendLine("      <Contato>7eef78ef-aeea-e411-bfbc-00155d013e80</Contato>");
            sb.AppendLine("      <PerfilAcesso>f9e3011d-71ed-e311-9407-00155d013d38</PerfilAcesso>");
            sb.AppendLine("      <Conta>6def78ef-aeea-e411-bfbc-00155d013e80</Conta>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <DataValidade>2049-12-31</DataValidade>");
            sb.AppendLine("      <Proprietario>259a8e4f-15e9-e311-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("    </MSG0119>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            //sb.AppendLine("<?xml version=\"1.0\"?>");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>Usuario Teste</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0119</CodigoMensagem>");
            //sb.AppendLine("    <LoginUsuario>Claudiney</LoginUsuario>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0119>");
            //sb.AppendLine("      <Nome>Usuario Teste</Nome>");
            //sb.AppendLine("      <TipoAcesso>eb37ac48-949d-e311-888d-00155d013e2e</TipoAcesso>");
            //sb.AppendLine("      <Contato>ec02ac22-91e5-e311-b278-00155d01330e</Contato>");
            //sb.AppendLine("      <PerfilAcesso>b6724984-949d-e311-888d-00155d013e2e</PerfilAcesso>");
            //sb.AppendLine("      <Conta>00695909-91e5-e311-b278-00155d01330e</Conta>");
            //sb.AppendLine("      <Situacao>0</Situacao>");
            //sb.AppendLine("      <DataValidade>2014-05-27</DataValidade>");
            //sb.AppendLine("      <Proprietario>ff3cbd6f-8e9d-e311-888d-00155d013e2e</Proprietario>");
            //sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("    </MSG0119>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }



        [Test]
        public void TestarMsgAcessoExtranetUpdate()
        {
            MSG0119 tst = new MSG0119(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0119")
            {
                CodigoAcesso = "4E4A55CC-C3C0-E311-B194-00155D013E44",
                Situacao = 0,
                Nome = "Messi Esteve aqui",
                TipoAcesso = "1A002B53-5DA5-E311-888D-00155D013E2F",
                Contato = "29935831-2A9E-E311-888D-00155D013E2E",
                PerfilAcesso = "B6724984-949D-E311-888D-00155D013E2E",
                DataValidade = new DateTime(2014, 10, 01),
                Conta = "08F14E0E-4B9E-E311-888D-00155D013E2E",
                Proprietario = "BEE55B63-9AAE-E311-9207-00155D013D19",
                TipoProprietario = "systemuser"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarConexao()
        {
            //MSG0131 tst = new MSG0131(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0131");
            //{
            //     Descricao = "descricao",
            //     DataInicio = new DateTime(2014, 10, 01),

            //     FuncaoPartePrincipal = "Representante Legal",

            //Situacao = 0,

            //DataTermino = new DateTime(2014, 10, 01),
            //     CodigoContato = "97EE2C9D-DCC0-E311-B194-00155D013E44",
            //     CodigoConta = "08F14E0E-4B9E-E311-888D-00155D013E2E",
            //     Proprietario = "BEE55B63-9AAE-E311-9207-00155D013D19",
            //TipoProprietario = "systemuser"
            //};

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            //sb.AppendLine("<MENSAGEM xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>5680dde2-63dc-e311-88a2-00155d013e44</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0131</CodigoMensagem>");
            //sb.AppendLine("    <LoginUsuario>Claudiney</LoginUsuario>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0131>");
            //sb.AppendLine("      <CodigoConexao>841EA2B6-96CC-E311-BB3D-00155D013E44</CodigoConexao>");
            //sb.AppendLine("      <FuncaoPartePrincipal>Representante Legal</FuncaoPartePrincipal>");
            //sb.AppendLine("      <Situacao>0</Situacao>");
            //sb.AppendLine("      <CodigoContato>FF3CBD6F-8E9D-E311-888D-00155D013E2E</CodigoContato>");
            //sb.AppendLine("      <CodigoConta>43A4A001-5CCA-E311-BB3D-00155D013E44</CodigoConta>");
            //sb.AppendLine("      <Proprietario>ff3cbd6f-8e9d-e311-888d-00155d013e2e</Proprietario>");
            //sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("    </MSG0131>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>5680dde2-63dc-e311-88a2-00155d013e44</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0131</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>Claudiney</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0131>");
            //sb.AppendLine("      <CodigoConexao></CodigoConexao>");//347e81ac-dfe1-e311-88a2-00155d013e44
            sb.AppendLine("      <FuncaoPartePrincipal>Representante Legal</FuncaoPartePrincipal>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <CodigoContato>605e5f4d-69dc-e311-88a2-00155d013e44</CodigoContato>");
            sb.AppendLine("      <CodigoConta>5680dde2-63dc-e311-88a2-00155d013e44</CodigoConta>");
            sb.AppendLine("      <Proprietario>ff3cbd6f-8e9d-e311-888d-00155d013e2e</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("    </MSG0131>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TestarMsgEmpresasColigadasCreate()
        {
            MSG0123 tst = new MSG0123(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0123")
            {
                Nome = "Empresa do Messi2",

                Conta = "08F14E0E-4B9E-E311-888D-00155D013E2E",

                CNPJ = "12345678",

                Nacionalidade = "Brasileira",

                PorcentagemCapital = (decimal)10.2,

                Situacao = 0,

                Proprietario = "BEE55B63-9AAE-E311-9207-00155D013D19",

                TipoProprietario = "systemuser"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        [Test]
        public void TestarMsgEmpresasColigadasUpdate()
        {
            MSG0123 tst = new MSG0123(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0123")
            {
                CodigoEmpresaColigada = "25CCF167-D4C3-E311-9BFF-00155D013E44",

                Nome = "Empresa do Messi-update",

                Conta = "08F14E0E-4B9E-E311-888D-00155D013E2E",

                CNPJ = "12345",

                Nacionalidade = "Brasileira",

                PorcentagemCapital = (decimal)10.66,

                Situacao = 0,

                //Proprietario = "BEE55B63-9AAE-E311-9207-00155D013D19",
                Proprietario = "B4E55B63-9AAE-E311-9207-00155D013D19",

                TipoProprietario = "systemuser"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [TestMethod]
        public void TestarMunicipio()
        {

            MSG0012 tst = new MSG0012(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0012")
            {
                Nome = "ALVORADA",
                Estado = "BRASIL,RS",
                ChaveIntegracao = "ALVORADA,RS,BRASIL",
                //CodigoIBGE = 4300604,
                Situacao = 0
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);
        }

        [Test]
        public void TestarEstadoCreate()
        {
            MSG0010 tst = new MSG0010(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0010")
            {
                ChaveIntegracao = "Guilhermino, GL",
                Sigla = "GL",
                Nome = "Guilhermino",
                Situacao = 0,
                Pais = "brasil"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        [Test]
        public void TestarEstadoUpdate()
        {
            MSG0010 tst = new MSG0010(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0010")
            {
                ChaveIntegracao = "Santa Catarina,SP",
                Sigla = "Sc",
                Nome = "Santa Catarina",
                Situacao = 1,
                RegiaoGeografica = "FB6198CA-66A5-E311-888D-00155D013E2E",
                Pais = "brasil"
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarPaisCreate()
        {
            MSG0006 tst = new MSG0006(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0006")
            {
                ChaveIntegracao = "GLR",
                Nome = "Guilherminolandia",
                Situacao = 0
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarPaisUpdate()
        {
            MSG0006 tst = new MSG0006(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0006")
            {
                ChaveIntegracao = "pais teste",
                Nome = "Pais Teste",
                Situacao = 0
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarTransportadoraCreate()
        {
            MSG0022 tst = new MSG0022(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0022")
            {
                CodigoTransportadora = 444,
                Nome = "Transportes Guilhermino2",
                NomeAbreviado = "Guilertran",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        [Test]
        public void TestarTransportadoraUpdate()
        {
            MSG0022 tst = new MSG0022(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0022")
            {
                CodigoTransportadora = 123,
                Nome = "Transportadora teste2",
                NomeAbreviado = "Transp.Te ",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarSubFamiliaProdutoCreate()
        {
            MSG0030 tst = new MSG0030(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0030")
            {
                CodigoSubFamilia = "22525",
                Nome = "SubGuilhermino",
                Familia = "2525",
                Situacao = 0
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarSubFamiliaProdutoUpdate()
        {
            MSG0030 tst = new MSG0030(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0030")
            {
                CodigoSubFamilia = "22525",
                Nome = "SubGuilhermino",
                Familia = "2525",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarNaturezaOperacaoCreate()
        {
            MSG0050 tst = new MSG0050(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0050")
            {
                CodigoNaturezaOperacao = "3333",
                Nome = "NatGuilhermino",
                Tipo = 993520000,
                Situacao = 0
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarNaturezaOperacaoUpdate()
        {
            MSG0050 tst = new MSG0050(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0050")
            {
                CodigoNaturezaOperacao = "123123",
                Nome = "Messi-test",
                Tipo = 993520001,
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarGrupoEstoqueCreate()
        {
            MSG0038 tst = new MSG0038(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0038")
            {
                CodigoGrupoEstoque = 33,
                Nome = "Guilhermino",
                Situacao = 0
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarGrupoEstoqueUpdate()
        {
            MSG0038 tst = new MSG0038(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0038")
            {
                CodigoGrupoEstoque = 9,
                Nome = "Messi-update",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarTblFinanciamentoCreate()
        {
            MSG0044 tst = new MSG0044(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0044")
            {
                NumeroTabelaFinanciamento = "33333",
                DataInicioValidade = new DateTime(2014, 03, 01),
                DataFinalValidade = new DateTime(2014, 06, 11),
                Situacao = 0
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarTblFinanciamentoUpdate()
        {
            MSG0044 tst = new MSG0044(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0044")
            {
                NumeroTabelaFinanciamento = "{123123123}",
                DataInicioValidade = new DateTime(2014, 11, 01),
                DataFinalValidade = new DateTime(2014, 12, 01),
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarRotaCreate()
        {
            MSG0054 tst = new MSG0054(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0054")
            {
                CodigoRota = "3333",
                Nome = "Rota Guilhermino",
                Roteiro = "Roteiro da rota Guilhermino",
                Situacao = 1
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarRotaUpdate()
        {
            MSG0054 tst = new MSG0054(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0054")
            {
                CodigoRota = "112233",
                Nome = "Rota Messi2",
                Roteiro = "Roteiro da rota do Messi2",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarFamiliaComercial()
        {
            MSG0036 tst = new MSG0036(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0036")
            {
                Nome = "Comercial Guilhermino",
                CodigoFamiliaComercial = "3333",
                Segmento = "33",
                Familia = "2525",
                SubFamilia = "22525",
                Origem = "2020",
                Situacao = 0
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarMsgBens()
        {
            //MSG0066 tst = new MSG0066(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0066")
            //{
            //    Nome = "teste",
            //    CodigoImovel = itb.RetornaSistema(itb.Sistema.Pollux),
            //    Conta = itb.RetornaSistema(itb.Sistema.Extranet),
            //    TipoImovel = 1,
            //    ValorImovel = 100000,
            //    ValorOnus = 100,
            //    Matricula = itb.RetornaSistema(itb.Sistema.Extranet),
            //    Moeda = "real",
            //    Situacao = 1,
            //    Proprietario = itb.RetornaSistema(itb.Sistema.Extranet),
            //    TipoProprietario = "a",
            //    EnderecoImovel = null


            //};
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Casa</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0066</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>ACINOM REPRESENTACAO COML LTDA</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0066>");
            sb.AppendLine("      <Conta xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />");
            sb.AppendLine("      <Contato>1bf4430c-90e6-e311-b278-00155d01330e</Contato>");
            sb.AppendLine("      <Nome>Casa</Nome>");
            sb.AppendLine("      <TipoImovel>993520000</TipoImovel>");
            sb.AppendLine("      <ValorImovel>540000</ValorImovel>");
            sb.AppendLine("      <ValorOnus>150000</ValorOnus>");
            sb.AppendLine("      <Matricula>MAT 321.6549</Matricula>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>8844a066-26e7-e311-b278-00155d01330e</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("      <NaturezaProprietarioImovel>993520000</NaturezaProprietarioImovel>");
            sb.AppendLine("      <EnderecoImovel>");
            sb.AppendLine("        <TipoEndereco xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />");
            sb.AppendLine("        <CEP>88075620</CEP>");
            sb.AppendLine("        <Logradouro>RUA CARLOS AUGUSTO DOMINGUES</Logradouro>");
            sb.AppendLine("        <Numero>54</Numero>");
            sb.AppendLine("        <Complemento>apto</Complemento>");
            sb.AppendLine("        <Bairro>Centro</Bairro>");
            sb.AppendLine("        <NomeCidade>ABDON BATISTA</NomeCidade>");
            sb.AppendLine("        <Cidade>ABDON BATISTA,SC,BRASIL</Cidade>");
            sb.AppendLine("        <UF>SC</UF>");
            sb.AppendLine("        <Estado>Brasil,SC</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("      </EnderecoImovel>");
            sb.AppendLine("    </MSG0066>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");



            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }
        [Test]
        public void TestarUnidadeNegocio()
        {
            MSG0002 tst = new MSG0002(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0002")
            {
                CodigoUnidadeNegocio = "44",
                Nome = "Jacob",
                Situacao = 0

            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);
        }
        [Test]
        public void TestarRegiaoGeoGrafica()
        {

            MSG0008 tst = new MSG0008(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0008")
            {
                Nome = "Guilherminos Area",
                //CodigoRegiaoGeografica = "33",
                Situacao = 0
            };


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);
        }

        [Test]
        public void TesteContato()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>SERGIO LUIZ</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0058</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0058>");
            sb.AppendLine("      <Canal>442F19AD-76EE-E311-9407-00155D013D38</Canal>");
            sb.AppendLine("      <TipoObjetoCanal>account</TipoObjetoCanal>");
            sb.AppendLine("      <NomeContato>SERGIO LUIZ</NomeContato>");
            sb.AppendLine("      <Sobrenome>ZANON</Sobrenome>");
            sb.AppendLine("      <Email>sergiozanon@alarma.com.br</Email>");
            sb.AppendLine("      <Telefone>(41) 3259-2000</Telefone>");
            sb.AppendLine("      <Ramal>2015</Ramal>");
            sb.AppendLine("      <Cargo>993520011</Cargo>");
            sb.AppendLine("      <MetodoEntrega>23</MetodoEntrega>");
            sb.AppendLine("      <TipoContato>993520008</TipoContato>");
            sb.AppendLine("      <CPF>32070837904</CPF>");
            sb.AppendLine("      <Proprietario>259a8e4f-15e9-e311-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <EnderecoPrincipal>");
            sb.AppendLine("        <TipoEndereco>3</TipoEndereco>");
            sb.AppendLine("        <CEP>80610020</CEP>");
            sb.AppendLine("        <Logradouro>RUA PARA </Logradouro>");
            sb.AppendLine("        <Numero>1834</Numero>");
            sb.AppendLine("        <Bairro>AGUA VERDE</Bairro>");
            sb.AppendLine("        <NomeCidade>CURITIBA</NomeCidade>");
            sb.AppendLine("        <Cidade>CURITIBA,PR,Brasil</Cidade>");
            sb.AppendLine("        <UF>PR</UF>");
            sb.AppendLine("        <Estado>Brasil,PR</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("      </EnderecoPrincipal>");
            sb.AppendLine("    </MSG0058>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);


        }

        [Test]
        public void TestarReferenciaCanal()
        {
            MSG0070 tst = new MSG0070(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0070")
            {
                Nome = "Ref Canal Teste1",
                Conta = "8EC9B9DB-96AF-E311-9207-00155D013D19",
                Situacao = 0,
                Telefone = "33333333",
                NomeContato = "Guilhermino",
                Proprietario = "B4E55B63-9AAE-E311-9207-00155D013D19",
                TipoProprietario = "systemuser"

            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }

        [Test]
        public void TestarForncedorCanal()
        {
            MSG0068 tst = new MSG0068(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0068")
            {
                Nome = "fornecedor Guilhermino",
                Conta = "8EC9B9DB-96AF-E311-9207-00155D013D19",
                Situacao = 0,
                Telefone = "30303030",
                NomeContato = "Marcio",
                Proprietario = "B4E55B63-9AAE-E311-9207-00155D013D19",
                TipoProprietario = "systemuser"
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }

        [Test]
        public void TestarSeguroConta()
        {
            MSG0064 tst = new MSG0064(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0064")
            {
                Nome = "Seguro conta Guilhermino",
                Conta = "8EC9B9DB-96AF-E311-9207-00155D013D19",
                Situacao = 0,
                Modalidade = "90909090",
                Valor = 3333,
                DataVencimento = DateTime.Now,
                Proprietario = "B4E55B63-9AAE-E311-9207-00155D013D19",
                TipoProprietario = "systemuser"
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }

        [Test]
        public void TestarEstruturaAtendimento()
        {
            MSG0062 tst = new MSG0062(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0062")
            {
                Nome = "Estrutura Guilhermino",
                Conta = "8EC9B9DB-96AF-E311-9207-00155D013D19",
                Situacao = 0,
                Tipo = 993520002,
                PossuiEstruturaMinima = true,
                UnidadeNegocio = "333",
                Proprietario = "B4E55B63-9AAE-E311-9207-00155D013D19",
                TipoProprietario = "systemuser"
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }

        [Test]
        public void TestarReceitaPadrao()
        {
            MSG0052 tst = new MSG0052(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0052")
            {
                Nome = "Receita 55",
                Situacao = 0,
                CodigoReceitaPadrao = 55,
                LoginUsuario = "Joselita"
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }

        [Test]
        public void TestarBens()
        {

            Intelbras.Message.Helper.Entities.Endereco enderecoBens = new Message.Helper.Entities.Endereco();
            enderecoBens.Bairro = "Taquaral";
            enderecoBens.CEP = "89500600";
            enderecoBens.Cidade = "Florianopolis, SC, Brasil";
            enderecoBens.Estado = "Brasil,SC";
            enderecoBens.Logradouro = "Rua Quantico";
            enderecoBens.NomeCidade = "Florianopolis";
            //enderecoBens.NomeContatoEndereco = "Carlos";
            enderecoBens.NomeEndereco = "Endereco Teste 2";
            enderecoBens.NomePais = "Brasil";
            enderecoBens.Numero = "500";
            enderecoBens.Pais = "Brasil";
            //enderecoBens.Telefone = "56565656";
            //enderecoBens.TipoEndereco = 1;
            enderecoBens.UF = "SP";

            MSG0066 tst = new MSG0066(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0066")
            {
                Nome = "Imovel Guilhermino",
                Conta = "8EC9B9DB-96AF-E311-9207-00155D013D19",
                Situacao = 0,
                EnderecoImovel = enderecoBens,
                Matricula = "6060",
                ValorOnus = 10000,
                ValorImovel = 2000000,
                TipoImovel = 993520001,
                Moeda = "Real",
                Proprietario = "B4E55B63-9AAE-E311-9207-00155D013D19",
                TipoProprietario = "systemuser",
                NaturezaProprietarioImovel = 993520000
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }

        [Test]
        public void TestarCanaDeVenda()
        {
            MSG0040 tst = new MSG0040(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0040")
            {
                CodigoCanalVenda = 444,
                Nome = "Canal Guilhermino",
                Situacao = 1
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);
        }

        [Test]
        public void TesteCondicaoPagamento()
        {
            MSG0004 tst = new MSG0004(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0004")
            {
                Nome = "Condicao 001",
                Situacao = 0,
                LoginUsuario = "Joselita",
                CodigoCondicaoPagamento = 001,
                NumeroParcelas = 3,
                PercentualDesconto = new decimal(0.2),
                Prazo = 0,
                SupplierCard = false

            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }
        [Test]
        public void TestePedido()
        {
            #region teste1





            #endregion
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            #region StringBuilder

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>914885</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0091</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>fr001832</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0091>");
            sb.AppendLine("      <NumeroPedido>914885</NumeroPedido>");
            sb.AppendLine("      <Nome>914885</Nome>");
            sb.AppendLine("      <NumeroPedidoCliente>0</NumeroPedidoCliente>");
            sb.AppendLine("      <NumeroPedidoRepresentante />");
            sb.AppendLine("      <CodigoClienteCRM>e49cb808-2708-e411-9420-00155d013d39</CodigoClienteCRM>");
            sb.AppendLine("      <TipoObjetoCliente>account</TipoObjetoCliente>");
            sb.AppendLine("      <NomeAbreviadoCliente>ROUTE TELECO</NomeAbreviadoCliente>");
            sb.AppendLine("      <TabelaPreco />");
            sb.AppendLine("      <TabelaPrecoEMS />");
            sb.AppendLine("      <TipoPreco>993520000</TipoPreco>");
            sb.AppendLine("      <Estabelecimento>104</Estabelecimento>");
            sb.AppendLine("      <CondicaoPagamento>558</CondicaoPagamento>");
            sb.AppendLine("      <TabelaFinanciamento>10</TabelaFinanciamento>");
            sb.AppendLine("      <Representante>1061</Representante>");
            sb.AppendLine("      <CodigoAssistente>30</CodigoAssistente>");
            sb.AppendLine("      <NaturezaOperacao>610100</NaturezaOperacao>");
            sb.AppendLine("      <DataEmissao>2014-07-10</DataEmissao>");
            sb.AppendLine("      <DataImplantacao>2014-07-10</DataImplantacao>");
            sb.AppendLine("      <UsuarioImplantacao>adm</UsuarioImplantacao>");
            sb.AppendLine("      <DataImplantacaoUsuario>2014-07-10</DataImplantacaoUsuario>");
            sb.AppendLine("      <DataEntrega>2014-07-10</DataEntrega>");
            sb.AppendLine("      <DataEntregaSolicitada>2014-07-10</DataEntregaSolicitada>");
            sb.AppendLine("      <DataReativacao>2014-07-10</DataReativacao>");
            sb.AppendLine("      <DataReativacaoUsuario>2014-07-10</DataReativacaoUsuario>");
            sb.AppendLine("      <DiasNegociacao>0</DiasNegociacao>");
            sb.AppendLine("      <TipoPedido>30</TipoPedido>");
            sb.AppendLine("      <OrigemPedido>993520015</OrigemPedido>");
            sb.AppendLine("      <Prioridade>10</Prioridade>");
            sb.AppendLine("      <CNPJ>01455957000195</CNPJ>");
            sb.AppendLine("      <InscricaoEstadual>401079156113</InscricaoEstadual>");
            sb.AppendLine("      <SituacaoPedido>100001</SituacaoPedido>");
            sb.AppendLine("      <Situacao>3</Situacao>");
            sb.AppendLine("      <PercentualDesconto1>0.000000</PercentualDesconto1>");
            sb.AppendLine("      <PercentualDesconto2>0.000000</PercentualDesconto2>");
            sb.AppendLine("      <CidadeCIF>JAU</CidadeCIF>");
            sb.AppendLine("      <Portador>999</Portador>");
            sb.AppendLine("      <ModalidadeCobranca>993520006</ModalidadeCobranca>");
            sb.AppendLine("      <Mensagem>0</Mensagem>");
            sb.AppendLine("      <Observacao>- liberado pra faturamento (22/7)4000088 - 5pç  </Observacao>");
            sb.AppendLine("      <CondicaoEspecial>PEDIDO 43184  </CondicaoEspecial>");
            sb.AppendLine("      <ObservacaoRedespacho />");
            sb.AppendLine("      <UsuarioAlteracao>ge048216</UsuarioAlteracao>");
            sb.AppendLine("      <DataAlteracao>2014-07-22</DataAlteracao>");
            sb.AppendLine("      <UsuarioCancelamento />");
            sb.AppendLine("      <DescricaoCancelamento />");
            sb.AppendLine("      <UsuarioReativacao>ge048216</UsuarioReativacao>");
            sb.AppendLine("      <UsuarioSuspensao>integra</UsuarioSuspensao>");
            sb.AppendLine("      <DescricaoSuspensao>4000088 - 5pç PEDIDO 43184  </DescricaoSuspensao>");
            sb.AppendLine("      <DataSuspensao>2014-07-10</DataSuspensao>");
            sb.AppendLine("      <IndicacaoAprovacao>false</IndicacaoAprovacao>");
            sb.AppendLine("      <AprovacaoForcada>OK</AprovacaoForcada>");
            sb.AppendLine("      <UsuarioAprovacao>em046525</UsuarioAprovacao>");
            sb.AppendLine("      <DataAprovacao>2014-07-23</DataAprovacao>");
            sb.AppendLine("      <DestinoMercadoria>993520000</DestinoMercadoria>");
            sb.AppendLine("      <Transportadora>312</Transportadora>");
            sb.AppendLine("      <Rota>SP - Int</Rota>");
            sb.AppendLine("      <FaturamentoParcial>true</FaturamentoParcial>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <ValorTotalLiquido>6213.1500000000</ValorTotalLiquido>");
            sb.AppendLine("      <ValorTotalPedido>8832.9900000000</ValorTotalPedido>");
            sb.AppendLine("      <ValorTotalAberto>8832.9900000000</ValorTotalAberto>");
            sb.AppendLine("      <ValorMercadoriaAberto>6213.1500000000</ValorMercadoriaAberto>");
            sb.AppendLine("      <IndiceFinanciamento>10;1</IndiceFinanciamento>");
            sb.AppendLine("      <SituacaoAvaliacao>993520002</SituacaoAvaliacao>");
            sb.AppendLine("      <MotivoBloqueioCredito>Cliente excedeu valor parametrizado para atrasos duplicatas.</MotivoBloqueioCredito>");
            sb.AppendLine("      <MotivoLiberacaoCredito>OK</MotivoLiberacaoCredito>");
            sb.AppendLine("      <SituacaoAlocacao>993520000</SituacaoAlocacao>");
            sb.AppendLine("      <ValorCreditoLiberado>0.00</ValorCreditoLiberado>");
            sb.AppendLine("      <ValorDesconto>0.0000000000</ValorDesconto>");
            sb.AppendLine("      <PercentualDesconto>0.0</PercentualDesconto>");
            sb.AppendLine("      <PercentualDescontoICMS>0.000</PercentualDescontoICMS>");
            sb.AppendLine("      <CodigoEndereco>Padrão</CodigoEndereco>");
            sb.AppendLine("      <ValorFrete>0.00</ValorFrete>");
            sb.AppendLine("      <CondicaoFrete>2</CondicaoFrete>");
            sb.AppendLine("      <PedidoCompleto>true</PedidoCompleto>");
            sb.AppendLine("      <CanalVenda>11</CanalVenda>");
            sb.AppendLine("      <CodigoEntregaTriangular />");
            sb.AppendLine("      <ListaPreco>Lista Padrão</ListaPreco>");
            sb.AppendLine("      <Descricao />");
            sb.AppendLine("      <ValorTotalImpostos>2619.85</ValorTotalImpostos>");
            sb.AppendLine("      <ValorTotalSemFrete>8832.99</ValorTotalSemFrete>");
            sb.AppendLine("      <ValorTotalDesconto>0.0</ValorTotalDesconto>");
            sb.AppendLine("      <PrecoBloqueado>false</PrecoBloqueado>");
            sb.AppendLine("      <Classificacao>e11378c7-6ded-e311-9407-00155d013d38</Classificacao>");
            sb.AppendLine("      <PedidoOriginal>0</PedidoOriginal>");
            sb.AppendLine("      <TotalIPI>931.98</TotalIPI>");
            sb.AppendLine("      <TotalSubstituicaoTributaria>1687.87</TotalSubstituicaoTributaria>");
            sb.AppendLine("      <Proprietario>e69cb808-2708-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("      <CondicaoFreteEntrega>1</CondicaoFreteEntrega>");
            sb.AppendLine("      <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("      <AprovadorPedido />");
            sb.AppendLine("      <EnderecoEntrega>");
            sb.AppendLine("        <NomeEndereco>AVENIDA ANTONIO ADIB CHAMMAS, 86</NomeEndereco>");
            sb.AppendLine("        <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("        <CaixaPostal />");
            sb.AppendLine("        <CEP>17212171</CEP>");
            sb.AppendLine("        <Logradouro>AVENIDA ANTONIO ADIB CHAMMAS</Logradouro>");
            sb.AppendLine("        <Numero>86   </Numero>");
            sb.AppendLine("        <Complemento />");
            sb.AppendLine("        <Bairro>JARDIM CONTINENTAL</Bairro>");
            sb.AppendLine("        <NomeCidade>JAU</NomeCidade>");
            sb.AppendLine("        <Cidade>JAU,SP,Brasil</Cidade>");
            sb.AppendLine("        <UF>SP</UF>");
            sb.AppendLine("        <Estado>Brasil,SP</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("        <NomeContato>DISTRIBUIDORA DE ELETRONICOS ROUTE 66 LT</NomeContato>");
            sb.AppendLine("        <Telefone>1436026666</Telefone>");
            sb.AppendLine("        <Fax />");
            sb.AppendLine("      </EnderecoEntrega>");
            sb.AppendLine("      <EnderecoCobranca>");
            sb.AppendLine("        <NomeEndereco>AVENIDA ANTONIO ADIB CHAMMAS, 86</NomeEndereco>");
            sb.AppendLine("        <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("        <CaixaPostal />");
            sb.AppendLine("        <CEP>17212171</CEP>");
            sb.AppendLine("        <Logradouro>AVENIDA ANTONIO ADIB CHAMMAS</Logradouro>");
            sb.AppendLine("        <Numero>86   </Numero>");
            sb.AppendLine("        <Complemento />");
            sb.AppendLine("        <Bairro>JARDIM CONTINENTAL</Bairro>");
            sb.AppendLine("        <NomeCidade>JAU</NomeCidade>");
            sb.AppendLine("        <Cidade>JAU,SP,BRASIL</Cidade>");
            sb.AppendLine("        <UF>SP</UF>");
            sb.AppendLine("        <Estado>BRASIL,SP</Estado>");
            sb.AppendLine("        <NomePais>BRASIL</NomePais>");
            sb.AppendLine("        <Pais>BRASIL</Pais>");
            sb.AppendLine("        <NomeContato>DISTRIBUIDORA DE ELETRONICOS ROUTE 66 LT</NomeContato>");
            sb.AppendLine("        <Telefone>1436026666</Telefone>");
            sb.AppendLine("        <Fax />");
            sb.AppendLine("      </EnderecoCobranca>");
            sb.AppendLine("      <PedidosItens>");
            sb.AppendLine("        <PedidoItem>");
            sb.AppendLine("          <ChaveIntegracao>12052,914885,10,4000037,</ChaveIntegracao>");
            sb.AppendLine("          <NumeroPedido>914885</NumeroPedido>");
            sb.AppendLine("          <NumeroPedidoCliente>0</NumeroPedidoCliente>");
            sb.AppendLine("          <Sequencia>10</Sequencia>");
            sb.AppendLine("          <Produto>4000037</Produto>");
            sb.AppendLine("          <DescricaoItemPedido>RADIO COM FRS INTELBRAS TWIN 9,6 PT</DescricaoItemPedido>");
            sb.AppendLine("          <UnidadeMedida>PC</UnidadeMedida>");
            sb.AppendLine("          <NaturezaOperacao>640300</NaturezaOperacao>");
            sb.AppendLine("          <DataEntregaSolicitada>2014-07-10</DataEntregaSolicitada>");
            sb.AppendLine("          <DataEntrega>2014-07-10</DataEntrega>");
            sb.AppendLine("          <DataImplantacao>2014-07-10</DataImplantacao>");
            sb.AppendLine("          <QuantidadePedida>30.0000</QuantidadePedida>");
            sb.AppendLine("          <QuantidadeEntregue>0.0000</QuantidadeEntregue>");
            sb.AppendLine("          <QuantidadePendente>0.0000</QuantidadePendente>");
            sb.AppendLine("          <QuantidadeDevolvida>0.0000</QuantidadeDevolvida>");
            sb.AppendLine("          <QuantidadeCancelada>0.0000</QuantidadeCancelada>");
            sb.AppendLine("          <UsuarioDevolucao />");
            sb.AppendLine("          <ValorTabela>168.7500000000</ValorTabela>");
            sb.AppendLine("          <ValorOriginal>168.7500000000</ValorOriginal>");
            sb.AppendLine("          <PrecoNegociado>168.7500000000</PrecoNegociado>");
            sb.AppendLine("          <PrecoMinimo>0.00</PrecoMinimo>");
            sb.AppendLine("          <SituacaoItem>993520000</SituacaoItem>");
            sb.AppendLine("          <UsuarioImplantacao>integra</UsuarioImplantacao>");
            sb.AppendLine("          <UsuarioAlteracao />");
            sb.AppendLine("          <UsuarioCancelamento />");
            sb.AppendLine("          <DescricaoCancelamento />");
            sb.AppendLine("          <UsuarioReativacao>ge048216</UsuarioReativacao>");
            sb.AppendLine("          <DataReativacao>2014-07-10</DataReativacao>");
            sb.AppendLine("          <DataReativacaoUsuario>2014-07-10</DataReativacaoUsuario>");
            sb.AppendLine("          <UsuarioSuspensao>integra</UsuarioSuspensao>");
            sb.AppendLine("          <DataSuspensao>2014-07-10</DataSuspensao>");
            sb.AppendLine("          <DataSuspensaoUsuario>2014-07-10</DataSuspensaoUsuario>");
            sb.AppendLine("          <AliquotaIPI>15.00</AliquotaIPI>");
            sb.AppendLine("          <RetemICMSFonte>true</RetemICMSFonte>");
            sb.AppendLine("          <PercentualDescontoICMS>0.0</PercentualDescontoICMS>");
            sb.AppendLine("          <ValorLiquido>5062.5000000000</ValorLiquido>");
            sb.AppendLine("          <ValorLiquidoAberto>7197.1600000000</ValorLiquidoAberto>");
            sb.AppendLine("          <ValorMercadoriaAberto>5062.5000000000</ValorMercadoriaAberto>");
            sb.AppendLine("          <ValorTotal>7197.1600000000</ValorTotal>");
            sb.AppendLine("          <TipoPreco>993520000</TipoPreco>");
            sb.AppendLine("          <Observacao />");
            sb.AppendLine("          <QuantidadeAlocada>0.0000</QuantidadeAlocada>");
            sb.AppendLine("          <SituacaoAlocacao>993520000</SituacaoAlocacao>");
            sb.AppendLine("          <PercentualMinimoFaturamento>0.0</PercentualMinimoFaturamento>");
            sb.AppendLine("          <DataMinimaFaturamento>2014-07-10</DataMinimaFaturamento>");
            sb.AppendLine("          <QuantidadeAlocadaLogica>15.0000</QuantidadeAlocadaLogica>");
            sb.AppendLine("          <PermiteSubstituirPreco>true</PermiteSubstituirPreco>");
            sb.AppendLine("          <FaturaQuantidadeFamilia>false</FaturaQuantidadeFamilia>");
            sb.AppendLine("          <TaxaCambio>0.0</TaxaCambio>");
            sb.AppendLine("          <DescontoManual>0.0000000000</DescontoManual>");
            sb.AppendLine("          <CondicaoFrete>2</CondicaoFrete>");
            sb.AppendLine("          <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("          <ValorTotalImposto>2134.67</ValorTotalImposto>");
            sb.AppendLine("          <ValorSubstituicaoTributaria>1375.29</ValorSubstituicaoTributaria>");
            sb.AppendLine("          <ValorIPI>759.3800</ValorIPI>");
            sb.AppendLine("          <ProdutoForaCatalogo>false</ProdutoForaCatalogo>");
            sb.AppendLine("          <DescricaoProdutoForaCatalogo />");
            sb.AppendLine("          <Moeda>Real</Moeda>");
            sb.AppendLine("          <UnidadeNegocio>TER</UnidadeNegocio>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <Representante>1061</Representante>");
            sb.AppendLine("          <NomeAbreviadoCliente>ROUTE TELECO</NomeAbreviadoCliente>");
            sb.AppendLine("          <ValorTotalProduto>7197.1600000000</ValorTotalProduto>");
            sb.AppendLine("          <EnderecoEntrega>");
            sb.AppendLine("            <NomeEndereco>AVENIDA ANTONIO ADIB CHAMMAS, 86</NomeEndereco>");
            sb.AppendLine("            <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("            <CaixaPostal />");
            sb.AppendLine("            <CEP>17212171</CEP>");
            sb.AppendLine("            <Logradouro>AVENIDA ANTONIO ADIB CHAMMAS</Logradouro>");
            sb.AppendLine("            <Numero>86   </Numero>");
            sb.AppendLine("            <Complemento />");
            sb.AppendLine("            <Bairro>JARDIM CONTINENTAL</Bairro>");
            sb.AppendLine("            <NomeCidade>JAU</NomeCidade>");
            sb.AppendLine("            <Cidade>JAU,SP,Brasil</Cidade>");
            sb.AppendLine("            <UF>SP</UF>");
            sb.AppendLine("            <Estado>Brasil,SP</Estado>");
            sb.AppendLine("            <NomePais>Brasil</NomePais>");
            sb.AppendLine("            <Pais>Brasil</Pais>");
            sb.AppendLine("            <NomeContato>DISTRIBUIDORA DE ELETRONICOS ROUTE 66 LT</NomeContato>");
            sb.AppendLine("            <Telefone>1436026666</Telefone>");
            sb.AppendLine("            <Fax />");
            sb.AppendLine("          </EnderecoEntrega>");
            sb.AppendLine("        </PedidoItem>");
            sb.AppendLine("        <PedidoItem>");
            sb.AppendLine("          <ChaveIntegracao>12052,914885,20,4000088,</ChaveIntegracao>");
            sb.AppendLine("          <NumeroPedido>914885</NumeroPedido>");
            sb.AppendLine("          <NumeroPedidoCliente>0</NumeroPedidoCliente>");
            sb.AppendLine("          <Sequencia>20</Sequencia>");
            sb.AppendLine("          <Produto>4000088</Produto>");
            sb.AppendLine("          <DescricaoItemPedido>RADIO COM FRS TWIN WATER PROOF</DescricaoItemPedido>");
            sb.AppendLine("          <UnidadeMedida>PC</UnidadeMedida>");
            sb.AppendLine("          <NaturezaOperacao>640300</NaturezaOperacao>");
            sb.AppendLine("          <DataEntregaSolicitada>2014-07-10</DataEntregaSolicitada>");
            sb.AppendLine("          <DataEntrega>2014-08-01</DataEntrega>");
            sb.AppendLine("          <DataImplantacao>2014-07-10</DataImplantacao>");
            sb.AppendLine("          <QuantidadePedida>5.0000</QuantidadePedida>");
            sb.AppendLine("          <QuantidadeEntregue>0.0000</QuantidadeEntregue>");
            sb.AppendLine("          <QuantidadePendente>0.0000</QuantidadePendente>");
            sb.AppendLine("          <QuantidadeDevolvida>0.0000</QuantidadeDevolvida>");
            sb.AppendLine("          <QuantidadeCancelada>0.0000</QuantidadeCancelada>");
            sb.AppendLine("          <DescricaoDevolucao />");
            sb.AppendLine("          <UsuarioDevolucao />");
            sb.AppendLine("          <ValorTabela>230.1300000000</ValorTabela>");
            sb.AppendLine("          <ValorOriginal>230.1300000000</ValorOriginal>");
            sb.AppendLine("          <PrecoNegociado>230.1300000000</PrecoNegociado>");
            sb.AppendLine("          <PrecoMinimo>0.00</PrecoMinimo>");
            sb.AppendLine("          <SituacaoItem>993520000</SituacaoItem>");
            sb.AppendLine("          <UsuarioImplantacao>integra</UsuarioImplantacao>");
            sb.AppendLine("          <UsuarioAlteracao>ge048216</UsuarioAlteracao>");
            sb.AppendLine("          <DataAlteracao>2014-07-10</DataAlteracao>");
            sb.AppendLine("          <UsuarioCancelamento />");
            sb.AppendLine("          <DescricaoCancelamento />");
            sb.AppendLine("          <UsuarioReativacao>ge048216</UsuarioReativacao>");
            sb.AppendLine("          <DataReativacao>2014-07-10</DataReativacao>");
            sb.AppendLine("          <DataReativacaoUsuario>2014-07-10</DataReativacaoUsuario>");
            sb.AppendLine("          <UsuarioSuspensao>integra</UsuarioSuspensao>");
            sb.AppendLine("          <DataSuspensao>2014-07-10</DataSuspensao>");
            sb.AppendLine("          <DataSuspensaoUsuario>2014-07-10</DataSuspensaoUsuario>");
            sb.AppendLine("          <AliquotaIPI>15.00</AliquotaIPI>");
            sb.AppendLine("          <RetemICMSFonte>true</RetemICMSFonte>");
            sb.AppendLine("          <PercentualDescontoICMS>0.0</PercentualDescontoICMS>");
            sb.AppendLine("          <ValorLiquido>1150.6500000000</ValorLiquido>");
            sb.AppendLine("          <ValorLiquidoAberto>1635.8300000000</ValorLiquidoAberto>");
            sb.AppendLine("          <ValorMercadoriaAberto>1150.6500000000</ValorMercadoriaAberto>");
            sb.AppendLine("          <ValorTotal>1635.8300000000</ValorTotal>");
            sb.AppendLine("          <TipoPreco>993520000</TipoPreco>");
            sb.AppendLine("          <Observacao />");
            sb.AppendLine("          <QuantidadeAlocada>0.0000</QuantidadeAlocada>");
            sb.AppendLine("          <SituacaoAlocacao>993520000</SituacaoAlocacao>");
            sb.AppendLine("          <PercentualMinimoFaturamento>0.0</PercentualMinimoFaturamento>");
            sb.AppendLine("          <DataMinimaFaturamento>2014-07-10</DataMinimaFaturamento>");
            sb.AppendLine("          <QuantidadeAlocadaLogica>2.0000</QuantidadeAlocadaLogica>");
            sb.AppendLine("          <PermiteSubstituirPreco>true</PermiteSubstituirPreco>");
            sb.AppendLine("          <FaturaQuantidadeFamilia>false</FaturaQuantidadeFamilia>");
            sb.AppendLine("          <TaxaCambio>0.0</TaxaCambio>");
            sb.AppendLine("          <DescontoManual>0.0000000000</DescontoManual>");
            sb.AppendLine("          <CondicaoFrete>2</CondicaoFrete>");
            sb.AppendLine("          <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("          <ValorTotalImposto>485.18</ValorTotalImposto>");
            sb.AppendLine("          <ValorSubstituicaoTributaria>312.58</ValorSubstituicaoTributaria>");
            sb.AppendLine("          <ValorIPI>172.6000</ValorIPI>");
            sb.AppendLine("          <ProdutoForaCatalogo>false</ProdutoForaCatalogo>");
            sb.AppendLine("          <DescricaoProdutoForaCatalogo />");
            sb.AppendLine("          <Moeda>Real</Moeda>");
            sb.AppendLine("          <UnidadeNegocio>TER</UnidadeNegocio>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <Representante>1061</Representante>");
            sb.AppendLine("          <NomeAbreviadoCliente>ROUTE TELECO</NomeAbreviadoCliente>");
            sb.AppendLine("          <ValorTotalProduto>1635.8300000000</ValorTotalProduto>");
            sb.AppendLine("          <EnderecoEntrega>");
            sb.AppendLine("            <NomeEndereco>AVENIDA ANTONIO ADIB CHAMMAS, 86</NomeEndereco>");
            sb.AppendLine("            <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("            <CaixaPostal />");
            sb.AppendLine("            <CEP>17212171</CEP>");
            sb.AppendLine("            <Logradouro>AVENIDA ANTONIO ADIB CHAMMAS</Logradouro>");
            sb.AppendLine("            <Numero>86   </Numero>");
            sb.AppendLine("            <Complemento />");
            sb.AppendLine("            <Bairro>JARDIM CONTINENTAL</Bairro>");
            sb.AppendLine("            <NomeCidade>JAU</NomeCidade>");
            sb.AppendLine("            <Cidade>JAU,SP,Brasil</Cidade>");
            sb.AppendLine("            <UF>SP</UF>");
            sb.AppendLine("            <Estado>Brasil,SP</Estado>");
            sb.AppendLine("            <NomePais>Brasil</NomePais>");
            sb.AppendLine("            <Pais>Brasil</Pais>");
            sb.AppendLine("            <NomeContato>DISTRIBUIDORA DE ELETRONICOS ROUTE 66 LT</NomeContato>");
            sb.AppendLine("            <Telefone>1436026666</Telefone>");
            sb.AppendLine("            <Fax />");
            sb.AppendLine("          </EnderecoEntrega>");
            sb.AppendLine("        </PedidoItem>");
            sb.AppendLine("      </PedidosItens>");
            sb.AppendLine("    </MSG0091>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");
            #endregion

            String teste = String.Empty;

            MSG0091 msg = MessageBase.LoadMessage<MSG0091>(XDocument.Parse(sb.ToString()));

            integ.Postar(usuario, senha, msg.GenerateMessage(false), out teste);


        }


        [Test]
        public void TesteNotaFiscal()//Klecas - RazaoStatus - ProdutoFatura
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>0999987</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0094</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>an046325</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0094>");
            sb.AppendLine("      <ChaveIntegracao>104,1,0999987</ChaveIntegracao>");
            sb.AppendLine("      <NumeroNotaFiscal>0999999</NumeroNotaFiscal>");
            sb.AppendLine("      <Nome>0285926-06/08/14-DEMO</Nome>");
            sb.AppendLine("      <NumeroSerie>1</NumeroSerie>");
            sb.AppendLine("      <CodigoClienteCRM>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoClienteCRM>");
            sb.AppendLine("      <TipoObjetoCliente>account</TipoObjetoCliente>");
            sb.AppendLine("      <NumeroPedido>920018</NumeroPedido>");
            sb.AppendLine("      <NumeroPedidoCliente>920018</NumeroPedidoCliente>");
            sb.AppendLine("      <Descricao>Descrição</Descricao>");
            sb.AppendLine("      <Estabelecimento>104</Estabelecimento>");
            sb.AppendLine("      <CondicaoPagamento>553</CondicaoPagamento>");
            sb.AppendLine("      <NomeAbreviadoCliente>TOOLSYSTEMS</NomeAbreviadoCliente>");
            sb.AppendLine("      <NaturezaOperacao>540100</NaturezaOperacao>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <SituacaoNota>0</SituacaoNota>");
            sb.AppendLine("      <SituacaoEntrega>4</SituacaoEntrega>");
            sb.AppendLine("      <DataEmissao>2014-08-06</DataEmissao>");
            sb.AppendLine("            <DataConfirmacao>2014-08-06</DataConfirmacao>");
            sb.AppendLine("            <ValorFrete>0.00</ValorFrete>");
            sb.AppendLine("      <PesoLiquido>22.550</PesoLiquido>");
            sb.AppendLine("      <PesoBruto>27.850</PesoBruto>");
            sb.AppendLine("      <Observacao>IPI REDUZIDO CONF. LEI 8.248/91 E ALTERACOES E DECRETO 5.906/06 E ALTERACOES - E PORTARIAS INTERMINISTERIAIS 816/01, 570/04, 571/04, 226/05, 799/07, 48/09, 55/09, 836/09, 502/07, 312/08, 84/09, 236/09, 301/09, 488/09, 47/10, 407/10, 531/10, 606/10, 1.103/10, 627/12, 984/12, 635/14 PORTARIA MDIC Nº 10/2013, PORT. PROVISORIAS Nº 72, 77, 38, 21, 26 E 29 DE 2014. PRAZO DE VALIDADE: INDETERMINADO. PRODUTO FABRICADO NO ESTABELECIMENTO CNPJ: 82.901.000/0001-27Diferimento parcial do ICMS, nos termos do RICMS-SC/01, Anexo 3, Art. 10-B, Inciso V, parágrafo 3º.  Pedido: 920317 Cod.Cliente: 184802</Observacao>");
            sb.AppendLine("      <Volume>11</Volume>");
            sb.AppendLine("      <ValorBaseICMS>0.00</ValorBaseICMS>");
            sb.AppendLine("      <ValorICMS>2108.18</ValorICMS>");
            sb.AppendLine("      <ValorIPI>0.00</ValorIPI>");
            sb.AppendLine("      <ValorBaseSubstituicaoTributaria>0.0</ValorBaseSubstituicaoTributaria>");
            sb.AppendLine("      <ValorSubstituicaoTributaria>0.00</ValorSubstituicaoTributaria>");
            sb.AppendLine("      <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("      <MetodoEntrega>17</MetodoEntrega>");
            sb.AppendLine("      <Transportadora>312</Transportadora>");
            sb.AppendLine("      <Frete>SAO JOSE</Frete>");
            sb.AppendLine("      <CondicaoFreteEntrega>1</CondicaoFreteEntrega>");
            sb.AppendLine("      <CNPJ>02071489000118</CNPJ>");
            sb.AppendLine("            <InscricaoEstadual>250082764</InscricaoEstadual>");
            sb.AppendLine("            <PrecoBloqueado>false</PrecoBloqueado>");
            sb.AppendLine("      <ValorDesconto>0.00</ValorDesconto>");
            sb.AppendLine("      <PercentualDesconto>0.0</PercentualDesconto>");
            sb.AppendLine("      <ListaPreco>Lista Padrão</ListaPreco>");
            sb.AppendLine("      <Prioridade>1</Prioridade>");
            sb.AppendLine("      <Representante>4000</Representante>");
            sb.AppendLine("      <Proprietario>d86a88b1-bc0d-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("      <ValorTotal>18348.2400000000</ValorTotal>");
            sb.AppendLine("      <ValorTotalImpostos>780.04</ValorTotalImpostos>");
            sb.AppendLine("      <ValorTotalSemImposto>17568.2000000000</ValorTotalSemImposto>");
            sb.AppendLine("      <ValorTotalSemFrete>18348.2400000000</ValorTotalSemFrete>");
            sb.AppendLine("      <ValorTotalDesconto>0.00</ValorTotalDesconto>");
            sb.AppendLine("      <ValorTotalProdutos>18348.2400000000</ValorTotalProdutos>");
            sb.AppendLine("      <ValorTotalProdutosSemImposto>17568.2000000000</ValorTotalProdutosSemImposto>");
            sb.AppendLine("      <TelefoneCobranca>4733813344</TelefoneCobranca>");
            sb.AppendLine("      <FaxCobranca />");
            sb.AppendLine("      <EnderecoEntrega>");
            sb.AppendLine("        <NomeEndereco>DEMO</NomeEndereco>");
            sb.AppendLine("        <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("        <CaixaPostal>555</CaixaPostal>");
            sb.AppendLine("        <CEP>88104800</CEP>");
            sb.AppendLine("        <Logradouro>Rodovia BR 101, 010</Logradouro>");
            sb.AppendLine("        <Numero>KM 21</Numero>");
            sb.AppendLine("        <Complemento>KM 210</Complemento>");
            sb.AppendLine("        <Bairro>Área Industrial</Bairro>");
            sb.AppendLine("        <NomeCidade>SAO JOSE</NomeCidade>");
            sb.AppendLine("        <Cidade>SAO JOSE,SC,Brasil</Cidade>");
            sb.AppendLine("        <UF>SC</UF>");
            sb.AppendLine("        <Estado>Brasil,SC</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("        <NomeContato>DEMO</NomeContato>");
            sb.AppendLine("        <Telefone>4733813344</Telefone>");
            sb.AppendLine("        <Fax />");
            sb.AppendLine("      </EnderecoEntrega>");
            sb.AppendLine("      <NotaFiscalItens>");
            sb.AppendLine("        <NotaFiscalItem>");
            sb.AppendLine("          <ChaveIntegracao>104,1,0888888,10,4990003</ChaveIntegracao>");
            sb.AppendLine("          <CodigoProduto>4990003</CodigoProduto>");
            sb.AppendLine("          <NomeProduto>PLACA RAMAL ANALOGICO NKMC 22000 16 RA</NomeProduto>");
            sb.AppendLine("          <Descricao>PLACA RAMAL ANALOGICO NKMC 22000 16 RA</Descricao>");
            sb.AppendLine("          <PrecoOriginal>574.9000000000</PrecoOriginal>");
            sb.AppendLine("          <PrecoUnitario>574.9000000000</PrecoUnitario>");
            sb.AppendLine("          <PrecoLiquido>574.9</PrecoLiquido>");
            sb.AppendLine("          <ValorMercadoriaTabela>5749.0000000000</ValorMercadoriaTabela>");
            sb.AppendLine("          <ValorMercadoriaOriginal>5749.0000000000</ValorMercadoriaOriginal>");
            sb.AppendLine("          <ValorMercadoriaLiquido>5749.0000000000</ValorMercadoriaLiquido>");
            sb.AppendLine("          <CodigoNaturezaOperacao>540100</CodigoNaturezaOperacao>");
            sb.AppendLine("          <NomeNaturezaOperacao />");
            sb.AppendLine("          <ProdutoForaCatalogo>false</ProdutoForaCatalogo>");
            sb.AppendLine("          <DescricaoProdutoForaCatalogo />");
            sb.AppendLine("          <PermiteSubstituirPreco>true</PermiteSubstituirPreco>");
            sb.AppendLine("          <UnidadeMedida>pc</UnidadeMedida>");
            sb.AppendLine("          <ValorBaseICMS>5749.00</ValorBaseICMS>");
            sb.AppendLine("          <ValorBaseICMSSubstituicao>5749.00</ValorBaseICMSSubstituicao>");
            sb.AppendLine("          <ValorICMS>689.88</ValorICMS>");
            sb.AppendLine("          <ValorICMSSubstituicao>255.26</ValorICMSSubstituicao>");
            sb.AppendLine("          <ValorICMSNaoTributado>0.00</ValorICMSNaoTributado>");
            sb.AppendLine("          <ValorICMSOutras>0.00</ValorICMSOutras>");
            sb.AppendLine("          <CodigoTributarioICMS>1</CodigoTributarioICMS>");
            sb.AppendLine("          <CodigoTributarioISS>2</CodigoTributarioISS>");
            sb.AppendLine("          <CodigoTributarioIPI>3</CodigoTributarioIPI>");
            sb.AppendLine("          <ValorBaseISS>0.00</ValorBaseISS>");
            sb.AppendLine("          <ValorBaseIPI>0.00</ValorBaseIPI>");
            sb.AppendLine("          <AliquotaISS>0.00</AliquotaISS>");
            sb.AppendLine("          <AliquotaIPI>0.00</AliquotaIPI>");
            sb.AppendLine("          <AliquotaICMS>12.00</AliquotaICMS>");
            sb.AppendLine("          <ValorISS>0.00</ValorISS>");
            sb.AppendLine("          <ValorISSNaoTributado>0.00</ValorISSNaoTributado>");
            sb.AppendLine("          <ValorISSOutras>0.00</ValorISSOutras>");
            sb.AppendLine("          <ValorIPI>0.00</ValorIPI>");
            sb.AppendLine("          <ValorIPINaoTributado>0.00</ValorIPINaoTributado>");
            sb.AppendLine("          <ValorIPIOutras>5749.00</ValorIPIOutras>");
            sb.AppendLine("          <PrecoConsumidor>0.0</PrecoConsumidor>");
            sb.AppendLine("          <QuantidadeCancelada>0.0</QuantidadeCancelada>");
            sb.AppendLine("          <QuantidadePendente>0.0</QuantidadePendente>");
            sb.AppendLine("                    <CondicaoFrete>2</CondicaoFrete>");
            sb.AppendLine("          <ValorOriginal>5749.0000000000</ValorOriginal>");
            sb.AppendLine("          <ValorTotalImposto>255.26</ValorTotalImposto>");
            sb.AppendLine("          <ValorDescontoManual>0.0</ValorDescontoManual>");
            sb.AppendLine("          <Quantidade>10.0</Quantidade>");
            sb.AppendLine("          <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("          <QuantidadeEntregue>0.0</QuantidadeEntregue>");
            sb.AppendLine("          <NumeroSequencia>10</NumeroSequencia>");
            sb.AppendLine("          <CodigoUnidadeNegocio>CEN</CodigoUnidadeNegocio>");
            sb.AppendLine("          <NomeUnidadeNegocio>ICORP</NomeUnidadeNegocio>");
            sb.AppendLine("          <CodigoRepresentante>4000</CodigoRepresentante>");
            sb.AppendLine("          <ValorTotal>6004.2600000000</ValorTotal>");
            sb.AppendLine("          <Moeda>Real</Moeda>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <EnderecoEntrega>");
            sb.AppendLine("            <NomeEndereco>DEMO</NomeEndereco>");
            sb.AppendLine("            <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("            <CaixaPostal>555</CaixaPostal>");
            sb.AppendLine("            <CEP>88104800</CEP>");
            sb.AppendLine("            <Logradouro>Rodovia BR 101, 010</Logradouro>");
            sb.AppendLine("            <Numero>KM 21</Numero>");
            sb.AppendLine("            <Complemento>KM 210</Complemento>");
            sb.AppendLine("            <Bairro>Área Industrial</Bairro>");
            sb.AppendLine("            <NomeCidade>SAO JOSE</NomeCidade>");
            sb.AppendLine("            <Cidade>SAO JOSE,SC,Brasil</Cidade>");
            sb.AppendLine("            <UF>SC</UF>");
            sb.AppendLine("            <Estado>Brasil,SC</Estado>");
            sb.AppendLine("            <NomePais>Brasil</NomePais>");
            sb.AppendLine("            <Pais>Brasil</Pais>");
            sb.AppendLine("            <NomeContato>DEMO</NomeContato>");
            sb.AppendLine("            <Telefone>4733813344</Telefone>");
            sb.AppendLine("            <Fax />");
            sb.AppendLine("          </EnderecoEntrega>");
            sb.AppendLine("        </NotaFiscalItem>");
            sb.AppendLine("        <NotaFiscalItem>");
            sb.AppendLine("          <ChaveIntegracao>104,1,077777,20,4990109</ChaveIntegracao>");
            sb.AppendLine("          <CodigoProduto>4990109</CodigoProduto>");
            sb.AppendLine("          <NomeProduto>PLACA BASE DE ACESSORIOS IMPACTA 140/220</NomeProduto>");
            sb.AppendLine("          <Descricao>PLACA BASE DE ACESSORIOS IMPACTA 140/220</Descricao>");
            sb.AppendLine("          <PrecoOriginal>238.0000000000</PrecoOriginal>");
            sb.AppendLine("          <PrecoUnitario>238.0000000000</PrecoUnitario>");
            sb.AppendLine("          <PrecoLiquido>238.0</PrecoLiquido>");
            sb.AppendLine("          <ValorMercadoriaTabela>2380.0000000000</ValorMercadoriaTabela>");
            sb.AppendLine("          <ValorMercadoriaOriginal>2380.0000000000</ValorMercadoriaOriginal>");
            sb.AppendLine("          <ValorMercadoriaLiquido>2380.0000000000</ValorMercadoriaLiquido>");
            sb.AppendLine("          <CodigoNaturezaOperacao>540100</CodigoNaturezaOperacao>");
            sb.AppendLine("          <NomeNaturezaOperacao />");
            sb.AppendLine("          <ProdutoForaCatalogo>false</ProdutoForaCatalogo>");
            sb.AppendLine("          <DescricaoProdutoForaCatalogo />");
            sb.AppendLine("          <PermiteSubstituirPreco>true</PermiteSubstituirPreco>");
            sb.AppendLine("          <UnidadeMedida>pc</UnidadeMedida>");
            sb.AppendLine("          <ValorBaseICMS>2380.00</ValorBaseICMS>");
            sb.AppendLine("          <ValorBaseICMSSubstituicao>2380.00</ValorBaseICMSSubstituicao>");
            sb.AppendLine("          <ValorICMS>285.60</ValorICMS>");
            sb.AppendLine("          <ValorICMSSubstituicao>105.67</ValorICMSSubstituicao>");
            sb.AppendLine("          <ValorICMSNaoTributado>0.00</ValorICMSNaoTributado>");
            sb.AppendLine("          <ValorICMSOutras>0.00</ValorICMSOutras>");
            sb.AppendLine("          <CodigoTributarioICMS>1</CodigoTributarioICMS>");
            sb.AppendLine("          <CodigoTributarioISS>2</CodigoTributarioISS>");
            sb.AppendLine("          <CodigoTributarioIPI>3</CodigoTributarioIPI>");
            sb.AppendLine("          <ValorBaseISS>0.00</ValorBaseISS>");
            sb.AppendLine("          <ValorBaseIPI>0.00</ValorBaseIPI>");
            sb.AppendLine("          <AliquotaISS>0.00</AliquotaISS>");
            sb.AppendLine("          <AliquotaIPI>0.00</AliquotaIPI>");
            sb.AppendLine("          <AliquotaICMS>12.00</AliquotaICMS>");
            sb.AppendLine("          <ValorISS>0.00</ValorISS>");
            sb.AppendLine("          <ValorISSNaoTributado>0.00</ValorISSNaoTributado>");
            sb.AppendLine("          <ValorISSOutras>0.00</ValorISSOutras>");
            sb.AppendLine("          <ValorIPI>0.00</ValorIPI>");
            sb.AppendLine("          <ValorIPINaoTributado>0.00</ValorIPINaoTributado>");
            sb.AppendLine("          <ValorIPIOutras>2380.00</ValorIPIOutras>");
            sb.AppendLine("          <PrecoConsumidor>0.0</PrecoConsumidor>");
            sb.AppendLine("          <QuantidadeCancelada>0.0</QuantidadeCancelada>");
            sb.AppendLine("          <QuantidadePendente>0.0</QuantidadePendente>");
            sb.AppendLine("                    <CondicaoFrete>2</CondicaoFrete>");
            sb.AppendLine("          <ValorOriginal>2380.0000000000</ValorOriginal>");
            sb.AppendLine("          <ValorTotalImposto>105.67</ValorTotalImposto>");
            sb.AppendLine("          <ValorDescontoManual>0.0</ValorDescontoManual>");
            sb.AppendLine("          <Quantidade>10.0</Quantidade>");
            sb.AppendLine("          <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("          <QuantidadeEntregue>0.0</QuantidadeEntregue>");
            sb.AppendLine("          <NumeroSequencia>20</NumeroSequencia>");
            sb.AppendLine("          <CodigoUnidadeNegocio>CEN</CodigoUnidadeNegocio>");
            sb.AppendLine("          <NomeUnidadeNegocio>ICORP</NomeUnidadeNegocio>");
            sb.AppendLine("          <CodigoRepresentante>4000</CodigoRepresentante>");
            sb.AppendLine("          <ValorTotal>2485.6700000000</ValorTotal>");
            sb.AppendLine("          <Moeda>Real</Moeda>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <EnderecoEntrega>");
            sb.AppendLine("            <NomeEndereco>DEMO</NomeEndereco>");
            sb.AppendLine("            <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("            <CaixaPostal>555</CaixaPostal>");
            sb.AppendLine("            <CEP>88104800</CEP>");
            sb.AppendLine("            <Logradouro>Rodovia BR 101, 010</Logradouro>");
            sb.AppendLine("            <Numero>KM 21</Numero>");
            sb.AppendLine("            <Complemento>KM 210</Complemento>");
            sb.AppendLine("            <Bairro>Área Industrial</Bairro>");
            sb.AppendLine("            <NomeCidade>SAO JOSE</NomeCidade>");
            sb.AppendLine("            <Cidade>SAO JOSE,SC,Brasil</Cidade>");
            sb.AppendLine("            <UF>SC</UF>");
            sb.AppendLine("            <Estado>Brasil,SC</Estado>");
            sb.AppendLine("            <NomePais>Brasil</NomePais>");
            sb.AppendLine("            <Pais>Brasil</Pais>");
            sb.AppendLine("            <NomeContato>DEMO</NomeContato>");
            sb.AppendLine("            <Telefone>4733813344</Telefone>");
            sb.AppendLine("            <Fax />");
            sb.AppendLine("          </EnderecoEntrega>");
            sb.AppendLine("        </NotaFiscalItem>");
            sb.AppendLine("        <NotaFiscalItem>");
            sb.AppendLine("          <ChaveIntegracao>104,1,099998,30,4990002</ChaveIntegracao>");
            sb.AppendLine("          <CodigoProduto>4990002</CodigoProduto>");
            sb.AppendLine("          <NomeProduto>PLACA TRONCO NKMc 22000 8 TR</NomeProduto>");
            sb.AppendLine("          <Descricao>PLACA TRONCO NKMc 22000 8 TR</Descricao>");
            sb.AppendLine("          <PrecoOriginal>485.7100000000</PrecoOriginal>");
            sb.AppendLine("          <PrecoUnitario>485.7100000000</PrecoUnitario>");
            sb.AppendLine("          <PrecoLiquido>485.71</PrecoLiquido>");
            sb.AppendLine("          <ValorMercadoriaTabela>4857.1000000000</ValorMercadoriaTabela>");
            sb.AppendLine("          <ValorMercadoriaOriginal>4857.1000000000</ValorMercadoriaOriginal>");
            sb.AppendLine("          <ValorMercadoriaLiquido>4857.1000000000</ValorMercadoriaLiquido>");
            sb.AppendLine("          <CodigoNaturezaOperacao>540100</CodigoNaturezaOperacao>");
            sb.AppendLine("          <NomeNaturezaOperacao />");
            sb.AppendLine("          <ProdutoForaCatalogo>false</ProdutoForaCatalogo>");
            sb.AppendLine("          <DescricaoProdutoForaCatalogo />");
            sb.AppendLine("          <PermiteSubstituirPreco>true</PermiteSubstituirPreco>");
            sb.AppendLine("          <UnidadeMedida>pc</UnidadeMedida>");
            sb.AppendLine("          <ValorBaseICMS>4857.10</ValorBaseICMS>");
            sb.AppendLine("          <ValorBaseICMSSubstituicao>4857.10</ValorBaseICMSSubstituicao>");
            sb.AppendLine("          <ValorICMS>582.85</ValorICMS>");
            sb.AppendLine("          <ValorICMSSubstituicao>215.66</ValorICMSSubstituicao>");
            sb.AppendLine("          <ValorICMSNaoTributado>0.00</ValorICMSNaoTributado>");
            sb.AppendLine("          <ValorICMSOutras>0.00</ValorICMSOutras>");
            sb.AppendLine("          <CodigoTributarioICMS>1</CodigoTributarioICMS>");
            sb.AppendLine("          <CodigoTributarioISS>2</CodigoTributarioISS>");
            sb.AppendLine("          <CodigoTributarioIPI>3</CodigoTributarioIPI>");
            sb.AppendLine("          <ValorBaseISS>0.00</ValorBaseISS>");
            sb.AppendLine("          <ValorBaseIPI>0.00</ValorBaseIPI>");
            sb.AppendLine("          <AliquotaISS>0.00</AliquotaISS>");
            sb.AppendLine("          <AliquotaIPI>0.00</AliquotaIPI>");
            sb.AppendLine("          <AliquotaICMS>12.00</AliquotaICMS>");
            sb.AppendLine("          <ValorISS>0.00</ValorISS>");
            sb.AppendLine("          <ValorISSNaoTributado>0.00</ValorISSNaoTributado>");
            sb.AppendLine("          <ValorISSOutras>0.00</ValorISSOutras>");
            sb.AppendLine("          <ValorIPI>0.00</ValorIPI>");
            sb.AppendLine("          <ValorIPINaoTributado>0.00</ValorIPINaoTributado>");
            sb.AppendLine("          <ValorIPIOutras>4857.10</ValorIPIOutras>");
            sb.AppendLine("          <PrecoConsumidor>0.0</PrecoConsumidor>");
            sb.AppendLine("          <QuantidadeCancelada>0.0</QuantidadeCancelada>");
            sb.AppendLine("          <QuantidadePendente>0.0</QuantidadePendente>");
            sb.AppendLine("                    <CondicaoFrete>2</CondicaoFrete>");
            sb.AppendLine("          <ValorOriginal>4857.1000000000</ValorOriginal>");
            sb.AppendLine("          <ValorTotalImposto>215.66</ValorTotalImposto>");
            sb.AppendLine("          <ValorDescontoManual>0.0</ValorDescontoManual>");
            sb.AppendLine("          <Quantidade>10.0</Quantidade>");
            sb.AppendLine("          <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("          <QuantidadeEntregue>0.0</QuantidadeEntregue>");
            sb.AppendLine("          <NumeroSequencia>30</NumeroSequencia>");
            sb.AppendLine("          <CodigoUnidadeNegocio>CEN</CodigoUnidadeNegocio>");
            sb.AppendLine("          <NomeUnidadeNegocio>ICORP</NomeUnidadeNegocio>");
            sb.AppendLine("          <CodigoRepresentante>4000</CodigoRepresentante>");
            sb.AppendLine("          <ValorTotal>5072.7600000000</ValorTotal>");
            sb.AppendLine("          <Moeda>Real</Moeda>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <EnderecoEntrega>");
            sb.AppendLine("            <NomeEndereco>DEMO</NomeEndereco>");
            sb.AppendLine("            <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("            <CaixaPostal>555</CaixaPostal>");
            sb.AppendLine("            <CEP>88104800</CEP>");
            sb.AppendLine("            <Logradouro>Rodovia BR 101, 010</Logradouro>");
            sb.AppendLine("            <Numero>KM 21</Numero>");
            sb.AppendLine("            <Complemento>KM 210</Complemento>");
            sb.AppendLine("            <Bairro>Área Industrial</Bairro>");
            sb.AppendLine("            <NomeCidade>SAO JOSE</NomeCidade>");
            sb.AppendLine("            <Cidade>SAO JOSE,SC,Brasil</Cidade>");
            sb.AppendLine("            <UF>SC</UF>");
            sb.AppendLine("            <Estado>Brasil,SC</Estado>");
            sb.AppendLine("            <NomePais>Brasil</NomePais>");
            sb.AppendLine("            <Pais>Brasil</Pais>");
            sb.AppendLine("            <NomeContato>DEMO</NomeContato>");
            sb.AppendLine("            <Telefone>4733813344</Telefone>");
            sb.AppendLine("            <Fax />");
            sb.AppendLine("          </EnderecoEntrega>");
            sb.AppendLine("        </NotaFiscalItem>");
            sb.AppendLine("        <NotaFiscalItem>");
            sb.AppendLine("          <ChaveIntegracao>104,1,099991,40,4450000</ChaveIntegracao>");
            sb.AppendLine("          <CodigoProduto>4450000</CodigoProduto>");
            sb.AppendLine("          <NomeProduto>CENTRAL IMPACTA 40 2/4</NomeProduto>");
            sb.AppendLine("          <Descricao>CENTRAL IMPACTA 40 2/4</Descricao>");
            sb.AppendLine("          <PrecoOriginal>458.2100000000</PrecoOriginal>");
            sb.AppendLine("          <PrecoUnitario>458.2100000000</PrecoUnitario>");
            sb.AppendLine("          <PrecoLiquido>458.21</PrecoLiquido>");
            sb.AppendLine("          <ValorMercadoriaTabela>4582.1000000000</ValorMercadoriaTabela>");
            sb.AppendLine("          <ValorMercadoriaOriginal>4582.1000000000</ValorMercadoriaOriginal>");
            sb.AppendLine("          <ValorMercadoriaLiquido>4582.1000000000</ValorMercadoriaLiquido>");
            sb.AppendLine("          <CodigoNaturezaOperacao>540100</CodigoNaturezaOperacao>");
            sb.AppendLine("          <NomeNaturezaOperacao />");
            sb.AppendLine("          <ProdutoForaCatalogo>false</ProdutoForaCatalogo>");
            sb.AppendLine("          <DescricaoProdutoForaCatalogo />");
            sb.AppendLine("          <PermiteSubstituirPreco>true</PermiteSubstituirPreco>");
            sb.AppendLine("          <UnidadeMedida>pc</UnidadeMedida>");
            sb.AppendLine("          <ValorBaseICMS>4582.10</ValorBaseICMS>");
            sb.AppendLine("          <ValorBaseICMSSubstituicao>4582.10</ValorBaseICMSSubstituicao>");
            sb.AppendLine("          <ValorICMS>549.85</ValorICMS>");
            sb.AppendLine("          <ValorICMSSubstituicao>203.45</ValorICMSSubstituicao>");
            sb.AppendLine("          <ValorICMSNaoTributado>0.00</ValorICMSNaoTributado>");
            sb.AppendLine("          <ValorICMSOutras>0.00</ValorICMSOutras>");
            sb.AppendLine("          <CodigoTributarioICMS>1</CodigoTributarioICMS>");
            sb.AppendLine("          <CodigoTributarioISS>2</CodigoTributarioISS>");
            sb.AppendLine("          <CodigoTributarioIPI>3</CodigoTributarioIPI>");
            sb.AppendLine("          <ValorBaseISS>0.00</ValorBaseISS>");
            sb.AppendLine("          <ValorBaseIPI>0.00</ValorBaseIPI>");
            sb.AppendLine("          <AliquotaISS>0.00</AliquotaISS>");
            sb.AppendLine("          <AliquotaIPI>0.00</AliquotaIPI>");
            sb.AppendLine("          <AliquotaICMS>12.00</AliquotaICMS>");
            sb.AppendLine("          <ValorISS>0.00</ValorISS>");
            sb.AppendLine("          <ValorISSNaoTributado>0.00</ValorISSNaoTributado>");
            sb.AppendLine("          <ValorISSOutras>0.00</ValorISSOutras>");
            sb.AppendLine("          <ValorIPI>0.00</ValorIPI>");
            sb.AppendLine("          <ValorIPINaoTributado>0.00</ValorIPINaoTributado>");
            sb.AppendLine("          <ValorIPIOutras>4582.10</ValorIPIOutras>");
            sb.AppendLine("          <PrecoConsumidor>0.0</PrecoConsumidor>");
            sb.AppendLine("          <QuantidadeCancelada>0.0</QuantidadeCancelada>");
            sb.AppendLine("          <QuantidadePendente>0.0</QuantidadePendente>");
            sb.AppendLine("                    <CondicaoFrete>2</CondicaoFrete>");
            sb.AppendLine("          <ValorOriginal>4582.1000000000</ValorOriginal>");
            sb.AppendLine("          <ValorTotalImposto>203.45</ValorTotalImposto>");
            sb.AppendLine("          <ValorDescontoManual>0.0</ValorDescontoManual>");
            sb.AppendLine("          <Quantidade>10.0</Quantidade>");
            sb.AppendLine("          <RetiraNoLocal>false</RetiraNoLocal>");
            sb.AppendLine("          <QuantidadeEntregue>0.0</QuantidadeEntregue>");
            sb.AppendLine("          <NumeroSequencia>40</NumeroSequencia>");
            sb.AppendLine("          <CodigoUnidadeNegocio>CEN</CodigoUnidadeNegocio>");
            sb.AppendLine("          <NomeUnidadeNegocio>ICORP</NomeUnidadeNegocio>");
            sb.AppendLine("          <CodigoRepresentante>4000</CodigoRepresentante>");
            sb.AppendLine("          <ValorTotal>4785.5500000000</ValorTotal>");
            sb.AppendLine("          <Moeda>Real</Moeda>");
            sb.AppendLine("          <Acao>A</Acao>");
            sb.AppendLine("          <EnderecoEntrega>");
            sb.AppendLine("            <NomeEndereco>DEMO</NomeEndereco>");
            sb.AppendLine("            <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("            <CaixaPostal>555</CaixaPostal>");
            sb.AppendLine("            <CEP>88104800</CEP>");
            sb.AppendLine("            <Logradouro>Rodovia BR 101, 010</Logradouro>");
            sb.AppendLine("            <Numero>KM 21</Numero>");
            sb.AppendLine("            <Complemento>KM 210</Complemento>");
            sb.AppendLine("            <Bairro>Área Industrial</Bairro>");
            sb.AppendLine("            <NomeCidade>SAO JOSE</NomeCidade>");
            sb.AppendLine("            <Cidade>SAO JOSE,SC,Brasil</Cidade>");
            sb.AppendLine("            <UF>SC</UF>");
            sb.AppendLine("            <Estado>Brasil,SC</Estado>");
            sb.AppendLine("            <NomePais>Brasil</NomePais>");
            sb.AppendLine("            <Pais>Brasil</Pais>");
            sb.AppendLine("            <NomeContato>DEMO</NomeContato>");
            sb.AppendLine("            <Telefone>4733813344</Telefone>");
            sb.AppendLine("            <Fax />");
            sb.AppendLine("          </EnderecoEntrega>");
            sb.AppendLine("        </NotaFiscalItem>");
            sb.AppendLine("      </NotaFiscalItens>");
            sb.AppendLine("    </MSG0094>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>    ");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }



        [Test]
        public void TesteWsPollux()
        {
            MSG0058 tst = new MSG0058(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0058");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;


            integ.EnviarMensagemBarramento(tst.GenerateMessage(false), "1", "1", out teste);
        }
        [Test]
        public void TesteCanalVenda()
        {
            MSG0040 tst = new MSG0040(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0040");
            tst.CodigoCanalVenda = 21;
            tst.Nome = "Teste Msg Canal";
            tst.Situacao = 0;

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);
        }

        [Test]
        public void TesteListarRegiaoAtuacao()
        {
            MSG0075 tst = new MSG0075(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0075");
            tst.CodigoConta = "283830EB-91BA-E311-9207-00155D013D19";

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }
        [Test]
        public void TesteListarUnidadeNegocio()
        {
            MSG0130 tst = new MSG0130(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0130");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }
        [Test]
        public void TesteListarSegmento()
        {
            // MSG0129 tst = new MSG0129(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0129");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>TER</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0129</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0129 />");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteListarProdutoEstabelecimento()
        {
            MSG0102 tst = new MSG0102(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0102");
            tst.CodigoEstabelecimento = 101;

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);

        }
        [Test]
        public void TestarFamiliaProduto()
        {
            MSG0028 tst = new MSG0028(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0028")
            {
                Nome = "Familia Guilhermino",
                CodigoFamilia = "2525",
                Segmento = "1305",
                Situacao = 1
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarMensagem()
        {
            MSG0048 tst = new MSG0048(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0048")
            {
                Nome = "MSG Guilhermino",
                CodigoMensagemPedido = 444,
                Situacao = 0
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }


        [Test]
        public void TestarItemListaPreco()
        {
            //MSG0082 tst = new MSG0082(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0082")
            //{
            //    Produto = "4005050",
            //    ListaPreco = "Lista Padrão",
            //    UnidadeMedida = "1",//"6B7E92C0-B9A6-44B4-9147-FC18A385B3F0",//1055
            //    ListaDesconto = "F9C56CC4-0DCB-E311-BB3D-00155D013E44",
            //    OpcaoVendaParcial = 2,
            //    MetodoPrecificacao = 1,
            //    Valor = 1000,
            //    ValorArredondamento = 0,
            //    Porcentagem = 1,
            //    //PoliticaArrdondamento = 1,
            //    OpcaoArredondamento = 1,
            //    Moeda = "Real"

            //};
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("<MENSAGEM xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance">\");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>1010010</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0082</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario />");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0082>");
            sb.AppendLine("      <CodigoItemListaPreco>95bca4f7-46f2-e311-9407-00155d013d38</CodigoItemListaPreco>");
            sb.AppendLine("      <Produto>4002111</Produto>");
            sb.AppendLine("      <ListaPreco>Lista Padrão</ListaPreco>");
            sb.AppendLine("      <Valor>0.0</Valor>");
            sb.AppendLine("      <Porcentagem>0.0</Porcentagem>");
            sb.AppendLine("      <ListaDesconto>64F76014-321B-E511-9902-00155D013F21</ListaDesconto>");
            sb.AppendLine("      <MetodoPrecificacao>1</MetodoPrecificacao>");
            sb.AppendLine("      <OpcaoVendaParcial>1</OpcaoVendaParcial>");
            sb.AppendLine("      <ValorArredondamento>0.0</ValorArredondamento>");
            sb.AppendLine("      <OpcaoArredondamento>2</OpcaoArredondamento>");
            sb.AppendLine("      <PoliticaArredondamento>1</PoliticaArredondamento>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <UnidadeMedida>PC</UnidadeMedida>");
            sb.AppendLine("    </MSG0082>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            //string tst1 = "<?xml version=\"1.0\"?><MENSAGEM xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">  <CABECALHO>    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>    <NumeroOperacao>1010069</NumeroOperacao>    <CodigoMensagem>MSG0082</CodigoMensagem>    <LoginUsuario />  </CABECALHO>  <CONTEUDO>    <MSG0082>      <CodigoItemListaPreco xsi:nil=\"true\" />      <Produto>1010069</Produto>      <ListaPreco>Lista Padrão</ListaPreco>      <Valor>0.0</Valor>      <Porcentagem>0.0</Porcentagem>      <ListaDesconto xsi:nil=\"true\" />      <MetodoPrecificacao>1</MetodoPrecificacao>      <OpcaoVendaParcial>1</OpcaoVendaParcial>      <ValorArredondamento>0.0</ValorArredondamento>      <OpcaoArredondamento>2</OpcaoArredondamento>      <PoliticaArredondamento>1</PoliticaArredondamento>      <Moeda>Real</Moeda>      <UnidadeMedida>PC</UnidadeMedida>    </MSG0082>  </CONTEUDO></MENSAGEM>";
            integ.Postar(usuario, senha, sb.ToString(), out teste);//tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TesteNivelPosVenda()
        {

            Domain.Model.NivelPosVenda nivelPosVenda = new Domain.Model.NivelPosVenda(OrganizationName, IsOffline);

            nivelPosVenda.ID = new Guid("12DA377C-479E-E311-888D-00155D013E2E");
            nivelPosVenda.Nome = "Nivel 11";
            nivelPosVenda.Status = 1;

            MSG0132 tst = new MSG0132(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0132");

            tst.Nome = nivelPosVenda.Nome;
            tst.Situacao = (int)nivelPosVenda.Status;
            tst.CodigoNivelPosVenda = nivelPosVenda.ID.ToString();

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            string resposta = string.Empty;

            integ.EnviarMensagemBarramento(tst.GenerateMessage(true), "1", "1", out resposta);

        }

        [Test]
        public void TestarAcaoRegiaoCanal()
        {
            MSG0076 tst = new MSG0076(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0076")
            {
                CodigoConta = "283830EB-91BA-E311-9207-00155D013D19",
                ChaveIntegracaoCidade = "Florianopolis, SC, Brasil",
                Acao = "I"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TesteCriptografia()
        {

            Util.Utilitario._symmetricAlgorithm = new AesManaged();
            Util.Utilitario._symmetricAlgorithm.GenerateIV();

            string textonormal = "barramento";
            string textoalterado = string.Empty;

            string textonormal2 = "vCha6rGxXmWHeu8YKWfeXW2E";
            string textoalterado2 = string.Empty;

            var ciphertextBase64 = Util.Utilitario.EncryptSymmetric(textonormal, Encoding.Unicode.GetBytes(ConfigurationManager.GetSettingValue("ChaveCriptografia")));

            ciphertextBase64 = "9kocG7iti3COXiID4VA3LgGj0B8f9c38C1JijyjWpvZGj2FeUGFXtWbAFgAZjlku";

            textoalterado = Util.Utilitario.DecryptSymmetric(ciphertextBase64, Encoding.Unicode.GetBytes(ConfigurationManager.GetSettingValue("ChaveCriptografia")));

            var ciphertextBase642 = Util.Utilitario.EncryptSymmetric(textonormal2, Encoding.Unicode.GetBytes(ConfigurationManager.GetSettingValue("ChaveCriptografia")));

            ciphertextBase642 = "MZ/9XcGODh8i8KH6ihBbqjMTEGDl5w5q50hsfI9/Acy7uSYo8Ud4tEZ7JgIIgy+RlcpEGN7kFT7RnfOSqDFpZgZYCc2J9tNRzhzRtyftsVg=";

            textoalterado2 = Util.Utilitario.DecryptSymmetric(ciphertextBase642, Encoding.Unicode.GetBytes(ConfigurationManager.GetSettingValue("ChaveCriptografia")));

        }

        [Test]
        public void TestarListarAcessoExtranet()
        {
            MSG0126 tst = new MSG0126(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0126")
            {
                TipoAcesso = "EB37AC48-949D-E311-888D-00155D013E2E"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarFuncaoConexao()
        {
            MSG0128 tst = new MSG0128(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0128");
            tst.CategoriaFuncaoConexao = 1;

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);
        }

        [Test]
        public void TestarEstabelecimento()
        {
            MSG0042 tst = new MSG0042(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0042");
            tst.CEP = "04545900";
            tst.Cidade = "São Paulo";
            tst.CNPJ = "00000000000100";
            tst.CodigoEstabelecimento = 204;
            tst.Endereco = "Rua do teste";
            tst.InscricaoEstadual = "300300";
            tst.Nome = "Estab Guilhermino";
            tst.RazaoSocial = "Intelmino";
            tst.Situacao = 0;
            tst.UF = "SP";

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(false), out teste);
        }

        [Test]
        public void TestarParametrosGlobais()
        {
            //MSG0111 tst = new MSG0111(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0111");
            //tst.TipoParametroGlobal = 1;

            StringBuilder sb = new StringBuilder();
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>5</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0111</CodigoMensagem>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0111>");
            //sb.AppendLine("      <CodigoBeneficio>4030A792-2D9E-E311-888D-00155D013E2E</CodigoBeneficio>");
            //sb.AppendLine("      <TipoParametroGlobal>5</TipoParametroGlobal>");
            //sb.AppendLine("      <CodigoUnidadeNegocio>CEN</CodigoUnidadeNegocio>");
            //sb.AppendLine("    </MSG0111>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");



            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>9</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0111</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0111>");

            sb.AppendLine("      <CodigoClassificacao>e11378c7-6ded-e311-9407-00155d013d38</CodigoClassificacao>");

            sb.AppendLine("      <CodigoCategoria>68c05902-99ee-e311-940a-00155d013d3b</CodigoCategoria>");
            sb.AppendLine("      <CodigoBeneficio>1d68e3c4-73ed-e311-9407-00155d013d38</CodigoBeneficio>");
            sb.AppendLine("      <TipoParametroGlobal>22</TipoParametroGlobal>");

            sb.AppendLine("      <CodigoUnidadeNegocio>IMG</CodigoUnidadeNegocio>");
            sb.AppendLine("    </MSG0111>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TestarListaPMA()
        {
            //MSG0086 tst = new MSG0086(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0086");
            //tst.Estado = "SP";
            //tst.Moeda = "BRL";
            //tst.ProdutosItens.Add(new Message.Helper.Entities.Produto() { CodigoProduto = "" });
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Brasil,SP</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0086</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0086>");
            sb.AppendLine("      <Estado>Brasil,SC</Estado>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <ProdutosItens>");
            sb.AppendLine("        <ProdutoItem>");
            sb.AppendLine("          <CodigoProduto>1870337</CodigoProduto>");
            sb.AppendLine("        </ProdutoItem>");
            sb.AppendLine("        <ProdutoItem>");
            sb.AppendLine("          <CodigoProduto>4390190</CodigoProduto>          ");
            sb.AppendLine("        </ProdutoItem>");
            sb.AppendLine("        <ProdutoItem>");
            sb.AppendLine("          <CodigoProduto>4760020</CodigoProduto>");
            sb.AppendLine("        </ProdutoItem>");
            sb.AppendLine("      </ProdutosItens>");
            sb.AppendLine("    </MSG0086>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }
        [Test]
        public void TestarListaPSD()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Brasil,SP</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0087</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0087>");
            sb.AppendLine("      <Estado>Brasil,SP</Estado>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <ProdutosItens>");
            sb.AppendLine("        <ProdutoItem>");
            sb.AppendLine("          <CodigoProduto>4002111</CodigoProduto>");
            sb.AppendLine("        </ProdutoItem>");
            //sb.AppendLine("        <ProdutoItem>");
            //sb.AppendLine("          <CodigoProduto>4400017</CodigoProduto>");
            //sb.AppendLine("        </ProdutoItem>");
            sb.AppendLine("      </ProdutosItens>");
            sb.AppendLine("    </MSG0087>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>Brasil,SC</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0087</CodigoMensagem>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0087>");
            //sb.AppendLine("      <Estado>Brasil,SP</Estado>");
            //sb.AppendLine("      <Moeda>Real</Moeda>");
            //sb.AppendLine("      <ProdutosItens>");
            //sb.AppendLine("        <ProdutoItem>");
            //sb.AppendLine("          <CodigoProduto>4005050</CodigoProduto>");
            //sb.AppendLine("        </ProdutoItem>");
            //sb.AppendLine("        <ProdutoItem>");
            //sb.AppendLine("          <CodigoProduto>4400017</CodigoProduto>");
            //sb.AppendLine("        </ProdutoItem>");
            //sb.AppendLine("      </ProdutosItens>");
            //sb.AppendLine("    </MSG0087>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>Brasil,SC</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0087</CodigoMensagem>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0087>");
            //sb.AppendLine("      <Estado>Brasil,SC</Estado>");
            //sb.AppendLine("      <Moeda>Real</Moeda>");
            //sb.AppendLine("      <ProdutosItens>");
            //sb.AppendLine("        <ProdutoItem>");
            //sb.AppendLine("          <CodigoProduto>4005050</CodigoProduto>");
            //sb.AppendLine("        </ProdutoItem>");
            //sb.AppendLine("        <ProdutoItem>");
            //sb.AppendLine("          <CodigoProduto>1111114</CodigoProduto>");
            //sb.AppendLine("        </ProdutoItem>");
            //sb.AppendLine("      </ProdutosItens>");
            //sb.AppendLine("    </MSG0087>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }
        [Test]
        public void TestarListarSolicitacaoCadastro()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>1048</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0136</CodigoMensagem>");
            //sb.AppendLine("    <LoginUsuario>Claudiney</LoginUsuario>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0136>");
            //sb.AppendLine("      <CodigoRepresentante>1048</CodigoRepresentante>");
            //sb.AppendLine("      <DataInicio>2014-04-03</DataInicio>");
            //sb.AppendLine("      <DataFinal>2014-06-02</DataFinal>");
            //sb.AppendLine("      <Situacao>993520001</Situacao>");
            //sb.AppendLine("    </MSG0136>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>1365</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0136</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0136>");
            sb.AppendLine("      <CodigoRepresentante>1365</CodigoRepresentante>");
            sb.AppendLine("      <DataInicio>2014-06-01</DataInicio>");
            sb.AppendLine("      <DataFinal>2014-07-31</DataFinal>");
            sb.AppendLine("    </MSG0136>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");



            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TestarFamiliaMaterial()
        {
            MSG0034 tst = new MSG0034(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0034")
            {
                CodigoFamiliaMaterial = "666",
                Nome = "Mat Guilhermino2",
                Situacao = 1
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        [Test]
        public void TestarSegmento()
        {
            MSG0026 tst = new MSG0026(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0026")
            {
                CodigoSegmento = "4444",
                Nome = "Segmento Guilhermino2",
                Situacao = 1,
                QuantidadeShowRoom = 4,
                UnidadeNegocio = "33"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            string tst1 = "<?xml version=\"1.0\" encoding=\"utf-8\"?><MENSAGEM>  <CABECALHO>    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>    <NumeroOperacao>Partes e pecas - ICORP</NumeroOperacao>    <CodigoMensagem>MSG0026</CodigoMensagem>  </CABECALHO>  <CONTEUDO>    <MSG0026>      <CodigoSegmento>1200</CodigoSegmento>      <Nome>Partes e pecas - ICORP</Nome>      <UnidadeNegocio>CEN</UnidadeNegocio>      <QuantidadeShowRoom>10</QuantidadeShowRoom>      <Situacao>1</Situacao>    </MSG0026>  </CONTEUDO></MENSAGEM>";


            integ.Postar(usuario, senha, tst1, out teste);//tst.GenerateMessage(), out teste);
        }
        [Test]
        public void TestarParametroGlobal()
        {
            MSG0111 tst = new MSG0111(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0111")
            {
                CodigoClassificacao = "E1D0E59F-1E9E-E311-888D-00155D013E2E",
                CodigoCategoria = "297762E3-439E-E311-888D-00155D013E2E",
                TipoParametroGlobal = 7,
                CodigoUnidadeNegocio = "CEN"
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            string teste = string.Empty;
            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);//tst.GenerateMessage(), out teste);
        }
        [Test]
        public void TestarOrigem()
        {
            MSG0032 tst = new MSG0032(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0032")
            {
                CodigoOrigem = "111",
                Nome = "Origem Guilhermino",
                Situacao = 0,
                SubFamilia = "3410115",
            };

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }

        //[Test]
        //public void aTesteRelacionamentoCanalDuplicidade()
        //{
        //    try
        //    {
        //        Domain.Model.RelacionamentoCanal objRelacionamento = new RelacionamentoCanal(OrganizationName, IsOffline);
        //        objRelacionamento.Canal = new SDKore.DomainModel.Lookup(new Guid("A08C2C69-D300-E411-9420-00155D013D39"),"");
        //        objRelacionamento.Supervisor = new SDKore.DomainModel.Lookup(new Guid("A16E2663-7CEE-E311-9407-00155D013D38"),"");
        //        objRelacionamento.KeyAccount = new SDKore.DomainModel.Lookup(new Guid("6D19FE56-7BEE-E311-9420-00155D013D39"),"");
        //        objRelacionamento.Assistente = new SDKore.DomainModel.Lookup(new Guid("F3BBF5EC-7C03-E411-9420-00155D013D39"),"");
        //        objRelacionamento.DataInicial = new DateTime(2051, 7, 16);
        //        objRelacionamento.DataFinal = new DateTime(2052,12,31);
        //        new Domain.Servicos.RelacionamentoCanalService(OrganizationName, IsOffline).VerificarRegistroDuplicado(objRelacionamento);
        //    }
        //    catch (Exception e)
        //    {
        //        var erro = e.Message;
        //    }
        //}
        [Test]
        public void TesteRelacionamentoCanal()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Representante Intelbras - TESTE ENVIO SH</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0137</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>ToolSystems</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0137>");
            sb.AppendLine("      <Nome>Bruno Alvaro Machado - DEMO</Nome>");
            sb.AppendLine("      <CodigoConta>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <CodigoRepresentante>4000</CodigoRepresentante>");
            sb.AppendLine("      <CodigoAssistente>99</CodigoAssistente>");
            sb.AppendLine("      <CodigoAssistenteCRM>6de6c975-3aeb-e311-9407-00155d013d38</CodigoAssistenteCRM>");
            sb.AppendLine("      <CodigoSupervisor>b56e2663-7cee-e311-9407-00155d013d38</CodigoSupervisor>");
            sb.AppendLine("      <DataInicial>2014-08-01</DataInicial>");
            sb.AppendLine("      <DataFinal>2014-11-01</DataFinal>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("    </MSG0137>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>CONTATO: 01b970c2-7dee-e311-940e-00155d0</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0137</CodigoMensagem>");
            //sb.AppendLine("    <LoginUsuario>ADRIANO</LoginUsuario>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0137>");
            //sb.AppendLine("      <CodigoRelacionamentoCanal>690B3816-040D-E411-9420-00155D013D39</CodigoRelacionamentoCanal>");
            //sb.AppendLine("      <Nome>teste messi</Nome>");
            //sb.AppendLine("      <CodigoConta>88FA38A0-9F03-E411-9420-00155D013D39</CodigoConta>");
            //sb.AppendLine("      <CodigoRepresentante>1243</CodigoRepresentante>");
            //sb.AppendLine("      <CodigoAssistente>49</CodigoAssistente>");
            //sb.AppendLine("      <CodigoSupervisor>A16E2663-7CEE-E311-9407-00155D013D38</CodigoSupervisor>");
            //sb.AppendLine("      <DataInicial>2014-06-30</DataInicial>");
            //sb.AppendLine("      <DataFinal>2014-10-28</DataFinal>");
            //sb.AppendLine("      <Situacao>0</Situacao>");
            //sb.AppendLine("    </MSG0137>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");



            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteListarRelacionamentoCanal()
        {
            //MSG0124 tst = new MSG0124(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0124")
            //{
            //    LoginUsuario = "AAAAAA"
            //};
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>08F14E0E-4B9E-E311-888D-00155D013E2E</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0124</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0124>");
            sb.AppendLine("      <Canal>08F14E0E-4B9E-E311-888D-00155D013E2E</Canal>");
            sb.AppendLine("      <Data>2014-05-21</Data>");
            sb.AppendLine("    </MSG0124>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0121()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>D13E633E-F5B8-E311-9207-00155D013D19</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0121</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0121>");
            sb.AppendLine("      <CodigoTarefa>52C931E4-91F1-E311-9407-00155D013D38</CodigoTarefa>");
            sb.AppendLine("    </MSG0121>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }


        [Test]
        public void TesteMsg0124()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>08F14E0E-4B9E-E311-888D-00155D013E2E</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0124</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0124>");
            sb.AppendLine("      <Canal>d66a88b1-bc0d-e411-9420-00155d013d39</Canal>");
            sb.AppendLine("      <Data>2014-08-21</Data>");
            sb.AppendLine("    </MSG0124>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }


        [Test]
        public void TesteListarLinhaCorteDistribuidor()
        {
            //MSG0078 tst = new MSG0078(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0078")
            //{
            //    //CodigoUnidadeNegocio = "TER",
            //    //ChaveIntegracaoEstado = "Brasil,sp"
            //    CodigoConta = "B191D1F8-80D8-E311-8F6C-00155D013E44"
            //};
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>TER</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0078</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0078>");
            sb.AppendLine("      <CodigoConta>f7e8e1af-d500-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <ChaveIntegracaoEstado>Brasil,SC</ChaveIntegracaoEstado>");
            sb.AppendLine("    </MSG0078>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteListarLinhaCorteRevenda()
        {
            //MSG0079 tst = new MSG0079(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0079")
            //{
            //    //CodigoUnidadeNegocio = "CEN",
            //    CodigoCategoria = "408DF895-1C9E-E311-888D-00155D013E2E",
            //    CodigoConta = "B191D1F8-80D8-E311-8F6C-00155D013E44"
            //};
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>TER</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0079</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0079>");
            //sb.AppendLine("      <CodigoConta>A4511415-26C7-E411-BFBC-00155D013E80</CodigoConta>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("    </MSG0079>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");



            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>TER</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0079</CodigoMensagem>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0079>");
            //sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            //sb.AppendLine("    </MSG0079>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }
        [Test]
        public void TesteListarUsuario()
        {
            //MSG0127 tst = new MSG0127(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0127")
            //{
            //    LoginUsuario = "AAAAAA",
            //    CodigoUsuario = "FF3CBD6F-8E9D-E311-888D-00155D013E2E"
            //};
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Listar usuario</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0127</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>teste</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0127>");
            sb.AppendLine("      <CodigoUsuario>746CF3E0-0FBF-E311-8BD1-00155D013E44</CodigoUsuario>");
            //sb.AppendLine("      <CodigoUsuario>DF9E69AD-F0D5-E311-9410-00155D013D36</CodigoUsuario>");
            sb.AppendLine("      <TipoObjetoUsuario>systemuser</TipoObjetoUsuario>");
            sb.AppendLine("    </MSG0127>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TestarProduto()
        {
            MSG0088 tst = new MSG0088(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0088");

            //<CodigoProduto>1000004</CodigoProduto>
            tst.CodigoProduto = "333333";

            //<Nome>AGENDA INTELBRAS PEQUININA...</Nome>
            tst.Nome = "GUILHEMINO";

            //<Descricao>AGENDA INTELBRAS PEQUININA...</Descricao>
            tst.Descricao = "GUILHERMINO...";

            //<PesoEstoque>0.11</PesoEstoque>
            tst.PesoEstoque = (decimal)0.11;

            //<Situacao>0</Situacao>
            tst.Situacao = 0;

            //<TipoProduto>11</TipoProduto>
            tst.TipoProduto = 11;

            //<NaturezaProduto>993520000</NaturezaProduto>
            tst.NaturezaProduto = 993520000;

            //<GrupoEstoque>0</GrupoEstoque>
            tst.GrupoEstoque = 0;

            //<UnidadeNegocio>ADM</UnidadeNegocio>
            tst.UnidadeNegocio = "ADM";

            //<Segmento>1200</Segmento>
            tst.Segmento = "1200";

            //<Familia>12000</Familia>
            tst.Familia = "12000";

            //<SubFamilia>1200000</SubFamilia>
            tst.SubFamilia = "1200000";

            //<Origem>12000000</Origem>
            tst.Origem = "12000000";

            //<UnidadeMedida>PC</UnidadeMedida>
            tst.UnidadeMedida = "PC";

            //<GrupoUnidadeMedida>Unidade Padrão</GrupoUnidadeMedida>
            tst.GrupoUnidadeMedida = "Unidade Padrão";

            //<FamiliaMaterial>40002150</FamiliaMaterial>
            tst.FamiliaMaterial = "Terc";

            //<FamiliaComercial>12000000</FamiliaComercial>
            tst.FamiliaComercial = "12000000";

            //<ListaPreco>Lista Padrão</ListaPreco>
            tst.ListaPreco = "Lista Padrão";

            //<Moeda>Real</Moeda>
            tst.Moeda = "Real";

            //<QuantidadeDecimal>0</QuantidadeDecimal>
            tst.QuantidadeDecimal = 0;

            //<CustoAtual>0.0</CustoAtual>
            tst.CustoAtual = (decimal)0.0;

            //<PrecoLista>0.0</PrecoLista>
            tst.PrecoLista = (decimal)0.0;

            //<Fabricante />
            //tst.Fabricante

            //<NumeroPecaFabricante />


            //<VolumeEstoque>0.0</VolumeEstoque>
            tst.VolumeEstoque = (decimal)0.0;

            //<ComplementoProduto />

            //<URL />

            //<QuantidadeDisponivel>0.0</QuantidadeDisponivel>
            tst.QuantidadeDisponivel = (decimal)0.0;

            //<ExigeTreinamento>false</ExigeTreinamento>
            tst.ExigeTreinamento = false;

            //<Fornecedor />

            //<CustoPadrao>0.0</CustoPadrao>
            tst.CustoPadrao = (decimal)0.0;

            //<RebateAtivado>false</RebateAtivado>
            tst.RebateAtivado = false;

            //tst.Nome = "Produto Guilhermino";

            //tst.CustoAtual = 0;
            //tst.CustoPadrao = 0;
            //tst.ExigeTreinamento = false;
            //tst.Descricao = "Produto do Guilhermino";
            //tst.Familia = "2525";
            //tst.FamiliaComercial = "3333";
            //tst.FamiliaMaterial = "333";
            //tst.ListaPreco = "Lista Padrão";


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao> INTEG CENTRAL DE ILUMINAÇÃO 24V 17 CIRC</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0088</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0088>");
            sb.AppendLine("      <CodigoProduto>4653000</CodigoProduto>");
            sb.AppendLine("      <Nome>INTEG CENTRAL DE ILUMINAÇÃO 24V 17 CIRCUITOS-2000W-60Hz-127</Nome>");
            //sb.AppendLine("      <Descricao>Paulo - INTEG CENTRAL DE ILUMINAÇÃO 24V 17 </Descricao>");
            //sb.AppendLine("      <PesoEstoque>21.00000</PesoEstoque>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <TipoProduto>11</TipoProduto>");
            sb.AppendLine("      <NaturezaProduto>993520001</NaturezaProduto>");
            sb.AppendLine("      <GrupoEstoque>40</GrupoEstoque>");
            sb.AppendLine("      <UnidadeNegocio>FIR</UnidadeNegocio>");
            sb.AppendLine("      <Segmento>3119</Segmento>");
            sb.AppendLine("      <Familia>31191</Familia>");
            sb.AppendLine("      <SubFamilia>3119101</SubFamilia>");
            sb.AppendLine("      <Origem>31191016</Origem>");
            sb.AppendLine("      <UnidadeMedida>PC</UnidadeMedida>");
            sb.AppendLine("      <GrupoUnidadeMedida>Unidade Padrão</GrupoUnidadeMedida>");
            sb.AppendLine("      <FamiliaMaterial>40006130</FamiliaMaterial>");
            sb.AppendLine("      <FamiliaComercial>31191016</FamiliaComercial>");
            sb.AppendLine("      <ListaPreco>Lista Padrão</ListaPreco>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <QuantidadeDecimal>0</QuantidadeDecimal>");
            sb.AppendLine("      <Fabricante>Paulo Fabrica</Fabricante>");
            sb.AppendLine("      <NumeroPecaFabricante>pa2015</NumeroPecaFabricante>");
            sb.AppendLine("      <VolumeEstoque>3.00</VolumeEstoque>");
            sb.AppendLine("      <ComplementoProduto>9999</ComplementoProduto>");
            sb.AppendLine("      <URL>http://www.intelbras.com.br</URL>");
            sb.AppendLine("      <QuantidadeDisponivel>100.00</QuantidadeDisponivel>");
            sb.AppendLine("      <ExigeTreinamento>false</ExigeTreinamento>");
            sb.AppendLine("      <Fornecedor>fornecedor</Fornecedor>");
            sb.AppendLine("      <CustoPadrao>0.0000</CustoPadrao>");
            sb.AppendLine("      <RebateAtivado>false</RebateAtivado>");
            sb.AppendLine("      <ConsiderarOrcamentoMeta>true</ConsiderarOrcamentoMeta>");
            sb.AppendLine("      <FaturamentoOutroProduto>false</FaturamentoOutroProduto>");
            sb.AppendLine("      <QuantidadeMultipla>1.00</QuantidadeMultipla>");
            sb.AppendLine("      <ShowRoom>false</ShowRoom>");
            sb.AppendLine("      <AliquotaIPI>3.00</AliquotaIPI>");
            sb.AppendLine("      <NCM>85044060</NCM>");
            sb.AppendLine("      <EAN>7896637665209</EAN>");
            sb.AppendLine("      <PossuiSubstituto>false</PossuiSubstituto>");
            sb.AppendLine("      <EKit>false</EKit>");
            sb.AppendLine("      <ComercializadoForaKit>false</ComercializadoForaKit>");
            sb.AppendLine("      <PoliticaPosVendas>993520000</PoliticaPosVendas>");
            sb.AppendLine("      <TempoGarantia>12</TempoGarantia>");
            sb.AppendLine("    </MSG0088>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TestarEnvioConta()
        {

            Intelbras.CRM2013.Domain.Model.Conta conta = new Conta(OrganizationName, IsOffline);

            conta.Agencia = "Teste";
            conta.AgenteRetencao = false;
            conta.ApuracaoBeneficiosCompromissos = 0;
            conta.Banco = "BancoTeste";
            conta.CalculaMulta = false;
            //conta.Classificacao = new SDKore.DomainModel.Lookup(

            MSG0072 tst = new MSG0072(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0072");
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            Domain.Integracao.MSG0072 msgConta = new Domain.Integracao.MSG0072(OrganizationName, IsOffline);

            msgConta.Enviar(conta);

            //String teste = String.Empty;
            //integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);



        }



        [Test]
        public void TesteAtualizarTarefa()
        {
            //MSG0115 tst = new MSG0115(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0115")
            //{
            //    CodigoTarefa = "B28449A6-74D7-E311-8F6C-00155D013E44",
            //    Situacao = 0,
            //    Resultado = 993520002,
            //    Descricao = "Teste de integração",
            //    DataHoraTerminoEsperada = new DateTime(2014, 6, 1),
            //    Duracao = 50
            //};
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>D13E633E-F5B8-E311-9207-00155D013D19</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0115</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0115>");
            sb.AppendLine("      <CodigoTarefa>A0AC27CB-78F8-E311-9412-00155D01420F</CodigoTarefa>");
            sb.AppendLine("      <Descricao>Descrição Mario Teste2</Descricao>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Resultado>993520001</Resultado>");
            sb.AppendLine("      <DataHoraTerminoEsperada>2014-05-20T09:50:28</DataHoraTerminoEsperada>");
            sb.AppendLine("      <Duracao>808</Duracao>");
            sb.AppendLine("    </MSG0115>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("  <CABECALHO>");
            //sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            //sb.AppendLine("    <NumeroOperacao>D13E633E-F5B8-E311-9207-00155D013D19</NumeroOperacao>");
            //sb.AppendLine("    <CodigoMensagem>MSG0115</CodigoMensagem>");
            //sb.AppendLine("  </CABECALHO>");
            //sb.AppendLine("  <CONTEUDO>");
            //sb.AppendLine("    <MSG0115>");
            //sb.AppendLine("      <CodigoTarefa>FD746923-68F1-E311-91F5-00155D013E44</CodigoTarefa>");
            //sb.AppendLine("      <Descricao>Aprovação de Solicitação</Descricao>");
            //sb.AppendLine("      <Situacao>1</Situacao>");
            //sb.AppendLine("      <Resultado>993520001</Resultado>");
            //sb.AppendLine("      <DataHoraTerminoEsperada>2014-05-21T09:08:28</DataHoraTerminoEsperada>");
            //sb.AppendLine("      <Duracao>100</Duracao>");
            //sb.AppendLine("    </MSG0115>");
            //sb.AppendLine("  </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");




            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteListarTarefa()
        {

            //};
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d1bcbcd8-d500-e411-9420-00155d013d39</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0116</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>BERNADETE </LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0116>");
            sb.AppendLine("      <CodigoObjeto>d1bcbcd8-d500-e411-9420-00155d013d39</CodigoObjeto>");
            sb.AppendLine("      <TipoObjeto>contact</TipoObjeto>");
            sb.AppendLine("      <TipoAtividade>b465fb22-72ed-e311-9407-00155d013d38</TipoAtividade>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("    </MSG0116>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;
            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        //[Test]
        //public void TesteObterValorProduto()
        //{
        //    MSG0101 tst = new MSG0101(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0101")
        //    {   

        //        Conta = "B191D1F8-80D8-E311-8F6C-00155D013E44",
        //        ProdutosItens

        //    };
        //    Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
        //    String teste = String.Empty;
        //    integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        //}


        [Test]
        public void TesteEnvioContaPlugin()
        {
            Conta objModel = new Intelbras.CRM2013.Domain.Servicos.ContaService(this.OrganizationName, this.IsOffline).
                BuscaConta(new Guid("63581F46-67D8-E311-8F6C-00155D013E44"));
            Domain.Integracao.MSG0072 integ = new Domain.Integracao.MSG0072(this.OrganizationName, this.IsOffline);
            string str1 = String.Empty, str2 = String.Empty, str3 = String.Empty;
            string retorrno = integ.Enviar(objModel, ref str1, ref str2, ref str3);

        }

        [Test]
        public void TesteEnvioContatoPlugin()
        {
            Contato objModel = new Intelbras.CRM2013.Domain.Servicos.ContatoService(this.OrganizationName, this.IsOffline).BuscaContato(new Guid("A719FE56-7BEE-E311-9420-00155D013D39"));
            Domain.Integracao.MSG0058 integ = new Domain.Integracao.MSG0058(this.OrganizationName, this.IsOffline);
            string retorrno = integ.Enviar(objModel);

        }


        [Test]
        public void ParecerTeste()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Nome do parecer</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0149</CodigoMensagem>");
            sb.AppendLine("      </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0149>");
            sb.AppendLine("      <CodigoParecer>834656ca-c518-e411-9233-00155d013e44</CodigoParecer>");
            sb.AppendLine("      <NomeParecer>DEMO Parecer do Gerente Nacional de Vendas alt</NomeParecer>");
            sb.AppendLine("      <CodigoRepresentante>1008</CodigoRepresentante>");
            sb.AppendLine("      <CodigoConta>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <CodigoTarefa>D37F373D-B518-E411-9233-00155D013E44</CodigoTarefa>");
            sb.AppendLine("      <TipoParecer>993520001</TipoParecer>");
            sb.AppendLine("      <Distribuidores>Distribuidores teste alt</Distribuidores>");
            sb.AppendLine("      <DescricaoImpactoAbertura>Descrição do impacto da abertura alt</DescricaoImpactoAbertura>");
            sb.AppendLine("      <DescricaoDemandaProduto>Descrição da demanda do produto alt</DescricaoDemandaProduto>");
            sb.AppendLine("      <DefesaAbertura>Descrição da defesa de abertura alt</DefesaAbertura>");
            sb.AppendLine("      <ValorFaturamentoRegiao>10500000.46</ValorFaturamentoRegiao>");
            sb.AppendLine("      <PorcentagemRegiao>20.52</PorcentagemRegiao>");
            sb.AppendLine("      <ValorPotencialRegiao>1500000.65</ValorPotencialRegiao>");
            sb.AppendLine("      <PorcentagemPotencialRegiao>25.35</PorcentagemPotencialRegiao>");
            sb.AppendLine("      <VolumeAnual>1000000.75</VolumeAnual>");
            sb.AppendLine("      <PorcentagemFaturamento>10.55</PorcentagemFaturamento>");
            sb.AppendLine("      <PrevisaoLinhaCorte>Descrição da previsão de linha de corte alt</PrevisaoLinhaCorte>");
            sb.AppendLine("      <ConflitoDistribuidores>Descrição de conflito com outros distribuidores alt</ConflitoDistribuidores>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>259a8e4f-15e9-e311-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("    </MSG0149>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            string teste = string.Empty;
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            integ.Postar(usuario, senha, sb.ToString(), out teste);


        }
        [Test]
        public void tarefaparecer()
        {
            Guid tarefa = new Guid("D94FACBB-C20D-E411-9233-00155D013E44");

            MSG0150 tst = new MSG0150(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0150")
            {
                CodigoTarefa = tarefa.ToString()
            };

            string teste = string.Empty;
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        [Test]
        public void aObterParecer()
        {
            Guid parecer = new Guid("C59FEAC8-C20D-E411-9233-00155D013E44");

            MSG0151 tst = new MSG0151(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0151")
            {
                CodigoParecer = parecer.ToString()
            };

            string teste = string.Empty;
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }
        //7AB00882-6AE6-E311-B278-00155D01330E
        [Test]
        public void TesteMsg0074()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>123456</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0074</CodigoMensagem>");
            sb.AppendLine("      </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0074>");
            //            sb.AppendLine("      <CodigoSolicitacao>d17f373d-b518-e411-9233-00155d013e44</CodigoSolicitacao>");
            sb.AppendLine("      <NomeSolicitacao>Teste</NomeSolicitacao>");
            sb.AppendLine("      <DescricaoSolicitacao>Solicitação da CONTA: 02.071.489/0001-18 </DescricaoSolicitacao>");
            sb.AppendLine("      <CodigoTipoSolicitacao>A2751D98-F94E-E411-9424-00155D013D3A</CodigoTipoSolicitacao>");
            sb.AppendLine("      <Necessidade>993520000</Necessidade>");
            sb.AppendLine("      <CodigoConta>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <CodigoRepresentante>4000</CodigoRepresentante>");
            sb.AppendLine("      <CodigoSupervisor>6de6c975-3aeb-e311-9407-00155d013d38</CodigoSupervisor>");
            sb.AppendLine("      <SituacaoSolicitacao>993520001</SituacaoSolicitacao>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Proprietario>d86a88b1-bc0d-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("    </MSG0074>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            string teste = string.Empty;
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);

            integ.Postar(usuario, senha, sb.ToString(), out teste);


        }

        [Test]
        public void TesteMsg0077()
        {
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>08F14E0E-4B9E-E311-888D-00155D013E2E</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0077</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0077>");
            sb.AppendLine("      <CodigoConta>08F14E0E-4B9E-E311-888D-00155D013E2E</CodigoConta>");
            sb.AppendLine("    </MSG0077>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");




            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }



        [Test]
        public void TesteMsg0100()
        {
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>3266b3cd-b739-e411-9421-00155d013d39</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0100</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>Ricardo</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0100>");
            sb.AppendLine("      <Conta>3266b3cd-b739-e411-9421-00155d013d39</Conta>");
            sb.AppendLine("      <Exclusivo>0</Exclusivo>");
            sb.AppendLine("      <Bloqueado>0</Bloqueado>");
            sb.AppendLine("    </MSG0100>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");



            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteMsg0101()
        {
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            #region Msg0101


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>647a5648-5843-e411-9424-00155d013d3a</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0101</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>SIDNEI MARCOS</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0101>");
            sb.AppendLine("      <Conta>f7e8e1af-d500-e411-9420-00155d013d39</Conta>");
            sb.AppendLine("      <ProdutosItens>");

            sb.AppendLine("      <ProdutoItem>");
            sb.AppendLine("      <CodigoProduto>4612026</CodigoProduto>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <Quantidade>1</Quantidade>");
            sb.AppendLine("      <TipoPortfolio>993520001</TipoPortfolio>");
            sb.AppendLine("      <CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>");
            sb.AppendLine("      <CodigoFamiliaComercial>31181016</CodigoFamiliaComercial>");
            sb.AppendLine("      <CodigoEstabelecimento>104</CodigoEstabelecimento>");
            sb.AppendLine("      </ProdutoItem>");


            sb.AppendLine("      </ProdutosItens>");
            sb.AppendLine("    </MSG0101>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("  </MENSAGEM>");


            #endregion


            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0162()
        {
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            //sb.AppendLine("<MENSAGEM>");
            //sb.AppendLine("    <CABECALHO>");
            //sb.AppendLine("        <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>");
            //sb.AppendLine("        <NumeroOperacao>647a5648-5843-e411-9424-00155d013d3a</NumeroOperacao>");
            //sb.AppendLine("        <CodigoMensagem>MSG0162</CodigoMensagem>");
            //sb.AppendLine("        <LoginUsuario>SIDNEI MARCOS</LoginUsuario>");
            //sb.AppendLine("    </CABECALHO>");
            //sb.AppendLine("    <CONTEUDO>");
            //sb.AppendLine("        <MSG0162>");
            //sb.AppendLine("            <CodigoArquivoSellout>5B5BBE5C-F7C7-E411-BFBC-00155D013E80</CodigoArquivoSellout>");
            //sb.AppendLine("            <Nome>2</Nome>");
            //sb.AppendLine("            <CodigoConta>4BF259A1-9008-E411-9420-00155D013D39</CodigoConta>");
            //sb.AppendLine("            <DataEnvio>2014-06-17T22:07:59</DataEnvio>");
            //sb.AppendLine("            <DataProcessamento>2015-03-11T23:59:59</DataProcessamento>");
            //sb.AppendLine("            <StatusProcessamento>2</StatusProcessamento>");
            //sb.AppendLine("            <Situacao>2</Situacao>");
            //sb.AppendLine("            <Proprietario>259a8e4f-15e9-e311-9420-00155d013d39</Proprietario>");
            //sb.AppendLine("            <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("        </MSG0162>");
            //sb.AppendLine("    </CONTEUDO>");
            //sb.AppendLine("</MENSAGEM>");

            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>f7e8e1af-d500-e411-9420-00155d013d39</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0162</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>BERNADETE </LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0162>");
            sb.AppendLine("<CodigoArquivoSellout>a6d50f95-49d2-e411-9407-00155de86406</CodigoArquivoSellout>");
            sb.AppendLine("<Nome>testando inativação - agora vai</Nome>");
            sb.AppendLine("<CodigoConta>f7e8e1af-d500-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("<DataEnvio>2015-03-24T14:10:09</DataEnvio>");
            sb.AppendLine("<StatusProcessamento>2</StatusProcessamento>");
            sb.AppendLine("<Situacao>1</Situacao>");
            sb.AppendLine("<Proprietario>f9e8e1af-d500-e411-9420-00155d013d39</Proprietario>");
            sb.AppendLine("<TipoProprietario>team</TipoProprietario>");
            sb.AppendLine("    </MSG0162>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0163()
        {
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("    <CABECALHO>");
            sb.AppendLine("        <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>");
            sb.AppendLine("        <NumeroOperacao>647a5648-5843-e411-9424-00155d013d3a</NumeroOperacao>");
            sb.AppendLine("        <CodigoMensagem>MSG0163</CodigoMensagem>");
            sb.AppendLine("        <LoginUsuario>SIDNEI MARCOS</LoginUsuario>");
            sb.AppendLine("    </CABECALHO>");
            sb.AppendLine("    <CONTEUDO>");
            sb.AppendLine("        <MSG0163>");
            sb.AppendLine("            <CodigoConta>4BF259A1-9008-E411-9420-00155D013D39</CodigoConta>");
            //           sb.AppendLine("            <StatusProcessamento>993520000</StatusProcessamento>");
            //            sb.AppendLine("            <DataEnvioInicio>2014-10-11T07:38:56</DataEnvioInicio>");
            //           sb.AppendLine("            <DataEnvioFim>2015-03-10T23:59:59</DataEnvioFim>");
            sb.AppendLine("        </MSG0163>");
            sb.AppendLine("    </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");





            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteMsg0072()
        {
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>COMERCIAL INDAIATUBA DE PRODUTOS ELETROM</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0072</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0072>");
            sb.AppendLine("      <CodigoConta>bc536b3f-5b6e-e411-9410-00155d013e7f</CodigoConta>");
            sb.AppendLine("      <NomeRazaoSocial>COMERCIAL INDAIATUBA DE PRODUTOS ELETROMECANICOS LTDA - EPP</NomeRazaoSocial>");
            sb.AppendLine("      <NomeFantasia>BRASTECH SOLUÇÕES EM SEGURANÇA</NomeFantasia>");
            sb.AppendLine("      <NomeAbreviado>059011250001</NomeAbreviado>");
            sb.AppendLine("      <TipoRelacao>993520000</TipoRelacao>");
            sb.AppendLine("      <Telefone>1933182381</Telefone>");
            sb.AppendLine("      <TelefoneAlternativo>1933182382</TelefoneAlternativo>");
            sb.AppendLine("      <Email>rodrigo@brastechsolucoes.com.br</Email>");
            sb.AppendLine("      <Site>http://www.brastechsolucoes.com.br </Site>");
            sb.AppendLine("      <Natureza>993520000</Natureza>");
            sb.AppendLine("      <CNPJ>05901125000106</CNPJ>");
            sb.AppendLine("      <InscricaoEstadual>353213260110</InscricaoEstadual>");
            sb.AppendLine("      <InscricaoMunicipal>1219450</InscricaoMunicipal>");
            sb.AppendLine("      <FormaTributacao>993520002</FormaTributacao>");
            sb.AppendLine("      <CoberturaGeografica>A EMPRESA FICA NA REGIÃO DE CAMPINAS NA CIDADE DE INDAIATUBA - SP");
            sb.AppendLine("</CoberturaGeografica>");
            sb.AppendLine("      <DataConstituicao>2003-12-24</DataConstituicao>");
            sb.AppendLine("      <DistribuicaoUnicaFonteReceita>false</DistribuicaoUnicaFonteReceita>");
            sb.AppendLine("      <QualificadoTreinamento>SIM, NOSSOS TECNICOS MARCAM CURSOS EM GRUPO OU INDIVIDUAL P/ EMPRESAS QUE NECESSITAM DE APOIO.</QualificadoTreinamento>");
            sb.AppendLine("      <Exclusividade>false</Exclusividade>");
            sb.AppendLine("      <Historico>A EMPRESA ESTÁ ABERTA DESDE 24/12/2003 </Historico>");
            sb.AppendLine("      <MetodoComercializacao>VENDA BALCAO VENDEDOR EXTERNO E EMAILS</MetodoComercializacao>");
            sb.AppendLine("      <NumeroFuncionarios>5</NumeroFuncionarios>");
            sb.AppendLine("      <NumeroColaboradoresAreaTecnica>3</NumeroColaboradoresAreaTecnica>");
            sb.AppendLine("      <NumeroRevendasAtivas>92</NumeroRevendasAtivas>");
            sb.AppendLine("      <NumeroRevendasInativas>17</NumeroRevendasInativas>");
            sb.AppendLine("      <NumeroTecnicosSuporte>3</NumeroTecnicosSuporte>");
            sb.AppendLine("      <NumeroVendedores>5</NumeroVendedores>");
            sb.AppendLine("      <ParticipaProgramaCanais>993520000</ParticipaProgramaCanais>");
            sb.AppendLine("      <PerfilRevendasDistribuidor>ATENDEMOS TODAS AS MODALIDADES ACIMA.</PerfilRevendasDistribuidor>");
            sb.AppendLine("      <PossuiFiliais>993520001</PossuiFiliais>");
            sb.AppendLine("      <PrazoMedioCompra>30</PrazoMedioCompra>");
            sb.AppendLine("      <PrazoMedioVenda>30</PrazoMedioVenda>");
            sb.AppendLine("      <RamoAtividadeEconomica>DISTRIBUIÇÃO DE PROD DE SEG ELETRONICA</RamoAtividadeEconomica>");
            sb.AppendLine("      <SistemaGestao>DATAPLACE SYMPHONY</SistemaGestao>");
            sb.AppendLine("      <ValorMedioCompra>123000.00</ValorMedioCompra>");
            sb.AppendLine("      <ValorMedioVenda>246000.00</ValorMedioVenda>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <Classificacao>026E4C24-6BED-E311-9420-00155D013D39</Classificacao>");
            sb.AppendLine("      <SubClassificacao>0bbbb193-6eed-e311-9407-00155d013d38</SubClassificacao>");
            sb.AppendLine("      <NivelPosVenda>37e3a262-75ed-e311-9407-00155d013d38</NivelPosVenda>");
            sb.AppendLine("      <ApuracaoBeneficio>993520000</ApuracaoBeneficio>");
            sb.AppendLine("      <TipoConstituicao>993520001</TipoConstituicao>");
            sb.AppendLine("      <Proprietario>259a8e4f-15e9-e311-9420-00155d013d39</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            sb.AppendLine("      <TipoConta>993520000</TipoConta>");
            sb.AppendLine("      <EnderecoPrincipal>");
            sb.AppendLine("        <TipoEndereco>3</TipoEndereco>");
            sb.AppendLine("        <CEP>13334200</CEP>");
            sb.AppendLine("        <Logradouro>RUA ALMIRANTE TAMANDARE </Logradouro>");
            sb.AppendLine("        <Numero>143</Numero>");
            sb.AppendLine("        <Bairro>CIDADE NOVA II</Bairro>");
            sb.AppendLine("        <NomeCidade>INDAIATUBA</NomeCidade>");
            sb.AppendLine("        <Cidade>INDAIATUBA,SP,Brasil</Cidade>");
            sb.AppendLine("        <UF>SP</UF>");
            sb.AppendLine("        <Estado>Brasil,SP</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("      </EnderecoPrincipal>");
            sb.AppendLine("      <EnderecoCobranca>");
            sb.AppendLine("        <TipoEndereco>993520000</TipoEndereco>");
            sb.AppendLine("        <CEP>13334200</CEP>");
            sb.AppendLine("        <Logradouro>RUA ALMIRANTE TAMANDARE </Logradouro>");
            sb.AppendLine("        <Numero>143</Numero>");
            sb.AppendLine("        <Bairro>CIDADE NOVA II</Bairro>");
            sb.AppendLine("        <NomeCidade>INDAIATUBA</NomeCidade>");
            sb.AppendLine("        <Cidade>INDAIATUBA,SP,Brasil</Cidade>");
            sb.AppendLine("        <UF>SP</UF>");
            sb.AppendLine("        <Estado>Brasil,SP</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("      </EnderecoCobranca>");
            sb.AppendLine("    </MSG0072>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [Test]
        public void TesteEnvio0139()
        {
            ProdutoEstabelecimento objModel = new Intelbras.CRM2013.Domain.Servicos.
                ProdutoEstabelecimentoService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("A10645F9-07E1-E311-88A2-00155D013E44"));
            Domain.Integracao.MSG0139 integ = new Domain.Integracao.MSG0139(this.OrganizationName, this.IsOffline);
            string retorno = integ.Enviar(objModel);

        }

        [Test]
        public void TesteEnvio0056()
        {
            AcessoExtranet objModel = new Intelbras.CRM2013.Domain.Servicos.
                AcessoExtranetService(this.OrganizationName, this.IsOffline).ObterPor(new Guid("B69EC6A5-5CA5-E311-888D-00155D013E2E"));
            Domain.Integracao.MSG0056 integ = new Domain.Integracao.MSG0056(this.OrganizationName, this.IsOffline);
            string retorno = integ.Enviar(objModel);

        }

        [Test]
        public void TesteMSg0088()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>Resistor SMD 0603 1/10 W ±1% 49,9 kR</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0088</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0088>");
            sb.AppendLine("      <CodigoProduto>1010004</CodigoProduto>");
            sb.AppendLine("      <Nome>Resistor SMD 0603 1/10 W ±1% 49,9 kR</Nome>");
            sb.AppendLine("      <Descricao>Resistor SMD 0603 1/10 W ±1% 49,9 kR</Descricao>");
            sb.AppendLine("      <PesoEstoque>0.0000100000</PesoEstoque>");
            sb.AppendLine("      <Situacao>0</Situacao>");
            sb.AppendLine("      <TipoProduto>11</TipoProduto>");
            sb.AppendLine("      <NaturezaProduto>993520000</NaturezaProduto>");
            sb.AppendLine("      <GrupoEstoque>10</GrupoEstoque>");
            sb.AppendLine("      <UnidadeNegocio>CEN</UnidadeNegocio>");
            sb.AppendLine("      <Segmento>1200</Segmento>");
            sb.AppendLine("      <Familia>12000</Familia>");
            sb.AppendLine("      <SubFamilia>1200000</SubFamilia>");
            sb.AppendLine("      <Origem>12000000</Origem>");
            sb.AppendLine("      <UnidadeMedida>PC</UnidadeMedida>");
            sb.AppendLine("      <GrupoUnidadeMedida>Unidade Padrão</GrupoUnidadeMedida>");
            sb.AppendLine("      <FamiliaMaterial>10118010</FamiliaMaterial>");
            sb.AppendLine("      <FamiliaComercial>12000000</FamiliaComercial>");
            sb.AppendLine("      <ListaPreco>Lista Padrão</ListaPreco>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <QuantidadeDecimal>0</QuantidadeDecimal>");
            sb.AppendLine("      <VolumeEstoque>0.0000000000</VolumeEstoque>");
            sb.AppendLine("      <QuantidadeDisponivel>0.0000000000</QuantidadeDisponivel>");
            sb.AppendLine("      <ExigeTreinamento>false</ExigeTreinamento>");
            sb.AppendLine("      <CustoPadrao>0</CustoPadrao>");
            sb.AppendLine("      <RebateAtivado>false</RebateAtivado>");
            sb.AppendLine("      <ConsiderarOrcamentoMeta>false</ConsiderarOrcamentoMeta>");
            sb.AppendLine("      <FaturamentoOutroProduto>false</FaturamentoOutroProduto>");
            sb.AppendLine("      <QuantidadeMultipla>1.0000000000</QuantidadeMultipla>");
            sb.AppendLine("    </MSG0088>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteEnvioProduto()
        {
            Product prod = new Domain.Servicos.ProdutoService(OrganizationName, IsOffline).BuscaPorCodigo("1010037");

            Domain.Integracao.MSG0088 msg = new Domain.Integracao.MSG0088(OrganizationName, IsOffline);

            string teste = msg.Enviar(prod);

        }

        [Test]
        public void TesteMSg0141()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>f7e8e1af-d500-e411-9420-00155d013d39</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0141</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>BERNADETE </LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0141>");
            sb.AppendLine("      <CodigoConta>f7e8e1af-d500-e411-9420-00155d013d39</CodigoConta>");
            sb.AppendLine("      <PassivelSolicitacao>true</PassivelSolicitacao>");
            sb.AppendLine("      <PossuiControleContaCorrente>993520001</PossuiControleContaCorrente>");
            sb.AppendLine("    </MSG0141>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");


            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }


        [Test]
        public void TesteMSg0142()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d773d4df-ccd6-e311-940c-00155d013d31</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0142</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0142>");
            sb.AppendLine("      <CodigoBeneficio>EB73D4DF-CCD6-E311-940C-00155D013D31</CodigoBeneficio>");
            sb.AppendLine("    </MSG0142>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteMSG0107RegistraDenuncia()
        {

            MSG0107 msg107 = new MSG0107(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0107")
            {

                IdentidadeEmissor = "95061229-FF31-4FD1-A875-96A98D67280C",
                NumeroOperacao = "Solicitacao Cadastro Tipo",
                CodigoCanalDenunciado = "d66a88b1-bc0d-e411-9420-00155d013d39",
                //Update - Homolog
                CodigoDenuncia = "08007cdb-9d27-e411-9235-00155d013e44",
                CodigoCanalDenunciante = "F42E19AD-76EE-E311-9407-00155D013D38",
                NomeDenunciante = "Representante Intelbras",
                NomeDenuncia = "Denuncia de: tipo teste",
                Descricao = "Descricao Denuncia Teste",
                CodigoTipoDenuncia = "024688b6-aff0-e311-9420-00155d013d39",//"86D59277-AFF0-E311-9420-00155D013D39",
                Justificativa = "Justificativa Teste",
                SituacaoDenuncia = 993520001,
                TipoDenunciante = 993520003,
                Situacao = 1,
                Proprietario = "d86a88b1-bc0d-e411-9420-00155d013d39",
                TipoProprietario = "systemuser",
                CodigoRepresentante = 4000

            };





            Domain.Model.Denuncia den = new Domain.Servicos.DenunciaService(this.OrganizationName, false).ObterDenuncia(new Guid("83322EBF-E617-E411-940F-00155D013D31"));

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, msg107.GenerateMessage(false), out teste);

        }

        [Test]
        public void TesteMSG0108ListarDenuncia()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d773d4df-ccd6-e311-940c-00155d013d31</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0108</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0108>");

            //sb.AppendLine("      <CanaisDenunciados>");
            //sb.AppendLine("          <CodigoCanalDenunciado>442F19AD-76EE-E311-9407-00155D013D38</CodigoCanalDenunciado>");
            //sb.AppendLine("      </CanaisDenunciados>");

            sb.AppendLine("      <CanaisDenunciantes>");
            sb.AppendLine("          <CodigoCanalDenunciante>d66a88b1-bc0d-e411-9420-00155d013d39</CodigoCanalDenunciante>");
            sb.AppendLine("      </CanaisDenunciantes>");


            //sb.AppendLine("     <SituacaoDenuncia>993520000</SituacaoDenuncia>");
            sb.AppendLine("     <DataInicio>2014-08-19</DataInicio>");
            sb.AppendLine("     <DataFinal>2014-08-19</DataFinal>");
            sb.AppendLine("    </MSG0108>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }


        [Test]
        public void TesteMSG0146()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("<IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("<NumeroOperacao>344b5d21-6c0e-e411-9421-00155d013d39</NumeroOperacao>");
            sb.AppendLine("<CodigoMensagem>MSG0146</CodigoMensagem>");
            sb.AppendLine("<LoginUsuario>ToolSystems</LoginUsuario>");
            sb.AppendLine("</CABECALHO>");
            sb.AppendLine("<CONTEUDO>");
            sb.AppendLine("<MSG0146>");
            sb.AppendLine("<CodigoBeneficioCanal>32051601-1522-E411-9233-00155D013E44</CodigoBeneficioCanal>");
            sb.AppendLine("</MSG0146>");
            sb.AppendLine("</CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteMSG0109ObterDenuncia()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d773d4df-ccd6-e311-940c-00155d013d31</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0109</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0109>");
            sb.AppendLine("      <CodigoDenuncia>1EC6037F-E826-E411-9235-00155D013E44</CodigoDenuncia>");
            sb.AppendLine("    </MSG0109>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteMSG0110ListarTipoDenuncia()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d773d4df-ccd6-e311-940c-00155d013d31</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0110</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0110>");
            sb.AppendLine("    </MSG0110>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }


        [Test]
        public void TesteMSG0144ListarCompromissoCanal()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d773d4df-ccd6-e311-940c-00155d013d31</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0144</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0144>");
            sb.AppendLine("    <CodigoConta>D66A88B1-BC0D-E411-9420-00155D013D39</CodigoConta>");
            sb.AppendLine("    </MSG0144>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteMSG0145ObterCompromissoCanal()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d773d4df-ccd6-e311-940c-00155d013d31</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0145</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0145>");
            sb.AppendLine("    <CodigoCompromissoCanal>3FE4C9D8-9D06-E411-9420-00155D013D39</CodigoCompromissoCanal>");
            sb.AppendLine("    </MSG0145>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteMSG0147ListarSolicitacaoBeneficio()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>d773d4df-ccd6-e311-940c-00155d013d31</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0147</CodigoMensagem>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0147>");
            sb.AppendLine("    <CodigoBeneficioCanal>59334575-1318-E411-940F-00155D013D31</CodigoBeneficioCanal>");
            sb.AppendLine("    </MSG0147>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }

        [Test]
        public void TesteMSG0148ObterSolicitacaoBeneficio()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>c2ae2332-3a55-e411-93f5-00155d013e70</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0148</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>BERNADETE </LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0148>");
            sb.AppendLine("      <CodigoSolicitacaoBeneficio>c2ae2332-3a55-e411-93f5-00155d013e70</CodigoSolicitacaoBeneficio>");
            sb.AppendLine("    </MSG0148>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");



            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);

        }



        [Test]
        public void TesteMario()
        {
            NaturezaOperacao naturezaOperacao = new NaturezaOperacao(this.OrganizationName, this.IsOffline);
            naturezaOperacao = new Intelbras.CRM2013.Domain.Servicos.NaturezaOperacaoService(this.OrganizationName, this.IsOffline).BuscaNaturezaOperacaoPorCodigo("610100");

        }

        [Test]
        public void TesteMsg0146()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM>");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>344b5d21-6c0e-e411-9421-00155d013d39</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0146</CodigoMensagem>");
            sb.AppendLine("    <LoginUsuario>ToolSystems</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0146>");
            sb.AppendLine("      <CodigoBeneficioCanal>344b5d21-6c0e-e411-9421-00155d013d39</CodigoBeneficioCanal>");
            sb.AppendLine("    </MSG0146>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, sb.ToString(), out teste);
        }

        [TestMethod]
        public void TesteMsg0169Create()
        {
            MSG0169 tst = new MSG0169(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0169")
            {
                CodigoContato = "D1BCBCD8-D500-E411-9420-00155D013D39",
                NomeFilaAtendimento = "Fila Teste",
                Assunto = "Teste",
                DescricaoEmail = "Teste",
                Referente = "salesorder",
                NumeroPedido = "1018592",
                Direcao = 0,
                StatusEmail = 4
            };
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, tst.GenerateMessage(), out teste);
        }


        [TestMethod]
        public void TesteMsg0152Envio()
        {
            string msg = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<MENSAGEM>
  <CABECALHO>
    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>
    <NumeroOperacao>110128 / VMC / ISEC / 784,30</NumeroOperacao>
    <CodigoMensagem>MSG0152</CodigoMensagem>
    <LoginUsuario>BERNADETE </LoginUsuario>
  </CABECALHO>
  <CONTEUDO>
    <MSG0152>
      <CodigoSolicitacaoBeneficio>1843a7e8-1c2a-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>
      <NomeSolicitacaoBeneficio>VMC /ISEC / 1442,26</NomeSolicitacaoBeneficio>
      <CodigoTipoSolicitacao>1f14f20a-f938-e411-9421-00155d013d39</CodigoTipoSolicitacao>
      <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>
      <BeneficioCodigo>21</BeneficioCodigo>
      <CodigoBeneficioCanal>086bfdc1-e33d-e411-9421-00155d013d39</CodigoBeneficioCanal>
      <CodigoUnidadeNegocio>SEC</CodigoUnidadeNegocio>
      <CodigoConta>f7e8e1af-d500-e411-9420-00155d013d39</CodigoConta>
      <ValorSolicitado>784.3</ValorSolicitado>
      <DescricaoSolicitacao>asdf asdf asd fasd fas df</DescricaoSolicitacao>
      <SolicitacaoIrregular>true</SolicitacaoIrregular>
      <DescricaoSituacaoIrregular>Canal com Benefícios Suspensos</DescricaoSituacaoIrregular>
      <CodigoAcaoSubsidiadaVMC>61099c38-acf0-e311-9420-00155d013d39</CodigoAcaoSubsidiadaVMC>
      <DataPrevistaRetornoAcao>2015-07-31</DataPrevistaRetornoAcao>
      <ValorAcao>100.00</ValorAcao>
      <CodigoFormaPagamento>c67df108-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>
      <SituacaoSolicitacaoBeneficio>993520005</SituacaoSolicitacaoBeneficio>
      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>
      <Situacao>0</Situacao>
      <Proprietario>f9e8e1af-d500-e411-9420-00155d013d39</Proprietario>
      <TipoProprietario>team</TipoProprietario>
      <CodigoAssistente>36</CodigoAssistente>
      <CodigoSupervisorEMS>al027000</CodigoSupervisorEMS>
      <SolicitacaoAjuste>false</SolicitacaoAjuste>
      <ValorAbater>784.3</ValorAbater>
      <ValorPago>0</ValorPago>
      <ValorCancelado>0</ValorCancelado>
      <DataCriacao>2015-07-31</DataCriacao>
      <DataValidade>2015-07-31</DataValidade>
      <CodigoCondicaoPagamento>327</CodigoCondicaoPagamento>
      <DescartarVerba>false</DescartarVerba>
      <TrimestreCompetencia>2015-T1</TrimestreCompetencia>
      <FormaCancelamento>993520000</FormaCancelamento>
      <ProdutoSolicitacaoItens>
        <ProdutoSolicitacaoItem>
          <CodigoProdutoSolicitacao>2343a7e8-1c2a-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>
          <CodigoSolicitacaoBeneficio>1843a7e8-1c2a-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>
          <CodigoProduto>4561000</CodigoProduto>
          <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>
          <ValorUnitario>54.8300</ValorUnitario>
          <Quantidade>12</Quantidade>
          <ValorTotal>657.9600</ValorTotal>
          <ValorUnitarioAprovado>54.8300</ValorUnitarioAprovado>
          <QuantidadeAprovado>12</QuantidadeAprovado>
          <ValorTotalAprovado>657.9600</ValorTotalAprovado>
          <Proprietario>f9e8e1af-d500-e411-9420-00155d013d39</Proprietario>
          <TipoProprietario>team</TipoProprietario>
          <Acao>E</Acao>
          <CodigoEstabelecimento>105</CodigoEstabelecimento>
          <Situacao>0</Situacao>
          <QuantidadeCancelada>0</QuantidadeCancelada>
          <ValorPago>0</ValorPago>
          <ValorCancelado>0</ValorCancelado>
        </ProdutoSolicitacaoItem>
        <ProdutoSolicitacaoItem>
          <CodigoProdutoSolicitacao>2743a7e8-1c2a-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>
          <CodigoSolicitacaoBeneficio>1843a7e8-1c2a-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>
          <CodigoProduto>4561114</CodigoProduto>
          <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>
          <ValorUnitario>49.8400</ValorUnitario>
          <Quantidade>12</Quantidade>
          <ValorTotal>598.0800</ValorTotal>
          <ValorUnitarioAprovado>49.8400</ValorUnitarioAprovado>
          <QuantidadeAprovado>12</QuantidadeAprovado>
          <ValorTotalAprovado>598.0800</ValorTotalAprovado>
          <Proprietario>f9e8e1af-d500-e411-9420-00155d013d39</Proprietario>
          <TipoProprietario>team</TipoProprietario>
          <Acao>A</Acao>
          <CodigoEstabelecimento>104</CodigoEstabelecimento>
          <Situacao>0</Situacao>
          <QuantidadeCancelada>0</QuantidadeCancelada>
          <ValorPago>0</ValorPago>
          <ValorCancelado>0</ValorCancelado>
        </ProdutoSolicitacaoItem>
        <ProdutoSolicitacaoItem>
          <CodigoProdutoSolicitacao>2b43a7e8-1c2a-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>
          <CodigoSolicitacaoBeneficio>1843a7e8-1c2a-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>
          <CodigoProduto>4565800</CodigoProduto>
          <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>
          <ValorUnitario>8.3500</ValorUnitario>
          <Quantidade>10</Quantidade>
          <ValorTotal>83.5000</ValorTotal>
          <ValorUnitarioAprovado>8.3500</ValorUnitarioAprovado>
          <QuantidadeAprovado>10</QuantidadeAprovado>
          <ValorTotalAprovado>83.5000</ValorTotalAprovado>
          <Proprietario>f9e8e1af-d500-e411-9420-00155d013d39</Proprietario>
          <TipoProprietario>team</TipoProprietario>
          <Acao>A</Acao>
          <CodigoEstabelecimento>104</CodigoEstabelecimento>
          <Situacao>0</Situacao>
          <QuantidadeCancelada>0</QuantidadeCancelada>
          <ValorPago>0</ValorPago>
          <ValorCancelado>0</ValorCancelado>
        </ProdutoSolicitacaoItem>
        <ProdutoSolicitacaoItem>
          <CodigoProdutoSolicitacao>55d2370f-1f2a-e511-8eff-00155d013d88</CodigoProdutoSolicitacao>
          <CodigoSolicitacaoBeneficio>1843a7e8-1c2a-e511-8eff-00155d013d88</CodigoSolicitacaoBeneficio>
          <CodigoProduto>4565105</CodigoProduto>
          <CodigoBeneficio>7adb8963-74ed-e311-9407-00155d013d38</CodigoBeneficio>
          <ValorUnitario>102.7200</ValorUnitario>
          <Quantidade>1</Quantidade>
          <ValorTotal>102.7200</ValorTotal>
          <ValorUnitarioAprovado>102.7200</ValorUnitarioAprovado>
          <QuantidadeAprovado>1</QuantidadeAprovado>
          <ValorTotalAprovado>102.7200</ValorTotalAprovado>
          <Proprietario>f9e8e1af-d500-e411-9420-00155d013d39</Proprietario>
          <TipoProprietario>team</TipoProprietario>
          <Acao>A</Acao>
          <CodigoEstabelecimento>104</CodigoEstabelecimento>
          <Situacao>0</Situacao>
          <QuantidadeCancelada>0</QuantidadeCancelada>
          <ValorPago>0</ValorPago>
          <ValorCancelado>0</ValorCancelado>
        </ProdutoSolicitacaoItem>
      </ProdutoSolicitacaoItens>
    </MSG0152>
  </CONTEUDO>
</MENSAGEM>";

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, msg, out teste);

        }

        [TestMethod]
        public void TesteMsg0159Envio()
        {
            string msg = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<MENSAGEM xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <CABECALHO>
    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>
    <NumeroOperacao>7cad9a31-7849-e411-9424-00155d013d3a</NumeroOperacao>
    <CodigoMensagem>MSG0159</CodigoMensagem>
    <LoginUsuario />
  </CABECALHO>
  <CONTEUDO>
    <MSG0159>
      <BeneficioCanalItens>
        <BeneficioCanalItem>
          <CodigoBeneficioCanal>14c4a360-7930-e511-8eff-00155d013d88</CodigoBeneficioCanal>
          <VerbaCalculada>120340.1234</VerbaCalculada>
          <VerbaPeriodoAnterior>12345.1234</VerbaPeriodoAnterior>
          <VerbaTotal>132685.2468</VerbaTotal>
          <VerbaEmpenhada>36456.1234</VerbaEmpenhada>
          <VerbaReembolsada>3467.2956</VerbaReembolsada>
          <VerbaCancelada>398.5896</VerbaCancelada>
          <VerbaAjustada>697.2885</VerbaAjustada>
          <VerbaDisponivel>93060.5267</VerbaDisponivel>
        </BeneficioCanalItem>
        <BeneficioCanalItem>
          <CodigoBeneficioCanal>16c4a360-7930-e511-8eff-00155d013d88</CodigoBeneficioCanal>
          <VerbaCalculada>50000.0000</VerbaCalculada>
          <VerbaPeriodoAnterior>5000.0000</VerbaPeriodoAnterior>
          <VerbaTotal>55000.0000</VerbaTotal>
          <VerbaEmpenhada>15.0000</VerbaEmpenhada>
          <VerbaReembolsada>10.0000</VerbaReembolsada>
          <VerbaCancelada>6000.0000</VerbaCancelada>
          <VerbaAjustada>-5000.0000</VerbaAjustada>
          <VerbaDisponivel>19000.0000</VerbaDisponivel>
        </BeneficioCanalItem>
      </BeneficioCanalItens>
    </MSG0159>
  </CONTEUDO>
</MENSAGEM>";

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String teste = String.Empty;

            integ.Postar(usuario, senha, msg, out teste);

        }

        [TestMethod]
        public void ObtemTelefoneAssistencia()
        {
            ArrayList list = new ArrayList();
            list.Add(new SqlParameter("@P_CEP", "88110550"));
            list.Add(new SqlParameter("@P_UNIDADE_NEGOCIO", "teste"));

            string pSaida = "@P_RESULTADO";

            string nomeProcedure = "dbo.p_TelefoneAssistencias";

            string telefones = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterTelefoneAssistencias(nomeProcedure, list, pSaida);
        }

        //[TestMethod]
        //public void LiveChatTracking()
        //{
        //    LiveChatTracking entity = new LiveChatTracking(OrganizationName, IsOffline);

        //    entity.NameVisitor = "JaimeTeste01";

        //    try
        //    {
        //      var liveChatTracking = new RepositoryService().LiveChatTracking.Create(entity);//Alterada a ocorrência para atualizar a data de modificação

        //        //new RepositoryService().Anexo.Create(
        //        //    new Anexo()
        //        //    {
        //        //        Texto = "Teste",
        //        //        EntidadeRelacionada = new Lookup(liveChatTracking.Id, "incident"),
        //        //        Assunto = "Gravação de Conversa."
        //        //    }
        //        //);
        //        Console.WriteLine("Conversa criada com sucesso!");
        //    }
        //    //Entra no catch caso encontre alguma inconsistência no update ou no envio de e-mail
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Erro! " + ex.Message);
        //        //}
        //    }
        //}

        //[TestMethod]
        //public void ListarPorContaSegmento()
        //{
        //    Guid guid = new Guid("F7E8E1AF-D500-E411-9420-00155D013D39");

        //    List<Segmento> lstSegmentos = new RepositoryService().Segmento.ListarPorContaSegmento((Guid)guid);

        //    if (lstSegmentos.Count > 0)
        //        foreach (Segmento segmentoItem in lstSegmentos)
        //        {
        //            Message.Helper.Entities.Segmento segmento = new Message.Helper.Entities.Segmento();

        //            segmento.CodigoSegmento = segmentoItem.ID.ToString();
        //            segmento.NomeSegmento = segmentoItem.Nome;

        //            UnidadeNegocio unidadeNegocio = new UnidadeNegocioService().BuscaUnidadeNegocio(objCrm.UnidadeNegocio.Id);
        //            if (unidadeNegocio != null)
        //            {
        //                xmlRetorno.CodigoUnidadeNegocio = unidadeNegocio.ChaveIntegracao;
        //                xmlRetorno.NomeUnidadeNegocio = unidadeNegocio.Nome;
        //            }
        //        }
        //}
    }
}


