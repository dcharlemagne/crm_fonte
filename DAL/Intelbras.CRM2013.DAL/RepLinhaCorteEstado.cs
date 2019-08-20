using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepLinhaCorteEstado<T> : CrmServiceRepository<T>, ILinhaCorteEstado<T>
    {

        public List<T> ListarPor(Guid linhaCorteDistId, Guid? estadoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_linhadecorteid", ConditionOperator.Equal, linhaCorteDistId.ToString());

            if (estadoId.HasValue)
                query.Criteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId);
                
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid estadoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId);
            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid linhaCorteDistId)
        {
            throw new NotImplementedException();
        }
    }
}
