function Form_onload() {
    crmForm.OpenScript = function () {

        if (Xrm.Page.ui.getFormType() == 2) {
            if (!Xrm.Page.data.entity.getIsDirty()) {
                if (!Xrm.Page.getAttribute("to").getValue()) {
                    alert("Selecione pelo menos um Destinatário!");
                    Xrm.Page.getControl("to").setFocus(true);
                    return false;
                }

                if (Xrm.Page.getAttribute("to").getValue().length > 1) {
                    alert("Somente selecione um Destinatário!");
                    Xrm.Page.getControl("to").setFocus(true);
                    return false;
                }

                if (!Xrm.Page.getAttribute("regardingobjectid").getValue()) {
                    alert("Selecione pelo menos um Referente a!");
                    Xrm.Page.getControl("regardingobjectid").setFocus(true);
                    return false;
                }

                if (!Xrm.Page.getAttribute("codek_searchid").getValue()) {
                    alert("Selecione uma pesquisa a ser executada!");
                    Xrm.Page.getControl("codek_searchid").setFocus(true);
                    return false;
                }

                var strParam = "pesquisaId=" + Xrm.Page.getAttribute("codek_searchid").getValue()[0].id
                       + "&contactId=" + Xrm.Page.getAttribute("to").getValue()[0].id
                       + "&regardingObjectId=" + Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id
                       + "&otc=" + Xrm.Page.getAttribute("regardingobjectid").getValue()[0].type
                       + "&activityId=" + Xrm.Page.data.entity.getId()
                       + "&orgname=" + Xrm.Page.context.getOrgUniqueName();

                openStdDlg("/ISV/Codek.ScriptTools/script.aspx?" + strParam, null, 800, 600, null, null, null);
                window.location.href = window.location.href;
            }
            else
                alert("Salve o formulário antes de prosseguir!");
        }
        else
            alert("A execução do Script não é permitida para o Status atual da Atividade!");
    }
}
