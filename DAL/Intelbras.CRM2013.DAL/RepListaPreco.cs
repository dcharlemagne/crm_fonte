using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepListaPreco<T> : CrmServiceRepository<T>, IListaPreco<T>
    {
        public List<T> ListarPor(Guid pricelevelid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("pricelevelid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, pricelevelid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Domain.Enum.ListaPreco.Tipo tipo, Guid? estadoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 100001);
            
            query.Criteria.AddCondition("itbc_tipodalista", ConditionOperator.Equal, (int)tipo);

            if (estadoId.HasValue)
            {
                query.AddLink("itbc_pricelevel_itbc_estado", "pricelevelid", "pricelevelid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());
            }

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public T ObterPor(Domain.Enum.ListaPreco.Tipo tipo, Guid? estadoId, Guid? unidadeNegocio)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 100001);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocio.ToString());
            query.Criteria.AddCondition("itbc_tipodalista", ConditionOperator.Equal, (int)tipo);

            if (estadoId.HasValue)
            {
                query.AddLink("itbc_pricelevel_itbc_estado", "pricelevelid", "pricelevelid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());
            }

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(Domain.Enum.ListaPreco.Tipo tipo, Guid? estadoId, Guid? unidadeNegocio)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 100001);
            if (unidadeNegocio.HasValue)
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocio.ToString());
            query.Criteria.AddCondition("itbc_tipodalista", ConditionOperator.Equal, (int)tipo);

            //Data Vigencia Lista
            query.Criteria.AddCondition("begindate", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("enddate", ConditionOperator.GreaterEqual, DateTime.Today);
            if (estadoId.HasValue)
            {
                query.AddLink("itbc_pricelevel_itbc_estado", "pricelevelid", "pricelevelid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());
            }

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;

            //if (colecao.List.Count == 0)
            //    return default(T);

            //return colecao.List[0];
        }


        public T ObterPor(Guid pricelevelid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("pricelevelid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, pricelevelid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(String nomeListaPreco)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, nomeListaPreco);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid unidadeNegocioId, List<Guid> lstEstados, Guid? listaGuid, DateTime dtInicio, DateTime dtFim)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.ToString());

            if (lstEstados != null && lstEstados.Count > 0)
            {
                query.AddLink("itbc_pricelevel_itbc_estado", "pricelevelid", "pricelevelid");

                //Adicionamos o OR para pegar qualquer lista PSD que esteja em um determinado range de data e que seja de um estado que ja tenha sido adicionado
                FilterExpression filterExp = new FilterExpression(LogicalOperator.Or);
                foreach (var item in lstEstados)
                {
                    filterExp.AddCondition("itbc_pricelevel_itbc_estado", "itbc_estadoid", ConditionOperator.Equal, item);
                }
                query.Criteria.AddFilter(filterExp);
            }
            else
            {
                query.AddLink("itbc_pricelevel_itbc_estado", "pricelevelid", "pricelevelid", JoinOperator.LeftOuter);
                query.Criteria.AddCondition("itbc_pricelevel_itbc_estado", "pricelevelid", ConditionOperator.Null);
            }

            //Ignoramos o proprio registro da consulta
            if (listaGuid.HasValue)
            {
                query.Criteria.AddCondition("pricelevelid", ConditionOperator.NotEqual, listaGuid.Value.ToString());
            }

            //Consulta pra basicamente pegar todos os intervalos possiveis que já possuem uma data no intervalo
            //Basicamente A B é o intervalo que o usuario ta tentando cadastrar
            // 1 e 2 são as datas que devemos verificar com base no A B para ver se já existem
            //valor a esquerda do A quer dizer <= A a direita de B é >= B
            /*
             * 1     -    2
             *  1 -  2
             *    A 1-2 B
             *        1  -  2
             */
            FilterExpression filtroPai = new FilterExpression(LogicalOperator.Or);

            FilterExpression filtro = new FilterExpression(LogicalOperator.And);
            filtro.AddCondition("begindate", ConditionOperator.LessEqual, dtInicio);
            FilterExpression filtroFilho = new FilterExpression(LogicalOperator.And);
            filtroFilho.AddCondition("enddate", ConditionOperator.GreaterEqual, dtInicio);
            filtroFilho.AddCondition("enddate", ConditionOperator.LessEqual, dtFim);
            filtro.AddFilter(filtroFilho);

            FilterExpression filtro2 = new FilterExpression(LogicalOperator.And);
            filtro2.AddCondition("begindate", ConditionOperator.GreaterEqual, dtInicio);
            filtro2.AddCondition("enddate", ConditionOperator.LessEqual, dtFim);

            FilterExpression filtro3 = new FilterExpression(LogicalOperator.And);
            filtro3.AddCondition("begindate", ConditionOperator.LessEqual, dtInicio);
            filtro3.AddCondition("enddate", ConditionOperator.GreaterEqual, dtFim);

            FilterExpression filtro4 = new FilterExpression(LogicalOperator.And);
            FilterExpression filtroFilho2 = new FilterExpression(LogicalOperator.And);
            filtroFilho2.AddCondition("begindate", ConditionOperator.LessEqual, dtFim);
            filtroFilho2.AddCondition("begindate", ConditionOperator.GreaterEqual, dtInicio);
            filtro4.AddFilter(filtroFilho2);
            filtro4.AddCondition("enddate", ConditionOperator.GreaterEqual, dtFim);

            filtroPai.AddFilter(filtro);
            filtroPai.AddFilter(filtro2);
            filtroPai.AddFilter(filtro3);
            filtroPai.AddFilter(filtro4);

            query.Criteria.AddFilter(filtroPai);

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
