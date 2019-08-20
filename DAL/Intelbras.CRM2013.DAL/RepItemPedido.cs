using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepItensDoPedido<T> : CrmServiceRepository<T>, IItensDoPedido<T>
    {
        public List<T> ListarItensDoPedidosPor(Pedido pedido)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("salesorderid", ConditionOperator.Equal, pedido.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
