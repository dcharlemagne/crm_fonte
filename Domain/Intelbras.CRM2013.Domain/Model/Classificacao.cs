using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_classificacao")]
    public class Classificacao : DomainBase
    {
        #region Construtores

            private RepositoryService RepositoryService { get; set; }

        public Classificacao() { }

        public Classificacao(string organization, bool isOffline)
                : base(organization, isOffline)
            {
                RepositoryService = new RepositoryService(organization, isOffline);
            }

            public Classificacao(string organization, bool isOffline, object provider)
                : base(organization, isOffline, provider)
            {
                RepositoryService = new RepositoryService(organization, isOffline, provider);
            }

        #endregion

        #region Atributos
            [LogicalAttribute("itbc_classificacaoid")]
            public Guid? ID {get; set;}

            [LogicalAttribute("itbc_name")]
            public String Nome {get; set; }

            [LogicalAttribute("itbc_participadoprogramadecanais")]
            public Boolean PertenceProgramaCanais { get; set; }

            [LogicalAttribute("itbc_acessapv")]
            public Boolean AcessaPV { get; set; }

            [LogicalAttribute("itbc_acessapp")]
            public Boolean AcessaPP { get; set; }          

            [LogicalAttribute("itbc_acessapsd")]
            public Boolean AcessaPSD { get; set; }

            [LogicalAttribute("itbc_acessapscf")]
            public Boolean AcessaPSCF { get; set; }

            [LogicalAttribute("itbc_acessacrossselling")]
            public Boolean AcessaCrossSelling { get; set; }

            [LogicalAttribute("itbc_acessasolucoes")]
            public Boolean AcessaSolucoes { get; set; }

            [LogicalAttribute("itbc_valida_qtd_multipla_item")]
            public Boolean ValidaQtdMultiplaItem { get; set; }

            [LogicalAttribute("itbc_valida_valor_minimo_pedido")]
            public Boolean ValidaValorMinimoPedido { get; set; }

        #endregion
    }
}
