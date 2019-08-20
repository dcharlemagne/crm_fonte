function Form_onsave() {
    Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_unidadedenegcioid").getValue()[0].name + " x " + Xrm.Page.getAttribute("new_categoriaid_rel").getValue()[0].name);
}
function Form_onload() {
    Xrm.Page.getAttribute("new_name").setValue(".");
}
