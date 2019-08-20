using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain;

namespace Intelbras.CRM2013.DAL
{
    public class RepSolicitacaoBeneficio<T> : CrmServiceRepository<T>, ISolicitacaoBeneficio<T>
    {
        public T ObterPor(Guid itbc_solicitacaodebeneficioid, int? stateCode)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_solicitacaodebeneficioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_solicitacaodebeneficioid);
            query.Criteria.Conditions.Add(cond1);

            if (stateCode.HasValue)
            {
                //Status Ativo
                query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, stateCode.Value);
            }

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(ProdutosdaSolicitacao produtoSolicitacao, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            query.AddLink("itbc_produtosdasolicitacao", "itbc_solicitacaodebeneficioid", "itbc_solicitacaodebeneficioid");
            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_produtosdasolicitacaoid", ConditionOperator.Equal, produtoSolicitacao.ID.Value);

            var colecao = RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public List<T> ObterPorStatusPrice(int status)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.SolicitacaoBeneficio.State.Ativo);
            query.Criteria.AddCondition("itbc_statuscalculopriceprotection", ConditionOperator.Equal, status);

            var colecao = RetrieveMultiple(query);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorBeneficioCanal(Guid beneficiocanalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.SolicitacaoBeneficio.State.Ativo);
            query.Criteria.AddCondition("itbc_benefdocanal", ConditionOperator.Equal, beneficiocanalId);

            #endregion

            #region Ordenações
            query.Orders.Add(new OrderExpression("createdon", OrderType.Ascending));
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorBeneficioCanalEAjusteSaldo(Guid beneficiocanalId, Boolean ajusteSaldo)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.SolicitacaoBeneficio.State.Ativo);
            query.Criteria.AddCondition("itbc_ajustesaldo", ConditionOperator.Equal, ajusteSaldo);
            query.Criteria.AddCondition("itbc_benefdocanal", ConditionOperator.Equal, beneficiocanalId);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorBeneficioCanalEStatus(Guid beneficiocanalId, Guid beneficioPrograma, int status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_status", ConditionOperator.Equal, status);
            query.Criteria.AddCondition("itbc_benefdocanal", ConditionOperator.Equal, beneficiocanalId);
            query.Criteria.AddCondition("itbc_beneficiodoprograma", ConditionOperator.Equal, beneficioPrograma);

            #endregion

            #region Ordenações
            query.Orders.Add(new OrderExpression("createdon", OrderType.Ascending));
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarAprovado(Guid beneficiocanalId, Guid canalId, Guid unidadenegocioId, Guid beneficioprograma)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_accountid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_beneficiodoprograma", ConditionOperator.Equal, beneficioprograma);
            query.Criteria.AddCondition("itbc_status", ConditionOperator.Equal, "993520001");
            query.Criteria.AddCondition("itbc_benefdocanal", ConditionOperator.Equal, beneficiocanalId);
            #endregion

            #region Ordenações
            query.Orders.Add(new OrderExpression("createdon", OrderType.Ascending));
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Boolean AlterarStatus(Guid id, int stateCode, int statusCode)
        {
            SetStateResponse resp = null;

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_solicitacaodebeneficio", id),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public List<T> ListarDiferenteDeCanceladaEPagAnteriorAAtual()
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();


            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>           ");
            strFetchXml.Append("  <entity name='itbc_solicitacaodebeneficio'>                                                   ");
            strFetchXml.Append("    <attribute name='itbc_solicitacaodebeneficioid' />                                          ");
            strFetchXml.Append("    <attribute name='itbc_name' />                                                              ");
            strFetchXml.Append("    <attribute name='createdon' />                                                              ");
            strFetchXml.Append("    <order attribute='itbc_name' descending='false' />                                          ");
            strFetchXml.Append("    <filter type='and'>                                                                         ");
            strFetchXml.Append("      <condition attribute='itbc_status' operator='not-in'>                                     ");
            strFetchXml.Append("        <value>" + (int)Intelbras.CRM2013.Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado + "</value>                                                                ");
            strFetchXml.Append("        <value>" + (int)Intelbras.CRM2013.Domain.Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada + "</value>                                                                ");
            strFetchXml.Append("      </condition>                                                                              ");
            strFetchXml.Append("      <condition attribute='itbc_datavalidade' operator='on-or-before' value='" + DateTime.Now.ToString("yyyy-MM-dd") + "' />    ");
            strFetchXml.Append("    </filter>                                                                                   ");
            strFetchXml.Append("  </entity>                                                                                     ");
            strFetchXml.Append("</fetch>                                                                                        ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;

            #region

            #endregion
        }        
    }
}
