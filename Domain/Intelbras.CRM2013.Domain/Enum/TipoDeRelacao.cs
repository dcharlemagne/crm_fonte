using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    public enum TipoDeRelacao
    {
        Vazio = 0,
        ClienteFinal          = 200000,
        Revenda               = 200002,
        TecnicoAutorizado     =      1,
        TecnicoAutonomo       = 200001,
        TecnicoDoLai          = 200003,
        TecnicoDoDistribuidor = 200004,
        TecnicoCorporativo    = 200005,
        RevendaDeTecnologia   = 200006
    }
}
