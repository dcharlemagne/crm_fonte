using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.ValueObjects
{
    public class ClienteDoContrato
    {
        public Guid ClienteParticipanteEnderecoId { get; set; }
        public Guid ClienteId { get; set; }
        public Guid ContratoId { get; set; }
        public Guid ContatoId { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string NomeFantasia { get; set; }

        // Endereço
        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
    }
}
