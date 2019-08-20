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
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.DAL
{
    public class RepPremioIntelbras<T>:CrmServiceRepository<T>, Domain.IRepository.IPremioFidelidade<T>
    {
        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
