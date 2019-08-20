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
		
	if (Xrm.Page.getAttribute('parentaccountid').getValue() == null) {
		if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
			EncontrarContaPorCNPJ(Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue());

			if (Xrm.Page.getAttribute('parentaccountid').getValue() != null) {
				Xrm.Page.getAttribute("itbc_clientefinaljacadastrado").setValue("993520000");
			} else {
				Xrm.Page.getAttribute("itbc_clientefinaljacadastrado").setValue("993520001");
			}
		}
	} else {
		Xrm.Page.getAttribute("itbc_clientefinaljacadastrado").setValue("993520000");
	}


	if (Xrm.Page.getAttribute('parentcontactid').getValue() == null) {
		if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
			EncontrarContatoPorCNPJ(Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue());
		}
	}

debugger;

	if (Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue() != null) {
		var nomeAbreviado = "";
		nomeAbreviado = Xrm.Page.getAttribute('itbc_cpfoucnpj').getValue();
		nomeAbreviado = nomeAbreviado.replace(".", "");
		nomeAbreviado = nomeAbreviado.replace(".", "");
		nomeAbreviado = nomeAbreviado.replace("-", "");
		nomeAbreviado = nomeAbreviado.replace("/", "");
		nomeAbreviado = nomeAbreviado.substr(0, 12);

		Xrm.Page.getAttribute("itbc_nomeabreviado").setValue(nomeAbreviado);

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
        if(Xrm.Page.getAttribute("itbc_tipo_solucao").getValue() != 993520000)
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
    },

    VerificaValorTipoSolucao: function() {
        // Unidades de Negocios que tem  o valor  Tipo de Solução, as demais seta nulo
        var unidadesNegocios = ["{75327F28-42E9-E311-9420-00155D013D39}"];
        var unidadesNegocio = Xrm.Page.getAttribute("itbc_businessunit").getValue();
        debugger;
        if(unidadesNegocio != null) {
            unidadesNegocio = unidadesNegocio[0]['id'];
            if(!unidadesNegocios.indexOf(unidadesNegocio)) {
                Xrm.Page.getAttribute("itbc_tipo_solucao").setValue(null);
            }
        } else {
            Xrm.Page.getAttribute("itbc_tipo_solucao").setValue(null);
        }
    }
}

function EncontrarContatoPorCNPJ(cnpj){
    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
        "<entity name='contact'>" +
        "  <attribute name='contactid' />" +
        "  <attribute name='fullname' />" +
        "  <order attribute='ownerid' descending='false' />" +
        "    <filter type='and'>" +
        "      <condition attribute='itbc_cpfoucnpj' operator='eq' value='" + cnpj + "' />" +
        "    </filter>" +
        "</entity>" +
        "</fetch>";
           
    // Consulta o Contato para obter o id e nome do registro
    var retrievedContacts = XrmServiceToolkit.Soap.Fetch(fetchXml);

    var LookupItem = new Object();
    // Verifica se localizou os dados
    if (retrievedContacts.length > 0) {
        if ((retrievedContacts[0].attributes['contactid'] != null) && (retrievedContacts[0].attributes['fullname'] != null)) {
            LookupItem.id = retrievedContacts[0].attributes.contactid.value;
            LookupItem.name = retrievedContacts[0].attributes.fullname.value;
            LookupItem.entityType = "contact";
            
            Xrm.Page.getAttribute("parentcontactid").setValue([LookupItem]);

        }
    } 
}

function EncontrarContaPorCNPJ(cnpj){
    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
        "<entity name='account'>" +
        "  <attribute name='accountid' />" +
        "  <attribute name='name' />" +
        "  <order attribute='ownerid' descending='false' />" +
        "    <filter type='and'>" +
        "      <condition attribute='itbc_cpfoucnpj' operator='eq' value='" + cnpj + "' />" +
        "    </filter>" +
        "</entity>" +
        "</fetch>";
           

    // Consulta a Conta para obter o id e nome do registro
    var retrievedAccounts = XrmServiceToolkit.Soap.Fetch(fetchXml);

    var LookupItem = new Object();
	
    // Verifica se localizou os dados
    if (retrievedAccounts.length > 0) {
        if ((retrievedAccounts[0].attributes['accountid'] != null) && (retrievedAccounts[0].attributes['name'] != null)) {
            LookupItem.id = retrievedAccounts[0].attributes.accountid.value;
            LookupItem.name = retrievedAccounts[0].attributes.name.value;
            LookupItem.entityType = "account";

            var url = Xrm.Page.context.getClientUrl() + "/main.aspx?etn=lead&id=" + Xrm.Page.data.entity.getId() + "&pagetype=entityrecord&extraqs=formid%3dE3B6DDB7-8DF0-4410-AC7B-FD32E5053D38%26parentaccountid%3d" + retrievedAccounts[0].attributes.accountid.value + "%26itbc_parentaccountid_temp%3d" + retrievedAccounts[0].attributes.accountid.value + "%26parentaccountidname%3d" + retrievedAccounts[0].attributes.name.value;
            
            Xrm.Page.getAttribute("parentaccountid").setValue([LookupItem]);

            //window.open(url, "_parent");
        }
    } else {
        Xrm.Page.getAttribute("itbc_clientefinaljacadastrado").setValue(993520001);
    }
}

