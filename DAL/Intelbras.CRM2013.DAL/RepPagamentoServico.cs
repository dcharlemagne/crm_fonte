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
    public class RepPagamentoServico<T> : CrmServiceRepository<T>, IPagamentoServico<T>
    {
        public List<T> ListarPor(Ocorrencia ocorrencia)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_pagamento_servico_ocorrenciaid", ConditionOperator.Equal, ocorrencia.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }
    }
}