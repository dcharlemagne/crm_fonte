using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Intelbras.CRM2013.Domain;
using System.Web.UI.HtmlControls;
using SDKore.Configuration;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace Intelbras.CRM2013.UI.Web
{
    public partial class BuscaNotasFiscaisDW : System.Web.UI.Page
    {
        public Domain.Model.Conta canal;
        private string organizationName = string.Empty;

        public string OrganizationName
        {
            get
            {
                if (string.IsNullOrEmpty(organizationName))
                    organizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
                return organizationName; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            SDKore.DomainModel.RepositoryFactory.SetTag(this.OrganizationName);
            Guid canalId = new Guid();

            if (Request["id"] != string.Empty)
                canalId = new Guid(Request["id"].ToString());

            //variavel usada para testes
            //canalId = new Guid("9484CD60-D400-E411-9420-00155D013D39");

            if (canalId != null)
            {
                canal = new Domain.Servicos.ContaService(this.OrganizationName, false).BuscaConta((canalId));

                if (canal == null)
                {
                    mensagemRetorno.Text = "Canal não encontrado";
                }
                else if (canal.ParticipantePrograma.Value.CompareTo(((int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)) != 0)
                {
                    mensagemRetorno.Text = "Canal / Conta não participante do Programa de Canais portanto sem configuração de portfólio";
                    ParticipantePrograma.Value = "0";
                    NomeOrganizacao.Value = this.OrganizationName;
                }
                else
                {//canal participante do programa
                    GuidCliente.Value = canal.ID.ToString();
                    ParticipantePrograma.Value = "1";
                    NomeOrganizacao.Value = this.OrganizationName;

                    //List<object> lstObjeto = new List<object>();
                    int index =0;
                    
                    for (int i = 0; i < 30; i++)
                    //foreach (Domain.Model.Product familia in familias)
                    {
                        index += 1;

                        string strID = index.ToString();

                        #region Popula as linhas
                        HtmlTableRow htr = new HtmlTableRow();
                        htr.Style.Add("background-color", CorDaLinha(index));

                        #endregion

                        htr.Cells.Add(MontaCelula("NumNF " + index, "nota"+index, "label")); //Codigo
                        htr.Cells.Add(MontaCelula("Serie " + index, "serie"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("DtEmissao " + index, "dataEmissao"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("Cnpj " + index, "cnpj"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("Filial " + index, "filial"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("VlrComImposto " + index, "valorComImposto"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("VlrIPI " + index, "valorIPI"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("VlrST " + index, "valorST"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("VlrSemImposto " + index, "valorSemImposto"+index,"label")); //Item
                        htr.Cells.Add(MontaCelula("Representante " + index, "representante"+index,"label")); //Item
                        
                        tblNF.Rows.Add(htr);
                    }

                }
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
    }
}
