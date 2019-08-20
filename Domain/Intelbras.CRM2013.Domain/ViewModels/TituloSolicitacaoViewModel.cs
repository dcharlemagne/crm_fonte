using System;
using System.ComponentModel;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class TituloSolicitacaoViewModel
    {
        [DisplayName("Drink or notaa")]
        [Description("Drink or not")]
        public string CNPJEstabelecimento { get; set; }
        public int? CodigoCliente { get; set; }
        public string CodigoConta { get; set; }
        public int? CodigoEstabelecimento { get; set; }
        public DateTime? DataVencimento { get; set; }
        public string NomeConta { get; set; }
        public string NumeroParcela { get; set; }
        public string NumeroSerie { get; set; }
        public string NumeroTitulo { get; set; }
        public decimal? SaldoTitulo { get; set; }
        public decimal? ValorAbatido { get; set; }
        public decimal? ValorOriginal { get; set; }
    }
}