function Form_onload() {

}
function Form_onsave() {
    if (Xrm.Page.getAttribute("new_produtoid").getValue() != null) {
        Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_produtoid").getValue()[0].name);
    }
}
function new_nf_remessaid_onchange() {

}