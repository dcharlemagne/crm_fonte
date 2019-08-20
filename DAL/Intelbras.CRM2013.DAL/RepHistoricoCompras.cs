using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using System.Text;


namespace Intelbras.CRM2013.DAL
{
    
    public class RepHistoricoCompras<T> : CrmServiceRepository<T>, IHistoricoCompras<T>
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
        public DataTable ListarPor(string ano)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_unidade_negocio, ");
            strSql.Append("cd_ano, ");
            strSql.Append("sum([NM_Vl_Liquido_Total]) Valor ");
            strSql.Append(" From viewFaturamentoCRM ");
            strSql.AppendFormat(" Where CD_Ano = {0} ", ano);

            Intelbras.CRM2013.Domain.Servicos.RepositoryService RepositoryService = new Intelbras.CRM2013.Domain.Servicos.RepositoryService(this.OrganizationName, this.IsOffline);
            var parametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Intelbras.CRM2013.Domain.Enum.ParametroGlobal.Parametrizar.GruposEstoqueGeracaoOrcamentosMeta);
            if (parametroGlobal == null || string.IsNullOrEmpty(parametroGlobal.Valor))
            {
                throw new ArgumentException("(CRM) Não foi encontrado Parametro Global [" + (int)Intelbras.CRM2013.Domain.Enum.ParametroGlobal.Parametrizar.GruposEstoqueGeracaoOrcamentosMeta + "].");
            }
            else
                strSql.AppendFormat(" And CD_Grupo_Estoque IN ({0}) ", parametroGlobal.Valor.Replace(';',','));

            strSql.AppendLine(" AND CD_Canal = 1 ");
            strSql.Append(" group by CD_Unidade_Negocio, CD_Ano ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        //obter ano e  unidadenegocio
        public T ObterPor(Guid UnidadeNeg, Int32 ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições

            
                ConditionExpression cond1 = new ConditionExpression("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, UnidadeNeg);
                query.Criteria.Conditions.Add(cond1);

                ConditionExpression cond2 = new ConditionExpression("itbc_ano", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ano);
                query.Criteria.Conditions.Add(cond2);
            
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