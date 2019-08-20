using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum FormaDeTributacao
    {
        Vazio = 0,
        LucroReal = 1,
        LucroPresumido = 2,
        Simples = 3,
        Nenhum = 4,
        Isento = 5
    }
}
