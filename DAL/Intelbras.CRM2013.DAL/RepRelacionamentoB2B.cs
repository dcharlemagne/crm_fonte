using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;


namespace Intelbras.CRM2013.DAL
{
    public class RepRelacionamentoB2B<T> : CrmServiceRepository<T>, IRelacionamentoB2B<T>
    {
        public List<T> ListarPor(Guid? itbc_relacionamentodob2bid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_relacionamentodob2bid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_relacionamentodob2bid);
            #endregion

            #region Validações

            if (itbc_relacionamentodob2bid.HasValue)
                query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(string codigoRelacionamentoB2B)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigorelacionamentob2b", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoRelacionamentoB2B);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid guidCanal, Guid guidSupervisor, Guid guidKeyAccount, Guid guidAssistente,DateTime dtInicial, DateTime dtFinal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidCanal);
            query.Criteria.AddCondition("itbc_supervisorid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidSupervisor);
            query.Criteria.AddCondition("itbc_contactid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidKeyAccount);
            query.Criteria.AddCondition("itbc_assistenteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidAssistente);

            query.Criteria.AddCondition("itbc_datafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, dtInicial);
            query.Criteria.AddCondition("itbc_datainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan, dtFinal);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid id, int stateCode, int statusCode)
        {
            SetStateResponse resp = null;

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_relacionamentodob2b", id),
                State  = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }
    }
}
