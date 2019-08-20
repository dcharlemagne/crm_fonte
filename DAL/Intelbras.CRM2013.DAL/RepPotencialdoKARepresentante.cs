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
    public class RepPotencialdoKARepresentante<T> : CrmServiceRepository<T>, IPotencialdoKARepresentante<T>
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

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            //query.Criteria.AddCondition("itbc_contact", ConditionOperator.Equal, "80658743-7DEE-E311-9407-00155D013D38");

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<PotencialdoKARepresentante> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var lista = new List<PotencialdoKARepresentante>();

            string fetch = @"<fetch aggregate='true' no-lock='true' returntotalrecordcount='true' >
                              <entity name='itbc_metatrimestreka' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <link-entity name='itbc_metakeyaccount' from='itbc_metakeyaccountid' to='itbc_metadokarepresentanteid' alias='ka' >
                                  <attribute name='itbc_metakeyaccountid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
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
                var potencialRepresentante = new PotencialdoKARepresentante(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(potencialRepresentante);
            }

            return lista;
        }

        public T Obter(Guid metadaunidadeportrimestreId, Guid KARepresentanteId, int trimestre)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //query.Criteria.AddCondition("itbc_metadokarepresentanteid", ConditionOperator.Equal, metadaunidadeportrimestreId);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            //query.Criteria.AddCondition("itbc_contact", ConditionOperator.Equal, KARepresentanteId);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }


        public T ObterPor(Guid KARepresentanteId, Guid UNId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                         ");
            strFetchXml.Append("  <entity name='itbc_metakeyaccount'>                                                                                        ");
            strFetchXml.Append("    <attribute name='itbc_ano' />                                                                                            ");
            strFetchXml.Append("    <attribute name='itbc_unidadedenegocioid' />                                                                             ");
            strFetchXml.Append("    <attribute name='itbc_trimestre' />                                                                                      ");
            strFetchXml.Append("    <attribute name='itbc_metarealizada' />                                                                                  ");
            strFetchXml.Append("    <attribute name='itbc_metaplanejada' />                                                                                  ");
            strFetchXml.Append("    <attribute name='itbc_contact' />                                                                                        ");
            strFetchXml.Append("    <attribute name='itbc_metakeyaccountid' />                                                                               ");
            strFetchXml.Append("    <order attribute='itbc_ano' descending='false' />                                                                        ");
            strFetchXml.Append("    <filter type='and'>                                                                                                      ");
            strFetchXml.AppendFormat("      <condition attribute='itbc_contact' operator='eq'  uitype='contact' value='{0}' />       ", KARepresentanteId);
            strFetchXml.AppendFormat("      <condition attribute='itbc_unidadedenegocioid' operator='eq'  uitype='businessunit' value='{0}' />         ", UNId);
            strFetchXml.Append("      <condition attribute='itbc_ano' operator='eq' value='" + ano + "' />                                                       ");
            strFetchXml.Append("    </filter>                                                                                                                ");
            strFetchXml.Append("  </entity>                                                                                                                  ");
            strFetchXml.Append("</fetch>                                                                                                                      ");

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

        public T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_contact", ConditionOperator.Equal, KARepresentanteId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_contact", ConditionOperator.Equal, KARepresentanteId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaTrimestreDW(int ano, List<UnidadeNegocio> lstUnidade)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_representante");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);

            if (lstUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstUnidade)
                    commandIn += string.Concat("'", item.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_representante");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<UnidadeNegocio> lstUnidade)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select CD_Ano,CD_Trimestre,CD_Unidade_Negocio,CD_Representante");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("and CD_Trimestre = {0} ", trimestre);

            if (lstUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstUnidade)
                    commandIn += string.Concat("'", item.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By CD_Ano,CD_Trimestre,CD_Representante,CD_Trimestre");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable ListarMetaTrimestreDW(int ano, List<MetadaUnidade> lstMeta)
        {
            List<UnidadeNegocio> lstUnidade = new List<UnidadeNegocio>();
            foreach (var item in lstMeta)
                lstUnidade.Add(item.UnidadedeNegocio);

            return this.ListarMetaTrimestreDW(ano, lstUnidade);
        }

        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMeta)
        {
            List<UnidadeNegocio> lstUnidade = new List<UnidadeNegocio>();
            foreach (var item in lstMeta)
                lstUnidade.Add(item.UnidadedeNegocio);

            return this.ListarMetaTrimestreDW(ano, trimestre, lstUnidade);
        }

        public T ObterPorSegmento(Guid segmnetoId)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;

            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>                                        ");
            strFetchXml.Append("  <entity name='itbc_metakeyaccount'>                                                                                       ");
            strFetchXml.Append("    <attribute name='itbc_ano' />                                                                                           ");
            strFetchXml.Append("    <attribute name='itbc_unidadedenegocioid' />                                                                            ");
            strFetchXml.Append("    <attribute name='itbc_trimestre' />                                                                                     ");
            strFetchXml.Append("    <attribute name='itbc_metarealizada' />                                                                                 ");
            strFetchXml.Append("    <attribute name='itbc_metaplanejada' />                                                                                 ");
            strFetchXml.Append("    <attribute name='itbc_contact' />                                                                                       ");
            strFetchXml.Append("    <attribute name='itbc_metakeyaccountid' />                                                                              ");
            strFetchXml.Append("    <order attribute='itbc_ano' descending='false' />                                                                       ");
            strFetchXml.Append("    <link-entity name='itbc_metatrimestreka' from='itbc_metadokarepresentanteid' to='itbc_metakeyaccountid' alias='ae'>     ");
            strFetchXml.Append("      <link-entity name='itbc_metadokaporsegmento' from='itbc_metatrimestrekaid' to='itbc_metatrimestrekaid' alias='af'>    ");
            strFetchXml.Append("        <filter type='and'>                                                                                                 ");
            strFetchXml.AppendFormat("          <condition attribute='itbc_segmentoid' operator='eq' uitype='itbc_segmento' value='{0}' />                        ", segmnetoId);
            strFetchXml.Append("        </filter>                                                                                                           ");
            strFetchXml.Append("      </link-entity>                                                                                                        ");
            strFetchXml.Append("    </link-entity>                                                                                                          ");
            strFetchXml.Append("  </entity>                                                                                                                 ");
            strFetchXml.Append("</fetch>                                                                                                                    ");



            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            var lst = this.RetrieveMultiple(retrieveMultiple.Query).List;
            if (lst.Count > 0)
                return this.RetrieveMultiple(retrieveMultiple.Query).List[0];

            return default(T);
        }

    }
}