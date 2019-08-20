if (typeof (CustomerAddress) == "undefined") { CustomerAddress = {}; }

CustomerAddress = {

    OnLoad: function () {
        Xrm.Page.getAttribute("postalcode").setRequiredLevel("required");
    },

    OnSave: function (context) {
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(true);
    },

    MascaraCampo: function () {
        AssociarEnderecosSelecionados();
        var cepField = Xrm.Page.getAttribute("postalcode");
        if (cepField.getValue() != null) {
            var cep = cepField.getValue();
            cep = cep.replace("-", "");
            if (cep.length > 8)
                cep = cep.substring(0, 8);

            cepField.setValue(cep);
            Util.funcao.Mascara("postalcode", "99999-999");
        }
    },

    MascaraCpfCnpj: function () {

        var format = null
        // CNPJ - CPF - Código Estrangeiro
        var cnpjCpf = Xrm.Page.getAttribute("new_cnpj");
        var value = cnpjCpf.getValue();
        if (value != null) {

            if (value.length == 11) {
                format = Util.funcao.MascaraCPF(value);
                cnpjCpf.setValue(format);
            } else if (value.length == 14) {
                format = Util.funcao.MascaraCNPJ(value);
                cnpjCpf.setValue(format);
            }
        }
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
        } else {
            this.PreencherCep(cepAttribute, resultado);
        }
    },

    PreencherCep: function (cepAttribute, resultado) {
        if (resultado != undefined) {

            Xrm.Page.getAttribute("line1").setValue(resultado.Endereco);
            Xrm.Page.getAttribute("line2").setValue(resultado.Bairro);
            Xrm.Page.getAttribute("stateorprovince").setValue(resultado.UF);
            Xrm.Page.getAttribute("city").setValue(resultado.Municipio.Name);
            Xrm.Page.getAttribute("country").setValue(resultado.Pais.Name);
        }
    },

    VerificaEnderecoPadrao: function () {

        // Não será possível alterar o endereço padrão
        /*var value = Util.funcao.CriarSlug(Xrm.Page.getAttribute("name").getValue());
        if (value == "padrao") {
            Util.funcao.formdisable(true);
        }*/
    }
}

function AssociarEnderecosSelecionados() {
    if (window.top.opener == null) return;
    if (window.top.opener.Xrm.Page.getAttribute("new_clienteid") == null) return;
    if (window.top.opener.Xrm.Page.getAttribute("new_clienteid").getValue()[0] == null) return;
    if (window.top.opener.Xrm.Page.getAttribute("new_contratoid") == null) return;
    if (window.top.opener.Xrm.Page.getAttribute("new_contratoid").getValue()[0] == null) return;
    if (!confirm("Confirma a inclusão do endereço selecionado ao contrato do cliente?")) return;
    var guidClienteParticipante = window.top.opener.Xrm.Page.data.entity.getId();

    //Configuração de parametros
    var clienteId = window.top.opener.Xrm.Page.getAttribute("new_clienteid").getValue()[0].id;
    var contratoId = window.top.opener.Xrm.Page.getAttribute("new_contratoid").getValue()[0].id;
    var enderecosSelecionados = Xrm.Page.data.entity.getId();

    //Configuração do serviço web
    Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "AdicionarParticipante");

    //Atribuição dos paramentros
    Util.funcao.SetParameter("clienteId", clienteId);
    Util.funcao.SetParameter("contratoId", contratoId);
    Util.funcao.SetParameter("clienteParticipanteId", guidClienteParticipante);
    Util.funcao.SetParameter("enderecosSelecionados", enderecosSelecionados);

    //Execução do serviço web
    var retorno = Util.funcao.Execute();
    var resultado = null;

    //Tratamento do retorno
    if (retorno.Success) {
        resultado = retorno.ReturnValue;
        if (resultado.Sucesso == false)
            alert(resultado.MensagemDeErro);
        else
            alert('Endereços incluídos com sucesso.\nALERTA : antes de salvar as alterações, preencher o campo \'Localidade\'.');
    }
    else
        alert('Ocorreu um erro no processo!');

    window.close();    
}

