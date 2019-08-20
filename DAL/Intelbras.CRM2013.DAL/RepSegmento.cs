using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepSegmento<T> : CrmServiceRepository<T>, ISegmento<T>
    {

        public T ObterPor(Guid itbc_segmentoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_segmentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_segmentoid);
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

        public T ObterPor(string itbc_codigo_segmento)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_segmento", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_codigo_segmento);
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

        public List<T> ListarPor(String codigoSegmento)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_segmento", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoSegmento);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
            
        }

        public List<T> ListarPor(Guid codigoUnidadeNegocio)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            ConditionExpression cond1 = new ConditionExpression("itbc_businessunitid", ConditionOperator.Equal, codigoUnidadeNegocio);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;

        }
        public List<T> ListarPorContaSegmento(Guid contaId)
        {
            var query = GetQueryExpression<T>(true);

            LinkEntity link = query.AddLink("itbc_contasegmento", "itbc_segmentoid", "itbc_segmentoid", JoinOperator.Inner);
            link.LinkCriteria.AddCondition(new ConditionExpression("statecode",ConditionOperator.Equal, 0));
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StatusCode.Ativo);
            link.LinkCriteria.AddCondition(new ConditionExpression("itbc_contaid", ConditionOperator.Equal, contaId));
            
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

        public List<T> ListarCanaisVerdes()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_desconto_verde_habilitado", ConditionOperator.Equal, true);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Boolean AlterarStatus(Guid itbc_segmentoid, int status)
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
                EntityMoniker = new EntityReference("itbc_segmento", itbc_segmentoid),
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
