using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.DAL
{
    public class RepValorDoServicoPorPosto<T> : CrmServiceRepository<T>, IValorDoServicoPorPosto<T>
    {
        public decimal ObterMaiorValorPor(Domain.Model.Conta cliente)
        {
            decimal maiorValor = decimal.MinValue;

            QueryExpression query = new QueryExpression("new_valor_servico_posto");
            query.ColumnSet.AddColumns("new_valor_servico_postoid", "new_valor");
            query.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            query.Orders.Add(new OrderExpression("new_valor", OrderType.Descending));

            EntityCollection colecao = base.Provider.RetrieveMultiple(query);
            if (colecao.Entities.Count > 0)
            {
                var valorServico = colecao.Entities[0];
                if (valorServico.Attributes.Contains("new_valor"))
                    maiorValor = ((Money)valorServico["new_valor"]).Value;
            }

            return maiorValor;
        }

        public decimal ObterMaiorValorPor(Product produto)
        {
            decimal maiorValor = decimal.MinValue;

            QueryExpression query = new QueryExpression("new_valor_servico_posto");
            query.ColumnSet.AddColumns("new_valor_servico_postoid", "new_valor");
            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produto.Id));
            query.Orders.Add(new OrderExpression("new_valor", OrderType.Descending));

            EntityCollection colecao = base.Provider.RetrieveMultiple(query);
            if (colecao.Entities.Count > 0)
            {
                var valorServico = colecao.Entities[0];
                if (valorServico.Attributes.Contains("new_valor"))
                    maiorValor = ((Money)valorServico["new_valor"]).Value;
            }

            return maiorValor;
        }

        public decimal ObterMaiorValorPor(Domain.Model.Conta cliente, Product produto)
        {
            decimal maiorValor = decimal.MinValue;

            QueryExpression query = new QueryExpression("new_valor_servico_posto");
            query.ColumnSet.AddColumn("new_valor");
            query.PageInfo = new PagingInfo() { Count = 1, PageNumber = 1 };
            query.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produto.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));
            query.Orders.Add(new OrderExpression("new_valor", OrderType.Descending));

            EntityCollection colecao = base.Provider.RetrieveMultiple(query);
            if (colecao.Entities.Count > 0)
            {
                var valorServico = colecao.Entities[0];
                if (valorServico.Attributes.Contains("new_valor"))
                    maiorValor = ((Money)valorServico["new_valor"]).Value;
            }

            return maiorValor;
        }

        public decimal ObterMaiorValorPorLinhaComercialDoProduto(Domain.Model.Conta cliente, Product produto)
        {
            decimal maiorValor = decimal.MinValue;

            QueryExpression query = new QueryExpression("new_valor_servico_posto");
            query.ColumnSet.AddColumn("new_valor");
            ConditionExpression condition2 = new ConditionExpression()
            {
                AttributeName = "new_clienteid",
                Operator = ConditionOperator.Equal
            };
            condition2.Values.Add(cliente.Id);

            query.Criteria = new FilterExpression();
            query.Criteria.Conditions.Add(condition2);

            LinkEntity linkEntity1 = new LinkEntity();
            linkEntity1.JoinOperator = JoinOperator.Natural;
            linkEntity1.LinkFromEntityName = "new_valor_servico_posto";
            linkEntity1.LinkFromAttributeName = "new_linha_unidade_negocioid";
            linkEntity1.LinkToEntityName = "new_linha_unidade_negocio";
            linkEntity1.LinkToAttributeName = "new_linha_unidade_negocioid";

            LinkEntity linkEntity2 = new LinkEntity();
            linkEntity2.JoinOperator = JoinOperator.Natural;
            linkEntity2.LinkFromEntityName = "new_linha_unidade_negocio";
            linkEntity2.LinkFromAttributeName = "new_linha_unidade_negocioid";
            linkEntity2.LinkToEntityName = "product";
            //linkEntity2.LinkToAttributeName = "new_linha_unidade_negocioid";
            linkEntity2.LinkToAttributeName = "itbc_linha_unidade_negocioid";

            linkEntity2.LinkCriteria = new FilterExpression();
            linkEntity2.LinkCriteria.FilterOperator = LogicalOperator.And;

            ConditionExpression condition1 = new ConditionExpression()
            {
                AttributeName = "productid",
                Operator = ConditionOperator.Equal
            };
            condition1.Values.Add(produto.Id);

            linkEntity2.LinkCriteria.Conditions.Add(condition1);

            linkEntity1.LinkEntities.Add(linkEntity2);

            query.LinkEntities.Add(linkEntity1);

            OrderExpression order = new OrderExpression()
            {
                AttributeName = "new_valor",
                OrderType = OrderType.Descending
            };

            query.Orders.Add(order);

            EntityCollection colecao = base.Provider.RetrieveMultiple(query);
            if (colecao.Entities.Count > 0)
            {
                var valorServico = colecao.Entities[0];
                if (valorServico.Attributes.Contains("new_valor"))
                    maiorValor = ((Money)valorServico["new_valor"]).Value;
            }

            return maiorValor;
        }
    }
}