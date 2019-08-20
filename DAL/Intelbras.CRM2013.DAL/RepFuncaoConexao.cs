using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepFuncaoConexao<T> : CrmServiceRepository<T>, IFuncaoConexao<T>
    {
        public List<T> ListarPor(int? categoria)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            if(categoria.HasValue)
            query.Criteria.AddCondition("category", ConditionOperator.Equal, categoria);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorNome(string nomeFuncao)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("name", ConditionOperator.Like, nomeFuncao);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
