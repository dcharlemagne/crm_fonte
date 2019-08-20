if (typeof (CompromissosCanal) == "undefined") { CompromissosCanal = {}; }

CompromissosCanal = {
    //enum do monitoramento manual
    MonitoramentoManual: 993520002,

    OnLoad: function () {
        Xrm.Page.getAttribute("itbc_name").setRequiredLevel("none");

        switch (Xrm.Page.ui.getFormType()) {
            case 0: //Undefined
                break;
            case 2: //Update
                SDKore.OData.configurarParametros("$select=itbc_TipodeMonitoramento");

                try {
                    //Verificamos se o campo compromisso está preenchido
                    if (Xrm.Page.getAttribute("itbc_compdoprogid") != null && Xrm.Page.getAttribute("itbc_compdoprogid").getValue() != null && Xrm.Page.getAttribute("itbc_compdoprogid").getValue() != "undefined") {
                        var guid = Xrm.Page.getAttribute("itbc_compdoprogid").getValue()[0].id;
                        var resposta = SDKore.OData.Retrieve(guid, "itbc_compromissos");
                        if (typeof resposta == "object" && resposta.itbc_TipodeMonitoramento != null && resposta.itbc_TipodeMonitoramento.Value != null && resposta.itbc_TipodeMonitoramento.Value != "undefined") {
                            if (resposta.itbc_TipodeMonitoramento.Value == CompromissosCanal.MonitoramentoManual) {
                                Xrm.Page.getControl("itbc_statuscompromissosid").setDisabled(false);
                            }
                            else {
                                Xrm.Page.getControl("itbc_statuscompromissosid").setDisabled(true);
                            }

                        }
                    }
                }
                catch (e) {
                    alert(e.message);
                }
                break;
        }
    },

    //false= habilita para edição/true desabilita para edicao
    DisabledEnabled: function (valor) {
        Xrm.Page.getControl("itbc_name").setDisabled(valor);
    },

    ForceFieldSave: function () {
        Xrm.Page.getAttribute("itbc_name").setSubmitMode("always");
    },


    OnSave: function (context) {
        var eventArgs = context.getEventArgs();

        CompromissosCanal.DisabledEnabled(false);

        Xrm.Page.getAttribute("itbc_name").setValue(Util.funcao.ContatenarCampos("itbc_canalid,itbc_compdoprogid", " - "));

        CompromissosCanal.DisabledEnabled(true);
        CompromissosCanal.ForceFieldSave();
    }

}