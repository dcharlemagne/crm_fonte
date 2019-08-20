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
    public class RepDefeitoOcorrenciaCliente<T> : CrmServiceRepository<T>, IDefeitoOcorrenciaCliente<T>
    {
        public List<T> ListarPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public void ExcluirPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.Id));
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            foreach (var item in result)
            {
                Delete(item.Id);
            }
        }

        public List<T> ListarDefeitosPor(string nome)
        {
            //INCRIVELMENTE NÃO É USADO O PARAMETRO NOME
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("new_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}