using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Diagnostics;
using Intelbras.CRM2013.Application.WebServices.RelatoriosB2B.WSIntelbras;

/// <summary>
/// Summary description for Faturamento
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Faturamento : System.Web.Services.WebService {

    public Faturamento () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet FaturamentoConsolidado()
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        //try
        //{
        //    //MONTANDO AS COLUNAS DA TABELA RELATORIO
        //    //tabelaRelatorio.Columns.Add("cod_unid_negoc", typeof(string)).AllowDBNull = true;
        //    //tabelaRelatorio.Columns.Add("des_unid_negoc", typeof(string)).AllowDBNull = true;
        //    //tabelaRelatorio.Columns.Add("periodo_unid", typeof(string)).AllowDBNull = true;
        //    //tabelaRelatorio.Columns.Add("vlcart", typeof(decimal)).AllowDBNull = true;
        //    //tabelaRelatorio.Columns.Add("vlfatorc", typeof(decimal)).AllowDBNull = true;
        //    //tabelaRelatorio.Columns.Add("vlfatreal", typeof(decimal)).AllowDBNull = true;
        //    //tabelaRelatorio.Columns.Add("vlpendente", typeof(decimal)).AllowDBNull = true;
        //    tabelaRelatorio.Columns.Add("cod_unid_negoc");
        //    tabelaRelatorio.Columns.Add("des_unid_negoc");
        //    tabelaRelatorio.Columns.Add("periodo_unid");
        //    tabelaRelatorio.Columns.Add("vlcart");
        //    tabelaRelatorio.Columns.Add("vlfatorc");
        //    tabelaRelatorio.Columns.Add("vlfatreal");
        //    tabelaRelatorio.Columns.Add("vlpendente");
            
        //    //CONSUMINDO WEBSERVICES DO ERP
        //    PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
        //    ErpService.Timeout = 99999999;
        //    PortalB2BEMS.WSIntelbras.FaturamentoConsolidado_ttPeriodoRow[] FaturamentoPeriodo;
        //    PortalB2BEMS.WSIntelbras.FaturamentoConsolidado_ttUnidadeRow[] FaturamentoUnidade;
        //    ErpService.FaturamentoConsolidado(out FaturamentoPeriodo, out FaturamentoUnidade);

        //    //LOOP PARA POPULAR OS DADOS NA TABELA
        //    for (int i = 0; i < FaturamentoUnidade.Length; i++)
        //    {
        //        tabelaRelatorio.Rows.Add(FaturamentoUnidade[i].cod_unid_negoc,
        //                                    FaturamentoUnidade[i].des_unid_negoc,
        //                                    FaturamentoUnidade[i].periodo,
        //                                    Convert.ToDecimal(FaturamentoUnidade[i].vlcart.Value).ToString("0.00"),
        //                                    Convert.ToDecimal(FaturamentoUnidade[i].vlfatorc.Value).ToString("0.00"),
        //                                    Convert.ToDecimal(FaturamentoUnidade[i].vlfatreal.Value).ToString("0.00"),
        //                                    Convert.ToDecimal(FaturamentoUnidade[i].vlpendente.Value).ToString("0.00"));
        //    }
        //}

        //catch (SoapException ex)
        //{
        //    System.Diagnostics.EventLog.WriteEntry("WebServices Report", ex.Detail.InnerText.ToString(), System.Diagnostics.EventLogEntryType.Error, 666);
        //    throw new SoapException("An error occurred in the webservices when retrieving the data, see the event viewer for more details.", SoapException.ClientFaultCode);
        //}

        ////ATRIBUINDO DATATABLE AO DATASET
        //dataSetPrincipal.Tables.Add(tabelaRelatorio);

        ////RETORNO DO DATASET
        return dataSetPrincipal = null;
    }

    [WebMethod]
    public DataSet FaturamentoPorEstabelecimento(string codigoEstabelecimento, string codigoRepresentante, string codigoUnidadeNegocio, DateTime dtInicial, DateTime dtFinal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("cod_unid_negoc_faturamento", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabel_faturamento", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codgerente_faturamento", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codrep_faturamento", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("des_unid_negoc", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrev_faturamento", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomegerente", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("pccota_faturamento", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlcarteira_faturamento", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlcota_faturamento", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vldevolvido_faturamento", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlfaturado_faturamento", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlorcamento_faturamento", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cod_unid_negoc_faturamento");
            tabelaRelatorio.Columns.Add("codestabel_faturamento");
            tabelaRelatorio.Columns.Add("codgerente_faturamento");
            tabelaRelatorio.Columns.Add("codrep_faturamento");
            tabelaRelatorio.Columns.Add("des_unid_negoc");
            tabelaRelatorio.Columns.Add("nomeabrev_faturamento");
            tabelaRelatorio.Columns.Add("nomegerente");
            tabelaRelatorio.Columns.Add("pccota_faturamento");
            tabelaRelatorio.Columns.Add("vlcarteira_faturamento");
            tabelaRelatorio.Columns.Add("vlcota_faturamento");
            tabelaRelatorio.Columns.Add("vldevolvido_faturamento");
            tabelaRelatorio.Columns.Add("vlfaturado_faturamento");
            tabelaRelatorio.Columns.Add("vlorcamento_faturamento");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            
            FaturamentoPorEstabelec_ttFaturamentoRow[] Faturamento;
            ErpService.FaturamentoPorEstabelec(codigoEstabelecimento, codigoRepresentante, codigoUnidadeNegocio, Convert.ToDateTime(dtInicial.ToString("yyyy/MM/dd")), Convert.ToDateTime(dtFinal.ToString("yyyy/MM/dd")), out Faturamento);
            
            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int z = 0; z < Faturamento.Length; z++)
            {
                tabelaRelatorio.Rows.Add(Faturamento[z].cod_unid_negoc,
                                            Faturamento[z].codestabel,
                                            Faturamento[z].codgerente.Value,
                                            Faturamento[z].codrep.Value,
                                            Faturamento[z].des_unid_negoc,
                                            Faturamento[z].nomeabrev,
                                            Faturamento[z].nomegerente,
                                            Convert.ToDecimal(Faturamento[z].pccota.Value).ToString("0.00"),
                                            Convert.ToDecimal(Faturamento[z].vlcarteira.Value).ToString("0.00"),
                                            Convert.ToDecimal(Faturamento[z].vlcota.Value).ToString("0.00"),
                                            Convert.ToDecimal(Faturamento[z].vldevolvido.Value).ToString("0.00"),
                                            Convert.ToDecimal(Faturamento[z].vlfaturado.Value).ToString("0.00"),
                                            Convert.ToDecimal(Faturamento[z].vlorcamento.Value).ToString("0.00"));

                //EventLog.WriteEntry("Report Faturamento", Faturamento[z].cod_unid_negoc + "\n" +
                //                            Faturamento[z].codestabel + "\n" +
                //                            Faturamento[z].codgerente.Value + "\n" +
                //                            Faturamento[z].codrep.Value + "\n" +
                //                            Faturamento[z].des_unid_negoc + "\n" +
                //                            Faturamento[z].nomeabrev + "\n" +
                //                            Faturamento[z].nomegerente + "\n" +
                //                            Faturamento[z].pccota.Value + "\n" +
                //                            Faturamento[z].vlcarteira.Value + "\n" +
                //                            Faturamento[z].vlcota.Value + "\n" +
                //                            Faturamento[z].vldevolvido.Value + "\n" +
                //                            Faturamento[z].vlfaturado.Value + "\n" +
                //                            Faturamento[z].vlorcamento.Value);
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
    public DataSet FaturamentoPorRepresentante(string codigoEstabelecimento, int codigoRepresentante, string codigoUnidadeNegocio, DateTime dtInicial, DateTime dtFinal)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");
        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("codgrcli", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descricao", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codgrcli_representante", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeemit", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlcarteira", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vldevolvido", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlfaturado", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codgrcli");
            tabelaRelatorio.Columns.Add("descricao");
            tabelaRelatorio.Columns.Add("codemitente");
            tabelaRelatorio.Columns.Add("codgrcli_representante");
            tabelaRelatorio.Columns.Add("nomeemit");
            tabelaRelatorio.Columns.Add("vlcarteira");
            tabelaRelatorio.Columns.Add("vldevolvido");
            tabelaRelatorio.Columns.Add("vlfaturado");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            FaturamentoRepresentante_ttGrCliRow[] FaturamentoGrupoDeCliente;
            FaturamentoRepresentante_ttFaturamentoRepresRow[] FaturamentoRepresentante;
            ErpService.FaturamentoRepresentante(codigoEstabelecimento, codigoRepresentante, codigoUnidadeNegocio, dtInicial, dtFinal, out FaturamentoGrupoDeCliente, out FaturamentoRepresentante);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < FaturamentoGrupoDeCliente.Length; i++)
            {
                for (int y = 0; y < FaturamentoRepresentante.Length; y++)
                {
                    if (FaturamentoGrupoDeCliente[i].codgrcli.Value == FaturamentoRepresentante[y].codgrcli.Value)
                    {
                        tabelaRelatorio.Rows.Add(FaturamentoGrupoDeCliente[i].codgrcli.Value,
                                                FaturamentoGrupoDeCliente[i].descricao,
                                                FaturamentoRepresentante[y].codemitente.Value,
                                                FaturamentoRepresentante[y].codgrcli.Value,
                                                FaturamentoRepresentante[y].nomeemit,
                                                Convert.ToDecimal(FaturamentoRepresentante[y].vlcarteira.Value).ToString("0.00"),
                                                Convert.ToDecimal(FaturamentoRepresentante[y].vldevolvido.Value).ToString("0.00"), //VERIFICAR 
                                                Convert.ToDecimal(FaturamentoRepresentante[y].vlfaturado.Value).ToString("0.00"));
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

}
