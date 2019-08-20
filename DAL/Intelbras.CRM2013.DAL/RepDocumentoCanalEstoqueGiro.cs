using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepDocumentoCanalEstoqueGiro<T> : CrmServiceRepository<T>, IDocumentoCanalEstoqueGiro<T>
    {
        public List<T> ListarPor(Guid? itbc_accountid, DateTime? itbc_datacriacao, string itbc_url, bool? itbc_somentearquivosnovos, DateTime? itbc_datainicial, DateTime? itbc_datafinal)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_accountid);
            query.Criteria.Conditions.Add(cond1);

            if (itbc_datacriacao.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_datadeenvio", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_datacriacao.Value);
                query.Criteria.Conditions.Add(cond2);
            }

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_url", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_url);
            query.Criteria.Conditions.Add(cond3);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_somentearquivosnovos", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_somentearquivosnovos);
            query.Criteria.Conditions.Add(cond4);

            if (itbc_datainicial.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond5 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_datainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, itbc_datainicial.Value);
                query.Criteria.Conditions.Add(cond5);
            }

            if (itbc_datafinal.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond6 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_datafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual, itbc_datafinal.Value);
                query.Criteria.Conditions.Add(cond6);
            }


            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_docsrequeridoscanalid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_docsrequeridoscanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_docsrequeridoscanalid);
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
