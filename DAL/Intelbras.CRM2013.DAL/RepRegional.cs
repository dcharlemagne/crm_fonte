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
    public class RepRegional<T> : CrmServiceRepository<T>, IRegional<T>
    {
        public List<T> ListarPor(Usuario usuario)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("new_systemuser_x_regional", "new_regionalid", "new_regionalid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("systemuserid", ConditionOperator.Equal, usuario.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
       
        public T ObterPor(Municipio cidade)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("itbc_municipios", "new_regionalid", "new_regionalid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_municipiosid", ConditionOperator.Equal, cidade.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
    }
}