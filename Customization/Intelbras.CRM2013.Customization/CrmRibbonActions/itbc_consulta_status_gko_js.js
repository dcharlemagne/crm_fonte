if (typeof Custom == "undefined") {
    Custom = {
        OpenDialog: function (webresource) {
            var $v_0 = new Mscrm.CrmDialog(Mscrm.CrmUri.create(webresource), window, 620, 400, null);

            $v_0.show();
        },
        __namespace: true
    };
}

function ConsultaStatus(entidade, registro) {
    localStorage.setItem("registro", registro);
    Custom.OpenDialog("/WebResources/itbc_consulta_status_gko_dialog");
}   