using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepPoliticaComercialXEstado<T> : CrmServiceRepository<T>, IPoliticaComercialXEstado<T>
    {

        public List<T> ListarPor(Guid? PoliticaComercialId, Guid? EstadoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            if (PoliticaComercialId != null && PoliticaComercialId.HasValue)
                query.Criteria.AddCondition("itbc_politicacomercialid", ConditionOperator.Equal, PoliticaComercialId.ToString());

            if (EstadoId != null && EstadoId.HasValue)
                query.Criteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, EstadoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
