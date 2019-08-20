using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.ViewModels
{
    public class CepViewModel
    {
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public bool? CidadeZonaFranca { get; set; }
        public int? CodigoIBGE { get; set; }
        public string Endereco { get; set; }
        public string NomeCidade { get; set; }
        public string UF { get; set; }
        public Lookup Municipio { get; set; }
        public Lookup Estado { get; set; }
        public Lookup Pais { get; set; }
        public string Menssagem { get; set; }
    }
}
