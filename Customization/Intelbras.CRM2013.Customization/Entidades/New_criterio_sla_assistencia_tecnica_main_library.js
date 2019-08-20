function Form_onsave() {
    //Campo nome recebe o valor de status (nome está escondido)

    var status = Xrm.Page.getAttribute("new_status_ocorrencia").getSelectedOption().text;
    Xrm.Page.getAttribute("new_name").setValue(status);
}
function Form_onload() {
    /*******************************
    Acoes OnLoad
    ****************************/
    //Esconde a secao HIDE
    crmForm.all.new_name_c.parentElement.parentElement.style.display = 'none';
}