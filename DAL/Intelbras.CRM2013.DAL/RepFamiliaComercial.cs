using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepFamiliaComercial<T> : CrmServiceRepository<T>, IFamiliaComercial<T>
    {

        public List<T> ListarPor(string codigoFamilia)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_codigo_familia_comercial", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoFamilia);

            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(string codigoFamiliaInicial, string codigoFamiliaFinal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            if (codigoFamiliaFinal != string.Empty)
            {
                query.Criteria.AddCondition("itbc_codigo_familia_comercial", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, codigoFamiliaInicial);
                query.Criteria.AddCondition("itbc_codigo_familia_comercial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual, codigoFamiliaFinal);
            } 
            else
            {
                query.Criteria.AddCondition("itbc_codigo_familia_comercial", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoFamiliaInicial);
            }

            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
              
        public T ObterPor(Guid itbc_familiacomercialid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_familiacomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_familiacomercialid);
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

        public T ObterPor(string codigoFamiliaComercial)
        {
            var query = GetQueryExpression<T>(true);
            if (!string.IsNullOrEmpty(codigoFamiliaComercial))
                query.Criteria.AddCondition("itbc_codigo_familia_comercial", ConditionOperator.Equal, codigoFamiliaComercial);
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid itbc_familia_comercialid, int status)
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
                EntityMoniker = new EntityReference("itbc_familiacomercial", itbc_familia_comercialid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public T ObterFamiliaComercialPor(Product produto)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("product", "itbc_familiacomercialid", "itbc_familiacomercial");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("productid", ConditionOperator.Equal, produto.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

    }
}