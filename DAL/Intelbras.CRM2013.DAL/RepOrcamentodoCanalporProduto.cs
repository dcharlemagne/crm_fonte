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

    public class RepOrcamentodoCanalporProduto<T> : CrmServiceRepository<T>, IOrcamentodoCanalporProduto<T>
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
        
        public T ObterOrcamentoCanalporProduto(Guid produtoId, Guid orccanalsubfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentodocanalporsubfamilia", ConditionOperator.Equal, orccanalsubfamiliaId);
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

        public T ObterOrcCanalporProduto(Guid canalId, Guid produtoId, Guid trimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);

            query.AddLink("itbc_orcdocanalporsubfamilia", "itbc_orcamentodocanalporsubfamilia", "itbc_orcdocanalporsubfamiliaid").AddLink(
                "itbc_orcamentodocanalporfamilia", "itbc_orcamentodocanalporfamiliaid", "itbc_orcamentodocanalporfamiliaid")
                .AddLink("itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmentoid")
                .AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
                .LinkCriteria.AddCondition("itbc_orcamentoportrimestredaunidadeid", ConditionOperator.Equal, trimestreId);
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

        public T ObterOrcCanalProduto(Guid canalId, Guid produtoId, Guid orccanalsubfamiliaId, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_orcamentodocanalporsubfamilia", ConditionOperator.Equal, orccanalsubfamiliaId);
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

        public List<T> ListarProdutoCanalOrcamento(Guid orcamentounidadeId)
        {
            return this.ListarProdutoCanalOrcamento(orcamentounidadeId, null);
        }

        public List<T> ListarProdutoCanalOrcamento(Guid orcamentounidadeId, List<Guid> lstIdProdutos)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            if (lstIdProdutos != null && lstIdProdutos.Count > 0)
                query.Criteria.AddCondition(new ConditionExpression("itbc_produtoid", ConditionOperator.NotIn, lstIdProdutos));

            query.AddLink("itbc_orcdocanalporsubfamilia", "itbc_orcamentodocanalporsubfamilia", "itbc_orcdocanalporsubfamiliaid")
                 .AddLink("itbc_orcamentodocanalporfamilia", "itbc_orcamentodocanalporfamiliaid", "itbc_orcamentodocanalporfamiliaid")
                 .AddLink("itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmentoid")
                 .AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
                 .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                 .AddLink("itbc_orcamentodaunidade", "new_orcamentoporunidadeid", "itbc_orcamentodaunidadeid")
                 .LinkCriteria.AddCondition("itbc_orcamentodaunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutoCanalPorOrcamentoUnidade(Guid orcamentounidadeId, List<Guid> lstIdCanal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            if (lstIdCanal != null && lstIdCanal.Count > 0)
                query.Criteria.AddCondition(new ConditionExpression("itbc_canalid", ConditionOperator.NotIn, lstIdCanal));

            query.AddLink("itbc_orcdocanalporsubfamilia", "itbc_orcamentodocanalporsubfamilia", "itbc_orcdocanalporsubfamiliaid")
               .AddLink("itbc_orcamentodocanalporfamilia", "itbc_orcamentodocanalporfamiliaid", "itbc_orcamentodocanalporfamiliaid")
               .AddLink("itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmentoid")
               .AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
               .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
               .AddLink("itbc_orcamentodaunidade", "new_orcamentoporunidadeid", "itbc_orcamentodaunidadeid")
               .LinkCriteria.AddCondition("itbc_orcamentodaunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterOrcCanalProduto(Guid unidadenegocioId, int ano, int trimestre, Guid canalId, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            //query.Criteria.AddCondition("itbc_unidadenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            query.AddLink("itbc_orcdocanalporsubfamilia", "itbc_orcamentodocanalporsubfamilia", "itbc_orcdocanalporsubfamiliaid").LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId.ToString());
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

        public List<T> ListarCanalProduto(int ano, int trimestre)
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
        public DataTable ListarCanalProdutoDW(int ano, int trimestre)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select count(*), cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_Emitente,CD_familia,CD_subfamilia,CD_Item ");
            strSql.Append(", sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("and CD_Trimestre = {0} ", trimestre);
            //strSql.Append("and CD_Unidade_Negocio='ter' ");
            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_Emitente,CD_familia,CD_subfamilia,CD_Item");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable ListarCanalProdutoDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade)
        {
            string commandIn = string.Empty;

            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select count(*), cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_Emitente,CD_familia,CD_subfamilia,CD_Item ");
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

            strSql.Append(" Group By cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre,cd_segmento,CD_Emitente,CD_familia,CD_subfamilia,CD_Item");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

