﻿using System;
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
    public class ReCompromisso<T> : CrmServiceRepository<T>, ICompromisso<T>
    {

        public List<T> ListarPorReferenteA(Guid Id)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("regardingobjectid", ConditionOperator.Equal, Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
