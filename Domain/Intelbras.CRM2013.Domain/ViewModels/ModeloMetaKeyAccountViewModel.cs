using SDKore.DomainModel;
using System;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class ModeloMetaKeyAccountViewModel
    {
        public ModeloMetaKeyAccountViewModel()
        {
            ListaProdutosMes = new ModeloMetaProdutoMesViewModel[12];
        }

        public int? Ano { get; set; }

        public Lookup UnidadeNegocio { get; set; }

        public Lookup Segmento { get; set; }

        public Lookup Familia { get; set; }

        public Lookup SubFamilia { get; set; }

        public Lookup Produto { get; set; }

        public Lookup KeyAccount { get; set; }

        public string StatusProduto { get; set; }

        public string CodigoProduto { get; set; }

        public string StatusKeyAccount { get; set; }

        public string CodigoKeyAccount { get; set; }

        public string CnpjKeyAccount { get; set; }

        public bool Ignorar { get; set; }

        public ModeloMetaProdutoMesViewModel[] ListaProdutosMes { get; set; }
    }
}
