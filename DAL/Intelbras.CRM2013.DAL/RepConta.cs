using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos.Docs;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Configuration;
using SDKore.Crm;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using SDKore.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Intelbras.CRM2013.DAL
{
    public class RepConta<T> : CrmServiceRepository<T>, IConta<T>
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

        public List<T> ListarPor(String razaoSocial)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, razaoSocial));
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCategoria(string[] Categorias)
        {
            var query = GetQueryExpression<T>(true);


            #region Condições

            ConditionExpression cond2 = new ConditionExpression("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);
            query.Criteria.Conditions.Add(cond2);
            ConditionExpression cond3 = new ConditionExpression("statecode", ConditionOperator.Equal, (int)Domain.Enum.Conta.StateCode.Ativo);
            query.Criteria.Conditions.Add(cond3);

            LinkEntity link = query.AddLink("itbc_categoria", "itbc_categoria", "itbc_categoriaid", JoinOperator.Inner);
            link.EntityAlias = "ctt";
            query.Criteria.AddCondition("ctt", "itbc_codigo_categoria", ConditionOperator.In, Categorias);

            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ListarContasFiliaisPorMatriz(Guid contaMatriz)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_matrizoufilial", ConditionOperator.Equal, (int)Domain.Enum.Conta.MatrizOuFilial.Filial));
            query.Criteria.Conditions.Add(new ConditionExpression("parentaccountid", ConditionOperator.Equal, contaMatriz.ToString()));
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim));
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContasSemMascara()
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>");
            strFetchXml.Append("<entity name='account'>");
            strFetchXml.Append("<attribute name='accountid'/>");
            strFetchXml.Append("<attribute name='itbc_cpfoucnpj'/>");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-null'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%e%'/>");
            strFetchXml.Append("<condition attribute='statecode' operator='eq' value='0'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%s%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%u%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%.%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%n%'/>");
            strFetchXml.Append("<condition attribute='itbc_cpfoucnpj' operator='not-like' value='%p%'/>");
            strFetchXml.Append("</filter>");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch> ");

            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarFiliaisMatriz(Guid contaId, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }
            query.Criteria.AddCondition("parentaccountid", ConditionOperator.Equal, contaId.ToString());
            query.Criteria.AddCondition("accountid", ConditionOperator.Equal, contaId.ToString());
            query.Criteria.FilterOperator = LogicalOperator.Or;
            OrderExpression ord1 = new OrderExpression("name", OrderType.Ascending);
            query.Orders.Add(ord1);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorCodigoEmitente(String accountNumber)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("accountnumber", ConditionOperator.Equal, accountNumber));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(string CPFCNPJ, int tipoConstituicao)
        {
            var query = GetQueryExpression<T>(true);

            if (tipoConstituicao != 993520002)
            {
                query.Criteria.FilterOperator = LogicalOperator.Or;
                query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ.InputMask());
                query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ.GetOnlyNumbers());
            }
            else
            {
                query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ);
            }
            //Trazer primeiro as ativas pois existiam contas com CNPJs repetidos no CRM, isso evitaria
            query.Orders.Add(new OrderExpression("statecode", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(string CPFCNPJ)
        {
            var query = GetQueryExpression<T>(true);

                query.Criteria.FilterOperator = LogicalOperator.Or;
                query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ.InputMask());
                query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ.GetOnlyNumbers());

            //Trazer primeiro as ativas pois existiam contas com CNPJs repetidos no CRM, isso evitaria
            query.Orders.Add(new OrderExpression("statecode", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterOutraContaPorCpfCnpj(string CPFCNPJ, Guid id, int tipoConstituicao)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("accountid", ConditionOperator.NotEqual, id);

            if (tipoConstituicao != 993520002) {
                var filter = new FilterExpression(LogicalOperator.Or);
                filter.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ.InputMask()));
                filter.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ.GetOnlyNumbers()));
                query.Criteria.AddFilter(filter);
            }else
            {
                query.Criteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Equal, CPFCNPJ);
            }
            query.Orders.Add(new OrderExpression("statecode", OrderType.Ascending));

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }



        public T ObterPorIntegracaoCrm4(String guidCrm40conta)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_guidcrm40", ConditionOperator.Equal, guidCrm40conta));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public void AtribuirEquipeParaConta(Guid Equipe, Guid Conta)
        {
            AssignRequest assignRequest = new AssignRequest()
            {
                Assignee = new EntityReference
                {
                    LogicalName = "team",
                    Id = Equipe
                },

                Target = new EntityReference("account", Conta)
            };
            this.Execute(assignRequest);
        }

        public void AdicionarPerfilParaEquipe(Guid Equipe)
        {
            Guid perfilEquipe;

            if (!Guid.TryParse(ConfigurationManager.GetSettingValue("PerfilIdParaEquipe"), out perfilEquipe))
                throw new ArgumentException("Parâmetro SDKore 'PerfilIdParaEquipe' configurado incorretamente");

            this.Provider.Associate(
                   "team",
                   Equipe,
               new Relationship("teamroles_association"),
               new EntityReferenceCollection() { new EntityReference("role", perfilEquipe) }
               );
        }

        public Boolean AlterarStatus(Guid account, Domain.Enum.Conta.StateCode stateCode, Domain.Enum.Conta.StatusCode statusCode)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("account", account),
                State = new OptionSetValue((int)stateCode),
                Status = new OptionSetValue((int)statusCode)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
            if (resp != null)
                return true;

            return false;
        }

        public ExecuteMultipleResponse UpdateMultiplos(List<T> collection)
        {
            EntityCollection listEntities = new EntityCollection();
            ExecuteMultipleRequest requestWithResults = new ExecuteMultipleRequest()
            {
                // Assign settings that define execution behavior: continue on error, return responses. 
                Settings = new ExecuteMultipleSettings()
                {
                    ContinueOnError = true,
                    ReturnResponses = true
                },
                // Create an empty organization request collection.
                Requests = new OrganizationRequestCollection()
            };

            requestWithResults.Requests = new OrganizationRequestCollection();

            foreach (T entity in collection)
            {
                var ent = EntityConvert.Convert<T>(entity, this.OrganizationName, this.IsOffline);
                listEntities.Entities.Add(ent);
                requestWithResults.Requests.Add(new UpdateRequest() { Target = ent });
            }

            this.Provider.Timeout = new TimeSpan(0, 50, 0); //50 minutos
            return (ExecuteMultipleResponse)this.Provider.Execute(requestWithResults);

        }

        public void AlterarStatus(string entidade, Guid id, int status, int razaoStatus)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference(entidade, id),
                State = new OptionSetValue(status),
                Status = new OptionSetValue(razaoStatus)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public List<T> ListarTudo(params string[] columns)
        {
            var query = GetQueryExpression<T>(true);
            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, Domain.Enum.Conta.StateCode.Ativo);
            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public List<T> ListarContas(ref int pagina, int contagem, out bool moreRecords)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            PagingInfo paging = new PagingInfo();
            paging.Count = contagem;
            paging.PageNumber = pagina;
            query.PageInfo = paging;
            DomainCollection<T> retorno = this.RetrieveMultiple(query);
            moreRecords = retorno.MoreRecords;
            pagina++;
            return (List<T>)retorno.List;
        }

        public List<T> ListarMatrizesParticipantes()
        {
            var query = GetQueryExpression<T>(true);

            ConditionExpression cond1 = new ConditionExpression("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);
            query.Criteria.Conditions.Add(cond1);

            ConditionExpression cond2 = new ConditionExpression("itbc_matrizoufilial", ConditionOperator.Equal, (int)Domain.Enum.Conta.MatrizOuFilial.Matriz);
            query.Criteria.Conditions.Add(cond2);

            //ativo
            ConditionExpression cond3 = new ConditionExpression("statecode", ConditionOperator.Equal, 0);
            query.Criteria.Conditions.Add(cond3);

            OrderExpression ord1 = new OrderExpression("name", OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;

        }

        public List<T> ListarMatrizesParticipantesApuracaoCentralizadaNaMatriz()
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            query.Criteria.AddCondition("itbc_matrizoufilial", ConditionOperator.Equal, (int)Domain.Enum.Conta.MatrizOuFilial.Matriz);

            query.Criteria.AddCondition("itbc_apuracaodebeneficiosecompromissos", ConditionOperator.Equal, (int)Domain.Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz);

            //ativo
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);


            OrderExpression ord1 = new OrderExpression("name", OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContasParticipantesApuracaoPorFilial()
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            query.Criteria.AddCondition("itbc_apuracaodebeneficiosecompromissos", ConditionOperator.Equal, (int)Domain.Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais);

            //ativo
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);


            //Observação de Carlos Roweder Nass em 04/05/2016
            //Tirado o Order by para maior rapidez na consulta pois estava demorando muito
            //Para voltar a rodar com order no campo name, criar um indice na tablea AcountBase com os campos itbc_participa_do_programa, itbc_ApuracaoDeBeneficiosECompromissos, StateCode (nessa ordem)
            //OrderExpression ord1 = new OrderExpression("name", OrderType.Ascending);
            //query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContasParticipantesMAtrizEFilial()
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>                                       ");
            strFetchXml.Append("  <entity name='account'>                                                                                                   ");
            strFetchXml.Append("<attribute name='name' />                                           ");
            strFetchXml.Append("<attribute name='primarycontactid' />                               ");
            strFetchXml.Append("<attribute name='telephone1' />                                     ");
            strFetchXml.Append("<attribute name='accountid' />                                      ");
            strFetchXml.Append("<attribute name='itbc_atacadistavarejista' />                       ");
            strFetchXml.Append("<attribute name='aging90_base' />                                   ");
            strFetchXml.Append("<attribute name='aging90' />                                        ");
            strFetchXml.Append("<attribute name='aging60_base' />                                   ");
            strFetchXml.Append("<attribute name='aging60' />                                        ");
            strFetchXml.Append("<attribute name='aging30_base' />                                   ");
            strFetchXml.Append("<attribute name='aging30' />                                        ");
            strFetchXml.Append("<attribute name='itbc_valormediovendasmensais_base' />              ");
            strFetchXml.Append("<attribute name='itbc_valormediovendasmensais' />                   ");
            strFetchXml.Append("<attribute name='itbc_valormediocomprasmensais_base' />             ");
            strFetchXml.Append("<attribute name='itbc_valormediocomprasmensais' />                  ");
            strFetchXml.Append("<attribute name='preferredsystemuserid' />                          ");
            strFetchXml.Append("<attribute name='itbc_usuariointegracao' />                         ");
            strFetchXml.Append("<attribute name='itbc_ultprocesssellout' />                         ");
            strFetchXml.Append("<attribute name='itbc_ult_atualizacao_integracao' />                ");
            strFetchXml.Append("<attribute name='itbc_transportadoraredespacho' />                  ");
            strFetchXml.Append("<attribute name='itbc_transportadora' />                            ");
            strFetchXml.Append("<attribute name='customertypecode' />                               ");
            strFetchXml.Append("<attribute name='address1_addresstypecode' />                       ");
            strFetchXml.Append("<attribute name='businesstypecode' />                               ");
            strFetchXml.Append("<attribute name='itbc_tipodeembalagem' />                           ");
            strFetchXml.Append("<attribute name='itbc_tipodeconstituicao' />                        ");
            strFetchXml.Append("<attribute name='accountcategorycode' />                            ");
            strFetchXml.Append("<attribute name='address1_telephone1' />                            ");
            strFetchXml.Append("<attribute name='telephone3' />                                     ");
            strFetchXml.Append("<attribute name='exchangerate' />                                   ");
            strFetchXml.Append("<attribute name='customersizecode' />                               ");
            strFetchXml.Append("<attribute name='itbc_tabelaprecoastec' />                          ");
            strFetchXml.Append("<attribute name='creditonhold' />                                   ");
            strFetchXml.Append("<attribute name='itbc_subclassificacaoid' />                        ");
            strFetchXml.Append("<attribute name='itbc_statusintegracaosefaz' />                     ");
            strFetchXml.Append("<attribute name='statecode' />                                      ");
            strFetchXml.Append("<attribute name='ftpsiteurl' />                                     ");
            strFetchXml.Append("<attribute name='websiteurl' />                                     ");
            strFetchXml.Append("<attribute name='itbc_softwaredenegocios' />                        ");
            strFetchXml.Append("<attribute name='tickersymbol' />                                   ");
            strFetchXml.Append("<attribute name='industrycode' />                                   ");
            strFetchXml.Append("<attribute name='preferredserviceid' />                             ");
            strFetchXml.Append("<attribute name='itbc_saldodecredito' />                            ");
            strFetchXml.Append("<attribute name='overriddencreatedon' />                            ");
            strFetchXml.Append("<attribute name='itbc_regimeapuracao' />                            ");
            strFetchXml.Append("<attribute name='territoryid' />                                    ");
            strFetchXml.Append("<attribute name='itbc_receitapadraoid' />                           ");
            strFetchXml.Append("<attribute name='revenue_base' />                                   ");
            strFetchXml.Append("<attribute name='revenue' />                                        ");
            strFetchXml.Append("<attribute name='itbc_recebenfe' />                                 ");
            strFetchXml.Append("<attribute name='itbc_recebe_informacao_sci' />                     ");
            strFetchXml.Append("<attribute name='statuscode' />                                     ");
            strFetchXml.Append("<attribute name='itbc_ramaltelefoneprincipal' />                    ");
            strFetchXml.Append("<attribute name='itbc_ramaloutrotelefone' />                        ");
            strFetchXml.Append("<attribute name='itbc_ramal_fax' />                                 ");
            strFetchXml.Append("<attribute name='itbc_quantasfiliais' />                            ");
            strFetchXml.Append("<attribute name='ownerid' />                                        ");
            strFetchXml.Append("<attribute name='ownershipcode' />                                  ");
            strFetchXml.Append("<attribute name='processid' />                                      ");
            strFetchXml.Append("<attribute name='itbc_prazomediovendas' />                          ");
            strFetchXml.Append("<attribute name='itbc_prazomediocompras' />                         ");
            strFetchXml.Append("<attribute name='itbc_possuifiliais' />                             ");
            strFetchXml.Append("<attribute name='itbc_possuiestruturacompleta' />                   ");
            strFetchXml.Append("<attribute name='itbc_portador' />                                  ");
            strFetchXml.Append("<attribute name='itbc_piscofinsporunidade' />                       ");
            strFetchXml.Append("<attribute name='itbc_perfilastec' />                               ");
            strFetchXml.Append("<attribute name='itbc_perfilrevendasdodistribuidor' />              ");
            strFetchXml.Append("<attribute name='itbc_participa_do_programa' />                     ");
            strFetchXml.Append("<attribute name='telephone2' />                                     ");
            strFetchXml.Append("<attribute name='itbc_outrafontereceita' />                         ");
            strFetchXml.Append("<attribute name='itbc_origemconta' />                               ");
            strFetchXml.Append("<attribute name='itbc_optantesuspensaoipi' />                       ");
            strFetchXml.Append("<attribute name='itbc_obsnf' />                                     ");
            strFetchXml.Append("<attribute name='itbc_obspedido' />                                 ");
            strFetchXml.Append("<attribute name='itbc_numeropassaporte' />                          ");
            strFetchXml.Append("<attribute name='itbc_numdevendedores' />                           ");
            strFetchXml.Append("<attribute name='itbc_numtecnicossuporte' />                        ");
            strFetchXml.Append("<attribute name='itbc_numerorevendasinativas' />                    ");
            strFetchXml.Append("<attribute name='itbc_numerorevendasativas' />                      ");
            strFetchXml.Append("<attribute name='itbc_numdecolaboradores' />                        ");
            strFetchXml.Append("<attribute name='numberofemployees' />                              ");
            strFetchXml.Append("<attribute name='itbc_nomefantasia' />                              ");
            strFetchXml.Append("<attribute name='yominame' />                                       ");
            strFetchXml.Append("<attribute name='itbc_nomeabrevmatrizeconomica' />                  ");
            strFetchXml.Append("<attribute name='itbc_nomeabreviado' />                             ");
            strFetchXml.Append("<attribute name='itbc_posvendaid' />                                ");
            strFetchXml.Append("<attribute name='itbc_natureza' />                                  ");
            strFetchXml.Append("<attribute name='donotphone' />                                     ");
            strFetchXml.Append("<attribute name='donotfax' />                                       ");
            strFetchXml.Append("<attribute name='donotbulkemail' />                                 ");
            strFetchXml.Append("<attribute name='donotemail' />                                     ");
            strFetchXml.Append("<attribute name='donotbulkpostalmail' />                            ");
            strFetchXml.Append("<attribute name='donotpostalmail' />                                ");
            strFetchXml.Append("<attribute name='transactioncurrencyid' />                          ");
            strFetchXml.Append("<attribute name='modifiedonbehalfby' />                             ");
            strFetchXml.Append("<attribute name='modifiedby' />                                     ");
            strFetchXml.Append("<attribute name='itbc_modelooperacaofiliais' />                     ");
            strFetchXml.Append("<attribute name='itbc_modalidade' />                                ");
            strFetchXml.Append("<attribute name='shippingmethodcode' />                             ");
            strFetchXml.Append("<attribute name='itbc_metodo_comercializacao_produtos' />           ");
            strFetchXml.Append("<attribute name='itbc_matrizoufilial' />                            ");
            strFetchXml.Append("<attribute name='itbc_localembarque' />                             ");
            strFetchXml.Append("<attribute name='defaultpricelevelid' />                            ");
            strFetchXml.Append("<attribute name='creditlimit_base' />                               ");
            strFetchXml.Append("<attribute name='creditlimit' />                                    ");
            strFetchXml.Append("<attribute name='itbc_intencaoapoio' />                             ");
            strFetchXml.Append("<attribute name='itbc_integradopor' />                              ");
            strFetchXml.Append("<attribute name='preferredequipmentid' />                           ");
            strFetchXml.Append("<attribute name='itbc_inscricaomunicipal' />                        ");
            strFetchXml.Append("<attribute name='itbc_inscricaoestadual' />                         ");
            strFetchXml.Append("<attribute name='itbc_substituicaotributaria' />                    ");
            strFetchXml.Append("<attribute name='itbc_indicada_isec' />                             ");
            strFetchXml.Append("<attribute name='itbc_indicada_inet' />                             ");
            strFetchXml.Append("<attribute name='itbc_indicada_icorp' />                            ");
            strFetchXml.Append("<attribute name='itbc_indicada_icon' />                             ");
            strFetchXml.Append("<attribute name='itbc_incoterm' />                                  ");
            strFetchXml.Append("<attribute name='preferredappointmenttimecode' />                   ");
            strFetchXml.Append("<attribute name='itbc_historico' />                                 ");
            strFetchXml.Append("<attribute name='itbc_guidcrm40' />                                 ");
            strFetchXml.Append("<attribute name='itbc_gera_aviso_credito' />                        ");
            strFetchXml.Append("<attribute name='preferredcontactmethodcode' />                     ");
            strFetchXml.Append("<attribute name='itbc_formadetributacao' />                         ");
            strFetchXml.Append("<attribute name='fax' />                                            ");
            strFetchXml.Append("<attribute name='itbc_exclusividade' />                             ");
            strFetchXml.Append("<attribute name='stageid' />                                        ");
            strFetchXml.Append("<attribute name='itbc_espaco_fisico_qualificado' />                 ");
            strFetchXml.Append("<attribute name='donotsendmm' />                                    ");
            strFetchXml.Append("<attribute name='emailaddress3' />                                  ");
            strFetchXml.Append("<attribute name='emailaddress2' />                                  ");
            strFetchXml.Append("<attribute name='address2_upszone' />                               ");
            strFetchXml.Append("<attribute name='address2_addresstypecode' />                       ");
            strFetchXml.Append("<attribute name='address2_telephone3' />                            ");
            strFetchXml.Append("<attribute name='address2_telephone2' />                            ");
            strFetchXml.Append("<attribute name='address2_telephone1' />                            ");
            strFetchXml.Append("<attribute name='address2_line1' />                                 ");
            strFetchXml.Append("<attribute name='itbc_address2_street' />                           ");
            strFetchXml.Append("<attribute name='address2_country' />                               ");
            strFetchXml.Append("<attribute name='itbc_address2_country' />                          ");
            strFetchXml.Append("<attribute name='itbc_address2_number' />                           ");
            strFetchXml.Append("<attribute name='address2_primarycontactname' />                    ");
            strFetchXml.Append("<attribute name='address2_name' />                                  ");
            strFetchXml.Append("<attribute name='itbc_address2_city' />                             ");
            strFetchXml.Append("<attribute name='address2_shippingmethodcode' />                    ");
            strFetchXml.Append("<attribute name='address2_longitude' />                             ");
            strFetchXml.Append("<attribute name='address2_latitude' />                              ");
            strFetchXml.Append("<attribute name='address2_fax' />                                   ");
            strFetchXml.Append("<attribute name='address2_stateorprovince' />                       ");
            strFetchXml.Append("<attribute name='itbc_address2_stateorprovince' />                  ");
            strFetchXml.Append("<attribute name='address2_freighttermscode' />                      ");
            strFetchXml.Append("<attribute name='address2_line3' />                                 ");
            strFetchXml.Append("<attribute name='address2_utcoffset' />                             ");
            strFetchXml.Append("<attribute name='address2_city' />                                  ");
            strFetchXml.Append("<attribute name='address2_postalcode' />                            ");
            strFetchXml.Append("<attribute name='address2_postofficebox' />                         ");
            strFetchXml.Append("<attribute name='address2_line2' />                                 ");
            strFetchXml.Append("<attribute name='address2_county' />                                ");
            strFetchXml.Append("<attribute name='address2_composite' />                             ");
            strFetchXml.Append("<attribute name='address1_upszone' />                               ");
            strFetchXml.Append("<attribute name='address1_telephone3' />                            ");
            strFetchXml.Append("<attribute name='address1_telephone2' />                            ");
            strFetchXml.Append("<attribute name='address1_line1' />                                 ");
            strFetchXml.Append("<attribute name='itbc_address1_street' />                           ");
            strFetchXml.Append("<attribute name='address1_country' />                               ");
            strFetchXml.Append("<attribute name='itbc_address1_country' />                          ");
            strFetchXml.Append("<attribute name='itbc_address1_number' />                           ");
            strFetchXml.Append("<attribute name='address1_primarycontactname' />                    ");
            strFetchXml.Append("<attribute name='address1_name' />                                  ");
            strFetchXml.Append("<attribute name='itbc_address1_city' />                             ");
            strFetchXml.Append("<attribute name='address1_shippingmethodcode' />                    ");
            strFetchXml.Append("<attribute name='address1_longitude' />                             ");
            strFetchXml.Append("<attribute name='address1_latitude' />                              ");
            strFetchXml.Append("<attribute name='address1_fax' />                                   ");
            strFetchXml.Append("<attribute name='itbc_address1_stateorprovince' />                  ");
            strFetchXml.Append("<attribute name='address1_stateorprovince' />                       ");
            strFetchXml.Append("<attribute name='address1_line3' />                                 ");
            strFetchXml.Append("<attribute name='address1_utcoffset' />                             ");
            strFetchXml.Append("<attribute name='address1_city' />                                  ");
            strFetchXml.Append("<attribute name='address1_postalcode' />                            ");
            strFetchXml.Append("<attribute name='address1_postofficebox' />                         ");
            strFetchXml.Append("<attribute name='address1_county' />                                ");
            strFetchXml.Append("<attribute name='address1_line2' />                                 ");
            strFetchXml.Append("<attribute name='address1_composite' />                             ");
            strFetchXml.Append("<attribute name='itbc_emitebloqueto' />                             ");
            strFetchXml.Append("<attribute name='itbc_emissordocidentidade' />                      ");
            strFetchXml.Append("<attribute name='itbc_embarquevia' />                               ");
            strFetchXml.Append("<attribute name='emailaddress1' />                                  ");
            strFetchXml.Append("<attribute name='itbc_docidentidade' />                             ");
            strFetchXml.Append("<attribute name='itbc_distribuidor_principal' />                    ");
            strFetchXml.Append("<attribute name='itbc_distfontereceita' />                          ");
            strFetchXml.Append("<attribute name='itbc_diasdeatraso' />                              ");
            strFetchXml.Append("<attribute name='preferredappointmentdaycode' />                    ");
            strFetchXml.Append("<attribute name='description' />                                    ");
            strFetchXml.Append("<attribute name='itbc_descontocat' />                               ");
            strFetchXml.Append("<attribute name='itbc_datalimitedecredito' />                       ");
            strFetchXml.Append("<attribute name='itbc_datahoraintegracaosefaz' />                   ");
            strFetchXml.Append("<attribute name='itbc_datadevencimentoconcessao' />                 ");
            strFetchXml.Append("<attribute name='modifiedon' />                                     ");
            strFetchXml.Append("<attribute name='itbc_datadeimplantacao' />                         ");
            strFetchXml.Append("<attribute name='createdon' />                                      ");
            strFetchXml.Append("<attribute name='itbc_datadeconstituio' />                          ");
            strFetchXml.Append("<attribute name='itbc_dataadesao' />                                ");
            strFetchXml.Append("<attribute name='lastusedincampaign' />                             ");
            strFetchXml.Append("<attribute name='itbc_databaixacontribuinte' />                     ");
            strFetchXml.Append("<attribute name='createdonbehalfby' />                              ");
            strFetchXml.Append("<attribute name='createdby' />                                      ");
            strFetchXml.Append("<attribute name='itbc_cpfoucnpj' />                                 ");
            strFetchXml.Append("<attribute name='itbc_contribuinteicms' />                          ");
            strFetchXml.Append("<attribute name='parentaccountid' />                                ");
            strFetchXml.Append("<attribute name='itbc_conta_corrente' />                            ");
            strFetchXml.Append("<attribute name='paymenttermscode' />                               ");
            strFetchXml.Append("<attribute name='address1_freighttermscode' />                      ");
            strFetchXml.Append("<attribute name='itbc_condicao_pagamento' />                        ");
            strFetchXml.Append("<attribute name='itbc_codigosuframa' />                             ");
            strFetchXml.Append("<attribute name='accountnumber' />                                  ");
            strFetchXml.Append("<attribute name='territorycode' />                                  ");
            strFetchXml.Append("<attribute name='itbc_coberturageografica' />                       ");
            strFetchXml.Append("<attribute name='sic' />                                            ");
            strFetchXml.Append("<attribute name='originatingleadid' />                              ");
            strFetchXml.Append("<attribute name='itbc_classificacaoid' />                           ");
            strFetchXml.Append("<attribute name='accountratingcode' />                              ");
            strFetchXml.Append("<attribute name='accountclassificationcode' />                      ");
            strFetchXml.Append("<attribute name='marketcap_base' />                                 ");
            strFetchXml.Append("<attribute name='marketcap' />                                      ");
            strFetchXml.Append("<attribute name='itbc_calcula_multa' />                             ");
            strFetchXml.Append("<attribute name='stockexchange' />                                  ");
            strFetchXml.Append("<attribute name='itbc_banco' />                                     ");
            strFetchXml.Append("<attribute name='itbc_atividadeeconmicaramodeatividade' />          ");
            strFetchXml.Append("<attribute name='itbc_isastec' />                                   ");
            strFetchXml.Append("<attribute name='itbc_apuracaodebeneficiosecompromissos' />         ");
            strFetchXml.Append("<attribute name='itbc_agenteretencao' />                            ");
            strFetchXml.Append("<attribute name='itbc_agencia' />                                   ");
            strFetchXml.Append("<attribute name='sharesoutstanding' />                              ");
            strFetchXml.Append("<attribute name='itbc_acaocrm' />");
            strFetchXml.Append("    <order attribute='name' descending='false' />                                                                           ");
            strFetchXml.Append("    <filter type='and'>                                                                                                     ");
            strFetchXml.Append("      <filter type='and'>                                                                                                   ");
            strFetchXml.Append("        <condition attribute='itbc_participa_do_programa' operator='eq' value='993520001' />                                ");
            strFetchXml.Append("        <condition attribute='itbc_matrizoufilial' operator='eq' value='993520000' />                                       ");
            strFetchXml.Append("        <condition attribute='statecode' operator='eq' value='0' />                                                         ");
            strFetchXml.Append("      </filter>                                                                                                             ");
            strFetchXml.Append("      <filter type='and'>                                                                                                    ");
            strFetchXml.Append("        <condition attribute='accountid' operator='eq' uitype='account' value='{9DD97380-5F2C-E411-9421-00155D013D39}' />   ");
            //strFetchXml.Append("        <condition attribute='accountid' operator='eq' uitype='account' value='{44CA5EE8-A60E-E411-9408-00155D013D38}' />   ");
            //strFetchXml.Append("        <condition attribute='accountid' operator='eq' uitype='account' value='{E333FE1D-A60E-E411-9408-00155D013D38}' />   ");
            //strFetchXml.Append("        <condition attribute='accountid' operator='eq' uitype='account' value='{A08C2C69-D300-E411-9420-00155D013D39}' />   ");
            //strFetchXml.Append("        <condition attribute='accountid' operator='eq' uitype='account' value='{24C7159B-D300-E411-9420-00155D013D39}' />   ");
            //strFetchXml.Append("        <condition attribute='accountid' operator='eq' uitype='account' value='{9DD97380-5F2C-E411-9421-00155D013D39}' />   ");
            strFetchXml.Append("      </filter>                                                                                                             ");
            strFetchXml.Append("    </filter>                                                                                                               ");
            strFetchXml.Append("  </entity>                                                                                                                 ");
            strFetchXml.Append("</fetch>");


            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarContasParticipantes(Guid unidadenegocioId)
        {
            return this.ListarContasParticipantes(unidadenegocioId, null);
        }

        public List<T> ListarContasParticipantes(Guid unidadenegocioId, List<Guid> lstIdCanal)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>");
            strFetchXml.Append("<entity name='account'>");
            strFetchXml.Append("<order attribute='name' descending='false' />");
            strFetchXml.Append("<filter type='and'>");
            strFetchXml.Append("<condition attribute='itbc_participa_do_programa' operator='eq' value='993520001' />");
            strFetchXml.Append("<condition attribute='statecode' operator='eq' value='0' />");
            if (lstIdCanal != null && lstIdCanal.Count > 0)
            {
                strFetchXml.Append("<condition attribute='accountid' operator='not-in'>");
                foreach (Guid id in lstIdCanal)
                {
                    strFetchXml.AppendFormat("<value>{0}</value>", id);
                }
                strFetchXml.Append("</condition>");
            }
            strFetchXml.Append("</filter>");
            if (unidadenegocioId != Guid.Empty)
            {
                strFetchXml.Append("<link-entity name='itbc_categoriasdocanal' from='itbc_canalid' to='accountid' alias='ac'>");
                strFetchXml.Append("<filter type='and'>");
                strFetchXml.Append("<condition attribute='statecode' operator='eq' value='0' />");
                strFetchXml.AppendFormat("<condition attribute='itbc_businessunit' operator='eq' value='{0}' />", unidadenegocioId);
                strFetchXml.Append("</filter>");
                strFetchXml.Append("</link-entity>");
            }
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarMetaDoCanalPor(Guid unidadeNegocioId, int ano, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            query.AddLink("itbc_metadocanal", "accountid", "itbc_canalid");

            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContasOrcamentoCanal(Guid orcamentoId)
        {
            return this.ListarContasOrcamentoCanal(orcamentoId, null, null);
        }

        public List<T> ListarContasOrcamentoCanal(Guid orcamentoId, int? pagina, int? contagem)
        {
            var query = GetQueryExpression<T>(true);

            query.AddLink("itbc_orcamentodocanal", "accountid", "itbc_canalid")
                 .AddLink("itbc_orcamentoportrimestredaunidade", "itbc_orcamentoportrimestredaunidadeid", "itbc_orcamentoportrimestredaunidadeid")
                 .LinkCriteria.AddCondition("new_orcamentoporunidadeid", ConditionOperator.Equal, orcamentoId);

            OrderExpression ord1 = new OrderExpression("name", OrderType.Ascending);
            query.Orders.Add(ord1);

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContasMetaCanal(Guid metaId)
        {
            return this.ListarContasMetaCanal(metaId, null, null);
        }

        public List<T> ListarContasMetaCanal(Guid metaId, int? pagina, int? contagem)
        {
            var query = GetQueryExpression<T>(true);

            query.AddLink("itbc_metadocanal", "accountid", "itbc_canalid")
                 .AddLink("itbc_metaportrimestre", "itbc_metadotrimestreid", "itbc_metaportrimestreid")
                 .LinkCriteria.AddCondition("itbc_metas", ConditionOperator.Equal, metaId);

            OrderExpression ord1 = new OrderExpression("name", OrderType.Ascending);
            query.Orders.Add(ord1);

            if (pagina.HasValue && contagem.HasValue)
            {
                PagingInfo paging = new PagingInfo();
                paging.Count = contagem.Value;
                paging.PageNumber = pagina.Value;
                query.PageInfo = paging;
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarContasMetaCanal(Guid unidadeNegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            query.AddLink("itbc_metadocanal", "accountid", "itbc_canalid");
            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.LinkEntities[0].LinkCriteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public T ObterCanal(string codigoemitente)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("accountnumber", ConditionOperator.Equal, codigoemitente);

            OrderExpression ord1 = new OrderExpression("accountnumber", OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public DomainCollection<T> ListarPor(Domain.Model.Classificacao classificacao, Domain.Model.Subclassificacoes subClassificacoes, int count = 100, string pagingCookie = "", int pageNumber = 1)
        {
            var query = GetQueryExpression<T>(true);
            query.PageInfo = new PagingInfo()
            {
                Count = count,
                PagingCookie = pagingCookie,
                PageNumber = pageNumber
            };

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Conta.StateCode.Ativo);
            query.Criteria.AddCondition("itbc_classificacaoid", ConditionOperator.Equal, classificacao.ID.Value);
            query.Criteria.AddCondition("itbc_subclassificacaoid", ConditionOperator.Equal, subClassificacoes.ID.Value);

            OrderExpression ord1 = new OrderExpression("accountnumber", OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = RetrieveMultiple(query);

            return colecao;
        }

        public List<T> ListarParticipantesDoProgramaApenasComApuracaoBeneficio(Guid? unidadeNegocioId, params string[] columns)
        {
            var query = GetQueryExpression<T>(true);

            if (columns != null && columns.Length > 0)
            {
                query.ColumnSet.AddColumns(columns);
            }

            var QEaccount_Criteria_0 = new FilterExpression(LogicalOperator.Or);
            query.Criteria.AddFilter(QEaccount_Criteria_0);

            QEaccount_Criteria_0.FilterOperator = LogicalOperator.Or;
            var QEaccount_Criteria_0_0 = new FilterExpression(LogicalOperator.And);
            QEaccount_Criteria_0.AddFilter(QEaccount_Criteria_0_0);

            QEaccount_Criteria_0_0.AddCondition("itbc_apuracaodebeneficiosecompromissos", ConditionOperator.Equal, (int)Domain.Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais);
            var QEaccount_Criteria_0_1 = new FilterExpression(LogicalOperator.And);
            QEaccount_Criteria_0.AddFilter(QEaccount_Criteria_0_1);

            QEaccount_Criteria_0_1.AddCondition("itbc_apuracaodebeneficiosecompromissos", ConditionOperator.Equal, (int)Domain.Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz);
            QEaccount_Criteria_0_1.AddCondition("itbc_matrizoufilial", ConditionOperator.Equal, (int)Domain.Enum.Conta.MatrizOuFilial.Matriz);
            var QEaccount_Criteria_1 = new FilterExpression(LogicalOperator.And);
            query.Criteria.AddFilter(QEaccount_Criteria_1);

            QEaccount_Criteria_1.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            if (unidadeNegocioId.HasValue)
            {
                var QEaccount_itbc_categoriasdocanal = query.AddLink("itbc_categoriasdocanal", "accountid", "itbc_canalid");
                QEaccount_itbc_categoriasdocanal.LinkCriteria.AddCondition("statecode", ConditionOperator.Equal, 0);
                QEaccount_itbc_categoriasdocanal.LinkCriteria.AddCondition("itbc_businessunit", ConditionOperator.Equal, unidadeNegocioId.Value);
            }

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        //CRM4
        internal Domain.Model.CondicaoPagamento PesquisarCondicaoDePagamentoPor(string codigoCondicaoPagamento)
        {
            Domain.Model.CondicaoPagamento condicaoPagamento = null;
            if (!string.IsNullOrEmpty(codigoCondicaoPagamento))
            {
                var queryHelper = new QueryExpression("new_condicao_pagamento");
                queryHelper.ColumnSet.AllColumns = true;
                queryHelper.TopCount = 1;
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_codicao_pagamento", ConditionOperator.Equal, codigoCondicaoPagamento));
                var bec = base.Provider.RetrieveMultiple(queryHelper);
                if (bec.Entities.Count > 0)
                {
                    Entity cond = bec.Entities[0];
                    condicaoPagamento = new Domain.Model.CondicaoPagamento(this.OrganizationName, this.IsOffline);
                    condicaoPagamento.Id = new Guid(Convert.ToString(cond["new_condicao_pagamentoid"]));
                    condicaoPagamento.Codigo = Convert.ToInt32(cond["new_codicao_pagamento"]);
                    condicaoPagamento.Nome = Convert.ToString(cond["new_name"]);
                    if (cond.Attributes.Contains("new_suppliercard"))
                        condicaoPagamento.SupplierCard = Convert.ToBoolean(cond["new_suppliercard"]);
                }
            }
            return condicaoPagamento;
        }

        private Representante PesquisarRepresentantePor(Buscar_DadosGrCliente_ttRetornoGrupoRow row)
        {
            Representante representante = null;

            var queryHelper = new QueryExpression("new_representante");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_codigo_representante", ConditionOperator.Equal, row.codrep.Value));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {


            }
            //    representante = ClienteFactory.InstanciarRepresentante((new_representante)bec.Entities[0], base.OrganizacaoCorrente);

            return representante;
        }

        private T PesquisarPor(string numeroDoDocumento, string nomeDoCampo)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression(nomeDoCampo, ConditionOperator.Equal, numeroDoDocumento));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];

            //var bec = base.Provider.RetrieveMultiple(queryHelper);

            //if (bec.Entities.Count > 0)
            //{
            //    cliente = ClienteFactory.InstanciarCliente((account)bec.Entities[0], base.OrganizacaoCorrente);
            //    cliente.JaEstaNoCrm = true;
            //}
            //else
            //{
            //    // Realizando a pesquisa no EMS.
            //    cliente = this.PesquisarPorDocumento(numeroDoDocumento);
            //}
            //return cliente;
        }

        public T ObterPor(Domain.Model.Contato contato)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("contact", "accountid", "parentcustomerid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.Equal, contato.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPorLogin(string login)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("contact", "accountid", "parentcustomerid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_login", ConditionOperator.Equal, login));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ObterClientesPorId(string[] clientesId)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            query.Criteria.Conditions.Add(new ConditionExpression("accountid", ConditionOperator.In, clientesId));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        private Domain.Model.Conta PesquisarPorDocumento(string numeroDoDocumento)
        {
            Domain.Model.Conta cliente = null;
            var row = new Buscar_dadosCliente_ttClienteRow[1];
            if (!string.IsNullOrEmpty(numeroDoDocumento))
            {
                numeroDoDocumento = numeroDoDocumento.Replace(".", "").Replace("-", "").Replace("/", "");
                Domain.Servicos.HelperWS.IntelbrasService.Buscar_dadosCliente(numeroDoDocumento, out row);
                if (null != row)
                {
                    if (row.Length > 0)
                    {
                        //CARLOS FAZER INSTANCIAR

                        //if (row[0].codemitente > 0)
                        //    cliente = ClienteFactory.InstanciarCliente(row[0], base.OrganizacaoCorrente);
                        //else if (row[0].nomematriz != null)
                        //    cliente = ClienteFactory.InstanciarCliente(row[0], base.OrganizacaoCorrente);

                    }
                }
            }

            return cliente;
        }

        public Boolean CidadeZonaFranca(string cidade, string uf)
        {
            Boolean resultado = false;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarCidadeZF(cidade, uf, out resultado);

            return resultado;
        }

        public Telefone PesquisarTelefonePor(Domain.Model.Conta cliente)
        {
            throw new NotImplementedException();
        }

        public Representante PesquisarRepresentantePor(int codGrupo)
        {
            Representante representante = null;
            Buscar_DadosGrCliente_ttRetornoGrupoRow[] row = null;
            Domain.Servicos.HelperWS.IntelbrasService.Buscar_DadosGrCliente(codGrupo, out row);
            if (row != null)
            {
                if (row.Length > 0)
                {
                    if (row[0].codrep.HasValue)
                        representante = this.PesquisarRepresentantePor(row[0]);
                }
            }

            return representante;
        }

        public T PesquisarPor(Documento documento)
        {
            return this.PesquisarPor(documento.Numero, "itbc_cpfoucnpj");
        }

        public T ObterAutorizadaPor(Guid ocorrenciaId)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("incident", "accountid", "new_autorizadaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ocorrenciaId));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("incident", "accountid", "customerid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ocorrencia.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Domain.Model.Fatura fatura)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("invoice", "accountid", "customerid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("invoiceid", ConditionOperator.Equal, fatura.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Extrato extrato)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_extrato_pagamento_ocorrencia", "accountid", "new_posto_servicoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_extrato_pagamento_ocorrenciaid", ConditionOperator.Equal, extrato.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPor(Ocorrencia ocorrencia, string campoOrigem)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("incident", "accountid", campoOrigem);
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ocorrencia.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> PesquisarClientes(string nome)
        {
            var query = GetQueryExpression<T>(true);
            if (nome != string.Empty && nome != "")
                query.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, "%" + nome + "%"));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        private T PesquisarCliente(string valorpesquisa, string nomeDoCampo)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression(nomeDoCampo, ConditionOperator.Equal, valorpesquisa));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public void CriarAnotacao(Guid Id, string entidade, Anotacao anexo)
        {
            //var be = new annotation();
            //be.notetext = "Anexo";
            //be.subject = "Anexo";

            //be.objectid = new Lookup();
            //be.objectid.type = entidade;
            //be.objectid.Value = Id;

            //be.objecttypecode = new EntityNameReference();
            //be.objecttypecode.Value = entidade;

            //var noteId = base.Provider.Create(be);

            //string encodedData = System.Convert.ToBase64String(anexo.BytesDoArquivo);

            //be = new annotation();
            //be.annotationid = new Key();
            //be.annotationid.Value = noteId;
            //be.documentbody = encodedData;
            //be.filename = anexo.NomeDoArquivo;
            //be.mimetype = @"application\ms-word";
            //be.isdocument = new CrmBoolean();
            //be.isdocument.Value = true;

            //base.Provider.Update(be);
        }

        public T PesquisarPor(string nomecliente)
        {
            return this.PesquisarCliente(nomecliente, "name");
        }

        public T PesquisarPor(int codigo)
        {
            return this.PesquisarCliente(codigo.ToString(), "accountnumber");
        }

        public List<T> PesquisarPor(string valorDoCampo, string nomeDoCampo, string condicao, int NumeroDeRegistros)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.TopCount = NumeroDeRegistros;
            queryHelper.Distinct = true;
            ConditionOperator condition = new ConditionOperator();
            switch (condicao)
            {
                case "Equal": condition = ConditionOperator.Equal; break;
                case "Like": condition = ConditionOperator.Like; break;
            }
            queryHelper.Criteria.Conditions.Add(new ConditionExpression(nomeDoCampo, condition, valorDoCampo));
            queryHelper.Distinct = true;
            queryHelper.Orders.Add(new OrderExpression(nomeDoCampo, OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<Domain.Model.Conta> PesquisarPorCampos(string codigo, string cnpj, string nome, string nomeAbreviado, string UF, Guid contatoId)
        {
            if (contatoId == Guid.Empty) return default(List<Domain.Model.Conta>);

            List<Domain.Model.Conta> cliente = new List<Domain.Model.Conta>();

            var queryHelper = new QueryExpression("account");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Distinct = true;
            if (codigo != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("accountnumber", ConditionOperator.Equal, codigo));
            if (cnpj != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cnpj.InputMask()));
            if (nome != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, string.Format("%{0}%", nome)));
            if (nomeAbreviado != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_nomeabreviado", ConditionOperator.Like, string.Format("%{0}%", nomeAbreviado)));
            if (UF != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("address1_stateorprovince", ConditionOperator.Equal, UF));

            //queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_contribuinteicms", ConditionOperator.Equal, true)); Fernando retirado chamado 103410
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));

            //Join com Relacionamentodob2b
            LinkEntity link1 = queryHelper.AddLink("itbc_relacionamentodob2b", "accountid", "itbc_codigocliente", JoinOperator.Inner);
            link1.EntityAlias = "rel";
            link1.Columns.AddColumn("itbc_codigo_representante");
            link1.Columns.AddColumn("itbc_codigounidadecomercial");

            FilterExpression filtroRelacionamento = new FilterExpression();
            filtroRelacionamento.Conditions.Add(new ConditionExpression("rel", "itbc_datainicial", ConditionOperator.LessEqual, DateTime.Now.Date));

            FilterExpression filtroRelacionamentoOr = new FilterExpression();
            filtroRelacionamentoOr.FilterOperator = LogicalOperator.Or;
            filtroRelacionamentoOr.Conditions.Add(new ConditionExpression("rel", "itbc_datafinal", ConditionOperator.GreaterEqual, DateTime.Now.Date));
            filtroRelacionamentoOr.Conditions.Add(new ConditionExpression("rel", "itbc_datafinal", ConditionOperator.Null));

            queryHelper.Criteria.Filters.Add(filtroRelacionamento);
            queryHelper.Criteria.Filters.Add(filtroRelacionamentoOr);

            queryHelper.Orders.Add(new OrderExpression("name", OrderType.Ascending));


            var bec = base.Provider.RetrieveMultiple(queryHelper);
            var incluiRegistro = false;

            Domain.Model.Contato contato = new Domain.Model.Contato(this.OrganizationName, this.IsOffline);
            if (contatoId != Guid.Empty)
                contato = (new Domain.Servicos.RepositoryService()).Contato.Retrieve(contatoId);

            if (bec.Entities.Count > 0)
            {
                foreach (Entity item in bec.Entities)
                {

                    //verifica as permissões do contato com o cliente
                    incluiRegistro = false;
                    if (contatoId == Guid.Empty)
                    {
                        incluiRegistro = true;
                    }
                    else
                    {
                        try
                        {
                            if (item.Attributes.Contains("rel.itbc_codigo_representante") || item.Attributes.Contains("rel.itbc_codigounidadecomercial"))
                            {
                                var qH1 = new QueryExpression("new_permissao_usuario_b2b");
                                qH1.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
                                qH1.ColumnSet.AllColumns = true;

                                var b2 = base.Provider.RetrieveMultiple(qH1);
                                if (b2.Entities.Count > 0)
                                {
                                    foreach (Entity itemB2B in b2.Entities)
                                    {
                                        if (!itemB2B.Attributes.Contains("itbc_codigo_representante") && !itemB2B.Attributes.Contains("itemB2B.new_unidade_negocioid"))
                                        {
                                            //não tem acesso
                                        }
                                        else
                                        {
                                            if (itemB2B.Attributes.Contains("itbc_codigo_representante") && !itemB2B.Attributes.Contains("new_unidade_negocioid"))
                                            {
                                                if (Convert.ToInt32(itemB2B["itbc_codigo_representante"]) == Convert.ToInt32(((AliasedValue)item.Attributes["rel.itbc_codigo_representante"]).Value))
                                                    incluiRegistro = true;
                                            }
                                            else if (itemB2B.Attributes.Contains("new_unidade_negocioid") && !itemB2B.Attributes.Contains("itbc_codigo_representante"))
                                            {
                                                if (((EntityReference)itemB2B["new_unidade_negocioid"]).Id == ((EntityReference)item["rel.itbc_codigounidadecomercial"]).Id)
                                                    if (contato.AssociadoA == null)
                                                        incluiRegistro = true;
                                                    else if (contato.AssociadoA.Id == Guid.Empty)
                                                        incluiRegistro = true;
                                            }
                                            else if (itemB2B.Attributes.Contains("itemB2B.new_unidade_negocioid") && itemB2B.Attributes.Contains("itemB2B.itbc_codigo_representante"))
                                            {
                                                if (((EntityReference)itemB2B["itemB2B.new_unidade_negocioid"]).Id == ((EntityReference)item["rel.itbc_codigounidadecomercial"]).Id
                                                    && (Convert.ToInt32(itemB2B["itemB2B.itbc_codigo_representante"]) == Convert.ToInt32(((AliasedValue)item.Attributes["rel.itbc_codigo_representante"]).Value)))
                                                    incluiRegistro = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            EventLog.WriteEntry("CRM B2B - Pesquisa Cliente", err.Message);
                        }
                        if (contato.AssociadoA != null)
                            if (((Guid)item["accountid"]) == contato.AssociadoA.Id)
                                incluiRegistro = true;
                    }

                    // Linq
                    var matchingvalue = cliente.FirstOrDefault(x => x.ID == item.Id);
                    if (incluiRegistro && matchingvalue == null)
                    {
                        cliente.Add((new Domain.Servicos.RepositoryService()).Conta.Retrieve(item.Id));
                    }
                }
            }
            return cliente;
        }

        public List<T> ListarPorAtributos(Domain.Model.Conta cliente)
        {
            var queryHelper = GetQueryExpression<T>(true);
            if (!string.IsNullOrEmpty(cliente.CpfCnpj)) queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cliente.CpfCnpj.InputMask()));
            if (!string.IsNullOrEmpty(cliente.Nome)) queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, string.Format("%{0}%", cliente.Nome)));
            if (!string.IsNullOrEmpty(cliente.NomeAbreviado)) queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_nomeabreviado", ConditionOperator.Like, string.Format("%{0}%", cliente.NomeAbreviado)));
            if (!string.IsNullOrEmpty(cliente.CodigoMatriz)) queryHelper.Criteria.Conditions.Add(new ConditionExpression("accountnumber", ConditionOperator.Equal, cliente.CodigoMatriz));
            if (!string.IsNullOrEmpty(cliente.Endereco1Estado)) queryHelper.Criteria.Conditions.Add(new ConditionExpression("address1_stateorprovince", ConditionOperator.Equal, cliente.Endereco1Estado));
            if (cliente.Natureza != (int)NaturezaDoCliente.Vazio) queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_natureza", ConditionOperator.Equal, (int)cliente.Natureza));
            queryHelper.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<T> PesquisarPorCampos(string codigo, string cnpj, string nome, string nomeAbreviado, string UF, Guid contatoId, NaturezaDoCliente natureza)
        {

            List<T> cliente = new List<T>();

            var queryHelper = new QueryExpression("account");
            if (codigo != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("accountnumber", ConditionOperator.Equal, codigo));
            if (cnpj != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cnpj.InputMask()));
            if (nome != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Like, string.Format("%{0}%", nome)));
            if (nomeAbreviado != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_nomeabreviado", ConditionOperator.Like, string.Format("%{0}%", nomeAbreviado)));
            if (UF != "")
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("address1_stateorprovince", ConditionOperator.Equal, UF));
            if (natureza != NaturezaDoCliente.Vazio)
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_natureza", ConditionOperator.Equal, (int)natureza));

            if (contatoId != Guid.Empty)
            {
                //Retirado por Carlos Roweder Nass em 22/06/2011
                //Discussão de Luciano Cestari e Jackson Luis sobre a disponibilidade de compra pelo B2B pelos cliente que não possuem inscrição estadual
                //queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contribuinte_icms", ConditionOperator.Equal, true);
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            }

            queryHelper.Orders.Add(new OrderExpression("name", OrderType.Ascending));

            var bec = base.Provider.RetrieveMultiple(queryHelper);
            var incluiRegistro = false;

            Domain.Model.Contato contato = new Domain.Model.Contato(this.OrganizationName, this.IsOffline);
            if (contatoId != Guid.Empty)
                contato = (new Domain.Servicos.RepositoryService()).Contato.Retrieve(contatoId);

            if (bec.Entities.Count > 0)
            {
                foreach (Entity item in bec.Entities)
                {
                    //verifica as permissões do contato com o cliente
                    incluiRegistro = false;
                    if (contatoId == Guid.Empty)
                    {
                        incluiRegistro = true;
                    }
                    else
                    {
                        var qH = new QueryExpression("itbc_relacionamentodob2b");
                        qH.ColumnSet.AllColumns = true;
                        qH.Criteria.Conditions.Add(new ConditionExpression("itbc_codigocliente", ConditionOperator.Equal, item.Id));
                        var b1 = base.Provider.RetrieveMultiple(qH);
                        if (b1.Entities.Count > 0)
                        {
                            foreach (Entity itemR in b1.Entities)
                            {
                                try
                                {
                                    if (itemR.Attributes.Contains("itbc_codigo_representante") || itemR.Attributes.Contains("itbc_codigounidadecomercial"))
                                    {
                                        var qH1 = new QueryExpression("new_permissao_usuario_b2b");
                                        //if (contatoId != Guid.Empty)
                                        qH1.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contatoId));
                                        qH1.ColumnSet.AllColumns = true;

                                        //qH1.Criteria.Conditions.Add(new ConditionExpression("new_representanteid", ConditionOperator.Equal, itemR.new_representanteid.Value);
                                        //qH1.Criteria.Conditions.Add(new ConditionExpression("new_unidade_negocioid", ConditionOperator.Equal, itemR.new_unidadedenegociosid.Value);
                                        var b2 = base.Provider.RetrieveMultiple(qH1);
                                        if (b2.Entities.Count > 0)
                                        {
                                            foreach (Entity itemB2B in b2.Entities)
                                            {
                                                if (!itemB2B.Attributes.Contains("itbc_codigo_representante") && !itemB2B.Attributes.Contains("itemB2B.new_unidade_negocioid"))
                                                {
                                                    //não tem acesso
                                                }
                                                else
                                                {
                                                    if (itemB2B.Attributes.Contains("itbc_codigo_representante") && !itemB2B.Attributes.Contains("new_unidade_negocioid"))
                                                    {
                                                        if (Convert.ToInt32(itemB2B["itbc_codigo_representante"]) == Convert.ToInt32(itemR["itbc_codigo_representante"]))
                                                            incluiRegistro = true;
                                                    }
                                                    else if (itemB2B.Attributes.Contains("new_unidade_negocioid") && !itemB2B.Attributes.Contains("itbc_codigo_representante"))
                                                    {
                                                        if (((EntityReference)itemB2B["new_unidade_negocioid"]).Id == ((EntityReference)itemR["itbc_codigounidadecomercial"]).Id)
                                                            if (contato.AssociadoA == null)
                                                                incluiRegistro = true;
                                                            else if (contato.AssociadoA.Id == Guid.Empty)
                                                                incluiRegistro = true;
                                                    }
                                                    else if (itemB2B.Attributes.Contains("new_unidade_negocioid") && itemB2B.Attributes.Contains("itbc_codigo_representante"))
                                                    {
                                                        if (((EntityReference)itemB2B["new_unidade_negocioid"]).Id == ((EntityReference)itemR["itbc_codigounidadecomercial"]).Id
                                                            && (Convert.ToInt32(itemB2B["itbc_codigo_representante"]) == Convert.ToInt32(itemR["itbc_codigo_representante"])))
                                                            incluiRegistro = true;
                                                    }
                                                }
                                            }

                                        }
                                    }

                                }
                                catch (Exception err)
                                {
                                    EventLog.WriteEntry("CRM B2B - Pesquisa Cliente", err.Message);
                                }
                            }

                            if (contato.AssociadoA != null)
                                if (((Guid)item["accountid"]) == contato.AssociadoA.Id)
                                    incluiRegistro = true;

                        }
                    }

                    if (incluiRegistro)
                        cliente.Add(Retrieve(item.Id));
                }
            }
            return cliente;
        }

        public List<CategoriaUN> ListarCategoriaUNB2BPor(Domain.Model.Conta cliente, Guid contato)
        {
            List<CategoriaUN> categoriaUN = new List<CategoriaUN>();
            var queryHelper = new QueryExpression("new_permissao_usuario_b2b");
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contato));
            queryHelper.ColumnSet.AllColumns = true;
            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                foreach (Entity item in bec.Entities)
                {
                    Categoria categoria = null;
                    UnidadeNegocio unidadeDeNegocio = null;
                    Domain.Model.Contato representante = null;

                    if (item.Attributes.Contains("new_unidade_negocioid"))
                    {
                        unidadeDeNegocio = new UnidadeNegocio(this.OrganizationName, this.IsOffline);
                        unidadeDeNegocio.Nome = ((EntityReference)item["new_unidade_negocioid"]).Name;
                        unidadeDeNegocio.Id = ((EntityReference)item["new_unidade_negocioid"]).Id;
                        unidadeDeNegocio.CodigoEms = ((EntityReference)item["new_unidade_negocioid"]).Name;
                    }

                    if (item.Attributes.Contains("itbc_codigo_representante"))
                    {
                        var lista = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ListarPorCodigoRepresentante(item["itbc_codigo_representante"].ToString());
                        if (lista != null && lista.Count > 0)
                            representante = lista[0];
                    }
                    CategoriaUN cat = new CategoriaUN(this.OrganizationName, this.IsOffline);
                    cat.Nome = Convert.ToString(item["new_name"]);
                    cat.Cliente = cliente;
                    cat.UnidadeDeNegocio = unidadeDeNegocio;
                    cat.Categoria = categoria;
                    cat.Representante = representante;
                    categoriaUN.Add(cat);
                }
            }
            return categoriaUN;
        }

        public decimal BuscarDescontoMangaPor(string codigoRepresentante, string codigoCliente, string codigoUnidadeDeNegocio, string codigoCategoria, string codigoFamiliaComercial)
        {
            decimal desconto = 0;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarDescManga(int.Parse(codigoRepresentante), int.Parse(codigoCliente), codigoUnidadeDeNegocio, int.Parse(codigoCategoria), codigoFamiliaComercial, out desconto);

            return desconto;
        }

        public List<decimal> BuscarDescontoPor(int codigoCliente, string codigoUnidadeDeNegocio, int codigoCategoria, string codigoFamiliaComercial, int qtdeDeProdutos)
        {
            List<decimal> descontos = new List<decimal>();
            decimal desconto1 = 0;
            decimal desconto2 = 0;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarDesconto(codigoCliente, codigoUnidadeDeNegocio, codigoCategoria, codigoFamiliaComercial, qtdeDeProdutos, out desconto1);
            descontos.Add(desconto1);
            descontos.Add(desconto2);
            return descontos;
        }

        public List<Representante> ListarRepresentantesB2BPor(Domain.Model.Conta cliente)
        {
            List<Representante> representantes = new List<Representante>();
            var queryHelper = new QueryExpression("itbc_relacionamentodob2b");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_codigocliente", ConditionOperator.Equal, cliente.Id));
            var bec = base.Provider.RetrieveMultiple(queryHelper);
            foreach (Entity item in bec.Entities)
            {
                var lista = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ListarPorCodigoRepresentante(item["itbc_codigo_representante"].ToString());
                if (lista != null && lista.Count > 0)
                {
                    Representante rep = new Representante(this.OrganizationName, this.IsOffline);
                    rep.CodigoRepresentante = Convert.ToInt32(item["itbc_codigodorepresentante"]);
                    rep.Nome = lista[0].NomeCompleto;
                    rep.Id = lista[0].Id;
                    representantes.Add(rep);
                }
            }
            return representantes;
        }

        public List<Categoria> ListarCategoriasB2BPor(Domain.Model.Conta cliente)
        {
            List<Categoria> categorias = new List<Categoria>();
            var queryHelper = new QueryExpression("itbc_categoriadob2b");
            queryHelper.ColumnSet.AllColumns = true;
            if (cliente != null)
            {
                queryHelper.AddLink("itbc_relacionamentodob2b", "itbc_categoriadob2bid", "itbc_codigocategoriab2b");
                queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_codigocliente", ConditionOperator.Equal, cliente.Id));
            }
            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                foreach (Entity item in bec.Entities)
                {
                    Categoria categoria = new Categoria(this.OrganizationName, this.IsOffline);
                    categoria.Id = item.Id;
                    categoria.CodigoCategoria = Convert.ToString(item["itbc_codigocategoriab2b"]);
                    categoria.Nome = Convert.ToString(item["itbc_name"]);
                    categorias.Add(categoria);
                }
            }

            return categorias;
        }

        public Categoria ObterCategoriaDoClientePor(Guid id, string codigo)
        {
            Categoria categoria = null;

            var queryHelper = new QueryExpression("itbc_categoriadob2b");
            queryHelper.ColumnSet.AllColumns = true;
            if (id != Guid.Empty)
            {
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_categoriadob2bid", ConditionOperator.Equal, id));
            }

            if (!string.IsNullOrEmpty(codigo))
            {
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_codigocategoriab2b", ConditionOperator.Equal, codigo));
            }

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                categoria = new Categoria(this.OrganizationName, this.IsOffline);
                Entity item = bec.Entities[0];
                categoria.Id = item.Id;
                categoria.CodigoCategoria = Convert.ToString(item["itbc_codigocategoriab2b"]);
                categoria.Nome = Convert.ToString(item["itbc_name"]);
            }

            return categoria;
        }

        public Representante ObterRepresentantePor(Guid id, string codigo)
        {
            Representante representante = new Representante(this.OrganizationName, this.IsOffline);
            var queryHelper = new QueryExpression("contact");
            queryHelper.ColumnSet.AllColumns = true;
            if (id != Guid.Empty)
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.Equal, id));
            if (!string.IsNullOrEmpty(codigo))
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_codigodorepresentante", ConditionOperator.Equal, codigo));
            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                Entity cat = bec.Entities[0];
                if (cat.Attributes.Contains("contactid"))
                    representante.Id = cat.Id;
                if (cat.Attributes.Contains("itbc_codigodorepresentante"))
                    representante.CodigoRepresentante = Convert.ToInt32(cat["itbc_codigodorepresentante"]);
                representante.Nome = Convert.ToString(cat["fullname"]);
                representante.DescricaoRepresentante = Convert.ToString(cat["fullname"]);
            }

            return representante;
        }

        public Categoria PesquisarCategoriaCliente(int codigo, UnidadeNegocio unidadeNegocio)
        {
            Categoria categoria = null;

            QueryExpression query = new QueryExpression();

            query.EntityName = "itbc_categoriadob2b";
            query.ColumnSet.AllColumns = true;
            LinkEntity link1 = query.AddLink("itbc_relacionamentodob2b", "itbc_categoriadob2bid", "itbc_codigocategoriab2b", JoinOperator.Natural);
            link1.EntityAlias = "rel";
            LinkEntity link2 = link1.AddLink("account", "itbc_codigocliente", "accountid", JoinOperator.Natural);
            link2.EntityAlias = "acc";

            query.Criteria = new FilterExpression();
            query.Criteria.AddCondition("rel", "itbc_codigounidadecomercial", ConditionOperator.Equal, unidadeNegocio.Id);
            query.Criteria.AddCondition("acc", "accountnumber", ConditionOperator.Equal, codigo.ToString());

            var bec = base.Provider.RetrieveMultiple(query);

            if (bec.Entities.Count > 0)
            {
                categoria = new Categoria();
                Entity cat = bec.Entities[0];
                categoria.Nome = Convert.ToString(cat["itbc_name"]);
                categoria.CodigoCategoria = Convert.ToString(cat["itbc_codigocategoriab2b"]);
                categoria.Id = new Guid(Convert.ToString(cat["itbc_categoriadob2bid"]));
            }

            return categoria;
        }

        public List<T> ListarPor(Contrato contrato)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.AddLink("new_cliente_participante_contrato", "accountid", "new_clienteid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<T> ListarPor(Contrato contrato, int pagina, int quantidadePorPagina, ref bool existemMaisRegistros, ref string cacheDaPagina)
        {
            List<Domain.Model.Conta> lista = new List<Domain.Model.Conta>();
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.AddLink("new_cliente_participante_contrato", "accountid", "new_clienteid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            queryHelper.PageInfo = new PagingInfo() { Count = quantidadePorPagina, PageNumber = pagina, PagingCookie = string.IsNullOrEmpty(cacheDaPagina) ? null : cacheDaPagina };

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            existemMaisRegistros = bec.MoreRecords;
            cacheDaPagina = bec.PagingCookie;

            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<T> ListarPor(Usuario criadoPor, DateTime dataCriacaoInicio, DateTime dataCriacaoFim)
        {
            List<Domain.Model.Conta> lista = new List<Domain.Model.Conta>();
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("createdby", ConditionOperator.Equal, criadoPor.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("createdon", ConditionOperator.GreaterEqual, dataCriacaoInicio.ToString("yyyy-MM-ddTHH:mm:ss")));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("createdon", ConditionOperator.LessEqual, dataCriacaoFim.ToString("yyyy-MM-ddTHH:mm:ss")));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public T ObterPorEmail(string email)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("emailaddress1", ConditionOperator.Equal, email));
            var colecao = this.RetrieveMultiple(queryHelper);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public Dictionary<Guid, string> ListarDistribuidorInfo(Guid accountId)
        {
            Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
            QueryExpression queryHelper = new QueryExpression("customerrelationship");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Distinct = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, accountId));
            queryHelper.AddLink("relationshiprole", "partnerroleid", "relationshiproleid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, "Distribuidor"));
            EntityCollection collection = base.Provider.RetrieveMultiple(queryHelper);
            foreach (Entity item in collection.Entities)
                dictionary.Add(((EntityReference)item["partnerid"]).Id, ((EntityReference)item["partnerid"]).Name);
            return dictionary;
        }

        public List<T> ListarDistribuidorPor(Guid contatoId)
        {
            Guid funcaoRevendaId = (new Domain.Servicos.RepositoryService()).FuncaoRelacionamento.ObterFuncaoPor("Revenda");
            Guid funcaoDistribuidorId = (new Domain.Servicos.RepositoryService()).FuncaoRelacionamento.ObterFuncaoPor("Distribuidor");

            List<T> distribuidores = new List<T>();

            QueryExpression queryFuncao1 = new QueryExpression("account");
            queryFuncao1.Distinct = true;
            queryFuncao1.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("partnerroleid", ConditionOperator.Equal, funcaoRevendaId));
            queryFuncao1.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("customerroleid", ConditionOperator.Equal, funcaoDistribuidorId));
            queryFuncao1.LinkEntities[0].AddLink("account", "partnerid", "accountid");
            queryFuncao1.LinkEntities[0].LinkEntities[0].AddLink("contact", "accountid", "parentcustomerid");
            queryFuncao1.LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.Equal, contatoId));

            EntityCollection colecaoFuncao1 = base.Provider.RetrieveMultiple(queryFuncao1);
            foreach (Entity item in colecaoFuncao1.Entities)
                distribuidores.Add(Retrieve(item.Id));

            QueryExpression queryFuncao2 = new QueryExpression("account");
            queryFuncao2.Distinct = true;
            queryFuncao1.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("partnerroleid", ConditionOperator.Equal, funcaoDistribuidorId));
            queryFuncao2.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("customerroleid", ConditionOperator.Equal, funcaoRevendaId));
            queryFuncao2.LinkEntities[0].AddLink("account", "customerid", "accountid");
            queryFuncao2.LinkEntities[0].LinkEntities[0].AddLink("contact", "accountid", "parentcustomerid");
            queryFuncao2.LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("contactid", ConditionOperator.Equal, contatoId));

            EntityCollection colecaoFuncao2 = base.Provider.RetrieveMultiple(queryFuncao2);
            foreach (Entity item in colecaoFuncao2.Entities)
            {
                bool adicionar = true;

                foreach (var itemAdicionado in colecaoFuncao1.Entities)
                    if (itemAdicionado.Id == item.Id)
                    {
                        adicionar = false;
                        break;
                    }

                if (adicionar)
                    distribuidores.Add(Retrieve(item.Id));
            }

            return distribuidores;
        }

        public T ObterPor(Diagnostico diagnostico)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.TopCount = 1;
            queryHelper.AddLink("incident", "accountid", "new_autorizadaid");
            queryHelper.LinkEntities[0].AddLink("new_diagnostico_ocorrencia", "incidentid", "new_ocorrenciaid");
            queryHelper.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.Equal, diagnostico.Id));
            var colecao = this.RetrieveMultiple(queryHelper);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public bool ExisteDuplicidade(Domain.Model.Conta cliente)
        {
            var queryHelper = GetQueryExpression<T>(true);
            if (!string.IsNullOrEmpty(cliente.CodigoMatriz))
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("accountnumber", ConditionOperator.Equal, cliente.CodigoMatriz));
            if (!string.IsNullOrEmpty(cliente.CpfCnpj))
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cliente.CpfCnpj));
            if (!string.IsNullOrEmpty(cliente.CpfCnpj))
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cliente.CpfCnpj));
            //if (!string.IsNullOrEmpty(cliente.CpfCnpjSemMascara))
            //    queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_sem_masc_cnpj_cpf", ConditionOperator.Equal, cliente.CpfCnpjSemMascara);
            var bec = base.Provider.RetrieveMultiple(queryHelper);
            return (bec.Entities.Count > 0);
        }

        public List<T> ObterRevendas(Guid distribuidor)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("customerrelationship", "accountid", "partnerid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, distribuidor));
            query.Distinct = true;
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterDistribuidor(string cnpj)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cnpj));
            query.Distinct = true;
            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;
        }

        public List<T> ObterAutorizadas(int modificadasEmDias)
        {
            DateTime dataModificacao = DateTime.Now.AddDays(modificadasEmDias);

            #region Lista Alteração nos Produtos da Autorizadas
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            query.ColumnSet.AllColumns = false;
            query.ColumnSet.AddColumns(new string[] { "accountid", "accountnumber", "itbc_nomefantasia", "emailaddress1", "name", "itbc_address1_stateorprovince", "itbc_address1_city", "address1_postalcode", "itbc_address1_street", "itbc_address1_number", "address1_line3", "address1_line2", "telephone1", "new_divulgada_site", "new_posto_servico" });

            query.Criteria.Conditions.Add(new ConditionExpression("accountnumber", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("new_posto_servico", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("new_divulgada_site", ConditionOperator.NotNull));

            // Criar Inner Join com a tabela de Produto da Assistência Técnica
            query.AddLink("new_produto_assistecia_tecica", "accountid", "new_assistencia_tecnicaid");

            // Cria a condição de OR com a tabela de JOIN
            var filter = new FilterExpression(LogicalOperator.Or);
            filter.Conditions.Add(new ConditionExpression("modifiedon", ConditionOperator.GreaterEqual, dataModificacao));
            filter.AddCondition("new_produto_assistecia_tecica", "modifiedon", ConditionOperator.GreaterEqual, dataModificacao);
            query.Criteria.AddFilter(filter);

            var colecao = this.RetrieveMultiple(query);
            return (List<T>)colecao.List;

            #endregion
        }

        public List<T> ListarPorIntegracaoRevendaSite(Domain.Enum.Cliente.IntegracaoRevendaSite[] listIntegracaoRevendaSite)
        {
            var list = new List<Domain.ViewModels.OndeComprarQuemInstala>();

            string fetchXml = @"
                                <fetch mapping=""logical"">
                                  <entity name=""account"">
                                    <attribute name=""address1_city"" />
                                    <attribute name=""address1_line1"" />
                                    <attribute name=""address1_line2"" />
                                    <attribute name=""address1_line3"" />
                                    <attribute name=""address1_postalcode"" />
                                    <attribute name=""address1_stateorprovince"" />
                                    <attribute name=""name"" />
                                    <attribute name=""itbc_cpfoucnpj"" />
                                    <attribute name=""itbc_cpfoucnpj"" />
                                    <attribute name=""new_nome_fantasia"" />
                                    <attribute name=""new_numero_endereco_principal"" />
                                    <attribute name=""telephone1"" />
                                    <attribute name=""telephone2"" />
                                    <attribute name=""emailaddress1"" />
                                    <attribute name=""new_mercado_atuacao_redes"" />
                                    <attribute name=""new_mercado_atuacao_seguranca"" />
                                    <attribute name=""new_mercado_atuacao_telecom"" />
                                    <attribute name=""new_integracao_revenda_site"" />
                                    <filter>
                                      <condition attribute=""new_integracao_revenda_site"" operator=""in"">
                                        {0}
                                      </condition>
                                    </filter>
                                    <link-entity name=""contact"" from=""contactid"" to=""primarycontactid"" link-type=""outer"">
                                      <attribute name=""firstname"" />
                                      <attribute name=""itbc_cpfoucnpj"" />
                                    </link-entity>
                                    <link-entity name=""account"" from=""accountid"" to=""parentaccountid"" link-type=""outer"">
                                      <attribute name=""itbc_cpfoucnpj"" />
                                    </link-entity>
                                  </entity>
                                </fetch>
                                ";

            string values = string.Empty;

            foreach (var item in listIntegracaoRevendaSite)
            {
                values += string.Format("<value>{0}</value>", (int)item);
            }

            fetchXml = string.Format(fetchXml, values);

            var query = GetQueryExpression<T>(true);
            //query.TopCount = 20000;
            //query.PageInfo = new PagingInfo() { Count = 20000, PageNumber = 1 }
            RetrieveMultipleRequest retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetchXml.ToString())
            };
            return (List<T>)this.RetrieveMultiple(retrieveMultiple.Query).List;
        }

        public List<T> ListarContasRecategorizar(DateTime dtTrimestreAtual)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_dataadesao", ConditionOperator.LessThan, dtTrimestreAtual);

            query.Criteria.AddCondition("itbc_classificacaoidname", ConditionOperator.Equal, Domain.Enum.Conta.Classificacao.Revendas);

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            query.Criteria.AddCondition("itbc_tipodeconstituicao", ConditionOperator.Equal, (int)Domain.Enum.Conta.TipoConstituicao.Cnpj);

            query.Criteria.AddCondition("itbc_categorizar", ConditionOperator.NotEqual, (int)Domain.Enum.Conta.Categorizacao.Recategorizar);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ListarContasCategorizar(DateTime dtMesAnterior, DateTime dtMesAtual)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_dataadesao", ConditionOperator.GreaterEqual, dtMesAnterior);

            query.Criteria.AddCondition("itbc_dataadesao", ConditionOperator.LessThan, dtMesAtual);

            query.Criteria.AddCondition("itbc_classificacaoidname", ConditionOperator.Equal, Domain.Enum.Conta.Classificacao.Revendas);

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            query.Criteria.AddCondition("itbc_tipodeconstituicao", ConditionOperator.Equal, (int)Domain.Enum.Conta.TipoConstituicao.Cnpj);

            query.Criteria.AddCondition("itbc_categorizar", ConditionOperator.NotEqual, (int)Domain.Enum.Conta.Categorizacao.Categorizar);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ListarContasCategorizacao()
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_classificacaoidname", ConditionOperator.Equal, Domain.Enum.Conta.Classificacao.Revendas);
            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);
            query.Criteria.AddCondition("itbc_tipodeconstituicao", ConditionOperator.Equal, (int)Domain.Enum.Conta.TipoConstituicao.Cnpj);
            query.Criteria.AddCondition("itbc_categorizar", ConditionOperator.Equal, (int)Domain.Enum.Conta.Categorizacao.Categorizar);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ListarContasReCategorizacao()
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_classificacaoidname", ConditionOperator.Equal, Domain.Enum.Conta.Classificacao.Revendas);
            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);
            query.Criteria.AddCondition("itbc_tipodeconstituicao", ConditionOperator.Equal, (int)Domain.Enum.Conta.TipoConstituicao.Cnpj);
            query.Criteria.AddCondition("itbc_categorizar", ConditionOperator.Equal, (int)Domain.Enum.Conta.Categorizacao.Recategorizar);

            return (List<T>)this.RetrieveMultiplePaged(query).List;

        }
        public List<T> ListarContasAcessoExtranet()
        {
            var query = GetQueryExpression<T>(true);

            //filtra os registros que foram modificados de ontem ate agora.
            query.Criteria.AddCondition("modifiedon", ConditionOperator.GreaterEqual, DateTime.Now.AddDays(-2));

            query.Criteria.AddCondition("itbc_classificacaoidname", ConditionOperator.Equal, Domain.Enum.Conta.Classificacao.Revendas);

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            query.Criteria.AddCondition("itbc_tipodeconstituicao", ConditionOperator.Equal, (int)Domain.Enum.Conta.TipoConstituicao.Cnpj);

            query.Criteria.AddCondition("itbc_categorizar", ConditionOperator.Equal, (int)Domain.Enum.Conta.Categorizacao.Categorizada);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ListarGuidContasParaEnviarRegistroFielo(List<Intelbras.CRM2013.Domain.ValueObjects.FiltroContaSellout> filtroContas)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet = new ColumnSet("accountid");
            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);
          
            FilterExpression filter = new FilterExpression(LogicalOperator.Or);

            foreach (var filtroConta in filtroContas)
            {
                FilterExpression filterConta = new FilterExpression(LogicalOperator.And);

                filterConta.Conditions.Add(new ConditionExpression("itbc_classificacaoidname", ConditionOperator.Equal, filtroConta.Classificacao));

                if(filtroConta.SubClassificacao != null)
                {
                    filterConta.Conditions.Add(new ConditionExpression("itbc_subclassificacaoidname", ConditionOperator.In, filtroConta.Classificacao));
                }

                if(filtroConta.Categorias != null)
                {
                    filterConta.Conditions.Add(new ConditionExpression("itbc_categorianame", ConditionOperator.In, filtroConta.Categorias));
                }

                filter.AddFilter(filterConta);
            }

            query.Criteria.AddFilter(filter);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ConsultaAssitenciaFetch(string queryFetch)
        {
            var colecao = this.RetrieveMultiple(queryFetch);
            return (List<T>)colecao.List;
        }

        public DataTable ObterSellinAstec(string dt_inicio, string dt_fim)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            query.Criteria.AddCondition("itbc_pode_pontuar_sellin", ConditionOperator.Equal, (bool)true);

            query.Criteria.AddCondition("itbc_isastec", ConditionOperator.Equal, (bool)true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Conta.StateCode.Ativo);

            List<Domain.Model.Conta> lstRevendas = (List<Domain.Model.Conta>)this.RetrieveMultiple(query).List;

            string commandIn = string.Empty;

            StringBuilder strSql = new StringBuilder();
            if ((lstRevendas != null) && (lstRevendas.Count > 0))
            {
                strSql.Append("Select REPLACE((CAST(ft.CD_Emitente AS CHAR)+'-'+CAST(ft.TX_ano AS CHAR)+'-'+CAST(ft.CD_MES AS CHAR)),' ','') AS SellinId, CAST(ft.CD_GUID AS CHAR(36)) AS IdRevendaCRM, ft.CD_Emitente AS CodigoEmitente, ");
                strSql.Append("ft.TX_Emitente AS Emitente, ft.TX_Ano AS CompAno, ft.CD_Mes AS CompMes, 'Compra' AS TipoOperacao, 'Crédito compra direta' AS Item, sum(NM_Vl_Liquido) as Valor ");
                strSql.Append("FROM viewFactFaturamentoDetalhado ft ");
               // strSql.Append("INNER JOIN DimItem i ON i.SK_Item = ft.SK_Item ");
                strSql.AppendFormat("WHERE ft.DT_Emissao BETWEEN  '{0}' AND '{1}' ", dt_inicio, dt_fim);
                strSql.Append("AND ft.CD_Emitente in(");

                foreach (var item in lstRevendas)
                    commandIn += string.Concat("'", item.CodigoMatriz, "',");

                strSql.Append(commandIn.Substring(0, commandIn.Length - 1));
                strSql.Append(") ");
                //Filtra por todos os produtos não iniciados com os codigos (4%,994%,195%), que n foram classificados produtos acabados para canal com classificacao provedores e solucoes
                //para todos os canais foi ignorado os servicos 990%
                strSql.Append(" AND ((ft.CD_Item NOT LIKE '990%' AND ft.CD_Classificacao_Canal NOT IN ('07DD5E73-6DD9-E511-8C4B-0050568D7C5E','15767803-D2CA-E711-80CC-0050568DED44')) OR ((ft.CD_Item NOT LIKE '990%' AND ft.CD_Item NOT LIKE '4%' AND ft.CD_Item NOT LIKE '994%' AND ft.CD_Item NOT LIKE '195%') AND ft.CD_Classificacao_Canal IN ('07DD5E73-6DD9-E511-8C4B-0050568D7C5E','15767803-D2CA-E711-80CC-0050568DED44')))");

                strSql.Append(" GROUP BY ft.CD_Emitente, ft.TX_Emitente, ft.CD_Mes, ft.TX_Ano, ft.CD_GUID ");
                strSql.Append(" ORDER BY ft.CD_Emitente, ft.CD_Mes");

                return DataBaseSqlServer.executeQuery(strSql.ToString());
            }
            else
            {
                return null;
            }
        }
        public List<T> ListarContasAstec(DateTime data)
        {
            var query = GetQueryExpression<T>(true);
            var ultimoDia = DateTime.DaysInMonth(data.Year, data.Month);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Conta.StateCode.Ativo);
            query.Criteria.AddCondition("new_posto_servico", ConditionOperator.Equal, true);

            //Verifica se é o úlitma dia do mês
            if (data.Day == ultimoDia)
            {
                query.Criteria.AddCondition("new_gera_extrato_ultimo_dia", ConditionOperator.Equal, true);
            }
            else
            {
                query.Criteria.AddCondition("new_gera_extrato_dia_" + data.Day.ToString("00"), ConditionOperator.Equal, true);
            }

            return (List<T>)this.RetrieveMultiplePaged(query).List;

        }

        public String ObterTelefoneAssistencias(string nomeProcedure, ArrayList sqlParameters, string pSaida)
        {
            return DataBaseSqlServer.executeProcedureP_TelefoneAssistencias("p_TelefoneAssistencias", sqlParameters, pSaida);
        }

        public T BuscaMatrizEconomica(string raizCNPJ)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("parentaccountid", ConditionOperator.Null);
            query.Criteria.AddCondition("accountnumber", ConditionOperator.NotNull);
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Conta.StateCode.Ativo);
            var filter = new FilterExpression(LogicalOperator.Or);
            filter.AddCondition("itbc_cpfoucnpj", ConditionOperator.Like, raizCNPJ + "%");
            filter.AddCondition("itbc_cpfoucnpj", ConditionOperator.Like, raizCNPJ.GetOnlyNumbers() + "%");
            query.Criteria.AddFilter(filter);

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        public List<T> ListarContasRecategorizarMensal(DateTime dtMesAnterior)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);
            query.Criteria.AddCondition("itbc_tipodeconstituicao", ConditionOperator.Equal, (int)Domain.Enum.Conta.TipoConstituicao.Cnpj);
            query.Criteria.AddCondition("itbc_classificacaoidname", ConditionOperator.Equal, Domain.Enum.Conta.Classificacao.Revendas);
            query.Criteria.AddCondition("itbc_dataadesao", ConditionOperator.LessEqual, dtMesAnterior);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }
        public List<T> ListarContasRecategorizacaoMensal()
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);
            query.Criteria.AddCondition("itbc_tipodeconstituicao", ConditionOperator.Equal, (int)Domain.Enum.Conta.TipoConstituicao.Cnpj);
            query.Criteria.AddCondition("itbc_classificacaoidname", ConditionOperator.Equal, Domain.Enum.Conta.Classificacao.Revendas);
            query.Criteria.AddCondition("itbc_categorizar", ConditionOperator.Equal, (int)Domain.Enum.Conta.Categorizacao.RecategorizarMensal);

            return (List<T>)this.RetrieveMultiplePaged(query).List;
        }

        public DataTable ObterSellinProvedoresSolucoes(string dt_inicio, string dt_fim)
        {
            List<string> lstGuids = new List<string>();

            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;

            query.Criteria.AddCondition("itbc_participa_do_programa", ConditionOperator.Equal, (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim);

            query.Criteria.AddCondition("itbc_pode_pontuar_sellin", ConditionOperator.Equal, (bool)true);

            var filter = new FilterExpression(LogicalOperator.Or);
            filter.Conditions.Add(new ConditionExpression("itbc_categorianame", ConditionOperator.Equal, Domain.Enum.Conta.CategoriaConta.Provedores));
            filter.Conditions.Add(new ConditionExpression("itbc_categorianame", ConditionOperator.Equal, Domain.Enum.Conta.CategoriaConta.Rev_Sol));
            query.Criteria.AddFilter(filter);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, (int)Domain.Enum.Conta.StateCode.Ativo);

            List<Domain.Model.Conta> lstContas = (List<Domain.Model.Conta>)this.RetrieveMultiple(query).List;

            if(lstContas != null && lstContas.Count > 0)
            {
                foreach (var conta in lstContas)
                {
                    lstGuids.Add(conta.Id.ToString());
                }

                StringBuilder strSql = new StringBuilder();
                strSql.Append(" Select ( CONVERT(VARCHAR,vf.CD_Emitente) + '-' + CONVERT(VARCHAR, vf.CD_Nota_Fiscal) + '-' + CONVERT(VARCHAR,vf.CD_Serie) + '-' + CONVERT(VARCHAR,vf.CD_Item) + '-' + CONVERT(VARCHAR,vf.CD_Devolucao) ) AS SK_Faturamento, ");
                strSql.Append(" CAST(vf.CD_GUID AS CHAR(36)) AS 'Emitente_CD_GUID', vf.CD_Item AS 'Item_CD_Item', vf.TX_Item AS 'Item_TX_Item', vf.NM_Quantidade AS 'Faturamento_NM_Quantidade_Gera', ");
                strSql.Append(" (vf.NM_Vl_Liquido/vf.NM_Quantidade) AS 'Faturamento_NM_Vl_Unitario', (CASE (vf.CD_Devolucao) WHEN  1 THEN 'DEVOLUCAO' ELSE 'VENDA' END) AS 'Natureza_Operacao_TX_Natureza_Operacao', convert(VARCHAR, vf.DT_Emissao, 103) AS 'Faturamento_DT_Emissao', vf.CD_Nota_Fiscal AS 'Faturamento_CD_Nota_Fiscal', vf.CD_Serie AS 'Faturamento_CD_Serie' ");
                strSql.Append(" FROM ViewFactFaturamentoDetalhado vf ");
                strSql.Append(" INNER JOIN DimItem i ON i.CD_Item = vf.CD_Item ");
                strSql.AppendFormat(" WHERE vf.DT_Emissao BETWEEN  '{0}' AND '{1}' ", dt_inicio, dt_fim);
                strSql.Append(" AND vf.CD_GUID in('"+string.Join("', '", lstGuids)+"') ");
                //Filtra pelo inicio dos codigos dos produtos, somente produtos acabados
                strSql.Append(" AND (vf.CD_Item LIKE '4%' OR vf.CD_Item LIKE '994%' OR vf.CD_Item LIKE '195%')");

                return DataBaseSqlServer.executeQuery(strSql.ToString());
            } else {
                return null;
            }
        }
    }
}
