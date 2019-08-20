IncluirDataTerminoContrato = function () {

    var data = Xrm.Page.getAttribute("new_data_termino_real").getValue();
    var tipo = 0;

    if (Xrm.Page.getAttribute("contractservicelevelcode").getValue() != null && data != null)
        tipo = parseInt(Xrm.Page.getAttribute("contractservicelevelcode").getValue());


    switch (tipo) {

        case 1:
            Xrm.Page.getAttribute("expireson").setValue(Xrm.Page.getAttribute("new_data_termino_real").getValue());
            break;

        case 2:
        case 200000:
        case 200001:
            Xrm.Page.getAttribute("expireson").setValue(data.setFullYear(data.getFullYear() + 30));
            Xrm.Page.getAttribute("expireson").setSubmitMode("always");
            break;

        default:
            Xrm.Page.getAttribute("expireson").setValue(null);
            break;
    }
}

function expireson_onchange() {
    if (Xrm.Page.getAttribute("expireson").getValue() != null) {
        if (Xrm.Page.ui.getFormType() == 1) {
            Xrm.Page.getAttribute("new_data_termino_real").setValue(Xrm.Page.getAttribute("expireson").getValue());
        }
    }
}