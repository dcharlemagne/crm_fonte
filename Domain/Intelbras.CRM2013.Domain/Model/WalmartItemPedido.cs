using System;
using System.Xml.Serialization;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    [XmlRoot(ElementName = "ItemPedido")]
    public class WalmartItemPedido
    {
        public int IdSku { get; set; }

        public string Nome { get; set; }

        public int Quantidade { get; set; }
        
        public decimal QtdPontos { get; set; }

        public decimal PrecoVenda { get; set; }
        
        public string Status { get; set; }

        public string CodigoStatus { get; set; }

        public string PrazoEntrega { get; set; }

        public string EmbalarPresente { get; set; }

        public decimal? ValorEmbalagemPresente { get; set; }
    }
}
