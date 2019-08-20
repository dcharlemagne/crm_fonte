using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepTreinamentoCertificacao<T> : CrmServiceRepository<T>, ITreinamentoCertificacao<T>
    {

        public List<T> ListarPor(Guid treinamentoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_treinamcertifid", ConditionOperator.Equal, treinamentoId);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Int32 idCurso)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_idinterno", ConditionOperator.Equal, idCurso);
            query.Criteria.Conditions.Add(cond1);

            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }


        public T ObterPor(Guid treinamentolId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_treinamcertifid", ConditionOperator.Equal, treinamentolId);
            query.Criteria.Conditions.Add(cond1);

            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid treinamentoId, int status)
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
                EntityMoniker = new EntityReference("itbc_treinamcertif", treinamentoId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

    }
}
