using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    [Serializable]
    public class ResgateDoParticipante
    {
        public Guid ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public int Quantidade { get; set; }
        public int PontosUnitario { get; set; }
        public int PontosTotal { get; set; }
        //public Guid DistribuidorId { get; set; }
        //public string DistribuidorNome { get; set; }
    }
}
