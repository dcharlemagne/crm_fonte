using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepTipoSolicitacao<T> : CrmServiceRepository<T>, ITipoSolicitacao<T>
    {
        public List<T> ListarPor()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_tipodesolicitacaoid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tipodesolicitacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_tipodesolicitacaoid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

    }
}