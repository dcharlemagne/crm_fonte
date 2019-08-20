using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepListaPrecoPSD<T> : CrmServiceRepository<T>, IListaPrecoPSD<T>
    {
        public List<T> ListarPor(Guid? estadoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            if (estadoId.HasValue)
            {
                query.AddLink("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", "itbc_psdidid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());
            }

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid? estadoId,Guid? UnidadeNegocio)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, UnidadeNegocio.ToString());

            if (estadoId.HasValue)
            {
                query.AddLink("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", "itbc_psdidid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estadoId.ToString());
            }

            //Data Vigencia Lista
            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(Guid itbc_businessunit, DateTime? itbc_Data_Inicio, DateTime? itbc_Data_Fim)
        {
            throw new NotImplementedException();
        }

    public List<T> ListarPor(Guid itbc_businessunit,Guid? estado)
    {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, itbc_businessunit.ToString());

            if (estado.HasValue)
            {
                query.AddLink("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", "itbc_psdidid").LinkCriteria.AddCondition("itbc_estadoid", ConditionOperator.Equal, estado.ToString());
            }
            else
            {
                LinkEntity link = query.AddLink("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", "itbc_psdidid", JoinOperator.LeftOuter);
                link.EntityAlias = "est";

                query.Criteria.AddCondition("est", "itbc_estadoid", ConditionOperator.Null);
            }

            //Data Vigencia Lista
            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(List<T>);

            return (List<T>) colecao.List;
        }
        public T ObterPor(Guid itbc_psdldid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_psdidid", ConditionOperator.Equal, itbc_psdldid.ToString());

            #endregion

            #region Ordenações
            query.AddOrder("modifiedon", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(Guid unidadeNegocioId,List<Guid> lstEstados,Guid? listaGuid,DateTime dtInicio,DateTime dtFim)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, unidadeNegocioId.ToString());

            if (lstEstados != null && lstEstados.Count > 0)
            {
                query.AddLink("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", "itbc_psdidid");
                
                //Adicionamos o OR para pegar qualquer lista PSD que esteja em um determinado range de data e que seja de um estado que ja tenha sido adicionado
                FilterExpression filterExp = new FilterExpression(LogicalOperator.Or);
                foreach (var item in lstEstados)
                {
                    filterExp.AddCondition("itbc_itbc_psdid_itbc_estado","itbc_estadoid", ConditionOperator.Equal, item);
                }
                query.Criteria.AddFilter(filterExp);
            }
            else
            {
                query.AddLink("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", "itbc_psdidid", JoinOperator.LeftOuter);
                query.Criteria.AddCondition("itbc_itbc_psdid_itbc_estado", "itbc_psdidid", ConditionOperator.Null);
            }
            
            //Ignoramos o proprio registro da consulta
            if (listaGuid.HasValue)
            {
                query.Criteria.AddCondition("itbc_psdidid", ConditionOperator.NotEqual, listaGuid.Value.ToString());
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
            filtro.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, dtInicio);
            FilterExpression filtroFilho = new FilterExpression(LogicalOperator.And);
            filtroFilho.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, dtInicio);
            filtroFilho.AddCondition("itbc_data_fim", ConditionOperator.LessEqual, dtFim);
            filtro.AddFilter(filtroFilho);

            FilterExpression filtro2 = new FilterExpression(LogicalOperator.And);
            filtro2.AddCondition("itbc_data_inicio", ConditionOperator.GreaterEqual, dtInicio);
            filtro2.AddCondition("itbc_data_fim", ConditionOperator.LessEqual, dtFim);

            FilterExpression filtro3 = new FilterExpression(LogicalOperator.And);
            filtro3.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, dtInicio);
            filtro3.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, dtFim);

            FilterExpression filtro4 = new FilterExpression(LogicalOperator.And);
            FilterExpression filtroFilho2 = new FilterExpression(LogicalOperator.And);
            filtroFilho2.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, dtFim);
            filtroFilho2.AddCondition("itbc_data_inicio", ConditionOperator.GreaterEqual, dtInicio);
            filtro4.AddFilter(filtroFilho2);
            filtro4.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, dtFim);

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
