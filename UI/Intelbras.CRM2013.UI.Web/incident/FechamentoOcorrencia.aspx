<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FechamentoOcorrencia.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.Pages.incident.FechamentoOcorrencia" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Fechamento de Ocorrência</title>
    <link rel="stylesheet" type="text/css" href="style/FechamentoOcorrenciaISOL.css" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.maskMoney.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        var quantidadeDespesa = 0;

        function mascara(o, f) {
            v_obj = o
            v_fun = f
            setTimeout("execmascara()", 1)
        }
        function execmascara() {
            v_obj.value = v_fun(v_obj.value)
        }


        function mdata(v) {
            v = v.replace(/\D/g, ""); //Remove tudo o que não é dígito
            v = v.replace(/^(\d{2})/, "$1/");
            v = v.replace(/^(\d{2})\/(\d{2})/, "$1/$2/");
            v = v.replace(/(\d{4})/, "$1 ");
            v = v.replace(/(\s)(\d{1})(\d{2})$/, "$1$2:$3");
            v = v.replace(/(\s)(\d{2})(\d{2})$/, "$1$2:$3");
            return v;
        }

        function criarDespesa() {
            var arr = [
                  { val: 0, text: '' },
                  { val: 1, text: 'Instalação' },
                  { val: 4, text: 'Manutenção' },
                  { val: 5, text: 'Configuração' },
                  { val: 6, text: 'Deslocamento (KM)' },
                  { val: 15, text: 'Refeição' },
                  { val: 16, text: 'Transporte' },
                  { val: 17, text: 'Hospedagem' },
                  { val: 18, text: 'Material Adicional' },
                  { val: 19, text: 'Improdutivo' }
                ];
            $('#tbody_despesas').append('<tr><td id="td_tipodespesas' + quantidadeDespesa + '"></td><td><input name="valordespesa' + quantidadeDespesa + '" class="textBox dinheiro" /></td></tr>');
            var sel = $('<select id="tipodespesa' + quantidadeDespesa + '" name="tipodespesa' + quantidadeDespesa + '" class="textBox" />').appendTo('#td_tipodespesas' + quantidadeDespesa);
            $(arr).each(function () {
                sel.append($("<option>").attr('value', JSON.stringify(this)).text(this.text));
            });
            $("input.dinheiro").maskMoney({ prefix: 'R$ ', allowNegative: true, thousands: '.', decimal: ',', affixesStay: false });
            quantidadeDespesa++;
        }
        $(document).ready(function () {
            criarDespesa();
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="divBody">
        <div id="div_logo">
            <asp:Image ID="img_logo" runat="server" ImageUrl="imagens/logo.png" /></div>
        <br style="clear: both;" />
        <table class="table" id="table_body">
            <tr>
                <td>
                    <div id="div_titulo" class="titulo">
                        Fechamento de Ocorrência
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="labelNumeroOcorrencia" runat="server" Text="Número da OS" class="label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="textNumeroOcorrencia" runat="server" class="textBox" Enabled="false"
                        ReadOnly="true" CssClass="textBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="labelDataInicio" runat="server" Text="Data e Hora Prevista para Visita"
                        class="label"></asp:Label>
                    <br />
                    <asp:TextBox ID="textDataInicio" runat="server" Enabled="false" ReadOnly="true" class="textBox"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="labelDataChegada" runat="server" Text="Data e Hora de Chegada do Técnico"
                        class="label"></asp:Label>
                    <br />
                    <asp:TextBox ID="textDataChegada" runat="server" class="textBox" MaxLength="16" onkeypress="mascara( this, mdata );"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_textDataChegada" runat="server"
                        ErrorMessage="Favor preencha esse campo." ControlToValidate="textDataChegada"
                        EnableClientScript="true" SetFocusOnError="true" Display="Dynamic" ValidationGroup="ValidationGroup_OK"
                        CssClass="Validation" />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="Data/Hora Inválida, Formato: dd/mm/aaaa hh:mm"
                        ValidationExpression="^((((([0-1]?\d)|(2[0-8]))\/((0?\d)|(1[0-2])))|(29\/((0?[1,3-9])|(1[0-2])))|(30\/((0?[1,3-9])|(1[0-2])))|(31\/((0?[13578])|(1[0-2]))))\/((19\d{2})|([2-9]\d{3}))|(29\/0?2\/(((([2468][048])|([3579][26]))00)|(((19)|([2-9]\d))(([2468]0)|([02468][48])|([13579][26]))))))\s(([01]?\d)|(2[0-3]))(:[0-5]?\d){1}$"
                        ControlToValidate="textDataChegada" Display="Dynamic" EnableClientScript="true"
                        SetFocusOnError="true" ValidationGroup="ValidationGroup_OK" CssClass="Validation" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="labelDataConclusao" runat="server" Text="Data e Hora de Saída do Técnico"
                        class="label"></asp:Label>
                    <br />
                    <asp:TextBox ID="textDataConclusao" runat="server" class="textBox" MaxLength="16"
                        onkeypress="mascara( this, mdata );"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_textDataConclusao" runat="server"
                        ErrorMessage="Favor preencha esse campo." ControlToValidate="textDataConclusao"
                        EnableClientScript="true" SetFocusOnError="true" Display="Dynamic" ValidationGroup="ValidationGroup_OK"
                        CssClass="Validation" />
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator_textDataConclusao"
                        runat="server" ErrorMessage="Data/Hora Inválida, Formato: dd/mm/aaaa hh:mm" ValidationExpression="^((((([0-1]?\d)|(2[0-8]))\/((0?\d)|(1[0-2])))|(29\/((0?[1,3-9])|(1[0-2])))|(30\/((0?[1,3-9])|(1[0-2])))|(31\/((0?[13578])|(1[0-2]))))\/((19\d{2})|([2-9]\d{3}))|(29\/0?2\/(((([2468][048])|([3579][26]))00)|(((19)|([2-9]\d))(([2468]0)|([02468][48])|([13579][26]))))))\s(([01]?\d)|(2[0-3]))(:[0-5]?\d){1}$"
                        ControlToValidate="textDataConclusao" Display="Dynamic" EnableClientScript="true"
                        SetFocusOnError="true" ValidationGroup="ValidationGroup_OK" CssClass="Validation" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="labelDetalhamento" runat="server" Text="Atividade Executada (Detalhada)"
                        class="label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="textDetalhamento" runat="server" CssClass="textBoxDetalhamento textBox"
                        TextMode="MultiLine"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator_textDetalhamento" runat="server"
                        ErrorMessage="Favor preencha esse campo." ControlToValidate="textDetalhamento"
                        EnableClientScript="true" Display="Dynamic" SetFocusOnError="true" ValidationGroup="ValidationGroup_OK"
                        CssClass="Validation" />
                </td>
            </tr>
            <tr>
                <td>
                <span class="label">Despesas</span>
                </td>
            </tr>
            <tr>
                <td>
                    <fieldset>
                        <table id="table_despesas" class="table">
                            <thead>
                                <tr>
                                    <th id="th_tipo_despesa" class="label">Tipo</th>
                                    <th id="th_valor_despesa"  class="label">Valor</th>
                                </tr>
                            </thead>
                            <tbody id="tbody_despesas"></tbody>
                            <tfoot>
                                <tr>
                                    <td></td>
                                    <td id="foot_despesas">
                                        <a id="adicionar_despesas" onclick="criarDespesa();">+</a>
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="labelAnexo" runat="server" Text="Anexo" class="label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:FileUpload ID="FileUploadAnexo" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label_Message" runat="server" />
                    <asp:Button ID="ButtonOk" runat="server" Text="  OK  " CssClass="btnOk" ValidationGroup="ValidationGroup_OK"
                        OnClick="buttonOK_Click" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
