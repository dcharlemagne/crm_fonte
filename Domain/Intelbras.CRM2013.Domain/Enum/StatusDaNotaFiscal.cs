using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    public enum StatusDaNotaFiscal
    {
        Calculada = 1,
        Impressa = 2,
        CanceladaObsoleta = 3,
        Cobrado = 4,
        AtualCR = 5,
        AtualOF = 6,
        Quitada = 7,
        Concluida = 100001,
        Parcial = 100002,
        Cancelada = 100003,
        AtualStat = 200000,
        Nenhum = 0

    }
}
