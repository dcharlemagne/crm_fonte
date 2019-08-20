if (typeof (SolicitacaoBeneficio) == "undefined") { SolicitacaoBeneficio = {}; }

SolicitacaoBeneficio = {

    ValorDisponivelAntigo: null,
    _codigoBeneficioPrograma: null,

    OnLoad: function () {
        SolicitacaoBeneficio.itbcStatusAtual = Xrm.Page.getAttribute('itbc_status').getValue();

        //Tive que adicionar o OnChange aqui pois esse campo já carrega um form detalhado da entidade, assim impossibilitando colocar o OnChange pela solução
        Xrm.Page.getAttribute("itbc_benefdocanal").addOnChange(this.AtualizarBeneficioPrograma);
        //Xrm.Page.getControl("itbc_valoraabater").setDisabled(true);
        //Adicionei o Onchange no código por uns bugs estranhos no onChange(loop eterno)
        Xrm.Page.getAttribute("itbc_formapagamentoid").addOnChange(this.AtualizarFormaPagamento);

        switch (Xrm.Page.ui.getFormType()) {
            case 0: //Undefined
                break;

            case 1: //Create
                //controleUpdate para saber se é update ou não, é uma variavel que será lida pelo produtosdasolicitacao.js dentro de um iframe
                controleUpdate = false;
                if (Xrm.Page.getAttribute("itbc_valorsolicitado").getValue() == null) {
                    Xrm.Page.getAttribute("itbc_valorsolicitado").setValue(0);
                }

                if (Xrm.Page.getAttribute("itbc_valoraprovado").getValue() == null) {
                    Xrm.Page.getAttribute("itbc_valoraprovado").setValue(0);
                }
                this.preencheDescricaoDefault();
                break;

            case 2: //Update
                controleUpdate = true; //controleUpdate para saber se é update ou não, é uma variavel que será lida pelo produtosdasolicitacao.js dentro de um iframe
                //Xrm.Page.ui.refreshRibbon();
                //Util.funcao.formdisable(true);
                break;
        }

        Xrm.Page.getControl("itbc_condicaopagamentoid").addPreSearch(this.AdicionarFiltroCondicaoPagamento);
        Xrm.Page.data.entity.save();

    },

    DefineObrigatoriedadeValorSolicitado: function () {

        if (!Util.Xrm.HasValue("itbc_status") ||
            (Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.Criada
            && Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.EmAnalise)) {

            Xrm.Page.ui.controls.get("itbc_valorsolicitado").setDisabled(true);
            return;
        }

        if (!Util.Xrm.HasValue("itbc_ajustesaldo")) {
            Xrm.Page.ui.controls.get("itbc_valorsolicitado").setDisabled(true);
            return;
        }

        if (Xrm.Page.getAttribute("itbc_ajustesaldo").getValue() == false) {
            if (!Util.Xrm.HasValue("itbc_formapagamentoid") || Xrm.Page.getAttribute("itbc_formapagamentoid").getValue()[0].name == Config.Entidade.FormaPagamento.Produto.name) {
                Xrm.Page.ui.controls.get("itbc_valorsolicitado").setDisabled(true);
                return;
            }
        }

        Xrm.Page.ui.controls.get("itbc_valorsolicitado").setDisabled(false);
    },

    DefineObrigatoriedadeValorAprovado: function () {
        if (!Util.Xrm.HasValue("itbc_status") ||
            (Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.Criada
            && Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.EmAnalise
            && Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.ComprovantesEmValidacao
            && Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.ComprovacaoConcluida
            && (Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.Aprovada
                        || this.ObterCodigoBeneficioPrograma() != Config.Entidade.BeneficioPrograma.VMC.code))) {
            Xrm.Page.ui.controls.get("itbc_valoraprovado").setDisabled(true);
            Xrm.Page.getAttribute("itbc_valoraprovado").setRequiredLevel("none");
            return;
        }

        if (!Util.Xrm.HasValue("itbc_ajustesaldo") || Xrm.Page.getAttribute("itbc_ajustesaldo").getValue() == true) {
            Xrm.Page.ui.controls.get("itbc_valoraprovado").setDisabled(true);
            Xrm.Page.getAttribute("itbc_valoraprovado").setRequiredLevel("none");
            return;
        }

        if (!Util.Xrm.HasValue("itbc_formapagamentoid") || Xrm.Page.getAttribute("itbc_formapagamentoid").getValue()[0].name == Config.Entidade.FormaPagamento.Produto.name) {
            Xrm.Page.data.entity.save();
            Xrm.Page.ui.controls.get("itbc_valoraprovado").setDisabled(true);
            Xrm.Page.getAttribute("itbc_valoraprovado").setRequiredLevel("none");
            return;
        }

        if (this.ObterCodigoBeneficioPrograma() != Config.Entidade.BeneficioPrograma.VMC.code
            && this.ObterCodigoBeneficioPrograma() != Config.Entidade.BeneficioPrograma.PriceProtection.code) {
            Xrm.Page.ui.controls.get("itbc_valoraprovado").setDisabled(true);
            Xrm.Page.getAttribute("itbc_valoraprovado").setRequiredLevel("none");
            return;
        }

        Xrm.Page.ui.controls.get("itbc_valoraprovado").setDisabled(false);
        Xrm.Page.getAttribute("itbc_valoraprovado").setRequiredLevel("required");
    },

    DefineObrigatoriedadeCondicaoPagamento: function () {

        var obrigatorio = true;

        if (Xrm.Page.ui.getFormType() == 1) {
            obrigatorio = false;
        }
        else if (!Util.Xrm.HasValue("itbc_status") || !Util.Xrm.HasValue("itbc_formapagamentoid") || !Util.Xrm.HasValue("itbc_ajustesaldo")) {
            obrigatorio = false;
        }
        else if (Xrm.Page.getAttribute("itbc_ajustesaldo").getValue()) {
            obrigatorio = false;
        }
        else if (Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.Criada
            && Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.EmAnalise
            && Xrm.Page.getAttribute("itbc_status").getValue() != Config.Entidade.SolicitacaoBeneficio.Status.Aprovada) {
            obrigatorio = false;
        }
        else if (Xrm.Page.getAttribute("itbc_formapagamentoid").getValue()[0].name != Config.Entidade.FormaPagamento.Produto.name) {
            obrigatorio = false;
        }

        Xrm.Page.ui.controls.get("itbc_condicaopagamentoid").setDisabled(!obrigatorio);
        var level = (obrigatorio) ? "required" : "none";
        Xrm.Page.getAttribute("itbc_condicaopagamentoid").setRequiredLevel(level);
    },

    LimparCodigoBeneficioPrograma: function () {
        this._codigoBeneficioPrograma = null;
    },

    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
        Xrm.Page.getAttribute("itbc_acaocrm").setSubmitMode("always");
    },

    OnSave: function (context) {
        if (Xrm.Page.getAttribute("itbc_name").getValue() == null) {
            try {
                var canalId = Xrm.Page.getAttribute("itbc_accountid").getValue()[0].id;
                SDKore.OData.configurarParametros("$select=AccountNumber");
                var accountNumber = SDKore.OData.Retrieve(canalId, "Account").AccountNumber;

                var valSolic;
                if (Xrm.Page.getAttribute("itbc_valorsolicitado").getValue() == null) {
                    valSolic = "0,00";
                } else {
                    valSolic = Xrm.Page.getAttribute("itbc_valorsolicitado").getValue();
                }

                var nome = Xrm.Page.getAttribute("itbc_businessunitid").getValue()[0].name;
                var nome = nome + " / " + this.FormatNumber(valSolic);
                var nome = nome + " / " + accountNumber;

                Xrm.Page.getAttribute("itbc_name").setValue(nome);
            }
            catch (e) {
                Xrm.Page.ui.setFormNotification("Não foi possível preencher o nome da solicitação.", "WARNING", "itbc_name");
            }
        }

        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
        var eventArgs = context.getEventArgs();
//Remover
//        var host = window.location.host;
//
//        if (!host.endsWith("/"))
//            host += "/";
//
//        var url = window.location.protocol + "//" + host + "ISV/ws/CrmWebServices.asmx/"
//
//        SDKore.CallService(url, "ListarCompromisosSolicitacaoBenef");
//        SDKore.SetParameter("benefCanalId", Xrm.Page.getAttribute('itbc_benefdocanal').getValue()[0].id);
//        var retorno = SDKore.Execute();
//        if (retorno['Success']) {
//            var resultado = $.parseJSON(retorno['ReturnValue']);
//            if (resultado["Resultado"]) {

        //Configuração do serviço web
        Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "ListarCompromisosSolicitacaoBenef");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("benefCanalId", Xrm.Page.getAttribute('itbc_benefdocanal').getValue()[0].id);

        //Execução do serviço web
        var retorno = Util.funcao.Execute();
        var resultado = null;

        //Tratamento do retorno
        if (retorno.Success) {
            var resultado = $.parseJSON(retorno['ReturnValue']);
            if (resultado["Resultado"]) {
                switch (resultado['StatusBeneficioCanal']) {

                    case "Bloqueado":
                        var alerta = "Status do Benefício do Canal : Bloqueado\n\n\n";
                        if (resultado['Resultado'] == true) {
                            alerta += "Compromisso(s):\n\n";
                            $.each(resultado["Compromissos"], function (key) {
                                alerta += this + "\n";
                            });
                        }
                        else
                            alerta = "Status do Benefício do Canal : Bloqueado";

                        alert(alerta);
                        eventArgs.preventDefault();
                        break;
                    case "Suspenso":
                        var alerta = "Status do Benefício do Canal : Suspenso\n\n\n";
                        if (resultado['Resultado'] == true) {
                            alerta += "Compromisso(s):\n\n";
                            $.each(resultado["Compromissos"], function (key) {
                                alerta += this + "\n";
                            });
                        }
                        else
                            alerta = "Status do Benefício do Canal : Suspenso\n\n\n";

                        alerta += "\n\nSalvar o registro mesmo assim ?";
                        var confirmado = confirm(alerta);
                        if (confirmado == false) {
                            eventArgs.preventDefault();
                        }
                        else {
                            //1 = Sim , 0 = nao
                            Xrm.Page.getAttribute('itbc_solicitacao_irregularidades').setValue(1);
                            Xrm.Page.getAttribute('itbc_situacao_irregular').setValue("Canal com Benefício Suspenso");
                            Xrm.Page.getAttribute("itbc_solicitacao_irregularidades").setSubmitMode("always");
                            Xrm.Page.getAttribute("itbc_situacao_irregular").setSubmitMode("always");
                        }
                        break;
                    default:
                        break;
                }
            }
            else {
                if (resultado["Mensagem"]) {
                    alert(resultado["Mensagem"]);
                    eventArgs.preventDefault();
                }
            }
        }

        SolicitacaoBeneficio.DisabledEnabled(false);

        SolicitacaoBeneficio.DisabledEnabled(true);
        //Xrm.Page.ui.refreshRibbon();
        SolicitacaoBeneficio.ForceFieldSave();
    },

    EnviarComprovantes: function (solicitacaobeneficioId) {
        if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code) {
            if (SolicitacaoBeneficio.itbcStatusAtual == Config.Entidade.SolicitacaoBeneficio.Status.AguardandoComprovantes) {
                Xrm.Utility.confirmDialog("Tem certeza que deseja confirmar o envio dos comprovantes da ação?", function () {
                    Xrm.Page.getControl("itbc_status").setDisabled(false);
                    Xrm.Page.getAttribute("itbc_status").setValue(Config.Entidade.SolicitacaoBeneficio.Status.AguardandoRetornoFinanceiro);
                    Xrm.Page.getControl("itbc_status").setDisabled(true);
                }, function () { });
            } else {
                Xrm.Utility.alertDialog("Para solicitar este evento, o Status do registro deve ser Aguardando Comprovantes.");
            }
        } else {
            Xrm.Utility.alertDialog("Está opção é somente utilizada para beneficio VMC.");
        }

        Xrm.Page.ui.refreshRibbon();
    },

    MudarNome: function () {
        try {
            var canalId = Xrm.Page.getAttribute("itbc_accountid").getValue()[0].id;
            SDKore.OData.configurarParametros("$select=AccountNumber");
            var accountNumber = SDKore.OData.Retrieve(canalId, "Account").AccountNumber;

            var valSolic;
            if (Xrm.Page.getAttribute("itbc_valorsolicitado").getValue() == null) {
                valSolic = "0,00";
            } else {
                valSolic = Xrm.Page.getAttribute("itbc_valorsolicitado").getValue();
            }

            var nome = Xrm.Page.getAttribute("itbc_businessunitid").getValue()[0].name;
            var nome = nome + " / " + this.FormatNumber(valSolic);
            var nome = nome + " / " + accountNumber;

            Xrm.Page.getAttribute("itbc_name").setValue(nome);
        }
        catch (e) {
            Xrm.Page.ui.setFormNotification("Não foi possível preencher o nome da solicitação.", "WARNING", "itbc_name");
        }
    },

    FormatNumber: function (number) {
        var number = number.toFixed(2) + '';
        var x = number.split('.');
        var x1 = x[0];
        var x2 = x.length > 1 ? ',' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + '.' + '$2');
        }
        return x1 + x2;
    },

    RibbonEnabledEnviarComprovantes: function () {
        try {
            if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code
                && Xrm.Page.getAttribute('itbc_status').getValue() == Config.Entidade.SolicitacaoBeneficio.Status.AguardandoComprovantes) {
                return true;
            }

            return false;
        }
        catch (error) {
            return false;
        }
    },

    RecalcularPriceAcao: function () {
        var statusSolic = Xrm.Page.getAttribute("itbc_status").getValue();
        var statusPrice = Xrm.Page.getAttribute("itbc_statuscalculopriceprotection").getValue();

        if (statusSolic == Config.Entidade.SolicitacaoBeneficio.Status.EmAnalise
            && (statusPrice != Config.Entidade.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calcular && statusPrice != Config.Entidade.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calculando)) {
            Xrm.Page.ui.setFormNotification("Status de Calculo alterado com sucesso.", "INFO", "RecalcularPriceAcaoNotif");

            setTimeout(function () {
                Xrm.Page.ui.clearFormNotification("RecalcularPriceAcaoNotif");
            }, 3000);

            Xrm.Page.data.entity.attributes.get("itbc_statuscalculopriceprotection").setValue(993520001);
            Xrm.Page.data.entity.attributes.get("itbc_statuscalculopriceprotection").getSubmitMode("dirty");
            Xrm.Page.data.entity.save();
            var id = Xrm.Page.data.entity.getId();
        }
        else {
            Xrm.Page.ui.setFormNotification("Para realizar o calculo, o Status da Solicitação tem que ser 'Em Análise' e o Status do Calculo tem que ser diferente de 'Calcular' e 'Calculando'", "WARNING", "RecalcularPriceAcaoNotif");

            setTimeout(function () {
                Xrm.Page.ui.clearFormNotification("RecalcularPriceAcaoNotif");
            }, 3000);
        }

    },

    AutorizarPagamento: function (solicitacaobeneficioId) {
        if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code) {
            if (SolicitacaoBeneficio.itbcStatusAtual == Config.Entidade.SolicitacaoBeneficio.Status.Aprovada) {
                Xrm.Utility.confirmDialog("Tem certeza que deseja confirmar a autorização do pagamento?", function () {
                    Xrm.Page.getControl("itbc_status").setDisabled(false);
                    Xrm.Page.getAttribute("itbc_status").setValue(Config.Entidade.SolicitacaoBeneficio.Status.PagamentoPendenteGeracaoPedidoPendente);
                    Xrm.Page.getControl("itbc_status").setDisabled(true);
                }, function () { });
            } else {
                Xrm.Utility.alertDialog("Para solicitar este evento, o Status do registro deve ser Aprovada.");
            }
        } else
            Xrm.Utility.alertDialog("Está opção é somente utilizada para beneficio VMC.");
        Xrm.Page.ui.refreshRibbon();
    },

    RibbonEnabledAutorizarPagamento: function () {
        try {
            if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code && (Xrm.Page.getAttribute('itbc_status').getValue() == Config.Entidade.SolicitacaoBeneficio.Status.Aprovada))
                return true;
            return false;
        }
        catch (error) {
            return false;
        }
    },

    EnviarRetornoFinanceiro: function (solicitacaobeneficioId) {
        if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code) {
            if (SolicitacaoBeneficio.itbcStatusAtual == Config.Entidade.SolicitacaoBeneficio.Status.AguardandoRetornoFinanceiro) {
                Xrm.Utility.confirmDialog("Tem certeza que deseja confirmar o envio do retorno financeiro da ação?", function () {
                    Xrm.Page.getControl("itbc_status").setDisabled(false);
                    Xrm.Page.getAttribute('itbc_status').setValue(Config.Entidade.SolicitacaoBeneficio.Status.RetornoFinanceiroEnviado);
                    Xrm.Page.getControl("itbc_status").setDisabled(true);
                }, function () { });

            } else {
                Xrm.Utility.alertDialog("Para solicitar este evento, o Status do registro deve ser Aguardando Retorno Financeiro.");
            }
        } else
            Xrm.Utility.alertDialog("Está opção é somente utilizada para beneficio VMC.");
        Xrm.Page.ui.refreshRibbon();

    },

    RibbonEnabledEnviarRetornoFinanceiro: function () {

        try {
            if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code && (Xrm.Page.getAttribute('itbc_status').getValue() == Config.Entidade.SolicitacaoBeneficio.Status.AguardandoRetornoFinanceiro))
                return true;
            return false;
        }
        catch (error) {
            return false;
        }
    },

    SolicitarReembolso: function (solicitacaobeneficioId) {
        if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code) {
            if (SolicitacaoBeneficio.itbcStatusAtual == Config.Entidade.SolicitacaoBeneficio.Status.RetornoFinanceiroEnviado) {
                Xrm.Utility.confirmDialog("Tem certeza que deseja confirmar a solicitação do reembolso da ação? ", function () {
                    Xrm.Page.getControl("itbc_status").setDisabled(false);
                    Xrm.Page.getAttribute('itbc_status').setValue(Config.Entidade.SolicitacaoBeneficio.Status.AnaliseReembolso);
                    Xrm.Page.getControl("itbc_status").setDisabled(true);
                }, function () { });
            } else {
                Xrm.Utility.alertDialog("Para solicitar este evento, o Status do registro deve ser Retorno Financeiro Enviado.");
            }
        } else
            Xrm.Utility.alertDialog("Está opção é somente utilizada para beneficio VMC.");
        Xrm.Page.ui.refreshRibbon();
    },

    RibbonEnabledSolicitarReembolso: function () {

        try {
            if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code && (Xrm.Page.getAttribute('itbc_status').getValue() == Config.Entidade.SolicitacaoBeneficio.Status.RetornoFinanceiroEnviadoRetornoFinanceiroEnviado))
                return true;
            return false;
        }
        catch (error) {
            return false;
        }
    },

    Avanca: function () {

        Xrm.Page.getAttribute('itbc_status').setValue(993520000);
        Xrm.Page.getAttribute("itbc_status").setSubmitMode("always");
        Xrm.Page.data.entity.save();
    },

    RibbonEnabledSolicicaoAvanca: function () {

        try {
            if ((Xrm.Page.getAttribute("itbc_formapagamentoid").getValue()[0].name == "produto"))
                return true;
            return false;
        }
        catch (error) {
            return false;
        }
    },

    VerificarPreenchimentoLookup: function (fieldName) {
        if (Xrm.Page.getAttribute(fieldName) != null && Xrm.Page.getAttribute(fieldName).getValue() != null && Xrm.Page.getAttribute(fieldName).getValue()[0] != null)
            return true;

        return false;
    },

    AtualizarBeneficioPrograma: function () {
        if (Xrm.Page.getAttribute("itbc_benefdocanal").getValue() != null) {
            var GuidBenefCanal = Xrm.Page.getAttribute("itbc_benefdocanal").getValue()[0].id;
            try {
                SDKore.OData.setBaseUrl();
                SDKore.OData.configurarParametros("$select=itbc_BeneficioId");
                var resultado = SDKore.OData.Retrieve(GuidBenefCanal, "itbc_benefDoCanal");
                Xrm.Page.getAttribute("itbc_beneficiodoprograma").setValue(SDKore.CreateLookup(resultado.itbc_BeneficioId.Id, resultado.itbc_BeneficioId.Name, resultado.itbc_BeneficioId.LogicalName));
                Xrm.Page.getAttribute("itbc_beneficiodoprograma").fireOnChange();
                Xrm.Page.getAttribute("itbc_beneficiodoprograma").setSubmitMode("always");
            }
            catch (error) {
                alert(error.message);
            }
        }
    },

    AtualizarFormaPagamento: function () {
        try {
            var beneficio = "";
            var unidadeNegocio = "";
            var codigo = "";

            //Exceção pra quando voce muda o campo forma de pagamento para vazio
            if (Xrm.Page.getAttribute("itbc_formapagamentoid") == null || Xrm.Page.getAttribute("itbc_formapagamentoid").getValue() == null) {
                this.SetarValoresPadraoFormaPagamentoManual();
                return;
            }

            if (Util.Xrm.HasValue("itbc_beneficiodoprograma"))
                beneficio = Xrm.Page.getAttribute("itbc_beneficiodoprograma").getValue()[0].id.replace("{", "").replace("}", "");
            else
                throw new Error("Benefício do Canal não preenchido.");

            if (Xrm.Page.getAttribute("itbc_businessunitid") != null && Xrm.Page.getAttribute("itbc_businessunitid").getValue() != "undefined" && Xrm.Page.getAttribute("itbc_businessunitid").getValue() != null)
                unidadeNegocio = Xrm.Page.getAttribute("itbc_businessunitid").getValue()[0].id.replace("{", "").replace("}", "");
            else {
                throw new Error("Unidade de Negócio não preenchida.");
            }

            if (!Util.Xrm.HasValue("itbc_valorsolicitado")) {
                throw new Error("Campo Valor Solicitado obrigatório, preencha e volte a selecionar a forma de pagamento");
            }
        }
        catch (error) {
            Xrm.Page.getAttribute("itbc_formapagamentoid").removeOnChange(SolicitacaoBeneficio.AtualizarFormaPagamento);
            Xrm.Page.getAttribute("itbc_formapagamentoid").setValue(null);
            SolicitacaoBeneficio.SetarValoresPadraoFormaPagamentoManual();
            Xrm.Page.getAttribute("itbc_formapagamentoid").addOnChange(SolicitacaoBeneficio.AtualizarFormaPagamento);
            alert("Erro  : " + error.message);
            return;
        }
    },

    AtualizaValorAbater: function () {
        try {
            if (!Util.Xrm.HasValue("itbc_beneficiodoprograma")) { return; }
            if (!Util.Xrm.HasValue("itbc_formapagamentoid")) { return; }

            if (Util.Xrm.HasValue("itbc_status")) {
            	if (Xrm.Page.getAttribute("itbc_status").getValue() != 993520000 && Xrm.Page.getAttribute("itbc_status").getValue() != 993520005 && Xrm.Page.getAttribute("itbc_status").getValue()
				!= 993520001 && Xrm.Page.getAttribute("itbc_status").getValue() != 993520011 != Xrm.Page.getAttribute("itbc_status").getValue() != 993520008 && Xrm.Page.getAttribute("itbc_status").getValue() != 993520010) {
                    return;
                }
            } else {
                return;
            }

            if (!Util.Xrm.HasValue("itbc_valoraprovado") || Xrm.Page.getAttribute("itbc_valoraprovado").getValue() == 0) {
                Xrm.Page.getAttribute("itbc_valoraabater").setValue(0);
                Xrm.Page.data.entity.save();
            } else {

                if (this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.VMC.code
                    && this.ObterCodigoBeneficioPrograma() == Config.Entidade.BeneficioPrograma.PriceProtection.code
                    && Xrm.Page.getAttribute("itbc_formapagamentoid").getValue()[0].name != Config.Entidade.FormaPagamento.Produto.name) {
                    return;
                } else {
                    var fator = this.ObterFatorDeFormaPagamento();
                    Xrm.Page.getAttribute("itbc_valoraabater").setValue(Xrm.Page.getAttribute("itbc_valoraprovado").getValue() / fator);

                    var tipodesolicitacao = Xrm.Page.getAttribute("itbc_tipodesolicitacaoid").getValue()[0].name;

                    if (tipodesolicitacao != "Ajuste manual de saldo de benefício") {
                        Xrm.Page.data.entity.save();
                    }
                }
            }
        }
        catch (e) {
            Xrm.Page.ui.setFormNotification("Não foi possível calcular o Valor Abater", "ERROR", "valorAbater");
        }
    },

    ObterFatorDeFormaPagamento: function () {

        switch (Xrm.Page.getAttribute("itbc_formapagamentoid").getValue()[0].name) {
            case Config.Entidade.FormaPagamento.Produto.name:
                codigo = Config.ParametroGlobal.FatorConversaoValorSolicitacaoBeneficioProduto.code;
                break;

            case Config.Entidade.FormaPagamento.Dinheiro.name:
                codigo = codigo = Config.ParametroGlobal.FatorConversaoValorSolicitacaoBeneficioDinheiro.code;

                break;

            case Config.Entidade.FormaPagamento.DescontoDuplicata.name:
                codigo = codigo = Config.ParametroGlobal.FatorConversaoValorSolicitacaoBeneficioDescontoEmDuplicata.code;
                break;

            default:
                throw new Error("Forma de Pagamento não conhecida,contate o administrador do sistema.");
        }

        SDKore.OData.setBaseUrl();
        SDKore.OData.configurarParametros("$expand=itbc_itbc_tipoparametroglobal_itbc_parmetrosgl&$filter=itbc_itbc_tipoparametroglobal_itbc_parmetrosgl/itbc_Codigo%20eq%20" + codigo);
        var resultado = SDKore.OData.RetrieveMultiple("itbc_parmetrosGlobais");

        if (resultado != "undefined" && resultado != null && resultado.length > 0) {
            var objetos = [];

            var beneficio = Xrm.Page.getAttribute("itbc_beneficiodoprograma").getValue()[0].id.replace("{", "").replace("}", "");
            var unidadeNegocio = Xrm.Page.getAttribute("itbc_businessunitid").getValue()[0].id.replace("{", "").replace("}", "");

            var regExp = new RegExp(beneficio, "i");
            var regExpUnidade = new RegExp(unidadeNegocio, "i");

            $.each(resultado, function () {
                if (this["itbc_BeneficioId"] != null && this["itbc_BeneficioId"].Id != null && regExp.test(this["itbc_BeneficioId"].Id)
                && this["itbc_businessunit"] != null && this["itbc_businessunit"].Id != null && regExpUnidade.test(this["itbc_businessunit"].Id)) {
                    if (this["itbc_Valor"] != "undefined" && this["itbc_Valor"] != null && $.isNumeric(this["itbc_Valor"].replace(",", "."))) {
                        if (this["statecode"] != "undefined" && this["statecode"] != null && this["statecode"].Value == 0) {
                            objetos.push(this);
                        }
                    }
                    else
                        throw new Error("Parâmetro não Cadastrado, informar Canais");
                }
            });

            if (objetos.length <= 0)
                throw new Error("Parâmetro Global de Forma de pagamento não cadastrado, informar Canais");

            if (objetos.length > 1)
                throw new Error("Parâmetros Globais duplicados, informar Canais");

            return objetos[0]["itbc_Valor"].replace(",", ".");
        }
    },

    ObterCodigoBeneficioPrograma: function () {

        if (this._codigoBeneficioPrograma == null && Util.Xrm.HasValue("itbc_beneficiodoprograma")) {
            SDKore.OData.setBaseUrl();
            SDKore.OData.configurarParametros("$select=itbc_Codigo");
            var beneficioProgramaId = Xrm.Page.getAttribute("itbc_beneficiodoprograma").getValue()[0].id;
            this._codigoBeneficioPrograma = SDKore.OData.Retrieve(beneficioProgramaId, "itbc_beneficio").itbc_Codigo;
        }

        return this._codigoBeneficioPrograma;
    },

    SetarValoresPadraoFormaPagamentoManual: function () {

        if (SolicitacaoBeneficio.ValorDisponivelAntigo != null)
            Xrm.Page.ui.controls.get("form_benefcanal_form_benefcanal_itbc_benefdocanal_itbc_verbadisponivel").getAttribute().setValue(SolicitacaoBeneficio.ValorDisponivelAntigo);

        Xrm.Page.ui.clearFormNotification("valorSolicitado");
        Xrm.Page.ui.clearFormNotification("verbaAbater");
        Xrm.Page.getAttribute("itbc_valoraabater").setValue(null);
        SolicitacaoBeneficio.ValorDisponivelAntigo = null;
    },

    FiltrarBeneficioDoCanal: function () {

        if (SolicitacaoBeneficio.VerificarPreenchimentoLookup("itbc_benefdocanal")) {
            Xrm.Page.getAttribute("itbc_benefdocanal").setValue(null);
            Xrm.Page.getAttribute("itbc_benefdocanal").fireOnChange();
        }

        if (SolicitacaoBeneficio.VerificarPreenchimentoLookup("itbc_accountid") && SolicitacaoBeneficio.VerificarPreenchimentoLookup("itbc_businessunitid")) {

            Xrm.Page.getControl("itbc_benefdocanal").removePreSearch(this.AddCustomFilterBeneficioCanal);
            Xrm.Page.getControl("itbc_benefdocanal").addPreSearch(this.AddCustomFilterBeneficioCanal);

            Xrm.Page.ui.controls.get("itbc_benefdocanal").setDisabled(false);
        } else {
            Xrm.Page.ui.controls.get("itbc_benefdocanal").setDisabled(true);
        }
    },

    AddCustomFilterBeneficioCanal: function () {
        var fetchQuery = "<filter type=\"and\">";
        fetchQuery += "<condition attribute=\"itbc_businessunitid\" value=\"" + Xrm.Page.getAttribute("itbc_businessunitid").getValue()[0].id + "\" uitype=\"businessunit\" operator=\"eq\"/>";
        fetchQuery += "<condition attribute=\"itbc_canalid\" value=\"" + Xrm.Page.getAttribute("itbc_accountid").getValue()[0].id + "\" uitype=\"account\" operator=\"eq\"/>";
        fetchQuery += "</filter>";
        fetchQuery += "<link-entity name=\"itbc_beneficio\" alias=\"aa\" to=\"itbc_beneficioid\" from=\"itbc_beneficioid\">";
        fetchQuery += "<filter type=\"and\">";
        fetchQuery += "<condition attribute=\"itbc_passivel_solicitacao\" value=\"1\" operator=\"eq\"/>";
        fetchQuery += "</filter>";
        fetchQuery += "</link-entity>";

        Xrm.Page.getControl("itbc_benefdocanal").addCustomFilter(fetchQuery);
    },

    AdicionarFiltroCondicaoPagamento: function () {
        try {
            if (!Util.Xrm.HasValue("itbc_accountid")) return;

            var canalId = Xrm.Page.getAttribute("itbc_accountid").getValue()[0].id;

            SDKore.OData.configurarParametros("$select=itbc_ClassificacaoId");
            var classificacao = SDKore.OData.Retrieve(canalId, "Account").itbc_ClassificacaoId;

            if (classificacao == null) {
                throw "Não foi encontrado classificação para a canal selecionado.";
            }

            var fetch = "<filter type=\"and\">";
            fetch += "<condition attribute=\"itbc_utilizadorevenda\" operator=\"eq\" value=\"1\" />";
            fetch += "</filter>";

            //Xrm.Page.getControl("itbc_condicaopagamentoid").addCustomFilter(fetch);
            Xrm.Page.getControl("itbc_condicaopagamentoid").addPreSearch(function () {
                Xrm.Page.getControl("itbc_condicaopagamentoid").addCustomFilter(fetch);
            });
        } catch (e) {
            Xrm.Page.ui.setFormNotification('Não foi possível filtrar as condições de pagamento: ' + e, 'ERROR', 'FiltrarAcessoExtranetPorTipo');
        }
    },
    preencheDescricaoDefault: function () {
        setTimeout(function(){
            $('#itbc_descricao_i').val('Descrição da ação:\n\nPúblico-alvo:\n\nObjetivo:\n');
        }, 500);
    },
}