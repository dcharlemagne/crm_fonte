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
    public class RepQuestionarioResposta<T> : CrmServiceRepository<T>, IQuestionarioResposta<T>
    {
        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public T ObterPorOpcaoId(Guid opcaoId, string contaId, bool status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_questionario_opcao_id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, opcaoId);

            if (status)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
                query.Criteria.Conditions.Add(cond2);
            }

            if (contaId != null)
            {
                
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_questionario_resposta_conta_id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contaId);
                query.Criteria.Conditions.Add(cond3);
            }
            query.Criteria.Conditions.Add(cond1);            

            #endregion

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ObterRespostasConta(string contaId, bool status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            if (status)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
                query.Criteria.Conditions.Add(cond2);
            }

            if (contaId != null)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_questionario_resposta_conta_id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contaId);
                query.Criteria.Conditions.Add(cond3);
            }

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public Boolean AlterarStatus(Guid questionarioRespostaId, int status)
        {
            int stateCode;
            if (status == 0)
            {
                //Ativar
                stateCode = 0;
                status = 1;
            }
            else
            {
                //Inativar
                stateCode = 1;
                status = 2;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_questionarioresposta", questionarioRespostaId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }
    }
}
