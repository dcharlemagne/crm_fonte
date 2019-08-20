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
    public class RepMetadaUnidadeporProduto<T> : CrmServiceRepository<T>, IMetadaUnidadeporProduto<T>
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

        public T Obter(Guid produtoId, Guid metaunidadesubfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, metaunidadesubfamiliaId);
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

        public T ObterporTrimestre(Guid produtoId, Guid metaunidadetrimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_metasubfamiliasegmentotrimestre", "itbc_subfamiliaid", "itbc_metasubfamiliasegmentotrimestreid")
                 .AddLink("itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestreid")
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

        public List<T> ObterProdutos(Guid metaunidadeId, List<Guid> lstIdProdutos)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            if (lstIdProdutos.Count > 0)
                query.Criteria.AddCondition(new ConditionExpression("itbc_productid", ConditionOperator.NotIn, lstIdProdutos));

            query.AddLink("itbc_metasubfamiliasegmentotrimestre", "itbc_subfamiliaid", "itbc_metasubfamiliasegmentotrimestreid")
                 .AddLink("itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestreid")
                 .AddLink("itbc_metaporsegmento", "itbc_metadosegmentoid", "itbc_metaporsegmentoid")
                 .AddLink("itbc_metaportrimestre", "itbc_metaportrimestre", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid metaunidadesubfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, metaunidadesubfamiliaId);
            
            //query.AddLink("itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestreid")
            //     .AddLink("itbc_metaporsegmento", "itbc_metadosegmentoid", "itbc_metaporsegmentoid")
            //     .AddLink("itbc_metaportrimestre", "itbc_metaportrimestre", "itbc_metaportrimestreid")
            //     .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metadaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidade_negociosid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            if (trimestre.HasValue)
            {
                query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre.Value);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterMetaProduto(Guid unidadenegocioId, Guid produtoId, int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId);
            query.AddLink("itbc_metasubfamiliasegmentotrimestre", "itbc_subfamiliaid", "itbc_metasubfamiliasegmentotrimestreid")
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
        
        public List<MetadaUnidadeporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre)
        {
            var lista = new List<MetadaUnidadeporProduto>();

            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_potencialdosupervisorporproduto' >
                                <attribute name='itbc_potencialplanejado' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_potencialrealizado' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_produtoid' alias='produto' groupby='true' />
                                <attribute name='itbc_qtdeplanejada' alias='quantidade_planejada' aggregate='sum' />
                                <attribute name='itbc_qtderealizada' alias='quantidade_realizada' aggregate='sum' />
                                <filter type='and' >
                                  <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  <condition attribute='itbc_unidade_negociosid' operator='eq' value='{1}' />
                                  <condition attribute='itbc_trimestre' operator='eq' value='{2}' />
                                </filter>
                                <link-entity name='itbc_potencial_supervisor_subfamilia' from='itbc_potencial_supervisor_subfamiliaid' to='itbc_potencialsupervisorporprodutoid' >
                                  <attribute name='itbc_subfamliadeprodutoid' alias='subfamilia' groupby='true' />
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
                var produto = ((EntityReference)((AliasedValue)item.Attributes["produto"]).Value);
                var subfamilia = ((EntityReference)((AliasedValue)item.Attributes["subfamilia"]).Value);

                var potencial = new MetadaUnidadeporProduto(OrganizationName, IsOffline, Provider)
                {
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada= ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value,
                    QtdePlanejada = (decimal)((AliasedValue)item.Attributes["quantidade_planejada"]).Value,
                    QtdeRealizada = (decimal)((AliasedValue)item.Attributes["quantidade_realizada"]).Value,
                    Trimestre = (int)trimestre,
                    Ano = ano,
                    Produto = new SDKore.DomainModel.Lookup(produto.Id, produto.Name, produto.LogicalName),
                    Subfamilia = new SDKore.DomainModel.Lookup(subfamilia.Id, subfamilia.Name, subfamilia.LogicalName)
                };

                lista.Add(potencial);
            }

            return lista;
        }

        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaProdutoDW(int ano, int trimestre)
        {
            return this.ListarMetaProdutoDW(ano, trimestre, null);
        }

        public DataTable ListarMetaProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;
            strSql.Append("Select cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item ");
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

            strSql.Append("Group By cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

