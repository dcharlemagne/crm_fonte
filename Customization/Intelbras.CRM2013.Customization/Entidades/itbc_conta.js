/// <reference path="d:\intelbras\canais\branch\2015-07-23\adesaomonitoramento\customization\intelbras.crm2013.customization\sdkorebase\sdkore.js" />
/// <reference path="d:\intelbras\canais\branch\2015-07-23\adesaomonitoramento\customization\intelbras.crm2013.customization\util\util.js" />
if (typeof (Conta) == "undefined") { Conta = {}; }

Conta = {

    ParticipanteDoPrograma: { Nao: 993520000, Sim: 993520001, Descredenciado: 993520002 },
    MatrizFilial: { Matriz: 993520000, Filial: 993520001 },
    ApuracaoDeBeneFiciosCompromissos: { Centralizada_na_Matriz: 993520000, Por_Filiais: 993520001 },
    Natureza: { Pessoa_Fisica: 993520003, Pessoa_Juridica: 993520000, Estrangeiro: 993520001, Trading: 993520002 },

    FORMTYPE_CREATE: 1,
    FORMTYPE_UPDATE: 2,

    OnLoad: function () {
        // Salva valor inicial de Apuração de benefícios.
        Conta.vInicial_apuracaoBeneficiosCompromissos = Util.Xrm.ObterValor("itbc_apuracaodebeneficiosecompromissos");

        var formType = Xrm.Page.ui.getFormType();

        if (formType == this.FORMTYPE_UPDATE) {
            Xrm.Page.getControl("itbc_datahoraintegracaosefaz").setDisabled(true);
            //Xrm.Page.getControl("itbc_statusintegracaosefaz").setDisabled(true);
            /*Xrm.Page.getControl("itbc_address1_country").setDisabled(true);
            Xrm.Page.getControl("itbc_address1_stateorprovince").setDisabled(true);
            Xrm.Page.getControl("itbc_address1_city").setDisabled(true);
            Xrm.Page.getControl("address1_line3").setDisabled(true);
            Xrm.Page.getControl("itbc_address1_street").setDisabled(true);
            Xrm.Page.getControl("itbc_address1_number").setDisabled(true);
            Xrm.Page.getControl("address1_line2").setDisabled(true);
            Xrm.Page.getControl("address1_postalcode").setDisabled(true);
            Xrm.Page.getControl("name").setDisabled(true);
            Xrm.Page.getControl("itbc_regimeapuracao").setDisabled(true);
            Xrm.Page.getControl("itbc_inscricaoestadual").setDisabled(true);
            Xrm.Page.getControl("itbc_contribuinteicms").setDisabled(true);
            Xrm.Page.getControl("itbc_databaixacontribuinte").setDisabled(true);
            Xrm.Page.getControl("itbc_nomefantasia").setDisabled(true);*/
            Xrm.Page.getControl("itbc_nomeabreviado").setDisabled(true);
        }

        if (formType == this.FORMTYPE_CREATE || formType == this.FORMTYPE_UPDATE) {
            Xrm.Page.getAttribute("itbc_participa_do_programa").addOnChange(Conta.itbc_participa_do_programa_OnChange);

            Conta.itbc_participa_do_programa_OnChange();
        }

        if (Xrm.Page.getAttribute("accountnumber").getValue() != "" || Xrm.Page.getAttribute("itbc_nomeabreviado").getValue() == "") {
            Xrm.Page.getControl("itbc_nomeabreviado").setDisabled(false);
        } else {
            Xrm.Page.getControl("itbc_nomeabreviado").setDisabled(true);
        }
    },

    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("address1_line1").setDisabled(valor);
        Xrm.Page.getControl("address1_stateorprovince").setDisabled(valor);
        Xrm.Page.getControl("address1_city").setDisabled(valor);
        Xrm.Page.getControl("address1_county").setDisabled(valor);
        Xrm.Page.getControl("address1_country").setDisabled(valor);

        Xrm.Page.getControl("address2_line1").setDisabled(valor);
        Xrm.Page.getControl("address2_stateorprovince").setDisabled(valor);
        Xrm.Page.getControl("address2_city").setDisabled(valor);
        Xrm.Page.getControl("address2_country").setDisabled(valor);
        Xrm.Page.getControl("address2_county").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("address1_line1").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_stateorprovince").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_city").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_county").setSubmitMode("always");
        Xrm.Page.getAttribute("address1_country").setSubmitMode("always");

        Xrm.Page.getAttribute("address2_line1").setSubmitMode("always");
        Xrm.Page.getAttribute("address2_stateorprovince").setSubmitMode("always");
        Xrm.Page.getAttribute("address2_city").setSubmitMode("always");
        Xrm.Page.getAttribute("address2_country").setSubmitMode("always");
        Xrm.Page.getAttribute("address2_county").setSubmitMode("always");

        Xrm.Page.getAttribute("parentaccountid").setSubmitMode("always");

        Xrm.Page.getAttribute("itbc_matrizoufilial").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        Conta.DisabledEnabled(false);
        //Util.funcao.Mascara("address1_postalcode", "99999-999");
        //Util.funcao.Mascara("address2_postalcode", "99999-999");
        Conta.address1_postalcode_onchange();
        Xrm.Page.getAttribute("address1_line1").setValue(Util.funcao.ContatenarCampos("itbc_address1_street,itbc_address1_number", " , "));
        //Xrm.Page.getAttribute("address1_stateorprovince").setValue(Util.funcao.ContatenarCampos("itbc_address1_stateorprovince", ""));
        Xrm.Page.getAttribute("address1_city").setValue(Util.funcao.ContatenarCampos("itbc_address1_city", ""));
        Xrm.Page.getAttribute("address1_country").setValue(Util.funcao.ContatenarCampos("itbc_address1_country", ""));

        Xrm.Page.getAttribute("address2_line1").setValue(Util.funcao.ContatenarCampos("itbc_address2_street,itbc_address2_number", " , "));
        //Xrm.Page.getAttribute("address2_stateorprovince").setValue(Util.funcao.ContatenarCampos("itbc_address2_stateorprovince", ""));
        Xrm.Page.getAttribute("address2_city").setValue(Util.funcao.ContatenarCampos("itbc_address2_city", ""));
        Xrm.Page.getAttribute("address2_country").setValue(Util.funcao.ContatenarCampos("itbc_address1_country", ""));
        Xrm.Page.getAttribute("address2_county").setValue(Util.funcao.ContatenarCampos("itbc_address2_city", ""));
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);

        Conta.DisabledEnabled(true);
        Conta.ForceFieldSave();

        if (Xrm.Page.getAttribute("itbc_natureza").getValue() != null) {
            if (Xrm.Page.getAttribute("itbc_natureza").getValue() == Conta.Natureza.Pessoa_Fisica) {//Pessoa Física
                if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue() != null)
                    if (!Util.funcao.ValidarCPF("itbc_cpfoucnpj")) {
                        eventArgs.preventDefault();
                    }
            } else if (Xrm.Page.getAttribute("itbc_natureza").getValue() == Conta.Natureza.Pessoa_Juridica) {//Pessoa Jurídica
                if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue() != null)
                    if (!Util.funcao.ValidarCNPJ("itbc_cpfoucnpj")) {
                        eventArgs.preventDefault();
                    }
            }
        }

        if (!Conta.ValidaAlteracaoApuracaoDeBeneficiosECompromissos())
            eventArgs.preventDefault();

    },

    ValidaAlteracaoApuracaoDeBeneficiosECompromissos: function () {
        var apuracaoBeneficiosCompromissos = Util.Xrm.ObterValor("itbc_apuracaodebeneficiosecompromissos");

        if (apuracaoBeneficiosCompromissos == Conta.vInicial_apuracaoBeneficiosCompromissos)
            return true; // Não houve alteração.

        var participante = Util.Xrm.ObterValor("itbc_participa_do_programa");

        if (participante != null && participante == Conta.ParticipanteDoPrograma.Sim) {
            alert("(CRM) Para alterar a forma de apuração de benefícios e compromissos de ‘Por Filial’ para ‘Centralizada na Matriz’ ou de ‘Centralizada na Matriz’ para ‘Por Filial’ os saldos dos benefícios deverão ser zerados via solicitação de ajuste e o canal retirado do PCI, estabelecer a relação matriz/filial, incluir os canais no PCI e incluir os saldos zerados na matriz.");
            return false;
        }

        if (!Conta.ValidaClassificacaoESubClassificacao())
            return false;

        return true;
    },

    ValidaClassificacaoESubClassificacao: function () {
        var matrizFilial = Util.Xrm.ObterValor("itbc_matrizoufilial");

        if (matrizFilial == null || matrizFilial != Conta.MatrizFilial.Filial)
            return true;  // Retorna caso a conta atual não seja uma Filial

        var apuracaoBeneficiosCompromissos = Util.Xrm.ObterValor("itbc_apuracaodebeneficiosecompromissos");

        if (apuracaoBeneficiosCompromissos == null || apuracaoBeneficiosCompromissos != Conta.ApuracaoDeBeneFiciosCompromissos.Centralizada_na_Matriz)
            return true; // Retorna caso a apruração da conta atual não seja por Matriz

        // Busca Classificação e SubClassificação da Matriz
        var parentAccount = Util.Xrm.ObterValor("parentaccountid");

        if (parentAccount == null)
            return true; // Não possui Matriz cadastrada, retorna.

        var parentAccountId = parentAccount[0].id;

        SDKore.OData.setBaseUrl();
        SDKore.OData.configurarParametros("$select=itbc_participa_do_programa,itbc_ClassificacaoId,itbc_SubclassificacaoId");

        var matriz = SDKore.OData.Retrieve(parentAccountId, "Account");

        if (matriz.itbc_participa_do_programa == null || matriz.itbc_participa_do_programa != Conta.ParticipanteDoPrograma.Sim)
            return true; // Se a Matriz não é participante do Programa, retorna.

        if (matriz.itbc_ClassificacaoId != null)
            matriz.itbc_ClassificacaoId = matriz.itbc_ClassificacaoId[0].id;

        if (matriz.itbc_SubclassificacaoId != null)
            matriz.itbc_SubclassificacaoId = matriz.itbc_SubclassificacaoId[0].id;

        var classificacaoId = Util.Xrm.ObterValor("itbc_classificacaoid");

        if (classificacaoId != null)
            classificacaoId = classificacaoId[0].id;

        var subClassificacaoId = Util.Xrm.ObterValor("itbc_subclassificacaoid");
        if (subClassificacaoId != null)
            subClassificacaoId = subClassificacaoId[0].id;

        if (matriz.itbc_ClassificacaoId != classificacaoId
            || matriz.itbc_SubclassificacaoId != subClassificacaoId) {
            alert("A classificação e subclassificação de uma filial deve ser igual a da sua matriz.");
            return false;
        }

        return true;
    },

    itbc_participa_do_programa_OnChange: function () {
        var participante = Util.Xrm.ObterValor("itbc_participa_do_programa");

        var bloquearCampos = false;
        if (participante != null && participante == Conta.ParticipanteDoPrograma.Sim) {
            bloquearCampos = true;
        }
        Xrm.Page.getControl("parentaccountid").setDisabled(bloquearCampos);
        Xrm.Page.getControl("itbc_matrizoufilial").setDisabled(bloquearCampos);
    },

    Ribbon_Categoria_DisableAddNew: function () {

        // Se Filial e Apuração na Matriz, desabilita Incluir e Excluir Categoria do Canal.

        if (Xrm.Page.getAttribute("itbc_matrizoufilial") == null)
            return true; // Atributo não existe, retorna true

        var matrizFilial = Xrm.Page.getAttribute("itbc_matrizoufilial").getValue();

        if (matrizFilial != null && matrizFilial == Conta.MatrizFilial.Filial) {
            if (Xrm.Page.getAttribute("itbc_apuracaodebeneficiosecompromissos") == null)
                return true; // Atributo não existe, retorna true

            var apuracao = Xrm.Page.getAttribute("itbc_apuracaodebeneficiosecompromissos").getValue();

            if (apuracao != null && apuracao == Conta.ApuracaoDeBeneFiciosCompromissos.Centralizada_na_Matriz)
                return false;

        }
        return true;
    },

    Ribbon_Categoria_DeleteRule: function () {

        if (!Conta.Ribbon_Categoria_DisableAddNew()) {
            alert("Esta Filial possui apuração centralizada na Matriz. Gerencie as Categorias do Canal na conta Matriz.");
            return false;
        }
        return true;
    },

    itbc_CPFouCNPJ_onchange: function () {
        debugger;
        //Se validar CNPJ Busca por Matriz Economica 
        if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue() != null) {
            Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "ProcuraMatrizEconomica");

            var raizCNPJ = Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue();
            var posicaofim = 8;
            if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue().indexOf("/") > 0) {
                posicaofim = Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue().indexOf("/")
            }
            Util.funcao.SetParameter("raizCNPJ", raizCNPJ.substring(0, posicaofim));

            var retorno = Util.funcao.Execute();
            if (retorno["Success"]) {
                retorno = $.parseJSON(retorno['ReturnValue'])
                if (retorno["Sucesso"] == true) {
                    Xrm.Utility.confirmDialog("Atenção: O sistema encontrou uma matriz com a mesma raiz de CNPJ informado: \n - " + retorno["RazaoMatrizEconomica"] + " - " + retorno["CNPJMatriz"] + "\n Deseja definir esta conta como matriz?", function () {
                        var LookupItem = new Object();
                        LookupItem.name = retorno["RazaoMatrizEconomica"];
                        LookupItem.id = retorno["IDMatrizEconomica"];
                        LookupItem.entityType = "account";
                        Xrm.Page.getAttribute("parentaccountid").setValue([LookupItem]);
                        Xrm.Page.getAttribute("itbc_nomeabrevmatrizeconomica").setValue(retorno["NomeAbreviadoMatrizEconomica"]);
                    }, function () {
                        //não faz nada
                    });
                }
            }
        }

        if (Xrm.Page.getAttribute("itbc_natureza").getValue() != null) {
            if (Xrm.Page.getAttribute("itbc_natureza").getValue() == 993520003) {//Pessoa Física
                if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue() != null)
                    Util.funcao.ValidarCPF("itbc_cpfoucnpj");
            } else if (Xrm.Page.getAttribute("itbc_natureza").getValue() == 993520000) {//Pessoa Jurídica
                if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue() != null) {
                    Util.funcao.ValidarCNPJ("itbc_cpfoucnpj");
                }
            }
        }
    },



    itbc_natureza_onchange: function () {
        if (Xrm.Page.getAttribute("itbc_natureza").getValue() == 993520003 || Xrm.Page.getAttribute("itbc_natureza").getValue() == 993520000)
            Xrm.Page.getAttribute("itbc_cpfoucnpj").setRequiredLevel("required");
        else
            Xrm.Page.getAttribute("itbc_cpfoucnpj").setRequiredLevel("none");
    },

    itbc_isastec_onchange: function () {
        if (Xrm.Page.getAttribute("itbc_isastec").getValue()) {
            Xrm.Page.getAttribute("itbc_perfilastec").setRequiredLevel("required");
            Xrm.Page.getAttribute("itbc_tabelaprecoastec").setRequiredLevel("required");
        }
        else {
            Xrm.Page.getAttribute("itbc_perfilastec").setRequiredLevel("required");
            Xrm.Page.getAttribute("itbc_tabelaprecoastec").setRequiredLevel("required");
        }
    },

    telephone1_onchange: function () {
        Util.funcao.Mascara("telephone1", "(00)-0000-0000");
    },

    telephone2_onchange: function () {
        if (Xrm.Page.getAttribute("telephone2").getValue() != null)
            Util.funcao.Mascara("telephone2", "(00)-0000-0000");
    },

    fax_onchange: function () {
        Util.funcao.Mascara("fax", "(00)-0000-0000");
    },

    address1_postalcode_onchange: function () {
        if (Xrm.Page.getAttribute("address1_postalcode").getValue() != null) {
            var cep = Xrm.Page.getAttribute("address1_postalcode").getValue();
            cep = cep.replace("-", "");
            if (cep.length > 8)
                cep = cep.substring(0, 8);

            Xrm.Page.getAttribute("address1_postalcode").setValue(cep);

            Util.funcao.Mascara("address1_postalcode", "99999-999");
        }
    },

    address2_postalcode_onchange: function () {
        Util.funcao.Mascara("address2_postalcode", "99999-999");
    },

    VerificaClassificacao: function () {
        if (Xrm.Page.getAttribute("itbc_classificacaoid").getValue() == null) return;

        var nome = Xrm.Page.getAttribute("itbc_classificacaoid").getValue()[0].name;

        if (Xrm.Page.ui.getFormType() == 1) {
            if (nome.indexOf("Distribuidor Box Mover") > -1
                || nome.indexOf("Distribuidor VAD") > -1
                || nome.indexOf("Revenda Relacional") > -1
                || nome.indexOf("Revenda Transacional") > -1) {
                //Xrm.Page.getControl("itbc_classificacaoid").setFocus();
                //Xrm.Page.getAttribute("itbc_classificacaoid").setValue(null);
                alert("Não é possível utilizar classificações do Programa de Canais durante o cadastro de uma conta. Para isto utilizar o processo de Adesão.");
                return;
            }
        }
    },
    ListaDistribuidoresDaRevenda: function () {
        var url = "/ISV/Web/ListaDistribuidoresDaRevenda.aspx?revendaID=" + Xrm.Page.data.entity.getId();
        window.open(url, "popUp", "location=no,width=800,height=400");
    },

    GerarUrlCRM4: function () {
        var url = Xrm.Page.context.getClientUrl().replace(Xrm.Page.context.getOrgUniqueName(), "") + "/ISV/Web/BuscaCadastroContaCrm4.aspx?id=" + Xrm.Page.data.entity.getId() + "&tipo=conta";
        window.open(url, "popUp", "location=no,width=500,height=400");
    },

    AtualizaInformacoesSefaz: function () {
        try {
            if (Xrm.Page.getAttribute("itbc_tipodeconstituicao").getValue() == null) {
                throw new Error("O campo Tipo de Constituição precisa estar preenchido");
            }

            if (Xrm.Page.getAttribute("itbc_address1_stateorprovince").getValue() == null) {
                throw new Error("O campo Estado precisa estar preenchido");
            }

            if (Xrm.Page.getAttribute("itbc_tipodeconstituicao").getValue() != 993520001 /*CNPJ*/) {
                throw new Error("O campo tipo de constituição precisa ser CNPJ");
            }

            if (Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue() == null) {
                throw new Error("O campo CNPJ precisa estar preenchido");
            }

            var itbc_address1_stateorprovinceid = Xrm.Page.getAttribute("itbc_address1_stateorprovince").getValue()[0].id;

            SDKore.OData.setBaseUrl();
            SDKore.OData.configurarParametros("$select=itbc_SiglaUF");

            //Configurações de propriedades
            var uf = SDKore.OData.Retrieve(itbc_address1_stateorprovinceid, "itbc_estado");
            var cnpj = Xrm.Page.getAttribute("itbc_cpfoucnpj").getValue()

            //Configuração do serviço web
            Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "ObterInformacoesSefaz");

            //Atribuição dos paramentros
            Util.funcao.SetParameter("cnpj", cnpj);
            Util.funcao.SetParameter("cpf", "");
            Util.funcao.SetParameter("uf", uf.itbc_SiglaUF);

            //Execução do serviço web
            var retorno = Util.funcao.Execute();
            var resultado = null;

            //Tratamento do retorno
            if (retorno.Success) {
                resultado = $.parseJSON(retorno["ReturnValue"]);

                Xrm.Page.getAttribute("itbc_datahoraintegracaosefaz").setValue(new Date());
                Xrm.Page.getAttribute("itbc_statusintegracaosefaz").setValue(resultado.StatusIntegracaoSefaz);

                if (resultado.Sucesso) {

                    if (resultado.PaisNome != "") {
                        Xrm.Page.getAttribute("itbc_address1_country").setValue(
                            SDKore.CreateLookup(resultado.PaisId, resultado.PaisNome, "itbc_pais"));
                    }
                    if (resultado.EstadoNome != "") {
                        Xrm.Page.getAttribute("itbc_address1_stateorprovince").setValue(
                            SDKore.CreateLookup(resultado.EstadoId, resultado.EstadoNome, "itbc_estado"));
                    }
                    if (resultado.CidadeNome != "") {
                        Xrm.Page.getAttribute("itbc_address1_city").setValue(
                            SDKore.CreateLookup(resultado.CidadeId, resultado.CidadeNome, "itbc_municipios"));
                    }
                    if (resultado.CidadeNome != "") {
                        Xrm.Page.getAttribute("itbc_address2_city").setValue(
                            SDKore.CreateLookup(resultado.CidadeId, resultado.CidadeNome, "itbc_municipios"));
                    }
                    if (resultado.CidadeNome != "") {
                        Xrm.Page.getAttribute("address2_city").setValue(
                            SDKore.CreateLookup(resultado.CidadeId, resultado.CidadeNome, "itbc_municipios"));
                    }

                    if (resultado.CEP != "") {
                        Xrm.Page.getAttribute("address1_postalcode").setValue(resultado.CEP);
                    }
                    if (resultado.Logradouro != "") {
                        Xrm.Page.getAttribute("itbc_address1_street").setValue(resultado.Logradouro);
                    }
                    if (resultado.Numero != "") {
                        Xrm.Page.getAttribute("itbc_address1_number").setValue(resultado.Numero);
                    }
                    if (resultado.Bairro != "") {
                        Xrm.Page.getAttribute("address1_line2").setValue(resultado.Bairro);
                    }
                    if (resultado.Complemento != "") {
                        Xrm.Page.getAttribute("address1_line3").setValue(resultado.Complemento);
                    }

                    Xrm.Page.getAttribute("name").setValue(resultado.Nome);
                    Xrm.Page.getAttribute("itbc_regimeapuracao").setValue(resultado.RegimeApuracao);
                    Xrm.Page.getAttribute("itbc_inscricaoestadual").setValue(resultado.InscricaoEstadual);
                    Xrm.Page.getAttribute("itbc_contribuinteicms").setValue(resultado.ContribuinteIcms);
                    Xrm.Page.getAttribute("itbc_databaixacontribuinte").setValue(resultado.DataBaixa);

                    if (resultado.NomeFantasia != null) {
                        Xrm.Page.getAttribute("itbc_nomefantasia").setValue(resultado.NomeFantasia);
                    }

                    Xrm.Page.getAttribute("itbc_datahoraintegracaosefaz").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_statusintegracaosefaz").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_address1_country").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_address1_stateorprovince").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_address1_city").setSubmitMode("always");
                    Xrm.Page.getAttribute("address1_line3").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_address1_street").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_address1_number").setSubmitMode("always");
                    Xrm.Page.getAttribute("address1_line2").setSubmitMode("always");
                    Xrm.Page.getAttribute("address1_postalcode").setSubmitMode("always");
                    Xrm.Page.getAttribute("name").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_regimeapuracao").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_inscricaoestadual").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_contribuinteicms").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_databaixacontribuinte").setSubmitMode("always");
                    Xrm.Page.getAttribute("itbc_nomefantasia").setSubmitMode("always");

                    Xrm.Utility.alertDialog("Validação efetuada com sucesso, é necessário salvar o registro!.");

                } else {
                    Xrm.Utility.alertDialog(resultado.Mensagem);
                    Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
                    Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
                }
            }
        }
        catch (erro) {
            Xrm.Utility.alertDialog(erro.message);
        }
    },

    CopiarEnderecoParaCobranca: function () {
        var aceitouCopiarEndereco = confirm("Deseja copiar este endereço para cobrança também?");
        Xrm.Page.getAttribute("new_altera_endereco_padrao").setValue("e");

        if (aceitouCopiarEndereco) {
            if (Xrm.Page.getAttribute("address1_postalcode") != null)
                Xrm.Page.getAttribute("address2_postalcode").setValue(Xrm.Page.getAttribute("address1_postalcode").getValue());
            if (Xrm.Page.getAttribute("itbc_address1_country") != null)
                Xrm.Page.getAttribute("itbc_address2_country").setValue(Xrm.Page.getAttribute("itbc_address1_country").getValue());
            if (Xrm.Page.getAttribute("itbc_address1_stateorprovince") != null)
                Xrm.Page.getAttribute("itbc_address2_stateorprovince").setValue(Xrm.Page.getAttribute("itbc_address1_stateorprovince").getValue());
            if (Xrm.Page.getAttribute("itbc_address1_city") != null)
                Xrm.Page.getAttribute("itbc_address2_city").setValue(Xrm.Page.getAttribute("itbc_address1_city").getValue());
            if (Xrm.Page.getAttribute("itbc_address1_street") != null)
                Xrm.Page.getAttribute("itbc_address2_street").setValue(Xrm.Page.getAttribute("itbc_address1_street").getValue());
            if (Xrm.Page.getAttribute("address1_line1") != null)
                Xrm.Page.getAttribute("address2_line1").setValue(Xrm.Page.getAttribute("address1_line1").getValue());
            if (Xrm.Page.getAttribute("address1_line2") != null)
                Xrm.Page.getAttribute("address2_line2").setValue(Xrm.Page.getAttribute("address1_line2").getValue());
            if (Xrm.Page.getAttribute("address1_line3") != null)
                Xrm.Page.getAttribute("address2_line3").setValue(Xrm.Page.getAttribute("address1_line3").getValue());
            if (Xrm.Page.getAttribute("address1_country") != null)
                Xrm.Page.getAttribute("address2_country").setValue(Xrm.Page.getAttribute("address1_country").getValue());
            if (Xrm.Page.getAttribute("itbc_address1_stateorprovince") != null)
                Xrm.Page.getAttribute("address2_stateorprovince").setValue(Xrm.Page.getAttribute("address1_stateorprovince").getValue());
            if (Xrm.Page.getAttribute("address1_city") != null)
                Xrm.Page.getAttribute("address2_city").setValue(Xrm.Page.getAttribute("address1_city").getValue());
            if (Xrm.Page.getAttribute("address1_county") != null)
                Xrm.Page.getAttribute("address2_county").setValue(Xrm.Page.getAttribute("address1_county").getValue());
            if (Xrm.Page.getAttribute("itbc_address1_number") != null)
                Xrm.Page.getAttribute("itbc_address2_number").setValue(Xrm.Page.getAttribute("itbc_address1_number").getValue());
        }
    },

    AtualizarEnderecoPorCep: function (executionContextObj) {
        if (executionContextObj == null)
            return;
        else if (executionContextObj.getEventSource() == null)
            return;
        else if (executionContextObj.getEventSource().getName() == null || executionContextObj.getEventSource().getName == undefined)
            return;

        var cepAttribute = executionContextObj.getEventSource();
        var cep = cepAttribute.getValue();

        var resultado = null;
        if (Xrm.Page.getAttribute("address1_postalcode") != null && Xrm.Page.getAttribute("address1_postalcode").getValue() != "") {
            resultado = Util.funcao.BuscarCep(cep);
        }

        if (resultado == null || resultado.Cidade == null) {
            return;
        }
        else {
            if (Xrm.Page.getAttribute("address1_postalcode") != null) {
                Util.funcao.PreencherCep(cepAttribute, resultado);
            }

            Conta.CopiarEnderecoParaCobranca();
        }
    },


    onChangeEndereco: function () {
        Conta.CopiarEnderecoParaCobranca();
    },

    itbc_participa_do_programa_OnChange_Aviso: function () {
        var participante = Util.Xrm.ObterValor("itbc_participa_do_programa");

        if (participante != null && participante == Conta.ParticipanteDoPrograma.Sim) {
            alert("Será necessário configurar os Acessos a Extranet dos colaboradores do Canal");
        }
    }
}