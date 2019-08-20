using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepAcaoSubsidiadaVmc<T> : CrmServiceRepository<T>, IAcaoSubsidiadaVmc<T>
    {
        public List<T> ListarPor(Guid codigoId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_acaosubsidiadavmcid", ConditionOperator.Equal, codigoId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> Listar()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            return (List<T>)this.RetrieveMultiple(query).List;
        }


        public T ObterPor(Guid codigoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_acaosubsidiadavmcid", ConditionOperator.Equal, codigoId));
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }


        public T ObterPorCodigo(String codigoAcao)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_codigo", ConditionOperator.Equal, codigoAcao));
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        
    }
}
