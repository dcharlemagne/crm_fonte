/**********************************
    Variáveis Globais.
    **********************************/
var TipodeOcorrenciaNoCRM = ""; // Variavel global para alteração de "casetypecode"
var FormularioEstaEmModoDeCriacao = Xrm.Page.ui.getFormType() == 1;
var FormularioEstaSendoAlterado = Xrm.Page.ui.getFormType() == 2;
var validacaoAssunto = false;
var UN_DO_USUARIO = GetMyBusinessUnit();

var OCORRENCIA_ACAO_RESTITUICAO = "DFA33258-DB27-E111-99C0-00155DA71A0D";

String.prototype.replaceAt = function (index, char) {
    return this.substr(0, index) + char + this.substr(index + char.length);
}
String.prototype.padL = function (width, pad) {
    if (!width || width < 1)
        return this;
    if (!pad) pad = " ";
    var length = width - this.length;
    if (length < 1) return this.substr(0, width);
    return (String.repeat(pad, length) + this).substr(0, width);
}

String.repeat = function (chr, count) {
    var str = "";
    for (var x = 0; x < count; x++) { str += chr }
    return str;
}

function GetMyBusinessUnit() {
    var UserID = Xrm.Page.context.getUserId();
    Cols = ["businessunitid"];
    var Retrieved = XrmServiceToolkit.Soap.Retrieve("systemuser", UserID, Cols);
    var UnitObj = Retrieved.attributes["businessunitid"];
    var UnitName = UnitObj.name
    return UnitName;
}

function QueryString(variavel) {
    var variaveis = location.search.replace(/\x3F/, "").replace(/\x2B/g, " ").split("&")
    var nvar
    if (variaveis != "") {
        var qs = []
        for (var i = 0; i < variaveis.length; i++) {
            nvar = variaveis[i].split("=")
            qs[nvar[0]] = unescape(nvar[1])
        }
        return qs[variavel]
    }
    return null
}

function IsNullValue(value) {
    return (value == "" || value == null);
}

/** INI ********************************
Data:      03/09/2013
Autor:     Alberto Freitas
Descrição: Atribui dados do LiveChat a Ocorrência
*** INI *******************************/
function onLoadLiveChat() {
    try {
        // CREATE
        if (Xrm.Page.ui.getFormType() != 1)
            return;

        //Seta o protocolo no campo informado
        if (document.getElementById(QueryString("protocolField")) != null) {
            document.getElementById(QueryString("protocolField")).setValue(QueryString("protocolNumber"));
            document.getElementById(QueryString("protocolField")).ForceSubmit = true;
        }

        //Relaciona com a Tracking Criada
        if (!IsNullValue(QueryString("relationshipTrackingField"))) {
            if (document.getElementById(QueryString("relationshipTrackingField")) != null) {
                var oItems = new Array();
                var item = new Object();
                item.id = QueryString("trackingId");
                item.entityType = 'codek_livechat_tracking';
                item.name = 'LiveChat Tracking';
                oItems[0] = item;
                document.getElementById(QueryString("relationshipTrackingField")).setValue(oItems);
                document.getElementById(QueryString("relationshipTrackingField")).ForceSubmit = true;
            }
        }

        //Seta o Lookup de assunto parametrizado na ocorrência
        if (!IsNullValue(QueryString("subjectFieldName")) && !IsNullValue(QueryString("crmSubjectId")) && !IsNullValue(QueryString("crmSubjectName"))) {
            if (document.getElementById(QueryString("subjectFieldName")) != null) {
                var oItems = new Array();
                var item = new Object();
                item.id = QueryString("crmSubjectId");
                item.entityType = 'subject';
                item.name = QueryString("crmSubjectName");
                oItems[0] = item;
                document.getElementById(QueryString("subjectFieldName")).setValue(oItems);
            }
        }
    }
    catch (e) { }
}

function getUserId() {
    return Xrm.Page.context.getUserId();
}

/**********************************
Data:      
Autor:     
Descrição: Cria um Lookup
**********************************/
function CreateLookup(id, name, type) {
    var lookupData = new Array();
    var lookupItem = new Object();

    lookupItem.id = id;
    lookupItem.entityType = type;
    lookupItem.name = name;

    lookupData[0] = lookupItem;

    return lookupData;
}

/**********************************
Descrição: Determina as opções do picklist a serem mostradas
**********************************/
function RangePickList(pPckObjeto, pPckAtributo, pRangeInicio, pRangeFim) {
    if (pRangeInicio > -1 && pRangeFim > -1) {
        var tempArray = new Array();
        var index = 0;
        var options = pPckAtributo.getOptions();
        //Loop dos Valores do Picklist
        for (var i = 0; i < options.length; i++) {
            //Verifica se o Range é Valido conforme Parametro
            if (options[i].value >= pRangeInicio &&
                options[i].value <= pRangeFim) {
                //Adiciona o elemento no Picklist
                tempArray[index] = options[i];
                index++;
            }
            else
                try { pPckObjeto.removeOption(options[i].value); } catch (e) { }
        }
    }
    else {
        pPckObjeto.clearOptions();
    }
}

/**********************************
Data:      
Autor:     
Descrição: Obriga o preenchimento do campo atividade executada quando o campo Data e Hora da conclusão estiver diferente de NULL
 **********************************/
function ObrigaPreencheerCampoAtividade() {
    if (Xrm.Page.getAttribute("new_data_hora_conclusao").getValue() != null) {
        Xrm.Page.getAttribute("new_atividade_executada").setRequiredLevel("required");
    }
    else {
        Xrm.Page.getAttribute("new_atividade_executada").setRequiredLevel("none");
    }
}

/**********************************
Descrição: Formatar a data e a hora
**********************************/
function FormataDataHora(data) {
    var day = data.getDate();
    var month = data.getMonth() + 1;
    var year = data.getFullYear();
    var min = data.getMinutes();
    var hour = data.getHours();
    day = day.toString().padL(2, "0");
    month = month.toString().padL(2, "0");
    min = min.toString().padL(2, "0");
    hour = hour.toString().padL(2, "0");
    data = year + '-' + month + '-' + day + 'T' + hour + ':' + min + ':00.0Z';
    return data;
}

/**********************************
Data:      01/12/2010
Autor:     Clausio Elmano de Oliveira
Descrição: Valida vigência do Contrato
**********************************/
function ValidarVigenciaDoContrato(contratoId, clienteId, enderecoId, dataInicioVigencia) {

    /*** EXECUTA O WEBSERVICE ***/
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "VerificarDataDeVigencia");

    var data = "1900-01-01";
    if (dataInicioVigencia != null) {
        var day = dataInicioVigencia.getDate();
        var month = dataInicioVigencia.getMonth() + 1;
        var year = dataInicioVigencia.getFullYear();
        day = day.toString().padL(2, "0");
        month = month.toString().padL(2, "0");
        data = year + '-' + month + '-' + day;
    }
    Util.funcao.SetParameter("contratoId", contratoId);
    Util.funcao.SetParameter("clienteId", clienteId);
    Util.funcao.SetParameter("enderecoId", enderecoId);
    Util.funcao.SetParameter("dataInicioVigencia", data);
    Util.funcao.SetParameter("organizacao", Xrm.Page.context.getOrgUniqueName());

    var retorno = Util.funcao.Execute();

    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];
        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False" && dataInicioVigencia != null && FormularioEstaEmModoDeCriacao) {
            alert($(data).find("MensagemDeErro").text());
            return false;
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
    return retorno;
}

