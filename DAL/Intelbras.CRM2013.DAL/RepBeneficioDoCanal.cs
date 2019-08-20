using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.Crm.Util;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepBeneficioDoCanal<T> : CrmServiceRepository<T>, IBeneficioDoCanal<T>
    {
        public List<T> ListarPor(Guid canalId, Guid unidadeNegocioId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(List<Guid> BeneficiosProg, Guid Canal, Guid UnidadeNegocios,Guid? Categoria)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_beneficioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, BeneficiosProg);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Canal);
            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, UnidadeNegocios);
            query.Criteria.Conditions.Add(cond3);

            if (Categoria.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Categoria);
                query.Criteria.Conditions.Add(cond4);
            }
            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPorConta(Guid canalId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorContaUnidadeNegocio(Guid canalId, Guid? unidadeNegocioId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.Value);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorContaUnidadeNegocioESaldo(Guid canalId, Guid unidadeNegocioId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 1);            

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorContaUnidadeNegocioPlanilha()
        {
            var query = GetQueryExpression<T>(true);
            //Para listar todos nao suspensos
            query.Criteria.AddCondition("itbc_statusbeneficiosid", ConditionOperator.NotEqual, new Guid("E1654A30-75ED-E311-9407-00155D013D38"));


            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);


            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPor(Guid BeneficioDoCanalID)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_benefdocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, BeneficioDoCanalID);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_beneficioid, Guid itbc_businessunitid, Guid itbc_canalid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_beneficioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_beneficioid);
            
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_businessunitid);
            
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_canalid);

            query.Criteria.Conditions.Add(cond1);
            query.Criteria.Conditions.Add(cond2);
            query.Criteria.Conditions.Add(cond3);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid BeneficioDoCanalID)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_benefdocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, BeneficioDoCanalID);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        
        public List<T> ListarPorBeneficioProg(Guid beneficioprogId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_beneficioid", ConditionOperator.Equal, beneficioprogId);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Boolean AtualizarStatus(Guid beneficioID, int stateCode, int statusCode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_benefdocanal", beneficioID),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public List<T> ListarComSaldoAtivos(Guid canalId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Conta.StateCode.Ativo);
            query.Criteria.AddCondition("itbc_verbadisponivel", ConditionOperator.GreaterThan, (decimal)0);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
