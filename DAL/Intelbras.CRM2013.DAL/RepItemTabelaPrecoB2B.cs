using System;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepItemTabelaPrecoB2B<T> : CrmServiceRepository<T>, IItemTabelaPrecoB2B<T>
    {
        public T ObterPor(Guid? tabela, string codigoItemPreco)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            #region Condições
            query.Criteria.AddCondition("itbc_tabelapreco", ConditionOperator.Equal, tabela);
            query.Criteria.AddCondition("itbc_codigoitempreco", ConditionOperator.Equal, codigoItemPreco);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid itbc_tabeladeprecoid, int status)
        {
            int stateCode;
            if (status == 0)
            {
                //Ativar
                stateCode = 0;
                status    = 1;
            }
            else
            {
                //Inativar
                stateCode = 1;
                status    = 2;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_itenstabelaprecob2b", itbc_tabeladeprecoid),
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
