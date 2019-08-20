using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class ModeloMetaResumidaClienteViewModel
    {
        public ModeloMetaResumidaClienteViewModel()
        {
            ListaProdutosMes = new ModeloMetaProdutoMesViewModel[12];
        }

        public Lookup Canal { get; set; }

        public Lookup ClassificacaoCanal { get; set; }

        public Enum.Conta.ParticipaDoPrograma StatusCanal { get; set; }

        public string CodigoCanal { get; set; }

        public string CnpjCanal { get; set; }

        public ModeloMetaProdutoMesViewModel[] ListaProdutosMes { get; set; }
    }
}