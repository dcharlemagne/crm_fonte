using Intelbras.CRM2013.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.UI.Web
{
    public partial class ListarTitulos : System.Web.UI.Page
    {
        public string OrganizationName { get; private set; }
        private readonly bool IsOffline = false;
        public Guid SolicitacaoId { get; private set; }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            try
            {
                SolicitacaoId = new Guid(Request.QueryString["id"]);
            }
            catch
            {
                throw new ArgumentException("(CRM) O paramentro solicitacaoId está incorreto!");
            }


            OrganizationName = Request.QueryString["orgname"];
            
            if(string.IsNullOrEmpty(OrganizationName))
            {
                throw new ArgumentException("(CRM) O paramentro organizationName está incorreto!");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ValidarDados();
            CarregarTitulos();
        }

        private void ValidarDados()
        {
            var solicitacao = new Domain.Servicos.RepositoryService(OrganizationName, IsOffline)
                .SolicitacaoBeneficio.Retrieve(SolicitacaoId, "itbc_status", "itbc_formapagamentoid");

            if (solicitacao.FormaPagamento == null)
            {
                throw new ArgumentException("(CRM) Não é possivel exibir os títulos pois a Forma de Pagamento está vazia.");
            }

            if (!solicitacao.FormaPagamento.Name.ToString().ToLower().Equals("desconto em duplicata"))
            {
                throw new ArgumentException("(CRM) Não é possivel exibir os títulos pois não é uma solicitação de desconto em duplicata.");
            }

            if (!solicitacao.StatusSolicitacao.HasValue)
            {
                throw new ArgumentException("(CRM) Não é possivel exibir os títulos pois o Status da Solicitação está vazio.");
            }

            if (solicitacao.StatusSolicitacao.Value != (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
            {
                throw new ArgumentException("(CRM) Não é possivel exibir os títulos pois a solicitação ainda não foi paga.");
            }
        }

        private void CarregarTitulos()
        {
            List<TituloSolicitacaoViewModel> lista = new Domain.Integracao.MSG0172(OrganizationName, IsOffline)
                .Enviar(SolicitacaoId);

            var listaView = from item in lista
                            select new
                            {
                                Canal = item.NomeConta,
                                Estabelecimento = string.Format("{0} ({1})", FormatarCnpj(item.CNPJEstabelecimento), item.CodigoEstabelecimento),
                                NumeroSerie = item.NumeroSerie,
                                NumeroTitulo = item.NumeroTitulo,
                                Parcela = item.NumeroParcela,
                                DataVencimento = (item.DataVencimento.HasValue) ? item.DataVencimento.Value.ToString("dd/MM/yyyy") : null,
                                ValorOriginal = "R$ " + item.ValorOriginal,
                                ValorAbater = "R$ " + item.ValorAbatido,
                                Saldo = "R$ " + item.SaldoTitulo
                            };

            GridView_titulos.DataSource = listaView;
            GridView_titulos.DataBind();
        }

        private string FormatarCnpj(string value)
        {
            try
            {
                return string.Format("{0}.{1}.{2}/{3}-{4}", value.Substring(0, 2), value.Substring(2, 3), value.Substring(5, 3), value.Substring(8, 4), value.Substring(12, 2));
            }
            catch
            {
                return value;
            }
        }
    }
}
