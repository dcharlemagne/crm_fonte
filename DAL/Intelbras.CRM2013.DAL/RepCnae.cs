using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;

namespace Intelbras.CRM2013.DAL
{
    public class RepCnae<T> : CrmServiceRepository<T>, ICNAE<T>
    {
        public T ObterPor(string classe)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_classe", ConditionOperator.Equal, classe);


            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}