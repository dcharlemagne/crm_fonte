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
    public class RepLiveChatTracking<T> : CrmServiceRepository<T>, ILiveChatTracking<T>
    {
        public T ObterPorOcorrenciaReferenciada(Guid ocorrenciaId)
        {
            var query = GetQueryExpression<T>(true);

            ConditionExpression cond1 = new ConditionExpression("regardingobjectid", ConditionOperator.Equal, ocorrenciaId);
            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
