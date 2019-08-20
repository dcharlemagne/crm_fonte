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
    public class RepMetadaUnidadeporSubfamilia<T> : CrmServiceRepository<T>, IMetadaUnidadeporSubfamilia<T>
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

        public T Obter(Guid subfamiliaId, Guid metadaunidadeporfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_subfamilia", ConditionOperator.Equal, subfamiliaId);
            query.Criteria.AddCondition("itbc_metafamiliasegmentotrimestre", ConditionOperator.Equal, metadaunidadeporfamiliaId);
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

        public T ObterporTrimestre(Guid subfamiliaId, Guid metaunidadetrimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_subfamilia", ConditionOperator.Equal, subfamiliaId);
            //query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);

            query.AddLink("itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestreid")
                 .AddLink("itbc_metaporsegmento", "itbc_metadosegmentoid", "itbc_metaporsegmentoid")
                 .AddLink("itbc_metaportrimestre", "itbc_metaportrimestre", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metaportrimestreid", ConditionOperator.Equal, metaunidadetrimestreId);
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

        public List<T> ListarPor(Guid metadaunidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            query.AddLink("itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestreid")
                 .AddLink("itbc_metaporsegmento", "itbc_metadosegmentoid", "itbc_metaporsegmentoid")
                 .AddLink("itbc_metaportrimestre", "itbc_metaportrimestre", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metadaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterSubFamiliaPor(Guid metafamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metafamiliasegmentotrimestre", ConditionOperator.Equal, metafamiliaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterMetaSubFamilia(Guid unidadenegocioId, Guid segmentoId, Guid familiaId, Guid subfamiliaId, int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            query.Criteria.AddCondition("itbc_familiaid", ConditionOperator.Equal, familiaId);
            query.Criteria.AddCondition("itbc_subfamilia", ConditionOperator.Equal, subfamiliaId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
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

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            if (trimestre.HasValue)
            {
                query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre.Value);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<MetadaUnidadeporSubfamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null)
        {
            var lista = new List<MetadaUnidadeporSubfamilia>();

            string fetch = @" <fetch aggregate='true' no-lock='true' >
                                  <entity name='itbc_potencial_supervisor_subfamilia' >
                                    <attribute name='itbc_familiadeprodutoid' alias='familia' groupby='true' />
                                    <attribute name='itbc_potencialplanejado' alias='valor_planejado' aggregate='sum' />
                                    <attribute name='itbc_potencialrealizado' alias='valor_realizado' aggregate='sum' />
                                    <attribute name='itbc_segmentoid' alias='segmento' groupby='true' />
                                    <attribute name='itbc_subfamliadeprodutoid' alias='subfamilia' groupby='true' />
                                    <attribute name='itbc_trimestre' alias='trimestre' groupby='true' />
                                    <filter type='and' >
                                      <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                      <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                      {2}
                                    </filter>
                                  </entity>
                                </fetch>";
           
            string filterTrimestre = string.Empty;
            if (trimestre.HasValue)
            {
                filterTrimestre = string.Format(@"<condition attribute='itbc_trimestre' operator='eq' value='{0}' />", (int)trimestre.Value);
            }

            fetch = string.Format(fetch, ano, unidadeNegocioId, filterTrimestre);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var familia = ((EntityReference)((AliasedValue)item.Attributes["familia"]).Value);
                var subfamilia = ((EntityReference)((AliasedValue)item.Attributes["subfamilia"]).Value);
                var segmento = ((EntityReference)((AliasedValue)item.Attributes["segmento"]).Value);

                var potencial = new MetadaUnidadeporSubfamilia(OrganizationName, IsOffline, Provider)
                {
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value,
                    Familia = new SDKore.DomainModel.Lookup(familia.Id, familia.Name, familia.LogicalName),
                    Subfamilia = new SDKore.DomainModel.Lookup(subfamilia.Id, subfamilia.Name, subfamilia.LogicalName),
                    Segmento = new SDKore.DomainModel.Lookup(segmento.Id, segmento.Name, segmento.LogicalName),
                    Trimestre = ((OptionSetValue)((AliasedValue)item.Attributes["trimestre"]).Value).Value
                };

                lista.Add(potencial);
            }

            return lista;
        }



        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaSubFamiliaDW(int ano, int trimestre)
        {
            return this.ListarMetaSubFamiliaDW(ano, trimestre, null);
        }

        public DataTable ListarMetaSubFamiliaDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;
            strSql.Append("Select cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia ");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And cd_trimestre = {0} ", trimestre);

            if (lstOrcamentodaUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstOrcamentodaUnidade)
                    commandIn += string.Concat("'", item.UnidadedeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

