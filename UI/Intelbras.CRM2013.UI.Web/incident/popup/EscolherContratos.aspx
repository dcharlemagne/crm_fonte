<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EscolherContratos.aspx.cs"
    Inherits="Intelbras.CRM2013.UI.Web.Pages.incident.popup.EscolherContratos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../style/EscolherEndereco.css" />
    <script language="javascript" type="text/javascript">
        function RetornaValor(contratoGuid, nomeContrato) {
            if (window.opener != null) {
                window.opener.crmForm.RecebeContrato(contratoGuid, nomeContrato);
                window.close();
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divHeader">
        <div class="areaBotao">
            <asp:LinkButton runat="server" ID="btnFecharJanela" OnClientClick="window.close()"
                Text="Fechar Janela" /></div>
    </div>
    <div class="divPopup">
        <asp:Panel runat="server" ID="pnMensagem" Visible="false" class="areaMensagem">
            <span>
                <asp:Label ID="MensagemServidor" runat="server"></asp:Label>
            </span>
        </asp:Panel>
        <fieldset>
            <legend>Contratos</legend>
            <asp:GridView CssClass="gridCrm" runat="server" ID="gridContratos" Width="100%" AutoGenerateColumns="false"
                DataKeyNames="Id, NumeroContrato, Nome, InicioVigencia, FimRealVigencia" OnRowCommand="gridContratos_RowCommand"
                EmptyDataText="Nenhum contrato encontrado." OnRowDataBound="gridContratos_RowDataBound">
                <Columns>
                    <asp:CommandField ButtonType="Image" SelectImageUrl="/Intelbras/_Common/icon.aspx?objectTypeCode=10004&iconType=GridIcon&inProduction=0&cache=1"
                        ShowSelectButton="true" ControlStyle-CssClass="mouseOver" />
                    <asp:BoundField DataField="NumeroContrato" HeaderText="Número do Contrato" />
                    <asp:BoundField DataField="Nome" HeaderText="Contrato" />
                    <asp:BoundField DataField="TipoVigencia" HeaderText="Tipo de Vigencia" />
                    <asp:BoundField DataField="InicioVigencia" HeaderText="Início da Vigencia" />
                    <asp:BoundField DataField="FimRealVigencia" HeaderText="Fim da vigencia" />
                    <asp:BoundField DataField="Id" HeaderText="ContratoID" Visible="true" />
                </Columns>
            </asp:GridView>
        </fieldset>
    </div>
    </form>
</body>
</html>
