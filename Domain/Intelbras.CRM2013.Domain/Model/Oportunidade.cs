using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("opportunity")]
    public class Oportunidade : IntegracaoBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public Oportunidade() { }

        public Oportunidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public Oportunidade(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }


        #endregion

        #region Atributos
        [LogicalAttribute("opportunityid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_dataprevistaparanegociacao")]
        public DateTime? DataEnvioCotacao { get; set; }
                
        [LogicalAttribute("itbc_reuniaorealizada")]
        public Int32? TeveReuniao { get; set; }
        
        [LogicalAttribute("scheduleproposalmeeting")]
        public DateTime? DataReuniao { get; set; }

        [LogicalAttribute("itbc_dataestimadaparaaprovacao")]
        public DateTime? DataEstimativaAprovacao { get; set; }

        [LogicalAttribute("itbc_propostaaprovada")]
        public Int32? PropostaAprovada { get; set; }

        [LogicalAttribute("itbc_datadaaprovacao")]
        public DateTime? DataAprovacao { get; set; }

        [LogicalAttribute("itbc_dtestimenviopedido")]
        public DateTime? DataEnvioPedidos { get; set; }

        [LogicalAttribute("itbc_pedidosinseridosnoerp")]
        public Int32? PedidosFaturados { get; set; }

        [LogicalAttribute("customerid")]
        public Lookup ClienteProvavel { get; set; }

        [LogicalAttribute("parentcontactid")]
        public Lookup Contato { get; set; }

        [LogicalAttribute("originatingleadid")]
        public Lookup ClientePotencialOriginador { get; set; }

        [LogicalAttribute("itbc_numeroprojeto")]
        public string NumeroProjeto { get; set; }

        private DateTime dataCriacao = DateTime.Now;
        [LogicalAttribute("createdon")]
        public DateTime DataCriacao
        {
            get { return dataCriacao; }
            set { dataCriacao = value; }
        }

        [LogicalAttribute("itbc_revendaintegrid")]
        public Lookup RevendaIntegrador { get; set; }

        [LogicalAttribute("itbc_keyaccountreprdistrid")]
        public Lookup Executivo { get; set; }

        [LogicalAttribute("itbc_distribuidorid")]
        public Lookup Distribuidor { get; set; }

        [LogicalAttribute("stageid")]
        public Guid? StageId { get; set; }

        #endregion
    }
}
