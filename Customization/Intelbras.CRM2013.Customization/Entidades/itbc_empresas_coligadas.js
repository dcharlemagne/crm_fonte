if (typeof (EmpresaColigada) == "undefined") { EmpresaColigada = {}; }

EmpresaColigada = {

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

        EmpresaColigada.DisabledEnabled(false);
        //Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_conta"," "));

        EmpresaColigada.DisabledEnabled(true);
        EmpresaColigada.ForceFieldSave();
    }



}