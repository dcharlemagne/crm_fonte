if (typeof (MetaTrimestre) == "undefined") { MetaTrimestre = {}; }

MetaTrimestre = {

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
    },

    OnSave: function (context) {

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_metas,itbc_trimestre", " - "));

    }



}