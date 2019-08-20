using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_prodportreinecertif")]
    public class ProdutoTreinamento : DomainBase
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoTreinamento() { }

        public ProdutoTreinamento(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }

        public ProdutoTreinamento(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }
        #endregion

        #region Atributos

        [LogicalAttribute("itbc_prodportreinecertifid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_treinamentoecertfid")]
        public Lookup Treinamento { get; set; }

        [LogicalAttribute("itbc_obrigatoriarealizacao")]
        public int? Obrigatorio { get; set; }

        [LogicalAttribute("itbc_nummindeprofissionais")]
        public int? NroMinimoProf { get; set; }

        [LogicalAttribute("itbc_productid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_bloqueiaportflio")]
        public int? BloqueiaPortfolio { get; set; }

        [LogicalAttribute("statuscode")]
        public int? Status { get; set; }

        #endregion
    }
}
