if (typeof (OrcamentodaUnidade) == "undefined") { OrcamentodaUnidade = {}; }

OrcamentodaUnidade = {

    EntityName: "itbc_orcamentodaunidade",

    stscode: Xrm.Page.getAttribute('statuscode').getValue(),
    Ativo: 1,
    PlanilhaOrcamentoImportadaSucesso: 993520005,
    ErroImportarPlanilhaOrcamento: 993520007,
    ModelodeOrcamentoGeradocomSucesso: 993520003,
    ErroGeracaoModeloOrcamento: 993520002,

    stscodemanual: Xrm.Page.getAttribute('itbc_razaodostatusoramentomanual').getValue(),
    ErroGerarOrcamentoCanalManual: 993520003,
    ErroImportarOrcamentoCanalManual: 993520007,
    PlanilhaOrcamentoCanalManualImportadaSucesso: 993520006,
    OrcamentoCanalManualGeradoSucesso: 993520002,

    FormularioCreate: 1,
    typeForm: Xrm.Page.ui.getFormType(),

    nivelorcamento: Xrm.Page.getAttribute('itbc_niveldoorcamento').getValue(),
    nivelorcamentoresumido: 993520001,
    nivelorcamentodetalhado: 993520000,


    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

        if (OrcamentodaUnidade.typeForm != OrcamentodaUnidade.FormularioCreate) {
            Xrm.Page.getControl("itbc_unidadedenegocioid").setDisabled(true);
            Xrm.Page.getControl("itbc_ano").setDisabled(true);
            Xrm.Page.getControl("itbc_niveldoorcamento").setDisabled(true);
            Xrm.Page.getControl("itbc_name").setDisabled(true);
        }


    },

    OnSave: function (context) {
        //if (Xrm.Page.ui.getFormType() == 1)
        //    Xrm.Utility.alertDialog('Será enviado um e-mail informando que a planilha de Orçamento encontra-se anexada \n ao orçamento.');

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_unidadedenegocioid,itbc_ano", " - "));

    },

    IsProcess: function () {
     
        if ((OrcamentodaUnidade.stscodemanual == null
            || OrcamentodaUnidade.stscodemanual == OrcamentodaUnidade.ErroGerarOrcamentoCanalManual
            || OrcamentodaUnidade.stscodemanual == OrcamentodaUnidade.ErroImportarOrcamentoCanalManual
            || OrcamentodaUnidade.stscodemanual == OrcamentodaUnidade.PlanilhaOrcamentoCanalManualImportadaSucesso
            || OrcamentodaUnidade.stscodemanual == OrcamentodaUnidade.OrcamentoCanalManualGeradoSucesso)
            &&
            (OrcamentodaUnidade.stscode == OrcamentodaUnidade.Ativo
            || OrcamentodaUnidade.stscode == OrcamentodaUnidade.ErroGerarOrcamentoCanalManual
            || OrcamentodaUnidade.stscode == OrcamentodaUnidade.ErroImportarOrcamentoCanalManual
            || OrcamentodaUnidade.stscode == OrcamentodaUnidade.OrcamentoCanalManualGeradoSucesso
            || OrcamentodaUnidade.stscode == OrcamentodaUnidade.PlanilhaOrcamentoImportadaSucesso))
            return true;
        else
            return false;

    },

    ProcessarPlanilha: function (orcamentodaUnidadeId) {

        if (OrcamentodaUnidade.IsProcess()) {
            Xrm.Utility.alertDialog('Ao terminar de gerar o modelo será enviado um e-mail informando.');

            Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(false);
            Xrm.Page.getAttribute('itbc_mensagem_processamento').setValue(null);
            Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(true);

            Xrm.Page.getControl("statuscode").setDisabled(false);
            Xrm.Page.getAttribute('statuscode').setValue(993520004);
            Xrm.Page.getControl("statuscode").setDisabled(true);

            Xrm.Page.getAttribute("statuscode").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");
            Xrm.Page.data.entity.save();
        } else {
            Xrm.Utility.alertDialog('Aguarde o processamento desse orçamento para aplicar outra ação');
        }
    },

    GerarTemplateOrcamento: function (orcamentodaUnidadeId) {
        
        if (OrcamentodaUnidade.IsProcess()) {
            Xrm.Utility.alertDialog('Ao terminar de gerar o modelo será enviado um e-mail informando.');

            Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(false);
            Xrm.Page.getAttribute('itbc_mensagem_processamento').setValue(null);
            Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(true);

            Xrm.Page.getControl("statuscode").setDisabled(false);
            Xrm.Page.getAttribute('statuscode').setValue(993520000);
            Xrm.Page.getControl("statuscode").setDisabled(true);

            Xrm.Page.getAttribute("statuscode").setSubmitMode("always");
            Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");
            Xrm.Page.data.entity.save();
        } else {
            Xrm.Utility.alertDialog('Aguarde o processamento desse orçamento para aplicar outra ação');
        }
    },

    GerarTemplateOrcamentoManual: function (orcamentodaUnidadeId) {

        if (OrcamentodaUnidade.nivelorcamento == OrcamentodaUnidade.nivelorcamentoresumido) {

            if (OrcamentodaUnidade.IsProcess()) {
                Xrm.Utility.alertDialog('Ao terminar de gerar o modelo será enviado um e-mail informando.');

                Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(false);
                Xrm.Page.getAttribute('itbc_mensagem_processamento').setValue(null);
                Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(true);

                Xrm.Page.getControl("itbc_razaodostatusoramentomanual").setDisabled(false);
                Xrm.Page.getAttribute('itbc_razaodostatusoramentomanual').setValue(993520000);
                Xrm.Page.getControl("itbc_razaodostatusoramentomanual").setDisabled(true);

                Xrm.Page.getAttribute("itbc_razaodostatusoramentomanual").setSubmitMode("always");
                Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");

                Xrm.Page.data.entity.save();
            } else {
                Xrm.Utility.alertDialog('Aguarde o processamento desse orçamento para aplicar outra ação');
            }
        } else {
            Xrm.Utility.alertDialog('Gerar Template Manual, somente para nível resumido');
        }
    },

    ImportarPlanilhaManual: function (orcamentodaUnidadeId) {
        if (OrcamentodaUnidade.nivelorcamento == OrcamentodaUnidade.nivelorcamentoresumido) {

            if (OrcamentodaUnidade.IsProcess()) {
                Xrm.Utility.alertDialog('Ao terminar de gerar o modelo será enviado um e-mail informando.');

                Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(false);
                Xrm.Page.getAttribute('itbc_mensagem_processamento').setValue(null);
                Xrm.Page.getControl("itbc_mensagem_processamento").setDisabled(true);

                Xrm.Page.getControl("itbc_razaodostatusoramentomanual").setDisabled(false);
                Xrm.Page.getAttribute('itbc_razaodostatusoramentomanual').setValue(993520004);
                Xrm.Page.getControl("itbc_razaodostatusoramentomanual").setDisabled(true);

                Xrm.Page.getAttribute("itbc_razaodostatusoramentomanual").setSubmitMode("always");
                Xrm.Page.getAttribute("itbc_mensagem_processamento").setSubmitMode("always");
                Xrm.Page.data.entity.save();
            } else {
                Xrm.Utility.alertDialog('Aguarde o processamento desse orçamento para aplicar outra ação');
            }
        } else {
            Xrm.Utility.alertDialog('Gerar Template Manual, somente para nível resumido');
        }
    }

}