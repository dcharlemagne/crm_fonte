function Form_onload() {
    crmForm.FilterLookup = function (attribute, url, param, ObjectTypeName) {
        /*
        if (param != null)
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
                //attribute.fireOnChange();
            }
    
        };*/
    };


    crmForm.FiltraContratos = function () {
        var oParam = "objectTypeCode=10101&filterDefault=false";
        var campoContratoId = Xrm.Page.getAttribute("new_contratoid");

        crmForm.FilterLookup(campoContratoId, "/ISV/Tridea.Web.Helper/FilterLookup/FilterLookup.aspx", oParam, "contract");
    }


    /**********************************
    Ações do OnLoad.
    **********************************/
    crmForm.FiltraContratos();
}
function Form_onsave() {
    //*Copiar o nome do Contato + Contrato//
    Xrm.Page.getAttribute("new_name").setValue(Xrm.Page.getAttribute("new_contatoid").getValue()[0].name + " - " + Xrm.Page.getAttribute("new_contratoid").getValue()[0].name);
}