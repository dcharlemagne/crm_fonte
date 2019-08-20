using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepParecer<T> : CrmServiceRepository<T>, IParecer<T>
    {
        public List<T> ListarPor(Guid itbc_parecer_tarefaId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_parecer_tarefaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_parecer_tarefaId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_parecerid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_parecerid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_parecerid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid itbc_parecerId, int status)
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
                EntityMoniker = new EntityReference("itbc_parecer", itbc_parecerId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }


        public T ObterPor(string pedidoEMS)
        {
            throw new NotImplementedException();
        }

        public void AtribuirEquipeParaPedido(Guid Equipe, Guid Pedido)
        {
            throw new NotImplementedException();
        }

        public bool AlterarStatus(Guid id, Domain.Enum.Pedido.StateCode stateCode, Domain.Enum.Pedido.RazaoStatus statusCode)
        {
            throw new NotImplementedException();
        }

        public bool FecharPedido(Guid pedidoId)
        {
            throw new NotImplementedException();
        }
    }
}
