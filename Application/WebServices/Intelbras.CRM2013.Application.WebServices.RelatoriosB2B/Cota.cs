using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Application.WebServices.RelatoriosB2B.WSIntelbras;


/// <summary>
/// Summary description for Cota
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Cota : System.Web.Services.WebService
{

    public Cota()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet ListagemCotasDoRepresentante(string codigoEstabelecimento, string codigoRepresentante, DateTime dtInicial, bool Diretoria, bool Gerente, bool Representante)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO      
            //tabelaRelatorio.Columns.Add("coddiretoria", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codgerente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codrep", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descfmcodcom", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descitem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("fmcodcom", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codigoitem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrev", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomediretoria", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomegerente", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("percrealizado", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtcarteira", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtcota", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtdevolvido", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtfatmenosdev", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtfaturado", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtfaturar", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtsaldovender", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("coddiretoria");
            tabelaRelatorio.Columns.Add("codestabel");
            tabelaRelatorio.Columns.Add("codgerente");
            tabelaRelatorio.Columns.Add("codrep");
            tabelaRelatorio.Columns.Add("descfmcodcom");
            tabelaRelatorio.Columns.Add("descitem");
            tabelaRelatorio.Columns.Add("fmcodcom");
            tabelaRelatorio.Columns.Add("codigoitem");
            tabelaRelatorio.Columns.Add("nomeabrev");
            tabelaRelatorio.Columns.Add("nomediretoria");
            tabelaRelatorio.Columns.Add("nomegerente");
            tabelaRelatorio.Columns.Add("percrealizado");
            tabelaRelatorio.Columns.Add("qtcarteira");
            tabelaRelatorio.Columns.Add("qtcota");
            tabelaRelatorio.Columns.Add("qtdevolvido");
            tabelaRelatorio.Columns.Add("qtfatmenosdev");
            tabelaRelatorio.Columns.Add("qtfaturado");
            tabelaRelatorio.Columns.Add("qtfaturar");
            tabelaRelatorio.Columns.Add("qtsaldovender");
            tabelaRelatorio.Columns.Add("descunidnegoc");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            ListagemCotasRepres_ttCotaFinalRow[] dadosDoErp;
            ListagemCotasRepres_ttErroRow[] ErroErp;

            ErpService.ListagemCotasRepres(codigoEstabelecimento, codigoRepresentante, dtInicial, Diretoria, Gerente, Representante, out dadosDoErp, out ErroErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            { 
                tabelaRelatorio.Rows.Add(dadosDoErp[i].coddiretoria,
                                            dadosDoErp[i].codestabel,
                                            dadosDoErp[i].codgerente.Value,
                                            dadosDoErp[i].codrep.Value,
                                            dadosDoErp[i].descfmcodcom,
                                            dadosDoErp[i].descitem,
                                            dadosDoErp[i].fmcodcom,
                                            dadosDoErp[i].itcodigo,
                                            dadosDoErp[i].nomeabrev,
                                            dadosDoErp[i].nomediretoria,
                                            dadosDoErp[i].nomegerente,
                                            Convert.ToDecimal(dadosDoErp[i].percrealizado.Value).ToString("0.00"),
                                            dadosDoErp[i].qtcarteira,
                                            dadosDoErp[i].qtcota.Value,
                                            dadosDoErp[i].qtdevolvido,
                                            Convert.ToDecimal(dadosDoErp[i].qtfatmenosdev.Value).ToString("0.00"),
                                            dadosDoErp[i].qtfaturado,
                                            Convert.ToDecimal(dadosDoErp[i].qtfaturar.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].qtsaldovender.Value).ToString("0.00"),
                                            dadosDoErp[i].descunidnegoc
                                            );
            }
        }
        catch (System.Web.Services.Protocols.SoapException ex)
        {
            System.Diagnostics.EventLog.WriteEntry("WebServices Report", ex.Detail.InnerText.ToString(), System.Diagnostics.EventLogEntryType.Error, 666);
            throw new SoapException("An error occurred in the webservices when retrieving the data, see the event viewer for more details.", SoapException.ClientFaultCode);
        }
        
        //ATRIBUINDO DATATABLE AO DATASET
        dataSetPrincipal.Tables.Add(tabelaRelatorio);

        //RETORNO DO DATASET
        return dataSetPrincipal;
    }

}
