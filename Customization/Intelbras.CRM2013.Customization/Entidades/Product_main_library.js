function Form_onload() {
    Xrm.Page.getControl("new_data_intervencao_tecnica").setDisabled(true);
    Xrm.Page.getControl("new_descricao_intervencao_tecnica").setDisabled(true);
    Xrm.Page.getControl("new_motivo_intervencao_tecnica").setDisabled(true);

    crmForm.desabalitaCampos = function () {

        var intervencao = Xrm.Page.getAttribute("new_intervencao_tecnica").getValue();
        var data = Xrm.Page.getAttribute("new_data_intervencao_tecnica");
        var descricao = Xrm.Page.getAttribute("new_descricao_intervencao_tecnica");
        var motivo = Xrm.Page.getAttribute("new_motivo_intervencao_tecnica");

        if (intervencao == 1) {//se for sim, os outros campos sao obrigatórios
            Xrm.Page.getAttribute("new_data_intervencao_tecnica").setRequiredLevel("required");
            Xrm.Page.getAttribute("new_descricao_intervencao_tecnica").setRequiredLevel("required");
            Xrm.Page.getAttribute("new_motivo_intervencao_tecnica").setRequiredLevel("required");
            data.Disabled = false;
            descricao.Disabled = false;
            motivo.Disabled = false;
        }

        else if (intervencao == 0) {
            Xrm.Page.getAttribute("new_data_intervencao_tecnica").setRequiredLevel("none");
            Xrm.Page.getAttribute("new_descricao_intervencao_tecnica").setRequiredLevel("none");
            Xrm.Page.getAttribute("new_motivo_intervencao_tecnica").setRequiredLevel("none");

            data.Disabled = true;
            descricao.Disabled = true;
            motivo.Disabled = true;
        }
    }
}
function new_intervencao_tecnica_onchange() {
    crmForm.desabalitaCampos();
}
