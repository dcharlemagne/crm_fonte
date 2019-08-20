using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class ModeloMetaDetalhadaClienteViewModel
    {
        public ModeloMetaDetalhadaClienteViewModel()
        {
            ListaProdutosMes = new ModeloMetaProdutoMesViewModel[12];
        }

        public Lookup Segmento { get; set; }

        public Lookup Familia { get; set; }

        public Lookup SubFamilia { get; set; }

        public Lookup Produto { get; set; }

        public Lookup Canal { get; set; }

        public Lookup ClassificacaoCanal { get; set; }

        public Enum.Produto.StatusCode StatusProduto { get; set; }

        public string CodigoProduto { get; set; }

        public Enum.Conta.ParticipaDoPrograma StatusCanal { get; set; }

        public string CodigoCanal { get; set; }

        public ModeloMetaProdutoMesViewModel[] ListaProdutosMes { get; set; }
    }
}