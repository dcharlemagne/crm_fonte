using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SDKore.Helper;
using System.Data;

namespace Intelbras.CRM2013.DAL
{
    public class OcorrenciaRepository<T> : CrmServiceRepository<T>, IOcorrencia<T>
    {
        #region Integração com os Correios

        public List<T> ListarOcorrenciasParaRastreioNosCorreios()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_numero_objeto", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotNull)); // Apenas ocorrencias com número de objeto preenchido
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotIn, new string[] { "200003", "200004", "200039", "200043", "200044" })); // Ignorar ocorrencias fechadas, canceladas, etc...
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 300005)); // Apenas atendimento avulso
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOcorrenciasParaGeracaoDeArquivoXmlParaOsCorreios()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_numero_objeto", Microsoft.Xrm.Sdk.Query.ConditionOperator.Null)); //Apenas ocorrencias sem número de objeto preenchido
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotIn, new string[] { "200003", "200004", "200039", "200043", "200044" })); // Ignorar ocorrencias fechadas, canceladas, etc...
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 300005)); // Apenas atendimento avulso
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_descricao_situacao_postagem", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, "E-Ticket Gerado!"));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_codigo_postagem", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotNull)); //Apenas ocorrencias com autorização de postagem preenchido
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_descricao_situacao_postagem", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotEqual, "E-Ticket Gerado!")); // Apenas atendimento avulso
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        #endregion

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

        public List<T> ListarOcorrenciasPor(Guid solicitante)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_solicitanteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, solicitante));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOcorrenciasSemSLADosUltimos(int dias)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", ConditionOperator.GreaterEqual, DateTime.Now.AddDays(-1 * dias).ToString("MM/dd/yyyy")));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", ConditionOperator.GreaterEqual, 200090));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", ConditionOperator.LessEqual, 200099));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", ConditionOperator.GreaterEqual, 200000)); //200000 Aberta 200001 Andamento 200002 Pendente 200003 Cancelada 200004 Fechada
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", ConditionOperator.LessEqual, 200004));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("prioritycode", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_localidadeid", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("followupby", ConditionOperator.Null));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<Ocorrencia> ListarOcorrenciasComDiagnosticoDivergenteDosUltimos(int dias)
        {
            List<Ocorrencia> ocorrencias = new List<Ocorrencia>();
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", ConditionOperator.GreaterEqual, DateTime.Now.AddDays(-1 * dias).ToString("MM/dd/yyyy")));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("productid", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", ConditionOperator.Equal, 300009)); //Ordem de Serviço
            query.Orders.Add(new OrderExpression("createdon", OrderType.Descending));
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;

            //cria um array de estruturas para não chamar novamente no EMS a estrutura de produtos
            List<Product> produtos = new List<Product>();
            List<List<Product>> estruturas = new List<List<Product>>();
            foreach (var item in result)
            {
                Ocorrencia _ocorrencia = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(item.Id);
                _ocorrencia.Diagnosticos = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.ListarPor(_ocorrencia);
                if (_ocorrencia.Diagnosticos.Count > 0)
                {
                    List<Product> estruturaProdutoPrincipal = new List<Product>();
                    if (produtos.Count > 0)
                    {
                        for (int x = 0; x < produtos.Count; x++)
                        {
                            if (produtos[x].Codigo == _ocorrencia.Produto.Codigo)
                                estruturaProdutoPrincipal = estruturas[x];
                        }
                    }
                    //Se não achou na tabela provisória, procura no EMS
                    if (estruturaProdutoPrincipal.Count == 0)
                    {
                        estruturaProdutoPrincipal = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Produto.BuscarEstruturaDoProdutoPor("", _ocorrencia.Produto.Codigo);
                        produtos.Add(_ocorrencia.Produto);
                        estruturas.Add(estruturaProdutoPrincipal);
                    }

                    bool existeAlgumItemNaEstrutura = false;
                    for (int x = 0; x < _ocorrencia.Diagnosticos.Count; x++)
                    {
                        bool existeNaEstrutura = false;

                        foreach (Product itemEstrutua in estruturaProdutoPrincipal)
                        {
                            if (_ocorrencia.Diagnosticos[x].Produto.Codigo == itemEstrutua.Codigo)
                            {
                                existeNaEstrutura = true;
                                break;
                            }
                        }

                        if (!existeNaEstrutura)
                        {
                            existeAlgumItemNaEstrutura = true;
                            _ocorrencia.Diagnosticos[x].Produto.ExportaERP = "ESTRANHO";
                        }
                    }

                    if (existeAlgumItemNaEstrutura)
                    {
                        ocorrencias.Add(_ocorrencia);
                    }
                }
            }
            return ocorrencias;
        }

        public List<Ocorrencia> ListarOcorrenciasComNumeroDeSerieDosUltimos(int dias)
        {
            List<Ocorrencia> ocorrencias = new List<Ocorrencia>();
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new ConditionExpression("createdon", ConditionOperator.GreaterEqual, DateTime.Now.AddDays(-1 * dias)));
            query.Criteria.Conditions.Add(new ConditionExpression("productserialnumber", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("caseorigincode", ConditionOperator.GreaterEqual, 200004)); //Ordem de Serviço
            query.Criteria.Conditions.Add(new ConditionExpression("caseorigincode", ConditionOperator.LessEqual, 200006)); //Integração
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotEqual, StatusDaOcorrencia.Cancelada));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotEqual, StatusDaOcorrencia.CanceladaSistema));
            query.Orders.Add(new OrderExpression("createdon", OrderType.Descending));
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            foreach (var item in result)
            {
                Ocorrencia _ocorrencia = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(item.Id);
                Product produto = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Produto.NumeroDeSerieExisteNoEMS(_ocorrencia.ProdutosDoCliente, "");
                _ocorrencia.ExportaERP = (produto != null).ToString();
                if (_ocorrencia.Produto == null)
                    _ocorrencia.DescricaoDaMensagemDeIntegracao = "Produto Nulo no CRM";
                if (produto == null)
                    _ocorrencia.Produto = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Produto.ObterPor(_ocorrencia);
                else
                {
                    _ocorrencia.Produto = produto;
                    Product produtoCRM = (new Intelbras.CRM2013.Domain.Servicos.RepositoryService()).Produto.ObterPorNumero(produto.Codigo);
                    _ocorrencia.Produto.Id = produtoCRM.Id;
                }
                ocorrencias.Add(_ocorrencia);
            }
            return ocorrencias;
        }

        public T ObterPor(string numeroDaOcorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("ticketnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, numeroDaOcorrencia));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterPorNumeroOcorrencia(string numeroDaOcorrencia, Domain.Model.Conta assistenciaTecnica)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("ticketnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, numeroDaOcorrencia));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public string ObterNumeroOcorrenciaPor(Guid id)
        {
            string numero = string.Empty;
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.ColumnSet.AddColumn("ticketnumber");
            query.Criteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, id));
            var colecao = base.Provider.RetrieveMultiple(query);
            if (colecao.Entities.Count > 0 && colecao.Entities[0].Contains("ticketnumber"))
                numero = Convert.ToString(colecao.Entities[0]["ticketnumber"]);
            return numero;
        }

        public List<T> ListarPorNumeroSerie(string numeroSerie, Domain.Model.Conta assistenciaTecnica, string[] status, string[] origem)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("productserialnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, numeroSerie));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCpfCnpjCliente(string cpfCnpj, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            List<T> retorno = new List<T>();
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            //Busca pelas contas, depois pelos contatos
            query.AddLink("account", "customerid", "accountid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.InputMask()));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);
            //Busca pelos contatos
            query.LinkEntities.Clear();
            query.AddLink("contact", "customerid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.InputMask()));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);

            return retorno;
        }

        public List<T> ListarPorNomeCliente(string nomeCliente, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            List<T> retorno = new List<T>();

            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            //Busca pelas contas, depois pelos contatos
            query.AddLink("account", "customerid", "accountid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, nomeCliente));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);
            //Busca pelos contatos
            query.LinkEntities.Clear();
            query.AddLink("contact", "customerid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("fullname", ConditionOperator.Equal, nomeCliente));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);

            return retorno;
        }

        public List<T> ListarPreOSPor(string CPFouCNPJ)
        {
            List<T> retorno = new List<T>();

            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_pre_os", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, true));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 2004)); //Portal ASTEC
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 300009)); //Ordem de Servi
            //Busca pelas contas, depois pelos contatos
            query.AddLink("account", "customerid", "accountid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, CPFouCNPJ.InputMask()));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);
            //Busca pelos contatos
            query.LinkEntities.Clear();
            query.AddLink("contact", "customerid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, CPFouCNPJ.InputMask()));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);

            return retorno;
        }

        public List<T> ListarPorStatus(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            //var query = GetQueryExpression<T>(true);
            var query = new QueryExpression("incident");
            query.ColumnSet.AddColumns("ticketnumber", "createdon", "statuscode", "statecode", "accountid", "productserialnumber", "productid", "followupby", "new_data_hora_escalacao");
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPorStatusDiagnostico(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, string[] statusDiagnostico, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));

            query.AddLink("new_diagnostico_ocorrencia", "incidentid", "new_ocorrenciaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", ConditionOperator.Equal, statusDiagnostico));

            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));

            query.Distinct = true;
            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPorNotaFiscalCompra(string notaFical, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("caseorigincode", ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.In, status));
            query.Criteria.Conditions.Add(new ConditionExpression("new_numero_nf_consumidor", ConditionOperator.Equal, notaFical));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOcorrenciaAbertaComIntervencaoTecnica(Domain.Model.Conta assistenciaTecnica)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, 200035, 200036, 200037, 200038, 200039, 200045));
            query.AddLink("new_intervencao_tecnica", "incidentid", "new_ocorrenciaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            query.Distinct = true;
            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public SLA ObterSLAPor(Contrato contrato, TipoDePrioridade prioridade, Guid localidadeId, TipoDeOcorrencia tipoOcorrencia)
        {
            SLA sla = null;
            if (contrato == null || prioridade == TipoDePrioridade.Vazio || localidadeId == Guid.Empty) return sla;

            var query = GetQueryExpression<SLA>(true);
            query.ColumnSet.AddColumns("new_tempo_sla", "new_tempo_escalacao", "itbc_tempo_solucao");
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_localidadeid", ConditionOperator.Equal, localidadeId));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_prioridade", ConditionOperator.Equal, (int)prioridade));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_tipo_ocorrencia", ConditionOperator.Equal, (int)tipoOcorrencia));

            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            if (result.Count > 0)
            {
                sla = new SLA();
                if (result[0].Attributes.Contains("new_tempo_sla"))
                    sla.Tempo = Convert.ToInt32(result[0].Attributes["new_tempo_sla"]);
                if (result[0].Attributes.Contains("new_tempo_escalacao"))
                    sla.TempoDeEscalacao = Convert.ToInt32(result[0].Attributes["new_tempo_escalacao"]);
                if (result[0].Attributes.Contains("itbc_tempo_solucao"))
                    sla.TempoSolucao = Convert.ToInt32(result[0].Attributes["itbc_tempo_solucao"]);
            }
            else
            {

                var query2 = GetQueryExpression<SLA>(true);
                query2.ColumnSet.AddColumns("new_tempo_sla", "new_tempo_escalacao", "itbc_tempo_solucao");
                query2.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", ConditionOperator.Equal, contrato.Id));
                query2.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_localidadeid", ConditionOperator.Equal, localidadeId));
                query2.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_prioridade", ConditionOperator.Equal, (int)prioridade));

                DataCollection<Entity> result2 = base.Provider.RetrieveMultiple(query2).Entities;
                if (result2.Count > 0)
                {
                    sla = new SLA();
                    if (result2[0].Attributes.Contains("new_tempo_sla"))
                        sla.Tempo = Convert.ToInt32(result2[0].Attributes["new_tempo_sla"]);
                    if (result2[0].Attributes.Contains("new_tempo_escalacao"))
                        sla.TempoDeEscalacao = Convert.ToInt32(result2[0].Attributes["new_tempo_escalacao"]);
                    if (result2[0].Attributes.Contains("itbc_tempo_solucao"))
                        sla.TempoSolucao = Convert.ToInt32(result2[0].Attributes["itbc_tempo_solucao"]);
                }
            }
            return sla;
        }

        public T ObtemReduntanteASTEC(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            RetrieveMultipleRequest retrieveMultiple;

            StringBuilder fetch = new StringBuilder();
            fetch.Append(@"<fetch mapping='logical' count='1'>
                              <entity name='incident'>
                                <attribute name='incidentid' />
                                <filter>
                                  <condition attribute='caseorigincode' operator='in'>
                                    <value>200004</value>
                                    <value>200006</value>
                                  </condition>");

            if (ocorrencia.Id != null && ocorrencia.Id != Guid.Empty)
                fetch.AppendFormat("<condition attribute='incidentid' operator='ne' value='{0}' />", ocorrencia.Id);

            if (ocorrencia.ProdutoId != null)
                fetch.AppendFormat("<condition attribute='productid' operator='eq' value='{0}' />", ocorrencia.ProdutoId.Id);

            if (ocorrencia.AutorizadaId != null)
                fetch.AppendFormat("<condition attribute='new_autorizadaid' operator='eq' value='{0}' />", ocorrencia.AutorizadaId.Id);

            if (ocorrencia.DataFabricacaoProduto == null)
            {
                if (ocorrencia.ClienteId != null)
                    fetch.AppendFormat("<condition attribute='customerid' operator='eq' value='{0}' />", ocorrencia.ClienteId.Id);

                if (String.IsNullOrEmpty(ocorrencia.NumeroNotaFiscalDeCompra))
                    fetch.Append("<condition attribute='new_numero_nf_consumidor' operator='null' />");
                else
                    fetch.AppendFormat("<condition attribute='new_numero_nf_consumidor' operator='eq' value='{0}' />", ocorrencia.NumeroNotaFiscalDeCompra);
            }

            if (!string.IsNullOrEmpty(ocorrencia.ProdutosDoCliente))
                fetch.AppendFormat("<condition attribute='productserialnumber' operator='eq' value='{0}' />", ocorrencia.ProdutosDoCliente);

            if (String.IsNullOrEmpty(ocorrencia.NumeroNotaFiscalDeCompra))
                fetch.Append("<condition attribute='new_numero_nf_consumidor' operator='null' />");
            else
                fetch.AppendFormat("<condition attribute='new_numero_nf_consumidor' operator='eq' value='{0}' />", ocorrencia.NumeroNotaFiscalDeCompra);

            fetch.Append("      <condition attribute='statuscode' operator='not-in'>");
            fetch.Append("        <value>5</value>");
            fetch.Append("        <value>6</value>");
            fetch.Append("        <value>200003</value>");
            fetch.Append("        <value>200040</value>");
            fetch.Append("        <value>200041</value>");
            fetch.Append("        <value>200042</value>");
            fetch.Append("        <value>200043</value>");
            fetch.Append("        <value>200044</value>");
            fetch.Append("      </condition>");

            fetch.AppendFormat(@" <filter type='or' >                                                        
                                        <condition attribute='new_data_origem' operator='le' value='{0}' />
                                        <condition attribute='new_data_entrega_cliente' operator='ge' value='{1}' />
                                   </filter>",
                                    ocorrencia.DataOrigem.Value.ToString("yyyy-MM-ddT23:59:59"),
                                    ocorrencia.DataOrigem.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
            fetch.Append(@"  </filter>
                            </entity>                            
                           </fetch>");


            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch.ToString())
            };
            var colecao = this.RetrieveMultiple(retrieveMultiple.Query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public void AtualizarStuatuDa(Guid ocorrenciaId)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("incidentid", ConditionOperator.Equal, ocorrenciaId));
            DataCollection<Entity> ocorrencia = base.Provider.RetrieveMultiple(query).Entities;
            if (ocorrencia.Count == 0) return;

            var q1 = GetQueryExpression<DiagnosticoOcorrencia>(true);
            q1.ColumnSet.AllColumns = true;
            q1.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorrenciaId));
            DataCollection<Entity> diagnosticos = base.Provider.RetrieveMultiple(q1).Entities;

            bool StatusEmAnalise = false, StatusAguardandoPeca = false, StatusPedidoSolicitado = false, StatusAguardandoConserto = false, StatusConsertoRealizado = false;
            int statusOcorrencia = ((OptionSetValue)ocorrencia[0].Attributes["statuscode"]).Value;
            int novoStatusOcorrencia = 0;
            if (diagnosticos.Count > 0)
            {
                foreach (var item in diagnosticos)
                {
                    int statusDiagnostico = ((OptionSetValue)item.Attributes["statuscode"]).Value;
                    bool geraTroca = ((Boolean)item.Attributes["new_geratroca"]);
                    if (geraTroca)
                    {
                        //Verifica os possíveis status da OS pelos diagnósticos informados, procurando o menor status possível para alterar o status da OS
                        if (statusDiagnostico <= 1) //Aguardando Peça
                        {
                            StatusAguardandoPeca = true;
                        }
                        else if (statusDiagnostico == 3) //Pedido solicitado
                        {
                            StatusPedidoSolicitado = true;
                        }
                        else if (statusDiagnostico == 4) //Aguardando Conserto
                        {
                            StatusAguardandoConserto = true;
                        }
                        else if (statusDiagnostico == 5) //Conserto Realizado
                        {
                            StatusConsertoRealizado = true;
                        }
                    }
                }
            }
            else //se não tiver nenhum dignóstico deve ir para Em Analise
            {
                StatusEmAnalise = true;
            }


            //Se já tiver passado pelo status de conserto realizado, não muda o staus da OS.
            if (statusOcorrencia != (int)StatusDaOcorrencia.Aguardando_Analise &&
                statusOcorrencia != (int)StatusDaOcorrencia.Aguardando_Peça &&
                statusOcorrencia != (int)StatusDaOcorrencia.Pedido_Solicitado &&
                statusOcorrencia != (int)StatusDaOcorrencia.Aguardando_Conserto &&
                statusOcorrencia != (int)StatusDaOcorrencia.Conserto_Realizado)
                return;

            if (StatusEmAnalise)
                novoStatusOcorrencia = (int)StatusDaOcorrencia.Aguardando_Analise;
            else if (StatusAguardandoPeca)
                novoStatusOcorrencia = (int)StatusDaOcorrencia.Aguardando_Peça;
            else if (StatusPedidoSolicitado)
                novoStatusOcorrencia = (int)StatusDaOcorrencia.Pedido_Solicitado;
            else if (StatusAguardandoConserto)
                novoStatusOcorrencia = (int)StatusDaOcorrencia.Aguardando_Conserto;
            else if (StatusConsertoRealizado &&
                (statusOcorrencia == (int)StatusDaOcorrencia.Aguardando_Analise || statusOcorrencia == (int)StatusDaOcorrencia.Aguardando_Peça
                || statusOcorrencia == (int)StatusDaOcorrencia.Pedido_Solicitado || statusOcorrencia == (int)StatusDaOcorrencia.Aguardando_Conserto))
                novoStatusOcorrencia = (int)StatusDaOcorrencia.Conserto_Realizado;
            else //qualuqer outro caso não muda o status da OS
                return;

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("incident", ocorrenciaId),
                State = new OptionSetValue(0),
                Status = new OptionSetValue((int)novoStatusOcorrencia)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            //if (resp != null)
            //    return true;
            //return false;
        }

        public List<T> ListarOcorrenciasDashBoard()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_hora_conclusao", ConditionOperator.Null));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_hora_escalacao", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("followupby", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractdetailid", ConditionOperator.NotNull));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Contrato contrato, Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("customerid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, cliente.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Domain.Model.Conta cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("customerid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, cliente.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(string notaFiscal, string numeroDeSerieDoProduto, string numeroDaOcorrencia, string postoDeServicoId, int status, int tipoDeOcorrencia, string nomeDoCliente, DateTime dataInicial, DateTime dataFinal)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));

            if (dataInicial != null && dataInicial != DateTime.MinValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dataInicial));

            if (dataFinal != null && dataFinal != DateTime.MinValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessEqual, dataFinal));

            if (tipoDeOcorrencia > 0)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", ConditionOperator.Equal, tipoDeOcorrencia));

            if (status > 0)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", ConditionOperator.Equal, status));

            if (!string.IsNullOrEmpty(postoDeServicoId))
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", ConditionOperator.Equal, postoDeServicoId));

            if (!string.IsNullOrEmpty(numeroDaOcorrencia))
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("ticketnumber", ConditionOperator.Like, numeroDaOcorrencia));

            if (!string.IsNullOrEmpty(numeroDeSerieDoProduto))
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("productserialnumber", ConditionOperator.Like, numeroDeSerieDoProduto));

            if (!string.IsNullOrEmpty(notaFiscal))
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_numero_nf_consumidor", ConditionOperator.Equal, notaFiscal));

            if (!string.IsNullOrEmpty(nomeDoCliente)) //usado para filtrar as OS dos Extratos de Pagamento
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_extrato_pagamentoid", ConditionOperator.Equal, nomeDoCliente));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(UnidadeNegocio unidadeDeNegocio)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("owningbusinessunit", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeDeNegocio.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorDataCriacao(DateTime dataCriacaoInicio, DateTime dataCriacaoFim, OrigemDaOcorrencia[] arrayOrigem)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", ConditionOperator.GreaterEqual, dataCriacaoInicio.Date));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", ConditionOperator.LessThan, dataCriacaoFim.Date.AddDays(1)));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", ConditionOperator.In, ConvertArray(arrayOrigem)));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> PesquisarOcorrenciaPor(DateTime dataInicial, DateTime dataFinal, UnidadeNegocio unidadeDeNegocio)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Ascending));
            if (dataInicial != null && dataInicial != DateTime.MinValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dataInicial));
            if (dataFinal != null && dataFinal != DateTime.MinValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessEqual, dataFinal));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("owningbusinessunit", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, unidadeDeNegocio.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOcorrenciasEspeciaisPor(Domain.Model.Fatura notaFiscal)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("new_nota_ocorrncia", "ticketnumber", "new_numero_ocorrencia");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_nota_fiscalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, notaFiscal.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Guid SalvarLogisticaReversa(List<Diagnostico> logisticaReversa, Domain.Model.Fatura notaFiscal)
        {
            LogisticaReversa extrato = new LogisticaReversa(base.OrganizationName, base.IsOffline);
            extrato.Nome = notaFiscal.Descricao + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
            extrato.NumeroNotaFiscal = notaFiscal.NumeroNF;
            extrato.SerieNotaFiscal = notaFiscal.Serie;
            extrato.DataNotaFiscal = notaFiscal.DataEmissao;
            extrato.PostoAutorizadoId = new Lookup() { Id = notaFiscal.Cliente.Id, Type = "account" };
            var idExtrato = (new Domain.Servicos.RepositoryService()).LogisticaReversa.Create(extrato);

            if (idExtrato != Guid.Empty)
            {
                foreach (Diagnostico diagnostico in logisticaReversa)
                {
                    diagnostico.ExtratoId = new Lookup(idExtrato, "new_extrato_logistica_reversa");
                    (new Domain.Servicos.RepositoryService()).Diagnostico.Update(diagnostico);
                }
            }
            return idExtrato;
        }

        public void AtualizaValoresDosLancamentosAvulsosNo(Guid extratoid)
        {
            var query = GetQueryExpression<LancamentoAvulsoDoExtrato>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new ConditionExpression("new_extratoid", ConditionOperator.Equal, extratoid));
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            decimal somaValores = 0;
            foreach (var item in result)
            {
                if (item.Attributes["new_valor"] != null)
                    somaValores += ((Money)item.Attributes["new_valor"]).Value;
            }

            var query1 = GetQueryExpression<Extrato>(true);
            query1.ColumnSet.AllColumns = true;
            query1.Criteria.Conditions.Add(new ConditionExpression("new_extrato_pagamento_ocorrenciaid", ConditionOperator.Equal, extratoid));
            DataCollection<Entity> result1 = base.Provider.RetrieveMultiple(query1).Entities;
            if (result1.Count > 0)
            {
                result1[0].Attributes["new_valor_lancamento"] = new Money(somaValores);
                if (result1[0].Attributes["new_valor_ordem_servico"] == null)
                {
                    result1[0].Attributes["new_valor_ordem_servico"] = new Money(0);
                }
                result1[0].Attributes["new_valor_extrato"] = new Money(result1[0].GetAttributeValue<Money>("new_valor_ordem_servico").Value + somaValores);
                base.Provider.Update(result1[0]);
            }
        }

        public List<T> ListarPor(StatusDaOcorrencia status)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)status));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(List<StatusDaOcorrencia> ListaStatus)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.FilterOperator = LogicalOperator.Or;
            foreach (StatusDaOcorrencia status in ListaStatus)
                query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, (int)status));
            query.Orders.Add(new OrderExpression("itbc_name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorAutorizada(Guid autorizadaId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.In, new string[] { "200042", "200043" })); //(200042) 9 - Aprovado(Compõe Extrato c\ Valor)  / (200043) - 10 - Reprovado (Compões Extrato 0) 
            query.Criteria.Conditions.Add(new ConditionExpression("new_valor_servico", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, autorizadaId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_extrato_pagamentoid", ConditionOperator.Null)); //Filtra pelos extratos vazios, pois haviam atividades pendentes que não permitem fechar a OS, deve ser fechada manualmente

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public void ReabrirOcorrencia(Ocorrencia ocorrencia)
        {
            this.AlterarStatus(ocorrencia.Id, 1);
        }

        public void FecharOcorrencia(Ocorrencia ocorrencia, SolucaoOcorrencia solucao)
        {
            Entity entity = new Entity("incidentresolution");
            entity.Attributes["subject"] = solucao.Nome;
            entity.Attributes["incidentid"] = new EntityReference("incident", solucao.OcorrenciaId);
            entity.Attributes["actualend"] = solucao.DataHoraConclusao;
            CloseIncidentRequest close = new CloseIncidentRequest();
            close.IncidentResolution = entity;
            close.Status = new OptionSetValue(5);
            var closeResponse = (CloseIncidentResponse)base.Provider.Execute(close);

            //Esse código abaixo Finaliza uma Ocorrencia
            //incidentresolution mysolution = new incidentresolution();
            //mysolution.incidentid = new Lookup();
            //mysolution.incidentid.Value = item.Id;
            //mysolution.statecode = new IncidentResolutionStateInfo();
            //mysolution.statecode.Value = IncidentResolutionState.Completed;
            //mysolution.statuscode = new Status();
            //mysolution.statuscode.Value = 6;
            //mysolution.statuscode.name = "Finalizada";

            //CloseIncidentRequest mycir = new CloseIncidentRequest();
            //mycir.IncidentResolution = mysolution;
            //mycir.Status = -1;
            //CloseIncidentResponse myresponse = (CloseIncidentResponse)base.Execute(mycir);
        }

        public void AtualizarStatusDosDiagnóstico(Ocorrencia ocorrencia, int novoStatusDoDiagnostico)
        {
            var query = GetQueryExpression<DiagnosticoOcorrencia>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorrencia.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotIn, new string[] { "7", novoStatusDoDiagnostico.ToString() })); // Substituido
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            foreach (var item in result)
            {
                SetStateRequest request = new SetStateRequest
                {
                    EntityMoniker = new EntityReference("new_diagnostico_ocorrencia", item.Id),
                    State = new OptionSetValue(0),
                    Status = new OptionSetValue(novoStatusDoDiagnostico)
                };

                SetStateResponse resp = (SetStateResponse)this.Execute(request);
            }
        }
        
        public Guid ObterIdEmailCorporativo(string identificador)
        {
            var emailDe = SDKore.Configuration.ConfigurationManager.GetSettingValue(identificador);
            return !string.IsNullOrEmpty(emailDe) ? new Guid(emailDe) : Guid.Empty;
        }

        public T ObterPor(Diagnostico servicoExecutado)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_diagnostico_ocorrencia", "incidentid", "new_ocorrenciaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.Equal, servicoExecutado.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public void ExcluirServicosRealizadosPor(Ocorrencia ocorrencia)
        {
            var query = new QueryExpression("new_diagnostico_ocorrencia");
            query.ColumnSet.AddColumn("new_diagnostico_ocorrenciaid");
            query.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorrencia.Id));

            foreach (var item in ocorrencia.Diagnosticos)
                query.Criteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.NotEqual, item.Id));

            var collection = base.Provider.RetrieveMultiple(query);

            foreach (Entity itemDel in collection.Entities)
                base.Provider.Delete("new_diagnostico_ocorrencia", itemDel.Id);
        }

        public List<T> ListarPor(Extrato extrato, Domain.Model.Conta assistenciaTecnica = null)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_extrato_pagamentoid", ConditionOperator.Equal, extrato.Id));
            if (assistenciaTecnica != null)
                query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, assistenciaTecnica.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(string[] diagnosticoIDs)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("new_diagnostico_ocorrencia", "incidentid", "new_ocorrenciaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.In, diagnosticoIDs));
            query.Distinct = true;
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorDiasDeReicidencia(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.NotEqual, ocorrencia.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("caseorigincode", ConditionOperator.Equal, 200004)); //Origem do Portal ASTEC
            query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, ocorrencia.Autorizada.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("productserialnumber", ConditionOperator.Equal, ocorrencia.ProdutosDoCliente));
            if (ocorrencia.Produto.LinhaComercial.NumeroDeDiasParaReincidencia.HasValue)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_origem", ConditionOperator.GreaterThan, DateTime.Now.AddDays(-1 * ocorrencia.Produto.LinhaComercial.NumeroDeDiasParaReincidencia.Value)));

            if (ocorrencia.Cliente != null && ocorrencia.Cliente.Id != Guid.Empty)
                query.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, ocorrencia.Cliente.Id));
            if (ocorrencia.ClienteOS != null && ocorrencia.ClienteOS.Id != Guid.Empty)
                query.Criteria.Conditions.Add(new ConditionExpression("customerid", ConditionOperator.Equal, ocorrencia.ClienteOS.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(HistoricoDePostagem historico)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("new_historico_postagem", "incidentid", "new_ocorrenciais");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_historico_postagemid", ConditionOperator.Equal, historico.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarPorPerfilUsuarioServico(Domain.Model.Contato contato, bool emAndamento)
        {
            var query = GetQueryExpression<T>(true);
            if (emAndamento)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_hora_conclusao", ConditionOperator.Null));
            query.AddLink("contract", "contractid", "contractid");
            query.LinkEntities[0].AddLink("new_permissao_usuario_servico", "contractid", "new_contratoid");
            query.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_contatoid", ConditionOperator.Equal, contato.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarParaIsol(Guid contratoId, List<int> status, DateTime inicioFechamento, DateTime fimFechamento)
        {
            var query = GetQueryExpression<T>(true);
            if (contratoId != Guid.Empty)
                query.Criteria.Conditions.Add(new ConditionExpression("contractid", ConditionOperator.Equal, contratoId));
            if (status.Count > 0)
                query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.In, status));
            if (inicioFechamento != DateTime.MinValue)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_hora_conclusao", ConditionOperator.GreaterEqual, inicioFechamento));
            if (fimFechamento != DateTime.MinValue)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_hora_conclusao", ConditionOperator.LessEqual, fimFechamento));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarParaIsol(Guid contratoId, Guid contatoId, List<int> status, DateTime inicioFechamento, DateTime fimFechamento, int pagina, int quantidade, ref bool existemMaisRegistros, ref string cookie, string numeroOcorrencia, bool exibirTodas, int tipoOcorrencia)
        {
            string fetch = @"<fetch mapping='logical' distinct='true'>
                              <entity name='incident'>
                                <attribute name='incidentid' />
                                <attribute name='customerid' />
                                <attribute name='contractid' />
                                <attribute name='new_data_origem' />
                                <order attribute='new_data_origem' descending='true'/>
                                <attribute name='new_os_cliente' />
                                <attribute name='ticketnumber' />
                                <attribute name='new_os_cliente' />
                                <attribute name='title' />
                                <attribute name='statuscode' />
                                <attribute name='new_data_hora_conclusao' />
                                <filter>
                                  {0}
                                  {1}
                                  {2}
                                  {3}
                                  {4}
                                  {5}
                                  {6}
                                </filter>
                                <link-entity name='contract' from='contractid' to='contractid'>
                                  <link-entity name='new_permissao_usuario_servico' from='new_contratoid' to='contractid'>
                                    <filter>
                                      <condition attribute='new_contatoid' operator='eq' value='{7}' />
                                    </filter>
                                  </link-entity>
                                </link-entity>
                                <link-entity name='account' from='accountid' to='accountid'>
                                  <attribute name='accountid' />
                                  <attribute name='name' />
                                  <attribute name='itbc_nomefantasia' />
                                </link-entity>
                              </entity>
                            </fetch>";

            string valor0 = string.Empty;
            if (status.Count > 0)
            {
                string interacoes = string.Empty;
                for (int i = 0; i < status.Count; i++)
                    interacoes += "<value>" + status[i] + "</value>";

                if (exibirTodas)
                {
                    valor0 = string.Format("<condition attribute='casetypecode' operator='in'>{0}</condition>", interacoes);
                }
                else
                {
                    valor0 = string.Format("<condition attribute='statuscode' operator='in'>{0}</condition>", interacoes);
                }
            }

            string valor1 = inicioFechamento == DateTime.MinValue ? string.Empty : string.Format(@"<condition attribute='new_data_hora_conclusao' operator='ge' value='{0}' />", inicioFechamento.ToString("yyyy-MM-dd HH:mm:ss"));
            string valor2 = fimFechamento == DateTime.MinValue ? string.Empty : string.Format(@"<condition attribute='new_data_hora_conclusao' operator='le' value='{0}' />", fimFechamento.ToString("yyyy-MM-dd HH:mm:ss"));
            string valor3 = contratoId == Guid.Empty ? string.Empty : string.Format(@"<condition attribute='contractid' operator='eq' value='{0}' />", contratoId);
            string valor4 = string.IsNullOrEmpty(numeroOcorrencia) ? string.Empty : string.Format(@"<condition attribute='ticketnumber' operator='like' value='%{0}%' />", numeroOcorrencia);
            string valor5 = string.Empty;

            if (exibirTodas)
            {
                valor5 = string.Format(@"<condition attribute='new_data_origem' operator='le' value='{0}' />", DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd HH:mm:ss"));
            }

            string valor6 = tipoOcorrencia == 0 ? string.Empty : string.Format(@"<condition attribute='casetypecode' operator='eq' value='{0}' />", tipoOcorrencia);

            fetch = string.Format(fetch, valor0, valor1, valor2, valor3, valor4, valor5, valor6, contatoId);

            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple = new RetrieveMultipleRequest();
            query.PageInfo = new PagingInfo();
            query.PageInfo.PageNumber = pagina;
            query.PageInfo.Count = quantidade;
            query.PageInfo.PagingCookie = cookie;
            retrieveMultiple.Query = new FetchExpression(fetch.ToString());

            var retorno = this.RetrieveMultiple(retrieveMultiple.Query);
            existemMaisRegistros = retorno.TotalRecordCountLimitExceeded;
            cookie = retorno.PagingCookie;

            return (List<T>)retorno.List;
        }

        public T ObterImpressaoOcorrenciaIsol(Guid id)
        {
            string fetch = string.Format(@"<fetch mapping='logical'>
                                              <entity name='incident'>
                                                <attribute name='casetypecode' />
                                                <attribute name='createdby' />
                                                <attribute name='description' />
                                                <attribute name='new_atividade_executada' />
                                                <attribute name='new_contato_visita' />
                                                <attribute name='new_data_hora_prevista_visita' />
                                                <attribute name='new_data_origem' />
                                                <attribute name='new_empresa_executanteid' />
                                                <attribute name='new_kilometragem_percorrida' />
                                                <attribute name='new_numero_nf_consumidor' />
                                                <attribute name='new_os_cliente' />
                                                <attribute name='new_solicitante_portal' />
                                                <attribute name='productserialnumber' />
                                                <attribute name='ticketnumber' />
                                                <attribute name='new_guid_endereco' />                                               
                                                <attribute name='customerid' />                                               
                                                <attribute name='contractid' />
                                                <attribute name='new_solicitanteid' />
                                                <attribute name='new_tecnico_visitaid' />
                                                <attribute name='new_tecnico_responsavelid' />
                                                <attribute name='itbc_reagendamento_visita' />
                                                <attribute name='ownerid' />
                                                <filter>
                                                 <condition attribute='incidentid' operator='eq' value='{0}' />
                                                </filter>
                                                <link-entity name='account' from='accountid' to='customerid' link-type='outer'>
                                                  <attribute name='accountid' />
                                                  <attribute name='name' />
                                                  <attribute name='itbc_nomefantasia' />
                                                  <attribute name='telephone1' />
                                                </link-entity>
                                                <link-entity name='contact' from='contactid' to='new_solicitanteid' link-type='outer'>
                                                  <attribute name='contactid' />
                                                  <attribute name='fullname' />
                                                  <attribute name='telephone1' />
                                                </link-entity>
                                                <link-entity name='contact' from='contactid' to='new_tecnico_visitaid' link-type='outer'>
                                                  <attribute name='contactid' />                                                  
                                                  <attribute name='fullname' />
                                                  <attribute name='new_rg' />
                                                  <attribute name='telephone1' />
                                                </link-entity>
                                                <link-entity name='contract' from='contractid' to='contractid' link-type='outer'>
                                                  <attribute name='contractid' />
                                                  <attribute name='contractnumber' />
                                                  <attribute name='contractlanguage' />
                                                </link-entity>
                                                <link-entity name='contact' from='contactid' to='new_tecnico_responsavelid' link-type='outer'>
                                                  <attribute name='contactid' />
                                                  <attribute name='fullname' />
                                                  <attribute name='new_rg' />
                                                  <attribute name='telephone1' />
                                                </link-entity>
                                                <link-entity name='customeraddress' from='customeraddressid' to='new_guid_endereco' link-type='outer'>
                                                  <attribute name='customeraddressid' />
                                                  <attribute name='city' />
                                                  <attribute name='line1' />
                                                  <attribute name='line2' />
                                                  <attribute name='line3' />
                                                  <attribute name='new_numero_endereco' />
                                                  <attribute name='postalcode' />
                                                  <attribute name='stateorprovince' />
                                                </link-entity>
                                              </entity>
                                            </fetch>", id);

            var query = GetQueryExpression<T>(true);
            RetrieveMultipleRequest retrieveMultiple = new RetrieveMultipleRequest();
            retrieveMultiple.Query = new FetchExpression(fetch.ToString());
            var colecao = this.RetrieveMultiple(retrieveMultiple.Query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterOcorrenciaPaiReincidente(string produtoSerial, Guid ignorarOcorrenciaId)
        {
            var query = GetQueryExpression<T>(true);

            if (!String.IsNullOrEmpty(produtoSerial))
                query.Criteria.Conditions.Add(new ConditionExpression("productserialnumber", ConditionOperator.Equal, produtoSerial));

            if (ignorarOcorrenciaId != Guid.Empty)
                query.Criteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.NotEqual, ignorarOcorrenciaId));

            query.Criteria.Conditions.Add(new ConditionExpression("casetypecode", ConditionOperator.Equal, 300009)); //Ordem de Serviço

            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotIn, new string[] { "200003", "6" })); //Cancelada

            query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.NotIn, SDKore.Configuration.ConfigurationManager.GetSettingValue("autorizadasIntelbras", true).Split(';')));

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[colecao.List.Count - 1];//Retornar o último registro
        }

        public void AlterarStatus(Guid ocorrenciaid, int status)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("incident", ocorrenciaid),
                State = new OptionSetValue(0),
                Status = new OptionSetValue(status)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public void Cancelar(Guid ocorrenciaid)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("incident", ocorrenciaid),
                State = new OptionSetValue(2),
                Status = new OptionSetValue(6)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        public DateTime ObterDataDeCriacaoDoReincidentePorDiagnostico(Guid diagnosticoid)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.AddLink("incident", "incidentid", "new_reincidenteid");
            query.LinkEntities[0].JoinOperator = JoinOperator.Natural;
            query.AddLink("new_diagnostico_ocorrencia", "incidentid", "new_ocorrenciaid");
            query.LinkEntities[1].JoinOperator = JoinOperator.Natural;
            query.LinkEntities[1].LinkCriteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.Equal, diagnosticoid));
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            if (result.Count > 0)
                return Convert.ToDateTime(result[0].Attributes["createdon"]);
            else
                return DateTime.MinValue;
        }

        private string[] ConvertArray(OrigemDaOcorrencia[] array)
        {
            string[] arrayFinal = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
                arrayFinal[i] = ((int)array[i]).ToString();

            return arrayFinal;
        }

        public List<T> ListarOcorrenciasTransportadoraPor(Domain.Model.Fatura notaFiscal)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("invoiceid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, notaFiscal.Id));
            query.AddLink("new_nota_ocorrncia", "ticketnumber", "new_numero_ocorrencia");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_nota_fiscalid", ConditionOperator.Equal, notaFiscal.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<ContratosAssociados> PesquisarClientesAssociadosPor(Guid contatoId)
        {
            var query = GetQueryExpression<ContratosAssociados>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contatoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contatoId));
            return (List<ContratosAssociados>)this.RetrieveMultiple(query).List;
        }

        public List<SolucaoOcorrencia> ListarSolucoesOcorrencia(Ocorrencia ocorrencia)
        {
            var query = GetQueryExpression<SolucaoOcorrencia>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("incidentid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, ocorrencia.Id));
            return (List<SolucaoOcorrencia>)this.RetrieveMultiple(query).List;
        }

        public List<Product> GerarLogisticaReversaDo(Domain.Model.Conta autorizada)
        {
            List<Product> logisticaReversa = new List<Product>();
            autorizada = (new Domain.Servicos.RepositoryService()).Conta.Retrieve(autorizada.Id);

            var queryHelper = new QueryExpression("new_diagnostico_ocorrencia");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.GreaterEqual, 5)); //Conserto Realizado
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.LessEqual, 7)); //Substituido
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotEqual, 6)); //Intervenção Técnica
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_geratroca", ConditionOperator.Equal, true));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_qtd_solicitada", ConditionOperator.GreaterThan, 0));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_qtd_solicitada", ConditionOperator.NotNull));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.NotNull));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_data_geracao_pedido", ConditionOperator.NotNull));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_regra_percentual_minimo", ConditionOperator.NotEqual, true));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.NotNull));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_extrato_logistica_reversaid", ConditionOperator.Null));

            queryHelper.AddLink("incident", "new_ocorrenciaid", "incidentid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, autorizada.Id));

            queryHelper.AddLink("product", "new_produtoid", "productid");
            queryHelper.LinkEntities[1].LinkCriteria.Conditions.Add(new ConditionExpression("new_logistica_reversa", ConditionOperator.Equal, true));

            DataCollection<Entity> result = base.Provider.RetrieveMultiple(queryHelper).Entities;
            foreach (var item in result)
            {
                bool jaTemLogisticaReversa = false;

                if (!jaTemLogisticaReversa)
                {
                    var produto = (new Domain.Servicos.RepositoryService()).Produto.Retrieve(((EntityReference)item["new_produtoid"]).Id);
                    if (produto != null)
                    {
                        produto.QuantidadeNaEstrutura = Convert.ToInt32(item["new_qtd_solicitada"]);
                        produto.QuantidadeMinimaMultipla = Convert.ToInt32(item["new_qtd_solicitada"]);
                        produto.ExportaERP = item.Id.ToString();
                        produto.LocalDeMontagem = Convert.ToString(item["new_nota_fiscal"]) + " / " + Convert.ToString(item["new_serie_nota_fiscal"]);
                        produto.NivelEstrutura = 0; //usado para Codigo do Estabelecimento
                        if (item.Attributes.Contains("itbc_estabelecimento"))
                        {
                            var estabelecimento = (new Domain.Servicos.RepositoryService()).Estabelecimento.Retrieve(((EntityReference)item["itbc_estabelecimento"]).Id);
                            if (estabelecimento != null)
                                produto.NivelEstrutura = estabelecimento.Codigo.Value;
                        }


                        decimal valorProduto = 0, aliquotaIPI = 0, valorIPI = 0, aluquotaICMS = 0, valorICMS = 0;

                        if (((OptionSetValue)item.Attributes["statuscode"]).Value == 7) //procura primeiro pelo item substituto para pegar seus valores - chamado CR810
                        {
                            var q0 = new QueryExpression("new_diagnostico_ocorrencia");
                            q0.ColumnSet.AllColumns = true;
                            q0.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ((EntityReference)item["new_ocorrenciaid"]).Id));
                            q0.Criteria.Conditions.Add(new ConditionExpression("new_produto_substitutoid", ConditionOperator.Equal, ((EntityReference)item["new_produtoid"]).Id));
                            q0.Criteria.Conditions.Add(new ConditionExpression("new_valor_unitario", ConditionOperator.NotNull));
                            q0.Criteria.Conditions.Add(new ConditionExpression("new_qtd_faturada", ConditionOperator.NotNull));
                            EntityCollection b0 = base.Provider.RetrieveMultiple(q0);

                            if (b0.Entities.Count > 0)
                            {
                                Entity diagSubstituto = b0.Entities[0];
                                valorProduto = ((Money)diagSubstituto["new_valor_unitario"]).Value;
                                if (diagSubstituto.Attributes.Contains("new_valor_ipi") && diagSubstituto.Attributes.Contains("new_aliquota_ipi"))
                                {
                                    valorIPI = ((Money)diagSubstituto["new_valor_ipi"]).Value;
                                    aliquotaIPI = Convert.ToDecimal(diagSubstituto["new_aliquota_ipi"]);
                                }
                                else if (diagSubstituto.Attributes.Contains("new_valor_ipi"))
                                {
                                    valorIPI = ((Money)diagSubstituto["new_valor_ipi"]).Value;
                                    aliquotaIPI = (((Money)diagSubstituto["new_valor_ipi"]).Value / (Convert.ToDecimal(diagSubstituto["new_qtd_faturada"]) * ((Money)diagSubstituto["new_valor_unitario"]).Value)) * 100;
                                }
                                else if (diagSubstituto.Attributes.Contains("new_aliquota_ipi"))
                                {
                                    aliquotaIPI = Convert.ToDecimal(diagSubstituto["new_aliquota_ipi"]);
                                    valorIPI = valorProduto * (aliquotaIPI / 100);
                                }

                                if (diagSubstituto.Attributes.Contains("new_valor_icms"))
                                {
                                    valorICMS = item.GetAttributeValue<Money>("new_valor_icms").Value;
                                    var ICMSBase = item.GetAttributeValue<Money>("new_valor_base_icms").Value;

                                    aluquotaICMS = (valorICMS / ICMSBase) * 100;
                                    aluquotaICMS = decimal.Round(aluquotaICMS, 0);
                                }
                                produto.LocalDeMontagem = Convert.ToString(diagSubstituto["new_nota_fiscal"]) + " / " + Convert.ToString(diagSubstituto["new_serie_nota_fiscal"]);
                                if (diagSubstituto.Attributes.Contains("itbc_estabelecimento"))
                                {
                                    var estabelecimento = (new Domain.Servicos.RepositoryService()).Estabelecimento.Retrieve(((EntityReference)diagSubstituto["itbc_estabelecimento"]).Id);
                                    if (estabelecimento != null)
                                        produto.NivelEstrutura = estabelecimento.Codigo.Value;
                                }
                            }

                        }

                        if (item.Attributes.Contains("new_valor_unitario") && valorProduto == 0)
                        {
                            valorProduto = ((Money)item["new_valor_unitario"]).Value;
                            if (item.Attributes.Contains("new_qtd_faturada"))
                            {
                                if (item.Attributes.Contains("new_valor_ipi") && item.Attributes.Contains("new_aliquota_ipi"))
                                {
                                    valorIPI = ((Money)item["new_valor_ipi"]).Value;
                                    aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                                }
                                else if (item.Attributes.Contains("new_valor_ipi"))
                                {
                                    valorIPI = ((Money)item["new_valor_ipi"]).Value;
                                    aliquotaIPI = (((Money)item["new_valor_ipi"]).Value / (Convert.ToDecimal(item["new_qtd_faturada"]) * ((Money)item["new_valor_unitario"]).Value)) * 100;
                                }
                                else if (item.Attributes.Contains("new_aliquota_ipi"))
                                {
                                    aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                                    valorIPI = valorProduto * (aliquotaIPI / 100);
                                }

                                if (item.Attributes.Contains("new_valor_icms"))
                                {
                                    valorICMS = item.GetAttributeValue<Money>("new_valor_icms").Value;
                                    var ICMSBase = item.GetAttributeValue<Money>("new_valor_base_icms").Value;

                                    aluquotaICMS = (valorICMS / ICMSBase) * 100;
                                    aluquotaICMS = decimal.Round(aluquotaICMS, 0);
                                }
                            }
                        }
                        else if (valorProduto == 0)
                        {
                            string un = "101";
                            if (produto.DadosFamiliaComercial != null && produto.LinhaComercial != null && produto.LinhaComercial.UnidadeDeNegocio != null)
                                un = produto.LinhaComercial.UnidadeDeNegocio.Nome;
                            BuscarPrecoItemAstec_ttErrosRow[] erros = null;

                            Domain.Servicos.HelperWS.IntelbrasService.BuscarPrecoItemAstec("101", Convert.ToInt32(autorizada.CodigoMatriz), produto.Codigo, "ASTEC 02", out valorProduto, out aliquotaIPI, out aluquotaICMS, out erros);
                            if (erros != null)
                                foreach (var erro in erros)
                                    produto.Descricao = produto.Descricao + " >> " + erro.mensagem;


                            valorIPI = valorProduto * (aliquotaIPI / 100);
                            valorICMS = valorProduto * (aluquotaICMS / 100);
                        }

                        if (valorProduto > 0)
                        {
                            produto.Preco = decimal.Round(valorProduto, 2);
                            produto.AliquotaIPI = decimal.Round(aliquotaIPI, 0);
                            produto.AliquotaICMS = decimal.Round(aluquotaICMS, 0);
                            produto.ValorIPI = decimal.Round(valorIPI, 2);
                            produto.ValorICMS = decimal.Round(valorICMS, 2);
                        }

                        var q2 = new QueryExpression("incident");
                        q2.ColumnSet.AllColumns = true;
                        q2.Criteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ((EntityReference)item["new_ocorrenciaid"]).Id));
                        var b2 = base.Provider.RetrieveMultiple(q2);
                        if (null != b2 && b2.Entities.Count > 0)
                        {
                            Entity os = b2.Entities[0];
                            produto.Descricao = "<a href=\"cadastroos.aspx?OcorrenciaId=" + os.Id.ToString() + "\" target=\"_blank\">" + Convert.ToString(os["ticketnumber"]) + "</a>";
                        }

                        logisticaReversa.Add(produto);
                    }
                }
            }

            return logisticaReversa;
        }

        public List<ProdutoBase> GerarLogisticaReversaDo(Domain.Model.Conta autorizada, Estabelecimento estabelecimento, bool naousar)
        {
            List<ProdutoBase> logisticaReversa = new List<ProdutoBase>();

            var queryHelper = new QueryExpression("new_diagnostico_ocorrencia");
            queryHelper.ColumnSet.AllColumns = true;

            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_extrato_logistica_reversaid", ConditionOperator.Null));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_qtd_faturada", ConditionOperator.GreaterThan, 0));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 5)); //conserto realizado
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_nota_fiscal", ConditionOperator.NotNull));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("itbc_estabelecimento", ConditionOperator.Equal, estabelecimento.Id));

            var link1 = queryHelper.AddLink("incident", "new_ocorrenciaid", "incidentid");
            link1.EntityAlias = "i";
            link1.LinkCriteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, autorizada.Id));
            link1.Columns.AddColumns(new string[] { "ticketnumber" });

            link1.AddLink("account", "new_autorizadaid", "accountid");
            link1.LinkEntities[0].EntityAlias = "a";
            link1.LinkEntities[0].Columns.AddColumns(new string[] { "accountnumber" });

            var link2 = queryHelper.AddLink("product", "new_produtoid", "productid");
            link2.EntityAlias = "p";
            link2.LinkCriteria.Conditions.Add(new ConditionExpression("new_logistica_reversa", ConditionOperator.Equal, true));
            link2.Columns.AddColumns(new string[] { "name", "productnumber" });

            var link3 = link2.AddLink("new_linha_unidade_negocio", "itbc_linha_unidade_negocioid", "new_linha_unidade_negocioid", JoinOperator.LeftOuter);
            link3.EntityAlias = "u";
            link3.Columns.AddColumn("new_name");

            queryHelper.Distinct = true;
            var bec = base.Provider.RetrieveMultiple(queryHelper);

            foreach (Entity item in bec.Entities)
            {
                ProdutoBase produto = new ProdutoBase();
                produto.Id = ((EntityReference)item["new_produtoid"]).Id;
                produto.ID = ((EntityReference)item["new_produtoid"]).Id;
                produto.Nome = ((AliasedValue)item["p.name"]).Value.ToString();
                produto.CodigoEms = ((AliasedValue)item["p.productnumber"]).Value.ToString();
                produto.Codigo = ((AliasedValue)item["p.productnumber"]).Value.ToString();
                produto.QuantidadeNaEstrutura = Convert.ToInt32(item["new_qtd_faturada"]);
                produto.QuantidadeMinimaMultipla = Convert.ToInt32(item["new_qtd_faturada"]);
                produto.ExportaERP = item.Id.ToString();
                produto.LocalDeMontagem = Convert.ToString(item["new_nota_fiscal"]) + " / " + Convert.ToString(item["new_serie_nota_fiscal"]);
                produto.NivelEstrutura = estabelecimento.Codigo.Value;

                decimal valorProduto = 0, aliquotaIPI = 0, valorIPI = 0, aluquotaICMS = 0, valorICMS = 0;

                if (item.Attributes.Contains("new_valor_unitario") && valorProduto == 0)
                {
                    valorProduto = ((Money)item["new_valor_unitario"]).Value;
                    if (item.Attributes.Contains("new_qtd_faturada"))
                    {
                        if (item.Attributes.Contains("new_valor_ipi") && item.Attributes.Contains("new_aliquota_ipi"))
                        {
                            valorIPI = ((Money)item["new_valor_ipi"]).Value;
                            aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                        }
                        else if (item.Attributes.Contains("new_valor_ipi"))
                        {
                            valorIPI = ((Money)item["new_valor_ipi"]).Value;
                            aliquotaIPI = (valorIPI / (Convert.ToDecimal(item["new_qtd_faturada"]) * ((Money)item["new_valor_unitario"]).Value)) * 100;
                        }
                        else if (item.Attributes.Contains("new_aliquota_ipi"))
                        {
                            aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                            valorIPI = valorProduto * (aliquotaIPI / 100);
                        }

                        if (item.Attributes.Contains("new_valor_icms"))
                        {
                            if (!item.Attributes.Contains("new_valor_base_icms"))
                                throw new ArgumentException("O campo Valor ICMS Base do produto " + produto.Codigo + " - " + produto.Nome + " não pode ter valor nulo!");

                            if (item.GetAttributeValue<Money>("new_valor_base_icms").Value == 0)
                                throw new ArgumentException("O campo Valor ICMS Base do produto " + produto.Codigo + " - " + produto.Nome + " não pode ter valor zero!");

                            valorICMS = item.GetAttributeValue<Money>("new_valor_icms").Value;
                            var ICMSBase = item.GetAttributeValue<Money>("new_valor_base_icms").Value;

                            aluquotaICMS = (valorICMS / ICMSBase) * 100;
                            aluquotaICMS = decimal.Round(aluquotaICMS, 0);
                        }
                    }
                }
                else if (valorProduto == 0)
                {
                    string un = "101";
                    if (item["u.new_name"] != null && item["u.new_name"].ToString() != "")
                        un = ((AliasedValue)item["u.new_name"]).Value.ToString();
                    BuscarPrecoItemAstec_ttErrosRow[] erros = null;

                    Domain.Servicos.HelperWS.IntelbrasService.BuscarPrecoItemAstec("101", Convert.ToInt32(item["a.accountnumber"].ToString()), produto.Codigo, "ASTEC 02", out valorProduto, out aliquotaIPI, out aluquotaICMS, out erros);
                    if (erros != null)
                        foreach (var erro in erros)
                            produto.Descricao = produto.Descricao + " >> " + erro.mensagem;

                    valorIPI = valorProduto * (aliquotaIPI / 100);
                    valorICMS = valorProduto * (aluquotaICMS / 100);
                }

                if (valorProduto > 0)
                {
                    produto.Preco = decimal.Round(valorProduto, 2);
                    produto.AliquotaIPI = decimal.Round(aliquotaIPI, 0);
                    produto.AliquotaICMS = decimal.Round(aluquotaICMS, 0);
                    produto.ValorIPI = decimal.Round(valorIPI, 2);
                    produto.ValorICMS = decimal.Round(valorICMS, 2);
                }

                if (item["new_ocorrenciaid"] != null)
                {
                    produto.Descricao = "<a href=\"cadastroos.aspx?OcorrenciaId=" + ((EntityReference)item["new_ocorrenciaid"]).Id.ToString() + "\" target=\"_blank\">" + ((AliasedValue)item["i.ticketnumber"]).Value + "</a>";
                }
                logisticaReversa.Add(produto);
            }

            return logisticaReversa;
        }

        private QueryExpression MontaConsultaDiagnosticosLGR(Guid EstabelecimentoId, Guid AutorizadaId, bool Substituto)
        {
            var queryHelper = GetQueryExpression<DiagnosticoOcorrencia>(true);
            queryHelper.ColumnSet.AddColumns(new string[] { "new_produtoid", "new_qtd_solicitada", "new_diagnostico_ocorrenciaid", "new_nota_fiscal", "new_serie_nota_fiscal", "new_valor_unitario", "new_qtd_faturada", "new_valor_ipi", "new_aliquota_ipi", "new_valor_icms", "statuscode", "new_ocorrenciaid" });

            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 5));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_extrato_logistica_reversaid", ConditionOperator.Null));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_estabelecimentoid", ConditionOperator.Equal, EstabelecimentoId));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_regra_percentual_minimo", ConditionOperator.NotEqual, true));

            queryHelper.AddLink("incident", "new_ocorrenciaid", "incidentid");
            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, AutorizadaId));

            queryHelper.AddLink("product", "new_produtoid", "productid");
            queryHelper.LinkEntities[1].LinkCriteria.Conditions.Add(new ConditionExpression("new_logistica_reversa", ConditionOperator.Equal, true));

            if (Substituto)
            {
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_qtd_solicitada", ConditionOperator.Equal, 0));

                queryHelper.AddLink("new_diagnostico_ocorrencia", "new_diagnostico_paiid", "new_diagnostico_ocorrenciaid");
                queryHelper.LinkEntities[2].LinkCriteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, 7));
                queryHelper.LinkEntities[2].LinkCriteria.Conditions.Add(new ConditionExpression("new_geratroca", ConditionOperator.Equal, true));
                queryHelper.LinkEntities[2].LinkCriteria.Conditions.Add(new ConditionExpression("new_qtd_solicitada", ConditionOperator.GreaterThan, 0));
                queryHelper.LinkEntities[2].LinkCriteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.NotNull));
                queryHelper.LinkEntities[2].LinkCriteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.NotNull));
                queryHelper.LinkEntities[2].LinkCriteria.Conditions.Add(new ConditionExpression("new_data_geracao_pedido", ConditionOperator.NotNull));
            }
            else
            {
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_geratroca", ConditionOperator.Equal, true));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_qtd_solicitada", ConditionOperator.GreaterThan, 0));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_data_geracao_pedido", ConditionOperator.NotNull));
            }

            return queryHelper;
        }

        public List<Product> GerarLogisticaReversaDo(Domain.Model.Conta autorizada, Estabelecimento estabelecimento)
        {
            List<Product> logisticaReversa = new List<Product>();

            autorizada = (new Domain.Servicos.RepositoryService()).Conta.Retrieve(autorizada.Id);

            #region Itens no status Conserto Realizado sem substitutos
            var bec = base.Provider.RetrieveMultiple(MontaConsultaDiagnosticosLGR(estabelecimento.Id, autorizada.Id, false));

            foreach (Entity item in bec.Entities)
            {
                var produto = (new Domain.Servicos.RepositoryService()).Produto.Retrieve(((EntityReference)item["new_produtoid"]).Id);
                if (produto != null)
                {
                    if (!item.Attributes.Contains("new_qtd_faturada"))
                    {
                        throw new ArgumentException("Quantidade faturada não pode ser vazia, entre em contato com a Intelbras.");
                    }
                    else if (Convert.ToDecimal(item["new_qtd_faturada"]) == 0)
                    {
                        throw new ArgumentException("Quantidade faturada precisa ser maior que zero, entre em contato com a Intelbras.");
                    }

                    produto.QuantidadeNaEstrutura = Convert.ToInt32(item["new_qtd_solicitada"]);
                    produto.QuantidadeMinimaMultipla = Convert.ToInt32(item["new_qtd_solicitada"]);
                    produto.ExportaERP = item.Id.ToString();
                    produto.LocalDeMontagem = Convert.ToString(item["new_nota_fiscal"]) + " / " + Convert.ToString(item["new_serie_nota_fiscal"]);
                    produto.NivelEstrutura = estabelecimento.Codigo.Value;

                    decimal valorProduto = 0, aliquotaIPI = 0, valorIPI = 0, aluquotaICMS = 0, valorICMS = 0;

                    if (item.Attributes.Contains("new_valor_unitario"))
                    {
                        valorProduto = ((Money)item["new_valor_unitario"]).Value;
                        if (item.Attributes.Contains("new_qtd_faturada"))
                        {
                            if (item.Attributes.Contains("new_valor_ipi") && item.Attributes.Contains("new_aliquota_ipi"))
                            {
                                valorIPI = ((Money)item["new_valor_ipi"]).Value;
                                aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                            }
                            else if (item.Attributes.Contains("new_valor_ipi"))
                            {
                                valorIPI = ((Money)item["new_valor_ipi"]).Value;
                                aliquotaIPI = (valorIPI / (Convert.ToDecimal(item["new_qtd_faturada"]) * ((Money)item["new_valor_unitario"]).Value)) * 100;
                            }
                            else if (item.Attributes.Contains("new_aliquota_ipi"))
                            {
                                aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                                valorIPI = valorProduto * (aliquotaIPI / 100);
                            }

                            if (item.Attributes.Contains("new_valor_icms"))
                            {
                                valorICMS = item.GetAttributeValue<Money>("new_valor_icms").Value;
                                var ICMSBase = item.GetAttributeValue<Money>("new_valor_base_icms").Value;

                                aluquotaICMS = (valorICMS / ICMSBase) * 100;
                                aluquotaICMS = decimal.Round(aluquotaICMS, 0);
                            }
                        }
                    }
                    else
                    {
                        string un = "101";
                        if (produto.DadosFamiliaComercial != null && produto.LinhaComercial != null && produto.LinhaComercial.UnidadeDeNegocio != null)
                            un = produto.LinhaComercial.UnidadeDeNegocio.Nome;
                        BuscarPrecoItemAstec_ttErrosRow[] erros = null;

                        Domain.Servicos.HelperWS.IntelbrasService.BuscarPrecoItemAstec("101", Convert.ToInt32(autorizada.CodigoMatriz), produto.Codigo, "ASTEC 02", out valorProduto, out aliquotaIPI, out aluquotaICMS, out erros);
                        if (erros != null)
                            foreach (var erro in erros)
                                produto.Descricao = produto.Descricao + " >> " + erro.mensagem;


                        valorIPI = valorProduto * (aliquotaIPI / 100);
                        valorICMS = valorProduto * (aluquotaICMS / 100);
                    }

                    if (valorProduto > 0)
                    {
                        produto.Preco = decimal.Round(valorProduto, 2);
                        produto.AliquotaIPI = decimal.Round(aliquotaIPI, 0);
                        produto.AliquotaICMS = decimal.Round(aluquotaICMS, 0);
                        produto.ValorIPI = decimal.Round(valorIPI, 2);
                        produto.ValorICMS = decimal.Round(valorICMS, 2);
                    }


                    var q2 = new QueryExpression("incident");
                    q2.ColumnSet.AllColumns = true;
                    q2.Criteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ((EntityReference)item["new_ocorrenciaid"]).Id));
                    var b2 = base.Provider.RetrieveMultiple(q2);
                    if (null != b2 && b2.Entities.Count > 0)
                    {
                        Entity os = b2.Entities[0];
                        produto.Descricao = "<a href=\"cadastroos.aspx?OcorrenciaId=" + os.Id.ToString() + "\" target=\"_blank\">" + Convert.ToString(os["ticketnumber"]) + "</a>";
                    }
                    logisticaReversa.Add(produto);
                }
            }
            #endregion

            #region Itens no status Conserto Realizado com substitutos
            bec = base.Provider.RetrieveMultiple(MontaConsultaDiagnosticosLGR(estabelecimento.Id, autorizada.Id, true));

            foreach (Entity item in bec.Entities)
            {
                var produto = (new Domain.Servicos.RepositoryService()).Produto.Retrieve(((EntityReference)item["new_produtoid"]).Id);
                if (produto != null)
                {
                    produto.QuantidadeNaEstrutura = Convert.ToInt32(item["new_qtd_solicitada"]);
                    produto.QuantidadeMinimaMultipla = Convert.ToInt32(item["new_qtd_solicitada"]);
                    produto.ExportaERP = item.Id.ToString();
                    produto.LocalDeMontagem = Convert.ToString(item["new_nota_fiscal"]) + " / " + Convert.ToString(item["new_serie_nota_fiscal"]);
                    produto.NivelEstrutura = estabelecimento.Codigo.Value;

                    decimal valorProduto = 0, aliquotaIPI = 0, valorIPI = 0, aluquotaICMS = 0, valorICMS = 0;

                    if (item.Attributes.Contains("new_valor_unitario") && valorProduto == 0)
                    {
                        valorProduto = ((Money)item["new_valor_unitario"]).Value;
                        if (item.Attributes.Contains("new_qtd_faturada"))
                        {
                            if (item.Attributes.Contains("new_valor_ipi") && item.Attributes.Contains("new_aliquota_ipi"))
                            {
                                valorIPI = ((Money)item["new_valor_ipi"]).Value;
                                aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                            }
                            else if (item.Attributes.Contains("new_valor_ipi"))
                            {
                                valorIPI = ((Money)item["new_valor_ipi"]).Value;
                                aliquotaIPI = (valorIPI / (Convert.ToDecimal(item["new_qtd_faturada"]) * ((Money)item["new_valor_unitario"]).Value)) * 100;
                            }
                            else if (item.Attributes.Contains("new_aliquota_ipi"))
                            {
                                aliquotaIPI = Convert.ToDecimal(item["new_aliquota_ipi"]);
                                valorIPI = valorProduto * (aliquotaIPI / 100);
                            }

                            if (item.Attributes.Contains("new_valor_icms"))
                            {
                                valorICMS = item.GetAttributeValue<Money>("new_valor_icms").Value;
                                var ICMSBase = item.GetAttributeValue<Money>("new_valor_base_icms").Value;

                                aluquotaICMS = (valorICMS / ICMSBase) * 100;
                                aluquotaICMS = decimal.Round(aluquotaICMS, 0);
                            }
                        }
                    }
                    else if (valorProduto == 0)
                    {

                        string un = "101";
                        if (produto.DadosFamiliaComercial != null && produto.LinhaComercial != null && produto.LinhaComercial.UnidadeDeNegocio != null)
                            un = produto.LinhaComercial.UnidadeDeNegocio.Nome;
                        BuscarPrecoItemAstec_ttErrosRow[] erros = null;

                        Domain.Servicos.HelperWS.IntelbrasService.BuscarPrecoItemAstec("101", Convert.ToInt32(autorizada.CodigoMatriz), produto.Codigo, "ASTEC 02", out valorProduto, out aliquotaIPI, out aluquotaICMS, out erros);
                        if (erros != null)
                            foreach (var erro in erros)
                                produto.Descricao = produto.Descricao + " >> " + erro.mensagem;


                        valorIPI = valorProduto * (aliquotaIPI / 100);
                        valorICMS = valorProduto * (aluquotaICMS / 100);
                    }

                    if (valorProduto > 0)
                    {
                        produto.Preco = decimal.Round(valorProduto, 2);
                        produto.AliquotaIPI = decimal.Round(aliquotaIPI, 0);
                        produto.AliquotaICMS = decimal.Round(aluquotaICMS, 0);
                        produto.ValorIPI = decimal.Round(valorIPI, 2);
                        produto.ValorICMS = decimal.Round(valorICMS, 2);
                    }

                    var q2 = new QueryExpression("incident");
                    q2.ColumnSet.AllColumns = true;
                    q2.Criteria.Conditions.Add(new ConditionExpression("incidentid", ConditionOperator.Equal, ((EntityReference)item["new_ocorrenciaid"]).Id));
                    var b2 = base.Provider.RetrieveMultiple(q2);
                    if (null != b2 && b2.Entities.Count > 0)
                    {
                        Entity os = b2.Entities[0];
                        produto.Descricao = "<a href=\"cadastroos.aspx?OcorrenciaId=" + os.Id.ToString() + "\" target=\"_blank\">" + Convert.ToString(os["ticketnumber"]) + "</a>";
                    }
                    logisticaReversa.Add(produto);
                }
            }
            #endregion

            return logisticaReversa;
        }

        public List<ProdutoBase> ObterLogisticaReversaPor(Guid extratoId)
        {
            List<ProdutoBase> logisticaReversa = new List<ProdutoBase>();
            var query = GetQueryExpression<Diagnostico>(true);
            query.ColumnSet.AllColumns = true;
            query.AddLink("product", "new_produtoid", "productid");
            query.LinkEntities[0].EntityAlias = "p";
            query.LinkEntities[0].Columns.AddColumns(new string[] { "name", "productnumber" });
            query.Criteria.Conditions.Add(new ConditionExpression("new_extrato_logistica_reversaid", ConditionOperator.Equal, extratoId));
            query.AddLink("incident", "new_ocorrenciaid", "incidentid");
            query.LinkEntities[1].EntityAlias = "i";
            query.LinkEntities[1].Columns.AddColumns(new string[] { "ticketnumber" });


            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            foreach (var item in result)
            {
                ProdutoBase produto = new ProdutoBase();
                produto.Id = ((EntityReference)item["new_produtoid"]).Id;
                produto.ID = ((EntityReference)item["new_produtoid"]).Id;
                produto.Nome = ((AliasedValue)item["p.name"]).Value.ToString();
                produto.CodigoEms = ((AliasedValue)item["p.productnumber"]).Value.ToString();
                produto.Codigo = ((AliasedValue)item["p.productnumber"]).Value.ToString();
                produto.QuantidadeNaEstrutura = (item.Attributes["new_qtd_faturada"] == null) ? 0 : Convert.ToInt32(item.Attributes["new_qtd_faturada"]);
                produto.QuantidadeMinimaMultipla = (item.Attributes["new_qtd_solicitada"] == null) ? 0 : Convert.ToInt32(item.Attributes["new_qtd_solicitada"]);
                produto.ExportaERP = extratoId.ToString();
                produto.LocalDeMontagem = Convert.ToString(item.Attributes["new_nota_fiscal"]) + " / " + Convert.ToString(item.Attributes["new_serie_nota_fiscal"]);

                if (item.Attributes["new_valor_unitario"] != null)
                    produto.Preco = item.GetAttributeValue<Money>("new_valor_unitario").Value;

                if (item.Attributes["new_aliquota_ipi"] != null)
                    produto.AliquotaIPI = decimal.Round(Convert.ToDecimal(item.Attributes["new_aliquota_ipi"]), 0);

                if (item.Attributes["new_valor_ipi"] != null)
                    produto.ValorIPI = item.GetAttributeValue<Money>("new_valor_ipi").Value;

                if (item.Attributes["new_valor_icms"] != null)
                {
                    produto.ValorICMS = item.GetAttributeValue<Money>("new_valor_icms").Value;
                    var ICMSBase = item.GetAttributeValue<Money>("new_valor_base_icms").Value;

                    produto.AliquotaICMS = (produto.ValorICMS / ICMSBase) * 100;
                    produto.AliquotaICMS = decimal.Round(produto.AliquotaICMS, 0);
                }

                if (item["new_ocorrenciaid"] != null)
                {
                    produto.Descricao = "<a href=\"cadastroos.aspx?OcorrenciaId=" + Convert.ToString(((EntityReference)item["new_ocorrenciaid"]).Id) + "\" target=\"_blank\">" + Convert.ToString(((AliasedValue)item["i.ticketnumber"]).Value) + "</a>";
                }
                logisticaReversa.Add(produto);

            }
            return logisticaReversa;
        }

        public DataTable ListarMenorDataSolucaoOcorrencia()
        {
            string commandIn = string.Empty;
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select IR.IncidentId, Min(actualend) actualend from IncidentResolution IR ");
            strSql.Append("inner join incident I on IR.IncidentId = I.IncidentId and I.StateCode = 1 ");
            strSql.Append("and itbc_data_hora_solucao_cliente is null and I.CaseTypeCode in (300005, ");
            strSql.Append("300000, 300002, 300013, 300008, 300010, 300006, 300003, 300015, 300007, 300017)");
            strSql.Append("group by IR.IncidentId ");

            return DataBaseSqlServer.executeQuery(strSql.ToString());
        }
        public List<T> ListarOcorrenciasPorDataCriacao(DateTime? dataCriacaoInicio, DateTime? dataCriacaoFim)
        {

            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("modifiedon", ConditionOperator.GreaterEqual, dataCriacaoInicio));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("modifiedon", ConditionOperator.LessEqual, dataCriacaoFim));

            FilterExpression queryOr = new FilterExpression(LogicalOperator.Or);
            queryOr.Conditions.Add(new ConditionExpression("new_empresa_executanteid", ConditionOperator.NotNull));
            queryOr.Conditions.Add(new ConditionExpression("itbc_atualizar_operacoes_suporte", ConditionOperator.Equal, true));
            
            query.Criteria.AddFilter(queryOr);

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarOcorrenciasPorDataModificacao()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("modifiedon", ConditionOperator.LessEqual, DateTime.Now.AddDays(-5));                        
            query.Criteria.AddCondition("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.NotIn, new string[] { "200003", "200004", "5", "6" });
            query.Criteria.AddCondition("casetypecode", ConditionOperator.In, "200000", "200001", "200002", "200003", "200004", "200005", "200006", "200007", "200008", "200090", "200091", "200092", "200093", "200094", "200095", "200096");

            return (List<T>)this.RetrieveMultiple(query).List;
        }        

        public List<T> ListarOcorrenciasRecalculaSLA(Feriado feriado)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("createdon", ConditionOperator.GreaterEqual, feriado.Data.AddDays(-7).ToString("MM/dd/yyyy")));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("casetypecode", ConditionOperator.In, 200000, 200001, 200002, 200003, 200004, 200005, 200006, 200007, 200008, 200090, 200091, 200092, 200093, 200094, 200095, 200096));
            if (feriado.Cidade != null)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_cidade", ConditionOperator.Equal, feriado.Cidade));
            if (feriado.Uf != null)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_uf", ConditionOperator.Equal, feriado.Uf));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("prioritycode", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_localidadeid", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("contractid", ConditionOperator.NotNull));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> BuscarOcorrenciaPorProtocoloChat(string protocolo)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_protocolo_chat", ConditionOperator.Equal, protocolo));
            query.Criteria.Conditions.Add(new ConditionExpression("itbc_status_atendimento_chat", ConditionOperator.Null));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorProtocoloTelefonico(string protocoloTelefonico)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_protocolo_telefonico", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, protocoloTelefonico));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarOcorrenciasPorNumeroSerie(string numeroSerie)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("productserialnumber", ConditionOperator.Equal, numeroSerie);
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOcorrenciasPorLinhaDoContrato(Guid linhaDeContratoId)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.AddCondition("contractdetailid", ConditionOperator.Equal, linhaDeContratoId);
            query.Criteria.AddCondition("statuscode", ConditionOperator.NotEqual, "200003");

            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}