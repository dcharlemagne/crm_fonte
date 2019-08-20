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
    [Serializable]
    public class RepAuditoria<T> : CrmServiceRepository<T>, IAuditoria<T>
    {
        public List<T> ListarPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorrencia.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}