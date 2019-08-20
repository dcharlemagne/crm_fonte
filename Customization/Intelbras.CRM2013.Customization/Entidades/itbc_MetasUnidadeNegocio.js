if (typeof (MetadaUnidade) == "undefined") { MetadaUnidade = {}; }

MetadaUnidade = {

    stscodeManual: null,
    MetaCanalManualGeradoSucesso: 993520002,
    ErroGerarMetaCanalManual: 993520003,
    ErroImportarMetaCanalManual: 993520007,
    PlanilhaMetaCanalManualImportadaSucesso: 993520006,

    stscodeka: null,
    MetaKAGeradoSucesso: 993520002,
    ErroGerarMetaKA: 993520003,
    ErroImportarMetaKA: 993520007,
    PlanilhaMetaKAImportadaSucesso: 993520006,

    OnLoad: function () {
        this.stscodeManual = Xrm.Page.getAttribute('itbc_razaodostatusmetamanual').getValue();
        this.stscodeka = Xrm.Page.getAttribute('itbc_razodostatusmetakarepresentante').getValue();
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");
    },

    OnSave: function (context) {
        if (Xrm.Page.ui.getFormType() == 1) {
            Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_businessunit,itbc_ano", " - "));
        }
    },

    ProcessarPlanilha: function (orcamentodaUnidadeId) {

        if (MetadaUnidade.IsProcess()) {
            Xrm.Utility.alertDialog('Ao terminar de processar a Meta será enviado um e-mail informando.');

            Xrm.Page.getControl("statuscode").setDisabled(false);
            Xrm.Page.getAttribute('statuscode').setValue(993520004);
            Xrm.Page.getControl("statuscode").setDisabled(false);

            Xrm.Page.getAttribute("statuscode").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");

            Xrm.Page.data.entity.save();
            Xrm.Page.ui.refreshRibbon();

        } else {
            Xrm.Utility.alertDialog('Aguarde o processamento dessa Meta para aplicar outra ação');
        }
    },

    GerarTemplate: function (orcamentodaUnidadeId) {

        if (MetadaUnidade.IsProcess()) {
            Xrm.Utility.alertDialog('Ao terminar de gerar o modelo será enviado um e-mail informando.');

            Xrm.Page.getControl("statuscode").setDisabled(false);
            Xrm.Page.getAttribute('statuscode').setValue(993520000);
            Xrm.Page.getControl("statuscode").setDisabled(false);

            Xrm.Page.getAttribute("statuscode").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");

            Xrm.Page.data.entity.save();
            Xrm.Page.ui.refreshRibbon();

        } else {
            Xrm.Utility.alertDialog('Aguarde o processamento dessa Meta para aplicar outra ação');
        }
    },

    IsProcess: function () {

        if ((MetadaUnidade.stscodeManual == null
            || MetadaUnidade.stscodeManual == MetadaUnidade.MetaCanalManualGeradoSucesso
            || MetadaUnidade.stscodeManual == MetadaUnidade.PlanilhaMetaCanalManualImportadaSucesso
            || MetadaUnidade.stscodeManual == MetadaUnidade.ErroGerarMetaCanalManual
            || MetadaUnidade.stscodeManual == MetadaUnidade.ErroImportarMetaCanalManual)
            &&
            (MetadaUnidade.stscodeka == null
            || MetadaUnidade.stscodeka == MetadaUnidade.MetaKAGeradoSucesso
            || MetadaUnidade.stscodeka == MetadaUnidade.PlanilhaMetaKAImportadaSucesso
            || MetadaUnidade.stscodeka == MetadaUnidade.ErroGerarMetaKA
            || MetadaUnidade.stscodeka == MetadaUnidade.ErroImportarMetaKA))
            return true;
        else
            return false;
    },

    GerarTemplateMetaManual: function (orcamentodaUnidadeId) {

        if (MetadaUnidade.IsProcess()) {

            Xrm.Page.getControl("itbc_razaodostatusmetamanual").setDisabled(false);
            Xrm.Page.getAttribute('itbc_razaodostatusmetamanual').setValue(993520000);
            Xrm.Page.getControl("itbc_razaodostatusmetamanual").setDisabled(true);

            Xrm.Page.getAttribute("itbc_razaodostatusmetamanual").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");

            Xrm.Page.data.entity.save();
            Xrm.Page.ui.refreshRibbon();

        } else {
            Xrm.Page.ui.setFormNotification("Essa meta já está sendo processada, aguarde!", 'WARNING', "execucao");
        }
    },

    ImportarPlanilhaMetaManual: function (orcamentodaUnidadeId) {
        if (MetadaUnidade.IsProcess()) {

            Xrm.Page.getControl("itbc_razaodostatusmetamanual").setDisabled(false);
            Xrm.Page.getAttribute('itbc_razaodostatusmetamanual').setValue(993520004);
            Xrm.Page.getControl("itbc_razaodostatusmetamanual").setDisabled(true);

            Xrm.Page.getAttribute("itbc_razaodostatusmetamanual").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");

            Xrm.Page.data.entity.save();
            Xrm.Page.ui.refreshRibbon();

        } else {
            Xrm.Page.ui.setFormNotification("Essa meta já está sendo processada, aguarde!", 'WARNING', "execucao");
        }
    },

    GerarPlanilhaKa: function (metaid) {
        if (MetadaUnidade.IsProcess()) {

            Xrm.Page.getControl("itbc_razodostatusmetakarepresentante").setDisabled(false);
            Xrm.Page.getAttribute('itbc_razodostatusmetakarepresentante').setValue(993520000);
            Xrm.Page.getControl("itbc_razodostatusmetakarepresentante").setDisabled(true);

            Xrm.Page.getAttribute("itbc_razodostatusmetakarepresentante").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");

            Xrm.Page.data.entity.save();
            Xrm.Page.ui.refreshRibbon();

        } else {
            Xrm.Page.ui.setFormNotification("Essa meta já está sendo processada, aguarde!", 'WARNING', "execucao");
        }
    },

    ImportarPlanilhaKa: function (metaid) {
        if (MetadaUnidade.IsProcess()) {

            Xrm.Page.getControl("itbc_razodostatusmetakarepresentante").setDisabled(false);
            Xrm.Page.getAttribute('itbc_razodostatusmetakarepresentante').setValue(993520004);
            Xrm.Page.getControl("itbc_razodostatusmetakarepresentante").setDisabled(true);

            Xrm.Page.getAttribute("itbc_razodostatusmetakarepresentante").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");

            Xrm.Page.data.entity.save();
            Xrm.Page.ui.refreshRibbon();

        } else {
            Xrm.Page.ui.setFormNotification("Essa meta já está sendo processada, aguarde!", 'WARNING', "execucao");
        }
    }
}