/**********************************
Data:      15/12/2010
Autor:     Clausio Elmano de Oliveira
Descrição: Retorna o responsavel técnico
**********************************/
function AbreOS() {
    if (FormularioEstaEmModoDeCriacao) {
        alert('Não é possivel enviar e-mail de uma nova ocorrência!');
        return false;
    }
    var pagina = "/activities/email/edit.aspx?pId=";
    pagina += Xrm.Page.data.entity.getId();
    pagina += "&pType=112";
    window.open(pagina, '', 'height = 600, width = 800, scrollbars=yes, location=no, toolbar=no, menubar=no');
}

function HoraAtual() {
    var dateTime = new Date().getTime();
    return dateTime.toString();
}

/**********************************
Data:      15/12/2010
Autor:     Clausio Elmano de Oliveira
Descrição: Retorna o responsável técnico
**********************************/
function ObterResponsavelTecnico(contratoId) {
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "PesquisarResponsavelTecnicoPor");

    Util.funcao.SetParameter("contratoId", contratoId);
    Util.funcao.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());
    var retorno = Util.funcao.Execute();
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];
        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
    Xrm.Page.getAttribute("new_descricao_produto").setValue($(data).find("Descricao").text());

    return retorno;
}
/**********************************
        Data:      28/12/2010
        Autor:     Clausio Elmano de Oliveira
        Descrição: Calcula diferença das horas
        **********************************/
function CalculaDiferencaEntreDatas(dataInicial, dataFinal, contratoId, uf, cidade) {

    //Configurações de propriedades
    dataInicial = FormataDataHora(dataInicial);
    dataFinal = FormataDataHora(dataFinal);

    //Configuração do serviço web
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "CalcularDiferencaEntreDatas");

    //Atribuição dos paramentros
    Util.funcao.SetParameter("dataInicial", dataInicial);
    Util.funcao.SetParameter("dataFinal", dataFinal);
    Util.funcao.SetParameter("contratoId", contratoId);
    Util.funcao.SetParameter("uf", uf != null ? uf : "");
    Util.funcao.SetParameter("cidade", cidade != null ? cidade : "");
    Util.funcao.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());

    //Execução do serviço web
    var retorno = Util.funcao.Execute();

    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];

        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";

        return $(data).find("Minutos").text();
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
}

/**********************************
Data:      01/12/2010
Autor:     Clausio Elmano de Oliveira
Descrição: Retorna a descrição do cliente participante por contrato
**********************************/
function ObterDescricaoClienteParticipante(contratoId, clienteId) {
    //Configuração do serviço web
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "PesquisarClienteParticipantePor");
    Util.funcao.SetParameter("contratoId", contratoId);
    Util.funcao.SetParameter("clienteId", clienteId);
    Util.funcao.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());
    //Execução do serviço web
    var retorno = Util.funcao.Execute();

    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];

        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";

        return $(data).find("Minutos").text();
    }
    if (typeof ($(data).find("Descricao").text()) != "object")
        Xrm.Page.getAttribute("new_descricao_produto").setValue($(data).find("Descricao").text());

    return retorno;
}

/**********************************
Data:      21/02/2011  
Autor:     Gabriel Dias Junckes
Descrição: Pesquisa o assunto por Guid
**********************************/
function PesquisaAssuntoPor(assuntoId) {
    debugger;
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Apoio, "PesquisarAssuntoPor");
    Util.funcao.SetParameter("assuntoId", assuntoId);
    Util.funcao.SetParameter("organizacaoNome", Xrm.Page.context.getOrgUniqueName());
    //Execução do serviço web
    var retorno = Util.funcao.Execute();
    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];

        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
    return data;
}

/**********************************
Data:      28/01/2011
Autor:     Gabriel Dias Junckes
Descrição: Valida o CPF
Retorna:   true ou false
**********************************/
function ValidarCpf(cpf) {
    var numeros, digitos, soma, i, resultado, digitos_iguais;
    digitos_iguais = 1;
    if (cpf.length < 11)
        return false;
    for (i = 0; i < cpf.length - 1; i++) {
        if (cpf.charAt(i) != cpf.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    }
    if (!digitos_iguais) {
        numeros = cpf.substring(0, 9);
        digitos = cpf.substring(9);
        soma = 0;
        for (i = 10; i > 1; i--)
            soma += numeros.charAt(10 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(0))
            return false;
        numeros = cpf.substring(0, 10);
        soma = 0;
        for (i = 11; i > 1; i--)
            soma += numeros.charAt(11 - i) * i;
        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (resultado != digitos.charAt(1))
            return false;
        return true;
    }
    else
        return false;
}

/**********************************
Data:      28/01/2011
Autor:     Gabriel Dias Junckes
Descrição: Formata o campo CPF
**********************************/
function FormatarCpf(campo) {
    var exp = /\-|\.|\/|\(|\)| /g
    var cpfFormatado = campo.getValue();
    if (cpfFormatado == null)
        return;
    cpfFormatado = cpfFormatado.replace(exp, "");
    var cpfEValido = ValidarCpf(cpfFormatado);
    if (cpfEValido) {
        cpfFormatado = cpfFormatado.substr(0, 3) + '.' + cpfFormatado.substr(3, 3) + '.' + cpfFormatado.substr(6, 3) + '-' + cpfFormatado.substr(9, 2);
        campo.setValue(cpfFormatado);
    }
    else {
        alert("Número de CPF inválido.");
        campo.setValue(null);
        return;
    }
}

/**********************************
Data:      24/02/2001
Autor:     Cleto May
Descrição: Formata o CNPJ
**********************************/
function FormatarCNPJ(campo) {
    var cnpjFormatado = campo.getValue();
    cnpjFormatado = cnpjFormatado.substr(0, 2) + '.' + cnpjFormatado.substr(2, 3) + '.' + cnpjFormatado.substr(5, 3) + '/' + cnpjFormatado.substr(8, 4) + '-' + cnpjFormatado.substr(12, 2);
    campo.setValue(cnpjFormatado);
}

/**********************************
Data:      24/02/2001
Autor:     
Descrição: Valida o CNPJ
**********************************/
function ValidarCnpj(cnpj) {
    var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
    digitos_iguais = 1;
    if (cnpj.length < 14 && cnpj.length < 15)
        return false;
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
        if (resultado != digitos.charAt(0))
            return false;
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
        if (resultado != digitos.charAt(1))
            return false;
        return true;
    }
    else
        return false;
}

/**********************************
Descrição: Formata o cnpj
**********************************/
function FormatarCnpj(campo) {
    var exp = /\-|\.|\/|\(|\)| /g
    var cnpjFormatado = campo.getValue();
    if (cnpjFormatado == null)
        return;
    /*Somente números*/
    var cnpjFormatado = cnpjFormatado.replace(exp, "");
    var cnpjEValido = ValidarCnpj(cnpjFormatado);
    if (cnpjEValido) {
        cnpjFormatado = cnpjFormatado.replace(exp, "");
        cnpjFormatado = cnpjFormatado.substr(0, 2) + '.' + cnpjFormatado.substr(2, 3) + '.' + cnpjFormatado.substr(5, 3) + '/' + cnpjFormatado.substr(8, 4) + '-' + cnpjFormatado.substr(12, 2);
    }
    else {
        alert("CNPJ Inválido.");
        campo.focus();
        return;
    }
    campo.setValue(cnpjFormatado);
}

