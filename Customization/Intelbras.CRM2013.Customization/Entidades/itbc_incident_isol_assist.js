var modalCSS = '<style>#modalListOcorrencias {font-family: Segoe UI, Arial, sans-serif;display: none;position: fixed;z-index: 999;padding-top: 30px;left: 0;top: 0;width: 100%;height: 100%;overflow: auto;background-color: rgb(0, 0, 0);background-color: rgba(0, 0, 0, 0.4);margin-bottom: 50px;padding-bottom: 20px;}\
#modalListOcorrencias .modal-header{background-color: #fefefe;margin: auto;padding: 20px;padding-bottom: 1px;padding-top: 1px;width: 90%;}\
#modalListOcorrencias .modal-content {background-color: #fefefe;margin: auto;padding: 20px;padding-top: 5px;width:90%;}\
#modalListOcorrencias .close {color: #aaaaaa;float: right;font-size: 28px;font-weight: bold;top: -8px;position: relative;right: -87px;}\
#modalListOcorrencias table {width:100%}\
#modalListOcorrencias tr{height: 35px;}\
#modalListOcorrencias #titleModal{font-weight: 400;color: #000;font-size: 24px;padding-bottom: 0px;margin-bottom: 0px;}\
#modalListOcorrencias .btn{background-image: none;float: right;position: relative;right: 30px;height: 32px;background-color: #dae0e4;border: 1px solid #C1C1C1;}\
#modalListOcorrencias thead td{border-right: 1px solid #d4d3d3;border-bottom: 1px solid #d4d3d3;border-top: 1px solid #d4d3d3;padding: 5px;font-size: 11px;text-align: center;color: #000;}\
#modalListOcorrencias tbody td{cursor: pointer; text-align: center;padding-bottom: 5px;font-size: 11px;}\
#modalListOcorrencias .close:hover,.close:focus {color: #000;text-decoration: none;cursor: pointer;\
#modalListOcorrencias #btn-listagem-astec, #modalListOcorrencias #btn-atualizar-astec{float: right;right: 50px;position: relative;font-size: 12px;background-color: #dce0dd;padding: 5px;height: 30px;background-image: none;}}</style>';

var listaOcorrencias = [];
var btnCloseOcorrencias = '<span onclick="closeModal()" class="close">&times;</span>'; 

function showModal() {
    if(!Xrm.Page.getAttribute("productserialnumber").getValue()) {
        Xrm.Page.ui.setFormNotification("Não possivel realizar a consulta ASTEC, pois o produto desta ocorrência não possui número de série", "WARNING");
    } else {
        $('#modalListOcorrencias').css('display', 'block');
        showListagemOcorrencias();
    }
}

function closeModal() {
    $('#modalListOcorrencias').css('display', 'none');
}

function setModal() {
    var modalTemp = modalCSS+'<div id="modalListOcorrencias" class="modal">\
                        <div class="modal-header"><h2 id="titleModal">Atendimentos para o número de Série: <span id="numeroDeSerie"></span></h2></div>\
                        <div class="modal-content"></div>\
                    </div>';
    
    $('body').append(modalTemp);
}

function atualizarListagemOcorrencias() {
    listaOcorrencias = [];
    showListagemOcorrencias();
}

function showListagemOcorrencias() {
    var numeroSerie = Xrm.Page.getAttribute("productserialnumber").getValue();
    var btnAtualizar = '<button id="btn-atualizar-astec" onclick="atualizarListagemOcorrencias()" type="button" class="btn">Atualizar</button>';
    $('#titleModal').empty();
    $('#titleModal').html('Atendimentos para o número de Série: '+(numeroSerie || '') + btnAtualizar + btnCloseOcorrencias);
    var conteudoModalList = '<table>\
                                <thead style="font-weight: bold;"><tr><td width="10%">Número da Ocorrência</td><td width="5%">Status</td><td width="10%">Tipo de Ocorrência</td><td width="20%">Cliente</td><td width="10%">Data Hora de Abertura</td><td width="10%">Data Hora de Conclusão</td><td width="15%">Proprietário</td><td width="20%">Descrição</td></tr></thead>\
                                <tbody id="bodyModalListAssist">\
                                    <tr><td colspan="8" style="text-align: center;padding: 20px;">Buscando Ocorrências ASTEC...</td></tr>\
                                </tbody>\
                            </table>';

    $('#modalListOcorrencias .modal-content').empty();
    $('#modalListOcorrencias .modal-content').html(conteudoModalList);

    if(numeroSerie == null || numeroSerie == 'null') {
        $('#modalListOcorrencias #bodyModalListAssist').empty();
        $('#modalListOcorrencias #bodyModalListAssist').html('<tr><td colspan="8" style="text-align: center;padding: 20px;">Nenhuma Ocorrência encontrada.</td></tr>');
    }

    if(listaOcorrencias.length == 0) {
        buscaOcorrenciasNumeroSerie(numeroSerie);
    } else {
        setListagemOcorrencias("Buscando Ocorrências ASTEC...");
    }
}

