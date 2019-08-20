using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum TipoDoCliente
    {
        Vazio = 0,
        Ciente = 1,
        Fornecedor = 2,
        Ambos = 3
    }
}
