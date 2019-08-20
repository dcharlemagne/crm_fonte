using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepOrcamentodaUnidade<T> : CrmServiceRepository<T>, IOrcamentodaUnidade<T>
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

        public List<T> ObterOrcamentoParaGerarPlanilha(int status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            ConditionExpression cond1 = new ConditionExpression("statuscode", ConditionOperator.Equal, status);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("createdon", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOrcamentos(int ano)
        {
            return ListarOrcamentos(Guid.Empty, ano);
        }

        public List<T> ListarOrcamentos(Guid unidadenegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            if (!unidadenegocioId.Equals(Guid.Empty))
                query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterOrcamentoParaGerarPlanilhaManual(int status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            ConditionExpression cond1 = new ConditionExpression("itbc_razaodostatusoramentomanual", ConditionOperator.Equal, status);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("createdon", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        /********************      ACESSO SQL      ********************/
        public DataTable ObterCapaDW(int ano)
        {
            return this.ObterCapaDW(ano, null);
        }

        public DataTable ObterCapaDW(int ano, List<OrcamentodaUnidade> lstOrcamento)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select count(*), cd_ano,CD_Unidade_Negocio,sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("from viewFaturamentoCRM ");
            strSql.AppendFormat("where CD_Ano = {0} ", ano);

            if (lstOrcamento != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstOrcamento)
                    commandIn += string.Concat("'", item.UnidadeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("group by cd_ano,CD_Unidade_Negocio ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public void ListarOrcamentosRealizados()
        {
            ArrayList list = new ArrayList();
            list.Add(new SqlParameter("@ID", Guid.NewGuid()));
            list.Add(new SqlParameter("@teste", Guid.NewGuid()));

            DataTable dt = DataBaseSqlServer.executeQuery("select * from MetasCapa");


            //SqlCommand cmd = new SqlCommand("selecionaContatosPorCidade");
            //cmd.CommandType = CommandType.StoredProcedure;

            //foreach (SqlParameter param in list)
            //    cmd.Parameters.Add(param);




        }

    }
}

