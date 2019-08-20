using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepArquivoDeSellOut<T> : CrmServiceRepository<T>, IArquivoDeSellOut<T>
    {
        public List<T> ListarPor(Guid? Canal, int? RazaoDoStatus, DateTime? DataDeEnvioInicio, DateTime? DataDeEnvioFim)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            if (Canal.HasValue)
            {
                ConditionExpression cond1 = new ConditionExpression("itbc_canalid", ConditionOperator.Equal, Canal);
                query.Criteria.Conditions.Add(cond1);
            }
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            if (RazaoDoStatus.HasValue)
            {
                ConditionExpression cond2 = new ConditionExpression("statuscode", ConditionOperator.Equal, RazaoDoStatus.Value);
                query.Criteria.Conditions.Add(cond2);
            }

            if (DataDeEnvioInicio.HasValue)
            {
                ConditionExpression cond3 = new ConditionExpression("itbc_datadeenvio", ConditionOperator.GreaterEqual, DataDeEnvioInicio.Value);
                query.Criteria.Conditions.Add(cond3);
            }

            if (DataDeEnvioFim.HasValue)
            {
                ConditionExpression cond4 = new ConditionExpression("itbc_datadeenvio", ConditionOperator.LessEqual, DataDeEnvioFim.Value);
                query.Criteria.Conditions.Add(cond4);
            }

            #endregion

            OrderExpression ord1 = new OrderExpression("itbc_datadeenvio", OrderType.Descending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public Boolean AlterarRazaoStatus(Guid ArquivoDeSelloutId, int razaoStatus)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_arquivodesellout", ArquivoDeSelloutId),
                State = new OptionSetValue(0),
                Status = new OptionSetValue(razaoStatus)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public Boolean AlterarSituacao(Guid ArquivoDeSelloutId, int status)
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
                EntityMoniker = new EntityReference("itbc_arquivodesellout", ArquivoDeSelloutId),
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