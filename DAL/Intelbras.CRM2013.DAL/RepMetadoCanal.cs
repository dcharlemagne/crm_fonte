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
    public class RepMetadoCanal<T> : CrmServiceRepository<T>, IMetadoCanal<T>
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

        public T ObterPor(Guid metaunidadetrimestreId, Guid canalId, int ano, int trimestre)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_metadotrimestreid", ConditionOperator.Equal, metaunidadetrimestreId);
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

        public T ObterPor(Guid unidadeNegocioId, Guid canalId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid UnidNeg, int trimestre, Guid canalId, int ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, UnidNeg);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_canalid", ConditionOperator.Equal, canalId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
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

        public T ObterPorCodigo(string chaveIntegracaoUnidadeNegocio, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre, string codigoEmitenteCanal, int ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, UnidNeg);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            query.AddLink("account", "itbc_canalid", "accountid");
            query.LinkEntities[0].LinkCriteria.AddCondition("accountnumber", ConditionOperator.Equal, codigoEmitenteCanal);

            query.AddLink("businessunit", "itbc_unidadedenegocioid", "businessunitid");
            query.LinkEntities[1].LinkCriteria.AddCondition("itbc_chave_integracao", ConditionOperator.Equal, chaveIntegracaoUnidadeNegocio);

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

        public List<T> ListarPor(Guid metadotrimestreid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, trimestre);
            query.Criteria.AddCondition("itbc_metadotrimestreid", ConditionOperator.Equal, metadotrimestreid);


            //query.AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
            //     .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> Listar(Guid metaunidadeId)
        {
            return this.Listar(metaunidadeId, null);
        }

        public List<T> Listar(Guid metaunidadeId, List<Guid> lstIdCanal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            if (lstIdCanal != null)
                query.Criteria.AddCondition(new ConditionExpression("itbc_canalid", ConditionOperator.NotIn, lstIdCanal));

            query.AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaunidadeId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<Domain.ViewModels.ModeloMetaDetalhadaClienteViewModel> ListarModeloMetaDetalhada(Guid unidadeNegocioId, int ano)
        {
            int page = 1;
            string cookie = string.Empty;
            bool continua = false;
            var lista = new List<Domain.ViewModels.ModeloMetaDetalhadaClienteViewModel>();

            do
            {
                string fetch = @"<fetch count='5000' distinct='true' no-lock='true' page='{2}' paging-cookie='{3}'>
                              <entity name='itbc_metadocanalporproduto' >
                                <attribute name='itbc_canalid' alias='canal' />
                                <attribute name='itbc_produtoid' alias='produto' />
                                <filter type='and' >
                                  <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  <condition attribute='itbc_unidade_negocio' operator='eq' value='{1}' />
                                </filter>
                                <link-entity name='account' from='accountid' to='itbc_canalid' alias='canal' >
                                  <attribute name='accountnumber' alias='canal_codigo' />
                                  <attribute name='itbc_participa_do_programa' alias='canal_status' />
                                  <attribute name='itbc_classificacaoid' alias='canal_classificacao' />
                                </link-entity>
                                <link-entity name='product' from='productid' to='itbc_produtoid' alias='produto' >
                                  <attribute name='itbc_familiadeprodid' alias='produto_familia' />
                                  <attribute name='itbc_segmentoid' alias='produto_segmento' />
                                  <attribute name='itbc_subfamiliadeproduto' alias='produto_subfamilia' />
                                  <attribute name='statuscode' alias='produto_status' />
                                  <attribute name='productnumber' alias='produto_codigo' />
                                </link-entity>
                              </entity>
                            </fetch>";

                fetch = string.Format(fetch, ano, unidadeNegocioId, page, cookie);

                var retrieveMultiple = new RetrieveMultipleRequest()
                {
                    Query = new FetchExpression(fetch)
                };

                EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;
                cookie = System.Security.SecurityElement.Escape(collection.PagingCookie);
                continua = collection.MoreRecords;
                page++;

                foreach (var item in collection.Entities)
                {
                    var produto = ((EntityReference)((AliasedValue)item.Attributes["produto"]).Value);
                    var canal = ((EntityReference)((AliasedValue)item.Attributes["canal"]).Value);
                    var familia = ((EntityReference)((AliasedValue)item.Attributes["produto_familia"]).Value);
                    var segmento = ((EntityReference)((AliasedValue)item.Attributes["produto_segmento"]).Value);
                    var subfamilia = ((EntityReference)((AliasedValue)item.Attributes["produto_subfamilia"]).Value);
                    var classificacaoCanal = ((EntityReference)((AliasedValue)item.Attributes["canal_classificacao"]).Value);


                    var meta = new Domain.ViewModels.ModeloMetaDetalhadaClienteViewModel()
                    {
                        Canal = new SDKore.DomainModel.Lookup(canal.Id, canal.Name, canal.LogicalName),
                        Produto = new SDKore.DomainModel.Lookup(produto.Id, produto.Name, produto.LogicalName),
                        Familia = new SDKore.DomainModel.Lookup(familia.Id, familia.Name, familia.LogicalName),
                        Segmento = new SDKore.DomainModel.Lookup(segmento.Id, segmento.Name, segmento.LogicalName),
                        SubFamilia = new SDKore.DomainModel.Lookup(subfamilia.Id, subfamilia.Name, subfamilia.LogicalName),
                        ClassificacaoCanal = new SDKore.DomainModel.Lookup(classificacaoCanal.Id, classificacaoCanal.Name, classificacaoCanal.LogicalName),
                        CodigoProduto = ((AliasedValue)item.Attributes["produto_codigo"]).Value.ToString(),
                        StatusProduto = (Domain.Enum.Produto.StatusCode)((OptionSetValue)((AliasedValue)item.Attributes["produto_status"]).Value).Value,
                        StatusCanal = (Domain.Enum.Conta.ParticipaDoPrograma)((OptionSetValue)((AliasedValue)item.Attributes["canal_status"]).Value).Value
                    };

                    if (item.Contains("canal_codigo"))
                    {
                        meta.CodigoCanal = ((AliasedValue)item.Attributes["canal_codigo"]).Value.ToString();
                    }

                    lista.Add(meta);
                }
            } while (continua);

            return lista;
        }

        public List<Domain.ViewModels.ModeloMetaResumidaClienteViewModel> ListarModeloMetaResumida(Guid unidadeNegocioId, int ano)
        {
            int page = 1;
            string cookie = string.Empty;
            bool continua = false;
            var lista = new List<Domain.ViewModels.ModeloMetaResumidaClienteViewModel>();

            do
            {
                string fetch = @"<fetch count='5000' distinct='true' no-lock='true' page='{2}' paging-cookie='{3}' >
                                  <entity name='itbc_metadocanal' >
                                    <attribute name='itbc_canalid' alias='canal' />
                                    <filter type='and' >
                                      <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                      <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                    </filter>
                                    <link-entity name='account' from='accountid' to='itbc_canalid' >
                                      <attribute name='accountnumber' alias='canal_codigo' />
                                      <attribute name='itbc_participa_do_programa' alias='canal_status' />
                                      <attribute name='itbc_cpfoucnpj' alias='canal_cnpj' />
                                      <attribute name='itbc_classificacaoid' alias='canal_classificacao' />
                                    </link-entity>
                                  </entity>
                                </fetch>";
              
                fetch = string.Format(fetch, ano, unidadeNegocioId, page, cookie);

                var retrieveMultiple = new RetrieveMultipleRequest()
                {
                    Query = new FetchExpression(fetch)
                };

                EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;
                cookie = System.Security.SecurityElement.Escape(collection.PagingCookie);
                continua = collection.MoreRecords;
                page++;

                foreach (var item in collection.Entities)
                {
                    var canal = ((EntityReference)((AliasedValue)item.Attributes["canal"]).Value);
                    var classificacaoCanal = ((EntityReference)((AliasedValue)item.Attributes["canal_classificacao"]).Value);

                    var meta = new Domain.ViewModels.ModeloMetaResumidaClienteViewModel()
                    {
                        Canal = new SDKore.DomainModel.Lookup(canal.Id, canal.Name, canal.LogicalName),
                        CnpjCanal = ((AliasedValue)item.Attributes["canal_cnpj"]).Value.ToString(),
                        ClassificacaoCanal = new SDKore.DomainModel.Lookup(classificacaoCanal.Id, classificacaoCanal.Name, classificacaoCanal.LogicalName),
                        StatusCanal = (Domain.Enum.Conta.ParticipaDoPrograma)((OptionSetValue)((AliasedValue)item.Attributes["canal_status"]).Value).Value
                    };

                    if (item.Contains("canal_codigo"))
                    {
                        meta.CodigoCanal = ((AliasedValue)item.Attributes["canal_codigo"]).Value.ToString();
                    }

                    lista.Add(meta);
                }
            } while (continua);

            return lista;
        }

        public List<MetadoCanal> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var lista = new List<MetadoCanal>();

            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_metatrimestrecanal' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <link-entity name='itbc_metadocanal' from='itbc_metadocanalid' to='itbc_metadocanalid' >
                                  <attribute name='itbc_metadocanalid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            
            fetch = string.Format(fetch, ano, unidadeNegocioId);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var meta = new MetadoCanal(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(meta);
            }

            return lista;
        }

        /********************      ACESSO SQL      ********************/
        public DataTable ListarMetaCanalDW(int ano, int trimestre, List<MetadaUnidade> lstMetasUnidade)
        {
            StringBuilder strSql = new StringBuilder();
            string commandIn = string.Empty;

            strSql.Append("Select cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre ");
            strSql.Append(",sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("From viewFaturamentoCRM ");
            strSql.AppendFormat("Where CD_Ano = {0} ", ano);
            strSql.AppendFormat("And cd_trimestre = {0} ", trimestre);

            if (lstMetasUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstMetasUnidade)
                    commandIn += string.Concat("'", item.UnidadedeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("Group By  cd_ano,cd_unidade_negocio,CD_Emitente,cd_trimestre ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
    }
}