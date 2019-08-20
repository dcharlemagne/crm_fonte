using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.SharepointWebService;
using System.Web.Services.Protocols;
using System.Diagnostics;
using Intelbras.CRM2013.Domain.IntelbrasService;
using SDKore.Crm.Util;

namespace Intelbras.CRM2013.DAL
{
    public class RepPedido<T> : CrmServiceRepository<T>, IPedido<T>
    {
        public List<T> ListarPor(Guid salesorderid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("salesorderid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, salesorderid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid salesorderid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("salesorderid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, salesorderid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(String pedidoEMS, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_pedido_ems", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, pedidoEMS);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AtribuirEquipeParaPedido(Guid Equipe, Guid Pedido)
        {


            Microsoft.Crm.Sdk.Messages.AssignRequest assignRequest = new Microsoft.Crm.Sdk.Messages.AssignRequest()
            {
                Assignee = new Microsoft.Xrm.Sdk.EntityReference
                {
                    LogicalName = "team",
                    Id = Equipe
                },

                Target = new Microsoft.Xrm.Sdk.EntityReference("salesorder", Pedido)
            };

            this.Execute(assignRequest);



        }

        public Boolean AlterarStatus(Guid guidId, int status)
        {
            return true;
        }

        public Boolean AlterarStatus(Guid id, int stateCode, int statusCode)
        {

            if ((int)Domain.Enum.Pedido.StateCode.Cancelada == stateCode)
            {
                CancelSalesOrderResponse cancelResponse = null;
                Entity cancelaPedido = new Entity("orderclose");
                cancelaPedido["salesorderid"] = new EntityReference(SDKore.Crm.Util.Utility.GetEntityName<Intelbras.CRM2013.Domain.Model.Pedido>(), id);

                CancelSalesOrderRequest requestCancel = new CancelSalesOrderRequest();

                requestCancel.OrderClose = cancelaPedido;
                requestCancel.Status = new OptionSetValue(statusCode);
                cancelResponse = (CancelSalesOrderResponse)this.Execute(requestCancel);
                if (cancelResponse != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {

                SetStateResponse resp = null;

                SetStateRequest request = new SetStateRequest
                {
                    EntityMoniker = new EntityReference("salesorder", id),
                    State = new OptionSetValue(stateCode),
                    Status = new OptionSetValue(statusCode)
                };

                resp = (SetStateResponse)this.Execute(request);


                if (resp != null)
                    return true;
            }
            return false;
        }

        public Boolean FecharPedido(Guid pedidoId)
        {
            FulfillSalesOrderResponse resp = null;
            Entity fechapedido = new Entity("orderclose");
            fechapedido["salesorderid"] = new EntityReference(SDKore.Crm.Util.Utility.GetEntityName<Intelbras.CRM2013.Domain.Model.Pedido>(), pedidoId);

            FulfillSalesOrderRequest fulfillSales = new FulfillSalesOrderRequest();
            fulfillSales.OrderClose = fechapedido;
            fulfillSales.Status = new OptionSetValue(100001);
            resp = (FulfillSalesOrderResponse)this.Execute(fulfillSales);

            if (resp != null)
                return true;

            return false;


        }

        public bool RemoveItemPedidoB2BnoCRM(string codigoItem, string codigoPedido)
        {
            var queryHelper = new QueryExpression("salesorderdetail");
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("salesorderid", ConditionOperator.Equal, codigoPedido));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("productid", ConditionOperator.Equal, codigoItem));
            var bec = base.Provider.RetrieveMultiple(queryHelper);
            //colecao.List[0]
            if (bec.Entities != null && bec.Entities.Count > 0)
            {
                var item = bec.Entities[0];
                base.Provider.Delete("salesorderdetail", item.Id);
                return true;
            }
            return false;
        }

        public Pedido SalvarPedidoB2BnoCRM(Pedido pedidoDeVenda)
        {
            string retorno = "";

            var be = new Entity("salesorder");

            be["salesorderid"] = new Guid();
            be["customerid"] = new EntityReference("account", (new Domain.Servicos.RepositoryService()).Conta.ObterPorCodigoEmitente(pedidoDeVenda.Cliente.CodigoMatriz).Id);
            be["itbc_tabela_preco_ems"] = pedidoDeVenda.TabelaDePreco.CodigoDaTabelaDePreco;
            be["itbc_dias_negociacao"] = pedidoDeVenda.DiasNegociacao;
            be["itbc_estabelecimento"] = new EntityReference("itbc_estabelecimento", pedidoDeVenda.Estabelecimento.Id);
            be["itbc_keyaccountourepresentanteid"] = new EntityReference("contact", pedidoDeVenda.Representante.Id);
            be["itbc_condicao_pagamento"] = new EntityReference("itbc_condicao_pagamento", pedidoDeVenda.CondicaoDePagamento.Id);
            be["transactioncurrencyid"] = new EntityReference("transactioncurrency",  pedidoDeVenda.Moeda.Id);
            be["itbc_faturamento_parcial"] = pedidoDeVenda.FaturamentoParcial;
            be["itbc_data_emissao"] = pedidoDeVenda.DataDeEmissao;
            be["itbc_data_entrega"] = pedidoDeVenda.DataDeFaturamento;
            be["itbc_data_negociacao"] = pedidoDeVenda.DataBaseNegociacao;
            be["itbc_vlrtotalprodutossemipiest"] = new Money(pedidoDeVenda.PrecoTotalComIPI);
            be["itbc_valor_total_aberto"] = new Money(pedidoDeVenda.PrecoTotal);
            be["totalamount"] = new Money(pedidoDeVenda.PrecoTotal);
            be["description"] = pedidoDeVenda.Descricao;
            be["itbc_condicoesespeciais"] = pedidoDeVenda.DescricaoNota;
            be["itbc_canaldevenda"] = new EntityReference("itbc_canaldevenda", new Guid("FE0C1A84-6CE9-E311-9420-00155D013D39"));
            
            if (pedidoDeVenda.ExportaERP == "OK")
            {
                be["itbc_pedido_ems"] = pedidoDeVenda.CodigoEms;
                be["name"] = pedidoDeVenda.CodigoEms;
                be["itbc_valor_substituicao_tributaria"] = new Money(pedidoDeVenda.SubstituicaoTributaria);
            }
            be["statuscode"] = new OptionSetValue(pedidoDeVenda.StatusPedido);

            Guid pedidoId = new Guid();

            try
            {
                if (pedidoDeVenda.Id != Guid.Empty)
                {
                    be["salesorderid"] = pedidoDeVenda.Id;
                    base.Provider.Update(be);
                    pedidoId = pedidoDeVenda.Id;

                    /*if (pedidoDeVenda.ExportaERP != "OK")
                    {
                        var queryHelper = new QueryExpression("salesorderdetail");
                        queryHelper.Criteria.Conditions.Add(new ConditionExpression("salesorderid", ConditionOperator.Equal, pedidoId));
                        var bec = base.Provider.RetrieveMultiple(queryHelper);

                        if (bec.Entities != null && bec.Entities.Count > 0)
                        {
                            foreach (var item in bec.Entities)
                                base.Provider.Delete("salesorderdetail", item.Id);
                        }
                    }*/
                }
                else
                {
                    be["salesorderid"] = Guid.NewGuid();
                    pedidoId = base.Provider.Create(be);
                }
                pedidoDeVenda.Id = pedidoId;
            }
            catch (SoapException ex)
            {
                retorno = "Tentativa de gravação do Pedido no CRM falhou: " + ex.Detail.InnerText.Replace("\n", " ").Replace("'", "`") + " == " + ex.Message.Replace("\n", " ").Replace("'", "`");
            }

            if (pedidoDeVenda.ExportaERP != "OK")
            {
                if (pedidoId != Guid.Empty)
                {
                    for (var x = 0; x < pedidoDeVenda.ItensDoPedido.Count; x++)
                    {

                        var item = pedidoDeVenda.ItensDoPedido[x];
                        if (item.Quantidade > 0)
                        {
                            var beIP = new Entity("salesorderdetail");

                            beIP["salesorderdetailid"] = new Guid();

                            var produto = (new CRM2013.Domain.Servicos.RepositoryService()).Produto.Retrieve(item.ProdutoModel.Id);
                            
                            beIP["uomid"] = new EntityReference("uom", produto.UnidadePadrao.Id);
                            beIP["itbc_unidadedenegocio"] = new EntityReference("businessunit", produto.UnidadeNegocio.Id);
                            beIP["itbc_kaourepresentante"] = new EntityReference("contact", pedidoDeVenda.Representante.Id);
                            beIP["salesorderid"] = new EntityReference("salesorder", pedidoId); 
                            beIP["productid"] = new EntityReference("product", item.ProdutoModel.Id); 
                            beIP["quantity"] = item.Quantidade;
                            beIP["ispriceoverridden"] = true;
                            beIP["isproductoverridden"] = false;
                            decimal price = 0;
                            if (item.PrecoNegociado == 0)
                                price = item.PrecoMinimo.Value;
                            else
                                price = item.PrecoNegociado.Value;
                            beIP["priceperunit"] = new Money(price);
                            beIP["itbc_preco_negociado"] = new Money(item.PrecoNegociado.Value);
                            beIP["itbc_preco_minimo"] = new Money(item.PrecoMinimo.Value);
                            beIP["manualdiscountamount"] = new Money(item.DescontoManual.Value);
                            var ipi = Convert.ToDecimal(item.AliquotaIPI);
                            beIP["itbc_aliquota_ipi"] = ipi;
                            var precoTotal = price * Convert.ToDecimal(beIP["quantity"]);
                            beIP["baseamount"] = new Money(precoTotal);
                            beIP["extendedamount"] = new Money(precoTotal * (1 + (ipi / 100)));

                            try
                            {
                                var queryHelper = new QueryExpression("salesorderdetail");
                                queryHelper.Criteria.Conditions.Add(new ConditionExpression("salesorderid", ConditionOperator.Equal, pedidoId));
                                queryHelper.Criteria.Conditions.Add(new ConditionExpression("productid", ConditionOperator.Equal, item.ProdutoModel.Id));
                                var bec = base.Provider.RetrieveMultiple(queryHelper);
                                //colecao.List[0]
                                if (bec.Entities != null && bec.Entities.Count > 0)
                                {
                                    var salesOrderDetail = bec.Entities[0];

                                    beIP["salesorderdetailid"] = salesOrderDetail.Id;
                                    base.Provider.Update(beIP);
                                }
                                else
                                {
                                    beIP["salesorderdetailid"] = Guid.NewGuid();
                                    pedidoDeVenda.ItensDoPedido[x].Id = base.Provider.Create(beIP);
                                }

                            }
                            catch (SoapException exS)
                            {
                                retorno = retorno + "\n\nErro SoapException: " + exS.Detail.InnerText;
                            }
                            catch (Exception ex)
                            {
                                retorno = retorno + "\n\nErro Exception: " + ex.Message;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(retorno))
                        EventLog.WriteEntry("CRM B2B - Erro Salvar Pedido CRM", retorno);
                }
            }
            return pedidoDeVenda;
        }

        public string SalvarPedidoB2BnoEMS(Pedido pedidoDeVenda)
        {
            string retorno = "";
            string pedido = "<pedido>";
            string itensDoPedido = "<ped-item>";
            try
            {                
                pedido = pedido + "<cod-cliente>" + pedidoDeVenda.Cliente.CodigoMatriz + "</cod-cliente>";
                pedido = pedido + "<nr-tabpre>" + pedidoDeVenda.TabelaDePreco.CodigoDaTabelaDePreco.ToString() + "</nr-tabpre>";
                pedido = pedido + "<cod-categoria>" + pedidoDeVenda.Cliente.CategoriaB2B.CodigoEms + "</cod-categoria>";
                pedido = pedido + "<cod-unid-negoc>" + pedidoDeVenda.UnidadeDeNegocio.CodigoEms + "</cod-unid-negoc>";
                pedido = pedido + "<cod-estabel>" + pedidoDeVenda.EstabelecimentoModel.Codigo + "</cod-estabel>";
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("CRM B2B - Erro 11", "Erro 11 do WebService Intelbras SalvarPedidoB2BnoEMS. Mensagem: " + ex.Message);
            }

            try
            {
                pedido = pedido + "<cod-cond-pag>" + pedidoDeVenda.CondicaoDePagamento.Codigo + "</cod-cond-pag>";
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("CRM B2B - Erro 12", "Erro 12 do WebService Intelbras SalvarPedidoB2BnoEMS. Mensagem: " + ex.Message);
            }

            try
            {
                pedido = pedido + "<cod-repres>" + pedidoDeVenda.Representante.CodigoRepresentante + "</cod-repres>";
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("CRM B2B - Erro 13", "Erro 13 do WebService Intelbras SalvarPedidoB2BnoEMS. Mensagem: " + ex.Message);
            }

            try
            {
                pedido = pedido + "<nat-operacao>510203</nat-operacao>";
                pedido = pedido + "<dt-emissao>" + pedidoDeVenda.DataDeEmissao.ToString("yyyy-MM-dd") + "</dt-emissao>"; //2010-10-08
                pedido = pedido + "<dt-entrega>" + pedidoDeVenda.DataDeFaturamento.ToString("yyyy-MM-dd") + "</dt-entrega>";
                if (DateTime.Now.Date == pedidoDeVenda.DataBaseNegociacao.Date)
                    pedido = pedido + "<dt-negociacao></dt-negociacao>"; //2010-10-08
                else
                    pedido = pedido + "<dt-negociacao>" + pedidoDeVenda.DataBaseNegociacao.ToString("yyyy-MM-dd") + "</dt-negociacao>"; //2010-10-08
                pedido = pedido + "<dias-negociacao>" + pedidoDeVenda.DiasNegociacao + "</dias-negociacao>";
                pedido = pedido + "<nome-transp> </nome-transp>";
                pedido = pedido + "<cod-atendente>0</cod-atendente>";
                pedido = pedido + "<ind-fat-parc>" + pedidoDeVenda.FaturamentoParcial + "</ind-fat-parc>";
                pedido = pedido + "<cod-moeda>0</cod-moeda>";
                pedido = pedido + "<cond-especial>" + pedidoDeVenda.DescricaoNota + " </cond-especial>";
                pedido = pedido + "<ds-observacao><![CDATA[" + pedidoDeVenda.Descricao + " ]]></ds-observacao>";
                pedido = pedido + "<ind-vendor>" + pedidoDeVenda.Vendor + "</ind-vendor>";
                pedido = pedido + "<vl-liq-ped>" + pedidoDeVenda.PrecoTotal.ToString("0.00").Replace(",", ".") + "</vl-liq-ped>";
                pedido = pedido + "<vl-tot-ped>" + pedidoDeVenda.PrecoTotalComIPI.ToString("0.00").Replace(",", ".") + "</vl-tot-ped>";
                pedido = pedido + "<guid-crm>" + pedidoDeVenda.Id + "</guid-crm>";
                pedido = pedido + "</pedido>";

                foreach (var item in pedidoDeVenda.ItensDoPedido)
                {
                    var produto = (new CRM2013.Domain.Servicos.RepositoryService()).Produto.Retrieve(item.ProdutoModel.Id);
                    itensDoPedido = itensDoPedido + "<item1>";
                    itensDoPedido = itensDoPedido + "<it-codigo>" + produto.Codigo + "</it-codigo>";
                    itensDoPedido = itensDoPedido + "<nat-operacao>510203</nat-operacao>";
                    itensDoPedido = itensDoPedido + "<de-qtd>" + item.Quantidade + "</de-qtd>";
                    itensDoPedido = itensDoPedido + "<qt-pedida>" + item.Quantidade + "</qt-pedida>";
                    if (item.PrecoNegociado == 0)
                        itensDoPedido = itensDoPedido + "<vl-preori>" + item.PrecoMinimo.Value.ToString("0.00").Replace(",", ".") + "</vl-preori>";
                    else
                        itensDoPedido = itensDoPedido + "<vl-preori>" + item.PrecoNegociado.Value.ToString("0.00").Replace(",", ".") + "</vl-preori>";
                    itensDoPedido = itensDoPedido + "<vl-liq-it>" + item.PrecoMinimo.Value.ToString("0.00").Replace(",", ".") + "</vl-liq-it>";
                    itensDoPedido = itensDoPedido + "<vl-liq-abe>0</vl-liq-abe>";
                    itensDoPedido = itensDoPedido + "<de-baixo-min>NO</de-baixo-min>";
                    //Alterado por Carlos Roweder Nass em 14/01/2011
                    //O Guid retornado deve ser o salesorderdetailid pois o integrador s'o atualiza por chave prim'aria, segundo o Tetsuo
                    //itensDoPedido = itensDoPedido + "<guid-crm>" + item.Produto.Id.ToString() + "</guid-crm>";
                    itensDoPedido = itensDoPedido + "<guid-crm>" + item.Id.ToString() + "</guid-crm>";
                    itensDoPedido = itensDoPedido + "</item1>";
                }
                itensDoPedido = itensDoPedido + "</ped-item>";
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("CRM B2B - Erro 22", "Erro 22 do WebService Intelbras SalvarPedidoB2BnoEMS. Mensagem: " + ex.Message);
            }

            string logXml = "++PEDIDO++: " + pedido + "       ++ITENS DO PEDIDO++: " + itensDoPedido;
            if (logXml.Length >= 31000)
                logXml = logXml.Substring(0, 31000);
            EventLog.WriteEntry("CRM B2B Envio Pedido EMS", logXml, EventLogEntryType.Information, 5060); //Maximo de 32k no EventLog


            try
            {
                var erros = new criarPedido_ttErroRow[1];
                Domain.Servicos.HelperWS.IntelbrasService.criarPedido(pedido, itensDoPedido, out erros);
                if (erros.Length > 0)
                {
                    var linha1 = erros[0].mensagem;
                    if (!string.IsNullOrEmpty(linha1) && !linha1.Contains("@@SUB. TRIBUTARIA"))
                    {
                        retorno += "\n\n##ERRO" + linha1 + "##";
                    }
                    for (int i = 0; i < erros.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(erros[i].mensagem))
                        {
                            retorno = retorno + "\n\n" + erros[i].mensagem;
                        }
                    }
                }
            }
            catch (SoapException Ex)
            {
                EventLog.WriteEntry("CRM B2B - Erro Envio Pedido EMS", "Erro do WebService Intelbras Salvar Pedido B2B EMS. \n\nErro: " + Ex.Detail.InnerText + "\n\n" + pedido, EventLogEntryType.Warning, 5051);
            }
            return retorno;
        }

        public List<Pedido> ListarPedidosPor(Domain.Model.Conta cliente)
        {
            List<Pedido> pedidosDeVenda = new List<Pedido>();

            //Se vier do Portal B2B, testa o Guid do contato para restrições em permissões
            Guid guidContato = Guid.Empty;
            try
            {
                guidContato = new Guid(cliente.Site.Trim());
            }
            catch { }

            var queryHelper = new QueryExpression("salesorder");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Orders.Add(new OrderExpression("itbc_data_emissao", OrderType.Descending));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, cliente.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_keyaccountourepresentanteid", ConditionOperator.NotNull));
            if (guidContato != Guid.Empty) //se vier do portal B2B, retorna somente as com status = Proposta
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)Domain.Enum.StatusDoPedido.Proposta));
                
            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                foreach (Entity item in bec.Entities)
                {
                    var podeMostrar = false;
                    try
                    {

                        var q1 = new QueryExpression("itbc_relacionamentodob2b");
                        q1.ColumnSet.AllColumns = true;
                        q1.Criteria.Conditions.Add(new ConditionExpression("itbc_codigocliente", ConditionOperator.Equal, cliente.Id));
                        var beRel = base.Provider.RetrieveMultiple(q1);

                        if (beRel.Entities.Count > 0)
                        {
                            foreach (Entity itemR in beRel.Entities)
                            {
                                var q2 = new QueryExpression("new_permissao_usuario_b2b");
                                q2.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, guidContato));
                                q2.ColumnSet.AllColumns = true;
                                var bePer = base.Provider.RetrieveMultiple(q2);
                                if (bePer.Entities.Count > 0)
                                {
                                    foreach (Entity itemB2B in bePer.Entities)
                                    {
                                        if (!itemB2B.Attributes.Contains("itbc_codigo_representante") && !itemB2B.Attributes.Contains("new_unidade_negocioid"))
                                        {
                                            //não tem acesso
                                        }
                                        else
                                        {
                                            if (itemB2B.Attributes.Contains("itbc_codigo_representante") && !itemB2B.Attributes.Contains("new_unidade_negocioid") && itemR.Attributes.Contains("itbc_codigo_representante"))
                                            {
                                                if (Convert.ToInt32(itemB2B["itbc_codigo_representante"]) == Convert.ToInt32(itemR["itbc_codigo_representante"]))
                                                    podeMostrar = true;
                                            }
                                            else if (itemB2B.Attributes.Contains("new_unidade_negocioid") && !itemB2B.Attributes.Contains("itbc_codigo_representante") && itemR.Attributes.Contains("itbc_codigounidadecomercial"))
                                            {
                                                if (((EntityReference)itemB2B["new_unidade_negocioid"]).Id == ((EntityReference)itemR["itbc_codigounidadecomercial"]).Id)
                                                    podeMostrar = true;
                                            }
                                            else if (itemB2B.Attributes.Contains("new_unidade_negocioid") && itemB2B.Attributes.Contains("itbc_codigo_representante") && itemR.Attributes.Contains("itbc_codigo_representante") && itemR.Attributes.Contains("itbc_codigounidadecomercial"))
                                            {
                                                if (((EntityReference)itemB2B["new_unidade_negocioid"]).Id == ((EntityReference)itemR["itbc_codigounidadecomercial"]).Id
                                                    && Convert.ToInt32(itemB2B["new_codigo_representante"]) == Convert.ToInt32(itemR["itbc_codigo_representante"]))
                                                    podeMostrar = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                    if (podeMostrar)
                    {
                        Pedido p = (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.Retrieve(item.Id);
                        p.ItensDoPedido = (new CRM2013.Domain.Servicos.RepositoryService()).ProdutoPedido.ListarPorPedido(item.Id);
                        pedidosDeVenda.Add(p);
                    }
                }
            }
            return pedidosDeVenda;
        }

        public List<Pedido> ListarItensDoPedidoNoERPPor(int CodigoDoPedido, string Aprovador)
        {
            List<Pedido> pedidosDeVenda = new List<Pedido>();

            ListaItensBloqueados_ttWebPedRow[] itens = null;
            ListaItensBloqueados_ttErroRow[] erros = null;
            Domain.Servicos.HelperWS.SharepointService.ListaItensBloqueados(CodigoDoPedido, Aprovador, out itens, out erros);

            if (erros != null && erros.Length > 0)
            {
                Pedido pedido = new Pedido(this.OrganizationName, this.IsOffline);
                foreach (ListaItensBloqueados_ttErroRow erro in erros)
                    pedido.MensagemErro = pedido.MensagemErro + " ||| (" + erro.codigo.ToString() + ") " + erro.descricao;
                pedidosDeVenda.Add(pedido);
            }
            else
            {
                foreach (ListaItensBloqueados_ttWebPedRow item in itens)
                {
                    Pedido pedido = new Pedido(this.OrganizationName, this.IsOffline);
                    ProdutoPedido itemPedido = new ProdutoPedido(this.OrganizationName, this.IsOffline);
                    itemPedido.ProdutoModel = new Product(this.OrganizationName, this.IsOffline);
                    itemPedido.ProdutoModel.Codigo = item.itcodigo;
                    itemPedido.ProdutoModel.CodigoEms = item.itcodigo;
                    itemPedido.ProdutoModel.Descricao = item.descitem;
                    if (item.qtpedida != null)
                        itemPedido.Quantidade = (int)item.qtpedida.Value;
                    if (item.precoinf != null)
                        itemPedido.PrecoNegociado = item.precoinf.Value;
                    if (item.nrsequencia != null)
                        itemPedido.CodigoEms = item.nrsequencia.Value.ToString();
                    if (item.precomintab != null)
                        itemPedido.PrecoMinimo = item.precomintab.Value;

                    pedido.ItensDoPedido.Add(itemPedido);
                    pedido.CodigoDoPedido = item.nrpedcli;
                    pedido.Nome = item.aprovador;
                    //item.codrefer
                    pedido.Cliente = new Domain.Model.Conta(this.OrganizationName, this.IsOffline);
                    pedido.Cliente.Nome = item.nomeemit;
                    pedido.Cliente.NomeAbreviado = item.nomeabrev;
                    pedido.Cliente.Representante = new SDKore.DomainModel.Lookup(Guid.Empty, item.nomerepres, "contact");
                    
                    pedidosDeVenda.Add(pedido);
                }

            }
            return pedidosDeVenda;
        }

        public string SalvarItensDoPedidoBloqueadoNoERPPor(List<Pedido> pedidos)
        {
            string erros = "", pedidoAnterior = pedidos[0].CodigoEms, aprovador = "", senha = "";
            aprovaReprovaPreco_ttErroRow[] erro = null;
            List<aprovaReprovaPreco_ttWebPedStatusRow> itens = new List<aprovaReprovaPreco_ttWebPedStatusRow>();
            foreach (Pedido pedido in pedidos)
            {
                aprovaReprovaPreco_ttWebPedStatusRow item = new aprovaReprovaPreco_ttWebPedStatusRow();

                if (pedidoAnterior != pedido.CodigoEms)
                {
                    //caso mude o numero do pedido, manda os dados para o WebService
                    erro = null;
                    Domain.Servicos.HelperWS.SharepointService.aprovaReprovaPreco(Convert.ToInt32(pedidoAnterior), aprovador, senha, itens.ToArray(), out erro);
                    if (erro != null && erro.Length > 0)
                    {
                        foreach (aprovaReprovaPreco_ttErroRow er in erro)
                            erros = erros + "Pedido: " + pedido.CodigoEms + " :: (" + er.codigo + ") - " + er.descricao + " |||| ";
                    }
                    pedidoAnterior = pedido.CodigoEms;
                    itens.Clear();
                }

                aprovador = pedido.Nome;
                senha = pedido.Descricao;
                item.aprovador = pedido.Nome;
                item.itcodigo = pedido.ItensDoPedido[0].ProdutoModel.Codigo;
                item.logaprovado = (pedido.ItensDoPedido[0].ExportaERP.ToLower() == "true");
                item.logreprovado = !(pedido.ItensDoPedido[0].ExportaERP.ToLower() == "true");
                item.motivoaprov = pedido.DescricaoNota;
                item.nrpedido = Convert.ToInt32(pedido.CodigoEms);
                item.nrsequencia = Convert.ToInt32(pedido.ItensDoPedido[0].CodigoEms);
                item.codrefer = "";
                itens.Add(item);


            }
            //manda o último pedido que não foi enviado dentro do foreach
            erro = null;
            Domain.Servicos.HelperWS.SharepointService.aprovaReprovaPreco(Convert.ToInt32(pedidoAnterior), aprovador, senha, itens.ToArray(), out erro);
            //Domain.Servicos.HelperWS.IntelbrasService.aprovaReprovaPreco(Convert.ToInt32(pedido.CodigoEms), pedido.Nome, pedido.Descricao, out erro);
            if (erro != null && erro.Length > 0)
            {
                erros = erros + "Pedido: " + pedidoAnterior + " :: (" + erro[0].codigo + ") - " + erro[0].descricao + " |||| ";
            }

            return erros;
        }

        public T ObterPedidosPor(string campo, string valor)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression(campo, ConditionOperator.Equal, valor));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public void EntradaPedidoASTEC(string pedidoXml, out string xmlRetorno)
        {
            Domain.Servicos.HelperWS.IntelbrasService.entradaPedidoAstec(pedidoXml, out xmlRetorno);
            //Retirado daqui e tratado no PedidoService
            //Devido a retorno de caracteres especiais do ERP, removemos aqui par anão dar mais problemas
            //xmlRetorno = xmlRetorno.Replace("&", "E");
        }
        
    }
}
