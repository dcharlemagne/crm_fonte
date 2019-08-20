using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepPerfil<T> : CrmServiceRepository<T>, IPerfil<T>
    {
        public List<T> ListarPor(Guid? classificacaoId, Guid? unidadenegocioId, Guid? categoriaId, Boolean? Exclusividade)
        {
            var query = GetQueryExpression<T>(true);

            if (classificacaoId.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, classificacaoId);

            if (unidadenegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadenegocioId);

            if (categoriaId.HasValue)
                query.Criteria.AddCondition("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaId);

            if (Exclusividade.HasValue)
                query.Criteria.AddCondition("itbc_exclusividade", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Exclusividade);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorConfigurado(Guid? classificacaoId, Guid? unidadenegocioId, Guid? categoriaId, Boolean? Exclusividade)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, classificacaoId);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadenegocioId);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaId);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_exclusividade", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Exclusividade);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond5 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_status", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.Pefil.Status.Configurado);

            #endregion

            #region Validações

            if (classificacaoId.HasValue)
                query.Criteria.Conditions.Add(cond1);

            if (unidadenegocioId.HasValue)
                query.Criteria.Conditions.Add(cond2);

            if (categoriaId.HasValue)
                query.Criteria.Conditions.Add(cond3);

            if (Exclusividade.HasValue)
                query.Criteria.Conditions.Add(cond4);

            query.Criteria.Conditions.Add(cond5);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }


        public T ObterPor(Guid PerfilID)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_perfilid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, PerfilID);
            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
