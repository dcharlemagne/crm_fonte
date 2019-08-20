using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum TipoDeETicket
    {
        Vazio=0,
        Postagem = 1,
        Coleta = 2,
        ConsultaAvulsa = 3
    }
}
