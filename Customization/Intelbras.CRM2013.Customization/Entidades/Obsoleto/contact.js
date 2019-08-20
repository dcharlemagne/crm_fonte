function Form_onsave() {
    if (Xrm.Page.getAttribute("new_cpf").getValue() != null)
        Xrm.Page.getAttribute("new_cpf_sem_mascara").setValue(crmForm.filtraNumeros(Xrm.Page.getAttribute("new_cpf").getValue()));
}
function Form_onload() {
    var UN_DO_USUARIO = GetMyBusinessUnit();
    GetMyBusinessUnit = function () { return UN_DO_USUARIO; };

    // Cria objeto LookupItem
    function LookupItem(typename) {
        this.id = null;
        this.name = null;
        this.entityType = typename;
    }

    // Adiciona uma msg no topo do formulário
    crmForm.Notification = function (msg) {
        // Recupera local onde será exibida mensagem
        var element = 'Notifications';
        var id = 'divMessage';
        var src = document.getElementById(element);

        // Se mensagem desejada (parâmetro) nula ou vazia, ocultar
        if ((msg == null) || (msg == "")) {
            src.style.display = 'none';
        } else {
            // Cria NOVO elemento para inserir mensagem.
            var newcontent = document.createElement("span");
            newcontent.id = id;

            // E insere mensagem (parâmetro) a ser exibida
            newcontent.innerHTML = "<table><tr><td><img src='/_imgs/ico/16_info.gif' /></td><td valign='top'>" + msg + "</td></tr></table>";
            src.style.display = "";

            // Insere novo elemento no formulário
            var previous = src.firstChild;
            if (previous == null || previous.attributes['id'].nodeValue != id)
                if (src.childNodes.length == 0)
                    src.appendChild(newcontent);
                else
                    src.insertBefore(newcontent, src.firstChild);
            else
                src.replaceChild(newcontent, previous);
        }
    }


    /**********************************
    Data:      01/10/2010
    Autor:     Gabriel Dias Junckes
    Descrição: Obriga os campos Login e Email quando o contato tiver acesso ao portal.
    **********************************/

    crmForm.obrigaCamposAcessoPortal = function () {
        document.crmForm.SetFieldReqLevel("new_login", document.all.new_acessoportal.getValue() == 1 ? 2 : 0);
        document.crmForm.SetFieldReqLevel("emailaddress1", document.all.new_acessoportal.getValue() == 1 ? 2 : 0);
    }

    crmForm.phoneFormat = function (campo) {
        if (campo.getValue() == null) return;

        var phone = crmForm.filtraNumeros(campo.getValue());

        if (phone.length < 10) { alert('Atenção! Informe o Número com o DDD.'); campo.setValue(null); return; };

        switch (phone.length) {
            case 10:
                phone = '(' + phone.substr(0, 2) + ') ' + phone.substr(2, 4) + '-' + phone.substr(6);
                break;

            case 11:
                phone = '(' + phone.substr(0, 2) + ') ' + phone.substr(2, 5) + '-' + phone.substr(7);
                break;
        }

        campo.setValue(phone);
    }

    crmForm.filtraNumeros = function (numero) {
        numero += ""; // Converte qualquer valor de "var numero" em String
        var numValidado = ""; // numero validado
        var temp; // Cria var temporaria

        for (var i = 0; i < numero.length; i++) { // Laço que percorre todos os caractere de "var numero"
            temp = numero.charAt(i); // Caractere atual
            if ((temp >= "0") && (temp <= "9"))
                numValidado += temp; // Checa cada caracter; se for nº adiciona em "var numValidado"
        }

        return numValidado; // Retorno somente números
    }

    /**********************************
    Data:      28/01/2011
    Autor:     Gabriel Dias Junckes
    Descrição: Busca endereço via WS. 
    **********************************/
    crmForm.PesquisarEnderecoPor = function (cep) {
        var comando = new RemoteCommand(APOIO_SERVICE_NAME, "PesquisarLocalidadePor", VENDAS_SERVICE_URL);
        comando.SetParameter("cep", cep);
        comando.SetParameter("organizacaoNome", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        return (execucao.Success) ? execucao.ReturnValue : null;
    }

    /**********************************
    Data:      28/01/2011
    Autor:     Gabriel Dias Junckes
    Descrição: Preenche os campos do endereço. 
    **********************************/
    crmForm.PesquisarEnderecoPrincipalPor = function (campoCep) {
        if (campoCep.getValue() == null) return;

        Xrm.Page.getAttribute("address1_line1").setValue(null);
        Xrm.Page.getAttribute("address1_line3").setValue(null);
        Xrm.Page.getAttribute("new_cidadeid").setValue(null);
        Xrm.Page.getAttribute("new_ufid").setValue(null);
        Xrm.Page.getAttribute("new_paisid").setValue(null);
        Xrm.Page.getAttribute("new_regionalid").setValue(null);
        campoCep.setValue(campoCep.getValue().replace("-", ""));

        if (campoCep.getValue().length != 8) throw ('Atenção ! Informe o CEP Correto.');

        var endereco = crmForm.PesquisarEnderecoPor(campoCep.getValue());

        if (endereco == null) throw ('O serviço para pesquisa de CEP não está disponivel');

        if (endereco.Sucesso == false)
            throw (endereco.Mensagem);
        else {
            crmForm.FormataCep(campoCep);
            Xrm.Page.getAttribute("address1_line1").setValue(endereco.Logradouro);
            Xrm.Page.getAttribute("address1_line3").setValue(endereco.Bairro);

            // Cidade
            if (endereco.CidadeId != null) {
                var lookupCidade = new LookupItem("new_cidade");
                lookupCidade.id = endereco.CidadeId;
                lookupCidade.name = endereco.CidadeNome;
                Xrm.Page.getAttribute("new_cidadeid").setValue([lookupCidade]);
            }

            // Regional
            if (endereco.RegionalId != null) {
                var lookupRegional = new LookupItem("new_regional");
                lookupRegional.id = endereco.RegionalId;
                lookupRegional.name = endereco.RegionalNome;
                Xrm.Page.getAttribute("new_regionalid").setValue([lookupRegional]);
            }

            // UF
            if (endereco.UfId != null) {
                var lookupUf = new LookupItem("new_uf");
                lookupUf.id = endereco.UfId;
                lookupUf.name = endereco.UfNome;
                Xrm.Page.getAttribute("new_ufid").setValue([lookupUf]);
            }

            // Pais
            if (endereco.PaisId != null) {
                var lookupPais = new LookupItem("new_pais");
                lookupPais.id = endereco.PaisId;
                lookupPais.name = endereco.PaisNome;
                Xrm.Page.getAttribute("new_paisid").setValue([lookupPais]);
            }
        }
    }

    /**********************************
    Data:      28/01/2011
    Autor:     Gabriel Dias Junckes
    Descrição: Formata CEP
    **********************************/
    crmForm.FormataCep = function (campo) {
        var pattern = /[^0-9]*([0-9])[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*.*/;
        var mask = '$1$2$3$4$5-$6$7$8';
        campo.setValue(campo.getValue().replace(pattern, mask));
    }

    /**********************************
    Data:      11/05/2011
    Autor:     Gabriel Dias Junckes
    Descrição:Valida o CNPJ.
    **********************************/
    crmForm.ValidarCnpj = function (cnpj) {
        var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
        digitos_iguais = 1;
        if (cnpj.length < 14 && cnpj.length < 15) return false;

        for (i = 0; i < cnpj.length - 1; i++)
            if (cnpj.charAt(i) != cnpj.charAt(i + 1)) {
                digitos_iguais = 0;
                break;
            }

        if (!digitos_iguais) {
            tamanho = cnpj.length - 2
            numeros = cnpj.substring(0, tamanho);
            digitos = cnpj.substring(tamanho);
            soma = 0;
            pos = tamanho - 7;

            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2)
                    pos = 9;
            }

            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0)) return false;

            tamanho = tamanho + 1;
            numeros = cnpj.substring(0, tamanho);
            soma = 0;
            pos = tamanho - 7;

            for (i = tamanho; i >= 1; i--) {
                soma += numeros.charAt(tamanho - i) * pos--;
                if (pos < 2)
                    pos = 9;
            }

            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1)) return false;

            return true;
        }
        else
            return false;
    }


    /**********************************
    Data:      28/01/2011
    Autor:     Gabriel Dias Junckes
    Descrição: Valida CPF, return true || false.
    **********************************/
    crmForm.ValidarCpf = function (cpf) {
        var numeros, digitos, soma, i, resultado, digitos_iguais;
        digitos_iguais = 1;

        if (cpf.length < 11) return false;

        for (i = 0; i < cpf.length - 1; i++)
            if (cpf.charAt(i) != cpf.charAt(i + 1)) {
                digitos_iguais = 0;
                break;
            }

        if (!digitos_iguais) {
            numeros = cpf.substring(0, 9);
            digitos = cpf.substring(9);
            soma = 0;

            for (i = 10; i > 1; i--) soma += numeros.charAt(10 - i) * i;

            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

            if (resultado != digitos.charAt(0)) return false;

            numeros = cpf.substring(0, 10);
            soma = 0;

            for (i = 11; i > 1; i--) soma += numeros.charAt(11 - i) * i;

            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

            if (resultado != digitos.charAt(1)) return false;

            return true;
        }
        else
            return false;
    }

    /**********************************
    Data:      28/01/2011
    Autor:     Gabriel Dias Junckes
    Descrição: Formata o campo CPF ou CNPJ.
    **********************************/
    crmForm.FormatarCpfCnpj = function (campo) {
        if (campo.getValue() == null) return;

        var exp = /\-|\.|\/|\(|\)| /g
        var campoFormatado = campo.getValue().replace(exp, "");
        var campoValido;
        var resultado = null;

        switch (campoFormatado.length) {
            case 11:
                campoValido = crmForm.ValidarCpf(campoFormatado);
                if (campoValido)
                    resultado = campoFormatado.substr(0, 3) + '.' + campoFormatado.substr(3, 3) + '.' + campoFormatado.substr(6, 3) + '-' + campoFormatado.substr(9, 2);
                else
                    alert("CPF é inválido.");
                break;
            case 14:
                campoValido = crmForm.ValidarCnpj(campoFormatado);
                if (campoValido)
                    resultado = campoFormatado.substr(0, 2) + '.' + campoFormatado.substr(2, 3) + '.' + campoFormatado.substr(5, 3) + '/' + campoFormatado.substr(8, 4) + '-' + campoFormatado.substr(12, 2);
                else
                    alert("CNPJ é inválido.");
                break;
            default:
                alert("Número de CPF ou CNPJ inválido.");
                break;
        }

        campo.setValue(resultado);
    }

    /**********************************
    Data:      16/03/2011
    Autor:     
    Descrição: Filtra um FilterLookup
    **********************************/
    crmForm.FilterLookup = function (attribute, url, param, ObjectTypeName) {
        if (param != null)
            url += "?" + param + "&orgname=" + Xrm.Page.context.getOrgUniqueName();
        else
            url += "?orgname=" + Xrm.Page.context.getOrgUniqueName();

        oImg = eval('attribute' + '.parentNode.childNodes[0]');

        oImg.onclick = function () {
            retorno = openStdDlg(url, null, 600, 450);

            if (typeof (retorno) != "undefined") {
                strValues = retorno.split('*|*');
                var lookupData = new Array();
                lookupItem = new Object();
                lookupItem.id = "{" + strValues[1] + "}";
                //lookupItem.type = parseInt(strValues[2]);
                lookupItem.entityType = ObjectTypeName;
                lookupItem.name = strValues[0];
                lookupData[0] = lookupItem;
                attribute.setValue(lookupData);
                attribute.fireOnChange();
            }

        };
    };

    /**********************************
    Data:      16/03/2011
    Autor:     
    Descrição: Filtra as cidades segundo a UF selecionada
    **********************************/
    crmForm.filtraUFCidade = function (campoUF, campoCidade) {
        var id = "";

        if (campoUF.getValue()) id = campoUF.getValue()[0].id;

        var oParam = "objectTypeCode=20009&filterDefault=false&_new_ufid=" + id;
        crmForm.FilterLookup(campoCidade, "/ISV/Tridea.Web.Helper/FilterLookup/FilterLookup.aspx", oParam, "new_cidade");

        if (campoUF.IsDirty) campoCidade.setValue(null);
    }

    /**********************************
    Data:      16/03/2011
    Autor:     Cleto May
    Descrição: Filtra as UF segundo o País selecionado
    **********************************/
    crmForm.filtraPaisEstado = function (campoPais, campoUF) {
        var id = "";

        if (campoPais.getValue()) id = campoPais.getValue()[0].id;

        var oParam = "objectTypeCode=20010&filterDefault=false&_new_paisid=" + id;
        crmForm.FilterLookup(campoUF, "/ISV/Tridea.Web.Helper/FilterLookup/FilterLookup.aspx", oParam, "new_uf");

        if (campoPais.IsDirty) campoUF.setValue(null);
    }

    /**********************************
    Data:      03/06/2011
    Autor:     Tiago Raupp
    Descrição: Limpa os campos abaixo
    **********************************/
    crmForm.desabalitaPais = function () {
        var pais = Xrm.Page.getAttribute("new_paisid");
        var cidade = Xrm.Page.getAttribute("new_cidadeid");
        var uf = Xrm.Page.getAttribute("new_ufid");
        var regional = Xrm.Page.getAttribute("new_regionalid");

        if (pais.getValue() != null) {
            cidade.setValue(null);
            uf.setValue(null);
            regional.setValue(null);
        }
    }

    crmForm.desabilitaUf = function () {
        var cidade = Xrm.Page.getAttribute("new_cidadeid");
        var uf = Xrm.Page.getAttribute("new_ufid");
        var regional = Xrm.Page.getAttribute("new_regionalid");

        if (uf.getValue() != null) {
            cidade.setValue(null);
            regional.setValue(null);
        }
    }

    crmForm.AcaoOrigemContato = function () {
        var origemContato = Xrm.Page.getAttribute("new_origem_contato").getValue();

        switch (origemContato) {
            case '1': //e-mail
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("none");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_cidadeid").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_ufid").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_paisid").setRequiredLevel("none");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_numero_endereco_principal").setRequiredLevel("none");
                break;
            case '2': //telefone
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("none");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_numero_endereco_principal").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_cidadeid").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_ufid").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_paisid").setRequiredLevel("recommended");
                break;
            case "3": // Portal
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_cidadeid").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_ufid").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_paisid").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("recommended");
                Xrm.Page.getAttribute("new_numero_endereco_principal").setRequiredLevel("recommended");
                break;
            default: //para todos os outros
                Xrm.Page.getAttribute("new_cpf").setRequiredLevel("none");
                Xrm.Page.getAttribute("telephone1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line1").setRequiredLevel("none");
                Xrm.Page.getAttribute("address1_line3").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_cidadeid").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_ufid").setRequiredLevel("none");
                Xrm.Page.getAttribute("new_paisid").setRequiredLevel("none");
                Xrm.Page.getAttribute("emailaddress1").setRequiredLevel("none");
                break;
        }
    }


    /**********************************
    Data:      17/08/2011
    Autor:     Tiago Raupp
    Descrição: Verifica se se o Cpf ou Cnpj
    estão cadastrado, se sim, abre o cadastro do mesmo.
    **********************************/
    crmForm.ValidarExistenciaDoContatoPor = function (cpfCnpj) {
        cpfCnpj = cpfCnpj.getValue();
        if (cpfCnpj == null) return;

        var comando = new RemoteCommand(ISOL_SERVICE_NAME, "PesquisarContatoPor", VENDAS_SERVICE_URL);
        comando.SetParameter("cpfCnpj", cpfCnpj);

        try {
            var execucao = comando.Execute();
        } catch (e) { alert(e.message); }

        if (execucao.Success) {
            var resultado = execucao.ReturnValue;

            if (resultado.Achou) {
                var existeDuplicidade = false;

                if (Xrm.Page.ui.getFormType() == 1) {
                    existeDuplicidade = true;
                } else {
                    var id = Xrm.Page.data.entity.getId().toLowerCase().replace("{", "").replace("}", "");
                    if (resultado.Id != id) {
                        existeDuplicidade = true;
                    }
                }

                if (existeDuplicidade) {
                    alert("Contato já cadastrado com este cnpj/cpf.");
                    window.open("/sfa/conts/edit.aspx?id=" + resultado.Id, "contact", "height=600,width=800,status=yes,resizable=yes");
                    window.close();
                }
            }
        }
    }

    crmForm.NFE = function () {

        var obrigatorio = 0;

        if (Xrm.Page.getAttribute("firstname").getValue() != null
            && Xrm.Page.getAttribute("firstname").getValue().substring(0, 3).toLowerCase() == "nfe")
            obrigatorio = 2;


        crmForm.SetFieldReqLevel("emailaddress1", obrigatorio);
        crmForm.SetFieldReqLevel("parentcustomerid", obrigatorio);
    }

    crmForm.TransferirMovimentacaoFidelidade = function () {
        try {
            var origem = Xrm.Page.data.entity.getId().toLowerCase().replace("{", "").replace("}", "");
            var destino = prompt('Informe o CPF ou CNPF', 'Sem mascara');

            if (!destino) return;

            var er = /^[0-9]+$/;
            if (!er.test(destino))
                throw { message: "O destino precisa estar prenchido corretamente, informe o CPF ou CNPJ (Apenas numeros)." }


            var comando = new RemoteCommand(FIDELIDADE_SERVICE_NAME, "TransferirMovimentacaoFidelidade", FIDELIDADE_SERVICE_URL);
            comando.SetParameter("origemId", origem);
            comando.SetParameter("destinoCpfCnpj", destino);


            alert("A operação esta sendo processada, ao concluir você ira receber uma mensagem!");

            var execucao = comando.Execute(function (response) {
                alert(response.ReturnValue);
            });

        } catch (e) { alert(e.message); }
    }

    crmForm.CriarResgateSistema = function () {

        try {
            var contatoId = Xrm.Page.data.entity.getId().toLowerCase().replace("{", "").replace("}", "");
            var nomeResgate = prompt('Informe o descricao do resgate');

            if (nomeResgate)
                if (nomeResgate.length > 100)
                    throw { message: "O nome deve conter menos de 100 caracteres!" }


            var quantidadePontos = prompt('Informe a quantidade de pontos');
            if (!quantidadePontos) return;

            var er = /^[0-9]+$/;
            if (!er.test(quantidadePontos))
                throw { message: "A quantidade de pontos deve conter apenas números!" }


            var comando = new RemoteCommand(FIDELIDADE_SERVICE_NAME, "CriarResgateSistema", FIDELIDADE_SERVICE_URL);
            comando.SetParameter("nome", nomeResgate);
            comando.SetParameter("quantidadePontos", quantidadePontos);
            comando.SetParameter("contatoId", contatoId);

            var execucao = comando.Execute();
            alert(execucao.ReturnValue);
        }
        catch (e) { alert(e.message); }
    }

    crmForm.ParticipaProgramaCanais = function () {
        //var value = Xrm.Page.getAttribute("new_crm2013.value");
        //return (value != "" && value != "0");
    }



    function ValidacoesProgramaCanais() {

        // Geral
        Xrm.Page.getControl("firstname").setDisabled(true);
        Xrm.Page.getControl("new_cpf").setDisabled(true);
        Xrm.Page.getControl("parentcustomerid").setDisabled(true);
        Xrm.Page.getControl("new_origem_contato").setDisabled(true);
        Xrm.Page.getControl("mobilephone").setDisabled(true);
        Xrm.Page.getControl("telephone1").setDisabled(true);
        Xrm.Page.getControl("telephone3").setDisabled(true);
        Xrm.Page.getControl("telephone2").setDisabled(true);
        Xrm.Page.getControl("new_ramal_telefone").setDisabled(true);
        Xrm.Page.getControl("new_ramal_comercial2").setDisabled(true);
        Xrm.Page.getControl("fax").setDisabled(true);
        Xrm.Page.getControl("new_ramal_fax").setDisabled(true);
        Xrm.Page.getControl("emailaddress1").setDisabled(true);
        Xrm.Page.getControl("emailaddress2").setDisabled(true);
        Xrm.Page.getControl("new_observacao").setDisabled(true);
        Xrm.Page.getControl("new_ramal_fax").setDisabled(true);
        Xrm.Page.getControl("new_ramal_fax").setDisabled(true);
        Xrm.Page.getControl("new_ramal_fax").setDisabled(true);
        Xrm.Page.getControl("new_ramal_fax").setDisabled(true);

        // Detalhes
        Xrm.Page.getControl("new_cargo").setDisabled(true);
        Xrm.Page.getControl("new_area").setDisabled(true);
        Xrm.Page.getControl("accountrolecode").setDisabled(true);
        Xrm.Page.getControl("managername").setDisabled(true);
        Xrm.Page.getControl("managerphone").setDisabled(true);
        Xrm.Page.getControl("assistantname").setDisabled(true);
        Xrm.Page.getControl("assistantphone").setDisabled(true);
        Xrm.Page.getControl("gendercode").setDisabled(true);
        Xrm.Page.getControl("familystatuscode").setDisabled(true);
        Xrm.Page.getControl("escolaridade").setDisabled(true);

        // Endereço
        Xrm.Page.getControl("address1_postalcode").setDisabled(true);
        Xrm.Page.getControl("address1_line1").setDisabled(true);
        Xrm.Page.getControl("new_numero_endereco_principal").setDisabled(true);
        Xrm.Page.getControl("address1_line2").setDisabled(true);
        Xrm.Page.getControl("address1_line3").setDisabled(true);
        Xrm.Page.getControl("new_paisid").setDisabled(true);
        Xrm.Page.getControl("new_ufid").setDisabled(true);
        Xrm.Page.getControl("new_cidadeid").setDisabled(true);
        Xrm.Page.getControl("new_regionalid").setDisabled(true);
    }

    /**********************************
    Ações do OnLoad.
    **********************************/
    crmForm.Notification(Xrm.Page.getAttribute("new_mensagem").getValue());
    crmForm.NFE();

    // Validaçoes acesso ao portal
    if (Xrm.Page.getAttribute("new_acessoportal").getValue() == 1) {
        Xrm.Page.getControl("new_acessoportal").setDisabled(true);
        Xrm.Page.getControl("new_login").setDisabled(true);
    } else {
        crmForm.obrigaCamposAcessoPortal();
    }

    // Esconde Guia
    crmForm.all.new_mensagem_c.parentElement.parentElement.style.display = 'none';

    // Validações por UN
    switch (crmForm.GetMyBusinessUnit()) {

        case "POS VENDA":
        case "POS VENDA MAXCOM":
            Xrm.Page.getAttribute("new_origem_contato").setRequiredLevel("recommended");
            Xrm.Page.getAttribute("customertypecode").setRequiredLevel("recommended");

            if (Xrm.Page.ui.getFormType() == 1) {
                Xrm.Page.getAttribute("new_origem_contato").setValue(2); // telefone = 2
            }

            crmForm.AcaoOrigemContato();
            break;
    }
}
function firstname_onchange() {
    crmForm.NFE();
}
function new_cpf_onchange() {
    crmForm.FormatarCpfCnpj(crmForm.all.new_cpf);
    crmForm.ValidarExistenciaDoContatoPor(crmForm.all.new_cpf);
}
function new_origem_contato_onchange() {
    if (crmForm.GetMyBusinessUnit() == 'POS VENDA' || crmForm.GetMyBusinessUnit() == 'POS VENDA MAXCOM')
        crmForm.AcaoOrigemContato();
}
function parentcustomerid_onchange() {

}
function mobilephone_onchange() {
    crmForm.phoneFormat(crmForm.all.mobilephone);
}
function telephone1_onchange() {
    crmForm.phoneFormat(crmForm.all.telephone1);
}
function telephone3_onchange() {
    crmForm.phoneFormat(crmForm.all.telephone3);
}
function telephone2_onchange() {
    crmForm.phoneFormat(crmForm.all.telephone2);
}
function fax_onchange() {
    crmForm.phoneFormat(crmForm.all.fax);
}
function emailaddress1_onchange() {

}
function managerphone_onchange() {
    var phone = Xrm.Page.getAttribute("managerphone");

    if (phone.value.length > 0) {
        if (phone.value.length < 10) {
            alert('Atenção ! Informe o Número do Telefone do Gerente com o DDD');
            phone.focus();
            return;
        }
    }

    phone.value = phoneFormat(phone.value);

    function phoneFormat(text) {
        var pattern = /[^1-9]*([1-9])[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*.*/;
        var mask = '($1$2) $3$4$5$6-$7$8$9$10';
        return text.replace(pattern, mask);
    }
}
function assistantphone_onchange() {
    var phone = Xrm.Page.getAttribute("assistantphone");

    if (phone.value.length > 0) {
        if (phone.value.length < 10) {
            alert('Atenção ! Informe o Número do Telefone do Assistente com o DDD');
            phone.focus();
            return;
        }
    }

    phone.value = phoneFormat(phone.value);

    function phoneFormat(text) {
        var pattern = /[^1-9]*([1-9])[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*.*/;
        var mask = '($1$2) $3$4$5$6-$7$8$9$10';
        return text.replace(pattern, mask);
    }
}
function address1_postalcode_onchange() {
    var cep = Xrm.Page.getAttribute("address1_postalcode");
    try {
        crmForm.PesquisarEnderecoPrincipalPor(cep);
    } catch (Error) {
        alert(Error);
        cep.focus();
    }
}
function new_paisid_onchange() {
    /**********************************
    Ações do OnChange.
    **********************************/

    // Função Filter Lookup
    crmForm.filtraPaisEstado(crmForm.all.new_paisid, crmForm.all.new_ufid);


    function LookupItem(typename) {
        this.id = null;
        this.nome = null;
        this.entityType = typename;
    }

    var pais = new LookupItem("new_paisid");
    pais.id = "{BC85DBEA-EC35-E011-BDAD-001CC0953230}";
    pais.nome = "Inglaterra";

    //Xrm.Page.getAttribute("new_paisid").setValue(pais);


    //********************************************
    crmForm.desabalitaPais();
}
function new_ufid_onchange() {
    /**********************************
    Ações do OnChange.
    **********************************/

    // Função Filter Lookup
    crmForm.filtraUFCidade(crmForm.all.new_ufid, crmForm.all.new_cidadeid);

    //*************************************
    crmForm.desabilitaUf();
}
function new_cidadeid_onchange() {
    /**********************************
    Autor: Cleto May   
    Data: 21/02/2011
    Descrição: Obtem a regional da cidade selecionada
    **********************************/
    crmForm.ObterRegionalDaCidade = function (cidadeid) {
        var resultado = null;

        var comando = new RemoteCommand(APOIO_SERVICE_NAME, "PesquisarRegionalPor", VENDAS_SERVICE_URL);

        comando.SetParameter("cidadeId", cidadeid);
        comando.SetParameter("organizacaoNome", Xrm.Page.context.getOrgUniqueName());

        var execucao = comando.Execute();

        if (execucao.Success) {
            resultado = execucao.ReturnValue;

            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return false;
            }
        }

        return resultado;
    }

    function CreateLookup(id, name, type) {
        var lookupData = new Array();
        var lookupItem = new Object();

        lookupItem.id = id;
        lookupItem.entityType = type;
        lookupItem.name = name;

        lookupData[0] = lookupItem;

        return lookupData;
    }

    /***********************************
    Ações OnChange
    ***********************************/
    if (Xrm.Page.getAttribute("new_cidadeid").getValue() != null) {
        var resultado = crmForm.ObterRegionalDaCidade(Xrm.Page.getAttribute("new_cidadeid").getValue()[0].id);
        if (resultado != false) {
            Xrm.Page.getAttribute("new_regionalid").setValue(CreateLookup(resultado.RegionalId, resultado.RegionalNome, "new_regional"));
        }
    }
}
function donotphone_onchange() {

}
function donotfax_onchange() {

}
function new_acessoportal_onchange() {
    crmForm.obrigaCamposAcessoPortal();
}
