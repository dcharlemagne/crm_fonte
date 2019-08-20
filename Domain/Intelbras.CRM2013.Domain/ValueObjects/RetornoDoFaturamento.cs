using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    public struct ItemDaNota
    {
        public string Codigo { get; set; }
        public string ItemSubstituido { get; set; }
        public Guid DiagnosticoId { get; set; }
        public int QuantidadeFaturada { get; set; }
        public int QuantidadeSubstituida { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal IPIaliquota { get; set; }
        public decimal IPIvalor { get; set; }
        public decimal ICMSitem { get; set; }
        public decimal ICMSbase { get; set; }
    }

    public class RetornoDoFaturamento
    {
        public string NotaFiscal { get; set; }
        public string CNPJ { get; set; }
        public string Estabelecimento { get; set; }
        public string Serie { get; set; }
        public DateTime DataEmissao { get; set; }

        public List<ItemDaNota> ItensDaNota { get; set; }
    }
}