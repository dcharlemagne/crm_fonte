using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepBeneficiosCompromissos<T> : CrmServiceRepository<T>, IBeneficiosCompromissos<T>
    {
        public List<T> ListarPor(Guid PerfilId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_perfilid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, PerfilId);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid PerfilId,Guid? CompromissoId,Guid? BeneficioId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_perfilid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, PerfilId);
            query.Criteria.Conditions.Add(cond1);

            if (CompromissoId != null)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_compdoprogid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, CompromissoId);
                query.Criteria.Conditions.Add(cond2);
            }
            if (BeneficioId != null)
            {
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_benefdoprogid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, BeneficioId);
                query.Criteria.Conditions.Add(cond3);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_acessosextranetcontatosid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_acessosextranetcontatosid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_acessosextranetcontatosid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
