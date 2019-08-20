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

    public class RepOrcamentodaUnidadeporSubFamilia<T> : CrmServiceRepository<T>, IOrcamentodaUnidadeporSubFamilia<T>
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

        public T ObterOrcamentoSubFamilia(Guid subfamiliaId, Guid orcamentofamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentoporfamiliaid", ConditionOperator.Equal, orcamentofamiliaId);
            query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, subfamiliaId);
            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterOrcSubFamiliaporProdTrimestre(Guid trimestreId, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_orcamentoporproduto", "itbc_orcamentoporsubfamiliaid", "itbc_orcamentoporsubfamilia").LinkCriteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_orcamentoporfamilia", "itbc_orcamentoporfamiliaid", "itbc_orcamentoporfamiliaid").AddLink("itbc_orcamentoporsegmento", "itbc_orcamentoporsegmento", "itbc_orcamentoporsegmentoid")
                .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid").LinkCriteria.AddCondition("itbc_orcamentoportrimestredaunidadeid", ConditionOperator.Equal, trimestreId);

            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];

        }

        public T ObterPorSubFamiliaTrimestreUnidade(Guid subfamiliaId, Guid orcunidadetrimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, subfamiliaId);
            //query.AddLink("itbc_orcamentoporproduto", "itbc_orcamentoporsubfamiliaid", "itbc_orcamentoporsubfamilia").LinkCriteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_orcamentoporfamilia", "itbc_orcamentoporfamiliaid", "itbc_orcamentoporfamiliaid").AddLink("itbc_orcamentoporsegmento", "itbc_orcamentoporsegmento", "itbc_orcamentoporsegmentoid")
                .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid").LinkCriteria.AddCondition("itbc_orcamentoportrimestredaunidadeid", ConditionOperator.Equal, orcunidadetrimestreId);

            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];

        }

        public T ObterPorSubFamiliaTrimestreUnidade(Guid unidadenegocioId, int ano, int trimestre, Guid segmentoId, Guid familiaId, Guid subfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_segmentoid", ConditionOperator.Equal, segmentoId);
            query.Criteria.AddCondition("itbc_familiadeprodutoid", ConditionOperator.Equal, familiaId);
            query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, subfamiliaId);
            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];

        }
        
        public List<T> ListarSubFamiliaPor(Guid familiaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentoporfamiliaid", ConditionOperator.Equal, familiaId);
            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOrcamentoSubFamilia(int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condi��es
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            #endregion

            #region Ordena��es
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        /********************      ACESSO SQL      ********************/
        public DataTable ListarOrcamentoSubFamiliaDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia ");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And CD_Trimestre = {0} ", trimestre);

            if (lstOrcamentodaUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstOrcamentodaUnidade)
                    commandIn += string.Concat("'", item.UnidadeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

