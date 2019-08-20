if (typeof (DeParaDeUnidadeDoKonviva) == "undefined") { DeParaDeUnidadeDoKonviva = {}; }

DeParaDeUnidadeDoKonviva = {

    TipoDePara: { ColaboradorIntelbras: 993520003 },

    TipoRelacao: {
        TodasOpcoes: null,
        ClienteFinal: 1,
        ColaboradorIntelbras: 993520000,
        KeyAccountRepresentante: 993520007
    },

    PapelCanalIntelbras: {
        TodasOpcoes: null,
        Outros: 993520004,
        RH: 993520005,
        Trade: 993520006,
        PosVendas: 993520007,
        Instrutores: 993520008,
        Representante: 993520009
    },

    ConjuntoOpcoesPorTipoRelacao: {
        ColaboradorIntelbras: new Array()
    },

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

        DeParaDeUnidadeDoKonviva.TipoRelacao.TodasOpcoes = Xrm.Page.getControl("itbc_tipoderelacao").getAttribute().getOptions();
        DeParaDeUnidadeDoKonviva.PapelCanalIntelbras.TodasOpcoes = Xrm.Page.getControl("itbc_papelnocanalintelbras").getAttribute().getOptions();
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();
        Xrm.Page.ui.clearFormNotification("1");
        Xrm.Page.ui.setFormNotification("As alterações realizadas só serão replicadas nos acessos ao Konviva existentes no próximo dia util.", "WARNING", "1");
        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_classificacaoid,itbc_categoriaid,itbc_subclassificacaoid,itbc_tipoderelacao,itbc_papelnocanalintelbras", " - "));
    },

    AtribuirFilhos: function (executionContext) {
        var campoAttribute = executionContext.getEventSource();
        var tipoRelacaoControl = Xrm.Page.getControl("itbc_tipoderelacao");
        var papelcanalControl = Xrm.Page.getControl("itbc_papelnocanalintelbras");

        switch (campoAttribute.getValue()) {
            case DeParaDeUnidadeDoKonviva.TipoDePara.ColaboradorIntelbras:
                if (campoAttribute.getName() == "itbc_tipodedepara") {
                    tipoRelacaoControl.removeOption(1);
                }
                break;
            case DeParaDeUnidadeDoKonviva.TipoRelacao.ColaboradorIntelbras:
                if (campoAttribute.getName() == "itbc_tipoderelacao") {
                    DeParaDeUnidadeDoKonviva.AdicionarOpcoes(DeParaDeUnidadeDoKonviva.PapelCanalIntelbras.TodasOpcoes, papelcanalControl);
                    papelcanalControl.removeOption(993520009);
                }
                break;
            case DeParaDeUnidadeDoKonviva.TipoRelacao.KeyAccountRepresentante:
                break;
            default:
                if (DeParaDeUnidadeDoKonviva.TipoRelacao.TodasOpcoes != null) {
                    tipoRelacaoControl.clearOptions();
                    DeParaDeUnidadeDoKonviva.AdicionarOpcoes(DeParaDeUnidadeDoKonviva.TipoRelacao.TodasOpcoes, tipoRelacaoControl);
                }
                if (DeParaDeUnidadeDoKonviva.PapelCanalIntelbras.TodasOpcoes != null) {
                    papelcanalControl.clearOptions();
                    DeParaDeUnidadeDoKonviva.AdicionarOpcoes(DeParaDeUnidadeDoKonviva.PapelCanalIntelbras.TodasOpcoes, papelcanalControl);
                }
                break;
        }
    },

    AdicionarOpcoes: function (opcoes, control) {
        if (control.getDisabled())
            control.setDisabled(false);

        control.clearOptions();

        for (var i = 0; i < (opcoes.length - 1) ; i++)
            control.addOption(opcoes[i]);
    }
}