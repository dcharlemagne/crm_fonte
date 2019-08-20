using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]

public class Pedido : System.Web.Services.WebService
{
    public Pedido () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet ListagemPedidoCliente(string codigoRepresentante, int codigoEmitente, DateTime dtInicial, DateTime dtFinal, string numeroDoPedidoDoCliente, int codigoSituacaoDoPedido, bool EmiteDuplic, string codigoEstabelecimento)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codsitaval", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codsitped", typeof(int)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("desccancela", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("desccondpagto", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtemissao", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtentrega", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("noabreppri", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeemit", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrpedcli", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlliqped", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vltotped", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codemitente");
            tabelaRelatorio.Columns.Add("codestabel");
            tabelaRelatorio.Columns.Add("codsitaval");
            tabelaRelatorio.Columns.Add("codsitped");
            tabelaRelatorio.Columns.Add("desccancela");
            tabelaRelatorio.Columns.Add("desccondpagto");
            tabelaRelatorio.Columns.Add("dtemissao");
            tabelaRelatorio.Columns.Add("dtentrega");
            tabelaRelatorio.Columns.Add("noabreppri");
            tabelaRelatorio.Columns.Add("nomeemit");
            tabelaRelatorio.Columns.Add("nrpedcli");
            tabelaRelatorio.Columns.Add("vlliqped");
            tabelaRelatorio.Columns.Add("vltotped");

            //CONSUMINDO WEBSERVICES DO ERP
            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.ListagemPedidosCliente_ttPedVendaRow[] dadosDoErp;
            ErpService.ListagemPedidosCliente(codigoRepresentante, codigoEmitente, dtInicial, dtFinal, numeroDoPedidoDoCliente, codigoSituacaoDoPedido, EmiteDuplic, codigoEstabelecimento, out dadosDoErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            {
                tabelaRelatorio.Rows.Add(dadosDoErp[i].codemitente,
                                        dadosDoErp[i].codestabel,
                                        dadosDoErp[i].codsitaval,
                                        dadosDoErp[i].codsitped,
                                        dadosDoErp[i].desccancela,
                                        dadosDoErp[i].desccondpagto,
                                        dadosDoErp[i].dtemissao,
                                        dadosDoErp[i].dtentrega,
                                        dadosDoErp[i].noabreppri,
                                        dadosDoErp[i].nomeemit,
                                        dadosDoErp[i].nrpedcli,
                                        Convert.ToDecimal(dadosDoErp[i].vlliqped.Value).ToString("0.00"),
                                        Convert.ToDecimal(dadosDoErp[i].vltotped.Value).ToString("0.00"));
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
    public DataSet DetalhePedido(string codigoEstabelecimento, int codigoEmitente, string numeroDoPedidoDoCliente)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            tabelaRelatorio.Columns.Add("bairro", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cep", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("cidade", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codemitente", typeof(int)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codestabel", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("codrep", typeof(int)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("condespec", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("descbloqcr", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("desccondpagto", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("dtemissao", typeof(DateTime)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("dtentrega", typeof(DateTime)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("endereco", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("estado", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("noabreppri", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("nomeabrev", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("nomeemit", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("nometransp", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("nrpedcli", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("observacoes", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("tipofrete", typeof(string)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("vendor", typeof(bool)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("vendordiascarencia", typeof(int)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("vendortaxa", typeof(decimal)).AllowDBNull = true;

            //CONSUMINDO WEBSERVICES DO ERP
            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttNotaFiscalRow[] NotaFiscalDoPedido;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttPedItemRow[] ItemDoPedido;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttPedVendaRow[] DetalheDoPedido;
            ErpService.DetalhePedidos(codigoEstabelecimento, codigoEmitente, numeroDoPedidoDoCliente, out DetalheDoPedido, out ItemDoPedido, out NotaFiscalDoPedido);
            
            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < DetalheDoPedido.Length; i++)
            {
               tabelaRelatorio.Rows.Add(DetalheDoPedido[i].bairro,
                                                    DetalheDoPedido[i].cep,
                                                    DetalheDoPedido[i].cidade,
                                                    DetalheDoPedido[i].codemitente,
                                                    DetalheDoPedido[i].codestabel,
                                                    DetalheDoPedido[i].codrep,
                                                    DetalheDoPedido[i].condespec,
                                                    DetalheDoPedido[i].descbloqcr,
                                                    DetalheDoPedido[i].desccondpagto,
                                                    DetalheDoPedido[i].dtemissao,
                                                    DetalheDoPedido[i].dtentrega,
                                                    DetalheDoPedido[i].endereco,
                                                    DetalheDoPedido[i].estado,
                                                    DetalheDoPedido[i].noabreppri,
                                                    DetalheDoPedido[i].nomeabrev,
                                                    DetalheDoPedido[i].nomeemit,
                                                    DetalheDoPedido[i].nometransp,
                                                    DetalheDoPedido[i].nrpedcli,
                                                    DetalheDoPedido[i].observacoes,
                                                    DetalheDoPedido[i].tipofrete,
                                                    DetalheDoPedido[i].vendor,
                                                    DetalheDoPedido[i].vendordiascarencia,
                                                    DetalheDoPedido[i].vendortaxa//22
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
    public DataSet ItensDoPedido(string codigoEstabelecimento, int codigoEmitente, string numeroDoPedidoDoCliente)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("aliqipiItem", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("descitemItem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("itcodigoItem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrevItem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrpedcliItem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("qtpedidaItem", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("saldoitem", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlpreuniItem", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("aliqipiItem");
            tabelaRelatorio.Columns.Add("descitemItem");
            tabelaRelatorio.Columns.Add("itcodigoItem");
            tabelaRelatorio.Columns.Add("nomeabrevItem");
            tabelaRelatorio.Columns.Add("nrpedcliItem");
            tabelaRelatorio.Columns.Add("qtpedidaItem");
            tabelaRelatorio.Columns.Add("saldoitem");
            tabelaRelatorio.Columns.Add("vlpreuniItem");
            tabelaRelatorio.Columns.Add("situacao");
            tabelaRelatorio.Columns.Add("vlsubstribItem");
            //CONSUMINDO WEBSERVICES DO ERP
            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttNotaFiscalRow[] NotaFiscalDoPedido;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttPedItemRow[] ItemDoPedido;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttPedVendaRow[] DetalheDoPedido;
            ErpService.DetalhePedidos(codigoEstabelecimento, codigoEmitente, numeroDoPedidoDoCliente, out DetalheDoPedido, out ItemDoPedido, out NotaFiscalDoPedido);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < ItemDoPedido.Length; i++)
            {
                tabelaRelatorio.Rows.Add(   Convert.ToDecimal(ItemDoPedido[i].aliqipi.Value).ToString("0.00"), //VERIFICAR
                                            ItemDoPedido[i].descitem,
                                            ItemDoPedido[i].itcodigo,
                                            ItemDoPedido[i].nomeabrev,
                                            ItemDoPedido[i].nrpedcli,
                                            Convert.ToDecimal(ItemDoPedido[i].qtpedida.Value).ToString("0.00"),
                                            Convert.ToDecimal(ItemDoPedido[i].saldoitem.Value).ToString("0.00"),
                                            Convert.ToDecimal(ItemDoPedido[i].vlpreuni.Value).ToString("0.00"),
                                            ItemDoPedido[i].situacao,
                                            Convert.ToDecimal(ItemDoPedido[i].vlsubstrib.Value).ToString("0.00")
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
    public DataSet NotasFiscaisDoPedido(string codigoEstabelecimento, int codigoEmitente, string numeroDoPedidoDoCliente)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("canceladaNota", typeof(bool)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("codestabelNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("desccondpagtoNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dtemisnota", typeof(DateTime)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nomeabrevNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nometranspNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrnotafisNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("nrpedcliNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("serieNota", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vltotnota", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("canceladaNota");
            tabelaRelatorio.Columns.Add("codestabelNota");
            tabelaRelatorio.Columns.Add("desccondpagtoNota");
            tabelaRelatorio.Columns.Add("dtemisnota");
            tabelaRelatorio.Columns.Add("nomeabrevNota");
            tabelaRelatorio.Columns.Add("nometranspNota");
            tabelaRelatorio.Columns.Add("nrnotafisNota");
            tabelaRelatorio.Columns.Add("nrpedcliNota");
            tabelaRelatorio.Columns.Add("serieNota");
            tabelaRelatorio.Columns.Add("vltotnota");

            //CONSUMINDO WEBSERVICES DO ERP
            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttNotaFiscalRow[] NotaFiscalDoPedido;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttPedItemRow[] ItemDoPedido;
            PortalB2BEMS.WSIntelbras.DetalhePedidos_ttPedVendaRow[] DetalheDoPedido;
            ErpService.DetalhePedidos(codigoEstabelecimento, codigoEmitente, numeroDoPedidoDoCliente, out DetalheDoPedido, out ItemDoPedido, out NotaFiscalDoPedido);
            
            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int z = 0; z < NotaFiscalDoPedido.Length; z++)
            {
                tabelaRelatorio.Rows.Add(
                                            NotaFiscalDoPedido[z].cancelada,
                                            NotaFiscalDoPedido[z].codestabel,
                                            NotaFiscalDoPedido[z].desccondpagto,
                                            NotaFiscalDoPedido[z].dtemisnota,
                                            NotaFiscalDoPedido[z].nomeabrev,
                                            NotaFiscalDoPedido[z].nometransp,
                                            NotaFiscalDoPedido[z].nrnotafis,
                                            NotaFiscalDoPedido[z].nrpedcli,
                                            NotaFiscalDoPedido[z].serie,
                                            Convert.ToDecimal(NotaFiscalDoPedido[z].vltotnota.Value).ToString("0.00")
                                            );//10
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
    public DataSet ListagemPedidoAbertoCliente(string unidadeNegocio, string codigoEstabelecimento, int codigoRepresentante,DateTime dtInicial,DateTime dtFinal, int codigoEmitente)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

    //    //MONTANDO AS COLUNAS DA TABELA RELATORIO
    //    tabelaRelatorio.Columns.Add("descitem", typeof(string)).AllowDBNull=true;
    //    tabelaRelatorio.Columns.Add("itcodigo", typeof(string)).AllowDBNull = true;
    //    tabelaRelatorio.Columns.Add("nrpedcli", typeof(string)).AllowDBNull = true;
    //    tabelaRelatorio.Columns.Add("nrsequencia", typeof(int)).AllowDBNull = true;
    //    tabelaRelatorio.Columns.Add("qtsaldo", typeof(decimal)).AllowDBNull = true;
    //    tabelaRelatorio.Columns.Add("vlcarteira", typeof(decimal)).AllowDBNull = true;
        try
        {
            tabelaRelatorio.Columns.Add("descitem");
            tabelaRelatorio.Columns.Add("itcodigo");
            tabelaRelatorio.Columns.Add("nrpedcli");
            tabelaRelatorio.Columns.Add("nrsequencia");
            tabelaRelatorio.Columns.Add("qtsaldo");
            tabelaRelatorio.Columns.Add("vlcarteira");

            //CONSUMINDO WEBSERVICES DO ERP
            PortalB2BEMS.WSIntelbras.relatoriosB2BService ErpService = new PortalB2BEMS.WSIntelbras.relatoriosB2BService();
            ErpService.Timeout = 99999999;
            PortalB2BEMS.WSIntelbras.ListagemPedidosAbertoCliente_ttSaldoCarteiraRow[] dadosDoErp;
            ErpService.ListagemPedidosAbertoCliente(codigoEstabelecimento, codigoRepresentante, codigoEmitente, unidadeNegocio, dtInicial, dtFinal, out dadosDoErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            {
                tabelaRelatorio.Rows.Add(dadosDoErp[i].descitem,
                                        dadosDoErp[i].itcodigo,
                                        dadosDoErp[i].nrpedcli,
                                        dadosDoErp[i].nrsequencia.Value,
                                        Convert.ToDecimal(dadosDoErp[i].qtsaldo.Value).ToString("0.00"),
                                        Convert.ToDecimal(dadosDoErp[i].vlcarteira.Value).ToString("0.00"));
                
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