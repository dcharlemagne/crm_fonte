using System;
using System.Collections.Generic;
using System.Linq;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepFatura<T> : CrmServiceRepository<T>, IFatura<T>
    {
        public List<T> ListarPor(Guid invoiceid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("invoiceid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, invoiceid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("invoiceid", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid invoiceid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("invoiceid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, invoiceid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("invoiceid", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        
        public T ObterPorChaveIntegracao(String chaveIntegracao, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, chaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("invoicenumber", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public T ObterPorNumeroNF(String numeroNF)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("invoicenumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, numeroNF);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("invoicenumber", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorPedidoEMS(String pedidoEMS)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, pedidoEMS);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public bool AlterarProprietario(Guid proprietario, string TipoProprietario, Guid invoice)
        {
            Microsoft.Crm.Sdk.Messages.AssignRequest assignRequest = new Microsoft.Crm.Sdk.Messages.AssignRequest()
            {
                Assignee = new Microsoft.Xrm.Sdk.EntityReference
                {
                    LogicalName = TipoProprietario,
                    Id = proprietario
                },

                Target = new Microsoft.Xrm.Sdk.EntityReference("invoice", invoice)
            };


            if (this.Execute(assignRequest).Results.Count > 0)
                return true;
            else
                return false;
        }
        public bool AlterarStatus(Guid invoice, int stateCode, int statuscode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("invoice", invoice),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;

        }

        //CRM4
        public List<T> ListarNotasFiscaisPor(Domain.Model.Conta cliente)
        {
            if (cliente.Site.Trim() != "") //validacao do B2B para direito de visualizar as notas
                return null;
            var queryFinal = GetQueryExpression<T>(true);
            queryFinal.Criteria.FilterOperator = LogicalOperator.Or;

            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("customerid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, cliente.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, DateTime.Now.AddDays(-90)));
            query.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("createdon", Microsoft.Xrm.Sdk.Query.OrderType.Descending));
            //return (List<T>)this.RetrieveMultiple(query).List;
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;

            //foreach (var itemNota in result)
            for (int x = result.Count - 1; x >= 0; x--)
            {
                var itemNota = result[x];
                var podeMostrar = false;
                var q1 = GetQueryExpression<RelacionamentoB2B>(true);
                q1.ColumnSet.AllColumns = true;
                q1.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigocliente", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, cliente.Id));
                DataCollection<Entity> r1 = base.Provider.RetrieveMultiple(q1).Entities;
                foreach (var itemRelacionamento in r1)
                {
                    var q2 = GetQueryExpression<PermissoesAcessoB2b>(true);
                    q2.ColumnSet.AllColumns = true;
                    q2.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contatoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, new Guid(cliente.Site.Trim())));
                    DataCollection<Entity> r2 = base.Provider.RetrieveMultiple(q2).Entities;
                    foreach (var itemB2B in r2)
                    {
                        if (itemB2B["itbc_codigo_representante"] == null && (itemB2B["new_unidade_negocioid"]) == null)
                        {
                            //não tem acesso
                        }
                        else
                        {
                            if (itemB2B["itbc_codigo_representante"] != null && (itemB2B["new_unidade_negocioid"]) == null)
                            {
                                if (Convert.ToInt32(itemB2B["itbc_codigo_representante"]) == Convert.ToInt32(itemRelacionamento["itbc_codigo_representante"]))
                                    podeMostrar = true;
                            }
                            else if ((itemB2B["new_unidade_negocioid"]) != null && (itemB2B["itbc_codigo_representante"]) == null)
                            {
                                if (((Microsoft.Xrm.Sdk.EntityReference)itemB2B["new_unidade_negocioid"]).Id == ((Microsoft.Xrm.Sdk.EntityReference)itemRelacionamento["new_unidadedenegociosid"]).Id)
                                    podeMostrar = true;
                            }
                            else if (((Microsoft.Xrm.Sdk.EntityReference)itemB2B["new_unidade_negocioid"]) != null && (itemB2B["itbc_codigo_representante"]) != null)
                            {
                                if (((Microsoft.Xrm.Sdk.EntityReference)itemB2B["new_unidade_negocioid"]).Id == ((Microsoft.Xrm.Sdk.EntityReference)itemRelacionamento["new_unidadedenegociosid"]).Id
                                    && Convert.ToInt32(itemB2B["itbc_codigo_representante"]) == Convert.ToInt32(itemRelacionamento["itbc_codigo_representanteid"]))
                                    podeMostrar = true;
                            }
                        }
                    }
                }
                //se não tem acesso, remove da lista de notas consultadas
                if (podeMostrar)
                    queryFinal.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("invoiceid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itemNota.Id));
            }
            if (queryFinal.Criteria.Conditions.Count > 0)
            {
                return (List<T>)this.RetrieveMultiple(queryFinal).List;
            }else
            {
                return new List<T>();
            }
        }

        public T ObterPor(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("contract", "invoiceid", "new_nota_fiscalid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", ConditionOperator.Equal, contrato.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterRemessaPor(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("contract", "invoiceid", "new_nfremessaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", ConditionOperator.Equal, contrato.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<ProdutoFatura> ListarItensDaNotaFiscalPor(Guid id)
        {
            var query = GetQueryExpression<ProdutoFatura>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("invoiceid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, id));
            return (List<ProdutoFatura>)this.RetrieveMultiple(query).List;
        }

        public Fatura PesquisarNotaFiscalFaturaDaOcorrenciaPor(Contrato contrato, Domain.Model.Conta cliente)
        {
            Fatura notaFiscal = null;
            if (contrato != null && cliente != null)
            {
                var query = GetQueryExpression<T>(true);
                query.TopCount = 1;
                query.ColumnSet.AllColumns = true;
                query.AddLink("new_cliente_participante_contrato", "invoiceid", "new_notafiscalid");
                query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
                query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
                DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;

                foreach (var itemNota in result)
                {
                    notaFiscal = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Fatura.Retrieve(itemNota.Id);
                    notaFiscal.Produtos = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).ProdutoFatura.ListarProdutosDaFaturaPor(notaFiscal.Id);
                }
            }
            return notaFiscal;
        }

        public T ObterFaturaPor(Contrato contrato, Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_cliente_participante_endereco", "invoiceid", "new_nf_fatura_locacaoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count > 0)
                return colecao.List[0];

            var query2 = GetQueryExpression<T>(true);
            query2.TopCount = 1;
            query2.AddLink("new_cliente_participante_contrato", "invoiceid", "new_notafiscalid");
            query2.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            query2.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            var colecao2 = this.RetrieveMultiple(query2);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao2.List[0];
        }

        public T ObterRemessaPor(Contrato contrato, Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_cliente_participante_endereco", "invoiceid", "new_nf_remessaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count > 0)
                return colecao.List[0];

            var query2 = GetQueryExpression<T>(true);
            query2.TopCount = 1;
            query2.AddLink("new_cliente_participante_contrato", "invoiceid", "new_nfremessaid");
            query2.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            query2.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            var colecao2 = this.RetrieveMultiple(query2);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao2.List[0];
        }
        //CRM4

    }
}
