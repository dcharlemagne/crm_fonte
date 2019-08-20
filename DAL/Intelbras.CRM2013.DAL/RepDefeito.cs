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
    [Serializable]
    public class RepDefeito<T> : CrmServiceRepository<T>, IDefeito<T>
    {   
        public List<T> ListarPor(LinhaComercial linhaComercial)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            query.AddLink("new_valor_servico", "new_defeitoid", "new_defeitoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, linhaComercial.Id));
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(FamiliaComercial linhaComercial)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            query.AddLink("new_valor_servico", "new_defeitoid", "new_defeitoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, linhaComercial.Id));
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Product produto)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            query.AddLink("new_valor_servico", "new_defeitoid", "new_defeitoid");
            query.LinkEntities[0].AddLink("new_linha_unidade_negocio", "new_linha_unidade_negocioid", "new_linha_unidade_negocioid");
            query.LinkEntities[0].LinkEntities[0].AddLink("product", "new_linha_unidade_negocioid", "itbc_linha_unidade_negocioid");
            query.LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("productid", ConditionOperator.Equal, produto.Id));
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
