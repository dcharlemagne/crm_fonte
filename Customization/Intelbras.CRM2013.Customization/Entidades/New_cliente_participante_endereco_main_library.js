function Form_onload() {
    // Esconde a guia Hide
    Xrm.Page.ui.tabs.get(2).setVisible(false);

    // Bloaqueia campos
    Xrm.Page.getControl("new_data_inicial").setDisabled(true);
    Xrm.Page.getControl("new_data_final").setDisabled(true);
}
function Form_onsave() {
    /**********************************
    Ações do OnSave.
    **********************************/
    //No crm4 tem um fluxo de trabalho que preenche no campo Nome o Código do Endereço.
    //No crm2015 foi feito esse evento para preencher o campo Nome após salvar o registro.
    //-------------------------------By Robson Bertolli - 24/03/2017------------------------------------

    if (Xrm.Page.getAttribute("new_codigoendereco").getValue() != null)
        Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_codigoendereco").getValue());

}