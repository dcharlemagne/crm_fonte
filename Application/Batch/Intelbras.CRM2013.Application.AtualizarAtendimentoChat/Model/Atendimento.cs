using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Application.AtualizarAtendimentoChat.Model
{
    public class Atendimento
    {
        public string protocolNumber { get; set; }
        public double onSystemDate { get; set; }
        public double contactStartedDate { get; set; }
        public double contactFinishedDate { get; set; }
        public int? customerInQueueTime { get; set; }
        public int? agentServingTime { get; set; }
        public int? contactTotalTime { get; set; }
        public string contactState { get; set; }
        public string contactStateDetail { get; set; }
        public string classifications { get; set; }
        public string origin { get; set; }
        public List<ClassificacaoAtendimento> formAnswers { get; set; }

        public List<ClassificacaoAtendimento> ratings { get; set; }
        public string message { get; set; }
    }
}
