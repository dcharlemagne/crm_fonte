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
    public class RepPrioridadeLigacaoCallCenter<T> : CrmServiceRepository<T>, IPrioridadeLigacaoCallCenter<T>
    {
        public List<T> ListarPorCpfCnpjNomeFila(string cpfCnpj, string[] filas, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            query.Criteria.Conditions.Add(new ConditionExpression("new_cpfcnpj", ConditionOperator.Equal, cpfCnpj));
            query.Criteria.Conditions.Add(new ConditionExpression("new_nome_fila", ConditionOperator.In, filas));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
