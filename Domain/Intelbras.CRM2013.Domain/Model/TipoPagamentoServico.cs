using System;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_tipo_pagamento_servico")]
    public class TipoPagamento : DomainBase
    {
        #region Construtor

        private RepositoryService RepositoryService { get; set; }

        public TipoPagamento() { }

        public TipoPagamento(string organization, bool isOffline): base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public TipoPagamento(string organization, bool isOffline, object provider): base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        #endregion

        #region Atributos

        [LogicalAttribute("itbc_tipo_pagamento_servicoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public String Nome { get; set; }

        #endregion

        #region Metodos

        #endregion
    }
}