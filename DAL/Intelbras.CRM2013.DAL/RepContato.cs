using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepContato<T> : CrmServiceRepository<T>, IContato<T>
    {
        public List<T> ListarPor(Conta conta)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            
            #region Status
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));
            #endregion

            ConditionExpression cond1 = new ConditionExpression("parentcustomerid", ConditionOperator.Equal, conta.ID.Value);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarPorEmail(string email, string cpfCnpj, bool apenasAtivos = true, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            if (apenasAtivos)
            {
                query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Contato.StateCode.Ativo);
            }

            var filter = new FilterExpression(LogicalOperator.Or);
            if (columns != null || columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            if (!string.IsNullOrEmpty(email))
            {
                filter.Conditions.Add(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));
            }

            cpfCnpj = cpfCnpj.GetOnlyNumbers();
            if (!string.IsNullOrEmpty(cpfCnpj))
            {
                filter.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj));
                filter.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.InputMask()));
            }
            query.Criteria.AddFilter(filter);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarSemAcessoKonviva()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo);

            #region Critérios para seleção dos contatos sem acesso ao Konviva

            LinkEntity link = query.AddLink("itbc_acessoaokonviva", "contactid", "itbc_contatoid", JoinOperator.LeftOuter);
            link.EntityAlias = "ack";

            LinkEntity link2 = query.AddLink("itbc_acessosextranetcontatos", "contactid", "itbc_contactid", JoinOperator.Inner);
            link2.EntityAlias = "aexc";

            query.Criteria = new FilterExpression();
            query.Criteria.AddCondition("ack", "itbc_acessoaokonvivaid", ConditionOperator.Null);
            query.Criteria.AddCondition("aexc", "itbc_acessoextranetid", ConditionOperator.NotNull);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContatosSemMascara()
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>");
            strFetchXml.Append("<entity name='contact'>");
            strFetchXml.Append("<attribute name='contactid'/>");
            strFetchXml.Append("<attribute name='itbc_cpfoucnpj'/>");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-null'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%a%'/>");
            strFetchXml.Append("<condition attribute='statecode' operator='eq' value='0'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%c%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%o%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%.%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%r%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%m%'/>");
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch> ");

            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarPor(String cpfCnpj)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.GetOnlyNumbers()));
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.InputMask()));
            query.Orders.Add(new OrderExpression("firstname", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCodigoRepresentante(string codigoRepresentante)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_codigodorepresentante", ConditionOperator.Equal, codigoRepresentante);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("firstname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarAssociadosA(string codigoConta)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("parentcustomerid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoConta);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("firstname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCodigoRepresentante(string[] codigosDeRepresentante)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.Columns.Add("itbc_codigodorepresentante");

            #region Condições
            ConditionExpression cond1 = new ConditionExpression("itbc_codigodorepresentante", ConditionOperator.In, codigosDeRepresentante);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("firstname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(string codigorepresentante)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_codigodorepresentante", ConditionOperator.Equal, codigorepresentante);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorIntegracaoCrm4(string guiCrm40Contato)
        {
            var query = GetQueryExpression<T>(true);
            #region Condições
            query.Criteria.AddCondition("itbc_guidcrm40", ConditionOperator.Equal, guiCrm40Contato);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public Boolean AlterarStatus(Guid contactid, int status)
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
                EntityMoniker = new EntityReference("contact", contactid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public List<T> ListarKaRepresentantes(Guid? unidadenegocioId)
        {
            return this.ListarKaRepresentantes(unidadenegocioId, null, null, null);
        }

        public List<T> ListarKaRepresentantes(Guid? unidadenegocioId, List<Guid> lstIdContatos)
        {
            return this.ListarKaRepresentantes(unidadenegocioId, lstIdContatos, null, null);
        }

        public List<T> ListarKaRepresentantes(Guid? unidadenegocioId, List<Guid> lstIdContatos, int? pagina, int? contagem)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("customertypecode", ConditionOperator.Equal, "993520007");

            if (lstIdContatos != null && lstIdContatos.Count > 0)
                query.Criteria.AddCondition(new ConditionExpression("contactid", ConditionOperator.NotIn, lstIdContatos));

            if (unidadenegocioId.HasValue)
                query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadenegocioId);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("firstname", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
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

        public List<T> ListarKaRepresentantesPotencial(Guid? unidadeNegocioId, int ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null || columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
        }

            query.AddLink("itbc_metakeyaccount", "contactid", "itbc_contact");
            if (unidadeNegocioId.HasValue)
        {
                query.LinkEntities[0].LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            }

            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            query.AddOrder("firstname", OrderType.Ascending);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public int TipoAcessoPortal(string login)
        {
            int representante = 0,
                assistenciaTecnica = 0;

            var queryHelper = new QueryExpression("new_permissao_usuario_b2b");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.AddLink("contact", "new_contatoid", "contactid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_login", ConditionOperator.Equal, login));            
            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                foreach (Entity item in bec.Entities)
                {
                    if (item.Attributes.Contains("itbc_codigo_representante"))
                        representante = 1;

                    else if (item.Attributes.Contains("new_unidade_negocioid"))
                        assistenciaTecnica = 2;
                }

            }

            return (representante + assistenciaTecnica);
        }

        public List<T> ListarContatosPor(Contato contato)
        {
            var queryHelper = GetQueryExpression<T>(true);
            if (contato.Id != Guid.Empty) queryHelper.Criteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.Equal, contato.Id));
            if (!string.IsNullOrEmpty(contato.CpfCnpj))
            {
                var filter = new FilterExpression(LogicalOperator.Or);
                var cpfCnpj = contato.CpfCnpj.GetOnlyNumbers();
                if (!string.IsNullOrEmpty(cpfCnpj))
                {
                    filter.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj));
                    filter.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.InputMask()));
                }
                queryHelper.Criteria.AddFilter(filter);
            }
            if (contato.AcessoAoPortal.HasValue && contato.AcessoAoPortal.Value)
            {
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_login", ConditionOperator.NotNull));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_login", ConditionOperator.GreaterThan, ""));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_acessoportal", ConditionOperator.NotNull));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_acessoportal", ConditionOperator.Equal, true));
            }
            if (!string.IsNullOrEmpty(contato.Nome))
            {
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("firstname", ConditionOperator.Like, "%" + contato.Nome + "%"));
            }
            if (contato.AssociadoA != null)
            {
                if (contato.AssociadoA.Id != Guid.Empty)
                    queryHelper.Criteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, contato.AssociadoA.Id));
            }
            if (contato.Regional != null)
            {
                if (contato.Regional.Id != Guid.Empty)
                    queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_regionalid", ConditionOperator.Equal, contato.Regional.Id));
            }
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<T> ListarContatosPor(Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, cliente.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(string login, Guid contatoId)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.TopCount = 1;
            if (!string.IsNullOrEmpty(login))
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_login", ConditionOperator.Equal, login));
            if (contatoId != Guid.Empty)
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.NotEqual, contatoId));
            var colecao = this.RetrieveMultiple(queryHelper);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(string cpf, string email)
        {
            var query = GetQueryExpression<T>(true);

            var queryHelperOr = new FilterExpression(LogicalOperator.Or);
            query.Criteria.AddFilter(queryHelperOr);

            if (!String.IsNullOrEmpty(email))
                queryHelperOr.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));

            if (!String.IsNullOrEmpty(cpf))
            {
                queryHelperOr.AddCondition(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpf));
                queryHelperOr.AddCondition(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpf.InputMask()));
                queryHelperOr.AddCondition(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpf.Replace(".", "").Replace("/", "").Replace("-", "")));
            }
            var queryHelperAnd = new FilterExpression(LogicalOperator.And);
            query.Criteria.AddFilter(queryHelperAnd);
            queryHelperAnd.AddCondition("statecode", ConditionOperator.Equal, 0);

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarPorEmail(string email)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public void UpdateEmailFBA(Contato contato)
        {
            SqlConnection conexao = null;

            try
            {
                string sqlGetUserId = string.Format("select [UserId] from aspnet_Users where UserName = '{0}' or LoweredUserName = '{1}'", contato.Login.Trim(), contato.Login.ToLower().Trim());
                string stringConn = SDKore.Configuration.ConfigurationManager.GetSettingValue("connector");

                conexao = new SqlConnection(stringConn);
                conexao.Open();

                if (conexao.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Não foi possível conectar ao banco de dados do FBA, \n\n Conexão: " + stringConn);
                }

                SqlCommand cmd = new SqlCommand(sqlGetUserId, conexao);
                object resultado = cmd.ExecuteScalar();

                if (resultado == null)
                {
                    throw new ArgumentException("CRM Application", "Login de Contato : [" + contato.Login + "] não encontrado no FBA!");
                }

                Guid UserIdFBA = (Guid)resultado;

                var sqlUpdateEmail = string.Format("UPDATE [aspnet_Membership] SET [Email] = '{0}', [LoweredEmail] = '{1}' WHERE UserId = '{2}'",
                                     contato.Email1,
                                     contato.Email1.ToLower(),
                                     resultado.ToString());

                cmd = new SqlCommand(sqlUpdateEmail, conexao);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception("CRM SQL não executou ao tentar atualizar contato no FBA \n\n SQL: " + sqlUpdateEmail);
                }
            }
            finally
            {
                if (conexao != null && conexao.State != System.Data.ConnectionState.Closed)
                {
                    conexao.Close();
                    conexao.Dispose();
                }
            }
        }

        public void EnviaEmailAcessoPortalCorporativo(Contato contato)
        {
            //Envia e-mail e salva como tarefa no CRM 
            try
            {
                string acesso = "";
                if (contato.AcessoAoPortal.HasValue && contato.AcessoAoPortal.Value)
                {
                    Email email = new Email(OrganizationName, IsOffline);
                    email.ReferenteAId = contato.Id;
                    email.ReferenteAType = "contact";
                    email.ReferenteAName = contato.NomeCompleto;
                    email.ReferenteA = new Lookup(contato.Id, contato.NomeCompleto, "contact");
                    email.Assunto = "Acesso ao Portal Corporativo Intelbras";

                    var contatoEmail = new ContatoService(OrganizationName, this.IsOffline).BuscaContato(contato.Id);
                    /*
                     * @Author José Luiz Silveira.
                     * Comentado pois o portal do fidelidade foi descontinuado. 
                     *
                    if(contato.ParticipaFidelidade.HasValue)
                        acesso = contato.ParticipaFidelidade.ToString();
                    */

                    //email.Mensagem = (new Domain.Servicos.RepositoryService(OrganizationName, IsOffline).ParametroGlobal.ObterPorCodigoTipoParametroGlobal(0).Valor .Configuration.ConfigurationManager.GetSettingValue("EMAIL_CORPORATIVO").Replace("#ID_CONTATO#", contato.Id.ToString()).Replace("#LOGIN_CONTATO#", contato.Login).Replace("#EMAIL_CONTATO#", contato.Email1).Replace("#PARTICIPA_FIDELIDADE#", fidelidade);
                    string corpoEmail = @"<style type=""text/css""> pre.mscrmpretag {  font-family: Tahoma, Verdana, Arial; style=""word-wrap: break-word;"" }</style>
                    <FONT size=2 face=Calibri>Prezado Usuário, <br /><br />
                    Você recebeu acesso ao Portal Corporativo Intelbras. Basta acessar o portal 
                    <STRONG><a href=""http://corporativo.intelbras.com.br/_auth/account/register.aspx?CRMID=#ID_CONTATO#&login=#LOGIN_CONTATO#&email=#EMAIL_CONTATO#&fidelidade=#PARTICIPA_FIDELIDADE#"">clicando aqui</a></STRONG> e cadastrar sua senha.
                    <br /><br />Seu login de acesso ao Portal Corporativo Intelbras é <STRONG>#LOGIN_CONTATO#</STRONG><br /><br />Obrigado,<br /><br />Intelbras S.A.</FONT>";
                    email.Mensagem = corpoEmail.Replace("#ID_CONTATO#", contato.Id.ToString()).Replace("#LOGIN_CONTATO#", contatoEmail.Login).Replace("#EMAIL_CONTATO#", contatoEmail.Email1).Replace("#PARTICIPA_FIDELIDADE#", acesso);
                    //email.De[0] = new Lookup(new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("ID_EMAIL_CORPORATIVO")), "systemuser");
                    email.De   = new Lookup[] { new Lookup(new Guid(SDKore.Configuration.ConfigurationManager.GetSettingValue("ID_EMAIL_CORPORATIVO")), "systemuser") };
                    email.Para = new Lookup[] { new Lookup(contato.Id, "contact") };

                    email.Direcao = false;
                    email.ID = (new CRM2013.Domain.Servicos.RepositoryService()).Email.Create(email);

                    (new CRM2013.Domain.Servicos.RepositoryService()).Email.EnviarEmail(email.ID.Value);
                }
            }
            catch (Exception ex)
            {
                SDKore.Helper.Log.Logar("emailportalcorportivolog.txt",ex.Message + ex.StackTrace);
            }
        }

        public string ObterSenha(Guid contatoid)
        {
            SqlConnection conexao = null;
            string senha = string.Empty;

            try
            {
                conexao = new SqlConnection(SDKore.Configuration.ConfigurationManager.GetDataBase("connector"));
                conexao.Open();

                string sqlGetUserId = string.Format("select m.Password from AutenticacaoExterna.dbo.aspnet_Membership m, AutenticacaoExterna.dbo.aspnet_Users u where u.UserId = m.UserId and u.CRMGuidContact = '{0}'", contatoid.ToString());

                SqlCommand cmd = new SqlCommand(sqlGetUserId, conexao);
                object retorno = cmd.ExecuteScalar();
                senha = String.IsNullOrEmpty(retorno.ToString()) ? string.Empty : retorno.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conexao != null)
                    if (conexao.State != System.Data.ConnectionState.Closed)
                    {
                        conexao.Close();
                        conexao.Dispose();
                    }
            }

            return senha;
        }

        public void ExecutaWorkFlow(Contato contato, Guid WorkFlowId)
        {
            ExecuteWorkflowRequest request = new ExecuteWorkflowRequest()
            {
                WorkflowId = WorkFlowId,
                EntityId = contato.Id
            };

            ExecuteWorkflowResponse response = (ExecuteWorkflowResponse)base.Execute(request);
        }

        public List<T> ListarPor(int diaAniversario, int mesAniversario)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_aniversario_dia", ConditionOperator.Equal, diaAniversario));
            query.Criteria.Conditions.Add(new ConditionExpression("new_aniversario_mes", ConditionOperator.Equal, mesAniversario));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
        
        public List<KeyValuePair<Guid, string>> ListarVendedorFidelidade(Guid clienteId)
        {
            List<KeyValuePair<Guid, string>> lista = new List<KeyValuePair<Guid, string>>();
            if (clienteId == Guid.Empty) return lista;

            var queryHelper = new QueryExpression("contact");
            queryHelper.ColumnSet.AddColumns(new string[]{"contactid","name"});
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, clienteId));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_cargo", ConditionOperator.Equal, 20)); // Vendedor
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_participafidelidade", ConditionOperator.Equal, true));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            foreach (Entity item in bec.Entities)
                lista.Add(new KeyValuePair<Guid, string>(item.Id, Convert.ToString(item["name"])));

            return lista;
        }

        public List<T> ListarContatosComCep()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("address1_postalcode", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarComNFE()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("firstname", ConditionOperator.Like, "nfe%"));
            query.Criteria.Conditions.Add(new ConditionExpression("emailaddress1", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.NotNull));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        // Metodo utilizado apenas para alteração de CEP em massa
        public List<T> ListarContatosComCep(DateTime ultimaDataModificacao)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("address1_postalcode", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            query.Criteria.Conditions.Add(new ConditionExpression("modifiedon", ConditionOperator.LessEqual, ultimaDataModificacao));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        //CRM4
        public T ObterRepresentatePor(Domain.Model.Conta cliente)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.TopCount = 1;
            queryHelper.AddLink("account", "contactid", "itbc_representante");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("accountid", ConditionOperator.Equal, cliente.Id));
            var colecao = this.RetrieveMultiple(queryHelper);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
            //Representante rep = null;
            //var query = new QueryExpression("contact");
            //query.TopCount = 1;
            //query.AddLink("account", "contactid", "new_representanteid");
            //query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("accountid", ConditionOperator.Equal, cliente.Id));
            //var bec = base.Provider.RetrieveMultiple(query);
            //if (bec.Entities.Count > 0)
            //{
            //    rep = new Representante(this.OrganizationName, this.IsOffline);
            //    Entity contato = bec.Entities[0];
            //    rep.CodigoRepresentante = Convert.ToInt32(contato["itbc_codigodorepresentante"]);
            //    rep.Nome = Convert.ToString(contato["name"]);
            //    rep.Id = contato.Id;
            //}
            //return rep;
        }

        public T ObterPorDuplicidade(string cpf, string login)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 0;
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpf));
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpf.InputMask()));
            query.Criteria.Conditions.Add(new ConditionExpression("new_login",ConditionOperator.Equal, login));
            var colecao = this.RetrieveMultiple(query);
            return colecao.List.Count == 0 ? default(T) : colecao.List[0];
        }

        public List<T> ObterVendedores(Guid distribuidorId)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("firstname", OrderType.Ascending));
            query.Criteria.Conditions.Add(new ConditionExpression("parentcustomerid", ConditionOperator.Equal, distribuidorId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_cargo", ConditionOperator.Equal, (int)Domain.Enum.Contato.Cargo.Vendedor));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ObterTodosComExtratoFidelidade(int quantidade, int pagina, string cookie)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("new_extrato_fidelidade", "contactid", "new_contatoid");

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = quantidade;
            query.PageInfo.PageNumber = pagina;
            query.PageInfo.PagingCookie = cookie;

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public void AssociarAreasAtuacao(List<AreaAtuacao> areasAtuacao, Guid contatoid)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach(AreaAtuacao areaAtuacao in areasAtuacao)
            {
                relatedEntities.Add(new EntityReference("itbc_area_atuacao", areaAtuacao.Id));
            }

            this.Provider.Associate(
                   "contact",
                   contatoid,
                   new Relationship("itbc_contact_itbc_area_atuacao"),
                   relatedEntities
               );
        }

        public void DesassociarAreasAtuacao(List<AreaAtuacao> areasAtuacao, Guid contatoid)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (AreaAtuacao areaAtuacao in areasAtuacao)
            {
                relatedEntities.Add(new EntityReference("itbc_area_atuacao", areaAtuacao.Id));
            }

            this.Provider.Disassociate(
                   "contact",
                   contatoid,
                   new Relationship("itbc_contact_itbc_area_atuacao"),
                   relatedEntities
               );
        }


        public void AssociarMarcas(List<Marca> marcas, Guid contatoid)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (Marca marca in marcas)
            {
                relatedEntities.Add(new EntityReference("itbc_marca_equipamento", marca.Id));
            }

            this.Provider.Associate(
                   "contact",
                   contatoid,
                   new Relationship("itbc_contact_itbc_marca_equipamento"),
                   relatedEntities
               );
        }

        public void DesassociarMarcas(List<Marca> marcas, Guid contatoid)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (Marca marca in marcas)
            {
                relatedEntities.Add(new EntityReference("itbc_marca_equipamento", marca.Id));
            }

            this.Provider.Disassociate(
                   "contact",
                   contatoid,
                   new Relationship("itbc_contact_itbc_marca_equipamento"),
                   relatedEntities
               );
        }
    }
}