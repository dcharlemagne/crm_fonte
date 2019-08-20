using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Microsoft.Xrm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{

    public class RepPotencialdoKAporProduto<T> : CrmServiceRepository<T>, IPotencialdoKAporProduto<T>
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


        public List<T> ObterProdutos(Guid metaunidadeId, List<Guid> lstIdProdutos)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_produtoid", ConditionOperator.NotIn, lstIdProdutos));

            query.AddLink("itbc_metadokaporsubfamilia", "itbc_metadokaporsubfamiliaid", "itbc_metadokaporsubfamiliaid")
                 .AddLink("itbc_metadokaporfamilia", "itbc_metadokaporfamiliaid", "itbc_metadokaporfamiliaid")
                 .AddLink("itbc_metadokaporsegmento", "itbc_metadokaporsegmentoid", "itbc_metadokaporsegmentoid")
                 .AddLink("itbc_metakeyaccount", "itbc_metadokarepresentanteid", "itbc_metakeyaccountid")
                 .AddLink("itbc_metaportrimestre", "itbc_metaportrimestredaunidadeid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);
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
            query.Criteria.AddCondition("itbc_unidade_negociosid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);


            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<PotencialdoKAporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes)
        {
            var lista = new List<PotencialdoKAporProduto>();

            string fetch = @"<fetch aggregate='true' no-lock='true' returntotalrecordcount='true' >
                              <entity name='itbc_metadetalhadadokaporproduto' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_qtdeplanejada' alias='quantidade_planejada' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_qtderealizada' alias='quantidade_realizado' aggregate='sum' />
                                <filter type='and' >
                                    <condition attribute='itbc_mes' operator='eq' value='{2}' />
                                </filter>
                                <link-entity name='itbc_metadokaporproduto' from='itbc_metadokaporprodutoid' to='itbc_metadokaporprodutoid' link-type='inner' alias='prod' >
                                  <attribute name='itbc_metadokaporprodutoid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidade_negociosid' operator='eq' value='{1}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";

            fetch = string.Format(fetch, ano, unidadeNegocioId, mes);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var potencialProduto = new PotencialdoKAporProduto(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value,
                    QtdePlanejada = (decimal)((AliasedValue)(item.Attributes["quantidade_planejada"])).Value,
                    QtdeRealizada = (decimal)((AliasedValue)(item.Attributes["quantidade_realizado"])).Value
                };

                lista.Add(potencialProduto);
            }

            return lista;
        }

        public List<PotencialdoKAporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Guid representanteId)
        {
            var lista = new List<PotencialdoKAporProduto>();

            string fetch = @"<fetch aggregate='true' no-lock='true' returntotalrecordcount='true' >
                              <entity name='itbc_metadetalhadadokaporproduto' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_qtdeplanejada' alias='quantidade_planejada' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_qtderealizada' alias='quantidade_realizado' aggregate='sum' />
                                <link-entity name='itbc_metadokaporproduto' from='itbc_metadokaporprodutoid' to='itbc_metadokaporprodutoid' link-type='inner' alias='prod' >
                                  <attribute name='itbc_metadokaporprodutoid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidade_negociosid' operator='eq' value='{1}' />
                                    <condition attribute='itbc_kaourepresentanteid' operator='eq' value='{2}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            

            fetch = string.Format(fetch, ano, unidadeNegocioId, representanteId);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var potencialProduto = new PotencialdoKAporProduto(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value,
                    QtdePlanejada = (decimal)((AliasedValue)(item.Attributes["quantidade_planejada"])).Value,
                    QtdeRealizada = (decimal)((AliasedValue)(item.Attributes["quantidade_realizado"])).Value
                };

                lista.Add(potencialProduto);
            }

            return lista;
        }

        public List<T> ListarProdutos(Guid potencialkasubfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadokaporsubfamiliaid", ConditionOperator.Equal, potencialkasubfamiliaId);

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid KARepresentanteId, Guid produtoId, int trimestre)
        {
            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                   ");
            strFetchXml.Append("  <entity name='itbc_metadokaporproduto'>                                                               ");
            strFetchXml.Append("    <attribute name='itbc_trimestre' />                                                                 ");
            strFetchXml.Append("    <attribute name='itbc_produtoid' />                                                                 ");
            strFetchXml.Append("    <attribute name='itbc_metarealizada' />                                                             ");
            strFetchXml.Append("    <attribute name='itbc_metaplanejada' />                                                             ");
            strFetchXml.Append("    <attribute name='itbc_ano' />                                                                       ");
            strFetchXml.Append("    <attribute name='itbc_kaourepresentanteid' />                                                       ");
            strFetchXml.Append("    <attribute name='itbc_metadokaporprodutoid' />                                                      ");
            strFetchXml.Append("    <order attribute='itbc_ano' descending='false' />                                                   ");
            strFetchXml.Append("    <filter type='and'>                                                                                 ");
            strFetchXml.AppendFormat("      <condition attribute='itbc_produtoid' operator='eq' uitype='product' value='{0}' />               ", produtoId);
            strFetchXml.AppendFormat("      <condition attribute='itbc_kaourepresentanteid' operator='eq' uitype='contact' value='{0}' />     ", KARepresentanteId);
            strFetchXml.Append("      <condition attribute='itbc_trimestre' operator='eq' value='" + trimestre + "' />                               ");
            strFetchXml.Append("    </filter>                                                                                           ");
            strFetchXml.Append("  </entity>                                                                                             ");
            strFetchXml.Append("</fetch>                                                                                                ");



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

        public T Obter(Guid CanalId, Guid produtoId, Guid subfamiliaId, Guid potencialdokaporsubfamiliaId, int Trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, CanalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            //query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, subfamiliaId);
            query.Criteria.AddCondition("itbc_metadokaporsubfamiliaid", ConditionOperator.Equal, potencialdokaporsubfamiliaId);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, Trimestre);
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

        public T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, KARepresentanteId);
            query.AddLink("itbc_metadokaporsubfamilia", "itbc_metadokaporsubfamiliaid", "itbc_metadokaporsubfamiliaid")
                 .LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
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

        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia,CD_Item ");
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

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia,CD_Item ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}