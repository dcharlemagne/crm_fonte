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

    public class RepHistoricoComprasProdutoMes<T> : CrmServiceRepository<T>, IHistoricoComprasProdutoMes<T>
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
            strSql.Append("Select  ");
            strSql.Append("cd_canal, ");
            strSql.Append("CD_Mes, ");
            strSql.Append("cd_ano, ");
            strSql.Append("cd_item, ");
            strSql.Append("cd_trimestre, ");
            strSql.Append(" sum([NM_Vl_Liquido_Total]) Valor, ");
            strSql.Append(" sum([NM_Quantidade_Total]) Qtd ");
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
            strSql.Append(" group by CD_Mes, CD_Ano, CD_Trimestre, cd_canal, cd_item ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public T ObterPor(Int32 trimestre, Int32 ano, Guid produto, Int32 mes)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições


            ConditionExpression cond1 = new ConditionExpression("itbc_trimestre", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, trimestre);
            query.Criteria.Conditions.Add(cond1);

            ConditionExpression cond2 = new ConditionExpression("itbc_ano", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ano);
            query.Criteria.Conditions.Add(cond2);

            ConditionExpression cond3 = new ConditionExpression("itbc_produtoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produto);
            query.Criteria.Conditions.Add(cond3);

            ConditionExpression cond4 = new ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);
            query.Criteria.Conditions.Add(cond4);

            ConditionExpression cond5 = new ConditionExpression("itbc_mes", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, mes);
            query.Criteria.Conditions.Add(cond5);
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