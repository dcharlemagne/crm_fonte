using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepPoliticaComercialXConta<T> : CrmServiceRepository<T>, IPoliticaComercialXConta<T>
    {

        public List<T> ListarPor(Guid? PoliticaComercialId, Guid? ContaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            if (PoliticaComercialId.HasValue)
                query.Criteria.AddCondition("itbc_politicacomercialid", ConditionOperator.Equal, PoliticaComercialId.ToString());

            if (ContaId.HasValue)
                query.Criteria.AddCondition("accountid", ConditionOperator.Equal, ContaId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
