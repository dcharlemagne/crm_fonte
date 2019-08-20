using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm.Util;

namespace Intelbras.CRM2013.DAL
{
    public class RepEstado<T> : CrmServiceRepository<T>, IEstado<T>
    {
        public List<T> ListarPor(Guid itbc_estadoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_estadoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_estadoid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(String ChaveIntegracao)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_chave_integracao", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ChaveIntegracao);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(ListaPrecoPSDPPPSCF listaPrecoPSDPPPSCF)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            #endregion

            #region Ordenações

            #endregion

            #region Links de Relacionamento
            query.LinkEntities.Add(new LinkEntity()
            {
                LinkFromEntityName = Utility.GetEntityName<T>(),
                LinkFromAttributeName = "itbc_estadoid",
                LinkToEntityName = "itbc_itbc_psdid_itbc_estado",
                LinkToAttributeName = "itbc_estadoid",
                LinkCriteria = new FilterExpression
                {
                    FilterOperator = LogicalOperator.And,
                    Conditions = 
                    {
                        new ConditionExpression("itbc_psdidid", ConditionOperator.In, listaPrecoPSDPPPSCF.ID.Value)
                    }
                }
            });
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorSigla(String siglaUF)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_siglauf", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, siglaUF);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Pais pais)
        {
            var query = GetQueryExpression<T>(true);
            if (pais.Id != Guid.Empty)
                query.Criteria.Conditions.Add(new ConditionExpression("itbc_pais", ConditionOperator.Equal, pais.Id));
            if (!string.IsNullOrEmpty(pais.Nome))
            {
                query.AddLink("itbc_pais", "itbc_pais", "itbc_paisid");
                query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_name", ConditionOperator.Equal, pais.Nome));
            }
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(String itbc_name)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_name);
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

        public T ObterPor(Guid itbc_estadoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_estadoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_estadoid);
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

        public Boolean AlterarStatus(Guid itbc_estadoid, int status)
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
                EntityMoniker = new EntityReference("itbc_estado", itbc_estadoid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public T PesquisarUfPor(string sigla, Pais pais)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_name", ConditionOperator.Equal, sigla));
            query.AddLink("itbc_pais", "itbc_pais", "itbc_paisid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_pais", ConditionOperator.Equal, pais.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarUf(Guid paisId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_paisid", ConditionOperator.Equal, paisId));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}