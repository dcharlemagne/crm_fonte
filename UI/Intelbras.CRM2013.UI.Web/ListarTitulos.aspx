<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListarTitulos.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.ListarTitulos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Listar Titulos da Solicitação</title>
    <link href="Styles/Site.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:GridView ID="GridView_titulos" runat="server" AutoGenerateColumns="false" EmptyDataText="(CRM) Nenhum titulo foi localizado.">
                <Columns>
                    <asp:BoundField HeaderText="Canal" DataField="Canal" />
                    <asp:BoundField HeaderText="Estabelecimento" DataField="Estabelecimento" />
                    <asp:BoundField HeaderText="Série" DataField="NumeroSerie" />
                    <asp:BoundField HeaderText="Número do Titulo" DataField="NumeroTitulo" />
                    <asp:BoundField HeaderText="Parcela" DataField="Parcela" />
                    <asp:BoundField HeaderText="Data do Vencimento" DataField="DataVencimento" />
                    <asp:BoundField HeaderText="Valor Original" DataField="ValorOriginal" />
                    <asp:BoundField HeaderText="Valor Abater" DataField="ValorAbater" />
                    <asp:BoundField HeaderText="Saldo" DataField="Saldo" />
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
