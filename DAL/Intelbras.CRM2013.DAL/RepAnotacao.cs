using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepAnotacao<T> : CrmServiceRepository<T>, IAnotacao<T>
    {
        public List<T> ListarPor(Guid objectId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("objectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, objectId));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid objectId, bool comAnexo)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("objectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, objectId));
            if (comAnexo)
            {
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("isdocument", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, comAnexo));
            }
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorTipoArquivo(string objectId, string tipoArquivo)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("objectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, objectId));
            if (!string.IsNullOrEmpty(tipoArquivo))
            {
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("filename", Microsoft.Xrm.Sdk.Query.ConditionOperator.EndsWith, tipoArquivo));
            }
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("filesize", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, 0));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
