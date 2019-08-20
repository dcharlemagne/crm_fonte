using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepUnidadeKonviva<T> : CrmServiceRepository<T>, IUnidadeKonviva<T>
    {

        

        public T ObterPorIdInterno(Int32 idInterno)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_IdInterno", ConditionOperator.Equal, idInterno);
            query.Criteria.Conditions.Add(cond1);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);

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


        public T ObterPorNome(String nome)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", ConditionOperator.Equal, nome);
            query.Criteria.Conditions.Add(cond1);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid idUnidadeKonviva)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_unidadedokonvivaid", ConditionOperator.Equal, idUnidadeKonviva);
            query.Criteria.Conditions.Add(cond1);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
