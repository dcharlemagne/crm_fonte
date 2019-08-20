<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OS.aspx.cs" Inherits="Intelbras.CRM2013.UI.Web.Pages.incident.OS" EnableViewState="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
  <script type="text/javascript">
      jQuery(document).ready(function () {
          var alturaScroll = $('#txt_defeitoAlegado')[0].scrollHeight;
          var alturaCaixa = $('#txt_defeitoAlegado').height();
          if (alturaScroll > alturaCaixa) {
              $('#txt_defeitoAlegado').css('height', alturaScroll);
          }

          $('#txt_defeitoAlegado').on('keyup onpaste', function () {
              var alturaScroll = this.scrollHeight;
              var alturaCaixa = $(this).height();

              if (alturaScroll > (alturaCaixa + 10)) {
                  if (alturaScroll > 500) return;
                  $(this).css('height', alturaScroll);
              }
          });
      });
      
  </script>
    <title>Ordem de Serviço</title>
    <style type="text/css">
        body
        {
            width: 800px;
            font-family: Tahoma, Verdana, Arial;
            margin-left: 0px;
            margin-top: 0px;
        }
        
        /*** Id ***/
        #div_logo
        {
            float: left;
            color: Green;
            
            font-size: 40px;
            font-family: Bauhaus 93;
        }
        
        #div_titulo
        {
            float: left;
            margin-top: 10px;
            margin-left: 100px;
        }
        
        #panel_produtos
        {
            width: 97%;
            height: 198px;
            border: 1px solid black;
            margin: 4px 2px 4px 0px;
            background-color: White;
        }
        
        #txt_atividadeExecultada, #txt_obs, #txt_defeitoAlegado, #txt_produtosDoCliente
        {
            width: 97%;
        }
        
        #txt_Header
        {
            margin-left: 0px;
            margin-top: 0px;
            width: 800px;
            background-color: #d6e8ff;
        }
        
        /*** Classes ***/
        .table
        {
            width: 100%;
        }
        
        .titulo
        {
            text-align: center;
            color: black;
            font-weight: bold;
            font-size: 18px;
        }

        .img_satisfacao
         {
             margin-right: 3%;
	
         }
        
        .assinatura
        {
            margin-top: 35px;
            border-top: 1px solid #000000;
            font-weight: bold;
            text-align: center;
            font-size: 11px;
            margin-left: 3%;
            margin-right: 3%;
        }
        .assinatura_direita
        {
            float: right;
            width: 15%;
            margin-top: 0px;
        }
        .assinatura_esquerda
        {
            margin-top: 0px;
            float: left;
            width: 32%;
        }
        
        .direita
        {
            float: right;
            width: 50%;
        }
        .esquerda
        {
            float: left;
            width: 50%;
        }
        .centro
        {
            width: 100%;
        }
        
        .clear
        {
            clear: both;
        }
        
        .textBox
        {
            float: right;
            margin: 1px 20px 1px 0px;
            border: #6699cc 1px solid;
            color: #000000;
            font-size: 11px;
        }
        
        .direita .textBox, .esquerda .textBox
        {
            width: 60%;
            height: 15px;
        }
        
        .direita * .textBox, .esquerda * .textBox
        {
            width: 50%;
            height: 15px;
        }
        
        .centro .textBox
        {
            width: 80%;
            height: 15px;
        }
        
        .label
        {
            float: left;
            margin: 1px 1px 1px 0px;
            font-size: 11px;
            height: 13px;
        }
        
        .table
        {
            margin: 0px;
            color: #000000;
            font-size: 9px;
            border: 1px solid black;
        }
        
        .table thead
        {
        }
        
        .table th
        {
            background-color: #6699cc;
            text-align: left;
        }
        
        .table tr td
        {            
            height: 10px;
        }
        
        .table .tableCodigo
        {
            width: 10%;
        }
        
        .table .tableDescricao
        {
            width: 70%;
        }
        
        .table .tableColor1
        {
            background-color: #BFDCFF;
        }
        
        .table .tableColor2
        {
            background-color: #B3D5FF;
        }
        
        .divisoriaSessao
        {
            padding: 2px 0px 2px 0px;
            width: 100%;
            color: #000000;
            overflow: hidden;
            font-weight: bold;
            font-size: 11px;
            border-bottom: #96b3dd 1px solid;
        }
        
        .msg-obs
        {
            margin: 5px 20px 0px 5px;
            font-size: 10px;
        }
        
        .obs
        {
            background-color: #FFFFFF;
            width: 780px;
            height: 80px;
            margin: 4px 20px 4px 0px;
            border: #6699cc 1px solid;
            color: #000000;
            font-size: 11px;
        }
    </style>
