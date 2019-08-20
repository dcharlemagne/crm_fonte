using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace SDKore.Crm.Base
{
    public abstract class SDKorePluginBase : IPlugin
    {
        #region Enum

        public enum MessageName
        {
            Create,
            Update,
            Delete,
            Send,
            SetState,
            SetStateDynamicEntity
        }

        public enum Stage
        {
            PreValidation = 10,
            PreOperation = 20,
            PostOperation = 40
        }

        public enum Mode
        {
            Asynchonous = 1,
            Synchonous = 0
        }

        #endregion

        #region Servicos do Contexto do plugin

        /// <summary>
        /// Contexto de execução do Plugin
        /// </summary>
        protected IPluginExecutionContext Context { get; private set; }

        /// <summary>
        /// ServiceFactory
        /// </summary>
        protected IOrganizationServiceFactory ServiceFactory { get; private set; }

        /// <summary>
        /// OrganizationProxyService no Contexto do Usuário logado
        /// </summary>
        private IOrganizationService _userService;
        protected IOrganizationService UserService
        {
            get
            {
                if (_userService == null) _userService = ServiceFactory.CreateOrganizationService(Guid.Empty);
                return _userService;
            }
        }

        /// <summary>
        /// OrganizationProxyService no Contexto do CrmAdmin
        /// </summary>
        private IOrganizationService _adminService;
        protected IOrganizationService AdminService
        {
            get
            {
                if (_adminService == null) _adminService = ServiceFactory.CreateOrganizationService(Guid.Empty);
                return _adminService;
            }
        }

        /// <summary>
        /// Servico de trace do Plugin
        /// </summary>
        protected ITracingService Tracing { get; private set; }

        #endregion

        #region Trace e ErrorHandle

        /// <summary>
        /// Cria uma linha no log de execução do plugin
        /// </summary>
        /// <param name="message">Mensagem a ser exibida</param>
        /// <param name="args">Argumentos passados como {0}, {1}... no parâmetro message</param>
        protected void Trace(string message, params string[] args)
        {
            Tracing.Trace(message, args);
        }

        /// <summary>
        /// Método que cria o log do erro na execução do plugin, extraindo as InnerExceptions
        /// </summary>
        /// <param name="ex">Exceção</param>
        protected void ErrorHandle(Exception ex)
        {
            var primeiraEx = ex;

            while (ex != null)
            {
                Trace("Erro      : {0}", ex.Message);
                Trace("StackTrace: {0}", ex.StackTrace);
                Trace("==========================================================================");

                ex = ex.InnerException;
            }

            throw new InvalidPluginExecutionException(primeiraEx.Message);
        }

        #endregion

        #region GetContextEntity

        /// <summary>
        /// Obtem a entidade do Contexto
        /// </summary>
        /// <returns>Entidade do InputParameters: Target</returns>
        protected Entity GetContextEntity()
        {
            return GetContextEntity(string.Empty);
        }

        /// <summary>
        /// Obtém a referência Moniker do Contexto
        /// </summary>
        /// <returns>Entidade de Referência: Moniker</returns>
        protected EntityReference GetContextMoniker()
        {
            if (Context.InputParameters.Contains("EntityMoniker") && Context.InputParameters["EntityMoniker"] is EntityReference)
                return (EntityReference)Context.InputParameters["EntityMoniker"];

            return null;
        }

        /// <summary>
        /// Obtem a imagem do Contexto
        /// </summary>
        /// <param name="nomeDaImagem">Nome da Imagem registrada (PreImage ou PostImage)</param>
        /// <returns>Entidade (PreImage ou PostImage)</returns>
        protected Entity GetContextEntity(string nomeDaImagem)
        {
            if (!String.IsNullOrEmpty(nomeDaImagem))
            {
                if (Context.PreEntityImages.Contains(nomeDaImagem)) return Context.PreEntityImages[nomeDaImagem];
                if (Context.PostEntityImages.Contains(nomeDaImagem)) return Context.PostEntityImages[nomeDaImagem];
            }
            else
            {
                if (Context.InputParameters.Contains("EntityMoniker")) return (Entity)Context.InputParameters["EntityMoniker"];
                if (Context.InputParameters.Contains("Target")) return (Entity)Context.InputParameters["Target"];
            }

            return null;
        }

        #endregion

        #region GetMessage e Stage

        /// <summary>
        /// Obtem a ação no CRM
        /// </summary>
        protected MessageName GetMessageName()
        {
            MessageName msg = (MessageName)System.Enum.Parse(typeof(MessageName), Context.MessageName);
            return msg;
        }

        /// <summary>
        /// Obtem o Estágio
        /// </summary>
        protected Stage GetStage()
        {
            Stage stage = (Stage)Context.Stage;
            return stage;
        }

        /// <summary>
        /// Valida ação e estágio
        /// </summary>
        protected bool Validate(MessageName message, Stage stage)
        {
            return GetMessageName() == message && GetStage() == stage;
        }

        #endregion

        #region Execute

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                ServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                Tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                //Garante que o serviço será criado na execução deste plugin
                _adminService = null;
                _userService = null;

                Execute();
            }
            catch (Exception ex)
            {
                ErrorHandle(ex);
            }
        }

        /// <summary>
        /// Método de controle de execução, já com tratamento de erro
        /// </summary>
        protected abstract void Execute();

        #endregion
    }
}