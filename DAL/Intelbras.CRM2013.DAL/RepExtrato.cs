using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepExtrato<T> : CrmServiceRepository<T>, IExtrato<T>
    {
        public T ObterPor(string numeroExtrato)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_extrato", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, numeroExtrato));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        
        public List<T> ListarPor(DateTime dataCriacao)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, dataCriacao));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan, dataCriacao.Date.AddDays(1)));
            query.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("new_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Domain.Model.Conta postoDeServico)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_posto_servicoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, postoDeServico.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterThan, 1));
            query.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("createdon", Microsoft.Xrm.Sdk.Query.OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}