/**********************************
Data:      24/02/2001
Autor:     Cleto May
Descrição: Verifica tipo: CPF ou CNPJ
**********************************/
function VerificaTipoDeCampo() {
    if (Xrm.Page.getAttribute("new_cpf_cnpj").getValue() == null) return;

    while (Xrm.Page.getAttribute("new_cpf_cnpj").getValue().indexOf(".") > -1) Xrm.Page.getAttribute("new_cpf_cnpj").setValue(Xrm.Page.getAttribute("new_cpf_cnpj").getValue().replace(".", ""));
    while (Xrm.Page.getAttribute("new_cpf_cnpj").getValue().indexOf("/") > -1) Xrm.Page.getAttribute("new_cpf_cnpj").setValue(Xrm.Page.getAttribute("new_cpf_cnpj").getValue().replace("/", ""));
    while (Xrm.Page.getAttribute("new_cpf_cnpj").getValue().indexOf("-") > -1) Xrm.Page.getAttribute("new_cpf_cnpj").setValue(Xrm.Page.getAttribute("new_cpf_cnpj").getValue().replace("-", ""));

    switch (Xrm.Page.getAttribute("new_cpf_cnpj").getValue().length) {
        case 11:
            FormatarCpf(Xrm.Page.getAttribute("new_cpf_cnpj"));
            return;
            break;
        case 14:
            FormatarCnpj(Xrm.Page.getAttribute("new_cpf_cnpj"))
            return;
            break;
    }

    Xrm.Page.getAttribute("new_cpf_cnpj").setValue("");
    alert("Campo de CPF/CNPJ é invalido");
}

/**********************************
Data:      24/02/2001
Autor:     Cleto May
Descrição: Pesquisa dados (nome cpf/cnpj) do cliente selecionado
**********************************/
function PesquisaRegistro() {
    var registro = null;
    if (Xrm.Page.getAttribute("customerid").getValue() != null) {
        var id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
        switch (Xrm.Page.getAttribute("customerid").getValue()[0].typename) {
            case "account":
                registro = PesquisarClientePor(id);
                if (registro != null) Xrm.Page.getAttribute("new_cpf_cnpj").setValue($(registro).find("Cnpj").text());
                break;
            case "contact":
                registro = PesquisarContatoPor(id);
                if (registro != null) Xrm.Page.getAttribute("new_cpf_cnpj").setValue($(registro).find("CPF").text());
                break;
        }

    }
    return registro;
}
/**********************************
Data:      03/03/2011
Descrição: Mascara de telefone
**********************************/
function phoneFormat(campo) {
    var phone = campo.getValue();
    if (campo.getValue() != null) {
        var pattern = /[^1-9]*([1-9])[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*(\d{1})[^\d]*.*/;
        var mask = '($1$2) $3$4$5$6-$7$8$9$10';
        var valorFormatado = phone.replace(pattern, mask);
        if (phone.length > 0) {
            if (phone.length < 10) {
                alert('Atenção ! Informe o Número com o DDD');
                campo.setValue(null);
                return;
            }
        }
        campo.setValue(valorFormatado);
    }
}

/**********************************		
Data:      25/02/2001
Autor:     Cleto May
Descrição: Verificacao de campos para diferentes tipos da ocorrencia
**********************************/
function VerificacaoTiposDaOcorrencia() {
    var TipodeOcorrencia = Xrm.Page.getAttribute("casetypecode").getValue();
    switch (TipodeOcorrencia) {
        case "300003":
        case "300005":
        case "300006":
        case "300007":
        case "300008":
            Xrm.Page.getControl("new_nome_nf").getParent().setVisible(true);
            Xrm.Page.getControl("new_autorizadaid").getParent().setVisible(true);
            Xrm.Page.getControl("new_visita_tecnica").getParent().setVisible(true);
            Xrm.Page.getControl("new_emitente").getParent().setVisible(true);
            Xrm.Page.getControl("new_causa_asstec").getParent().setVisible(true);
            break;

        case "300000":
        case "300001":
        case "300002":
        case "300004":
            Xrm.Page.getControl("new_nome_nf").getParent().setVisible(false);
            Xrm.Page.getControl("new_autorizadaid").getParent().setVisible(false);
            Xrm.Page.getControl("new_visita_tecnica").getParent().setVisible(false);
            Xrm.Page.getControl("new_emitente").getParent().setVisible(false);
            Xrm.Page.getControl("new_causa_asstec").getParent().setVisible(false);
            break;

        default:
            Xrm.Page.getControl("new_nome_nf").getParent().setVisible(true);
            Xrm.Page.getControl("new_autorizadaid").getParent().setVisible(true);
            Xrm.Page.getControl("new_visita_tecnica").getParent().setVisible(true);
            Xrm.Page.getControl("new_emitente").getParent().setVisible(true);
            Xrm.Page.getControl("new_causa_asstec").getParent().setVisible(true);
            break;
    }
    Xrm.Page.ui.tabs.get("tab_7").setDisplayState('collapsed');
    Xrm.Page.ui.tabs.get("tab_13").setDisplayState('expanded');
}

/**********************************
Data:      30/03/2011
Autor:     Cleto May
Descrição: Pesquisar o contato por id
**********************************/
function PesquisarContatoPor(contatoId) {

    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "PesquisarContatoPor");
    Util.funcao.SetParameter("contatoId", contatoId);
    //Execução do serviço web
    var retorno = Util.funcao.Execute();

    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];

        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
    return retorno;
}


/**********************************
Data:      30/03/2011
Autor:     Cleto May
Descrição: Pesquisar o cliente por id
**********************************/
function PesquisarClientePor(clienteId) {

    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Apoio, "PesquisarClientePor");
    Util.funcao.SetParameter("clienteId", clienteId);
    //Execução do serviço web
    var retorno = Util.funcao.Execute();
    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];
        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
    return retorno;
}

/**********************************
Data:      14/03/2011
Autor:     
Descrição: Define varios intervalos para o RangePickList
**********************************/
function ArrayRangePickList(pPckObjeto, pPckAtributo, pRangeInicio, pRangeFim) {

    var arrayOpcoes = new Array();
    var quantidadeOpcoes = 0;
    var options = pPckAtributo.getOptions();
    //Monta a Sequencia conforme o Range de Inicio e Fim
    //--------------------------------------------------
    for (var i = 0; i < options.length; i++) {
        for (var j = pRangeInicio[i]; j <= pRangeFim[i]; j++) {
            arrayOpcoes[quantidadeOpcoes] = j;
            quantidadeOpcoes++;
        }
    }

    //Identifica os Elementos do PickList que fazem parte do Range da Sequencia
    //-------------------------------------------------------------------------
    var arrayIndexOpcoes = new Array();
    var k = 0;
    for (var i = 0; i < options.length; i++) {
        for (var j = 0; j < arrayOpcoes.length; j++) {
            if (options[i].value == arrayOpcoes[j]) {
                arrayIndexOpcoes[k] = i;
                k++;
            }
        }
    }

    //Identifica os Elementos do Picklist que devem ser Excluido
    //----------------------------------------------------------
    var excluir = false;
    for (var i = (options.length - 1) ; i >= 0; i--) {
        excluir = true;

        //Para cada Elemento do PickList, Busca no Array se deve ficar
        for (var j = 0; j < arrayIndexOpcoes.length; j++) {
            if (i == arrayIndexOpcoes[j]) {
                excluir = false;
            }
        }
        //Elemento do PickList com Exclusao
        if (excluir) {
            pPckObjeto.removeOption(options[i].value);
        }
    }
}

