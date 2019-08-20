using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
namespace Intelbras.CRM2013.DAL
{
    public class RepTipoDeDenuncia<T> : CrmServiceRepository<T>, ITipoDeDenuncia<T>
    {
        
        public T ObterPor(Guid TipoDeAtividade)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tipodedenunciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TipoDeAtividade);
            query.Criteria.Conditions.Add(cond1);
            //Status Ativo
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.TipoDeDenuncia.status.Ativo);
            query.Criteria.Conditions.Add(cond2);
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> Listar()
        {
            var query = GetQueryExpression<T>(true);
            //Status Ativo
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression(
                "statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.TipoDeDenuncia.status.Ativo);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
            
        }

        
    }
}
