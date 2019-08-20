<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashBoard.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.Pages.incident.DashBoard" %>

<%--<%@ Register assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>--%>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="refresh" content="120" />
    <title>.: Intelbras - DashBoard :.</title>
    <script type="text/javascript" src="/_static/_controls/util/util.js"></script>
    <script type="text/javascript" src="/_static/_common/scripts/global.js"></script>
    <script type="text/javascript">
        function AbrirOcorrencia(ocorrenciaID) {            
            window.open("/main.aspx?etn=incident&pagetype=entityrecord&id={" + ocorrenciaID + "}");
        }
        function AbrirCrmFormAccount(accountId) {
            window.open("/main.aspx?etn=account&pagetype=entityrecord&id={" + accountId + "}");
        }
    </script>
    <script type="text/javascript" src="/_static/_common/scripts/global.js"></script>
    <link rel="stylesheet" type="text/css" href="style/Master.css">
    
    <style>
      html, body, #container {
      width: 100%;
      height: 100%;
      margin: 0;
      padding: 0;
    }

      body {
        text-align: center;
      }
      p {
        display: block;
        width: 450px;
        margin: 2em auto;
        text-align: left;
      }            
      #grafico_dashboard{
          width:100%;
      }
      #grid_ocorrencias{
          width:100%
      }            
    </style>        
