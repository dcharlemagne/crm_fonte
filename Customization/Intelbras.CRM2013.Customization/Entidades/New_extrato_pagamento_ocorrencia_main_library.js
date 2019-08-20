function Form_onload() {
    /*VERIFICA CAMPO DATA DE RECEBIMENTO*/
    crmForm.ValidaDataRecebimento = function () {

        if (Xrm.Page.getAttribute("new_data_recebimento_nf").getValue() != null)
            Xrm.Page.getAttribute("new_data_prevista_pagamento").setRequiredLevel("recommended");
        else
            Xrm.Page.getAttribute("new_data_prevista_pagamento").setRequiredLevel("none");
    }


    /*DESABILITA FORMULARIO QUANDO O STATUS E IGUAL RECEBIMENTO NF*/
    if (Xrm.Page.getAttribute("statuscode").getValue() == 5) {
        //for (i = 1; i < document.crmForm.elements.length; i++)
        //    document.crmForm.elements[i].disabled = true;
        Xrm.Page.data.entity.attributes.forEach(function (attribute, index) {
            var control = Xrm.Page.getControl(attribute.getName());
            if (control) {
                control.setDisabled(true)
            }
        });
    }


    crmForm.Recalcular = function () {
        try {
            Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "RecalcularValorExtratoDePagamentoOcorrencia");

            Util.funcao.SetParameter("new_extrato_pagamento_ocorrenciaid", Xrm.Page.data.entity.getId().replace("{", "").replace("}", ""));
            var retorno = Util.funcao.Execute();
            if (retorno["Success"] == true) {
                window.location.reload(true);
            }
            else {
                var data = retorno['ReturnValue'];
                throw alert($(data).find("Mensagem").text());

                Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
                Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
            }
        }
        catch (ex) { alert(ex.Message); }
    }
}
function new_data_recebimento_nf_onchange() {
    debugger;
    crmForm.ValidaDataRecebimento();
}
