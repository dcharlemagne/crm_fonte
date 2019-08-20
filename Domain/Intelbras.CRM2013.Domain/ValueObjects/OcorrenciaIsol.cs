using System;
using System.Collections.Generic;
using System.Text;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    [Serializable]
    public class OcorrenciaIsol
    {
        // Contrato
        // Lista<int> Status 
        // DataInicioFechamento
        // DataFimFechamento
        public Guid OcorrenciaId { get; set; }
        public string Descricao { get; set; }
        public string Numero { get; set; }
        public string DataAbertura { get; set; }
        public string DataFechamento { get; set; }
        public string OsCliente { get; set; }
        public int Status { get; set; }

        public Guid ClienteId { get; set; }
        public string ClienteNome { get; set; }
        public string NomeFantasia { get; set; }

        public Guid ContratoId { get; set; }
        public string ContratoNome { get; set; }
    }
}
