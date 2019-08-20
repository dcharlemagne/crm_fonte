<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ListaFamiliaComercial.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.ListaFamiliaComercial" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
        <title>Intelbras | Politica Comercial - Inserir Produtos</title>
        <link href="Styles/Site.css" rel="stylesheet" />

        <script lang="ja" type="text/javascript">
            function MarcaSelecao(chk) {

                var chkid = chk.id;

                var campo = "hdFamilias";

                if (chk.checked) {
                    IncluiProduto(campo, chk.value);
                }
                else {
                    ExcluiProduto(campo, chk.value)
                }
            }

        
            function SelecionaTodos(chk) {

                var hdProdutos = "hdFamilias";

                var totProd = document.getElementById("hdQtdTotal").value;
                for (i = 1; i <= totProd; i++) {

                    var idCampo = "chk_" + i.toString();

                    if (chk.checked) {
                        document.getElementById(idCampo).checked = true;
                        IncluiProduto(hdProdutos, document.getElementById(idCampo).value);
                    } else {
                        document.getElementById(idCampo).checked = false;
                        document.getElementById(hdProdutos).value = "";
                    }
                }

            }

            function IncluiProduto(campo, produtoID) {
                document.getElementById(campo).value = document.getElementById(campo).value + "|" + produtoID;
            }

            function ExcluiProduto(campo, produtoID) {

                var arrProdutos = document.getElementById(campo).value.split("|");
                var strProdutosAtuais = "";

                for (i = 1; i < arrProdutos.length; i++) {
                    if (arrProdutos[i].toString() != produtoID) {
                        strProdutosAtuais = strProdutosAtuais + "|" + arrProdutos[i].toString();
                    }
                }

                document.getElementById(campo).value = strProdutosAtuais;
            }

            function VerificaCodigo(campo) {
                
                var codi = parseInt(document.getElementById("txtIni").value);
                var codf = parseInt(document.getElementById("txtFin").value);

                if (codi == "") return;
                if (codf == "") return;

                if (codi > codf) {
                    alert("O código inicial deve ser menor que o código final");
                    document.getElementById(campo).value = "";
                    document.getElementById(campo).focus();
                }

            }

            function VerificaQuantidade(campo) {

                var qtdi = parseInt(document.getElementById("txtQtdIni").value);
                var qtdf = parseInt(document.getElementById("txtQtdFin").value);

                if (qtdi == "") return;
                if (qtdf == "") return;

                if (qtdi > qtdf) {
                    alert("A quantidade inicial deve ser menor que a quantidade final");
                    document.getElementById(campo).value = "";
                    document.getElementById(campo).focus();
                }

            }
         </script>
    </head>
    <body>
        <h1>Inserir Produtos na Política Comercial</h1>
        <form runat="server" id="form1" name="form1" method="post">
            <div style="float: left; margin-bottom: 30px;">
                <table cellspacing='0' style="width: 580px;">
                    <tr>
                        <th colspan="4">
                            Você pode inserir diversos Produtos na Política Comercial informando os Códigos da Família de Produtos.<br />
                            Abaixo informe o código inicial e final da Família de Produtos. </th>
                    </tr>
                    <tr>
                        <td>
                            Código Inicial:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtIni" onblur="VerificaCodigo('txtIni')"></asp:TextBox>
                        </td>
                        <td>
                            Código Final:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtFin" onblur="VerificaCodigo('txtFin')"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="botao">
                        <td colspan="4" style="text-align:right;" class="botao">
                            <asp:Button runat="server" ID="btnPesquisar" Text="Pesquisar" CssClass="submitbutton" OnClick="btnPesquisar_Click" />
                        </td>
                    </tr>
                </table>
                <asp:Panel runat="server" ID="pnExtratos" Visible="true">
                    <asp:PlaceHolder ID="phFamilias" runat="server"></asp:PlaceHolder>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnCampos" Visible="false">
                    <table cellspacing='0' style="width: 580px;">
                        <tr>
                            <td>
                                Quantidade Inicial:
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtQtdIni" onblur="VerificaQuantidade('txtQtdIni')"></asp:TextBox>
                            </td>
                            <td>
                                Quantidade Final:
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtQtdFin" onblur="VerificaQuantidade('txtQtdFin')"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Fator:
                            </td>
                            <td colspan="3">
                                <asp:TextBox runat="server" ID="txtFator"></asp:TextBox>
                            </td>
                        </tr>
                        <tr class="clear" >
                            <td colspan="4"style="text-align:right;">
                                <asp:Button runat="server" ID="btnSalvar" Text="Salvar" CssClass="submitbutton" OnClick="btnSalvar_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <asp:HiddenField runat="server" ID="hdQtdTotal" />
            <asp:HiddenField runat="server" ID="hdFamilias" />
        </form>
    </body>
</html>
