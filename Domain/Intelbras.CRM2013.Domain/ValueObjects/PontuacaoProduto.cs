using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    public class PontuacaoProduto
    {
        public Guid FamiliaComercialId { get; set; }
        public Guid ProdutoId { get; set; }
        public string FamiliaComercialNome { get; set; }
        public string CodigoDoProduto { get; set; }
        public string NomeDoProduto { get; set; }
        public int PontosDoProduto { get; set; }
    }
}