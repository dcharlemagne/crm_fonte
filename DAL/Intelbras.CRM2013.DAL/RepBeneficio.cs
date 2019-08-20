using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{

    public class RepBeneficio<T> : CrmServiceRepository<T>, IBeneficio<T>
    {

        public List<T> ListarPor(Guid beneficioId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_beneficioid", ConditionOperator.Equal, beneficioId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }



        public T ObterPor(Guid beneficioId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_beneficioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, beneficioId);
            query.Criteria.Conditions.Add(cond1);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(int codigo)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_codigo", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigo.ToString());

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorBenCanalId(Guid beneficioCanalId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_benefdocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, beneficioCanalId.ToString());

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
