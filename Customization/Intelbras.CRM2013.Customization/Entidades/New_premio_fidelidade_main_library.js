function IFRAME_imagem_premio_fidelidade_onload() {

}
function Form_onload() {
    var CRM_FORM_TYPE_CREATE = 1;
    var CRM_FORM_TYPE_UPDATE = 2;

    /**********************************
    Data: 03/09/2012      
    Autor: Douglas Azevedo     
    Descrição: Carrega a imagem do premio no iframe
    **********************************/
    crmForm.CarregarImagemPremio = function () {
        var imagem = "about:blank";

        // Verifica se o formulário está no modo update
        if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {

            //Verifica se o campo da imagem contem dados
            if (crmForm.all.new_caminho_imagem)
                imagem = Xrm.Page.getAttribute("new_caminho_imagem").getValue();
        }

        // Carrega a URL do Iframe
        Xrm.Page.getControl("IFRAME_imagem_premio_fidelidade").setSrc(imagem);
    }


    crmForm.CarregarImagemPremio();
}
function new_caminho_imagem_onchange() {
    crmForm.CarregarImagemPremio();
}