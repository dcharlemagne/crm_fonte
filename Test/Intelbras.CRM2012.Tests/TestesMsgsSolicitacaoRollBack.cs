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
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;


namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class TestesMsgsSolicitacaoRollBack : Base
    {

        [Test]
        public void Validar_Existencia_Registros()
        {
            Domain.Integracao.MSG0154 msgRebate = new Domain.Integracao.MSG0154(OrganizationName, false);

            String teste = String.Empty;
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><MENSAGEM>  <CABECALHO>    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor> 	<NumeroOperacao>Solicitação de Rebate Pós-Venda para ICO</NumeroOperacao> 	<CodigoMensagem>MSG0154</CodigoMensagem>    <LoginUsuario>Carlos Alberto</LoginUsuario> 	</CABECALHO>  <CONTEUDO>    <MSG0154> <NomeSolicitacaoBeneficio>Solicitação de Rebate Pós-Venda para ICON</NomeSolicitacaoBeneficio>	<CodigoTipoSolicitacao>44b50f47-8b5f-e411-9424-00155d013d3a</CodigoTipoSolicitacao>    <CodigoBeneficio>7205dbc0-8403-e411-9420-00155d013d39</CodigoBeneficio>    <BeneficioCodigo>66</BeneficioCodigo>  	<CodigoBeneficioCanal>850a4662-105e-e411-9424-00155d013d3a</CodigoBeneficioCanal>     	<CodigoUnidadeNegocio>TER</CodigoUnidadeNegocio>      <CodigoConta>b2eaf028-4c5d-e411-940b-00155d014212</CodigoConta>   	<ValorSolicitado>537.59</ValorSolicitado>      <DescricaoSolicitacao>Solicitação de rebate pós venda 3º trimestre. </DescricaoSolicitacao> 	<SolicitacaoIrregular>true</SolicitacaoIrregular>      <DescricaoSituacaoIrregular>Canal com Benefícios Suspensos</DescricaoSituacaoIrregular> 	<CodigoFormaPagamento>c67df108-9c03-e411-9420-00155d013d39</CodigoFormaPagamento>      <SituacaoSolicitacaoBeneficio>993520005</SituacaoSolicitacaoBeneficio>      <RazaoStatusSolicitacaoBeneficio>1</RazaoStatusSolicitacaoBeneficio>      <Situacao>0</Situacao>   	<Proprietario>b4eaf028-4c5d-e411-940b-00155d014212</Proprietario>      <TipoProprietario>team</TipoProprietario>      <CodigoAssistente>36</CodigoAssistente>      <CodigoSupervisorEMS>al027000</CodigoSupervisorEMS>    	<SolicitacaoAjuste>false</SolicitacaoAjuste>      <ValorAbater>537.59</ValorAbater>      	<ProdutoSolicitacaoItens>       <ProdutoSolicitacaoItem>     	<CodigoSolicitacaoBeneficio>03ed5289-9f7f-e411-942c-00155d013d5d</CodigoSolicitacaoBeneficio>        	<CodigoProduto>4070350</CodigoProduto>       <CodigoBeneficio>7205dbc0-8403-e411-9420-00155d013d39</CodigoBeneficio>         <ValorUnitario>49.79</ValorUnitario>          <Quantidade>10</Quantidade>          <ValorTotal>497.9</ValorTotal>     <ValorUnitarioAprovado>49.79</ValorUnitarioAprovado>          <QuantidadeAprovado>10</QuantidadeAprovado>       	<ValorTotalAprovado>497.9</ValorTotalAprovado>          <Proprietario>b4eaf028-4c5d-e411-940b-00155d014212</Proprietario>          <TipoProprietario>team</TipoProprietario>          <Acao>I</Acao>          <CodigoEstabelecimento>104</CodigoEstabelecimento>   	<Situacao>0</Situacao>        </ProdutoSolicitacaoItem>        <ProdutoSolicitacaoItem>   	<CodigoSolicitacaoBeneficio>03ed5289-9f7f-e411-942c-00155d013d5d</CodigoSolicitacaoBeneficio>         <CodigoProduto>4070352</CodigoProduto>          <CodigoBeneficio>7205dbc0-8403-e411-9420-00155d013d39</CodigoBeneficio>  	<ValorUnitario>39.69</ValorUnitario>          <Quantidade>1</Quantidade>          <ValorTotal>39.69</ValorTotal>          <ValorUnitarioAprovado>39.69</ValorUnitarioAprovado>          <QuantidadeAprovado>1</QuantidadeAprovado>         <ValorTotalAprovado>39.69</ValorTotalAprovado>          <Proprietario>b4eaf028-4c5d-e411-940b-00155d014212</Proprietario>    <TipoProprietario>team</TipoProprietario>          <Acao>I</Acao>          <CodigoEstabelecimento>104</CodigoEstabelecimento>          <Situacao>0</Situacao>        </ProdutoSolicitacaoItem>      </ProdutoSolicitacaoItens>    </MSG0154>  </CONTEUDO></MENSAGEM>";
            MSG0154 msg = MessageBase.LoadMessage<MSG0154>(XDocument.Parse(xml));
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            integ.Postar(usuario, senha, msg.GenerateMessage(false), out teste);
        }

        [Test]
        public void Validar_Existencia_Registros2()
        {

        }

    }
}
