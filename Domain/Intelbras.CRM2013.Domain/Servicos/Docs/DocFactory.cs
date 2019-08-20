using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.Domain.Servicos.Docs
{
   
   internal class DocFactory {

      internal static Documento Gerar(Type tipoDoDocumento, string numero) {
         
         if (tipoDoDocumento == typeof(CNPJ))
            return new CNPJ(numero);
         
         return new CPF(numero);

      }
   }
}
