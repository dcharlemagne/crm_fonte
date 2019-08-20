if (typeof (Produto) == "undefined") { Produto = {}; }

var vlrOriginal;

Produto.Formulario = {

    OnLoad: function () {
        crmForm.all.statuscode.attachEvent('onclick', Produto.Formulario.ValorOriginal);
    },

    ConfirmarStatus: function (contexto) {
        //if (window.confirm("Se você continuar, o sistema irá alterar o status de Produtos do Portfólio, Produtos do Treinamento e Produtos da Política Comercial. Tem certeza que deseja continuar?")) {
        Xrm.Utility.confirmDialog("Se você continuar, o sistema irá alterar o status de Produtos do Portfólio, Produtos do Treinamento e Produtos da Política Comercial. Tem certeza que deseja continuar?", function () {
            //Xrm.Page.data.entity.save(); //Yes
        }, function () {
            AddressType = Xrm.Page.data.entity.attributes.get("statuscode").setValue(vlrOriginal);//No
        });
    },

    ValorOriginal: function () {
        vlrOriginal = Xrm.Page.data.entity.attributes.get("statuscode").getValue();
    },

    OnSave: function () {
        Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
        Xrm.Page.getAttribute("itbc_acaocrm").setSubmitMode("always");
    },

    IntervencaoTecnicaOnChange: function () {
        if (Xrm.Page.getAttribute("new_intervencao_tecnica").getValue() == true) {
            Xrm.Page.getAttribute("new_data_intervencao_tecnica").setRequiredLevel("required");
            Xrm.Page.getAttribute("new_descricao_intervencao_tecnica").setRequiredLevel("required");
            Xrm.Page.getAttribute("new_motivo_intervencao_tecnica").setRequiredLevel("required");
        } else {
            Xrm.Page.getAttribute("new_data_intervencao_tecnica").setRequiredLevel("none");
            Xrm.Page.getAttribute("new_descricao_intervencao_tecnica").setRequiredLevel("none");
            Xrm.Page.getAttribute("new_motivo_intervencao_tecnica").setRequiredLevel("none");
        }
    },

    LogisticaReversaOnChange: function () {
        if (Xrm.Page.getAttribute("new_logistica_reversa").getValue() == false) {
            Xrm.Page.getAttribute("itbc_processamento_material").setValue("");
            Xrm.Page.getAttribute("itbc_motivo_tratativa_diferenciada").setValue("");
        }
    },

    ProcessamentoMaterialOnChange: function () {
        if (Xrm.Page.getAttribute("itbc_processamento_material").getValue() != 100000004) {
            Xrm.Page.getAttribute("itbc_motivo_tratativa_diferenciada").setValue("");
        }
    },
}