using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepMetaDetalhadadoCanalporProduto<T> : CrmServiceRepository<T>, IMetaDetalhadadoCanalporProduto<T>
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

        public T Obter(int ano, int trimestre, int mes, Guid canalId, Guid metadocanalproprodutoId, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_metadocanalporprodutoid", ConditionOperator.Equal, metadocanalproprodutoId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
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

        public T ListarPorManual(Guid canalId, int mes)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Null);

            query.AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid");
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_canalid", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterManual(int trimestre, int ano, Guid canalId, Guid metadocanalId, int mes, Guid unidadenegocioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_metadocanalid", ConditionOperator.Equal, metadocanalId);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Null);

            query.AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                .LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_canalid", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(Guid canalId, Guid produtoId, Guid metadotrimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_metadocanalporproduto", "itbc_metadocanalporprodutoid", "itbc_metadocanalporprodutoid")
                .AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
                .AddLink("itbc_metadocanalporfamilia", "itbc_metadocanalporfamiliaid", "itbc_metadocanalporfamiliaid")
                .AddLink("itbc_metadocanalporsegmento", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                .AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                .LinkCriteria.AddCondition("itbc_metadotrimestreid", ConditionOperator.Equal, metadotrimestreId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_canalid", OrderType.Ascending);
            OrderExpression ord2 = new OrderExpression("itbc_produtoid", OrderType.Ascending);
            query.Orders.Add(ord1);
            query.Orders.Add(ord2);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid canalId, Guid unidadeNegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_unidade_negocio", ConditionOperator.Equal, unidadeNegocioId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarDetalheProdutosPorMeta(Guid metaId, Guid canalId, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_metadocanalporproduto", "itbc_metadocanalporprodutoid", "itbc_metadocanalporprodutoid")
                .AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
                .AddLink("itbc_metadocanalporfamilia", "itbc_metadocanalporfamiliaid", "itbc_metadocanalporfamiliaid")
                .AddLink("itbc_metadocanalporsegmento", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                .AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                .AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
                .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_canalid", OrderType.Ascending);
            OrderExpression ord2 = new OrderExpression("itbc_produtoid", OrderType.Ascending);
            query.Orders.Add(ord1);
            query.Orders.Add(ord2);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, Guid? produtoId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre);
            query.Criteria.AddCondition("itbc_unidade_negocio", ConditionOperator.Equal, unidadeNegocioId);

            if (produtoId.HasValue)
            {
                query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId.Value);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_unidade_negocio", ConditionOperator.Equal, unidadeNegocioId);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarMetaCanalDetalhadoProduto(int ano, int trimestre)
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
        public DataTable ListarMetaCanalDetalhadoProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item,cd_mes ");
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

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item,cd_mes ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable ListarMetaCanalManualDetalhadoProdutoDW(int ano, int trimestre, List<MetadaUnidade> lstMetadaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_mes ");
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

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_mes ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable ListarDadosDWPor(int ano, int trimestre, string unidadeNegocioChaveIntegracao)
        {
            string query = @"Select CD_Emitente
                              , cd_mes 
                              , SUM(nm_quantidade_total) as Qtde
                              , SUM(nm_vl_liquido_total) as vlr 
                            From viewFaturamentoCRM 
                            Where CD_Ano = {0} 
                                And cd_trimestre = {1}
                                AND CD_Unidade_Negocio = '{2}'
                            Group By CD_Emitente, cd_mes;";

            query = string.Format(query, ano, trimestre, unidadeNegocioChaveIntegracao);

            return DataBaseSqlServer.executeQuery(query);
        }
    }
}