if (typeof (Denuncia) == "undefined") { Denuncia = {}; }

Denuncia = {
    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    RealizarAcaoDenuncia: function () {
        debugger;
        var parameters = {};

        var acaoTomada = Xrm.Page.getAttribute("itbc_acaotomada").getText();
        // tipoSolicitacao é como esta escrito o atributo no CRM
        var tipoSolicitacao = "";
        if (acaoTomada != "undefined" && acaoTomada != null && acaoTomada != "") {
            if (acaoTomada == "Alerta ao Canal") {
                //Nao tem solicitacao correspondente
                Xrm.Utility.openEntityForm("itbc_solicitacaodecadastro");
            }
            else if (acaoTomada == "Perda de Benefício") {
                tipoSolicitacao = "Informar compromisso não cumprido";
            }
            else if (acaoTomada == "Descredenciamento") {
                tipoSolicitacao = "Solicitar descredenciamento de um canal";
            }

            var query = "$select=itbc_tipodesolicitacaoId&$filter=itbc_name eq '" + tipoSolicitacao + "' ";

            var records = Util.funcao.retornarRegistrosSincrono("itbc_tipodesolicitacaoSet", query);

            var registro = records.results[0];
            if (registro != "undefined" && registro != null && registro != "") {
                parameters["itbc_tipodesolicitacaoid"] = registro.itbc_tipodesolicitacaoId;
                parameters["itbc_tipodesolicitacaoidname"] = tipoSolicitacao;
                //parameters["itbc_tipodesolicitacaoidtype"] = "itbc_tipodesolicitacao";

                Xrm.Utility.openEntityForm("itbc_solicitacaodecadastro", null, parameters);
            }
        } else {
            Xrm.Utility.alertDialog("O campo Ação Tomada precisa esta preenchido!");
        }
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },

    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        Denuncia.DisabledEnabled(false);
        Xrm.Page.getAttribute("itbc_name").setValue("Denuncia de:" + Util.funcao.ContatenarCampos("itbc_tipodedenunciaid", ""));

        Denuncia.DisabledEnabled(true);
        Denuncia.ForceFieldSave();
    },

    VerificaDenunciante: function () {
        var tipoDoDenunciante = Xrm.Page.getAttribute("itbc_tipododenunciante").getValue();
        var nomeDoDenunciante = null;

        switch (tipoDoDenunciante) {
            case 993520001: /* Colaborador do Canal */
                if (Xrm.Page.getAttribute("itbc_colaboradordocanalid").getValue() != null)
                    nomeDoDenunciante = Xrm.Page.getAttribute("itbc_colaboradordocanalid").getValue()[0].name;
                break;
            case 993520002: /* Colaborador Intelbras */
                if (Xrm.Page.getAttribute("itbc_colaboradorintelbrasid").getValue() != null)
                    nomeDoDenunciante = Xrm.Page.getAttribute("itbc_colaboradorintelbrasid").getValue()[0].name;
                break;
            case 993520003: /* Key Account / Representante */
                if (Xrm.Page.getAttribute("itbc_keyaccountourepresentanteid").getValue() != null)
                    nomeDoDenunciante = Xrm.Page.getAttribute("itbc_keyaccountourepresentanteid").getValue()[0].name;
                break;
        }

        if (nomeDoDenunciante != null)
            Xrm.Page.getAttribute("itbc_denunciante").setValue(nomeDoDenunciante);
    }
}
