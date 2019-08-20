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
using Intelbras.CRM2013.Domain.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using SDKore.DomainModel;
using Microsoft.Crm.Sdk.Query;
using Intelbras.CRM2013.Domain.Servicos;
using isol = Intelbras.CRM2013.Application.WebServices.Isol.IsolService;

namespace Intelbras.CRM2013.Tests
{
    [TestClass]
    [TestFixture]
    public class TestesPedidoB2B : Base
    {
        [TestMethod]
        public void TestarInserirPedido()
        {
            Domain.Model.Pedido pedido = new Domain.Model.Pedido();

            pedido.Cliente = new Conta() { CodigoMatriz = "113879" };
            pedido.TabelaDePreco = new TabelaDePreco() { CodigoDaTabelaDePreco = "123" };
            pedido.DiasNegociacao = 0;
            pedido.Estabelecimento = new Lookup(id: new Guid("11397390-19E9-E311-9420-00155D013D39"), type: "itbc_estabelecimento");
            pedido.Representante = new Contato() { Id = new Guid("2D19FE56-7BEE-E311-9420-00155D013D39") };
            pedido.CondicaoDePagamento = new CondicaoPagamento() { Id = new Guid("F72C5441-A61E-E511-941A-00155D013A30") };
            pedido.Moeda = new Lookup(id: new Guid("24B5CE59-90C5-E311-93FD-00155D013E56"), type: "transactioncurrency");
            pedido.FaturamentoParcial = false;
            pedido.DataDeEmissao = DateTime.Now;
            pedido.DataDeFaturamento = DateTime.Now;
            pedido.DataBaseNegociacao = DateTime.Now;
            pedido.PrecoTotalComIPI = (decimal)100.1;
            pedido.PrecoTotal = (decimal)100.1;
            pedido.Descricao = "pedido";
            pedido.DescricaoNota = "descrição Nota";
            pedido.StatusPedido = 1;
            pedido.CanalVendaID = new Lookup(id: new Guid("30E0D942-231F-E511-941A-00155D013A30"), type: "itbc_canaldevenda");

            //itens do pedido

            Domain.Model.ProdutoPedido item = new Domain.Model.ProdutoPedido();

            item.Produto = new Lookup(id: new Guid("DED25120-BDED-E311-9420-00155D013D39"), type: "product");
            item.Quantidade = 10;
            item.PrecoNegociado = 0;
            item.DescontoManual = 0;
            item.PrecoMinimo = 10;
            item.DescontoManual = 0;
            item.AliquotaIPI = 1;

            pedido.ItensDoPedido = new List<ProdutoPedido>();
            pedido.ItensDoPedido.Add(item);
            (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.SalvarPedidoB2BnoCRM(pedido);


            /*var produto = (new CRM2013.Domain.Servicos.RepositoryService()).Produto.Retrieve(new Guid("DED25120-BDED-E311-9420-00155D013D39"));
           ProdutoPedido item = new ProdutoPedido();
           item.NullableProperties = new List<string>();
           item.Quantidade = 10;
           item.Produto = new Lookup(id: new Guid("DED25120-BDED-E311-9420-00155D013D39"), type: "product");
           item.PrecoNegociado = 10;
           item.DescontoManual = 0;
           item.PrecoMinimo = 10;
           item.ValorLiquidoSemIpiSt = 10;
           item.Precificacao = true;
           item.AliquotaIPI = 5;
           item.Pedido = new Lookup(id: new Guid("DF817A5B-EEC1-443B-8791-967D220A6946"), type: "salesorder");
           item.Unidade = produto.UnidadePadrao;
           item.UnidadeNegocio = produto.UnidadeNegocio;
           item.IntegradoEm = DateTime.Now;
           item.IntegradoPor = "teste";
           item.UsuarioIntegracao = "teste";
           item.ChaveIntegracao = "teste";
           item.Descricao = "teste";
           item.TaxaCambio = 0;
           item.SelecionarProduto = false;
           item.DataAlteracao = DateTime.Now;
           item.AddNullProperty("DataCancelamentoUsuario");
           item.AddNullProperty("DataCancelamentoSequencia");
           item.AddNullProperty("DataDevolucao");
           item.AddNullProperty("DataDevolucaoUsuario");
           item.DataEntrega = DateTime.Now;
           item.DataEntregaOriginal = DateTime.Now;
           item.DataImplantacao = DateTime.Now;
           item.UsuarioImplantacao = "teste";
           item.CalcularRebate = true;
           item.AddNullProperty("DataMaximaFaturamento");
           item.AddNullProperty("DataMinimaFaturamento");
           item.AddNullProperty("DataReativacao");
           item.AddNullProperty("DataReativacaoUsuario");
           item.AddNullProperty("DataSuspensao");
           item.AddNullProperty("DataSuspensaoUsuario");
           item.AddNullProperty("DescricaoCancelamento");
           item.AddNullProperty("DescricaoDevolucao");
           item.AddNullProperty("FaturaQtdeFamilia");
           item.RepresentanteOriginal = "teste";
           item.Representante = new Lookup(id: new Guid("2D19FE56-7BEE-E311-9420-00155D013D39"), type: "contact");
           item.IntegradoRepresentanteComErro = false;
           item.NaturezaOperacao = new Lookup(new Guid("A57BD840-701E-E511-941A-00155D013A30"), "");
           item.NomeAbreviado = "teste";
           item.AddNullProperty("Observacao");
           item.PedidoCliente = "54545";
           item.AddNullProperty("PercentualDescontoICMS");
           item.AddNullProperty("PercentualMinimoFaturamento");
           item.AddNullProperty("QtdeAlocada");
           item.AddNullProperty("QtdeAlocadaLogica");
           item.AddNullProperty("QtdeDevolvida");
           //item.AddNullProperty("RetemICMSFonte");
           item.RetemICMSFonte = false;
           item.AddNullProperty("SituacaoAlocacao");
           //item.AddNullProperty("SituacaoItem");
           item.SituacaoItem = 993520002;
           item.TipoPreco = 993520000;
           item.AddNullProperty("UsuarioAlteracao");
           item.AddNullProperty("UsuarioCancelamento");
           item.AddNullProperty("UsuarioDevolucao");
           item.UsuarioImplantacao = "teste";
           item.AddNullProperty("UsuarioReativacao");
           item.AddNullProperty("UsuarioSuspensao");
           item.ValorLiquidoAberto = 100;
           item.ValorLiquidoItem = 100;
           item.ValorMercadoriaAberto = 100;
           item.ValorOriginal = 10;
           item.ValorTabela = 10;
           item.ValorTotalItem = 100;
           item.ValorSubstTributaria = 0;
           item.ValorIPI = 1;
           item.DescontoManual = 0;
           item.AddNullProperty("ProdutoForaCatalogo");
           item.AddNullProperty("QtdePedidoPendente");
           item.AddNullProperty("QtdeCancelada");
           item.AddNullProperty("QtdeEntregue");
           item.DateEntregaSolicitada = DateTime.Now;
           item.NumeroSequencia = 10;
           item.CidadeEntrega = "skfskjf";
           item.NomeContatoEntrega = "shfshfg";
           item.PaisEntrega = "jghjg";
           item.AddNullProperty("FAXEntrega");
           item.AddNullProperty("CondicoesFrete");
           item.RuaEntrega = "hgjhg";
           item.BairroEntrega = "jhjhg";
           item.AddNullProperty("ComplementoEntrega");
           item.AddNullProperty("NomeEntrega");
           item.CEPEntrega = "65656";
           item.EstadoEntrega = "as";
           item.AddNullProperty("TelefoneEntrega");
           item.TotalImposto = 10;
           item.Moeda = new Lookup(new Guid("24B5CE59-90C5-E311-93FD-00155D013E56"), "");


           //(new CRM2013.Domain.Servicos.RepositoryService()).ProdutoPedido.Create(item);

           var organizacao = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

           new Intelbras.CRM2013.Domain.Servicos.PedidoService(organizacao, false).Persistir(item);*/



        }
        [Test]
        public void ListarCatUNB2BPor()
        {
            Conta cliente = new Conta() { Id = new Guid("9EFF14B8-0BBA-DF11-8266-00155DA09700") };
            (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ListarCategoriaUNB2BPor(cliente, new Guid("70412fc3-32e7-df11-8630-00155da09700"));
        }

        [TestMethod]
        public void ListarSolucaoesPorFamilia()
        {
            LinhaComercial linha = new LinhaComercial();

            linha = new LinhaComercial() { Id = new Guid("404300a3-64a7-e611-80bd-0050568f3ab2") };

            Defeito defeitoFamilia = new Defeito() { Id = new Guid("c477bf9f-63a7-e611-80bd-0050568f3ab2") };

            var solucoes = (new CRM2013.Domain.Servicos.RepositoryService()).Solucao.ListarSolucaoesPorFamilia(linha.Id, defeitoFamilia);
        }

        [TestMethod]
        public void SalvarOs()
        {

            new OcorrenciaService("Intelbras", false).Criar();


            Ocorrencia ocorrencia = new Ocorrencia();
            //ocorrencia.Id = new Guid("");
            ocorrencia.ClienteOS = new Contato() { CpfCnpj = ("07.775.753/0001-18") };
            ocorrencia.ClienteOS.Endereco1CEP = "03411000";
            ocorrencia.ClienteOS.Endereco1Estado = "";
            ocorrencia.ClienteOS.Endereco1Municipio = "";

            ocorrencia.SalvarOS(ocorrencia);
        }

        [TestMethod]
        public void ConsultarVincVeiculoClienteContrato()
        {
            var contratoId = "9BD3AD5E-4325-E911-80DC-005056AA4874";
            var placa = "WOO 5532-8714";
            Contrato contrato = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.Retrieve(new Guid(contratoId));
                if(contrato != null) {
                    List<ClienteParticipante> lstClientesParticipantes = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.ObterClientesParticipantesPor(contrato);
                    if(lstClientesParticipantes.Count > 0) {
                        List<Veiculo> lstVeiculo = (new CRM2013.Domain.Servicos.RepositoryService()).Veiculo.ListarPorClientesParticipantesContrato(lstClientesParticipantes, placa);
                    }
                }
        }
    }
}
