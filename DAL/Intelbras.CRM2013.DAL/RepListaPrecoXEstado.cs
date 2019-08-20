using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepListaPrecoXEstado<T> : CrmServiceRepository<T>, IListaPrecoXEstado<T>
    {

        public List<T> ListarPor(Guid? estadoId, Guid? listaPrecoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            if (estadoId.HasValue)
                query.Criteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());

            if (listaPrecoId.HasValue)
                query.Criteria.AddCondition("pricelevelid", ConditionOperator.Equal, listaPrecoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid listaPrecoXEstadoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_pricelevel_itbc_estadoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, listaPrecoXEstadoId);
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
    }
}