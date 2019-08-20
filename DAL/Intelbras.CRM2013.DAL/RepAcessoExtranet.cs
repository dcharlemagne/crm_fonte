using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepAcessoExtranet<T> : CrmServiceRepository<T>, IAcessoExtranet<T>
    {
        public List<T> ListarPor(Guid itbc_tipodeacesso)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_tipodeacesso", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_tipodeacesso);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid acessosextranetid)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_acessoextranetid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, acessosextranetid);
            //Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_acessoextranetid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, acessosextranetid);
            //query.Criteria.Conditions.Add(cond1);

            //Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            //query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid classificacaoid, Guid categoriaid, string Cargo)
        {
            //- Nome = contenha a palavra "Gestor"
            //- Classificao = Classificação da Conta
            //- Categoria = Categoria da Conta
            //- Status = Ativo

            var query = GetQueryExpression<T>(true);          

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, classificacaoid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaid);
            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond3);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Like, "%"+ Cargo + "%");
            query.Criteria.Conditions.Add(cond4);
                        
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);
            
            return colecao.List[0];
        }

        public T ObterAdesao()
        {
            //- Nome = Conta Adesão

            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Like, "%Conta Adesão%");
            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
    }
}
