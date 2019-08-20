using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepSeguroConta<T> : CrmServiceRepository<T>, ISeguroConta<T>
    {
        public List<T> ListarPor(Guid itbc_segurodacontaid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_segurodacontaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_segurodacontaid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_segurodacontaid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_segurodacontaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_segurodacontaid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid itbc_segurodacontaid, int status)
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
                EntityMoniker = new EntityReference("itbc_segurodaconta", itbc_segurodacontaid),
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
