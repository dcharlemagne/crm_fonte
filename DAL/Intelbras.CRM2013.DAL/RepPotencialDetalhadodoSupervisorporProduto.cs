using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Intelbras.CRM2013.DAL
{

    public class RepPotencialDetalhadodoSupervisorporProduto<T> : CrmServiceRepository<T>, IPotencialDetalhadodoSupervisorporProduto<T>
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

        public List<T> ListarDetalheProdutosPorMeta(Guid metaId, Guid supervisorId, Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            #region Condições
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_supervisor", ConditionOperator.Equal, supervisorId);

            query.AddLink("itbc_potencialdosupervisorporproduto", "itbc_potencialdetalhadodosupervisorid", "itbc_potencialdosupervisorporprodutoid")
                .AddLink("itbc_potencial_supervisor_subfamilia", "itbc_potencialsupervisorporprodutoid", "itbc_potencial_supervisor_subfamiliaid")
                .AddLink("itbc_potencialdosupervisorporfamilia", "itbc_potencialsupervisorsubfamiliaid", "itbc_potencialdosupervisorporfamiliaid")
                .AddLink("itbc_potencial_supervisorporsegmento", "itbc_potencialsupervisorporfamiliaid", "itbc_potencial_supervisorporsegmentoid")
                .AddLink("itbc_potencialdosupervisor", "itbc_potencialdosupervisorid", "itbc_potencialdosupervisorid")
                .AddLink("itbc_metaportrimestre", "itbc_potencialportrimestredaunidadeid", "itbc_metaportrimestreid")
                .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> Listar(Guid supervisorId, Guid produtoId, Guid trimestreId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_supervisor", ConditionOperator.Equal, supervisorId);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.AddLink("itbc_potencialdosupervisorporproduto", "itbc_potencialdetalhadodosupervisorid", "itbc_potencialdosupervisorporprodutoid")
                 .AddLink("itbc_potencial_supervisor_subfamilia", "itbc_potencialsupervisorporprodutoid", "itbc_potencial_supervisor_subfamiliaid")
                 .AddLink("itbc_potencialdosupervisorporfamilia", "itbc_potencialsupervisorsubfamiliaid", "itbc_potencialdosupervisorporfamiliaid")
                 .AddLink("itbc_potencial_supervisorporsegmento", "itbc_potencialsupervisorporfamiliaid", "itbc_potencial_supervisorporsegmentoid")
                 .AddLink("itbc_potencialdosupervisor", "itbc_potencialdosupervisorid", "itbc_potencialdosupervisorid")
                 .LinkCriteria.AddCondition("itbc_potencialportrimestredaunidadeid", ConditionOperator.Equal, trimestreId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes? mes = null)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            query.AddLink("itbc_potencialdosupervisorporproduto", "itbc_potencialdetalhadodosupervisorid", "itbc_potencialdosupervisorporprodutoid")
                .LinkCriteria.AddCondition("itbc_unidade_negociosid", ConditionOperator.Equal, unidadeNegocioId);

            if (mes.HasValue)
            {
                query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, (int)mes.Value);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<PotencialdoSupervisorporProdutoDetalhado> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes mes)
        {
            var lista = new List<PotencialdoSupervisorporProdutoDetalhado>();

            string fetch = @"<fetch aggregate='true' no-lock='true' returntotalrecordcount='true' >
                              <entity name='itbc_metadokaporsegmento' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_supervisorid' alias='supervisor' groupby='true' />
                                <filter type='and' >
                                  <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                </filter>
                                <link-entity name='itbc_metadokaporfamilia' from='itbc_metadokaporsegmentoid' to='itbc_metadokaporsegmentoid' >
                                  <link-entity name='itbc_metadokaporsubfamilia' from='itbc_metadokaporfamiliaid' to='itbc_metadokaporfamiliaid' >
                                    <link-entity name='itbc_metadokaporproduto' from='itbc_metadokaporsubfamiliaid' to='itbc_metadokaporsubfamiliaid' >
                                      <attribute name='itbc_produtoid' alias='produto' groupby='true' />
                                      <link-entity name='itbc_metadetalhadadokaporproduto' from='itbc_metadokaporprodutoid' to='itbc_metadokaporprodutoid' >
                                        <attribute name='itbc_qtderealizada' alias='quantidade_realizada' aggregate='sum' />
                                        <attribute name='itbc_qtdeplanejada' alias='quantidade_planejada' aggregate='sum' />
                                        <filter type='and' >
                                          <condition attribute='itbc_mes' operator='eq' value='{2}' />
                                        </filter>
                                      </link-entity>
                                    </link-entity>
                                  </link-entity>
                                </link-entity>
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
                var supervisor = ((EntityReference)((AliasedValue)item.Attributes["supervisor"]).Value);

                var potencial = new PotencialdoSupervisorporProdutoDetalhado(OrganizationName, IsOffline, Provider)
                {
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value,
                    QtdePlanejada = (decimal)((AliasedValue)item.Attributes["quantidade_planejada"]).Value,
                    QtdeRealizada = (decimal)((AliasedValue)item.Attributes["quantidade_realizada"]).Value,
                    Supervisor = new SDKore.DomainModel.Lookup(supervisor.Id, supervisor.Name, supervisor.LogicalName),
                    Mes = (int)mes,
                    Ano = ano,
                    Produto = new SDKore.DomainModel.Lookup(produto.Id, produto.Name, produto.LogicalName)
                };

                lista.Add(potencial);
            }

            return lista;
        }

        public T Obter(int ano, int trimestre, int mes, Guid produtoId, Guid supervisorId, Guid potencialsupervisorprodutoId)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_mes", ConditionOperator.Equal, mes);
            query.Criteria.AddCondition("itbc_produtoid", ConditionOperator.Equal, produtoId);
            query.Criteria.AddCondition("itbc_supervisor", ConditionOperator.Equal, supervisorId);
            query.Criteria.AddCondition("itbc_potencialdetalhadodosupervisorid", ConditionOperator.Equal, potencialsupervisorprodutoId);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaTrimestreDW(int ano, int trimestre, List<MetadaUnidade> lstMetas)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia,CD_Item,cd_mes ");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And cd_trimestre = {0} ", trimestre);

            if (lstMetas != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstMetas)
                    commandIn += string.Concat("'", item.UnidadedeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By cd_ano,cd_unidade_negocio,CD_representante,cd_trimestre,cd_segmento,cd_familia,cd_subfamilia,CD_Item,cd_mes ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}

