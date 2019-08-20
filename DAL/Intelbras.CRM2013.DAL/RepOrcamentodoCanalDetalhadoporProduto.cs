using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using System.Text;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{

    public class RepOrcamentodoCanalDetalhadoporProduto<T> : CrmServiceRepository<T>, IOrcamentodoCanalDetalhadoporProduto<T>
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

        public List<T> ObterOrcCanalDetalhadoProdutos(Guid orcamentocanalprodutoId, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcdocanalporprodutoid", ConditionOperator.Equal, orcamentocanalprodutoId);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterDetalhadoProdutos(Guid orcamentounidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            query.AddLink("itbc_orcdocanalporproduto", "itbc_orcdocanalporprodutoid", "itbc_orcdocanalporprodutoid")
                .AddLink("itbc_orcdocanalporsubfamilia", "itbc_orcamentodocanalporsubfamilia", "itbc_orcdocanalporsubfamiliaid")
                .AddLink("itbc_orcamentodocanalporfamilia", "itbc_orcamentodocanalporfamiliaid", "itbc_orcamentodocanalporfamiliaid")
                .AddLink("itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmentoid")
                .AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
                .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                .AddLink("itbc_orcamentodaunidade", "new_orcamentoporunidadeid", "itbc_orcamentodaunidadeid")
                .LinkCriteria.AddCondition("itbc_orcamentodaunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_canalid", OrderType.Ascending);
            OrderExpression ord2 = new OrderExpression("itbc_produtoid", OrderType.Ascending);
            query.Orders.Add(ord1);
            query.Orders.Add(ord2);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterDetalhadoProdutosManual(Guid orcamentounidadeId, Guid canalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);

            query.AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
                .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                .LinkCriteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_canalid", OrderType.Ascending);
            OrderExpression ord2 = new OrderExpression("itbc_produtoid", OrderType.Ascending);
            query.Orders.Add(ord1);
            query.Orders.Add(ord2);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarDetalheProdutosPorCanal(Guid orcamentounidadeId, Guid canalId, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_orcdocanalporproduto", "itbc_orcdocanalporprodutoid", "itbc_orcdocanalporprodutoid")
                .AddLink("itbc_orcdocanalporsubfamilia", "itbc_orcamentodocanalporsubfamilia", "itbc_orcdocanalporsubfamiliaid")
                .AddLink("itbc_orcamentodocanalporfamilia", "itbc_orcamentodocanalporfamiliaid", "itbc_orcamentodocanalporfamiliaid")
                .AddLink("itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmentoid")
                .AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
                .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                .LinkCriteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_canalid", OrderType.Ascending);
            OrderExpression ord2 = new OrderExpression("itbc_produtoid", OrderType.Ascending);
            query.Orders.Add(ord1);
            query.Orders.Add(ord2);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterOrcamentoProdutoDetalhadoManual(Guid orcamentodocanalId, Guid unidadenegocioId, Guid canalId, int ano, int trimestre, int mes)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentodocanalid", ConditionOperator.Equal, orcamentodocanalId);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Null);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
            query.AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
                .LinkCriteria.AddCondition("itbc_unidadedenegocio", ConditionOperator.Equal, unidadenegocioId);
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

        public T ObterOrcamentoProdutoDetalhado(Guid unidadenegocioId, Guid canalId, Guid produtoId, int ano, int trimestre, int mes)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //query.Criteria.AddCondition("itbc_unidadenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);

            query.AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
            .LinkCriteria.AddCondition("itbc_unidadedenegocio", ConditionOperator.Equal, unidadenegocioId);

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

        public List<T> ListarProdutoDetalhadoCanal(int ano, int trimestre)
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
        public DataTable ListarProdutoDetalhadoCanalDW(int ano, int trimestre)
        {
            return this.ListarProdutoDetalhadoCanalDW(ano, trimestre, null);
        }

        public DataTable ListarProdutoDetalhadoCanalDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("Select count(*), cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_Emitente,CD_familia,CD_subfamilia,CD_Item,cd_mes ");
            strSql.Append(", sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("and CD_Trimestre = {0} ", trimestre);
            if (lstOrcamentodaUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstOrcamentodaUnidade)
                    commandIn += string.Concat("'", item.UnidadeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }
            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_Emitente,CD_familia,CD_subfamilia,CD_Item,cd_mes");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

