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
using SDKore.DomainModel;

namespace Intelbras.CRM2013.DAL
{
    public class RepResgate<T> : CrmServiceRepository<T>, IResgate<T>
    {
        public List<T> ObterPorRevenda(Guid revenda)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("contact", "new_participanteid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, revenda));
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterPorContato(Guid contatoId, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("contact", "new_participanteid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.Equal, contatoId));
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterPorResgateParceiro(Domain.Enum.Resgate.RazaoDoStatus statuscode, int count, int pageNumber, string cookie)
        {
            var query = GetQueryExpression<T>(true);
            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = count;
            query.PageInfo.PageNumber = pageNumber;
            query.PageInfo.PagingCookie = cookie;
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)statuscode));
            query.Criteria.Conditions.Add(new ConditionExpression("new_codigo_ordem_pedido", ConditionOperator.NotNull));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public void AlterarStatus(Guid id, int statuscode, bool stateActive)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("new_resgate_premio_fidelidade", id),
                State = new OptionSetValue(stateActive ? 0 : 1),
                Status = new OptionSetValue(statuscode)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }
    }
}
