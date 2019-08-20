<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EscolherEndereco.aspx.cs"
    Inherits="Intelbras.CRM2013.UI.Web.Pages.incident.popup.EscolherEndereco" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../style/EscolherEndereco.css">
    <script type="text/javascript">

        function SairDoCampo() {
            document.getElementById('divWait').style.visibility = 'visible';
            setTimeout('EscondeLoading()', 60000);
            return;
        }

        function EscondeLoading() {
            document.getElementById('divWait').style.visibility = 'hidden';
            return;
        }

        function RetornaValor(enderecoId, rua, bairro, cidade, uf, localidadeid, apelido, localidade) {
            if (window.opener != null) {
                //alert(localidade);
                window.opener.crmForm.RecebeEndereco(enderecoId, rua, bairro, cidade, uf, localidadeid, apelido, localidade);
                window.close();
            }
        }
    </script>
</head>
<body>
  <form id="form1" runat="server" onsubmit="SairDoCampo();return true;">

    <div id="divWait" style="position: absolute; top: 0; left: 0; width: 100%; height: 100%;
        background-image: url('images/transparencia.gif'); visibility: hidden;"><br /><br /><br />
        <table width="500" style="background-color:#cccccc;" align="center" border="1">
            <tr>
                <td align="center">
                    <br /><br /><br />
                    <img src="../imagens/loading.gif" alt="Aguarde..." />
                    <br />
                    <font face="Arial" size="2"><b>Aguarde...</b></font>
                    <br /><br /><br />
                </td>
            </tr>
        </table>
    </div>

    <div id="divHeader">
        <div class="areaBotao">
            <asp:LinkButton runat="server" ID="btnFecharJanela" OnClientClick="window.close()"
                Text="Fechar Janela" /></div>
    </div>
    <asp:Panel runat="server" ID="pnMensagem" Visible="false" class="areaMensagem">
        <span>
            <asp:Label ID="MensagemServidor" CssClass="erro" runat="server"></asp:Label>
        </span>
    </asp:Panel>
    <div class="divPopup">
        <fieldset>
            <legend>Endereços do Cliente </legend>
            <asp:GridView CssClass="gridCrm" runat="server" ID="gridEnderecos" Width="100%" AutoGenerateColumns="false"
                DataKeyNames="Id,Nome,Cidade,Uf,Bairro,Logradouro,Cep,Pais,CodigoDaLocalidade,Localidade"
                OnRowCommand="gridEnderecos_RowCommand" EmptyDataText="Nenhum endereço encontrado.">
                <Columns>
                    <asp:CommandField ButtonType="Image" SelectImageUrl="/Intelbras/_Common/icon.aspx?objectTypeCode=10004&iconType=GridIcon&inProduction=0&cache=1"
                        ShowSelectButton="true" ControlStyle-CssClass="mouseOver" />
                    <asp:BoundField DataField="Nome" HeaderText="Código do Endereço" ItemStyle-CssClass="coluna01" />
                    <asp:BoundField DataField="Logradouro" HeaderText="Endereço" />
                    <asp:BoundField DataField="Bairro" HeaderText="Bairro" />
                    <asp:BoundField DataField="Cep" HeaderText="Cep" />
                    <asp:BoundField DataField="Cidade" HeaderText="Cidade" />
                    <asp:BoundField DataField="Uf" HeaderText="Uf" />
                    <asp:BoundField DataField="Pais" HeaderText="País" />
                </Columns>
            </asp:GridView>
        </fieldset>
    </div>
    </form>
</body>
</html>
