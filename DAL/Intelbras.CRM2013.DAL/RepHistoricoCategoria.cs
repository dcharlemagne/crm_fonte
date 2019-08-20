using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepHistoricoCategoria<T> : CrmServiceRepository<T>, IHistoricoCategoria<T>
    {
        public List<T> ListarPorConta(Guid contaId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_contaid", ConditionOperator.Equal, contaId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid historicoId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_histricodecategoriasdacontaid", ConditionOperator.Equal, historicoId);

            return this.RetrieveMultiple(query).List[0];
        }
    }
}
