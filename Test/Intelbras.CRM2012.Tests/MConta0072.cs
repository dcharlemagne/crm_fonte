using System;
using NUnit.Framework;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Domain;
using System.Web;
using System.Web.Services;
using Intelbras.Message.Helper;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;
using System.Text;

namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class MConta0072 : Base
    {

        [Test]
        public void testeConta72Konviva()
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?><MENSAGEM>  <CABECALHO>    <IdentidadeEmissor>DBFC273E-4811-40C4-8A4E-1629731ADD9A</IdentidadeEmissor>    <NumeroOperacao>ACK COMERCIO DE COMPONENTES ELETRONICOS </NumeroOperacao>   <CodigoMensagem>MSG0072</CodigoMensagem>  </CABECALHO> <CONTEUDO>
    <MSG0072>
      <CodigoConta>069dd596-c232-e411-9421-00155d013d39</CodigoConta>
      <CodigoCliente>140629</CodigoCliente>
      <NomeRazaoSocial>ACK COMERCIO DE COMPONENTES ELETRONICOS LTDA - EPP.</NomeRazaoSocial>
      <NomeFantasia>ALARMA</NomeFantasia>
      <NomeAbreviado>ACK COMERCIO</NomeAbreviado>
      <DescricaoConta>.</DescricaoConta>
      <TipoRelacao>993520000</TipoRelacao>
      <EmiteBloqueto>false</EmiteBloqueto>
      <GeraAvisoCredito>false</GeraAvisoCredito>
      <CalculaMulta>false</CalculaMulta>
      <RecebeInformacaoSCI>false</RecebeInformacaoSCI>
      <Telefone>4132592023</Telefone>
      <TelefoneAlternativo>4132592000</TelefoneAlternativo>
      <RamalTelefoneAlternativo>2006</RamalTelefoneAlternativo>
      <Fax>(41) 3232-8246</Fax>
      <Email>cadastro@alarma.com.br</Email>
      <Site>http:alarma@alarma.com.br</Site>
      <Natureza>993520000</Natureza>
      <CNPJ>05142150000153</CNPJ>
      <InscricaoEstadual>90355980-20</InscricaoEstadual>
      <InscricaoMunicipal>1401442.808-8</InscricaoMunicipal>
      <SuspensaoCredito>false</SuspensaoCredito>
      <ContribuinteICMS>true</ContribuinteICMS>
      <CodigoSUFRAMA>1</CodigoSUFRAMA>
      <InscricaoSubstituicaoTributaria>1</InscricaoSubstituicaoTributaria>
      <OptanteSuspensaoIPI>true</OptanteSuspensaoIPI>
      <AgenteRetencao>true</AgenteRetencao>
      <PisCofinsUnidade>true</PisCofinsUnidade>
      <RecebeNotaFiscalEletronica>true</RecebeNotaFiscalEletronica>
      <FormaTributacao>993520002</FormaTributacao>
      <DataVencimentoConcessao>2015-04-25</DataVencimentoConcessao>
      <DescontoAssistenciaTecnica>1.00</DescontoAssistenciaTecnica>
      <CoberturaGeografica>A Alarma é Nacional e atende as seguintes regiões:
Sul
Sudeste
Nordeste
</CoberturaGeografica>
      <DataConstituicao>2002-04-04</DataConstituicao>
      <DistribuicaoUnicaFonteReceita>false</DistribuicaoUnicaFonteReceita>
      <QualificadoTreinamento>Possuímos espaço físico, porem não temos uma pessoa qualificada para treinamento e instruções.</QualificadoTreinamento>
      <Exclusividade>false</Exclusividade>
      <Historico>A Alarma esta no mercado a 25 anos, sempre presente entre as maiores empresas do ramos, com credibilidade no mercado, cumprindo com o seu compromisso como distribuidora. Seus principais eventos foi ter participado da Feira Exposec durante 5 anos consecutivos 2006/2007/2008/2009 e 2010.</Historico>
      <IntencaoApoio>Suporte técnico, suporte comercial, suporte com marketing, parceria nas negociações.
</IntencaoApoio>
      <MetodoComercializacao>telemarketing, vendedor interno</MetodoComercializacao>
      <NumeroFuncionarios>11</NumeroFuncionarios>
      <NumeroColaboradoresAreaTecnica>0</NumeroColaboradoresAreaTecnica>
      <NumeroRevendasAtivas>1466</NumeroRevendasAtivas>
      <NumeroRevendasInativas>4898</NumeroRevendasInativas>
      <NumeroTecnicosSuporte>0</NumeroTecnicosSuporte>
      <NumeroVendedores>5</NumeroVendedores>
      <OutraFonteReceita>Não existe.</OutraFonteReceita>
      <ParticipaProgramaCanais>993520001</ParticipaProgramaCanais>
      <PerfilRevendasDistribuidor>Revendedores de Varejo</PerfilRevendasDistribuidor>
      <PossuiEstruturaCompleta>993520001</PossuiEstruturaCompleta>
      <PossuiFiliais>993520001</PossuiFiliais>
      <QuantidadeFiliais>0</QuantidadeFiliais>
      <PrazoMedioCompra>120.00</PrazoMedioCompra>
      <PrazoMedioVenda>90.00</PrazoMedioVenda>
      <RamoAtividadeEconomica>comercio de componentes eletronicos.</RamoAtividadeEconomica>
      <SistemaGestao>menenger</SistemaGestao>
      <ValorMedioCompra>150000.0000</ValorMedioCompra>
      <ValorMedioVenda>450000.0000</ValorMedioVenda>
      <VendeAtacadista>true</VendeAtacadista>
      <Situacao>0</Situacao>
      <Classificacao>e11378c7-6ded-e311-9407-00155d013d38</Classificacao>
      <SubClassificacao>962d89df-6eed-e311-9407-00155d013d38</SubClassificacao>
      <NivelPosVenda>c17d5b87-75ed-e311-9407-00155d013d38</NivelPosVenda>
      <ApuracaoBeneficio>993520000</ApuracaoBeneficio>
      <TipoConstituicao>993520001</TipoConstituicao>
      <Proprietario>5b26fe9e-c232-e411-9421-00155d013d39</Proprietario>
      <TipoProprietario>team</TipoProprietario>
      <TipoConta>993520000</TipoConta>

      <DataAdesao>2014-07-01</DataAdesao>
      <CodigoEstrangeiro>1GW456U89D2D4D6789A</CodigoEstrangeiro>  
<OrigemConta>993520002</OrigemConta>
<NumeroPassaporte>01HQ45UDT90L234H6789</NumeroPassaporte>
<StatusIntegracaoSefaz>993520002</StatusIntegracaoSefaz>

      <DataHoraIntegracaoSefaz>2015-04-22T13:53:38</DataHoraIntegracaoSefaz>
<RegimeApuracao>RegimeApuracaohuestd</RegimeApuracao>
<DataBaixaContribuinte>2015-04-22</DataBaixaContribuinte>
      <EnderecoPrincipal>
        <TipoEndereco>3</TipoEndereco>
        <CEP>80610020</CEP>
        <Logradouro>RUA PARA </Logradouro>
        <Numero>1834</Numero>
        <Bairro>AGUA VERDE</Bairro>
        <NomeCidade>CURITIBA</NomeCidade>
        <Cidade>CURITIBA,PR,Brasil</Cidade>
        <UF>PR</UF>
        <Estado>Brasil,PR</Estado>
        <NomePais>Brasil</NomePais>
        <Pais>Brasil</Pais>
      </EnderecoPrincipal>
      <EnderecoCobranca>
        <TipoEndereco>3</TipoEndereco>
        <CEP>80610020</CEP>
        <Logradouro>RUA PARA </Logradouro>
        <Numero>1834</Numero>
        <Bairro>AGUA VERDE</Bairro>
        <NomeCidade>CURITIBA</NomeCidade>
        <Cidade>CURITIBA,PR,Brasil</Cidade>
        <UF>PR</UF>
        <Estado>Brasil,PR</Estado>
        <NomePais>Brasil</NomePais>
        <Pais>Brasil</Pais>
      </EnderecoCobranca>
    </MSG0072>
  </CONTEUDO>
</MENSAGEM>");
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String msgSaida = String.Empty;
            string xml = sb.ToString();
            integ.Postar(usuario, senha, xml, out msgSaida);
        }
        [Test]
        public void TesteMConta0072()
        {
            //var t = 00;

            MSG0072 Conta = new MSG0072(itb.RetornaSistema(itb.Sistema.Pollux), "MSG0072")
            {
                AgenteRetencao = true,
                ApuracaoBeneficio = 993520000,
                CalculaMulta = true,
                Classificacao = "E1D0E59F-1E9E-E311-888D-00155D013E2E",
                ClientePotencialOriginador = "",
                CNAE = "1111",
                CNPJ = "66264213000666",
                CoberturaGeografica = "Sudeste e Sul do Brasil.",
                CodigoCliente = 419,
                CodigoConta = "",
                CodigoCRM4 = "",
                CodigoIncoterm = "88",
                CodigoSUFRAMA = "7777",
                CondicaoPagamento = 1,
                ContaPrimaria = "",
                ContatoPrincipal = "",
                ContribuinteICMS = true,
                CPF = "",
                DataConstituicao = Convert.ToDateTime("2014-04-06"),
                DataImplantacao = Convert.ToDateTime("2014-04-06"),
                DataLimiteCredito = Convert.ToDateTime("2014-04-06"),
                DataVencimentoConcessao = Convert.ToDateTime("2014-04-06"),
                DescontoAssistenciaTecnica = Convert.ToDecimal("20"),
                DescricaoConta = "Descrição Avurto",
                DistribuicaoUnicaFonteReceita = true,
                DistribuidorPrincipal = "",
                Email = "emailavurto@avurto.com",
                EmiteBloqueto = true,
                // EnderecoCobranca = "",
                EnderecoPrincipal = new Message.Helper.Entities.Endereco() { TipoEndereco = 3, Logradouro = "Rua Cel Artur de Paula Ferreira", Numero = "95", Complemento = "Apto 9", Bairro = "VNC", NomeCidade = "Fernandopolis", Cidade = "Fernandopolis,SP,Brasil", UF = "SP", Estado = "Brasil,SP", CEP = "04511060", Pais = "1", NomePais = "Brasil" },
                EstruturaPropriedade = 2,
                Exclusividade = true,
                Fax = "999999",
                FormaTributacao = 993520001,
                GeraAvisoCredito = true,
                Historico = "Ótimo Canal",
                //-- erro, ERP é invalido -- IdentidadeEmissor = "ERP",
                InscricaoEstadual = "isento",
                InscricaoMunicipal = "também",
                InscricaoSubstituicaoTributaria = "não há",
                IntencaoApoio = "Vamos apoiar esse Canal.",
                LimiteCredito = 10000000,
                ListaPreco = "",
                LocalEmbarque = "Porto",
                LoginUsuario = "Usuario de Integracao",
                MetodoComercializacao = "Comercializacao direta",
                ModalidadeCobranca = 993520000,
                ModeloOperacao = "Não há filiais",
                Natureza = 993520000,
                NivelPosVenda = "D2A1461D-2CA6-E311-888D-00155D013E2E",
                NomeAbreviado = "Avurto",
                NomeFantasia = "Avurto2 LTDA",
                NomeRazaoSocial = "Avurtinho Jr.",
                NumeroAgencia = "0262",
                NumeroBanco = "1",
                NumeroColaboradoresAreaTecnica = 5,
                NumeroContaCorrente = "01010",
                NumeroDiasAtraso = 2,
                NumeroFuncionarios = 30,
                NumeroOperacao = "0072",
                NumeroRevendasAtivas = 1,
                NumeroRevendasInativas = "0",
                NumeroTecnicosSuporte = 3,
                NumeroVendedores = 6,
                ObservacaoNotaFiscal = "Observações NF",
                ObservacaoPedido = "Observações Pedido",
                OptanteSuspensaoIPI = true,
                OrgaoExpeditor = "SSP/SC",
                OutraFonteReceita = "Outra fonte99",
                ParticipaProgramaCanais = 993520000,  //-- 993.520.000 = Não 
                PerfilRevendasDistribuidor = "Revendas vendem muito",
                PisCofinsUnidade = true,
                Portador = 2,
                PossuiEstruturaCompleta = 993520000,
                PossuiFiliais = 993520000,
                PrazoMedioCompra = Convert.ToDecimal("14,3"),
                PrazoMedioVenda = Convert.ToDecimal("20,7"),
                Proprietario = "FF3CBD6F-8E9D-E311-888D-00155D013E2E",
                QualificadoTreinamento = "1",
                QuantidadeFiliais = 0,
                Ramal = "5555",
                RamalFax = "4444",
                RamalTelefoneAlternativo = "3333",
                RamoAtividadeEconomica = "Eletronicos",
                RecebeInformacaoSCI = true,
                RecebeNotaFiscalEletronica = true,
                ReceitaAnual = 120000,
                ReceitaPadrao = 8,
                Regiao = "",
                RG = "24",
                SaldoCredito = 7000,
                Setor = 48,
                SistemaGestao = "Dynamics CRM baby",
                Site = "www.avurto.com.br",
                Situacao = 0,
                SubClassificacao = "D1DEFFFD-1E9E-E311-888D-00155D013E2E",
                SuspensaoCredito = true,
                Telefone = "343434",
                TelefoneAlternativo = "343554",
                TipoConstituicao = 993520001,
                TipoConta = 993520000,
                TipoEmbalagem = "pallet",
                TipoProprietario = "systemuser",
                TipoRelacao = 993520000,
                Transportadora = 103,
                TransportadoraRedespacho = 445,
                ValorMedioCompra = 14000,
                ValorMedioVenda = 17000,
                VendeAtacadista = true,
                ViaEmbarque = "portuária"
            };


            //string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><MENSAGEM>  <CABECALHO>    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>    <NumeroOperacao>Canal de Teste Marcelo 20140508-1716</NumeroOperacao>    <CodigoMensagem>MSG0072</CodigoMensagem>    <LoginUsuario>Tools</LoginUsuario>  </CABECALHO>  <CONTEUDO>    <MSG0072>      <CodigoConta>1c261be8-97da-e311-b278-00155d01330e</CodigoConta>      <ContaPrimaria xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <NomeRazaoSocial>Canal de Teste Marcelo 20140508-1716</NomeRazaoSocial>      <NomeFantasia>Teste MCS</NomeFantasia>      <NomeAbreviado>te Marcelo 2</NomeAbreviado>      <TipoRelacao>993520000</TipoRelacao>      <Telefone>48123654987</Telefone>      <Ramal>48</Ramal>      <TelefoneAlternativo>65466546546</TelefoneAlternativo>      <RamalTelefoneAlternativo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><Fax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <RamalFax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <Email>marcelo.souza@toolsystems.com.br</Email>      <Site>http://www.toolsystems.com.br</Site>      <Natureza>993520000</Natureza>      <CNPJ>10387848000179</CNPJ>      <InscricaoEstadual>ISENTO</InscricaoEstadual>      <InscricaoMunicipal>ISENTO</InscricaoMunicipal>      <FormaTributacao>993520004</FormaTributacao>      <CoberturaGeografica>Grande Fpolis</CoberturaGeografica>      <DataConstituicao>2015-01-01</DataConstituicao>      <DistribuicaoUnicaFonteReceita>false</DistribuicaoUnicaFonteReceita>      <DistribuidorPrincipal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <QualificadoTreinamento>Possui espaço físico e pessoal qualificado para instrução e treinamento à equipe de vendedores e cli</QualificadoTreinamento><Exclusividade>false</Exclusividade>      <Historico>Descreva o tempo de existência, histórico e principais eventos desde sua fundação:</Historico>      <IntencaoApoio>Defina os apoios que espera receber da Intelbras para cumprir seus objetivos:</IntencaoApoio>      <MetodoComercializacao>Quais são os métodos utilizados para comercializar seus produtos (venda balcão, telemarketing e/ou v</MetodoComercializacao>      <NumeroFuncionarios>1</NumeroFuncionarios><NumeroColaboradoresAreaTecnica>2</NumeroColaboradoresAreaTecnica>      <NumeroTecnicosSuporte>4</NumeroTecnicosSuporte>      <NumeroVendedores>3</NumeroVendedores>      <OutraFonteReceita xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <ParticipaProgramaCanais>993520000</ParticipaProgramaCanais>      <PossuiEstruturaCompleta>993520001</PossuiEstruturaCompleta>      <PossuiFiliais>993520000</PossuiFiliais>      <QuantidadeFiliais>1</QuantidadeFiliais>      <PrazoMedioCompra>15</PrazoMedioCompra>      <PrazoMedioVenda>15</PrazoMedioVenda>      <RamoAtividadeEconomica>Atividade Economica</RamoAtividadeEconomica>      <SistemaGestao>Qual sistema (fornecedor) a empresa utiliza como software de gestão do negócio (ERP)?</SistemaGestao>      <ValorMedioCompra>1000.00</ValorMedioCompra>      <ValorMedioVenda>750.00</ValorMedioVenda>      <Situacao>0</Situacao>      <Classificacao>fa6892a7-1e9e-e311-888d-00155d013e2e</Classificacao>      <SubClassificacao>d3defffd-1e9e-e311-888d-00155d013e2e</SubClassificacao>      <NivelPosVenda>D2A1461D-2CA6-E311-888D-00155D013E2E</NivelPosVenda><ApuracaoBeneficio>993520000</ApuracaoBeneficio>      <TipoConstituicao>993520001</TipoConstituicao>      <Proprietario>BEE55B63-9AAE-E311-9207-00155D013D19</Proprietario>      <TipoProprietario>systemuser</TipoProprietario>      <TipoConta>993520000</TipoConta>      <EnderecoPrincipal>        <CEP>88113420</CEP>        <Logradouro>RUA CRISTO REI </Logradouro>        <Numero>1</Numero>        <Complemento xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />        <Bairro>REAL PARQUE</Bairro>        <NomeCidade>SAO JOSE</NomeCidade>        <Cidade>SAO JOSE,SC,Brasil</Cidade>        <UF>SC</UF>        <Estado>Brasil,SC</Estado>        <NomePais>Brasil</NomePais>        <Pais>Brasil</Pais>      </EnderecoPrincipal>      <EnderecoCobranca>        <CEP>88113420</CEP>        <Logradouro>RUA CRISTO REI </Logradouro>        <Numero>1</Numero>        <Complemento xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />        <Bairro>REAL PARQUE</Bairro>        <NomeCidade>SAO JOSE</NomeCidade>        <Cidade>SAO JOSE,SC,Brasil</Cidade>        <UF>SC</UF>        <Estado>Brasil,SC</Estado>        <NomePais>Brasil</NomePais>        <Pais>Brasil</Pais>      </EnderecoCobranca>    </MSG0072>  </CONTEUDO></MENSAGEM>";

            //string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><MENSAGEM>  <CABECALHO>    <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>    <NumeroOperacao>Canal de Teste Marcelo 20140508-1716</NumeroOperacao>    <CodigoMensagem>MSG0072</CodigoMensagem>    <LoginUsuario>Tools</LoginUsuario>  </CABECALHO>  <CONTEUDO>    <MSG0072>     <ContaPrimaria xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <NomeRazaoSocial>Canal Testa7 Marcelo 20140508-1716</NomeRazaoSocial>      <NomeFantasia>Teste MC7</NomeFantasia>      <NomeAbreviado>te Marcel 7</NomeAbreviado>      <TipoRelacao>993520000</TipoRelacao>      <Telefone>48123654987</Telefone>      <Ramal>48</Ramal>      <TelefoneAlternativo>65466546546</TelefoneAlternativo>      <RamalTelefoneAlternativo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <Fax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <RamalFax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <Email>marcelo.souza@toolsystems.com.br</Email>      <Site>http://www.toolsystems.com.br</Site>      <Natureza>993520000</Natureza>      <CNPJ>04415447000182</CNPJ>      <InscricaoEstadual>ISENTO</InscricaoEstadual>      <InscricaoMunicipal>ISENTO</InscricaoMunicipal>      <FormaTributacao>993520004</FormaTributacao>      <CoberturaGeografica>Grande Fpolis</CoberturaGeografica>      <DataConstituicao>2015-01-01</DataConstituicao>      <DistribuicaoUnicaFonteReceita>false</DistribuicaoUnicaFonteReceita>      <DistribuidorPrincipal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <QualificadoTreinamento>Possui espaço físico e pessoal qualificado para instrução e treinamento à equipe de vendedores e cli</QualificadoTreinamento>      <Exclusividade>false</Exclusividade>      <Historico>Descreva o tempo de existência, histórico e principais eventos desde sua fundação:</Historico>      <IntencaoApoio>Defina os apoios que espera receber da Intelbras para cumprir seus objetivos:</IntencaoApoio>      <MetodoComercializacao>Quais são os métodos utilizados para comercializar seus produtos (venda balcão, telemarketing e/ou v</MetodoComercializacao>      <NumeroFuncionarios>1</NumeroFuncionarios>      <NumeroColaboradoresAreaTecnica>2</NumeroColaboradoresAreaTecnica>      <NumeroTecnicosSuporte>4</NumeroTecnicosSuporte>      <NumeroVendedores>3</NumeroVendedores>      <OutraFonteReceita xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />      <ParticipaProgramaCanais>993520000</ParticipaProgramaCanais>      <PossuiEstruturaCompleta>993520001</PossuiEstruturaCompleta>      <PossuiFiliais>993520000</PossuiFiliais>      <QuantidadeFiliais>1</QuantidadeFiliais>      <PrazoMedioCompra>15</PrazoMedioCompra>      <PrazoMedioVenda>15</PrazoMedioVenda>      <RamoAtividadeEconomica>Atividade Economica</RamoAtividadeEconomica>      <SistemaGestao>Qual sistema (fornecedor) a empresa utiliza como software de gestão do negócio (ERP)?</SistemaGestao>      <ValorMedioCompra>1000.00</ValorMedioCompra>      <ValorMedioVenda>750.00</ValorMedioVenda>      <Situacao>0</Situacao>      <Classificacao>fa6892a7-1e9e-e311-888d-00155d013e2e</Classificacao>      <SubClassificacao>d3defffd-1e9e-e311-888d-00155d013e2e</SubClassificacao>      <NivelPosVenda>D2A1461D-2CA6-E311-888D-00155D013E2E</NivelPosVenda><ApuracaoBeneficio>993520000</ApuracaoBeneficio>      <TipoConstituicao>993520001</TipoConstituicao>      <Proprietario>BEE55B63-9AAE-E311-9207-00155D013D19</Proprietario>      <TipoProprietario>systemuser</TipoProprietario>      <TipoConta>993520000</TipoConta>      <EnderecoPrincipal>       <TipoEndereco>3</TipoEndereco> <CEP>88113420</CEP>        <Logradouro>RUA CRISTO REI </Logradouro>        <Numero>1</Numero>        <Complemento xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />        <Bairro>REAL PARQUE</Bairro>        <NomeCidade>SAO JOSE</NomeCidade>        <Cidade>SAO JOSE,SC,Brasil</Cidade>        <UF>SC</UF>        <Estado>Brasil,SC</Estado>        <NomePais>Brasil</NomePais>        <Pais>Brasil</Pais>      </EnderecoPrincipal>      <EnderecoCobranca>        <TipoEndereco>1</TipoEndereco><CEP>88113420</CEP>        <Logradouro>RUA CRISTO REI </Logradouro>        <Numero>1</Numero>        <Complemento xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />        <Bairro>REAL PARQUE</Bairro>        <NomeCidade>SAO JOSE</NomeCidade>        <Cidade>SAO JOSE,SC,Brasil</Cidade>        <UF>SC</UF>        <Estado>Brasil,SC</Estado>        <NomePais>Brasil</NomePais>        <Pais>Brasil</Pais>      </EnderecoCobranca>    </MSG0072>  </CONTEUDO></MENSAGEM>";

            //string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><MENSAGEM><CABECALHO><IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor><NumeroOperacao>Canal de Teste Marcelo 2014-05-14 1519</NumeroOperacao><CodigoMensagem>MSG0072</CodigoMensagem><LoginUsuario>Tools</LoginUsuario></CABECALHO><CONTEUDO><MSG0072><ContaPrimaria xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><NomeRazaoSocial>Canal de Teste Marcelo 2014-05-14 1519</NomeRazaoSocial><NomeFantasia>Teste 2014-05-14-15192</NomeFantasia><NomeAbreviado>te Marcelo 2</NomeAbreviado><TipoRelacao>993520000</TipoRelacao><Telefone>48123654987</Telefone><Ramal>48</Ramal><TelefoneAlternativo>65466546546</TelefoneAlternativo><RamalTelefoneAlternativo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><Fax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><RamalFax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><Email>marcelo.souza@toolsystems.com.br</Email><Site>http://www.toolsystems.com.br</Site><Natureza>993520000</Natureza><CNPJ>72828685000170</CNPJ><InscricaoEstadual>ISENTO</InscricaoEstadual><InscricaoMunicipal>ISENTO</InscricaoMunicipal><FormaTributacao>993520004</FormaTributacao><CoberturaGeografica>Grande Fpolis</CoberturaGeografica><DataConstituicao>2015-01-01</DataConstituicao><DistribuicaoUnicaFonteReceita>false</DistribuicaoUnicaFonteReceita><DistribuidorPrincipal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><QualificadoTreinamento>Possui espaço físico e pessoal qualificado para instrução e treinamento à equipe de vendedores e cli</QualificadoTreinamento><Exclusividade>false</Exclusividade><Historico>Descreva o tempo de existência, histórico e principais eventos desde sua fundação:</Historico><IntencaoApoio>Defina os apoios que espera receber da Intelbras para cumprir seus objetivos:</IntencaoApoio><MetodoComercializacao>Quais são os métodos utilizados para comercializar seus produtos (venda balcão, telemarketing e/ou v</MetodoComercializacao><NumeroFuncionarios>1</NumeroFuncionarios><NumeroColaboradoresAreaTecnica>2</NumeroColaboradoresAreaTecnica><NumeroTecnicosSuporte>4</NumeroTecnicosSuporte><NumeroVendedores>3</NumeroVendedores><OutraFonteReceita xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><ParticipaProgramaCanais>993520000</ParticipaProgramaCanais><PossuiEstruturaCompleta>993520001</PossuiEstruturaCompleta><PossuiFiliais>993520000</PossuiFiliais><QuantidadeFiliais>1</QuantidadeFiliais><PrazoMedioCompra>15</PrazoMedioCompra><PrazoMedioVenda>15</PrazoMedioVenda><RamoAtividadeEconomica>Atividade Economica</RamoAtividadeEconomica><SistemaGestao>Qual sistema (fornecedor) a empresa utiliza como software de gestão do negócio (ERP)?</SistemaGestao><ValorMedioCompra>1000.00</ValorMedioCompra><ValorMedioVenda>750.00</ValorMedioVenda><Situacao>0</Situacao><Classificacao>fa6892a7-1e9e-e311-888d-00155d013e2e</Classificacao><SubClassificacao>d3defffd-1e9e-e311-888d-00155d013e2e</SubClassificacao><NivelPosVenda>D2A1461D-2CA6-E311-888D-00155D013E2E</NivelPosVenda><ApuracaoBeneficio>993520000</ApuracaoBeneficio><TipoConstituicao>993520001</TipoConstituicao><Proprietario>E53CBD6F-8E9D-E311-888D-00155D013E2E</Proprietario><TipoProprietario>systemuser</TipoProprietario><TipoConta>993520000</TipoConta><EnderecoPrincipal><TipoEndereco>3</TipoEndereco><CEP>88113420</CEP><Logradouro>RUA CRISTO REI</Logradouro><Numero>1</Numero><Complemento xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><Bairro>REAL PARQUE</Bairro><NomeCidade>SAO JOSE</NomeCidade><Cidade>SAO JOSE,SC,Brasil</Cidade><UF>SC</UF><Estado>Brasil,SC</Estado><NomePais>Brasil</NomePais><Pais>Brasil</Pais></EnderecoPrincipal><EnderecoCobranca><TipoEndereco>1</TipoEndereco><CEP>88113420</CEP><Logradouro>RUA CRISTO REI</Logradouro><Numero>1</Numero><Complemento xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" /><Bairro>REAL PARQUE</Bairro><NomeCidade>SAO JOSE</NomeCidade><Cidade>SAO JOSE,SC,Brasil</Cidade><UF>SC</UF><Estado>Brasil,SC</Estado><NomePais>Brasil</NomePais><Pais>Brasil</Pais></EnderecoCobranca></MSG0072></CONTEUDO></MENSAGEM>";

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><MENSAGEM>   <CABECALHO>      <IdentidadeEmissor>95061229-FF31-4FD1-A875-96A98D67280C</IdentidadeEmissor>      <NumeroOperacao>Revenda Teste 20140515 0721</NumeroOperacao>      <CodigoMensagem>MSG0072</CodigoMensagem>      <LoginUsuario>Tools</LoginUsuario>   </CABECALHO>   <CONTEUDO>      <MSG0072>         <ContaPrimaria xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />         <NomeRazaoSocial>Revenda Teste 20140515 0721</NomeRazaoSocial>         <NomeFantasia>Revenda 0512 0721</NomeFantasia>         <NomeAbreviado>736174070001</NomeAbreviado>         <TipoRelacao>993520000</TipoRelacao>         <Telefone>65465465465</Telefone>         <Ramal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />         <TelefoneAlternativo>98731465465</TelefoneAlternativo>         <RamalTelefoneAlternativo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />         <Fax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />         <RamalFax xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />         <Email>marcelo.souza@toolsystems.com.br</Email>         <Site>http://www.teste.com</Site>         <Natureza>993520000</Natureza>         <CNPJ>73617407000136</CNPJ>         <InscricaoEstadual>ISENTO</InscricaoEstadual>         <InscricaoMunicipal>ISENTO</InscricaoMunicipal>         <FormaTributacao>993520000</FormaTributacao>         <CoberturaGeografica>Qual a cobertura geográfica da empresa (nacional ou regional)? Especifique as regiões, estados, etc.:</CoberturaGeografica>         <DataConstituicao>2014-01-01</DataConstituicao>         <DistribuicaoUnicaFonteReceita>false</DistribuicaoUnicaFonteReceita>         <DistribuidorPrincipal xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:nil=\"true\" />         <QualificadoTreinamento>Possui espaço físico e pessoal qualificado para instrução e treinamento à equipe de vendedores e cli</QualificadoTreinamento>         <Exclusividade>false</Exclusividade>         <Historico>Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência,  desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência, histórico e principais eventos desde sua fundação: Descreva o tempo de existência,</Historico>         <IntencaoApoio>Defina os apoios que espera receber da Intelbras para cumprir seus objetivos:Defina os apoios que espera receber da Intelbras para cumprir seus objetivos:</IntencaoApoio>         <MetodoComercializacao>Quais são os métodos utilizados para comercializar seus produtos (venda balcão, telemarketing e/ou v</MetodoComercializacao>         <NumeroFuncionarios>55</NumeroFuncionarios>         <NumeroColaboradoresAreaTecnica>66</NumeroColaboradoresAreaTecnica>         <NumeroTecnicosSuporte>88</NumeroTecnicosSuporte>         <NumeroVendedores>77</NumeroVendedores>         <OutraFonteReceita>.asd asd asd asd asd asd</OutraFonteReceita>         <ParticipaProgramaCanais>993520000</ParticipaProgramaCanais>         <PossuiEstruturaCompleta>993520001</PossuiEstruturaCompleta>         <PossuiFiliais>993520000</PossuiFiliais>         <QuantidadeFiliais>2</QuantidadeFiliais>         <PrazoMedioCompra>15</PrazoMedioCompra>         <PrazoMedioVenda>10</PrazoMedioVenda>         <RamoAtividadeEconomica>Atividade Econômica:</RamoAtividadeEconomica>         <SistemaGestao>Qual sistema (fornecedor) a empresa utiliza como software de gestão do negócio (ERP)?</SistemaGestao>         <ValorMedioCompra>50000.00</ValorMedioCompra>         <ValorMedioVenda>60000.00</ValorMedioVenda>         <Situacao>0</Situacao>         <Classificacao>fa6892a7-1e9e-e311-888d-00155d013e2e</Classificacao>         <SubClassificacao>d3defffd-1e9e-e311-888d-00155d013e2e</SubClassificacao>         <NivelPosVenda>D2A1461D-2CA6-E311-888D-00155D013E2E</NivelPosVenda>         <ApuracaoBeneficio>993520000</ApuracaoBeneficio>         <TipoConstituicao>993520001</TipoConstituicao>         <Proprietario>E53CBD6F-8E9D-E311-888D-00155D013E2E</Proprietario>         <TipoProprietario>systemuser</TipoProprietario>         <TipoConta>993520000</TipoConta>         <EnderecoPrincipal>            <TipoEndereco>3</TipoEndereco>            <CEP>88085720</CEP>            <Logradouro>RUA CARLOS AUGUSTO DOMINGUES</Logradouro>            <Numero>100</Numero>            <Complemento>Complemento 123</Complemento>            <Bairro>COQUEIROS</Bairro>            <NomeCidade>FLORIANOPOLIS</NomeCidade>            <Cidade>FLORIANOPOLIS,SC,Brasil</Cidade>            <UF>SC</UF>            <Estado>Brasil,SC</Estado>            <NomePais>Brasil</NomePais>            <Pais>Brasil</Pais>         </EnderecoPrincipal>         <EnderecoCobranca>            <TipoEndereco>1</TipoEndereco>            <CEP>88085720</CEP>            <Logradouro>RUA CARLOS AUGUSTO DOMINGUES</Logradouro>            <Numero>100</Numero>            <Complemento>Complemento 123</Complemento>            <Bairro>COQUEIROS</Bairro>            <NomeCidade>FLORIANOPOLIS</NomeCidade>            <Cidade>FLORIANOPOLIS,SC,Brasil</Cidade>            <UF>SC</UF>            <Estado>Brasil,SC</Estado>            <NomePais>Brasil</NomePais>            <Pais>Brasil</Pais>         </EnderecoCobranca>      </MSG0072>   </CONTEUDO></MENSAGEM>";   
            Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(OrganizationName, IsOffline);
            String msgSaida = String.Empty;

            integ.Postar(usuario,senha,xml, out msgSaida);

        }
    }
}
