using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Intelbras.CRM2013.Domain.ValueObjects;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using System.Xml.Linq;
using System.IO;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.DAL
{
    public class RepProduto<T> : CrmServiceRepository<T>, IProduto<T>
    {
        #region Objeto Q obtem a conexao com o SQL
        private DataBaseSqlServer _DataBaseSqlServer = null;
        private string emailAEnviarLog = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail");
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
        public List<T> ListarPor(Guid? unidadeNegocioId, Guid? familiaComercialId, Guid? familiaMaterialId, Guid? familiaProdutoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            if (unidadeNegocioId.HasValue)
                query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadeNegocioId.ToString());

            if (familiaComercialId.HasValue)
                query.Criteria.AddCondition("itbc_familiacomercial", ConditionOperator.Equal, familiaComercialId.ToString());

            if (familiaMaterialId.HasValue)
                query.Criteria.AddCondition("itbc_familia_material", ConditionOperator.Equal, familiaMaterialId.ToString());

            if (familiaProdutoId.HasValue)
                query.Criteria.AddCondition("itbc_familiadeprodid", ConditionOperator.Equal, familiaProdutoId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(List<string> codigosProduto, bool apenasAtivos = true, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            #region Condições
            if (apenasAtivos)
            {
                query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StatusCode.Ativo);
            }

            query.Criteria.AddCondition(new ConditionExpression("productnumber", ConditionOperator.In, codigosProduto));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(List<Guid> Familias, List<Guid> Produtos)
        {
            var query = GetQueryExpression<T>(true);

            #region Colunas de retorno

            #endregion

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition(new ConditionExpression("itbc_familiadeprodid", ConditionOperator.In, Familias));
            query.Criteria.AddCondition(new ConditionExpression("productid", ConditionOperator.In, Produtos));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(String codigoProduto)
        {
            var query = GetQueryExpression<T>(true);

            #region Colunas de retorno

            //query.ColumnSet.AddColumns("");

            #endregion

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);

            query.Criteria.AddCondition(new ConditionExpression("productnumber", ConditionOperator.Equal, codigoProduto));

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Colunas de retorno

            //query.ColumnSet.AddColumns("");

            #endregion

            #region Condições



            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 1);
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("productid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoId);
            query.Criteria.Conditions.Add(cond2);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid produtoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("productid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoId);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(int produtoNumber)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("productnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produtoNumber);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(string codigoProduto)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("productnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoProduto);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ObterPor(string[] codigoProduto)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("productnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, codigoProduto);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        /// <summary>
        /// Lista todos os produtos de uma determinada unidade de negocio
        /// </summary>
        /// <param name="unidadenegocioId"></param>
        /// <returns></returns>
        public List<T> ListarPorUnidadeNegocio(Guid unidadenegocioId)
        {
            return this.ListarParaMeta(unidadenegocioId, null);
        }

        public List<T> ListarProdutos(List<Guid> lstProductId)
        {
            var query = GetQueryExpression<T>(true);
            if (lstProductId.Count > 0)
            {
                query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StatusCode.Ativo);
                query.Criteria.AddCondition(new ConditionExpression("productid", ConditionOperator.In, lstProductId));
                return (List<T>)this.RetrieveMultiple(query).List;
            }
            else
                return null;

        }

        public List<T> ListarProdutosSegmento(List<Guid> lstSegmentoId)
        {
            var query = GetQueryExpression<T>(true);
            if (lstSegmentoId.Count > 0)
            {
                query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StatusCode.Ativo);
                query.Criteria.AddCondition(new ConditionExpression("itbc_segmentoid", ConditionOperator.In, lstSegmentoId));
                return (List<T>)this.RetrieveMultiple(query).List;
            }
            else
                return null;

        }
        

        public List<T> ListarParaMeta(Guid unidadenegocioId, Guid[] lstGrupoEstoque)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_considera_orc_metas", ConditionOperator.Equal, true);
            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StatusCode.Ativo);
            query.Criteria.AddCondition("itbc_businessunitid", ConditionOperator.Equal, unidadenegocioId);
            query.Criteria.AddCondition("itbc_acumulo_outroproduto", ConditionOperator.Equal, false);
            query.Criteria.AddCondition(new ConditionExpression("itbc_grupodeestoque", ConditionOperator.In, lstGrupoEstoque));

            query.AddLink("itbc_segmento", "itbc_segmentoid", "itbc_segmentoid")
                .LinkCriteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Segmento.StateCode.Ativo);

            query.AddLink("itbc_famildeprod", "itbc_familiadeprodid", "itbc_famildeprodid")
            .LinkCriteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.FamiliaProduto.StateCode.Ativo);

            query.AddLink("itbc_subfamiliadeproduto", "itbc_subfamiliadeproduto", "itbc_subfamiliadeprodutoid")
                .LinkCriteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.SubFamiliaProduto.StateCode.Ativo);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutosMetasCanal(Guid metaId)
        {
            return this.ListarProdutosMetasCanal(metaId, null, null);
        }

        public List<T> ListarProdutosMetasCanal(Guid metaId, int? pagina, int? contagem)
        {
            // mantem
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_metadocanalporproduto", "productid", "itbc_produtoid")
                 .AddLink("itbc_metadocanalporsubfamilia", "itbc_metadocanalporsubfamiliaid", "itbc_metadocanalporsubfamiliaid")
                 .AddLink("itbc_metadocanalporfamilia", "itbc_metadocanalporfamiliaid", "itbc_metadocanalporfamiliaid")
                 .AddLink("itbc_metadocanalporsegmento", "itbc_metadocanalporsegmentoid", "itbc_metadocanalporsegmentoid")
                 .AddLink("itbc_metadocanal", "itbc_metadocanalid", "itbc_metadocanalid")
                 .AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_subfamiliadeproduto", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutosMetasUnidade(Guid metaId)
        {
            return this.ListarProdutosMetasCanal(metaId, null, null);
        }

        public List<T> ListarProdutosMetasUnidade(Guid metaId, int? pagina, int? contagem)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_metaporproduto", "productid", "itbc_productid")
                 .AddLink("itbc_metasubfamiliasegmentotrimestre", "itbc_subfamiliaid", "itbc_metasubfamiliasegmentotrimestreid")
                 .AddLink("itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestre", "itbc_metafamiliasegmentotrimestreid")
                 .AddLink("itbc_metaporsegmento", "itbc_metadosegmentoid", "itbc_metaporsegmentoid")
                 .AddLink("itbc_metaportrimestre", "itbc_metaportrimestre", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_subfamiliadeproduto", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutosOrcamentoCanal(Guid orcamentoId)
        {
            return this.ListarProdutosOrcamentoCanal(orcamentoId, null, null);
        }

        public List<T> ListarProdutosOrcamentoCanal(Guid orcamentoId, int? pagina, int? contagem)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_orcdocanalporproduto", "productid", "itbc_produtoid")
                  .AddLink("itbc_orcdocanalporsubfamilia", "itbc_orcamentodocanalporsubfamilia", "itbc_orcdocanalporsubfamiliaid")
                  .AddLink("itbc_orcamentodocanalporfamilia", "itbc_orcamentodocanalporfamiliaid", "itbc_orcamentodocanalporfamiliaid")
                  .AddLink("itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmento", "itbc_orcamentodocanalporsegmentoid")
                  .AddLink("itbc_orcamentodocanal", "itbc_orcamentodocanalid", "itbc_orcamentodocanalid")
                  .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                  .LinkCriteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentoId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_subfamiliadeproduto", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutosOrcamentoUnidade(Guid orcamentoId)
        {
            return ListarProdutosOrcamentoUnidade(orcamentoId, null, null);
        }

        public List<T> ListarProdutosOrcamentoUnidade(Guid orcamentoId, int? pagina, int? contagem)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_orcamentoporproduto", "productid", "itbc_produtoid")
                 .AddLink("itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamilia", "itbc_orcamentoporsubfamiliaid")
                 .AddLink("itbc_orcamentoporfamilia", "itbc_orcamentoporfamiliaid", "itbc_orcamentoporfamiliaid")
                 .AddLink("itbc_orcamentoporsegmento", "itbc_orcamentoporsegmento", "itbc_orcamentoporsegmentoid")
                 .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                 .LinkCriteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentoId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_subfamiliadeproduto", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodosProdutos(ref int pagina, int contagem, out bool moreRecords)
        {
            var query = GetQueryExpression<T>(true);

            query.ColumnSet.AddColumns("statuscode", "statecode", "productid", "productnumber");

            #region Condições
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            #endregion


            #region Paginacao
            Microsoft.Xrm.Sdk.Query.PagingInfo paging = new PagingInfo();
            paging.Count = contagem;
            paging.PageNumber = pagina;

            query.PageInfo = paging;

            #endregion

            DomainCollection<T> retorno = this.RetrieveMultiple(query);
            moreRecords = retorno.MoreRecords;
            pagina++;
            return (List<T>)retorno.List;
        }

        public List<T> ListarTodosProdutos()
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet = new ColumnSet(true);

            DomainCollection<T> retorno = this.RetrieveMultiplePaged(query);

            return (List<T>)retorno.List;
        }

        public List<T> ListarProdutosKaRepresentante(Guid metaId, List<Guid> lstGrupoEstoque, int? pagina, int? contagem)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_grupodeestoque", ConditionOperator.In, lstGrupoEstoque[0].ToString());

            //if (lstGrupoEstoque != null && lstGrupoEstoque.Count > 0)
            //    query.Criteria.AddCondition(new ConditionExpression("itbc_grupodeestoque", ConditionOperator.In, lstGrupoEstoque));


            query.AddLink("itbc_metadokaporproduto", "productid", "itbc_produtoid")
                 .AddLink("itbc_metadokaporsubfamilia", "itbc_metadokaporsubfamiliaid", "itbc_metadokaporsubfamiliaid")
                 .AddLink("itbc_metadokaporfamilia", "itbc_metadokaporfamiliaid", "itbc_metadokaporfamiliaid")
                 .AddLink("itbc_metadokaporsegmento", "itbc_metadokaporsegmentoid", "itbc_metadokaporsegmentoid")
                 .AddLink("itbc_metakeyaccount", "itbc_metadokarepresentanteid", "itbc_metakeyaccountid")
                 .AddLink("itbc_metaportrimestre", "itbc_metaportrimestredaunidadeid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_subfamiliadeproduto", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPotencialKaRepresentante(Guid? unidadeNegocioId, int ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StateCode.ativo);
            query.AddLink("itbc_metadokaporproduto", "productid", "itbc_produtoid");
            if (unidadeNegocioId.HasValue)
            {
                query.LinkEntities[0].LinkCriteria.AddCondition("itbc_unidade_negociosid", ConditionOperator.Equal, unidadeNegocioId);
            }
            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarProdutosSupervisor(Guid metaId)
        {
            return this.ListarProdutosSupervisor(metaId, null, null);
        }

        public List<T> ListarProdutosSupervisor(Guid metaId, int? pagina, int? contagem)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.AddLink("itbc_potencialdosupervisorporproduto", "productid", "itbc_produtoid")
                 .AddLink("itbc_potencial_supervisor_subfamilia", "itbc_potencialsupervisorporprodutoid", "itbc_potencial_supervisor_subfamiliaid")
                 .AddLink("itbc_potencialdosupervisorporfamilia", "itbc_potencialsupervisorsubfamiliaid", "itbc_potencialdosupervisorporfamiliaid")
                 .AddLink("itbc_potencial_supervisorporsegmento", "itbc_potencialsupervisorporfamiliaid", "itbc_potencial_supervisorporsegmentoid")
                 .AddLink("itbc_potencialdosupervisor", "itbc_potencialdosupervisorid", "itbc_potencialdosupervisorid")
                 .AddLink("itbc_metaportrimestre", "itbc_potencialportrimestredaunidadeid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_subfamiliadeproduto", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ProdutoSubstituto(Guid produtoId)
        {
            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            strFetchXml.Append("<entity name='productsubstitute'>");
            strFetchXml.Append("<all-attributes />");
            strFetchXml.Append("<order attribute='productid' descending='false' />");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.AppendFormat("<condition attribute='substitutedproductid' operator='eq' value='{0}' />", produtoId);
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            // Recupera o Resultado do FetchXml
            var Registros = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;
            if (Registros.Entities.Count > 0)
            {
                var query = GetQueryExpression<T>(true);
                query.Criteria.AddCondition("productid", ConditionOperator.Equal, Registros.Entities[0]["productid"].ToString());
                var colecao = this.RetrieveMultiple(query);

                if (colecao.List.Count == 0)
                    return default(T);

                return colecao.List[0];
            }

            return default(T);
        }

        public Boolean AlterarStatus(Guid productId, int status)
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
                EntityMoniker = new EntityReference("product", productId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        //CRM4
        public SerieDoProduto PesquisarSerieDoProdutoPor(string numeroDeSerie)
        {
            SerieDoProduto serieDoProduto = null;

            bool pcorporativo;
            string pserie, pnrnotafis, pnomeemit, pitcodigo, pdescitem, pSigla, pDescSigla;
            DateTime pdtemisnota;
            int pnrordprod;
            BuscarItemCDB_ttcomponenteRow[] row = null;
            DateTime dataFabricacao;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarItemCDB(numeroDeSerie, out pcorporativo, out pserie, out pnrnotafis, out pdtemisnota, out pnomeemit, out pitcodigo, out pdescitem, out pnrordprod, out pSigla, out pDescSigla, out dataFabricacao, out row);

            if (pitcodigo != null)
            {
                serieDoProduto = new SerieDoProduto(this.OrganizationName, this.IsOffline);
                serieDoProduto.Produto = BuscarProdutoPor(pitcodigo, false, "", "", "", "", "", 0);
                serieDoProduto.NotaFiscal = new Domain.Model.Fatura(this.OrganizationName, this.IsOffline);
                serieDoProduto.NotaFiscal.IDFatura = pnrnotafis;
                serieDoProduto.NotaFiscal.DataEmissao = pdtemisnota;
                serieDoProduto.NotaFiscal.Cliente = new Domain.Model.Conta();
                serieDoProduto.NotaFiscal.Cliente.Nome = pnomeemit;
                serieDoProduto.Ordem = Convert.ToString(pnrordprod);
                serieDoProduto.Corporativo = pcorporativo;
                serieDoProduto.Celula = pSigla + " - " + pDescSigla;
                serieDoProduto.DataFabricacaoProduto = dataFabricacao;

                foreach (var item in row)
                    serieDoProduto.Descricao = serieDoProduto.Descricao + item.seq.ToString() + " - " + item.componente + "\n";
            }

            return serieDoProduto;
        }

        public Product ObterPorNumeroSerie(string numeroDeSerie)
        {
            var serie = this.PesquisarSerieDoProdutoPor(numeroDeSerie);
            if (serie != null) return serie.Produto;
            return null;
        }

        public Product NumeroDeSerieExisteNoEMS(string numeroDeSerie, string keyCode)
        {
            SerieDoProduto buscaProduto = PesquisarSerieDoProdutoPor(numeroDeSerie.Replace(".", "").Replace("-", "").Replace(" ", ""));
            if (buscaProduto != null)
            {
                return buscaProduto.Produto;
            }
            else
            {
                //Produto produto = new Produto() { Id = new Guid() };
                Product produto = null;
                return produto;
            }
        }
        
        public List<T> ListarProdutosPor(FamiliaComercial familiaComercial)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            try
            {
                if (familiaComercial != null)
                {
                    queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_familiacomercialid", ConditionOperator.Equal, familiaComercial.Id));
                }
            }
            catch { }
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<T> ListarProdutosPorCampos(string codigo, string familia, string produto, string tabelaDePreco)
        {
            int index = 0;
            var queryHelper = GetQueryExpression<T>(true);

            queryHelper.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            queryHelper.Distinct = true;

            if (!string.IsNullOrEmpty(codigo))
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("productnumber", ConditionOperator.Equal, codigo));
            else
            {
                if (!string.IsNullOrEmpty(produto)) queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, "%" + produto + "%"));
                if (!string.IsNullOrEmpty(familia))
                {
                    queryHelper.AddLink("itbc_familiacomercial", "itbc_familiacomercial", "itbc_familiacomercialid");
                    queryHelper.LinkEntities[index++].LinkCriteria.Conditions.Add(new ConditionExpression("new_name", ConditionOperator.Like, "%" + familia + "%"));
                }
            }

            if (!string.IsNullOrEmpty(tabelaDePreco))
            {
                queryHelper.AddLink("itbc_itenstabelaprecob2b", "productid", "itbc_codigoproduto");
                queryHelper.LinkEntities[index].AddLink("itbc_tabelaprecob2b", "itbc_tabelapreco", "itbc_tabelaprecob2bid");
                queryHelper.LinkEntities[index].LinkCriteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
                queryHelper.LinkEntities[index].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_tabelaprecoems", ConditionOperator.Equal, tabelaDePreco));
            }
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<T> ListarProdutosPor(Domain.Model. Fatura notaFiscal)
        {
            var query = GetQueryExpression<T>(true); ;
            query.AddLink("invoicedetail", "productid", "productid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("invoiceid", ConditionOperator.Equal, notaFiscal.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Product BuscarProdutoPor(string codigo, bool integradoEMS, string codigoCliente, string unidadeDeNegocio, string codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco, int tabelaEspecifica)
        {
            Product produto = null;
            var queryHelper = new QueryExpression("product");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("productnumber", ConditionOperator.Equal, codigo));
            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                produto = (new Domain.Servicos.RepositoryService()).Produto.Retrieve(bec.Entities[0].Id);

                if (integradoEMS)
                {
                    BuscarPreco_ttPrecoItemRow[] row = null;
                    BuscarPreco_ttErrosRow[] erros = null;
                    Domain.Servicos.HelperWS.IntelbrasService.BuscarPreco(codigoEstabelecimento, int.Parse(codigoCliente), unidadeDeNegocio, int.Parse(codigoCategoria), produto.Codigo, codigoTabelaDePreco, tabelaEspecifica, out row, out erros);

                    //if (erros.Length > 0)
                    //    foreach (var item in erros)
                    //        produto.DescricaoDaMensagemDeIntegracao = item.mensagem;
                    //else
                    if (row != null && row.Length > 0)
                        foreach (var item in row)
                        {
                            if (item.preco.HasValue) produto.Preco = item.preco.Value;
                            if (item.vlipi.HasValue) produto.AliquotaIPI = item.vlipi.Value;
                            if (item.quantmin.HasValue) produto.QuantidadeMinimaMultipla = Convert.ToInt32(item.quantmin.Value);
                            break;
                        }
                }
            }
            return produto;
        }

        public PrecoItem ObterPrecoItem(string codigoProduto, int codigoCliente, string unidadeDeNegocio, int codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco, int tabelaEspecifica)
        {
            BuscarPreco_ttPrecoItemRow[] row = null;
            BuscarPreco_ttErrosRow[] erros = null;

            Domain.Servicos.HelperWS.IntelbrasService.BuscarPreco(codigoEstabelecimento,
                                              codigoCliente,
                                              unidadeDeNegocio,
                                              codigoCategoria,
                                              codigoProduto,
                                              codigoTabelaDePreco,
                                              tabelaEspecifica,
                                              out row,
                                              out erros);

            if (erros != null && erros.Length > 0)
                throw new ArgumentException(erros[0].mensagem);

            if (row != null && row.Length > 0)
            {
                PrecoItem precoItem = new PrecoItem()
                {
                    DescricaoItem = row[0].descitem,
                    DescricaoFamiliaComercial = row[0].descfamilia,
                    CodigoItem = row[0].itcodigo,
                    CodigoFamiliaComercial = row[0].fmcodcom,
                    Preco = row[0].preco,
                    ValorIPI = row[0].vlipi
                };

                // QuantidadeMinima
                try { precoItem.QuantidadeMinima = Convert.ToInt16(row[0].quantmin.Value); }
                catch { precoItem.QuantidadeMinima = null; }

                return precoItem;
            }

            return null;
        }

        public decimal BuscarIndiceFinanceiroDoProdutoPor(int codigoCondicaoDePagamento)
        {
            decimal retorno = 0;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarIndiceFinanciamento(codigoCondicaoDePagamento, out retorno);
            return retorno;
        }

        public List<Product> BuscarEstruturaDoProdutoPor(string numeroDeSerie, string codigoDoProduto)
        {
            List<Product> produtos = new List<Product>();
            buscarEstruturaDoItemPor_ttEstruturaRow[] row = null;

            Domain.Servicos.HelperWS.IntelbrasService.buscarEstruturaDoItemPor(numeroDeSerie, codigoDoProduto, out row);


            if (row != null && row.Length > 0)
            {
                int maiorGarantia = 0;
                foreach (var item in row)
                {
                    if (maiorGarantia < item.garantia.Value) maiorGarantia = item.garantia.Value;
                    Product produto = new Product(this.OrganizationName, this.IsOffline)
                    {
                        Codigo = item.itcodigo,
                        GarantiaEmDias = item.garantia.Value,
                        LocalDeMontagem = item.localmontag,
                        NivelEstrutura = item.nivel.Value,
                        QuantidadeNaEstrutura = Convert.ToInt16(item.quantusada.Value),
                        SequenciaEstrutura = item.seq.Value,
                        PermitidoVenda = item.venda.Value,
                        Nome = item.descricao,
                        PermiteOS = item.permiteos.Value,
                        DataFabricacaoProduto = item.datafabric
                    };
                    produtos.Add(produto);
                }
                produtos[0].GarantiaEmDias = maiorGarantia;
            }

            return produtos;
        }

        public bool AcessoProdutoParaAssistenciaTecnica(Domain.Model.Conta assistenciaTecnica, Product produto)
        {
            QueryExpression queryHelper = new QueryExpression("new_linha_posto_servico");
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, assistenciaTecnica.Id));
            queryHelper.AddLink("new_linha_unidade_negocio", "new_linha_unidade_negocioid", "new_linha_unidade_negocioid");
            queryHelper.LinkEntities[0].AddLink("product", "new_linha_unidade_negocioid", "itbc_linha_unidade_negocioid");
            queryHelper.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("productnumber", ConditionOperator.Equal, produto.Codigo));
            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);
            if (colecao.Entities.Count > 0)
                return true;
            return false;
        }

        public bool ProdutoPossuiGarantiaEspecificaDentroDaVigenciaPor(string numeroDeSerie)
        {
            var queryHelper = new QueryExpression("new_produto_contrato");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_name", ConditionOperator.Equal, numeroDeSerie));
            var bec = base.Provider.RetrieveMultiple(queryHelper);
            if (bec.Entities.Count > 0)
            {
                var produtoContrato = bec.Entities[0];
                if (produtoContrato.Attributes.Contains("new_data_fim_vigencia"))
                {
                    if (Convert.ToDateTime(produtoContrato["new_data_fim_vigencia"]) > DateTime.Now)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<Product> ListarProdutosDeTabelaEspecificaPor(string codigoCliente, string unidadeDeNegocio, string codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco)
        {
            List<Product> produtos = new List<Product>();
            BuscarPreco_ttPrecoItemRow[] row = null;
            BuscarPreco_ttErrosRow[] erros = null;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarPreco(codigoEstabelecimento, int.Parse(codigoCliente), unidadeDeNegocio, int.Parse(codigoCategoria), " ", codigoTabelaDePreco, 1, out row, out erros);
            if (row != null)
            {
                if (row.Length > 0)
                {
                    foreach (var item in row)
                    {
                        Product produto = BuscarProdutoPor(item.itcodigo, false, "", "", "", "", "", 0);
                        if (produto == null) produto = new Product(this.OrganizationName, this.IsOffline);

                        produto.Codigo = item.itcodigo;
                        produto.Nome = item.descitem;
                        produto.DadosFamiliaComercial = new FamiliaComercial(this.OrganizationName, this.IsOffline);
                        produto.DadosFamiliaComercial.Codigo = item.fmcodcom;
                        produto.DadosFamiliaComercial.Nome = item.descfamilia;

                        if (item.preco.HasValue) produto.Preco = item.preco.Value;
                        if (item.vlipi.HasValue) produto.AliquotaIPI = item.vlipi.Value;
                        if (item.quantmin.HasValue) produto.QuantidadeMinimaMultipla = Convert.ToInt16(item.quantmin.Value);

                        //if (erros != null && erros.Count() > 0)
                        //    foreach (var erro in erros)
                        //        if (erro.mensagem.Contains(item.itcodigo))
                        //            produto.DescricaoDaMensagemDeIntegracao = erro.mensagem;


                        //EventLog.WriteEntry("CRM B2B Produtos Tab Esp", "Código Cliente: " + codigoCliente + "\n\nCódigo Produto: " + produto.Codigo + "\n\nCódigo Categoria: " + codigoCategoria + "\n\nUnidade Negócio: " + unidadeDeNegocio + "\n\nQtde Minima: " + produto.QuantidadeMinimaMultipla.ToString());
                        produtos.Add(produto);
                    }
                }
            }
            return produtos;
        }

        public T ObterPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("incident", "productid", "productid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ocorrencia.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Diagnostico diagnostico)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_diagnostico_ocorrencia", "productid", "new_produtoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.Equal, diagnostico.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPorNumero(string numero, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            if (columns != null && columns.Length > 0)
                query.ColumnSet.AddColumns(columns);
            else
                query.ColumnSet.AllColumns = true;
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("productnumber", ConditionOperator.Equal, numero));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<PontuacaoProduto> ListatPontuacaoPor(Guid produtoId, Guid distribuidorId, Guid ufId)
        {
            List<PontuacaoProduto> pontuacoes = new List<PontuacaoProduto>();

            QueryExpression queryHelper = new QueryExpression("new_promocao_pontuacao_fidelidade");
            queryHelper.ColumnSet.AllColumns = true;

            if (ufId != Guid.Empty || distribuidorId != Guid.Empty)
            {
                if (ufId != Guid.Empty) queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_ufid", ConditionOperator.Equal, ufId));
                if (distribuidorId != Guid.Empty) queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_distribuidorid", ConditionOperator.Equal, distribuidorId));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produtoId));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_data_terminio", ConditionOperator.GreaterEqual, DateTime.Now));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_data_inicio", ConditionOperator.LessEqual, DateTime.Now));

                EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

                foreach (Entity item in colecao.Entities)
                {
                    PontuacaoProduto novaPontuacao = new PontuacaoProduto();
                    novaPontuacao.NomeDoProduto = ((EntityReference)item["new_produtoid"]).Name;
                    novaPontuacao.ProdutoId = ((EntityReference)item["new_produtoid"]).Id;

                    pontuacoes.Add(novaPontuacao);
                }
            }

            if (pontuacoes.Count == 0)
            {
                queryHelper = new QueryExpression("new_promocao_pontuacao_fidelidade");
                queryHelper.ColumnSet.AllColumns = true;

                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produtoId));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_data_terminio", ConditionOperator.GreaterEqual, DateTime.Now));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_data_inicio", ConditionOperator.LessEqual, DateTime.Now));

                EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

                foreach (Entity item in colecao.Entities)
                {
                    PontuacaoProduto novaPontuacao = new PontuacaoProduto();
                    novaPontuacao.NomeDoProduto = ((EntityReference)item["new_produtoid"]).Name;
                    novaPontuacao.ProdutoId = ((EntityReference)item["new_produtoid"]).Id;

                    pontuacoes.Add(novaPontuacao);
                }
            }

            return pontuacoes;
        }

        public List<T> ObterPorClienteExtratoFidelidade(Guid cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("new_extrato_fidelidade", "productid", "new_produtoid");
            query.LinkEntities[0].AddLink("contact", "new_contatoid", "contactid");
            query.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, cliente));
            query.Distinct = true;
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterPorAutorizada(Guid autorizada)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;

            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));
            query.AddLink("new_produto_assistecia_tecica", "productid", "new_produtoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_assistencia_tecnicaid", ConditionOperator.Equal, autorizada));
            //query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("deletionstatecode", ConditionOperator.Equal, "0"));

            var colecao = this.RetrieveMultiple(query);

            return (List<T>)colecao.List;
        }

        public List<T> ObterPorAutorizada(Domain.Model.Conta autorizada, string[] codigoGrupoEstoque)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));

            // new_produto_assistecia_tecica
            query.AddLink("new_produto_assistecia_tecica", "productid", "new_produtoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_assistencia_tecnicaid", ConditionOperator.Equal, autorizada.ID.Value));
            //query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("deletionstatecode", ConditionOperator.Equal, "0"));
            
            // new_grupo_estoque
            query.AddLink("itbc_grupodeestoque", "itbc_grupodeestoque", "itbc_grupodeestoqueid");
            query.LinkEntities[1].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_codigo_grupo_estoque", ConditionOperator.In, codigoGrupoEstoque));

            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterPorVendedorExtratoFidelidade(Guid vendedor)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("new_extrato_fidelidade", "productid", "new_produtoid");
            query.LinkEntities[0].AddLink("contact", "new_vendedorid", "contactid");
            query.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.Equal, vendedor));
            query.Distinct = true;
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public void EnviarValorPSD()
        {
            var query = GetQueryExpression<T>(true);
            ProdutoService ProdutoServices = new ProdutoService(OrganizationName, IsOffline);
            string usuario = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
            string senha = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");            
            int qtderro = 0;
            int qtdcerto = 0;
            int linha = 2;
            int qtdtotalEnviado = 0;
            string erros = "";

            
            var excel = new ClosedXML.Excel.XLWorkbook();
            var ws = excel.Worksheets.Add("MaiorValorPSD");
            ws.Cell(1, 1).Value = "Produto";
            ws.Cell(1, 2).Value = "Sucesso";
            ws.Cell(1, 3).Value = "Erro";

            string dataAtual = DateTime.Now.ToString();
            dataAtual = dataAtual.Replace("/", "-").Replace(":", "-");

            string nomeArquivo = "MaiorValorPSD_" + dataAtual + ".xlsx";

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' aggregate='true'> ");
            strFetchXml.Append("   <entity name='product'>                                                                             ");
            strFetchXml.Append("     <attribute name='productid' alias='productid' groupby='true' />                                   ");
            strFetchXml.Append("     <attribute name='productnumber' alias='productnumber' groupby='true' />                           ");
            strFetchXml.Append("     <attribute name='statecode' alias='statecode' groupby='true' />                                   ");
            strFetchXml.Append("     <attribute name='statuscode' alias='statuscode' groupby='true' />                                 ");
            strFetchXml.Append("     <attribute name='description' alias='description' groupby='true' />                               ");
            strFetchXml.Append("     <link-entity name='itbc_produtosdalistapsdid' from='itbc_productid' to='productid' alias='ab'>    ");
            strFetchXml.Append("        <attribute name='itbc_valor' alias='itbc_valor_max' aggregate='max' />                         ");
            strFetchXml.Append("        <filter type='and'>                                                                            ");
            strFetchXml.Append("          <condition attribute='statecode' operator='eq' value='0' />                                  ");
            strFetchXml.Append("        </filter>                                                                                      ");
            strFetchXml.Append("       <link-entity name='itbc_psdid' from='itbc_psdidid' to='itbc_psdid' alias='ac'>                  ");
            strFetchXml.Append("       </link-entity >                                                                                 ");
            strFetchXml.Append("        <filter type='and'>                                                                            ");
            strFetchXml.Append("          <condition attribute='statecode' operator='eq' value='0' />                                  ");
            strFetchXml.Append("        </filter>                                                                                      ");
            strFetchXml.Append("     </link-entity>                                                                                    ");
            strFetchXml.Append("   </entity>                                                                                           ");
            strFetchXml.Append(" </fetch>                                                                                              ");


            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                string xml;
                string resposta;

                XDocument xmlroot = new XDocument(
                new XDeclaration("1.0", "utf-8", "no"),
                new XElement("Produto",
                    new XElement("Idprodutocrm", ((AliasedValue)item.Attributes["productid"]).Value),
                    new XElement("Idprodutoerp", ((AliasedValue)item.Attributes["productnumber"]).Value),
                    new XElement("Statuscode", ((OptionSetValue)((AliasedValue)item.Attributes["statuscode"]).Value).Value),
                    new XElement("Statecode", ((OptionSetValue)((AliasedValue)item.Attributes["statecode"]).Value).Value),
                    new XElement("Criado_em", DateTime.Now),
                    new XElement("MaiorValorPSD", ((Money)((AliasedValue)item.Attributes["itbc_valor_max"]).Value).Value),
                    new XElement("DataAtualizacaoPSD", DateTime.Now),
                    new XElement("DescricaoProduto", ((AliasedValue)item.Attributes["description"]).Value)
                    ));

                xml = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString();
                try
                {
                    new Domain.Servicos.SellOutService(this.OrganizationName, this.IsOffline).PersistirProdutoSellOut(usuario, senha, xml, out resposta);
                    // grava Log no excel
                    ws.Cell(linha, 1).Value = ((AliasedValue)item.Attributes["productnumber"]).Value.ToString();
                    ws.Cell(linha, 2).Value = "Sim";
                    qtdcerto++;
                }
                catch (Exception ex)
                {
                    ws.Cell(linha, 1).Value = ((AliasedValue)item.Attributes["productnumber"]).Value.ToString();
                    ws.Cell(linha, 2).Value = "Não";
                    SDKore.Helper.Error.Create("Problemas ao enviar maior valor PSD " + ((AliasedValue)item.Attributes["productnumber"]).Value.ToString() + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    erros += "<b> - </b> Problemas ao enviar maior valor PSD, produto [" + ((AliasedValue)item.Attributes["productnumber"]).Value.ToString() + "] <b> Erro: </b>" + ex.Message + "<br />";
                    qtderro++;
                    //Excel - Coluna Erro
                    ws.Cell(linha, 3).Value = ex.Message;
                }
                linha++;     
            }
            //Salva excel e envia e-mail.
            qtdtotalEnviado = linha - 2; //desconsiderar cabeçalho
            excel.SaveAs("c:\\temp\\" + nomeArquivo);            
            ProdutoServices.EnviaEmailRegistroMaiorValorPSD(qtdtotalEnviado, qtdcerto, qtderro, nomeArquivo);
        }        
    }
}
