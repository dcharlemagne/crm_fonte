if (typeof (Contato) == "undefined") { Contato = {}; }

Contato = {

    TipoRelacao: {
        KeyAccountRepresentante: 993520007,
        ColaboradorCanal: 993520008,
        ClienteFinal: 1,
        ColaboradorIntelbras: 993520000,
        Outro: 993520006
    },

    PapelCanalIntelbras: {
        TecnicoAutorizado: 993520000,
        TecnicoAutonomo: 993520001,
        TecnicoLAI: 993520002,
        TecnicoDistribuidor: 993520003,        
        RH: 993520004,
        Trade: 993520005,
        PosVendas: 993520006,
        Instrutores: 993520007,
        Representante: 993520008,
        Outros: 993520009,
        Revenda: 993520010,
        TecnicoTI: 993520011,
        ProvedorInternet: 993520012,
        TecnicoEstGov: 993520013,
        ExecutivoVendas: 993520014        
    },

    ConjuntoOpcoesPorTipoRelacao: {
        KeyAccountRepresentante: new Array(),
        ColaboradorCanal: new Array(),
        ColaboradorIntelbras: new Array()
    },

    OnLoad: function () {
        
        Contato.ValidarEmail();

        if (Xrm.Page.getAttribute("firstname").getValue() != null)
            Xrm.Page.getAttribute('firstname').setValue(Xrm.Page.getAttribute("firstname").getValue().toUpperCase());

        if (Xrm.Page.getAttribute("lastname").getValue() != null)
            Xrm.Page.getAttribute('lastname').setValue(Xrm.Page.getAttribute("lastname").getValue().toUpperCase());

        Contato.VerificaObrigatoriedadeEndereco();
        Contato.CarregarOpcoesPorCaso();
        Contato.itbc_cpfoucnpj_onchange();

        if (Xrm.Page.getAttribute("itbc_integrabarramento").getValue() == null) {
            Xrm.Page.getAttribute('itbc_integrabarramento').setValue(false);
        };

        //Esconder Tipo de relação CRM 4
        var inicio = new Array();
        inicio[0] = 1;
        inicio[1] = 900000000;

        var fim = new Array();
        fim[0] = 1;
        fim[1] = 999999999;


        try {
            ArrayRangePickList(Xrm.Page.getControl("customertypecode"), Xrm.Page.getAttribute("customertypecode"), inicio, fim);
        }
        catch (e) { }



        if (!Xrm.Page.getAttribute("itbc_papelnocanal").getValue() ||
            Xrm.Page.getAttribute("customertypecode").getValue() == this.TipoRelacao.KeyAccountRepresentante)
            Contato.AtribuirPapelNoCanalIntelbras();

        //se o campo login pré-definido estiver preenchido, não poderá ser editado.
        if (Xrm.Page.getAttribute("new_login").getValue() != null) {
            Xrm.Page.getControl("new_login").setDisabled(true);
        };

        switch (Xrm.Page.ui.getFormType()) {

            case 1: //Create
                if (Xrm.Page.getAttribute("lastname").getValue() != null) {
                    Xrm.Page.getAttribute('itbc_cpfoucnpj').setValue(Xrm.Page.getAttribute("lastname").getValue());
                }
                Xrm.Page.getAttribute('lastname').setValue(null);
                break;
        }
    },

    itbc_firstname_onchange: function () {
        if (Xrm.Page.getAttribute("firstname").getValue() != null)
            Xrm.Page.getAttribute('firstname').setValue(Xrm.Page.getAttribute("firstname").getValue().toUpperCase());
    },

    itbc_lastname_onchange: function () {
        if (Xrm.Page.getAttribute("lastname").getValue() != null)
            Xrm.Page.getAttribute('lastname').setValue(Xrm.Page.getAttribute("lastname").getValue().toUpperCase());
    },

    VerificaObrigatoriedadeEndereco: function () {
        if (Xrm.Page.getAttribute("address1_postalcode").getValue() != null) {
            Xrm.Page.getAttribute("address1_postalcode").setRequiredLevel("required");
            Xrm.Page.getAttribute("itbc_address1_street").setRequiredLevel("required");
            Xrm.Page.getAttribute("address1_line2").setRequiredLevel("required");
            Xrm.Page.getAttribute("itbc_address1_city").setRequiredLevel("required");
            Xrm.Page.getAttribute("itbc_address1_stateorprovince").setRequiredLevel("required");
            Xrm.Page.getAttribute("itbc_address1_country").setRequiredLevel("required");
            Xrm.Page.getAttribute("itbc_address1_number").setRequiredLevel("required");
        }
    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("address1_line1").setDisabled(valor);
        Xrm.Page.getControl("address1_stateorprovince").setDisabled(valor);
        Xrm.Page.getControl("address1_city").setDisabled(valor);
        Xrm.Page.getControl("address1_county").setDisabled(valor);
        Xrm.Page.getControl("address1_country").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_cpfoucnpj").setSubmitMode("always");
        Xrm.Page.getAttribute("emailaddress1").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_line1").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_stateorprovince").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_city").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_county").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_country").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        if(Xrm.Page.getAttribute('firstname').getValue() != null){
            Xrm.Page.getAttribute('firstname').setValue(Xrm.Page.getAttribute("firstname").getValue().toUpperCase());
        }
        if (Xrm.Page.getAttribute('lastname').getValue() != null) {
            Xrm.Page.getAttribute('lastname').setValue(Xrm.Page.getAttribute("lastname").getValue().toUpperCase());
        }

        Contato.DisabledEnabled(false);
        Contato.address1_postalcode_onchange();

        Xrm.Page.getAttribute("address1_line1").setValue(Util.funcao.ContatenarCampos("itbc_address1_street,itbc_address1_number", ",").substr(0, 40));
        Xrm.Page.getAttribute("address1_stateorprovince").setValue(Util.funcao.ContatenarCampos("itbc_address1_stateorprovince", ""));
        Xrm.Page.getAttribute("address1_city").setValue(Util.funcao.ContatenarCampos("itbc_address1_city", ""));
        Xrm.Page.getAttribute("address1_county").setValue(Util.funcao.ContatenarCampos("itbc_address1_city", ""));
        Xrm.Page.getAttribute("address1_country").setValue(Util.funcao.ContatenarCampos("itbc_address1_country", ""));
        Contato.DisabledEnabled(true);
        Contato.ForceFieldSave();
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
        
        if( Contato.itbc_cpfoucnpj_onchange() == false ) {
            eventArgs.preventDefault();
        }

        if (Contato.ExibeMensagemCamposObrigatoriosIntegracao()) {
            Xrm.Page.ui.setFormNotification("Os dados do contato foram salvos, porém não foram integrados com os demais sistemas! Para que a integração ocorra os dados do endereço, CPF/CNPJ e e-mail devem estar preenchidos.", "WARNING", "itbc_name");
        }
    },

    ExibeMensagemCamposObrigatoriosIntegracao: function () {
        if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue() == null ||
           Xrm.Page.getAttribute("emailaddress1").getValue() == null ||
           Xrm.Page.getAttribute("address1_city").getValue() == null ||
           Xrm.Page.getAttribute("address1_county").getValue() == null ||
           Xrm.Page.getAttribute("address1_country").getValue() == null) {

            return true;
        }
        return false;
    },

    ValidarEmail: function () {
        var email = Xrm.Page.getAttribute("emailaddress1").getValue();
        if (email != null) {
            novastr = Util.funcao.SubstituirCaracterEspecial(email);
            
            Xrm.Page.getAttribute("emailaddress1").setValue(novastr);
        }
    },

    ControlarObrigatoriedadeCpf: function () {
        var origemContato = Xrm.Page.getAttribute("new_origem_contato").getValue();

        switch (origemContato) {
            case 1: //e-mail
                Xrm.Page.getAttribute("address1_postalcode").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_street").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line2").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_cpfoucnpj").setRequiredLevel("none");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_city").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_stateorprovince").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_country").setRequiredLevel("none");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("required");
                Xrm.Page.getAttribute("itbc_address1_number").setRequiredLevel("none");
                break;
            case 2: //telefone
                Xrm.Page.getAttribute("itbc_cpfoucnpj").setRequiredLevel("required");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("none");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("required");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_number").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_city").setRequiredLevel("required");
                Xrm.Page.getAttribute("itbc_address1_stateorprovince").setRequiredLevel("required");
                Xrm.Page.getAttribute("itbc_address1_country").setRequiredLevel("required");
                break;
            case 3: // Portal
                Xrm.Page.getAttribute("itbc_cpfoucnpj").setRequiredLevel("required");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("required");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("required");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("required");
                Xrm.Page.getAttribute("itbc_address1_city").setRequiredLevel("required");
                Xrm.Page.getAttribute("itbc_address1_stateorprovince").setRequiredLevel("required");
                Xrm.Page.getAttribute("itbc_address1_country").setRequiredLevel("required");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("required");
                Xrm.Page.getAttribute("itbc_address1_number").setRequiredLevel("required");

                break;
            default: //para todos os outros
                Xrm.Page.getAttribute("address1_postalcode").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_street").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line2").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_cpfoucnpj").setRequiredLevel("none");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_city").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_stateorprovince").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_country").setRequiredLevel("none");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("none");
                Xrm.Page.getAttribute("itbc_address1_number").setRequiredLevel("none");
                break;
        }
    },

    itbc_cpfoucnpj_onchange: function () {
        var result = true;

        if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
            // Garante que o valor é uma string
            var valor = Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue();
            valor = valor.toString();

            // Remove caracteres inválidos do valor
            valor = valor.replace(/[^0-9]/g, '');
            var pais = Xrm.Page.getAttribute('itbc_address1_country').getValue();

            if(pais != null) {
                pais = pais[0].name;
            }

            switch (pais) {
                case "Brasil":
                case null:
                    // Verifica CPF
                    if (valor.length == 11) {
                        result = Util.funcao.ValidarCPF("itbc_cpfoucnpj");
                    }
                        // Verifica CNPJ
                    else if (valor.length == 14) {
                        result = Util.funcao.ValidarCNPJ("itbc_cpfoucnpj");
                    }
                    else {
                        Xrm.Utility.alertDialog('CPF ou CNPJ Inválido!');
                        Xrm.Page.getControl('itbc_cpfoucnpj').setFocus(true);
                        result = false;
                    }
                    break;

                case "URUGUAI":
                    result = Util.funcao.ValidarCI("itbc_cpfoucnpj");
                    break;

                default:
                    break;
            }

            return result;
        }
    },

    telephone1_onchange: function () {
        Util.funcao.Mascara("telephone1", "(00)-0000-0000");
    },

    telephone3_onchange: function () {
        Util.funcao.Mascara("telephone3", "(00)-0000-0000");
    },

    fax_onchange: function () {
        Util.funcao.Mascara("fax", "(00)-0000-0000");
    },

    telephone2_onchange: function () {
        Util.funcao.Mascara("telephone2", "(00)-0000-0000");
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

    mobilephone_onchange: function () {
        Util.funcao.Mascara("mobilephone", "(00)-00000-0000");
    },

    GerarUrlCRM4: function () {
        var url = Xrm.Page.context.getClientUrl().replace(Xrm.Page.context.getOrgUniqueName(), "") + "/ISV/Web/BuscaCadastroContaCrm4.aspx?id=" + Xrm.Page.data.entity.getId() + "&tipo=contato";
        window.open(url, 'popUp', 'location=no,width=500,height=400');
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
        var cep = cepAttribute.getValue();

        var resultado = Util.funcao.BuscarCep(cep);

        if (resultado == null) {
            return;
        }
        else {
            Util.funcao.PreencherCep(cepAttribute, resultado);
        }
    },

    AtribuirPapelNoCanalIntelbras: function () {
        var tipoRelacao = Xrm.Page.getAttribute("customertypecode").getValue();
        var controlPapelNoCanal = Xrm.Page.getControl("itbc_papelnocanal");

        if (Xrm.Page.getAttribute("customertypecode").getValue()) {
            switch (tipoRelacao) {
                case Contato.TipoRelacao.KeyAccountRepresentante:
                    Contato.AdicionarOpcoes(Contato.ConjuntoOpcoesPorTipoRelacao.KeyAccountRepresentante, controlPapelNoCanal);
                    //Xrm.Page.getAttribute("itbc_papelnocanal").setValue(Contato.ConjuntoOpcoesPorTipoRelacao.KeyAccountRepresentante[0].value);
                    break;
                case Contato.TipoRelacao.ColaboradorCanal:
                    Contato.AdicionarOpcoes(Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal, controlPapelNoCanal);
                    break;
                case Contato.TipoRelacao.ColaboradorIntelbras:
                    Contato.AdicionarOpcoes(Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorIntelbras, controlPapelNoCanal);
                    break;
                case Contato.TipoRelacao.ClienteFinal:
                case Contato.TipoRelacao.Outro:
                    Xrm.Page.getAttribute("itbc_papelnocanal").setRequiredLevel("none");
                    controlPapelNoCanal.clearOptions();
                    controlPapelNoCanal.setDisabled(true);
                    break;
            }
        }
    },

    CarregarOpcoesPorCaso: function () {
        debugger;
        var controlPapelNoCanal = Xrm.Page.getControl("itbc_papelnocanal");
        if (controlPapelNoCanal != null) {
            var todasOpcoes = controlPapelNoCanal.getAttribute().getOptions();
            for (var i = 0; i < todasOpcoes.length; i++) {
                Contato.AlimentarConjuntos(todasOpcoes[i]);
            }
        }
    },

    AdicionarOpcoes: function (opcoes, control) {
        if (control.getDisabled())
            control.setDisabled(false);

        control.clearOptions();

        for (var i = 0; i < opcoes.length; i++)
            control.addOption(opcoes[i]);

        Xrm.Page.getAttribute("itbc_papelnocanal").setRequiredLevel("required");
    },

    AlimentarConjuntos: function (opcao) {
        switch (parseInt(opcao.value)) {
            case Contato.PapelCanalIntelbras.Representante:
                Contato.ConjuntoOpcoesPorTipoRelacao.KeyAccountRepresentante.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.ExecutivoVendas:
                Contato.ConjuntoOpcoesPorTipoRelacao.KeyAccountRepresentante.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.TecnicoAutorizado:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.TecnicoAutonomo:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.TecnicoDistribuidor:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.TecnicoLAI:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.RH:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorIntelbras.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.Trade:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorIntelbras.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.PosVendas:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorIntelbras.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.Instrutores:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorIntelbras.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.Outros:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorIntelbras.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.Revenda:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.TecnicoTI:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.ProvedorInternet:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.TecnicoEstGov:
                Contato.ConjuntoOpcoesPorTipoRelacao.ColaboradorCanal.push(opcao);
                break;
            case Contato.PapelCanalIntelbras.ExecutivoVendas:
                Contato.ConjuntoOpcoesPorTipoRelacao.KeyAccountRepresentante.push(opcao);
                break;
        }
    }
}