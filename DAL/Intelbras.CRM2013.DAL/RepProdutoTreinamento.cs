using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoTreinamento<T> : CrmServiceRepository<T>, IProdutoTreinamento<T>
    {

        public List<T> ListarPor(Guid treinamentoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_treinamentoecertfid", ConditionOperator.Equal, treinamentoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }



        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorProduto(Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid produtoTreinamentoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_prodportreinecertifid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoTreinamentoId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorTreinamento(Guid TreinamentoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_treinamentoecertfid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TreinamentoId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AlterarStatus(Guid produtoTreinamentoId, int status)
        {

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_prodportreinecertif", produtoTreinamentoId),
                State = new OptionSetValue(0),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

         public List<T> ListarPorTreinamento(Guid treinamentoId, Guid unidadeNegocioId)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

             strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                  ");                                    
             strFetchXml.Append("     <entity name='itbc_prodportreinecertif'>                                                                          ");
             strFetchXml.Append("       <attribute name='itbc_prodportreinecertifid' />                                                                 ");
             strFetchXml.Append("       <attribute name='itbc_name' />                                                                                  ");
             strFetchXml.Append("       <attribute name='createdon' />                                                                                  ");
             strFetchXml.Append("       <attribute name='itbc_nummindeprofissionais' />                                                                 ");
             strFetchXml.Append("       <order attribute='itbc_name' descending='false' />                                                              ");
             strFetchXml.Append("       <filter type='and'>                                                                                             ");
             strFetchXml.AppendFormat("         <condition attribute='itbc_treinamentoecertfid' operator='eq' uitype='itbc_treinamcertif' value='{0}' />", treinamentoId);
             strFetchXml.Append("       </filter>                                                                                                       ");
             strFetchXml.Append("       <link-entity name='product' from='productid' to='itbc_productid' alias='ad'>                                    ");
             strFetchXml.Append("         <filter type='and'>                                                                                           ");
             strFetchXml.AppendFormat("           <condition attribute='itbc_businessunitid' operator='eq' uitype='businessunit' value='{0}' />          ", unidadeNegocioId);
             strFetchXml.Append("         </filter>                                                                                                     ");
             strFetchXml.Append("       </link-entity>                                                                                                  ");
             strFetchXml.Append("     </entity>                                                                                                         ");
             strFetchXml.Append("   </fetch>                                                                                                                                                        ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }
    }
}