if (typeof (categoria) == "undefined") { categoria = {}; }

categoria = {
    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
    },


    itbc_codigo_categoria_onchange: function () {
        this.numeroInteiro();
    },

    numeroInteiro: function () {
        var codValidar = Xrm.Page.getAttribute("itbc_codigo_categoria").getValue();
        var regExNumber = /^\d+$/;

        if (codValidar != null && !regExNumber.test(codValidar)) {
            Xrm.Page.getAttribute("itbc_codigo_categoria").setValue("");
            Xrm.Utility.alertDialog('Código :' + codValidar + ' - inválido. O Código deve ser numérico!');
        }
    }
}