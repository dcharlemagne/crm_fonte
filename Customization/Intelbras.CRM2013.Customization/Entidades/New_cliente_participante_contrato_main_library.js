function IFRAME_Endereco_Disponivel_onload() {

}
//function Form_onsave() {
//	/**********************************
//    Ações do OnSave.
//    **********************************/
//
//	var clienteId = null;
//	var contratoId = null;
//
//	if (Xrm.Page.getAttribute("new_clienteid") != null)
//		clienteId = Xrm.Page.getAttribute("new_clienteid").getValue()[0].id;
//
//	if (Xrm.Page.getAttribute("new_contratoid").getValue() != null && Xrm.Page.getAttribute("new_contratoid").getValue() != undefined)
//		contratoId = Xrm.Page.getAttribute("new_contratoid").getValue()[0].id;
//
//	/*
//    Autor: Filipe de Campos Cavalcante
//    Data: 16/03/2011
//    Solicitação: 
//        Ao salvar o cliente participante do realizar uma verificação se os endereços selecionados estão com
//        a "Localidade" preenchida corretamente.
//        Caso não estejam mandar uma pergunta ao usuário se deseja continuar a operação assim mesmo. 
//        Se escolher "Sim", então apagar os endereços cuja Localidade não esteja preenchido; 
//        Se escolher "Não" entao manter a página aberta para que o usuário corrida os endereços
//    */
//
//	function VerificarEnderecos() {
//		//alert('to aqui');
//		var count = crmForm.BuscarEnderecosParticipantes(contratoId, clienteId);
//		//alert(count);
//		if (count != 0) {
//			var continuar = confirm('Existem endereços associados que não possuem localidade definida. Deseja continuar assim mesmo? Se continuar, estes endereços serão descartados');
//			if (continuar) {
//				event.returnValue = true;
//				//alert('aqui vem o processo de eliminação dos endereços');
//				crmForm.ExcluirEnderecosSemLocalidade(contratoId, clienteId);
//			}
//			else {
//				event.returnValue = false;
//			}
//		}
//		else {
//			event.returnValue = true;
//		}
//	}
//
//	//VerificarEnderecos();
//}
function Form_onload() {
    var FormularioEstaEmModoDeCriacao = Xrm.Page.ui.getFormType() == 1;

    /**********************************
    Data:      
    Autor:     
    Descrição: Recarregar Grids
    **********************************/

    crmForm.RecarregarGrids = function (clientID) {

        CarregaGridEnderecos(clientID);
    }

    function CarregaGridEnderecos(clienteID) {
        var urlDomain = window.location.hostname;
        var urlDisponiveis = '#';

        var guidLocal = "";
        if (Xrm.Page.getAttribute("new_clienteid").getValue() != null)
            guidLocal = Xrm.Page.data.entity.getId();

        var iframeDisponiveis = document.getElementById('IFRAME_Endereco_Disponivel');

        if (clienteID != null)
            urlDisponiveis = "/" + Xrm.Page.context.getOrgUniqueName() + "/sfa/accts/areas.aspx?oId=" + clienteID + "&oType=1&security=852023&tabSet=areaAddresses";

        if (iframeDisponiveis != null) iframeDisponiveis.src = urlDisponiveis;
    }

    /**********************************
    Data:      
    Autor:     
    Descrição: Notification
    **********************************/

    function Notification(msg) {
        Xrm.Page.ui.setFormNotification(msg, 'INFO', 'notificationid');
    }

    /**********************************
    Data:        26/04/2011
    Autor:       Cleto May
    Descrição:   Pesquisa o cliente pelo id retornando nome fantasia e código
    **********************************/

    crmForm.PesquisaClientePor = function (clienteId) {

        //Configuração do serviço web
        Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "ObterClientePor");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("clienteId", clienteId);

        //Execução do serviço web
        var retorno = Util.funcao.Execute();

        //Tratamento do retorno
        if (retorno["Success"] == true) {
            var data = retorno['ReturnValue'];
            Xrm.Page.getAttribute("new_name").setValue($(data).find("NomeFantasia").text());
            Xrm.Page.getAttribute("new_codigo_cliente").setValue($(data).find("CodigoMatriz").text());
        }
        else {
            Xrm.Page.ui.setFormNotification('Não foi possível concluir a operação solicitada.', 'ERROR');
            Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
        }
    }

    /**********************************
    Ações do OnLoad.
    **********************************/

    //CarregaGridEnderecos(window.parent.crmForm.all.new_clienteid);

    // Esconde a guia Hide
    //Xrm.Page.ui.tabs.get(3).setVisible(false);
    /*if (FormularioEstaEmModoDeCriacao) {
        //Xrm.Page.ui.tabs.get(1).setVisible(false);
        //Xrm.Page.ui.tabs.get(2).setVisible(false);
        Notification("Pressione o botão salvar para relacionar os endereços");
    } else {
        //CarregaGridEnderecos(Xrm.Page.getAttribute("new_clienteid").getValue()[0].id);
    }*/


    //Desabilita campos
    Xrm.Page.getControl("new_data_inicial").setDisabled(true);
    Xrm.Page.getControl("new_data_final").setDisabled(true);
}

function new_clienteid_onchange() {
    Xrm.Page.getAttribute("new_codigo_cliente").setValue(null);
    Xrm.Page.getAttribute("new_name").setValue(null);

    if (Xrm.Page.getAttribute("new_clienteid").getValue() != null) {
        var resultado = crmForm.PesquisaClientePor(Xrm.Page.getAttribute("new_clienteid").getValue()[0].id);

        /*if (resultado != null) {
            if (resultado.CodigoMatriz != null && resultado.CodigoMatriz.toString() != "[object Object]")
                Xrm.Page.getAttribute("new_codigo_cliente").setValue(resultado.CodigoMatriz.toString());

            if (resultado.NomeFantasia != null && resultado.NomeFantasia.toString() != "[object Object]")
                Xrm.Page.getAttribute("new_name").setValue(resultado.NomeFantasia.toString());
        }*/
    }
}