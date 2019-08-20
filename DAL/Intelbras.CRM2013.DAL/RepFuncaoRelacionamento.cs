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
    public class RepFuncaoRelacionamento<T> : CrmServiceRepository<T>, IRelacionamento<T>
    {
        public List<T> ObterPor(Domain.Model.Conta cliente, Guid funcao)
        {
            var query = GetQueryExpression<T>(true);

            #region Filtro1
            //Pesquiso por Participante1 e Função 2
            FilterExpression filter1 = new FilterExpression();

            ConditionExpression condition1 = new ConditionExpression();
            condition1.AttributeName = "customerid";
            condition1.Operator = ConditionOperator.Equal;
            condition1.Values.Add(cliente.Id);

            filter1.FilterOperator = LogicalOperator.And;

            ConditionExpression condition2 = new ConditionExpression();
            condition2.AttributeName = "partnerroleid";
            condition2.Operator = ConditionOperator.Equal;
            condition2.Values.Add(funcao);

            filter1.Conditions.Add(condition1);
            filter1.Conditions.Add(condition2);

            #endregion

            #region Filtro2
            //Pesquiso por Participante2 e Função 1
            FilterExpression filter2 = new FilterExpression();

            ConditionExpression condition3 = new ConditionExpression();
            condition3.AttributeName = "partnerid";
            condition3.Operator = ConditionOperator.Equal;
            condition3.Values.Add(cliente.Id);

            filter2.Conditions.Add(condition1);


            filter2.FilterOperator = LogicalOperator.And;

            ConditionExpression condition4 = new ConditionExpression();
            condition4.AttributeName = "customerroleid";
            condition4.Operator = ConditionOperator.Equal;
            condition4.Values.Add(funcao);

            filter2.Conditions.Add(condition3);
            filter2.Conditions.Add(condition4);

            #endregion

            //Filtro1 OR Filtro2
            query.Criteria = new FilterExpression();
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.Filters.Add(filter1);
            query.Criteria.Filters.Add(filter2);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Guid ObterFuncaoPor(string nome)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.ColumnSet.AddColumn("relationshiproleid");
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, nome));
            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);
            if (colecao.Entities.Count > 0)
                if (colecao.Entities[0].Contains("relationshiproleid"))
                    return ((EntityReference)colecao.Entities[0]["relationshiproleid.Value"]).Id;

            return Guid.Empty;
        }

        public List<T> ListarTodas()
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
