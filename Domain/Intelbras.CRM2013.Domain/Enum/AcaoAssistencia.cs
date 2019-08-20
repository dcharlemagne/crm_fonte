using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum AcaoAssistencia
    {
        Vazio = 0,
        Planejamento_de_estoque = 1,
        Assegurar_qualidade_de_estoque = 2,
        Esclarecimento_e_advertencia = 3,
        Orientacao_tecnica = 4
    }
}
