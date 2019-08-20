using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos.Docs
{

   public abstract class Documento {

      protected Documento(string numero) {
         this.numero = numero;
      }

      private string numero = "";
      public string Numero {
         get { return numero; }
      }

      public abstract bool EValido();
        public abstract string Formatar();
   }
}
