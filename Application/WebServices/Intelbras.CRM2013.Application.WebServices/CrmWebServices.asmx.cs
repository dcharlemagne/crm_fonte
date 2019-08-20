﻿using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.ViewModels;
using Intelbras.CRM2013.Util.CustomException;
using Intelbras.Message.Helper;
using SDKore.Configuration;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml.Linq;
using itb = Intelbras.CRM2013.Domain.Enum.Sistemas;

namespace Intelbras.CRM2013.Application.WebServices
{
    /// <summary>
    /// Summary description for CrmWebServices
    /// </summary>
    [WebService(Namespace = "urn:crm2013:intelbras.com.br/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]


    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class CrmWebServices : System.Web.Services.WebService
    {
        #region Metodos
        private RepositoryService repoService = null;
        static CrmWebServices()
        {
            Util.Utilitario._symmetricAlgorithm = new AesManaged();
            Util.Utilitario._symmetricAlgorithm.GenerateIV();
        }

        private string SDKoreOrganizacaoIntelbras
        {
            get { return ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"); }
        }

        private bool IsOffline
        {
            get { return false; }
        }

        [WebMethod]
        public bool Postar(string usuario, string senha, string requisicao, out string resposta)
        {
            resposta = requisicao;
            var key = string.Empty;

            try
            {

                #region remove criptografia do usuário e senha

                try
                {
                    if (string.IsNullOrWhiteSpace(SDKoreOrganizacaoIntelbras))
                    {
                        throw new ArgumentException("(CRM) Nome da Organização não está cadastrada no SDKore.config");
                    }

                    key = ConfigurationManager.GetSettingValue("ChaveCriptografia");

                    usuario = Util.Utilitario.DecryptSymmetric(usuario, Encoding.Unicode.GetBytes(key));
                }
                catch (Exception ex)
                {
                    ERR0001 objErro = new ERR0001(itb.RetornaSistema(itb.Sistema.Pollux), "Criptografia");
                    objErro.DescricaoErro = "Erro ao decriptar usuario/senha do ESB! - Key: " + key + " usuario:" + usuario + " senha: " + senha + " EX:" + ex.Message.ToString() + " StackTrace - > " + ex.StackTrace.ToString();
                    resposta = objErro.GenerateMessage(false);
                }

                #endregion

                Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(SDKoreOrganizacaoIntelbras, IsOffline);
                return integ.Postar(usuario, senha, requisicao, out resposta);
            }
            catch (Exception ex)
            {
                SDKore.Helper.Error.Handler(ex);
            }

            return false;
        }

        [WebMethod]
        public bool Ping()
        {
            return true;
        }

        [WebMethod]
        public string BuscarContaIntegracaoCrm4(string guidCrm40Conta)
        {
            if (!String.IsNullOrEmpty(guidCrm40Conta))
            {
                Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);
                return integ.ObterUrlCrm2013(guidCrm40Conta, true);
            }
            return String.Empty;
        }

        [WebMethod]
        public string BuscarContatoIntegracaoCrm4(string guidCrm40Contato)
        {
            if (!String.IsNullOrEmpty(guidCrm40Contato))
            {
                Domain.Servicos.Integracao integ = new Domain.Servicos.Integracao(ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);
                return integ.ObterUrlCrm2013(guidCrm40Contato, false);
            }
            return String.Empty;
        }

        private Boolean ValidarUsuario(string usuario, string senha, out string resposta)
        {
            try
            {
                string usuarioConfig = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSUser");
                string senhaConfig = SDKore.Configuration.ConfigurationManager.GetSettingValue("SellOutCRMWSPasswd");

                if (usuario == usuarioConfig && senha == senhaConfig)
                {
                    resposta = "";
                    return true;
                }
                else
                {
                    resposta = "Usuário ou senha Sell Out incorretos!";
                    return false;
                }
            }
            catch (Exception e)
            {
                resposta = e.Message;
                return false;
            }
        }

        [WebMethod]
        public bool PesquisarDistribuidor(string usuario, string senha, string distribuidorCNPJ, out string resposta)
        {
            if (this.ValidarUsuario(usuario, senha, out resposta))
            {
                Domain.Model.Conta conta = new Intelbras.CRM2013.Domain.Servicos.ContaService(SDKoreOrganizacaoIntelbras, IsOffline)
                    .BuscaContaPorCpfCnpj(distribuidorCNPJ);

                if (conta == null)
                {
                    resposta = "Conta não encontrada";
                    return false;
                }

                if (conta.Status.Value != (int)Domain.Enum.Conta.StateCode.Ativo)
                {
                    resposta = "Conta não está ativa";
                    return false;
                }

                if (conta.ParticipantePrograma != (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
                {
                    resposta = "Conta não participa do Programa de Canais";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(conta.CodigoMatriz))
                {
                    resposta = "Conta não encontrada";
                    return false;
                }

                var xmlroot = new XDocument(
                                            new XDeclaration("1.0", "utf-8", "no"),
                                            new XElement("Distribuidor",
                                                new XElement("Iddistribuidorcrm", conta.ID),
                                                new XElement("Iddistribuidorerp", conta.CodigoMatriz),
                                                new XElement("Cnpj", conta.CpfCnpj),
                                                new XElement("Statuscode", conta.RazaoStatus),
                                                new XElement("Statecode", conta.Status)
                                                ));
                resposta = xmlroot.Declaration.ToString() + Environment.NewLine + xmlroot.ToString();
                return true;
            }
            else
                return false;
        }

        [WebMethod]
        public bool ListarArquivosEmAguardando(string usuario, string senha, out string resposta)
        {
            try
            {
                var lstArquivoSellout = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeSellOutServices(SDKoreOrganizacaoIntelbras, IsOffline)
                    .ListarPor(null, (int)Domain.Enum.ArquivoSellOut.RazaoStatus.NaoProcessado, null, null);
                var listaContasAtivas = new Domain.Servicos.EstabelecimentoService(SDKoreOrganizacaoIntelbras, IsOffline).BuscaTodosEstabelecimentos("itbc_cnpj");
                if (lstArquivoSellout.Count <= 0)
                {
                    throw new ArgumentException("(CRM) Não existem registros na fila para serem processados");
                }


                var listElement = new List<XElement>();
                var contaRepositorio = new Domain.Servicos.ContaService(SDKoreOrganizacaoIntelbras, IsOffline);

                foreach (var item in lstArquivoSellout)
                {
                    var listaContas = contaRepositorio.ListarMatrizFiliais(item.Conta.Id, "itbc_cpfoucnpj");

                    var cnpj = (from x in listaContas
                                where !string.IsNullOrWhiteSpace(x.CpfCnpj)
                                select new XElement("Cnpj", x.CpfCnpj));


                    listElement.Add(new XElement("PlanilhaSellOut",
                        new XElement("Idarquivoimportacao", item.ID.Value.ToString()),
                        new XElement("NomeArquivoSellOut", item.Nome),
                        new XElement("Conta", item.Conta.Id.ToString()),
                        new XElement("ListaCnpjGrupo", cnpj),
                        new XElement("OrigemArquivo", item.Origem)
                    ));
                }

                listaContasAtivas = new Domain.Servicos.EstabelecimentoService(SDKoreOrganizacaoIntelbras, IsOffline).BuscaTodosEstabelecimentos("itbc_cnpj");

                IEnumerable<XElement> ListaCnpj = null;

                if (listaContasAtivas.Count > 0)
                    ListaCnpj = (from x in listaContasAtivas
                                 where !string.IsNullOrWhiteSpace(x.CNPJ)
                                 select new XElement("Cnpj", x.CNPJ));

                var xml = new XDocument(
                            new XDeclaration("1.0", "utf-8", "no"),
                            new XElement("ArquivosSellout",
                            new XElement("ListaArquivosSellout", listElement),
                            new XElement("ListaCnpjIntelbras", ListaCnpj)
                        ));

                resposta = xml.Declaration.ToString() + Environment.NewLine + xml.ToString();
                return true;
            }
            catch (Exception ex)
            {
                resposta = SDKore.Helper.Error.Handler(ex);
                return false;
            }
        }

        [WebMethod]
        public bool ListarRevendasSelloutFielo(string usuario, string senha, out string resposta)
        {
            try
            {
                var xml = new ContaService(SDKoreOrganizacaoIntelbras, IsOffline).MontaXmlRevendasFielo();
                resposta = xml.Declaration.ToString() + Environment.NewLine + xml.ToString();
                return true;
            }
            catch (Exception ex)
            {
                resposta = Error.Handler(ex);
                return false;
            }
        }

        [WebMethod]
        public bool EnviarAtacadoDistribuidor(string usuario, string senha, string acao, out string resposta)
        {
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "no"));
            var RepositoryService = new RepositoryService(SDKoreOrganizacaoIntelbras, IsOffline);
            string data = "";
            string meses = "";
            bool result = false;
            string mensagem = "";
            string guidClassificacao = "";
            string guidSubclassificacao = "";
            string guidCategoria = "";

            try
            {
                var parametroGlobal = new ParametroGlobalService(RepositoryService).ObterPor((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.DataEnvioRegistrosSelloutAtacadoFielo);
                if (parametroGlobal == null)
                {
                    throw new ArgumentException("(CRM) Parâmetro Global(" + (int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.DataEnvioRegistrosSelloutAtacadoFielo + ") não encontrado!");
                }

                data = parametroGlobal.Valor;
                DateTime dataConsulta = Convert.ToDateTime(data);

                if(acao == "update") {
                    dataConsulta = dataConsulta.AddMonths(1);
                    parametroGlobal.Valor = dataConsulta.GetDateTimeFormats('d')[0];
                    RepositoryService.ParametroGlobal.Update(parametroGlobal);
                } else {
                    if (dataConsulta.Date != DateTime.Now.Date)
                    {
                        resposta = "";
                        return false;
                    }

                    var parametroGlobalMeses = new ParametroGlobalService(RepositoryService).ObterPor((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.MesesAEnviarParaFielo);
                    if (parametroGlobalMeses == null)
                    {
                        throw new ArgumentException("(CRM) Parâmetro Global(" + (int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.MesesAEnviarParaFielo + ") não encontrado!");
                    }
                    meses = parametroGlobalMeses.Valor;

                    #region Busca os guids de Classificacao, subclassificacao e categoria para passar para o sellout
                    Classificacao classificacao = RepositoryService.Classificacao.ObterPor(Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Atac_Dist);
                    Subclassificacoes subclassificacao = RepositoryService.Subclassificacoes.ObterPor(Intelbras.CRM2013.Domain.Enum.Conta.SubClassificacao.Atac_Distribuidor);
                    Categoria categoria = RepositoryService.Categoria.ObterPorNome(Intelbras.CRM2013.Domain.Enum.Conta.CategoriaConta.Completo);

                    if(classificacao != null)
                    {
                        guidClassificacao = classificacao.Id.ToString();
                    }
                    else
                    {
                        throw new ArgumentException("(CRM) Não foi encontrado a classificação.");
                    }

                    if(subclassificacao != null)
                    {
                        guidSubclassificacao = subclassificacao.Id.ToString();
                    }
                    else
                    {
                        throw new ArgumentException("(CRM) Não foi encontrado a subclassificacao.");
                    }

                    if(categoria != null)
                    {
                        guidCategoria = categoria.Id.ToString();
                    }
                    else
                    {
                        throw new ArgumentException("(CRM) Não foi encontrado a categoria.");
                    }
                    #endregion

                }
                result = true;
            }
            catch (Exception ex)
            {
                mensagem = ex.Message;
                result = false;
            }

            xml = new XDocument(
                    new XDeclaration("1.0", "utf-8", "no"),
                    new XElement("EnviarAtacadoDistribuidor",
                        new XElement("DataParaProcessamento", data),
                        new XElement("Meses", meses),
                        new XElement("GuidClassificacao", guidClassificacao),
                        new XElement("GuidSubclassificacao", guidSubclassificacao),
                        new XElement("GuidCategoria", guidCategoria),
                        new XElement("Result", result),
                        new XElement("Mensagem", mensagem)
                    )
                );

            resposta = xml.Declaration.ToString() + Environment.NewLine + xml.ToString();
            return true;
        }

        [WebMethod]
        public bool ListarArquivosEmAguardandoEstoqueGiro(string usuario, string senha, out string resposta)
        {
            try
            {
                var lstArquivoEstoquegiro = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeEstoqueGiroServices(SDKoreOrganizacaoIntelbras, IsOffline)
                    .ListarPor(null, (int)Domain.Enum.ArquivoSellOut.RazaoStatus.NaoProcessado, null, null);
                var listaContasAtivas = new Domain.Servicos.EstabelecimentoService(SDKoreOrganizacaoIntelbras, IsOffline).BuscaTodosEstabelecimentos("itbc_cnpj");
                if (lstArquivoEstoquegiro.Count <= 0)
                {
                    throw new ArgumentException("(CRM) Não existem registros na fila para serem processados");
                }


                var listElement = new List<XElement>();
                var contaRepositorio = new Domain.Servicos.ContaService(SDKoreOrganizacaoIntelbras, IsOffline);

                foreach (var item in lstArquivoEstoquegiro)
                {
                    var listaContas = contaRepositorio.ListarMatrizFiliais(item.Conta.Id, "itbc_cpfoucnpj");

                    var cnpj = (from x in listaContas
                                where !string.IsNullOrWhiteSpace(x.CpfCnpj)
                                select new XElement("Cnpj", x.CpfCnpj));


                    listElement.Add(new XElement("PlanilhaEstoqueGiro",
                        new XElement("Idarquivoimportacao", item.ID.Value.ToString()),
                        new XElement("NomeArquivoEstoqueGiro", item.Nome),
                        new XElement("Conta", item.Conta.Id.ToString()),
                        new XElement("ListaCnpjGrupo", cnpj),
                        new XElement("OrigemArquivo", item.Origem)
                    ));
                }

                listaContasAtivas = new Domain.Servicos.EstabelecimentoService(SDKoreOrganizacaoIntelbras, IsOffline).BuscaTodosEstabelecimentos("itbc_cnpj");

                IEnumerable<XElement> ListaCnpj = null;

                if (listaContasAtivas.Count > 0)
                    ListaCnpj = (from x in listaContasAtivas
                                 where !string.IsNullOrWhiteSpace(x.CNPJ)
                                 select new XElement("Cnpj", x.CNPJ));

                var xml = new XDocument(
                            new XDeclaration("1.0", "utf-8", "no"),
                            new XElement("ArquivosEstoquegiro",
                            new XElement("ListaArquivosEstoquegiro", listElement),
                            new XElement("ListaCnpjIntelbras", ListaCnpj)
                        ));

                resposta = xml.Declaration.ToString() + Environment.NewLine + xml.ToString();
                return true;
            }
            catch (Exception ex)
            {
                resposta = SDKore.Helper.Error.Handler(ex);
                return false;
            }
        }

        [WebMethod]
        public bool ObterUrlArquivo(string GuidEntidade, out string url)
        {
            Guid sharepointGuid;

            try
            {
                if (!Guid.TryParse(GuidEntidade, out sharepointGuid))
                {
                    throw new ArgumentException("(CRM) Guid em formato inválido.");
                }

                var sharePointSiteService = new Intelbras.CRM2013.Domain.Servicos.SharePointSiteService(SDKoreOrganizacaoIntelbras, false);
                Domain.Model.SharePointSite sharepoint = sharePointSiteService.ObterPor(sharepointGuid);

                if (string.IsNullOrEmpty(sharepoint.UrlAbsoluta))
                {
                    throw new ApplicationException("Url sharepoint não definida.");
                }

                List<DocumentoSharePoint> lstDocSharePoint = sharePointSiteService.ListarPorIdRegistro(sharepointGuid);

                if (lstDocSharePoint.Count > 0 && (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlRelativa) || !String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta)))
                {
                    if (!string.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta))
                    {
                        url = lstDocSharePoint[0].UrlAbsoluta;
                    }
                    else
                    {
                        url = sharepoint.UrlAbsoluta + "/itbc_arquivodesellout/" + lstDocSharePoint[0].UrlRelativa;
                    }
                }
                else
                {
                    throw new ApplicationException("Entidade enviada não possui local de documento no sharepoint.");
                }

                return true;
            }
            catch (Exception ex)
            {
                url = SDKore.Helper.Error.Handler(ex);
                return false;
            }
        }

        [WebMethod]
        public bool ObterUrlArquivoEstoqueGiro(string GuidEntidade, out string url)
        {
            Guid sharepointGuid;

            try
            {
                if (!Guid.TryParse(GuidEntidade, out sharepointGuid))
                {
                    throw new ArgumentException("(CRM) Guid em formato inválido.");
                }

                var sharePointSiteService = new Intelbras.CRM2013.Domain.Servicos.SharePointSiteService(SDKoreOrganizacaoIntelbras, false);
                Domain.Model.SharePointSite sharepoint = sharePointSiteService.ObterPor(sharepointGuid);

                if (string.IsNullOrEmpty(sharepoint.UrlAbsoluta))
                {
                    throw new ApplicationException("Url sharepoint não definida.");
                }

                List<DocumentoSharePoint> lstDocSharePoint = sharePointSiteService.ListarPorIdRegistro(sharepointGuid);

                if (lstDocSharePoint.Count > 0 && (!String.IsNullOrEmpty(lstDocSharePoint[0].UrlRelativa) || !String.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta)))
                {
                    if (!string.IsNullOrEmpty(lstDocSharePoint[0].UrlAbsoluta))
                    {
                        url = lstDocSharePoint[0].UrlAbsoluta;
                    }
                    else
                    {
                        url = sharepoint.UrlAbsoluta + "/itbc_arquivoestoquegiro/" + lstDocSharePoint[0].UrlRelativa;
                    }
                }
                else
                {
                    throw new ApplicationException("Entidade enviada não possui local de documento no sharepoint.");
                }

                return true;
            }
            catch (Exception ex)
            {
                url = SDKore.Helper.Error.Handler(ex);
                return false;
            }
        }

        [WebMethod]
        public bool AtualizarStatusArquivo(Guid arquivoSellOutId, int status, int QuantidadeLinhasErro, int QuantidadeLinhasProcessadas, int quantidadeErrosDuplicidade, int QuantidadeTotalLinhas, string descricaoErro, out string resposta)
        {
            try
            {
                ArquivoDeSellOut arquivoSellout = new Domain.Model.ArquivoDeSellOut(SDKoreOrganizacaoIntelbras, IsOffline);
                arquivoSellout.RazaoStatus = status;
                arquivoSellout.ID = arquivoSellOutId;
                arquivoSellout.QuantidadeLinhasErro = QuantidadeLinhasErro;
                arquivoSellout.QuantidadeLinhasProcessadas = QuantidadeLinhasProcessadas;
                arquivoSellout.QuantidadeLinhasDuplicadas = quantidadeErrosDuplicidade;
                arquivoSellout.QuantidadeTotalLinhas = QuantidadeTotalLinhas;
                arquivoSellout.DescricaoErros = descricaoErro;

                switch ((Domain.Enum.ArquivoSellOut.RazaoStatus)status)
                {
                    case Domain.Enum.ArquivoSellOut.RazaoStatus.ArquivoInvalido:
                    case Domain.Enum.ArquivoSellOut.RazaoStatus.ProcessadoComplato:
                    case Domain.Enum.ArquivoSellOut.RazaoStatus.ProcessadoParcial:
                        arquivoSellout.DataDeProcessamento = DateTime.Now;

                        break;

                    case Domain.Enum.ArquivoSellOut.RazaoStatus.Processando:
                        arquivoSellout.DataInicioProcessamento = DateTime.Now;
                        arquivoSellout.AddNullProperty("DataDeProcessamento");
                        break;
                }


                arquivoSellout = new Intelbras.CRM2013.Domain.Servicos.ArquivoDeSellOutServices(SDKoreOrganizacaoIntelbras, IsOffline)
                    .Persistir(arquivoSellout);


                if (arquivoSellout == null)
                {
                    resposta = "Não foi possível atualizar o status do SellOut";
                    return false;
                }

                resposta = "";
                return true;
            }
            catch (Exception ex)
            {
                resposta = SDKore.Helper.Error.Handler(ex);
                return false;
            }
        }

        [WebMethod]
        public bool AtualizarStatusArquivoEstoqueGiro(Guid arquivoEstoqueGiroId, int status, int QuantidadeLinhasErro, int QuantidadeLinhasProcessadas, int QuantidadeTotalLinhas, string descricaoErro, out string resposta)
        {
            try
            {
                ArquivoDeEstoqueGiro arquivoEstoquegiro = new ArquivoDeEstoqueGiro(SDKoreOrganizacaoIntelbras, IsOffline);
                arquivoEstoquegiro.RazaoStatus = status;
                arquivoEstoquegiro.ID = arquivoEstoqueGiroId;
                arquivoEstoquegiro.QuantidadeLinhasErro = QuantidadeLinhasErro;
                arquivoEstoquegiro.QuantidadeLinhasProcessadas = QuantidadeLinhasProcessadas;
                arquivoEstoquegiro.QuantidadeTotalLinhas = QuantidadeTotalLinhas;
                arquivoEstoquegiro.DescricaoErros = descricaoErro;

                switch ((Domain.Enum.ArquivoSellOut.RazaoStatus)status)
                {
                    case Domain.Enum.ArquivoSellOut.RazaoStatus.ArquivoInvalido:
                    case Domain.Enum.ArquivoSellOut.RazaoStatus.ProcessadoComplato:
                    case Domain.Enum.ArquivoSellOut.RazaoStatus.ProcessadoParcial:
                        arquivoEstoquegiro.DataDeProcessamento = DateTime.Now;

                        break;

                    case Domain.Enum.ArquivoSellOut.RazaoStatus.Processando:
                        arquivoEstoquegiro.DataInicioProcessamento = DateTime.Now;
                        arquivoEstoquegiro.AddNullProperty("DataDeProcessamento");
                        break;
                }


                arquivoEstoquegiro = new ArquivoDeEstoqueGiroServices(SDKoreOrganizacaoIntelbras, IsOffline)
                    .Persistir(arquivoEstoquegiro);


                if (arquivoEstoquegiro == null)
                {
                    resposta = "Não foi possível atualizar o status do SellOut";
                    return false;
                }

                resposta = "";
                return true;
            }
            catch (Exception ex)
            {
                resposta = Error.Handler(ex);
                return false;
            }
        }

        [WebMethod]
        public Revenda CriarRevendaSellout(string nomeOrganizacao, string xml)
        {
            return new Domain.Integracao.IntegracaoSellout(nomeOrganizacao, IsOffline).CriarRevendaSellout(xml);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ObterValorProdutoNotaFiscal(string solicitacaoBeneficioID, string produtoID, string notaFiscalID)
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.SolicitacaoBeneficio solBeneficio = new Domain.Model.SolicitacaoBeneficio(organizationName, false);
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            try
            {
                //string produtoID = "08bc0401-bef0-e311-9420-00155d013d39";
                //string notaFiscalID = "2D109EDA-AC18-E411-9233-00155D013E44";
                //string solicitacaoBeneficioID = "F9A8E1BB-A418-E411-9233-00155D013E44";

                Domain.Model.ProdutoFatura prodFatura = new Domain.Model.ProdutoFatura(organizationName, false);
                Guid produtoGuid = Guid.Parse(produtoID);
                Guid? notaFiscalGuid = (notaFiscalID == "null") ? (Guid?)null : Guid.Parse(notaFiscalID);
                Guid solicitacaoBeneficioGuid = Guid.Parse(solicitacaoBeneficioID);
                Decimal ValorRetorno;
                decimal? quantidade;

                solBeneficio = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(solicitacaoBeneficioGuid);

                if (solBeneficio == null)
                    throw new ArgumentException("Não foi possível encontrar a solicitação de benefício");

                if (solBeneficio.BeneficioPrograma == null)
                    throw new ArgumentException("Campo Benefício do Programa Vazio.");

                if (notaFiscalGuid != null)
                {
                    prodFatura = new Intelbras.CRM2013.Domain.Servicos.ProdutoFaturaService(organizationName, false).ObterPorProdutoEfatura(produtoGuid, notaFiscalGuid.Value);

                    if (prodFatura == null)
                        throw new ArgumentException("Produto não encontrado na Nota Fiscal");

                    if (!prodFatura.ValorLiquido.HasValue)
                        throw new ArgumentException("Produto sem valor líquido.");

                    ValorRetorno = prodFatura.ValorLiquido.Value;
                    quantidade = prodFatura.Quantidade;
                }
                else
                {
                    List<Domain.Model.PrecoProduto> lstPreco = new List<Domain.Model.PrecoProduto>();
                    Domain.Model.PrecoProduto precoProdutoItem = new Domain.Model.PrecoProduto(organizationName, false);
                    precoProdutoItem.ContaId = solBeneficio.Canal.Id;
                    Intelbras.CRM2013.Domain.Model.Product produto = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(organizationName, false).ObterPor(produtoGuid);
                    if (produto == null)
                        throw new ArgumentException("Produto não encontrado");
                    precoProdutoItem.ProdutoId = produtoGuid;
                    lstPreco.Add(precoProdutoItem);
                    lstPreco = new Intelbras.CRM2013.Domain.Servicos.ProdutoService(organizationName, false).ListarPor(lstPreco);

                    quantidade = lstPreco.First<Domain.Model.PrecoProduto>().Quantidade;
                    ValorRetorno = lstPreco.First<Domain.Model.PrecoProduto>().ValorProduto;
                }

                Domain.Model.Beneficio benefPrograma = new Intelbras.CRM2013.Domain.Servicos.BeneficioService(organizationName, false).ObterPor(solBeneficio.BeneficioPrograma.Id);

                if (benefPrograma == null)
                    throw new ArgumentException("Benefício não encontrado.");

                dictResposta.Add("Resultado", true);
                if (benefPrograma.Codigo == (int)Domain.Enum.BeneficiodoPrograma.Codigos.PriceProtection)
                {
                    dictResposta.Add("TipoBeneficio", "PriceProtection");
                }
                else
                {
                    dictResposta.Add("TipoBeneficio", "Outros");
                }
                if (solBeneficio.BeneficioCanal != null)
                    dictResposta.Add("BeneficioCanalGuid", solBeneficio.BeneficioCanal.Id);
                if (quantidade != null)
                    dictResposta.Add("quantidade", quantidade.ToString());
                dictResposta.Add("ValorLiquido", ValorRetorno.ToString());
            }
            catch (FormatException)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", "Guid em formato incorreto!Esperado : (xxxxxxxx-xxxx-xxxxx-xxxx-xxxxxxxxxxxx)");
            }
            catch (Exception ex)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", ex.Message);
            }

            return jsonConverter.Serialize(dictResposta);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ObterValorDoProdutoPelaListaDePreco(string produtoSolicitacaoId, string solicitacaoBeneficioID, string produtoID, string canalId, string classificacaoId, string unidadeNegocioId)
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.SolicitacaoBeneficio solBeneficio = new Domain.Model.SolicitacaoBeneficio(organizationName, false);
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();

            try
            {

                Domain.Model.ProdutoFatura prodFatura = new Domain.Model.ProdutoFatura(organizationName, false);
                Guid produtoGuid = Guid.Parse(produtoID);
                Guid solicitacaoBeneficioGuid = Guid.Parse(solicitacaoBeneficioID);
                Guid canalGuid = Guid.Parse(canalId);
                //Guid classificacaoGuid = Guid.Parse(classificacaoId);
                //Guid unidadeNegocioGuid = Guid.Parse(unidadeNegocioId);
                decimal valorUnitario = 0;
                Conta contaCliente = new Domain.Servicos.ContaService(organizationName, false).BuscaConta(canalGuid);

                if (contaCliente.Classificacao.Name != "Revendas")
                {
                    List<ProdutosdaSolicitacao> listaProdSolicitacao = new Domain.Servicos.ProdutosdaSolicitacaoService(organizationName, false).ListarPorSolicitacaoAtivos(solicitacaoBeneficioGuid);

                    if (listaProdSolicitacao != null)
                    {
                        ProdutosdaSolicitacao prodSolicitacao = new ProdutosdaSolicitacao(organizationName, false);
                        foreach (var item in listaProdSolicitacao)
                        {
                            if (item.Produto.Id == produtoGuid)
                            {
                                prodSolicitacao = item;
                            }
                        }
                        Estabelecimento estabelecimento = new Domain.Servicos.EstabelecimentoService(organizationName, false).BuscaEstabelecimento(prodSolicitacao.Estabelecimento.Id);
                        Product produto = new Domain.Servicos.ProdutoService(organizationName, false).ObterPor(produtoGuid);
                        SolicitacaoBeneficio solicBen = new Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(solicitacaoBeneficioGuid);

                        var lstPrecoProduto = new List<PrecoProduto>();
                        lstPrecoProduto.Add(new PrecoProduto(organizationName, false)
                        {
                            codEstabelecimento = estabelecimento.Codigo.Value,
                            CodigoProduto = produto.Codigo,
                            ContaId = solicBen.Canal.Id,
                            Produto = produto,
                            TipoCrossSelling = false,
                            Quantidade = Convert.ToInt32(prodSolicitacao.QuantidadeAprovada.Value)
                        });


                        var precoProduto = new Domain.Servicos.ProdutoService(organizationName, false).ListarPor(lstPrecoProduto).First();

                        if (precoProduto == null || precoProduto.ValorProduto <= 0)
                        {
                            throw new ArgumentException("(CRM) Não foi possível calcular o preço do produto [" + produto.Nome + "]. Verifique política comercial");
                        }

                        valorUnitario = precoProduto.ValorProduto;
                        dictResposta.Add("ValorLiquido", valorUnitario.ToString());
                    }
                    else
                    {
                        throw new ArgumentException("(CRM) Nenhum produto foi encontrado na solicitação [" + solicitacaoBeneficioID + "].");
                    }
                }
                else
                {
                    Product produto = new Domain.Servicos.ProdutoService(organizationName, false).ObterPor(produtoGuid);
                    repoService = new RepositoryService(organizationName, false);
                    ListaPrecoPSDPPPSCF lstListaPreco = repoService.ListaPrecoPSD.ObterPor(contaCliente.Endereco1Estadoid.Id, produto.UnidadeNegocio.Id);
                    if (lstListaPreco == null)
                    {
                        throw new ArgumentException("(CRM) Não foi possível encontrar uma Lista de Preço(PSD) para o Estado [" + contaCliente.Endereco1Estadoid.Name + "] e Unidade de Negócio [" + produto.UnidadeNegocio.Name + "]");
                    }
                    var precoProduto = repoService.ProdutoListaPSD.ListarPor(lstListaPreco.ID.Value, produto.ID).First();
                    if (precoProduto == null || precoProduto.ValorPSD <= 0)
                    {
                        throw new ArgumentException("(CRM) Não foi possível calcular o preço do produto [" + produto.Nome + "]. Verifique a Lista PSD para o Estado [" + contaCliente.Endereco1Estadoid.Name + "]");
                    }
                    valorUnitario = precoProduto.ValorPSD.Value;
                    dictResposta.Add("ValorLiquido", valorUnitario.ToString());
                }


            }
            catch (FormatException)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", "Guid em formato incorreto!Esperado : (xxxxxxxx-xxxx-xxxxx-xxxx-xxxxxxxxxxxx)");
            }
            catch (Exception ex)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", ex.Message);
            }

