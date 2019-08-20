using System;
using System.Collections.Generic;
using System.Linq;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoPortfolio<T> : CrmServiceRepository<T>, IProdutoPortfolio<T>
    {
        //List<T> ListarPor(Guid portfolioId);
        public List<T> ListarPor(Guid portfolioId, List<Guid>lstSegmentoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_portfolioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, portfolioId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond2);

            if (lstSegmentoId != null)
            {
                if (lstSegmentoId.Count > 0)
                {
                    var link = query.AddLink("product", "itbc_productid", "productid");
                    Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_segmentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, lstSegmentoId);
                    link.LinkCriteria.Conditions.Add(cond3);
                }
            }

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPor(List<Guid> listId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_portfolioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, listId);

            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);

            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorProduto(Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_productid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoId);
            query.Criteria.Conditions.Add(cond1);


            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        //List<T> ListarPorProduto(Guid produtoId);
        //T ObterPor(Guid produtoPortfolioId);
        public T ObterPor(Guid produtoPortfolioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_productid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoPortfolioId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        //void AlterarStatus(Guid produtoPortfolioId, int status);
        public void AlterarStatus(Guid produtoPortfolioId, int status)
        {
            int stateCode;
            if (status == 0)
            {
                //Ativar
                stateCode = 0;
                status = 1;

            }
            else
            {
                //Inativar
                stateCode = 1;
                status = 2;
            }


            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_proddoport", produtoPortfolioId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);


        }
    }
}
