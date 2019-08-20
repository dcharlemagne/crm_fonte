function AssociarEnderecosSelecionados()
{
    if (window.parent.document.Xrm.Page.getAttribute("new_clienteid").getValue()[0] == null) return;
    if (window.parent.document.Xrm.Page.getAttribute("new_contratoid").getValue()[0] == null) return;
                          
    //Configuração de parametros
    var guidClienteParticipante = window.parent.document.location.search.substring(4, 42);
    var clienteId = window.top.opener.Xrm.Page.getAttribute("new_clienteid").getValue()[0].id;
    var contratoId = window.top.opener.Xrm.Page.getAttribute("new_contratoid").getValue()[0].id;
    var organizacao = Xrm.Page.context.getOrgUniqueName();
    var itensSelecionados = getSelected('crmGrid');

    //Configuração do serviço web
    Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.Vendas.Isolservice, "AdicionarParticipante");

    //Atribuição dos paramentros
    Util.funcao.SetParameter("clienteId", clienteId);
    Util.funcao.SetParameter("contratoId", contratoId);
    Util.funcao.SetParameter("guidClienteParticipante", guidClienteParticipante);
    Util.funcao.SetParameter("enderecosSelecionados", itensSelecionados);
    Util.funcao.SetParameter("organizacao", organizacao);

    //Execução do serviço web
    var retorno = Util.funcao.Execute();
    var resultado = null;

    //Tratamento do retorno
    if (retorno.Success) {
        resultado = retorno.ReturnValue;
        if (resultado.Sucesso == false)
            alert(resultado.MensagemDeErro);
        else
            alert('Endereços incluídos com sucesso.\nALERTA : antes de salvar as alterações, preencher o campo \'Localidade\'.');
    }
    else
        alert('Ocorreu um erro no processo!');
} 