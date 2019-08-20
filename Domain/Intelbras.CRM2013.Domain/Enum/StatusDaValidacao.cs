using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    public enum StatusDaValidacao
    {
        Nao_Validado = 993520000,
        Aceito = 993520001,
        Nao_Aceito_Premissas = 993520002,
        Nao_Aceito_Ja_Existe = 993520003,
        Dados_Incompletos = 993520004
    }
}