using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Xml;
using System.Configuration;
using System.Threading;
using System.ServiceModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Intelbras.CRM2013.Domain.Servicos;
using Intelbras.CRM2013.Domain.Servicos.Docs;
using Intelbras.CRM2013.Domain.ValueObjects;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Sdk.Query;
using SDKore.DomainModel;
using System.Web.Script.Services;
using Newtonsoft.Json;

namespace Intelbras.CRM2013.Application.WebServices.Isol
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    //[WebService(Namespace = "urn:crm2013:intelbras.com.br/")]
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2009/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    //[System.ComponentModel.ToolboxItem(false)]

    public class IsolService : Domain.WebServiceBase
    {

        #region " Integração com Correios "

        /// <summary>
        /// Autor: Marcelo Ferreira de Láias
        /// Data: 09/09/2011
        /// Descrição: Gera o código de postagem nos Correios para Atendimento Avulço (Logistica Reversa)
        /// </summary>
        /// <param name="ocorrenciaId"></param>
        /// <param name="nomeOrganizacao"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument GerarCodigoAutorizacaoDePostagem(string ocorrenciaId)
        {
            try
            {
                Ocorrencia ocorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrenciaId));

                if (ocorrencia != null)
                {
                    base.Mensageiro.AdicionarTopico("Achou", true);

                    ocorrencia.ValidarObtencaoContratoAutorizacaoPostagemCorreios();
                    var contrato = ocorrencia.ObtemContratoAutorizacaoPostagemCorreios((new CRM2013.Domain.Servicos.RepositoryService()).AutorizacaoDePostagem.ListarContratoAutorizacaoPostagemCorreiosPor(ocorrencia));

                    string eTiket = contrato.ObterCodigoAutorizacaoDePostagem();

                    Ocorrencia novaOcorrencia = new Ocorrencia(nomeOrganizacao, false) { Id = ocorrencia.Id };
                    novaOcorrencia.CodigoPostagemCorreios = eTiket;
                    novaOcorrencia.SituacaoPostagemCorreios = "E-Ticket Gerado!";
                    novaOcorrencia.NumeroObjetoPostagemCorreios = string.Empty;
                    (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Update(novaOcorrencia);

                    EnviarXmlDeLogisticaReversaParaFTPPor(new Ocorrencia[1] { ocorrencia });

                    base.Mensageiro.AdicionarTopico("Sucesso", true);

                    return base.Mensageiro.Mensagem;
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a geração do código de autorização de postagem pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                   erro, "GerarCodigoAutorizacaoDePostagem");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument EnviarXmlDeLogisticaReversaParaFTP(string ocorrenciaId)
        {
            /********************************
             * Obter ocorrencias
            ********************************/
            Ocorrencia[] ocorrencias;

            if (ocorrenciaId == string.Empty)
                ocorrencias = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarOcorrenciasParaGeracaoDeArquivoXmlParaOsCorreios().ToArray();
            else
                ocorrencias = new Ocorrencia[1] { (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrenciaId)) };

            return EnviarXmlDeLogisticaReversaParaFTPPor(ocorrencias);
        }

        [WebMethod]
        public XmlDocument EnviarXmlDeLogisticaReversaParaFTPPor(Ocorrencia[] ocorrencias)
        {
            try
            {
                /********************************
                 * Gerar arquivo xml para cada ocorrencias
                 ********************************/
                SolicitacaoLogisticaReversaXML solicitacaoLogisticaReversaXml = null;
                AutorizacaoPostagemCorreios contrato = null;
                Domain.Model.Endereco endereco = new Domain.Model.Endereco(nomeOrganizacao, false);
                HistoricoDePostagem historico = new HistoricoDePostagem(nomeOrganizacao, false);

                base.Mensageiro.AdicionarTopico("Achou", ocorrencias.Length > 0 ? true : false);

                foreach (Ocorrencia ocorrencia in ocorrencias)
                {
                    solicitacaoLogisticaReversaXml = new SolicitacaoLogisticaReversaXML(nomeOrganizacao, false);

                    ocorrencia.ValidarObtencaoContratoAutorizacaoPostagemCorreios();
                    contrato = ocorrencia.ObtemContratoAutorizacaoPostagemCorreios((new CRM2013.Domain.Servicos.RepositoryService()).AutorizacaoDePostagem.ListarContratoAutorizacaoPostagemCorreiosPor(ocorrencia));

                    /* Cabeçalho do Arquivo */
                    solicitacaoLogisticaReversaXml.CodigoAdministrativo = contrato.CodigoAdministrativo.ToString();
                    solicitacaoLogisticaReversaXml.Contrato = contrato.NumeroDoContrato.Trim();
                    solicitacaoLogisticaReversaXml.CodigoServico = contrato.TipoDeServico.ToString();
                    solicitacaoLogisticaReversaXml.Cartao = contrato.NumeroDoCartao.ToString();

                    if (ocorrencia.TipoDeETicketPostagemCorreios == TipoDeETicket.Postagem)
                    {
                        /* Dados da Autorizada */
                        solicitacaoLogisticaReversaXml.Destinatario.Nome = ocorrencia.Autorizada.Nome.Trim();
                        solicitacaoLogisticaReversaXml.Destinatario.DDD = string.Empty;
                        solicitacaoLogisticaReversaXml.Destinatario.Telefone = ocorrencia.Autorizada.Telefone.Trim();
                        solicitacaoLogisticaReversaXml.Destinatario.Email = ocorrencia.Autorizada.Email.Trim();

                        endereco.Cidade = ocorrencia.Autorizada.Endereco1Cidade;
                        endereco.Uf = ocorrencia.Autorizada.Endereco1Estado;
                        endereco.Logradouro = ocorrencia.Autorizada.Endereco1Rua;
                        endereco.Numero = ocorrencia.Autorizada.Endereco1Numero;
                        endereco.Complemento = ocorrencia.Autorizada.Endereco1Complemento;
                        endereco.Bairro = ocorrencia.Autorizada.Endereco1Bairro;
                        endereco.Cep = ocorrencia.Autorizada.Endereco1CEP;
                    }
                    else if (ocorrencia.TipoDeETicketPostagemCorreios == TipoDeETicket.Coleta)
                    {
                        /* Dados do Cliente */
                        if (ocorrencia.Cliente != null)
                        {
                            solicitacaoLogisticaReversaXml.Destinatario.Nome = ocorrencia.Cliente.Nome.Trim();
                            solicitacaoLogisticaReversaXml.Destinatario.DDD = string.Empty;
                            if (ocorrencia.Cliente.Telefone != null)
                                solicitacaoLogisticaReversaXml.Destinatario.Telefone = ocorrencia.Cliente.Telefone.Trim();
                            solicitacaoLogisticaReversaXml.Destinatario.Email = ocorrencia.Cliente.Email.Trim();

                            endereco.Cidade = ocorrencia.Cliente.Endereco1Cidade;
                            endereco.Uf = ocorrencia.Cliente.Endereco1Estado;
                            endereco.Logradouro = ocorrencia.Cliente.Endereco1Rua;
                            endereco.Numero = ocorrencia.Cliente.Endereco1Numero;
                            endereco.Complemento = ocorrencia.Cliente.Endereco1Complemento;
                            endereco.Bairro = ocorrencia.Cliente.Endereco1Bairro;
                            endereco.Cep = ocorrencia.Cliente.Endereco1CEP;
                        }
                        else if (ocorrencia.ClienteOS != null)
                        {
                            if (ocorrencia.ClienteOS.TelefoneComercial != null) solicitacaoLogisticaReversaXml.Destinatario.Telefone = ocorrencia.ClienteOS.TelefoneComercial.Trim();
                            solicitacaoLogisticaReversaXml.Destinatario.Nome = ocorrencia.ClienteOS.Nome.Trim();
                            solicitacaoLogisticaReversaXml.Destinatario.DDD = string.Empty;
                            solicitacaoLogisticaReversaXml.Destinatario.Email = ocorrencia.ClienteOS.Email1.Trim();

                            endereco.Cidade = ocorrencia.ClienteOS.Endereco1Municipio;
                            endereco.Uf = ocorrencia.ClienteOS.Endereco1Estado;
                            endereco.Logradouro = ocorrencia.ClienteOS.Endereco1Rua;
                            endereco.Numero = ocorrencia.ClienteOS.Endereco1Numero;
                            endereco.Complemento = ocorrencia.ClienteOS.Endereco1Complemento;
                            endereco.Bairro = ocorrencia.ClienteOS.Endereco1Bairro;
                            endereco.Cep = ocorrencia.ClienteOS.Endereco1CEP;
                        }
                    }

                    if (endereco != null)
                    {
                        solicitacaoLogisticaReversaXml.Destinatario.Logradouro = endereco.Logradouro.Trim();
                        if (!string.IsNullOrEmpty(endereco.Numero)) solicitacaoLogisticaReversaXml.Destinatario.Numero = endereco.Numero.Trim();
                        if (!string.IsNullOrEmpty(endereco.Complemento)) solicitacaoLogisticaReversaXml.Destinatario.Complemento = endereco.Complemento.Trim();
                        if (!string.IsNullOrEmpty(endereco.Bairro)) solicitacaoLogisticaReversaXml.Destinatario.Bairro = endereco.Bairro.Trim();
                        if (!string.IsNullOrEmpty(endereco.Cidade)) solicitacaoLogisticaReversaXml.Destinatario.Cidade = endereco.Cidade.Trim();
                        if (!string.IsNullOrEmpty(endereco.Uf)) solicitacaoLogisticaReversaXml.Destinatario.UF = endereco.Uf.Trim();
                        if (!string.IsNullOrEmpty(endereco.Cep)) solicitacaoLogisticaReversaXml.Destinatario.CEP = endereco.Cep.Trim();
                        solicitacaoLogisticaReversaXml.Destinatario.Referencia = "";
                    }

                    /* Dados da Solicitação */
                    Solicitacao solicitacao = new Solicitacao(nomeOrganizacao, false);

                    solicitacao.Numero = ocorrencia.CodigoPostagemCorreios.Trim();
                    solicitacao.IdCliente = ocorrencia.Numero;
                    solicitacao.Cartao = string.Empty;
                    solicitacao.ValorDeclarado = 0;
                    solicitacao.ServicoAdicional = string.Empty;
                    solicitacao.Descricao = string.Empty;
                    solicitacao.AR = false; //Não enviar Aviso de Recebimento
                    solicitacao.CKList = string.Empty;

                    switch (ocorrencia.TipoDeETicketPostagemCorreios)
                    {
                        case TipoDeETicket.Coleta:
                            historico.TipoDeETiket = "Coleta";
                            solicitacao.Tipo = TipoDeSolicitacaoCorreios.ColetaDomiciliaria;

                            /* Dados da Autorizada */
                            solicitacao.Remetente.Nome = ocorrencia.Autorizada.Nome.Trim();
                            solicitacao.Remetente.DDD = string.Empty;
                            solicitacao.Remetente.Telefone = ocorrencia.Autorizada.Telefone.Trim();
                            solicitacao.Remetente.Email = ocorrencia.Autorizada.Email.Trim();

                            endereco.Cidade = ocorrencia.Autorizada.Endereco1Cidade;
                            endereco.Uf = ocorrencia.Autorizada.Endereco1Estado;
                            endereco.Logradouro = ocorrencia.Autorizada.Endereco1Rua;
                            endereco.Numero = ocorrencia.Autorizada.Endereco1Numero;
                            endereco.Complemento = ocorrencia.Autorizada.Endereco1Complemento;
                            endereco.Bairro = ocorrencia.Autorizada.Endereco1Bairro;
                            endereco.Cep = ocorrencia.Autorizada.Endereco1CEP;

                            break;
                        case TipoDeETicket.Postagem:
                            historico.TipoDeETiket = "Postagem";
                            solicitacao.Tipo = TipoDeSolicitacaoCorreios.AutorizacaoDePostagem;

                            /* Dados do Cliente */
                            if (ocorrencia.Cliente != null)
                            {
                                solicitacao.Remetente.Nome = ocorrencia.Cliente.Nome.Trim();
                                solicitacao.Remetente.DDD = string.Empty;
                                solicitacao.Remetente.Telefone = ocorrencia.Cliente.Telefone.Trim();
                                solicitacao.Remetente.Email = ocorrencia.Cliente.Email.Trim();

                                endereco.Cidade = ocorrencia.Cliente.Endereco1Cidade;
                                endereco.Uf = ocorrencia.Cliente.Endereco1Estado;
                                endereco.Logradouro = ocorrencia.Cliente.Endereco1Rua;
                                endereco.Numero = ocorrencia.Cliente.Endereco1Numero;
                                endereco.Complemento = ocorrencia.Cliente.Endereco1Complemento;
                                endereco.Bairro = ocorrencia.Cliente.Endereco1Bairro;
                                endereco.Cep = ocorrencia.Cliente.Endereco1CEP;
                            }
                            else if (ocorrencia.ClienteOS != null)
                            {
                                if (ocorrencia.ClienteOS.TelefoneComercial != null) solicitacao.Remetente.Telefone = ocorrencia.ClienteOS.TelefoneComercial.Trim();
                                solicitacao.Remetente.Nome = ocorrencia.ClienteOS.Nome.Trim();
                                solicitacao.Remetente.DDD = string.Empty;
                                solicitacao.Remetente.Email = ocorrencia.ClienteOS.Email1.Trim();

                                endereco.Cidade = ocorrencia.ClienteOS.Endereco1Municipio;
                                endereco.Uf = ocorrencia.ClienteOS.Endereco1Estado;
                                endereco.Logradouro = ocorrencia.ClienteOS.Endereco1Rua;
                                endereco.Numero = ocorrencia.ClienteOS.Endereco1Numero;
                                endereco.Complemento = ocorrencia.ClienteOS.Endereco1Complemento;
                                endereco.Bairro = ocorrencia.ClienteOS.Endereco1Bairro;
                                endereco.Cep = ocorrencia.ClienteOS.Endereco1CEP;
                            }

                            break;
                        default:
                            historico.TipoDeETiket = "Consulta Avulsa";
                            break;
                    }

                    if (endereco != null)
                    {
                        solicitacao.Remetente.Logradouro = endereco.Logradouro.Trim();
                        if (!string.IsNullOrEmpty(endereco.Numero)) solicitacao.Remetente.Numero = endereco.Numero.Trim();
                        if (!string.IsNullOrEmpty(endereco.Complemento)) solicitacao.Remetente.Complemento = endereco.Complemento.Trim();
                        if (!string.IsNullOrEmpty(endereco.Bairro)) solicitacao.Remetente.Bairro = endereco.Bairro.Trim();
                        if (!string.IsNullOrEmpty(endereco.Cidade)) solicitacao.Remetente.Cidade = endereco.Cidade.Trim();
                        if (!string.IsNullOrEmpty(endereco.Uf)) solicitacao.Remetente.UF = endereco.Uf.Trim();
                        if (!string.IsNullOrEmpty(endereco.Cep)) solicitacao.Remetente.CEP = endereco.Cep.Trim();
                        solicitacao.Remetente.Referencia = "";
                    }

                    solicitacaoLogisticaReversaXml.ColetasSolicitadas.Add(solicitacao);

                    /* Obter Stream XML */
                    System.IO.MemoryStream xmlStream = solicitacaoLogisticaReversaXml.GetMemoryStream();

                    /********************************
                     Transações com FTP
                    ********************************/
                    //ConfiguracaoDeSistema configHostName = ObterConfiguracaoDoCRM("IntegradorCorreiosServidorDeDados", nomeOrganizacao);
                    //ConfiguracaoDeSistema configUserName = ObterConfiguracaoDoCRM("IntegradorCorreiosUsuarioDeAcesso", nomeOrganizacao);
                    //ConfiguracaoDeSistema configPassword = ObterConfiguracaoDoCRM("IntegradorCorreiosSenhaDoUsuario", nomeOrganizacao);
                    //ConfiguracaoDeSistema configUpload = ObterConfiguracaoDoCRM("IntegradorCorreiosDiretorioDosArquivosDeUpload", nomeOrganizacao);
                    //ConfiguracaoDeSistema configDownload = ObterConfiguracaoDoCRM("IntegradorCorreiosDiretorioDosArquivosDeDownload", nomeOrganizacao);
                    //ConfiguracaoDeSistema configDefinicaoECT = ObterConfiguracaoDoCRM("IntegradorCorreiosParametroDefinidoPelaECT", nomeOrganizacao);

                    string hostName = "ftp.intelbras.com.br";
                    string userName = "carlosal";
                    string password = "carlosal";
                    string diretorioUpload = "/TesteCorreios/Upload/";
                    string valorDefinidoPelaECT = "INTEL0584";

                    //string hostName = configHostName.Valor;
                    //string userName = configUserName.Valor;
                    //string password = configPassword.Valor;
                    //string diretorioUpload = configUpload.Valor;
                    //string valorDefinidoPelaECT = configDefinicaoECT.Valor;

                    //FtpClient ftp = new FtpClient(hostName, userName, password);

                    /* Obter proxima sequencia de arquivo */
                    int seqArquivoNoDia = 0;
                    string dataAtual = DateTime.Now.ToString("yyMMdd");

                    //foreach (string arquivo in ftp.ListDirectory(diretorioUpload))
                    //    if (arquivo.StartsWith("LR_"))
                    //        if (arquivo.Contains(dataAtual))
                    //        {
                    //            string[] splitUnder = arquivo.Split('_');
                    //            int seq = Convert.ToInt32(splitUnder[splitUnder.Length - 1].Split('.')[0]);

                    //            if (seq > seqArquivoNoDia) seqArquivoNoDia = seq;
                    //        }

                    seqArquivoNoDia += 1;

                    /* Devolve um nome de arquivo válido de arquivo */
                    string nomeDoArquivo = "LR_" + valorDefinidoPelaECT + "_" + dataAtual + "_" + seqArquivoNoDia.ToString("00") + ".xml";

                    System.IO.MemoryStream memoryStream = solicitacaoLogisticaReversaXml.GetMemoryStream();
                    memoryStream.Position = 0;

                    /* Envia arquivo para o servidor FTP */
                    //ftp.Upload(memoryStream, diretorioUpload, nomeDoArquivo);

                    historico.Id = Guid.NewGuid();
                    historico.Ocorrencia = ocorrencia;
                    historico.DescricaoSituacaoDaPostagem = "Arquivo XML de logística reversa gerado - " + nomeDoArquivo;

                    historico.CodigoDePostagem = ocorrencia.CodigoPostagemCorreios;
                    historico.NumeroDeObjeto = string.Empty;
                    historico.Nome = string.Empty;
                    historico.LocalDoEvento = string.Empty;
                    historico.TipoDeSituacaoDaPostagem = string.Empty;
                    historico.CodigoSituacaoDaPostagem = string.Empty;
                    historico.DataHoraDaSituacaoDaPostagem = DateTime.Now;

                    historico.Salvar();

                    /* Atualizar Ocorrencia */
                    ocorrencia.StatusDaOcorrencia = StatusDaOcorrencia.Aguardando_Tratativa;
                    (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Update(ocorrencia);

                    base.Mensageiro.AdicionarTopico("Sucesso", true);

                    return base.Mensageiro.Mensagem;
                }
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar o envio do XML de Logística Reversa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                   erro, "EnviarXmlDeLogisticaReversaParaFTP");
            }

            return base.Mensageiro.Mensagem;
        }


        /// <summary>
        /// Autor: Marcelo Ferreira de Láias
        /// Data: 12/09/2011
        /// Descrição: Pesquisa o histórico de postagem pelo código de objeto nos Correios e atualiza as ocorrencias
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument ObterHistoricoDePostagem(string ocorrenciaId)
        {
            /*
             * Obter Números de Objeto para rastreio nos Correios
             */
            List<Ocorrencia> ocorrencias = ocorrenciaId == string.Empty ? (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarOcorrenciasParaRastreioNosCorreios() : new List<Ocorrencia>() { (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrenciaId)) };
            string objetos = string.Empty;

            if (ocorrencias != null)
                foreach (Ocorrencia item in ocorrencias)
                    if (!string.IsNullOrEmpty(item.NumeroObjetoPostagemCorreios))
                        objetos += item.NumeroObjetoPostagemCorreios.Trim();

            if (objetos == string.Empty)
            {
                base.Mensageiro.AdicionarTopico("Achou", false);
                return base.Mensageiro.Mensagem;
            }

            //base.Mensageiro.AdicionarTopico("Achou", true);

            /*
             * Pesquisar histórico de postagem nos Correios
             */
            //ConfiguracaoDeSistema configUrlCorreios = ObterConfiguracaoDoCRM("IntegradorCorreiosURLAcesso", nomeOrganizacao);
            //string urlCorreios = configUrlCorreios.Valor;
            const string urlCorreios = "http://websro.correios.com.br/sro_bin/sroii_xml.eventos?usuario=ECT&senha=SRO&tipo={0}&Resultado={1}&Objetos={2}";

            try
            {
                System.Net.WebClient web = new System.Net.WebClient();
                web.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                byte[] dBytes = web.DownloadData(new Uri(string.Format(urlCorreios, "L", "T", objetos), UriKind.RelativeOrAbsolute));

                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                string xmlString = enc.GetString(dBytes);

                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xmlString);

                /* Para cada objeto pesquisado */
                foreach (System.Xml.XmlNode objeto in doc.SelectNodes("sroxml/objeto"))
                    // Localizar ocorrencia pelo número de objeto de rstareio
                    foreach (Ocorrencia ocorrencia in ocorrencias)
                    {
                        string numeroObjeto = objeto.FirstChild.InnerText;

                        if (numeroObjeto == ocorrencia.NumeroObjetoPostagemCorreios.Trim())
                        {
                            /* Para cada evento do objeto*/
                            bool primeiraVez = true;

                            foreach (System.Xml.XmlNode evento in objeto.SelectNodes("evento"))
                            {
                                string tipoEvento = evento.SelectSingleNode("tipo").InnerText;
                                string statusEvento = evento.SelectSingleNode("status").InnerText;
                                string descricao = evento.SelectSingleNode("descricao").InnerText;
                                DateTime DataHora = Convert.ToDateTime(evento.SelectSingleNode("data").InnerText + " " + evento.SelectSingleNode("hora").InnerText);

                                /* De-Para que atualiza o status da ocorrencia - pegar com Gabriel */
                                if (primeiraVez)
                                {
                                    Ocorrencia novaOcorrencia = new Ocorrencia(nomeOrganizacao, false);
                                    novaOcorrencia.Id = ocorrencia.Id;
                                    novaOcorrencia.SituacaoPostagemCorreios = descricao;

                                    (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Update(novaOcorrencia);
                                    primeiraVez = false;
                                }

                                /* Gravar histório de rastreio */
                                if (ocorrencia.PesquisarHistoricoOcorrencia(ocorrencia, numeroObjeto, tipoEvento, statusEvento, DataHora) == null)
                                {
                                    // Gravar Histórico
                                    HistoricoDePostagem historico = new HistoricoDePostagem(nomeOrganizacao, false);

                                    historico.CodigoDePostagem = ocorrencia.CodigoPostagemCorreios;
                                    historico.NumeroDeObjeto = numeroObjeto;
                                    historico.Nome = numeroObjeto;
                                    historico.LocalDoEvento = evento.SelectSingleNode("local").InnerText;
                                    historico.Ocorrencia = ocorrencia;
                                    historico.TipoDeSituacaoDaPostagem = tipoEvento;
                                    historico.CodigoSituacaoDaPostagem = statusEvento;
                                    historico.DescricaoSituacaoDaPostagem = descricao;
                                    historico.DataHoraDaSituacaoDaPostagem = DataHora;

                                    switch (ocorrencia.TipoDeETicketPostagemCorreios)
                                    {
                                        case TipoDeETicket.Coleta: historico.TipoDeETiket = "Coleta"; break;
                                        case TipoDeETicket.Postagem: historico.TipoDeETiket = "Postagem"; break;
                                        default: historico.TipoDeETiket = "Consulta Avulsa"; break;
                                    }

                                    historico.Salvar();
                                }
                            }
                        }
                    }

                base.Mensageiro.AdicionarTopico("Sucesso", true);
            }
            catch (Exception)
            {
                base.Mensageiro.AdicionarTopico("Sucesso", false);
            }

            return base.Mensageiro.Mensagem;
        }

        #endregion

        [WebMethod]
        public XmlDocument CalcularDiferencaEntreDatas(DateTime dataInicial, DateTime dataFinal, string contratoId, string uf, string cidade)
        {

            try
            {
                int iMinutos = 0;

                var calendario = new CalendarioDeFeriados(nomeOrganizacao, false);
                var feriados = calendario.ObterFeriadosPor(uf, cidade);

                var incident = new Ocorrencia(nomeOrganizacao, false);
                Guid _contratoId = new Guid(contratoId);

                CalendarioDeTrabalho calendarioTrabalho = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.ObterCalendario(_contratoId);

                iMinutos = incident.CalcularDiferencaMinutosSLAEntreDatas(dataInicial, dataFinal, calendarioTrabalho, feriados);


                if (iMinutos > 0)
                {
                    base.Mensageiro.AdicionarTopico("Minutos", iMinutos);
                    base.Mensageiro.AdicionarTopico("Achou", true);
                    base.Mensageiro.AdicionarTopico("Sucesso", true);

                    return base.Mensageiro.Mensagem;
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }

            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                   erro, "CalcularDiferencaEntreDatas");
            }

            return base.Mensageiro.Mensagem;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contratoId"></param>
        /// <param name="clienteId"></param>
        /// <param name="nomeOrganizacao"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument PesquisarResponsavelTecnicoPor(string ocorrenciaId)
        {
            try
            {
                var ocorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrenciaId));

                if (ocorrencia != null && ocorrencia.TecnicoResponsavel != null)
                {


                    base.Mensageiro.AdicionarTopico("Nome", ocorrencia.TecnicoResponsavel.Nome);
                    base.Mensageiro.AdicionarTopico("Id", ocorrencia.TecnicoResponsavel.Id.ToString());
                    base.Mensageiro.AdicionarTopico("Achou", true);
                    base.Mensageiro.AdicionarTopico("Sucesso", true);

                    return base.Mensageiro.Mensagem;
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }

            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                   erro, "PesquisarResponsavelTecnicoPor");
            }

            return base.Mensageiro.Mensagem;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contratoId"></param>        
        /// <returns></returns>
        [WebMethod]
        public XmlDocument PesquisarDataTerminoContrato(string contratoId)
        {
            try
            {
                var contrato = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.Retrieve(new Guid(contratoId));

                if (contrato != null && contrato.FimRealVigencia != null)
                {
                    base.Mensageiro.AdicionarTopico("Data", String.Format("{0:M/d/yyyy HH:mm:ss}", contrato.FimRealVigencia));
                    base.Mensageiro.AdicionarTopico("Achou", true);
                    base.Mensageiro.AdicionarTopico("Sucesso", true);

                    return base.Mensageiro.Mensagem;
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                   erro, "PesquisarDataTerminoContrato");
            }

            return base.Mensageiro.Mensagem;


        }

        /// <summary>
        /// Data: 01/12/2010
        /// Autor: Clausio Elmano de Oliveira
        /// 
        /// </summary>
        /// <param name="contratoId"></param>
        /// <param name="nomeOrganizacao"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument PesquisarClienteParticipantePor(string contratoId, string clienteId)
        {
            try
            {
                //var cliente = ClienteService.PesquisarClientePor(nomecliente, new Organizacao(nomeOrganizacao));
                Contrato contrato = new Contrato(nomeOrganizacao, false);
                contrato.Id = new Guid(contratoId);

                Domain.Model.Conta cliente = new Domain.Model.Conta(nomeOrganizacao, false);
                cliente.Id = new Guid(clienteId);



                //TodosOsContratos todosOsContratos = new TodosOsContratos(new Organizacao(nomeOrganizacao));
                List<ClienteParticipante> clientesParticiantes = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.ObterClientesParticipantesPor(contrato, cliente);

                //TratarErro("QTde Clientes Participantes: " + clientesParticiantes.Count.ToString)

                foreach (ClienteParticipante clienteParticipante in clientesParticiantes)
                {
                    if (null != cliente)
                    {
                        base.Mensageiro.AdicionarTopico("Sucesso", true);
                        base.Mensageiro.AdicionarTopico("Id", clienteParticipante.Id);
                        base.Mensageiro.AdicionarTopico("Descricao", clienteParticipante.Descricao != null ? clienteParticipante.Descricao : "");
                        base.Mensageiro.AdicionarTopico("Nome", clienteParticipante.Nome != null ? clienteParticipante.Nome : "");
                        return base.Mensageiro.Mensagem;
                    }
                    else
                    {
                        base.Mensageiro.AdicionarTopico("Achou", false);
                    }

                }
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message,
                   erro, "PesquisarClienteParticipantePor");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarClientePorNome(string nomecliente)
        {
            try
            {
                var cliente = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ListarPor(nomecliente);

                if (null != cliente)
                {
                    base.Mensageiro.AdicionarTopico("Sucesso", true);
                    base.Mensageiro.AdicionarTopico("Id", cliente[0].Id);
                    base.Mensageiro.AdicionarTopico("JaEstaNoCrm", true);
                    base.Mensageiro.AdicionarTopico("Cnpj", cliente[0].CpfCnpj);

                    //if (!cliente.JaEstaNoCrm)
                    this.MontarEstruturaRetornadaDoEMS(cliente[0]);
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message,
                   erro, "PesquisarClientePorDocumento");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarEnderecoDoClientePorCep(string cep)
        {
            var cepService = new CepService(cep);

            try
            {
                var endereco = cepService.Pesquisar();

                if (null != endereco)
                {
                    base.Mensageiro.AdicionarTopico("Achou", true);
                    base.Mensageiro.AdicionarTopico("Logradouro", endereco.Logradouro);
                    base.Mensageiro.AdicionarTopico("Bairro", endereco.Bairro);
                    base.Mensageiro.AdicionarTopico("Cidade", endereco.Cidade);
                    base.Mensageiro.AdicionarTopico("UF", endereco.Uf);
                    base.Mensageiro.AdicionarTopico("Cep", endereco.Cep);
                    base.Mensageiro.AdicionarTopico("ZonaFranca", endereco.ZonaFranca);
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }

            }
            catch (Exception erro)
            {
                return base.TratarErro("Ocorreu um erro ao pesquisar o endereço. Se o erro persistir entre em contato com o administrador do sistema.",
                   erro, "PesquisarEnderecoDoClientePor");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public Domain.Model.Endereco PesquisarEnderecoDoClientePor(string cep)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Endereco.PesquisarEnderecoPor(cep);
        }

        [WebMethod]
        public Domain.Model.Endereco ObterEnderecoDoClienteParticipantePor(ClienteParticipanteEndereco clienteparticipante)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Endereco.ObterPor(clienteparticipante);
        }

        [WebMethod]
        public ClienteParticipanteEndereco ObterClienteParticipanteDoEnderecoPor(Guid id)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.Retrieve(id);
        }

        [WebMethod]
        public Domain.Model.Endereco PesquisarEnderecoDoClientePelo(string codigo, Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Endereco.ObterPor(codigo, cliente);
        }

        [WebMethod]
        public List<Pais> ListarPaises()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Pais.ListarTodos();
        }

        [WebMethod]
        public List<Estado> ListarEstadosDo(Pais pais)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Estado.ListarPor(pais);
        }

        [WebMethod]
        public List<Municipio> ListarCidadesDo(Estado estado)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Municipio.ListarPor(estado);
        }

        [WebMethod]
        public Municipio ObterCidadePor(int codigoIBGE)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Municipio.ObterPor(codigoIBGE);
        }

        [WebMethod]
        public Municipio ObterCidadePelo(Guid id)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Municipio.Retrieve(id);
        }

        [WebMethod]
        public XmlDocument PesquisarClientePorDocumento(string documento, string natureza)
        {
            try
            {
                int codigoDaNatureza;
                if (!int.TryParse(natureza, out codigoDaNatureza))
                    throw new Exception("O código da natureza informado é inválido.");
                Documento doc = null;


                switch ((NaturezaDoCliente)Convert.ToInt32(natureza))
                {
                    case NaturezaDoCliente.PessoaFisica:
                        doc = new CPF(documento);
                        break;
                    case NaturezaDoCliente.PessoaJuridica:
                        doc = new CNPJ(documento);
                        break;
                    case NaturezaDoCliente.Estrangeiro:
                        doc = new Estrangeiro(documento);
                        break;
                }


                if (!doc.EValido())
                    throw new Exception("O documento informado para a pesquisa é inválido.");



                var cliente = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarPor(documento);

                if (null != cliente)
                {

                    base.Mensageiro.AdicionarTopico("Sucesso", true);
                    base.Mensageiro.AdicionarTopico("Id", cliente.Id);
                    base.Mensageiro.AdicionarTopico("JaEstaNoCrm", true);
                    base.Mensageiro.AdicionarTopico("InscricaoEstadual", cliente.InscricaoEstadual);

                    //if (!cliente.JaEstaNoCrm)
                    this.MontarEstruturaRetornadaDoEMS(cliente);
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message,
                   erro, "PesquisarClientePorDocumento");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarClientePorCodigo(string codigo)
        {
            try
            {
                int _codigo = int.MinValue;

                if (int.TryParse(codigo, out _codigo))
                {
                    var cliente = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarPor(_codigo);

                    if (null != cliente)
                    {
                        base.Mensageiro.AdicionarTopico("Sucesso", true);
                        base.Mensageiro.AdicionarTopico("Id", cliente.Id);
                        base.Mensageiro.AdicionarTopico("Nome", cliente.Nome);
                        base.Mensageiro.AdicionarTopico("JaEstaNoCrm", true);
                        base.Mensageiro.AdicionarTopico("InscricaoEstadual", cliente.InscricaoEstadual);
                    }
                    else
                    {
                        base.Mensageiro.AdicionarTopico("Achou", false);
                    }
                }
                else
                {
                    return base.TratarErro("O código EMS está inválido.", new ArgumentException("O código EMS está inválido"), "PesquisarClientePorCodigo");
                }
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message,
                   erro, "PesquisarClientePorDocumento");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarSerieDoProdutoPor(string numeroDeSerie)
        {
            try
            {
                SerieDoProduto serieDoProduto = new Domain.Integracao.MSG0198(nomeOrganizacao, false)
                .Enviar(numeroDeSerie);//(new CRM2013.Domain.Servicos.RepositoryService()).Produto.PesquisarSerieDoProdutoPor(numeroDeSerie);

                //A partir daqui monta o retorno para ser usado no form da ocorrência
                if (serieDoProduto != null)
                {
                    if (serieDoProduto.Produto != null)
                    {
                        base.Mensageiro.AdicionarTopico("CodigoProduto", serieDoProduto.Produto.Codigo);
                        if (serieDoProduto.DataFabricacaoProduto.HasValue)
                            base.Mensageiro.AdicionarTopico("DataFabricacaoProduto", serieDoProduto.DataFabricacaoProduto.Value.ToString("MM/dd/yyyy"));
                        else
                            base.Mensageiro.AdicionarTopico("DataFabricacaoProduto", "");

                        base.Mensageiro.AdicionarTopico("DescricaoProduto", serieDoProduto.Produto.Nome.Replace("\"", "`"));
                        base.Mensageiro.AdicionarTopico("IdProduto", serieDoProduto.Produto.Id.ToString());
                        if (!string.IsNullOrEmpty(serieDoProduto.NumeroPedido))
                        {
                            base.Mensageiro.AdicionarTopico("NumeroPedido", serieDoProduto.NumeroPedido);
                        }else
                        {
                            base.Mensageiro.AdicionarTopico("NumeroPedido", "");
                        }
                    }
                    else
                    {
                        base.Mensageiro.AdicionarTopico("CodigoProduto", "");
                        base.Mensageiro.AdicionarTopico("DataFabricacaoProduto", "");
                        base.Mensageiro.AdicionarTopico("DescricaoProduto", "");
                    }
                    if (serieDoProduto.NotaFiscal != null)
                    {
                        if (serieDoProduto.NotaFiscal.Cliente.Nome != null)
                        {
                            base.Mensageiro.AdicionarTopico("NomeCliente", serieDoProduto.NotaFiscal.Cliente.Nome.Replace("\"", "`"));
                        }
                        else
                        {
                            base.Mensageiro.AdicionarTopico("NomeCliente", "");
                        }

                        if (serieDoProduto.NotaFiscal.DataEmissao != null)
                        {
                            base.Mensageiro.AdicionarTopico("DataEmissaoNotaFiscal", serieDoProduto.NotaFiscal.DataEmissao.Value.ToString("MM/dd/yyyy"));
                        }
                        else
                        {
                            base.Mensageiro.AdicionarTopico("DataEmissaoNotaFiscal", "");
                        }

                        if (serieDoProduto.NotaFiscal.IDFatura != null)
                        {
                            base.Mensageiro.AdicionarTopico("NumeroNotaFiscal", serieDoProduto.NotaFiscal.IDFatura);
                        }
                        else
                        {
                            base.Mensageiro.AdicionarTopico("NumeroNotaFiscal", "");
                        }
                        
                    }
                    else
                    {
                        base.Mensageiro.AdicionarTopico("NomeCliente", "");
                        base.Mensageiro.AdicionarTopico("DataEmissaoNotaFiscal", "");
                        base.Mensageiro.AdicionarTopico("NumeroNotaFiscal", "");
                    }

                    base.Mensageiro.AdicionarTopico("Achou", true);
                    base.Mensageiro.AdicionarTopico("Sucesso", true);

                    return base.Mensageiro.Mensagem;
                }
                else
                    base.Mensageiro.AdicionarTopico("Achou", false);
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                   erro, "PesquisarSerieDoProduto");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public List<Domain.Model.Conta> ListarClientePor(string nome, string campo, int qtdeRegistros)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarPor(nome, campo, "Like", qtdeRegistros);
        }

        [WebMethod]
        public List<ClienteDoContrato> ListarClientesParticipantesDoContratoPor(ClienteDoContrato clienteDoContrato, int pagina, int quantidade, ref bool existemMaisRegistros, ref string cookie)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.ListarClientesParticipantes(clienteDoContrato, pagina, quantidade, ref existemMaisRegistros, ref cookie);
        }

        [WebMethod]
        public List<Advertencia> ListarAdvertenciasPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Advertencia.ListarPor(cliente);
        }

        [WebMethod]
        public Advertencia ObterAdvertenciasPor(Guid advertenciaId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Advertencia.Retrieve(advertenciaId);
        }

        [WebMethod]
        public Guid SalvarAdvertencia(Advertencia advertencia)
        {
            if (advertencia.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Advertencia.Create(advertencia);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Advertencia.Update(advertencia);
                return advertencia.Id;
            }
        }

        [WebMethod]
        public List<Ocorrencia> ListarPreOSPor(string CPFouCNPJ)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarPreOSPor(CPFouCNPJ);
        }

        [WebMethod]
        public List<Ocorrencia> ListarOcorrenciasParaIsol(Guid contratoId, List<int> status, DateTime inicioFechamento, DateTime fimFechamento)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarParaIsol(contratoId, status, inicioFechamento, fimFechamento);
        }

        [WebMethod]
        public List<Ocorrencia> ListarOcorrenciasParaIsolAprimorado(Guid contratoId, Guid contatoId, List<int> status, DateTime inicioFechamento, DateTime fimFechamento, int pagina, int quantidade, ref bool existemMaisRegistros, ref string cookie, string numeroOcorrencia, bool exibirTodas, int tipoOcorrencia)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarParaIsol(contratoId, contatoId, status, inicioFechamento, fimFechamento, pagina, quantidade, ref existemMaisRegistros, ref cookie, numeroOcorrencia, exibirTodas, tipoOcorrencia);
        }

        //[WebMethod]
        //public List<Defeito> ListarDefeitosPorFamilia(FamiliaComercial familiaComercial)
        //{
        //    return (new CRM2013.Domain.Servicos.RepositoryService()).Defeito.ListarDefeitosPor(familiaComercial);
        //}

        [WebMethod]
        public List<Solucao> ListarSolucaoesPor(string defeitoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Solucao.ListarSolucaoesPor(defeitoId);
        }

        [WebMethod]
        public List<Solucao> ListarSolucaoesPelos(Guid linhaComercialId, Defeito defeitoFamilia)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Solucao.ListarSolucaoesPorFamilia(linhaComercialId, defeitoFamilia);
        }

        [WebMethod]
        public List<Diagnostico> BuscarservicosExecutadosPor(Ocorrencia ocorrencia)
        {
            var diagnosticos = ocorrencia.Diagnosticos;
            for (int x = 0; x < diagnosticos.Count; x++)
                diagnosticos[x] = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.CarregarCamposRelacionadosDiagnostico(diagnosticos[x]);
            return diagnosticos;
        }

        [WebMethod]
        public List<Diagnostico> ListarDiagnosticosDoPoralPor(Ocorrencia ocorrencia)
        {
            var diagnosticos = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.ListarDiagnosticoPortalPor(ocorrencia);
            for (int x = 0; x < diagnosticos.Count; x++)
                diagnosticos[x] = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.CarregarCamposRelacionadosDiagnostico(diagnosticos[x]);
            return diagnosticos;
        }

        [WebMethod]
        public Guid SalvarOS(Ocorrencia ocorrencia)
        {
            ocorrencia.OrganizationName = nomeOrganizacao;
            ocorrencia.IsOffline = false;
            //if (ocorrencia.Id == Guid.Empty)
            //    return (new CRM2013.Domain.Servicos.RepositoryService()).ClientePotencial.Create(lead);
            //else
            //{
            //    (new CRM2013.Domain.Servicos.RepositoryService()).ClientePotencial.Update(lead);
            //    return ocorrencia.Id;
            //}

            try
            {
                return ocorrencia.SalvarOS(ocorrencia);
            }
            catch(Exception ex)
            {
                throw new System.Web.Services.Protocols.SoapException(ex.Message, System.Web.Services.Protocols.SoapException.ClientFaultCode);
            }
        }

        [WebMethod]
        public Veiculo ConsultarVincVeiculoClienteContrato(string placa,string contratoId)
        {
            try
            {
                Contrato contrato = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.Retrieve(new Guid(contratoId));
                if(contrato != null) {
                    List<ClienteParticipante> lstClientesParticipantes = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.ObterClientesParticipantesPor(contrato);
                    if (lstClientesParticipantes.Count > 0)
                    {
                        List<Veiculo> lstVeiculo = (new CRM2013.Domain.Servicos.RepositoryService()).Veiculo.ListarPorClientesParticipantesContrato(lstClientesParticipantes, placa);
                        if(lstVeiculo.Count > 0)
                        {
                            return lstVeiculo[0];
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                base.Mensageiro.AdicionarTopico("MensagemDeErro", erro.Message);
            }

            return null;
        }

        [WebMethod]
        public List<Veiculo> ConsultarVeiculosContrato(string contratoId)
        {
            try
            {
                Contrato contrato = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.Retrieve(new Guid(contratoId));
                if(contrato != null) {
                    List<ClienteParticipante> lstClientesParticipantes = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.ObterClientesParticipantesPor(contrato);
                    if (lstClientesParticipantes.Count > 0)
                    {
                        List<Veiculo> lstVeiculo = (new CRM2013.Domain.Servicos.RepositoryService()).Veiculo.ListarPorClientesParticipantesContrato(lstClientesParticipantes, null);
                        if(lstVeiculo.Count > 0)
                        {
                            return lstVeiculo;
                        }
                    }
                }
            }
            catch (Exception erro)
            {
                base.Mensageiro.AdicionarTopico("MensagemDeErro", erro.Message);
            }

            return null;
        }

        [WebMethod]
        public Veiculo ObterVeiculoPor(string veiculoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Veiculo.Retrieve(new Guid(veiculoId));
        }

        [WebMethod]
        public void AcoesSecundariaDaOS(Ocorrencia ocorrencia, string acao)
        {
            ocorrencia.OrganizationName = nomeOrganizacao;
            ocorrencia.IsOffline = false;
            if (acao == "FinalizarDiagnosticos")
                ocorrencia.FinalizarDiagnosticos(ocorrencia);
            else if (acao == "CancelarOcorrencia")
                (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Cancelar(ocorrencia.Id);

        }

        [WebMethod]
        public bool GarantiaPorContratoEstaVigente(Ocorrencia ocorrencia)
        {
            ocorrencia.OrganizationName = nomeOrganizacao;
            ocorrencia.IsOffline = false;
            return ocorrencia.GarantiaPorContratoEstaVigente();
        }

        [WebMethod]
        public void AcoesSecundariaDoDiagnostico(Diagnostico diagnostico, string acao)
        {
            diagnostico = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.Retrieve(diagnostico.Id);

            if (acao == "Cancelar")
                diagnostico.Cancelar();
        }

        [WebMethod]
        public bool PermiteCancelarDiagnostico(Diagnostico diagnostico)
        {
            diagnostico = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.Retrieve(diagnostico.Id);
            return diagnostico.PodeCancelar();
        }

        [WebMethod]
        public List<Diagnostico> BuscarservicosExecutadosPorFiltros(Ocorrencia ocorrencia)
        {
            var diagnosticos = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.BuscarservicosExecutadosPorFiltros(ocorrencia);
            for (int x = 0; x < diagnosticos.Count; x++)
                diagnosticos[x] = (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.CarregarCamposRelacionadosDiagnostico(diagnosticos[x]);
            return diagnosticos;
        }

        [WebMethod]
        public List<Domain.Model.Endereco> ObterTodosOsEnderecosPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Endereco.ObterTodosOsEnderecosPor(cliente);
        }

        [WebMethod]
        public List<Domain.Model.Conta> ListarClientePorCampos(string codigo, string cnpj, string nome, string nomeAbreviado, string UF, Guid contatoId)
        {
            //LogService.GravaLog("<Cliente> ListarClientePorCampos(string codigo, string cnpj, string nome, string nomeAbreviado, string UF, Guid contatoId)", TipoDeLog.WSVendasIsol);
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarPorCampos(codigo, cnpj, nome, nomeAbreviado, UF, contatoId);
        }

        [WebMethod]
        public List<Domain.Model.Conta> PesquisarClientePorCampos(string codigo, string cnpj, string nome, string nomeAbreviado, string UF, Guid contatoId, NaturezaDoCliente natureza)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarPorCampos(codigo, cnpj, nome, nomeAbreviado, UF, contatoId, natureza);
        }

        [WebMethod]
        public List<Domain.Model.Pedido> ListarPedidosPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.ListarPedidosPor(cliente);
        }

        [WebMethod]
        public List<Domain.Model.Pedido> ListarItensDoPedidoNoERPPor(int CodigoDoPedido, string Aprovador)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.ListarItensDoPedidoNoERPPor(CodigoDoPedido, Aprovador);
        }

        [WebMethod]
        public List<Domain.Model.ProdutoPedido> ListarItensDoPedidosPor(Domain.Model.Pedido pedido)
        {
            return pedido.ItensDoPedido;
        }

        [WebMethod]
        public Domain.Model.Pedido ObterPedidosPor(string campo, string valor)
        {
            var pedido = (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.ObterPedidosPor(campo, valor);
            if (pedido != null)
            {
                pedido.ItensDoPedido = (new CRM2013.Domain.Servicos.RepositoryService()).ProdutoPedido.ListarPorPedido(pedido.Id);
                foreach (Domain.Model.ProdutoPedido item in pedido.ItensDoPedido)
                {
                    if (item.Produto != null)
                        item.ProdutoModel = (new CRM2013.Domain.Servicos.RepositoryService()).Produto.Retrieve(item.Produto.Id);
                }
            }
            return pedido;
        }

        [WebMethod]
        public string SalvarItensDoPedidoBloqueadoNoERPPor(List<Domain.Model.Pedido> pedido)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.SalvarItensDoPedidoBloqueadoNoERPPor(pedido);
        }

        [WebMethod]
        public List<Domain.Model.Fatura> ListarNotasFiscaisPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Fatura.ListarNotasFiscaisPor(cliente);
        }

        [WebMethod]
        public Domain.Model.Fatura ObterNotaFiscalPor(Guid Id)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Fatura.Retrieve(Id);
        }

        [WebMethod]
        public List<Estabelecimento> ListarEstabelecimentos()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Estabelecimento.ListarTodos();
        }

        [WebMethod]
        public List<Estabelecimento> ListarEstabelecimentosDoB2B()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Estabelecimento.ListarB2B();
        }

        [WebMethod]
        public List<Estabelecimento> ListarEstabelecimentosPor(TabelaDePreco tabelaDePreco)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Estabelecimento.ListarEstabelecimentosPor(tabelaDePreco);
        }

        [WebMethod]
        public Estabelecimento ObterEstabelecimentoPor(string codigo)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Estabelecimento.ObterPor(Convert.ToInt32(codigo));
        }

        [WebMethod]
        public LinhaComercial ObterLinhaComercialPor(Product produto)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).LinhaComercial.ObterPor(produto);
        }

        [WebMethod]
        public List<Defeito> ListarDefeitosPor(Product produto)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Defeito.ListarPor(produto);
        }

        [WebMethod]
        public List<Defeito> ListarDefeitosPela(LinhaComercial linha)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Defeito.ListarPor(linha);
        }

        [WebMethod]
        public List<Domain.Model.CondicaoPagamento> ListarCondicaoDePagamento()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).CondicaoPagamento.ListarPor(0);
        }

        [WebMethod]
        public List<UnidadeNegocio> ListarUnidadesDeNegocioB2BPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).UnidadeNegocio.ListarUnidadesDeNegocioB2BPor(cliente);
        }

        [WebMethod]
        public List<Representante> ListarRepresentantesB2BPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ListarRepresentantesB2BPor(cliente);
        }

        [WebMethod]
        public List<Categoria> ListarCategoriasB2BPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ListarCategoriasB2BPor(cliente);
        }

        [WebMethod]
        public List<CategoriaUN> ListarCategoriaUNB2BPor(Domain.Model.Conta cliente, Guid contato)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ListarCategoriaUNB2BPor(cliente, contato);
        }

        [WebMethod]
        public List<TabelaDePreco> ListarTabelaDePreco(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).TabelaPreco.ListarPor(cliente);
        }

        [WebMethod]
        public List<TabelaDePreco> ListarTabelaDePrecoPor(int codigoCliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).TabelaPreco.ListarPor((new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterPorCodigoEmitente(codigoCliente.ToString()));
        }

        [WebMethod]
        public List<decimal> BuscarDescontosDoClientePor(int codigoCliente, string codigoUnidadeDeNegocio, int codigoCategoria, string codigoFamiliaComercial, int qtdeDeProdutos)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.BuscarDescontoPor(codigoCliente, codigoUnidadeDeNegocio, codigoCategoria, codigoFamiliaComercial, qtdeDeProdutos);
        }

        [WebMethod]
        public decimal BuscarDescontoMangaPor(string codigoRepresentante, string codigoCliente, string codigoUnidadeDeNegocio, string codigoCategoria, string codigoFamiliaComercial)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.BuscarDescontoMangaPor(codigoRepresentante, codigoCliente, codigoUnidadeDeNegocio, codigoCategoria, codigoFamiliaComercial);
        }

        [WebMethod]
        public void Anexar(Anotacao anotacao)
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).Anexo.Create(anotacao);
        }

        [WebMethod]
        public Anotacao ObterAnexo(Guid anexoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Anexo.Retrieve(anexoId);
        }

        [WebMethod]
        public List<Anotacao> ListarAnotacoes(string objectId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Anexo.ListarPor(new Guid(objectId));
        }

        [WebMethod]
        public List<Oportunidade> ListarOportunidadePorRevenda(Revenda revenda)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Oportunidade.ListarPor(revenda);
        }

        [WebMethod]
        public List<Product> ListarProdutosPorCampos(string codigo, string familia, string produto, string tabelaDePreco)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ListarProdutosPorCampos(codigo, familia, produto, tabelaDePreco);
        }

        [WebMethod]
        public PrecoItem ObterPrecoDoItemPor(string codigoProduto, int codigoCliente, string codigoTabelaDePreco, string codigoEstabelecimento, string unidadeNegocio, int codigoCategoria)
        {
            return (new PrecoItem()).Obter(codigoProduto, codigoCliente, codigoTabelaDePreco, codigoEstabelecimento, unidadeNegocio, codigoCategoria);
        }

        [WebMethod]
        public List<Product> ListarProdutosPor(FamiliaComercial familiaComercial)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ListarProdutosPor(familiaComercial);
        }

        [WebMethod]
        public List<FamiliaComercial> ListarFamiliaComercial()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FamiliaComercial.ListarPor("");
        }

        [WebMethod]
        public Product BuscarProdutoPor(string codigo, bool integradoEMS, string codigoCliente, string unidadeDeNegocio, string codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco, int tabelaEspecifica)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.BuscarProdutoPor(codigo, integradoEMS, codigoCliente, unidadeDeNegocio, codigoCategoria, codigoEstabelecimento, codigoTabelaDePreco, tabelaEspecifica);
        }

        [WebMethod]
        public Product BuscarProdutoPela(Ocorrencia ocorrencia)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ObterPor(ocorrencia);
        }

        [WebMethod]
        public decimal BuscarIndiceFinanceiroDoProdutoPor(int codigoCondicaoDePagamento)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.BuscarIndiceFinanceiroDoProdutoPor(codigoCondicaoDePagamento);
        }

        [WebMethod]
        public List<Product> ListarEstruturaDoProdutoPor(string numeroDeSerie, string codigoDoProduto)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.BuscarEstruturaDoProdutoPor(numeroDeSerie, codigoDoProduto);
        }

        [WebMethod]
        public bool PostoDeServicoTemAcesso(Domain.Model.Conta postoDeServico, Product produto)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.AcessoProdutoParaAssistenciaTecnica(postoDeServico, produto);
        }

        [WebMethod]
        public bool PostoDeServicoCredenciado(Domain.Model.Conta postoDeServico)
        {
            return (bool)(new CRM2013.Domain.Servicos.RepositoryService()).Conta.Retrieve(postoDeServico.Id).AcessoPortalASTEC;
        }

        [WebMethod]
        public List<LogisticaReversa> ListarLogisticaReversaDo(Domain.Model.Conta postoDeServico)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).LogisticaReversa.ListarLogisticaReversaDo(postoDeServico);
        }

        [WebMethod]
        public Guid SalvarLogisticaReversa(LogisticaReversa logisticaReversa)
        {
            if (logisticaReversa.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).LogisticaReversa.Create(logisticaReversa);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).LogisticaReversa.Update(logisticaReversa);
                return logisticaReversa.Id;
            }
        }

        [WebMethod]
        public Guid SalvarDadosLogisticaReversa(List<Diagnostico> diagnosticos, Domain.Model.Fatura fatura)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.SalvarLogisticaReversa(diagnosticos, fatura);
        }

        [WebMethod]
        public bool ProdutoPossuiGarantiaEspecíficaDentroDaVigênciaPor(string numeroDeSerie)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ProdutoPossuiGarantiaEspecificaDentroDaVigenciaPor(numeroDeSerie);
        }

        //[WebMethod]
        //public List<Product> GerarLogisticaReversaDo(Domain.Model.Conta postoDeServico, Domain.Model.Estabelecimento estabelecimento)
        //{
        //    return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.GerarLogisticaReversaDo(postoDeServico, estabelecimento);
        //}

        [WebMethod]
        public List<ProdutoBase> GerarLogisticaReversaDo(Domain.Model.Conta postoDeServico, Domain.Model.Estabelecimento estabelecimento, bool naoUsar = false)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.GerarLogisticaReversaDo(postoDeServico, estabelecimento, naoUsar);
        }


        /* Utilizado no método de associar Endereço do Contrato - Chamada no ISV */
        [WebMethod]
        public XmlDocument AdicionarParticipante(string clienteId, string contratoId, string clienteParticipanteId, string enderecosSelecionados)
        {
            bool sucesso = true;
            try
            {
                if (!string.IsNullOrEmpty(enderecosSelecionados))
                {
                    Contrato contrato = new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) };
                    List<Domain.Model.Endereco> enderecos = new List<Domain.Model.Endereco>();

                    foreach (var end in enderecosSelecionados.Split(','))
                        enderecos.Add((new CRM2013.Domain.Servicos.RepositoryService()).Endereco.Retrieve(new Guid(end)));

                    contrato.AdicionarParticipante(new Guid(clienteParticipanteId), new Domain.Model.Conta(nomeOrganizacao, false) { Id = new Guid(clienteId) }, enderecos);
                }
            }
            catch (Exception ex)
            {
                base.Mensageiro.AdicionarTopico("MensagemDeErro", ex.Message);
                sucesso = false;
            }

            this.Mensageiro.AdicionarTopico("Sucesso", sucesso);
            if (!sucesso) this.Mensageiro.AdicionarTopico("Mensagem", "Não foi possível adicionar os participantes");

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public List<ProdutoBase> ObterLogisticaReversaPor(Guid extratoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ObterLogisticaReversaPor(extratoId);
        }

        [WebMethod]
        public LogisticaReversa ObterDadosLogisticaReversaPor(Guid extratoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).LogisticaReversa.Retrieve(extratoId);
        }

        [WebMethod]
        public Diagnostico ObterDiagnosticoPor(Guid diagnosticoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.Retrieve(diagnosticoId);
        }

        [WebMethod]
        public Guid SalvarDiagnostico(Diagnostico diagnostico)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Diagnostico.Create(diagnostico);
        }

        [WebMethod]
        public List<Extrato> ObterExtratoLogisticaReversaPor(Domain.Model.Conta autorizada)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Extrato.ListarPor(autorizada);
        }

        [WebMethod]
        public XmlDocument RecalcularValorExtratoDePagamentoOcorrencia(string new_extrato_pagamento_ocorrenciaid)
        {
            try
            {
                Extrato extrato = (new CRM2013.Domain.Servicos.RepositoryService()).Extrato.Retrieve(new Guid(new_extrato_pagamento_ocorrenciaid));
                extrato.AtualizarValor();
                (new CRM2013.Domain.Servicos.RepositoryService()).Extrato.Update(extrato);

                base.Mensageiro.AdicionarTopico("Sucesso", true);
                return base.Mensageiro.Mensagem;
            }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSIntelbras_WSOcorrencia, string.Format("Metodo: RecalcularValorExtratoDePagamentoOcorrencia(string new_extrato_pagamento_ocorrenciaid) \n Parametro: {0} - {1}", new_extrato_pagamento_ocorrenciaid, organizacaoNome));
                return base.TratarErro("Não foi possível concluir a operação!", ex, "RecalcularValorExtratoDePagamentoOcorrencia(string new_extrato_pagamento_ocorrenciaid)");
            }
        }

        [WebMethod]
        public Guid SalvarExtratoPagamento(Domain.Model.Extrato extrato)
        {
            if (extrato.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Extrato.Create(extrato);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Extrato.Update(extrato);
                return extrato.Id;
            }
        }

        [WebMethod]
        public List<Product> ListarProdutosDeTabelaEspecificaPor(string codigoCliente, string unidadeDeNegocio, string codigoCategoria, string codigoEstabelecimento, string codigoTabelaDePreco)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ListarProdutosDeTabelaEspecificaPor(codigoCliente, unidadeDeNegocio, codigoCategoria, codigoEstabelecimento, codigoTabelaDePreco);
        }

        [WebMethod]
        public bool RemoveItemPedidoB2BnoCRM(string codigoItem, string codigoPedido)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.RemoveItemPedidoB2BnoCRM(codigoItem, codigoPedido); ;
        }

        [WebMethod]
        public Domain.Model.Pedido SalvarPedidoB2BnoCRM(Domain.Model.Pedido pedidoDeVenda)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.SalvarPedidoB2BnoCRM(pedidoDeVenda); ;
        }

        [WebMethod]
        public string SalvarPedidoB2BnoEMS(Domain.Model.Pedido pedidoDeVenda)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Pedido.SalvarPedidoB2BnoEMS(pedidoDeVenda);
        }

        [WebMethod]
        public Guid SalvarClientePotencial(ClientePotencial lead)
        {
            if (lead.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).ClientePotencial.Create(lead);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).ClientePotencial.Update(lead);
                return lead.Id;
            }
        }

        [WebMethod]
        public Guid SalvarCliente(Domain.Model.Conta cliente)
        {
            if (cliente.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.Create(cliente);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Conta.Update(cliente);
                return cliente.Id;
            }
        }

        [WebMethod]
        public List<ClientePotencial> ListarLeadPorRevenda(Revenda revenda)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).ClientePotencial.ListarPor(revenda);
        }

        [WebMethod]
        public Guid SalvarTarefa(Domain.Model.Tarefa tarefa)
        {
            if (tarefa.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Tarefa.Create(tarefa);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Tarefa.Update(tarefa);
                return tarefa.Id;
            }
        }

        [WebMethod]
        public Guid SalvarFilaDaTarefa(Fila fila)
        {
            if (fila.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Fila.Create(fila);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Fila.Update(fila);
                return fila.Id;
            }
        }

        [WebMethod]
        public Fila ObterFilaPadraoFidelidade()
        {
            return (new Fila()).FilaPadraoFidelidade;
        }

        [WebMethod]
        public Guid SalvarItemDaFilaDeTarefa(ItemFila itemFila)
        {
            if (itemFila.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).ItemFila.Create(itemFila);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).ItemFila.Update(itemFila);
                return itemFila.Id;
            }
        }

        [WebMethod]
        public Guid BuscarClientePor(Guid contatoId)
        {
            var cliente = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterPor(new Domain.Model.Contato(nomeOrganizacao, false) { Id = contatoId });
            if (cliente != null)
                return cliente.Id;
            else
                return Guid.Empty;
        }

        [WebMethod]
        public List<GrupoDoCliente> ObterGrupoDaRevendaPor(string[] codigos)
        {
            return (new GrupoDoCliente())[codigos];
        }


        [WebMethod]
        public XmlDocument BuscarClienteParticipanteDoContratoPor(string clienteId, string contratoId)
        {
            try
            {
                var clienteParticipante = new ClienteParticipante(nomeOrganizacao, false);
                clienteParticipante = clienteParticipante.RetornaGuidClienteParticipantePorContrato(new Guid(clienteId), new Guid(contratoId));
                base.Mensageiro.AdicionarTopico("guid", clienteParticipante.Id);
                base.Mensageiro.AdicionarTopico("Sucesso", true);
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível encontrar o Cliente Participante do Contrato pelo seguinte motivo: " + erro.Message, erro, "VerificaParametros");
            }


            return base.Mensageiro.Mensagem;

        }

        [WebMethod]
        public void AtualizarOportunidade(Oportunidade oportunidade)
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).Oportunidade.Update(oportunidade);
        }

        [WebMethod]
        public bool VerificaExistenciaContatosPor(string cpfCnpj)
        {
            return ((new CRM2013.Domain.Servicos.RepositoryService()).Contato.ObterPor(cpfCnpj) != null);
        }

        /// <summary>
        /// Faz a verificação dos níveis de permissão do usuário para o formulário
        /// </summary>
        /// <param name="guidUsuario"></param>
        /// <param name="guidGrupoCliente"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument VerificaPermissaoUsuarioGrupoCliente(string guidUsuario, string guidGrupoCliente)
        {
            try
            {
                //var objAutorizador = new Autorizador();

                base.Mensageiro.AdicionarTopico("Sucesso", true);
                //base.Mensageiro.AdicionarTopico("TemAutorizacao", objAutorizador.UsuarioTemAutorizacaoNoGrupoCliente(new Guid(guidUsuario), new Guid(guidGrupoCliente), nomeOrganizacao));
                base.Mensageiro.AdicionarTopico("TemAutorizacao", true);
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível fazer a verificação de permissão pelo seguinte motivo: " + erro.Message,
                   erro, "VerificaPermissaoUsuarioGrupoCliente");
            }

            return base.Mensageiro.Mensagem;
        }

        /// <summary>
        /// Autor: Clausio Elmano de Oliveira
        /// Data: 10/11/2010
        /// Descrição: Pega o valor setado no webconfig (AppSettings)
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        [WebMethod]
        public string PegarValorWebConfig(string valor_chave)
        {
            string xretorno = "";

            try
            {
                xretorno = ConfigurationManager.AppSettings[valor_chave].ToString();
            }
            catch
            {
                xretorno = "erro";
            }

            return xretorno;
        }

        /// <summary>
        /// Autor: Carlos Roweder Nass
        /// Data: 10/10/2011
        /// Descrição: Lista os valores de um PickList de uma entidade
        /// </summary>
        /// <param name = "valor" ></ param >
        /// < returns ></ returns >
        [WebMethod]
        public List<System.Web.UI.WebControls.ListItem> RetornaOpcoesPickList(string entidade, string campo)
        {
            List<System.Web.UI.WebControls.ListItem> retorno = new List<System.Web.UI.WebControls.ListItem>();
            var opcoes = (new CRM2013.Domain.Servicos.RepositoryService()).Conexao.GetOptionSetValues(entidade, campo);
            foreach (var item in opcoes)
                retorno.Add(new System.Web.UI.WebControls.ListItem(item.Value, item.Key.ToString()));
            return retorno;
        }

        /// <summary>
        /// Autor: Carlos Roweder Nass
        /// Data: 17/10/2011
        /// Descrição: Retorna um valor de configuração do Sistema CRM
        //[WebMethod]
        //public ConfiguracaoDeSistema ObterConfiguracaoDoCRM(string atributo)
        //{
        //    var config = new ConfiguracaoDeSistema();
        //    config.OrganizationName = nomeOrganizacao;
        //    return config.ObterConfiguracaoDoCRM(atributo);
        //}

        /// <summary>
        /// Autor: Clausio Elmano de Oliveira
        /// Data: 09/11/2010
        /// Descrição: Verifica se o usuário tem acesso ao Grupo (função/role)
        /// </summary>
        /// <param name="guidUsuario"></param>
        /// <param name="guidGrupo"></param>
        /// <param name="nomeOrganizacao"></param>
        /// <returns></returns>
        //[WebMethod]
        //public XmlDocument VerificaPermissaoUsuarioGrupoAcesso(string guidUsuario, string guidGrupoAcesso)
        //{

        //    try
        //    {
        //        var objAutorizador = new Autorizador();

        //        base.Mensageiro.AdicionarTopico("TemAutorizacao", objAutorizador.UsuarioTemAutorizacaoNoGrupoAcesso(new Guid(guidUsuario), new Guid(guidGrupoAcesso), nomeOrganizacao));
        //        base.Mensageiro.AdicionarTopico("Sucesso", true);
        //    }
        //    catch (Exception erro)
        //    {
        //        return base.TratarErro("Não foi possível fazer a verificação de permissão pelo seguinte motivo: " + erro.Message,
        //           erro, "VerificaPermissaoUsuario");
        //    }



        //    return base.Mensageiro.Mensagem;

        //}

        /// <summary>
        /// Autor: Gabriel Dias Junckes
        /// Data: 20/01/2011
        /// Descrição: Consulta ao ERP para saber se o cliente usa ALC
        /// </summary>
        /// <param name="cidade"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument CidadeZonaFranca(string cidade, string uf)
        {
            Boolean resultado = false;
            Domain.Servicos.HelperWS.IntelbrasService.BuscarCidadeZF(cidade, uf, out resultado);
            base.Mensageiro.AdicionarTopico("Sucesso", true);
            base.Mensageiro.AdicionarTopico("ZonaFranca", resultado);
            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument VerificarDataDeVigencia(string contratoId, string clienteId, string enderecoId, DateTime dataInicioVigencia)
        {
            try
            {
                Contrato contrato = (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.Retrieve(new Guid(contratoId));
                Domain.Model.Conta cliente = new Domain.Model.Conta(nomeOrganizacao, false) { Id = new Guid(clienteId) };
                Domain.Model.Endereco endereco = new Domain.Model.Endereco(nomeOrganizacao, false) { Id = new Guid(enderecoId) };
                base.Mensageiro.AdicionarTopico("EstaVigente", contrato.EstaVigente(cliente, endereco, dataInicioVigencia));
                base.Mensageiro.AdicionarTopico("Sucesso", true);
            }
            catch (ArgumentException ex) { return base.TratarErro(ex.Message, ex, "VerificarDataDeVigencia"); }
            catch (Exception ex)
            {
                //LogService.GravaLog(ex, TipoDeLog.WSVendasIsol, "VerificarDataDeVigencia(string contratoId, string clienteId, string enderecoId, DateTime dataInicioVigencia)");
                return base.TratarErro("Não foi possível validar a vigencia do contrato pelo seguinte motivo: " + ex.Message, ex, "VerificarDataDeVigencia");
            }

            return base.Mensageiro.Mensagem;
        }

        [WebMethod]
        public XmlDocument PesquisarRepresentantePorCodigo(string codigoId)
        {
            Domain.Model.Contato representante = null;
            GrupoDoCliente grupo = (new CRM2013.Domain.Servicos.RepositoryService()).GrupoDoCliente.Retrieve(new Guid(codigoId));
            if (grupo != null)
            {
                var lista = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ListarPorCodigoRepresentante(grupo.Codigo.Value.ToString());
                if (lista != null && lista.Count > 0)
                    representante = lista[0];
            }
            ///representante = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarRepresentantePor(grupo.Codigo.Value);

            base.Mensageiro.AdicionarTopico("Sucesso", true);
            base.Mensageiro.AdicionarTopico("TemRepresentante", (representante != null));
            base.Mensageiro.AdicionarTopico("CodigoRepresentante", (representante != null) ? representante.Id : Guid.Empty);
            base.Mensageiro.AdicionarTopico("NomeRepresentante", (representante != null ? representante.NomeCompleto : string.Empty));

            return base.Mensageiro.Mensagem;
        }

        /// <summary>
        /// Autor:     Gabriel Dias Junckes
        /// Data:      09/02/2011
        /// Descrição: Apaga os Clintes participante e Endereços do Contrato de Destino, Depois o todos do contrado de origem.
        /// </summary>
        /// <param name="contratoOrigemId"></param>
        /// <param name="contratoDestinoId"></param>
        /// <param name="organizacao"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument CopiarClientesParticipantes(string contratoOrigemId, string contratoDestinoId)
        {
            try
            {
                if (contratoDestinoId == string.Empty)
                    throw new Exception("O Guid do contrato de destino é inválido");

                if (contratoOrigemId == string.Empty)
                    throw new Exception("O Guid do contrato de origem é inválido");

                Contrato contrato = new Contrato(nomeOrganizacao, false);

                Guid contratoDestino = new Guid(contratoDestinoId);
                Guid contratoOrigem = new Guid(contratoOrigemId);

                //Thread workerThread = new Thread(() => contrato._CopiarClientesParticipantesPor(contratoOrigem, contratoDestino));
                Thread workerThread = new Thread(() => contrato.EfetuaCopiaClientesParticipantesPor(contratoOrigem, contratoDestino));

                //workerThread.Priority = ThreadPriority.Highest;

                workerThread.Start();


                base.Mensageiro.AdicionarTopico("Sucesso", true);

                return base.Mensageiro.Mensagem;
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível Copiar os Clientes Participantes Por : " + erro.Message, erro, "PesquisarLinhaDeContrato");
            }
        }



        /// <summary>
        /// Autor: Filipe de Campos Cavalcante
        /// Data: 11/11/2010
        /// Realiza a pesquisa da linha de contrato (contractdetail) para poder salvar a ocorrencia
        /// Retorna sempre o primeiro registro encontrado.
        /// </summary>
        /// <param name="contratoId"></param>
        /// <param name="organizacao"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument PesquisarLinhaDeContrato(string contratoId)
        {
            try
            {
                if (contratoId == string.Empty) throw new Exception("O Guid do contrato é inválido");

                Contrato contrato = new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) };
                LinhaDeContrato linhaDeContrato = (new CRM2013.Domain.Servicos.RepositoryService()).LinhaDoContrato.ObterPor(contrato);
                var temLinhaDeContrato = (linhaDeContrato != null);

                base.Mensageiro.AdicionarTopico("Sucesso", true);
                base.Mensageiro.AdicionarTopico("TemLinhaDeContrato", temLinhaDeContrato);

                if (temLinhaDeContrato)
                {
                    base.Mensageiro.AdicionarTopico("LinhaDeContratoId", linhaDeContrato.Id.ToString());
                    base.Mensageiro.AdicionarTopico("Nome", linhaDeContrato.Nome);
                }

                return base.Mensageiro.Mensagem;
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível recuperar a Linha de Contrato pelo seguinte motivo: " + erro.Message, erro, "PesquisarLinhaDeContrato");
            }
        }

        [WebMethod]
        public int TipoAcessoPortal(string login)
        {
            try
            {
                Domain.Model.Contato contato = new Domain.Model.Contato(nomeOrganizacao, false);
                int cont = contato.TipoAcessoPortal(login);
                return cont;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        [WebMethod]
        public int TipoAcessoAstec(string login)
        {
            try
            {
                TipoPosto tipoPosto = new TipoPosto(nomeOrganizacao, false);
                int tipo = tipoPosto.TipoAcessoRelatorio(login);
                return tipo;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        [WebMethod]
        public Moeda ObterMoedaPor(string simbolo)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Moeda.ObterPor(simbolo);
        }

        [WebMethod]
        public XmlDocument PesquisarContatoPor(string cpfCnpj)
        {
            Domain.Model.Contato contato = (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ObterPor(cpfCnpj);

            if (contato != null)
            {
                base.Mensageiro.AdicionarTopico("Id", contato.Id);
                base.Mensageiro.AdicionarTopico("TipoDeRelacaoNme", contato.TipoRelacao.Value);
                base.Mensageiro.AdicionarTopico("DataDeModificacao", contato.ModificadoEm.Value.ToString("yyyy/MM/dd"));
                base.Mensageiro.AdicionarTopico("Nome", contato.Nome);
                base.Mensageiro.AdicionarTopico("Cpf", contato.CpfCnpj);
                base.Mensageiro.AdicionarTopico("Email", contato.Email1);
                base.Mensageiro.AdicionarTopico("Endereco", contato.Endereco1Rua);
                base.Mensageiro.AdicionarTopico("EnderecoNumero", contato.Endereco1Numero);

                base.Mensageiro.AdicionarTopico("CidadeId", (contato.Endereco1Municipioid == null) ? "" : contato.Endereco1Municipioid.Id.ToString());
                base.Mensageiro.AdicionarTopico("PaisId", (contato.Endereco1Pais == null) ? "" : contato.Endereco1Pais.Id.ToString());
                base.Mensageiro.AdicionarTopico("ClienteId", (contato.AssociadoA == null) ? "" : contato.AssociadoA.Id.ToString());
                base.Mensageiro.AdicionarTopico("Telefone", (contato.TelefoneComercial == null) ? "" : contato.TelefoneComercial);

                base.Mensageiro.AdicionarTopico("Achou", true);
            }
            else
                base.Mensageiro.AdicionarTopico("Achou", false);


            return base.Mensageiro.Mensagem;
        }

        #region OCORRENCIAS

        [WebMethod]
        public Ocorrencia ObterOcorrenciaPor(string ocorrenciaId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrenciaId));
            //var ocorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Retrieve(new Guid(ocorrenciaId));
            //ocorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.CarregarCamposRelacionadosOcorrencia(ocorrencia);
            //return  ocorrencia;
        }

        [WebMethod]
        public Ocorrencia ObterOcorrenciaPelo(string numeroDaocorrencia)
        {
            var ocorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ObterPor(numeroDaocorrencia);
            return ocorrencia;
        }

        [WebMethod]
        public OcorrenciaBase ObterOcorrenciaBasePelo(string numeroDaocorrencia)
        {
            var ocorrencia = (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ObterPor(numeroDaocorrencia);
            return ocorrencia;
        }

        [WebMethod]
        public bool ExcluirOcorrenciaPor(string ocorrenciaId)
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.Delete(new Guid(ocorrenciaId));
            return true;
        }

        [WebMethod]
        public List<Ocorrencia> ListarOcorrenciasPorCampos(string notaFiscal, string numeroDeSerieDoProduto, string numeroDaOcorrencia, string postoDeServicoId, int status, int tipoDeOcorrencia, string nomeDoCliente, DateTime dataInicial, DateTime dataFinal)
        {
            var ocorrencias = (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarPor(notaFiscal, numeroDeSerieDoProduto, numeroDaOcorrencia, postoDeServicoId, status, tipoDeOcorrencia, nomeDoCliente, dataInicial, dataFinal);
            var x = ocorrencias.Count;
            return ocorrencias;
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarOcorrenciasPorCpfCnpjCliente(string cpfCnpj, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarPorCpfCnpjCliente(cpfCnpj, dtAberturaIncio, dtAberturaFim, status, assistenciaTecnica, origem);
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarOcorrenciasPorNomeCliente(string nomeCliente, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarPorNomeCliente(nomeCliente, dtAberturaIncio, dtAberturaFim, status, assistenciaTecnica, origem);
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarOcorrenciasPorNumeroDeSerie(string numeroSerie, Domain.Model.Conta assistenciaTecnica, string[] status, string[] origem)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarPorNumeroSerie(numeroSerie, assistenciaTecnica, status, origem);
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarOcorrenciasPorStatus(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarPorStatus(dtAberturaIncio, dtAberturaFim, status, assistenciaTecnica, origem);
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarPorStatusDiagnostico(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, string[] statusDiagnostico, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarPorStatusDiagnostico(dtAberturaIncio, dtAberturaFim, status, statusDiagnostico, assistenciaTecnica, origem);
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarOcorrenciasPorNotaFiscalDeCompra(string notaFical, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarPorNotaFiscalCompra(notaFical, dtAberturaIncio, dtAberturaFim, status, assistenciaTecnica, origem);
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarOcorrenciasPorExtratoDePagamento(Extrato extrato, Domain.Model.Conta assistenciaTecnica)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarPor(extrato, assistenciaTecnica);
        }

        [WebMethod]
        public List<OcorrenciaBase> ListarOcorrenciasComIntervencaoTecnicaEmAberto(Domain.Model.Conta assistenciaTecnica)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).OcorrenciaBase.ListarOcorrenciaAbertaComIntervencaoTecnica(assistenciaTecnica);
        }

        [WebMethod]
        public bool OcorrenciaTemIntervencaoAtiva(Ocorrencia ocorrencia)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Intervencao.OcorrenciaTemIntervencaoAtiva(ocorrencia);
        }

        [WebMethod]
        public IntervencaoTecnica ObterIntervencaoTecnicaPor(Ocorrencia ocorrencia)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Intervencao.ObterPor(ocorrencia);
        }

        [WebMethod]
        public List<Domain.Model.Endereco> ListarEnderecosPor(string contratoId, string clienteid)
        {
            var contrato = new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) };
            var cliente = new Domain.Model.Conta(nomeOrganizacao, false) { Id = new Guid(clienteid) };

            return contrato.ObterParticipantesPor(contrato, cliente);
        }

        [WebMethod]
        public List<Auditoria> ListarAuditoriaPor(Ocorrencia ocorrencia)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Auditoria.ListarPor(ocorrencia);
        }

        [WebMethod]
        public Guid SalvarAuditoria(Auditoria auditoria)
        {
            if (auditoria.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Auditoria.Create(auditoria);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Auditoria.Update(auditoria);
                return auditoria.Id;
            }
        }

        [WebMethod]
        public List<Ocorrencia> ListarOcorrenciasPor(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarPor(cliente);
        }

        [WebMethod]
        public List<Ocorrencia> ListarOcorrenciasEspeciaisPor(Domain.Model.Fatura notaFiscal)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ListarOcorrenciasEspeciaisPor(notaFiscal);
        }

        [WebMethod]
        public string ObterOcorrenciaPorNumeroSerie(string numeroDeSerie)
        {
            try
            {
                Domain.Integracao.MSG0315 msg0315 = new Domain.Integracao.MSG0315(nomeOrganizacao, false);
                Ocorrencia ocorrencia = new Ocorrencia();
                ocorrencia.ProdutosDoCliente = numeroDeSerie;
                var result = msg0315.Enviar(ocorrencia);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);

                return JsonConvert.SerializeXmlNode(doc);
            }
            catch (Exception erro)
            {
                var msgRetorno = new
                {
                    Sucesso = false,
                    Mensagem = "Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                };

                return JsonConvert.SerializeObject(msgRetorno);
            }
        }

        #endregion

        #region CONTRATOS

        [WebMethod]
        public Contrato PesquisarContratoPor(string contratoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.Retrieve(new Guid(contratoId));
        }

        [WebMethod]
        public LinhaDeContrato ObterLinhaDeContratoPor(string contratoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).LinhaDoContrato.Retrieve(new Guid(contratoId));
        }

        [WebMethod]
        public List<LinhaDeContrato> ListarLinhaDeContratoPor(string contratoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).LinhaDoContrato.ListarPor(new Contrato() { Id = new Guid(contratoId) });
        }

        [WebMethod]
        public List<Contrato> ListarContratosAssociados(string contatoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contrato.ListarPorPerfilUsuarioServico(new Domain.Model.Contato(nomeOrganizacao, false) { Id = new Guid(contatoId) });
        }

        [WebMethod]
        public List<Domain.Model.Conta> ListarClientesAssociadosPor(string contatoId, string contratoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ListarPor(new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) });
        }

        [WebMethod]
        public List<ClienteParticipanteEndereco> PesquisaClientesAssociadosPor(string contratoId, string razaosocial, string cnpj, string nomefantasia, string endereco)
        {
            Contrato contrato = new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) };
            Domain.Model.Conta cliente = new Domain.Model.Conta(nomeOrganizacao, false)
            {
                Nome = razaosocial,
                CpfCnpj = cnpj,
                NomeFantasia = nomefantasia
            };
            return (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.ListarPor(cliente, contrato, endereco);
        }

        [WebMethod]
        public XmlDocument PesquisarEnderecosParticipantes(string contratoId, string clienteId)
        {
            try
            {
                if (contratoId == string.Empty)
                    throw new Exception("O Guid do contrato é inválido");
                if (clienteId == string.Empty)
                    throw new Exception("O Guid do cliente é inválido");

                Contrato contrato = new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) };
                Domain.Model.Conta cliente = new Domain.Model.Conta(nomeOrganizacao, false) { Id = new Guid(clienteId) };


                var clientesParticipantes = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.ListarPor(cliente, contrato);
                var countSemLocalidade = (from c in clientesParticipantes
                                          where c.LocalidadeId == null
                                          select c).ToList().Count;

                base.Mensageiro.AdicionarTopico("Sucesso", true);
                base.Mensageiro.AdicionarTopico("TotalSemLocalidade", countSemLocalidade);

                return base.Mensageiro.Mensagem;
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível verificar a quantidade de endereços sem localidade pelo seguinte motivo: " + erro.Message, erro, "PesquisarEnderecosParticipantes");
            }
        }

        [WebMethod]
        public XmlDocument ExcluirEnderecosSemLocalidade(string contratoId, string clienteId)
        {
            try
            {
                if (contratoId == string.Empty)
                    throw new Exception("O Guid do contrato é inválido");
                if (clienteId == string.Empty)
                    throw new Exception("O Guid do cliente é inválido");

                Contrato contrato = new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) };
                Domain.Model.Conta cliente = new Domain.Model.Conta(nomeOrganizacao, false) { Id = new Guid(clienteId) };


                var clientesParticipantes = (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.ListarPor(cliente, contrato);
                var semLocalidades = (from c in clientesParticipantes
                                      where c.Localidade == null
                                      select c).ToList();

                foreach (ClienteParticipanteEndereco end in semLocalidades)
                    (new CRM2013.Domain.Servicos.RepositoryService()).ClienteParticipanteDoEndereco.Delete(end.Id);

                base.Mensageiro.AdicionarTopico("Sucesso", true);

                return base.Mensageiro.Mensagem;
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível excluir o endereços sem localidade pelo seguinte motivo: " + erro.Message, erro, "ExcluirEnderecosSemLocalidade");
            }
        }

        [WebMethod]
        public List<LinhaDeContrato> ListarLinhasDeContratoPor(string contratoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).LinhaDoContrato.ListarPor(new Contrato(nomeOrganizacao, false) { Id = new Guid(contratoId) });
        }

        #endregion

        #region CLIENTES
        [WebMethod]
        public Domain.Model.Conta ObterClientePor(string clienteId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.Retrieve(new Guid(clienteId));
        }

        [WebMethod]
        public Domain.Model.Conta ObterClientePelo(string cpfOuCnpj)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterPor(cpfOuCnpj);
        }

        [WebMethod]
        public Domain.Model.Conta ObterClientePeloCodigo(int codigoCliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarPor(codigoCliente);
        }

        [WebMethod]
        public Domain.Model.Conta ObterClientePela(Ocorrencia ocorrencia)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterPor(ocorrencia);
        }

        [WebMethod]
        public Domain.Model.Contato ObterContatoPor(string login, Guid contatoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ObterPor(login, contatoId);
        }

        [WebMethod]
        public Domain.Model.Contato ObterContatoDuplicadoPor(string cpf, string login)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ObterPorDuplicidade(cpf, login);
        }

        [WebMethod]
        public Guid SalvarContato(Domain.Model.Contato contato)
        {
            if (contato.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.Create(contato);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Contato.Update(contato);
                return contato.Id;
            }
        }

        [WebMethod]
        public Domain.Model.Contato ObterContatoPelo(string cpfOuCnpj)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ObterPor(cpfOuCnpj, "");
        }

        [WebMethod]
        public List<Domain.Model.Contato> ListarContatosPor(Domain.Model.Contato contato)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ListarContatosPor(contato);
        }

        [WebMethod]
        public List<Domain.Model.Contato> ListarContatosPela(Domain.Model.Conta conta)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ListarContatosPor(conta);
        }

        [WebMethod]
        public Usuario ObterUsuarioPor(string login, Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Usuario.ObterPor(login);
            else
                return (new CRM2013.Domain.Servicos.RepositoryService()).Usuario.Retrieve(usuarioId);
        }

        [WebMethod]
        public Categoria BuscarCategoriaClientePor(int codigo, UnidadeNegocio unidadeNegocio)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.PesquisarCategoriaCliente(codigo, unidadeNegocio);
        }

        [WebMethod]
        public XmlDocument PesquisaClientePor(string clienteId)
        {
            try
            {
                var cliente = (new CRM2013.Domain.Servicos.RepositoryService()).Conta.Retrieve(new Guid(clienteId));

                base.Mensageiro.AdicionarTopico("NomeFantasia", cliente.NomeFantasia);
                base.Mensageiro.AdicionarTopico("Codigo", cliente.CodigoEms);
                base.Mensageiro.AdicionarTopico("PrestacaoServicoIsol", cliente.PrestacaoServicoIsol.ToString());
                base.Mensageiro.AdicionarTopico("Sucesso", true);
            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível encontrar o Cliente pelo seguinte motivo: " + erro.Message, erro, "VerificaParametros");
            }


            return base.Mensageiro.Mensagem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contratoId"></param>
        /// <param name="clienteId"></param>
        /// <param name="nomeOrganizacao"></param>
        /// <returns></returns>
        [WebMethod]
        public XmlDocument PesquisarNumeroEndereco(string enderecoid)
        {
            try
            {
                var numeroEndereco = (new CRM2013.Domain.Servicos.RepositoryService()).Endereco.ObterPorEnderecoId(enderecoid);

                if (numeroEndereco != null && numeroEndereco.EnderecoNumero != null)
                {

                    base.Mensageiro.AdicionarTopico("Numero", numeroEndereco.Numero);
                    base.Mensageiro.AdicionarTopico("Achou", true);
                    base.Mensageiro.AdicionarTopico("Sucesso", true);

                    return base.Mensageiro.Mensagem;
                }
                else
                {
                    base.Mensageiro.AdicionarTopico("Achou", false);
                }

            }
            catch (Exception erro)
            {
                return base.TratarErro("Não foi possível realizar a pesquisa pelo seguinte motivo: " + erro.Message + " - " + erro.StackTrace.ToString(),
                   erro, "PesquisarNumeroEndereco");
            }

            return base.Mensageiro.Mensagem;


        }

        [WebMethod]
        public List<Domain.Model.Conta> ListarClientesPorAtributos(Domain.Model.Conta cliente)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ListarPorAtributos(cliente);
        }
        #endregion

        #region FIDELIDADE
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ListarExtratoFidelidaadeInvalidosAtivos(int pagina, int quantidade, ref string pagingCookie, ref bool moreRecords)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterInvalidosAtivos(pagina, quantidade, ref pagingCookie, ref moreRecords);
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ListarExtratoFidelidaadeNaoValidosEAtivos(DateTime aPartiDe, int pagina, int quantidade, ref string pagingCookie, ref bool moreRecords)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterNaoValidadosAtivos(aPartiDe, pagina, quantidade, ref pagingCookie, ref moreRecords);
        }
        [WebMethod]
        public List<GrupoPremio> ListarGrupoPremioFidelidade()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).GrupoPremio.ListarTodos();
        }
        [WebMethod]
        public string ValidarNumeroSerieKeyCode(Domain.Model.ExtratoFidelidade extratoFidelidade)
        {
            return extratoFidelidade.ValidarNumeroSerieKeyCode();
        }
        [WebMethod]
        public Guid SalvarExtratoFidelidade(Domain.Model.ExtratoFidelidade extratoFidelidade)
        {
            if (extratoFidelidade.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.Create(extratoFidelidade);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.Update(extratoFidelidade);
                return extratoFidelidade.Id;
            }
        }
        [WebMethod]
        public Guid SalvarProcessamentoFidelidade(Domain.Model.ProcessamentoFidelidade processamentoFidelidade)
        {
            if (processamentoFidelidade.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeProcessamento.Create(processamentoFidelidade);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeProcessamento.Update(processamentoFidelidade);
                return processamentoFidelidade.Id;
            }
        }
        [WebMethod]
        public Guid SalvarEmail(Domain.Model.Email email)
        {
            if (email.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Email.Create(email);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Email.Update(email);
                return email.Id;
            }
        }
        [WebMethod]
        public Guid SalvarResgateFidelidade(Domain.Model.Resgate resgateFidelidade)
        {
            if (resgateFidelidade.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).Resgate.Create(resgateFidelidade);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).Resgate.Update(resgateFidelidade);
                return resgateFidelidade.Id;
            }
        }
        [WebMethod]
        public Guid SalvarRelacionamentoCliente(RelacionamentoCliente relacionamentoCliente)
        {
            if (relacionamentoCliente.Id == Guid.Empty)
                return (new CRM2013.Domain.Servicos.RepositoryService()).RelacionamentoCliente.Create(relacionamentoCliente);
            else
            {
                (new CRM2013.Domain.Servicos.RepositoryService()).RelacionamentoCliente.Update(relacionamentoCliente);
                return relacionamentoCliente.Id;
            }
        }
        [WebMethod]
        public List<RelacionamentoCliente> ListarParticipanteFidelidadePor(Guid contaId, Guid funcao1Id, Guid funcao2Id)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).RelacionamentoCliente.ListarParticipantePor(contaId, funcao1Id, funcao2Id);
        }
        [WebMethod]
        public List<RelacionamentoCliente> ObterRelacionamentoClienteFidelidadePor(Guid participanteUmId, Guid funcaoRelacionamentoUmId, Guid participanteDoisId, Guid funcaoRelacionamentoDoisId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).RelacionamentoCliente.ObterPor(participanteUmId, funcaoRelacionamentoUmId, participanteDoisId, funcaoRelacionamentoDoisId);
        }
        [WebMethod]
        public void ExcluirRelacionamentoClienteFidelidade(Guid relacionamentoId)
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).RelacionamentoCliente.Delete(relacionamentoId);
        }
        [WebMethod]
        public List<Domain.Model.Contato> ListarTodosConattosComExtratoFidelidade(int quantidade, int pagina, string cookie)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ObterTodosComExtratoFidelidade(quantidade, pagina, cookie);
        }
        [WebMethod]
        public void AssociarExtratoFidelidade(Domain.Model.ExtratoFidelidade extrato, Domain.Model.Resgate resgate)
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.Associar(extrato, resgate);
        }
        [WebMethod]
        public List<Pontuacao> ObterListaCompletaVigenciaValida()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadePontuacao.ObterListaCompletaVigenciaValida();
        }
        [WebMethod]
        public List<Pontuacao> ListarPontuacaoFidelidadePor(Guid produtoId, DateTime inicio, DateTime? fim, Lookup pais, Lookup estado, Lookup distribuidor, Guid? pontuacaoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadePontuacao.ObterPor(produtoId, inicio, fim, pais, estado, distribuidor, pontuacaoId);
        }
        [WebMethod]
        public List<PremioFidelidade> ObterListaCompletaPremioFidelidade()
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadePremio.ListarTodos();
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterExtratoPontosValidos(Guid contatoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterExtratoPontosValidos(contatoId);
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterExtratosFidelidadePorContato(Guid contatoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterExtratosContato(contatoId);
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterExtratoPontosAExpirar(Guid contatoId, int dias)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterExtratoPontosAExpirar(contatoId, dias);
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterTodosExtratosPontosAExpirar(int pagina, int dias)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterExtratoPontosAExpirar(pagina, dias);
        }
        [WebMethod]
        public List<Domain.Model.Conta> ObterRevendasPor(Guid distribuidorId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Conta.ObterRevendas(distribuidorId);
        }
        [WebMethod]
        public List<Domain.Model.Contato> ObterVendedoresPor(Guid distribuidorId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Contato.ObterVendedores(distribuidorId);
        }
        [WebMethod]
        public FuncaoRelacionamento ObterFuncaoRelacionamentoPor(Domain.Enum.Contato.TipoAcesso nomeFuncao)
        {
            return (new FuncaoRelacionamento())[nomeFuncao];
        }

        [WebMethod]
        public void CarregaElementos(WalmartClientePedido wlc, WalmartEnderecoPedido wle, WalmartItemPedido wli, WalmartPedido wlp, PremioFidelidade pf, FuncaoRelacionamento fr, Domain.Model.Email mail)
        {

        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterExtratoFidelidadePorNumeroSerie(string numeroDeSerie)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterExtratoPorNumeroSerie(numeroDeSerie);
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterPontosClienteFidelidade(Guid clienteId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterPontosCliente(clienteId);
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterVendedoresPontuadosPorDistribuidor(Guid revenda, Guid distribuidorId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterVendedoresPontuadosPorDistribuidor(revenda, distribuidorId);
        }
        [WebMethod]
        public List<Domain.Model.ExtratoFidelidade> ObterExtratoPontosExpirados(Guid contatoId, int dias)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).FidelidadeExtrato.ObterExtratoPontosExpirados(contatoId, dias);
        }
        [WebMethod]
        public List<Domain.Model.Resgate> ObterResgateParceiroFidelidade(Domain.Enum.Resgate.RazaoDoStatus statuscode, int count, int pageNumber, string cookie)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Resgate.ObterPorResgateParceiro(statuscode, count, pageNumber, cookie);
        }
        [WebMethod]
        public List<Domain.Model.Resgate> ObterResgatePorRevendaFidelidade(Guid revendaId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Resgate.ObterPorRevenda(revendaId);
        }
        [WebMethod]
        public List<Domain.Model.Resgate> ObterResgatePorContatoFidelidade(Guid contatoId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Resgate.ObterPorContato(contatoId);
        }
        [WebMethod]
        public List<Domain.Model.Product> ListarProdutosPorClienteExtratoFidelidade(Guid revendaId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ObterPorClienteExtratoFidelidade(revendaId);
        }
        [WebMethod]
        public List<Domain.Model.Product> ListarProdutosPorVendedorExtratoFidelidade(Guid vendedorId)
        {
            return (new CRM2013.Domain.Servicos.RepositoryService()).Produto.ObterPorVendedorExtratoFidelidade(vendedorId);
        }
        [WebMethod]
        public void InativarTodosProdutosResgatadosFidelidade(Domain.Model.Resgate resgate)
        {
            resgate.InativarTodosProdutosResgatadosFidelidade();
        }
        [WebMethod]
        public Pontuacao ObterPontuacaoFidelidadePor(Guid produtoId, Guid distribuidorId, Domain.Model.Contato usuario)
        {
            return new Pontuacao()[produtoId, distribuidorId, usuario];
        }
        [WebMethod]
        public void AlterarStatus(string entidade, Guid id, int status, int razaoStatus)
        {
            (new CRM2013.Domain.Servicos.RepositoryService()).Conta.AlterarStatus(entidade, id, status, razaoStatus);
        }
        #endregion

        #region Métodos Privados
        private void MontarEstruturaRetornadaDoEMS(Domain.Model.Conta cliente)
        {

            base.Mensageiro.AdicionarTopico("Bairro", cliente.Endereco1Bairro);
            base.Mensageiro.AdicionarTopico("Cep", cliente.Endereco1CEP);
            base.Mensageiro.AdicionarTopico("Cidade", cliente.Endereco1Cidade);
            base.Mensageiro.AdicionarTopico("Logradouro", cliente.Endereco1Rua);
            base.Mensageiro.AdicionarTopico("TipoDoEndereco", (int)Domain.Enum.TipoDeEndereco.Primario);
            base.Mensageiro.AdicionarTopico("Uf", cliente.Endereco1Estado);
            base.Mensageiro.AdicionarTopico("Pais", cliente.Endereco1Pais);

            base.Mensageiro.AdicionarTopico("BairroCobranca", cliente.Endereco2Bairro);
            base.Mensageiro.AdicionarTopico("CepCobranca", cliente.Endereco2CEP);
            base.Mensageiro.AdicionarTopico("CidadeCobranca", cliente.Endereco2Cidade);
            base.Mensageiro.AdicionarTopico("LogradouroCobranca", cliente.Endereco2Rua);
            base.Mensageiro.AdicionarTopico("TipoDoEnderecoDeCobranca", (int)Domain.Enum.TipoDeEndereco.Cobranca);
            base.Mensageiro.AdicionarTopico("UfCobranca", cliente.Endereco2Estado);
            base.Mensageiro.AdicionarTopico("PaisCobranca", cliente.Endereco2Pais);

            base.Mensageiro.AdicionarTopico("Nome", cliente.Nome);
            base.Mensageiro.AdicionarTopico("NomeAbreviado", cliente.NomeAbreviado);

            base.Mensageiro.AdicionarTopico("NomeDaMatriz", cliente.NomeAbreviadoMatrizEconomica);
            base.Mensageiro.AdicionarTopico("CodigoDaMatriz", cliente.CodigoMatriz);
            //base.Mensageiro.AdicionarTopico("MatrizID", cliente.);

            base.Mensageiro.AdicionarTopico("Tipo", cliente.TipoConta.Value);
            base.Mensageiro.AdicionarTopico("Natureza", cliente.Natureza.Value);
            base.Mensageiro.AdicionarTopico("CodigoEms", cliente.CodigoMatriz);

            base.Mensageiro.AdicionarTopico("Telefone", cliente.Telefone);
            base.Mensageiro.AdicionarTopico("Ramal", cliente.RamalTelefonePrincipal);
            base.Mensageiro.AdicionarTopico("Fax", cliente.Fax);
            base.Mensageiro.AdicionarTopico("RamalFax", cliente.RamalFax);

            base.Mensageiro.AdicionarTopico("OutroTelefone", cliente.TelefoneAlternativo);
            base.Mensageiro.AdicionarTopico("OutroRamal", cliente.RamalOutroTelefone);

            base.Mensageiro.AdicionarTopico("Email", cliente.Email);

            base.Mensageiro.AdicionarTopico("NomeRepresentante", cliente.Representante == null ? "" : cliente.Representante.Name);
            base.Mensageiro.AdicionarTopico("RepresentanteId", cliente.Representante == null ? "" : cliente.Representante.Id.ToString());

            base.Mensageiro.AdicionarTopico("NomeDoGrupo", cliente.Grupo == null ? "" : cliente.Grupo.Name);
            base.Mensageiro.AdicionarTopico("GrupoId", cliente.Grupo == null ? "" : cliente.Grupo.Id.ToString());

            base.Mensageiro.AdicionarTopico("EcontribuinteDeICMS", cliente.ContribuinteICMS);
            base.Mensageiro.AdicionarTopico("CodigoSufurama", cliente.CodigoSuframa);


            base.Mensageiro.AdicionarTopico("NomeTransportadora", cliente.Transportadora == null ? "" : cliente.Transportadora.Name);
            base.Mensageiro.AdicionarTopico("TransportadoraId", cliente.Transportadora == null ? "" : cliente.Transportadora.Id.ToString());

            //base.Mensageiro.AdicionarTopico("NomeDoCanalDeVenda", cliente.CanalDeVenda == null ? "" : cliente.);
            //base.Mensageiro.AdicionarTopico("CanalDeVendaId", cliente.CanalDeVenda == null ? "" : cliente.CanalDeVenda.Id.ToString());

            base.Mensageiro.AdicionarTopico("InscricaoAuxiliarDeSubstituicao", cliente.InscricaoAuxiliarDeSubstituicao);
            base.Mensageiro.AdicionarTopico("OptanteDeSuspencaoIPI", cliente.OptanteDeSuspensaoDeIPI);

            base.Mensageiro.AdicionarTopico("AgenteRetencao", cliente.AgenteRetencao);
            base.Mensageiro.AdicionarTopico("PisConfinsPorUnidade", cliente.PisConfinsPorUnidade);

            base.Mensageiro.AdicionarTopico("RecebeNFE", cliente.RecebeNFE);
            base.Mensageiro.AdicionarTopico("FormaDeTributacao", cliente.FormaTributacao.Value);
            base.Mensageiro.AdicionarTopico("DescontoCAT", cliente.DescontoCAT);
            base.Mensageiro.AdicionarTopico("TipoDeEmbalagem", cliente.TipoEmbalagem);
            base.Mensageiro.AdicionarTopico("Observacao", cliente.ObservacoesNF);
            base.Mensageiro.AdicionarTopico("DispositivoLegal", cliente.DispositivoLegal);


            base.Mensageiro.AdicionarTopico("Incoterm", cliente.Incoterm);
            base.Mensageiro.AdicionarTopico("LocalEmbarque", cliente.LocalEmbarque);
            base.Mensageiro.AdicionarTopico("EmbarqueVia", cliente.EmbarqueVia);


            var dt = DateTime.Now;
            if (cliente.DataImplantacao.HasValue)
                base.Mensageiro.AdicionarTopico("DataDeImplantacao", cliente.DataImplantacao.Value.ToString("MM/dd/yyyy hh:mm:ss"));
            else
                base.Mensageiro.AdicionarTopico("DataDeImplantacao", "[NO DATE]");

            if (cliente.DataLimiteCredito.HasValue)
                base.Mensageiro.AdicionarTopico("DataLimiteDeCredito", cliente.DataLimiteCredito.Value.ToString("MM/dd/yyyy hh:mm:ss"));
            else
                base.Mensageiro.AdicionarTopico("DataLimiteDeCredito", "[NO DATE]");

            if (cliente.DataVenctoConcessao.HasValue)
                base.Mensageiro.AdicionarTopico("DataVencimentoConcessao", cliente.DataVenctoConcessao.Value.ToString("MM/dd/yyyy hh:mm:ss"));
            else
                base.Mensageiro.AdicionarTopico("DataVencimentoConcessao", "[NO DATE]");


            base.Mensageiro.AdicionarTopico("LimiteDeCredito", cliente.LimiteCredito.Value);
            base.Mensageiro.AdicionarTopico("NomeDoPortador", cliente.Portador == null ? "" : cliente.Portador.Name);
            base.Mensageiro.AdicionarTopico("PortadorId", cliente.Portador == null ? "" : cliente.Portador.Id.ToString());

            base.Mensageiro.AdicionarTopico("ContaCorrente", cliente.ContaCorrente);
            base.Mensageiro.AdicionarTopico("Agencia", cliente.Agencia);
            base.Mensageiro.AdicionarTopico("Banco", cliente.Banco);

            base.Mensageiro.AdicionarTopico("NomeDaReceitaPadrao", cliente.ReceitaPadrao == null ? "" : cliente.ReceitaPadrao.Name);
            base.Mensageiro.AdicionarTopico("ReceitaPadraoId", cliente.ReceitaPadrao == null ? "" : cliente.ReceitaPadrao.Id.ToString());

            base.Mensageiro.AdicionarTopico("CalculaMulta", cliente.CalculaMulta);

            base.Mensageiro.AdicionarTopico("NomeDaCondicaoDePagamento", cliente.CondicaoPagamento == null ? "" : cliente.CondicaoPagamento.Name);
            base.Mensageiro.AdicionarTopico("CondicaoDePagamentoId", cliente.CondicaoPagamento == null ? "" : cliente.CondicaoPagamento.Id.ToString());

            base.Mensageiro.AdicionarTopico("EmiteBloquete", cliente.EmiteBloqueto);
            base.Mensageiro.AdicionarTopico("GeraAvisoDeCredito", cliente.GeraAvisoCredito);

            base.Mensageiro.AdicionarTopico("RecebeInformacaoSCI", cliente.RecebeInformacaoSCI);
            base.Mensageiro.AdicionarTopico("Modalidade", (int)cliente.Modalidade);
            base.Mensageiro.AdicionarTopico("Cpf", cliente.CpfCnpj);
            base.Mensageiro.AdicionarTopico("Cnpj", cliente.CpfCnpj);
            base.Mensageiro.AdicionarTopico("InscricaoEstadual", cliente.InscricaoEstadual);

        }
        #endregion

    }
}
