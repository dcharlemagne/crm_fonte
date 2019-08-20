if (typeof (Fatura) == "undefined") { Fatura = {}; }

Fatura = {

    OnLoad: function () {


    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("shipto_name").setDisabled(valor);
        Xrm.Page.getControl("shipto_line1").setDisabled(valor);
        Xrm.Page.getControl("shipto_city").setDisabled(valor);
        Xrm.Page.getControl("billto_name").setDisabled(valor);
        Xrm.Page.getControl("billto_line1").setDisabled(valor);
        Xrm.Page.getControl("billto_city").setDisabled(valor);
        Xrm.Page.getControl("billto_postalcode").setDisabled(valor);
        Xrm.Page.getControl("shipto_stateorprovince").setDisabled(valor);
        Xrm.Page.getControl("billto_line2").setDisabled(valor);
        Xrm.Page.getControl("billto_stateorprovince").setDisabled(valor);
        Xrm.Page.getControl("shipto_country").setDisabled(valor);
        Xrm.Page.getControl("billto_line3").setDisabled(valor);
        Xrm.Page.getControl("billto_country").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("shipto_name").setSubmitMode("always");
        Xrm.Page.getAttribute("shipto_line1").setSubmitMode("always");
        Xrm.Page.getAttribute("shipto_city").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_name").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_line1").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_city").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_postalcode").setSubmitMode("always");
        Xrm.Page.getAttribute("shipto_stateorprovince").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_line2").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_stateorprovince").setSubmitMode("always");
        Xrm.Page.getAttribute("shipto_country").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_line3").setSubmitMode("always");
        Xrm.Page.getAttribute("billto_country").setSubmitMode("always");
    },

    DeParaEnderecos: function () {
        Xrm.Page.getAttribute("shipto_name").setValue();
        Xrm.Page.getAttribute("shipto_line1").setValue(Util.funcao.ContatenarCampos("itbc_shipto_street,itbc_endereodeentreganr", ","));
        Xrm.Page.getAttribute("shipto_city").setValue(Util.funcao.ContatenarCampos("itbc_shipto_city", ""));
        Xrm.Page.getAttribute("shipto_stateorprovince").setValue(Util.funcao.ContatenarCampos("itbc_shipto_stateorprovince", ""));
        Xrm.Page.getAttribute("shipto_country").setValue(Util.funcao.ContatenarCampos("itbc_shipto_country", ""));

        Xrm.Page.getAttribute("billto_name").setValue(Util.funcao.ContatenarCampos("", ""));
        Xrm.Page.getAttribute("billto_line1").setValue(Util.funcao.ContatenarCampos("", ""));
        Xrm.Page.getAttribute("billto_city").setValue(Util.funcao.ContatenarCampos("", ""));
        Xrm.Page.getAttribute("billto_postalcode").setValue(Util.funcao.ContatenarCampos("shipto_postalcode", ""));
        Xrm.Page.getAttribute("billto_line2").setValue(Util.funcao.ContatenarCampos("", ""));
        Xrm.Page.getAttribute("billto_stateorprovince").setValue(Util.funcao.ContatenarCampos("", ""));
        Xrm.Page.getAttribute("billto_line3").setValue(Util.funcao.ContatenarCampos("", ""));
        Xrm.Page.getAttribute("billto_country").setValue(Util.funcao.ContatenarCampos("", ""));
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        Fatura.DisabledEnabled(false);
        Fatura.DeParaEnderecos();

        Fatura.DisabledEnabled(true);
        Fatura.ForceFieldSave();

    },

    itbc_cpfoucnpj_onchange: function () {

        if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
            Util.funcao.ValidarCPF("itbc_cpfoucnpj");
        }
    }

}