using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum TipoDeVigencia
    {
        Vazio = -1,
        Contrato = 1,
        Cliente = 2,
        Endereco = 200000,
        Produto = 200001,
        Por_Veiculo_Instalacao = 993520000,
        Por_veiculo_Contrato = 993520001,
    }
}
