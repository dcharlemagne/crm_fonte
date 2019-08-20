using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using SDKore.Crm.Util;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepPostagem<T> : CrmServiceRepository<T>, IPostagem<T>
    {
        public void Update(T entity)
        {
            var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);

            this.Provider.Update(ent);
        }

        public List<T> ListarPorReferenteA(Guid referenteA)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AddColumn("createdby");
            query.ColumnSet.AddColumn("createdon");
            query.ColumnSet.AddColumn("text");

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("regardingobjectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, referenteA);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("source", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 2);
            query.Criteria.Conditions.Add(cond2);


            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("createdon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
