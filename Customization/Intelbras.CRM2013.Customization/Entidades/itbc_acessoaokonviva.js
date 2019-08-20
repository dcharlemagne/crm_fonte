if (typeof (AcessoKonviva) == "undefined") { AcessoKonviva = {}; }

AcessoKonviva = {

    OnLoad: function () {

    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_acaocrm").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_acaocrm").setSubmitMode("always");
    },

    OnSave: function () {
        AcessoKonviva.DisabledEnabled(false);
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
        AcessoKonviva.DisabledEnabled(false);
        AcessoKonviva.ForceFieldSave();
    }
}