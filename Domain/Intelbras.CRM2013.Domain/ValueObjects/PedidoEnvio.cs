using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    public class PedidoEnvio
    {
        public string Cliente { get; set; }
        public Transportadora Transportadora { get; set; }
        public int Estabelecimento { get; set; }
        public string Atendente { get; set; }
        public string CanalDeVendas { get; set; }

        public List<Diagnostico> Diagnosticos { get; set; }
    }
}