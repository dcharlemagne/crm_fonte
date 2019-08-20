using System;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.IRepository;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Messages;
using SDKore.Crm.Util;

namespace Intelbras.CRM2013.DAL
{
    public class RepCanalVerde<T> : CrmServiceRepository<T>, ICanalVerde<T>
    {
        public T ObterPor(Guid itbc_canais_verdeid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canais_verdeid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_canais_verdeid);
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

        public List<T> ListarPorCanal(Guid canalGuid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canalGuid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCanalTodos(Guid canalGuid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canalGuid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorSegmento(Guid segmentoGuid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_segmento", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, segmentoGuid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorFamilia(Guid familiaGuid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_familia_produto_id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, familiaGuid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterParaCalculo(Guid canalId, Guid familiaProdId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canalId);
            query.Criteria.Conditions.Add(cond1);
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_familia_produto_id", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, familiaProdId);
            query.Criteria.Conditions.Add(cond2);
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

        public Boolean AlterarStatus(Guid itbc_canais_verdeid, int status)
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
                EntityMoniker = new EntityReference("itbc_canais_verde", itbc_canais_verdeid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }


        public void InativarMultiplos(List<T> collection, int status)
        {
            ExecuteMultipleRequest request = new ExecuteMultipleRequest();
            EntityCollection listEntities = new EntityCollection();
            request.Settings = new ExecuteMultipleSettings()
            {
                ContinueOnError = true,
                ReturnResponses = false
            };

            request.Requests = new OrganizationRequestCollection();

            foreach (T entity in collection)
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
                listEntities.Entities.Add(ent);
            }

            foreach (var entity in listEntities.Entities)
            {
                request.Requests.Add(GerarRequestInativacao(entity, status));
            }

            this.Provider.Execute(request);
        }

        private SetStateRequest GerarRequestInativacao(Entity entity, int status)
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

            SetStateRequest state = new SetStateRequest();
            state.State = new OptionSetValue(stateCode);
            state.Status = new OptionSetValue(status);
            state.EntityMoniker = new EntityReference(entity.LogicalName, entity.Id);

            return state;
        }
    }
}
