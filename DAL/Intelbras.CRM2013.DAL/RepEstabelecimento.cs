using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk.Query;
using Intelbras.CRM2013.Domain.IntelbrasService;

namespace Intelbras.CRM2013.DAL
{
    public class RepEstabelecimento<T> : CrmServiceRepository<T>, IEstabelecimento<T>
    {
        public List<T> ListarPor(Guid itbc_estabelecimentoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_estabelecimentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_estabelecimentoid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodos(params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }
            else
            {
                query.ColumnSet.AllColumns = true;
            }

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        

        public T ObterPor(Guid itbc_estabelecimentoid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_estabelecimentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_estabelecimentoid);
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

        public T ObterPor(int itbc_codigo_estabelecimento)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_estabelecimento", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_codigo_estabelecimento);
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

        public Boolean AlterarStatus(Guid itbc_estabelecimentoid, int status)
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
                EntityMoniker = new EntityReference("itbc_estabelecimento", itbc_estabelecimentoid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        //CRM4
        public List<T> ListarB2B()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_exibir_b2b", ConditionOperator.Equal, true));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(LinhaComercial linhaComercial)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_linha_unidade_negocio", "itbc_estabelecimentoid", "new_estabelecimentoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, linhaComercial.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Pedido pedido)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("salesorder", "itbc_estabelecimento", "itbc_estabelecimentoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("salesorderid", ConditionOperator.Equal, pedido.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarEstabelecimentosPor(TabelaDePreco tabelaDePreco)
        {
            List<T> estabelecimentos = new List<T>();
            BuscarEstabTabPreco_ttEstabelecRow[] row = null;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarEstabTabPreco(tabelaDePreco.CodigoDaTabelaDePreco, out row);
            if (row != null)
                foreach (var item in row)
                {
                    estabelecimentos.Add(ObterPor(Convert.ToInt32(item.codestabel)));
                }

            return estabelecimentos;
        }

    }
}