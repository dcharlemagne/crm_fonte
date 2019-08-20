<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsumoPriceProtection.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.ConsumoPriceProtection" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Consumo Price Protection</title>
    <link href="Styles/Site.css" rel="stylesheet" />
</head>
<body>
    <h1>Consumo Price Protection</h1>

    <br />
    <form runat="server" id="form1" name="form1" method="post">
        <asp:HiddenField runat="server" ID="NomeOrganizacao" Value="" />
        <asp:HiddenField runat="server" ID="GuidCliente" Value="" />

        <div style="float: left; margin-bottom: 30px;" >
            <asp:Label ID="mensagemRetorno" runat="server" Text=""></asp:Label>
            <table runat="server" id="tblRetornoProduto" width="100%"  >
            </table>
            <table>
                <tr>
                    <td style="width: 300px;"></td>
                    <td>
                        <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" />
                    </td>
                    <td>
                        <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar" OnClick="btnConfirmar_Click" />
                    </td>
                </tr>

            </table>
        </div>
    </form>

</body>
</html>
