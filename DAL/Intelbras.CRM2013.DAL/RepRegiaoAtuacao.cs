using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepRegiaoAtuacao<T> : CrmServiceRepository<T>, IRegiaoAtuacao<T>
    {
        public List<T> ListarPor(Guid accountid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, accountid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public T ObterPor(Guid municipioId, Guid accountid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, accountid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_municipioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, municipioId);
            query.Criteria.Conditions.Add(cond2);

            #endregion
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        
        public void CreateManytoMany(Guid municipioId, Guid accountid )
        {
            CreateManyToManyRequest create = new CreateManyToManyRequest();

            CreateManyToManyRequest createManyToManyRelationshipRequest =
                            new CreateManyToManyRequest
                            {
                                IntersectEntitySchemaName = "itbc_account_itbc_municipios",
                                ManyToManyRelationship = new ManyToManyRelationshipMetadata
                                {
                                    SchemaName = "new_accounts_campaigns",
                                    Entity1LogicalName = "account",
                                    Entity1AssociatedMenuConfiguration =
                                    new AssociatedMenuConfiguration
                                    {
                                        Behavior = AssociatedMenuBehavior.UseLabel,
                                        Group = AssociatedMenuGroup.Details,
                                        Label = new Label("Conta", 1033),
                                        Order = 10000
                                    },
                                    Entity2LogicalName = "itbc_municipio",
                                    Entity2AssociatedMenuConfiguration =
                                    new AssociatedMenuConfiguration
                                    {
                                        Behavior = AssociatedMenuBehavior.UseLabel,
                                        Group = AssociatedMenuGroup.Details,
                                        Label = new Label("Municipio", 1033),
                                        Order = 10000
                                    }
                                }
                            };
        
        }

    }
}
