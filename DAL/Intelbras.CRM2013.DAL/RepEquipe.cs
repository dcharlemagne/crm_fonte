using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    class RepEquipes<T> : CrmServiceRepository<T>, IEquipe<T>
    {
        public List<T> ListarPor(Guid equipeId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("teamid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, equipeId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorNome(String nomeEquipe)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("name", Microsoft.Xrm.Sdk.Query.ConditionOperator.BeginsWith, nomeEquipe);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid equipeId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("teamid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, equipeId);
            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AdicionarPerfilAcesso(Guid equipeId,Guid perfilId)
        {
            this.Provider.Associate(
                   "team",
                   equipeId,
               new Relationship("teamroles_association"),
               new EntityReferenceCollection() { new EntityReference("role", perfilId) }
               );
        }

        public void RemoverPerfilAcesso(Guid equipeId, Guid perfilId)
        {
            this.Provider.Disassociate(
                   "team",
                   equipeId,
               new Relationship("teamroles_association"),
               new EntityReferenceCollection() { new EntityReference("role", perfilId) }
               );
        }
    }
}
