using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Web.Services.Protocols;
using PortalB2BEMS;

/// <summary>
/// Summary description for CurvaABC
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class CurvaABC : System.Web.Services.WebService {

    public CurvaABC () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet CurvaDetalheCliente(int codigoRepresentante, string codigoUnidadeNegocio ,int codigoEmitente)
    {
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            #region colunasDaTabela

            tabelaRelatorio.Columns.Add("familia");
            tabelaRelatorio.Columns.Add("produto");
            tabelaRelatorio.Columns.Add("descfamilia");
            tabelaRelatorio.Columns.Add("descproduto");
            tabelaRelatorio.Columns.Add("unidneg");
            tabelaRelatorio.Columns.Add("periodo1");
            tabelaRelatorio.Columns.Add("periodo2");
            tabelaRelatorio.Columns.Add("periodo3");
            tabelaRelatorio.Columns.Add("periodo4");
            tabelaRelatorio.Columns.Add("periodo5");
            tabelaRelatorio.Columns.Add("periodo6");
            tabelaRelatorio.Columns.Add("quantidade1");
            tabelaRelatorio.Columns.Add("quantidade2");
            tabelaRelatorio.Columns.Add("quantidade3");
            tabelaRelatorio.Columns.Add("quantidade4");
            tabelaRelatorio.Columns.Add("quantidade5");
            tabelaRelatorio.Columns.Add("quantidade6");
            tabelaRelatorio.Columns.Add("valor1");
            tabelaRelatorio.Columns.Add("valor2");
            tabelaRelatorio.Columns.Add("valor3");
            tabelaRelatorio.Columns.Add("valor4");
            tabelaRelatorio.Columns.Add("valor5");
            tabelaRelatorio.Columns.Add("valor6");
            tabelaRelatorio.Columns.Add("valortotal1");
            tabelaRelatorio.Columns.Add("valortotal2");
            tabelaRelatorio.Columns.Add("valortotal3");
            tabelaRelatorio.Columns.Add("valortotal4");
            tabelaRelatorio.Columns.Add("valortotal5");
            tabelaRelatorio.Columns.Add("valortotal6");
            tabelaRelatorio.Columns.Add("registros1");
            tabelaRelatorio.Columns.Add("registros2");
            tabelaRelatorio.Columns.Add("registros3");
            tabelaRelatorio.Columns.Add("registros4");
            tabelaRelatorio.Columns.Add("registros5");
            tabelaRelatorio.Columns.Add("registros6");
            tabelaRelatorio.Columns.Add("carteiraqtde");
            tabelaRelatorio.Columns.Add("carteiravalor");
            tabelaRelatorio.Columns.Add("carteiravalortotal");
            tabelaRelatorio.Columns.Add("carteiraregs");
            tabelaRelatorio.Columns.Add("faturadoqtde");
            tabelaRelatorio.Columns.Add("faturadovalor");
            tabelaRelatorio.Columns.Add("faturadovalortotal");
            tabelaRelatorio.Columns.Add("faturadoregs");

            #endregion

            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.CurvaDetalhadoCliente_ttDetalheRow[] ttDetalhe;
            ErpService.CurvaDetalhadoCliente(codigoRepresentante,codigoUnidadeNegocio,codigoEmitente, out ttDetalhe);

            //for (int j = 0; j < ttDetalhe[0].periodo.Length; j++)
            //{
            for (int i = 0; i < ttDetalhe.Length; i++)
            {
                #region variaveis



                var periodo1 = ttDetalhe[i].periodo[0];
                var periodo2 = ttDetalhe[i].periodo[1];
                var periodo3 = ttDetalhe[i].periodo[2];
                var periodo4 = ttDetalhe[i].periodo[3];
                var periodo5 = ttDetalhe[i].periodo[4];
                var periodo6 = ttDetalhe[i].periodo[5];

                var quantidade1 = ttDetalhe[i].quantidade[0].Value;
                var quantidade2 = ttDetalhe[i].quantidade[1].Value;
                var quantidade3 = ttDetalhe[i].quantidade[2].Value;
                var quantidade4 = ttDetalhe[i].quantidade[3].Value;
                var quantidade5 = ttDetalhe[i].quantidade[4].Value;
                var quantidade6 = ttDetalhe[i].quantidade[5].Value;

                var valor1 = Convert.ToDecimal(ttDetalhe[i].valor[0].Value).ToString("0.00");
                var valor2 = Convert.ToDecimal(ttDetalhe[i].valor[1].Value).ToString("0.00");
                var valor3 = Convert.ToDecimal(ttDetalhe[i].valor[2].Value).ToString("0.00");
                var valor4 = Convert.ToDecimal(ttDetalhe[i].valor[3].Value).ToString("0.00");
                var valor5 = Convert.ToDecimal(ttDetalhe[i].valor[4].Value).ToString("0.00");
                var valor6 = Convert.ToDecimal(ttDetalhe[i].valor[5].Value).ToString("0.00");

                var valortotal1 = Convert.ToDecimal(ttDetalhe[i].valortotal[0].Value).ToString("0.00");
                var valortotal2 = Convert.ToDecimal(ttDetalhe[i].valortotal[1].Value).ToString("0.00");
                var valortotal3 = Convert.ToDecimal(ttDetalhe[i].valortotal[2].Value).ToString("0.00");
                var valortotal4 = Convert.ToDecimal(ttDetalhe[i].valortotal[3].Value).ToString("0.00");
                var valortotal5 = Convert.ToDecimal(ttDetalhe[i].valortotal[4].Value).ToString("0.00");
                var valortotal6 = Convert.ToDecimal(ttDetalhe[i].valortotal[5].Value).ToString("0.00");

                var registros1 = ttDetalhe[i].registros[0].Value;
                var registros2 = ttDetalhe[i].registros[1].Value;
                var registros3 = ttDetalhe[i].registros[2].Value;
                var registros4 = ttDetalhe[i].registros[3].Value;
                var registros5 = ttDetalhe[i].registros[4].Value;
                var registros6 = ttDetalhe[i].registros[5].Value;

                #endregion

                #region adicionandoLinhas
                tabelaRelatorio.Rows.Add(
                    ttDetalhe[i].familia,
                    ttDetalhe[i].produto,
                    ttDetalhe[i].descfamilia,
                    ttDetalhe[i].descproduto,
                    ttDetalhe[i].unidneg,
                    periodo1,
                    periodo2,
                    periodo3,
                    periodo4,
                    periodo5,
                    periodo6,
                    quantidade1,
                    quantidade2,
                    quantidade3,
                    quantidade4,
                    quantidade5,
                    quantidade6,
                    valor1,
                    valor2,
                    valor3,
                    valor4,
                    valor5,
                    valor6,
                    valortotal1,
                    valortotal2,
                    valortotal3,
                    valortotal4,
                    valortotal5,
                    valortotal6,
                    registros1,
                    registros2,
                    registros3,
                    registros4,
                    registros5,
                    registros6,
                    ttDetalhe[i].carteiraqtde,
                    Convert.ToDecimal(ttDetalhe[i].carteiravalor).ToString("0.00"),
                    Convert.ToDecimal(ttDetalhe[i].carteiravalortotal).ToString("0.00"),
                    ttDetalhe[i].carteiraregs,
                    ttDetalhe[i].faturadoqtde,
                    Convert.ToDecimal(ttDetalhe[i].faturadovalor).ToString("0.00"),
                    Convert.ToDecimal(ttDetalhe[i].faturadovalortotal).ToString("0.00"),
                    ttDetalhe[i].faturadoregs
                );
                #endregion
            }
            //}

        }
        catch (SoapException ex)
        {
            System.Diagnostics.EventLog.WriteEntry("WebServices Report", ex.Detail.InnerText.ToString(), System.Diagnostics.EventLogEntryType.Error, 666);
            throw new SoapException("An error occurred in the webservices when retrieving the data, see the event viewer for more details.", SoapException.ClientFaultCode);
        }

        dataSetPrincipal.Tables.Add(tabelaRelatorio);

        return dataSetPrincipal;
    }


    [WebMethod]
    public DataSet DetalheFaturamentoCliente(int codigoEmitente)
    {
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            #region colunasDaTabela

            tabelaRelatorio.Columns.Add("familia");
            tabelaRelatorio.Columns.Add("produto");
            tabelaRelatorio.Columns.Add("descfamilia");
            tabelaRelatorio.Columns.Add("descproduto");
            tabelaRelatorio.Columns.Add("unidneg");
            tabelaRelatorio.Columns.Add("periodo1");
            tabelaRelatorio.Columns.Add("periodo2");
            tabelaRelatorio.Columns.Add("periodo3");
            tabelaRelatorio.Columns.Add("periodo4");
            tabelaRelatorio.Columns.Add("periodo5");
            tabelaRelatorio.Columns.Add("periodo6");
            tabelaRelatorio.Columns.Add("quantidade1");
            tabelaRelatorio.Columns.Add("quantidade2");
            tabelaRelatorio.Columns.Add("quantidade3");
            tabelaRelatorio.Columns.Add("quantidade4");
            tabelaRelatorio.Columns.Add("quantidade5");
            tabelaRelatorio.Columns.Add("quantidade6");
            tabelaRelatorio.Columns.Add("valor1");
            tabelaRelatorio.Columns.Add("valor2");
            tabelaRelatorio.Columns.Add("valor3");
            tabelaRelatorio.Columns.Add("valor4");
            tabelaRelatorio.Columns.Add("valor5");
            tabelaRelatorio.Columns.Add("valor6");
            tabelaRelatorio.Columns.Add("valortotal1");
            tabelaRelatorio.Columns.Add("valortotal2");
            tabelaRelatorio.Columns.Add("valortotal3");
            tabelaRelatorio.Columns.Add("valortotal4");
            tabelaRelatorio.Columns.Add("valortotal5");
            tabelaRelatorio.Columns.Add("valortotal6");
            tabelaRelatorio.Columns.Add("registros1");
            tabelaRelatorio.Columns.Add("registros2");
            tabelaRelatorio.Columns.Add("registros3");
            tabelaRelatorio.Columns.Add("registros4");
            tabelaRelatorio.Columns.Add("registros5");
            tabelaRelatorio.Columns.Add("registros6");
            tabelaRelatorio.Columns.Add("carteiraqtde");
            tabelaRelatorio.Columns.Add("carteiravalor");
            tabelaRelatorio.Columns.Add("carteiravalortotal");
            tabelaRelatorio.Columns.Add("carteiraregs");
            tabelaRelatorio.Columns.Add("faturadoqtde");
            tabelaRelatorio.Columns.Add("faturadovalor");
            tabelaRelatorio.Columns.Add("faturadovalortotal");
            tabelaRelatorio.Columns.Add("faturadoregs");

            #endregion

            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.DetalheFaturamentoCliente_ttDetalheRow[] ttDetalhe;
            ErpService.DetalheFaturamentoCliente(codigoEmitente, out ttDetalhe);

            //for (int j = 0; j < ttDetalhe[0].periodo.Length; j++)
            //{
                for (int i = 0; i < ttDetalhe.Length; i++)
                {
                    #region variaveis
                    


                    var periodo1 = ttDetalhe[i].periodo[0];
                    var periodo2 = ttDetalhe[i].periodo[1];
                    var periodo3 = ttDetalhe[i].periodo[2];
                    var periodo4 = ttDetalhe[i].periodo[3];
                    var periodo5 = ttDetalhe[i].periodo[4];
                    var periodo6 = ttDetalhe[i].periodo[5];
                    
                    var quantidade1 = ttDetalhe[i].quantidade[0].Value;
                    var quantidade2 = ttDetalhe[i].quantidade[1].Value;
                    var quantidade3 = ttDetalhe[i].quantidade[2].Value;
                    var quantidade4 = ttDetalhe[i].quantidade[3].Value;
                    var quantidade5 = ttDetalhe[i].quantidade[4].Value;
                    var quantidade6 = ttDetalhe[i].quantidade[5].Value;

                    var valor1 = Convert.ToDecimal(ttDetalhe[i].valor[0].Value).ToString("0.00");
                    var valor2 = Convert.ToDecimal(ttDetalhe[i].valor[1].Value).ToString("0.00");
                    var valor3 = Convert.ToDecimal(ttDetalhe[i].valor[2].Value).ToString("0.00");
                    var valor4 = Convert.ToDecimal(ttDetalhe[i].valor[3].Value).ToString("0.00");
                    var valor5 = Convert.ToDecimal(ttDetalhe[i].valor[4].Value).ToString("0.00");
                    var valor6 = Convert.ToDecimal(ttDetalhe[i].valor[5].Value).ToString("0.00");

                    var valortotal1 = Convert.ToDecimal(ttDetalhe[i].valortotal[0].Value).ToString("0.00");
                    var valortotal2 = Convert.ToDecimal(ttDetalhe[i].valortotal[1].Value).ToString("0.00");
                    var valortotal3 = Convert.ToDecimal(ttDetalhe[i].valortotal[2].Value).ToString("0.00");
                    var valortotal4 = Convert.ToDecimal(ttDetalhe[i].valortotal[3].Value).ToString("0.00");
                    var valortotal5 = Convert.ToDecimal(ttDetalhe[i].valortotal[4].Value).ToString("0.00");
                    var valortotal6 = Convert.ToDecimal(ttDetalhe[i].valortotal[5].Value).ToString("0.00");

                    var registros1 = ttDetalhe[i].registros[0].Value;
                    var registros2 = ttDetalhe[i].registros[1].Value;
                    var registros3 = ttDetalhe[i].registros[2].Value;
                    var registros4 = ttDetalhe[i].registros[3].Value;
                    var registros5 = ttDetalhe[i].registros[4].Value;
                    var registros6 = ttDetalhe[i].registros[5].Value;
                    
                    #endregion

                    #region adicionandoLinhas
                    tabelaRelatorio.Rows.Add(
                        ttDetalhe[i].familia,
                        //ttDetalhe[i].produto,
                        ttDetalhe[i].descfamilia,
                        //ttDetalhe[i].descproduto,
                        ttDetalhe[i].unidneg,
                        periodo1,
                        periodo2,
                        periodo3,
                        periodo4,
                        periodo5,
                        periodo6,
                        quantidade1,
                        quantidade2,
                        quantidade3,
                        quantidade4,
                        quantidade5,
                        quantidade6,
                        valor1,
                        valor2,
                        valor3,
                        valor4,
                        valor5,
                        valor6,
                        valortotal1,
                        valortotal2,
                        valortotal3,
                        valortotal4,
                        valortotal5,
                        valortotal6,
                        registros1,
                        registros2,
                        registros3,
                        registros4,
                        registros5,
                        registros6,
                        ttDetalhe[i].carteiraqtde,
                        Convert.ToDecimal(ttDetalhe[i].carteiravalor).ToString("0.00"),
                        Convert.ToDecimal(ttDetalhe[i].carteiravalortotal).ToString("0.00"),
                        ttDetalhe[i].carteiraregs,
                        ttDetalhe[i].faturadoqtde,
                        Convert.ToDecimal(ttDetalhe[i].faturadovalor).ToString("0.00"),
                        Convert.ToDecimal(ttDetalhe[i].faturadovalortotal).ToString("0.00"),
                        ttDetalhe[i].faturadoregs
                    );
                    #endregion
                }
            //}

        }
        catch (SoapException ex)
        {
            System.Diagnostics.EventLog.WriteEntry("WebServices Report", ex.Detail.InnerText.ToString(), System.Diagnostics.EventLogEntryType.Error, 666);
            throw new SoapException("An error occurred in the webservices when retrieving the data, see the event viewer for more details.", SoapException.ClientFaultCode);
        }

        dataSetPrincipal.Tables.Add(tabelaRelatorio);

        return dataSetPrincipal;
    }

    [WebMethod]
    public DataSet CurvaABCRepresentante(int codigoRepresentante, string codigoUnidadeNegocio)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codgrcli", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descricao", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeemit", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("totalperiodo", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codemitente");
            tabelaRelatorio.Columns.Add("codgrcli");
            tabelaRelatorio.Columns.Add("descricao");
            tabelaRelatorio.Columns.Add("nomeemit");
            tabelaRelatorio.Columns.Add("totalperiodo");
            tabelaRelatorio.Columns.Add("desunidnegoc");
            tabelaRelatorio.Columns.Add("codunidnegoc");

            //CONSUMINDO WEBSERVICES DO ERP
            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.CurvaABCRepres_ttCurvaRow[] curvaABC;
            ErpService.CurvaABCRepres(codigoRepresentante, codigoUnidadeNegocio, out curvaABC);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < curvaABC.Length; i++)
            {
                tabelaRelatorio.Rows.Add(curvaABC[i].codemitente,
                                        curvaABC[i].codgrcli,
                                        curvaABC[i].descricao,
                                        curvaABC[i].nomeemit,
                                        Convert.ToDecimal(curvaABC[i].totalperiodo.Value).ToString("0.00").Replace(",","."),
                                        curvaABC[i].desunidnegoc,
                                        curvaABC[i].codunidnegoc
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
