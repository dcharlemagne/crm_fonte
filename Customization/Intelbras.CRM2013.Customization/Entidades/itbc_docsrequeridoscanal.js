if (typeof (DocRequeridoCanal) == "undefined") { DocRequeridoCanal = {}; }

DocRequeridoCanal = {

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


    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        DocRequeridoCanal.DisabledEnabled(false);
        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_accountid,itbc_tipodocumentoid", " - "));
        DocRequeridoCanal.DisabledEnabled(true);
        DocRequeridoCanal.ForceFieldSave();
    }



}