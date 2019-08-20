using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Servicos.Docs;

namespace Intelbras.CRM2013.Domain.Servicos
{

   public class DocService<DocType> where DocType : Documento {

      private Documento documento;

      public DocService(string numeroDoDocumento) {
         this.documento = DocFactory.Gerar(typeof(DocType), numeroDoDocumento);
      }

      public bool FormatoEValido() {
         return this.documento.EValido();
      }

      public string Formatar() {
         return this.documento.Formatar();
      }
   }
}
