if (typeof Custom == "undefined") {
    Custom = {
        OpenDialog: function (webresource, registros) {
            var $v_0 = new Mscrm.CrmDialog(Mscrm.CrmUri.create(webresource), window, 620, 400, null);

            $v_0.show();
        },
        __namespace: true
    };
}

function Copiar(entidade, registros) {
    localStorage.setItem("registrosNum", registros.length);
    localStorage.setItem("registros", registros);
    Custom.OpenDialog("/WebResources/itbc_copiar_politicas_comerciais_form", registros);
}

function ExibirBotao(registros) {
    var btnRunWorklfow = top.document.getElementById("itbc_politicacomercial|NoRelationship|HomePageGrid|itbc_politicacomercial_ribbon_duplicar");
    if (registros.length > 0) {
        btnRunWorklfow.style.display = 'inline-block';
    }
    else {
        btnRunWorklfow.style.display = "none";
    }
}