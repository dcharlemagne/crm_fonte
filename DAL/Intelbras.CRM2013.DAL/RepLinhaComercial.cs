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
    public class RepLinhaComercial<T> : CrmServiceRepository<T>, ILinhaComercial<T>
    {
        public T ObterPor(Product produto)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("product", "new_linha_unidade_negocioid", "itbc_linha_unidade_negocioid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("productid", ConditionOperator.Equal, produto.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarLinhaComercialPor(FamiliaComercial familiaComercial)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("itbc_familiacomercial", "new_linha_unidade_negocioid", "new_linha_unidade_negocioid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_familiacomercialid", ConditionOperator.Equal, familiaComercial.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterLinhaComercialPor(FamiliaComercial familiaComercial)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("itbc_familiacomercial", "new_linha_unidade_negocioid", "new_linha_unidade_negocioid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_familiacomercialid", ConditionOperator.Equal, familiaComercial.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
    }
}
