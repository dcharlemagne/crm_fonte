using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Enum;


namespace Intelbras.CRM2013.Domain.Model
{
    [LogicalEntity("new_extrato_fidelidade")]
    public class ExtratoFidelidade : DomainBase
    {
        public ExtratoFidelidade()
        {

        }

        public ExtratoFidelidade(string organization, bool isOffline)
            : base(organization, isOffline)
        {

        }

        [LogicalAttribute("new_extrato_fidelidadeid")]
        public Guid? ID { get; set; }

        [LogicalAttribute("new_contatoid")]
        public Lookup Participante { get; set; }

        [LogicalAttribute("new_produtoid")]
        public Lookup Produto { get; set; }

        [LogicalAttribute("new_data_pontuacao")]
        public DateTime? DataPontuacao { get; set; }

        [LogicalAttribute("new_data_expiracao")]
        public DateTime? DataExpiracao { get; set; }

        [LogicalAttribute("new_quantidade_pontos")]
        public int? QuantidadePontos { get; set; }

        [LogicalAttribute("new_debito_pontos")]
        public int? DebitoPontos { get; set; }

        [LogicalAttribute("new_numero_serie")]
        public string NumeroSerie { get; set; }

        [LogicalAttribute("new_key_code")]
        public string KeyCode { get; set; }

        [LogicalAttribute("new_fornecedorid")]
        public Lookup Distribuidor { get; set; }

        [LogicalAttribute("new_vendedorid")]
        public Lookup Vendedor { get; set; }

        [LogicalAttribute("statuscode")]
        public new int? Status { get; set; }

        [LogicalAttribute("new_revendaid")]
        public Lookup Revenda { get; set; }

        [LogicalAttribute("new_name")]
        public string Nome { get; set; }

        [LogicalAttribute("new_numero_serie_valido")]
        public bool? NumeroSerieValido { get; set; }


        public string ValidarNumeroSerieKeyCode()
        {
            if (string.IsNullOrEmpty(NumeroSerie))
            {
                throw new ArgumentException("Número de Série não pode estar em branco!");
            }

            string CodigoProduto = string.Empty;
            string DescProduto = string.Empty;
            string KeyCodeProduto = string.Empty;

            try
            {
                Domain.Servicos.HelperWS.IntelbrasService.BuscaItemPorNrSerie(NumeroSerie, out CodigoProduto, out DescProduto, out KeyCodeProduto);
            }
            catch (Exception ex)
            {
                Exception ex_aux = new Exception("Erro ao conectar no serviço do ERP. Método CadastraProdutos, Classe PortalFidelidade", ex);
                ex_aux.Source = ex.Source;
                //throw new ArgumentException(Intelbras.Util.Geral.TratarMensagemErro(ex_aux));
                throw ex_aux;
            }

            if (!string.IsNullOrEmpty(KeyCodeProduto))
            {
                if (string.IsNullOrEmpty(KeyCode) || KeyCodeProduto.ToLower() != KeyCode.ToLower())
                {
                    throw new ArgumentException("Key Code não está correto.");
                }
            }

            if (string.IsNullOrEmpty(CodigoProduto) || CodigoProduto == "0")
            {
                throw new ArgumentException("O código do produto não foi encontrado.");
            }

            return CodigoProduto;
        }
    }
}
