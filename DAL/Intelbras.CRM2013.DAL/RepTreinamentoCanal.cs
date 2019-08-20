using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepTreinamentoCanal<T> : CrmServiceRepository<T>, ITreinamentoCanal<T>
    {

        public List<T> ListarPor(Guid? treinamentoId, Guid? canalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            if (treinamentoId.HasValue)
                query.Criteria.AddCondition("itbc_treinamento_certificacao", ConditionOperator.Equal, treinamentoId);

            if (canalId.HasValue)
                query.Criteria.AddCondition("itbc_account", ConditionOperator.Equal, canalId);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid? treinamentoId, Guid? canalId, Guid? compCanalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            if (treinamentoId.HasValue)
                query.Criteria.AddCondition("itbc_treinamento_certificacao", ConditionOperator.Equal, treinamentoId);

            if (canalId.HasValue)
                query.Criteria.AddCondition("itbc_account", ConditionOperator.Equal, canalId);

            if (compCanalId.HasValue)
                query.Criteria.AddCondition("itbc_compromissodocanalid", ConditionOperator.Equal, compCanalId);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorInativo(Guid? treinamentoId, Guid? canalId, Guid? compCanalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);

            if (treinamentoId.HasValue)
                query.Criteria.AddCondition("itbc_treinamento_certificacao", ConditionOperator.Equal, treinamentoId);

            if (canalId.HasValue)
                query.Criteria.AddCondition("itbc_account", ConditionOperator.Equal, canalId);

            if (compCanalId.HasValue)
                query.Criteria.AddCondition("itbc_compromissodocanalid", ConditionOperator.Equal, compCanalId);

            #endregion

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

        public List<T> ListarExpirados()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_datalimite", ConditionOperator.LessThan, DateTime.Now);            
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

       

        public T ObterPor(Guid treinamentoCanalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_treinamento_certificacao_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, treinamentoCanalId);
                                                                                                                 
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

        public Boolean AlterarStatus(Guid treinamentoCertCanalId, int status)
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
                EntityMoniker = new EntityReference("itbc_treinamento_certificacao_canal", treinamentoCertCanalId),
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
