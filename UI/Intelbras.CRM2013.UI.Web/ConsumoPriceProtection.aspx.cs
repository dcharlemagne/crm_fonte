using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intelbras.CRM2013.Domain;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System.Web.UI.HtmlControls;
using SDKore.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace Intelbras.CRM2013.UI.Web
{
    public partial class ConsumoPriceProtection : System.Web.UI.Page
    {
        public Domain.Model.SolicitacaoBeneficio solicitacaoBeneficio;

        private string organizationName = string.Empty;

        public string OrganizationName
        {
            get
            {
                if (string.IsNullOrEmpty(organizationName))
                    organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
                return organizationName;
            }
        }

        #region PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {

            SDKore.DomainModel.RepositoryFactory.SetTag(this.OrganizationName);
            Guid solBenefId = new Guid();

            //Tipo do registro conta/contato
            string tipo = string.Empty;

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

            //if (Request["id"] != string.Empty)
            //{
            //    solBenefId = new Guid(Request["id"].ToString());
            //    this.CarregarDadosSolicitacao(solBenefId);
            //}

            //Teste:
            solBenefId = new Guid("80d6a196-bb65-e411-93f7-00155d013e70");
            this.CarregarDadosSolicitacao(solBenefId);

        }

        #endregion

        #region Metodos

        protected void CarregarDadosSolicitacao(Guid solBenefId)
        {

            solicitacaoBeneficio = new Domain.Servicos.SolicitacaoBeneficioService(this.organizationName, false).ObterPor(solBenefId);

            if (solicitacaoBeneficio != null
                && (int)Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada == solicitacaoBeneficio.StatusSolicitacao.Value)
            {
                List<ProdutosdaSolicitacao> lstProdSolicitacao = new ProdutosdaSolicitacaoService(this.organizationName, false).ListarPorSolicitacao(solBenefId);

                if (lstProdSolicitacao != null && lstProdSolicitacao.Count > 0)
                {
                    bool existe = lstProdSolicitacao.Any(prodSol => prodSol.QuantidadeAprovada.HasValue
                                        && prodSol.QuantidadeSolicitada.HasValue
                                        && (prodSol.QuantidadeSolicitada - prodSol.QuantidadeAprovada) > 0);

                    if (!existe)
                    {
                        mensagemRetorno.Text = "Não existem produtos com quantidades disponíveis para solicitar consumo.";
                        this.MontaHeader();
                        return;
                    }
                    int index = 0;
                    this.MontaHeader();
                    foreach (var prodSol in lstProdSolicitacao)
                    {
                        if (prodSol.QuantidadeAprovada.HasValue
                                        && prodSol.QuantidadeSolicitada.HasValue
                                        && (prodSol.QuantidadeSolicitada - prodSol.QuantidadeAprovada) > 0)
                        {
                            index += 1;


                            Product produto = new ProdutoService(this.organizationName, false).ObterPor(prodSol.Produto.Id);

                            

                            if (produto != null)
                            {
                                var quant = prodSol.QuantidadeSolicitada - prodSol.QuantidadeAprovada;
                                string codigoProduto = produto.Codigo;
                                string nomeProduto = produto.Nome;
                                string quantidade = quant.ToString();
                                string priceProtection = prodSol.ValorUnitario.Value.ToString();

                                #region Popula as linhas
                                HtmlTableRow htr = new HtmlTableRow();
                                htr.Style.Add("background-color", CorDaLinha(index));
                                #endregion

                                htr.Cells.Add(MontaCelula("CodProduto " + index, codigoProduto, "label"));
                                htr.Cells.Add(MontaCelula("NomeProduto " + index, nomeProduto, "label"));
                                htr.Cells.Add(MontaCelula("Quantidade " + index, quantidade, "label"));
                                htr.Cells.Add(MontaCelula("PriceProtection " + index, priceProtection, "label"));
                                htr.Cells.Add(MontaCelula("Seleciona " + index, "<input id=\"Checkbox1\" type=\"checkbox\" />", "label"));
                                //<input id="Checkbox1" type="checkbox" />
                                tblRetornoProduto.Rows.Add(htr);
                            }
                            else
                            {
                                mensagemRetorno.Text = "Produto não encontrado.";
                                this.MontaHeader();
                                return;
                            }
                        }
                    }

                }
                else
                {
                    mensagemRetorno.Text = "Solicitação não possui produtos para solicitar consumo. Favor inserir produtos na solicitação.";
                    this.MontaHeader();
                    return;
                }
            }
            else
            {
                mensagemRetorno.Text = "Solicitação com status diferente de Aprovada.";
                this.MontaHeader();
            }


        }

        private string CorDaLinha(int index)
        {
            string cor = "#fff";

            if (index % 2 == 0)
            {
                cor = "#e4f7d1";
            }

            return cor;
        }
        private void MontaHeader()
        {
            HtmlTableRow htr = new HtmlTableRow();

            htr.Attributes.Add("class", "cabecalho");

            htr.Cells.Add(this.MontaCelula("tdCodProduto", "Código do Produto", "cabecalho"));
            htr.Cells.Add(this.MontaCelula("tdNomeProduto", "Produto", "cabecalho"));
            htr.Cells.Add(this.MontaCelula("tdQuantidade", "Quantidade", "cabecalho"));
            htr.Cells.Add(this.MontaCelula("tdPriceProtection", "Price Protection Unitário (R$)", "cabecalho"));
            htr.Cells.Add(this.MontaCelula("tdSeleciona", "Selecionar", "cabecalho"));

            tblRetornoProduto.Rows.Add(htr);

        }
        private HtmlTableCell MontaCelula(string strID, string strValor, string classe)
        {
            HtmlTableCell htc1 = new HtmlTableCell();

            htc1.ID = strID;
            htc1.InnerHtml = strValor;
            htc1.Attributes.Add("Class", classe);
            htc1.Style.Add("border-right", "0.5px");
            htc1.Style.Add("padding", "1px");
            htc1.Style.Add("align", "right");
            return htc1;
        }

        #endregion

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {

        }
    }
}
