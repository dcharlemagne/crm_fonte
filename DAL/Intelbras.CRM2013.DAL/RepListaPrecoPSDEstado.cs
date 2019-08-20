using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepListaPrecoPSDEstado<T> : CrmServiceRepository<T>, IListaPrecoPSDEstado<T>
    {

        public List<T> ListarPor(Guid? estado, Guid? listaPSD)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            if (estado.HasValue)
            {
                query.Criteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estado);
            }

            if (listaPSD.HasValue)
            {
                query.Criteria.AddCondition("itbc_psdidid", ConditionOperator.Equal, listaPSD);
            }

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
