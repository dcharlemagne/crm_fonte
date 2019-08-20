using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Intelbras.CRM2013.DAL
{
    public class RepConexao<T> : CrmServiceRepository<T>, IConexao<T>
    {

        public List<T> ListarPor(Guid? funcaoAte, Guid? conectadoA, Guid? funcaoDe, Guid? conectadoDe)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("record2roleid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, funcaoAte);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("record2id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, conectadoA);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("record1roleid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, funcaoDe);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("record1id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, conectadoDe);

            #endregion

            #region Validações

            if (funcaoAte.HasValue)
                query.Criteria.Conditions.Add(cond1);

            if (conectadoA.HasValue)
                query.Criteria.Conditions.Add(cond2);

            if (funcaoDe.HasValue)
                query.Criteria.Conditions.Add(cond3);

            if (conectadoDe.HasValue)
                query.Criteria.Conditions.Add(cond4);

            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid conexaoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("connectionid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, conexaoId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AlterarStatus(Guid conexaoId, int state, int status, Guid usuarioId)
        {

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_receitapadrao", conexaoId),
                State = new OptionSetValue(state),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request, usuarioId);
        }

        public Dictionary<int, string> GetOptionSetValues(string entidade, string campo)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            RetrieveAttributeRequest request = new RetrieveAttributeRequest();
            request.EntityLogicalName = entidade;
            request.LogicalName = campo;
            request.RetrieveAsIfPublished = true;
            RetrieveAttributeResponse response = (RetrieveAttributeResponse)this.Provider.Execute(request);
            if (response.Results.Count > 0)
            {
                PicklistAttributeMetadata picklist = (PicklistAttributeMetadata)response.AttributeMetadata;
                foreach (var item in picklist.OptionSet.Options)
                    dic.Add(item.Value.Value, item.Label.LocalizedLabels[0].Label);
            }
            return dic;
        }
    }
}