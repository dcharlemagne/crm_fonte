using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutosdaSolicitacao<T> : CrmServiceRepository<T>, IProdutosdaSolicitacao<T>
    {


        public T ObterPor(Guid produtoSolicitacaoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_produtosdasolicitacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoSolicitacaoId);
            query.Criteria.Conditions.Add(cond1);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterAtivoInativoPor(Guid produtoSolicitacaoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_produtosdasolicitacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoSolicitacaoId);
            query.Criteria.Conditions.Add(cond1);
         

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }


        public Boolean AlterarStatus(Guid id, int stateCode, int statusCode)
        {
            SetStateResponse resp = null;

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_produtosdasolicitacao", id),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public List<T> ListarPorSolicitacao(Guid solicitacaoId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_solicitacaodebeneficioid", ConditionOperator.Equal, solicitacaoId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.ProdutoSolicitacao.Status.Ativo);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorSolicitacaoAtivos(Guid solicitacaoId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_solicitacaodebeneficioid", ConditionOperator.Equal, solicitacaoId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            return (List<T>)this.RetrieveMultiple(query).List;
        }


    }
}
