using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Microsoft.Xrm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepMetadaUnidadeporProdutoMes<T> : CrmServiceRepository<T>, IMetadaUnidadeporProdutoMes<T>
    {
        #region Objeto Q obtem a conexao com o SQL
        private DataBaseSqlServer _DataBaseSqlServer = null;
        private DataBaseSqlServer DataBaseSqlServer
        {
            get
            {
                if (_DataBaseSqlServer == null)
                    _DataBaseSqlServer = new DataBaseSqlServer();

                return _DataBaseSqlServer;
            }
        }
        #endregion

        public T Obter(Guid produtoId, Guid metaunidadesubfamiliaId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_productid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_subfamiliaid", ConditionOperator.Equal, metaunidadesubfamiliaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        
        public List<T> ListarPor(Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public List<T> ListarPorCanal(Guid canalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_metadocanalid", ConditionOperator.Equal, canalId);

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<MetaDetalhadadaUnidadeporProduto> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes)
        {
            var lista = new List<MetaDetalhadadaUnidadeporProduto>();

            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_potencialdetalhadosuper_produto' >
                                <attribute name='itbc_potencialplanejado' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_potencialrealizado' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_produtoid' alias='produto' groupby='true' />
                                <attribute name='itbc_qtdeplanejada' alias='quantidade_planejada' aggregate='sum' />
                                <attribute name='itbc_qtderealizada' alias='quantidade_realizada' aggregate='sum' />
                                <filter type='and' >
                                  <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  <condition attribute='itbc_mes' operator='eq' value='{2}' />
                                  <condition attribute='itbc_unidade_negociosid' operator='eq' value='{1}' />
                                </filter>
                              </entity>
                            </fetch>";
           
            fetch = string.Format(fetch, ano, unidadeNegocioId, (int)mes);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var produto = ((EntityReference)((AliasedValue)item.Attributes["produto"]).Value);

                var potencial = new MetaDetalhadadaUnidadeporProduto(OrganizationName, IsOffline, Provider)
                {
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value,
                    QtdePlanejada = (decimal)((AliasedValue)item.Attributes["quantidade_planejada"]).Value,
                    QtdeRealizada = (decimal)((AliasedValue)item.Attributes["quantidade_realizada"]).Value,
                    Mes = (int)mes, 
                    Ano = ano,
                    Produto = new SDKore.DomainModel.Lookup(produto.Id, produto.Name, produto.LogicalName)
                };

                lista.Add(potencial);
            }

            return lista;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes? mes = null)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_unidade_negociosid", ConditionOperator.Equal, unidadeNegocioId);
                      
            if (mes.HasValue)
            {
                query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, (int)mes.Value);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}