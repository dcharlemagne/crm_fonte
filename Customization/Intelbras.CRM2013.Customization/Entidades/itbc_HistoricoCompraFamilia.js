if (typeof (HistoricoCompraFamilia) == "undefined") { HistoricoCompraFamilia = {}; }

HistoricoCompraFamilia = {

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

        HistoricoCompraFamilia.DisabledEnabled(false);

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_familiadeprodutoid,itbc_ano", " - "));

        HistoricoCompraFamilia.DisabledEnabled(true);
        HistoricoCompraFamilia.ForceFieldSave();
    }
}