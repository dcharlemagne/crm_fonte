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
    public class RepLogisticaReversa<T> : CrmServiceRepository<T>, ILogisticaReversa<T>
    {
        public List<T> ListarLogisticaReversaDo(Domain.Model.Conta postoDeServico)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_posto_autorizadoid", ConditionOperator.Equal, postoDeServico.Id));
            query.Orders.Add(new OrderExpression("createdon", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
