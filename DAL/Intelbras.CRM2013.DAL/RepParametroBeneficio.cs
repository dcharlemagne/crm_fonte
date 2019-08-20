using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{

    public class RepParametroBeneficio<T> : CrmServiceRepository<T>, IParametroBeneficio<T>
    {
        public List<T> ListarPor(Guid beneficiosid)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_beneficioid", ConditionOperator.Equal, beneficiosid);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public T ObterPor(Guid itbc_parametrosbeneficiosid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_parametrosbeneficiosid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_parametrosbeneficiosid);
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
