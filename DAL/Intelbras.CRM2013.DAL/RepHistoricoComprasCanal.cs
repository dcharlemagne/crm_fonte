using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepHistoricoComprasCanal<T> : CrmServiceRepository<T>, IHistoricoComprasCanal<T>
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
        public T ObterPor(Guid UnidadeNeg, Int32 trimestre, Int32 ano, Guid canal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

           
                ConditionExpression cond1 = new ConditionExpression("itbc_unidadedenegocioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, UnidadeNeg);
                query.Criteria.Conditions.Add(cond1);
           
                ConditionExpression cond2 = new ConditionExpression("itbc_trimestre", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, trimestre);
                query.Criteria.Conditions.Add(cond2);
           
                ConditionExpression cond3 = new ConditionExpression("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canal);
                query.Criteria.Conditions.Add(cond3);

                ConditionExpression cond4 = new ConditionExpression("itbc_ano", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ano);
                query.Criteria.Conditions.Add(cond4);

                ConditionExpression cond5 = new ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);
                query.Criteria.Conditions.Add(cond5);


            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public DataTable ListarPor(string ano, string trimestre)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.AppendLine("Select cd_unidade_negocio, ");
            strSql.AppendLine("cd_ano, ");
            strSql.AppendLine("cd_guid, ");
            strSql.AppendLine("cd_canal, ");
            strSql.AppendLine("cd_trimestre ");
            strSql.AppendLine(",sum([NM_Vl_Liquido_Total]) Valor ");
            strSql.AppendLine(" From viewFaturamentoCRM ");
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
            strSql.AppendLine(" group by CD_Unidade_Negocio, CD_Ano, CD_Trimestre, cd_guid, cd_canal ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}