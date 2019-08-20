using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepGrupoPremio<T> : CrmServiceRepository<T>, IGrupoPremio<T>
    {
        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
