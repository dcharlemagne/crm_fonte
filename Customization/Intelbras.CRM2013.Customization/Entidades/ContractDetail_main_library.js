function Form_onsave() {

}

function Form_onload() {
    //*Informa o Campo Total de Ocorrências 1000000*//
    if (Xrm.Page.getAttribute("totalallotments").getValue() == null) {
        Xrm.Page.getAttribute("totalallotments").setValue(1000000);
    }

    ValidaDataTerminoContrato(Xrm.Page.getAttribute("contractid").getValue()[0].id);
}

/**********************************
Data:      23/04/2018
Autor:     Jaime Campos
Descrição: Retorna a data do término do contrato
**********************************/
function ValidaDataTerminoContrato(contratoId) {

    //Configuração do serviço web
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "PesquisarDataTerminoContrato");

    //Atribuição dos paramentros
    Util.funcao.SetParameter("contratoId", contratoId);

    //Execução do serviço web
    var retorno = Util.funcao.Execute();

    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];

        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";

        var date = new Date($(data).find("Data").text());
        return Xrm.Page.getAttribute("expireson").setValue(date);
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
}

function totalallotments_onchange() {

}