</head>
<body>        
    <form id="form1" runat="server">
        <div id="proprietario" style="text-align:left; margin-left:20px; margin-top:10px; width:100%; height:30px;">
            <label for="proprietario" class="label">Proprietário:</label>
            <asp:DropDownList ID="DropDownList1" AutoPostBack="true" OnSelectedIndexChanged="DropDownList" runat="server"></asp:DropDownList>
            <div id="contLinha" style="text-align:right;">
                <asp:Label ID="lbContLinha" runat="server" style="font-size:10px; margin-right:30px; margin-bottom:10px; position:relative;" Font-Bold="false" ></asp:Label>        
            </div>
        </div>

        
    <div id="div_body">
        <!-- Grafico com Numero de Ocorrencias -->
        <asp:Table runat="server" ID="grafico_dashboard">
            <asp:TableRow runat="server">
                <asp:TableCell runat="server" HorizontalAlign="Center" BackColor="Red" Font-Bold="true" ForeColor="White">Red</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center" BackColor="Yellow" Font-Bold="true">Yellow</asp:TableCell>
                <asp:TableCell runat="server" HorizontalAlign="Center" BackColor="Green" Font-Bold="true" ForeColor="White"> Green</asp:TableCell>
            </asp:TableRow>
        </asp:Table>        

        
        <div>
            <asp:GridView ID="grid_ocorrencias" runat="server" ShowFooter="True" DataKeyNames="OcorrenciaId"
                AutoGenerateColumns="False" EnableModelValidation="True" BackColor="LightGray" ForeColor="blue" Font-Size="14px"
                AlternatingRowStyle-BackColor="LightBlue" RowStyle-BackColor="White" AllowSorting="True"
                OnSorting="grid_ocorrencias_Sorting" OnRowCommand="grid_ocorrencias_RowCommand" OnRowDataBound="GridDataBound_RowDataBound"
                EnableViewState="true">
                <Columns>
                    <asp:ImageField HeaderText="!" DataImageUrlField="ImgSinal" ItemStyle-Height="29px" ItemStyle-Width="29px" ControlStyle-Height="29px" ControlStyle-Width="29px" HeaderStyle-Width="29px" />
                    <asp:BoundField HeaderText="Ordenacao" DataField="OrdSinal" Visible="false" ItemStyle-Height="58px" ItemStyle-CssClass="corAzul"/>
                    <asp:BoundField HeaderText="Id da Ocorrência" DataField="OcorrenciaId" Visible="false" ItemStyle-Height="58px" ItemStyle-CssClass="corAzul"/>
                    <asp:BoundField HeaderText="Nro da Ocorrência" DataField="NroOcorrencia" Visible="false" ItemStyle-Height="58px" ItemStyle-CssClass="corAzul"/>
                    <asp:BoundField HeaderText="Sinal Id" DataField="SinalID" Visible="false" ItemStyle-Height="58px" SortExpression="Nome" />
                    <asp:BoundField HeaderText="Nome" DataField="Nome" Visible="false" ItemStyle-Height="58px" SortExpression="Nome" />
                    <asp:BoundField HeaderText="Cliente" DataField="Cliente" Visible="false" ItemStyle-Height="58px" />                    
                    <asp:BoundField HeaderText="Data Criação" DataField="DtCriacao" Visible="false" ItemStyle-Height="58px" />                    
                    <asp:BoundField HeaderText="Data SLA" DataField="DtSLA" Visible="false" ItemStyle-Height="58px" />
                    <asp:BoundField HeaderText="ClienteId" DataField="Id do Cliente" Visible="false" ItemStyle-Height="58px" />

                    <asp:TemplateField HeaderText="Ocorrência" ItemStyle-Width="140px">
                        <ItemTemplate>
                        <a href="javascript:AbrirOcorrencia('<%# Eval("OcorrenciaID") %>');">
                            <asp:Label ID="lbNroOcorrencia" runat="server" class="label" Font-Bold="true" Text='<%# Bind("NroOcorrencia") %>'></asp:Label>
                        </a>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center"/>

                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Nome" SortExpression="Nome" Visible="false" ItemStyle-Width="140px">
                        <ItemTemplate>
                            <asp:Label ID="lbNome" runat="server" class="label" Text='<%# Bind("Nome") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Cliente">
                        <ItemTemplate>
                            <a href="javascript:AbrirCrmFormAccount('<%# Eval("ClienteId") %>');">
                                <asp:Label ID="lbNomeDoCliente" runat="server" class="label" Font-Bold="true" Style="text-align:left;" Text='<%# Bind("NomeDoCliente") %>'></asp:Label>
                            </a>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Empresa Executante">
                        <ItemTemplate>
                            <asp:Label ID="lbEmpresaExecutante" runat="server" class="label" Font-Bold="true" Style="text-align:left;" Text='<%# Bind("EmpresaExecutante") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" ForeColor="black"/>
                    </asp:TemplateField>                    
                    <asp:TemplateField HeaderText="Proprietário">
                        <ItemTemplate>
                            <asp:Label ID="lbProprietario" runat="server" class="label" Style="text-align:left;" Font-Bold="true" Text='<%# Bind("Proprietario") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" ForeColor="black"/>
                    </asp:TemplateField>                    
                    <asp:TemplateField HeaderText="Abertura" ItemStyle-Width="105px">
                        <ItemTemplate>
                            <asp:Label ID="lbDtCriacao" runat="server" class="label" Font-Bold="true" Text='<%# Bind("DtCriacao") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" ForeColor="black"/>
                    </asp:TemplateField>  
                    <asp:TemplateField HeaderText="Prioridade" >
                        <ItemTemplate>
                            <asp:Label ID="lbPrioridade" runat="server" style="float:none;" class="label centro_texto" Font-Bold="true"  Text='<%# Bind("Prioridade") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" ForeColor="black"/>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="SLA" ItemStyle-Width="105px">
                        <ItemTemplate>
                            <asp:Label ID="lbDtSLA" runat="server" class="label" Font-Bold="true" Text='<%# Bind("DtSLA") %>'></asp:Label>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" ForeColor="black"/>
                    </asp:TemplateField>
                    <asp:HyperLinkField Visible="false" Text="<img src='images/update-icon24.png' alt='Alterar Pedido' border=0 />"
                        DataNavigateUrlFields="Nome" DataNavigateUrlFormatString="CadastroDoPedido.aspx?CNPJ={0}">
                        <HeaderStyle Width="10px" CssClass="label" />
                        <ItemStyle Font-Bold="True" ForeColor="#0000CC" CssClass="label" HorizontalAlign="Center" />
                    </asp:HyperLinkField>
                </Columns>
            </asp:GridView>
        </div>
        <div id="mensagem" style="text-align:left;margin-left:20px;">
           <asp:Label ID="lbMensagem" runat="server" style="font-size:12px; position:relative; top:6px;" Font-Bold="false" ></asp:Label>
        </div>
    </div>
    </form>
</body>
</html>
