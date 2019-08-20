using System;
using System.Data;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepQuestionarioOpcao<T> : CrmServiceRepository<T>, IQuestionarioOpcao<T>
    {
        public List<T> ListarQuestionarioOpcaoPor(Guid questionarioPerguntaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_questionario_pergunta_id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, questionarioPerguntaId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_ordem", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion        

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPorContaId(Guid contaId)
        {
            var query = GetQueryExpression<T>(true);

            LinkEntity link = query.AddLink("itbc_questionarioresposta", "itbc_questionarioopcaoid", "itbc_questionario_opcao_id", JoinOperator.Inner);
            link.LinkCriteria.AddCondition(new ConditionExpression("itbc_questionario_resposta_conta_id", ConditionOperator.Equal, contaId));
            link.Columns.AddColumn("itbc_questionariorespostaid");

            return (List<T>)this.RetrieveMultiple(query).List;


        }


    }
}
