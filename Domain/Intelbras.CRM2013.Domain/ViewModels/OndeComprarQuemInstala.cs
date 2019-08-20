using System;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    [Serializable]
    public class OndeComprarQuemInstala
    {
        public Guid? ID { get; set; } // accountid
        public String Cnpj { get; set; } // new_cnpj
        public String Cpf { get; set; } // new_cpf
        public String RazaoSocial { get; set; } // name
        public String NomeFantasia { get; set; } // new_nome_fantasia
        public String Cep { get; set; } // address1_postalcode
        public String Estado { get; set; } // address1_stateorprovince
        public String Cidade { get; set; } // address1_city
        public String Bairro { get; set; } // address1_line3
        public String Endereco { get; set; } // address1_line1
        public String NumeroEndereco { get; set; } // new_numero_endereco_principal
        public String ComplementoEndereco { get; set; } // address1_line2
        public String TelefoneComercial { get; set; } // telephone1
        public String TelefoneCelular { get; set; } // telephone2
        public String Email { get; set; } // emailaddress1
        public bool? MercadoAtuacaoRedes { get; set; } // new_mercado_atuacao_redes
        public bool? MercadoAtuacaoSeguranca { get; set; } // new_mercado_atuacao_seguranca
        public bool? MercadoAtuacaoTelecom { get; set; } // new_mercado_atuacao_telecom
        public int? StatusIntegracaoRevendaSite { get; set; } // new_integracao_revenda_site
        
        // Contato Primário
        public String NomeResponsavel { get; set; } // primarycontactid.firstname
        public String CpfResponsavel { get; set; } // primarycontactid.new_cpf
        
        // Conta Principal
        public String CnpjMatriz { get; set; } // parentaccountid.new_cnpj
    }
}