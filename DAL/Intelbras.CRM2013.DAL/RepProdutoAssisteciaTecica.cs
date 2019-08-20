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
    public class RepProdutoAssisteciaTecica<T> : CrmServiceRepository<T>, IProdutoAssisteciaTecnica<T>
    {
        public List<T> ObterPor(Guid produto, Guid assistenciaTecnica)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produto));
            query.Criteria.Conditions.Add(new ConditionExpression("new_assistencia_tecnicaid", ConditionOperator.Equal, assistenciaTecnica));
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }
    }
}
