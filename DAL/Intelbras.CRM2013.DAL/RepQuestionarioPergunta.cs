using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepQuestionarioPergunta<T> : CrmServiceRepository<T>, IQuestionarioPergunta<T>
    {
        public List<T> ListarQuestionarioPerguntaPorQuestionario(string questionarioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));
            #endregion

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_questionario_id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, questionarioId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_ordem", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion        

            return (List<T>)this.RetrieveMultiple(query).List;

        }

        public List<T> ListarQuestionarioPerguntaPorNomeQuestionario(string nomeQuestionario)
        {
            var query = GetQueryExpression<T>(true);

            #region Relacionamento
            LinkEntity link = query.AddLink("itbc_questionario", "itbc_questionario_id", "itbc_questionarioid", JoinOperator.Inner);
            link.LinkCriteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, nomeQuestionario));

            return (List<T>)this.RetrieveMultiple(query).List;
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_ordem", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion        

            return (List<T>)this.RetrieveMultiple(query).List;

        }
    }
}
