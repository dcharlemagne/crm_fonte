using Intelbras.CRM2013.Domain.IRepository;
using SDKore.Crm;
using System;

namespace Intelbras.CRM2013.DAL
{
    public class RepCausa<T> : CrmServiceRepository<T>, ICausa<T>
    {
        public T ObterPor(Guid causaId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_causaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, causaId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("new_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}