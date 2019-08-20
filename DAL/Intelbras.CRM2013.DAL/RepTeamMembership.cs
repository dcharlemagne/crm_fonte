using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepTeamMembership<T> : CrmServiceRepository<T>, ITeamMembership<T>
    {
        public List<T> ListarPor(Guid TeamMembershipId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("teamid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TeamMembershipId);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid TeamMembershipId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("teamid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TeamMembershipId);
            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
