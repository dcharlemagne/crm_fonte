function Form_onload() {
    /*************************  Funções  *************************/
    /*Copiar as Informações dos Campos Localidade e Prioridade para Nome*/
    crmForm.tipo_cobranca =
    function (campo) {
        Xrm.Page.getAttribute("new_name").getValue() = Xrm.Page.getAttribute("new_tipo_cobranca").getSelectedOption().text + "  -  " + Xrm.Page.getAttribute("new_tipo_ocorrencia").getSelectedOption().text
    }
}
function new_tipo_cobranca_onchange() {
    /*************************  Ações *************************/
    /* Copia os campos Tipo de Cobrança e Ipod e Ocorrencia para Nome */
    crmForm.tipo_cobranca(crmForm.all.new_tipo_cobranca);
}
function new_tipo_ocorrencia_onchange() {
    /*************************  Ações *************************/
    /* Copia os campos Tipo de Cobrança e Ipod e Ocorrencia para Nome */
    crmForm.tipo_cobranca(crmForm.all.new_tipo_cobranca);
}
