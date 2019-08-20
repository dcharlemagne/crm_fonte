using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Microsoft.Xrm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{

    public class RepMetaDetalhadadoKAporProduto<T> : CrmServiceRepository<T>, IMetaDetalhadadoKAporProduto<T>
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

        public List<T> ListarDetalheProdutosKaRepresentante(Guid metaId)
        {
            return this.ListarDetalheProdutosKaRepresentante(metaId, null, null);
        }

        public List<T> ListarTodosDetalheProdutosKaRepresentante(Guid metaId, int? pagina, int? contagem, Guid unidadeId, int ano)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            if (ano != null)
                query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            query.AddLink("itbc_metadokaporproduto", "itbc_metadokaporprodutoid", "itbc_metadokaporprodutoid")
                .AddLink("itbc_metadokaporsubfamilia", "itbc_metadokaporsubfamiliaid", "itbc_metadokaporsubfamiliaid")
                .AddLink("itbc_metadokaporfamilia", "itbc_metadokaporfamiliaid", "itbc_metadokaporfamiliaid")
                .AddLink("itbc_metadokaporsegmento", "itbc_metadokaporsegmentoid", "itbc_metadokaporsegmentoid")
                .AddLink("itbc_metatrimestreka", "itbc_metatrimestrekaid", "itbc_metatrimestrekaid")
                .AddLink("itbc_metakeyaccount", "itbc_metadokarepresentanteid", "itbc_metakeyaccountid")
                .AddLink("contact", "itbc_contact", "contactid")
                .AddLink("itbc_portfoliokeyaccountrepresentantes", "contactid", "itbc_contatoid")
                .LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarDetalheProdutosKaRepresentante(Guid metaId, Guid? karepresentanteId, Guid? produtoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            #region Condições
            if (produtoId != null)
                query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId.Value);

            if (karepresentanteId != null)
                query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, karepresentanteId.Value);

            query.AddLink("itbc_metadokaporproduto", "itbc_metadokaporprodutoid", "itbc_metadokaporprodutoid")
                .AddLink("itbc_metadokaporsubfamilia", "itbc_metadokaporsubfamiliaid", "itbc_metadokaporsubfamiliaid")
                .AddLink("itbc_metadokaporfamilia", "itbc_metadokaporfamiliaid", "itbc_metadokaporfamiliaid")
                .AddLink("itbc_metadokaporsegmento", "itbc_metadokaporsegmentoid", "itbc_metadokaporsegmentoid")
                .AddLink("itbc_metakeyaccount", "itbc_metadokarepresentanteid", "itbc_metakeyaccountid")
                .AddLink("itbc_metaportrimestre", "itbc_metaportrimestredaunidadeid", "itbc_metaportrimestreid")
                .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid potencialdokaporprodutoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadokaporprodutoid", ConditionOperator.Equal, potencialdokaporprodutoId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T Obter(Guid unidadenegocioId, Guid KARepresentanteId, int ano, int trimestre, int mes, Guid produtoId, Guid potencialkaprodutoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_kaourepresentanteid", ConditionOperator.Equal, KARepresentanteId);
            query.Criteria.AddCondition("itbc_metadokaporprodutoid", ConditionOperator.Equal, potencialkaprodutoId);
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
        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia,CD_Item,cd_mes ");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And cd_trimestre = {0} ", trimestre);

            if (lstMetas != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstMetas)
                    commandIn += string.Concat("'", item.UnidadedeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia,CD_Item,cd_mes ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

        public List<T> ListarPorAnoUnidadeNegocio(Guid unidadenegocioId, int ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            if(columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            query.AddLink("itbc_metadokaporproduto", "itbc_metadokaporprodutoid", "itbc_metadokaporprodutoid")
                 .AddLink("itbc_metadokaporsubfamilia", "itbc_metadokaporsubfamiliaid", "itbc_metadokaporsubfamiliaid");

            query.LinkEntities[0].LinkEntities[0].LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            query.LinkEntities[0].LinkEntities[0].LinkCriteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

    }
}

