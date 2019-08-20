using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepPais<T> : CrmServiceRepository<T>, IPais<T>
    {
        public List<T> ListarPor(String ChaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao_pais", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ChaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_paisid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_paisid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_paisid);
            query.Criteria.Conditions.Add(cond1);           
            #endregion


            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public Boolean AlterarStatus(Guid itbc_paisid, int status)
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
                EntityMoniker = new EntityReference("itbc_pais", itbc_paisid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public T PesquisarPaisPor(string nome)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_name", ConditionOperator.Equal, nome));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T PesquisarPaisPor(Guid ufId)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("itbc_estado", "itbc_paisid", "itbc_pais");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_estadoid", ConditionOperator.Equal, ufId));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

    }
}
