using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepMarca<T> : CrmServiceRepository<T>, IMarca<T>
    {
        public T ObterPor(Guid marcaId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_marca_equipamentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, marcaId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public List<T> ListarPorContato(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.AddLink("itbc_contact_itbc_marca_equipamento", "itbc_marca_equipamentoid", "itbc_marca_equipamentoid").LinkCriteria.AddCondition("contactid", ConditionOperator.Equal, contatoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}