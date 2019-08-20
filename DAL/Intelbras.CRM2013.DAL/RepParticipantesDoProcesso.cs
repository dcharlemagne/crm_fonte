using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepParticipantesDoProcesso<T> : CrmServiceRepository<T>, IParticipantesDoProcesso<T>
    {
        public List<T> ListarPor(Guid Processo, int Ordem)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_processoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Processo);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_ordem", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Ordem);
            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);
            query.Criteria.Conditions.Add(cond3);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_ordem", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);




            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid ParticipantesDoProcessoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_participantedoprocessoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ParticipantesDoProcessoId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(int ordem, Guid tipoSolicitacao)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_ordem", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ordem);
            query.Criteria.Conditions.Add(cond1);

            query.AddLink("itbc_processo", "itbc_processoid", "itbc_processoid").LinkCriteria.AddCondition("itbc_tipodesolicitacaoid", ConditionOperator.Equal, tipoSolicitacao.ToString());

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

    }
}