/**********************************
Data:      27/10/2011
Descrição: Retorna as notificação das informações do cadastro de Contato
**********************************/
function NotificacaoCadastroContato(contatoId) {
    debugger;
    var mensage = "";
    var resultado = PesquisarContatoPor(contatoId);
    if (!resultado) return mensage;
    var mensageAux = "";
    if ($(resultado).find("TipoRelacaoNome").text() == "" || $(resultado).find("TipoRelacaoNome").text() == null) mensageAux += "Tipo de Relação";
    if ($(resultado).find("Nome").text() == "" || $(resultado).find("Nome").text() == null) mensageAux += " Nome,";
    if ($(resultado).find("CPF").text() == "" || $(resultado).find("CPF").text() == null) mensageAux += " CPF,";
    if ($(resultado).find("Email").text() == "" || $(resultado).find("Email").text() == null) mensageAux += " Email,";
    if ($(resultado).find("Telefone").text() == "" || $(resultado).find("Telefone").text() == null) mensageAux += " Telefone,";
    if ($(resultado).find("ClienteId").text() == "" || $(resultado).find("ClienteId").text() == null) mensageAux += " Cliente,";
    if ($(resultado).find("Endereco").text() == "" || $(resultado).find("Endereco").text() == null) mensageAux += " Endereco,";
    if ($(resultado).find("NumeroEndereco").text() == "" || $(resultado).find("NumeroEndereco").text() == null) mensageAux += " Numero Endereco,";
    if ($(resultado).find("CidadeId").text() == "" || $(resultado).find("CidadeId").text() == null) mensageAux += " Bairro,";
    if ($(resultado).find("UfId").text() == "" || $(resultado).find("UfId").text() == null) mensageAux += " UF,";
    if ($(resultado).find("PaisId").text() == "" || $(resultado).find("PaisId").text() == null) mensageAux += " Pais,";
    if (mensageAux != "") {
        mensageAux = mensageAux.replaceAt(mensageAux.length - 1, " ");
        mensage = "Dados do contato estão incompletos (" + mensageAux + ")";
    }
    return mensage;
}


/**********************************
Data:      31/10/2011
Descrição: Atualiza o campo new_arvore_assunto 
**********************************/
function AtualizaArvoreAssunto(id) {
    debugger;
    var resultado = PesquisaAssuntoPor(id);
    if (resultado)
        Xrm.Page.getAttribute("new_arvore_assunto").setValue($(resultado).find("EstruturaAssunto").text());

    if ($(resultado).find("TemAssuntoFilho").text() == true)
        validacaoAssunto = true;
    else
        validacaoAssunto = false;
}

/**********************************
Data:      17/02/2012
Autor:     Tiago Raupp
Descrição: Busca WebService Congigurações de Sistemas valor restituição de ocorrências
**********************************/
function VerificaCampoAcaoFinal() {
    var resultado = {};
    resultado.Valor = OCORRENCIA_ACAO_RESTITUICAO;
    return resultado;
}

/**********************************
Data:      27/10/2011
Autor:     Tiago Raupp
Descrição: Verifica se acao final é restituição de valor
**********************************/
function AlterarCampoRestituicao() {
    var verifica = true;
    var acao = Xrm.Page.getAttribute("new_acao_final2").getValue();
    var dataRestituicao = Xrm.Page.getAttribute("new_data_restituicao");
    var valorRestituicao = Xrm.Page.getAttribute("new_valor_restituicao");
    var resultado = "{" + VerificaCampoAcaoFinal().Valor + "}";
    if (acao == null)
        verifica = false;
    else if (resultado != acao[0].id)
        verifica = false;
    if (verifica) {
        Xrm.Page.getControl("new_data_restituicao").setVisible(true);
        Xrm.Page.getControl("new_valor_restituicao").setVisible(true);
        Xrm.Page.getAttribute("new_data_restituicao").setRequiredLevel("recommended");
        Xrm.Page.getAttribute("new_valor_restituicao").setRequiredLevel("recommended");
    }
    else {
        dataRestituicao.setValue(null);
        valorRestituicao.setValue(null);
        Xrm.Page.getControl("new_data_restituicao").setVisible(false);
        Xrm.Page.getControl("new_valor_restituicao").setVisible(false);
        Xrm.Page.getAttribute("new_data_restituicao").setRequiredLevel("none");
        Xrm.Page.getAttribute("new_valor_restituicao").setRequiredLevel("none");
    }
}

/**********************************
Data: 09/09/2011     
Autor: Marcelo Ferreira de Láias   
Descrição: Integração com os Correios
**********************************/
function ObterCodigoAutorizacaoDePostagem() {
    if (!ValidacaoParaCorreios) return;

    //Configuração do serviço web
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "GerarCodigoAutorizacaoDePostagem");

    Util.funcao.SetParameter("ocorrenciaId", Xrm.Page.data.entity.getId());
    Util.funcao.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());
    //Execução do serviço web
    var retorno = Util.funcao.Execute();

    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];

        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";

        window.location.reload(true);
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
}

/**********************************
Data: 09/09/2011     
Autor: Marcelo Ferreira de Láias   
Descrição: Integração com os Correios
**********************************/
function AtualizarHistoricoPostagemCorreios() {
    if (!ValidacaoParaCorreios) return;

    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "ObterHistoricoDePostagem");
    Util.funcao.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());
    Util.funcao.SetParameter("ocorrenciaId", Xrm.Page.data.entity.getId());
    //Execução do serviço web
    var retorno = Util.funcao.Execute();
    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];
        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            alert("Número de objeto de Postagem nos Correios não encontrado."); return "0";

        window.location.reload(true);
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
}

/**********************************
Data: 09/09/2011     
Autor: Marcelo Ferreira de Láias   
Descrição: Integração com os Correios
**********************************/
function ValidacaoParaCorreios() {
    if (FormularioEstaEmModoDeCriacao) {
        alert("Não é possível executar esta ação quando o formulário estiver em modo de criação.");
        return false;
    }
    if (Xrm.Page.data.entity.getIsDirty()) {
        alert("Por favor, salve todas as alterações na Ocorrência para continuar.");
        return false;
    }
    if (Xrm.Page.getAttribute("new_tipo_coleta_postagem").getValue() == null) {
        alert("Selecione um tipo de E-Tiket para continuar.");
        return false;
    }
    return true;
}

function customerid_onchange() {
    AcaoCustomerId();
}

/**********************************
Data:      27/10/2011
Descrição: Funções do CustomerId
**********************************/
function AcaoCustomerId() {
    // Se campo for null limpa campos e sai da ação
    if (Xrm.Page.getAttribute("customerid").getValue() == null) {
        Xrm.Page.getAttribute("new_nome_nf").setValue(null);
        Xrm.Page.getAttribute("new_cpf_cnpj").setValue(null);
        return;
    }
    var id = Xrm.Page.getAttribute("customerid").getValue()[0].id;
    Xrm.Page.getAttribute("new_nome_nf").setValue(Xrm.Page.getAttribute("customerid").getValue()[0].name);
    var empresa = PesquisaRegistro();
    Xrm.Page.getAttribute("new_empresa_executanteid").setValue($(empresa).find("EmpresaExecutante").text());

    if (Xrm.Page.getAttribute("customerid").getValue()[0].typename == "contact") {
        var mensage = NotificacaoCadastroContato(id);
        Xrm.Page.ui.setFormNotification(mensage, "INFORMATION", "1");
    }
    addCustomViewContrato();
}

