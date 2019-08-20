using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{

    public class RepTipodeParametroGlobal<T> : CrmServiceRepository<T>, ITipodeParametroGlobal<T>
    {
        public T ObterpoCodigo(int codigo)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_codigo", ConditionOperator.Equal, codigo);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public T ObterPorNomeParametro(string nomeTipoParametro)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_name", ConditionOperator.Equal, nomeTipoParametro);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid id)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_tipoparametroglobalid", ConditionOperator.Equal, id);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}

