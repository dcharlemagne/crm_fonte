using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intelbras.CRM2013.DAL
{

    public class RepPotencialdoKAporTrimestre<T> : CrmServiceRepository<T>, IPotencialdoKAporTrimestre<T>
    {
        #region Objeto Q obtem a conexao com o SQL
        private DataBaseSqlServer _DataBaseSqlServer = null;
        private DataBaseSqlServer DataBaseSqlServer
        {
            get
            {
                if (_DataBaseSqlServer == null)
                    _DataBaseSqlServer = new DataBaseSqlServer();

                return _DataBaseSqlServer;
            }
        }
        #endregion

        public T Obter(Guid potencialdokarepresentanteId, Guid karepresentante, int trimestre, Guid segmentoId)
        {

            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, karepresentante);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_metadokarepresentanteid", ConditionOperator.Equal, potencialdokarepresentanteId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> Obter(Guid potencialKA)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                        ");
            strFetchXml.Append("  <entity name='itbc_metatrimestreka'>                                                                                       ");
            strFetchXml.Append("    <attribute name='itbc_metatrimestrekaid' />                                                                              ");
            strFetchXml.Append("    <attribute name='itbc_name' />                                                                                           ");
            strFetchXml.Append("    <attribute name='createdon' />                                                                                           ");
            strFetchXml.Append("    <order attribute='itbc_name' descending='false' />                                                                       ");
            strFetchXml.Append("    <link-entity name='itbc_metakeyaccount' from='itbc_metakeyaccountid' to='itbc_metadokarepresentanteid' alias='ac'>       ");
            strFetchXml.Append("      <filter type='and'>                                                                                                    ");
            strFetchXml.AppendFormat("        <condition attribute='itbc_metakeyaccountid' operator='eq' uitype='itbc_metakeyaccount' value='{0}' />               ", potencialKA);
            strFetchXml.Append("      </filter>                                                                                                              ");
            strFetchXml.Append("    </link-entity>                                                                                                           ");
            strFetchXml.Append("  </entity>                                                                                                                  ");
            strFetchXml.Append("</fetch>                                                                                                                     ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarPor(Guid metadokarepresentanteId, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadokarepresentanteid", ConditionOperator.Equal, metadokarepresentanteId);
            if (trimestre != null && trimestre != int.MinValue && trimestre != 0)
                query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarPorTrimestre(int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<PotencialdoKAporTrimestre> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var lista = new List<PotencialdoKAporTrimestre>();

            string fetch = @"<fetch aggregate='true' no-lock='true' returntotalrecordcount='true' >
                              <entity name='itbc_metadokaporsegmento' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <link-entity name='itbc_metatrimestreka' from='itbc_metatrimestrekaid' to='itbc_metatrimestrekaid' alias='tri' >
                                  <attribute name='itbc_metatrimestrekaid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidadenegocioid' operator='eq' value='{1}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            

            fetch = string.Format(fetch, ano, unidadeNegocioId);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var potencialTrimestre = new PotencialdoKAporTrimestre(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(potencialTrimestre);
            }

            return lista;
        }

        public T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_keyaccountreprid", ConditionOperator.Equal, KARepresentanteId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas)
        {
            List<UnidadeNegocio> lstUnidade = new List<UnidadeNegocio>();
            foreach (var item in lstMetas)
                lstUnidade.Add(item.UnidadedeNegocio);

            return this.ListarMetaTrimestreDW(ano, trimestre, lstUnidade);
        }

        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<UnidadeNegocio> lstMetas)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And cd_trimestre = {0} ", trimestre);

            if (lstMetas != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstMetas)
                    commandIn += string.Concat("'", item.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public T ObterPor(Guid potencialKAId, Guid karepresentante, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();


            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                                       ");
            strFetchXml.Append("  <entity name='itbc_metatrimestreka'>                                                                                                      ");
            strFetchXml.Append("    <attribute name='itbc_metatrimestrekaid' />                                                                                             ");
            strFetchXml.Append("    <attribute name='itbc_name' />                                                                                                          ");
            strFetchXml.Append("    <attribute name='createdon' />                                                                                                          ");
            strFetchXml.Append("    <order attribute='itbc_name' descending='false' />                                                                                      ");
            strFetchXml.Append("    <filter type='and'>                                                                                                                     ");
            strFetchXml.Append("      <condition attribute='itbc_trimestre' operator='eq' value='" + trimestre + "' />                                                              ");
            strFetchXml.Append("    </filter>                                                                                                                               ");
            strFetchXml.Append("    <link-entity name='itbc_metakeyaccount' from='itbc_metakeyaccountid' to='itbc_metadokarepresentanteid' alias='aj'>                      ");
            strFetchXml.Append("      <filter type='and'>                                                                                                                   ");
            strFetchXml.AppendFormat("        <condition attribute='itbc_metakeyaccountid' operator='eq' uiname=' - 1o Trimestre' uitype='itbc_metakeyaccount' value='{0}' />", potencialKAId);
            strFetchXml.Append("      </filter>                                                                                                                             ");
            strFetchXml.Append("    </link-entity>                                                                                                                          ");
            strFetchXml.Append("  </entity>                                                                                                                                 ");
            strFetchXml.Append("</fetch>                                                                                                                                    ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            var colecao = this.RetrieveMultiple(retrieveMultiple.Query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}