using Microsoft.Crm.Sdk;
using Microsoft.Xrm.Sdk;
using System;

namespace Microsoft.Xrm.Sdk
{
    public static class IPluginExecutionContextExtension
    {
        public static bool ObterApplicationOrigin(this IPluginExecutionContext context)
        {
            var propertyInfo = context.GetType().GetProperty("CallerOrigin");

            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(context, null);

                if (value is ApplicationOrigin)
                {
                    return true;
                }
            }
            return false;
        }

        #region GetContextEntity
        /// <summary>
        /// Obtem a entidade do contexto
        /// </summary>
        /// <returns>Entidade do InputParameters: EntityMoniker ou Target</returns>
        public static Entity GetContextEntity(this IPluginExecutionContext context)
        {
            return GetContextEntity(context, string.Empty);
        }

        public static Entity GetContextEntityMerge(this IPluginExecutionContext context, string nomeDaImagem)
        {
            var entityTarget = GetContextEntity(context, string.Empty);
            var entityPre = GetContextEntity(context, nomeDaImagem);

            foreach (var item in entityTarget.Attributes)
            {
                entityPre.Attributes[item.Key] = item.Value;
            }

            return entityPre;
        }

        /// <summary>
        /// Obtem a imagem do Contexto
        /// </summary>
        /// <param name="nomeDaImagem">Nome da Imagem registrada (PreImage ou PostImage)</param>
        /// <param name="PreImageOrPostImage">PreImage = True, PostImage = False</param>
        /// <returns>Entidade (PreImage ou PostImage)</returns>
        public static Entity GetContextEntity(this IPluginExecutionContext context, string nomeDaImagem, bool PreImageOrPostImage)
        {
            if (!String.IsNullOrEmpty(nomeDaImagem))
            {
                if (PreImageOrPostImage)
                {
                    if (context.PreEntityImages.Contains(nomeDaImagem)) return context.PreEntityImages[nomeDaImagem];
                }
                else
                {
                    if (context.PostEntityImages.Contains(nomeDaImagem)) return context.PostEntityImages[nomeDaImagem];
                }
            }

            return null;
            //throw new Exception(string.Format("Imagem {0} não registrada no plugin", nomeDaImagem));
        }

        /// <summary>
        /// Obtem a imagem do Contexto
        /// </summary>
        /// <param name="nomeDaImagem">Nome da Imagem registrada (PreImage ou PostImage)</param>
        /// <returns>Entidade (PreImage ou PostImage)</returns>
        public static Entity GetContextEntity(this IPluginExecutionContext context, string nomeDaImagem)
        {
            if (!String.IsNullOrEmpty(nomeDaImagem))
            {
                if (context.PreEntityImages.Contains(nomeDaImagem)) return context.PreEntityImages[nomeDaImagem];

                if (context.PostEntityImages.Contains(nomeDaImagem)) return context.PostEntityImages[nomeDaImagem];

                throw new ApplicationException(string.Format("Imagem {0} não registrada no plugin", nomeDaImagem));
            }

            if (context.InputParameters.Contains("EntityMoniker"))
            {
                var entityReference = (EntityReference)context.InputParameters["EntityMoniker"];
                Entity e = new Entity(entityReference.LogicalName);
                e.Id = entityReference.Id;
                return e;
            }

            if (context.InputParameters.Contains("Target")) return (Entity)context.InputParameters["Target"];

            throw new ApplicationException(string.Format("Imagem {0} não registrada no plugin", nomeDaImagem));
        }
        #endregion

        #region GetMessage e Stage

        /// <summary>
        /// Obtem a ação no CRM
        /// </summary>
        public static Intelbras.CRM2013.Application.Plugin.PluginBase.MessageName GetMessageName(this IPluginExecutionContext context)
        {
            Intelbras.CRM2013.Application.Plugin.PluginBase.MessageName msg = (Intelbras.CRM2013.Application.Plugin.PluginBase.MessageName)System.Enum.Parse(typeof(Intelbras.CRM2013.Application.Plugin.PluginBase.MessageName), context.MessageName);
            return msg;
        }

        /// <summary>
        /// Obtem o Estágio
        /// </summary>
        public static Intelbras.CRM2013.Application.Plugin.PluginBase.Stage GetStage(this IPluginExecutionContext context)
        {
            Intelbras.CRM2013.Application.Plugin.PluginBase.Stage stage = (Intelbras.CRM2013.Application.Plugin.PluginBase.Stage)context.Stage;
            return stage;
        }

        /// <summary>
        /// Valida ação e estágio
        /// </summary>
        public static bool Validate(this IPluginExecutionContext context, Intelbras.CRM2013.Application.Plugin.PluginBase.MessageName message, Intelbras.CRM2013.Application.Plugin.PluginBase.Stage stage)
        {
            return GetMessageName(context) == message && GetStage(context) == stage;
        }

        #endregion
    }
}