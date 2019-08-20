if (typeof (BeneficioCanal) == "undefined") { BeneficioCanal = {}; }

BeneficioCanal = {

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

        BeneficioCanal.DisabledEnabled(false);

        var nome = Xrm.Page.getAttribute("itbc_name").getValue();
        if (nome == null || nome == "") {
            Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_canalid,itbc_categoriaid,itbc_beneficioid", " - "));
        }

        BeneficioCanal.DisabledEnabled(true);
        BeneficioCanal.ForceFieldSave();
    }
}