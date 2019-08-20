using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepReceitaPadrao<T> : CrmServiceRepository<T>, IReceitaPadrao<T>
    {
        public List<T> ListarPor()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(int codigoReceita)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_codigoreceitapadrao", ConditionOperator.Equal, codigoReceita);

            #endregion

            #region Ordenação

            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid receitaPadraoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_receitapadraoid", ConditionOperator.Equal, receitaPadraoId);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AlterarStatus(Guid receitaPadraoId, int state, int status, Guid usuarioId)
        {

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_receitapadrao", receitaPadraoId),
                State = new OptionSetValue(state),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request, usuarioId);
        }
    }
}