function casetypecode_onchange() {
    if (Xrm.Page.getAttribute("casetypecode").getValue() == null) return;
    /* 
    Autor: Clausio Elmano de Oliveira
    Data: 07/12/2010
    Descrição: Verificando as ranges de atualização
    */
    if (TipodeOcorrenciaNoCRM != "") {
        var TipodeOcorrenciaNova = Xrm.Page.getAttribute("casetypecode").getValue();
        // Dentro do RANGE do SLA
        if ((TipodeOcorrenciaNoCRM >= 200090) && (TipodeOcorrenciaNoCRM <= 200099)) {
            if ((TipodeOcorrenciaNova >= 200090) && (TipodeOcorrenciaNova <= 200099)) {
                //alert('Dentro da range')
            }
            else {
                Xrm.Page.getAttribute("casetypecode").setValue(TipodeOcorrenciaNoCRM);
                alert('Ocorrência em operação, selecione um tipo de ocorrência válido para SLA!');
            }
        }
        else {  // Fora do RANGE do SLA
            if ((TipodeOcorrenciaNova >= 200090) && (TipodeOcorrenciaNova <= 200099)) {
                Xrm.Page.getAttribute("casetypecode").setValue(TipodeOcorrenciaNoCRM);
                alert('Ocorrência em operação, selecione um tipo de ocorrência que não possua SLA!');
            }
        }
    }

    //VerificacaoTiposDaOcorrencia(); Transformado em regra de negócio - Robson 08/12/2017
    Xrm.Page.getControl("new_emitente").setVisible(false);
    if (Xrm.Page.getAttribute("new_emitente").getValue() != null) {
        if (Xrm.Page.getAttribute("new_emitente").getValue() != "")
            Xrm.Page.getControl("new_emitente").setVisible(true);
    }
}

function subjectid_onchange() {
    debugger;
    if (Xrm.Page.getAttribute("subjectid").getValue() == null) {
        Xrm.Page.getAttribute("new_arvore_assunto").getValue() == null;
        return;
    }
    var resultado = PesquisaAssuntoPor(Xrm.Page.getAttribute("subjectid").getValue()[0].id);
    Xrm.Page.getAttribute("new_arvore_assunto").setValue($(resultado).find("EstruturaAssunto").text());

    if ($(resultado).find("TemAssuntoFilho").text() == true)
        validacaoAssunto = true;
    else
        validacaoAssunto = false;

    obterRelacaoAssuntoAcao(Xrm.Page.getAttribute("subjectid").getValue());
}

/********************************************************************
    Data:      03/09/2018
    Autor:     Jaime Campos
    Descrição: Obtem a lista de ações vinculadas ao assunto
********************************************************************/
function obterRelacaoAssuntoAcao(assunto) {
    if (assunto != null) {
        var query = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                    "<entity name='itbc_assunto_acao'>" +
                        "<attribute name='itbc_name' />" +
                        "<attribute name='itbc_assunto_id' />" +
                        "<attribute name='itbc_acao_id' />" +
                        "<order attribute='itbc_name' descending='false' />" +
                        "<filter type='and'>" +
                            "<condition attribute='itbc_assunto_id' operator='eq' uiname='" + assunto[0].name + "' uitype='subject' value='" + assunto[0].id + "' />" +
                        "</filter>" +
                    "</entity>" +
                "</fetch>";

        var lista = XrmServiceToolkit.Soap.Fetch(query);

        if (lista.length > 0) {
            var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                                "<entity name='new_acao'>" +
                                    "<attribute name='new_name' />" +
                                    "<attribute name='new_codigo' />" +
                                    "<attribute name='new_acaoid' />" +
                                    "<order attribute='new_name' descending='false' />" +
                                    "<link-entity name='itbc_assunto_acao' from='itbc_acao_id' to='new_acaoid' alias='ab'>" +
                                        "<filter type='and'>" +
                                            "<condition attribute='itbc_assunto_id' operator='eq' uiname='" + assunto[0].name + "' uitype='subject' value='" + assunto[0].id + "' />" +
                                        "</filter>" +
                                    "</link-entity>" +
                                "</entity>" +
                            "</fetch>";

            var layoutXml = '<grid name="resultset" object="1010" jump="title" select="1" icon="1" preview="1"> <row name="result" id="new_acaoid"> <cell name="new_name" width="350"/> <cell name="new_codigo" width="100"/> </row> </grid>';

            var entityName = "new_acao";
            var viewDisplayName = "Assunto x Ação";
            var viewId = Xrm.Page.getControl("new_acaoid").getDefaultView();

            Xrm.Page.getControl("new_acaoid").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
        }
    }
}


/********************************************************************
    Data:      14/02/2011
    Autor:     Carlos Roweder Nass
    Descrição: Popula os campos conforme número de série do produto
********************************************************************/
function productserialnumber_onchange() {
    if (Xrm.Page.getAttribute("productserialnumber").getValue() != null) {
        //Configuração do serviço web
        Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "PesquisarSerieDoProdutoPor");

        Util.funcao.SetParameter("numeroDeSerie", Xrm.Page.getAttribute("productserialnumber").getValue());
        Util.funcao.SetParameter("nomeDaOrganizacao", Xrm.Page.context.getOrgUniqueName());

        //Execução do serviço web
        var retorno = Util.funcao.Execute();

        if (retorno["Success"] == true) {
            var data = retorno['ReturnValue'];
            if ($(data).find("Achou").text()) {

                Xrm.Page.getAttribute("new_emitente").setValue("");
                Xrm.Page.getAttribute("new_numero_nota_fiscal").setValue("");
                Xrm.Page.getAttribute("new_data_compra_intelbras").setValue(null);
                Xrm.Page.getAttribute("new_data_fabricacao_produto").setValue(null);
                Xrm.Page.getAttribute("new_ordem_produto").setValue("");
                Xrm.Page.getAttribute("new_componentes").setValue("");
                Xrm.Page.getAttribute("new_nr_pedido").setValue("");
                Xrm.Page.getAttribute("productid").setValue(null);
                Xrm.Page.getControl("new_emitente").setVisible(false);

                if (($(data).find("NumeroPedido").text()) != "[object Object]") {
                    Xrm.Page.getAttribute("new_nr_pedido").setValue("" + $(data).find("NumeroPedido").text());
                    Xrm.Page.getControl("new_emitente").setVisible(true);
                }

                if (($(data).find("NomeCliente").text()) != "[object Object]")
                    Xrm.Page.getAttribute("new_emitente").setValue($(data).find("NomeCliente").text());

                if (($(data).find("NumeroNotaFiscal").text()) != "[object Object]")
                    Xrm.Page.getAttribute("new_numero_nota_fiscal").setValue($(data).find("NumeroNotaFiscal").text());

                if (($(data).find("Ordem").text()) != "[object Object]")
                    Xrm.Page.getAttribute("new_ordem_produto").setValue("" + ($(data).find("Ordem").text()));

                if (($(data).find("Componentes").text()) != "[object Object]")
                    Xrm.Page.getAttribute("new_componentes").setValue($(data).find("Componentes").text());

                if (($(data).find("Celula").text()) != "[object Object]")
                    Xrm.Page.getAttribute("new_celula").setValue($(data).find("Celula").text());


                if (($(data).find("DataEmissaoNotaFiscal").text()) != "[object Date]") {
                    var date = new Date($(data).find("DataEmissaoNotaFiscal").text());
                    if (isNaN(date) == false)
                        Xrm.Page.getAttribute("new_data_compra_intelbras").setValue(date);
                }

                if (($(data).find("DescricaoProduto").text()) != "[object Date]") {
                    var LookupItem = new Object();
                    LookupItem.name = $(data).find("DescricaoProduto").text();
                    LookupItem.id = $(data).find("IdProduto").text();
                    LookupItem.entityType = "product";
                    Xrm.Page.getAttribute("productid").setValue([LookupItem]);
                }

                if (($(data).find("DataFabricacaoProduto").text()) != "[object Date]") {
                    var date = new Date($(data).find("DataFabricacaoProduto").text());
                    if (isNaN(date) == false)
                        Xrm.Page.getAttribute("new_data_fabricacao_produto").setValue(date);
                }
            }
        }

        if (Xrm.Page.getAttribute("ticketnumber").getValue() != null) {
            Xrm.Page.data.entity.save();
        }
    }
}

