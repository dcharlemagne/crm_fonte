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
using SDKore.DomainModel;

namespace Intelbras.CRM2013.DAL
{
    public class RepPontuacao<T> : CrmServiceRepository<T>, IRepositoryBase
    {
        public List<T> ObterPor(Guid produtoId, DateTime inicio, DateTime? fim, Lookup pais, Lookup estado, Lookup distribuidor, Guid? pontuacaoId)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AddColumns("new_pontuacaoid", "new_data_inicio_vigencia", "new_data_final_vigencia");
            query.Criteria.FilterOperator = LogicalOperator.And;

            if (pontuacaoId != null)
            {
                query.Criteria.Conditions.Add(new ConditionExpression("new_pontuacaoid", ConditionOperator.NotEqual, pontuacaoId));
            }

            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produtoId));

            if (pais != null && pais.Id != Guid.Empty)
            {
                query.Criteria.Conditions.Add(new ConditionExpression("new_paisid", ConditionOperator.Equal, pais.Id));
            }
            else
            {
                query.Criteria.Conditions.Add(new ConditionExpression("new_paisid", ConditionOperator.Null));
            }

            if (estado != null && estado.Id != Guid.Empty)
            {
                query.Criteria.Conditions.Add(new ConditionExpression("new_ufid", ConditionOperator.Equal, estado.Id));
            }
            else
            {
                query.Criteria.Conditions.Add(new ConditionExpression("new_ufid", ConditionOperator.Null));
            }

            if (distribuidor != null && distribuidor.Id != Guid.Empty)
            {
                query.Criteria.Conditions.Add(new ConditionExpression("new_distribuidorid", ConditionOperator.Equal, distribuidor.Id));
            }
            else
            {
                query.Criteria.Conditions.Add(new ConditionExpression("new_distribuidorid", ConditionOperator.Null));
            }
            
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterListaCompletaVigenciaValida()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_final_vigencia", ConditionOperator.GreaterThan, DateTime.Today));
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_final_vigencia", ConditionOperator.Null));
            query.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }


        public T ObterPor(Guid produto)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produto));
            query.PageInfo = new PagingInfo { Count = 1, PageNumber = 1 };
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            
                return colecao.List[0];
        }
    }
}
