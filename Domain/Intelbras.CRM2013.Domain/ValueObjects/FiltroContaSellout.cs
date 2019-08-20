using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    public class FiltroContaSellout
    {
        public string Classificacao { get; set; }
        public List<string> SubClassificacao { get; set; }
        public List<string> Categorias { get; set; }
    }
}
