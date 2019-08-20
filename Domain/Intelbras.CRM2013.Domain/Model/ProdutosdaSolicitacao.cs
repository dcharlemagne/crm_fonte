using System;
using System.Collections.Generic;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("itbc_produtosdasolicitacao")]
    public class ProdutosdaSolicitacao : DomainBase
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public ProdutosdaSolicitacao(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            RepositoryService = new RepositoryService(organization, isOffline);
        }

        public ProdutosdaSolicitacao(string organization, bool isOffline, object provider)
            : base(organization, isOffline, provider)
        {
            RepositoryService = new RepositoryService(organization, isOffline, provider);
        }
        #endregion

        #region Atributos
        [LogicalAttribute("itbc_produtosdasolicitacaoid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("itbc_name")]
        public string Nome { get; set; }

        [LogicalAttribute("itbc_productid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("itbc_solicitacaodebeneficioid")]
        public Lookup SolicitacaoBeneficio { get; set; }

        [LogicalAttribute("itbc_benefciodoprogramaid")]
        public Lookup BeneficioPrograma { get; set; }

        [LogicalAttribute("itbc_valorunitario")]
        public Decimal? ValorUnitario { get; set; }

        [LogicalAttribute("itbc_valortotal")]
        public Decimal? ValorTotal { get; set; }

        [LogicalAttribute("itbc_faturaid")]
        public Lookup Fatura { get; set; }

        [LogicalAttribute("itbc_qtdaprovada")]
        public decimal? QuantidadeAprovada { get; set; }

        [LogicalAttribute("itbc_valorunitarioaprovado")]
        public Decimal? ValorUnitarioAprovado { get; set; }
        
        [LogicalAttribute("itbc_valortotalaprovado")]
        public Decimal? ValorTotalAprovado { get; set; }

        [LogicalAttribute("itbc_qtdsolicitado")]
        public decimal? QuantidadeSolicitada { get; set; }
        
        [LogicalAttribute("itbc_quantidadecancelada")]
        public Decimal? QuantidadeCancelada { get; set; }
        [LogicalAttribute("statecode")]
        public int? State { get; set; }

        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }

        [LogicalAttribute("itbc_estabelecimentoid")]
        public Lookup Estabelecimento { get; set; }

        [LogicalAttribute("itbc_valorpago")]
        public decimal? ValorPago { get; set; }

        [LogicalAttribute("itbc_valorcancelado")]
        public decimal? ValorCancelado { get; set; }

        public String Acao { get; set; }

        [LogicalAttribute("itbc_IntegradoFrom")]
        public string IntegradoDe { get; set; }

        [LogicalAttribute("itbc_acaocrm")]
        public bool? IntegrarNoPlugin { get; set; }

        [LogicalAttribute("itbc_quantidadeajustada")]
        public decimal? QuantidadeAjustada { get; set; }

        #endregion
    }
}
