using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Intelbras.CRM2013.Application.AtualizarAtendimentoChat.Model
{
    public class DetalheAtendimento
    {
        public String ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinalDate { get; set; }
        public String Dialogo { get; set; }
        public String NameVisitor { get; set; }
        public String IPVisitor { get; set; }
        public String PhoneVisitor { get; set; }
        public String CPF { get; set; }
        public String EmailVisitor { get; set; }
        public string Browser { get; set; }
        public string Version { get; set; }
        public string Platform { get; set; }     
    }   
}
