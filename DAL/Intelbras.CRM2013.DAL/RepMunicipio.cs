using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;

namespace Intelbras.CRM2013.DAL
{
    public class RepMunicipio<T> : CrmServiceRepository<T>, IMunicipio<T>
    {
        public List<T> ListarPor(string ChaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chavedeintegracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ChaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid itbc_municipioid)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_municipiosid", ConditionOperator.Equal, itbc_municipioid));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Estado uf)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_estadoid", ConditionOperator.Equal, uf.Id));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_municipioid)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_municipiosid", ConditionOperator.Equal, itbc_municipioid));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Guid itbc_estadoid, string itbc_name)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_estadoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_estadoid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_name);
            query.Criteria.Conditions.Add(cond2);
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

        public T ObterPor(string uf, string itbc_name)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_name));
            query.AddLink("itbc_estado", "itbc_estadoid", "itbc_estadoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_siglauf", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, uf));
            query.Orders.Add(new OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(string chaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.AddCondition("itbc_chavedeintegracao", ConditionOperator.Equal, chaveIntegracao);
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        
        public Boolean AlterarStatus(Guid itbc_municipiosid, int status)
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
                EntityMoniker = new EntityReference("itbc_municipios", itbc_municipiosid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public Domain.ViewModels.IbgeViewModel ObterIbgeViewModelPor(int codigoIbge)
        {
            Domain.ViewModels.IbgeViewModel ibgeViewModel = null; 
            var query = new QueryExpression("itbc_municipios");
            query.ColumnSet.AllColumns = true;
            query.TopCount = 1;
            query.Criteria.AddCondition("itbc_codigo_ibge", ConditionOperator.Equal, codigoIbge);
            query.AddLink("itbc_estado", "itbc_estadoid", "itbc_estadoid");
            query.LinkEntities[0].EntityAlias = "estado";
            query.LinkEntities[0].Columns.AllColumns = true;
            query.LinkEntities[0].AddLink("itbc_pais", "itbc_pais", "itbc_paisid");
            query.LinkEntities[0].LinkEntities[0].EntityAlias = "pais";
            query.LinkEntities[0].LinkEntities[0].Columns.AllColumns = true;
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            if (result.Count > 0)
            {
                var estado = ((Microsoft.Xrm.Sdk.EntityReference)result[0]["itbc_estadoid"]);
                var pais = ((Microsoft.Xrm.Sdk.AliasedValue)result[0]["estado.itbc_pais"]).Value as Microsoft.Xrm.Sdk.EntityReference;
                ibgeViewModel = new Domain.ViewModels.IbgeViewModel()
                {
                    CodigoIbge = codigoIbge,
                    CidadeNome = result[0]["itbc_name"].ToString(),
                    EstadoNome = estado.Name,
                    PaisNome = pais.Name,
                    EstadoUF = ((AliasedValue)result[0]["estado.itbc_siglauf"]).Value.ToString(),
                    CidadeId = result[0].Id,
                    EstadoId = estado.Id,
                    PaisId = pais.Id
                };
            }

            return ibgeViewModel;
        }

        public T ObterPorCep(string cep)
        {
            var row = new Buscar_DadosCep_ttRetornoCEPRow[1];
            Domain.Servicos.HelperWS.IntelbrasService.Buscar_DadosCep(cep.Replace("-", ""), out row);

            if (row == null) return default(T);
            if (!row[0].ibge.HasValue) return default(T);
            if (row[0].ibge.Value <= 0) return default(T);

            return this.ObterPor(row[0].ibge.Value);
        }
        
        public T ObterPor(int codigoIBGE)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_ibge", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoIBGE);
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

        public List<T> ListarPor(List<Regional> listaRegionais)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.FilterOperator = LogicalOperator.Or;

            #region Condições
            foreach (Regional regional in listaRegionais)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_regionalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, regional.Id));
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarCidadesPor(Regional regional)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_regionalid", ConditionOperator.Equal, regional.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}