function setListagemOcorrencias(msg) {
    try {
        var htmlBody = '';

        if(listaOcorrencias.length > 0) {
            listaOcorrencias.forEach(function(obj, key) {
                htmlBody += '<tr onclick="showDetalheOcorrencia('+key+')">';
                htmlBody += '<td>'+(obj.NumeroOcorrencia || '--')+'</td>';
                htmlBody += '<td>'+(obj.DescricaoStatusOcorrencia || '--')+'</td>';
                htmlBody += '<td>'+(obj.DescricaoTipoOcorrencia || '--')+'</td>';
                htmlBody += '<td>'+(obj.NomeCliente || '--')+'</td>';
                htmlBody += '<td>'+(getDateFomartada(obj.DataHoraAbertura) || '--')+'</td>';
                htmlBody += '<td>'+(getDateFomartada(obj.DataHoraConclusao) || '--')+'</td>';
                htmlBody += '<td>'+(obj.NomeProprietario || '--')+'</td>';
                htmlBody += '<td>'+(obj.TituloOcorrencia || '--')+'</td>';
                htmlBody += '</tr>';
            });
        } else {
            msg = (msg || "Nenhuma ocorrência encontrada para este número de série.");
            htmlBody = '<tr><td colspan="8" style="text-align: center;padding: 20px;font-weight: 500;">'+msg+'</td></tr>';
        }
        
        $('#modalListOcorrencias #bodyModalListAssist').empty();
        $('#modalListOcorrencias #bodyModalListAssist').html(htmlBody);
    } catch (error) {
        $('#modalListOcorrencias #bodyModalListAssist').empty();
        $('#modalListOcorrencias #bodyModalListAssist').html('<tr><td colspan="8" style="text-align: center;padding: 20px;font-weight: 500;">Erro inesperado, formato de retorno inválido.</td></tr>');
    }
}

function showDetalheOcorrencia(indice) {
    var ocorrencia = listaOcorrencias[indice];
    var btnListagem = '<button id="btn-listagem-astec" type="button" onclick="showListagemOcorrencias()" class="btn">Listagem</button>';

    $('#titleModal').empty();
    $('#titleModal').html('Dados do Atendimento da ASTEC' + btnListagem + btnCloseOcorrencias);

    var conteudoModalList = '<table>\
                                <tbody id="bodyModalListAssist">\
                                    <tr>\
                                        <td width="15%" style="text-align: right;">Número da Ocorrência:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;">'+(ocorrencia.NumeroOcorrencia|| "--")+'</td>\
                                        <td width="15%" style="text-align: right;">Proprietpario:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;"><img alt="" class="ms-crm-Lookup-Item" src="/_imgs/ico_16_8.gif?ver=767814251">'+(ocorrencia.NomeProprietario|| "--")+'</td>\
                                    </tr>\
                                    <tr>\
                                        <td width="15%" style="text-align: right;">Cliente:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;"><img alt="" class="ms-crm-Lookup-Item" src="/_imgs/ico_16_1.gif">'+(ocorrencia.NomeCliente|| "--")+'</td>\
                                        <td width="15%" style="text-align: right;">Status:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;">'+(ocorrencia.DescricaoStatusOcorrencia|| "--")+'</td>\
                                    </tr>\
                                    <tr>\
                                        <td width="15%" style="text-align: right;">Descrição:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;">'+(ocorrencia.TituloOcorrencia|| "--")+'</td>\
                                        <td width="15%" style="text-align: right;">Data e Hora de Abertura:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;">'+(getDateFomartada(ocorrencia.DataHoraAbertura) || "--")+'</td>\
                                    </tr>\
                                    <tr>\
                                        <td width="15%" style="text-align: right;">Tipo de Ocorrência:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;">'+(ocorrencia.DescricaoTipoOcorrencia|| "--")+'</td>\
                                        <td width="15%" style="text-align: right;">Data e Hora de Conclusão:</td>\
                                        <td width="35%" style="text-align: left;font-weight: bold;">'+(getDateFomartada(ocorrencia.DataHoraConclusao) || "--")+'</td>\
                                    </tr>\
                                    <tr>\
                                        <td colspan="4" style="text-align: left;"><p style="font-weight: bold;">Resumo Atendimento:</p> <br/>'+(ocorrencia.ResumoOcorrencia|| "--")+'</td>\
                                    </tr>\
                                </tbody>\
                            </table>';

    $('#modalListOcorrencias .modal-content').empty();
    $('#modalListOcorrencias .modal-content').html(conteudoModalList);
}

