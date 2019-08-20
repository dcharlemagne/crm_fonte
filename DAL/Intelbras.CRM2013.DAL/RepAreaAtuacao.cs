using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Intelbras.CRM2013.DAL
{
    public class RepAreaAtuacao<T> : CrmServiceRepository<T>, IAreaAtuacao<T>
    {
        public T ObterPor(Guid areaAtuacaoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_area_atuacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, areaAtuacaoId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public T ObterPorCodigo(int codigoAreaAtuacao)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoAreaAtuacao);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPorContato(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.AddLink("itbc_contact_itbc_area_atuacao", "itbc_area_atuacaoid", "itbc_area_atuacaoid").LinkCriteria.AddCondition("contactid", ConditionOperator.Equal, contatoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}