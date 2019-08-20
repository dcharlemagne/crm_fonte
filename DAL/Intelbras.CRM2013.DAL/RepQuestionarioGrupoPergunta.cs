using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepQuestionarioGrupoPergunta<T> : CrmServiceRepository<T>, IQuestionarioGrupoPergunta<T>
    {
        public List<T> ListarGrupoById(string questionarioId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_questionario_grupo_perguntaid", ConditionOperator.Equal, questionarioId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
