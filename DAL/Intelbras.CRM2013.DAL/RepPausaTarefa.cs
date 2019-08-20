using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepPausaTarefa<T> : CrmServiceRepository<T>, IPausaTarefa<T>
    {
        public List<T> ListarPor(Guid activityid)
        {
            var query = GetQueryExpression<T>(true);

            ConditionExpression cond1 = new ConditionExpression("itbc_pausa_de_tarefaid", ConditionOperator.Equal, activityid);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTarefaPor(Guid activityid)
        {
            var query = GetQueryExpression<T>(true);

            ConditionExpression cond1 = new ConditionExpression("itbc_tarefa", ConditionOperator.Equal, activityid);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}