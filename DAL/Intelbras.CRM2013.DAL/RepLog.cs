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
    public class RepLog<T> : CrmServiceRepository<T>, ILog<T>
    {
        public List<T> ListarLogDo(Domain.Model.Log diagnostico)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("New_DiagnosticoId", ConditionOperator.Equal, diagnostico.Id));
            query.Orders.Add(new OrderExpression("createdon", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
