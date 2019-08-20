using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepParametroGlobal<T> : CrmServiceRepository<T>, IParametroGlobal<T>
    {
        public T ObterPorCodigoTipoParametroGlobal(int Codigo)
        {
            return this.ObterPor(Codigo, null, null, null, null, null, null,null);
        }

        public List<T> ListarPor(int tipoParametro, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId, Guid? nivelPosVendaId, Guid? compromissoId, Guid? beneficioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.AddLink("itbc_tipoparametroglobal", "itbc_tipoparametroglobalid", "itbc_tipoparametroglobalid").LinkCriteria.AddCondition("itbc_codigo", ConditionOperator.Equal, tipoParametro.ToString());

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, unidadeNegocioId.ToString());

            if (classificacaoId.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", ConditionOperator.Equal, classificacaoId.ToString());

            if (categoriaId.HasValue)
                query.Criteria.AddCondition("itbc_categoriaid", ConditionOperator.Equal, categoriaId.ToString());

            if (nivelPosVendaId.HasValue)
                query.Criteria.AddCondition("itbc_nivelposvendaid", ConditionOperator.Equal, nivelPosVendaId.ToString());

            if (compromissoId.HasValue)
                query.Criteria.AddCondition("itbc_compromissoid", ConditionOperator.Equal, compromissoId.ToString());

            if (beneficioId.HasValue)
                query.Criteria.AddCondition("itbc_beneficioid", ConditionOperator.Equal, beneficioId.ToString());

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(int tipoParametro, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId, Guid? nivelPosVendaId, Guid? compromissoId, Guid? beneficioId, int? Parametrizar)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.AddLink("itbc_tipoparametroglobal", "itbc_tipoparametroglobalid", "itbc_tipoparametroglobalid")
                 .LinkCriteria.AddCondition("itbc_codigo", ConditionOperator.Equal, tipoParametro.ToString());

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, unidadeNegocioId.ToString());

            if (classificacaoId.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", ConditionOperator.Equal, classificacaoId.ToString());

            if (categoriaId.HasValue)
                query.Criteria.AddCondition("itbc_categoriaid", ConditionOperator.Equal, categoriaId.ToString());

            if (nivelPosVendaId.HasValue)
                query.Criteria.AddCondition("itbc_nivelposvendaid", ConditionOperator.Equal, nivelPosVendaId.ToString());

            if (compromissoId.HasValue)
                query.Criteria.AddCondition("itbc_compromissoid", ConditionOperator.Equal, compromissoId.ToString());

            if (beneficioId.HasValue)
                query.Criteria.AddCondition("itbc_beneficioid", ConditionOperator.Equal, beneficioId.ToString());

            if (Parametrizar.HasValue)
                query.Criteria.AddCondition("itbc_parametrizar", ConditionOperator.Equal, Parametrizar.Value.ToString());
            
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}