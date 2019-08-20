using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("customeraddress")]
    public class Endereco : DomainBase
    {
        #region Atributos

        private TipoDeEndereco tipo;
        private string codigoIncrementalEMS = "";
        private Guid codigoDaLocalidade;
        private Boolean zonaFranca = false;
        private int codigoIBGE = int.MinValue;
        private Conta parentid = null;
        private string accountNumber = "";

        [LogicalAttribute("name")]
        public String Nome { get; set; }
        public Boolean ZonaFranca
        {
            get { return zonaFranca; }
            set { zonaFranca = value; }
        }

        public string AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }

        public Conta ParentId
        {
            get { return parentid; }
            set { parentid = value; }
        }

        public bool Padrao
        {
            get { return (this.Nome == "Padrão"); }
        }

        public string Localidade { get; set; }

        public Guid CodigoDaLocalidade
        {
            get { return codigoDaLocalidade; }
            set { codigoDaLocalidade = value; }
        }

        public string CodigoIncrementalEMS
        {
            get { return codigoIncrementalEMS; }
            set { codigoIncrementalEMS = value; }
        }

        [LogicalAttribute("country")]
        public string Pais { get; set; }

        public TipoDeEndereco Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        [LogicalAttribute("line1")]
        public string Logradouro { get; set; }

        [LogicalAttribute("city")]
        public string Cidade { get; set; }

        [LogicalAttribute("stateorprovince")]
        public string Uf { get; set; }

        public int CodigoIBGE
        {
            get { return codigoIBGE; }
            set { codigoIBGE = value; }
        }

        #endregion

        #region Contrutores
        private RepositoryService RepositoryService { get; set; }

        public Endereco()
        {

        }

        public Endereco(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Endereco(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion
        [LogicalAttribute("customeraddressid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("parentid")]
        public Lookup Conta { get; set; }

        [LogicalAttribute("new_cnpj")]
        public String Identificacao { get; set; }

        [LogicalAttribute("name")]
        public String CodigoEndereco { get; set; }

        [LogicalAttribute("new_codigo_taxa")]
        public int? CodigoTaxa { get; set; }

        [LogicalAttribute("new_codigo_tipo_entrega")]
        public int? CodigoTipoEntrega { get; set; }

        [LogicalAttribute("new_email")]
        public String Email { get; set; }

        [LogicalAttribute("new_incricao_estadual")]
        public String InscricaoEstadual { get; set; }

        [LogicalAttribute("new_observacao")]
        public String Observacao { get; set; }
        
        [LogicalAttribute("addresstypecode")]
        public int? TipoEndereco { get; set; }

        [LogicalAttribute("addressnumber")]
        public int? AddressNumber { get; set; }

        [LogicalAttribute("line1")]
        public String EnderecoNumero { get; set; }

        [LogicalAttribute("new_numero_endereco")]
        public String Numero { get; set; }

        [LogicalAttribute("postofficebox")]
        public String CaixaPostal { get; set; }

        [LogicalAttribute("postalcode")]
        public String Cep { get; set; }

        [LogicalAttribute("line3")]
        public String Complemento { get; set; }

        [LogicalAttribute("line2")]
        public String Bairro { get; set; }

        [LogicalAttribute("city")]
        public String NomeCidade { get; set; }

        /* Ficou decidido que não seria armazenada a chave de integração da CIDADE por enquanto, a lógica de envio fica na MSG */

        [LogicalAttribute("stateorprovince")]
        public String SiglaEstado { get; set; }

        /* Ficou decidido que não seria armazenada a chave de integração do ESTADO por enquanto, a lógica de envio fica na MSG */

        [LogicalAttribute("country")]
        public String NomePais { get; set; }

        /* Ficou decidido que não seria armazenada a chave de integração do PAÍS por enquanto, a lógica de envio fica na MSG */

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_status")]
        public Boolean StatusAtivo { get; set; }
    }
}
