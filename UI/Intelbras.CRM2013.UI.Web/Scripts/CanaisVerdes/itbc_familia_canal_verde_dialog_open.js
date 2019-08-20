if (typeof Custom == "undefined") {
    Custom = {
        OpenDialog: function (webresource) {
            var $v_0 = new Mscrm.CrmDialog(Mscrm.CrmUri.create(webresource), window, 620, 400, null);

            $v_0.show();
        },
        __namespace: true
    };
}

function Copiar(registro) {
    localStorage.setItem("registro", registro);
    localStorage.setItem("nomeRegistro", Xrm.Page.data.entity.getPrimaryAttributeValue());
    Custom.OpenDialog("/WebResources/itbc_vinculo_familia_canal_verde_dialog");
}