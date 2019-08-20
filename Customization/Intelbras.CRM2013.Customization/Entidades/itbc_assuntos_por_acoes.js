/**********************************
   Variáveis Globais.
**********************************/
var validacaoAssunto = false;


/**********************************
Data:      31/08/2018  
Autor:     Jaime Campos
Descrição: Pesquisa o assunto por Guid
**********************************/
function PesquisaAssuntoPor(assuntoId) {
    Util.funcao.CallServiceXML(Config.ParametroGlobal.IntegrationWS.Vendas.Apoio, "PesquisarAssuntoPor");
    Util.funcao.SetParameter("assuntoId", assuntoId);
    Util.funcao.SetParameter("organizacaoNome", Xrm.Page.context.getOrgUniqueName());

    //Execução do serviço web
    var retorno = Util.funcao.Execute();
    //Tratamento do retorno
    if (retorno["Success"] == true) {
        var data = retorno['ReturnValue'];

        if ($(data).find("Sucesso").text() == "false" || $(data).find("Sucesso").text() == "False") {
            alert($(data).find("MensagemDeErro").text());
            return "0";
        }
        if ($(data).find("Achou").text() == "false" || $(data).find("Achou").text() == "False")
            return "0";
    }
    else {
        Xrm.Page.ui.setFormNotification('Ocorreu um erro no registro de pesquisa dinâmica.', 'ERROR');
        Xrm.Page.ui.setFormNotification("Por favor, entre em contato com o Administrador do Sistema!", "WARNING")
    }
    return data;
}

function OnSave(contexto) {
    var resultado = PesquisaAssuntoPor(Xrm.Page.getAttribute("itbc_assunto_id").getValue()[0].id);

    Xrm.Page.getAttribute("itbc_name").setValue($(resultado).find("EstruturaAssunto").text() + " :: " + Xrm.Page.getAttribute("itbc_acao_id").getValue()[0].name);

    if ($(resultado).find("TemAssuntoFilho").text() == true)
        validacaoAssunto = true;
    else
        validacaoAssunto = false;

    if (validacaoAssunto) {
        alert("O Assunto selecionado não é o ultimo da estrutura.");
        contexto.getEventArgs().preventDefault(); //evita que o Salvar continue (contexto deve ser configurado no CRM)
        return;
    }
}