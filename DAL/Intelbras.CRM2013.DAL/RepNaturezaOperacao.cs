using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepNaturezaOperacao<T> : CrmServiceRepository<T>, INaturezaOperacao<T>
    {
        public List<T> ListarPor(Guid itbc_natureza_operacaoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_natureza_operacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_natureza_operacaoid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public T ObterPor(Guid itbc_natureza_operacaoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_natureza_operacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_natureza_operacaoid);
            query.Criteria.Conditions.Add(cond1);            
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);           
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);
            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public T ObterPor(String itbc_codigo_natureza_operacao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_natureza_operacao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_codigo_natureza_operacao);
            query.Criteria.Conditions.Add(cond1);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ObterPor(String[] conjCodigosNatureza)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.Columns.Add("itbc_codigo_natureza_operacao");

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_natureza_operacao", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, conjCodigosNatureza);
            query.Criteria.Conditions.Add(cond1);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public Boolean AlterarStatus(Guid itbc_natureza_operacaoid, int status)
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
                EntityMoniker = new EntityReference("itbc_natureza_operacao", itbc_natureza_operacaoid),
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
