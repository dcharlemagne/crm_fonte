using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain;

namespace Intelbras.CRM2013.DAL
{
    public class RepSolicitacaoXUnidades<T> : CrmServiceRepository<T>, ISolicitacaoXUnidades<T>
    {
        public T ObterPor(Guid itbc_solicitacaodebeneficioid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_solicitacaodebeneficioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_solicitacaodebeneficioid);
            query.Criteria.Conditions.Add(cond1);
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
              
        public List<T> ListarPor(Guid id)
        {
            var query = GetQueryExpression<T>(true);                        
            query.Criteria.AddCondition("itbc_solicitacaoid", ConditionOperator.Equal, id);
            var colecao = RetrieveMultiple(query);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
