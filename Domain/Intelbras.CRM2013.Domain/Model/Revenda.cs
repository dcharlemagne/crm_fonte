using System;

namespace Intelbras.CRM2013.Domain.Model
{
    public class Revenda
    {
        public int? Revendaid { get; set; }
        public Guid Idrevendacrm { get; set; }
        public int? Idrevendaerp { get; set; }
        public string CpfCnpj { get; set; } 
        public int? Statuscode { get; set; }
        public int? Statecode { get; set; }
        public DateTime? CriadoEm { get; set; }
        public string TipoPessoaCliente { get; set; }
        public string CepCliente { get; set; }
        public string EstadoCliente { get; set; }
        public string CidadeCliente { get; set; }
        public string BairroCliente { get; set; }
        public string NumeroEnderecoCliente { get; set; }
        public string EnderecoCliente { get; set; }
        public string TelefoneCliente { get; set; }
        public string InscricaoEstadual { get; set; }
        public string RazaoSocial { get; set; }
    }
}