using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using System.Net;
using SDKore.Helper;
using System.Diagnostics;
using System.ComponentModel;

namespace SDKore.Configuration
{
    public class ConfigurationManager
    {
        enum Keys
        {
            user,
            password,
            domain
        }

        /// <summary>
        /// Obtém o valor do arquivo de configuração por chave.
        /// </summary>
        /// <param name="key">chave</param>
        /// <param name="attribute">atributo</param>
        /// <returns></returns>
        private static XmlNode GetNode(string node, string key)
        {
            string path = string.Format("//SDKoreSettings/{0}/add[@{1}='{2}']", node, (node == "users" ? "appName" : "key"), key);
            XmlNode xmlNode = null;
            try
            {

                if (InternalXmlHelper.Instance == null || InternalXmlHelper.Instance.Xml == null)
                    throw new ArgumentException("Arquivo de configuração não carregado.");


                xmlNode = InternalXmlHelper.Instance.Xml.SelectSingleNode(path);
                if (xmlNode == null)
                    throw new ArgumentException("Node não encontrado. Path: " + path);
            }
            catch (Exception ex)
            {
                Helper.Error.Handler(ex);
            }

            return xmlNode;
        }

        /// <summary>
        /// Obtém o valor do node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="attribute">Atributo</param>
        /// <returns></returns>
        private static string GetValueBy(XmlNode node, string attribute)
        {
            string valor = string.Empty;
            try
            {
                var tmp = node.Attributes[attribute];
                if (tmp == null)
                {
                    throw new ApplicationException("Atributo não encontrado.");
                }

                valor = tmp.Value;
            }
            catch (Exception ex)
            {
                Helper.Error.Handler(ex);
            }

            return valor;
        }

        /// <summary>
        /// Recupera a url de referencia ao CRM Web Service Offline.
        /// </summary>
        public static string CrmServiceOffline
        {
            get
            {
                return GetValueBy(GetNode("webServices", "CrmServiceOffline"), "url");
            }
        }

        /// <summary>
        /// Recupera a url de referencia ao Banco de dados principal do Crm
        /// </summary>
        public static string CrmDataBase
        {
            get
            {
                return GetValueBy(GetNode("dataBases", "CrmDataBase"), "connectionString");
            }
        }

        /// <summary>
        /// Nome de usuário na chave de users.
        /// </summary>
        /// <param name="organization">Organização</param>
        /// <param name="app">Aplicação (parametro opcional)</param>
        public static string GetUserName(string organization, string app = "CrmAdmin")
        {
            return GetUserValueXML(string.Concat(organization, app), Keys.user, true);
        }

        /// <summary>
        /// Senha na chave de users.
        /// </summary>
        /// <param name="organization">Organização</param>
        /// <param name="app">Aplicação (parametro opcional)</param>
        public static string GetPassword(string organization, string app = "CrmAdmin")
        {
            return GetUserValueXML(string.Concat(organization, app), Keys.password, true);
        }

        /// <summary>
        /// Domínio na chave de users.
        /// </summary>
        /// <param name="organization">Organização</param>
        /// <param name="app">Aplicação (parametro opcional)</param>
        public static string GetDomain(string organization, string app = "CrmAdmin")
        {
            return GetUserValueXML(string.Concat(organization, app), Keys.domain, false);
        }


        /// <summary>
        /// Recupera a url de referencia ao Banco de dados principal do Tridea Essentials.
        /// </summary>
        public static string TrideaEssentialsDataBase
        {
            get
            {
                return GetValueBy(GetNode("dataBases", "TrideaEssentialsDataBase"), "connectionString");
            }
        }

        /// <summary>
        /// Recupera o endereço do CRM para ambientes IFD.
        /// </summary>
        /// <param name="organization">Organização</param>
        /// <returns></returns>
        public static string GetCrmService(string organization)
        {
            return GetValueBy(GetNode("webServices", string.Format("{0}CrmService", organization)), "url");
        }

        /// <summary>
        /// Obtém o valor para o Trace.
        /// <para>Em caso da tag Trace não existir, o valor retornado será 'empty'</para>
        /// </summary>
        /// <returns>On ou Off</returns>
        public static string GetTraceEnable()
        {
            try
            {
                return GetValueBy(GetNode("trace", "Habilitado"), "value");
            }
            catch
            { return string.Empty; }
        }

