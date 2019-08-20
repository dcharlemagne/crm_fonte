using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoPoliticaComercial<T> : CrmServiceRepository<T>, IProdutoPoliticaComercial<T>
    {

        public List<T> ListarPor(Guid politicaComercialId, Guid produtoId, Guid? produtoPoliticaComercialId,DateTime dtInicio,DateTime dtFim,int qntInicial,int qntFinal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            if (produtoPoliticaComercialId.HasValue)
                query.Criteria.AddCondition("itbc_produtopoliticacomercialid", ConditionOperator.NotEqual, produtoPoliticaComercialId.ToString());

            query.Criteria.AddCondition("itbc_politicacomercialid", ConditionOperator.Equal, politicaComercialId.ToString());
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId.ToString());
            query.AddLink("itbc_politicacomercial", "itbc_politicacomercialid", "itbc_politicacomercialid").LinkCriteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);

            //Data de vigencia (hoje)
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
            filtro.AddCondition("itbc_data_inicio_vigencia", ConditionOperator.LessEqual, dtInicio);
            FilterExpression filtroFilho = new FilterExpression(LogicalOperator.And);
            filtroFilho.AddCondition("itbc_data_fim_vigencia", ConditionOperator.GreaterEqual, dtInicio);
            filtroFilho.AddCondition("itbc_data_fim_vigencia", ConditionOperator.LessEqual, dtFim);
            filtro.AddFilter(filtroFilho);

            FilterExpression filtro2 = new FilterExpression(LogicalOperator.And);
            filtro2.AddCondition("itbc_data_inicio_vigencia", ConditionOperator.GreaterEqual, dtInicio);
            filtro2.AddCondition("itbc_data_fim_vigencia", ConditionOperator.LessEqual, dtFim);

            FilterExpression filtro3 = new FilterExpression(LogicalOperator.And);
            filtro3.AddCondition("itbc_data_inicio_vigencia", ConditionOperator.LessEqual, dtInicio);
            filtro3.AddCondition("itbc_data_fim_vigencia", ConditionOperator.GreaterEqual, dtFim);

            FilterExpression filtro4 = new FilterExpression(LogicalOperator.And);
            FilterExpression filtroFilho2 = new FilterExpression(LogicalOperator.And);
            filtroFilho2.AddCondition("itbc_data_inicio_vigencia", ConditionOperator.LessEqual, dtFim);
            filtroFilho2.AddCondition("itbc_data_inicio_vigencia", ConditionOperator.GreaterEqual, dtInicio);
            filtro4.AddFilter(filtroFilho2);
            filtro4.AddCondition("itbc_data_fim_vigencia", ConditionOperator.GreaterEqual, dtFim);

            filtroPai.AddFilter(filtro);
            filtroPai.AddFilter(filtro2);
            filtroPai.AddFilter(filtro3);
            filtroPai.AddFilter(filtro4);

            query.Criteria.AddFilter(filtroPai);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid politicaComercialId)
        {
            var query = GetQueryExpression<T>(true);

 
            query.Criteria.AddCondition("itbc_politicacomercialid", ConditionOperator.Equal, politicaComercialId.ToString());
        


            return (List<T>)this.RetrieveMultiple(query).List;
        }


        public T ObterPor(Guid politicaComercialId, Guid produtoId, int quantidade)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            //query.ColumnSet.AddColumns("itbc_produtopoliticacomercialid", "itbc_quantidade_inicial", "itbc_quantidade_final");

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            query.Criteria.AddCondition("itbc_data_inicio_vigencia", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim_vigencia", ConditionOperator.GreaterEqual,  DateTime.Today);

            query.Criteria.AddCondition("itbc_quantidade_inicial", ConditionOperator.LessEqual, quantidade);
            query.Criteria.AddCondition("itbc_quantidade_final", ConditionOperator.GreaterEqual, quantidade);

            query.Criteria.AddCondition("itbc_politicacomercialid", ConditionOperator.Equal, politicaComercialId.ToString());
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId.ToString());

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPorProduto(Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId.ToString());


            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid produtoPoliticaComercialId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_produtopoliticacomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoPoliticaComercialId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AlterarStatus(Guid produtoPoliticaComercialId, int status)
        {

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_produtopoliticacomercial", produtoPoliticaComercialId),
                State = new OptionSetValue(0),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }
    }
}
