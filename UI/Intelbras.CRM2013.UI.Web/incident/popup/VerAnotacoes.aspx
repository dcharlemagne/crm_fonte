<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerAnotacoes.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.Pages.incident.popup.VerAnotacoes" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="../style/EscolherEndereco.css">
</head>
<body>
    <form id="form1" runat="server">
    <div id="divHeader">
        <div class="areaBotao">
            <asp:LinkButton runat="server" ID="btnFecharJanela" OnClientClick="window.close()"
                Text="Fechar Janela" /></div>
    </div>
    <div class="divPopup">
        <fieldset>
            <legend>Anotações</legend>
            <asp:Label runat="server" ID="lblMensagem" ForeColor="Red" Visible="false" />
            <asp:Repeater runat="server" ID="rtpAnotacoes">
                <ItemTemplate>
                    Assunto:
                    <%# DataBinder.Eval(Container.DataItem, "Assunto") %>
                    <br />
                </ItemTemplate>
            </asp:Repeater>
        </fieldset>
    </div>
    </form>
</body>
</html>
