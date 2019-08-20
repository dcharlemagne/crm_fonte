if (typeof (Contato) == "undefined") { Contato = {}; }

Contato = {
    OnLoad: function () {

        Contato.ValidarEmail();

        switch (Xrm.Page.ui.getFormType()) {

            case 1: //Create
                if (Xrm.Page.getAttribute("lastname").getValue() != null) {
                    Xrm.Page.getAttribute('itbc_cpfoucnpj').setValue(Xrm.Page.getAttribute("lastname").getValue());
                }
                Xrm.Page.getAttribute('lastname').setValue(null);
                break;
        }
        Contato.itbc_cpfoucnpj_onchange();
    },

    itbc_firstname_onchange: function () {
        if (Xrm.Page.getAttribute("firstname").getValue() != null)
            Xrm.Page.getAttribute('firstname').setValue(Xrm.Page.getAttribute("firstname").getValue().toUpperCase());
    },

    itbc_lastname_onchange: function () {
        if (Xrm.Page.getAttribute("lastname").getValue() != null)
            Xrm.Page.getAttribute('lastname').setValue(Xrm.Page.getAttribute("lastname").getValue().toUpperCase());
    },

    ValidarEmail: function () {
        var email = Xrm.Page.getAttribute("emailaddress1").getValue();
        if (email != null) {
            novastr = Util.funcao.SubstituirCaracterEspecial(email);
            Xrm.Page.getAttribute("emailaddress1").setValue(novastr);
        }
    },

    itbc_cpfoucnpj_onchange: function () {
        debugger;
        if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
            // Garante que o valor é uma string
            var valor = Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue();
            valor = valor.toString();

            // Remove caracteres inválidos do valor
            valor = valor.replace(/[^0-9]/g, '');

            // Verifica CPF
            if (valor.length == 11) {
                Util.funcao.ValidarCPF("itbc_cpfoucnpj");
            }
                // Verifica CNPJ
            else if (valor.length == 14) {
                Util.funcao.ValidarCNPJ("itbc_cpfoucnpj");
            }
            else {
                Xrm.Utility.alertDialog('CPF ou CNPJ Inválido!');
                Xrm.Page.getAttribute('itbc_cpfoucnpj').setValue(null);
                Xrm.Page.getControl('itbc_cpfoucnpj').setFocus(true);
                return false;
            }
        }
    },
    address1_postalcode_onchange: function () {
        debugger;
        if (Xrm.Page.getAttribute("address1_postalcode").getValue() != null) {
            var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();
            cep = cep.replace("-", "");
            if (cep.length > 8)
                cep = cep.substring(0, 8);

            Xrm.Page.getAttribute("address1_postalcode").setValue(cep);

            Util.funcao.Mascara("address1_postalcode", "99999-999");
        }
    },
    AtualizarEnderecoPorCep: function (executionContextObj) {
        debugger;
        if (executionContextObj == null)
            return;
        else if (executionContextObj.getEventSource() == null)
            return;
        else if (executionContextObj.getEventSource().getName() == null || executionContextObj.getEventSource().getName == undefined)
            return;

        var cepAttribute = executionContextObj.getEventSource();
        var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();

        var resultado = Util.funcao.BuscarCep(cep);

        if (resultado == null) {
            return;
        }
        else {
            Util.funcao.PreencherCep(cepAttribute, resultado);
        }
    }
}