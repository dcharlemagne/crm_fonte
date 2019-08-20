if (typeof (HistoricoDetalhadoCompradoCanal) == "undefined") { HistoricoDetalhadoCompradoCanal = {}; }

HistoricoDetalhadoCompradoCanal = {

    OnLoad: function () {
        Xrm.Page.ui.controls.get("itbc_name").setVisible(false);
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        HistoricoDetalhadoCompradoCanal.DisabledEnabled(false);

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_account,itbc_product,itbc_ano", " - "));

        HistoricoDetalhadoCompradoCanal.DisabledEnabled(true);
        HistoricoDetalhadoCompradoCanal.ForceFieldSave();
    }
}