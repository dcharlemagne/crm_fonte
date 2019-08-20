using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;
using Microsoft.Xrm.Sdk.Query;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoFatura<T> : CrmServiceRepository<T>, IProdutoFatura<T>
    {
        public List<T> ListarPor(Guid accountid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

           ConditionExpression cond1 = new ConditionExpression("invoicedetailid", ConditionOperator.Equal, accountid);

            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("productdescription", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> listarContagemVendaPrice(Guid customerid, List<Guid?> lstProdutos)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.AddLink("invoice", "invoiceid", "invoiceid", JoinOperator.Inner).LinkCriteria.AddCondition("customerid", ConditionOperator.Equal, customerid);
            DateTime dateFilter = DateTime.Now;
            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_data_emissao", ConditionOperator.LessEqual, dateFilter);
            dateFilter = dateFilter.AddMonths(-6);
            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_data_emissao", ConditionOperator.GreaterEqual, dateFilter);
            query.LinkEntities[0].EntityAlias = "inv";

            ConditionExpression cond1 = new ConditionExpression("productid", ConditionOperator.In, lstProdutos);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid accountid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("invoicedetailid", ConditionOperator.Equal, accountid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("productdescription", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }



        public T ObterPorChaveIntegracao(String chaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_chave_integracao", ConditionOperator.Equal, chaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("productdescription", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid productid, Guid invoiceid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("productid", ConditionOperator.Equal, productid);
            query.Criteria.Conditions.Add(cond1);
            ConditionExpression cond2 = new ConditionExpression("invoiceid", ConditionOperator.Equal, invoiceid);
            query.Criteria.Conditions.Add(cond2);
            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("productdescription", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarProdutosDaFaturaPor(Guid invoiceid)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("invoiceid", ConditionOperator.Equal, invoiceid));
            query.Orders.Add(new OrderExpression("productdescription", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public bool AlterarProprietario(Guid proprietario, string TipoProprietario, Guid invoicedetail)
        {
            Microsoft.Crm.Sdk.Messages.AssignRequest assignRequest = new Microsoft.Crm.Sdk.Messages.AssignRequest()
            {
                Assignee = new Microsoft.Xrm.Sdk.EntityReference
                {
                    LogicalName = TipoProprietario,
                    Id = proprietario
                },

                Target = new Microsoft.Xrm.Sdk.EntityReference("invoicedetail", invoicedetail)
            };


            if (this.Execute(assignRequest).Results.Count > 0)
                return true;
            else
                return false;
        }
        public bool AlterarStatus(Guid invoicedetail, int stateCode, int statuscode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("invoicedetail", invoicedetail),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;

        }

        public T ObterObtemPorNotaFiscal(Guid prodExistId, Guid nfeId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("productid", ConditionOperator.Equal, prodExistId);
            query.Criteria.AddCondition("invoiceid", ConditionOperator.Equal, nfeId);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
