using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Application.WebServices.RelatoriosB2B.WSIntelbras;

/// <summary>
/// Summary description for Devolucao
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Devolucao : System.Web.Services.WebService {

    public Devolucao () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet DevolucaoCliente( string codigoEstabelecimento, int codigoRepresentante, string codigoUnidadeNegocio, DateTime dtInicial, DateTime dtFinal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codrep", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descitem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtdevol", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("itcodigo", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrev", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeemit", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrnotafis", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("precototal", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtdevolvida", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("sequencia", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("serie", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codemitente");
            tabelaRelatorio.Columns.Add("codrep");
            tabelaRelatorio.Columns.Add("descitem");
            tabelaRelatorio.Columns.Add("dtdevol");
            tabelaRelatorio.Columns.Add("itcodigo");
            tabelaRelatorio.Columns.Add("nomeabrev");
            tabelaRelatorio.Columns.Add("nomeemit");
            tabelaRelatorio.Columns.Add("nrnotafis");
            tabelaRelatorio.Columns.Add("precototal");
            tabelaRelatorio.Columns.Add("qtdevolvida");
            tabelaRelatorio.Columns.Add("sequencia");
            tabelaRelatorio.Columns.Add("serie");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            ListagemDevolClientes_ttDevolucaoRow[] dadosDoErp;
            ErpService.ListagemDevolClientes(codigoEstabelecimento,codigoRepresentante, codigoUnidadeNegocio, dtInicial, dtFinal, out dadosDoErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            {
                tabelaRelatorio.Rows.Add(dadosDoErp[i].codemitente,
                                            dadosDoErp[i].codrep,
                                            dadosDoErp[i].descitem,
                                            dadosDoErp[i].dtdevol,
                                            dadosDoErp[i].itcodigo,
                                            dadosDoErp[i].nomeabrev,
                                            dadosDoErp[i].nomeemit,
                                            dadosDoErp[i].nrnotafis,
                                            Convert.ToDecimal(dadosDoErp[i].precototal.Value).ToString("0.00"),
                                            dadosDoErp[i].qtdevolvida,
                                            dadosDoErp[i].sequencia,
                                            dadosDoErp[i].serie);

            }
        }

        catch (SoapException ex)
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
