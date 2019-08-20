using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using System.Text;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Data.SqlClient;

namespace Intelbras.CRM2013.DAL
{
    public class RepUsuario<T> : CrmServiceRepository<T>, IUsuario<T>
    {
        #region Objeto Q obtem a conexao com o SQL
        private DataBaseSqlServer _DataBaseSqlServer = null;
        private DataBaseSqlServer DataBaseSqlServer
        {
            get
            {
                if (_DataBaseSqlServer == null)
                    _DataBaseSqlServer = new DataBaseSqlServer(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.CRM2013.Database"));

                return _DataBaseSqlServer;
            }
        }
        #endregion

        public List<T> ListarPor(Guid userId)
        {
            var query = GetQueryExpression<T>(true);
            ConditionExpression cond1 = new ConditionExpression("systemuserid", ConditionOperator.Equal, userId);
            query.Criteria.AddCondition(cond1);
            //apenas users não habilitados
            query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
            query.Orders.Add(new OrderExpression("firstname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUsuarioDesabilitado(Guid userId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("systemuserid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, userId);
            query.Criteria.AddCondition(cond1);
            //apenas users não habilitados
            query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, true);

            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("firstname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(String accountid)
        {
            {
                var query = GetQueryExpression<T>(true);

                Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("systemuserid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, accountid);
                query.Criteria.Conditions.Add(cond1);
                //apenas users habilitados
                query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);

                return (List<T>)this.RetrieveMultiple(query).List;
            }
        }

        public DataTable ListarUsuarioSemAcessar40Dias()
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT distinct FSU.systemuserid, lastaccesstime ");
            strSql.Append("FROM SystemUser FSU ");
            strSql.Append("INNER JOIN[MSCRM_CONFIG].[dbo].[SystemUserOrganizations] SUO ON SUO.CrmUserId = FSU.SystemUserId ");
            strSql.AppendFormat("where suo.LastAccessTime < GETDATE() - 40 and FSU.IsDisabled = 0 and FSU.itbc_funcao is null ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());

        }

        public T ObterUsuarioPorMatricula(string matricula)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("domainname", ConditionOperator.Equal, matricula);
            query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Guid systemuserid)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("systemuserid", ConditionOperator.Equal, systemuserid);
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPorCodigoAssistente(int itbc_codigodoassistcoml)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_codigodoassistcoml", ConditionOperator.Equal, itbc_codigodoassistcoml);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorCodigoSupervisorEMS(String itbc_codigo_supervisor)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_codigo_supervisor", ConditionOperator.Equal, itbc_codigo_supervisor);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(string login)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
            query.Criteria.AddCondition("domainname", ConditionOperator.Equal, login);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AlterarStatus(Guid usuarioId, int state, int status)
        {

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("systemuser", usuarioId),
                State = new OptionSetValue(state),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public List<T> ListarSupervisoresPor(Guid unidadeId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("businessunitid", ConditionOperator.Equal, unidadeId);
            query.Criteria.AddCondition("itbc_funcao", ConditionOperator.Equal, 993520000);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("fullname", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarSupervisoresPorMeta(Guid metaId)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições
            //query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
            query.Criteria.AddCondition("itbc_funcao", ConditionOperator.Equal, 993520000);

            query.AddLink("itbc_potencialdosupervisor", "systemuserid", "itbc_supervisor")
                .AddLink("itbc_metaportrimestre", "itbc_potencialportrimestredaunidadeid", "itbc_metaportrimestreid")
                .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("fullname", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarSupervisoresPor(Guid unidadeId, List<Guid> lstIdSupervisor)
        {

            var query = GetQueryExpression<T>(true);

            #region Condições
            //query.Criteria.AddCondition("isdisabled", ConditionOperator.Equal, false);
            query.Criteria.AddCondition("itbc_funcao", ConditionOperator.Equal, 993520000);
            query.Criteria.AddCondition("businessunitid", ConditionOperator.Equal, unidadeId);

            if (lstIdSupervisor != null && lstIdSupervisor.Count > 0)
                query.Criteria.AddCondition(new ConditionExpression("systemuserid", ConditionOperator.NotIn, lstIdSupervisor));
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("fullname", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterSupervisorPorRepresentanteKA(Guid KARepresentanteId)
        {
            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>                                           ");
            strFetchXml.Append("  <entity name='systemuser'>                                                                                                   ");
            strFetchXml.Append("    <attribute name='fullname' />                                                                                              ");
            strFetchXml.Append("    <attribute name='businessunitid' />                                                                                        ");
            strFetchXml.Append("    <attribute name='title' />                                                                                                 ");
            strFetchXml.Append("    <attribute name='address1_telephone1' />                                                                                   ");
            strFetchXml.Append("    <attribute name='systemuserid' />                                                                                          ");
            strFetchXml.Append("    <order attribute='fullname' descending='false' />                                                                          ");
            strFetchXml.Append("    <link-entity name='itbc_portfoliokeyaccountrepresentantes' from='itbc_supervisordevendas' to='systemuserid' alias='ab'>    ");
            strFetchXml.Append("      <filter type='and'>                                                                                                      ");
            strFetchXml.AppendFormat("        <condition attribute='itbc_contatoid' operator='eq' uitype='contact' value='{0}' />                                    ", KARepresentanteId);
            strFetchXml.Append("      </filter>                                                                                                                ");
            strFetchXml.Append("    </link-entity>                                                                                                             ");
            strFetchXml.Append("  </entity>                                                                                                                    ");
            strFetchXml.Append("</fetch>                                                                                                                       ");

                                                                                                                                        
             // Build fetch request and obtain results.                                                                                                                             
            retrieveMultiple = new RetrieveMultipleRequest()                                                                                                                        
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            var lst = this.RetrieveMultiple(retrieveMultiple.Query).List;
            if (lst.Count > 0)
                return this.RetrieveMultiple(retrieveMultiple.Query).List[0];

            return default(T);  
        }
        
        public T ObterPor(string login, string nomeDeDominio)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            if (!String.IsNullOrEmpty(nomeDeDominio)) query.Criteria.Conditions.Add(new ConditionExpression("domainname", ConditionOperator.Equal, nomeDeDominio));
            if (!String.IsNullOrEmpty(login)) query.Criteria.Conditions.Add(new ConditionExpression("new_login", ConditionOperator.Equal, login));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public void AlterarStatus(Guid usuarioId, bool ativar)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("systemuser", usuarioId),
                State = new OptionSetValue(ativar ? 0 : 1),
                Status = new OptionSetValue(-1)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public List<T> ListarPorFamiliaComercial(Product produto)
        {
            if (produto != null && produto.DadosFamiliaComercial != null && produto.LinhaComercial != null)
            {
                var query = GetQueryExpression<T>(true);
                query.AddLink("new_systemuser_new_linha_unidade_negocio", "systemuserid", "systemuserid");
                query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, produto.LinhaComercial.Id));
                return (List<T>)this.RetrieveMultiple(query).List;
            }
            else
                return new List<T>();
        }

        public T BuscarProprietario(string entidadeRelacionada, string parametroIdEntidadeRelacionada, Guid idEntidadeRelacionada)
        {
            var query = GetQueryExpression<T>(true);

            query.AddLink(entidadeRelacionada, "systemuserid", "ownerid");

            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression(parametroIdEntidadeRelacionada, ConditionOperator.Equal, idEntidadeRelacionada));

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
    }
}
