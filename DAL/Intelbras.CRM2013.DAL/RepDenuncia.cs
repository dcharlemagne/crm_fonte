using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepDenuncia<T> : CrmServiceRepository<T>, IDenuncia<T>
    {
        public T ObterPor(Guid denunciaId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_denunciaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, denunciaId);
            query.Criteria.Conditions.Add(cond1);
            //Status
            //query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.Denuncia.Status.Ativo);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid denunciaId, int status)
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
                EntityMoniker = new EntityReference("itbc_denuncia", denunciaId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public List<T> ListarDenuncias(DateTime dtInicio, DateTime dtFim, List<Guid> lstDenunciantes, List<Guid> lstDenunciados, Guid? representanteId, int? situacaoDenuncia)
        {

            var query = GetQueryExpression<T>(true);
            if (lstDenunciantes.Count > 0)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_canaldenuncianteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, lstDenunciantes);
                query.Criteria.Conditions.Add(cond1);
            }

            if (lstDenunciados.Count > 0)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_account", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, lstDenunciados);
                query.Criteria.Conditions.Add(cond2);
            }

            if(situacaoDenuncia.HasValue)
                query.Criteria.AddCondition("itbc_status", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, situacaoDenuncia);

            if (representanteId.HasValue)
                query.Criteria.AddCondition("itbc_keyaccountourepresentanteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, representanteId);

            //Data 
           
                query.Criteria.AddCondition("createdon", ConditionOperator.GreaterEqual, dtInicio);
                query.Criteria.AddCondition("createdon", ConditionOperator.LessThan, dtFim.AddDays(1));                
          
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.Denuncia.Status.Ativo);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
