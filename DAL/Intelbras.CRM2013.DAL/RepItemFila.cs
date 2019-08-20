using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepItemFila<T> : CrmServiceRepository<T>, IItemFila<T>
    {
        public List<T> ListarPor(Guid ItemFilaId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("queueitemid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ItemFilaId);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid queueitemid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("queueitemid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, queueitemid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("subject", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        
    }
}
