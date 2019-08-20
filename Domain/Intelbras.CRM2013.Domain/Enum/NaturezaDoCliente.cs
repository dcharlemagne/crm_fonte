using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum NaturezaDoCliente
    {
        Vazio = 0,
        PessoaFisica = 1,
        PessoaJuridica = 2,
        Estrangeiro = 3,
        Trading = 4
    }
}
