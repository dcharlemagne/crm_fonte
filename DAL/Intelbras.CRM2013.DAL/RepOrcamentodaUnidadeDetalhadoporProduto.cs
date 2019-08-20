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

    public class RepOrcamentodaUnidadeDetalhadoporProduto<T> : CrmServiceRepository<T>, IOrcamentodaUnidadeDetalhadoporProduto<T>
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

        public List<T> ObterOrcDetalhadoProdutos(Guid orcamentoprodutoId)
        {
            return this.ObterOrcDetalhadoProdutos(orcamentoprodutoId, -1, Guid.Empty);
        }

        public List<T> ObterOrcDetalhadoProdutos(Guid orcamentoprodutoId, int trimestre)
        {
            return this.ObterOrcDetalhadoProdutos(orcamentoprodutoId, trimestre, Guid.Empty);
        }

        public List<T> ObterOrcDetalhadoProdutos(Guid orcamentoprodutoId, int trimestre, Guid produtoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentodoproduto", ConditionOperator.Equal, orcamentoprodutoId);
            if (trimestre != -1)
                query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            if (produtoid != Guid.Empty)
                query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoid);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterOrcDetalhadoProdutos(Guid produtoId, Guid orcamentounidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_orcamentoporproduto", "itbc_orcamentodoproduto", "itbc_orcamentoporprodutoid")
                .AddLink("itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamiliaid")
                .AddLink("itbc_orcamentoporfamilia", "itbc_orcamentoporfamiliaid", "itbc_orcamentoporfamiliaid")
                .AddLink("itbc_orcamentoporsegmento", "itbc_orcamentoporsegmento", "itbc_orcamentoporsegmentoid")
                .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                .LinkCriteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterProdutosDetalhados(Guid orcamentounidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            query.AddLink("itbc_orcamentoporproduto", "itbc_orcamentodoproduto", "itbc_orcamentoporprodutoid")
                .AddLink("itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamiliaid")
                .AddLink("itbc_orcamentoporfamilia", "itbc_orcamentoporfamiliaid", "itbc_orcamentoporfamiliaid")
                .AddLink("itbc_orcamentoporsegmento", "itbc_orcamentoporsegmento", "itbc_orcamentoporsegmentoid")
                .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                .AddLink("itbc_orcamentodaunidade", "new_orcamentoporunidadeid", "itbc_orcamentodaunidadeid")
                .LinkCriteria.AddCondition("itbc_orcamentodaunidadeid", ConditionOperator.Equal, orcamentounidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_produtoid", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOrcamentoProdutoDetalhado(int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterOrcamentoProdutoDetalhado(Guid orcamentodoprodutoId, int mes)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentodoproduto", ConditionOperator.Equal, orcamentodoprodutoId);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
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
        /********************      ACESSO SQL      ********************/
        public DataTable ListarOrcamentoProdutoMesDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select count(*), cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item,CD_Mes ");
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

            strSql.Append("Group By cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item,CD_Mes");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

