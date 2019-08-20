using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepProcesso<T> : CrmServiceRepository<T>, IProcesso<T>
    {
        public List<T> ListarPor(Guid processoid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_processoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, processoid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid processoid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_processoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, processoid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorTipoDeSolicitacao(Guid TipoDeSolicitacaoId,Guid? BeneficioPrograma)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tipodesolicitacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TipoDeSolicitacaoId);
            query.Criteria.Conditions.Add(cond1);

            if (BeneficioPrograma.HasValue)
            {
                ConditionExpression cond2 = new ConditionExpression("itbc_beneficiodoprogramaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, BeneficioPrograma.Value);
                query.Criteria.Conditions.Add(cond2);
            }
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
