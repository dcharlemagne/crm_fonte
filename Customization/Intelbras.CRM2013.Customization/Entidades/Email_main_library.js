function Form_onload() {
    crmForm.ObtemOcorrencia = function () {
        if (Xrm.Page.ui.getFormType() == 1
            && Xrm.Page.getAttribute("regardingobjectid").getValue() != null
            && Xrm.Page.getAttribute("regardingobjectid").getValue()[0].typename == "incident") {

            resultado = crmForm.ObterResponsavelTecnico(Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id);
            Xrm.Page.getAttribute("new_incidentid").setValue(Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id);

            obterTecnicosVinculadosEmpreseExecutante(Xrm.Page.getAttribute("regardingobjectid").getValue()[0].id);

            if (resultado.Achou == true) {
                contactId = resultado.Id;
                contactName = resultado.Nome;

                var lookupData = new Array();
                var lookupItem = new Object();

                lookupItem.id = contactId;
                lookupItem.entityType = "contact";
                lookupItem.name = contactName;

                lookupData[0] = lookupItem;
                Xrm.Page.getAttribute("to").setValue(lookupData);
            }
        }
    }

    crmForm.ObterResponsavelTecnico = function (ocorrenciaId) {

        //Configuração de parametros
        var nomeDaOrganizacao = Xrm.Page.context.getOrgUniqueName();

        //Configuração do serviço web
        Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "PesquisarResponsavelTecnicoPor");

        //Atribuição dos paramentros
        Util.funcao.SetParameter("ocorrenciaId", ocorrenciaId);
        Util.funcao.SetParameter("nomeDaOrganizacao", nomeDaOrganizacao);

        //Execução do serviço web
        var retorno = Util.funcao.Execute();
        var resultado = null;

        //Tratamento do retorno
        if (retorno.Success) {
            resultado = retorno.ReturnValue;
            if (resultado.Sucesso == false) {
                alert(resultado.MensagemDeErro);
                return false;
            }
        }
        else {
            Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
            Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
        }

        return resultado;
    }


    // Acoes
    crmForm.ObtemOcorrencia();
}

/********************************************************************
    Data:      28/12/2018
    Autor:     Jaime Campos
    Descrição: Obtem a lista de técnicos vinculados a emprese executante
********************************************************************/
function obterTecnicosVinculadosEmpreseExecutante(incident) {
    debugger;
    if (incident != null) {
        var query = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'> " +
                        "<entity name='incident'> " +
                            "<attribute name='ticketnumber' /> " +
                            "<attribute name='new_empresa_executanteid' /> " +
                            "<attribute name='incidentid' /> " +
                            "<order attribute='ticketnumber' descending='true' /> " +
                            "<filter type='and'> " +
                                "<condition attribute='incidentid' operator='eq' value='" + incident + "' />" +
                            "</filter>" +
                        "</entity>" +
                    "</fetch>";

        var resultIncident = XrmServiceToolkit.Soap.Fetch(query);

        if (resultIncident.length > 0) {
            if (resultIncident[0].attributes['new_empresa_executanteid'] != null) {
                var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                    "<entity name='contact'>" +
                                        "<attribute name='fullname'/>" +
                                        "<attribute name='emailaddress1'/>" +
                                        "<attribute name='parentcustomerid' />" +
                                        "<attribute name='contactid'/>" +
                                        "<order attribute='fullname' descending='false' />" +
                                        "<filter type='and'>" +
                                            "<condition attribute='parentcustomerid' operator='eq' uitype='account' value='{" + resultIncident[0].attributes['new_empresa_executanteid'].id + "}' />" +
                                        "</filter>" +
                                    "</entity>" +
                                "</fetch>";

                var layoutXml = '<grid name="resultset" object="1010" jump="title" select="1" icon="1" preview="1"> <row name="result" id="contactid"> <cell name="fullname" width="350"/> </row> </grid>';

                var entityName = "contact";
                var viewDisplayName = "Técnicos da empresa executante";
                var viewId = Xrm.Page.getControl("to").getDefaultView();

                Xrm.Page.getControl("to").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
            }
        }
    }
}

function Form_onsave() {

}
function regardingobjectid_onchange() {
    crmForm.ObtemOcorrencia();
}
