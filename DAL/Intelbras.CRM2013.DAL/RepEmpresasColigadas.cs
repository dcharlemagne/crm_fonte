using System;
using System.Collections.Generic;
using System.Linq;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Helper;

namespace Intelbras.CRM2013.DAL
{
    public class RepEmpresasColigadas<T> : CrmServiceRepository<T>, IEmpresasColigadas<T>
    {
        public List<T> ListarPor(Guid itbc_empresas_coligadasid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_empresas_coligadasid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_empresas_coligadasid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(String nome)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, nome);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_empresas_coligadasid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_empresas_coligadasid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_empresas_coligadasid);
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

        public T ObterPor(String cnpj)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cnpj));
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cnpj.InputMask()));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public bool AlterarStatus(Guid itbc_empresas_coligadasid, int stateCode,int statuscode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_empresas_coligadas", itbc_empresas_coligadasid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statuscode)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;

        }

        public bool AlterarProprietario(Guid proprietario, string TipoProprietario,Guid EmpresasColigadas)
        {
            Microsoft.Crm.Sdk.Messages.AssignRequest assignRequest = new Microsoft.Crm.Sdk.Messages.AssignRequest()
            {
                Assignee = new Microsoft.Xrm.Sdk.EntityReference
                {
                    LogicalName = TipoProprietario,
                    Id = proprietario
                },

                Target = new Microsoft.Xrm.Sdk.EntityReference("itbc_empresas_coligadas", EmpresasColigadas)
            };


            if (this.Execute(assignRequest).Results.Count > 0)
                return true;
            else
                return false;
        }
    }
}