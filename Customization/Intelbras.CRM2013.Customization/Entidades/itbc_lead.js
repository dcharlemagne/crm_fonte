if (typeof (Lead) == "undefined") { Lead = {}; }

Lead = {

    OnLoad: function () {

        //debugger;

        //Xrm.Page.getAttribute("subject").setRequiredLevel("none");
        Xrm.Page.getAttribute("fullname").setRequiredLevel("none");

        if (Xrm.Page.getAttribute("itbc_natureza").getValue() != null) {
            switch (Xrm.Page.getAttribute("itbc_natureza").getValue()) {
                case 993520003: //Pessoa Física
                    if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
                        Util.funcao.Mascara("itbc_cpfoucnpj", "000.000.000-00");
                    }
                    break;
                case 993520000: //Pessoa Jurídica
                    if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
                        Util.funcao.Mascara("itbc_cpfoucnpj", "00.000.000/0000-00");
                    }
                    break;
            }
        }

    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        //Xrm.Page.getControl("subject").setDisabled(valor);
        Xrm.Page.getControl("fullname").setDisabled(valor);
        Xrm.Page.getControl("address1_line1").setDisabled(valor);
        Xrm.Page.getControl("address1_stateorprovince").setDisabled(valor);
        Xrm.Page.getControl("address1_city").setDisabled(valor);
        Xrm.Page.getControl("address1_county").setDisabled(valor);
        Xrm.Page.getControl("address1_country").setDisabled(valor);
    },

    ForceFieldSave: function () {
        //Xrm.Page.getAttribute("subject").setSubmitMode("always");
        Xrm.Page.getAttribute("fullname").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_line1").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_stateorprovince").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_city").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_county").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_country").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        Lead.DisabledEnabled(false);
        //Xrm.Page.getAttribute("subject").setValue(Util.funcao.ContatenarCampos("fullname,lastname,companyname", " - "));
        //Xrm.Page.getAttribute("fullname").setValue(Util.funcao.ContatenarCampos("fullname,lastname,companyname", " - "));
        Xrm.Page.getAttribute("fullname").setValue(Util.funcao.ContatenarCampos("fullname,lastname", " - "));
        Xrm.Page.getAttribute("address1_line1").setValue(Util.funcao.ContatenarCampos("itbc_address1_street,itbc_address1_number", ","));
        Xrm.Page.getAttribute("address1_stateorprovince").setValue(Util.funcao.ContatenarCampos("itbc_address1_stateorprovince", ""));
        Xrm.Page.getAttribute("address1_city").setValue(Util.funcao.ContatenarCampos("itbc_address1_city", ""));
        Xrm.Page.getAttribute("address1_county").setValue(Util.funcao.ContatenarCampos("itbc_address1_city", ""));
        Xrm.Page.getAttribute("address1_country").setValue(Util.funcao.ContatenarCampos("itbc_address1_country", ""));

        Lead.DisabledEnabled(true);
        Lead.ForceFieldSave();

        //Xrm.Page.getAttribute('itbc_cpfoucnpj').setValue(Util.funcao.RetirarMascara(Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue()));
    },

    itbc_CPFouCNPJ_onchange: function () {
        if (Xrm.Page.getAttribute("itbc_natureza").getValue() != null) {
            switch (Xrm.Page.getAttribute("itbc_natureza").getValue()) {
                case 993520003: //Pessoa Física
                    if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
                        Util.funcao.ValidarCPF("itbc_cpfoucnpj");
                    }
                    break;
                case 993520000: //Pessoa Jurídica
                    if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
                        Util.funcao.ValidarCNPJ("itbc_cpfoucnpj");
                    }
                    break;
            }
        }
    },

    telephone2_onchange: function () {
        Util.funcao.Mascara("telephone2", "(00)-0000-0000");
    },

    mobilephone_onchange: function () {
        Util.funcao.Mascara("mobilephone", "(00)-00000-0000");
    },

    telephone1_onchange: function () {
        Util.funcao.Mascara("telephone1", "(00)-0000-0000");
    },

    genetec_onchange: function () {
        debugger;
        Xrm.Page.getAttribute("itbc_valor_genetec").setValue(null);
    },

    AtualizarEnderecoPorCep: function (executionContextObj) {
        if (executionContextObj == null)
            return;
        else if (executionContextObj.getEventSource() == null)
            return;

        var cepAttribute = executionContextObj.getEventSource();
        var cep = cepAttribute.getValue();

        var resultado = Util.funcao.BuscarCep(cep);

        if (resultado == null) {
            return;
        }
        else {
            Util.funcao.PreencherCep(cepAttribute, resultado);
        }
    }
}
