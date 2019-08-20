using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepCategoriasCanal<T> : CrmServiceRepository<T>, ICategoriasCanal<T>
    {
        public List<T> ListarPor(Conta canal)
        {
            var query = GetQueryExpression<T>(true);

            //query.ColumnSet = new ColumnSet("itbc_categoria");
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_canalid", ConditionOperator.Equal, canal.ID.Value));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid? canalId, Guid? unidadenegocioId, Guid? classificacaoId, Guid? subclassificacaoId, Guid? CategoriaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canalId);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_businessunit", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadenegocioId);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, classificacaoId);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_subclassificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, subclassificacaoId);

            //  Microsoft.Xrm.Sdk.Query.ConditionExpression cond5 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_categoriasdocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual, CategoriaId);
            #endregion

            #region Validações

            if (canalId.HasValue)
                query.Criteria.Conditions.Add(cond1);

            if (unidadenegocioId.HasValue)
                query.Criteria.Conditions.Add(cond2);

            if (classificacaoId.HasValue)
                query.Criteria.Conditions.Add(cond3);

            if (subclassificacaoId.HasValue)
                query.Criteria.Conditions.Add(cond4);


            ConditionExpression condAtivo = new ConditionExpression("statecode", (int)Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(condAtivo);
            //if (CategoriaId.HasValue)
            //    query.Criteria.Conditions.Add(cond5);

            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid categoriaId, Guid? canalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_categoria", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaId);
            query.Criteria.Conditions.Add(cond1);


            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canalId);
            query.Criteria.Conditions.Add(cond2);
            #endregion

            ConditionExpression condAtivo = new ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.Status.Ativo);
            query.Criteria.Conditions.Add(condAtivo);
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid categoriaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_categoria", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaId);
            query.Criteria.Conditions.Add(cond1);

            #endregion

            ConditionExpression condAtivo = new ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.Status.Ativo);
            query.Criteria.Conditions.Add(condAtivo);
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Domain.Model.Conta conta, Domain.Model.UnidadeNegocio unidade)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, conta.ID.Value);
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, unidade.ID.Value);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.CategoriaCanal.StateCode.Ativado);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public bool AtualizarStatus(Guid categoriaCanalId, int stateCode, int statusCode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_categoriasdocanal", categoriaCanalId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadenegocioId, Domain.Enum.Conta.ParticipaDoPrograma? participanteDoPrograma, bool? unidadeNegocioAtivo)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, unidadenegocioId);

            if (unidadeNegocioAtivo.HasValue)
            {
                int value = unidadeNegocioAtivo.Value ? 0 : 1;
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, value);
            }
            
            query.AddLink("account", "itbc_canalid", "accountid");

            if (participanteDoPrograma.HasValue)
            {
                query.LinkEntities[0].LinkCriteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)participanteDoPrograma.Value);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}