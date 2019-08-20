using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intelbras.CRM2013.Domain;
using System.Web.UI.HtmlControls;
using SDKore.Configuration;

namespace Intelbras.CRM2013.UI.Web
{
    public partial class ListaFamiliaComercial : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            Domain.Servicos.FamiliaComercialService famComServ = new Domain.Servicos.FamiliaComercialService(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);
            List<Domain.Model.Product> familiaComercial = famComServ.ObterPor(Request["politicaComercialId"],txtIni.Text, txtFin.Text);
            MontaTabelaRetorno(familiaComercial);
        }

        private void MontaTabelaRetorno(List<Domain.Model.Product> familias)
        {

            #region Cria a tabela base para o grid de famílias
            HtmlTable ht = new HtmlTable();
            ht.ID = "tblFamilias";
            ht.Attributes.Add("Width", "100%");
            #endregion

            int index = 0;

            #region Cabeçalho do grid
            HtmlTableRow cab = new HtmlTableRow();

            #region Checkbox para seleção de todos os itens
            CheckBox chkt = new CheckBox();
            chkt.ID = "cabChk";

            chkt.Attributes.Add("onclick", "SelecionaTodos(this)");
            #endregion

            cab.Cells.Add(MontaCelulaControle("cabChk", chkt, "cabecalho")); //Permite a seleção de todas as famílias

            cab.Cells.Add(MontaCelula("cabCod", "Código do Produto", "cabecalho"));
            cab.Cells.Add(MontaCelula("cabNom", "Nome do Produto", "cabecalho"));

            ht.Rows.Add(cab);
            #endregion

            #region Monta as linhas com as famílias disponíveis
            foreach (Domain.Model.Product familia in familias)
            {
                index += 1;

                string strID = index.ToString(); // +"_" + familia.Codigo;

                #region Popula as linhas
                HtmlTableRow htr = new HtmlTableRow();
                htr.Style.Add("background-color", CorDaLinha(index));

                #region Cria o checkbox para seleção da família
                CheckBox chk = new CheckBox();
                chk.ID = "chk_" + strID;
                chk.InputAttributes.Add("value", familia.ID.ToString());

                chk.Attributes.Add("onclick", "MarcaSelecao(this)");
                #endregion

                htr.Cells.Add(MontaCelulaControle("cchk_" + strID, chk, "label")); //Seleção da família

                htr.Cells.Add(MontaCelula("codfam_" + strID, familia.Codigo, "label")); //Codigo
                htr.Cells.Add(MontaCelula("nomefam_" + strID, familia.Nome, "label")); //Item

                ht.Rows.Add(htr);

                hdQtdTotal.Value = index.ToString();

                #endregion

            }
            #endregion

            phFamilias.Controls.Add(ht);

            if (index > 0)
            {
                pnCampos.Visible = true;
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

        private HtmlTableCell MontaCelula(string strID, string strValor, string classe)
        {
            HtmlTableCell htc1 = new HtmlTableCell();

            htc1.ID = strID;
            htc1.InnerHtml = strValor;
            htc1.Attributes.Add("Class", classe);
            htc1.Style.Add("border-right", "0.5px");
            htc1.Style.Add("padding", "1px");
            return htc1;
        }

        private HtmlTableCell MontaCelulaControle(string strID, Control controle, string classe)
        {
            HtmlTableCell htc1 = new HtmlTableCell();

            htc1.ID = strID;
            htc1.Controls.Add(controle);
            if (classe != string.Empty)
            {
                htc1.Attributes.Add("Class", classe);
            }
            htc1.Style.Add("border-right", "0.5px");
            htc1.Style.Add("padding", "1px");
            return htc1;
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                Domain.Servicos.PoliticaComercialService polComServ = new Domain.Servicos.PoliticaComercialService(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);
                polComServ.InsereProdutosporFamiliaComercial(Request["politicaComercialId"], hdFamilias.Value, Convert.ToInt32(txtQtdIni.Text), Convert.ToInt32(txtQtdFin.Text), Convert.ToDouble(txtFator.Text));
                pnCampos.Visible = false;
                ClientScript.RegisterStartupScript(typeof(Page), "fechar", "window.close();", true);
            }
            catch (Exception erro)
            {
                ClientScript.RegisterStartupScript(typeof(Page), "fechar", "alert('" + erro.Message + "')", true);
            }
        }
    }
}