﻿if (typeof (BensImoveisEmpresa) == "undefined") { BensImoveisEmpresa = {}; }

BensImoveisEmpresa = {

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

        BensImoveisEmpresa.DisabledEnabled(false);

        //Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_pais,itbc_estado,itbc_cidade,itbc_contactid,itbc_accountid", " - "));

        BensImoveisEmpresa.DisabledEnabled(true);
        BensImoveisEmpresa.ForceFieldSave();
    }
}