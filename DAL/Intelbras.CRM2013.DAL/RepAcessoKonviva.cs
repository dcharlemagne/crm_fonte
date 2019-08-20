using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepAcessoKonviva<T> : CrmServiceRepository<T>, IAcessoKonviva<T>
    {
        public T ObterPorContato(Guid contatoId, Domain.Enum.StateCode status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)status));
            #endregion

            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_contatoid", ConditionOperator.Equal, contatoId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_contatoid", ConditionOperator.Equal, contatoId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(object[] dicionarioValores)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.ColumnSet = new ColumnSet(true);

            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));
            #endregion

            query.Criteria.Conditions.Add(new ConditionExpression("itbc_deparadeunidadedokonvivaid", ConditionOperator.In, dicionarioValores));

            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ListarPorUnidade(Guid[] dicionarioValores)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.ColumnSet = new ColumnSet(true);

            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));
            #endregion

            query.Criteria.Conditions.Add(new ConditionExpression("itbc_unidadekonvivaid", ConditionOperator.In, dicionarioValores));

            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarPorCriterioDeParaContas(DeParaDeUnidadeDoKonviva objDePara, Guid idUnidadePadrao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.ColumnSet = new ColumnSet(true);

            query.Distinct = true;
            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));
            #endregion

            query.Criteria.Conditions.Add(new ConditionExpression("itbc_unidadekonvivaid", ConditionOperator.Equal, idUnidadePadrao));

            LinkEntity link = query.AddLink("contact", "itbc_contatoid", "contactid", JoinOperator.Inner);
            link.EntityAlias = "ctt";

            LinkEntity link2 = link.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Inner);
            link2.EntityAlias = "act";

            if (objDePara.Categoria != null)
            {
                LinkEntity link3 = link2.AddLink("itbc_categoriasdocanal", "accountid", "itbc_canalid", JoinOperator.Inner);
                link3.EntityAlias = "cat";
                query.Criteria.AddCondition("cat", "itbc_categoria", ConditionOperator.Equal, objDePara.Categoria.Id);
            }

            query.Criteria.AddCondition("act", "itbc_classificacaoid", ConditionOperator.Equal, objDePara.Classificacao.Id);
            query.Criteria.AddCondition("act", "itbc_subclassificacaoid", ConditionOperator.Equal, objDePara.SubClassificacao.Id);
            query.Criteria.AddCondition("ctt", "statecode", ConditionOperator.Equal, (int)Domain.Enum.CategoriaCanal.StateCode.Ativado);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCriterioDeParaContatos(DeParaDeUnidadeDoKonviva objDePara, Guid idUnidadePadrao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.ColumnSet = new ColumnSet(true);
            query.Distinct = true;

            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));
            #endregion

            query.Criteria.Conditions.Add(new ConditionExpression("itbc_unidadekonvivaid", ConditionOperator.Equal, idUnidadePadrao));

            LinkEntity link = query.AddLink("contact", "itbc_contatoid", "contactid", JoinOperator.Inner);
            link.EntityAlias = "ctt";

            query.Criteria.AddCondition("ctt", "parentcustomerid", ConditionOperator.Null);
            query.Criteria.AddCondition("ctt", "itbc_papelnocanal", ConditionOperator.Equal, objDePara.PapelNoCanalIntelbras);
            query.Criteria.AddCondition("ctt", "customertypecode", ConditionOperator.Equal, objDePara.TipoDeRelacao);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid acessoKonvivaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));
            #endregion

            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_acessoaokonvivaid", ConditionOperator.Equal, acessoKonvivaId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            OrderExpression ord1 = new OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorNome(String nome)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", ConditionOperator.Equal, nome);
            query.Criteria.Conditions.Add(cond1);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.AcessoKonviva.StateCode.Ativo);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid acessoKonvivaId, int status)
        {
            int stateCode;
            if (status == (int)Domain.Enum.AcessoKonviva.StateCode.Ativo)
            {
                //Ativar
                stateCode = (int)Domain.Enum.AcessoKonviva.StateCode.Ativo;
                status = (int)Domain.Enum.AcessoKonviva.StatusCode.Ativo;
            }
            else
            {
                //Inativar
                stateCode = (int)Domain.Enum.AcessoKonviva.StateCode.Inativo;
                status = (int)Domain.Enum.AcessoKonviva.StatusCode.Inativo;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_acessoaokonviva", acessoKonvivaId),
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