function new_cpf_cnpj_onchange() {
    if (Xrm.Page.getAttribute("new_cpf_cnpj").getValue() != null) {
        VerificaTipoDeCampo();
    }
}

function new_cnpj_loja_onchange() {
    if (Xrm.Page.getAttribute("new_cnpj_loja").getValue() != null) {
        while (Xrm.Page.getAttribute("new_cnpj_loja").getValue().indexOf(".") > -1) Xrm.Page.getAttribute("new_cnpj_loja").setValue(Xrm.Page.getAttribute("new_cnpj_loja").getValue().replace(".", ""));
        while (Xrm.Page.getAttribute("new_cnpj_loja").getValue().indexOf("/") > -1) Xrm.Page.getAttribute("new_cnpj_loja").setValue(Xrm.Page.getAttribute("new_cnpj_loja").getValue().replace("/", ""));
        while (Xrm.Page.getAttribute("new_cnpj_loja").getValue().indexOf("-") > -1) Xrm.Page.getAttribute("new_cnpj_loja").setValue(Xrm.Page.getAttribute("new_cnpj_loja").getValue().replace("-", ""));

        if (Xrm.Page.getAttribute("new_cnpj_loja").getValue().length == 14)
            FormatarCnpj(Xrm.Page.getAttribute("new_cnpj_loja"));
        else {
            Xrm.Page.getAttribute("new_cnpj_loja").setValue(null);
            alert("Campo de CNPJ Loja é invalido");
        }
    }
}

function new_telefone_loja_onchange() {
    //Formata o campo Telefone Loja para telefone
    if (Xrm.Page.getAttribute("new_telefone_loja").getValue() != null) {
        phoneFormat(Xrm.Page.getAttribute("new_telefone_loja"));
    }
}

function new_acao_final2_onchange() {
    AlterarCampoRestituicao();
}

function contractid_onchange() {
    Xrm.Page.getAttribute("new_rua").setValue(null);
    Xrm.Page.getAttribute("new_endereco").setValue(null);
    Xrm.Page.getAttribute("new_bairro").setValue(null);
    Xrm.Page.getAttribute("new_uf").setValue(null);
    Xrm.Page.getAttribute("new_cidade").setValue(null);
    Xrm.Page.getAttribute("new_descricao_produto").setValue(null);
    Xrm.Page.getAttribute("new_localidadeid").setValue(null);

    if (Xrm.Page.getAttribute("contractid").getValue() != null && Xrm.Page.getAttribute("customerid").getValue() != null)
        if (Xrm.Page.getAttribute("customerid").getValue()[0].typename == "account") {
            ObterDescricaoClienteParticipante(Xrm.Page.getAttribute("contractid").getValue()[0].id, Xrm.Page.getAttribute("customerid").getValue()[0].id);
        }
        else {
            alert("É preciso que a ocorrência esteja vinculada a um cliente!");
        }
}

function contractdetailid_onchange() {
    if (Xrm.Page.getAttribute("contractdetailid").getValue() != null && Xrm.Page.getAttribute("customerid").getValue() != null) {

        var id = Xrm.Page.getAttribute("contractdetailid").getValue()[0].id;
        var idclient = Xrm.Page.getAttribute("customerid").getValue()[0].id;

        var Cols = ["initialquantity", "new_quantidade_ocorrencia_periodo"];
        var linha = XrmServiceToolkit.Soap.Retrieve("contractdetail", id, Cols);

        var query = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                      "<entity name='incident'>" +
                        "<attribute name='title' />" +
                        "<attribute name='incidentid' />" +
                        "<filter type='and'>" +
                          "<condition attribute='customerid' operator='eq' uiname='BANCO BRADESCO' uitype='account' value='" + idclient + "' />" +
                          "<condition attribute='contractdetailid' operator='eq' uiname='teste' uitype='contractdetail' value='" + id + "' />";
        if (linha.attributes['new_quantidade_ocorrencia_periodo'] != null && linha.attributes['new_quantidade_ocorrencia_periodo'].value == 1)
            query += "<condition attribute='new_data_origem' operator='this-month' />";
        query += "</filter>" +
                      "</entity>" +
                    "</fetch>";

        var lista = XrmServiceToolkit.Soap.Fetch(query);

        var qtd = lista.length;
        var quantidadeMaxima = linha.attributes['initialquantity'] != null ? linha.attributes['initialquantity'].value : 999999999;

        if (Xrm.Page.ui.getFormType() == 2) { qtd--; }

        if (qtd >= quantidadeMaxima) {
            alert("Quantidade de ocorrência permitida para esse cliente, ultrapassa o limite de ocorrências da linha de contrato.");
        }
    }
}

/**********************************
Data:      
Autor:     
Descrição: Pesquisa o nome do usuário logado
    **********************************/
function PesquisaNomeUsuarioLogado() {
    return Xrm.Page.context.getUserName();
}

/**********************************
Data:      
Autor:     
Descrição: Cria um Lookup
    **********************************/
function CreateLookup(id, name, type) {
    var lookupData = new Array();
    var lookupItem = new Object();
    lookupItem.id = id;
    lookupItem.entityType = type;
    lookupItem.name = name;
    lookupData[0] = lookupItem;
    return lookupData;
}

function new_data_hora_conclusao_onchange() {
    /**********************************
    Ações do OnChange.
    **********************************/
}

//function new_empresa_executanteid_onchange() {
/**
* @Author José Luiz Silveira.
* Comentado pois não há mais necessidade de validar se a empresa executante e prestador de serviço da ISOL, 
* pois o lookup já filtra somente empresas prestadoras de serviço para seleção.
if (Xrm.Page.getAttribute("new_empresa_executanteid").getValue() != null) {
    var empresaExecutante = Xrm.Page.getAttribute("new_empresa_executanteid").getValue()[0].id;
    var resultado = ValidarClienteIsol(empresaExecutante);

    if (!resultado.PrestacaoServicoIsol) {
        alert("Cliente não é um Prestador de Serviço ISOL")
        Xrm.Page.getAttribute("new_empresa_executanteid").setValue(null);
    }
}*/
//}

function AoSalvar(contexto) {
    FormularioEstaEmModoDeCriacao = false;
    Xrm.Page.getControl("new_arvore_assunto").setDisabled(false);

    if (validacaoAssunto) {
        alert("O Assunto selecionado não é o ultimo da estrutura.");
        contexto.getEventArgs().preventDefault(); //evita que o Salvar continue (contexto deve ser configurado no CRM)
        return;
    }

    //Valida os campos associados a integração com o HPSM
    if (Xrm.Page.getAttribute("contractid").getValue() != null)
        if (Xrm.Page.getAttribute("contractid").getValue()[0].id == "{292CBD71-068B-E611-BE52-0050568DEA94}") {
            Xrm.Page.getAttribute("itbc_integrar_barramento").setValue(true);
        }

    Xrm.Page.getAttribute("contractdetailid").setRequiredLevel("none");
    Xrm.Page.getAttribute("itbc_data_hora_solucao_cliente").setSubmitMode("always");
}

