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
    public class RepLancamentoAvulsoDoExtrato<T> : CrmServiceRepository<T>, ILancamentoAvulsoDoExtrato<T>
    {
        public List<T> ListarSemExtratoPor(Domain.Model.Conta autorizada)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, autorizada.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("new_extratoid", ConditionOperator.Null));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Extrato extrato)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_extratoid", ConditionOperator.Equal, extrato.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
