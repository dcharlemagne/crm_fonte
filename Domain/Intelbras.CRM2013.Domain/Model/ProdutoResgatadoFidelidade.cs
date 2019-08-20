using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_produto_resgatado_fidelidade")]
    public class ProdutoResgatadoFidelidade : DomainBase
    {
        public ProdutoResgatadoFidelidade() {}

        public ProdutoResgatadoFidelidade(string organization, bool isOffline)
            : base(organization, isOffline)
        { }


        [LogicalAttribute("new_produto_resgatado_fidelidadeid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_name")]
        public string Nome { get; set; }

        [LogicalAttribute("new_codigo_produto")]
        public string CodigoProduto { get; set; }

        [LogicalAttribute("new_quantidade")]
        public int? Quantidade { get; set; }

        [LogicalAttribute("new_pontos")]
        public decimal? Pontos { get; set; }

        [LogicalAttribute("new_valor_unitario")]
        public decimal? ValorUnitario { get; set; }

        [LogicalAttribute("new_resgate_premiosid")]
        public Lookup Resgate { get; set; }
    }
}
