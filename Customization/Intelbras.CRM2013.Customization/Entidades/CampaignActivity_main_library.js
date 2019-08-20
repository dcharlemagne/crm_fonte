function IFRAME_HTMLsCampanha_onload() {

}
function Form_onload() {
    var CRM_FORM_TYPE_CREATE = 1;
    var CRM_FORM_TYPE_UPDATE = 2;
    switch (Xrm.Page.ui.getFormType()) {
        case CRM_FORM_TYPE_CREATE:
            crmForm.all.tab1Tab.style.display = "none";
            break;

        case CRM_FORM_TYPE_UPDATE:
            crmForm.all.tab1Tab.style.display = "";
            break;
    }

    Xrm.Page.getAttribute("codek_searchid").fireOnChange();
}
function codek_searchid_onchange() {
    crmForm.SetFieldReqLevel("codek_url_answer_search", (Xrm.Page.getAttribute("codek_searchid").getValue() != null));
}
