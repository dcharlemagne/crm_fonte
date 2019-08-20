using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepPremio<T> : CrmServiceRepository<T>, IPremio<T>
    {
        /// <summary>
        /// Método que retorna todos os premios ativos cadastrados
        /// </summary>
        /// <returns>Lista de Premios</returns>
        public List<T> ListarPremios()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
