function Form_onload() {
    /******************************Funções******************************************/
    /*** Filtra um FilterLookup ***/
    crmForm.FilterLookup = function (attribute, url, param, ObjectTypeName) {
        /*
        if (param != null)
            url += "?" + param + "&orgname=" + Xrm.Page.context.getOrgUniqueName();
        else
            url += "?orgname=" + Xrm.Page.context.getOrgUniqueName();

        oImg = eval('attribute' + '.parentNode.childNodes[0]');

        oImg.onclick = function() {

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
        crmForm.FilterLookup(campoServico,
            "/ISV/Tridea.Web.Helper/FilterLookup/FilterLookup.aspx",
            oParam,
            "new_servico_assistencia_tecnica");

        if (campoDefeito.IsDirty)
            campoServico.setValue(null);

    }


    /******************************
    Ações OnLoad
    ******************************/
    // Função Filter Lookup
    if (Xrm.Page.getAttribute("new_defeitoid").getValue() == null)
        Xrm.Page.getAttribute("new_defeitoid").fireOnChange();
    else
        crmForm.filtraDefeitoServico(Xrm.Page.getAttribute("new_defeitoid"), Xrm.Page.getAttribute("new_servicoid"));
}

function Form_onsave() {
    /*Concatenar o Nome do Cliente e da Linha no Campo Nome*/
    if (Xrm.Page.getAttribute("new_linha_unidade_negocioid").getValue() != null)
        Xrm.Page.getAttribute("new_name").getValue(Xrm.Page.getAttribute("new_linha_unidade_negocioid").getValue()[0].name + '--' + Xrm.Page.getAttribute("new_defeitoid").getValue()[0].name + '--' + Xrm.Page.getAttribute("new_servicoid").getValue()[0].name);
    else
        Xrm.Page.getAttribute("new_name").getValue(Xrm.Page.getAttribute("new_defeitoid").getValue()[0].name + '--' + Xrm.Page.getAttribute("new_servicoid").getValue()[0].name);

}
function new_defeitoid_onchange() {
    // Função Filter Lookup
    crmForm.filtraDefeitoServico(Xrm.Page.getAttribute("new_defeitoid"), Xrm.Page.getAttribute("new_servicoid"));
}
function new_servicoid_onchange() {

}