function AoCarregar() {
    debugger;
    if (FormularioEstaEmModoDeCriacao) {
        Xrm.Page.ui.tabs.get("tab_13").sections.get("tab_13_section_2").setVisible(false);
        //se na criação veio campo itbc_cpfconsulta busca contato
        if (Xrm.Page.getAttribute("itbc_cpfconsulta").getValue() != null) {
            EncontrarContatoSistemaTelefonia(Xrm.Page.getAttribute("itbc_cpfconsulta").getValue());
        }
    } else {
        //UPDATE
        Xrm.Page.ui.tabs.get("tab_13").sections.get("tab_13_section_2").setVisible(true);
    }

    if (typeof (crmForm) == "undefined") { crmForm = {}; }
    crmForm = {

        /**********************************
		Data:      15/12/2010
		Autor:     Clausio Elmano de Oliveira
		Descrição: Retorna o responsavel técnico
		**********************************/
        AbreOS: function () {
            if (FormularioEstaEmModoDeCriacao) {
                alert('Não é possivel enviar e-mail de uma nova ocorrência!');
                return false;
            }
            var pagina = "/activities/email/edit.aspx?pId=" + Xrm.Page.data.entity.getId() + "&pType=112";
            window.open(pagina, '', 'height = 600, width = 800, scrollbars=yes, location=no, toolbar=no, menubar=no');
        },

        /**********************************
		Data:      21/12/2010
		Autor:     Gabriel Dias Junckes
		Descrição: Abre o cliente participante do contrato
		**********************************/
        AbreClienteParticipanteDoContrato: function () {
            clienteId = Xrm.Page.getAttribute("customerid").getValue()[0].id;
            contratoId = Xrm.Page.getAttribute("contractid").getValue()[0].id;
            /*** EXECUTA O WEBSERVICE ***/
            Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "BuscarClienteParticipanteDoContratoPor");

            Util.funcao.SetParameter("contratoId", contratoId);
            Util.funcao.SetParameter("clienteId", clienteId);
            Util.funcao.SetParameter("nomeDaOrganizacao", ORG_UNIQUE_NAME);
            //Execução do serviço web
            var retorno = Util.funcao.Execute();
            //Tratamento do retorno
            if (retorno["Success"] == true) {
                var data = retorno['ReturnValue'];

                if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
                    alert($(data).find("MensagemDeErro").text());
                    return "0";
                }
                if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
                    return "0";
            }
            else {
                Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
                Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
            }
            var pagina = "/IntelbrasCRM/userdefined/edit.aspx?id={" + retorno.Intelbras.guid +
					     "}&_CreateFromType=1010&_CreateFromId={B96E0387-6A0C-E011-917B-001CC0953230}&etc=10201";
            window.open(pagina, '', 'height = 600, width = 800, scrollbars=yes, location=no, toolbar=no, menubar=no');
        }
    }

    onLoadLiveChat();

    /**********************************
    Descrição: Verifica se os campos obrigatórios estão nulos. Caso estiverem, determina que a ocorrencia é de criação.
    **********************************/
    if (UN_DO_USUARIO == "POS VENDA") {
        if (Xrm.Page.getAttribute("casetypecode").getValue() == null) {
            FormularioEstaEmModoDeCriacao = true;
            FormularioEstaSendoAlterado = false;
            Xrm.Page.getAttribute("caseorigincode").setValue(200001);
        }

    }

    try {
        Xrm.Page.getControl("new_data_restituicao").setVisible(false);
        Xrm.Page.getControl("new_emitente").setVisible(false);
        // retirado por Carlos Xrm.Page.getControl("new_guid_endereco").setVisible(false);

        if (FormularioEstaEmModoDeCriacao) {
            Xrm.Page.getAttribute("new_unidade_negocio_call_center").setValue(GetMyBusinessUnit());
            if (Xrm.Page.getAttribute("subjectid").getValue() != null) {
                AtualizaArvoreAssunto(Xrm.Page.getAttribute("subjectid").getValue()[0].id);
            }
            Xrm.Page.getAttribute("new_data_origem").setValue(new Date());
        }

        //VerificacaoTiposDaOcorrencia() Transformado em regra de negócio - Robson 08/12/2017
        AlterarCampoRestituicao();

        RangePickList(Xrm.Page.getControl("casetypecode"), Xrm.Page.getAttribute("casetypecode"), 300000, 300020); //Tipo de Ocorrencia
        RangePickList(Xrm.Page.getControl("prioritycode"), Xrm.Page.getAttribute("prioritycode"), 200000, 200004); //Prioridade       

        //Status
        var inicio = new Array();
        inicio[0] = 200000;
        inicio[1] = 200010;
        inicio[2] = 200035;
        inicio[3] = 200057;
        inicio[4] = 993520000;
        inicio[5] = 993520013;

        var fim = new Array();
        fim[0] = 200004;
        fim[1] = 200025;
        fim[2] = 200050;
        fim[3] = 200058;
        fim[4] = 993520001;
        fim[5] = 993520013;

        ArrayRangePickList(Xrm.Page.getControl("statuscode"), Xrm.Page.getAttribute("statuscode"), inicio, fim);

        if (FormularioEstaEmModoDeCriacao) {
            Xrm.Page.getAttribute("prioritycode").setValue(200001); // Média
            Xrm.Page.getAttribute("statuscode").setValue(200010); //Aguardando Tratativa
            Xrm.Page.getAttribute("title").setValue("Atendimento Call Center");
            Xrm.Page.getAttribute("caseorigincode").setValue(200000); //Telefone
        }
        Xrm.Page.getControl("customerid").setFocus(true);
    } catch (e) { alert(e.message); }

    //Executa os as rotinas primárias da ocorrencia
    try {
        if (FormularioEstaEmModoDeCriacao) {
            Xrm.Page.getAttribute("new_data_origem").setValue(new Date());
            Xrm.Page.getAttribute("new_empresa_executanteid").setValue(null);
            Xrm.Page.getAttribute("new_autorizadaid").setValue(null);
        } else {
            TipodeOcorrenciaNoCRM = Xrm.Page.getAttribute("casetypecode").getValue();
        }
    } catch (e) { }

    if (Xrm.Page.getAttribute("itbc_cliente_participante_endereco") != null) {
        Xrm.Page.getControl("itbc_cliente_participante_endereco").addPreSearch(addLookupFilterEndereco);
    }
    addCustomViewContrato();

    /*
    O Código a seguir é necessário, pois quando uma ocorrência é criada a partir de um cliente,
    os campos empresa executante e autorizada eram preenchidos automaticamente com o cliente,
    mas esse não é o comportamento desejado.
    */

    //Colocado na linha 1234 por Robson - 08/12/2017
    //if (Xrm.Page.ui.getFormType() == 1) {
    //    Xrm.Page.getAttribute("new_empresa_executanteid").setValue(null);
    //    Xrm.Page.getAttribute("new_autorizadaid").setValue(null);
    //}

    $('body').on('blur', '.notesTextBox', function (e) {
        if (this.value != '' && this.defaultValue != this.value) {
            dispatchNoteCommand(this, 'post', this.id.replace('_notesTextBox', ''));
            var domEvent = new Sys.UI.DomEvent(event);
            domEvent.stopPropagation();
            return false;
        }
    });

    $('body').on('keydown', '#itbc_anexo_i', function (e) {
        if ((this.scrollHeight - 5) > parseInt(this.style.height.replace("px", ""))) {
            this.style.height = (this.scrollHeight) + "px";
        }
    });
}

