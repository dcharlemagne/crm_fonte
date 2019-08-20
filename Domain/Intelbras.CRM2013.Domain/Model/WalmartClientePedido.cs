using System;
using System.Xml.Serialization;


namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [XmlRoot(ElementName = "Cliente")]
    public class WalmartClientePedido
    {      
        public string Email { get; set; }

        public string Cpf { get; set; }

        public string NomeCompleto { get; set; }
        
        public string TelefoneResidencial { get; set; }

        public string TelefoneCelular { get; set; }

        public string DataNascimento { get; set; }

        public string Genero { get; set; }
    }
}
