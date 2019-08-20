using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;
using Intelbras.CRM2013.Application.WebServices.RelatoriosB2B.WSIntelbras;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.Model;


/// <summary>
/// Summary description for Preco
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class Preco : System.Web.Services.WebService
{

    public Preco()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public DataSet ListagemPreco(string codigoEstabelecimento, string Estado, int codigoCategoria, string DescricaoUnidadeNegocio, int codigoGrupoCliente, int codigoCliente)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            //tabelaRelatorio.Columns.Add("descitem", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("dsfmcodcom", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("fmcodcom", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("itcodigo", typeof(string)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlprecocomipi", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlprecopma", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlprecopmd", typeof(decimal)).AllowDBNull = true;
            //tabelaRelatorio.Columns.Add("vlprecosemipi", typeof(decimal)).AllowDBNull = true;
            tabelaRelatorio.Columns.Add("descitem");
            tabelaRelatorio.Columns.Add("dsfmcodcom");
            tabelaRelatorio.Columns.Add("fmcodcom");
            tabelaRelatorio.Columns.Add("itcodigo");
            tabelaRelatorio.Columns.Add("vlprecocomipi");
            tabelaRelatorio.Columns.Add("vlprecopma");
            tabelaRelatorio.Columns.Add("vlprecopmd");
            tabelaRelatorio.Columns.Add("vlprecosemipi");
            tabelaRelatorio.Columns.Add("laynome");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            ListagemTabelasPreco_ttPrecoItemRow[] dadosDoErp;
            ErpService.ListagemTabelasPreco(codigoEstabelecimento, Estado, codigoCategoria, DescricaoUnidadeNegocio, codigoGrupoCliente, codigoCliente, out dadosDoErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            {

                tabelaRelatorio.Rows.Add(dadosDoErp[i].descitem,
                                            dadosDoErp[i].dsfmcodcom,
                                            dadosDoErp[i].fmcodcom,
                                            dadosDoErp[i].itcodigo,
                                            Convert.ToDecimal(dadosDoErp[i].vlprecocomipi.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].vlprecopma.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].vlprecopmd.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].vlprecosemipi.Value).ToString("0.00"),
                                            dadosDoErp[i].laynome
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
    public DataSet TabelaPrecoAstecGeral()
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            tabelaRelatorio.Columns.Add("itcodigo");
            tabelaRelatorio.Columns.Add("itdescricao");
            tabelaRelatorio.Columns.Add("precovenda1");
            tabelaRelatorio.Columns.Add("precovenda2");
            tabelaRelatorio.Columns.Add("precovenda3");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            TabPrecoAstec_ttPrecoItemRow[] dadosDoErp;
            ErpService.TabPrecoAstec(out dadosDoErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            {

                tabelaRelatorio.Rows.Add(Convert.ToDecimal(dadosDoErp[i].itcodigo).ToString(),
                                            dadosDoErp[i].descricao,
                                            Convert.ToDecimal(dadosDoErp[i].precovenda.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].precovenda2.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].precovenda3.Value).ToString("0.00")
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
    public DataSet TabelaPrecoAstecEsp(string coditem)
    {
        //INICIALIZAÇÃO DO DATASET E DATATABLE
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO
            tabelaRelatorio.Columns.Add("itcodigo");
            tabelaRelatorio.Columns.Add("itdesc");
            tabelaRelatorio.Columns.Add("aliquotaipi");
            tabelaRelatorio.Columns.Add("nivel");
            tabelaRelatorio.Columns.Add("precovenda1");
            tabelaRelatorio.Columns.Add("precovenda2");
            tabelaRelatorio.Columns.Add("precovenda3");
            tabelaRelatorio.Columns.Add("seq");
            tabelaRelatorio.Columns.Add("localmontag");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            TabPrecoAstecPorItem_ttEstruturaRow[] dadosDoErp;
            ErpService.TabPrecoAstecPorItem(coditem, out dadosDoErp);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int i = 0; i < dadosDoErp.Length; i++)
            {

                tabelaRelatorio.Rows.Add(dadosDoErp[i].itcodigo,
                                            dadosDoErp[i].descricao,
                                            dadosDoErp[i].aliquotaipi,
                                            dadosDoErp[i].nivel,
                                            Convert.ToDecimal(dadosDoErp[i].precovenda.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].precovenda2.Value).ToString("0.00"),
                                            Convert.ToDecimal(dadosDoErp[i].precovenda3.Value).ToString("0.00"),
                                            dadosDoErp[i].seq,
                                            ""//dadosDoErp[i].localmontag
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
    public DataSet EstruturaProdutoAstec(string codigoItem, string descricao)
    {
        DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
        DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

        try
        {
            //MONTANDO AS COLUNAS DA TABELA RELATORIO 
            tabelaRelatorio.Columns.Add("seq");
            tabelaRelatorio.Columns.Add("nivel");
            tabelaRelatorio.Columns.Add("itcodigo");
            tabelaRelatorio.Columns.Add("descricao");
            tabelaRelatorio.Columns.Add("itpai");
            tabelaRelatorio.Columns.Add("localmontag");

            //CONSUMINDO WEBSERVICES DO ERP
            relatoriosB2BObjClient ErpService = new relatoriosB2BObjClient();
            //ErpService.Timeout = 99999999;
            BuscaItemVenda_ttEstruturaRow[] Estrutura;
            ErpService.BuscaItemVenda(codigoItem, descricao, out Estrutura);

            //LOOP PARA POPULAR OS DADOS NA TABELA
            for (int y = 0; y < Estrutura.Length; y++)
            {
                tabelaRelatorio.Rows.Add(
                                    Estrutura[y].seq,
                                    Estrutura[y].nivel,
                                    Estrutura[y].itcodigo,
                                    Estrutura[y].descricao,
                                    Estrutura[y].itpai,
                                    Estrutura[y].localmontag
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
    public DataSet DetalheProdutoAstec(string codItem, string codEmitente, string codEstabel)
    {
            DataSet dataSetPrincipal = new DataSet("dataSetPrincipal");
            DataTable tabelaRelatorio = new DataTable("tabelaRelatorio");

            //try
            //{

                BuscarPreco_ttErrosRow[] erro = null;
                BuscarPreco_ttPrecoItemRow[] row = null;

                List<UnidadeNegocio> lstUnidadeNegocio = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).UnidadeNegocio.ListarPor("POS VENDA");
                UnidadeNegocio unidadeNegocio = lstUnidadeNegocio.First<UnidadeNegocio>();
                var categoria = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarCategoriaCliente(int.Parse(codEmitente), unidadeNegocio);
        
                if (categoria != null)
                {
                    Intelbras.CRM2013.Domain.Servicos.HelperWS.IntelbrasService.BuscarPreco(codEstabel,                        //fixo relatório (101,104)
                        int.Parse(codEmitente),          //cliente
                        unidadeNegocio.Nome,             //fixo
                        int.Parse(categoria.CodigoCategoria),
                        codItem,                         //produto
                        "ASTEC 02",
                        0,                               //0 = nao
                        out row,
                        out erro);
                }
                //MONTANDO AS COLUNAS DA TABELA RELATORIO
                tabelaRelatorio.Columns.Add("itcodigo");
                tabelaRelatorio.Columns.Add("descitem");
                tabelaRelatorio.Columns.Add("vlipi");
                tabelaRelatorio.Columns.Add("preco");
                tabelaRelatorio.Columns.Add("total");

                if (row != null)
                {
                    for (int i = 0; i < row.Length; i++)
                    {
                        tabelaRelatorio.Rows.Add(row[i].itcodigo,
                                                 row[i].descitem,
                                                 row[i].vlipi,
                                                 Math.Round(Convert.ToDecimal(row[i].preco), 2),
                                                 Math.Round(Convert.ToDecimal(row[i].preco * (1 + (row[i].vlipi / 100))), 2)
                                                 );
                    }
                }
            //}
            //catch (SoapException ex)
            //{
            //    throw new SoapException("", SoapException.ClientFaultCode);
            //}

            //catch (Exception ex)
            //{
            //    EventLog.WriteEntry("Erro Report", "Relatório BuscaItemAstec\nCliente com categoria diferente de ASTEC\nEmitente: "+ codEmitente+"\n"+ ex.Message);
            //    throw new Exception("Cadastro incompleto no sistema. Entre em contato com a Intelbras.");
            //}

            //ATRIBUINDO DATATABLE AO DATASET
            dataSetPrincipal.Tables.Add(tabelaRelatorio);

            //RETORNO DO DATASET
            return dataSetPrincipal;
    }
}
