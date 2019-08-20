using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepFamiliaProduto<T> : CrmServiceRepository<T>, IFamiliaProduto<T>
    {
        public T ObterPor(Guid itbc_familiadeprodutoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_famildeprodid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_familiadeprodutoid);
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
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_familia", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoFamiliaProduto);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
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
                query.Criteria.AddCondition(new ConditionExpression("itbc_codigo_familia", ConditionOperator.GreaterEqual, codigoFamiliaInicial));
                query.Criteria.AddCondition(new ConditionExpression("itbc_codigo_familia", ConditionOperator.LessEqual, codigoFamiliaFinal));
            }
            else
                query.Criteria.AddCondition(new ConditionExpression("itbc_codigo_familia", ConditionOperator.GreaterEqual, codigoFamiliaInicial));
                
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
 


        }

        public List<T> ListarPor(Guid unidadenegocioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_segmento", "itbc_segmentoid", "itbc_segmentoid").LinkCriteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadenegocioId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorSegmento(Guid segmentoId, bool filtrarCanaisVerdes, Guid? canalId, string[] notInList)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_segmento", "itbc_segmentoid", "itbc_segmentoid").LinkCriteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);

            if (filtrarCanaisVerdes)
            {
                query.Criteria.AddCondition("itbc_desconto_verde_habilitado", ConditionOperator.Equal, true);
                if(notInList != null && notInList.GetLength(0) > 0)
                {
                    query.Criteria.AddCondition("itbc_famildeprodid", ConditionOperator.NotIn, notInList);
                    query.Criteria.AddCondition(new ConditionExpression("itbc_famildeprodid", ConditionOperator.NotIn, notInList));
                }
            }
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Boolean AlterarStatus(Guid familiaProduto, int status)
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
                EntityMoniker = new EntityReference("itbc_famildeprod", familiaProduto),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public T ObterPor(string codigofamilia)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_codigo_familia", ConditionOperator.Equal, codigofamilia);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_codigo_familia", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

    }
}