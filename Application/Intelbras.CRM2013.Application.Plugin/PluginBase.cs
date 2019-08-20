using Microsoft.Xrm.Sdk;
using System;

namespace Intelbras.CRM2013.Application.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        #region Enum

        public enum MessageName
        {
            Create,
            Update,
            Delete,
            Send,
            SetStateDynamicEntity,
            Assign
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

        public T EnumConverter<T>(string valor)
        {
            return (T)System.Enum.Parse(typeof(T), valor, true);
        }

        #endregion


        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracing = null;

            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                tracing = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                //Garante que o serviço será criado na execução deste plugin
                IOrganizationService adminService = serviceFactory.CreateOrganizationService(null);
                IOrganizationService userService = serviceFactory.CreateOrganizationService(context.UserId);
                bool applicationOrigin = context.ObterApplicationOrigin();

                Execute(context, serviceFactory, adminService, userService);
            }
            catch (Exception ex)
            {
                string message = SDKore.Helper.Error.Handler(ex);

                tracing.Trace(SDKore.Helper.Error.GetMessageError(ex));
                throw new InvalidPluginExecutionException(message, ex);
            }
        }

        /// <summary>
        /// Método de controle de execução, já com tratamento de erro
        /// </summary>
        protected abstract void Execute(IPluginExecutionContext context, IOrganizationServiceFactory serviceFactory, IOrganizationService adminService, IOrganizationService userService);

        protected Entity UpdateImage(Entity imagem, Entity contexto)
        {
            foreach (var atributo in contexto.Attributes)
            {
                imagem[atributo.Key] = atributo.Value;
            }

            return imagem;
        }
    }
}
