using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Practices.Unity;

namespace SDKore.DomainModel
{
    /// <summary>
    /// Fábrica de repositórios
    /// </summary>
    public class RepositoryFactory
    {
        public readonly static RepositoryFactory Instance = new RepositoryFactory();
        private static string _tag = string.Empty;
        private static bool _initialize = true;

        /// <summary>
        /// Método para setar a tag que contém o namespace das Dlls da Solução.
        /// </summary>
        /// <param name="tag"></param>
        public static void SetTag(string tag)
        {
            if (_tag != tag)
            {
                _tag = tag;
                _initialize = true;
            }
        }

        UnityContainer _container = null;
        public UnityContainer Container
        {
            get
            {
                if (_container == null)
                    _container = new UnityContainer();

                if (_initialize)
                    this.Initialize();
                return _container;
            }
        }

        /// <summary>
        /// Construtor
        /// </summary>
        public RepositoryFactory()
        {

        }

        private void Initialize()
        {
            _initialize = false;
            Type[] interfaces = GetInterfacesDomain(string.Format("{0}Domain", _tag));
            foreach (Type item in interfaces)
            {
                var typeRepository = GetRepositories(string.Format("{0}Repository", _tag)).Where(t => t.GetInterface(item.Name) != null).FirstOrDefault();
                if (typeRepository == null) continue;

                Container.RegisterType(item, typeRepository, new PerThreadLifetimeManager());
            }
        }

        /// <summary>
        /// Obtém todas as interfaces de domínio
        /// </summary>
        /// <param name="domainKey"></param>
        /// <returns></returns>
        private static Type[] GetInterfacesDomain(string domainKey)
        {
            string path = SDKore.Configuration.ConfigurationManager.GetApplicationDomain(domainKey).Replace(" ", "");
            string[] array = path.Split(',');
            List<Type> listInterfaces = new List<Type>();
            Assembly ass = null;
            try
            {
                ass = Assembly.Load(array[0]);
            }
            catch (System.IO.FileNotFoundException)
            {
                ass = System.Reflection.Assembly.GetExecutingAssembly();
            }

            Type[] types2 = ass.GetTypes();

            foreach (var item in types2)
                if (item.IsInterface)
                    listInterfaces.Add(item);


            return listInterfaces.ToArray<Type>();
        }

        /// <summary>
        /// Obtém todas as interfaces de repositório
        /// </summary>
        /// <param name="repositoryKey"></param>
        /// <returns></returns>
        private static Type[] GetRepositories(string repositoryKey)
        {
            string path = SDKore.Configuration.ConfigurationManager.GetRepository(repositoryKey).Replace(" ", "");
            string[] array = path.Split(',');

            Assembly ass = null;
            try
            {
                ass = Assembly.Load(array[0]);
            }
            catch (System.IO.FileNotFoundException)
            {
                ass = System.Reflection.Assembly.GetExecutingAssembly();
            }

            Type[] types2 = ass.GetTypes();
            IEnumerable<Type> types3 = types2.Where(t => t.GetInterface(typeof(IRepositoryBase).Name) != null && t.Namespace == array[1]);
            Type[] types = types3.ToArray<Type>();

            return types;
        }

        /// <summary>
        /// Cria Repositório para a Interface informada.
        /// <para>Será preciso atribuir Organização (SetOrganization) e IsOffline (SetIsOffline) ao repostório de retorno.</para>
        /// </summary>
        /// <typeparam name="I">Interface para qual o repositório será criado.</typeparam>
        /// <returns>Repositório</returns>
        public static I GetRepository<I>() where I : IRepositoryBase
        {
            var _repository = RepositoryFactory.Instance.Container.Resolve<I>();
            return (I)_repository;
        }

        /// <summary>
        /// Cria Repositório para a Interface informada.
        /// </summary>
        /// <typeparam name="I">Interface para qual o repositório será criado.</typeparam>
        /// <param name="organizacao">Nome da Organizacao do CRM</param>
        /// <param name="isOffline">Identificador de acesso offline do CRM</param>
        /// <returns>Repositório</returns>
        public static I GetRepository<I>(string organizacao, bool isOffline) where I : IRepositoryBase
        {
            return GetRepository<I>(organizacao, isOffline, null, null);
        }

        /// <summary>
        /// Cria Repositório para a Interface informada.
        /// </summary>
        /// <typeparam name="I">Interface para qual o repositório será criado.</typeparam>
        /// <param name="organizacao">Nome da Organizacao do CRM</param>
        /// <param name="isOffline">Identificador de acesso offline do CRM</param>
        /// <param name="tag">Tag do SDKore (opcional)</param>
        /// <returns>Repositório</returns>
        public static I GetRepository<I>(string organizacao, bool isOffline, string tag) where I : IRepositoryBase
        {
            return GetRepository<I>(organizacao, isOffline, null, tag);
        }

        /// <summary>
        /// Cria Repositório para a Interface informada.
        /// </summary>
        /// <typeparam name="I">Interface para qual o repositório será criado.</typeparam>
        /// <param name="organizacao">Nome da Organizacao do CRM</param>
        /// <param name="isOffline">Identificador de acesso offline do CRM</param>
        /// <param name="provider">IOrganizationService (opcional)</param>
        /// <returns>Repositório</returns>
        public static I GetRepository<I>(string organizacao, bool isOffline, object provider) where I : IRepositoryBase
        {
            return GetRepository<I>(organizacao, isOffline, provider, null);
        }

        /// <summary>
        /// Cria Repositório para a Interface informada.
        /// </summary>
        /// <typeparam name="I">Interface para qual o repositório será criado.</typeparam>
        /// <param name="organizacao">Nome da Organizacao do CRM</param>
        /// <param name="isOffline">Identificador de acesso offline do CRM</param>
        /// <param name="provider">IOrganizationService (opcional)</param>
        /// <param name="tag">Tag do SDKore (opcional)</param>
        /// <returns>Repositório</returns>
        public static I GetRepository<I>(string organizacao, bool isOffline, object provider, string tag) where I : IRepositoryBase
        {
            SetTag(!String.IsNullOrEmpty(tag) ? tag : organizacao);
            dynamic _repository = RepositoryFactory.Instance.Container.Resolve<I>();
            _repository.SetOrganization(organizacao);
            _repository.SetIsOffline(isOffline);
            if (provider != null) _repository.SetProvider(provider);

            return (I)_repository;
        }
    }
}
