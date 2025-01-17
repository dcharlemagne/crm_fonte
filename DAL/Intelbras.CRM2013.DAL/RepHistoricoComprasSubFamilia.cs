﻿using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using System.Text;


namespace Intelbras.CRM2013.DAL
{

    public class RepHistoricoComprasSubFamilia<T> : CrmServiceRepository<T>, IHistoricoComprasSubFamilia<T>
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
        public DataTable ListarPor(string ano, string trimestre)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select CD_Unidade_Negocio, ");
            strSql.Append("cd_canal, ");
            strSql.Append("cd_segmento, ");
            strSql.Append("CD_Familia, ");
            strSql.Append("CD_SubFamilia, ");
            strSql.Append("cd_trimestre, ");
            strSql.Append(" sum([NM_Vl_Liquido_Total]) Valor ");
            strSql.Append(" From viewFaturamentoCRM ");
            strSql.AppendFormat(" Where CD_Ano = {0} ", ano);

            Intelbras.CRM2013.Domain.Servicos.RepositoryService RepositoryService = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(this.OrganizationName, this.IsOffline);
            var parametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Intelbras.CRM2013.Domain.Enum.ParametroGlobal.Parametrizar.GruposEstoqueGeracaoOrcamentosMeta);
            if (parametroGlobal == null || string.IsNullOrEmpty(parametroGlobal.Valor))
            {
                throw new ArgumentException("(CRM) Não foi encontrado Parametro Global [" + (int)Intelbras.CRM2013.Domain.Enum.ParametroGlobal.Parametrizar.GruposEstoqueGeracaoOrcamentosMeta + "].");
            }
            else
                strSql.AppendFormat(" And CD_Grupo_Estoque IN ({0}) ", parametroGlobal.Valor.Replace(';', ','));

            strSql.AppendFormat(" And cd_trimestre = {0} ", trimestre);
            strSql.AppendLine(" AND CD_Canal = 1 ");
            strSql.Append(" group by CD_Unidade_Negocio, cd_canal, cd_segmento, CD_Familia, CD_SubFamilia,  CD_Trimestre ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public T ObterPor(Guid UnidadeNeg, Guid subfamilia, Guid segmento, Guid familia,int ano,int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            ConditionExpression cond1 = new ConditionExpression("itbc_unidadedenegocioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, UnidadeNeg);
            query.Criteria.Conditions.Add(cond1);

            ConditionExpression cond2 = new ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);
            query.Criteria.Conditions.Add(cond2);

            ConditionExpression cond3 = new ConditionExpression("itbc_segmentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, segmento);
            query.Criteria.Conditions.Add(cond3);

            ConditionExpression cond4 = new ConditionExpression("itbc_subfamiliaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, subfamilia);
            query.Criteria.Conditions.Add(cond4);

            ConditionExpression cond5 = new ConditionExpression("itbc_familiadeprodutoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, familia);
            query.Criteria.Conditions.Add(cond5);

            ConditionExpression cond6 = new ConditionExpression("itbc_ano", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ano);
            query.Criteria.Conditions.Add(cond6);

            ConditionExpression cond7 = new ConditionExpression("itbc_trimestre", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, trimestre);
            query.Criteria.Conditions.Add(cond7);

            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}