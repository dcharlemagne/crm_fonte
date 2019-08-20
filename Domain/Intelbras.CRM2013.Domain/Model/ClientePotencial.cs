using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;
using System.IO;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [LogicalEntity("lead")]
    public class ClientePotencial : IntegracaoBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public ClientePotencial() { }

        public ClientePotencial(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ClientePotencial(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos
        [LogicalAttribute("leadid")]
        public Guid? ID { get; set; }

        private string descricao = string.Empty;
        [LogicalAttribute("description")]
        public string Descricao
        {
            get { return descricao; }
            set { descricao = value; }
        }

        [LogicalAttribute("estimatedamount")]
        public Decimal? ValorEstimado { get; set; }

        private Stream anexo = null;
        public Stream Anexo
        {
            get { return anexo; }
            set { anexo = value; }
        }

        private string nomeDoAnexo = string.Empty;
        public string NomeDoAnexo
        {
            get { return nomeDoAnexo; }
            set { nomeDoAnexo = value; }
        }

        private DateTime dataEstimada = DateTime.Now;
        [LogicalAttribute("estimatedclosedate")]
        public DateTime DataEstimada
        {
            get { return dataEstimada; }
            set { dataEstimada = value; }
        }

        [LogicalAttribute("itbc_proximainteracaoem")]
        public DateTime? DataProximaInteracao { get; set; }
        
        private string cnpj = string.Empty;
        [LogicalAttribute("itbc_cpfoucnpj")]
        public string Cnpj
        {
            get { return cnpj; }
            set { cnpj = value; }
        }

        private string email = string.Empty;
        [LogicalAttribute("emailaddress1")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private string telefoneComercial = string.Empty;
        [LogicalAttribute("telephone1")]
        public string TelefoneComercial
        {
            get { return telefoneComercial; }
            set { telefoneComercial = value; }
        }

        private string telefoneCelular = string.Empty;
        [LogicalAttribute("mobilephone")]
        public string TelefoneCelular
        {
            get { return telefoneCelular; }
            set { telefoneCelular = value; }
        }

        private string primeiroNomeDoContato = string.Empty;
        [LogicalAttribute("firstname")]
        public string PrimeiroNomeDoContato
        {
            get { return primeiroNomeDoContato; }
            set { primeiroNomeDoContato = value; }
        }

        private string sobreNomeDoContato = string.Empty;
        [LogicalAttribute("lastname")]
        public string SobreNomeDoContato
        {
            get { return sobreNomeDoContato; }
            set { sobreNomeDoContato = value; }
        }

        private string nomeCompletoDoContato = string.Empty;
        [LogicalAttribute("fullname")]
        public string NomeCompletoDoContato
        {
            get { return nomeCompletoDoContato; }
            set { nomeCompletoDoContato = value; }
        }

        private string nomeDaEmpresa = string.Empty;
        [LogicalAttribute("companyname")]
        public string NomeDaEmpresa
        {
            get { return nomeDaEmpresa; }
            set { nomeDaEmpresa = value; }
        }

        private string topico = string.Empty;
        [LogicalAttribute("subject")]
        public string Topico
        {
            get { return topico; }
            set { topico = value; }
        }

        private string numeroprojeto = string.Empty;
        [LogicalAttribute("itbc_numeroprojeto")]
        public string NumeroProjeto
        {
            get { return numeroprojeto; }
            set { numeroprojeto = value; }
        }

        [LogicalAttribute("itbc_revendaintegrid")]
        public Lookup RevendaIntegrador { get; set; }

        [LogicalAttribute("itbc_distribuidorid")]
        public Lookup Distribuidor { get; set; }

        [LogicalAttribute("itbc_businessunit")]
        public Lookup UnidadeNegocio { get; set; }

        [LogicalAttribute("itbc_keyaccountreprdistrid")]
        public Lookup Executivo { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_tipodelead")]
        public Int32? TipoProjeto { get; set; }

        [LogicalAttribute("itbc_envolverengenharia")]
        public Boolean EnvolverEngenharia { get; set; }

        [LogicalAttribute("itbc_enviarcotacao")]
        public Int32? CotacaoEnviada { get; set; }

        private DateTime dataCriacao = DateTime.Now;
        [LogicalAttribute("createdon")]
        public DateTime DataCriacao
        {
            get { return dataCriacao; }
            set { dataCriacao = value; }
        }

        [LogicalAttribute("address1_postalcode")]
        public String Endereco1CEP { get; set; }

        [LogicalAttribute("itbc_address1_street")]
        public String Endereco1Rua { get; set; }

        [LogicalAttribute("itbc_address1_number")]
        public String Endereco1Numero { get; set; }

        [LogicalAttribute("address1_line3")]
        public String Endereco1Complemento { get; set; }

        [LogicalAttribute("address1_line2")]
        public String Endereco1Bairro { get; set; }

        [LogicalAttribute("itbc_address1_city")]
        public Lookup Endereco1Municipioid { get; set; }
        public String Endereco1Municipio { get; set; }

        [LogicalAttribute("itbc_address1_stateorprovince")]
        public Lookup Endereco1Estadoid { get; set; }

        [LogicalAttribute("itbc_address1_country")]
        public Lookup Endereco1Pais { get; set; }

        [LogicalAttribute("stageid")]
        public Guid? StageId { get; set; }

        [LogicalAttribute("itbc_statusdavalidacao")]
        public Int32? StatusValidacao { get; set; }

        [LogicalAttribute("itbc_tipo_solucao")]
        public Int32? TipoSolucao { get; set; }

        [LogicalAttribute("itbc_necessita_atencao")]
        public Boolean? NecessitaAtencao { get; set; }

        [LogicalAttribute("itbc_possivel_duplicidade")]
        public Boolean? PossivelDuplicidade { get; set; }

        #endregion

        #region Métodos

        //protected override void Validar()
        //{

        //    var docService = new DocService<CNPJ>(this.Cnpj);
        //    if (!docService.FormatoEValido())
        //    {
        //        this.AdicionarRegraViolada(MensagensDeValidacaoPadrao.CnpjInvalido);
        //    }
        //    else if (this.DataEstimada <= DateTime.Today)
        //    {
        //        this.AdicionarRegraViolada(
        //           LeadMensagensParaRegrasVioladas.MensagensEspecificas.DataEstimadaInvalida);
        //    }
        //    else if ((this.Email != "" && this.Email != null) &&
        //             (!this.Email.Contains("@") || !this.Email.Contains(".")))
        //    {
        //        this.AdicionarRegraViolada(MensagensDeValidacaoPadrao.EmailInvalido);
        //    }
        //}

        //protected override Services.Validacoes.ServicoDeMensagensParaRegrasVioladas CarregarRegrasQueSeraoValidadas()
        //{
        //    return new LeadMensagensParaRegrasVioladas();
        //}

        public void EnviaEmailRegistroProjeto(bool duplicado)
        {
            (new CRM2013.Domain.Servicos.RepositoryService(OrganizationName, IsOffline)).ClientePotencial.EnviaEmailRegistroProjeto(this, duplicado);
        }
        #endregion
    }
}