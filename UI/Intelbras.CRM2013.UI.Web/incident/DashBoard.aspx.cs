using DocumentFormat.OpenXml.Math;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.EnterpriseServices.CompensatingResourceManager;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace Intelbras.CRM2013.UI.Web.Pages.incident
{
    public partial class DashBoard : System.Web.UI.Page
    {
        //public static DataSet dsOcorrencias = new DataSet();
        //public static DataTable dtDashBoard = new DataTable("DashBoard");

        public static int TotalRed = 0;
        public static int TotalYellow = 0;
        public static int TotalGreen = 0;

        public static String m_strSortExp;
        public static SortDirection m_SortDirection;

        private SqlConnection SqlConnection
        {
            get { return new SqlConnection(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.CRM2013.Database")); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            TotalRed = 0;
            TotalYellow = 0;
            TotalGreen = 0;
            SqlConnection sqlConnection = null;
            sqlConnection = this.SqlConnection;
            sqlConnection.Open();

            try
            {
                if (!IsPostBack)
                {
                    //Carregar Combo Proprietário
                    SqlConnection sqlConnection1 = null;
                    sqlConnection1 = this.SqlConnection;
                    sqlConnection1.Open();

                    String query2 = "SELECT FULLNAME, SystemUserId FROM DBO.SYSTEMUSER SU WHERE SU.BUSINESSUNITIDNAME = 'ISOL' order by fullname";
                    SqlCommand cmd2 = new SqlCommand(query2, sqlConnection1);
                    SqlDataReader IDnoCRM2 = cmd2.ExecuteReader();

                    this.DropDownList1.DataSource = IDnoCRM2;
                    this.DropDownList1.DataValueField = "SystemUserId";
                    this.DropDownList1.DataTextField = "fullname";
                    this.DropDownList1.DataBind();
                    this.DropDownList1.Items.Insert(0, new ListItem("Todos...", string.Empty));


                    DateTime inicio = DateTime.Now;

                    sqlConnection = this.SqlConnection;
                    sqlConnection.Open();

                    string query = @"SELECT incidentid
                                    , ticketnumber
                                    , CustomerId
                                    , CustomerIdName
		    		                , new_empresa_executanteid
	     			                , new_empresa_executanteidName 
                                    , Ownerid
     				                , OwneridName
                                    , new_data_origem                        
				                    , PriorityCode
                                    , followupby
                                    , new_data_hora_escalacao
                                FROM Incident
                                WHERE new_data_hora_conclusao IS NULL
                                    AND new_data_hora_escalacao IS NOT NULL
                                    AND followupby IS NOT NULL
                                    AND contractdetailid IS NOT NULL 
                                    AND statuscode in (200000,200001,200002,200005,200006,993520002,993520003,993520010)
                                    AND CaseTypeCode in (200094,200095,200090,200091,200092,200096,200093)
                                    ORDER BY OwneridName";

                    SqlCommand cmd = new SqlCommand(query, sqlConnection);
                    SqlDataReader IDnoCRM = cmd.ExecuteReader();

                    List<Ocorrencia> ocorrencias = new List<Ocorrencia>();
                    if (IDnoCRM.HasRows)
                    {
                        while (IDnoCRM.Read())
                        {
                            Ocorrencia ocorrencia = new Ocorrencia();
                            ocorrencia.Id = new Guid(IDnoCRM["incidentid"].ToString());
                            ocorrencia.Nome = IDnoCRM["ticketnumber"].ToString();
                            ocorrencia.Numero = IDnoCRM["ticketnumber"].ToString();
                            ocorrencia.Cliente = new CRM2013.Domain.Model.Conta();
                            if (IDnoCRM["CustomerId"] != null && IDnoCRM["CustomerId"] != DBNull.Value)
                            {
                                ocorrencia.Cliente.Id = new Guid(IDnoCRM["CustomerId"].ToString());
                                ocorrencia.Cliente.Nome = IDnoCRM["CustomerIdName"].ToString();
                                ocorrencia.Cliente.NomeFantasia = IDnoCRM["CustomerIdName"].ToString();
                            }

                            ocorrencia.EmpresaExecutante = new CRM2013.Domain.Model.Conta();
                            if (IDnoCRM["new_empresa_executanteid"] != null && IDnoCRM["new_empresa_executanteid"] != DBNull.Value)
                            {
                                ocorrencia.EmpresaExecutante.Id = new Guid(IDnoCRM["new_empresa_executanteid"].ToString());
                                ocorrencia.EmpresaExecutante.Nome = IDnoCRM["new_empresa_executanteidName"].ToString();
                            }

                            if (IDnoCRM["new_data_hora_escalacao"] != null && IDnoCRM["new_data_hora_escalacao"] != DBNull.Value) //Abertura
                                ocorrencia.DataEscalacao = Convert.ToDateTime(IDnoCRM["new_data_hora_escalacao"]).ToLocalTime();

                            if (IDnoCRM["new_data_origem"] != null && IDnoCRM["new_data_origem"] != DBNull.Value) //Abertura
                                ocorrencia.DataOrigem = Convert.ToDateTime(IDnoCRM["new_data_origem"]).ToLocalTime();

                            ocorrencia.PrioridadeValue = (int)(IDnoCRM["PriorityCode"]);

                            if (IDnoCRM["followupby"] != null && IDnoCRM["followupby"] != DBNull.Value) // SLA
                                ocorrencia.DataSLA = Convert.ToDateTime(IDnoCRM["followupby"]).ToLocalTime();

                            ocorrencia.LinhaDeContrato = new LinhaDeContrato();

                            ocorrencias.Add(ocorrencia);
                        }
                    }

                    List<Ocorrencia> lista = ocorrencias;

                    DataSet dsOcorrencias = new DataSet();
                    DataTable dtDashBoard = new DataTable("DashBoard");

                    dtDashBoard.Columns.Add("OcorrenciaId");
                    dtDashBoard.Columns.Add("nome");
                    dtDashBoard.Columns.Add("ImgSinal");
                    dtDashBoard.Columns.Add("SinalID");
                    dtDashBoard.Columns.Add("NomeDoCliente");
                    dtDashBoard.Columns.Add("ClienteId");
                    dtDashBoard.Columns.Add("Cliente");
                    dtDashBoard.Columns.Add("DtCriacao");
                    dtDashBoard.Columns.Add("DtSLA");
                    dtDashBoard.Columns.Add("NroOcorrencia");
                    dtDashBoard.Columns.Add("EmpresaExecutante");
                    dtDashBoard.Columns.Add("Proprietario");
                    dtDashBoard.Columns.Add("Prioridade");
                    dtDashBoard.Columns.Add("DtEscalacao");
                    dtDashBoard.Columns.Add("OrdSinal");
                    dtDashBoard.Columns.Add("Porcentagem");



                    foreach (Ocorrencia ocorrencia in lista)
                    {

                        if (ocorrencia.LinhaDeContrato != null)
                        {
                            DataRow drOcorrencia = dtDashBoard.NewRow();
                            drOcorrencia["OcorrenciaId"] = ocorrencia.Id;
                            drOcorrencia["Nome"] = ocorrencia.Nome;
                            drOcorrencia["ImgSinal"] = "imagens/Sinal_Verde.jpg";
                            drOcorrencia["SinalID"] = "2";

                            drOcorrencia["Cliente"] = "";
                            if (ocorrencia.Cliente != null)
                            {
                                drOcorrencia["Cliente"] = ocorrencia.Cliente.NomeFantasia.ToString();
                                drOcorrencia["ClienteId"] = ocorrencia.Cliente.Id.ToString();

                                if (drOcorrencia["Cliente"].ToString() == "")
                                    drOcorrencia["Cliente"] = ocorrencia.Cliente.Nome.ToString();

                                drOcorrencia["NomeDoCliente"] = ocorrencia.Cliente.Nome.ToString();
                            }

                            drOcorrencia["DtCriacao"] = ocorrencia.DataOrigem.Value.ToString("dd/MM/yyyy HH:mm");
                            drOcorrencia["DtSLA"] = ocorrencia.DataSLA.Value.ToString("dd/MM/yyyy HH:mm");
                            drOcorrencia["NroOcorrencia"] = ocorrencia.Numero;

                            if (ocorrencia.EmpresaExecutante != null)
                            {
                                if (ocorrencia.EmpresaExecutante.Nome != null)
                                    drOcorrencia["EmpresaExecutante"] = ocorrencia.EmpresaExecutante.Nome.ToString();
                            }

                            var usuario = (new Domain.Servicos.RepositoryService()).Usuario.BuscarProprietario("incident", "incidentid", ocorrencia.Id);

                            if (usuario != null)
                            {
                                drOcorrencia["Proprietario"] = usuario.NomeCompleto.ToString();
                            }


                            drOcorrencia["DtEscalacao"] = ocorrencia.DataEscalacao;
                            drOcorrencia["Prioridade"] = ocorrencia.Prioridade;
                            drOcorrencia["OrdSinal"] = ocorrencia.DataSLA.Value.ToString("dd/MM/yyyy HH:mm");


                            if (DateTime.Now < ocorrencia.DataEscalacao)
                            {
                                drOcorrencia["ImgSinal"] = "imagens/Sinal_Verde.jpg";
                                drOcorrencia["SinalID"] = "2";
                                drOcorrencia["OrdSinal"] = "2" + ocorrencia.DataEscalacao.Value.ToString("yyyyMMddHH:mm");
                                TotalGreen++;
                            }
                            else if ((ocorrencia.DataEscalacao < DateTime.Now) && (DateTime.Now < ocorrencia.DataSLA))
                            {
                                drOcorrencia["ImgSinal"] = "imagens/Sinal_Amarelo.jpg";
                                drOcorrencia["SinalID"] = "1";
                                drOcorrencia["OrdSinal"] = "1" + ocorrencia.DataSLA.Value.ToString("yyyyMMddHH:mm");
                                TotalYellow++;

                            }
                            else if (DateTime.Now > ocorrencia.DataSLA)
                            {
                                drOcorrencia["ImgSinal"] = "imagens/Sinal_Vermelho.jpg";
                                drOcorrencia["SinalID"] = "0";
                                drOcorrencia["OrdSinal"] = "0" + ocorrencia.DataSLA.Value.ToString("yyyyMMddHH:mm");
                                TotalRed++;
                            }

                            double totalMinutos = ((DateTime)ocorrencia.DataSLA).Subtract((DateTime)ocorrencia.DataOrigem).TotalMinutes;

                            double totalNow = (DateTime.Now.Subtract((DateTime)ocorrencia.DataOrigem)).TotalMinutes;

                            double totalSla = (totalNow / totalMinutos) * 100;

                            drOcorrencia["Porcentagem"] = totalSla.ToString("0.00") + "%";

                            lbContLinha.Text = "Total de linhas: " + lista.Count.ToString();

                            dtDashBoard.Rows.Add(drOcorrencia);
                        }
                    }

                    dsOcorrencias.DataSetName = "Ocorrencias";
                    dsOcorrencias.Tables.Add(dtDashBoard);
                    dsOcorrencias.AcceptChanges();

                    grid_ocorrencias.DataSource = dsOcorrencias.DefaultViewManager.DataSet;
                    grid_ocorrencias.DataBind();

                    //grid_ocorrencias.Sort("SinalID, DtSLA", SortDirection.Ascending);
                    grid_ocorrencias.Sort("OrdSinal", SortDirection.Ascending);
                    PopulaGrid();
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
            }
        }

        protected void GridDataBound_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            System.Web.UI.WebControls.Label porcentagem = e.Row.FindControl("lbPorcentagem") as System.Web.UI.WebControls.Label;

            HtmlGenericControl div = (HtmlGenericControl)e.Row.FindControl("grafico");

            if (div != null)
            {
                var convPorcent = porcentagem.Text.Remove(porcentagem.Text.Length - 1);

                if (Convert.ToDouble(convPorcent) <= 60)
                {
                    div.Attributes.Add("style", "background-color:#008000; border: 1px solid; font-size:25px; color:#FFFFFF; Font-Bold=true; width:100%; height:24px;");
                }
                else if (Convert.ToDouble(convPorcent) >= 60 && Convert.ToDouble(convPorcent) <= 100)
                {
                    div.Attributes.Add("style", "background-color:#ffff00; border: 1px solid; font-size:25px; color:#000000; Font-Bold=true; Font-Bold=true; width:100%; height:24px;");
                }
                else
                {
                    div.Attributes.Add("style", "background-color:#ff0000; border: 1px solid; font-size:25px; color:#FFFFFF; Font-Bold=true; Font-Bold=true; width:100%; height:24px;");
                }
            }

        }

        protected void PopulaGrid()
        {
            try
            {
                // Red
                grafico_dashboard.Rows[0].Cells[0].Text = TotalRed.ToString();
                grafico_dashboard.Rows[0].Cells[0].Width = Convert.ToInt32(TotalRed / grafico_dashboard.Rows.Count * 100);
                // Yellow
                grafico_dashboard.Rows[0].Cells[1].Text = TotalYellow.ToString();
                grafico_dashboard.Rows[0].Cells[1].Width = Convert.ToInt32(TotalYellow / grafico_dashboard.Rows.Count * 100);
                // Green
                grafico_dashboard.Rows[0].Cells[2].Text = TotalGreen.ToString();
                grafico_dashboard.Rows[0].Cells[2].Width = Convert.ToInt32(TotalGreen / grafico_dashboard.Rows.Count * 100);

            }
            catch (Exception ex)
            {
                Response.Write("Bugs: " + ex.InnerException + "<br>" + ex.Message + "<br>" + ex.Source + "<br>" + ex.StackTrace);
            }

        }

        protected void grid_ocorrencias_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataView dv = new DataView(grid_ocorrencias.DataSource as DataTable);
            DataTable dtResult = new DataTable();

            DataSet ds = new DataSet();
            ds = (DataSet)grid_ocorrencias.DataSource;

            ds.Tables[0].DefaultView.Sort = e.SortExpression;

            grid_ocorrencias.DataSource = ds.Tables[0].DefaultView;
            grid_ocorrencias.DataBind();
        }

        protected void grid_ocorrencias_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString() == "Select")
            {
                var ocorrenciaId = this.grid_ocorrencias.DataKeys[Convert.ToInt32(e.CommandArgument)]["OcorrenciaId"];
                var scriptAbrir = string.Format("AbrirOcorrencia('{0}');", ocorrenciaId);
                this.ExecutaScript(scriptAbrir, "abrir");
            }
        }

        private void ExecutaScript(string script, string key)
        {
            ClientScript.RegisterClientScriptBlock(this.Page.GetType(), key, script, true);
        }

        protected void DropDownList(object sender, EventArgs e)
        {

            TotalRed = 0;
            TotalYellow = 0;
            TotalGreen = 0;
            SqlConnection sqlConnection = null;

            try
            {
                string cmditem = DropDownList1.SelectedValue.ToString();
                string nmProprietario = DropDownList1.SelectedItem.Text;
                lbMensagem.Text = "";
                grafico_dashboard.Visible = true;
                DateTime inicio = DateTime.Now;

                sqlConnection = this.SqlConnection;
                sqlConnection.Open();

                string query;
                if (cmditem != string.Empty)
                {
                    query = @"SELECT incidentid
                                    , ticketnumber
                                    , CustomerId
                                    , CustomerIdName
			    	                , new_empresa_executanteid
		    		                , new_empresa_executanteidName 
                                    , Ownerid
	    			                , OwneridName
                                    , new_data_origem                        
    				                , PriorityCode
                                    , followupby
                                    , new_data_hora_escalacao
                                FROM Incident
                                WHERE new_data_hora_conclusao IS NULL
                                    AND new_data_hora_escalacao IS NOT NULL
                                    AND followupby IS NOT NULL
                                    AND contractdetailid IS NOT NULL 
                                    AND statuscode in (200000,200001,200002,200005,200006,993520002,993520003,993520010)
                                    AND CaseTypeCode in (200094,200095,200090,200091,200092,200096,200093)	
                                    AND dbo.Incident.OwnerId = '" + cmditem + "'";
                }
                else
                {
                    query = @"SELECT incidentid
                                    , ticketnumber
                                    , CustomerId
                                    , CustomerIdName
				                    , new_empresa_executanteid
				                    , new_empresa_executanteidName 
                                    , Ownerid
				                    , OwneridName
                                    , new_data_origem                        
				                    , PriorityCode
                                    , followupby
                                    , new_data_hora_escalacao
                                FROM Incident
                                WHERE new_data_hora_conclusao IS NULL
                                    AND new_data_hora_escalacao IS NOT NULL
                                    AND followupby IS NOT NULL
                                    AND contractdetailid IS NOT NULL 
                                    AND statuscode in (200000,200001,200002,200005,200006,993520002,993520003,993520010)
                                    AND CaseTypeCode in (200094,200095,200090,200091,200092,200096,200093)";

                }

                SqlCommand cmd = new SqlCommand(query, sqlConnection);
                SqlDataReader IDnoCRM = cmd.ExecuteReader();

                List<Ocorrencia> ocorrencias = new List<Ocorrencia>();
                if (IDnoCRM.HasRows)
                {
                    while (IDnoCRM.Read())
                    {
                        Ocorrencia ocorrencia = new Ocorrencia();
                        ocorrencia.Id = new Guid(IDnoCRM["incidentid"].ToString());
                        ocorrencia.Nome = IDnoCRM["ticketnumber"].ToString();
                        ocorrencia.Numero = IDnoCRM["ticketnumber"].ToString();
                        ocorrencia.Cliente = new CRM2013.Domain.Model.Conta();
                        if (IDnoCRM["CustomerId"] != null && IDnoCRM["CustomerId"] != DBNull.Value)
                        {
                            ocorrencia.Cliente.Id = new Guid(IDnoCRM["CustomerId"].ToString());
                            ocorrencia.Cliente.Nome = IDnoCRM["CustomerIdName"].ToString();
                            ocorrencia.Cliente.NomeFantasia = IDnoCRM["CustomerIdName"].ToString();
                        }

                        ocorrencia.EmpresaExecutante = new CRM2013.Domain.Model.Conta();
                        if (IDnoCRM["new_empresa_executanteid"] != null && IDnoCRM["new_empresa_executanteid"] != DBNull.Value)
                        {
                            ocorrencia.EmpresaExecutante.Id = new Guid(IDnoCRM["new_empresa_executanteid"].ToString());
                            ocorrencia.EmpresaExecutante.Nome = IDnoCRM["new_empresa_executanteidName"].ToString();
                        }

                        if (IDnoCRM["new_data_origem"] != null && IDnoCRM["new_data_origem"] != DBNull.Value) //Abertura
                            ocorrencia.DataOrigem = Convert.ToDateTime(IDnoCRM["new_data_origem"]).ToLocalTime();

                        if (IDnoCRM["new_data_hora_escalacao"] != null && IDnoCRM["new_data_hora_escalacao"] != DBNull.Value) //Abertura
                            ocorrencia.DataEscalacao = Convert.ToDateTime(IDnoCRM["new_data_hora_escalacao"]).ToLocalTime();

                        ocorrencia.PrioridadeValue = (int)(IDnoCRM["PriorityCode"]);

                        if (IDnoCRM["followupby"] != null && IDnoCRM["followupby"] != DBNull.Value) // SLA
                            ocorrencia.DataSLA = Convert.ToDateTime(IDnoCRM["followupby"]).ToLocalTime();

                        ocorrencia.LinhaDeContrato = new LinhaDeContrato();

                        ocorrencias.Add(ocorrencia);
                    }
                }

                List<Ocorrencia> lista = ocorrencias;

                DataSet dsOcorrencias = new DataSet();
                DataTable dtDashBoard = new DataTable("DashBoard");

                dtDashBoard.Columns.Add("OcorrenciaId");
                dtDashBoard.Columns.Add("nome");
                dtDashBoard.Columns.Add("ImgSinal");
                dtDashBoard.Columns.Add("SinalID");
                dtDashBoard.Columns.Add("NomeDoCliente");
                dtDashBoard.Columns.Add("ClienteId");
                dtDashBoard.Columns.Add("Cliente");
                dtDashBoard.Columns.Add("DtCriacao");
                dtDashBoard.Columns.Add("DtSLA");
                dtDashBoard.Columns.Add("NroOcorrencia");
                dtDashBoard.Columns.Add("EmpresaExecutante");
                dtDashBoard.Columns.Add("Prioridade");
                dtDashBoard.Columns.Add("DtEscalacao");
                dtDashBoard.Columns.Add("Proprietario");
                dtDashBoard.Columns.Add("OrdSinal");
                dtDashBoard.Columns.Add("Porcentagem");

                foreach (Ocorrencia ocorrencia in lista)
                {

                    if (ocorrencia.LinhaDeContrato != null)
                    {
                        DataRow drOcorrencia = dtDashBoard.NewRow();
                        drOcorrencia["OcorrenciaId"] = ocorrencia.Id;
                        drOcorrencia["Nome"] = ocorrencia.Nome;
                        drOcorrencia["ImgSinal"] = "imagens/Sinal_Verde.jpg";
                        drOcorrencia["SinalID"] = "2";

                        drOcorrencia["Cliente"] = "";
                        if (ocorrencia.Cliente != null)
                        {
                            drOcorrencia["Cliente"] = ocorrencia.Cliente.NomeFantasia.ToString();
                            drOcorrencia["ClienteId"] = ocorrencia.Cliente.Id.ToString();

                            if (drOcorrencia["Cliente"].ToString() == "")
                                drOcorrencia["Cliente"] = ocorrencia.Cliente.Nome.ToString();

                            drOcorrencia["NomeDoCliente"] = ocorrencia.Cliente.Nome.ToString();
                        }

                        drOcorrencia["DtCriacao"] = ocorrencia.DataOrigem.Value.ToString("dd/MM/yyyy HH:mm");
                        drOcorrencia["DtSLA"] = ocorrencia.DataSLA.Value.ToString("dd/MM/yyyy HH:mm");
                        drOcorrencia["NroOcorrencia"] = ocorrencia.Numero;

                        if (ocorrencia.EmpresaExecutante != null)
                        {
                            if (ocorrencia.EmpresaExecutante.Nome != null)
                                drOcorrencia["EmpresaExecutante"] = ocorrencia.EmpresaExecutante.Nome.ToString();
                        }


                        var usuario = (new Domain.Servicos.RepositoryService()).Usuario.BuscarProprietario("incident", "incidentid", ocorrencia.Id);

                        if (usuario != null)
                        {
                            drOcorrencia["Proprietario"] = usuario.NomeCompleto.ToString();
                        }


                        drOcorrencia["Prioridade"] = ocorrencia.Prioridade;
                        drOcorrencia["DtEscalacao"] = ocorrencia.DataEscalacao;
                        drOcorrencia["OrdSinal"] = ocorrencia.DataSLA.Value.ToString("dd/MM/yyyy HH:mm");

                        if (DateTime.Now < ocorrencia.DataEscalacao)
                        {
                            drOcorrencia["ImgSinal"] = "imagens/Sinal_Verde.jpg";
                            drOcorrencia["SinalID"] = "2";
                            drOcorrencia["OrdSinal"] = "2" + ocorrencia.DataEscalacao.Value.ToString("yyyyMMddHH:mm");
                            TotalGreen++;
                        }
                        else if ((ocorrencia.DataEscalacao < DateTime.Now) && (DateTime.Now < ocorrencia.DataSLA))
                        {
                            drOcorrencia["ImgSinal"] = "imagens/Sinal_Amarelo.jpg";
                            drOcorrencia["SinalID"] = "1";
                            drOcorrencia["OrdSinal"] = "1" + ocorrencia.DataSLA.Value.ToString("yyyyMMddHH:mm");
                            TotalYellow++;

                        }
                        else if (DateTime.Now > ocorrencia.DataSLA)
                        {
                            drOcorrencia["ImgSinal"] = "imagens/Sinal_Vermelho.jpg";
                            drOcorrencia["SinalID"] = "0";
                            drOcorrencia["OrdSinal"] = "0" + ocorrencia.DataSLA.Value.ToString("yyyyMMddHH:mm");
                            TotalRed++;
                        }

                        double totalMinutos = ((DateTime)ocorrencia.DataSLA).Subtract((DateTime)ocorrencia.DataOrigem).TotalMinutes;

                        double totalNow = (DateTime.Now.Subtract((DateTime)ocorrencia.DataOrigem)).TotalMinutes;

                        double totalSla = (totalNow / totalMinutos) * 100;

                        drOcorrencia["Porcentagem"] = totalSla.ToString("0.00") + "%";

                        dtDashBoard.Rows.Add(drOcorrencia);
                    }
                }

                lbContLinha.Text = "Total de linhas: " + lista.Count.ToString();

                if (!IDnoCRM.HasRows)
                {
                    grafico_dashboard.Visible = false;
                    lbMensagem.Text = "Consulta não retornou registro para o proprietário: " + "<b>" + nmProprietario + "</b>";
                }

                dsOcorrencias.DataSetName = "Ocorrencias";
                dsOcorrencias.Tables.Add(dtDashBoard);
                dsOcorrencias.AcceptChanges();

                grid_ocorrencias.DataSource = dsOcorrencias.DefaultViewManager.DataSet;
                grid_ocorrencias.DataBind();

                grid_ocorrencias.Sort("OrdSinal", SortDirection.Ascending);
                PopulaGrid();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                if (sqlConnection.State != ConnectionState.Closed)
                {
                    sqlConnection.Close();
                }
            }
        }


    }
}
