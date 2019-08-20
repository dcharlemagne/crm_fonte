using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoEstabelecimento<T> : CrmServiceRepository<T>, IProdutoEstabelecimento<T>
    {
        public List<T> ListarPor(Guid estabelecimentoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_estabelecimentoid", ConditionOperator.Equal, estabelecimentoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPorProduto(Guid? produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            if(produtoId.HasValue)
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorProduto(Guid? produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            if (produtoId.HasValue)
                query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId.ToString());

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPorEstabelecimento(Guid itbc_estabelecimentoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_estabelecimentoid", ConditionOperator.Equal, itbc_estabelecimentoid.ToString());
  
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorProdutoEstabelecimento(Guid produtoId, Guid itbc_estabelecimentoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_estabelecimentoid", ConditionOperator.Equal, itbc_estabelecimentoid.ToString());
           // query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }



        public T ObterPor(Guid produtoEstabelecimentoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_itbc_prodestabelecimentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoEstabelecimentoId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid produtoEstabelecimentoId, int status)
        {
            int stateCode;
            if (status == 0)
            {
                //Ativar
                stateCode = 0;
                status = 1;
            }
            else
            {
                //Inativar
                stateCode = 1;
                status = 2;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_itbc_prodestabelecimento", produtoEstabelecimentoId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }
    }
}
