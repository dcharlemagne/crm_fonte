<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuscaProdutosPortfolio.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.BuscaProdutosPortfolio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <title>Intelbras | Buscar Produtos Portfolio</title>
    <link href="Styles/Site.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/json2.js"></script>
    <script type="text/javascript" src="Scripts/uteis.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.4.2.min.js"></script>
    <script type="text/javascript">
        $().ready(function () {
            var flagLoop = false;
            function ativarCampos(tipo) {
                if (tipo == false) {
                    $('#btnListar').attr('disabled', "disabled");
                    $('#btnGerarTabela').attr('disabled', "disabled");
                    $('#gerarPSD').attr('disabled', "disabled");
                    $('#gerarPP').attr('disabled', "disabled");
                    $('#gerarPSCF').attr('disabled', "disabled");
                }
                else {
                    $('#btnListar').attr('disabled', "");
                    $('#btnGerarTabela').attr('disabled', "");
                    $('#gerarPSD').attr('disabled', "");
                    $('#gerarPP').attr('disabled', "");
                    $('#gerarPSCF').attr('disabled', "");
                }
            }

            function adicionarLinha(registro) {
                var botao;
                switch (registro["status"]) {
                    case "5 - Tabela Gerada":
                        botao = "<button type=\"button\" onClick=\"location.href = '" + registro["url"] + "'\">Baixar Planilha</button>";
                        break;
                    case "Erro Processamento Preços":
                    case "Erro de Processamento":
                        botao = '<a href="javascript:alert(\'' + registro["mensagem"] + '\');"><button type=\"button\">Log do erro</button></a>';
                        break;
                    default:
                        botao = "";
                        break;
                }
                var linha = "<tr class=\"registro\"><td class=\"registro canal\" >" + $("#nomeCliente").text() + "</td><td class=\"registro data\">" + registro["data"] + "</td><td class=\"registro statusProcessamento\">" + registro["status"] + "</td><td class=\"registro download\">" + botao + "</td></tr>";
                $("#tblHistorico").append(linha);
            }

            function formatarNumero(n) {
                return String(n).replace(".", ",").replace(".", ",");
            }
            function limparPlanilha() {
                var tabelaNova = "<tr class=\"cabecalho\"><td id=\"cabCanal\" class=\"cabecalho\" style=\"border-right:0.5px;padding:1px;\">Canal</td><td id=\"cabData\" class=\"cabecalho\" style=\"border-right:0.5px;padding:1px;\">Data e Hora da Criação</td><td id=\"cabProcessamento\" class=\"cabecalho\" style=\"border-right:0.5px;padding:1px;\">Status do Processamento</td><td id=\"cabDownload\" class=\"cabecalho\" style=\"border-right:0.5px;padding:1px;\">Download Planilha</td></tr>";
                $("table#tblHistorico").empty();
                $("table#tblHistorico").append(tabelaNova);
            }
            $("#btnGerarTabela").click(function () {
                $("#carregando").toggle("slow");
                ativarCampos(false);
                parametros = {
                    organizationName: $("#NomeOrganizacao").val(),
                    guidCliente: $("#GuidCliente").val(),
                    gerarPSD: $("#gerarPSD").attr("checked"),
                    gerarPP: $("#gerarPP").attr("checked"),
                    gerarPSCF: $("#gerarPSCF").attr("checked"),
                };
                CallPageMethod("GerarNovaTabela", parametros, function (sucesso, res) {
                    if (sucesso) {
                        $("#carregando").toggle("slow");
                        if (res == "null") {
                            ativarCampos(true);
                            alert("Erro ao Gerar a planilha,tente novamente mais tarde");
                        } else {
                            $.each(JSON.parse(res), function (name, value) {
                                if (name == "resultado" && value == true) {
                                    $("#btnListar").click();
                                }
                                else {
                                    if (name == "mensagem")
                                        alert(value);
                                }
                            });
                            //sucesso
                            ativarCampos(true);
                            limparPlanilha();
                        }
                    } else {
                        ativarCampos(true);
                        $("#carregando").toggle("slow");
                        alert("Erro ao Gerar a planilha,tente novamente mais tarde");
                    }

                });
            });
            $("#btnListar").click(function () {
                $("#carregando").toggle("slow");
                ativarCampos(false);
                parametros = {
                    organizationName: $("#NomeOrganizacao").val(),
                    guidCliente: $("#GuidCliente").val(),
                };
                CallPageMethod("ListarTabelas", parametros, function (sucesso, res) {
                    if (sucesso) {
                        $("#carregando").toggle("slow");
                        if (res == "null") {
                            ativarCampos(true);
                            alert("Erro ao Gerar a planilha,tente novamente mais tarde");
                        }
                        else if (res == "Nenhum resultado encontrado") {
                            ativarCampos(true);
                            alert(res);
                        }
                        else if(res.indexOf("ERRO |") > -1){
                            ativarCampos(true);
                            alert(res);
                        }
                        else {
                            try {
                                $.each(JSON.parse(res), function (name, value) {
                                    if (name != null && name == "resposta" && value != null) {
                                        limparPlanilha();
                                        $.each(this, function (nome, valor) {
                                            adicionarLinha(valor);
                                        });
                                    }
                                    else {
                                        alert("Erro ao Gerar a planilha,tente novamente mais tarde");
                                    }
                                });
                            }
                            catch (e) {
                                alert("Ocorreu um erro na listagem das tabelas de preço.");
                            }
                            //sucesso
                            ativarCampos(true);
                            if (flagLoop == false) {
                                setInterval(function () { $("#btnListar").click() }, 30000);
                                flagLoop = true;
                            }
                        }
                    } else {
                        ativarCampos(true);
                        $("#carregando").toggle("slow");
                        alert("Erro ao Gerar a planilha,tente novamente mais tarde");
                    }

                });
            });
        });
    </script>

    <style type="text/css">
        #testekskadla {
            width: 20px;
        }
    </style>