            return jsonConverter.Serialize(dictResposta);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ObterValorParametroGlobalValidado(string solicitacaoBeneficioID, string canalId, string unidadeNegocioId)
        {
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Model.SolicitacaoBeneficio solBeneficio = new Domain.Model.SolicitacaoBeneficio(organizationName, false);
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();

            try
            {
                Guid solicitacaoBeneficioGuid = Guid.Parse(solicitacaoBeneficioID);
                SolicitacaoBeneficio solicBen = new Domain.Servicos.SolicitacaoBeneficioService(organizationName, false).ObterPor(solicitacaoBeneficioGuid);
                Beneficio beneficio = new Domain.Servicos.BeneficioService(organizationName, false).ObterPor(solicBen.BeneficioPrograma.Id);
                Guid canalGuid = Guid.Parse(canalId);
                Guid unidadeNegocioGuid = Guid.Parse(unidadeNegocioId);
                Conta conta = new Domain.Servicos.ContaService(organizationName, false).BuscaConta(canalGuid);

                decimal? fator = null;

                switch (beneficio.Codigo)
                {
                    case (int)Domain.Enum.BeneficiodoPrograma.Codigos.Showroom:

                        if(conta.Classificacao.Name == "Revendas")
                        {
                            var fatorShowRoom = new Domain.Servicos.ParametroGlobalService(organizationName, false).ObterPor((int)Domain.Enum.TipoParametroGlobal.PercentualDescontoShowRoom, unidadeNegocioGuid, conta.Classificacao.Id, conta.Categoria.Id, null, null, beneficio.ID, null);
                            if (fatorShowRoom == null)
                            {
                                throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Show Room não localizado para Unidade de Negócio [" + solBeneficio.UnidadedeNegocio.Name + "]");
                            }

                            fator = Convert.ToDecimal(fatorShowRoom.Valor);
                            dictResposta.Add("ValorParametroGlobal", fator);
                        }
                        else
                        {
                            var fatorShowRoom = new Domain.Servicos.ParametroGlobalService(organizationName, false).ObterPor((int)Domain.Enum.TipoParametroGlobal.PercentualDescontoShowRoom, unidadeNegocioGuid, null, null, null, null, beneficio.ID, null);
                            if (fatorShowRoom == null)
                            {
                                throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Show Room não localizado para Unidade de Negócio [" + solBeneficio.UnidadedeNegocio.Name + "]");
                            }

                            fator = Convert.ToDecimal(fatorShowRoom.Valor);
                            dictResposta.Add("ValorParametroGlobal", fator);
                        }
                    break;

                    case (int)Domain.Enum.BeneficiodoPrograma.Codigos.Backup:
                        if (conta.Classificacao.Name == "Revendas")
                        {
                            var fatorBackup = new Domain.Servicos.ParametroGlobalService(organizationName, false).ObterPor((int)Domain.Enum.TipoParametroGlobal.PercentualDescontoBackup, unidadeNegocioGuid, conta.Classificacao.Id, conta.Categoria.Id, null, null, beneficio.ID, null);

                            if (fatorBackup == null)
                            {
                                throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Backup não localizado para Unidade de Negócio [" + solBeneficio.UnidadedeNegocio.Name + "]");
                            }

                            fator = Convert.ToDecimal(fatorBackup.Valor);
                            dictResposta.Add("ValorParametroGlobal", fator);
                        }else
                        {
                            var fatorBackup = new Domain.Servicos.ParametroGlobalService(organizationName, false).ObterPor((int)Domain.Enum.TipoParametroGlobal.PercentualDescontoBackup, unidadeNegocioGuid, null, null, null, null, beneficio.ID, null);

                            if (fatorBackup == null)
                            {
                                throw new ArgumentException("(CRM) Parâmetro global Percentual Desconto Backup não localizado para Unidade de Negócio [" + solBeneficio.UnidadedeNegocio.Name + "]");
                            }

                            fator = Convert.ToDecimal(fatorBackup.Valor);
                            dictResposta.Add("ValorParametroGlobal", fator);
                        }

                        break;
                }

            }
            catch (FormatException)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", "Guid em formato incorreto!Esperado : (xxxxxxxx-xxxx-xxxxx-xxxx-xxxxxxxxxxxx)");
            }
            catch (Exception ex)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", ex.Message);
            }

            return jsonConverter.Serialize(dictResposta);
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String ObterProdutosPortfolio(String canalGuid)
        {
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            Dictionary<string, object> listResposta = new Dictionary<string, object>();
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            Domain.Servicos.ProdutoService prodServ = new Domain.Servicos.ProdutoService(organizationName, false);

            try
            {
                Guid canal = Guid.Parse(canalGuid);
                Domain.Model.Conta contaCliente = new Domain.Servicos.ContaService(organizationName, false).BuscaConta(canal);
                if (contaCliente == null)
                    throw new Exception("Canal não encontrado");

                if (contaCliente.Classificacao.Name != "Revendas")
                {
                    List<ProdutoPortfolio> prodPortfolio = prodServ.ProdutosPortfolio(contaCliente, contaCliente.Classificacao.Id, null);

                    if (prodPortfolio.Count <= 0)
                        throw new Exception("Canal não possui produtos cadastrados no portfólio");
                    foreach (var item in prodPortfolio)
                    {
                        if (item.Product != null && item.Product.Showroom != null && item.Product.Showroom == true)
                        {
                            if (!string.IsNullOrEmpty(item.Product.Nome) && item.Product.ID != null)
                                if (!listResposta.ContainsKey(item.Product.Nome.ToString()))
                                    listResposta.Add(item.Product.Nome.ToString(), item.Product.ID.ToString());
                        }
                    }
                }

                dictResposta.Add("Produtos", listResposta);
                dictResposta.Add("Sucesso", true);
            }
            catch (FormatException)
            {
                dictResposta.Add("Sucesso", false);
                dictResposta.Add("Mensagem", "Guid em formato incorreto!Esperado : (xxxxxxxx-xxxx-xxxxx-xxxx-xxxxxxxxxxxx)");
            }
            catch (Exception e)
            {
                dictResposta.Add("Sucesso", false);
                dictResposta.Add("Mensagem", e.Message);
            }
            return jsonConverter.Serialize(dictResposta);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String ListarCompromisosSolicitacaoBenef(String benefCanalId)
        {
            Guid? categoria = Guid.Empty;
            Guid? unidNeg = Guid.Empty;
            Guid? benefPrograma = Guid.Empty;
            Guid? classifCanal = Guid.Empty;
            string organizationName = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
            string resposta = string.Empty;
            Domain.Model.Conta canal = new Domain.Model.Conta(organizationName, false);
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();

            try
            {
                Intelbras.CRM2013.Domain.Model.BeneficioDoCanal benefCanal = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(organizationName, false).ObterPor(new Guid(benefCanalId));

                if (benefCanal == null || benefCanal.StatusBeneficio == null || (benefCanal.StatusBeneficio.Name != "Suspenso" && benefCanal.StatusBeneficio.Name != "Bloqueado"))
                {
                    dictResposta.Add("Resultado", false);
                    return jsonConverter.Serialize(dictResposta);
                }
                //Adiciona um atributo no json para ver se o beneficio do canal é suspenso ou bloqueado
                dictResposta.Add("StatusBeneficioCanal", benefCanal.StatusBeneficio.Name);
                #region Se tiver algum atributo no beneficio do canal e canal vazio, seta a variavel como null se não,pega o Guid dela
                categoria = (benefCanal.Categoria == null) ? (Guid?)null : benefCanal.Categoria.Id;
                unidNeg = (benefCanal.UnidadeDeNegocio == null) ? (Guid?)null : benefCanal.UnidadeDeNegocio.Id;
                benefPrograma = (benefCanal.Beneficio == null) ? (Guid?)null : benefCanal.Beneficio.Id;

                if (benefCanal.Canal != null)
                {
                    canal = new Intelbras.CRM2013.Domain.Servicos.ContaService(organizationName, false).BuscaConta(benefCanal.Canal.Id);
                    if (canal != null && canal.Classificacao != null)
                        classifCanal = canal.Classificacao.Id;
                    else
                        classifCanal = null;
                }

                #endregion

                #region Pega a lista de Beneficio x Compromisso com base no perfil(Unidade negocio,classificacao canal,categoria)
                Domain.Model.Perfil perfil = new Intelbras.CRM2013.Domain.Servicos.PerfilServices(organizationName, false).BuscarPerfil(classifCanal, unidNeg, categoria, null);

                if (perfil == null)
                {
                    dictResposta.Add("Resultado", false);
                    dictResposta.Add("Mensagem", "Não possui perfil configurado");
                    return jsonConverter.Serialize(dictResposta);
                }
                List<Domain.Model.BeneficiosCompromissos> benefCompr = new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(organizationName, false).BuscaBeneficiosCompromissos(perfil.ID.Value, null, benefPrograma.Value);

                if (benefCompr.Count == 0)
                {
                    dictResposta.Add("Resultado", false);
                    dictResposta.Add("Mensagem", "Não possui Benefícios x Compromissos configurados");
                    return jsonConverter.Serialize(dictResposta);
                }
                #endregion

                List<string> lstCompromissos = new List<string>();
                foreach (Domain.Model.BeneficiosCompromissos item in benefCompr)
                {
                    if (item.Compromisso != null)
                    {
                        Domain.Model.CompromissosDoCanal comproDoCanal = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(organizationName, false).BuscarCompromissoCanal(item.Compromisso.Id, unidNeg.Value, canal.ID.Value);
                        if (comproDoCanal != null && comproDoCanal.StatusCompromisso != null && comproDoCanal.StatusCompromisso.Name == "Não Cumprido")
                        {
                            lstCompromissos.Add(comproDoCanal.Nome);
                        }
                    }
                }
                if (lstCompromissos.Count > 0)
                {
                    dictResposta.Add("Resultado", true);
                    dictResposta.Add("Compromissos", lstCompromissos);
                }
                else
                    dictResposta.Add("Resultado", false);
                return jsonConverter.Serialize(dictResposta);
            }
            catch (Exception e)
            {
                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Resultado", false);
                dictResposta.Add("Mensagem", e.Message);
                return jsonConverter.Serialize(dictResposta);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string EstaExecutandoAdesaoAoProgramaTodasAsContas()
        {
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            dictResposta = new Dictionary<string, object>();
            dictResposta.Add("Sucesso", false);
            dictResposta.Add("EstaExecutando", false);
            dictResposta.Add("Mensagem", string.Empty);
            dictResposta.Add("MensagemErro", string.Empty);

            string statusFileName = "status.txt";

            try
            {
                var appConsole = ConfigurationManager.GetSettingValue("Intelbras.CRM2013.Aplication.RevalidacaoDaAdesao");

                if (string.IsNullOrEmpty(appConsole))
                {
                    dictResposta["Sucesso"] = false;
                    dictResposta["MensagemErro"] = "appSetting [Intelbras.CRM2013.Aplication.RevalidacaoDaAdesao] não configurado no web.config.";
                    return jsonConverter.Serialize(dictResposta);
                }

                if (!File.Exists(appConsole))
                {
                    dictResposta["Sucesso"] = false;
                    dictResposta["MensagemErro"] = string.Format("Aplicação console não encontrada: [{0}].", appConsole);
                    return jsonConverter.Serialize(dictResposta);
                }

                var path = Path.GetDirectoryName(appConsole);

                statusFileName = Path.Combine(path, statusFileName);
                if (File.Exists(statusFileName))
                {
                    try { File.Delete(statusFileName); }
                    catch { }

                    if (File.Exists(statusFileName))
                    {
                        var fs = new FileStream(statusFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        var buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        fs.Close();

                        dictResposta["Sucesso"] = true;
                        dictResposta["EstaExecutando"] = true;
                        dictResposta["Mensagem"] = System.Text.Encoding.UTF8.GetString(buffer);
                        return jsonConverter.Serialize(dictResposta);
                    }
                }
                dictResposta["Sucesso"] = true;
                dictResposta["EstaExecutando"] = false;
            }
            catch (Exception e)
            {
                dictResposta["Sucesso"] = false;
                dictResposta["MensagemErro"] = string.Format("{0}. StackTrace: {1}", e.Message, e.StackTrace);
            }
            return jsonConverter.Serialize(dictResposta);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AdesaoAoProgramaTodasAsContas()
        {
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();
            dictResposta = new Dictionary<string, object>();
            dictResposta.Add("Sucesso", false);
            dictResposta.Add("EstaExecutando", false);
            dictResposta.Add("Mensagem", string.Empty);
            dictResposta.Add("MensagemErro", string.Empty);
            string appConsole = string.Empty;

            try
            {
                var operacaoExecutando = this.EstaExecutandoAdesaoAoProgramaTodasAsContas();
                var resposta = jsonConverter.Deserialize<Dictionary<string, object>>(operacaoExecutando);
                if (!(bool)resposta["Sucesso"])
                {
                    dictResposta["Sucesso"] = false;
                    dictResposta["MensagemErro"] = resposta["MensagemErro"].ToString();
                    return jsonConverter.Serialize(dictResposta);
                }

                if ((bool)resposta["Sucesso"] && (bool)resposta["EstaExecutando"])
                {
                    dictResposta["Sucesso"] = true;
                    dictResposta["EstaExecutando"] = true;
                    dictResposta["Mensagem"] = resposta["Mensagem"].ToString();
                    return jsonConverter.Serialize(dictResposta);
                }

                appConsole = ConfigurationManager.GetSettingValue("Intelbras.CRM2013.Aplication.RevalidacaoDaAdesao");

                if (!File.Exists(appConsole))
                {
                    dictResposta["Sucesso"] = false;
                    dictResposta["MensagemErro"] = string.Format("Aplicação [{0}] não encontrada.", appConsole);
                    return jsonConverter.Serialize(dictResposta);
                }

                var path = Path.GetDirectoryName(appConsole);

                var processStartInfo = new ProcessStartInfo();

                processStartInfo.FileName = appConsole;
                processStartInfo.Arguments = this.SDKoreOrganizacaoIntelbras;
                processStartInfo.UseShellExecute = true;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardOutput = false;
                processStartInfo.WorkingDirectory = path;

                Process.Start(processStartInfo);

                dictResposta["Sucesso"] = true;
                dictResposta["EstaExecutando"] = false;
            }
            catch (Exception e)
            {
                dictResposta["Sucesso"] = false;
                dictResposta["MensagemErro"] = string.Format("{0}. {1}. StackTrace: {2}", appConsole, e.Message, e.StackTrace);
            }

            return jsonConverter.Serialize(dictResposta);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String ObterInformacoesSefaz(string cpf, string cnpj, string uf)
        {
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();

            try
            {
                var repository = new Domain.Servicos.RepositoryService(SDKoreOrganizacaoIntelbras, IsOffline);
                var msg0164 = new Domain.Integracao.MSG0164(SDKoreOrganizacaoIntelbras, IsOffline);
                var sefazViewModel = new Domain.ViewModels.SefazViewModel()
                {
                    CPF = Regex.Replace(cpf, "[^0-9]", string.Empty),
                    CNPJ = Regex.Replace(cnpj, "[^0-9]", string.Empty),
                    UF = uf
                };
                sefazViewModel = msg0164.Enviar(sefazViewModel);

                if (sefazViewModel == null)
                {
                    throw new ArgumentException("(CRM) Não foi encontrado informações do Sefaz");
                }

                if (sefazViewModel.EnderecoContribuinte == null)
                {
                    throw new ArgumentException("(CRM) Não foi encontrado informações do Endereço Sefaz");
                }

                if (!sefazViewModel.EnderecoContribuinte.CodigoIBGE.HasValue)
                {
                    throw new ArgumentException("(CRM) Código IBGE não enviado.");
                }

                int codigoIbge = sefazViewModel.EnderecoContribuinte.CodigoIBGE.Value;
                var ibge = repository.Municipio.ObterIbgeViewModelPor(codigoIbge);

                if (ibge == null)
                {
                    throw new ArgumentException("(CRM) Não foi possível obter os dados do Sefaz. Favor realizar a validação manual do cadastro.");
                }

                dictResposta.Add("DataBaixa", sefazViewModel.DataBaixa);
                dictResposta.Add("DataInicioAtividade", sefazViewModel.DataInicioAtividade);
                dictResposta.Add("DataModificacaoSituacao", sefazViewModel.DataModificacaoSituacao);
                dictResposta.Add("SituacaoCredenciamentoNFE", sefazViewModel.SituacaoCredenciamentoNFE);
                dictResposta.Add("CNAE", sefazViewModel.CNAE);
                dictResposta.Add("ContribuinteIcms", (sefazViewModel.ContribuinteIcms.HasValue) ? sefazViewModel.ContribuinteIcms.Value : 0);
                dictResposta.Add("Nome", sefazViewModel.Nome.Truncate(80));
                dictResposta.Add("NomeFantasia", sefazViewModel.NomeFantasia.Truncate(60));

                dictResposta.Add("RegimeApuracao", (sefazViewModel.RegimeApuracao == null)
                                                ? string.Empty
                                                : sefazViewModel.RegimeApuracao.Truncate(60));

                dictResposta.Add("InscricaoEstadual", (sefazViewModel.InscricaoEstadual == null)
                                                ? "ISENTO"
                                                : sefazViewModel.InscricaoEstadual);

                dictResposta.Add("Bairro", (sefazViewModel.EnderecoContribuinte.Bairro == null)
                                                ? string.Empty
                                                : sefazViewModel.EnderecoContribuinte.Bairro.Truncate(30));

                dictResposta.Add("CEP", (sefazViewModel.EnderecoContribuinte.CEP == null)
                                                ? string.Empty
                                                : sefazViewModel.EnderecoContribuinte.CEP.Truncate(9));

                dictResposta.Add("Complemento", (sefazViewModel.EnderecoContribuinte.Complemento == null)
                                                ? string.Empty
                                                : sefazViewModel.EnderecoContribuinte.Complemento.Truncate(40));

                dictResposta.Add("Logradouro", (sefazViewModel.EnderecoContribuinte.Logradouro == null)
                                                ? string.Empty
                                                : sefazViewModel.EnderecoContribuinte.Logradouro.Truncate(35));

                dictResposta.Add("Numero", (sefazViewModel.EnderecoContribuinte.Numero == null)
                                                ? string.Empty
                                                : sefazViewModel.EnderecoContribuinte.Numero.Truncate(5));

                dictResposta.Add("CodigoIBGE", sefazViewModel.EnderecoContribuinte.CodigoIBGE);
                dictResposta.Add("CidadeId", ibge.CidadeId);
                dictResposta.Add("CidadeNome", ibge.CidadeNome);
                dictResposta.Add("EstadoId", ibge.EstadoId);
                dictResposta.Add("EstadoNome", ibge.EstadoNome);
                dictResposta.Add("EstadoUF", ibge.EstadoUF);
                dictResposta.Add("PaisId", ibge.PaisId);
                dictResposta.Add("PaisNome", ibge.PaisNome);
                dictResposta.Add("StatusIntegracaoSefaz", (int)Domain.Enum.Conta.StatusIntegracaoSefaz.Validado);

                dictResposta.Add("Sucesso", true);
                return jsonConverter.Serialize(dictResposta);
            }
            catch (BarramentoException ex)
            {
                dictResposta = new Dictionary<string, object>();

                var sefazErroViewModel = new SefazErroViewModel(int.Parse(ex.CodeErro));

                dictResposta.Add("Sucesso", false);
                dictResposta.Add("CodigoErro", ex.CodeErro);
                dictResposta.Add("Mensagem", sefazErroViewModel.MessageError);
                dictResposta.Add("StatusIntegracaoSefaz", (int)sefazErroViewModel.StatusIntegracaoSefaz);

                return jsonConverter.Serialize(dictResposta);
            }
            catch (Exception ex)
            {
                string messageError = SDKore.Helper.Error.Handler(ex);

                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Sucesso", false);
                dictResposta.Add("CodigoErro", 0);
                dictResposta.Add("Mensagem", messageError);
                return jsonConverter.Serialize(dictResposta);
            }
        }

        #region Buscar Conta Matriz Economica
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String ProcuraMatrizEconomica(string raizCNPJ)
        {
            JavaScriptSerializer jsonConverter = new JavaScriptSerializer();
            Dictionary<string, object> dictResposta = new Dictionary<string, object>();

            try
            {
                Domain.Model.Conta conta = new Intelbras.CRM2013.Domain.Servicos.ContaService(SDKoreOrganizacaoIntelbras, IsOffline)
                    .BuscaMatrizEconomica(raizCNPJ);

                if (conta != null)
                {
                    dictResposta.Add("IDMatrizEconomica", conta.Id);
                    dictResposta.Add("CNPJMatriz", conta.CpfCnpj);
                    dictResposta.Add("CodigoMatriz",conta.CodigoMatriz);
                    dictResposta.Add("RazaoMatrizEconomica", (!string.IsNullOrEmpty(conta.RazaoSocial)) ? conta.RazaoSocial : "");
                    dictResposta.Add("NomeAbreviadoMatrizEconomica", (!string.IsNullOrEmpty(conta.NomeAbreviado)) ? conta.NomeAbreviado : "");
                    dictResposta.Add("Sucesso", true);
                }
                else
                {
                    dictResposta.Add("Sucesso", false);
                    dictResposta.Add("CodigoErro", 0);
                }
                return jsonConverter.Serialize(dictResposta);
            }
            catch (Exception ex)
            {
                string messageError = SDKore.Helper.Error.Handler(ex);

                dictResposta = new Dictionary<string, object>();
                dictResposta.Add("Sucesso", false);
                dictResposta.Add("CodigoErro", 0);
                dictResposta.Add("Mensagem", messageError);
                return jsonConverter.Serialize(dictResposta);
            }
        }
        #endregion
        #endregion
    }
}