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
    public class RepMetadoCanalporProduto<T> : CrmServiceRepository<T>, IMetadoCanalporProduto<T>
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

        public T Obter(Guid canalId, Guid produtoId, Guid subfamiliaId, Guid metaporsubfamiliaId,int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            //query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, subfamiliaId);
            //query.Criteria.AddCondition("itbc_metaporsubfamiliaid", ConditionOperator.Equal, metaporsubfamiliaId);
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

        public T Obterpor(Guid canalId, Guid produtoId, Guid trimesteId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
                 .AddLink("itbc_metadocanalporfamilia", "itbc_metadocanalporfamiliaid", "itbc_metadocanalporfamiliaid")
                 .AddLink("itbc_metadocanalporsegmento", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                 .AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                 .LinkCriteria.AddCondition("itbc_metadotrimestreid", ConditionOperator.Equal, trimesteId);
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

        public T Obterpor(Guid unidadenegocioId,Guid canalId,int ano,int trimestre,Guid subfamiliaId,Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            //Comentei pois nao tem sentido filtrar pela subfamilia E produtoID, se ja esta enviando o guid do produto nao precisa da sub familia para filtrar.
            //query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, subfamiliaId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
            .LinkCriteria.AddCondition("itbc_unidadedenegociosid", ConditionOperator.Equal, unidadenegocioId);
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

        public List<T> ListarProdutosCanal(Guid metacanalsubfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadocanalporsubfamiliaid", ConditionOperator.Equal, metacanalsubfamiliaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutos(Guid metaunidadeId, List<Guid> lstIdProdutos)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_produtoid", ConditionOperator.NotIn, lstIdProdutos));

            query.AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
                 .AddLink("itbc_metadocanalporfamilia", "itbc_metadocanalporfamiliaid", "itbc_metadocanalporfamiliaid")
                 .AddLink("itbc_metadocanalporsegmento", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                 .AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                 .AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarCanalProdutos(Guid metaunidadeId, List<Guid> lstIdCanal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_canalid", ConditionOperator.NotIn, lstIdCanal));

            query.AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
                 .AddLink("itbc_metadocanalporfamilia", "itbc_metadocanalporfamiliaid", "itbc_metadocanalporfamiliaid")
                 .AddLink("itbc_metadocanalporsegmento", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                 .AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                 .AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid subfamiliaProdutoId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre);
            query.Criteria.AddCondition("itbc_unidade_negocio", ConditionOperator.Equal, unidadeNegocioId);
            
            query.AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
                 .LinkCriteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, subfamiliaProdutoId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocioProduto(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre);
            query.Criteria.AddCondition("itbc_unidade_negocio", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<MetadoCanalporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Guid canalId)
        {
            var lista = new List<MetadoCanalporProduto>();

            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_metadetalhadadocanalporproduto' >
                                <attribute name='itbc_qtderealizada' alias='quantidade_realizado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_qtdeplanejada' alias='quantidade_planejada' aggregate='sum' />
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <filter type='and' >
                                  <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  <condition attribute='itbc_unidade_negocio' operator='eq' value='{1}' />
                                  <condition attribute='itbc_canalid' operator='eq' value='{2}' />
                                </filter>
                                <link-entity name='itbc_metadocanalporproduto' from='itbc_metadocanalporprodutoid' to='itbc_metadocanalporprodutoid' >
                                  <attribute name='itbc_metadocanalporprodutoid' alias='id' groupby='true' />
                                </link-entity>
                              </entity>
                            </fetch>";

            
            fetch = string.Format(fetch, ano, unidadeNegocioId, canalId);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var meta = new MetadoCanalporProduto(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                    //qu = (decimal)((AliasedValue)(item.Attributes["quantidade_planejada"])).Value,
                    //QtdeRealizada = (decimal)((AliasedValue)(item.Attributes["quantidade_realizado"])).Value
                };

                lista.Add(meta);
            }

            return lista;
        }

        public List<T> ListarMetaCanalProduto(int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }



        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaCanalProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item ");
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

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}