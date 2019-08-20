using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;


namespace Intelbras.CRM2013.DAL
{
    public class RepUnidadeDeMedida<T> : CrmServiceRepository<T>, IUnidadeDeMedida<T>
    {
        public List<T> ListarPor(Guid uomid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("uomid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, uomid);

            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarPor(String name)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, name);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarPor(String[] conjNomes)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.Columns.Add("name");

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("name", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, conjNomes);

            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public T ObterPor(Guid uomid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("uomid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, uomid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

    }
}