        /// <summary>
        /// Obtém o diretório para o Trace.
        /// <para>Em caso da tag Trace não existir, o valor retornado será 'empty'</para>
        /// </summary>
        /// <returns>Diretório que o trace será gravado</returns>
        public static string GetTraceDirectory()
        {
            try
            {
                return GetValueBy(GetNode("trace", "Diretorio"), "value");
            }
            catch
            { return string.Empty; }
        }

        /// <summary>
        /// Recupera o namespace do repositório.
        /// </summary>
        public static string GetRepository(string key)
        {
            return GetValueBy(GetNode("repositories", key), "type");
        }

        /// <summary>
        /// Recupera o namespace do dominio.
        /// </summary>
        public static string GetApplicationDomain(string key)
        {
            return GetValueBy(GetNode("domains", key), "type");
        }

        /// <summary>
        /// Obtém uma string de conexão de um banco de dados qualquer configurado na seção database do arquivo de configuração.
        /// </summary>
        /// <param name="key">Nome da chave para a obtenção da connection string do banco de dados desejado.</param>
        /// <returns>String de Conexão do Banco Solicitado.</returns>
        public static string GetDataBase(string key)
        {
            return GetValueBy(GetNode("dataBases", key), "connectionString");
        }



        /// <summary>
        /// Obtém um valor específico da sessão appSettings..
        /// </summary>
        /// <param name="appName">Chave que contém o valor a ser obteido.</param>
        /// <returns>Valor de configuração.</returns>
        public static string GetSettingValue(string key, bool exception = false)
        {
            try
            {
                return GetValueBy(GetNode("appSettings", key), "value");
            }
            catch (Exception ex)
            {
                if (exception)
                {
                    throw ex;
                }
                else
                {
                    Helper.Error.Create(ex, EventLogEntryType.Information);
                }
            }

            return string.Empty;
        }

        public static T GetSettingValue<T>(string key)
        {
            try
            {
                string value = GetValueBy(GetNode("appSettings", key), "value");
                var foo = TypeDescriptor.GetConverter(typeof(T));

                return (T)(foo.ConvertFromInvariantString(value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) Não foi possível encontrar ou convertar o parâmetro '" + key +  "'", ex);
            }
        }

        /// <summary>
        /// Obter valores na chave users
        /// </summary>
        /// <param name="appName">chave appName</param>
        /// <param name="key">atributo a retornar</param>
        /// <param name="decrypt">descriptograr ou nao</param>
        private static string GetUserValueXML(string appName, Keys key, bool decrypt)
        {
            var tmp = InternalXmlHelper.Instance.Xml.SelectSingleNode(string.Format("//SDKoreSettings/users/add[@appName='{0}']", appName)).Attributes[key.ToString()].Value;
            if (decrypt)
                return Cryptography.Decrypt(tmp);
            else
                return tmp;
        }

        /// <summary>
        /// Classe utilitária para carregar XmlDocument.
        /// </summary>
        private class InternalXmlHelper
        {
            /// <summary>
            /// Mantém uma única intância desta classe.
            /// </summary>
            public readonly static InternalXmlHelper Instance = new InternalXmlHelper();

            /// <summary>
            /// Objeto que contém o xml de configuração.
            /// </summary>
            XmlDocument xml = new XmlDocument();

            /// <summary>
            /// Representa uma instância da classe que carrega o xml de configuração.
            /// </summary>
            public InternalXmlHelper()
            {

                try
                {
                    //Caminho de arquivo de configuração para debug local.
                    string file = @"C:\sdkore.config.xml";
                    if (!File.Exists(file))
                        throw new FileNotFoundException("Arquivo não encontrado no C:");

                    xml.Load(file);

                }
                catch (Exception ex)
                {
                    Helper.Error.Handler(ex);
                }
            }

            /// <summary>
            /// Documento xml carregado pela classe.
            /// </summary>
            public XmlDocument Xml
            {
                get
                {
                    return xml;
                }
            }
        }
    }
}
