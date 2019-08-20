if (typeof (HistoricoCompradoCanal) == "undefined") { HistoricoCompradoCanal = {}; }

HistoricoCompradoCanal = {

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

        HistoricoCompradoCanal.DisabledEnabled(false);

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_accountid,itbc_trimestre,itbc_ano", " - "));

        HistoricoCompradoCanal.DisabledEnabled(true);
        HistoricoCompradoCanal.ForceFieldSave();
    }



}