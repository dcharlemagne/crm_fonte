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

    public class RepMetadoCanalporSegmento<T> : CrmServiceRepository<T>, IMetadoCanalporSegmento<T>
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

        public T Obter(Guid unidadenegocioId, Guid canalId, int ano, int trimestre, Guid segmentoId, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(Guid metaunidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_metadocanalporfamilia", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                 .AddLink("itbc_metadocanalporsegmento", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                 .AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                 .AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);

            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }


        public List<MetadoCanalporSegmento> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimeste)
        {
            var lista = new List<MetadoCanalporSegmento>();

            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_metadocanalporfamilia' >
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <link-entity name='itbc_metadocanalporsegmento' from='itbc_metadocanalporsegmentoid' to='itbc_metadocanalporsegmentoid' >
                                  <attribute name='itbc_metadocanalporsegmentoid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                    <condition attribute='itbc_trimestre' operator='eq' value='{2}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";

            
            fetch = string.Format(fetch, ano, unidadeNegocioId, (int)trimeste);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var meta = new MetadoCanalporSegmento(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(meta);
            }

            return lista;
        }


        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaCanalSegmentoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento ");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And cd_trimestre = {0} ", trimestre);

            if (lstMetadaUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstMetadaUnidade)
                    commandIn += string.Concat("'", item.UnidadedeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

