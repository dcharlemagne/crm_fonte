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
    public class RepAutorizacaoDePostagem<T> : CrmServiceRepository<T>, IAutorizacaoDePostagem<T>
    {
        public List<T> PesquisarAutorizacaoPostagemCorreiosPor(string eTiketOuNumeroObjeto)
        {
            //NÃO IMPLANTADO
            var query = GetQueryExpression<T>(true);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> PesquisarAutorizacaoPostagemCorreiosPor(Ocorrencia ocorrencia)
        {
            //NÃO IMPLANTADO
            var query = GetQueryExpression<T>(true);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> PesquisarAutorizacaoPostagemCorreiosPor(Domain.Model.Conta cliente)
        {
            //NÃO IMPLANTADO
            var query = GetQueryExpression<T>(true);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodosAutorizacaoPostagemCorreios()
        {
            var query = GetQueryExpression<T>(true);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodosAutorizacaoPostagemCorreios(int count, int page)
        {
            var query = GetQueryExpression<T>(true);
            query.PageInfo = new PagingInfo()
            {
                Count = count,
                PageNumber = page
            };
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContratoAutorizacaoPostagemCorreiosPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_assunto_unidadeid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.AssuntoUnidade.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_situacao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1));
            query.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("new_assunto_unidadeid", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
