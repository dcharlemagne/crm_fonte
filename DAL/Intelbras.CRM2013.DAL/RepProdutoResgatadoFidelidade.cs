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
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoResgatadoFidelidade<T> : CrmServiceRepository<T>, IProdutoResgatadoFidelidade<T>
    {
        public void AlterarStatus(Guid id, int statuscode, bool stateActive)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("new_produto_resgatado_fidelidade", id),
                State = new OptionSetValue(stateActive ? 0 : 1),
                Status = new OptionSetValue(statuscode)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public List<T> ListarAtivosPor(Domain.Model.Resgate resgate, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_resgate_premiosid", ConditionOperator.Equal, resgate.ID.Value));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));

            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }
    }
}
