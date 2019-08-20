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
    public class RepAdvertencia<T> : CrmServiceRepository<T>, IAdvertencia<T>
    {
        public List<T> ListarPor(Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, cliente.Id));
            query.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("new_data_justficativa", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
