using Intelbras.CRM2013.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Intelbras.CRM2013.UI.Web.Pages.incident.popup
{
    public partial class EscolherEndereco : System.Web.UI.Page
    {
        private Guid? ClienteId
        {
            get
            {
                if (!String.IsNullOrEmpty(Request.QueryString["ClienteId"]))
                    return new Guid(Request.QueryString["ClienteId"]);
                else
                    return null;
            }
        }
        private Guid? ContratoId
        {
            get
            {
                if (!String.IsNullOrEmpty(Request.QueryString["ContratoId"]))
                    return new Guid(Request.QueryString["ContratoId"]);
                else
                    return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                this.IniciarTela();
        }

        protected void IniciarTela()
        {
            if (this.ClienteId.HasValue && this.ContratoId.HasValue)
                this.BuscarEnderecosParticipantes(this.ContratoId.Value, this.ClienteId.Value);
            else if (this.ClienteId.HasValue)
                this.BuscarMaisEnderecos(this.ClienteId.Value);
            else
                this.ExecutaScript("alert('Os parâmetros não foram carregados corretamente. Contate o administrador do sistema.');", "alerta");
        }

        protected void BindGrid(object dataSource)
        {
            this.gridEnderecos.DataSource = dataSource;
            this.gridEnderecos.DataBind();
        }

        protected void BuscarEnderecosParticipantes(Guid contratoId, Guid clienteId)
        {
            var contrato = new Contrato();
            contrato.Id = contratoId;

            var cliente = new Conta();
            cliente.Id = clienteId;

            var enderecos = contrato.ObterParticipantesPor(contrato, cliente);

            this.BindGrid(enderecos);
        }

        protected void BuscarMaisEnderecos(Guid enderecoId)
        {
            var cliente = new Conta();
            cliente.Id = enderecoId;

            var enderecos = new Domain.Servicos.RepositoryService().Endereco.ObterTodosOsEnderecosPor(cliente);
            this.BindGrid(enderecos);
        }

        protected void gridEnderecos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                var apelido = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Nome"];
                var logradouro = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Logradouro"];
                var bairro = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Bairro"];
                var cep = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Cep"];
                var cidade = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Cidade"];
                var uf = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Uf"];
                var pais = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Pais"];
                var localidadeid = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["CodigoDaLocalidade"];
                var enderecoId = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Id"];
                var localidade = this.gridEnderecos.DataKeys[Convert.ToInt32(e.CommandArgument)]["Localidade"];


                var scriptRetorno = string.Format("RetornaValor('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", RemoveCaracteresQueNaoPodem(enderecoId), RemoveCaracteresQueNaoPodem(logradouro), RemoveCaracteresQueNaoPodem(bairro), RemoveCaracteresQueNaoPodem(cidade), RemoveCaracteresQueNaoPodem(uf), RemoveCaracteresQueNaoPodem(localidadeid), RemoveCaracteresQueNaoPodem(apelido), RemoveCaracteresQueNaoPodem(localidade));

                this.ExecutaScript(scriptRetorno, "retorno");
            }
            catch (Exception ex) { Mensagem("Não foi possível concluir a operação! Tente novamente, caso o erro persista contate a Intelbras.", ex); }
        }

        private string RemoveCaracteresQueNaoPodem(object value)
        {
            if (value != null)
            {
                var str = value.ToString().Replace("'", "");
                return str;
            }
            return string.Empty;
        }
        private void ExecutaScript(string script, string key)
        {
            ClientScript.RegisterClientScriptBlock(this.Page.GetType(), key, script, true);
        }


        private void Mensagem(string mensagem)
        {
            this.pnMensagem.Visible = true;
            this.MensagemServidor.Text = mensagem;
        }

        private void Mensagem(string mensagem, Exception ex)
        {
            Mensagem(mensagem);
        }
    }
}