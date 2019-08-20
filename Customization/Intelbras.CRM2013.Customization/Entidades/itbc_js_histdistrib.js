if (typeof (HistoricoDistbuidor) == "undefined") { HistoricoDistbuidor = {}; }

HistoricoDistbuidor = {

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);

        if (Xrm.Page.getAttribute("itbc_historicodistribuidorpai").getValue() != null) {
            Xrm.Page.getControl("itbc_historicodistribuidorpai").setDisabled(true);
        }
    },
    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_revendaid").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_distribuidorid").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_datainicio").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_datafim").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_motivotroca").setSubmitMode("always");
    },

    OnSave: function () {
        //var distribuidorNome = Xrm.Page.getAttribute("itbc_distribuidorid").getValue()[0].itbc_nomeabreviado;
        //var revendaNome = Xrm.Page.getAttribute("itbc_revendaid").getValue()[0].itbc_nomeabreviado;
        //Xrm.Page.getAttribute("itbc_name").setValue(distribuidorNome);
        HistoricoDistbuidor.ForceFieldSave();
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
    }
}