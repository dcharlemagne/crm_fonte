using SDKore.DomainModel;
using SDKore.Helper;
using SDKore.Helper.Cache;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class Base
    {
        #region Construtuor

        public Base(string organizacao, bool isOffline)
        {
            this.Organizacao = organizacao;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Organização do MS CRM
        /// </summary>
        public string Organizacao { get; set; }

        /// <summary>
        /// Informa o tipo de conexão com o CRM
        /// </summary>
        public bool IsOffline { get; set; }

        /// <summary>
        /// Nome físico da Entidade
        /// </summary>
        public string NomeEntidade { get; set; }

        /// <summary>
        /// Objeto que gerência cache.
        /// </summary>
        protected ICacheManager GerenciadorCache
        {
            get
            {
                return CacheFactory.Instance.Container.Resolve<ICacheManager>();
            }
        }


        #endregion

        #region Métodos Públicos

        public T Enviar<T>(string mensagem) where T : Intelbras.Message.Helper.MessageBase
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Efetua o carregamento da Mensagem. <para>Transforma string XML em objeto T</para>
        /// </summary>
        /// <typeparam name="T">Tipo de objeto</typeparam>
        /// <param name="mensagem">string XML</param>
        /// <returns>Objeto T carregado.</returns>
        public T CarregarMensagem<T>(string mensagem) where T : Intelbras.Message.Helper.MessageBase
        {
            return Intelbras.Message.Helper.MessageBase.LoadMessage<T>(System.Xml.Linq.XDocument.Parse(mensagem), true);
        }

        /// <summary>
        /// Cria uma mensagem de retotno.
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="numeroMensagem">Número da mensagem</param>
        /// <returns>Mensagem de retorno estruturada.</returns>
        public string CriarMensagemRetorno<T>(string numeroMensagem, Dictionary<string, object> resultados)
        {
            var retorno = (Intelbras.Message.Helper.MessageBase)Activator.CreateInstance(typeof(T), "DBFC273E-4811-40C4-8A4E-1629731ADD9A", numeroMensagem);
            foreach (var item in resultados)
            {
                System.Reflection.PropertyInfo propTmp = retorno.GetType().GetProperty(item.Key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                propTmp.SetValue(retorno, item.Value, null);
            }
            
            return retorno.GenerateMessage(true);
        }

        /// <summary>
        /// Instância Lookup
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="atributoConsulta">Atributo que serve como parâmetro de consulta.</param>
        /// <param name="valorConsulta">Valor para consulta.</param>
        /// <returns></returns>
        public SDKore.DomainModel.Lookup DefinirLookup<T>(string atributoConsulta, string valorConsulta)
        {
            dynamic tmp = this.ObterPor<T>(atributoConsulta, valorConsulta);
            return (tmp == null) ? null : new SDKore.DomainModel.Lookup { Id = tmp.ID, Type = SDKore.Crm.Util.Utility.GetEntityName<T>() };
        }

        /// <summary>
        /// Persiste a informação recebida no MS CRM.
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="objModel">Objeto populado</param>
        /// <param name="atributoConsulta">Atributo que serve como parâmetro de consulta.</param>
        /// <param name="valorConsulta">Valor para consulta.</param>
        public Guid Persistir<T>(T objModel, string atributoConsulta, string valorConsulta)
        {
            string[] tmpAtributoConsulta = new string[] { atributoConsulta };
            string[] tmpValorConsulta = new string[] { valorConsulta };

            return Persistir<T>(objModel, tmpAtributoConsulta, tmpValorConsulta);
        }

        

        /// <summary>
        /// Persiste a informação recebida no MS CRM.
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="objModel">Objeto populado</param>
        /// <param name="atributoConsulta">Atributos que serviram como parâmetro de consulta.</param>
        /// <param name="valorConsulta">Valores para a consulta.</param>
        public Guid Persistir<T>(T objModel, string[] atributoConsulta, string[] valorConsulta)
        {
            //Verificar se existe
            dynamic crmTMP = this.ObterPor<T>(atributoConsulta, valorConsulta);
            if (crmTMP == null)
                return this.Create<T>(objModel);
            else
            {
                PropertyInfo propTmp = objModel.GetType().GetProperty("ID", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                propTmp.SetValue(objModel, crmTMP.ID, null);
                this.Update<T>(objModel);
                return crmTMP.ID;
            }
        }

        #endregion

        #region Métodos Privados
        /// <summary>
        /// Obtém o registro
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="atributoConsulta">Atributo que serve como parâmetro de consulta.</param>
        /// <param name="valorConsulta">Valor para consulta.</param>
        /// <returns>Objeto Model populado.</returns>
        private T ObterPor<T>(string atributoConsulta, string valorConsulta)
        {
            return ObterPor<T>(new string[] { atributoConsulta }, new string[] { valorConsulta });
        }

        /// <summary>
        /// Obtém o registro
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="atributoConsulta">Atributos que servirão como parâmetro de consulta.</param>
        /// <param name="valorConsulta">Valores para consulta.</param>
        /// <returns>Objeto Model populado.</returns>
        private T ObterPor<T>(string[] atributoConsulta, string[] valorConsulta)
        {
            if (atributoConsulta.Length == 1 && string.IsNullOrEmpty(atributoConsulta[0]))
                return default(T);
            
            var repositorio = RepositoryFactory.Instance.Container.Resolve<IRepository.IIntegrador<T>>();
            repositorio.SetOrganization(this.Organizacao);
            repositorio.SetIsOffline(this.IsOffline);

            var obj = GerenciadorCache.Get(DefinirChaveCache<T>(atributoConsulta, valorConsulta));
            if (obj == null)
            {
                obj = repositorio.ObterPor(atributoConsulta, valorConsulta);
                if (obj != null)
                    GerenciadorCache.Add(DefinirChaveCache<T>(atributoConsulta, valorConsulta), obj, 1);
            }

            return obj != null ? (T)obj : default(T);
        }

        /// <summary>
        /// Cria o registro no MS CRM
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="model">Objeto Populado</param>
        /// <returns></returns>
        private Guid Create<T>(T model)
        {
            var repositorio = RepositoryFactory.Instance.Container.Resolve<IRepository.IIntegrador<T>>();
            repositorio.SetOrganization(this.Organizacao);
            repositorio.SetIsOffline(this.IsOffline);

            return repositorio.Create(model);
        }

        /// <summary>
        /// Atualiza o registro no MS CRM
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="model">Objeto Populado</param>
        private void Update<T>(T model)
        {
            var repositorio = RepositoryFactory.Instance.Container.Resolve<IRepository.IIntegrador<T>>();
            repositorio.SetOrganization(this.Organizacao);
            repositorio.SetIsOffline(this.IsOffline);

            repositorio.Update(model);
        }

        /// <summary>
        /// Define a chave para cache.
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="atributoConsulta">Atributo que serve como parâmetro de consulta.</param>
        /// <param name="valorConsulta">Valor para consulta.</param>
        /// <returns></returns>
        private string DefinirChaveCache<T>(string atributoConsulta, string valorConsulta)
        {
            return DefinirChaveCache<T>(new string[] { atributoConsulta }, new string[] { valorConsulta });
        }

        /// <summary>
        /// Define a chave para cache.
        /// </summary>
        /// <typeparam name="T">Classe de Negocio</typeparam>
        /// <param name="atributoConsulta">Atributo que serve como parâmetro de consulta.</param>
        /// <param name="valorConsulta">Valor para consulta.</param>
        /// <returns></returns>
        private string DefinirChaveCache<T>(string[] atributoConsulta, string[] valorConsulta)
        {
            string formato = "Integracao_{0}_{1}_{2}";
            return string.Format(formato, SDKore.Crm.Util.Utility.GetEntityName<T>(), this.ArrayParaString(atributoConsulta), this.ArrayParaString(valorConsulta));
        }

        private string ArrayParaString(string[] array)
        {
            string tmp = string.Empty;
            foreach (var item in array)
                tmp += string.Concat(item, "_");

            return tmp;
        }

        protected object PreencherAtributoVazio(string atributo)
        {

            //System.Reflection.PropertyInfo p = typeof(object testc)
            switch (atributo)
            {
                case "int32":
                    return 0;
                case "int":
                    return 0;
                case "int16":
                    return 0;
                case "decimal":
                    return 0;
                case "guid":
                   return Guid.Empty;
                case "string":
                    return "N/A";
                default:
                    return null;
            }
        }

        //Pensei que seria usado mas nao sera, vou deixar aqui para caso precise, basicamente ele preencher todos atributos de um objeto com um valor pre-definido
        //protected object PreencherTodosAtributosVazios(object objeto)
        //{
        //    foreach (PropertyInfo info in objeto.GetType().GetProperties())
        //    {
        //        if (info.CanRead)
        //        {
        //            if (info != null && info.CanWrite)
        //            {
        //                Type propertyType = info.PropertyType;

        //                if (propertyType == typeof(String))
        //                {
        //                    if (info.GetValue(objeto,null) == null)
        //                        info.SetValue(objeto, "N/A", null);
        //                }
        //                else if (propertyType == typeof(Nullable<int>))
        //                {
        //                    info.SetValue(this, (double)12, null);
        //                }
        //                else if (propertyType == typeof(Nullable<long>))
        //                {
        //                    info.SetValue(this, 10, null);
        //                }
        //                else if (propertyType == typeof(Guid))
        //                    info.SetValue(objeto, new Guid("00000000-0000-00000-0000-000000000000"), null);
        //                else
        //                {
        //                    throw new ApplicationException("Unexpected property type");
        //                }
        //            }
        //        }
        //    }
        //    return objeto;
        //}

        #endregion
    }
}
