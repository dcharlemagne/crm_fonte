function Form_onload() {

}
function Form_onsave() {
    //*Copiar o nome da Linha para um campo nvarchar*//
    if (Xrm.Page.getAttribute("new_linha_unidade_negocioid").getValue() != null) {
        Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_linha_unidade_negocioid").getValue()[0].name);
    }
}
