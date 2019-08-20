function Form_onload() {
    if (Xrm.Page.getAttribute("statuscode").getValue() == '3') {
        Xrm.Page.getControl("statuscode").setDisabled(true);
        Xrm.Page.getControl("new_justificativa_posto").setDisabled(true);
        Xrm.Page.getControl("new_ocorrenciaid").setDisabled(true);
    }
}
function Form_onsave() {
    if (Xrm.Page.getAttribute("statuscode").getValue() == '3') //aprovado
    {
        Xrm.Page.getAttribute("new_data_liberacao").setValue(new Date());
        Xrm.Page.getAttribute("new_data_liberacao").setSubmitMode("always");
    }
}
function statuscode_onchange() {

}
function new_justificativa_posto_onchange() {

}