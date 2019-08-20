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
    public class RepIntervencao<T> : CrmServiceRepository<T>, IIntervencao<T>
    {
        public T EmAnalisePor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.IntervencaoTecnicaEnum.StatusCode.AguardandoAnalise));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarPor(Ocorrencia ocorrencia)
        {
            return ListarPorOcorrenciaId(ocorrencia.Id);
        }

        public List<T> ListarPorOcorrenciaId(Guid ocorrenciaId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrenciaId));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarPor(Ocorrencia ocorrencia, int status)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, status));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public bool OcorrenciaTemIntervencaoAtiva(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1)); //Aguardando Análise
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count > 0)
                return true;
            else
                return false;
        }
    }
}