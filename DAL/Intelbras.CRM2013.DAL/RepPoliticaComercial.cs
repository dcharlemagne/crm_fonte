using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepPoliticaComercial<T> : CrmServiceRepository<T>, IPoliticaComercial<T>
    {

        public List<T> ListarPor(Guid estabelecimentoId, Guid? politicaComercialId, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId, int TipoPolitica, DateTime dtInicio, DateTime dtFim)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_estabelecimentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, estabelecimentoId);

            if (politicaComercialId.HasValue)
                query.Criteria.AddCondition("itbc_politicacomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual, politicaComercialId);

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeNegocioId);

            if (classificacaoId.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, classificacaoId);

            if (categoriaId.HasValue)
                query.Criteria.AddCondition("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaId);

            if (TipoPolitica != 0)
                query.Criteria.AddCondition("itbc_tipodepolitica", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TipoPolitica);

            //Isso é para pegar apenas registros sem estados relacionados(registros que valem por nacionalidade
            query.AddLink("itbc_itbc_politicacomercial_itbc_estado", "itbc_politicacomercialid", "itbc_politicacomercialid", JoinOperator.LeftOuter);
            query.Criteria.AddCondition("itbc_itbc_politicacomercial_itbc_estado", "itbc_politicacomercialid", ConditionOperator.Null);

            query.AddLink("itbc_itbc_politicacomercial_account", "itbc_politicacomercialid", "itbc_politicacomercialid", JoinOperator.LeftOuter);
            query.Criteria.AddCondition("itbc_itbc_politicacomercial_account", "itbc_politicacomercialid", ConditionOperator.Null);

            //Status Ativo
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);

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

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }


        public List<T> ListarPorTipoNull()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

        
           
                query.Criteria.AddCondition("itbc_tipodepolitica", Microsoft.Xrm.Sdk.Query.ConditionOperator.Null);

   
     
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid? PoliticaComercial,int TipoPolitica, int AplicarPoliticaPara, Guid estabelecimentoId, Guid unidadeNegocioId, List<Guid> Canais,DateTime dtInicio,DateTime dtFim)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_estabelecimentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, estabelecimentoId);

            query.Criteria.AddCondition("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeNegocioId);

            if (TipoPolitica != 0)
                query.Criteria.AddCondition("itbc_tipodepolitica", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, TipoPolitica);

            if (AplicarPoliticaPara != 0)
                query.Criteria.AddCondition("itbc_aplicarpoliticapara", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, AplicarPoliticaPara);

            if (PoliticaComercial.HasValue)
                query.Criteria.AddCondition("itbc_politicacomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual, PoliticaComercial);

            if (Canais.Count > 0)
            {
                query.AddLink("itbc_itbc_politicacomercial_account", "itbc_politicacomercialid", "itbc_politicacomercialid");
                FilterExpression filterExp = new FilterExpression(LogicalOperator.Or);
                foreach (var item in Canais)
                {
                    filterExp.AddCondition("itbc_itbc_politicacomercial_account", "accountid", ConditionOperator.Equal, item);
                }
                query.Criteria.AddFilter(filterExp);
            }
            //Status Ativo
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);

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

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCanal(Guid Canal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições


            query.AddLink("itbc_itbc_politicacomercial_account", "itbc_politicacomercialid", "itbc_politicacomercialid");
            FilterExpression filterExp = new FilterExpression(LogicalOperator.And);

            filterExp.AddCondition("itbc_itbc_politicacomercial_account", "accountid", ConditionOperator.Equal, Canal.ToString());
            query.Criteria.AddFilter(filterExp);

            //Status Ativo
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);
            //Razão do Status Ativo
            query.Criteria.AddCondition("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);


            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorEstado(Guid Estado)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições



            query.AddLink("itbc_itbc_politicacomercial_itbc_estado", "itbc_politicacomercialid", "itbc_politicacomercialid");
            FilterExpression filterExp = new FilterExpression(LogicalOperator.And);

            filterExp.AddCondition("itbc_itbc_politicacomercial_itbc_estado", "itbc_estadoid", ConditionOperator.Equal, Estado.ToString());

            query.Criteria.AddFilter(filterExp);

            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);
            //Razão do Status Ativo
            query.Criteria.AddCondition("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPorEstado(Guid? PoliticaComercial,int TipoPolitica, int AplicarPoliticaPara, Guid estabelecimentoId, Guid unidadeNegocioId, Guid Classificacao, Guid Categoria, List<Guid> Estados,DateTime dtInicio,DateTime dtFim)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_estabelecimentoid", ConditionOperator.Equal, estabelecimentoId);

            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId);

            query.Criteria.AddCondition("itbc_classificacaoid",ConditionOperator.Equal, Classificacao);

            query.Criteria.AddCondition("itbc_categoriaid", ConditionOperator.Equal, Categoria);

            if (TipoPolitica != 0)
                query.Criteria.AddCondition("itbc_tipodepolitica", ConditionOperator.Equal, TipoPolitica);

            if (AplicarPoliticaPara != 0)
                query.Criteria.AddCondition("itbc_aplicarpoliticapara", ConditionOperator.Equal, AplicarPoliticaPara);

            if (PoliticaComercial.HasValue)
                query.Criteria.AddCondition("itbc_politicacomercialid", ConditionOperator.NotEqual, PoliticaComercial);

            if (Estados.Count > 0)
            {
                query.AddLink("itbc_itbc_politicacomercial_itbc_estado", "itbc_politicacomercialid", "itbc_politicacomercialid");
                FilterExpression filterExp = new FilterExpression(LogicalOperator.Or);
                foreach (var item in Estados)
                {
                    filterExp.AddCondition("itbc_itbc_politicacomercial_itbc_estado", "itbc_estadoid", ConditionOperator.Equal, item);
                }
                query.Criteria.AddFilter(filterExp);
            }

            //Status Ativo
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);

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

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid? estabelecimentoId, Guid? politicaComercialId, Guid? canalId, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            if (estabelecimentoId.HasValue)
                query.Criteria.AddCondition("itbc_estabelecimentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, estabelecimentoId);

            if (politicaComercialId.HasValue)
                query.Criteria.AddCondition("itbc_politicacomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual, politicaComercialId);

            if (canalId.HasValue)
                query.Criteria.AddCondition("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canalId);

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeNegocioId);

            if (classificacaoId.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, classificacaoId);

            if (categoriaId.HasValue)
                query.Criteria.AddCondition("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaId);
            //Status Ativo
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);

            //Data de vigencia (hoje)
            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);

            //PoliticaCoemrcialEspecifica
            //query.Criteria.AddCondition("itbc_polticaespecificacanal", ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.PoliticaEspecifica.Nao);


            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPoliticasComClassificacao()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

           
                query.Criteria.AddCondition("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotNull);

            //Status Ativo
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);
            //Razão do Status Ativo
            query.Criteria.AddCondition("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);

            //Data de vigencia (hoje)
            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);

            //PoliticaCoemrcialEspecifica
            //query.Criteria.AddCondition("itbc_polticaespecificacanal", ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.PoliticaEspecifica.Nao);


            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPoliticaComercialEspecificaTodos()
        {
            var query = GetQueryExpression<T>(true);
            //Data de vigencia (hoje)
            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);
            //PoliticaCoemrcialEspecifica
            query.Criteria.AddCondition("itbc_polticaespecificacanal", ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.PoliticaEspecifica.Sim);


            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_fator", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;

        }

        public T ObterPorPoliticaEspecifica(Guid estabelecimentoId, Guid? politicaComercialId, Guid? canalId, Guid? unidadeNegocioId, Guid? classificacaoId, Guid? categoriaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_estabelecimentoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, estabelecimentoId);

            if (politicaComercialId.HasValue)
                query.Criteria.AddCondition("itbc_politicacomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual, politicaComercialId);

            if (canalId.HasValue)
                query.Criteria.AddCondition("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, canalId);

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeNegocioId);

            if (classificacaoId.HasValue)
                query.Criteria.AddCondition("itbc_classificacaoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, classificacaoId);

            if (categoriaId.HasValue)
                query.Criteria.AddCondition("itbc_categoriaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, categoriaId);

            //Data de vigencia (hoje)
            query.Criteria.AddCondition("itbc_data_inicio", ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_data_fim", ConditionOperator.GreaterEqual, DateTime.Today);
            //Status
            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.Status.Ativo);
            //PoliticaCoemrcialEspecifica
            query.Criteria.AddCondition("itbc_polticaespecificacanal", ConditionOperator.Equal, (int)Domain.Enum.PoliticaComercial.PoliticaEspecifica.Sim);


            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_fator", Microsoft.Xrm.Sdk.Query.OrderType.Descending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid politicaComercialId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_politicacomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, politicaComercialId);
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

        public void CriarAssociacaoCanal(Guid PoliticaID, Guid CanalID)
        {
            EntityReference Moniker1 = new EntityReference();
            Moniker1.Id = PoliticaID;
            Moniker1.Name = SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.PoliticaComercial>();
            
            EntityReference Moniker2 = new EntityReference();
            Moniker2.Id = CanalID;
            Moniker2.Name = SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.Conta>();

            this.AssociateManyToMany(Moniker1, Moniker2, SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.PoliticaComercialXConta>());
        }

        public void CriarAssociacaoEstado(Guid PoliticaID, Guid EstadoID)
        {
            EntityReference Moniker1 = new EntityReference();
            Moniker1.Id = PoliticaID;
            Moniker1.Name = SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.PoliticaComercial>();

            EntityReference Moniker2 = new EntityReference();
            Moniker2.Id = EstadoID;
            Moniker2.Name = SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.Estado>();

            this.AssociateManyToMany(Moniker1, Moniker2, SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.PoliticaComercialXEstado>());
        }
    }
}