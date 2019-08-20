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
    public class RepPrivilegio<T> : CrmServiceRepository<T>, IPrivilegio<T>
    {
        #region Metodos
        public bool PermissaoIncluirEm(Usuario usuario, String entidade)
        {
            String nomePrivilegio = "prvCreate" + entidade;
            var queryHelper = new QueryExpression("privilege");
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, nomePrivilegio));
            queryHelper.AddLink("roleprivileges", "privilegeid", "privilegeid");
            queryHelper.LinkEntities[0].AddLink("role", "roleid", "roleid");
            queryHelper.LinkEntities[0].LinkEntities[0].AddLink("systemuserroles", "roleid", "roleid");
            queryHelper.LinkEntities[0].LinkEntities[0].LinkEntities[0].AddLink("systemuser", "systemuserid", "systemuserid");
            queryHelper.LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("systemuserid", ConditionOperator.Equal, usuario.Id));
            var bec = base.Provider.RetrieveMultiple(queryHelper);
            if (bec.Entities.Count > 0)
                return true;
            else
                return false;
        }

        public List<T> PesquisarPrivilegioPor(Usuario usuario)
        {
            var query = GetQueryExpression<Extrato>(true);
            query.AddLink("roleprivileges", "privilegeid", "privilegeid");
            query.LinkEntities[0].AddLink("role", "roleid", "roleid");
            query.LinkEntities[0].LinkEntities[0].AddLink("systemuserroles", "roleid", "roleid");
            query.LinkEntities[0].LinkEntities[0].LinkEntities[0].AddLink("systemuser", "systemuserid", "systemuserid");
            query.LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("systemuserid", ConditionOperator.Equal, usuario.Id));
            return(List<T>)this.RetrieveMultiple(query).List;
        }
        #endregion
    }
}
