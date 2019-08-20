using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("systemuser")]
    public class Usuario : DomainBase
    {
        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public Usuario() { }

        public Usuario(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Usuario(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("systemuserid")]
        public Guid? ID { get; set; }

      //  [LogicalAttribute("accessmode")]
      //  public Int32? ModoAcesso { get; set; }

        [LogicalAttribute("address1_city")]
        public String Cidade { get; set; }

        [LogicalAttribute("address1_composite")]
        public String Endereco { get; set; }

        [LogicalAttribute("address1_country")]
        public String Pais { get; set; }

        [LogicalAttribute("address1_fax")]
        public String Endereco1 { get; set; }

        [LogicalAttribute("address1_line1")]
        public String Rua1 { get; set; }

        [LogicalAttribute("address1_line2")]
        public String Rua2 { get; set; }

        [LogicalAttribute("address1_line3")]
        public String Rua3 { get; set; }

        [LogicalAttribute("address1_postalcode")]
        public String CEP { get; set; }

        [LogicalAttribute("address1_stateorprovince")]
        public String Estado { get; set; }

        [LogicalAttribute("address1_telephone1")]
        public String Telefone { get; set; }

        [LogicalAttribute("address1_telephone2")]
        public String Telefone2 { get; set; }

        [LogicalAttribute("address1_telephone3")]
        public String Telefone3 { get; set; }

        [LogicalAttribute("address2_city")]
        public String Endereco2Cidade { get; set; }

        [LogicalAttribute("address2_composite")]
        public String Endereco2Endereco { get; set; }

        [LogicalAttribute("address2_country")]
        public String Endereco2Pais { get; set; }

        [LogicalAttribute("address2_line1")]
        public String Endereco2Rua1 { get; set; }

        [LogicalAttribute("address2_line2")]
        public String Endereco2Rua2 { get; set; }

        [LogicalAttribute("address2_line3")]
        public String Endereco2Rua3 { get; set; }

        [LogicalAttribute("address2_postalcode")]
        public String Endereco2CEP { get; set; }

        [LogicalAttribute("address2_stateorprovince")]
        public String Endereco2Estado { get; set; }

        [LogicalAttribute("businessunitid")]
        public Lookup UnidadeNegocios { get; set; }

        [LogicalAttribute("caltype")]
        public Int32? TipoLicenca { get; set; }

        [LogicalAttribute("domainname")]
        public String NomeUsuario { get; set; }

        [LogicalAttribute("firstname")]
        public String Nome { get; set; }

        [LogicalAttribute("fullname")]
        public String NomeCompleto { get; set; }

        [LogicalAttribute("homephone")]
        public String TelefoneResidencial { get; set; }

        [LogicalAttribute("internalemailaddress")]
        public String EmailPrimario { get; set; }
        
        [LogicalAttribute("invitestatuscode")]
        public Int32? StatusConvite { get; set; }

        [LogicalAttribute("isdisabled")]
        public Boolean IsDisabled { get; set; }

        [LogicalAttribute("itbc_codigodoassistcoml")]
        public Int32? CodigoAssistenteComercial { get; set; }

        [LogicalAttribute("itbc_departamento")]
        public Int32? Departamento { get; set; }

        [LogicalAttribute("lastname")]
        public String Sobrenome { get; set; }

        [LogicalAttribute("mobilealertemail")]
        public String EmailAlertaMovel { get; set; }

        [LogicalAttribute("mobilephone")]
        public String TelefoneCelular { get; set; }

        [LogicalAttribute("parentsystemuserid")]
        public Lookup Gerente { get; set; }

        [LogicalAttribute("personalemailaddress")]
        public String Email2 { get; set; }

        [LogicalAttribute("preferredaddresscode")]
        public Int32? EnderecoPreferencial { get; set; }

        [LogicalAttribute("preferredphonecode")]
        public Int32? TelefonePreferencial { get; set; }

        [LogicalAttribute("queueid")]
        public Lookup FilaPadrao { get; set; }

        [LogicalAttribute("territoryid")]
        public Lookup Regiao { get; set; }

        [LogicalAttribute("title")]
        public String Titulo { get; set; }

        [LogicalAttribute("windowsliveid")]
        public String WindowsLiveId { get; set; }

        [LogicalAttribute("itbc_codigo_supervisor")]
        public String CodigoSupervisorEMS { get; set; }

        [LogicalAttribute("itbc_descricao_departamento")]
        public String DescricaoDepartamento { get; set; }

        #endregion
    }
}
