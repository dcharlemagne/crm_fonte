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

    public class RepPotencialdoKAporSegmento<T> : CrmServiceRepository<T>, IPotencialdoKAporSegmento<T>
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
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, potencialdokarepresentanteId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> __Obter(Guid unId, Guid representanteId, int trimestre, Guid trimestreId, Guid segId)
        {

            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append(" <fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                  ");
            strFetchXml.Append(" <entity name='itbc_metadokaporsegmento'>                                                                               ");
            strFetchXml.Append("   <attribute name='itbc_unidadedenegocioid' />                                                                         ");
            strFetchXml.Append("   <attribute name='itbc_trimestre' />                                                                                  ");
            strFetchXml.Append("   <attribute name='itbc_segmentoid' />                                                                                 ");
            strFetchXml.Append("   <attribute name='itbc_metarealizada' />                                                                              ");
            strFetchXml.Append("   <attribute name='itbc_metaplanejada' />                                                                              ");
            strFetchXml.Append("   <attribute name='itbc_kaourepresentanteid' />                                                                        ");
            strFetchXml.Append("   <attribute name='itbc_ano' />                                                                                        ");
            strFetchXml.Append("   <attribute name='itbc_metadokaporsegmentoid' />                                                                      ");
            strFetchXml.Append("   <attribute name='createdon' />                                                                                       ");
            strFetchXml.Append("   <order attribute='itbc_ano' descending='false' />                                                                    ");
            strFetchXml.Append("   <filter type='and'>                                                                                                  ");
            strFetchXml.Append("     <condition attribute='statecode' operator='eq' value='0' />                                                        ");
            strFetchXml.AppendFormat("     <condition attribute='itbc_kaourepresentanteid' operator='eq' uitype='contact' value='{0}' />                      ", representanteId);
            strFetchXml.AppendFormat("     <condition attribute='itbc_trimestre' operator='eq' value='" + trimestre + "' />                                                ");
            strFetchXml.AppendFormat("     <condition attribute='itbc_unidadedenegocioid' operator='eq' uitype='businessunit' value='{0}' />                  ", unId);
            strFetchXml.AppendFormat("     <condition attribute='itbc_segmentoid' operator='eq' uitype='itbc_segmento' value='{0}' />                       ", segId);
            strFetchXml.Append("   </filter>                                                                                                            ");
            strFetchXml.Append("   <link-entity name='itbc_metatrimestreka' from='itbc_metatrimestrekaid' to='itbc_metatrimestrekaid' alias='am'>       ");
            //strFetchXml.Append("     <filter type='and'>                                                                                                ");
            //strFetchXml.AppendFormat("       <condition attribute='itbc_metatrimestrekaid' operator='eq' uitype='itbc_metatrimestreka' value='{0}' />         ", trimestreId);
            //strFetchXml.Append("     </filter>                                                                                                          ");
            strFetchXml.Append("   </link-entity>                                                                                                       ");
            strFetchXml.Append(" </entity>                                                                                                              ");
            strFetchXml.Append("</fetch>                                                                                                                 ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarSegmentos(MetadaUnidade metaunidade, Guid representanteId)
        {

            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                                                           ");
            strFetchXml.Append("  <entity name='itbc_metadokaporsegmento'>                                                                                                                      ");
            strFetchXml.Append("    <attribute name='itbc_unidadedenegocioid' />                                                                                                                ");
            strFetchXml.Append("    <attribute name='itbc_trimestre' />                                                                                                                         ");
            strFetchXml.Append("    <attribute name='itbc_segmentoid' />                                                                                                                        ");
            strFetchXml.Append("    <attribute name='itbc_metarealizada' />                                                                                                                     ");
            strFetchXml.Append("    <attribute name='itbc_metaplanejada' />                                                                                                                     ");
            strFetchXml.Append("    <attribute name='itbc_kaourepresentanteid' />                                                                                                               ");
            strFetchXml.Append("    <attribute name='itbc_ano' />                                                                                                                               ");
            strFetchXml.Append("    <attribute name='itbc_metadokaporsegmentoid' />                                                                                                             ");
            strFetchXml.Append("    <order attribute='itbc_ano' descending='false' />                                                                                                           ");
            strFetchXml.Append("    <filter type='and'>                                                                                                                                         ");
            strFetchXml.AppendFormat("      <condition attribute='itbc_unidadedenegocioid' operator='eq' uitype='businessunit' value='{0}' />                                                   ", metaunidade.UnidadedeNegocio.ID);
            strFetchXml.AppendFormat("      <condition attribute='itbc_ano' operator='eq' value='" + metaunidade.Ano + "' />                                                                           ");
            strFetchXml.AppendFormat("      <condition attribute='itbc_kaourepresentanteid' operator='eq' uitype='contact' value='{0}' />                                                            ", representanteId);
            strFetchXml.Append("    </filter>                                                                                                                                                   ");
            strFetchXml.Append("  </entity>                                                                                                                                                     ");
            strFetchXml.Append("</fetch>                                                                                                                                                        ");


            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarPor(Guid metadokarepresentanteId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadokarepresentanteid", ConditionOperator.Equal, metadokarepresentanteId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocio, int ano)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocio);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<PotencialdoKAporSegmento> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var lista = new List<PotencialdoKAporSegmento>();

            string fetch = @"<fetch aggregate='true' no-lock='true' returntotalrecordcount='true' >
                              <entity name='itbc_metadokaporfamilia' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <link-entity name='itbc_metadokaporsegmento' from='itbc_metadokaporsegmentoid' to='itbc_metadokaporsegmentoid' alias='seg' >
                                  <attribute name='itbc_metadokaporsegmentoid' alias='id' groupby='true' />
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
                var potencialSegmento = new PotencialdoKAporSegmento(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(potencialSegmento);
            }

            return lista;
        }

        public T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, Guid segmentoId)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, KARepresentanteId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento ");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And cd_trimestre = {0} ", trimestre);

            if (lstMetas != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstMetas)
                    commandIn += string.Concat("'", item.UnidadedeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public T ObterPor(Guid segmentoId, Guid potencialkasegmentoId, Guid trimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, karepresentanteId);
            query.Criteria.AddCondition("itbc_metadokaporsegmentoid", ConditionOperator.Equal, potencialkasegmentoId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            //query.Criteria.AddCondition("itbc_familiadeprodutoid", ConditionOperator.Equal, familiaId);
            //query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_metatrimestrekaid", ConditionOperator.Equal, trimestreId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        //public T ObterPor(Guid TrimestreId, Guid SegmentoId)
        //{
        //    var query = GetQueryExpression<T>(true);

        //    RetrieveMultipleRequest retrieveMultiple;
        //    StringBuilder strFetchXml = new StringBuilder();

        //   strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                ");
        //   strFetchXml.Append("  <entity name='itbc_metadokaporsegmento'>                                                                           ");
        //   strFetchXml.Append("    <attribute name='itbc_unidadedenegocioid' />                                                                     ");
        //   strFetchXml.Append("    <attribute name='itbc_trimestre' />                                                                              ");
        //   strFetchXml.Append("    <attribute name='itbc_segmentoid' />                                                                             ");
        //   strFetchXml.Append("    <attribute name='itbc_metarealizada' />                                                                          ");
        //   strFetchXml.Append("    <attribute name='itbc_metaplanejada' />                                                                          ");
        //   strFetchXml.Append("    <attribute name='itbc_kaourepresentanteid' />                                                                    ");
        //   strFetchXml.Append("    <attribute name='itbc_ano' />                                                                                    ");
        //   strFetchXml.Append("    <attribute name='itbc_metadokaporsegmentoid' />                                                                  ");
        //   strFetchXml.Append("    <order attribute='itbc_ano' descending='false' />                                                                ");
        //   strFetchXml.Append("    <filter type='and'>                                                                                              ");
        //   //strFetchXml.AppendFormat("      <condition attribute='itbc_kaourepresentanteid' operator='eq' uitype='contact' value='{0}' />                  ", KARepresentanteId);
        //   //strFetchXml.Append("      <condition attribute='itbc_trimestre' operator='eq' value='"+numTrimestre+"' />                                       ");
        //   strFetchXml.AppendFormat("      <condition attribute='itbc_segmentoid' operator='eq' uitype='itbc_segmento' value='{0}' />                     ", SegmentoId);
        //   strFetchXml.Append("    </filter>                                                                                                        ");
        //   strFetchXml.Append("    <link-entity name='itbc_metatrimestreka' from='itbc_metatrimestrekaid' to='itbc_metatrimestrekaid' alias='am'>   ");
        //   strFetchXml.Append("      <filter type='and'>                                                                                            ");
        //   strFetchXml.AppendFormat("        <condition attribute='itbc_metatrimestrekaid' operator='eq' uitype='itbc_metatrimestreka' value='{0}' />     ", TrimestreId);
        //   strFetchXml.Append("      </filter>                                                                                                      ");
        //   strFetchXml.Append("    </link-entity>                                                                                                   ");
        //   strFetchXml.Append("  </entity>                                                                                                          ");
        //   strFetchXml.Append("</fetch>                                                                                                             ");



        //    // Build fetch request and obtain results.
        //    retrieveMultiple = new RetrieveMultipleRequest()
        //    {
        //        Query = new FetchExpression(strFetchXml.ToString())
        //    };

        //    var colecao = this.RetrieveMultiple(retrieveMultiple.Query);

        //    if (colecao.List.Count == 0)
        //        return default(T);

        //    return colecao.List[0];

        //}

        public List<T> ListarPorTrimestreId(Guid TrimestreId)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                ");
            strFetchXml.Append("  <entity name='itbc_metadokaporsegmento'>                                                                           ");
            strFetchXml.Append("<all-attributes />");
            strFetchXml.Append("    <order attribute='itbc_ano' descending='false' />                                                                ");
            strFetchXml.Append("    <filter type='and'>                                                                                              ");
            strFetchXml.Append("    </filter>                                                                                                        ");
            strFetchXml.Append("    <link-entity name='itbc_metatrimestreka' from='itbc_metatrimestrekaid' to='itbc_metatrimestrekaid' alias='am'>   ");
            strFetchXml.Append("      <filter type='and'>                                                                                            ");
            strFetchXml.AppendFormat("        <condition attribute='itbc_metatrimestrekaid' operator='eq' uitype='itbc_metatrimestreka' value='{0}' />     ", TrimestreId);
            strFetchXml.Append("      </filter>                                                                                                      ");
            strFetchXml.Append("    </link-entity>                                                                                                   ");
            strFetchXml.Append("  </entity>                                                                                                          ");
            strFetchXml.Append("</fetch>                                                                                                             ");



            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;

        }
    }
}

