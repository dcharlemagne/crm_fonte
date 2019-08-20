using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos.Docs
{
    public class Estrangeiro : Documento
    {
        public Estrangeiro(string numero)
            : base(numero)
        {
        }

        public override bool EValido()
        {
            return true;
        }

        public override string Formatar()
        {
            throw new NotImplementedException();
        }
    }
}
