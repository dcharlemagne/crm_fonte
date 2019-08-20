using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepFormaPagamento<T> : CrmServiceRepository<T>, IFormaPagamento<T>
    {
        public T ObterPor(string nomeFormaPagamento)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, nomeFormaPagamento);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> Listar()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}