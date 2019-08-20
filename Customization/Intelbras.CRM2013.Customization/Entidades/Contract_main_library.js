IncluirDataTerminoContrato = function () {

    var data = Xrm.Page.getAttribute("new_data_termino_real").getValue();
    var tipo = 0;

    if (Xrm.Page.getAttribute("contractservicelevelcode").getValue() != null && data != null)
        tipo = parseInt(Xrm.Page.getAttribute("contractservicelevelcode").getValue());


    switch (tipo) {

        case 1:
            Xrm.Page.getAttribute("expireson").setValue(Xrm.Page.getAttribute("new_data_termino_real").getValue());
            break;

        case 2:
        case 200000:
        case 200001:
            Xrm.Page.getAttribute("expireson").setValue(data.setFullYear(data.getFullYear() + 30));
            Xrm.Page.getAttribute("expireson").setSubmitMode("always");
            break;

        default:
            Xrm.Page.getAttribute("expireson").setValue(null);
            break;
    }
}

function expireson_onchange() {
    if (Xrm.Page.getAttribute("expireson").getValue() != null) {
        if (Xrm.Page.ui.getFormType() == 1) {
            Xrm.Page.getAttribute("new_data_termino_real").setValue(Xrm.Page.getAttribute("expireson").getValue());
        }
    }
}
//function Form_onload() {
//    crmForm.CopiarClientesParticipantesPor = function (contratoOrigemId, contratoDestinoId) {
//        if (Xrm.Page.getAttribute("statuscode").getValue() == 1) {
//            var resultado = null;
//            var comando = new RemoteCommand(ISOL_SERVICE_NAME, "CopiarClientesParticipantes", VENDAS_SERVICE_URL);
//            comando.SetParameter("contratoOrigemId", contratoOrigemId);
//            comando.SetParameter("contratoDestinoId", contratoDestinoId);
//
//            var execucao = comando.Execute();
//            alert("Fim da cópia dos participantes do contrato");
//            document.location.reload(true);
//            return (execucao.Success) ? execucao.ReturnValue.Intelbras : null;
//        }
//        else {
//            alert("Somente é possível efetuar a Cópia Participantes do Contrato de Origem quando a razão do status for igual a 'Rascunho'.");
//        }
//    }
//
//
//    /*************************************
//    Data:      09/02/2011
//    Autor:     Gabriel Dias Junckes
//    Descrição: Valida campos e chama a função para copiar os clientes participantes (Função usada no ISV).
//    **************************************/
//    crmForm.CopiarClientesParticipantes = function () {
//
//        if (Xrm.Page.getAttribute("originatingcontract").getValue() == null) {
//            alert("Não existe Contrato de Origem para copiar os Clientes Participantes!");
//            return;
//        }
//
//        var contratoOrigemId = Xrm.Page.getAttribute("originatingcontract").getValue()[0].id;
//        var contratoDestinoId = Xrm.Page.getAttribute("contractid").getValue();
//
//        crmForm.CopiarClientesParticipantesPor(contratoOrigemId, contratoDestinoId);
//    }
//
//    crmForm.IncluirDataTerminoContrato = function () {
//
//        var data = Xrm.Page.getAttribute("new_data_termino_real").getValue();        
//        var tipo = 0;
//
//        if (Xrm.Page.getAttribute("contractservicelevelcode").getValue() != null && data != null)
//            tipo = parseInt(Xrm.Page.getAttribute("contractservicelevelcode").getValue());
//
//
//        switch (tipo) {
//
//            case 1:
//                Xrm.Page.getAttribute("expireson").setValue(Xrm.Page.getAttribute("new_data_termino_real").getValue());
//                break;
//
//            case 2:            
//            case 200000:
//            case 200001:
//                Xrm.Page.getAttribute("expireson").setValue(data.setFullYear(data.getFullYear() + 30));
//                break;
//
//            default:
//                Xrm.Page.getAttribute("expireson").setValue(null);
//                break;
//        }        
//    }
//}
//
CopiarClientesParticipantes = function () {

    if (Xrm.Page.getAttribute("originatingcontract").getValue() == null) {
        alert("Não existe Contrato de Origem para copiar os Clientes Participantes!");
        return;
    }

    var contratoOrigemId = Xrm.Page.getAttribute("originatingcontract").getValue()[0].id;
    var contratoDestinoId = Xrm.Page.getAttribute("contractid").getValue();

    CopiarClientesParticipantesPor(contratoOrigemId, contratoDestinoId);
}

CopiarClientesParticipantesPor = function (contratoOrigemId, contratoDestinoId) {
    if (Xrm.Page.getAttribute("statuscode").getValue() == 1) {
        Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "CopiarClientesParticipantes");

        Util.funcao.SetParameter("contratoOrigemId", contratoOrigemId);
        Util.funcao.SetParameter("contratoDestinoId", contratoDestinoId);

        var retorno = Util.funcao.Execute();
        alert("Fim da cópia dos participantes do contrato");
        document.location.reload(true);

        if (retorno["Success"] == true) {
            var data = retorno['ReturnValue'];
            if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
                alert($(data).find("MensagemDeErro").text());
                return "0";
            } else {
                return $(data).find("Intelbras")
            }
        }
    }
    else {
        alert("Somente é possível efetuar a Cópia Participantes do Contrato de Origem quando a razão do status for igual a 'Rascunho'.");
    }
}

//IncluirDataTerminoContrato = function () {
//
//    var data = Xrm.Page.getAttribute("new_data_termino_real").getValue();
//    var tipo = 0;
//
//    if (Xrm.Page.getAttribute("contractservicelevelcode").getValue() != null && data != null)
//        tipo = parseInt(Xrm.Page.getAttribute("contractservicelevelcode").getValue());
//
//
//    switch (tipo) {
//
//        case 1:
//            Xrm.Page.getAttribute("expireson").setValue(Xrm.Page.getAttribute("new_data_termino_real").getValue());
//            break;
//
//        case 2:
//        case 200000:
//        case 200001:
//            Xrm.Page.getAttribute("expireson").setValue(data.setFullYear(data.getFullYear() + 30));
//            Xrm.Page.getAttribute("expireson").setSubmitMode("always");
//            break;
//
//        default:
//            Xrm.Page.getAttribute("expireson").setValue(null);
//            break;
//    }
//}
//
//function expireson_onchange() {
//    if (Xrm.Page.getAttribute("expireson").getValue() != null) {
//        if (Xrm.Page.ui.getFormType() == 1) {
//            Xrm.Page.getAttribute("new_data_termino_real").setValue(Xrm.Page.getAttribute("expireson").getValue());
//        }
//    }
//}