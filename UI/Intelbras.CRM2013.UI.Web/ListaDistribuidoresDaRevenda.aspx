<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaDistribuidoresDaRevenda.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.ListaDistribuidoresDaRevenda" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <title>Intelbras | Revendas do Distribuidor</title>
        <link href="Styles/Site.css" rel="stylesheet" />
    </head>
    <body>
        <h1>Lista de Distribuidores da Revenda</h1>
        <form runat="server" id="form1" name="form1" method="post">
            <div style="float: left; margin-bottom: 30px;">
                <asp:Panel runat="server" ID="pnExtratos" Visible="true">
                    <asp:PlaceHolder ID="phFamilias" runat="server"></asp:PlaceHolder>
                </asp:Panel>
            </div>
        </form>
    </body>
</html>
