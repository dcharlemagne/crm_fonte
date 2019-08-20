using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Application.SiteIntelbras
{
    class Program
    {
        private ClassificacaoLog ClassificacaoLog { get { return ClassificacaoLog.ServicoSiteIntelbras; } }
        private bool CriaLogInfomarion { get { return true; } }

        const string messageErro = "Erro recebendo resposta. Verifique os dados da excessão interna para maiores detalhes.";

        private RestClient ClienteRest;
        private string csrf_token;
        private string token;

        public void CriarSessao()
        {
            ClienteRest = new RestClient();
            ClienteRest.BaseUrl = new Uri(SDKore.Configuration.ConfigurationManager.GetSettingValue("IntegradorSiteURLWebService"));
            ClienteRest.CookieContainer = new System.Net.CookieContainer();

            Autentica(SDKore.Configuration.ConfigurationManager.GetSettingValue("IntegradorSiteUsuarioWebService"), SDKore.Configuration.ConfigurationManager.GetSettingValue("IntegradorSiteSenhaWebService"));
        }

        public int Executar(string[] arrayCodigoCliente)
        {
            List<Domain.Model.Conta> lista = new List<Domain.Model.Conta>();
            foreach (string codigo in arrayCodigoCliente)
                lista.Add((new Domain.Servicos.RepositoryService()).Conta.ObterPorCodigoEmitente(codigo));

            Executar(lista);

            return lista.Count;
        }

        static int Main(string[] args)
        {
            int dias = 3;
            if (args != null && args.Length > 1)
                dias = Convert.ToInt32(args[0]);
            var retorno = new Program().Executar(dias);

            return retorno;
        }
        public int Executar(int dias)
        {
            CriarSessao();
           
            List<Domain.Model.Conta> lista = (new Domain.Servicos.RepositoryService()).Conta.ObterAutorizadas(-1 * dias);
            
            Executar(lista);

            return lista.Count;
        }

        private void Executar(List<Domain.Model.Conta> lista)
        {
            foreach (CRM2013.Domain.Model.Conta assistenciaTecnica in lista)
                try
                {
                    // Verifica se a Conta É Divulgada no Site e Posto de Serviço = TRUE
                    if (assistenciaTecnica.DivulgadaNoSite.HasValue
                        && assistenciaTecnica.DivulgadaNoSite.Value
                        && assistenciaTecnica.AcessoPortalASTEC.HasValue
                        && assistenciaTecnica.AcessoPortalASTEC.Value)
                    {
                        CreateLog("Criando Requisição para ATUALIZACAO assistencia: " + assistenciaTecnica.CodigoMatriz);
                        SalvaAssistenciaTecnica(assistenciaTecnica);
                        CreateLog("Requisição de ATUALIZACAO enviada com sucesso, assistencia: " + assistenciaTecnica.CodigoMatriz);                        
                    }
                    else
                    {
                        CreateLog("Criando Requisição para EXCLUIR assistencia: " + assistenciaTecnica.CodigoMatriz);
                      //  ExcluiAssistenciaTecnica(assistenciaTecnica.CodigoMatriz);
                        CreateLog("Requisição de EXCLUSAO enviada com sucesso, assistencia: " + assistenciaTecnica.CodigoMatriz);
                    }
                }
                catch (Exception ex) { SDKore.Helper.Error.Create(ex, System.Diagnostics.EventLogEntryType.Error); }
        }

        private void SalvaAssistenciaTecnica(Domain.Model.Conta assistencia)
        {
            StringBuilder sb = null;
            StringWriter sw = null;
            JsonWriter wt = null;

            try
            {
                var reqInserir = new RestRequest();

                reqInserir.AddHeader("Content-Type", "application/json");
                reqInserir.AddHeader("Accept", "application/json");
                reqInserir.AddHeader("accept-language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4,es;q=0.2");
                reqInserir.AddHeader("Cookie", token);
                reqInserir.AddHeader("X-CSRF-Token", csrf_token);
                reqInserir.Resource = "/api/auth/technical-support/{crm_id}";
                reqInserir.Method = Method.PUT;
                reqInserir.RequestFormat = DataFormat.Json;
                reqInserir.AddParameter("crm_id", assistencia.CodigoMatriz, ParameterType.UrlSegment);

                reqInserir.OnBeforeDeserialization = resp =>
                {
                    resp.ContentType = "application/json";
                };

                sb = new StringBuilder();
                sw = new StringWriter(sb);

                wt = new JsonTextWriter(sw);

                wt.Formatting = Formatting.Indented;
                wt.WriteStartObject();
                wt.WritePropertyName("title");
                wt.WriteValue(assistencia.NomeFantasia);

                wt.WritePropertyName("field_email");
                wt.WriteStartObject();
                wt.WritePropertyName("und");
                wt.WriteStartArray();
                wt.WriteStartObject();
                wt.WritePropertyName("email");
                wt.WriteValue(assistencia.Email);
                wt.WriteEndObject();
                wt.WriteEndArray();
                wt.WriteEndObject();

                wt.WritePropertyName("field_address");
                wt.WriteStartObject();
                wt.WritePropertyName("und");
                wt.WriteStartArray();
                wt.WriteStartObject();
                wt.WritePropertyName("organisation_name");
                wt.WriteValue(assistencia.Nome);
                wt.WritePropertyName("country");
                wt.WriteValue("BR");
                wt.WritePropertyName("administrative_area");
                var estado = (new Domain.Servicos.RepositoryService()).Estado.ObterPor(assistencia.Endereco1Estadoid.Id);
                wt.WriteValue(estado.SiglaUF);

                wt.WritePropertyName("locality");
                wt.WriteValue(assistencia.Endereco1Municipioid.Name);
                wt.WritePropertyName("postal_code");
                wt.WriteValue(assistencia.Endereco1CEP);
                wt.WritePropertyName("thoroughfare");
                wt.WriteValue(assistencia.Endereco1Rua + ", " + assistencia.Endereco1Numero + " " + assistencia.Endereco1Complemento);
                wt.WritePropertyName("premise");
                wt.WriteValue(assistencia.Endereco1Bairro);
                wt.WritePropertyName("phone_number");
                wt.WriteValue(assistencia.Telefone);
                wt.WriteEndObject();
                wt.WriteEndArray();
                wt.WriteEndObject();

                #region products_related

                List<Product> produtos = ObterProtutos(assistencia);

                wt.WritePropertyName("products_related");
                wt.WriteStartArray();


                foreach (Product item in produtos.Take(10000))
                    wt.WriteValue(item.Codigo);

                wt.WriteEnd();

                #endregion

                wt.WriteEndObject();

                reqInserir.AddParameter("application/json", sw.ToString(), ParameterType.RequestBody);

                CreateLog("Enviado JSON de ATUALIZACAO \n" + sw.ToString());

                var resultado = ClienteRest.Execute<RespostaPut>(reqInserir);


                if (resultado == null)
                    throw new ApplicationException("[Incluir Assistencia Tecnica] Mensagem de Retorno esta vazia!");

                if (resultado.ErrorException != null)
                    throw new ApplicationException("[Incluir Assistencia Tecnica] Mensagem de Retorno: " + resultado.ErrorException
                                                  + " \nRequest: " + sw.ToString());
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            finally
            {
                sb = null;

                wt.Close();

                sw.Close();
                sw.Dispose();
            }
        }

        private void ExcluiAssistenciaTecnica(string codigoPosto)
        {
            var reqRemover = new RestRequest();

            reqRemover.AddHeader("Content-Type", "application/json");
            reqRemover.AddHeader("Accept", "application/json");
            reqRemover.AddHeader("X-CSRF-Token", csrf_token);
            reqRemover.Resource = "/api/auth/technical-support/{crm_id}";
            reqRemover.Method = Method.DELETE;
            reqRemover.RequestFormat = DataFormat.Json;
            reqRemover.AddParameter("crm_id", codigoPosto, ParameterType.UrlSegment);

            reqRemover.OnBeforeDeserialization = resp =>
            {
                resp.ContentType = "application/json";
            };

            var resultado = ClienteRest.Execute(reqRemover);

            if (resultado.ErrorException != null)
                throw new ApplicationException("[RemoverAssistenciaTecnica] Código Assistencia: " + codigoPosto
                                              + " \nResponse: " + resultado.ErrorException
                                              + " \nJsonSerializer: " + reqRemover.JsonSerializer.ToString());
        }

        private RestSharp.IRestResponse<RespostaLogin> ObtemRespostaLogin(string username, string password)
        {
            var reqLogin = new RestRequest();

            reqLogin.AddHeader("Content-Type", "application/json");
            reqLogin.AddHeader("Accept", "application/json");
            reqLogin.Resource = "/api/auth/user/login";
            reqLogin.Method = Method.POST;
            reqLogin.RequestFormat = DataFormat.Json;

            reqLogin.OnBeforeDeserialization = resp =>
            {
                resp.ContentType = "application/json";
            };

            reqLogin.AddBody(new
            {
                username = username,
                password = password
            });

            return ClienteRest.Execute<RespostaLogin>(reqLogin);
        }

        private void Autentica(string username, string password)
        {
            var respostaLogin = ObtemRespostaLogin(username, password);

            if (respostaLogin == null)
                throw new ApplicationException("Problema com a Autenticação! O método RespostaLogin do site está retornando NULL.");

            if (respostaLogin.Data == null)
                throw new ApplicationException("Problema com a Autenticação! O método RespostaLogin.Data do site está retornando NULL.");

            token = respostaLogin.Data.session_name + respostaLogin.Data.sessid;


            //Token
            var reqToken = new RestRequest();

            reqToken.AddHeader("Accept", "application/json");
            reqToken.AddHeader("Content-Type", "application/json");
            reqToken.AddHeader("accept-language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4,es;q=0.2");
            reqToken.Resource = "/api/auth/user/token";
            reqToken.Method = Method.POST;
            reqToken.RequestFormat = DataFormat.Json;

            reqToken.OnBeforeDeserialization = resp =>
            {
                resp.ContentType = "application/json";
            };


            var respToken = ClienteRest.Execute<RespostaToken>(reqToken);

            csrf_token = respToken.Data.token;
        }

        private void ProcessarItem(Domain.Model.ImportacaoAssistenciaTecnica item)
        {
            try
            {
                if (item == null)
                {
                    throw new ArgumentException("Item de importação não pode ser vazio!");
                }

                if (item.PesquisaFetch == null || string.IsNullOrEmpty(item.PesquisaFetch))
                {
                    throw new ArgumentException("O item pesquisa não pode ser vazio");
                }

                if (item.Produto == null)
                {
                    throw new ArgumentException("Produto não pode ser vazio!");
                }

                // Busca os Clientes através do Fetch!  (de acordo com a documentação, Somente 2.000 registros)
                var ContasFetch = (new Domain.Servicos.RepositoryService()).Conta.ConsultaAssitenciaFetch(item.PesquisaFetch);

                //Loop para todos os clientes do Fetch da Localização Avançada gravado no registro.
                foreach (var Conta in ContasFetch)
                {
                    //Adiciona ou remove os produtos                       
                    if (item.Acao == 1) //Acao = 1, Adiciona
                    {
                        ProdutoAssisteciaTecnica produto = new ProdutoAssisteciaTecnica();
                        produto.AssistenciaTecnica = new SDKore.DomainModel.Lookup(Conta.ID.Value, "account");
                        produto.Produto = new SDKore.DomainModel.Lookup(item.Produto.Id, "product");
                        (new Domain.Servicos.RepositoryService()).ProdutoAssisteciaTecnica.Create(produto);
                    }
                    else if (item.Acao == 2) //Acao = 2, Remove
                    {
                        var produto = (new CRM2013.Domain.Servicos.RepositoryService()).ProdutoAssisteciaTecnica.ObterPor(item.Produto.Id, Conta.ID.Value);
                        (new CRM2013.Domain.Servicos.RepositoryService()).Conta.AlterarStatus("new_produto_assistecia_tecica", produto[0].Id, 1, 2);
                    }
                }

                item.RazaoStatus = (int)Domain.Enum.ImportacaoAssistenciaTecnica.RazaoStatus.Processado_com_Sucesso;
                (new Domain.Servicos.RepositoryService()).ImportacaoAssistenciaTecnica.Update(item);
            }
            catch (Exception ex)
            {
                item.RazaoStatus = (int)Domain.Enum.ImportacaoAssistenciaTecnica.RazaoStatus.Processado_com_Erro;
                (new Domain.Servicos.RepositoryService()).ImportacaoAssistenciaTecnica.Update(item);
                SDKore.Helper.Error.Create(ex, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private List<Product> ObterProtutos(Domain.Model.Conta assistencia)
        {
            string[] codigoGrupoEstoque = SDKore.Configuration.ConfigurationManager.GetSettingValue("IntegradorSiteCodigoGrupoEstoque").Split(';');
            return (new Domain.Servicos.RepositoryService()).Produto.ObterPorAutorizada(assistencia, codigoGrupoEstoque);
        }

        private void CreateLog(string mensagem)
        {
            if (CriaLogInfomarion)
            {
                SDKore.Helper.Error.Create(mensagem, System.Diagnostics.EventLogEntryType.Information);
            }
        }
    }
}
