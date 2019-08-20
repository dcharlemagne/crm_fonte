using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepLinhaCorteRevenda<T> : CrmServiceRepository<T>, ILinhaCorteRevenda<T>
    {
        public List<T> ListarPor(Guid itbc_linhadecorterevendaid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_linhadecorterevendaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_linhadecorterevendaid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(List<Guid> itbc_businessunitid, Guid? itbc_categoriaid)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições

            if (itbc_businessunitid != null)
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, itbc_businessunitid));

            if (itbc_categoriaid != null)
                query.Criteria.AddCondition("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_categoriaid);



            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond1);


            #endregion

            #region Ordenações
            query.AddOrder("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_linhadecorterevendaid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("linhadecorterevendaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_linhadecorterevendaid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
