﻿if (typeof (ProdutosListaPSD) == "undefined") { ProdutosListaPSD = {}; }

ProdutosListaPSD = {

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
        Xrm.Page.getAttribute("itbc_productid").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_psdid").setSubmitMode("always");

    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        ProdutosListaPSD.DisabledEnabled(false);

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_psdid,itbc_productid", " - "));

        ProdutosListaPSD.DisabledEnabled(true);
        ProdutosListaPSD.ForceFieldSave();
    }
}