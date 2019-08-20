using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Application.WebServices.RelatoriosB2B.WSIntelbras;

/// <summary>
/// Summary description for Titulo
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Titulo : System.Web.Services.WebService {

    public Titulo () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet ListagemTitulosCliente(int codigoCliente, string statusTitulo,string codigoEstabelecimento, DateTime dtInicial, DateTime dtFinal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            #region Colunas da Tabela
            //tabelaRelatorio.Columns.Add("codparcela", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codtitacr", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codtitacrbco", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("datvecto", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("desespecdocto", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("diasatraso", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeportador", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrnotafis", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("sernotafis", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("titstatus", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("valcartorio", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("valsaldo", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codparcela");
            tabelaRelatorio.Columns.Add("codtitacr");
            tabelaRelatorio.Columns.Add("codtitacrbco");
            tabelaRelatorio.Columns.Add("datvecto");
            tabelaRelatorio.Columns.Add("desespecdocto");
            tabelaRelatorio.Columns.Add("diasatraso");
            tabelaRelatorio.Columns.Add("nomeportador");
            tabelaRelatorio.Columns.Add("nrnotafis");
            tabelaRelatorio.Columns.Add("sernotafis");
            tabelaRelatorio.Columns.Add("titstatus");
            tabelaRelatorio.Columns.Add("valcartorio");
            tabelaRelatorio.Columns.Add("valsaldo");
            #endregion

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            ListagemTitulosCliente_ttTitAcrRow[] listaDeTitulos;
            ErpService.ListagemTitulosCliente(codigoCliente, codigoEstabelecimento,statusTitulo, dtInicial, dtFinal, out listaDeTitulos);
            
            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < listaDeTitulos.Length; i++)
            {
                #region Adcionando Valores na Tabela
                tabelaRelatorio.Rows.Add(listaDeTitulos[i].codparcela,
                                        listaDeTitulos[i].codtitacr,
                                        listaDeTitulos[i].codtitacrbco,
                                        listaDeTitulos[i].datvecto,
                                        listaDeTitulos[i].desespecdocto,
                                        listaDeTitulos[i].diasatraso,
                                        listaDeTitulos[i].nomeportador,
                                        listaDeTitulos[i].nrnotafis,
                                        listaDeTitulos[i].sernotafis,
                                        listaDeTitulos[i].titstatus,
                                        Convert.ToDecimal(listaDeTitulos[i].valcartorio.Value).ToString("0.00"),
                                        Convert.ToDecimal(listaDeTitulos[i].valsaldo.Value).ToString("0.00")
                                        );
                #endregion
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

   [WebMethod]
    public DataSet ListagemTitulosClienteDoRepresentante(int codigoRepresentante, int codigoCliente,string codigoEstabelecimento, string statusTitulo, DateTime dtInicial, DateTime dtFinal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            #region Colunas da Tabela
            //tabelaRelatorio.Columns.Add("cidade", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtlimcred", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("estado", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("limcredito", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrevrepres", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeemit", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("totalgeral", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("totavencer", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("totvencido", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cidade");
            tabelaRelatorio.Columns.Add("codemitente");
            tabelaRelatorio.Columns.Add("dtlimcred");
            tabelaRelatorio.Columns.Add("estado");
            tabelaRelatorio.Columns.Add("limcredito");
            tabelaRelatorio.Columns.Add("nomeabrevrepres");
            tabelaRelatorio.Columns.Add("nomeemit");
            tabelaRelatorio.Columns.Add("totalgeral");
            tabelaRelatorio.Columns.Add("totavencer");
            tabelaRelatorio.Columns.Add("totvencido");
            #endregion

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            ListagemTitulosClientesRepres_ttDevolucaoRow[] listaDeTitulos;
            ErpService.ListagemTitulosClientesRepres(codigoRepresentante,codigoCliente,codigoEstabelecimento, statusTitulo, dtInicial, dtFinal, out listaDeTitulos);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < listaDeTitulos.Length; i++)
            {
                #region Adcionando Valores na Tabela
                tabelaRelatorio.Rows.Add(listaDeTitulos[i].cidade,
                                        listaDeTitulos[i].codemitente,
                                        listaDeTitulos[i].dtlimcred,
                                        listaDeTitulos[i].estado,
                                        listaDeTitulos[i].limcredito,
                                        listaDeTitulos[i].nomeabrevrepres,
                                        listaDeTitulos[i].nomeemit,
                                        Convert.ToDecimal(listaDeTitulos[i].totalgeral.Value).ToString("0.00"),
                                        Convert.ToDecimal(listaDeTitulos[i].totavencer.Value).ToString("0.00"),
                                        Convert.ToDecimal(listaDeTitulos[i].totvencido.Value).ToString("0.00")
                                        );
                #endregion
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
