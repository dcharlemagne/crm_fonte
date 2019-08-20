using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepCategoriaB2B<T> : CrmServiceRepository<T>, ICategoriaB2B<T>
    {

        public List<T> ListarPor(int stateCode)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, stateCode);
            
            #endregion

            #region Validações
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid guid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_categoriadob2bid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guid);
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

        public T ObterPor(int codigoCategoriaB2B)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigocategoriab2b", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoCategoriaB2B);
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

        public void AlterarStatus(Guid guid, int state, int status)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_categoriadob2b", guid),
                State  = new OptionSetValue(state),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }
    }
}