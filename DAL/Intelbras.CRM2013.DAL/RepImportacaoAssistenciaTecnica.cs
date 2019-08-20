using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepImportacaoAssistenciaTecnica<T> : CrmServiceRepository<T>, IImportacaoAssistenciaTecnica<T>
    {
        public List<T> ListarImportacoesPendentes()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, ImportacaoAssistenciaTecnica.RazaoStatus.AProcessar));

            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }
    }
}
