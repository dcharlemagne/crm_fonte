if (typeof (ModelodeMeta) == "undefined") { ModelodeMeta = {}; }

ModelodeMeta = {
    OnLoad: function () {

        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

        Xrm.Page.getControl("itbc_unidadedenegocioid").addPreSearch(this.AddCustomFilterUnidadedeNegocio);
    },


    AddCustomFilterUnidadedeNegocio: function () {
        var fetch = "<filter type='and'>";
        fetch += "<condition attribute='name' operator='eq' value='ADMINISTRATIVO' />";
        fetch += "</filter>";

        Xrm.Page.getControl("itbc_unidadedenegocioid").addCustomFilter(fetch);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    OnSave: function (context) {
        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_unidadedenegocioid,itbc_categoriaid,itbc_categoriaid", " - "));

    }
}