</head>
<body>
    <h1>Produto</h1>
    <h3>Razão Social:<asp:Label ID="nomeCliente" runat="server" Text=""></asp:Label></h3>
    <h3>Código:<asp:Label ID="CodigoCliente" runat="server" Text=""></asp:Label></h3>
    <br />
    <form runat="server" id="form1" name="form1" method="post">
        <asp:HiddenField runat="server" ID="NomeOrganizacao" Value="" />
        <asp:HiddenField runat="server" ID="GuidCliente" Value="" />
        <div style="float: left; margin-bottom: 30px;">
            <input id="btnListar" type="button" style="padding: 10px;" value="Listar Tabelas de Preço" />
            <input id="btnGerarTabela" type="button" style="padding: 10px;" value="Gerar Nova Tabela" />
            <div>
                <label>Gerar PSD</label>
                <input id="gerarPSD" type="checkbox" title="Selecionar opção para gerar planilha com os valores PSD" />
                <label>Gerar PP</label>
                <input id="gerarPP" type="checkbox" title="Selecionar opção para gerar planilha com os valores PP" />
                <label>Gerar PSCF</label>
                <input id="gerarPSCF" type="checkbox" title="Selecionar opção para gerar planilha com os valores PSCF" />
            </div>
            <br />
            <div id="carregando" style="display: none;">Carregando Dados<img src="Styles/ajax-loader.gif" /></div>
            <asp:Label ID="mensagemRetorno" runat="server" Text=""></asp:Label>
            <table id="tblHistorico" width="100%">
                <tr class="cabecalho">
                    <td id="cabCanal" class="cabecalho" style="border-right: 0.5px; padding: 1px;">Canal</td>
                    <td id="cabData" class="cabecalho" style="border-right: 0.5px; padding: 1px;">Data e Hora da Criação</td>
                    <td id="cabProcessamento" class="cabecalho" style="border-right: 0.5px; padding: 1px;">Status do Processamento</td>
                    <td id="cabDownload" class="cabecalho" style="border-right: 0.5px; padding: 1px;">Download Planilha</td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
