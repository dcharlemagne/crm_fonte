using System;
using NUnit.Framework;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Domain;
using System.Web;
using System.Web.Services;
using Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class MFatura0094 : Base
    {
        [Test]
        public void TesteMsgFaturaCreate()
        {

            //EnderecoFatura
            Intelbras.Message.Helper.Entities.Endereco enderecoFatura = new Message.Helper.Entities.Endereco();
            enderecoFatura.Bairro = "Xalisto";
            enderecoFatura.CEP = "09595300";
            enderecoFatura.Cidade = "Florianopolis, SC, Brasil";
            enderecoFatura.Estado = "Brasil,SC";
            enderecoFatura.Logradouro = "Rua Teste";
            enderecoFatura.NomeCidade = "Florianopolis";
            enderecoFatura.NomeContato = "Carlos";
            enderecoFatura.NomeEndereco = "Teste";
            enderecoFatura.NomePais = "Brasil";
            enderecoFatura.Numero = "20";
            enderecoFatura.Pais = "Brasil";
            enderecoFatura.Telefone = "56565656";
            enderecoFatura.TipoEndereco = 1;
            enderecoFatura.UF = "SP";
            //Fim EnderecoFatura

            //NotaFiscalItem
             Message.Helper.Entities.NotaFiscalItem notaFiscalItem = new Message.Helper.Entities.NotaFiscalItem();
        
                notaFiscalItem.ChaveIntegracao="123";
                notaFiscalItem.CodigoProduto="12";
                notaFiscalItem.NomeProduto="teste";
                notaFiscalItem.Descricao="descri teste";
                notaFiscalItem.PrecoOriginal=0;
                notaFiscalItem.PrecoUnitario=0;
                notaFiscalItem.PrecoLiquido=0;
                notaFiscalItem.ValorMercadoriaTabela=0;
                notaFiscalItem.ValorMercadoriaOriginal = 0;
                notaFiscalItem.ValorMercadoriaLiquido=0;
                notaFiscalItem.CodigoNaturezaOperacao="123";
                notaFiscalItem.NomeNaturezaOperacao="nat op";
                notaFiscalItem.ProdutoForaCatalogo=true;
                notaFiscalItem.DescricaoProdutoForaCatalogo="";
                notaFiscalItem.PermiteSubstituirPreco=true;
                notaFiscalItem.UnidadeMedida="um";
                notaFiscalItem.ValorBaseICMS=0;
                notaFiscalItem.ValorBaseICMSSubstituicao=0;
                notaFiscalItem.ValorICMS=0;
                notaFiscalItem.ValorICMSSubstituicao=0;
                notaFiscalItem.ValorICMSNaoTributado=0;
                notaFiscalItem.ValorICMSOutras=0;
                notaFiscalItem.CodigoTributarioICMS="tributado";
                notaFiscalItem.CodigoTributarioISS="Isento";
                notaFiscalItem.CodigoTributarioIPI="tributado";
                notaFiscalItem.ValorBaseISS=0;
                notaFiscalItem.ValorBaseIPI=0;
                notaFiscalItem.AliquotaISS=0;
                notaFiscalItem.AliquotaICMS=0;
                notaFiscalItem.AliquotaIPI=0;
                notaFiscalItem.ValorISS=0;
                notaFiscalItem.ValorISSNaoTributado=0;
                notaFiscalItem.ValorISSOutras=0;
                notaFiscalItem.ValorIPI=0;
                notaFiscalItem.ValorIPINaoTributado=0;
                notaFiscalItem.ValorIPIOutras=0;
                notaFiscalItem.PrecoConsumidor=0;
                notaFiscalItem.QuantidadeCancelada=0;
                notaFiscalItem.QuantidadePendente=0;
                
                //endereço
                notaFiscalItem.EnderecoEntrega = enderecoFatura;
                notaFiscalItem.DataEntrega = new DateTime();
                notaFiscalItem.CondicaoFrete=0;
                notaFiscalItem.ValorOriginal=0;
                notaFiscalItem.ValorTotalImposto=0;
                notaFiscalItem.ValorDescontoManual=0;
                notaFiscalItem.Quantidade=0;
                notaFiscalItem.RetiraNoLocal=true;
                notaFiscalItem.QuantidadeEntregue=0;
                notaFiscalItem.NumeroSequencia=0;
                notaFiscalItem.CodigoUnidadeNegocio="123";
                notaFiscalItem.NomeUnidadeNegocio="";
                notaFiscalItem.CodigoRepresentante=0;
                notaFiscalItem.NomeRepresentante="";
                notaFiscalItem.ValorTotal=0;
                notaFiscalItem.Moeda="";
                notaFiscalItem.Acao="I";
                

                
                //fim notaFiscalItem

            List<Message.Helper.Entities.NotaFiscalItem> lstFatura = new List<Message.Helper.Entities.NotaFiscalItem>();

            lstFatura.Add(notaFiscalItem);

            MSG0094 Conta = new MSG0094(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0094")
            {
                NumeroNotaFiscal ="123123",
                
                // nao tem nome na doc mas a dll exige
                NumeroSerie = "123",
                CodigoClienteCRM = "BEE55B63-9AAE-E311-9207-00155D013D19",
                TipoObjetoCliente = "account",
                NumeroPedido = 0,
                NumeroPedidoCliente="",
                Descricao="",
                Estabelecimento=0,
                CondicaoPagamento=0,
                NomeAbreviadoCliente="N.Abrev",
                NaturezaOperacao="123",
                Moeda="Real",
                SituacaoNota=0,
                //RazaoSituacao=0,
                DataEmissao = new DateTime(),
                DataSaida = new DateTime(),
                DataEntrega = new DateTime(),
                DataConfirmacao = new DateTime(),
                DataCancelamento = new DateTime(),
                DataConclusao = new DateTime(),
                ValorFrete=0.1M,
                PesoLiquido=0,
                PesoBruto=0,
                Observacao="",
                Volume="",
                ValorBaseICMS=0,
                ValorICMS=0,
                ValorIPI=0,
                ValorBaseSubstituicaoTributaria=0,
                ValorSubstituicaoTributaria=0,
                RetiraNoLocal=true,
                MetodoEntrega=2,
                Transportadora = 103,
                EnderecoEntrega = enderecoFatura,
                Frete="sao paulo",
                CondicaoFreteEntrega=1,
                TelefoneCobranca="",
                FaxCobranca="",
                CNPJ = "66264213000666",
                CPF = "",
                InscricaoEstadual = "isento",
                Oportunidade="",
                PrecoBloqueado=true,
                ValorDesconto=0,
                PercentualDesconto=0,
                ListaPreco = "",
                Prioridade=0,
                Representante=0,
                Proprietario = "FF3CBD6F-8E9D-E311-888D-00155D013E2E",
                TipoProprietario="systemuser",
                ValorTotal=0,
                ValorTotalImpostos = 0,
                ValorTotalSemImposto=0,
                ValorTotalSemFrete=0,
                ValorTotalDesconto=0,
                ValorTotalProdutos=0,
                ValorTotalProdutosSemImposto=0,
                
                NotaFiscalItens = lstFatura
            };



            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String msgSaida = String.Empty;

            integ.Postar(usuario, senha, Conta.GenerateMessage(), out msgSaida);

        }
    }
}
