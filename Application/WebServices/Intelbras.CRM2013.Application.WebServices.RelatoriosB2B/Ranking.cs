using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Application.WebServices.RelatoriosB2B.WSIntelbras;


/// <summary>
/// Summary description for Ranking
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Ranking : System.Web.Services.WebService {

    public Ranking () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet RankingEstabelecimento(string codigoEstabelecimento, string periodo)
    {
        string AnoPeriodo = periodo.Substring(3, 4);
        string MesPeriodo = periodo.Substring(0, 2);

        periodo = AnoPeriodo + MesPeriodo;

        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("cod_unid_negoc", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codrep", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("des_unid_negoc", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrev", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("perc", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlcota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vldevolvido", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlfaturado", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cod_unid_negoc");
            tabelaRelatorio.Columns.Add("codestabel");
            tabelaRelatorio.Columns.Add("codrep");
            tabelaRelatorio.Columns.Add("des_unid_negoc");
            tabelaRelatorio.Columns.Add("nomeabrev");
            tabelaRelatorio.Columns.Add("perc");
            tabelaRelatorio.Columns.Add("vlcota");
            tabelaRelatorio.Columns.Add("vldevolvido");
            tabelaRelatorio.Columns.Add("vlfaturado");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            RankingEstabelecimento_ttRankingRow[] Ranking;
            //PortalB2BEMS.WSIntelbras.RankingEstabelecimento_ttUnidNegRow[] UnidadeNegocio;
            ErpService.RankingEstabelecimento(codigoEstabelecimento, periodo, out Ranking);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int y = 0; y < Ranking.Length; y++)
            {
                tabelaRelatorio.Rows.Add(Ranking[y].cod_unid_negoc,
                                    Ranking[y].codestabel,
                                    Ranking[y].codrep,
                                    Ranking[y].des_unid_negoc,
                                    Ranking[y].nomeabrev,
                                    Convert.ToDecimal(Ranking[y].perc.Value).ToString("0.00"),
                                    Convert.ToDecimal(Ranking[y].vlcota.Value).ToString("0.00"),
                                    Convert.ToDecimal(Ranking[y].vldevolvido.Value).ToString("0.00"),
                                    Convert.ToDecimal(Ranking[y].vlfaturado.Value).ToString("0.00"));

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
