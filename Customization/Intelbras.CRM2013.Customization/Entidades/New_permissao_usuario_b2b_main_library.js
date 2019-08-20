function Form_onload() {
    //Esconder a tab Hide
    Xrm.Page.ui.tabs.get(1).setVisible(false);
}
function Form_onsave() {
    //*Copiar o nome do Contato  + UN   para um campo nvarchar*//
    if (Xrm.Page.getAttribute("itbc_codigo_representante").getValue() == null) {
        Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_contatoid").getValue()[0].name + " - " + Xrm.Page.getAttribute("new_unidade_negocioid").getValue()[0].name);
    } else {
        Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_contatoid").getValue()[0].name + " - " + Xrm.Page.getAttribute("itbc_codigo_representante").getValue());
    }
}
function new_contatoid_onchange() {

}
function itbc_representante_onchange() {
    // Retira a informação do Campo Unidade de Negócio
    Xrm.Page.getAttribute("new_unidade_negocioid").setValue(null);
}
function new_unidade_negocioid_onchange() {
    // Retira a informação do Campo Unidade de Negócio
    Xrm.Page.getAttribute("itbc_codigo_representante").setValue(null);
}