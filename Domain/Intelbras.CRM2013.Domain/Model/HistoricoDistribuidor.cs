using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_histdistrib")]
    public class HistoricoDistribuidor : DomainBase
    {

        private RepositoryService RepositoryService { get; set; }

        public HistoricoDistribuidor() { }

        public HistoricoDistribuidor(string organization, bool isOffline) : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public HistoricoDistribuidor(string organization, bool isOffline, object provider) : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        [LogicalAttribute("itbc_histdistribid")]
        public Guid? ID { get; set; }


        //String
        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }
        

        // Int
        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        // Int
        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        [LogicalAttribute("itbc_motivotroca")]
        public int? MotivoTroca { get; set; }



        // Lookup
        [LogicalAttribute("itbc_distribuidorid")]
        public Lookup Distribuidor { get; set; }

        [LogicalAttribute("itbc_revendaid")]
        public Lookup Revenda { get; set; }

        [LogicalAttribute("itbc_historicoDistribuidorPai")]
        public Lookup DistribuidorPai { get; set; }




        // Datetime
        [LogicalAttribute("itbc_datainicio")]
        public DateTime? DataInicio { get; set; }

        [LogicalAttribute("itbc_datafim")]
        public DateTime? DataFim { get; set; }

        //controle de looping na msg0179
        [LogicalAttribute("itbc_acaocrm")]
        public bool IntegrarNoPlugin { get; set; }
    }
}
