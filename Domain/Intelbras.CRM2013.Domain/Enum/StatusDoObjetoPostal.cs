using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Enum
{
    /// <summary>
    /// Indica se o objeto foi cadastrado corretamente.
    /// 01 – cadastrado corretamente
    /// 00 – Erro no cadastro do objeto
    /// </summary>
    public enum StatusDoObjetoPostal
    {
        ErroNoCadastro = 0,
        CadastradoCorretamente = 1
    }
}
