using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoPedido<T> : CrmServiceRepository<T>, IProdutoPedido<T>
    {

        public List<T> ListarPor(Guid salesorderdetailid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("salesorderdetailid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, salesorderdetailid);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorPedido(Guid salesorderid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("salesorderid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, salesorderid);
            query.Criteria.Conditions.Add(cond1);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid salesorderdetailid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("salesorderdetailid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, salesorderdetailid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorChaveIntegracao(String chaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, chaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        //public Boolean AlterarStatus(Guid id, int status)
        //{
        //    //int stateCode;
        //    //if (status == 0)
        //    //{
        //    //    //Ativar
        //    //    stateCode = 0;
        //    //    status = 1;
        //    //}
        //    //else
        //    //{
        //    //    //Inativar
        //    //    stateCode = 1;
        //    //    status = 2;
        //    //}

        //    //SetStateRequest request = new SetStateRequest
        //    //{
        //    //    EntityMoniker = new EntityReference("salesorderdetail", id),
        //    //    State = new OptionSetValue(stateCode),
        //    //    Status = new OptionSetValue(status)
        //    //};

        //    //SetStateResponse resp = (SetStateResponse)this.Execute(request);

        //    //if (resp != null)
        //    //    return true;

        //    return false;
        //}

    }
}
