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

    public class RepMetadaUnidade<T> : CrmServiceRepository<T>, IMetadaUnidade<T>
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

        public MetadaUnidade ObterValoresPor(Guid unidadeNegocioId, int ano)
        {
            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_metadokaporsegmento' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <filter type='and' >
                                  <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                </filter>
                              </entity>
                            </fetch>";

            fetch = string.Format(fetch, ano, unidadeNegocioId);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            if (collection.Entities.Count > 0)
            {
                var item = collection.Entities[0];

                var metaDaUnidade = new MetadaUnidade(OrganizationName, IsOffline, Provider);

                if (item.Attributes.Contains("valor_planejado")) metaDaUnidade.MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value;
                if (item.Attributes.Contains("valor_realizado")) metaDaUnidade.MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value;


                return metaDaUnidade;
            }
            else
            {
                return null;
            }
        }

        public List<T> ListarMetas(int ano)
        {
            return ListarMetas(Guid.Empty, ano);
        }

        public List<T> ListarMetas(Guid unidadenegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            if (!unidadenegocioId.Equals(Guid.Empty))
                query.Criteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, unidadenegocioId);

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterLerGerarPlanilha(int status)
        {
            var query = GetQueryExpression<T>(true);

            ConditionExpression cond1 = new ConditionExpression("statuscode", ConditionOperator.Equal, status);
            query.Criteria.Conditions.Add(cond1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Domain.Enum.MetaUnidade.RazaodoStatusMetaManual status)
        {
            var query = GetQueryExpression<T>(true);

            ConditionExpression cond1 = new ConditionExpression("itbc_razaodostatusmetamanual", ConditionOperator.Equal, (int)status);
            query.Criteria.Conditions.Add(cond1);
            
            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarPor(Domain.Enum.MetaUnidade.RazaodoStatusMetaKARepresentante status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_razodostatusmetakarepresentante", ConditionOperator.Equal, (int)status);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("createdon", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ObterLerGerarPlanilhaSupervisor(int status)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_razodostatusmetasupervisor", ConditionOperator.Equal, status);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("createdon", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        /********************      ACESSO SQL      ********************/
        public DataTable ObterCapaDW(int ano, List<MetadaUnidade> lstMetasUnidade)
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("Select count(*), cd_ano,CD_Unidade_Negocio,sum(nm_quantidade_total) as Qtde,sum(nm_vl_liquido_total) as vlr ");
            strSql.Append("from viewFaturamentoCRM ");
            strSql.AppendFormat("where CD_Ano = {0} ", ano);

            if (lstMetasUnidade != null)
            {
                strSql.Append("and CD_Unidade_Negocio in(");
                foreach (var item in lstMetasUnidade)
                    commandIn += string.Concat("'", item.UnidadedeNegocio.ChaveIntegracao, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(")");
            }

            strSql.Append("group by cd_ano,CD_Unidade_Negocio ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }

    }
}

