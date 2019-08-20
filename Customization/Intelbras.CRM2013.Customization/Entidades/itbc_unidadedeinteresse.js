if (typeof (itbc_unidadedeinteresse) == "undefined") { itbc_unidadedeinteresse = {}; }

itbc_unidadedeinteresse = {
    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();
        itbc_unidadedeinteresse.DisabledEnabled(false);
        //criando variavel para unidade de negócio e fazendo substring
        if (Xrm.Page.getAttribute("itbc_unidadedenegocioid").getValue() != null) {
            var lookupitem = new Array;
            lookupitem = Xrm.Page.getAttribute("itbc_unidadedenegocioid").getValue();
            var unidadeNegocio = lookupitem[0].name;
            unidadeNegocio.substring(1, 20);
        }
        else { var unidadeNegocio = "Não Definida" }

        //criando variavel para valor
        if (Xrm.Page.getAttribute("itbc_valordoorcamento").getValue() != null) {
            var valor = parseFloat(Xrm.Page.getAttribute("itbc_valordoorcamento").getValue());
        }
        else { var valor = "0,00" }

        //concatena valores no nome
        Xrm.Page.getAttribute("itbc_name").setValue(unidadeNegocio + " - " + valor);

        itbc_unidadedeinteresse.DisabledEnabled(true);
        itbc_unidadedeinteresse.ForceFieldSave();
    },
}