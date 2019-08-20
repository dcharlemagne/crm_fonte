using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class ProdutoFaturaViewModel
    {
        public string CodigoProduto { get; set; }
        public string  DescProduto{ get; set; }
        public string CodigoFaturaEMS { get; set; }
        public DateTime DataEmissaoFatura { get; set; }
        public decimal PrecoFatura { get; set; }
        public decimal QtdFatura { get; set; }
        public decimal QtdCalculo { get; set; }
        public decimal? PrecoCalculoAtual { get; set; }
        public decimal? SaldoDiferenca { get; set; }
        public decimal? TotalDiferenca { get; set; }
        public decimal? ValorIPIProduto { get; set; }
        public decimal? TotalComIPI { get; set; }
    }
}
