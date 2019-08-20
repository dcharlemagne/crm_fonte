using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepFila<T> : CrmServiceRepository<T>, IFila<T>
    {
        public List<T> ListarPor(Guid FilaId)
        {
            var query = GetQueryExpression<T>(true);
            ConditionExpression cond1 = new ConditionExpression("queueid", ConditionOperator.Equal, FilaId);
            query.Criteria.Conditions.Add(cond1);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid queueid)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("queueid", ConditionOperator.Equal, queueid));
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterFilaPublicaPor(Guid usuarioId)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("primaryuserid", ConditionOperator.Equal, usuarioId));
            query.Criteria.Conditions.Add(new ConditionExpression("queuetypecode", ConditionOperator.Equal, "3"));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(string nome)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, nome));
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public bool TargetQueuedTask(Domain.Model.Tarefa tarefa, Guid filaDestino)
        {
            //var target = new TargetQueuedTask();
            //target.EntityId = tarefa.ID.Value;

            ////Identifica o SourceQueueId conforme usuário
            //WhoAmIRequest userRequest = new WhoAmIRequest();
            //WhoAmIResponse user = (WhoAmIResponse)this.Execute(userRequest);

            //QueryByAttribute query = new QueryByAttribute();
            //query.ColumnSet = new AllColumns();
            //query.EntityName = EntityName.queue.ToString();
            //query.Attributes = new string[] { "primaryuserid", "queuetypecode" };
            //query.Values = new string[] { user.UserId.ToString(), "3" };
            //BusinessEntityCollection results = this.Provider.RetrieveMultiple(query);
            //queue wipQueue = (queue)results.BusinessEntities[0];

            ////Monta atributos da fila e adiciona na fila
            //var request = new Microsoft.Crm.SdkTypeProxy.RouteRequest();
            //request.Target = target;
            //request.RouteType = Microsoft.Crm.Sdk.RouteType.Queue;
            //request.EndpointId = filaDestino;
            //request.SourceQueueId = wipQueue.queueid.Value;
            //RouteResponse routed = (RouteResponse)this.Execute(request);

            //return (routed != null);
            return true;
        }


    }
}
