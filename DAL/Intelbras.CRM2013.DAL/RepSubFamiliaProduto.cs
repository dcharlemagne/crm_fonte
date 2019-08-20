using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepSubFamiliaProduto<T> : CrmServiceRepository<T>, ISubfamiliaProduto<T>
    {
        public List<T> ListarPor(Guid itbc_famildeprodid)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_famildeprodid", ConditionOperator.Equal, itbc_famildeprodid));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(string itbc_codigo_subfamilia)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_codigo_subfamilia", ConditionOperator.Equal, itbc_codigo_subfamilia));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Guid itbc_subfamiliadeprodutoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_subfamiliadeprodutoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_subfamiliadeprodutoid);
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

        public List<T> ListarPor(String codigoFamiliaProduto)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_subfamilia", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoFamiliaProduto);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public bool AlterarStatus(Guid itbc_subfamiliadeprodutoid, int stateCode, int statuscode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_subfamiliadeproduto", itbc_subfamiliadeprodutoid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;

        }



        public List<T> ListaPorUnidadeNegocio(Guid unidadenegocioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_famildeprod", "itbc_famildeprod", "itbc_famildeprodid").AddLink(
                "itbc_segmento", "itbc_segmentoid", "itbc_segmentoid").LinkCriteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadenegocioId);

            //query.AddLink("itbc_famildeprod", "itbc_famildeprodid", "itbc_famildeprod").LinkCriteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadenegocioId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}