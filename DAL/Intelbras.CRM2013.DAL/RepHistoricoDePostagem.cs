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
     [Serializable]
    public class RepHistoricoDePostagem<T> : CrmServiceRepository<T>, IHistoricoDePostagem<T>
    {
        public List<T> PesquisarHistoricoDePostagemPor(string eTiketOuNumeroObjeto)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("incident", "new_ocorrenciais", "incidentid");
            query.LinkEntities[0].LinkCriteria.FilterOperator = LogicalOperator.Or;
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_numero_eticket", ConditionOperator.Equal, eTiketOuNumeroObjeto));
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_numero_objeto", ConditionOperator.Equal, eTiketOuNumeroObjeto));
            query.Orders.Add(new OrderExpression("new_data_hora", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> PesquisarHistoricoDePostagemPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("incident", "new_ocorrenciais", "incidentid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("incidentid", ConditionOperator.Equal, ocorrencia.Id));
            query.Orders.Add(new OrderExpression("new_data_hora", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> PesquisarHistoricoDePostagemPor(Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("incident", "new_ocorrenciais", "incidentid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("customerid", ConditionOperator.Equal, cliente.Id));
            query.Orders.Add(new OrderExpression("new_data_hora", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodosHistoricoDePostagem()
        {
            var query = GetQueryExpression<T>(true);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodosHistoricoDePostagem(int count, int page)
        {
            var query = GetQueryExpression<T>(true);
            query.PageInfo = new PagingInfo()
            {
                Count = count,
                PageNumber = page
            };
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T PesquisarHistoricoOcorrencia(Ocorrencia ocorrencia, string numeroObjeto, string tipoEvento, string statusEvento, DateTime DataHora)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_tipo", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, tipoEvento));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_codigo_situacao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, statusEvento));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_hora", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, DataHora));
            query.AddLink("incident", "new_ocorrenciais", "incidentid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("incidentid", ConditionOperator.Equal, ocorrencia.Id));
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_numero_objeto", ConditionOperator.Equal, numeroObjeto));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
    }
}
