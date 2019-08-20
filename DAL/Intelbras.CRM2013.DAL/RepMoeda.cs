using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepMoeda<T> : CrmServiceRepository<T>, IMoeda<T>
    {
        public List<T> ListarPor(Guid transactioncurrencyid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("transactioncurrencyid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, transactioncurrencyid);
                query.Criteria.Conditions.Add(cond1);          
            #endregion

            #region Ordenações
                Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("currencyname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
                query.Orders.Add(ord1);          
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid transactioncurrencyid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("transactioncurrencyid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, transactioncurrencyid);
                query.Criteria.Conditions.Add(cond1);           
            #endregion

            #region Ordenações
                Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("currencyname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
                query.Orders.Add(ord1);           
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(string transactioncurrencyname)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("currencyname", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, transactioncurrencyname);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(String nomeMoeda)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("currencyname", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, nomeMoeda);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("currencyname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
