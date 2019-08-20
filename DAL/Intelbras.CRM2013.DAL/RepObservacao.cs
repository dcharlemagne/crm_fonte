using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Text;

namespace Intelbras.CRM2013.DAL
{

    public class RepObservacao<T> : CrmServiceRepository<T>, IObservacao<T>
    {

        public T ObterPorParametrosGlobais(Guid parametroglobalid)
        {
            return this.Obter("itbc_parmetrosglobais", "objectid", "itbc_parmetrosglobaisid", "itbc_parmetrosglobaisid", parametroglobalid);
        }

        public T ObterPorMeta(Guid metaid)
        {
            return this.Obter("itbc_metas", "objectid", "itbc_metasid", "itbc_metasid", metaid);
        }

        public T Obter(string Entidade, string LinkTo, string LinkFrom,string FieldFilter, Guid ID)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;

            #region Condições
            query.AddLink(Entidade, LinkTo, LinkFrom).LinkCriteria.AddCondition(FieldFilter, ConditionOperator.Equal, ID);
            query.Criteria.AddCondition("isdocument", ConditionOperator.Equal, "1");
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("createdon", OrderType.Descending);
            query.Orders.Add(ord1);
            #endregion
            
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}

