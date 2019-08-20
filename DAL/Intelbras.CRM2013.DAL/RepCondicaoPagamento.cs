using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepCondicaoPagamento<T> : CrmServiceRepository<T>, ICondicaoPagamento<T>
    {
        public List<T> ListarPor(int? stateCode)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            ConditionExpression cond1 = new ConditionExpression("statecode", ConditionOperator.Equal, stateCode);
            
            #endregion

            #region Validações
            
            if (stateCode.HasValue)
                query.Criteria.Conditions.Add(cond1);

            query.Criteria.Conditions.Add(new ConditionExpression("itbc_condicao_pagamento", ConditionOperator.NotEqual, 0));
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_utilizadoparab2b", ConditionOperator.Equal, true));

            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_condicao_pagamentoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_condicao_pagamentoid", ConditionOperator.Equal, itbc_condicao_pagamentoid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(int itbc_condicao_pagamento)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_condicao_pagamento", ConditionOperator.Equal, itbc_condicao_pagamento);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Pedido pedido)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("salesorder", "itbc_condicao_pagamento", "itbc_condicao_pagamentoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("salesorderid", ConditionOperator.Equal, pedido.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public void AlterarStatus(Guid condicaoPagamentoId, int state, int status, Guid usuarioId)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_condicao_pagamento", condicaoPagamentoId),
                State = new OptionSetValue(state),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request, usuarioId);
        }

    }
}