function addCustomViewContrato() {
    if (Xrm.Page.getAttribute("customerid").getValue() != null) {

        var customerId = Xrm.Page.getAttribute("customerid").getValue()[0].id;

        var fetchXml = '<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="true">' +
                            '<entity name="contract">' +
                                '<attribute name="title" />' +
                                '<attribute name="contractid" />' +
                                '<attribute name="statecode" />' +
                                '<attribute name="statuscode" />' +
                                '<attribute name="expireson" />' +
                                '<order attribute="title" descending="false" />' +
                                '<link-entity name="new_cliente_participante_contrato" from="new_contratoid" to="contractid" alias="ad"></link-entity>' +
                                '<filter type="and">' +
                                    '<condition attribute="statecode" operator="eq" uitype="account" value="2" />' +
                                    '<condition  entityname="new_cliente_participante_contrato" attribute="new_clienteid" operator="eq" uitype="account" value="' + customerId + '" />' +
                                '</filter>' +
                            '</entity>' +
                        '</fetch>';
        var layoutXml = '<grid name="resultset" object="1010" jump="title" select="1" icon="1" preview="1"><row name="result" id="contractid"><cell name="title" width="300"/><cell name="expireson" width="100"/><cell name="statuscode" width="100"/><cell name="statecode" width="100"/></row></grid>';

        var entityName = "contract";

        var viewDisplayName = "Contratos Clientes ISOL";

        var viewId = Xrm.Page.getControl("contractid").getDefaultView();

        Xrm.Page.getControl("contractid").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);

    }
}

function addLookupFilterEndereco() {
    if (Xrm.Page.getAttribute("contractid").getValue() != null) {
        var contractId = Xrm.Page.getAttribute("contractid").getValue()[0].id;
        fetchXml = "<filter type='and'><condition attribute='new_contratoid' operator='eq' value='" + contractId + "' /></filter>";
        Xrm.Page.getControl("itbc_cliente_participante_endereco").addCustomFilter(fetchXml);
    }
}

function addLookupFilterContrato() {
    if (Xrm.Page.getAttribute("customerid").getValue() != null) {
        var customerId = Xrm.Page.getAttribute("customerid").getValue()[0].id;
        var fetchXml = '<link-entity name="new_cliente_participante_contrato" from="new_contratoid" to="contractid" alias="ad"><filter type="and"><condition attribute="new_clienteid" operator="eq" uiname="BANCO DO BRASIL SA" uitype="account" value="' + customerId + '" /></filter></link-entity>';
        Xrm.Page.getControl("contractid").addCustomFilter(fetchXml);
    }
}

function clienteParticipanteEnderecoOnChange() {
    if (Xrm.Page.getAttribute("itbc_cliente_participante_endereco").getValue() != null) {
        var id = Xrm.Page.getAttribute("itbc_cliente_participante_endereco").getValue()[0].id;
        var Cols = ["new_codigoendereco", "new_cep", "new_rua", "new_bairro", "new_cidade", "new_uf", "new_localidadeid", "new_produtos_endereco"];
        var result = XrmServiceToolkit.Soap.Retrieve("new_cliente_participante_endereco", id, Cols);

        Xrm.Page.getAttribute("new_guid_endereco").setValue(id);
        Xrm.Page.getAttribute("new_endereco").setValue(result.attributes['new_codigoendereco'].value);
        Xrm.Page.getAttribute("new_rua").setValue(result.attributes['new_rua'].value);
        Xrm.Page.getAttribute("new_bairro").setValue(result.attributes['new_bairro'].value);
        Xrm.Page.getAttribute("new_cidade").setValue(result.attributes['new_cidade'].value);
        Xrm.Page.getAttribute("new_uf").setValue(result.attributes['new_uf'].value);

        if (result.attributes['new_localidadeid'] != null)
            Xrm.Page.getAttribute("new_localidadeid").setValue(CreateLookup(result.attributes['new_localidadeid'].id, result.attributes['new_localidadeid'].name, result.attributes['new_localidadeid'].logicalName));
        else
            Xrm.Page.getAttribute("new_localidadeid").setValue(null);

        if (result.attributes['new_produtos_endereco'] != null)
            Xrm.Page.getAttribute("new_produtos_endereco").setValue(result.attributes['new_produtos_endereco'].value);
        else
            Xrm.Page.getAttribute("new_produtos_endereco").setValue(null);

    } else {
        Xrm.Page.getAttribute("new_guid_endereco").setValue(null);
        Xrm.Page.getAttribute("new_endereco").setValue(null);
        Xrm.Page.getAttribute("new_rua").setValue(null);
        Xrm.Page.getAttribute("new_bairro").setValue(null);
        Xrm.Page.getAttribute("new_cidade").setValue(null);
        Xrm.Page.getAttribute("new_uf").setValue(null);
        Xrm.Page.getAttribute("new_localidadeid").setValue(null);
        Xrm.Page.getAttribute("new_produtos_endereco").setValue(null);
    }
}

function AbrirLookup() {
    //debugger;
    var domEvent = new Sys.UI.DomEvent(event);
    var isError = true;
    var nomeJanela = "Lookup - " + new Date().format('yyyy-MM-dd HH:mm:ss').toString();
    var control = Xrm.Page.ui.getCurrentControl();
    if (control != null && control.getControlType() == "lookup") {
        var field = control.getAttribute();
        if (field != null) {
            var value = field.getValue();
            if (value != null) {
                var record = value[value.length - 1];
                var guid = record.id;
                var type = record.entityType;
                if (guid != null && guid != "" && type != null && type != "") {
                    var url = Xrm.Page.context.getClientUrl() + "/main.aspx?etn=" + type + "&id=" + guid + "&pagetype=entityrecord";
                    isError = false;
                    //if (typeof (winRef) == 'undefined' || winRef.closed) {
                    winRef = window.open(url, nomeJanela, 'toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=yes,width=800,height=600');
                    //}
                }
            }
        }
    } else {
        var target = domEvent.target;
        var otypename = target.getAttribute("otypename");
        var oid = target.getAttribute("oid");

        var url = Xrm.Page.context.getClientUrl() + "/main.aspx?etn=" + otypename + "&id=" + oid + "&pagetype=entityrecord";
        isError = false;
        //if (typeof (winRef) == 'undefined' || winRef.closed) {
        winRef = window.open(url, nomeJanela, 'toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=yes,width=800,height=600');
    }

    if (isError) {
        alert("Não foi possível abrir o registro de um Lookup. Selecione um Lookup na tela e pressione o botão novamente");
    }
}

function AbrirNovoEmNovaJanela() {

    //debugger;
    var url = Xrm.Page.context.getClientUrl() + "/main.aspx?etn=incident&pagetype=entityrecord&navbar=off";
    var isError = false;
    var nomeJanela = "NovaOcorrencia_" + Date.now().toString();

    winRef = window.open(url, '_blank', "toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=yes,width=800,height=600");

}

// Método da biblioteca openlui(new Sys.UI.DomEvent(event))
openlui = function (domEvent) {
    //debugger;	
    AbrirLookup();
}

function EncontrarContatoSistemaTelefonia(cpf) {
    debugger;
    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
        "<entity name='contact'>" +
        "  <attribute name='contactid' />" +
        "  <attribute name='fullname' />" +
        "  <order attribute='ownerid' descending='false' />" +
        "    <filter type='and'>" +
        "      <condition attribute='new_cpf_sem_mascara' operator='eq' value='" + cpf + "' />" +
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
            Xrm.Page.getAttribute("customerid").setValue([LookupItem]);
        }
    } else {
        LookupItem.id = null;
        LookupItem.name = cpf;
        LookupItem.entityType = "contact";
        Xrm.Page.getAttribute("customerid").setValue([LookupItem]);
    }
}

function AoAlterar_TipoOcorrencia() {
    Xrm.Page.getAttribute("itbc_integrar_astec").setValue(null);
}