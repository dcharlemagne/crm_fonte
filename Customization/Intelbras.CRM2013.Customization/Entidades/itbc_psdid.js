if (typeof (ListaPSD) == "undefined") { ListaPSD = {}; }

ListaPSD = {
    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_businessunit").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_data_fim").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_data_inicio").setSubmitMode("always");
    },

    OnSave: function () {
        if (Xrm.Page.data.entity.getIsDirty()) {
            if (Xrm.Page.getAttribute("statuscode").getValue() != 1) {
                Xrm.Page.getControl("statuscode").setDisabled(false);
                Xrm.Page.getAttribute("statuscode").setValue(1);
                Xrm.Page.getAttribute("statuscode").setSubmitMode("always");
            }
        }
        if (!Xrm.Page.getControl("statuscode").getDisabled())
            Xrm.Page.getControl("statuscode").setDisabled(true);
    }
}