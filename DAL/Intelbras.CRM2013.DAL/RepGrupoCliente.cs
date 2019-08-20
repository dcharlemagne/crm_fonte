    using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepGrupCliente<T> : CrmServiceRepository<T>, IGrupoCliente<T>
    {
        public T ObterPor(Guid contaId)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("account", "new_grupo_clienteid", "new_grupo_clienteid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("accountid", ConditionOperator.Equal, contaId));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
