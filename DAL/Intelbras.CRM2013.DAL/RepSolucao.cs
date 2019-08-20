using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepSolucao<T> : CrmServiceRepository<T>, ISolucao<T>
    {
        public List<T> ListarSolucaoesPor(string defeitoId)
        {
            //INCRIVELMENTE O PARAMETRO NÃO ERA USADO
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarSolucaoesPorFamilia(Guid linhaComercialId, Defeito defeito)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            query.AddLink("new_valor_servico", "new_servico_assistencia_tecnicaid", "new_servicoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, linhaComercialId));
            if (defeito.Id != Guid.Empty)
                query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_defeitoid", ConditionOperator.Equal, defeito.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
