using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum TipoDeModalidade
    {
        Vazio = 0,
        CobrancaSimples = 1,
        Desconto = 2,
        Caucao = 3,
        Judicial = 4,
        Repres = 5,
        Carteira = 6,
        Vendor = 7,
        Cheque = 8,
        NotaPromissoria = 9,
        NaoInformado = 99

    }
}
