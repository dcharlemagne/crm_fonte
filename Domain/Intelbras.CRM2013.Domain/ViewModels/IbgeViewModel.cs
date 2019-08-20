using System;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class IbgeViewModel
    {
        public int CodigoIbge { get; set; }
        
        public string CidadeNome { get; set; }
        public string EstadoNome { get; set; }
        public string PaisNome { get; set; }
        public string EstadoUF{ get; set; }

        public Guid PaisId { get; set; }
        public Guid EstadoId { get; set; }
        public Guid CidadeId { get; set; }
    }
}