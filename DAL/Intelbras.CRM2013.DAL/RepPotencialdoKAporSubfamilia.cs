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
    public class RepPotencialdoKAporSubfamilia<T> : CrmServiceRepository<T>, IPotencialdoKAporSubfamilia<T>
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

        public T Obterpor(Guid canalId, Guid familiaId, Guid metadocanalporfamiliaId, Guid segmentoId, Guid subfamiliaId, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_familiadeprodutoid", ConditionOperator.Equal, familiaId);
            query.Criteria.AddCondition("itbc_metadokaporfamiliaid", ConditionOperator.Equal, metadocanalporfamiliaId);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            query.Criteria.AddCondition("itbc_subfamiliadeprodutoid", ConditionOperator.Equal, subfamiliaId);
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

        public T Obter(Guid subfamiliaId, Guid karepresentanteId, Guid trimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_subfamiliadeprodutoid", ConditionOperator.Equal, subfamiliaId);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, karepresentanteId);

            query.AddLink("itbc_metadokaporfamilia", "itbc_metadokaporfamiliaid", "itbc_metadokaporfamiliaid")
                 .AddLink("itbc_metadokaporsegmento", "itbc_metadokaporsegmentoid", "itbc_metadokaporsegmentoid")
                 .AddLink("itbc_metakeyaccount", "itbc_metadokarepresentanteid", "itbc_metakeyaccountid")
                 .LinkCriteria.AddCondition("itbc_metaportrimestredaunidadeid", ConditionOperator.Equal, trimestreId);
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

        public List<T> ListsarSubfamilia(Guid metaunidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            query.AddLink("itbc_metadokaporfamilia", "itbc_metadokaporfamiliaid", "itbc_metadokaporfamiliaid")
                .AddLink("itbc_metadokaporsegmento", "itbc_metadokaporsegmentoid", "itbc_metadokaporsegmentoid")
                .AddLink("itbc_metatrimestreka", "itbc_metatrimestrekaid", "itbc_metatrimestrekaid")
                .AddLink("itbc_metakeyaccount", "itbc_metadokarepresentanteid", "itbc_metakeyaccountid");
                //.LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);
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

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ObterSubFamiliaPor(Guid potencialdokafamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadokaporfamiliaid", ConditionOperator.Equal, potencialdokafamiliaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<PotencialdoKAporSubfamilia> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre)
        {
            var lista = new List<PotencialdoKAporSubfamilia>();

            string fetch = @"<fetch aggregate='true' no-lock='true' returntotalrecordcount='true' >
                              <entity name='itbc_metadokaporproduto' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <link-entity name='itbc_metadokaporsubfamilia' from='itbc_metadokaporsubfamiliaid' to='itbc_metadokaporsubfamiliaid' link-type='inner' alias='sub' >
                                  <attribute name='itbc_metadokaporsubfamiliaid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                    <condition attribute='itbc_trimestre' operator='eq' value='{2}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";


            fetch = string.Format(fetch, ano, unidadeNegocioId, (int)trimestre);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var potencialSubfamilia = new PotencialdoKAporSubfamilia(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(potencialSubfamilia);
            }

            return lista;
        }

        public T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, Guid segmentoId, Guid familiaId, Guid subfamiliaId)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, KARepresentanteId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            query.Criteria.AddCondition("itbc_familiadeprodutoid", ConditionOperator.Equal, familiaId);
            query.Criteria.AddCondition("itbc_subfamiliadeprodutoid", ConditionOperator.Equal, subfamiliaId);
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
            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia ");
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

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

