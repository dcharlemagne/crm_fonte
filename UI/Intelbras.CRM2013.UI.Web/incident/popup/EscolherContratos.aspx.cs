using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Intelbras.CRM2013.UI.Web.Pages.incident.popup
{
    public partial class EscolherContratos : System.Web.UI.Page
    {
        private ClienteParticipante ClienteParticipante
        {
            get
            {
                ClienteParticipante _participante = null;
                if (ViewState["ClienteParticipante"] != null)
                    _participante = (ClienteParticipante)ViewState["ClienteParticipante"];
                else if (this.Cliente != null)
                {
                    //_participante = this.ObterClienteParticipante();
                    ViewState["ClienteParticipante"] = _participante;
                }

                return _participante;
            }
        }

        private Guid ClienteId
        {
            get
            {
                try
                {
                    return new Guid(Request.QueryString["ClienteId"]);
                }
                catch
                {
                    return Guid.Empty;
                }
            }
        }

        private CRM2013.Domain.Model.Conta _cliente = null;
        private CRM2013.Domain.Model.Conta Cliente
        {
            get
            {
                if (_cliente == null)
                {
                    if (ClienteId != Guid.Empty)
                    {
                        _cliente = new CRM2013.Domain.Servicos.RepositoryService().Conta.Retrieve(ClienteId);
                    }
                }
                return _cliente;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                if (this.Cliente != null) this.ListarContratos();
                else this.ExecutaScript("alert('Os parâmetros não foram carregados corretamente. Contate o administrador do sistema.');", "alerta");
        }

        protected void ListarContratos()
        {
            this.gridContratos.DataSource = new CRM2013.Domain.Servicos.RepositoryService().ClienteParticipante.ListarPor(this.Cliente);//, new List<StatusDoContrato>() { StatusDoContrato.Ativo, StatusDoContrato.Expirado });
            this.gridContratos.DataBind();
            this.gridContratos.Columns[6].Visible = false;
        }
        
        protected void gridContratos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            var contratoId = this.gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Id"];
            var numeroContrato = this.gridContratos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Nome"];

            var scriptRetorno = string.Format("RetornaValor('{0}', '{1}');", contratoId, numeroContrato);

            this.ExecutaScript(scriptRetorno, "retorno");
        }

        private void ExecutaScript(string script, string key)
        {
            ClientScript.RegisterClientScriptBlock(this.Page.GetType(), key, script, true);
        }

        protected void gridContratos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                switch (e.Row.Cells[3].Text)
                {
                    case "Cliente":
                        var _participante = this.ObterClienteParticipante(new Guid(e.Row.Cells[6].Text));

                        e.Row.Cells[3].Text = "Por Cliente";
                        if (_participante != null)
                        {
                            e.Row.Cells[4].Text = _participante.InicioVigencia.HasValue ? _participante.InicioVigencia.Value.ToString("dd/MM/yyyy") : e.Row.Cells[4].Text = string.Empty;
                            e.Row.Cells[5].Text = _participante.FimVigencia.HasValue ? _participante.FimVigencia.Value.ToString("dd/MM/yyyy") : e.Row.Cells[5].Text = string.Empty;
                        }
                        break;
                    case "Contrato":
                        e.Row.Cells[3].Text = "Por Contrato";
                        try { if (!String.IsNullOrEmpty(e.Row.Cells[4].Text)) e.Row.Cells[4].Text = Convert.ToDateTime(e.Row.Cells[4].Text).ToShortDateString(); }
                        catch { }
                        try { if (!String.IsNullOrEmpty(e.Row.Cells[5].Text)) e.Row.Cells[5].Text = Convert.ToDateTime(e.Row.Cells[5].Text).ToShortDateString(); }
                        catch { }
                        break;
                    case "Endereco":
                        e.Row.Cells[3].Text = "Por Endereço";
                        e.Row.Cells[4].Text = string.Empty;
                        e.Row.Cells[5].Text = string.Empty;
                        break;
                }
        }

        private string RetornaTipoDeVigencia(string vigencia)
        {
            string strVigencia = string.Empty;
            int intVigencia = 0;
            if (int.TryParse(vigencia.Substring(0, 1), out intVigencia))
                switch ((TipoDeVigencia)intVigencia)
                {
                    case TipoDeVigencia.Cliente: strVigencia = "Por Cliente"; break;
                    case TipoDeVigencia.Contrato: strVigencia = "Por Contrato"; break;
                    case TipoDeVigencia.Endereco: strVigencia = "Por Endereço"; break;
                }

            return strVigencia;
        }

        internal ClienteParticipante ObterClienteParticipante(Guid contratoId)
        {
            return new CRM2013.Domain.Servicos.RepositoryService().ClienteParticipante.ObterPor(this.Cliente.Id, contratoId);
        }
    }
}