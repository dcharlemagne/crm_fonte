function Form_onload() {
    Xrm.Page.getAttribute("itbc_acaocrm").setValue(false);
    PesquisarOrcamentoPor(Xrm.Page.getAttribute("new_pagamento_servico_ocorrenciaid").getValue()[0].id)
}

/**********************************
Data:      28/11/2017
Autor:     Fernando Rodrigues
Descrição: Pesquisar orçamento por id da ocorrencia
**********************************/
function PesquisarOrcamentoPor(ocorrenciaId) {

    //Configuração do serviço web
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Apoio, "PesquisarOrcamentoPor");

    //Atribuição dos paramentros
    Util.funcao.SetParameter("ocorrenciaId", ocorrenciaId);

    //Execução do serviço web
    var retorno = Util.funcao.Execute();

    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];
        if ($(data).find("Observacao").text() != null)
            Xrm.Page.ui.setFormNotification($(data).find("Observacao").text(), "INFO", "observacao_orcamento");
        if ($(data).find("Limite").text() != null && $(data).find("Limite").text() != "")
            Xrm.Page.ui.setFormNotification("O limite orçado para esta Ordem de Serviço é de R$ " + $(data).find("Limite").text(), "INFO", "limite_orcamento");
        if ($(data).find("ObservacoesServicos").text() != null && $(data).find("ObservacoesServicos").text() != "")
            Xrm.Page.ui.setFormNotification($(data).find("ObservacoesServicos").text(), "INFO", "observacoes_servico");
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
}