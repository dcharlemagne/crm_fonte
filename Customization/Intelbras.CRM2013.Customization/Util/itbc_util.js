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

    BuscarCep: function (cep) {

        //Configuração do serviço web
        Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebApoioFormulario, "BuscarCep");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("cep", cep);

        var retorno = Util.funcao.Execute();

        if (retorno['Success']) {
            return retorno['ReturnValue'];
        }
        else {
            throw new Error(retorno["Mensagem"]);
        }
    },

    SubstituirCaracterEspecial: function (email){
        com_acento = "ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖØÙÚÛÜÝŔÞßàáâãäåæçèéêëìíîïðñòóôõöøùúûüýþÿŕ";
        sem_acento = "AAAAAAACEEEEIIIIDNOOOOOOUUUUYRsBaaaaaaaceeeeiiiionoooooouuuuybyr";
        novastr = "";
        for (i = 0; i < email.length; i++) {
            troca = false;
            for (a = 0; a < com_acento.length; a++) {
                if (email.substr(i, 1) == com_acento.substr(a, 1)) {
                    novastr += sem_acento.substr(a, 1);
                    troca = true;
                    break;
                }
            }
            if (troca == false) {
                novastr += email.substr(i, 1);
            }
        }
        return novastr;
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
            if (numero != null) {
                Xrm.Page.getAttribute(enderecoTipo + "line1").setValue((resultado.Endereco + ", " + numero).substr(0, 40));
            } else { Xrm.Page.getAttribute(enderecoTipo + "line1").setValue(resultado.Endereco.substr(0, 40)) }
            Xrm.Page.getAttribute(enderecoTipo + "county").setValue(resultado.Bairro);
        }
    },


    PreencherCep: function (cepAttribute, resultado) {
        debugger;
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
            if (numero != null) {
                Xrm.Page.getAttribute(enderecoTipo + "line1").setValue((resultado.Endereco + ", " + numero).substr(0, 40));
            } else { Xrm.Page.getAttribute(enderecoTipo + "line1").setValue(resultado.Endereco.substr(0, 40)) }
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
            if (cnpj == "00000000000000") {
                Xrm.Utility.alertDialog('CNPJ Invalido!');
                Xrm.Page.getControl(CampoCnpj).setFocus(true);
                return false;
            }
            var digito = new Number(eval(cnpj.charAt(12) + cnpj.charAt(13)));

            for (i = 0; i < valida.length; i++) {
                dig1 += (i > 0 ? (cnpj.charAt(i - 1) * valida[i]) : 0);
                dig2 += cnpj.charAt(i) * valida[i];
            }
            dig1 = (((dig1 % 11) < 2) ? 0 : (11 - (dig1 % 11)));
            dig2 = (((dig2 % 11) < 2) ? 0 : (11 - (dig2 % 11)));

            if (((dig1 * 10) + dig2) != digito) {
                Xrm.Utility.alertDialog('CNPJ Invalido!');
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
                    Xrm.Page.getControl(CampoCpf).setFocus(true);
                    return false;
                }
                Xrm.Page.getAttribute(CampoCpf).setValue(Util.funcao.MascaraCPF(strCPF));
                return true;
            }
            else {
                Xrm.Utility.alertDialog('CPF Invalido!');
                Xrm.Page.getControl(CampoCpf).setFocus(true);
                return false;
            }
        }
    },

    //valida o CI(Uruguaio) digitado
    ValidarCI: function (campoCi) {
        strCI = Xrm.Page.getAttribute(campoCi).getValue();

        if (strCI != null) {
            exp = /\.|\-/g
            strCI = strCI.toString().replace(exp, "");
            var dig = strCI[strCI.length - 1];

            var a = 0; var i = 0;
            if(strCI.length <= 6){
                for(i = strCI.length; i < 7; i++){
                strCI = '0' + strCI;
                }
            }
            for(i = 0; i < 7; i++){
                a += (parseInt("2987634"[i]) * parseInt(strCI[i])) % 10;
            }

            var result = 10 - a % 10;

            if(parseInt(result) === parseInt(dig)) {
                return true
            }

            Xrm.Utility.alertDialog('CI Invalido!');
            Xrm.Page.getControl(campoCi).setFocus(true);
            return false;
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
        if (vlrcampo != null && vlrcampo != undefined) {
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
        var to = "aaaaaeeeeiiiioooouuuunc------";
        for (var i = 0, l = from.length ; i < l ; i++) {
            str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
        }

        str = str.replace(/[^a-z0-9 -]/g, '')
                 .replace(/\s+/g, '-')
                 .replace(/-+/g, '-');

        return str;
    },

    CallServiceJSON: function (url, method) {
        /// <summary>Executa chamada ao método do WCF informado.</summary>
        /// <param name="url" type="string">A url do WCF.</param>
        /// <param name="method" type="string">O nome do método a ser executado.</param>
        /// <returns type="Objeto">O retorno oferecido pelo método executado.</returns>

        var servico = this;
        var parameter = "";
        var obj = new Object();
        obj.Success = true;
        obj.ReturnValue = null;

        servico.SetParameter = function (name, value) {
            parameter += '"' + name + '": "' + value + '",';
        }

        servico.Execute = function () {
            $.ajax({
                async: false,
                type: "POST",
                url: url + "/" + method,
                data: configureParameter(),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                processdata: true,
                beforeSend: function (XMLHttpRequest) {
                    XMLHttpRequest.setRequestHeader("Accept", "application/json");
                },
                success: function (data, textStatus, XmlHttpRequest) {
                    obj.ReturnValue = data;
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    alert("Falha na chamada do método:" + method + textStatus + " erro : " + errorThrown);
                    obj.Success = false;
                }
            });

            return configureReturn(obj);
        }

        function configureReturn() {
            if (obj.ReturnValue == undefined || obj.ReturnValue == null)
                return obj;

            if (obj.ReturnValue.hasOwnProperty("d"))
                obj.ReturnValue = obj.ReturnValue.d;

            return obj;
        }

        function configureParameter() {
            parameter = parameter.length == 0 ? "," : parameter;
            return "{" + parameter.substr(0, parameter.length - 1) + "}";
        }
    },


    CallServiceXML: function (url, method) {
        /// <summary>Executa chamada ao método do WCF informado.</summary>
        /// <param name="url" type="string">A url do WCF.</param>
        /// <param name="method" type="string">O nome do método a ser executado.</param>
        /// <returns type="Objeto">O retorno oferecido pelo método executado.</returns>

        //Propriedades
        var servico = this;
        var parameter = {};
        var obj = new Object();
        obj.Success = true;
        obj.ReturnValue = null;

        //Configuração dos Parametros
        servico.SetParameter = function (name, value) {
            parameter[name] = value;
        }

        //Execução
        servico.Execute = function () {
            $.ajax({
                async: false,
                type: "POST",
                url: url + "/" + method,
                data: configureParameter(),
                dataType: "xml",
                success: function (data, textStatus, XmlHttpRequest) {
                    obj.ReturnValue = data;
                },
                error: function (XmlHttpRequest, textStatus, errorThrown) {
                    alert("Falha na chamada do método: " + method + " \n" + textStatus + " \nErro encontrado -> " + errorThrown);
                    obj.Success = false;
                }
            });

            return configureReturn(obj);
        }

        //Tratamento do retorno
        function configureReturn() {
            if (obj.ReturnValue == undefined || obj.ReturnValue == null)
                return obj;

            if (obj.ReturnValue.hasOwnProperty("d"))
                obj.ReturnValue = obj.ReturnValue.d;

            return obj;
        }

        //Retorno dos parametros
        function configureParameter() {
            return parameter;
        }
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

Util.Xrm.UI + {
    Refresh: function () {
        Xrm.Page.ui.refreshRibbon();
    }
}