</head>
<body>

    <form id="form1" runat="server">
    <div id="txt_Header">
    <div>
        <div id="div_logo">
            intelbras
            <!--<asp:Image ID="img_logo" runat="server" ImageUrl="imagens/logo.png" />-->
        </div>
        <div id="div_titulo" class="titulo">
            Ordem de Serviço
        </div>
        <div class="titulo">
            <asp:Image ID="img_qrcode" runat="server" Style="margin-right: 20px" margin-botton="10px" Height="100px" ImageAlign="Right" Width="100px"/>
        </div>
    </div>
        <!-- 
        <div class="divisoriaSessao clear">
            Informações</div>
            -->
        
        <div class="esquerda clear">
            <asp:Label ID="lb_numeroOcorrencia" runat="server" Text="Número da OS" class="label"></asp:Label>
            <asp:TextBox ID="txt_numeroOs" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:Label ID="lbl_rec" runat="server" Text="REC" class="label"></asp:Label>
            <asp:TextBox ID="txt_rec" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda clear">
            <asp:Label ID="lbl_tipoOcorrencia" runat="server" Text="Tipo da OS" class="label"></asp:Label>
            <asp:TextBox ID="txt_tipoOcorrencia" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:Label ID="lbl_criadoPor" runat="server" Text="Gerado Por" class="label"></asp:Label>
            <asp:TextBox ID="txt_criadoPor" runat="server" class="textBox"></asp:TextBox>
        </div>
        <!-- <div class="esquerda">
            <asp:Label ID="lbl_numeroNfFatura" runat="server" Text="Nr. NF Fatura" class="label"></asp:Label>
            <asp:TextBox ID="txt_numeroNfFatura" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:TextBox ID="txt_numeroNfRemessa" runat="server" class="textBox"></asp:TextBox>
            <asp:Label ID="lbl_numeroNfRemessa" runat="server" Text="Nr. NF Remessa" class="label"></asp:Label>
        </div> -->
        <div class="clear divisoriaSessao">
            Dados do Cliente</div>
        <div class="esquerda clear">
            <asp:Label ID="lbl_nomeCliente" runat="server" Text="Cliente" class="label"></asp:Label>
            <asp:TextBox ID="txt_nomeCliente" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:TextBox ID="txt_telefoneCliente" runat="server" class="textBox"></asp:TextBox>
            <asp:Label ID="Label10" runat="server" Text="Telefone" class="label"></asp:Label>
        </div>
        <div class="esquerda">
            <asp:Label ID="lbl_localDeIntelacaoCliente" runat="server" Text="Local de Instalação"
                class="label"></asp:Label>
            <asp:TextBox ID="txt_localDeIntelacaoCliente" runat="server" class="textBox"></asp:TextBox>
        </div>
          <div class="direita">
            <asp:Label ID="lbl_CEP" runat="server" Text="CEP"
                class="label"></asp:Label>
            <asp:TextBox ID="txt_CEP" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda">
            <asp:Label ID="lbl_ruaCliente" runat="server" Text="Endereço" class="label"></asp:Label>
            <asp:TextBox ID="txt_ruaCliente" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:Label ID="lbl_bairroCliente" runat="server" Text="Bairro" class="label"></asp:Label>
            <asp:TextBox ID="txt_bairroCliente" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda">
            <asp:Label ID="Label17" runat="server" Text="Label" class="label">Cidade</asp:Label>
            <asp:TextBox ID="txt_cidadeCliente" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:Label ID="Label18" runat="server" Text="Estado" class="label"></asp:Label>
            <asp:TextBox ID="txt_ufCliente" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="clear divisoriaSessao">
            Dados da Visita</div>
        <div class="esquerda clear">
            <asp:Label ID="lbl_nomeSolicitante" runat="server" Text="Solicitante" class="label"></asp:Label>
            <asp:TextBox ID="txt_nomeSolicitante" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:Label ID="lbl_telefoneSolicitante" runat="server" Text="Telefone do Solicitante"
                class="label"></asp:Label>
            <asp:TextBox ID="txt_telefoneSolicitante" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda clear">
            <asp:Label ID="lbl_contato" runat="server" Text="Contato da Visita" class="label"></asp:Label>
            <asp:TextBox ID="txt_contato" runat="server" class="textBox"></asp:TextBox>
        </div>        
        <div class="direita">
            <asp:Label ID="lbl_kilometragemPercorrida" runat="server" Text="Km Percorrida" class="label"></asp:Label>
            <asp:TextBox ID="txt_kilometragemPercorrida" runat="server" class="textBox"></asp:TextBox>
        </div>        
        <div class="esquerda">
            <asp:Label ID="Label5" runat="server" Text="Data Prevista para Visita" class="label"></asp:Label>
            <asp:TextBox ID="txt_dataPrevistaParaVisita" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda clear">
            <asp:Label ID="Label21" runat="server" Text="Data e Hora Início" class="label"></asp:Label>
            <asp:TextBox ID="txt_dataDaExecucao" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
            <asp:Label ID="Label22" runat="server" Text="Data e Hora Conclusão" class="label"></asp:Label>
            <asp:TextBox ID="txt_dataDeConclusao" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="clear divisoriaSessao">
            Defeito Alegado / Motivo</div>
        <asp:TextBox ID="txt_defeitoAlegado" Rows="7" runat="server" TextMode="MultiLine"
            Width="97%" class="textBox" Font-Bold="true">

            </asp:TextBox>
        <div class="clear divisoriaSessao">
            Equipamentos Instalados</div>
         <asp:TextBox ID="txt_equipamentosInstalados" Rows="16" runat="server" TextMode="MultiLine"
            Width="97.5%" class="textBox">
            </asp:TextBox>       
        <asp:Panel ID="panel_produtos" runat="server">
            <asp:GridView ID="tb_produtos" runat="server" CssClass="table" BackColor="White"
                CellPadding="3" EnableModelValidation="True" GridLines="Horizontal">
                <AlternatingRowStyle BackColor="#F7F7F7" />
                <FooterStyle BackColor="#B5C7DE" ForeColor="#4A3C8C" />
                <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#F7F7F7" />
                <PagerStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" HorizontalAlign="Right" />
                <RowStyle BackColor="#E7E7FF" ForeColor="#4A3C8C" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="#F7F7F7" />
            </asp:GridView>
        </asp:Panel>
        <asp:Panel runat="server" ID="panel_produtosDoCliente">
            <div class="divisoriaSessao" id="div_produtosDoCliente">
                Produtos do Cliente</div>
            <asp:TextBox ID="txt_produtosDoCliente" runat="server" TextMode="MultiLine" Rows="4"
                Width="97%" class="textBox"></asp:TextBox>
        </asp:Panel>
        <div class="divisoriaSessao">
            Dados da Empresa Executante</div>
        <div class="centro">
            <asp:Label ID="Label11" runat="server" Text="Nome da Empresa" class="label"></asp:Label>
            <asp:TextBox ID="txt_nomeEmpresaExecutante" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda clear">
            <asp:Label ID="lbl_nomeTecnicoResponsavel" runat="server" Text="Técnico Responsável" class="label"></asp:Label>
            <asp:TextBox ID="txt_nomeTecnicoResponsavel" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
        <div class="esquerda">
            <asp:Label ID="Label14" runat="server" Text="RG" class="label"></asp:Label>
            <asp:TextBox ID="txt_rgTecnicoResponsavel" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda">
            <!--<asp:Label ID="Label33" runat="server" Text="Telefone" class="label"></asp:Label>
            <asp:TextBox ID="txt_telefoneTecnicoResponsavel" runat="server" class="textBox"></asp:TextBox>-->
        </div>
        </div>
        <div class="esquerda clear">
            <asp:Label ID="Label13" runat="server" Text="Visita Executada por" class="label"></asp:Label>
            <asp:TextBox ID="txt_nomeTecnicoExecutante" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="direita">
        <div class="esquerda">
            <asp:Label ID="Label31" runat="server" Text="RG" class="label"></asp:Label>
            <asp:TextBox ID="txt_rgTecnicoExecultante" runat="server" class="textBox"></asp:TextBox>
        </div>
        <div class="esquerda">
            <!--<asp:Label ID="Label32" runat="server" Text="Telefone" class="label"></asp:Label>
            <asp:TextBox ID="txt_telefoneTecnicoExecultante" runat="server" class="textBox"></asp:TextBox>-->
        </div>
        </div>
        <div class="divisoriaSessao">
            Observações</div>
        <asp:TextBox ID="txt_obs" runat="server" TextMode="MultiLine" Rows="6" Style="width: 97%;"
            class="textBox"></asp:TextBox>
        <div class="clear divisoriaSessao">
            Atividade Executada</div>
        <asp:TextBox ID="txt_atividadeExecultada" runat="server" TextMode="MultiLine" Rows="9"
            Style="width: 97%; overflow: hidden;" class="textBox"></asp:TextBox>
        <div class="img_satisfacao">
             <asp:Image ID="img_satisfacaocliente" runat="server" ImageUrl="imagens/satisfacaoCliente.png" ImageAlign="Right" Width="601px"/>             
        </div>
        <div class="clear divisoriaSessao">
            Visto do Cliente</div>
        <div class="clear msg-obs">
            Declaramos para os devidos fins e direitos, que os serviços acima descritos foram por nós conferidos, atendendo nossas solicitações e necessidades.
        </div>
        <br class="clear" />
        <br class="clear" />
        <div class="assinatura_esquerda assinatura">
            Nome</div>
        <div class="assinatura_esquerda assinatura">
            Assinatura</div>
        <div class="assinatura_direita assinatura">
            Data</div>
        <div class="clear msg-obs">
            <asp:Label ID="lblMostraNF" runat="server" Text="A Nota Fiscal de cobrança do serviço deverá ser emitida e protocolada na Intelbras
	           até o dia 20 do mês corrente, caso contrário a mesma será devolvida." class="clear msg-obs"></asp:Label> 
        </div>
    </div>
    </form>
</body>
</html>
