if (typeof (MetaFamilia) == "undefined") { MetaFamilia = {}; }

MetaFamilia = {

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
    },

    OnSave: function (context) {

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_segmentoid,itbc_familiadeprodutos", " - "));

    }
}