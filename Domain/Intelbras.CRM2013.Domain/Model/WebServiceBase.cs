using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Web.Services.Protocols;
using System.Diagnostics;

namespace Intelbras.CRM2013.Domain
{

    public class WebServiceBase : System.Web.Services.WebService
    {
        public string nomeOrganizacao { get { return SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"); } }
        private GerenciadorMensagem.RemoteCommand mensageiro = null;
        private GerenciadorExcecao tratadorDeErros = null;

        protected GerenciadorMensagem.RemoteCommand Mensageiro
        {
            get { return this.mensageiro; }
        }

        public WebServiceBase()
        {
            this.mensageiro = new GerenciadorMensagem.RemoteCommand();
            this.tratadorDeErros = new GerenciadorExcecao();
        }

        protected XmlDocument TratarErro(string mensagemQueSeraExibida, Exception erro, string metodo)
        {

            var mensagemDeErro = this.tratadorDeErros.TratarErro(erro, metodo);

            this.mensageiro.AdicionarTopico("Sucesso", false);
            this.mensageiro.AdicionarTopico("Mensagem", mensagemQueSeraExibida);
            this.mensageiro.AdicionarTopico("MensagemDeErro", mensagemDeErro);

            return this.mensageiro.Mensagem;
        }

    }

    public class GerenciadorMensagem
    {

        /// <summary>
        /// Gerenciador de mensagens de retorno para API RemoteCommand.
        /// </summary>
        public class RemoteCommand
        {

            // Membros privados.
            private XmlDocument documentoXml = null;

            /// <summary>
            /// Objeto que contém a mensagem xml de retorno.
            /// </summary>
            private XmlDocument DocumentoXml
            {
                get
                {

                    if (null == documentoXml)
                    {
                        documentoXml = new XmlDocument();
                        documentoXml.LoadXml(XML_ROOT_NODE);
                    }

                    return documentoXml;
                }
            }

            /// <summary>
            /// Constante contendo o node root para mensagens xml de retorno.
            /// </summary>
            //private const string XML_ROOT_NODE = @"<Intelbras xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""></Intelbras>";
            private const string XML_ROOT_NODE = @"<Intelbras xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/crm/2009/WebServices""></Intelbras>";

            /// <summary>
            /// Cria mensagem de retorno
            /// </summary>
            /// <returns>
            /// Retorna a mensagem formatada.
            /// </returns>
            public void AdicionarTopico(string nome, object valor)
            {

                var node = DocumentoXml.CreateElement(nome);
                node.InnerText = Convert.ToString(valor);
                DocumentoXml.DocumentElement.AppendChild(node);

            }

            /// <summary>
            /// Cria mensagem de retorno
            /// </summary>
            /// <returns>
            /// Acrescenta o XML a mensagem de retorno.
            /// </returns>
            //public void AdicionarTopico(XmlDocument xml)
            //{

            //   DocumentoXml.DocumentElement.AppendChild(xml.DocumentElement);
            //}


            /// <summary>
            /// Mensagem formatada com os tópicos desejados.
            /// </summary>
            public XmlDocument Mensagem
            {
                get
                {
                    return DocumentoXml;
                }
            }
        }

    }

    public enum OrigemAplicacoes
    {
        PortalTreinamento = 1000
    }

    public class GerenciadorExcecao
    {

        public GerenciadorExcecao() { }
        public GerenciadorExcecao(OrigemAplicacoes origem)
        {
            Origem = origem;
        }

        private OrigemAplicacoes origem;
        public OrigemAplicacoes Origem
        {
            get { return origem; }
            set { origem = value; }
        }


        /// <summary>
        /// Trata a mensagem quando ocorre um erro na execução de um método dos web services.
        /// </summary>
        /// <param name="error">Objeto que contém a mensagem com o erro.</param>
        /// <returns>Documento Xml com detalhes da execução;</returns>
        public string TratarErro(Exception erro, string origem)
        {

            var mensagemDeErro = "";

            if (null != erro as SoapException)
            {
                if (null != ((SoapException)erro).Detail)
                {
                    mensagemDeErro = ((SoapException)erro).Detail.InnerText;
                    try
                    {
                        EventLog.WriteEntry("Application", string.Format("Mensagem de Erro: {0} -> StackTrace: {1}", ((SoapException)erro).Detail.InnerText, erro.StackTrace));
                    }
                    catch
                    {
                    }
                }
                else
                {
                    mensagemDeErro = erro.Message;
                    try
                    {
                        EventLog.WriteEntry("Application", string.Format("Mensagem de Erro: {0} -> StackTrace: {1}", erro.Message, erro.StackTrace));
                    }
                    catch
                    {
                    }
                }
            }
            else if (erro != null)
            {
                mensagemDeErro = erro.Message;

                try
                {
                    EventLog.WriteEntry("Application", string.Format("Mensagem de Erro: {0} -> StackTrace: {1}", erro.Message, erro.StackTrace));
                }
                catch
                {
                }
            }

            return mensagemDeErro;
        }

    }
}