function Form_onload() {
    //Esconde Seção Hide
    crmForm.all.new_name_c.parentElement.parentElement.style.display = 'none';
}
function Form_onsave() {
    Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_tipo_postoid").getValue()[0].name + ' x ' + Xrm.Page.getAttribute("new_linha_produtoid").getValue()[0].name);
}
