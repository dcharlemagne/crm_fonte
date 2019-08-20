using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Intelbras.CRM2013.UI.Web.Pages.incident.popup
{
    public partial class VerAnotacoes : System.Web.UI.Page
    {
        private Guid? ObjectId
        {
            get
            {
                if (!String.IsNullOrEmpty(Request.QueryString["ObjId"]))
                    return new Guid(Request.QueryString["ObjId"]);
                else
                    return null;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                if (this.ObjectId.HasValue)
                    CarregarAnotacoes();
        }

        protected void CarregarAnotacoes()
        {
            var anotacoes = new Domain.Servicos.RepositoryService().Anexo.ListarPor(this.ObjectId.Value, false);

            if (anotacoes.Count > 0) this.BindGrid(anotacoes);
            else
            {
                this.lblMensagem.Visible = true;
                this.lblMensagem.Text = "Nenhum registro encontrado.";
                this.rtpAnotacoes.Visible = false;
            }
        }

        protected void BindGrid(object datasource)
        {
            this.rtpAnotacoes.DataSource = datasource;
            this.rtpAnotacoes.DataBind();
        }
    }
}