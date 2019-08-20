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

namespace Intelbras.CRM2013.DAL
{

    public class RepMetaDetalhadadaUnidadeporProduto<T> : CrmServiceRepository<T>, IMetaDetalhadadaUnidadeporProduto<T>
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

        public List<T> Listar(Guid metaunidadeprodutoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadoprodutoid", ConditionOperator.Equal, metaunidadeprodutoId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterDetalhadoProdutos(Guid produtoId, Guid metaunidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_metaporproduto", "itbc_metadoprodutoid", "itbc_metaporprodutoid")
                .AddLink("itbc_metasubfamiliasegmentotrimestre", "itbc_subfamiliaid", "itbc_metasubfamiliasegmentotrimestreid")
                .AddLink("itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestreid")
                .AddLink("itbc_metaporsegmento", "itbc_metadosegmentoid", "itbc_metaporsegmentoid")
                .AddLink("itbc_metaportrimestre", "itbc_metaportrimestre", "itbc_metaportrimestreid")
                .AddLink("itbc_metas", "itbc_metas", "itbc_metasid")
                .LinkCriteria.AddCondition("itbc_metasid", ConditionOperator.Equal, metaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> Obterpor(Guid metaprodutoId, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_metadoprodutoid", ConditionOperator.Equal, metaprodutoId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterProdutoDetalhado(Guid unidadenegocioId, Guid produtoId, int ano, int trimestre, int mes)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
            query.AddLink("itbc_metaporproduto", "itbc_metadoprodutoid", "itbc_metaporprodutoid")
                 .AddLink("itbc_metasubfamiliasegmentotrimestre", "itbc_subfamiliaid", "itbc_metasubfamiliasegmentotrimestreid")
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
        public DataTable ListarProdutoDetalhadoDW(int ano, int trimestre)
        {
            return this.ListarProdutoDetalhadoDW(ano, trimestre, null);
        }

        public DataTable ListarProdutoDetalhadoDW(int ano, int trimestre, List<MetadaUnidade> lstOrcamentodaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;
            strSql.Append("Select cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item,cd_mes ");
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

            strSql.Append("Group By cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item,cd_mes ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

