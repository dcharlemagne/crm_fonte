using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using System.Text;
using Microsoft.Xrm.Sdk.Messages;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepOrcamentodaUnidadeporProduto<T> : CrmServiceRepository<T>, IOrcamentodaUnidadeporProduto<T>
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

        public T ObterOrcamentoporProduto(Guid produtoId, Guid orcamentosubfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentoporsubfamilia", ConditionOperator.Equal, orcamentosubfamiliaId);
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

        public T ObterOrcUnidadeProduto(Guid produtoId, Guid trimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            query.AddLink("itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamiliaid").AddLink(
                "itbc_orcamentoporfamilia", "itbc_orcamentoporfamiliaid", "itbc_orcamentoporfamiliaid")
                .AddLink("itbc_orcamentoporsegmento", "itbc_orcamentoporsegmento", "itbc_orcamentoporsegmentoid").LinkCriteria.AddCondition("itbc_orcamentoportrimestredaunidadeid", ConditionOperator.Equal, trimestreId);
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

        public List<T> ListarOrcUnidadeProduto(Guid subfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_orcamentoporsubfamilia", ConditionOperator.Equal, subfamiliaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutosOrcamento(Guid orcamentounidadeId)
        {
            return this.ListarProdutosOrcamento(orcamentounidadeId, null);
        }

        public List<T> ListarProdutosOrcamento(Guid orcamentounidadeId, List<Guid> lstIdProdutos)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            if (lstIdProdutos != null && lstIdProdutos.Count > 0)
                query.Criteria.AddCondition(new ConditionExpression("itbc_produtoid", ConditionOperator.NotIn, lstIdProdutos));

            query.AddLink("itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamiliaid")
                 .AddLink("itbc_orcamentoporfamilia", "itbc_orcamentoporfamiliaid", "itbc_orcamentoporfamiliaid")
                 .AddLink("itbc_orcamentoporsegmento", "itbc_orcamentoporsegmento", "itbc_orcamentoporsegmentoid")
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

        public List<T> ListarOrcamentoProduto(int ano, int trimestre)
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

        public T ObterOrcamentoporProduto(Guid produtoId, Guid unidadenegocioId, int ano, int trimestre, string codsegmento, string codfamilia, string codsubfamilia)
        {
            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            strFetchXml.Append("<entity name='itbc_orcamentoporproduto'>");
            strFetchXml.Append("<all-attributes />");
            strFetchXml.Append("<order attribute='itbc_name' descending='false' />");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.AppendFormat("<condition attribute='itbc_ano' operator='eq' value='{0}' />", ano);
            strFetchXml.AppendFormat("<condition attribute='itbc_trimestre' operator='eq' value='{0}' />", trimestre);
            strFetchXml.AppendFormat("<condition attribute='itbc_produtoid' operator='eq' value='{0}' />", produtoId);
            strFetchXml.Append("</filter>");
            strFetchXml.Append("<link-entity name='itbc_orcamentoporsubfamilia' from='itbc_orcamentoporsubfamiliaid' to='itbc_orcamentoporsubfamilia' alias='af'>");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.AppendFormat("<condition attribute='itbc_unidadedenegocioid' operator='eq'  value='{0}' />", unidadenegocioId);
            strFetchXml.Append("</filter>");
            strFetchXml.Append("<link-entity name='itbc_segmento' from='itbc_segmentoid' to='itbc_segmentoid' alias='ag'>");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.AppendFormat("<condition attribute='itbc_codigo_segmento' operator='eq' value='{0}' />", codsegmento);
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("<link-entity name='itbc_famildeprod' from='itbc_famildeprodid' to='itbc_familiadeprodutoid' alias='ah'>");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.AppendFormat("<condition attribute='itbc_codigo_familia' operator='eq' value='{0}' />", codfamilia);
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("<link-entity name='itbc_subfamiliadeproduto' from='itbc_subfamiliadeprodutoid' to='itbc_subfamiliaid' alias='ai'>");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.AppendFormat("<condition attribute='itbc_codigo_subfamilia' operator='eq' value='{0}' />", codsubfamilia);
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("</link-entity>");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            var lst = this.RetrieveMultiple(retrieveMultiple.Query).List;
            if (lst.Count > 0)
                return this.RetrieveMultiple(retrieveMultiple.Query).List[0];

            return default(T);

            //// Recupera o Resultado do FetchXml
            //var Registros = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;
            //if (Registros.Entities.Count > 0)
            //    return this.Retrieve(Registros.Entities[0].Id);

            //return default(T);
        }

        /********************      ACESSO SQL      ********************/
        public DataTable ListarOrcamentoProdutoDW(int ano, int trimestre, List<OrcamentodaUnidade> lstOrcamentodaUnidade)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select count(*), cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item ");
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

            strSql.Append("Group By cd_ano,CD_Unidade_Negocio,cd_trimestre,cd_segmento,CD_familia,CD_subfamilia,CD_Item ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

