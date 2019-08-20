function Form_onload() {
    /***************************
    Ações OnLoad
    ***************************/

    /*Esconder Seção Hide*/
    Xrm.Page.ui.tabs.get(2).setVisible(false);

    /*Desabilitar os campos abaixo*/
    Xrm.Page.getControl("new_data_resposta").setDisabled(true);
    crmForm.all.new_login_posto.disabled = true;
    crmForm.all.new_resposta_posto.disabled = true;
}
function Form_onsave() {
    /***************************
    Ações OnSave
    ***************************/

    Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_clienteid").getValue()[0].name);
}
