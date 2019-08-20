function Form_onload() {
    /*Desabilita o campo Logística Reversa e Data de Envio*/
    Xrm.Page.getControl("new_logistica_reversa").setDisabled(true);
    Xrm.Page.getControl("new_data_envio_posto").setDisabled(true);
    Xrm.Page.getControl("new_nota_fiscal_posto").setDisabled(true);

    /*Desabilita o campo data de recebimento quando  Logistica estiver igual a Nao*/
    if (Xrm.Page.getAttribute("new_logistica_reversa").getValue() == 0) {
        Xrm.Page.getControl("new_data_recebimento").setDisabled(true);
    }

    /******************************Funções******************************************/
    /*** Filtra um FilterLookup ***/
    crmForm.FilterLookup = function (attribute, url, param, ObjectTypeName) {
        /*if (param != null)
            url += "?" + param + "&orgname=" + Xrm.Page.context.getOrgUniqueName();
        else
            url += "?orgname=" + Xrm.Page.context.getOrgUniqueName();
    
        oImg = eval('attribute' + '.parentNode.childNodes[0]');
    
        oImg.onclick = function () {
            retorno = openStdDlg(url, null, 600, 450);
    
            if (typeof (retorno) != "undefined") {
    
                strValues = retorno.split('*|*');
                var lookupData = new Array();
                lookupItem = new Object();
                lookupItem.id = "{" + strValues[1] + "}";
                //lookupItem.type = parseInt(strValues[2]);
                lookupItem.entityType = ObjectTypeName;
                lookupItem.name = strValues[0];
                lookupData[0] = lookupItem;
                attribute.setValue(lookupData);
                attribute.fireOnChange();
            }
    
        };*/
    };

    crmForm.filtraDefeitoServico = function (campoDefeito, campoServico) {
        var id = "";

        if (campoDefeito.getValue())
            id = campoDefeito.getValue()[0].id;

        var oParam = "objectTypeCode=20011&filterDefault=false&_new_defeitoid=" + id;
        crmForm.FilterLookup(campoServico.getValue()[0].id, "/ISV/Tridea.Web.Helper/FilterLookup/FilterLookup.aspx", oParam, "new_servico_assistencia_tecnica");

        if (campoDefeito.IsDirty)
            campoServico.setValue(null);
    }

    /******************************
    Ações OnLoad
    ******************************/
    // Função Filter Lookup
    if (Xrm.Page.getAttribute("new_defeitoid").getValue() == null)
        Xrm.Page.getAttribute("new_defeitoid").fireOnChange();

    if (Xrm.Page.getAttribute("new_defeitoid").getValue() != null) {
        crmForm.filtraDefeitoServico(Xrm.Page.getAttribute("new_defeitoid"), Xrm.Page.getAttribute("new_servicoid"));
    }
}
function Form_onsave() {
    /*Concatena os campos Produto,Defeito e Serviço no campo Nome*/
    Xrm.Page.getAttribute("new_name").setValue((Xrm.Page.getAttribute("new_produtoid").getValue() != null ? Xrm.Page.getAttribute("new_produtoid").getValue()[0].name : "") + "--" + (Xrm.Page.getAttribute("new_defeitoid").getValue() != null ? Xrm.Page.getAttribute("new_defeitoid").getValue()[0].name : "") + '--' + (Xrm.Page.getAttribute("new_servicoid").getValue() != null ? Xrm.Page.getAttribute("new_servicoid").getValue()[0].name : ""));
}
function new_defeitoid_onchange() {
    // Função Filter Lookup
    crmForm.filtraDefeitoServico(Xrm.Page.getAttribute("new_defeitoid"), Xrm.Page.getAttribute("new_servicoid"));
}
function statuscode_onchange() {

}