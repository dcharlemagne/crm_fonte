using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    /// <summary>
    /// Indica se o arquivo foi processado corretamente
    /// 01 – Processado corretamente,
    /// 00 – Ocorreu algum tipo de erro durante o processamento.
    /// </summary>
    public enum StatusDoProcessamentoLogisticaReversa
    {
        OcorreuErro = 0,
        ProcessadoCorretamente = 1
    }
}
