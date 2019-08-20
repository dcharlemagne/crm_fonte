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
    public class RepOrcamentodaUnidadeporTrimestre<T> : CrmServiceRepository<T>, IOrcamentodaUnidadeporTrimestre<T>
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

        public List<T> ObterOrcamentoTrimestre(Guid orcamentounidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterOrcamentoTrimestre(Guid orcamentounidadeId, int trimestre)
        {
            return this.ObterOrcamentoTrimestre(orcamentounidadeId, 0, trimestre);
        }

        public T ObterOrcamentoTrimestre(Guid orcamentounidadeId, int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            if (ano > 0)
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

        public T ObterOrcamentoTrimestre(Guid orcamentounidadeId, Guid trimestreId)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentoportrimestredaunidadeid", ConditionOperator.Equal, trimestreId);
            query.Criteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentounidadeId);
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

        public List<T> ListarOrcamentoTrimestre(int ano)
        {
            return this.ListarOrcamentoTrimestre(ano, 0);
        }

        public List<T> ListarOrcamentoTrimestre(int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            if (trimestre > 0)
                query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        /********************      ACESSO SQL      ********************/
        public DataTable ObterOrcamentoTrimestreDW(int ano)
        {
            return this.ObterOrcamentoTrimestreDW(ano, 0, null);
        }

        public DataTable ObterOrcamentoTrimestreDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,CD_Unidade_Negocio,cd_trimestre, ");
            strSql.Append("sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("from viewFaturamentoCRM ");
            strSql.AppendFormat("where CD_Ano = {0} ", ano);
            if (trimestre > 0)
                strSql.AppendFormat("and cd_trimestre = {0} ", trimestre);

            if (lstOrcamentodaUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstOrcamentodaUnidade)
                    commandIn += string.Concat("'", item.UnidadeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }


            strSql.Append("group by cd_ano,CD_Unidade_Negocio,cd_trimestre ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

    }
}

