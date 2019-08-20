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
    public class RepClienteParticipanteDoContrato<T> : CrmServiceRepository<T>, IClienteParticipanteDoContrato<T>
    {
        public List<T> ListarPor(Contrato contrato, Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            if (cliente != null) query.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0)); //busca os clientes ativos

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Contrato contrato)
        {
            return ListarPor(contrato, null);
        }

        public void DeleteAll(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;

            foreach (var item in result)
                this.Delete(item.Id);
        }

    }
}
