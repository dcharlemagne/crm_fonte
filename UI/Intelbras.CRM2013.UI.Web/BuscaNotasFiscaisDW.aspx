<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuscaNotasFiscaisDW.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.BuscaNotasFiscaisDW" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <title>Intelbras | Notas Fiscais usadas para cálculo de Benefícios</title>
        <link href="Styles/Site.css" rel="stylesheet" />
    </head>
    <body>
        <h1>Notas Fiscais usadas para cálculo de Benefícios</h1>

        <br />
        <form runat="server" id="form1" name="form1" method="post">
            <asp:HiddenField runat="server" ID="NomeOrganizacao" Value="" />
            <asp:HiddenField runat="server" ID="GuidCliente" Value="" />
            <asp:HiddenField runat="server" ID="ParticipantePrograma" Value="" />

            <div style="float: left; margin-bottom: 30px;">
                <asp:Label ID="mensagemRetorno" runat="server" Text=""></asp:Label>
                <table runat="server" id="tblNF" width="100%">
                    <tr class="cabecalho">
                        <td id="cabNumNF" class="cabecalho" style="border-right:0.5px;padding:1px;">Núm. NF</td>
                        <td id="cabSerie" class="cabecalho" style="border-right:0.5px;padding:1px;">Série</td>
                        <td id="cabDtEmissao" class="cabecalho" style="border-right:0.5px;padding:1px;">Data Emissão</td>
                        <td id="cabCnpj" class="cabecalho" style="border-right:0.5px;padding:1px;">CNPJ</td>
                        <td id="cabFilial" class="cabecalho" style="border-right:0.5px;padding:1px;">Filial</td>
                        <td id="cabVlrComImposto" class="cabecalho" style="border-right:0.5px;padding:1px;">Valor com Imposto</td>
                        <td id="cabVlrIPI" class="cabecalho" style="border-right:0.5px;padding:1px;">Valor IPI</td>
                        <td id="cabVlrST" class="cabecalho" style="border-right:0.5px;padding:1px;">Valor ST</td>
                        <td id="cabVlrSemImposto" class="cabecalho" style="border-right:0.5px;padding:1px;">Valor Sem Imposto</td>
                        <td id="cabRepresentante" class="cabecalho" style="border-right:0.5px;padding:1px;">Representante</td>
                    </tr>
                </table>


                <asp:Panel runat="server" ID="pnProdutos" Visible="true">
                    <asp:PlaceHolder ID="phNF" runat="server"></asp:PlaceHolder>
                </asp:Panel>
            </div>
        </form>
    </body>
</html>
