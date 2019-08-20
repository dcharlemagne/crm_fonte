using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Web.Services.Protocols;

/// <summary>
/// Summary description for Cliente
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Cliente : System.Web.Services.WebService
{

    public Cliente()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet DetalheCliente(int codigoEmitente, string codigoEstabelecimento)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            #region Colunas da Tabela
            tabelaRelatorio.Columns.Add("bairro", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("bairrocob", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("bonificacao", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("categoria", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cep", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cepcob", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cgc", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cidade", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cidadecob", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codsuframa", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("consumidorfinal", typeof(bool)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("contratovendor", typeof(bool)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("descpontual", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("dsgrcli", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("dtlimcred", typeof(DateTime)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("endereco", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("enderecocob", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("estado", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("estadocob", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("indcredcli", typeof(int)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("indice", typeof(int)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("insestadual", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("limcredito", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("nomeemit", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("observacoes", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("telefone", typeof(string)).AllowDBNull = true;

            tabelaRelatorio.Columns.Add("codemitEmitente", typeof(int)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("emailEmitente", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("nomeEmitente", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("ramalEmitente", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("telefoneEmitente", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("valLimSuperCard", typeof(decimal)).AllowDBNull= true;
            tabelaRelatorio.Columns.Add("valLimIntel", typeof(decimal)).AllowDBNull= true;


            #endregion

            //CONSUMINDO WEBSERVICES DO ERP
            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.DetalheCliente_ttContEmitRow[] ContatosEmitente;
            PortalB2BEMS.WSIntelbras.DetalheCliente_ttEmitenteRow[] Emitente;
            ErpService.DetalheCliente(codigoEmitente, codigoEstabelecimento, out Emitente, out ContatosEmitente);

            DataRow newCustomersRow = tabelaRelatorio.NewRow();

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < Emitente.Length; i++)
            {

                newCustomersRow["bairro"] = Emitente[i].bairro;
                newCustomersRow["bairrocob"] = Emitente[i].bairrocob;
                newCustomersRow["bonificacao"] = Emitente[i].bonificacao;
                newCustomersRow["categoria"] = Emitente[i].categoria;
                newCustomersRow["cep"] = Emitente[i].cep;
                newCustomersRow["cepcob"] = Emitente[i].cepcob;
                newCustomersRow["cgc"] = Emitente[i].cgc;
                newCustomersRow["cidade"] = Emitente[i].cidade;
                newCustomersRow["cidadecob"] = Emitente[i].cidadecob;
                newCustomersRow["codemitente"] = Emitente[i].codemitente;
                newCustomersRow["codestabel"] = Emitente[i].codestabel;
                newCustomersRow["codsuframa"] = Emitente[i].codsuframa;
                newCustomersRow["consumidorfinal"] = Emitente[i].consumidorfinal;
                newCustomersRow["contratovendor"] = Emitente[i].contratovendor;
                newCustomersRow["dsgrcli"] = Emitente[i].dsgrcli;
                newCustomersRow["dtlimcred"] = Emitente[i].dtlimcred;
                newCustomersRow["endereco"] = Emitente[i].endereco;
                newCustomersRow["enderecocob"] = Emitente[i].enderecocob;
                newCustomersRow["estado"] = Emitente[i].estado;
                newCustomersRow["estadocob"] = Emitente[i].estadocob;
                newCustomersRow["indcredcli"] = Emitente[i].indcredcli;
                newCustomersRow["indice"] = Emitente[i].indice;
                newCustomersRow["insestadual"] = Emitente[i].insestadual;
                newCustomersRow["limcredito"] = Emitente[i].limcredito;
                newCustomersRow["nomeemit"] = Emitente[i].nomeemit;
                newCustomersRow["observacoes"] = Emitente[i].observacoes;
                newCustomersRow["telefone"] = Emitente[i].telefone;
                newCustomersRow["valLimSuperCard"] = Emitente[i].vallimitesupcard;
                newCustomersRow["valLimIntel"] = Emitente[i].vallimiteintelbras;
                if (ContatosEmitente.Length == 0)
                {
                    tabelaRelatorio.Rows.Add(newCustomersRow);
                }
                else
                {
                    for (int y = 0; y < ContatosEmitente.Length; y++)
                    {
                        if (Emitente[i].codemitente.Value == ContatosEmitente[y].codemit.Value)
                        {
                            newCustomersRow["codemitEmitente"] = ContatosEmitente[i].codemit;
                            newCustomersRow["emailEmitente"] = ContatosEmitente[i].email;
                            newCustomersRow["nomeEmitente"] = ContatosEmitente[i].nome;
                            newCustomersRow["ramalEmitente"] = ContatosEmitente[i].ramal;
                            newCustomersRow["telefoneEmitente"] = ContatosEmitente[i].telefone;
                        }
                    }
                    tabelaRelatorio.Rows.Add(newCustomersRow);
                }
            }
        }

        catch (SoapException ex)
        {
            System.Diagnostics.EventLog.WriteEntry("WebServices Report", ex.Detail.InnerText.ToString(), System.Diagnostics.EventLogEntryType.Error, 666);
            throw new SoapException(" An error occurred in the webservices when retrieving the data, see the event viewer for more details.", SoapException.ClientFaultCode);
        }

        //ATRIBUINDO DATATABLE AO DATASET
        dataSetPrincipal.Tables.Add(tabelaRelatorio);

        //RETORNO DO DATASET
        return dataSetPrincipal;
    }

}
