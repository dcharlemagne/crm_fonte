using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;

namespace Intelbras.CRM2013.DAL
{
    public class RepFamiliaPoliticaComercial<T> : CrmServiceRepository<T>, IFamiliaPoliticaComercial<T>
    {
        public List<T> ListarTodas()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições         
 
            //Data Vigencia Lista
            query.Criteria.AddCondition("itbc_datainicial", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_datafinal", ConditionOperator.GreaterEqual, DateTime.Today);

            //Status Ativo
            query.Criteria.AddCondition("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid politicaComercialId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_politicacomercialid", ConditionOperator.Equal, politicaComercialId.ToString());

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
