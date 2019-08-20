function Form_onload() {
    crmForm.CopiarClientesParticipantesPor = function (contratoOrigemId, contratoDestinoId) {
        if (Xrm.Page.getAttribute("statuscode").getValue() == 1) {
            var resultado = null;
            var comando = new RemoteCommand(ISOL_SERVICE_NAME, "CopiarClientesParticipantes", VENDAS_SERVICE_URL);
            comando.SetParameter("contratoOrigemId", contratoOrigemId);
            comando.SetParameter("contratoDestinoId", contratoDestinoId);
            comando.SetParameter("organizacao", Xrm.Page.context.getOrgUniqueName());

            var execucao = comando.Execute();

            return (execucao.Success) ? execucao.ReturnValue.Intelbras : null;
        }
        else {
            alert("Somente é possível efetuar a Cópia Participantes do Contrato de Origem quando a razão do status for igual a 'Rascunho'.");
        }
    }


    /*************************************
    Data:      09/02/2011
    Autor:     Gabriel Dias Junckes
    Descrição: Valida campos e chama a função para copiar os clientes participantes (Função usada no ISV).
    **************************************/
    crmForm.CopiarClientesParticipantes = function () {

        if (Xrm.Page.getAttribute("originatingcontract").getValue() == null) {
            alert("Não existe Contrato de Origem para copiar os Clientes Participantes!");
            return;
        }

        var contratoOrigemId = Xrm.Page.getAttribute("originatingcontract").getValue()[0].id;
        var contratoDestinoId = Xrm.Page.getAttribute("contractid").getValue();

        crmForm.CopiarClientesParticipantesPor(contratoOrigemId, contratoDestinoId);
    }

    crmForm.IncluirDataTerminoContrato = function () {

        var data = Xrm.Page.getAttribute("new_data_termino_real").getValue();
        var tipo = 0;

        if (Xrm.Page.getAttribute("contractservicelevelcode").getValue() != null && data != null)
            tipo = parseInt(Xrm.Page.getAttribute("contractservicelevelcode").getValue());


        switch (tipo) {

            case 1:
                Xrm.Page.getAttribute("expireson").setValue(Xrm.Page.getAttribute("new_data_termino_real").getValue());
                break;

            case 2:
            case 3:
                Xrm.Page.getAttribute("expireson").setValue(data.setFullYear(data.getFullYear() + 30));
                break;

            default:
                Xrm.Page.getAttribute("expireson").setValue(null);
                break;
        }
    }
}
function contractservicelevelcode_onchange() {
    crmForm.IncluirDataTerminoContrato();
}
function new_data_termino_real_onchange() {
    crmForm.IncluirDataTerminoContrato();
}
function new_cobertura_onchange() {

}
function expireson_onchange() {
    if (Xrm.Page.getAttribute("expireson").getValue() != null) {
        if (Xrm.Page.ui.getFormType() == 1) {
            Xrm.Page.getAttribute("new_data_termino_real").setValue(Xrm.Page.getAttribute("expireson").getValue());
        }
    }
}
