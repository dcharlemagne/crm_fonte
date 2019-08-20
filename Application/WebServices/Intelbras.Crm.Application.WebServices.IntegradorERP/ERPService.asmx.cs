using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.IO;
using Intelbras.CRM2013.Domain;
using System.Diagnostics;

namespace Intelbras.CRM2013.Application.WebServices.IntegradorERP
{
    /// <summary>
    /// Summary description for ERPService
    /// </summary>
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2009/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
    public class ERPService : WebServiceBase
    {
        [WebMethod]
        public string InsertIntoIntegrationLog(string organizationName, string origin, string entity, string action, string message, DateTime messageDate, int status)
        {
            //SDKore.Helper.Log.Logar("organizationName=" + organizationName
            //                    + " :::: origin=" + origin
            //                    + " :::: entity=" + entity
            //                    + " :::: action=" + action
            //                    + " :::: message=" + message
            //                    + " :::: messageDate=" + messageDate.ToString()
            //                    + " :::: status=" + status.ToString());

            var result = Domain.Servicos.ERPIntegrationService.InserirNoIntegrationLog(organizationName, origin, entity, action, messageDate, message, status);
            string retorno = (result != null) ? result.ToString() : "Retorno nulo";

            return retorno;
        }

        [WebMethod]
        public string IntegraNFASTEC(string textoEmFormatoXML)
        {
            //Para teste
            //<?xml version="1.0" ?><nota-fiscal numero="0506526" serie="1" numeroConhecimento="" dtEmissao="2016-11-25" estabelecimento="104"><item><it-codigo><![CDATA[4580185]]></it-codigo><nr-os><![CDATA[OCOR-01285-H4D3D8]]></nr-os><guid-os><![CDATA[94454cdf-14b3-e611-80be-0050568f3ab2]]></guid-os><qt-faturada>1.00</qt-faturada><vl-preuni>167.01</vl-preuni><aliquota-ipi>5.00</aliquota-ipi><vl-ipi-it>8.35</vl-ipi-it><vl-icms-it>28.39</vl-icms-it><vl-bicms-it>167.01</vl-bicms-it><it-substituto><![CDATA[3080044]]></it-substituto><qtd-substituida>1.00</qtd-substituida></item><item><it-codigo><![CDATA[4580125]]></it-codigo><nr-os><![CDATA[OCOR-01284-S3N4D4]]></nr-os><guid-os><![CDATA[a7c28997-14b3-e611-80be-0050568f3ab2]]></guid-os><qt-faturada>1.00</qt-faturada><vl-preuni>313.25</vl-preuni><aliquota-ipi>5.00</aliquota-ipi><vl-ipi-it>15.66</vl-ipi-it><vl-icms-it>53.25</vl-icms-it><vl-bicms-it>313.25</vl-bicms-it><it-substituto><![CDATA[3640011]]></it-substituto><qtd-substituida>1.00</qtd-substituida></item></nota-fiscal>
            
            try
            {
                //SDKore.Helper.Log.Logar("integranotaasteclog.txt","Integração Nota Fiscal ASTEC\n" + textoEmFormatoXML);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(textoEmFormatoXML);

                string mensagem = "OK";
                
                var lista = new Domain.Servicos.PedidoService(nomeOrganizacao, false).EfetuaFaturamento(doc);

                foreach (var item in lista)
                {
                    mensagem += ";" + item;
                }

                ///EventLog.WriteEntry("IntegraNFASTEC","Mensagem: "+ textoEmFormatoXML + " Resultado: "+mensagem);

                return mensagem;
            }
            catch (Exception ex) 
            {
                EventLog.WriteEntry("IntegraNFASTEC",ex.Message,EventLogEntryType.Error);
                //SDKore.Helper.Log.Logar("integranotaasteclog.txt", string.Format("<strong>{0}</strong><br />{1}", ex.Message, ex.StackTrace));
                return ex.Message; 
            }
        }

        [WebMethod]
        public string IntegraRastreamentoASTEC(string codigoEstabelecimento, string serieNF, string numeroNF, string numeroRastreamento)
        {
            SDKore.Helper.Log.Logar("integrarastreamentoasteclog.txt", string.Format("CRM ASTEC Rastreamento \n\nEstabelecimento: {0} \nNF: {1}/{2} \nRastreamento: {3}",
                                              codigoEstabelecimento,
                                              numeroNF,
                                              serieNF,
                                              numeroRastreamento));

            try
            {
                new Domain.Servicos.PedidoService(nomeOrganizacao, false).AtualizarCodigoDeRastreamento(numeroNF, serieNF, codigoEstabelecimento, numeroRastreamento);
                return "OK";
            }
            catch (ArgumentException ex) { return ex.Message; }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.ServicoASTEC, "IntegraRastreamentoASTEC");
                return "Erro ao salvar dados no banco de integração: " + ex.Message + " - " + ex.StackTrace.ToString();
            }
        }

        [WebMethod]
        public string IntegraNFCanceladaASTEC(string codigoEstabelecimento, string serieNF, string numeroNF)
        {
            SDKore.Helper.Log.Logar("integranotacanceladaasteclog.txt", string.Format("CRM ASTEC Cancelamento NF \n\nEstabelecimento: {0} \nNF: {1}/{2}",
                                              codigoEstabelecimento,
                                              numeroNF,
                                              serieNF));

            try
            {
                new Domain.Servicos.PedidoService(nomeOrganizacao, false).CancelarNotaFiscal(numeroNF, serieNF, codigoEstabelecimento);
                return "OK";
            }
            catch (ArgumentException ex) { return ex.Message; }
            catch (Exception ex)
            {
                SDKore.Helper.Log.Logar("integranotacanceladaasteclog.txt", string.Format("<strong>{0}</strong><br />{1}", ex.Message, ex.StackTrace));
                return string.Format("<strong>{0}</strong><br />{1}", ex.Message, ex.StackTrace);
            }
        }

    }
}

