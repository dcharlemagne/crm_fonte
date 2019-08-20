/// <reference path="d:\intelbras\canais\branch\2015-07-23\adesaomonitoramento\customization\intelbras.crm2013.customization\sdkorebase\sdkore.js" />
/// <reference path="d:\intelbras\canais\branch\2015-07-23\adesaomonitoramento\customization\intelbras.crm2013.customization\util\util.js" />

if (typeof (Xrm) == "undefined") { Xrm = parent.Xrm; }
if (typeof (ItbcSiteMap) == "undefined") { ItbcSiteMap = {}; }

ItbcSiteMap = {

    FORMTYPE_CREATE: 1,
    FORMTYPE_UPDATE: 2,

    OnLoad: function () {
        $("#but_confirm").click(ItbcSiteMap.Ribbon_AdesaoBatch);
        $("#but_cancelar").click(ItbcSiteMap.Cancelar);
    },

    Ribbon_AdesaoBatch: function () {
        // Verifica se o processo já está em execução.
        try {

            //Configuração do serviço web
            Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "EstaExecutandoAdesaoAoProgramaTodasAsContas");

            //Execução do serviço web
            var retorno = Util.funcao.Execute();
            var resultado = null;

            //Tratamento do retorno
            if (retorno.Success) {
                var resultado = $.parseJSON(retorno["ReturnValue"]);

                if (resultado.Sucesso) {

                    if (resultado.EstaExecutando) {
                        alert("O processo já está e execução.\n\n" + resultado.Mensagem);
                        return;
                    }
                } else {
                    Xrm.Utility.alertDialog(resultado.MensagemErro);
                }
            }
        }
        catch (erro) {
            Xrm.Utility.alertDialog(erro.message);
        }

        // if (!confirm("Este procedimento realiza o processo de Adesão para todas as contas Participantes do Programa de Canais. Este é um processo demorado, dependendo das configurações das contas e pode levar algumas horas para ser concluído. Deseja continuar?"))
        // return;

        // Diapara o Processo.
        try {
            //Configuração do serviço web
            Util.funcao.CallServiceJSON(Config.ParametroGlobal.IntegrationWS.CRM.CrmWebServices, "AdesaoAoProgramaTodasAsContas");

            //Execução do serviço web
            var retorno = Util.funcao.Execute();
            var resultado = null;

            //Tratamento do retorno
            if (retorno.Success) {
                var resultado = $.parseJSON(retorno["ReturnValue"]);

                if (resultado.Sucesso) {

                    if (resultado.EstaExecutando) {
                        alert("O processo já está e execução.\n\n" + resultado.Mensagem);
                        return;
                    }

                    alert('Processamento iniciado com sucesso. Você pode acompanhar a evolução do processamento clicando novamente no botão "Revalidação Adesão". Ao final do processo será enviado um e-mail com os erros encontrados para o e-mail configurado no Parâmetro Global "Contatos Administrativos".');

                } else {
                    Xrm.Utility.alertDialog(resultado.MensagemErro);
                }
            }
        }
        catch (erro) {
            Xrm.Utility.alertDialog(erro.message);
        }
    },

    Cancelar: function () {
        window.history.back();
    }
}

window.onload = ItbcSiteMap.OnLoad;