using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intelbras.CRM2013.Domain;
using System.Web.UI.HtmlControls;
using SDKore.Configuration;
using Intelbras.CRM2013.Domain.Integracao;
using Intelbras.CRM2013.Domain.Model;
using System.ComponentModel;

namespace Intelbras.CRM2013.UI.Web
{
    public partial class ListaDistribuidoresDaRevenda : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Request["revendaID"] != null)
            {
                MSG0180 busca = new MSG0180(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);
                List<Conta> contas = busca.Executar(Convert.ToString(Page.Request["revendaID"]).Replace("{", "").Replace("}", ""), "MSG0180");
                MontaTabelaRetorno(contas);
                // http://crm2013d.intelbras.com.br:5554/ISV/WEB/ListaDistribuidoresDaRevenda.aspx?revendaID=5A35FF39-471E-E511-941A-00155D013A30
            }
            else
            {
                Response.Write("<script>window.close();</script>");
            }
        }
        
        private void MontaTabelaRetorno(List<Conta> contas)
        {

            #region Cria a tabela base para o grid de famílias
            HtmlTable ht = new HtmlTable();
            ht.ID = "tblFamilias";
            ht.Attributes.Add("Width", "100%");
            #endregion

            int index = 0;

            #region Cabeçalho do grid
            HtmlTableRow cab = new HtmlTableRow();

            cab.Cells.Add(MontaCelula("cabRaz", "Razão", "cabecalho"));
            cab.Cells.Add(MontaCelula("cabNom", "Nome", "cabecalho"));
            cab.Cells.Add(MontaCelula("cabCnp", "CNPJ", "cabecalho"));
            cab.Cells.Add(MontaCelula("cabCod", "Código", "cabecalho"));
            cab.Cells.Add(MontaCelula("cabSel", "Data da Última Nota Fiscal Encontrada no SellOut", "cabecalho"));
            cab.Cells.Add(MontaCelula("cabPCI", "Participa do PCI", "cabecalho"));

            ht.Rows.Add(cab);
            #endregion

            #region Monta as linhas com as contas disponíveis
            foreach (Conta conta in contas)
            {
                index += 1;

                string strID = index.ToString(); // +"_" + familia.Codigo;

                #region Popula as linhas
                HtmlTableRow htr = new HtmlTableRow();
                htr.Style.Add("background-color", CorDaLinha(index));

                htr.Cells.Add(MontaCelula("razao_" + strID, conta.RazaoSocial, "label"));
                htr.Cells.Add(MontaCelula("nome_" + strID, conta.NomeFantasia, "label"));
                htr.Cells.Add(MontaCelula("cnpj_" + strID, conta.CpfCnpj, "label"));
                htr.Cells.Add(MontaCelula("cod_" + strID, conta.CodigoMatriz, "label"));
                htr.Cells.Add(MontaCelula("sellout_" + strID, (conta.DataUltimoSelloutRevenda.HasValue ? conta.DataUltimoSelloutRevenda.Value.ToString("dd/MM/yyyy") : ""), "label"));
                htr.Cells.Add(MontaCelula("pci_" + strID, (conta.ParticipantePrograma.Value == 993520001 ? "Sim" : (conta.ParticipantePrograma.Value == 993520000 ? "Não" : (conta.ParticipantePrograma.Value == 993520002 ? "Descredenciado" : (conta.ParticipantePrograma.Value == 993520003 ? "Pendente" : "Não")))), "label"));

                ht.Rows.Add(htr);
                #endregion

            }
            #endregion

            phFamilias.Controls.Add(ht);
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
    }
}