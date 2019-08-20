using System;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class ModeloMetaProdutoMesViewModel
    {
        public Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes Mes { get; set; }

        private decimal? _valor;
        public decimal? Valor
        {
            get { return _valor; }
            set
            {
                if(value < 0)
                {
                    throw new ArgumentException("(CRM) O valor não pode ser negativo.");
                }

                _valor = value;
            }
        }

        private decimal? _quantidade;
        public decimal? Quantidade
        {
            get { return _quantidade; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("(CRM) A quantidade não pode ser negativa.");
                }

                _quantidade = value;
            }
        }
    }
}