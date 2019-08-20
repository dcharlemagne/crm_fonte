using Intelbras.CRM2013.Domain.IRepository;
using SDKore.Crm;
using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepUnidadeNegocio<T> : CrmServiceRepository<T>, IUnidadeNegocio<T>
    {
        public List<T> ListarPor(Guid itbc_businessunitid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_businessunitid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorParticipaProgramaPci(bool participaProgramaPci, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if(columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            var value = participaProgramaPci ? "1" : "0";
            query.Criteria.AddCondition("itbc_participadopci", ConditionOperator.Equal, value);
            query.Criteria.AddCondition("name", ConditionOperator.Like, "%" + "ADMINISTRATIVO" + "%");

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodasParticipaProgramaPci(bool participaProgramaPci)
        {
            var query = GetQueryExpression<T>(true);

            var value = participaProgramaPci ? "1" : "0";
            query.Criteria.AddCondition("itbc_participadopci", ConditionOperator.Equal, value);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorConta(Guid itbc_accountid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_accountid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_businessunitid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_businessunitid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorChaveIntegracao(string itbc_chave_integracao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_chave_integracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ObterPorChaveIntegracao(string[] conjChavesIntegracao)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.Columns.Add("itbc_chave_integracao");

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, conjChavesIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public T ObterPor(int businessunitid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, businessunitid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(LinhaComercial linhaComercial)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_linha_unidade_negocio", "businessunitid", "new_unidade_negocioid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, linhaComercial.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarPor(String unidadeNegocioNome)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeNegocioNome);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições


            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarTodosChaveIntegracao()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições


            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarUnidadesDeNegocioB2BPor(Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("itbc_relacionamentodob2b", "businessunitid", "itbc_codigounidadecomercial");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_codigocliente", ConditionOperator.Equal, cliente.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }      
        public Guid ObterRelacionamentoUnidadeNegocioBenef(string chaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, chaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            LinkEntity link = query.AddLink("itbc_solicitacaoxunidades", "businessunitid", "itbc_unidadesid", JoinOperator.Inner);
            
            EntityCollection colecao = base.Provider.RetrieveMultiple(query);
            if (colecao.Entities.Count > 0)
                if (colecao.Entities[0].Attributes.Contains("itbc_unidadesid"))
                    return (new Guid(((AliasedValue)colecao.Entities[0].Attributes["itbc_unidadesid"]).Value.ToString()));
            return Guid.Empty;            
        }
    }
}