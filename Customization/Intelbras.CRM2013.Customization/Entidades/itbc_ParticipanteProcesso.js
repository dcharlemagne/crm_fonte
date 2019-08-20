if (typeof (ParticipanteProcesso) == "undefined") { ParticipanteProcesso = {}; }

ParticipanteProcesso = {

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

        ParticipanteProcesso.DisabledEnabled(false);

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_processoid,itbc_contactid", " - "));

        ParticipanteProcesso.DisabledEnabled(true);
        ParticipanteProcesso.ForceFieldSave();
    }
}