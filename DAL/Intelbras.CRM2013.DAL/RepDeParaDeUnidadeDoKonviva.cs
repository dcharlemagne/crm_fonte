using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepDeParaDeUnidadeDoKonviva<T> : CrmServiceRepository<T>, IDeParaDeUnidadeDoKonviva<T>
    {
        public List<T> ListarPor(Dictionary<string, object> conjuntoCampos, DateTime? datainicial, bool somenteAtivos = true)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.ColumnSet = new ColumnSet(true);

            if (somenteAtivos)
                query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));

            if (datainicial.HasValue)
            {
                query.Criteria.AddCondition(new ConditionExpression("modifiedon", ConditionOperator.GreaterEqual, datainicial));
                query.Criteria.AddCondition(new ConditionExpression("modifiedon", ConditionOperator.LessEqual, DateTime.Now));
            }

            foreach (var item in conjuntoCampos)
            {
                query.Criteria.Conditions.Add(new ConditionExpression(item.Key, ConditionOperator.Equal, item.Value));
            }
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
    }
}
