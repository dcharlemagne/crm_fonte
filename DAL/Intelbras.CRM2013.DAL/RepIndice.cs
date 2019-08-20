using System;
using System.Collections.Generic;
using System.Linq;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepIndice<T> : CrmServiceRepository<T>, IIndice<T>
    {
        public List<T> ListarPor(Guid itbc_indiceid, Guid? itbc_tabeladefinanciamento)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
                Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_indiceid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_indiceid);
                query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Validações
                if (itbc_tabeladefinanciamento.HasValue)
                {
                    Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tabeladefinanciamento", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_tabeladefinanciamento);
                    query.Criteria.Conditions.Add(cond2);
                }
            #endregion

            #region Ordenações
                Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
                query.Orders.Add(ord1);
            #endregion

            var lista = (List<T>)this.RetrieveMultiple(query).List;
            return lista;
        }

        public T ObterPor(String chaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, chaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public bool AlterarStatus(Guid itbc_indice, int stateCode, int statuscode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_indice", itbc_indice),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;

        }
    }
}
