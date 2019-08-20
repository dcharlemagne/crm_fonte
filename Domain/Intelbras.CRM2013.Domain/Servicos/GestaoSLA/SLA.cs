using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using SDKore.Crm.Util;

namespace Intelbras.CRM2013.Domain.Servicos.GestaoSLA {
   
    [Serializable]
    [LogicalEntity("new_sla")]
    public class SLA
    {

        private RepositoryService RepositoryService { get; set; }

        public SLA(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public SLA(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public SLA()  {  }

        [LogicalAttribute("new_tempo_escalacao")]
        public int TempoDeEscalacao { get; set; }
        [LogicalAttribute("new_tempo_sla")]
        public int Tempo { get; set; }
       
        public DateTime? DataSLA { get; set; }

        public DateTime? DataEscalacao { get; set; }
        [LogicalAttribute("itbc_tempo_solucao")]
        public int TempoSolucao { get; set; }

   }
}
