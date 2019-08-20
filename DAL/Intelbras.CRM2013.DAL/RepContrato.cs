using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Intelbras.CRM2013.Domain.ValueObjects;
using System.Xml;
using Intelbras.CRM2013.Domain.Enum;
using Microsoft.Xrm.Sdk.Messages;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.DAL
{
    public class RepContrato<T> : CrmServiceRepository<T>, IContrato<T>
    {
        public void AlterarStatusDoContrato(Guid contratoId, int statusContrato)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("contract", contratoId),
                State = new OptionSetValue(0),
                Status = new OptionSetValue(statusContrato)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public bool ExisteEndereco(Contrato contrato, Guid clienteId, Guid enderecoId)
        {
            var query = new QueryExpression("new_cliente_participante_endereco");
            query.ColumnSet.AllColumns = true;
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, clienteId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_enderecoid", ConditionOperator.Equal, enderecoId.ToString().Replace("{", "").Replace("}", "")));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count > 0)
                return true;

            return false;
        }

        public void SalvarEnderecosParticipantes(Guid clienteParticipanteId, Contrato contrato, Domain.Model.Conta cliente, Domain.Model.Endereco endereco)
        {
            ClienteParticipanteEndereco be = new ClienteParticipanteEndereco(OrganizationName, IsOffline);
            
            be.ContratoId = new Lookup(contrato.Id, "contract");
            be.ClienteId = new Lookup(cliente.Id, "account");
            be.ClienteParticipanteId = new Lookup(clienteParticipanteId, "new_cliente_participante_contrato");
            be.Endereco = endereco.Id.ToString();
            be.CodigoEndereco = endereco.Nome;
            be.Nome = endereco.Nome;
            be.Rua = endereco.Logradouro;
            be.Uf = endereco.SiglaEstado;
            be.Cidade = endereco.NomeCidade;
            be.Cep = endereco.Cep;
            be.Bairro = endereco.Bairro;
            (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.Create(be);
        }

        public Guid SalvarClienteParticipante(ClienteParticipante clienteParticipante)
        {
            var cli = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipante.InstanciarClienteParticipanteDoContrato(clienteParticipante);
            return (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoContrato.Create(cli);
        }

        public ClienteParticipante PesquisarParticipantePor(Contrato contrato, Domain.Model.Conta cliente)
        {
            ClienteParticipante clienteParticipante = null;
            var queryHelper = new QueryExpression("new_cliente_participante_contrato");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            var be = base.Provider.RetrieveMultiple(queryHelper);
            if (be.Entities.Count > 0)
                clienteParticipante = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipante.InstanciarClienteParticipante((new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoContrato.Retrieve(be.Entities[0].Id));

            return clienteParticipante;
        }

        public ClienteParticipante PesquisarParticipantePor(Contrato contrato, Domain.Model.Conta cliente, Domain.Model.Endereco endereco)
        {
            ClienteParticipante clienteParticipante = null;

            var queryHelper = new QueryExpression("new_cliente_participante_endereco");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_enderecoid", ConditionOperator.Equal, endereco.Id.ToString().Replace("{", "").Replace("}", "")));

            var be = base.Provider.RetrieveMultiple(queryHelper);

            if (be.Entities.Count > 0)
                clienteParticipante = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipante.InstanciarClienteParticipante((new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.Retrieve(be.Entities[0].Id));

            return clienteParticipante;
        }

        public ClienteParticipante PesquisarEnderecoParticipantePor(Guid enderecoparticipanteId)
        {
            ClienteParticipante clienteParticipante = null;

            var queryHelper = new QueryExpression("new_cliente_participante_endereco");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_cliente_participante_enderecoid", ConditionOperator.Equal, enderecoparticipanteId));

            var be = base.Provider.RetrieveMultiple(queryHelper);

            if (be.Entities.Count > 0)
                clienteParticipante = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipante.InstanciarClienteParticipante((new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.Retrieve(be.Entities[0].Id));

            return clienteParticipante;
        }

        /// <summary>
        /// Autor: Clausio Elmano de Oliveira
        /// Data: 10/12/2010
        /// Descrição: Obtem os endereços por contrato e cliente
        /// </summary>
        /// <param name="contrato"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public List<ClienteParticipante> ObterEnderecoClientesParticipantesPor(Contrato contrato, Domain.Model.Conta cliente)
        {
            var listaEndereco = new List<ClienteParticipante>();
            ClienteParticipante clienteParticipante = null;

            var queryHelper = new QueryExpression("new_cliente_participante_endereco");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));

            var be = base.Provider.RetrieveMultiple(queryHelper);

            if (be.Entities.Count > 0)
            {
                foreach (var item in be.Entities)
                {
                    clienteParticipante = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipante.InstanciarClienteParticipante((new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.Retrieve(be.Entities[0].Id));
                    listaEndereco.Add(clienteParticipante);
                }
            }

            return listaEndereco;
        }


        public Guid ObterEnderecoIDPrimeiroClienteParticipante(Contrato contrato, Domain.Model.Conta cliente)
        {
            Guid enderecoID = Guid.Empty;
            var queryHelper = new QueryExpression("new_cliente_participante_endereco");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));

            var be = base.Provider.RetrieveMultiple(queryHelper);

            if (be.Entities.Count > 0)
                enderecoID = ((EntityReference)be.Entities[0]["new_enderecoid"]).Id;

            return enderecoID;
        }

        public List<ClienteParticipante> ObterClientesParticipantesPor(Contrato contrato, Domain.Model.Conta cliente)
        {
            var listaClientesParticipantes = new List<ClienteParticipante>();
            ClienteParticipante clienteParticipante = null;

            var queryHelper = new QueryExpression("new_cliente_participante_contrato");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                foreach (var item in bec.Entities)
                {
                    clienteParticipante = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipante.InstanciarClienteParticipante((new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoContrato.Retrieve(item.Id));
                    listaClientesParticipantes.Add(clienteParticipante);
                }
            }
            return listaClientesParticipantes;
        }
        public List<ClienteParticipante> ObterClientesParticipantesPor(Contrato contrato)
        {
            var listaClientesParticipantes = new List<ClienteParticipante>();
            ClienteParticipante clienteParticipante = null;

            var queryHelper = new QueryExpression("new_cliente_participante_contrato");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));

            var bec = base.Provider.RetrieveMultiple(queryHelper);

            if (bec.Entities.Count > 0)
            {
                foreach (var item in bec.Entities)
                {
                    clienteParticipante = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipante.InstanciarClienteParticipante((new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoContrato.Retrieve(item.Id));
                    listaClientesParticipantes.Add(clienteParticipante);
                }
            }
            return listaClientesParticipantes;
        }

        public void ExcluirEnderecosSemLocalidadePor(ClienteParticipante clienteSemLocalidade)
        {
            base.Provider.Delete("new_cliente_participante_endereco", clienteSemLocalidade.Id);
        }

        public CalendarioDeTrabalho ObterCalendario(Guid contratoId)
        {
            var be = base.Provider.Retrieve("contract", contratoId, new ColumnSet(true));
            CalendarioDeTrabalho c = null;
            string stringDoCalendarioDoContratoDoCrm = string.Empty;
            if (be.Contains("effectivitycalendar"))
                stringDoCalendarioDoContratoDoCrm = Convert.ToString(be["effectivitycalendar"]);
            if (!string.IsNullOrEmpty(stringDoCalendarioDoContratoDoCrm))
            {
                var arrayDoCalendarioDoContratoDoCrm = ObterPlanejamentoDoCalendario(stringDoCalendarioDoContratoDoCrm);

                var diasDaSemana = ObterDiasDaSemanaComHoraDeInicioEFimDaJornadaDeTrabalho(arrayDoCalendarioDoContratoDoCrm);
                if (null != diasDaSemana && diasDaSemana.Length == 7)
                {

                    c = new CalendarioDeTrabalho(OrganizationName, IsOffline)
                    {
                        Domingo = diasDaSemana[0],
                        Segunda = diasDaSemana[1],
                        Terca = diasDaSemana[2],
                        Quarta = diasDaSemana[3],
                        Quinta = diasDaSemana[4],
                        Sexta = diasDaSemana[5],
                        Sabado = diasDaSemana[6]
                    };
                }

            }

            return c;
        }

        private static DiaDaSemana[] ObterDiasDaSemanaComHoraDeInicioEFimDaJornadaDeTrabalho(string[] planejamentoDoCalendario)
        {

            var arr = new DiaDaSemana[planejamentoDoCalendario.Length];
            var c = new CalendarioDeTrabalho("", false);

            var arr2 = System.Enum.GetValues(typeof(DayOfWeek));

            for (var i = 0; i < planejamentoDoCalendario.Length; i++)
            {

                var s = planejamentoDoCalendario[i];
                var horaInicio = s.IndexOf('+');
                var horaFim = s.LastIndexOf('+');

                // TODO: Incluir clausula para tratar dias sem jornada de trabalho

                if (horaInicio > -1 && horaFim > -1)
                    arr[i] = new DiaDaSemana((DayOfWeek)arr2.GetValue(i), new TimeSpan(horaInicio, 0, 0), new TimeSpan(horaFim, 0, 0));
                else
                    arr[i] = new DiaDaSemana((DayOfWeek)arr2.GetValue(i), TimeSpan.MinValue, TimeSpan.MinValue);
            }

            return arr;
        }
        private static string[] ObterPlanejamentoDoCalendario(string s)
        {
            var arr = new string[7];
            var j = 0;
            var x = 24;
            for (var i = 0; i < s.Length; i++)
            {

                if (i == 24)
                {
                    arr[j] = s.Substring(x - 24, 24);
                    j++;
                    x += i;
                    i = 0;
                }

                if (x == (s.Length + 24))
                    break;
            }

            return arr;
        }

        public List<T> ListarPorClientesParticipantes(Domain.Model.Conta cliente, List<StatusDoContrato> status)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Distinct = true;
            string[] arrayStatus = new string[status.Count];
            for (int i = 0; i < status.Count; i++) arrayStatus[i] = Convert.ToString((int)status[i]);
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.In, arrayStatus));
            queryHelper.AddLink("new_cliente_participante_contrato", "contractid", "new_contratoid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, cliente.Id));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<T> ListarPorPerfilUsuarioServico(Domain.Model.Contato contato)
        {
            var query = GetQueryExpression<T>(true);
            query.Distinct = true;
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 3)); // Ativo
            query.AddLink("new_permissao_usuario_servico", "contractid", "new_contratoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contato.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<ClienteDoContrato> ListarClientesParticipantes(ClienteDoContrato clienteDoContrato, int pagina, int quantidade, ref bool existemMaisRegistros, ref string cookie)
        {
            string fetch = @"<fetch mapping='logical' distinct='true'>
                              <entity name='new_cliente_participante_endereco'>
                                <attribute name='new_cliente_participante_enderecoid' />
                                <attribute name='new_rua' />
                                {0}
                                <link-entity name='new_cliente_participante_contrato' from='new_cliente_participante_contratoid' to='new_cliente_participanteid'>
                                {1}
                                  <link-entity name='account' from='accountid' to='new_clienteid'>
                                    <attribute name='name' />
                                    <attribute name='itbc_cpfoucnpj' />
                                    <attribute name='itbc_nomefantasia' />
                                    <attribute name='accountid' />
                                    {2}
                                  </link-entity>
                                  <link-entity name='contract' from='contractid' to='new_contratoid'>
                                    <link-entity name='new_permissao_usuario_servico' from='new_contratoid' to='contractid'>
                                    {3}
                                    </link-entity>
                                  </link-entity>
                                </link-entity>
                                <link-entity name='customeraddress' from='customeraddressid' to='new_enderecoid'>
                                  <attribute name='itbc_numero' />
                                  <attribute name='line3' />
                                  <attribute name='new_numero_endereco' />
                                </link-entity>
                              </entity>
                            </fetch>";

            string valor0 = string.IsNullOrEmpty(clienteDoContrato.Rua) ? "" : string.Format(@"<filter>
                                <condition attribute='new_rua' operator='like' value='%{0}%' />
                            </filter>", clienteDoContrato.Rua);

            string valor1 = clienteDoContrato.ContratoId == Guid.Empty ? "" : string.Format(@"<filter>
                                <condition attribute='new_contratoid' operator='eq' value='{0}' />
                            </filter>", clienteDoContrato.ContratoId);

            string valor2 = String.IsNullOrEmpty(clienteDoContrato.CNPJ) ? "" : "<filter><condition attribute='itbc_cpfoucnpj' operator='eq' value='" + clienteDoContrato.CNPJ + "' />";
            valor2 += String.IsNullOrEmpty(clienteDoContrato.Nome) ? "" : (String.IsNullOrEmpty(valor2) ? "<filter>" : "") + "<condition attribute='name' operator='like' value='%" + clienteDoContrato.Nome + "%' />";
            valor2 += String.IsNullOrEmpty(clienteDoContrato.NomeFantasia) ? "" : (String.IsNullOrEmpty(valor2) ? "<filter>" : "") + "<condition attribute='itbc_nomefantasia' operator='like' value='%" + clienteDoContrato.NomeFantasia + "%' />";
            valor2 += clienteDoContrato.ClienteId == Guid.Empty ? "" : (String.IsNullOrEmpty(valor2) ? "<filter>" : "") + "<condition attribute='accountid' operator='eq' value='" + clienteDoContrato.ClienteId + "' />";
            valor2 += String.IsNullOrEmpty(valor2) ? "" : "</filter>";

            string valor3 = clienteDoContrato.ContatoId == Guid.Empty ? "" : string.Format(@"<filter>
                                <condition attribute='new_contatoid' operator='eq' value='{0}' />
                            </filter>", clienteDoContrato.ContatoId);

            fetch = string.Format(fetch, valor0, valor1, valor2, valor3);

            var query = GetQueryExpression<T>(true);
            query.PageInfo = new PagingInfo() { PageNumber = pagina, Count = quantidade, PagingCookie = cookie };
            RetrieveMultipleRequest retrieveMultiple;
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch.ToString())
            };
            EntityCollection colecao = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;
            existemMaisRegistros = colecao.TotalRecordCountLimitExceeded;
            cookie = colecao.PagingCookie;
            //var listaXml = cadastroRepository.ListarFetchPaginado(pagina, quantidade, fetch, ref existemMaisRegistros, ref cookie);
            List<ClienteDoContrato> clientesDoContrato = new List<ClienteDoContrato>();

            foreach (var item in colecao.Entities)
            {
                var cliDoContrato = new ClienteDoContrato();
                if(item.Contains("account2.accountid")) cliDoContrato.ClienteId = new Guid(((AliasedValue)item["account2.accountid"]).Value.ToString());
                if (item.Contains("account2.itbc_cpfoucnpj")) cliDoContrato.CNPJ = Convert.ToString(((AliasedValue)item["account2.itbc_cpfoucnpj"]).Value).InputMask();
                if (item.Contains("account2.itbc_nomefantasia")) cliDoContrato.NomeFantasia = Convert.ToString(((AliasedValue)item["account2.itbc_nomefantasia"]).Value);
                if (item.Contains("account2.name")) cliDoContrato.Nome = Convert.ToString(((AliasedValue)item["account2.name"]).Value);
                if (item.Contains("customeraddress5.line3")) cliDoContrato.Complemento = Convert.ToString(((AliasedValue)item["customeraddress5.line3"]).Value);
                if (item.Contains("customeraddress5.itbc_numero")) cliDoContrato.Numero = Convert.ToString(((AliasedValue)item["customeraddress5.itbc_numero"]).Value);
                if (item.Contains("new_rua")) cliDoContrato.Rua = Convert.ToString(item["new_rua"]);
                cliDoContrato.ClienteParticipanteEnderecoId = item.Id;
                cliDoContrato.ContratoId = clienteDoContrato.ContratoId;
                cliDoContrato.ContratoId = clienteDoContrato.ContratoId;
                clientesDoContrato.Add(cliDoContrato);
            }

            return clientesDoContrato;
        }

        public T ObterPor(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("incident", "contractid", "contractid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ocorrencia.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ObterContratosProximoVencimento(DateTime dataTermino)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("expireson", ConditionOperator.On, dataTermino));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

    }
}