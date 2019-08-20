if (typeof (RelacionamentodoCanal) == "undefined") { RelacionamentodoCanal = {}; }

RelacionamentodoCanal = {

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    VerificaData: function (campo) {

        var datai = Xrm.Page.getAttribute("itbc_datainicial");
        var dataf = Xrm.Page.getAttribute("itbc_datafinal");

        if (datai.getValue() == null || dataf.getValue() == null) return;

        if (datai.getValue() > dataf.getValue()) {
            alert("A data inicial deve ser menor que a data final");

            Xrm.Page.getAttribute(campo).setValue(null);
            Xrm.Page.getControl(campo).setFocus();
        }

    },

    OnSave: function (context) {
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
        var eventArgs = context.getEventArgs();
        RelacionamentodoCanal.DisabledEnabled(false);
        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_supervisorid,itbc_accountid", " - "));

        RelacionamentodoCanal.DisabledEnabled(true);
        RelacionamentodoCanal.ForceFieldSave();
    }


}