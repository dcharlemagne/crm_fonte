if (typeof (itbc_task) == "undefined") { itbc_task = {}; }

itbc_task = {
    FORMTYPE_CREATE: 1,
    FORMTYPE_UPDATE: 2,

    OnLoad: function () {

        var formType = Xrm.Page.ui.getFormType();

        if (formType == this.FORMTYPE_CREATE || formType == this.FORMTYPE_UPDATE) {

            Xrm.Page.getAttribute("itbc_tipoatividadeid").addOnChange(itbc_task.TipoAtividade_OnChange);
            Xrm.Page.getAttribute("itbc_tipoatividadeid").setSubmitMode("always");
            Xrm.Page.getControl("itbc_tipoatividadeid").setDisabled(true);

            itbc_task.TipoAtividade_OnChange();
        }
        if (Xrm.Page.getAttribute("regardingobjectid").getValue() != null && Xrm.Page.getAttribute("regardingobjectid").getValue() != undefined && Xrm.Page.getAttribute("regardingobjectid").getValue()[0].typename == "itbc_solicitacaodebeneficio") {
            Xrm.Page.getControl("itbc_resultado").setRequiredLevel("required");
        }
    },

    TipoAtividade_OnChange: function () {
        try {
            if (!Util.Xrm.HasValue("itbc_tipoatividadeid") == null) {
                return;
            }

            SDKore.OData.setBaseUrl();
            SDKore.OData.configurarParametros("$select=itbc_codigo");
            var tipoAtividadeId = Xrm.Page.getAttribute("itbc_tipoatividadeid").getValue()[0].id;
            var tipoAtividadeCodigo = SDKore.OData.Retrieve(tipoAtividadeId, "itbc_tipoatividade").itbc_codigo;

            if (tipoAtividadeCodigo == null) {
                Xrm.Page.ui.setFormNotification('O código do Tipo de Atividade não está preenchido!', 'WARNING', 'TipoAtividade_OnChange');
                return;
            }

            var resultadoOptionSet = Xrm.Page.getAttribute("itbc_resultado").getOptions();
            Xrm.Page.getControl("itbc_resultado").clearOptions();

            var optionValues = [3];
            optionValues[0] = "null";

            switch (tipoAtividadeCodigo.toString()) {

                case Config.Entidade.TipoDeAtividade.ParecerSolicitacoes.code:
                    optionValues[1] = Config.Entidade.Tarefa.Resultado.Favoravel.code;
                    optionValues[2] = Config.Entidade.Tarefa.Resultado.Desfavoravel.code;
                    break;

                case Config.Entidade.TipoDeAtividade.AprovacaoSolicitacao.code:
                case Config.Entidade.TipoDeAtividade.Checklist.code:
                    optionValues[1] = Config.Entidade.Tarefa.Resultado.Aprovada.code;
                    optionValues[2] = Config.Entidade.Tarefa.Resultado.Reprovada.code;
                    break;

                case Config.Entidade.TipoDeAtividade.AutorizarPagamento.code:
                    optionValues[1] = Config.Entidade.Tarefa.Resultado.PagamentoAutorizado.code;
                    optionValues[2] = Config.Entidade.Tarefa.Resultado.Reprovada.code;
                    break;

                case Config.Entidade.TipoDeAtividade.ExecucaoSolicitacoes.code:
                    optionValues[1] = Config.Entidade.Tarefa.Resultado.PagamentoEfetuadoPedidoGeradoSolicitacaoAtendida.code;
                    optionValues[2] = Config.Entidade.Tarefa.Resultado.PagamentoNaoEfetuadoNaoSeraGeradoPedido.code;
                    break;
            }

            for (var i in resultadoOptionSet) {
                for (var y in optionValues) {
                    if (resultadoOptionSet[i].value == optionValues[y]) {
                        Xrm.Page.getControl("itbc_resultado").addOption(resultadoOptionSet[i]);
                    }
                }
            }
        }
        catch (e) {
            Xrm.Page.ui.setFormNotification('Não foi possível carregar as ações do Tipo de Atividades!', 'WARNING', 'TipoAtividade_OnChange');
        }
    }

}