﻿using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Data;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepLinhaCorteDistribuidor<T> : CrmServiceRepository<T>, ILinhaCorteDistribuidor<T>
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
        public List<T> ListarPor(List<Guid> unidadeNegocioId, Guid? estadoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            if (unidadeNegocioId != null)
                query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.In, unidadeNegocioId));

            if (estadoId.HasValue)
                query.AddLink("itbc_itbc_linhadecorte_itbc_estado", "itbc_linhadecorteid", "itbc_linhadecorteid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ListarPorUnidade(Guid unidadeNegocioId, Guid? estadoId, Guid classificacaoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            ConditionExpression cond1 = new ConditionExpression("itbc_classificacaoid", ConditionOperator.Equal, classificacaoId);
            query.Criteria.Conditions.Add(cond1);

            ConditionExpression cond2 = new ConditionExpression("statecode", ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond2);

            ConditionExpression cond3 = new ConditionExpression("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.Conditions.Add(cond3);

            if (estadoId.HasValue)
                query.AddLink("itbc_itbc_linhadecorte_itbc_estado", "itbc_linhadecorteid", "itbc_linhadecorteid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPort(Guid classificacaoid, Guid? estadoId, int? capitalOuInterior, Guid? categoriaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            ConditionExpression cond1 = new ConditionExpression("itbc_classificacaoid", ConditionOperator.Equal, classificacaoid);
            query.Criteria.Conditions.Add(cond1);

            ConditionExpression cond2 = new ConditionExpression("statecode", ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond2);

            if (capitalOuInterior != null)
            {
                ConditionExpression cond3 = new ConditionExpression("itbc_capitalouinterior", ConditionOperator.Equal, capitalOuInterior);
                query.Criteria.Conditions.Add(cond3);
            }

            if (categoriaId != null)
            {
                ConditionExpression cond4 = new ConditionExpression("itbc_categoriaid", ConditionOperator.Equal, categoriaId);
                query.Criteria.Conditions.Add(cond4);
            }

            query.AddLink("itbc_itbc_linhadecorte_itbc_estado", "itbc_linhadecorteid", "itbc_linhadecorteid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());
            #endregion

            var colecao = this.RetrieveMultiple(query);



            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_linhadecorteid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_linhadecorteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_linhadecorteid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond2);
            
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
       
        public DataTable FaturamentoUltimoTrimestreRevendaDW(int ano, int trimestreAnterior, Guid canalid)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;
            
            strSql.Append("Select SK_Unidade_Negocio,CD_Faturamento,CD_SellOut,");
            strSql.Append("CD_Revenda_SellOut,TX_Revenda_SellOut,CD_Classificacao_Canal,TX_Classificacao_Canal,");
            strSql.Append("CD_Unidade_Negocio,TX_Unidade_Negocio,TX_Ano,CD_Trimestre,sum(NM_Vl_Liquido) NM_Vl_Liquido ");

            strSql.Append("From ViewFactFaturamentoConsolidado");
            strSql.AppendFormat(" Where TX_Ano = {0} ", "'" + ano.ToString() + "' ");
            strSql.AppendFormat("And CD_Trimestre = {0} ", trimestreAnterior);
            strSql.AppendFormat("And CD_Revenda_SellOut = {0} ", "'" + canalid + "'");

            strSql.Append("Group by SK_Unidade_Negocio,CD_Faturamento,CD_SellOut, CD_Revenda_SellOut,");
            strSql.Append("TX_Revenda_SellOut, CD_Classificacao_Canal, TX_Classificacao_Canal, CD_Unidade_Negocio,");
            strSql.Append("TX_Unidade_Negocio, TX_Ano,CD_Trimestre");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable FaturamentoGlobalTrimestreDW(int ano, int trimestreAnterior, string canalid)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("SELECT f.CD_Revenda_SellOut, ");
            strSql.Append("sum(f.NM_Vl_Liquido) as FaturamentoTOTAL ");
            strSql.Append("FROM ViewFactFaturamentoConsolidado f ");
            strSql.AppendFormat("  where f.CD_Revenda_SellOut in {0} ", "(" + canalid + ") ");
            strSql.AppendFormat("	and f.TX_Ano = {0} ", "'" + ano.ToString() + "' ");
            strSql.AppendFormat("   and f.CD_SellOut = 1 ");
            strSql.AppendFormat("And f.CD_Trimestre = {0} ", trimestreAnterior );
            strSql.Append("group by f.CD_Revenda_SellOut");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
        public DataTable FaturamentoUnidadeTrimestreDW(int ano, int trimestreAnterior, string canalid)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("SELECT f.CD_Revenda_SellOut, ");
            strSql.Append("f.CD_Unidade_Negocio, ");
            strSql.Append("sum(f.NM_Vl_Liquido) as FaturamentoUnidade ");            
            strSql.Append("FROM ViewFactFaturamentoConsolidado f ");
            strSql.AppendFormat(" where f.CD_Revenda_SellOut in {0} ", "(" + canalid + ") ");
            strSql.AppendFormat("	and f.TX_Ano = {0} ", "'" + ano.ToString() + "' ");
            strSql.AppendFormat("   and f.CD_SellOut = 1  ");
            strSql.AppendFormat("   And f.CD_Trimestre = {0} ", trimestreAnterior);
            strSql.Append("group by f.CD_Revenda_SellOut, f.CD_Unidade_Negocio");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable FaturamentoGlobalSemestreDW(int ano1, int ano2, string trimestres, string canalid)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;
                        
            strSql.Append("SELECT f.CD_Revenda_SellOut, sum(f.NM_Vl_Liquido) as FaturamentoTOTAL ");
            strSql.Append("FROM ViewFactFaturamentoConsolidado f ");            
            strSql.AppendFormat("  where f.CD_Revenda_SellOut in {0} ", "(" + canalid + ") ");
            strSql.AppendFormat("	and f.TX_Ano = {0} ", "'" + ano1.ToString() + "' ");
            strSql.AppendFormat("   and f.CD_SellOut = 1 ");
            strSql.AppendFormat("And f.CD_Trimestre in {0} ", "(" + trimestres + ") ");
            strSql.Append("group by f.CD_Revenda_SellOut");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
        public DataTable FaturamentoUnidadeSemestreDW(int ano1, int ano2, string trimestres, string canalid)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("SELECT f.CD_Revenda_SellOut, ");
            strSql.Append("f.CD_Unidade_Negocio, ");
            strSql.Append("sum(f.NM_Vl_Liquido) as FaturamentoUnidade ");
            strSql.Append("FROM ViewFactFaturamentoConsolidado f ");            
            strSql.AppendFormat(" where f.CD_Revenda_SellOut in {0} ", "(" + canalid + ") ");
            strSql.AppendFormat("	and f.TX_Ano = {0} ", "'" + ano1.ToString() + "' ");
            strSql.AppendFormat("   and f.CD_SellOut = 1  ");
            strSql.AppendFormat("   And f.CD_Trimestre in {0} ", "(" + trimestres + ") ");
            strSql.Append("group by f.CD_Revenda_SellOut, f.CD_Unidade_Negocio");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public DataTable FaturamentoLinhasCorteDWTrimestre(string canalid, string x)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("SELECT l.CD_Categoria_Canal, ");
            strSql.Append("l.cd_guid, ");            
            strSql.Append("l.CD_Unidade_Negocio, "); 
            strSql.Append("l.TX_Unidade_Negocio, ");
            strSql.Append("l.TX_Categoria_Canal, ");
            strSql.Append("(l.NM_Vl_Linha_Corte*"+x+") as NM_Vl_Linha_Corte ");
            strSql.Append("FROM ViewFactLinhaCorte l ");            
            strSql.AppendFormat("where l.cd_guid in {0} ", "(" + canalid + ") ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }       
    }
}