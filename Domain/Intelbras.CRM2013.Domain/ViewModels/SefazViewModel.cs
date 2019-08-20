using System;
using System.ComponentModel;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class SefazViewModel
    {
        public string CNPJ { get; set; }
        public string CPF { get; set; }
        public string InscricaoEstadual { get; set; }
        public string UF { get; set; }
        public int? CNAE { get; set; }
        public DateTime? DataBaixa { get; set; }
        public DateTime? DataInicioAtividade { get; set; }
        public DateTime? DataModificacaoSituacao { get; set; }
        public SefazEnderecoContribuinteViewModel EnderecoContribuinte { get; set; }
        public string InscricaoEstadualAtual { get; set; }
        public string InscricaoEstadualUnica { get; set; }
        public string Nome { get; set; }
        public string NomeFantasia { get; set; }
        public string RegimeApuracao { get; set; }
        public int? ContribuinteIcms { get; set; }
        public int? SituacaoCredenciamentoCTE { get; set; }
        public int? SituacaoCredenciamentoNFE { get; set; }
    }

    public class SefazEnderecoContribuinteViewModel
    {
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public int? CodigoIBGE { get; set; }
        public string Complemento { get; set; }
        public string Logradouro { get; set; }
        public string NomeCidade { get; set; }
        public string Numero { get; set; }
    }

    public class SefazErroViewModel
    {
        public SefazErroViewModel(int codeErro)
        {
            StatusSefaz = (StatusSefazViewModel)codeErro;
        }

        public string MessageError
        {
            get
            {
                DescriptionAttribute desc = StatusSefaz.GetAttributeOfType<DescriptionAttribute>();
                return (desc != null) ? desc.Description : string.Empty;
            }
        }

        public StatusSefazViewModel StatusSefaz { get; private set; }

        public Domain.Enum.Conta.StatusIntegracaoSefaz StatusIntegracaoSefaz
        {
            get
            {
                switch (StatusSefaz)
                {
                    case StatusSefazViewModel.Sucesso:
                    case StatusSefazViewModel.SucessoIntegracao:
                        return Domain.Enum.Conta.StatusIntegracaoSefaz.Validado;

                    case StatusSefazViewModel.CnpjInvalido:
                        return Domain.Enum.Conta.StatusIntegracaoSefaz.CnpjNaoEncontrado;

                    default:
                        return Domain.Enum.Conta.StatusIntegracaoSefaz.IntegracaoIndisponivel;
                }
            }
        }
    }

    public enum StatusSefazViewModel
    {
        [Description("Dados fiscais validados com sucesso.")]
        Sucesso = 1,

        [Description("Não foi possível validar os dados fiscais. Serviço de integração indisponível temporariamente.")]
        ServicoIndisponivel = 108,

        [Description("Não foi possível validar os dados fiscais. Serviço de integração indisponível temporariamente.")]
        ServicoParalizado = 109,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        UsoDenegado = 110,

        [Description("Dados fiscais validados com sucesso.")]
        SucessoIntegracao = 111,

        [Description("Não foi possível validar os dados fiscais. CNPJ consultado com mais de uma ocorrência.")]
        Code112 = 112,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CnpjBaseCertificadoDigital = 213,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        ExcedeuTamanhoLimite = 214,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        FalhaSchemaXml = 215,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        ChaveAcessoDiferente = 216,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        VersaoArquivoXmlDiferente = 238,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        VersaoArquivoXmlNaoSuportada = 239,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        FalhaCabecalhoSchemaXml = 242,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        XmlMalFormatado = 243,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code244 = 244,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code247 = 247,

        [Description("O UF/Município destinatário não pertence a SUFRAMA cadastrado na Receita Federal.")]
        Code251 = 251,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code252 = 252,

        [Description("Cnpj informado é inválido.")]
        CnpjInvalido = 258,

        [Description("Cnpj informado não é contribuinte da UF.")]
        CnpjNaoContribuinteUf = 259,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code260 = 260,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code261 = 261,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code262 = 262,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code263 = 263,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code264 = 264,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code265 = 265,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoTransmissorInvalido = 280,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoTransmissorDataInvalida = 281,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoTransmissorSemCnpj = 282,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoTransmissorCadeiaCertificacao = 283,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoTransmissorRevogado = 284,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoTransmissorDiferenteIcpBrasil = 285,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoTransmissorErroAcessoLcr = 286,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CodigoUfDiferenteSolicitada = 289,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoAssinaturaInvalida = 290,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoAssinaturaDataValidade = 291,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoAssinaturaSemCnpj = 292,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoAssinaturaSemCnpjCadeida = 293,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        CertificadoAssinaturaRevogada = 294,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code295 = 295,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code296 = 296,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code297 = 297,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code298 = 298,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code299 = 299,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code402 = 402,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code404 = 404,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code409 = 409,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code410 = 410,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code411 = 411,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code516 = 516,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code522 = 522,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code545 = 545,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        Code587 = 587,

        [Description("A consulta de dados fiscais para esta UF ainda não está disponível pelo sistema. Por favor preencha os dados manualmente.")]
        ConsultaParaUfIndisponivel = 888,

        [Description("Não foi possível validar os dados fiscais. Preencha os dados manualmente.")]
        SemResultado = 999
    }
}