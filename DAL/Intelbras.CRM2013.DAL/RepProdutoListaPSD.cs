using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoListaPSD<T> : CrmServiceRepository<T>, IProdutoListaPSD<T>
    {


        public T ObterPor(Guid produtoListaPSDId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_produtosdalistapsdidid", ConditionOperator.Equal, produtoListaPSDId);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }


        public List<T> ListarPor(Guid listaPrecoPSDId, Guid? produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StateCode.ativo);
            query.Criteria.AddCondition("itbc_psdid", ConditionOperator.Equal, listaPrecoPSDId);

            if (produtoId.HasValue)
                query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Produto">Produto que será procurado</param>
        /// <param name="EstadoProduto">Estado da Lista Preço PSD pesquisada</param>
        /// <param name="UnidadeNegocioProduto">Unidade de Negócio da lista de preço psd pesquisada</param>
        /// <returns></returns>
        public T ObterPor(Guid Produto, Guid EstadoProduto, Guid UnidadeNegocioProduto)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, Produto);

            //LinkEntity link = new LinkEntity();
            query.AddLink("itbc_psdid", "itbc_psdid", "itbc_psdidid").LinkCriteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, UnidadeNegocioProduto.ToString());
            query.LinkEntities[0].AddLink("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", "itbc_psdidid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, EstadoProduto.ToString());
            
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
