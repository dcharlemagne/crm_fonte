using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using SDKore.Crm.Util;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepTarefa<T> : CrmServiceRepository<T>, ITarefa<T>
    {
        public void Update(T entity)
        {
            var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
            
            // Utilizar Mudar status para alterar o status da tarefa
            if (ent.Attributes.Contains("statecode"))
                ent.Attributes.Remove("statecode");

            // Utilizar Mudar status para alterar o status da tarefa
            if (ent.Attributes.Contains("statuscode"))
                ent.Attributes.Remove("statuscode");

            this.Provider.Update(ent);
        }

        public List<T> ListarPor(Guid TarefaId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("activityid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TarefaId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_ordem", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorReferenteA(Guid referenteA)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("regardingobjectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, referenteA);
            query.Criteria.Conditions.Add(cond1);


            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_ordem", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorReferenteA(Guid referenteA)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("regardingobjectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, referenteA);

            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPorReferenteAAtivo(Guid referenteA)
        {
            var query = GetQueryExpression<T>(true);

            ConditionExpression cond1 = new ConditionExpression("regardingobjectid", ConditionOperator.Equal, referenteA);
            query.Criteria.Conditions.Add(cond1);

            ConditionExpression cond2 = new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.Tarefa.StateCode.Ativo);
            query.Criteria.Conditions.Add(cond2);

            OrderExpression ord1 = new OrderExpression("itbc_ordem", OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid activityid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("activityid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, activityid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("subject", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid activityid, int status, int? chtun)
        {

            int statuscode = 0;
            //int stateCode;
            if (status == 0)
            {
                //Aberto
                statuscode = 2;
            }
            else if (status == 1)
            {
                //Concluido
                statuscode = 5;
            }
            else if (status == 2)
            {
                //Cancelado
                statuscode = 6;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("task", activityid),
                State = new OptionSetValue(status),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);


            if (resp != null)
                return true;

            return false;

        }
                   
        public List<T> ListarPor(Guid referenteA, Guid tipoAtividade, DateTime? dtInicial, DateTime? dtFim, int? situacao)
        {
            var query = GetQueryExpression<T>(true);
            #region CRiteria
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("regardingobjectid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, referenteA);
            query.Criteria.Conditions.Add(cond2);

            if (!tipoAtividade.Equals(Guid.Empty))
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tipoatividadeid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, tipoAtividade);
                query.Criteria.Conditions.Add(cond1);
            }

            if (dtInicial.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, dtInicial);
                query.Criteria.Conditions.Add(cond3);
            }

            if (dtFim.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual, dtFim);
                query.Criteria.Conditions.Add(cond4);
            }
            if(situacao.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond5 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, situacao.Value);
                query.Criteria.Conditions.Add(cond5);
            }

            #endregion


            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("overriddencreatedon", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> TarefasPorSolicitacao(Guid solicId)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();
        
            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                                           ");
            strFetchXml.Append("  <entity name='task'>                                                                                                                          ");
            strFetchXml.Append("    <attribute name='subject' />                                                                                                                ");
            strFetchXml.Append("    <attribute name='statecode' />                                                                                                              ");
            strFetchXml.Append("    <attribute name='prioritycode' />                                                                                                           ");
            strFetchXml.Append("    <attribute name='scheduledend' />                                                                                                           ");
            strFetchXml.Append("    <attribute name='createdby' />                                                                                                              ");
            strFetchXml.Append("    <attribute name='regardingobjectid' />                                                                                                      ");
            strFetchXml.Append("    <attribute name='activityid' />                                                                                                             ");
            strFetchXml.Append("    <order attribute='subject' descending='false' />                                                                                            ");
            strFetchXml.Append("    <filter type='and'>                                                                                                                         ");
            strFetchXml.AppendFormat("      <condition attribute='itbc_tipoatividadeid' operator='eq' uitype='itbc_tipoatividade' value='{0}' />                                 ", solicId);
            strFetchXml.Append("      <condition attribute='statecode' operator='eq' value='0' />                                                                               ");
            strFetchXml.Append("    </filter>                                                                                                                                   ");
            strFetchXml.Append("    <link-entity name='itbc_solicitacaodebeneficio' from='itbc_solicitacaodebeneficioid' to='regardingobjectid' alias='av'></link-entity>       ");
            strFetchXml.Append("  </entity>                                                                                                                                     ");
            strFetchXml.Append("</fetch>                                                                                                                                        ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }
    }
}
