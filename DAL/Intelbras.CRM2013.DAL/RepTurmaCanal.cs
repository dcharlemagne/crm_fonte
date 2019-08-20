using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepTurmaCanal<T> : CrmServiceRepository<T>, ITurmaCanal<T>
    {

        public List<T> ListarPor(Guid turmaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_turmadocanalid", ConditionOperator.Equal, turmaId);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorIdTurma(String identificadorTurma)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_idturma", ConditionOperator.Equal, identificadorTurma);
            query.Criteria.Conditions.Add(cond1);

            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public T ObterPor(Guid turmaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_turmadocanalid", ConditionOperator.Equal, turmaId);
            query.Criteria.Conditions.Add(cond1);

            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid turmaId, int status)
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
                EntityMoniker = new EntityReference("itbc_turmadocanal", turmaId),
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
