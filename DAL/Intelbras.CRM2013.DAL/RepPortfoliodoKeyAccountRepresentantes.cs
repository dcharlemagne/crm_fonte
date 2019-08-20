using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepPortfoliodoKeyAccountRepresentantes<T> : CrmServiceRepository<T>, IPortfoliodoKeyAccountRepresentantes<T>
    {
        public List<T> ListarPor(Guid? unidadeNegocioId, int? tipo)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_tipoid", ConditionOperator.Equal, tipo);

            if (tipo.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPorCrosseling(List<Guid> unidadesNegocioIds, int tipo)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.NotIn, unidadesNegocioIds));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipos)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            // query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.In, unidadesNegocioIds));
            query.Criteria.AddCondition(new ConditionExpression("itbc_tipoid", ConditionOperator.In, tipos));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;

        }

        public List<T> ListarPor(Guid contatoId, Guid unidadeNegocioId, Guid segmentoId, Guid supervisorId, Guid assintenteId)
        {
            QueryExpression query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_contatoid", ConditionOperator.Equal, contatoId);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            if (segmentoId != Guid.Empty && segmentoId != null)
                query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            else
                query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Null);
            query.Criteria.AddCondition("itbc_supervisordevendas", ConditionOperator.Equal, supervisorId);
            query.Criteria.AddCondition("itbc_assistentedeadministracaodevendas", ConditionOperator.Equal, assintenteId);

            return (List<T>)RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocioEsegmentoVazio(Guid PortfoliodoKeyAccountRepresentantesId, Guid contatoId, Guid unidadeNegocioId)
        {
            QueryExpression query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_portfoliokeyaccountrepresentantesid", ConditionOperator.NotEqual, PortfoliodoKeyAccountRepresentantesId);
            query.Criteria.AddCondition("itbc_contatoid", ConditionOperator.Equal, contatoId);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Null);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            return (List<T>)RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocioEsegmentoNaoVazio(Guid PortfoliodoKeyAccountRepresentantesId, Guid contatoId, Guid unidadeNegocioId)
        {
            QueryExpression query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_portfoliokeyaccountrepresentantesid", ConditionOperator.NotEqual, PortfoliodoKeyAccountRepresentantesId);
            query.Criteria.AddCondition("itbc_contatoid", ConditionOperator.Equal, contatoId);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.NotNull);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            return (List<T>)RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPorUnidadeNegocioEsegmento(Guid PortfoliodoKeyAccountRepresentantesId, Guid contatoId, Guid segmentoId, Guid unidadeNegocioId)
        {
            QueryExpression query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_portfoliokeyaccountrepresentantesid", ConditionOperator.NotEqual, PortfoliodoKeyAccountRepresentantesId);
            query.Criteria.AddCondition("itbc_contatoid", ConditionOperator.Equal, contatoId);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);


            return (List<T>)RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPorRepresentanteECodigos(Guid usuario)
        {
            QueryExpression query = GetQueryExpression<T>(true);


            FilterExpression filtro = new FilterExpression(LogicalOperator.Or);
            filtro.AddCondition("itbc_supervisordevendas", ConditionOperator.Equal, usuario);
            filtro.AddCondition("itbc_assistentedeadministracaodevendas", ConditionOperator.Equal, usuario);
            query.Criteria.AddFilter(filtro);

            return (List<T>)RetrieveMultiple(query).List;
        }
        
        public T ObterPor(Guid unidadeNegocioId, Guid contatoId)
        {
            QueryExpression query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_contatoid", ConditionOperator.Equal, contatoId);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        
        public T ObterPor(Guid portfolioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_portfolioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, portfolioId);
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

        public List<T> ListarPorCodigoRepresentante(Guid unId, string codigodorepresentante)
        {
            var query = GetQueryExpression<T>(true);

            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                       ");
            strFetchXml.Append("  <entity name='itbc_portfoliokeyaccountrepresentantes'>                                                    ");
            strFetchXml.Append("    <attribute name='itbc_unidadedenegocioid' />                                                            ");
            strFetchXml.Append("    <attribute name='itbc_supervisordevendas' />                                                            ");
            strFetchXml.Append("    <attribute name='itbc_segmentoid' />                                                                    ");
            strFetchXml.Append("    <attribute name='itbc_contatoid' />                                                                     ");
            strFetchXml.Append("    <attribute name='itbc_assistentedeadministracaodevendas' />                                             ");
            strFetchXml.Append("    <attribute name='itbc_portfoliokeyaccountrepresentantesid' />                                           ");
            strFetchXml.Append("    <order attribute='itbc_unidadedenegocioid' descending='false' />                                        ");
            strFetchXml.Append("    <order attribute='itbc_segmentoid' descending='false' />                                                ");
            strFetchXml.Append("    <filter type='and'>                                                                                     ");
            strFetchXml.AppendFormat("  <condition attribute='itbc_unidadedenegocioid' operator='eq' uitype='businessunit' value='{0}' />", unId);
            strFetchXml.AppendFormat("  <condition attribute='itbc_contatoid' operator='eq' uitype='contact' value='{0}' />"              , codigodorepresentante);
            strFetchXml.Append("        <condition attribute='statecode' operator='eq' value='0' />                                           ");
            strFetchXml.Append("    </filter>                                                                                               ");
            strFetchXml.Append("  </entity>                                                                                                 ");
            strFetchXml.Append("</fetch>                                                                                                    ");

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId)
        {
            QueryExpression query = GetQueryExpression<T>(true);


            FilterExpression filtro = new FilterExpression(LogicalOperator.And);
            filtro.AddCondition("statecode", ConditionOperator.Equal, 0);
            filtro.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddFilter(filtro);

            return (List<T>)RetrieveMultiple(query).List;
        }
    }
}