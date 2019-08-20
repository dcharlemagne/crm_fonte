//function Form_onsave()
//{
////Exportar Cliente para o EMS
//Xrm.Page.getAttribute("new_exporta_erp").getValue() = "S"
//Xrm.Page.getAttribute("new_status_integracao").getValue() = "Integrando com o ERP"
//}
//function Form_onload()
//{
//SERVICE_NAME = "IsolService";
//VENDAS_SERVICE_URL = "/ISV/Intelbras/WebServices/Vendas/";

//podeBuscarOCep = true;

//crmForm.AssociarEnderecosSelecionados = function (botao) {
//    var items = getSelected('crmGrid');
//}


//crmForm.FormatarCep = function (cep) {
//	cep.getValue() = cep.getValue().replace("-","")
	
//    if (cep.getValue().length < 8 || cep.getValue().length > 9) {
//        alert("Formato do CEP é inválido.");
//        cep.focus();
//        return;
//    }

//    return cep.getValue().substr(0, 5) + "-" + cep.getValue().substr(5, 3);
//}

//crmForm.PesquisarEnderecoPrincipalPor = function (cep) {

//    Xrm.Page.getAttribute("line1").setValue(null);
//    Xrm.Page.getAttribute("line2").setValue(null);
//    Xrm.Page.getAttribute("city").setValue(null);
//    Xrm.Page.getAttribute("stateorprovince").setValue(null);
//    Xrm.Page.getAttribute("country").setValue(null);

//    var endereco = crmForm.PesquisarEnderecoPor(cep);

//    if (null != endereco && endereco.Achou) {

//        Xrm.Page.getAttribute("line1").setValue(endereco.Logradouro);
//        Xrm.Page.getAttribute("line2").setValue(endereco.Bairro);
//        Xrm.Page.getAttribute("city").setValue(endereco.Cidade);
//        Xrm.Page.getAttribute("stateorprovince").setValue(endereco.UF);
//        Xrm.Page.getAttribute("country").setValue("BRASIL");
//    }
//    else {
//        alert("CEP não encontrado.");
//    }
//}

//crmForm.PesquisarEnderecoPor = function (cep) {

//    //Configuração do serviço web
//    Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebApoioFormulario, "BuscarCep");

//    //Atribuição dos paramentros
//    Util.funcao.SetParameter("cep", cep);

//    var retorno = Util.funcao.Execute();

//    if (retorno['Success']) {
//        return retorno['ReturnValue'];
//    }
//    else {
//        throw new Error(retorno["Mensagem"]);
//    }

//    //if (null == cep) return;

//    //var resultado = null;

//    //var comando = new RemoteCommand(SERVICE_NAME, "PesquisarEnderecoDoClientePorCep", VENDAS_SERVICE_URL);
//    //comando.SetParameter("cep", cep);

//    //var execucao = comando.Execute();

//    //if (execucao.Success) {

//    //    resultado = execucao.ReturnValue;

//    //    if (resultado.Sucesso == false) {
//    //        alert(resultado.MensagemDeErro);
//    //        return;
//    //    }
//    //}

//    return resultado;
//}


//Notification = function (msg) {
//    // Recupera local onde será exibida mensagem
//    var element = 'Notifications';
//    var id = 'divMessage';
//    var src = document.getElementById(element);

//    // Se mensagem desejada (parâmetro) nula ou vazia, ocultar
//    if ((msg == null) || (msg == "")) {
//        src.style.display = 'none';
//    }
//    else {
//        // Cria NOVO elemento para inserir mensagem.
//        var newcontent = document.createElement("span");
//        newcontent.id = id;

//        // E insere mensagem (parâmetro) a ser exibida
//        newcontent.innerHTML = "<table><tr><td><img src='/_imgs/ico/16_info.gif' /></td><td valign='top'>" + msg + "</td></tr></table>";
//        src.style.display = "";

//        // Insere novo elemento no formulário
//        var previous = src.firstChild;
//        if (previous == null || previous.attributes['id'].nodeValue != id) {

//            if (src.childNodes.length == 0)
//                src.appendChild(newcontent);
//            else
//                src.insertBefore(newcontent, src.firstChild);
//        }
//        else
//            src.replaceChild(newcontent, previous);
//    }
//}


//Notification(Xrm.Page.getAttribute("new_mensagem").getValue());
//crmForm.all.name.Disabled = (Xrm.Page.getAttribute("new_chaveintegracao").getValue() != null);
//Xrm.Page.ui.tabs.get(1).setVisible(false);
//}
//function postalcode_onchange()
//{
//if (podeBuscarOCep) {debugger;
//	if(crmForm.all.postalcode){
//		Xrm.Page.getAttribute("postalcode").setValue(crmForm.FormatarCep(crmForm.all.postalcode));
//		crmForm.PesquisarEnderecoPrincipalPor(Xrm.Page.getAttribute("postalcode").getValue());
//	}
//}
//}
