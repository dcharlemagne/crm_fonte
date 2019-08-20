if (typeof (Util) == "undefined") { Util = {}; }

Util.funcao = {
    // DIV Absoluta
    // EXEMPLO: Carregando(true, "Carregando...");
    Carregando: function (visivel, frase) {
        //Localiza elemento BODY
        var body = document.getElementsByTagName("body")[0];

        //Remove Div Absoluta
        if (document.getElementById("div_absoluta"))
            body.removeChild(document.getElementById("div_absoluta"));

        //Aparecer Div
        if (visivel) {
            //Cria a DIV
            var absoluta = document.createElement("div");

            //Atribui as Propriedades
            absoluta.id = "div_absoluta";
            absoluta.style.position = "absolute";
            absoluta.style.border = "2px solid #000000";
            absoluta.style.background = "#ffffee";
            absoluta.style.fontSize = "16px";
            absoluta.style.fontFamily = "Arial";
            absoluta.style.fontWeight = "bold";
            absoluta.style.color = "#000099";
            absoluta.style.textAlign = "center";
            absoluta.style.paddingTop = 62;
            absoluta.style.top = "50%";
            absoluta.style.left = "50%";
            absoluta.style.width = 400;
            absoluta.style.height = 150;
            absoluta.style.marginLeft = -200;
            absoluta.style.marginTop = -75;

            //Cria o Texto da DIV
            var texto = document.createTextNode(frase);

            //Adiociona o Nó TEXTO na DIV
            absoluta.appendChild(texto);

            //Adiciona a DIV no BODY
            body.appendChild(absoluta);
        }
    },
    /*
    var result;
		var nome = null;
		var codigoCliente = null;
		clienteId = clienteId.replace("{", "").replace("}", "");
		$.ajax({
			type: "POST",
			datatype: "xml",
			url: "/isv/intelbras/webservices/vendas/isolservice.asmx/ObterClientePor",
			data: {
				clienteId: clienteId
			},
			success: function (data) {
				Xrm.Page.getAttribute("new_name").setValue($(data).find("NomeFantasia").text());
				Xrm.Page.getAttribute("new_codigo_cliente").setValue($(data).find("CodigoMatriz").text());
			},
			error: function (XmlHttpRequest, textStatus, errorThrown) {

				result = "An error ocurred for dynamic search record.\n" +
                     "Please contact the Administrator and inform this message.\n " +
                     "Error : " + textStatus + ": " + XmlHttpRequest.statusText + " - errorThrow " + errorThrown;

				result = null;
			}
		});
        */
    BuscarCep: function (cep) {
        $.ajax({
            type: "POST",
            datatype: "xml",
            url: Config.ParametroGlobal.IntegrationWS.URLServico.CrmWebApoioFormulario + "/BuscarCep",
            data: {
                cep: cep
            },
            success: function (data) {
                if (data != null) {
                    return retorno['ReturnValue'];
                }
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                throw new Error(retorno["Mensagem"]);
                result = null;
            }
        });
    },

    PreencherCepContato: function (cepAttribute, resultado) {
        if (resultado != undefined) {
            var cepAttributeName = cepAttribute
            if (cepAttribute.getName != undefined)
                cepAttributeName = cepAttribute.getName();

            var enderecoTipo = cepAttributeName.replace("postalcode", "");

            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "country").setValue(SDKore.CreateLookupWithLookUp(resultado.Pais));//SDKore.CreateLookup(resultado.PaisId, resultado.Pais, "itbc_pais"));
            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "stateorprovince").setValue(SDKore.CreateLookupWithLookUp(resultado.Estado));//SDKore.CreateLookup(resultado.EstadoId, resultado.EstadoNome, "itbc_estado"));
            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "city").setValue(SDKore.CreateLookupWithLookUp(resultado.Municipio));//SDKore.CreateLookup(resultado.CidadeId, resultado.CidadeNome, "itbc_municipios"));
            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "street").setValue(resultado.Endereco);
            Xrm.Page.getAttribute(enderecoTipo + "line3").setValue(resultado.Bairro);

            Xrm.Page.getAttribute(enderecoTipo + "country").setValue(resultado.Pais.Name);
            Xrm.Page.getAttribute(enderecoTipo + "stateorprovince").setValue(resultado.UF);
            Xrm.Page.getAttribute(enderecoTipo + "city").setValue(resultado.Municipio.Name);
            var numero = Xrm.Page.getAttribute("itbc_" + enderecoTipo + "number").getValue();
            Xrm.Page.getAttribute(enderecoTipo + "line1").setValue(resultado.Endereco + ", " + numero);
            Xrm.Page.getAttribute(enderecoTipo + "county").setValue(resultado.Bairro);
        }
    },


    PreencherCep: function (cepAttribute, resultado) {
        if (resultado != undefined) {
            var enderecoTipo = cepAttribute.getName().replace("postalcode", "");

            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "country").setValue(SDKore.CreateLookupWithLookUp(resultado.Pais));//SDKore.CreateLookup(resultado.PaisId, resultado.Pais, "itbc_pais"));
            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "stateorprovince").setValue(SDKore.CreateLookupWithLookUp(resultado.Estado));//SDKore.CreateLookup(resultado.EstadoId, resultado.EstadoNome, "itbc_estado"));
            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "city").setValue(SDKore.CreateLookupWithLookUp(resultado.Municipio));//SDKore.CreateLookup(resultado.CidadeId, resultado.CidadeNome, "itbc_municipios"));
            Xrm.Page.getAttribute("itbc_" + enderecoTipo + "street").setValue(resultado.Endereco);
            Xrm.Page.getAttribute(enderecoTipo + "line2").setValue(resultado.Bairro);

            Xrm.Page.getAttribute(enderecoTipo + "country").setValue(resultado.Pais.Name);
            Xrm.Page.getAttribute(enderecoTipo + "stateorprovince").setValue(resultado.UF);
            Xrm.Page.getAttribute(enderecoTipo + "city").setValue(resultado.Municipio.Name);
            var numero = Xrm.Page.getAttribute("itbc_" + enderecoTipo + "number").getValue();
            Xrm.Page.getAttribute(enderecoTipo + "line1").setValue(resultado.Endereco + ", " + numero);
            Xrm.Page.getAttribute(enderecoTipo + "county").setValue(resultado.Bairro);
        }
    },

    //valida o CNPJ digitado
    ValidarCNPJ: function (CampoCnpj) {
        ObjCnpj = Xrm.Page.getAttribute(CampoCnpj).getValue();
        if (ObjCnpj != null) {
            //var cnpj = ObjCnpj.value;
            var cnpj = ObjCnpj;
            var valida = new Array(6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2);
            var dig1 = new Number;
            var dig2 = new Number;

            exp = /\.|\-|\//g
            cnpj = cnpj.toString().replace(exp, "");
            var digito = new Number(eval(cnpj.charAt(12) + cnpj.charAt(13)));

            for (i = 0; i < valida.length; i++) {
                dig1 += (i > 0 ? (cnpj.charAt(i - 1) * valida[i]) : 0);
                dig2 += cnpj.charAt(i) * valida[i];
            }
            dig1 = (((dig1 % 11) < 2) ? 0 : (11 - (dig1 % 11)));
            dig2 = (((dig2 % 11) < 2) ? 0 : (11 - (dig2 % 11)));

            if (((dig1 * 10) + dig2) != digito) {
                Xrm.Utility.alertDialog('CNPJ Invalido!');
                Xrm.Page.getAttribute(CampoCnpj).setValue(null);
                Xrm.Page.getControl(CampoCnpj).setFocus(true);
                return false;
            }

            Xrm.Page.getAttribute(CampoCnpj).setValue(Util.funcao.MascaraCNPJ(ObjCnpj));
            return true;
        }
    },

    //valida o CPF digitado
    ValidarCPF: function (CampoCpf) {
        strCPF = Xrm.Page.getAttribute(CampoCpf).getValue();

        if (strCPF != null) {
            exp = /\.|\-/g
            strCPF = strCPF.toString().replace(exp, "");

            if (strCPF.length != 11 || strCPF == "00000000000" || strCPF == "11111111111" || strCPF == "22222222222" || strCPF == "33333333333" || strCPF == "44444444444" || strCPF == "55555555555" || strCPF == "66666666666" || strCPF == "77777777777" || strCPF == "88888888888" || strCPF == "99999999999") {
                Xrm.Utility.alertDialog('CPF Invalido!');
                Xrm.Page.getAttribute(CampoCpf).setValue(null);
                Xrm.Page.getControl(CampoCpf).setFocus(true);
                return false;
            }

            var numeros, digitos, soma, i, resultado, digitos_iguais;
            digitos_iguais = 1;
            if (strCPF.length < 11) {
                Xrm.Utility.alertDialog('CPF Invalido!');
                Xrm.Page.getAttribute(CampoCpf).setValue(null);
                Xrm.Page.getControl(CampoCpf).setFocus(true);
                return false;
            }
            for (i = 0; i < strCPF.length - 1; i++)
                if (strCPF.charAt(i) != strCPF.charAt(i + 1)) {
                    digitos_iguais = 0;
                    break;
                }
            if (!digitos_iguais) {
                numeros = strCPF.substring(0, 9);
                digitos = strCPF.substring(9);
                soma = 0;
                for (i = 10; i > 1; i--)
                    soma += numeros.charAt(10 - i) * i;
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(0)) {
                    Xrm.Utility.alertDialog('CPF Invalido!');
                    Xrm.Page.getAttribute(CampoCpf).setValue(null);
                    Xrm.Page.getControl(CampoCpf).setFocus(true);
                    return false;
                }
                numeros = strCPF.substring(0, 10);
                soma = 0;
                for (i = 11; i > 1; i--)
                    soma += numeros.charAt(11 - i) * i;
                resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                if (resultado != digitos.charAt(1)) {
                    Xrm.Utility.alertDialog('CPF Invalido!');
                    Xrm.Page.getAttribute(CampoCpf).setValue(null);
                    Xrm.Page.getControl(CampoCpf).setFocus(true);
                    return false;
                }
                Xrm.Page.getAttribute(CampoCpf).setValue(Util.funcao.MascaraCPF(strCPF));
                return true;
            }
            else {
                Xrm.Utility.alertDialog('CPF Invalido!');
                Xrm.Page.getAttribute(CampoCpf).setValue(null);
                Xrm.Page.getControl(CampoCpf).setFocus(true);
                return false;
            }
        }
    },

    //adiciona mascara de cnpj
    MascaraCNPJ: function (cnpj) {
        exp = /\.|\-/g
        cpf = cnpj.toString().replace(exp, "");

        //if (onchange) {
        //    if (Util.funcao.mascaraInteiro() == false) {
        //        event.returnValue = false;
        //    }
        //} else {
        for (var i = 0; i < cnpj.length; i++) {
            if (!cnpj.charAt(i) === parseInt(cnpj.charAt(i)))
                return false;
        }
        //}

        return Util.funcao.formataCampo(cnpj, '00.000.000/0000-00', event);
    },

    //adiciona mascara ao CPF
    MascaraCPF: function (cpf) {
        exp = /\.|\-/g
        cpf = cpf.toString().replace(exp, "");

        //if (onchange) {
        //    if (Util.funcao.mascaraInteiro() == false) {
        //        event.returnValue = false;
        //    }
        //} else {
        for (var i = 0; i < cpf.length; i++) {
            if (!cpf.charAt(i) === parseInt(cpf.charAt(i)))
                return false;
        }
        //}

        return Util.funcao.formataCampo(cpf, '000.000.000-00');
    },

    //adiciona mascara informada
    Mascara: function (campo, mask) {
        var vlrcampo = Xrm.Page.getAttribute(campo).getValue();
        if(vlrcampo != null && vlrcampo != undefined){
            vlrcampo = Util.funcao.RetirarMascara(vlrcampo);
            Xrm.Page.getAttribute(campo).setValue(Util.funcao.formataCampo(vlrcampo, mask));
        }
    },

    ///Retira caracteres de . / - ,
    RetirarMascara: function (valor) {

        while (valor.indexOf(".") >= 0) {
            valor = valor.replace(".", "");
        }

        while (valor.indexOf("-") >= 0) {
            valor = valor.replace("-", "");
        }

        while (valor.indexOf("/") >= 0) {
            valor = valor.replace("/", "");
        }

        while (valor.indexOf(" ") >= 0) {
            valor = valor.replace(" ", "");
        }

        while (valor.indexOf(",") >= 0) {
            valor = valor.replace(",", "");
        }

        while (valor.indexOf("(") >= 0) {
            valor = valor.replace("(", "");
        }

        while (valor.indexOf(")") >= 0) {
            valor = valor.replace(")", "");
        }

        return valor;
    },

    //valida numero inteiro com mascara
    mascaraInteiro: function (event) {
        if (event.keyCode < 48 || event.keyCode > 57) {
            event.returnValue = false;
            return false;
        }
        return true;
    },

    //formata de forma generica os campos verificando o evento da tecla
    formataCampo: function (campo, Mascara, evento) {
        var boleanoMascara;

        var Digitato = evento.keyCode;
        exp = /\-|\.|\/|\(|\)| /g
        campoSoNumeros = campo.toString().replace(exp, "");

        var posicaoCampo = 0;
        var NovoValorCampo = "";
        var TamanhoMascara = campoSoNumeros.length;;

        if (Digitato != 8) { // backspace 
            for (i = 0; i <= TamanhoMascara; i++) {
                boleanoMascara = ((Mascara.charAt(i) == "-") || (Mascara.charAt(i) == ".") || (Mascara.charAt(i) == "/"))
                boleanoMascara = boleanoMascara || ((Mascara.charAt(i) == "(") || (Mascara.charAt(i) == ")") || (Mascara.charAt(i) == " "))
                if (boleanoMascara) {
                    NovoValorCampo += Mascara.charAt(i);
                    TamanhoMascara++;
                } else {
                    NovoValorCampo += campoSoNumeros.charAt(posicaoCampo);
                    posicaoCampo++;
                }
            }
            return NovoValorCampo;
            //campo.value = NovoValorCampo;
            //return true;
        } else {
            return ""; //return true;
        }
    },

    //formata de forma generica os campos sem o evento do tecla
    formataCampo: function (campo, Mascara) {
        var boleanoMascara;

        exp = /\-|\.|\/|\(|\)| /g
        campoSoNumeros = campo.toString().replace(exp, "");

        var posicaoCampo = 0;
        var NovoValorCampo = "";
        var TamanhoMascara = campoSoNumeros.length;;

        for (i = 0; i <= TamanhoMascara; i++) {
            boleanoMascara = ((Mascara.charAt(i) == "-") || (Mascara.charAt(i) == ".") || (Mascara.charAt(i) == "/"))
            boleanoMascara = boleanoMascara || ((Mascara.charAt(i) == "(") || (Mascara.charAt(i) == ")") || (Mascara.charAt(i) == " "))
            if (boleanoMascara) {
                NovoValorCampo += Mascara.charAt(i);
                TamanhoMascara++;
            } else {
                NovoValorCampo += campoSoNumeros.charAt(posicaoCampo);
                posicaoCampo++;
            }
        }
        return NovoValorCampo;
    },

    ///concatena valores 
    ///setfield = campo que vai receber o valor concatenado
    ///fields = campos que contem os valores
    //;separador = o separador entre os valores
    //ContatenarCampos: function (setfield, fields, separador) {
    ContatenarCampos: function (fields, separador) {

        var vlrconcatenado = "";
        var vFields = fields.split(",");

        for (var i = 0; i < vFields.length; i++) {
            var attribute = Xrm.Page.getAttribute(vFields[i]);
            if (attribute == null) {
                alert("Atributo '" + vFields[i] + "' não existe no formulário.");
                continue;
            }

            var type = attribute.getAttributeType();
            if (Xrm.Page.getAttribute(vFields[i]).getValue() != null) {
                if (type == "boolean") {
                    vlrconcatenado += Xrm.Page.getAttribute(vFields[i]).getValue() + separador;
                }
                else if (type == "optionset") {
                    vlrconcatenado += Xrm.Page.getAttribute(vFields[i]).getSelectedOption().text + separador;
                }
                else if (type == "string") {
                    vlrconcatenado += Xrm.Page.getAttribute(vFields[i]).getValue() + separador;
                }
                else if (type == "lookup") {
                    vlrconcatenado += Xrm.Page.getAttribute(vFields[i]).getValue()[0].name + separador;
                }
                else if (type == "integer") {
                    vlrconcatenado += Xrm.Page.getAttribute(vFields[i]).getValue() + separador;
                }
            }
        }

        vlrconcatenado = vlrconcatenado.substr(0, (vlrconcatenado.length - separador.length));
        if (vlrconcatenado.length > 100)
            vlrconcatenado = vlrconcatenado.substring(0, 100);

        return vlrconcatenado;
        //Xrm.Page.getAttribute(setfield).setValue(vlrconcatenado);
    },

    formdisable: function (disablestatus) {

        var allAttributes = Xrm.Page.data.entity.attributes.get();
        for (var i in allAttributes) {
            var myattribute = Xrm.Page.data.entity.attributes.get(allAttributes[i].getName());
            var myname = myattribute.getName();
            if (Xrm.Page.getControl(myname) != null)
                Xrm.Page.getControl(myname).setDisabled(disablestatus);
        }
    },

    retornarRegistrosSincrono: function (entitySet, query) {

        var retrieveRecordsReq = new XMLHttpRequest();
        var ODataPath = Xrm.Page.context.getServerUrl() + "/XRMServices/2011/OrganizationData.svc/" + entitySet + "?" + encodeURI(query);
        retrieveRecordsReq.open('GET', ODataPath, false);
        retrieveRecordsReq.setRequestHeader("Accept", "application/json");
        retrieveRecordsReq.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        retrieveRecordsReq.send(null);
        var records = JSON.parse(retrieveRecordsReq.responseText).d;
        return records;
    },

    CriarSlug: function (str) {

        if (str == null) {
            return str;
        }

        str = str.replace(/^\s+|\s+$/g, '');
        str = str.toLowerCase();  
        
        var from = "ãàáäâèéëêìíïîòóöôùúüûñç·/_,:;";
        var to   = "aaaaaeeeeiiiioooouuuunc------";
        for (var i=0, l=from.length ; i<l ; i++) {
            str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
        }

        str = str.replace(/[^a-z0-9 -]/g, '')
                 .replace(/\s+/g, '-')
                 .replace(/-+/g, '-');

        return str;
    }
}

Util.Xrm = {
    ObterValor: function (controlid) {

        /// <summary>
        /// Retorna um controle
        /// </summary>     
        if (Xrm.Page.getAttribute(controlid) != null) {
            return Xrm.Page.getAttribute(controlid).getValue();
        }

        return null;
    },

    AtribuirValor: function (controlid, value) {

        /// <summary>
        /// Atribui um valor ao controle
        /// </summary>     

        if (Xrm.Page.getAttribute(controlid) != null) {
            Xrm.Page.getAttribute(controlid).setValue(value);
        }
    },

    HasValue: function (nomeAtributo) {
        return (Xrm.Page.getAttribute(nomeAtributo) == null || Xrm.Page.getAttribute(nomeAtributo).getValue() == null) ? false : true;
    },

    OptionSet: {

        GetValues: function (fieldName) {
            var values = [];
            var attribute = Xrm.Page.getAttribute(fieldName);
            if (attribute != null && attribute.getAttributeType() == "optionset") {
                var options = attribute.getOptions();
                for (var i in options) {
                    if (options[i].value != "null") values.push(options[i].value * 1);
                }
            }
            return values;
        }

    }
}