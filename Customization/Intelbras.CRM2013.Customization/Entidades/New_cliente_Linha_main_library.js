function Form_onsave() {
    /*Concatenar o Nome do Cliente e da Linha no Campo Nome*/
    Xrm.Page.getAttribute("new_name").getValue() = Xrm.Page.getAttribute("new_clienteid").getValue()[0].name + '--' + Xrm.Page.getAttribute("new_linhaid").getValue()[0].name;
}
function Form_onload() {
    /*Esconder a Tab Hide*/
    Xrm.Page.ui.tabs.get(1).setVisible(false);
}