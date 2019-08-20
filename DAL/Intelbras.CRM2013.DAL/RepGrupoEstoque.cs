using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepGrupoEstoque<T> : CrmServiceRepository<T>, IGrupoEstoque<T>
    {
        public T ObterPor(int itbc_codigo_grupo_estoque)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_grupo_estoque", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_codigo_grupo_estoque);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Validações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid itbc_grupodeestoqueid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_grupodeestoqueid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_grupodeestoqueid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Validações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(string[] codigo)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_grupo_estoque", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, codigo);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public bool AlterarStatus(Guid grupoEstoque, int stateCode, int statuscode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_grupodeestoque", grupoEstoque),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;

        }
    }
}
