using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_resgate_premio_fidelidade")]
    public class Resgate : DomainBase
    {
        public Resgate()
        {

        }

        public Resgate(string organization, bool isOffline)
            : base(organization, isOffline)
        {
            this.Tipo = Enum.Resgate.Tipo.NaoInformado;
        }

        public Enum.Resgate.Tipo Tipo { get; set; }

        #region Atributos do CRM

        [LogicalAttribute("new_resgate_premio_fidelidadeid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_name")]
        public string Nome { get; set; }

        [LogicalAttribute("new_codigo_ordem_pedido")]
        public string OrdemPedido { get; set; }

        [LogicalAttribute("new_quantidade_produtos")]
        public int? QuantidadeProdutos { get; set; }

        [LogicalAttribute("new_participanteid")]
        public Lookup Contato { get; set; }

        [LogicalAttribute("new_data_resgate")]
        public DateTime? DataResgate { get; set; }

        [LogicalAttribute("new_premioid")]
        public Lookup PremioIntelbras { get; set; }

        [LogicalAttribute("new_quantidade_pontos_utilizados")]
        public int? QuantidadePontos { get; set; }

        [LogicalAttribute("new_valor")]
        public decimal? Valor { get; set; }

        [LogicalAttribute("new_valor_frete")]
        public decimal? ValorFrete { get; set; }

        [LogicalAttribute("new_valor_total")]
        public decimal? ValorTotal { get; set; }

        [LogicalAttribute("new_detalhes")]
        public string Detalhes { get; set; }
        
        [LogicalAttribute("statuscode")]
        public new int? Status { get; set; }

        #endregion

        public void InativarTodosProdutosResgatadosFidelidade()
        {
            var lista = (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeProdutoResgatado.ListarAtivosPor(this, "new_produto_resgatado_fidelidadeid");
            foreach (var item in lista)
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeProdutoResgatado.AlterarStatus(item.ID.Value, (int)Enum.ProdutoResgatadoFidelidade.RazaoDoStatus.CanceladoPeloSistema, false);
            }
        }
    }
}
