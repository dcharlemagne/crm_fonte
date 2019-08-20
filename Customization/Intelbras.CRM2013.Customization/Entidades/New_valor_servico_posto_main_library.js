function Form_onload() {
    crmForm.DesabilitaFamiliaComercial = function () {
        var familia = Xrm.Page.getAttribute("new_linha_unidade_negocioid");
        var produto = Xrm.Page.getAttribute("new_produtoid");

        if (produto.getValue() !== null) {
            Xrm.Page.getAttribute("new_linha_unidade_negocioid").setRequiredLevel("none");
            familia.Disabled = true;
        }
        else if (produto.getValue() == null) {
            Xrm.Page.getAttribute("new_linha_unidade_negocioid").setRequiredLevel("required");
            familia.Disabled = false;
        }
    }



    crmForm.DesabilitaProduto = function () {
        var familia = Xrm.Page.getAttribute("new_linha_unidade_negocioid");
        var produto = Xrm.Page.getAttribute("new_produtoid");

        if (familia.getValue() != null) {

            Xrm.Page.getAttribute("new_produtoid").setRequiredLevel("none");
            produto.Disabled = true;
        }
        else if (familia.getValue() == null) {
            Xrm.Page.getAttribute("new_produtoid").setRequiredLevel("required");
            produto.Disabled = false;
        }
    }



    /**********************************
    Ações do OnLoad.
    **********************************/

    /*Esconder a Tab Hide*/
    Xrm.Page.ui.tabs.get(1).setVisible(false);


    crmForm.DesabilitaFamiliaComercial();
    crmForm.DesabilitaProduto();
}
function Form_onsave() {
    /**********************************
    Ações do OnSave.
    **********************************/

    //Copiar o nome do Estado para um campo nvarchar
    if (Xrm.Page.getAttribute("new_familia_comercialid").getValue() != null)
        Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_clienteid").getValue()[0].name + '--' + Xrm.Page.getAttribute("new_linha_unidade_negocioid").getValue()[0].name + '--R$' + Xrm.Page.getAttribute("new_valor").getValue());
    else
        Xrm.Page.getAttribute("new_name").getValue() = Xrm.Page.getAttribute("new_clienteid").getValue()[0].name + '--' + Xrm.Page.getAttribute("new_produtoid").getValue()[0].name + '--R$' + Xrm.Page.getAttribute("new_valor").getValue()

    // Obriga o preenchimento de Produto ou Serviço
    if (Xrm.Page.getAttribute("new_linha_unidade_negocioid").getValue() == null && Xrm.Page.getAttribute("new_produtoid").getValue() == null) {
        alert("Ação não executada. É necessário o preenchimentos de Produto ou Segmento.");
        event.returnValue = false;
        return false;
    }
}
function new_linha_unidade_negocioid_onchange() {
    crmForm.DesabilitaProduto();
}
function new_produtoid_onchange() {
    crmForm.DesabilitaFamiliaComercial();
}
