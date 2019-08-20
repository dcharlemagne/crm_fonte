using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    public enum StatusDoPedido
    {
        Vazio = 0,
        Aberto = 1,
        Pendente = 2,
        Em_Andamento = 3,
        Cancelado1 = 4,
        Concluído = 100001,
        Parcial = 100002,
        Atendido_Parcial1 = 100003,
        Atendido_Total1 = 200000,
        Faturamento_Balcão1 = 200001,
        Suspenso1 = 200002,
        Proposta = 200003,
        Cancelado = 200004,
        Suspenso = 200005,
        Atendido_Parcial = 200006,
        Atendido_Total = 200007,
        Faturamento_Balcão = 200008
    }
}