using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.IRepository;
using SDKore.Crm;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepContaSegmento<T> : CrmServiceRepository<T>, IContaSegmento<T>
    {
        public List<T> ListarPor(Guid id)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("itbc_contasegmentoid", ConditionOperator.Equal, id);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
    }
}
