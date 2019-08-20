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

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class MConta0091 : Base
    {
        [Test]
        public void TesteMConta0091()
        {
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<MENSAGEM xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            sb.AppendLine("  <CABECALHO>");
            sb.AppendLine("    <IdentidadeEmissor>64546C2E-6DAB-4311-A74A-5ACA96134AFF</IdentidadeEmissor>");
            sb.AppendLine("    <NumeroOperacao>892392</NumeroOperacao>");
            sb.AppendLine("    <CodigoMensagem>MSG0091</CodigoMensagem>");
            sb.AppendLine("     <LoginUsuario>ve888001</LoginUsuario>");
            sb.AppendLine("  </CABECALHO>");
            sb.AppendLine("  <CONTEUDO>");
            sb.AppendLine("    <MSG0091>");
            sb.AppendLine("      <NumeroPedido>1</NumeroPedido>");
            sb.AppendLine("      <Nome>abc</Nome>");
            sb.AppendLine("      <NumeroPedidoCliente>892392</NumeroPedidoCliente>");
            //sb.AppendLine("      <NumeroPedidoRepresentante />");
            sb.AppendLine("      <CodigoClienteCRM>08F14E0E-4B9E-E311-888D-00155D013E2E</CodigoClienteCRM>");
            sb.AppendLine("      <TipoObjetoCliente>account</TipoObjetoCliente>");
            sb.AppendLine("      <NomeAbreviadoCliente>ZZ MOVEIS</NomeAbreviadoCliente>");
            //sb.AppendLine("      <TabelaPreco />");
            //sb.AppendLine("      <TabelaPrecoEMS />");
            sb.AppendLine("      <TipoPreco>993520000</TipoPreco>");
            sb.AppendLine("      <Estabelecimento>104</Estabelecimento>");
            sb.AppendLine("      <CondicaoPagamento>1</CondicaoPagamento>");
            sb.AppendLine("      <TabelaFinanciamento>1</TabelaFinanciamento>");
            sb.AppendLine("      <Representante>123123</Representante>");
            sb.AppendLine("      <NaturezaOperacao>610100</NaturezaOperacao>");
            sb.AppendLine("      <DataEmissao>2014-05-13</DataEmissao>");
            sb.AppendLine("      <DataImplantacao>2014-05-13</DataImplantacao>");
            sb.AppendLine("      <DataImplantacaoUsuario>2012-04-25</DataImplantacaoUsuario>");
            sb.AppendLine("      <DataEntrega>2014-05-13</DataEntrega>");
            sb.AppendLine("      <DataEntregaSolicitada>2014-05-13</DataEntregaSolicitada>");
            //sb.AppendLine("      <DataMinimaFaturamento xsi:nil=\"true\" />");
            //sb.AppendLine("      <DataLimiteFaturamento xsi:nil=\"true\" />");
            //sb.AppendLine("      <DataCumprimento xsi:nil=\"true\" />");
            //sb.AppendLine("      <DataReativacao xsi:nil=\"true\" />");
            //sb.AppendLine("      <DataReativacaoUsuario xsi:nil=\"true\" />");
            //sb.AppendLine("      <DataNegociacao xsi:nil=\"true\" />");
            sb.AppendLine("      <DiasNegociacao>0</DiasNegociacao>");
            sb.AppendLine("      <TipoPedido>90</TipoPedido>");
            sb.AppendLine("      <OrigemPedido>993520010</OrigemPedido>");
            sb.AppendLine("      <Prioridade>1</Prioridade>");
            //sb.AppendLine("      <CPF xsi:nil=\"true\" />");
            sb.AppendLine("      <CNPJ>09666984000119</CNPJ>");
            sb.AppendLine("      <InscricaoEstadual>133576027</InscricaoEstadual>");
            sb.AppendLine("      <SituacaoPedido>0</SituacaoPedido>");
            sb.AppendLine("      <Situacao>1</Situacao>");
            sb.AppendLine("      <PercentualDesconto1>0.000000</PercentualDesconto1>");
            sb.AppendLine("      <PercentualDesconto2>0.000000</PercentualDesconto2>");
            sb.AppendLine("      <CidadeCIF>CAMPO VERDE</CidadeCIF>");
            sb.AppendLine("      <Portador>999</Portador>");
            sb.AppendLine("      <ModalidadeCobranca>993520000</ModalidadeCobranca>");
            sb.AppendLine("      <Mensagem>0</Mensagem>");
            sb.AppendLine("      <Observacao>REF SUA NF; 0000410;1;</Observacao>");
            sb.AppendLine("      <CondicaoEspecial>REF SUA NF; 0000410;1;PRODUTO TROCADOCC 23410</CondicaoEspecial>");
            //sb.AppendLine("      <ObservacaoRedespacho />");
            sb.AppendLine("      <UsuarioAlteracao>an046325</UsuarioAlteracao>");
            sb.AppendLine("      <DataAlteracao>2014-05-13</DataAlteracao>");
            //sb.AppendLine("      <UsuarioCancelamento />");
            //sb.AppendLine("      <DescricaoCancelamento />");
            //sb.AppendLine("      <DataCancelamento xsi:nil=\"true\" />");
            //sb.AppendLine("      <DataCancelamentoUsuario xsi:nil=\"true\" />");
            //sb.AppendLine("      <UsuarioReativacao />");
            //sb.AppendLine("      <UsuarioSuspensao />");
            //sb.AppendLine("      <DescricaoSuspensao />");
            //sb.AppendLine("      <DataSuspensao xsi:nil=\"true\" />");
            sb.AppendLine("      <IndicacaoAprovacao>false</IndicacaoAprovacao>");
            //sb.AppendLine("      <AprovacaoForcada />");
            //sb.AppendLine("      <UsuarioAprovacao />");
            //sb.AppendLine("      <DataAprovacao xsi:nil=\"true\" />");
            sb.AppendLine("      <DestinoMercadoria>993520000</DestinoMercadoria>");
            sb.AppendLine("      <Transportadora>123123</Transportadora>");
            //sb.AppendLine("      <Rota />");
            sb.AppendLine("      <FaturamentoParcial>false</FaturamentoParcial>");
            sb.AppendLine("      <Moeda>Real</Moeda>");
            sb.AppendLine("      <ValorTotalLiquido>118.3500000000</ValorTotalLiquido>");
            sb.AppendLine("      <ValorTotalPedido>118.3500000000</ValorTotalPedido>");
            sb.AppendLine("      <ValorTotalAberto>0.0000000000</ValorTotalAberto>");
            sb.AppendLine("      <ValorMercadoriaAberto>118.3500000000</ValorMercadoriaAberto>");
            sb.AppendLine("      <IndiceFinanciamento>1</IndiceFinanciamento>");
            sb.AppendLine("      <SituacaoAvaliacao>993520000</SituacaoAvaliacao>");
            //sb.AppendLine("      <MotivoBloqueioCredito />");
            //sb.AppendLine("      <MotivoLiberacaoCredito />");
            sb.AppendLine("      <SituacaoAlocacao>993520001</SituacaoAlocacao>");
            sb.AppendLine("      <ValorCreditoLiberado>0.00</ValorCreditoLiberado>");
            sb.AppendLine("      <ValorDesconto>0.0000000000</ValorDesconto>");
            sb.AppendLine("      <PercentualDesconto>0.0</PercentualDesconto>");
            sb.AppendLine("      <PercentualDescontoICMS>0.000</PercentualDescontoICMS>");
            sb.AppendLine("      <CodigoEndereco>Padrão</CodigoEndereco>");
            sb.AppendLine("      <ValorFrete>0.00</ValorFrete>");
            //sb.AppendLine("      <TipoFrete xsi:nil=\"true\" />");
            sb.AppendLine("      <PedidoCompleto>false</PedidoCompleto>");
            sb.AppendLine("      <CanalVenda>786</CanalVenda>");
            //sb.AppendLine("      <ClienteTriangular xsi:nil=\"true\" />");
            //sb.AppendLine("      <CodigoEntregaTriangular />");
            sb.AppendLine("      <ListaPreco>Lista Padrão</ListaPreco>");
            //sb.AppendLine("      <Descricao />");
            sb.AppendLine("      <ValorTotalImpostos>-11.84</ValorTotalImpostos>");
            sb.AppendLine("      <ValorTotalSemFrete>118.35</ValorTotalSemFrete>");
            sb.AppendLine("      <ValorTotalDesconto>0.0</ValorTotalDesconto>");
            //sb.AppendLine("      <CampanhaOrigem xsi:nil=\"true\" />");
            sb.AppendLine("      <PrecoBloqueado>false</PrecoBloqueado>");
            //sb.AppendLine("      <Classificacao xsi:nil=\"true\" />");
            sb.AppendLine("      <PedidoOriginal>0</PedidoOriginal>");
            sb.AppendLine("      <TotalIPI>0.0</TotalIPI>");
            sb.AppendLine("      <TotalSubstituicaoTributaria>-11.84</TotalSubstituicaoTributaria>");
            sb.AppendLine("      <Oportunidade>D922DCE5-33B0-E311-9207-00155D013D19</Oportunidade>");
            sb.AppendLine("      <Proprietario>26599738-D4C4-E311-9BFF-00155D013E44</Proprietario>");
            sb.AppendLine("      <TipoProprietario>systemuser</TipoProprietario>");
            //sb.AppendLine("      <FormaPagamento xsi:nil=\"true\" />");
            //sb.AppendLine("      <Cotacao xsi:nil=\"true\" />");
            sb.AppendLine("      <CondicaoFrete>1</CondicaoFrete>");
            sb.AppendLine("      <RetiraNoLocal>false</RetiraNoLocal>");
            //sb.AppendLine("      <AprovadorPedido />");

            sb.AppendLine("      <EnderecoEntrega>");
            sb.AppendLine("      <NomeEndereco>AV. FLORIANOPOLIS, 843 - SL. A</NomeEndereco>");
            sb.AppendLine("        <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("        <CaixaPostal />");
            sb.AppendLine("        <CEP>78840000</CEP>");
            sb.AppendLine("        <Logradouro>AV. FLORIANOPOLIS</Logradouro>");
            sb.AppendLine("        <Numero>843</Numero>");
            sb.AppendLine("        <Complemento>SL. A</Complemento>");
            sb.AppendLine("        <Bairro>CENTRO</Bairro>");
            sb.AppendLine("        <NomeCidade>CAMPO VERDE</NomeCidade>");
            sb.AppendLine("        <Cidade>CAMPO VERDE,MT,Brasil</Cidade>");
            sb.AppendLine("        <UF>MT</UF>");
            sb.AppendLine("        <Estado>Brasil,MT</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("        <NomeContato>ZZ MOVEIS LTDA</NomeContato>");
            sb.AppendLine("        <Telefone>6634611819</Telefone>");
            sb.AppendLine("        <Fax />");
            sb.AppendLine("     </EnderecoEntrega>");

            sb.AppendLine("      <EnderecoCobranca>");
            sb.AppendLine("      <NomeEndereco>AV. FLORIANOPOLIS, 843 - SL. A</NomeEndereco>");
            sb.AppendLine("        <TipoEndereco>1</TipoEndereco>");
            sb.AppendLine("        <CaixaPostal />");
            sb.AppendLine("        <CEP>78840000</CEP>");
            sb.AppendLine("        <Logradouro>AV. FLORIANOPOLIS</Logradouro>");
            sb.AppendLine("        <Numero>843</Numero>");
            sb.AppendLine("        <Complemento>SL. A</Complemento>");
            sb.AppendLine("        <Bairro>CENTRO</Bairro>");
            sb.AppendLine("        <NomeCidade>CAMPO VERDE</NomeCidade>");
            sb.AppendLine("        <Cidade>CAMPO VERDE,MT,Brasil</Cidade>");
            sb.AppendLine("        <UF>MT</UF>");
            sb.AppendLine("        <Estado>Brasil,MT</Estado>");
            sb.AppendLine("        <NomePais>Brasil</NomePais>");
            sb.AppendLine("        <Pais>Brasil</Pais>");
            sb.AppendLine("        <NomeContato>ZZ MOVEIS LTDA</NomeContato>");
            sb.AppendLine("        <Telefone>6634611819</Telefone>");
            sb.AppendLine("        <Fax />");
            sb.AppendLine("     </EnderecoCobranca>");

            sb.AppendLine("     <PedidosItens>");
            sb.AppendLine("     <PedidoItem>");
            sb.AppendLine("     <ChaveIntegracao xsi:nil=\"true\" />");
            sb.AppendLine("     <NumeroPedido>892392</NumeroPedido>");
            sb.AppendLine("     <NumeroPedidoCliente>892392</NumeroPedidoCliente>");
            sb.AppendLine("     <Sequencia>10</Sequencia>");
            sb.AppendLine("     <Produto>4000054</Produto>");
            sb.AppendLine("     <DescricaoItemPedido>TELEFONE S/FIO TS62 V</DescricaoItemPedido>");
            sb.AppendLine("     <UnidadeMedida>pc</UnidadeMedida>");
            sb.AppendLine("     <DataEntregaSolicitada>2012-04-25</DataEntregaSolicitada>");
            sb.AppendLine("     <DataEntrega>2012-04-25</DataEntrega>");
            sb.AppendLine("     <DataImplantacao>2014-05-13</DataImplantacao>");
            sb.AppendLine("     <QuantidadePedida>1.0000</QuantidadePedida>");
            sb.AppendLine("     <QuantidadeEntregue>0.0000</QuantidadeEntregue>");
            sb.AppendLine("     <QuantidadePendente>0.0000</QuantidadePendente>");
            sb.AppendLine("     <QuantidadeDevolvida>0.0000</QuantidadeDevolvida>");
            sb.AppendLine("     <QuantidadeCancelada>0.0000</QuantidadeCancelada>");
            sb.AppendLine("     <DataDevolucao xsi:nil=\"true\" />");
            sb.AppendLine("     <DataDevolucaoUsuario xsi:nil=\"true\" />");
            sb.AppendLine("     <DescricaoDevolucao />");
            sb.AppendLine("     <ValorTabela>118.3500000000</ValorTabela>");
            sb.AppendLine("     <ValorOriginal>118.3500000000</ValorOriginal>");
            sb.AppendLine("     <PrecoNegociado>118.3500000000</PrecoNegociado>");
            sb.AppendLine("     <PrecoMinimo>0.00</PrecoMinimo>");
            sb.AppendLine("     <SituacaoItem>1</SituacaoItem>");
            sb.AppendLine("     <UsuarioImplantacao>ve888001</UsuarioImplantacao>");
            sb.AppendLine("     <UsuarioAlteracao />");
            sb.AppendLine("     <DataAlteracao xsi:nil=\"true\" />");
            sb.AppendLine("     <UsuarioCancelamento />");
            sb.AppendLine("     <DataCancelamentoSequencia xsi:nil=\"true\" />");
            sb.AppendLine("     <DataCancelamentoUsuario xsi:nil=\"true\" />");
            sb.AppendLine("     <UsuarioReativacao />");
            sb.AppendLine("     <DataReativacao xsi:nil=\"true\" /><DataReativacaoUsuario xsi:nil=\"true\" />");
            sb.AppendLine("     <UsuarioSuspensao /><DataSuspensao xsi:nil=\"true\" /><DataSuspensaoUsuario xsi:nil=\"true\" />");
            sb.AppendLine("     <AliquotaIPI>10.00</AliquotaIPI>");
            sb.AppendLine("     <RetemICMSFonte>false</RetemICMSFonte>");
            sb.AppendLine("     <PercentualDescontoICMS>0.0</PercentualDescontoICMS>");
            sb.AppendLine("     <ValorLiquido>118.3500000000</ValorLiquido>");
            sb.AppendLine("     <ValorLiquidoAberto>0.0000000000</ValorLiquidoAberto><ValorMercadoriaAberto>0.0000000000</ValorMercadoriaAberto>");
            sb.AppendLine("     <ValorTotal>118.3500000000</ValorTotal><TipoPreco>1</TipoPreco>");
            sb.AppendLine("     <Observacao /><QuantidadeAlocada>0.0000</QuantidadeAlocada><SituacaoAlocacao>1</SituacaoAlocacao>");
            sb.AppendLine("     <PercentualMinimoFaturamento>0.0</PercentualMinimoFaturamento>");
            sb.AppendLine("     <DataMaximaFaturamento xsi:nil=\"true\" /><DataMinimaFaturamento>2012-04-25</DataMinimaFaturamento>");
            sb.AppendLine("     <PermiteSubstituirPreco>false</PermiteSubstituirPreco><FaturaQuantidadeFamilia>false</FaturaQuantidadeFamilia>");
            sb.AppendLine("     <TaxaCambio>0.0</TaxaCambio><DescontoManual>0.0000000000</DescontoManual><CondicaoFrete>1</CondicaoFrete>");
            sb.AppendLine("     <RetiraNoLocal>false</RetiraNoLocal><ValorTotalImposto>0.0</ValorTotalImposto>");
            sb.AppendLine("     <ValorSubstituicaoTributaria>-11.84</ValorSubstituicaoTributaria><ValorIPI>0.0000</ValorIPI><ProdutoForaCatalogo>false</ProdutoForaCatalogo>");
            sb.AppendLine("     <DescricaoProdutoForaCatalogo /><Moeda>0</Moeda><UnidadeNegocio>TER</UnidadeNegocio><Acao>A</Acao>");
            sb.AppendLine("     <Representante>4000</Representante><NomeAbreviadoCliente>ZZ MOVEIS</NomeAbreviadoCliente>");
            sb.AppendLine("     <EnderecoEntrega><NomeEndereco>AV. FLORIANOPOLIS, 843 - SL. A</NomeEndereco><TipoEndereco>1</TipoEndereco><CaixaPostal /><CEP>78840000</CEP><Logradouro>AV. FLORIANOPOLIS</Logradouro><Numero>843</Numero><Complemento>SL. A</Complemento><Bairro>CENTRO</Bairro><NomeCidade>CAMPO VERDE</NomeCidade><Cidade>CAMPO VERDE,MT,Brasil</Cidade><UF>MT</UF><Estado>Brasil,MT</Estado><NomePais>Brasil</NomePais><Pais>Brasil</Pais><NomeContato>ZZ MOVEIS LTDA</NomeContato><Telefone>6634611819</Telefone><Fax /></EnderecoEntrega>");
            sb.AppendLine("     </PedidoItem>");
            sb.AppendLine("     </PedidosItens>");


            sb.AppendLine("    </MSG0091>");
            sb.AppendLine("  </CONTEUDO>");
            sb.AppendLine("</MENSAGEM>");

            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String msgSaida = String.Empty;
            string xml = sb.ToString();
            integ.Postar(usuario, senha, xml, out msgSaida);

        }

        [Test]
        public void TesteEnvioPedido()
        {

            Domain.Model.Pedido pedido = new Domain.Model.Pedido(this.OrganizationName, this.IsOffline);

            pedido = new Domain.Servicos.PedidoService(this.OrganizationName, this.IsOffline).BuscaPedido(new Guid("5D5FE3E4-A5DB-E311-B278-00155D01330E"));

            MSG0091 pedidoXml = new MSG0091(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0091");


            #region Propriedades - Pedido pedidoXml->pedido

            if (!String.IsNullOrEmpty(pedido.UsuarioAprovacao))
                pedidoXml.UsuarioAprovacao = pedido.UsuarioAprovacao;

            pedidoXml.FaturamentoParcial = pedido.FaturamentoParcial;

            if (pedido.Modalidade.HasValue)
            {
                // if (System.Enum.IsDefined(typeof(Intelbras.CRM2013.Util.Utilitario pedidoXml2013.Domain.Enum.Pedido.Modalidade), pedido.ModalidadeCobranca))
                pedidoXml.ModalidadeCobranca = pedido.Modalidade;
                //else
                //{
                //    resultadoPersistencia.Sucesso = false;
                //    resultadoPersistencia.Mensagem = "Modalidade Cobranca não encontrada!";
                //    return pedidoXml;
                //}
            }

            //if (pedido.CanalVendaID.HasValue)
            //{
            CanaldeVenda canalDeVenda = new CanaldeVenda(this.OrganizationName, this.IsOffline);
            canalDeVenda = new Intelbras.CRM2013.Domain.Servicos.CanalDeVendaService(this.OrganizationName, this.IsOffline).BuscaCanalDeVenda(pedido.CanalVendaID.Id);
            //if (canalDeVenda != null)
            pedidoXml.CanalVenda = canalDeVenda.CodigoVenda;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Canal de Venda não encontrado.";
            //    return pedidoXml;
            //}
            //}

            if (!String.IsNullOrEmpty(pedido.PedidoCliente))
                pedidoXml.NumeroPedidoCliente = pedido.PedidoCliente;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "NumeroPedidoCliente não enviado.";
            //    return pedidoXml;
            //}

            //if (System.Enum.IsDefined(typeof(Intelbras.pedidoXml2013.Domain.Enum.Pedido.SituacaoAlocacao), pedido.SituacaoAlocacao))
            pedidoXml.SituacaoAlocacao = pedido.SituacaoAlocacao;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Situacao Alocacao não encontrado!";
            //    return pedidoXml;
            //}

            //if (!String.IsNullOrEmpty(pedido.MotivoBloqueioCredito))
            pedidoXml.MotivoBloqueioCredito = pedido.MotivoBloqueioCredito;

            //if (pedido.TotalSubstituicaoTributaria.HasValue)
            pedidoXml.TotalSubstituicaoTributaria = pedido.TotalSubstituicaoTributaria;

            //pedidoXml.DataImplantacao = pedido.DataImplantacao;

            //if (!String.IsNullOrEmpty(pedido.UsuarioAlteracao))
            pedidoXml.UsuarioAlteracao = pedido.UsuarioAlteracao;

            //if (pedido.CondicaoPagamento.HasValue)

            //    pedidoXml.CondicaoPagamento = pedido.CondicaoPagamento;

            ////CondicaoPagamento

            //if (xml.CondicaoPagamento.HasValue)
            //{
            CondicaoPagamento condPgto = new Domain.Servicos.CondicaoPagamentoService(this.OrganizationName, this.IsOffline).BuscaCondicaoPagamento(pedido.CondicaoPagamento.Id);
            pedidoXml.CondicaoPagamento = condPgto.Codigo;
            //    if (condPgto != null)
            //    {
            //        crm.CondicaoPagamento = new Lookup(condPgto.ID.Value, "");
            //    }
            //}



            //if (!String.IsNullOrEmpty(pedido.CodigoEntregaTriangular))
            pedidoXml.CodigoEntregaTriangular = pedido.CodigoEntregaTriangular;

            //if (!String.IsNullOrEmpty(pedido.UsuarioCancelamento))
            pedidoXml.UsuarioCancelamento = pedido.UsuarioCancelamento;

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Telefone))
            pedidoXml.EnderecoEntrega.Telefone = pedido.TelefoneEntrega;


            //if (System.Enum.IsDefined(typeof(Intelbras.pedidoXml2013.Domain.Enum.Pedido.DestinoMercadoria), pedido.DestinoMercadoria))
            if (pedido.DestinoMercadoria.HasValue)
                pedidoXml.DestinoMercadoria = pedido.DestinoMercadoria.Value;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Destino Mercadoria não encontrado!";
            //    return pedidoXml;
            //}


            //if (pedido.PercentualDescontoICMS.HasValue)
            //    pedidoXml.PercentualDescontoICMS = pedido.PercentualDescontoICMS;

            //if (!String.IsNullOrEmpty(pedido.UsuarioReativacao))
            //    pedidoXml.UsuarioReativacao = pedido.UsuarioReativacao;

            //if (!String.IsNullOrEmpty(pedido.ObservacaoRedespacho))
            //    pedidoXml.CondicoesRedespacho = pedido.ObservacaoRedespacho;

            //pedidoXml.RazaoStatus = pedido.SituacaoPedido;

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Estado))
            //{
            //    Model.Estado estado = new Model.Estado(this.Organizacao, this.IsOffline);
            //    estado = new Intelbras.pedidoXml2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaEstado(pedido.EnderecoEntrega.Estado);

            //    if (estado != null && estado.ID.HasValue)
            //        pedidoXml.EnderecoEntregaEstado = new Lookup(estado.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Estado não encontrado!";
            //        return pedidoXml;
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Estado não enviado.";
            //    return pedidoXml;
            //}

            //Estabelecimento estabelecimento = new Estabelecimento(this.Organizacao, this.IsOffline);
            //estabelecimento = new Intelbras.pedidoXml2013.Domain.Servicos.EstabelecimentoService(this.Organizacao, this.IsOffline).BuscaEstabelecimentoPorCodigo(pedido.Estabelecimento);
            //if (estabelecimento != null)
            //{
            //    pedidoXml.Estabelecimento = new Lookup(estabelecimento.ID.Value, "");
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Estabelecimento não encontrado.";
            //    return pedidoXml;
            //}


            //if (pedido.ValorTotalAberto.HasValue)
            //    pedidoXml.ValorTotalAberto = pedido.ValorTotalAberto;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Valor Total Aberto não enviado.";
            //    return pedidoXml;
            //}

            //pedidoXml.CPFCNPJ = !String.IsNullOrEmpty(pedido.CPF) ? pedido.CPF : !String.IsNullOrEmpty(pedido.CNPJ) ? pedido.CNPJ : String.Empty;
            //if (String.IsNullOrEmpty(pedidoXml.CPFCNPJ))
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "CPF/CNPJ não enviado.";
            //    return pedidoXml;
            //}

            //if (!String.IsNullOrEmpty(pedido.NumeroPedidoRepresentante))
            //    pedidoXml.PedidoRepresentante = pedido.NumeroPedidoRepresentante;

            //if (pedido.DataCancelamento.HasValue)
            //    pedidoXml.DataCancelamento = pedido.DataCancelamento;

            //pedidoXml.DataEmissao = pedido.DataEmissao;

            //if (pedido.TipoPreco.HasValue)
            //{
            //    if (System.Enum.IsDefined(typeof(Intelbras.pedidoXml2013.Domain.Enum.Pedido.TipoPreco), pedido.TipoPreco))
            //        pedidoXml.TipoPreco = pedido.TipoPreco;
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Tipo Preco não encontrado!";
            //        return pedidoXml;
            //    }
            //}

            //if (!String.IsNullOrEmpty(pedido.MotivoLiberacaoCredito))
            //    pedidoXml.MotivoLiberacaoCredito = pedido.MotivoLiberacaoCredito;

            //if (pedido.CondicaoPagamento.HasValue)
            //{
            //    CondicaoPagamento condicaoPagamento = new CondicaoPagamento(this.Organizacao, this.IsOffline);
            //    condicaoPagamento = new Intelbras.pedidoXml2013.Domain.Servicos.CondicaoPagamentoService(this.Organizacao, this.IsOffline).BuscaCondicaoPagamentoPorCodigo(pedido.CondicaoPagamento.Value);
            //    if (condicaoPagamento != null)
            //        pedidoXml.CondicaoPagamento = new Lookup(condicaoPagamento.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "CondicaoPagamento não encontrado.";
            //        return pedidoXml;
            //    }
            //}

            //if (pedido.CondicaoFrete.HasValue)
            //{
            //    if (System.Enum.IsDefined(typeof(Intelbras.pedidoXml2013.Domain.Enum.Pedido.CondicoesFrete), pedido.CondicaoFrete))
            //        pedidoXml.CondicoesFrete = pedido.CondicaoFrete;
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Condições de frete não encontrado!";
            //        return pedidoXml;
            //    }
            //}

            //if (!String.IsNullOrEmpty(pedido.TabelaFinanciamento))
            //{
            //    TabelaFinanciamento tabelaFinanciamento = new TabelaFinanciamento(this.Organizacao, this.IsOffline);
            //    tabelaFinanciamento = new Intelbras.pedidoXml2013.Domain.Servicos.TabelaFinanciamentoService(this.Organizacao, this.IsOffline).ObterTabelaFinanciamento(pedido.TabelaFinanciamento);
            //    if (tabelaFinanciamento != null)
            //        pedidoXml.TabelaFinanciamento = new Lookup(tabelaFinanciamento.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Tabela Financiamento não encontrada.";
            //        return pedidoXml;
            //    }
            //}

            //if (!String.IsNullOrEmpty(pedido.CidadeCIF))
            //    pedidoXml.CidadeCIF = pedido.CidadeCIF;

            //if (!String.IsNullOrEmpty(pedido.InscricaoEstadual))
            //    pedidoXml.InscricaoEstadual = pedido.InscricaoEstadual;

            //if (pedido.IndicacaoAprovacao.HasValue)
            //    pedidoXml.Aprovacao = pedido.IndicacaoAprovacao;

            //if (!String.IsNullOrEmpty(pedido.UsuarioAprovacao))
            //    pedidoXml.Aprovador = pedido.UsuarioAprovacao;

            //if (!String.IsNullOrEmpty(pedido.AprovacaoForcada))
            //    pedidoXml.AprovacaoForcadoPedido = pedido.AprovacaoForcada;

            //if (pedido.ValorMercadoriaAberto.HasValue)
            //    pedidoXml.ValorMercadoriaAberto = pedido.ValorMercadoriaAberto;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Valor Mercadoria Aberto não enviado.";
            //    return pedidoXml;
            //}

            //if (pedido.TipoFrete.HasValue)
            //    pedidoXml.CondicoesFreteEntrega = pedido.TipoFrete;

            //if (pedido.DataReativacaoUsuario.HasValue)
            //    pedidoXml.DataReativacaoUsuario = pedido.DataReativacaoUsuario;

            //if (pedido.DataCancelamentoUsuario.HasValue)
            //    pedidoXml.DataCancelamentoUsuario = pedido.DataCancelamentoUsuario;

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.CaixaPostal))
            //    pedidoXml.EnderecoEntregaCaixaPostal = pedido.EnderecoEntrega.CaixaPostal;

            //if (pedido.ValorTotalLiquido.HasValue)
            //    pedidoXml.ValorTotalProdutosSemIPI = pedido.ValorTotalLiquido;

            //if (!String.IsNullOrEmpty(pedido.Descricao))
            //    pedidoXml.Descricao = pedido.Descricao;


            //if (!String.IsNullOrEmpty(pedido.NaturezaOperacao))
            //{
            //    NaturezaOperacao naturezaOperacao = new NaturezaOperacao(this.Organizacao, this.IsOffline);
            //    naturezaOperacao = new Intelbras.pedidoXml2013.Domain.Servicos.NaturezaOperacaoService(this.Organizacao, this.IsOffline).BuscaNaturezaOperacaoPorCodigo(pedido.NaturezaOperacao);

            //    if (naturezaOperacao != null)
            //        pedidoXml.NaturezaOperacao = new Lookup(naturezaOperacao.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Natureza Operacao não encontrada.";
            //        return pedidoXml;
            //    }
            //}

            //if (pedido.PercentualDesconto2.HasValue)
            //    pedidoXml.PercentualDesconto2 = pedido.PercentualDesconto2;


            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.NomeContato))
            //    pedidoXml.NomeContatoEntrega = pedido.EnderecoEntrega.NomeContato;


            //if (!String.IsNullOrEmpty(pedido.Rota))
            //{
            //    Rota rota = new Rota(this.Organizacao, this.IsOffline);
            //    rota = new Intelbras.pedidoXml2013.Domain.Servicos.RotaService(this.Organizacao, this.IsOffline).BuscaRotaPorCodigo(pedido.Rota);

            //    if (rota != null)
            //        pedidoXml.Rota = new Lookup(rota.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Rota não encontrada.";
            //        return pedidoXml;
            //    }
            //}

            //if (!String.IsNullOrEmpty(pedido.UsuarioSuspensao))
            //    pedidoXml.UsuarioSuspensao = pedido.UsuarioSuspensao;

            //if (pedido.Representante.HasValue)
            //{
            //    Contato contato = new Contato(this.Organizacao, this.IsOffline);
            //    contato = new Intelbras.pedidoXml2013.Domain.Servicos.ContatoService(this.Organizacao, this.IsOffline).BuscaContatoPorCodigoRepresentante(pedido.Representante.Value.ToString());

            //    if (contato != null)
            //        pedidoXml.KeyAccountRepresentante = new Lookup(contato.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Representante não encontrado.";
            //        return pedidoXml;
            //    }
            //}

            //if (!String.IsNullOrEmpty(pedido.UsuarioImplantacao))
            //    pedidoXml.UsuarioIntegracao = pedido.UsuarioImplantacao;

            //if (pedido.ValorFrete.HasValue)
            //    pedidoXml.ValorFrete = pedido.ValorFrete;

            //if (pedido.DataAprovacao.HasValue)
            //    pedidoXml.DataAprovacao = pedido.DataAprovacao;

            //if (!String.IsNullOrEmpty(pedido.Nome))
            //    pedidoXml.Nome = pedido.Nome;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Nome não enviado.";
            //    return pedidoXml;
            //}

            //if (pedido.RetiraNoLocal.HasValue)
            //    pedidoXml.Remessa = pedido.RetiraNoLocal.Value;

            //if (!String.IsNullOrEmpty(pedido.DescricaoSuspensao))
            //    pedidoXml.DescricaoSuspensao = pedido.DescricaoSuspensao;

            //pedidoXml.DataEntregaSolicitada = pedido.DataEntregaSolicitada;

            //pedidoXml.DataImplantacaoUsuario = pedido.DataImplantacaoUsuario;

            //if (pedido.SituacaoAvaliacao.HasValue)
            //{
            //    if (System.Enum.IsDefined(typeof(Intelbras.pedidoXml2013.Domain.Enum.Pedido.SituacaoAvaliacao), pedido.SituacaoAvaliacao))
            //        pedidoXml.CodigoSituacaoAvaliacao = pedido.SituacaoAvaliacao;
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Situacao Avaliacao não encontrada!";
            //        return pedidoXml;
            //    }

            //}
            //if (!String.IsNullOrEmpty(pedido.TipoObjetoCliente) && !String.IsNullOrEmpty(pedido.CodigoClientepedidoXml))
            //{
            //    String tipoObjetoCliente;
            //    if (pedido.TipoObjetoCliente == "account" || pedido.TipoObjetoCliente == "contact")
            //    {
            //        tipoObjetoCliente = pedido.TipoObjetoCliente;
            //        pedidoXml.ClienteID = new Lookup(new Guid(pedido.CodigoClientepedidoXml), pedido.TipoObjetoCliente);
            //    }
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "TipoObjetoCliente ou CodigoCliente fora do padrão.";
            //        return pedidoXml;
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "TipoObjetoCliente ou CodigoCliente não enviado.";
            //    return pedidoXml;
            //}

            //if (pedido.Prioridade.HasValue)
            //    pedidoXml.Prioridade = pedido.Prioridade;

            //if (pedido.Transportadora.HasValue)
            //{
            //    Transportadora transportadora = new Transportadora(this.Organizacao, this.IsOffline);
            //    transportadora = new Intelbras.pedidoXml2013.Domain.Servicos.TransportadoraService(this.Organizacao, this.IsOffline).ObterPorCodigoTransportadora(pedido.Transportadora.Value);
            //    if (transportadora != null)
            //        pedidoXml.Transportadora = new Lookup(transportadora.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Transportadora não encontrada.";
            //        return pedidoXml;
            //    }
            //}

            //if (!String.IsNullOrEmpty(pedido.Oportunidade))
            //    pedidoXml.Oportunidade = new Lookup(new Guid(pedido.Oportunidade), "");
            ////Não obrigatorio
            ////else
            ////{
            ////    resultadoPersistencia.Sucesso = false;
            ////    resultadoPersistencia.Mensagem = "Oportunidade não Enviada.";
            ////    return pedidoXml;
            ////}

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Bairro))
            //    pedidoXml.BairroEntrega = pedido.EnderecoEntrega.Bairro;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Bairro não enviado.";
            //    return pedidoXml;
            //}

            //if (!String.IsNullOrEmpty(pedido.CondicaoEspecial))
            //    pedidoXml.CondicoesEspeciais = pedido.CondicaoEspecial;

            ////Removido Pollux
            ////if (pedido.TabelaPreco.HasValue)
            ////{
            ////    TabelaPreco tabelaPreco = new TabelaPreco(this.Organizacao, this.IsOffline);
            ////    tabelaPreco = new Intelbras.pedidoXml2013.Domain.Servicos.TabelaPrecoService(this.Organizacao, this.IsOffline).BuscaTabelaPrecoPorCodigo(pedido.TabelaPreco.Value);
            ////    if (tabelaPreco != null)
            ////        pedidoXml.TabelaPreco = new Lookup(tabelaPreco.ID.Value, "");
            ////}


            //if (pedido.DataAlteracao.HasValue)
            //    pedidoXml.DataAlteracao = pedido.DataAlteracao;

            //// Moeda - service
            //if (!String.IsNullOrEmpty(pedido.Moeda))
            //{
            //    Model.Moeda moeda = new Model.Moeda(this.Organizacao, this.IsOffline);
            //    moeda = new Intelbras.pedidoXml2013.Domain.Servicos.MoedaService(this.Organizacao, this.IsOffline).BuscaMoedaPorNome(pedido.Moeda);

            //    if (moeda != null && moeda.ID.HasValue)
            //        pedidoXml.Moeda = new Lookup(moeda.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Moeda não encontrada!";
            //        return pedidoXml;
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Moeda não enviada.";
            //    return pedidoXml;
            //}

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.CEP))
            //    pedidoXml.CEPEntrega = pedido.EnderecoEntrega.CEP;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "CEP não enviado.";
            //    return pedidoXml;
            //}

            ////if (!String.IsNullOrEmpty(pedido.CampanhaOrigem))
            //// pedidoXml.CampanhaID = pedido.CampanhaOrigem;

            //if (pedido.Mensagem.HasValue)
            //{
            //    Mensagem mensagem = new Mensagem(this.Organizacao, this.IsOffline);
            //    mensagem = new Intelbras.pedidoXml2013.Domain.Servicos.MensagemService(this.Organizacao, this.IsOffline).BuscaMensagemPorCodigo(pedido.Mensagem.Value);
            //    if (mensagem != null)
            //        pedidoXml.Mensagem = new Lookup(mensagem.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Mensagem do pedido não encontrada.";
            //        return pedidoXml;
            //    }
            //}

            //if (pedido.PrecoBloqueado.HasValue)
            //    pedidoXml.PrecoBloqueado = pedido.PrecoBloqueado;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Preco Bloqueado não enviado.";
            //    return pedidoXml;
            //}

            //if (!String.IsNullOrEmpty(pedido.NumeroPedido))
            //{
            //    pedidoXml.PedidoEMS = pedido.NumeroPedido;
            //    pedidoXml.IDPedido = pedido.NumeroPedido;
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "NumeroPedido não enviado.";
            //    return pedidoXml;
            //}

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Logradouro))
            //    pedidoXml.EnderecoEntregaRua = pedido.EnderecoEntrega.Logradouro;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Logradouro não enviado.";
            //    return pedidoXml;
            //}
            //if (pedido.PedidoCompleto.HasValue)
            //    pedidoXml.Completo = pedido.PedidoCompleto;

            //if (pedido.DataLimiteFaturamento.HasValue)
            //    pedidoXml.DataLimiteFaturamento = pedido.DataLimiteFaturamento;

            //if (!String.IsNullOrEmpty(pedido.CodigoEntrega))
            //    pedidoXml.CodigoEntrega = pedido.CodigoEntrega;

            //if (!String.IsNullOrEmpty(pedido.NomeAbreviadoCliente))
            //    pedidoXml.NomeAbreviado = pedido.NomeAbreviadoCliente;

            //if (!String.IsNullOrEmpty(pedido.ClienteTriangular))
            //{
            //    pedidoXml.ClienteTriangular = new Lookup(new Guid(pedido.ClienteTriangular), "account");
            //}

            //if (pedido.DiasNegociacao.HasValue)
            //    pedidoXml.DiasNegociacao = pedido.DiasNegociacao;

            //if (pedido.DataNegociacao.HasValue)
            //    pedidoXml.DataNegociacao = pedido.DataNegociacao;

            //if (pedido.DataCumprimento.HasValue)
            //    pedidoXml.DataCumprimento = pedido.DataCumprimento;

            //if (!String.IsNullOrEmpty(pedido.Classificacao))
            //{
            //    pedidoXml.Classificacao = new Lookup(new Guid(pedido.Classificacao), "");
            //}

            //if (!String.IsNullOrEmpty(pedido.DescricaoCancelamento))
            //    pedidoXml.DescricaoCancelamento = pedido.DescricaoCancelamento;

            //if (pedido.OrigemPedido.HasValue)
            //{
            //    if (System.Enum.IsDefined(typeof(Intelbras.pedidoXml2013.Domain.Enum.Pedido.OrigemPedido), pedido.OrigemPedido))
            //        pedidoXml.Origem = pedido.OrigemPedido;
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Origem Pedido não encontrado!";
            //        return pedidoXml;
            //    }
            //}
            //if (pedido.DataReativacao.HasValue)
            //    pedidoXml.DataReativacao = pedido.DataReativacao;

            //if (pedido.ValorCreditoLiberado.HasValue)
            //    pedidoXml.ValorCreditoLiberado = pedido.ValorCreditoLiberado;

            ////Nao preencher tabelaprecoEMS - orientado por Jose.

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Complemento))
            //    pedidoXml.ComplementoEntrega = pedido.EnderecoEntrega.Complemento;

            //if (pedido.TotalIPI.HasValue)
            //    pedidoXml.TotalIPI = pedido.TotalIPI;

            //if (pedido.DataMinimaFaturamento.HasValue)
            //    pedidoXml.DataMinimaFaturamento = pedido.DataMinimaFaturamento;

            //if (pedido.DataSuspensao.HasValue)
            //    pedidoXml.DataSuspensao = pedido.DataSuspensao;

            //if (pedido.PercentualDesconto1.HasValue)
            //    pedidoXml.PercentualDesconto1 = pedido.PercentualDesconto1;


            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Cidade))
            //{
            //    Model.Municipio cidade = new Model.Municipio(this.Organizacao, this.IsOffline);
            //    cidade = new Intelbras.pedidoXml2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaMunicipio(pedido.EnderecoEntrega.Cidade);

            //    if (cidade != null && cidade.ID.HasValue)
            //        pedidoXml.EnderecoEntregaCidade = new Lookup(cidade.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Cidade não encontrada!";
            //        return pedidoXml;
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Município não enviado.";
            //    return pedidoXml;
            //}


            ////if (!String.IsNullOrEmpty(pedido.Cotacao))
            ////    pedidoXml.Cotacao = pedido.Cotacao;

            //if (pedido.ValorTotalDesconto.HasValue)
            //    pedidoXml.DescontoGlobalAdicional = pedido.ValorTotalDesconto.ToString();

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Fax))
            //    pedidoXml.FaxEntrega = pedido.EnderecoEntrega.Fax;

            //if (pedido.DataEntrega.HasValue)
            //    pedidoXml.DataEntrega = pedido.DataEntrega;

            //// País
            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Pais))
            //{
            //    Model.Pais pais = new Model.Pais(this.Organizacao, this.IsOffline);
            //    pais = new Intelbras.pedidoXml2013.Domain.Servicos.EnderecoServices(this.Organizacao, this.IsOffline).BuscaPais(pedido.EnderecoEntrega.Pais);

            //    if (pais != null && pais.ID.HasValue)
            //        pedidoXml.EnderecoEntregaPais = new Lookup(pais.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "País não encontrado.";
            //        return pedidoXml;
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "País não enviado.";
            //    return pedidoXml;
            //}

            ////if (!String.IsNullOrEmpty(pedido.PedidoOriginal))
            ////{
            ////    pedidoXml.PedidoEMS = pedido.PedidoOriginal;
            ////}

            //if (!String.IsNullOrEmpty(pedido.EnderecoEntrega.Numero))
            //    pedidoXml.EnderecoEntregaNumero = pedido.EnderecoEntrega.Numero;
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Numero endereco do pedido não enviado.";
            //    return pedidoXml;
            //}

            //if (pedido.Portador.HasValue)
            //{
            //    Portador portador = new Portador(this.Organizacao, this.IsOffline);
            //    portador = new Intelbras.pedidoXml2013.Domain.Servicos.PortadorService(this.Organizacao, this.IsOffline).BuscaPorCodigo(pedido.Portador.Value);
            //    if (portador != null)
            //        pedidoXml.Portador = new Lookup(portador.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Portador do pedido não encontrado.";
            //        return pedidoXml;
            //    }
            //}

            //pedidoXml.DataEntregaOriginal = pedido.DataEntregaSolicitada;

            //if (!String.IsNullOrEmpty(pedido.TipoPedido))
            //    pedidoXml.TipoPedido = pedido.TipoPedido;

            //if (pedido.PercentualDesconto.HasValue)
            //    pedidoXml.DescontoGlobalAdicional = pedido.PercentualDesconto.ToString();

            //pedidoXml.Status = pedido.Situacao;

            //if (!String.IsNullOrEmpty(pedido.CodigoPedido))
            //{
            //    Pedido pedido = new Pedido(this.Organizacao, this.IsOffline);
            //    pedido = new Intelbras.pedidoXml2013.Domain.Servicos.PedidoService(this.Organizacao, this.IsOffline).BuscaPedidoEMS(pedido.CodigoPedido);
            //    if (pedido != null)
            //        pedidoXml.ID = pedido.ID;
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "Pedido não encontrado.";
            //        return pedidoXml;
            //    }

            //}

            //// ListaPreco
            //if (!String.IsNullOrEmpty(pedido.ListaPreco))
            //{
            //    Model.ListaPreco listaPreco = new Model.ListaPreco(this.Organizacao, this.IsOffline);
            //    listaPreco = new Intelbras.pedidoXml2013.Domain.Servicos.ListaPrecoService(this.Organizacao, this.IsOffline).BuscaListaPreco(pedido.ListaPreco);

            //    if (listaPreco != null && listaPreco.ID.HasValue)
            //        pedidoXml.ListaPreco = new Lookup(listaPreco.ID.Value, "");
            //    else
            //    {
            //        resultadoPersistencia.Sucesso = false;
            //        resultadoPersistencia.Mensagem = "ListaPreco não encontrado!";
            //        return pedidoXml;
            //    }
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "ListaPreco não enviada.";
            //    return pedidoXml;
            //}


            //if (!String.IsNullOrEmpty(pedido.Proprietario) && !String.IsNullOrEmpty(pedido.TipoProprietario))
            //{
            //    if ((!String.IsNullOrEmpty(pedido.TipoProprietario) && (pedido.TipoProprietario == "team" || pedido.TipoProprietario == "systemuser")))
            //        tipoProprietario = pedido.TipoProprietario;
            //    else
            //        tipoProprietario = "systemuser";

            //    pedidoXml.Proprietario = new Lookup(new Guid(pedido.Proprietario), tipoProprietario);
            //}
            //else
            //{
            //    resultadoPersistencia.Sucesso = false;
            //    resultadoPersistencia.Mensagem = "Proprietário ou Tipo Proprietário não enviado.";
            //    return pedidoXml;
            //}
            //pedidoXml.IntegradoEm = DateTime.Now;
            //pedidoXml.IntegradoPor = usuarioIntegracao.NomeCompleto;
            //pedidoXml.UsuarioIntegracao = pedido.LoginUsuario;

            #endregion


        }

    }
}
