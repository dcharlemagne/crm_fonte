using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepVeiculo<T> : CrmServiceRepository<T>, IVeiculo<T>
    {
        public List<T> ListarPorClienteParticipanteContrato(Guid clienteParticipanteId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_cliente_participante_do_contrato", ConditionOperator.Equal, clienteParticipanteId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorClientesParticipantesContrato(List<ClienteParticipante> lstClienteParticipante, string placa)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            string[] arrayClientesId = new string[lstClienteParticipante.Count];

            if(placa != null) {
                query.Criteria.AddCondition("itbc_placa", ConditionOperator.Equal, placa);
            }

            for (int i = 0; i < lstClienteParticipante.Count; i++) arrayClientesId[i] = lstClienteParticipante[i].Id.ToString();

            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cliente_participante_do_contrato", ConditionOperator.In, arrayClientesId));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorPlacaVeiculo(string numeroplaca)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_placa", ConditionOperator.Equal, numeroplaca));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }


    }
}
