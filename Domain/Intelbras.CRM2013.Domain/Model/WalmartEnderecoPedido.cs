using System;
using System.Xml.Serialization;


namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [XmlRoot(ElementName = "EnderecoEntrega")]
    public class WalmartEnderecoPedido
    {
        public string NomeDestinatario { get; set; }

        public string Cep { get; set; }

        public string Estado { get; set; }

        public string Cidade { get; set; }

        public string Bairro { get; set; }

        public string TipoLogradouro { get; set; }

        public string Logradouro { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public string PontoReferencia { get; set; }
    }
}