function getDateFomartada(data) {
    try {
        if(data == null || data == undefined || data == '') return '--'; 
        var tempData = new Date(data);
        return tempData.toLocaleString('pt-BR');
    } catch (error) {
        return '--';
    }
}

function buscaOcorrenciasNumeroSerie(numeroSerie) {
    var urlRequest = getUrl() + "Vendas/IsolService.asmx/ObterOcorrenciaPorNumeroSerie";
    try {
        $.ajax({
            type: "POST",
            datatype: "xml",
            url: urlRequest,
            data: {
                numeroDeSerie: numeroSerie
            },
            success: function (data) {
                var msgErro = "";
                try {
                    var retornoXml = data;
                    retornoXml = new XMLSerializer().serializeToString(data);
                    retornoXml = retornoXml.replace('<?xml version="1.0" encoding="utf-8"?><string xmlns="http://schemas.microsoft.com/crm/2009/WebServices">', '');
                    retornoXml = retornoXml.replace('</string>', '');
                    var jsonTemp = JSON.parse(retornoXml);
                    if(jsonTemp['MENSAGEM']) {
                        listaOcorrencias = jsonTemp['MENSAGEM']['CONTEUDO']['MSG0315R1']['Ocorrencia'];
                    } else {
                        if(jsonTemp['Sucesso'] == false)
                        console.log(jsonTemp['Mensagem']);
                            msgErro = jsonTemp['Mensagem'].split('at Intelbras.CRM2013.Domain.Integracao.MSG0315.Enviar')[0];
                    }
                } catch (error) {
                    try {
                        var retornoXml = data;
                        retornoXml = new XMLSerializer().serializeToString(data);
                        retornoXml = retornoXml.replace('<?xml version="1.0" encoding="utf-8"?><string xmlns="http://schemas.microsoft.com/crm/2009/WebServices">', '');
                        retornoXml = retornoXml.replace('</string>', '');
                        var jsonTemp = JSON.parse(retornoXml);
                        msgErro = jsonTemp['MENSAGEM']['CONTEUDO']['MSG0315R1']['Resultado']['Mensagem'];
                    } catch (error2) {
                        msgErro = error;
                    }
                }
                setListagemOcorrencias(msgErro);
                return;
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                listaOcorrencias = [];
                result = "An error ocurred for dynamic search record.\n" +
                     "Please contact the Administrator and inform this message.\n " +
                     "Error : " + textStatus + ": " + XmlHttpRequest.statusText + " - errorThrow " + errorThrown;
                setListagemOcorrencias(result);
                console.log(result);
                return;
            }
        });
    } catch (error) {
        setListagemOcorrencias("Erro inesperado, formato de retorno inválido.");
        listaOcorrencias = [];
        return;
    }
}

function getUrl() {
    var url = window.location.host;
    var URLServico = "";

    if (url == "crm2015dev.intelbras.com.br")
    URLServico = "https://integracrmd.intelbras.com.br/";

    else if (url == "crm2015h.intelbras.com.br")
        URLServico = "https://integracrmh.intelbras.com.br/";

    else if (url == "crm2015.intelbras.com.br")
        URLServico = "https://integracrm.intelbras.com.br/";

    return URLServico;
}
