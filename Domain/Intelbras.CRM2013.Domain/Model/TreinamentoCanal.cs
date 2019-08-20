using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_treinamento_certificacao_canal")]
    public class TreinamentoCanal : DomainBase
    {

        #region Construtores
        
        private RepositoryService RepositoryService { get; set; }

        public TreinamentoCanal(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public TreinamentoCanal(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_treinamento_certificacao_canalid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_account")]
        public Lookup Canal { get; set; }

        [LogicalAttribute("itbc_treinamento_certificacao")]
        public Lookup Treinamento { get; set; }

        [LogicalAttribute("itbc_compromissodocanalid")]
        public Lookup CompromissoCanal { get; set; }

        [LogicalAttribute("itbc_statuscompromissos")]
        public Lookup StatusCompromisso { get; set; }

        [LogicalAttribute("itbc_datalimite")]
        public DateTime? DataLimite { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }


        #endregion
    }
}
