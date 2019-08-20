using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Linq;


namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoProjeto<T> : CrmServiceRepository<T>, IProdutoProjeto<T>
    {
        public List<T> ListarPorClientePotencial(Guid clientePotencialId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_cliente_potencial", ConditionOperator.Equal, clientePotencialId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}