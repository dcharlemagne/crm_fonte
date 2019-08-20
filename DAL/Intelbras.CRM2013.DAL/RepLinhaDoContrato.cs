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
    public class RepLinhaDoContrato<T> : CrmServiceRepository<T>, ILinhaDoContrato<T>
    {
  
        public List<T> ListarPor(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, 1, 2));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        
        public T ObterPor(Ocorrencia ocorrencia, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("incident", "contractdetailid", "contractdetailid");
            query.LinkEntities[0].LinkCriteria.AddCondition("incidentid", ConditionOperator.Equal, ocorrencia.Id);
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
    }
}
