using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_solicitacaodecadastro")]
    public class SolicitacaoCadastro : DomainBase
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SolicitacaoCadastro() { }

        public SolicitacaoCadastro(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public SolicitacaoCadastro(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_solicitacaodecadastroid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_descricao_solicitacao")]
        public string Descricao { get; set; }

        [LogicalAttribute("itbc_participante")]
        public Lookup Participante { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_oportunidade")]
        public Lookup Oportunidade { get; set; }

        [LogicalAttribute("itbc_accountid")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_tipodesolicitacaoid")]
        public Lookup TipoDeSolicitacao { get; set; }

        //[LogicalAttribute("itbc_papel")]
        //public int? Papel { get; set; }
        
        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("itbc_necessidade")]
        public int? Necessidade { get; set; }

        [LogicalAttribute("itbc_keyaccountourepresentanteid")]
        public Lookup Representante { get; set; }

        [LogicalAttribute("itbc_supervisor_de_vendas")]
        public Lookup SupervisorVendas { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public Boolean IntegrarNoPlugin { get; set; }

        [LogicalAttribute("createdon")]
        public DateTime? DataCriacao { get; set; }
        
        #endregion
    }
}
