function Form_onload() {
    /*Esconder a Tab Hide*/
    Xrm.Page.ui.tabs.get(1).setVisible(false);
}
function Form_onsave() {
    /*Concatenar a Unidade de Negócio e da Linha no Campo Nome*/
    Xrm.Page.getAttribute("new_name").getValue() = Xrm.Page.getAttribute("new_unidade_negocioid").getValue()[0].name + '--' + Xrm.Page.getAttribute("new_linhaid").getValue()[0].name;
}
