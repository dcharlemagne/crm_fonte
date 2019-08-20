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
    public class RepRelacionamentoCliente<T> : CrmServiceRepository<T>, IRelacionamentoCliente<T>
    {
        public List<T> ListarParticipantePor(Guid participanteId, Guid funcaoRelacionamentoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("partneridname", OrderType.Ascending));
            query.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, participanteId));
            query.Criteria.Conditions.Add(new ConditionExpression("customerroleid", ConditionOperator.Equal, funcaoRelacionamentoId));
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }


        public List<T> ListarParticipantePorTopUm(Guid participanteId, Guid funcaoRelacionamentoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, participanteId));
            query.Criteria.Conditions.Add(new ConditionExpression("customerroleid", ConditionOperator.Equal, funcaoRelacionamentoId));
            query.PageInfo = new PagingInfo { Count = 1, PageNumber = 1 };
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }


        public List<T> ObterPor(Guid participanteUmId, Guid funcaoRelacionamentoUmId, Guid participanteDoisId, Guid funcaoRelacionamentoDoisId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, participanteUmId));
            query.Criteria.Conditions.Add(new ConditionExpression("customerroleid", ConditionOperator.Equal, funcaoRelacionamentoUmId));
            query.Criteria.Conditions.Add(new ConditionExpression("partnerid", ConditionOperator.Equal, participanteDoisId));
            query.Criteria.Conditions.Add(new ConditionExpression("partnerroleid", ConditionOperator.Equal, funcaoRelacionamentoDoisId));

            query.PageInfo = new PagingInfo { Count = 1, PageNumber = 1 };
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ListarParticipantePor(Guid participanteId, Guid funcaoRelacionamentoId, Guid funcaoparceiroId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, participanteId));
            query.Criteria.Conditions.Add(new ConditionExpression("customerroleid", ConditionOperator.Equal, funcaoRelacionamentoId));
            if (funcaoparceiroId != Guid.Empty)
                query.Criteria.Conditions.Add(new ConditionExpression("partnerroleid", ConditionOperator.Equal, funcaoparceiroId));

            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

    }
}
