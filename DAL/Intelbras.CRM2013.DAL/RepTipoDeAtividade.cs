using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
namespace Intelbras.CRM2013.DAL
{
    public class RepTipoDeAtividade<T> : CrmServiceRepository<T>, ITipoDeAtividade<T>
    {
        public List<T> ListarPor(Guid TipoDeAtividade)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tipoatividade", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TipoDeAtividade);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorNome(string NomeAtividade)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, NomeAtividade);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorPapel(Guid PapelId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_papelid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, PapelId);
            query.Criteria.Conditions.Add(cond1);
            
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public T ObterPor(Guid TipoDeAtividade)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tipoatividade", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TipoDeAtividade);
            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
