using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepSolicitacaoCadastro<T> : CrmServiceRepository<T>, ISolicitacaoCadastro<T>
    {
        public List<T> ListarPorConta(Guid itbc_solicitacaodecadastroid, DateTime? dtInicio, DateTime? dtFim, int? status)
        {
            var query = GetQueryExpression<T>(true);


            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_keyaccountourepresentanteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_solicitacaodecadastroid.ToString());
            query.Criteria.Conditions.Add(cond1);

            if (status.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, status.Value);
                query.Criteria.Conditions.Add(cond2);
            }

            if (dtInicio.HasValue && dtFim.HasValue)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, dtInicio.Value);
                query.Criteria.Conditions.Add(cond3);

                Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual, dtFim.Value);
                query.Criteria.Conditions.Add(cond4);
            }

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(String SolicitacaoCadastroNome)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, SolicitacaoCadastroNome);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }



        public T ObterPor(Guid itbc_solicitacaodecadastroid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_solicitacaodecadastroid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_solicitacaodecadastroid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid itbc_solicitacaodecadastroid, int status)
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
                EntityMoniker = new EntityReference("itbc_solicitacaodecadastro", itbc_solicitacaodecadastroid),
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
