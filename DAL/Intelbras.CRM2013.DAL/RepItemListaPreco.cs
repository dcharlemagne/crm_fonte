using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepItemListaPreco<T> : CrmServiceRepository<T>, IItemListaPreco<T>
    {
        public List<T> ListarPor(Guid listaPrecoId, Guid? produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("pricelevelid", ConditionOperator.Equal, listaPrecoId);

            if (produtoId.HasValue)
                query.Criteria.AddCondition("productid", ConditionOperator.Equal, produtoId);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;

        }

        public T ObterPor(Guid productpricelevelid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("productpricelevelid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, productpricelevelid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("productpricelevelid", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion
            
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
