using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Application.WebServices.RelatoriosB2B.WSIntelbras;


/// <summary>
/// Summary description for NotaFiscal
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class NotaFiscal : System.Web.Services.WebService {

    public NotaFiscal () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet ListagemDeNotasFiscaisPorEmitente(string codigoEstabelecimento, int codigoRepresentante, int codigoEmitente, DateTime dtInicial, DateTime dtFinal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        //MONTANDO AS COLUNAS DA TABELA RELATORIO
        //tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
        //tabelaRelatorio.Columns.Add("dtemisnota", typeof(DateTime)).AllowDBNull = true;
        //tabelaRelatorio.Columns.Add("nrnotafis", typeof(string)).AllowDBNull = true;
        //tabelaRelatorio.Columns.Add("serie", typeof(string)).AllowDBNull = true;
        //tabelaRelatorio.Columns.Add("vlmerctotfat", typeof(decimal)).AllowDBNull = true;
        //tabelaRelatorio.Columns.Add("vltotnota", typeof(decimal)).AllowDBNull = true;
        tabelaRelatorio.Columns.Add("codestabel");
        tabelaRelatorio.Columns.Add("dtemisnota");
        tabelaRelatorio.Columns.Add("nrnotafis");
        tabelaRelatorio.Columns.Add("serie");
        tabelaRelatorio.Columns.Add("vlmerctotfat");
        tabelaRelatorio.Columns.Add("vltotnota");
        tabelaRelatorio.Columns.Add("nrpedido");
        

        try
        {
            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            ListagemNotasCliente_ttNotasRow[] dadosDoErp;
            
            ErpService.ListagemNotasCliente(codigoEstabelecimento, codigoRepresentante, codigoEmitente, Convert.ToDateTime(dtInicial), Convert.ToDateTime(dtFinal), out dadosDoErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            {
                tabelaRelatorio.Rows.Add(dadosDoErp[i].codestabel,
                                           Convert.ToDateTime(dadosDoErp[i].dtemisnota.Value).ToShortDateString(),
                                            dadosDoErp[i].nrnotafis,
                                            dadosDoErp[i].serie,
                                            Convert.ToDecimal(dadosDoErp[i].vlmerctotfat.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].vltotnota.Value).ToString("0.00")
                                            ,dadosDoErp[i].nrpedido
                                            );
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
    public DataSet DetalhesDaNotaFiscal(string codigoEstabelecimento, string serieNotaFiscal, string nrNotaFiscal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            #region Colunas da Tabela
            //tabelaRelatorio.Columns.Add("cgc", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codcondpag", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codnota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtcancel", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtconfirma", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtemisnota", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtsaida", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("insestadual", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("natoperacao", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nometransp", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrnotafis", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrpedcli", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrvolumes", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("observnota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("pesobrutot", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("pesoliqtot", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("serie", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlmercad", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vltotipi", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vltotnota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("cgcEmitente", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codemitenteEmitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codgrcliEmitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descricaoEmitente", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("insestadualEmitente", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeEmitente", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("telefoneEmitente", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cgc");
            tabelaRelatorio.Columns.Add("codcondpag");
            tabelaRelatorio.Columns.Add("codemitente");
            tabelaRelatorio.Columns.Add("codestabel");
            tabelaRelatorio.Columns.Add("codnota");
            tabelaRelatorio.Columns.Add("dtcancel");
            tabelaRelatorio.Columns.Add("dtconfirma");
            tabelaRelatorio.Columns.Add("dtemisnota");
            tabelaRelatorio.Columns.Add("dtsaida");
            tabelaRelatorio.Columns.Add("insestadual");
            tabelaRelatorio.Columns.Add("natoperacao");
            tabelaRelatorio.Columns.Add("nometransp");
            tabelaRelatorio.Columns.Add("nrnotafis");
            tabelaRelatorio.Columns.Add("nrpedcli");
            tabelaRelatorio.Columns.Add("nrvolumes");
            tabelaRelatorio.Columns.Add("observnota");
            tabelaRelatorio.Columns.Add("pesobrutot");
            tabelaRelatorio.Columns.Add("pesoliqtot");
            tabelaRelatorio.Columns.Add("serie");
            tabelaRelatorio.Columns.Add("vlmercad");
            tabelaRelatorio.Columns.Add("vltotipi");
            tabelaRelatorio.Columns.Add("vltotnota");
            tabelaRelatorio.Columns.Add("codrep");
            tabelaRelatorio.Columns.Add("cgcEmitente");
            tabelaRelatorio.Columns.Add("codemitenteEmitente");
            tabelaRelatorio.Columns.Add("codgrcliEmitente");
            tabelaRelatorio.Columns.Add("descricaoEmitente");
            tabelaRelatorio.Columns.Add("insestadualEmitente");
            tabelaRelatorio.Columns.Add("nomeEmitente");
            tabelaRelatorio.Columns.Add("telefoneEmitente");
            #endregion

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            DetalheNotaFiscal_ttNotaFiscalRow[] NotaFiscal;
            DetalheNotaFiscal_ttEmitenteRow[] EmitenteNotaFiscal;
            DetalheNotaFiscal_ttFatDuplicRow[] FaturasNotaFiscal;
            DetalheNotaFiscal_ttItNotaFiscRow[] ItensNotaFiscal;
            DetalheNotaFiscal_ttTotaisRow[] TotaisNotaFiscal;
            DetalheNotaFiscal_ttTransporteRow[] TransportadoraNotaFiscal;

            ErpService.DetalheNotaFiscal(codigoEstabelecimento, serieNotaFiscal, nrNotaFiscal, out NotaFiscal, out EmitenteNotaFiscal, out TransportadoraNotaFiscal, out ItensNotaFiscal, out FaturasNotaFiscal, out TotaisNotaFiscal);

            //LOOP PARA POPULAR OS DADOS NA TABELA

            for (int i = 0; i < NotaFiscal.Length; i++)
            {
                for (int y = 0; y < EmitenteNotaFiscal.Length; y++)
                {
                    if (NotaFiscal[i].codemitente.Value == EmitenteNotaFiscal[y].codemitente.Value)
                    {
                        #region Adcionando Valores na Tabela
                        tabelaRelatorio.Rows.Add(NotaFiscal[i].cgc,
                                                    NotaFiscal[i].codcondpag.GetValueOrDefault(0),
                                                    NotaFiscal[i].codemitente.GetValueOrDefault(0),
                                                    NotaFiscal[i].codestabel,
                                                    NotaFiscal[i].codnota,
                                                    Convert.ToDateTime(NotaFiscal[i].dtcancel.GetValueOrDefault()).ToShortDateString(),
                                                    Convert.ToDateTime(NotaFiscal[i].dtconfirma.GetValueOrDefault()).ToShortDateString(),
                                                    Convert.ToDateTime(NotaFiscal[i].dtemisnota.GetValueOrDefault()).ToShortDateString(),
                                                    Convert.ToDateTime(NotaFiscal[i].dtsaida.GetValueOrDefault()).ToShortDateString(),
                                                    NotaFiscal[i].insestadual,
                                                    NotaFiscal[i].natoperacao,
                                                    NotaFiscal[i].nometransp,
                                                    NotaFiscal[i].nrnotafis,
                                                    NotaFiscal[i].nrpedcli,
                                                    NotaFiscal[i].nrvolumes,
                                                    NotaFiscal[i].observnota,
                                                    Convert.ToDecimal(NotaFiscal[i].pesobrutot.GetValueOrDefault(0)).ToString("0.00"),
                                                    Convert.ToDecimal(NotaFiscal[i].pesoliqtot.GetValueOrDefault(0)).ToString("0.00"),
                                                    NotaFiscal[i].serie,
                                                    Convert.ToDecimal(NotaFiscal[i].vlmercad.GetValueOrDefault(0)).ToString("0.00"),
                                                    Convert.ToDecimal(NotaFiscal[i].vltotipi.GetValueOrDefault(0)).ToString("0.00"),
                                                    Convert.ToDecimal(NotaFiscal[i].vltotnota.GetValueOrDefault(0)).ToString("0.00"),//22
                                                    NotaFiscal[i].codrep.GetValueOrDefault(0),
                                                    EmitenteNotaFiscal[y].cgc,
                                                    EmitenteNotaFiscal[y].codemitente.GetValueOrDefault(0),
                                                    EmitenteNotaFiscal[y].codgrcli.GetValueOrDefault(0),
                                                    EmitenteNotaFiscal[y].descricao,
                                                    EmitenteNotaFiscal[y].insestadual,
                                                    EmitenteNotaFiscal[y].nomeemit,
                                                    EmitenteNotaFiscal[y].telefone //7
                                                    );
                        #endregion
                    }
                }
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
    public DataSet ItensDaNotaFiscal(string codigoEstabelecimento, string serieNotaFiscal, string nrNotaFiscal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("aliquotaicmNota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("aliquotaipiNota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("classfiscalNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabelNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descitemNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("itcodigoNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrnotafisNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrseqfatNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtfaturadaNota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("serieNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlipiitNota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlmercliqNota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlpreuniNota", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vltotalNota", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("aliquotaicmNota");
            tabelaRelatorio.Columns.Add("aliquotaipiNota");
            tabelaRelatorio.Columns.Add("classfiscalNota");
            tabelaRelatorio.Columns.Add("codestabelNota");
            tabelaRelatorio.Columns.Add("descitemNota");
            tabelaRelatorio.Columns.Add("itcodigoNota");
            tabelaRelatorio.Columns.Add("nrnotafisNota");
            tabelaRelatorio.Columns.Add("nrseqfatNota");
            tabelaRelatorio.Columns.Add("qtfaturadaNota");
            tabelaRelatorio.Columns.Add("serieNota");
            tabelaRelatorio.Columns.Add("vlipiitNota");
            tabelaRelatorio.Columns.Add("vlmercliqNota");
            tabelaRelatorio.Columns.Add("vlpreuniNota");
            tabelaRelatorio.Columns.Add("vltotalNota");


            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            DetalheNotaFiscal_ttEmitenteRow[] EmitenteNotaFiscal;
            DetalheNotaFiscal_ttFatDuplicRow[] FaturasNotaFiscal;
            DetalheNotaFiscal_ttItNotaFiscRow[] ItensNotaFiscal;
            DetalheNotaFiscal_ttNotaFiscalRow[] NotaFiscal;
            DetalheNotaFiscal_ttTotaisRow[] TotaisNotaFiscal;
            DetalheNotaFiscal_ttTransporteRow[] TransportadoraNotaFiscal;

            ErpService.DetalheNotaFiscal(codigoEstabelecimento, serieNotaFiscal, nrNotaFiscal, out NotaFiscal, out EmitenteNotaFiscal, out TransportadoraNotaFiscal, out ItensNotaFiscal, out FaturasNotaFiscal, out TotaisNotaFiscal);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < ItensNotaFiscal.Length; i++)
            {
                tabelaRelatorio.Rows.Add(Convert.ToDecimal(ItensNotaFiscal[i].aliquotaicm.Value).ToString("0.00"),
                                            Convert.ToDecimal(ItensNotaFiscal[i].aliquotaipi.Value).ToString("0.00"),
                                            ItensNotaFiscal[i].classfiscal,
                                            ItensNotaFiscal[i].codestabel,
                                            ItensNotaFiscal[i].descitem,
                                            ItensNotaFiscal[i].itcodigo,
                                            ItensNotaFiscal[i].nrnotafis,
                                            ItensNotaFiscal[i].nrseqfat,
                                            Convert.ToDecimal(ItensNotaFiscal[i].qtfaturada.Value).ToString("0.00"),
                                            ItensNotaFiscal[i].serie,
                                            Convert.ToDecimal(ItensNotaFiscal[i].vlipiit.Value).ToString("0.00"),
                                            Convert.ToDecimal(ItensNotaFiscal[i].vlmercliq.Value).ToString("0.00"),
                                            Convert.ToDecimal(ItensNotaFiscal[i].vlpreuni.Value).ToString("0.00"),
                                            Convert.ToDecimal(ItensNotaFiscal[i].vltotal.Value).ToString("0.00")
                                            );
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
    public DataSet FaturasDaNotaFiscal(string codigoEstabelecimento, string serieNotaFiscal, string nrNotaFiscal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtvenciment", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("flagatualiz", typeof(bool)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("indfatnota", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrfatura", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("parcela", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("serie", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("valorparcela", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codestabel");
            tabelaRelatorio.Columns.Add("dtvenciment");
            tabelaRelatorio.Columns.Add("flagatualiz");
            tabelaRelatorio.Columns.Add("indfatnota");
            tabelaRelatorio.Columns.Add("nrfatura");
            tabelaRelatorio.Columns.Add("parcela");
            tabelaRelatorio.Columns.Add("serie");
            tabelaRelatorio.Columns.Add("valorparcela");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            DetalheNotaFiscal_ttEmitenteRow[] EmitenteNotaFiscal;
            DetalheNotaFiscal_ttFatDuplicRow[] FaturasNotaFiscal;
            DetalheNotaFiscal_ttItNotaFiscRow[] ItensNotaFiscal;
            DetalheNotaFiscal_ttNotaFiscalRow[] NotaFiscal;
            DetalheNotaFiscal_ttTotaisRow[] TotaisNotaFiscal;
            DetalheNotaFiscal_ttTransporteRow[] TransportadoraNotaFiscal;

            ErpService.DetalheNotaFiscal(codigoEstabelecimento, serieNotaFiscal, nrNotaFiscal, out NotaFiscal, out EmitenteNotaFiscal,out TransportadoraNotaFiscal, out ItensNotaFiscal, out FaturasNotaFiscal, out TotaisNotaFiscal);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < FaturasNotaFiscal.Length; i++)
            {
               tabelaRelatorio.Rows.Add(FaturasNotaFiscal[i].codestabel,
                                            Convert.ToDateTime(FaturasNotaFiscal[i].dtvenciment.Value).ToShortDateString(),
                                            FaturasNotaFiscal[i].flagatualiz,
                                            FaturasNotaFiscal[i].indfatnota,
                                            FaturasNotaFiscal[i].nrfatura,
                                            FaturasNotaFiscal[i].parcela,
                                            FaturasNotaFiscal[i].serie,
                                            Convert.ToDecimal(FaturasNotaFiscal[i].valorparcela.Value).ToString("0.00"));
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
    public DataSet TotaisDaNotaFiscal(string codigoEstabelecimento, string serieNotaFiscal, string nrNotaFiscal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("baseicmsTotais", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("baseicmssubTotais", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabelTotais", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrnotafisTotais", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("serieTotais", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlicmsTotais", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlicmssubTotais", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("baseicmsTotais");
            tabelaRelatorio.Columns.Add("baseicmssubTotais");
            tabelaRelatorio.Columns.Add("codestabelTotais");
            tabelaRelatorio.Columns.Add("nrnotafisTotais");
            tabelaRelatorio.Columns.Add("serieTotais");
            tabelaRelatorio.Columns.Add("vlicmsTotais");
            tabelaRelatorio.Columns.Add("vlicmssubTotais");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            DetalheNotaFiscal_ttEmitenteRow[] EmitenteNotaFiscal;
            DetalheNotaFiscal_ttFatDuplicRow[] FaturasNotaFiscal;
            DetalheNotaFiscal_ttItNotaFiscRow[] ItensNotaFiscal;
            DetalheNotaFiscal_ttNotaFiscalRow[] NotaFiscal;
            DetalheNotaFiscal_ttTotaisRow[] TotaisNotaFiscal;
            DetalheNotaFiscal_ttTransporteRow[] TransportadoraNotaFiscal;

            ErpService.DetalheNotaFiscal(codigoEstabelecimento, serieNotaFiscal, nrNotaFiscal, out NotaFiscal, out EmitenteNotaFiscal, out TransportadoraNotaFiscal, out ItensNotaFiscal, out FaturasNotaFiscal, out TotaisNotaFiscal);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < TotaisNotaFiscal.Length; i++)
            {

                tabelaRelatorio.Rows.Add(Convert.ToDecimal(TotaisNotaFiscal[i].baseicms.Value).ToString("0.00"),
                                            Convert.ToDecimal(TotaisNotaFiscal[i].baseicmssub.Value).ToString("0.00"),
                                            TotaisNotaFiscal[i].codestabel,
                                            TotaisNotaFiscal[i].nrnotafis,
                                            TotaisNotaFiscal[i].serie,
                                            Convert.ToDecimal(TotaisNotaFiscal[i].vlicms.Value).ToString("0.00"),
                                            Convert.ToDecimal(TotaisNotaFiscal[i].vlicmssub.Value).ToString("0.00")
                                            );
                
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
    public DataSet TransportadoraDaNotaFiscal(string codigoEstabelecimento, string serieNotaFiscal, string nrNotaFiscal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("bairroTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("cepTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("cgcTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("cidadeTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codtranspTransporte", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("enderecoTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("estadoTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("insestadualTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrevTransporte", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("paisTransporte", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("bairroTransporte");
            tabelaRelatorio.Columns.Add("cepTransporte");
            tabelaRelatorio.Columns.Add("cgcTransporte");
            tabelaRelatorio.Columns.Add("cidadeTransporte");
            tabelaRelatorio.Columns.Add("codtranspTransporte");
            tabelaRelatorio.Columns.Add("enderecoTransporte");
            tabelaRelatorio.Columns.Add("estadoTransporte");
            tabelaRelatorio.Columns.Add("insestadualTransporte");
            tabelaRelatorio.Columns.Add("nomeTransporte");
            tabelaRelatorio.Columns.Add("nomeabrevTransporte");
            tabelaRelatorio.Columns.Add("paisTransporte");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            DetalheNotaFiscal_ttEmitenteRow[] EmitenteNotaFiscal;
            DetalheNotaFiscal_ttFatDuplicRow[] FaturasNotaFiscal;
            DetalheNotaFiscal_ttItNotaFiscRow[] ItensNotaFiscal;
            DetalheNotaFiscal_ttNotaFiscalRow[] NotaFiscal;
            DetalheNotaFiscal_ttTotaisRow[] TotaisNotaFiscal;
            DetalheNotaFiscal_ttTransporteRow[] TransportadoraNotaFiscal;
            
            ErpService.DetalheNotaFiscal(codigoEstabelecimento, serieNotaFiscal, nrNotaFiscal, out NotaFiscal, out EmitenteNotaFiscal, out TransportadoraNotaFiscal, out ItensNotaFiscal, out FaturasNotaFiscal, out TotaisNotaFiscal);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < TransportadoraNotaFiscal.Length; i++)
            {
                tabelaRelatorio.Rows.Add(TransportadoraNotaFiscal[i].bairro,
                                            TransportadoraNotaFiscal[i].cep,
                                            TransportadoraNotaFiscal[i].cgc,
                                            TransportadoraNotaFiscal[i].cidade,
                                            TransportadoraNotaFiscal[i].codtransp,
                                            TransportadoraNotaFiscal[i].endereco,
                                            TransportadoraNotaFiscal[i].estado,
                                            TransportadoraNotaFiscal[i].insestadual,
                                            TransportadoraNotaFiscal[i].nome,
                                            TransportadoraNotaFiscal[i].nomeabrev,
                                            TransportadoraNotaFiscal[i].pais
                                            );
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
