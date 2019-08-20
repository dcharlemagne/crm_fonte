using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepPortfolio<T> : CrmServiceRepository<T>, IPortfolio<T>
    {

        //public List<T> ListarPor(Guid? unidadeNegocioId, int? tipo)
        //{
        //    var query = GetQueryExpression<T>(true);

        //    #region Condições

        //    query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

        //    if (unidadeNegocioId.HasValue)
        //        query.Criteria.AddCondition("itbc_tipoid", ConditionOperator.Equal, tipo);

        //    if (tipo.HasValue)
        //        query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.ToString());

        //    #endregion

        //    return (List<T>)this.RetrieveMultiple(query).List;
        //}

        //public List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipos)
        //{
        //    var query = GetQueryExpression<T>(true);

        //    #region Condições

        //    query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
        //    query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.In, unidadesNegocioIds));
        //    query.Criteria.AddCondition(new ConditionExpression("itbc_tipoid", ConditionOperator.In, tipos));


        //    #endregion

        //    return (List<T>)this.RetrieveMultiple(query).List;
        //}
        //public List<T> ListarPor(Guid? unidadeNegocioId, int? tipo, Guid? classificacao, Guid? categoria, Guid portfolioId)
        //{
        //    var query = GetQueryExpression<T>(true);

        //    #region Condições

        //    query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

        //    query.Criteria.AddCondition("itbc_portfolioid", ConditionOperator.NotEqual, portfolioId.ToString());

        //    if (unidadeNegocioId.HasValue)
        //        query.Criteria.AddCondition("itbc_tipoid", ConditionOperator.Equal, tipo);

        //    if (tipo.HasValue)
        //        query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.ToString());

        //    if (classificacao.HasValue)
        //        query.Criteria.AddCondition("itbc_classificacaoid", ConditionOperator.Equal, classificacao.ToString());

        //    if (categoria.HasValue)
        //        query.Criteria.AddCondition("itbc_categoriaid", ConditionOperator.Equal, categoria.ToString());

        //    #endregion

        //    return (List<T>)this.RetrieveMultiple(query).List;
        //}

        public List<T> ListarPor(Guid? unidadeNegocioId, int? tipo, Guid? classificacao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_tipoid", ConditionOperator.Equal, tipo);

            if (tipo.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.ToString());

            if (classificacao.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", ConditionOperator.Equal, classificacao.ToString());

         
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPor(Guid? unidadeNegocioId, int? tipo, Guid? classificacaoId, Guid portfolioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);


            query.Criteria.AddCondition("itbc_portfolioid", ConditionOperator.NotEqual, portfolioId.ToString());


            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_tipoid", ConditionOperator.Equal, tipo);

            if (tipo.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.ToString());

            if (classificacaoId.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", ConditionOperator.Equal, classificacaoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPor(List<Guid> unidadesNegocioIds, int tipos, Guid? classificacao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.In, unidadesNegocioIds));
            query.Criteria.AddCondition(new ConditionExpression("itbc_tipoid", ConditionOperator.Equal, tipos));
            query.Criteria.AddCondition(new ConditionExpression("itbc_classificacaoid", ConditionOperator.In, classificacao));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPor(List<Guid> unidadesNegocioIds, int tipos, List<Guid> classificacao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.In, unidadesNegocioIds));
            query.Criteria.AddCondition(new ConditionExpression("itbc_tipoid", ConditionOperator.Equal, tipos));
            query.Criteria.AddCondition(new ConditionExpression("itbc_classificacaoid", ConditionOperator.In, classificacao));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipos, Guid? classificacaoIds)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.In, unidadesNegocioIds));
            query.Criteria.AddCondition(new ConditionExpression("itbc_tipoid", ConditionOperator.In, tipos));
            query.Criteria.AddCondition(new ConditionExpression("itbc_classificacaoid", ConditionOperator.In, classificacaoIds));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }              
        public List<T> ListarPor(List<Guid> unidadesNegocioIds, List<int> tipos, List<Guid> classificacaoIds)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.In, unidadesNegocioIds));
            query.Criteria.AddCondition(new ConditionExpression("itbc_tipoid", ConditionOperator.In, tipos));
            query.Criteria.AddCondition(new ConditionExpression("itbc_classificacaoid", ConditionOperator.In, classificacaoIds));


            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPorCrosseling(List<Guid> unidadesNegocioIds, int tipo, Guid? classificacaoIds)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(new ConditionExpression("itbc_businessunitid", ConditionOperator.NotIn, unidadesNegocioIds));
            //new 
            query.Criteria.AddCondition(new ConditionExpression("itbc_classificacaoid", ConditionOperator.In, classificacaoIds));
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPorProduto(Guid produtoId, bool apenasAtivos = true, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            
            #region Condições

            query.AddLink("itbc_proddoport", "itbc_portfolioid", "itbc_portfolioid")
                .LinkCriteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId);

            if (apenasAtivos)
            {
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Portfolio.StateCode.Ativo);
                query.LinkEntities[0].LinkCriteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.ProdutoPortfolio.StateCode.Ativo);
            }

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid portfolioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_portfolioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, portfolioId);
            query.Criteria.Conditions.Add(cond1);
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
    }
}
