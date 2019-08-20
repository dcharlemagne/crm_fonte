// =====================================================================
//
//  This file is part of the Microsoft CRM Code Samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
//
// =====================================================================
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Metadata;
using Microsoft.Crm.SdkTypeProxy.Metadata;
using Microsoft.Crm.SdkTypeProxy;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Microsoft.Crm.Workflow;
using Intelbras.Crm.Domain.Services;
using Intelbras.Crm.Application.Plugin;

namespace Intelbras.Crm.Application.Plugin
{
    /// <summary>
    /// Class created to serialize the IPluginExecutionContext
    /// Since we cannot serialize the Interfaces
    /// </summary>
    public class PluginExecutionContext
    {
        public PluginExecutionContext()
        {
        }
        public PluginExecutionContext(IPluginExecutionContext context)
        {
            BusinessUnitId = context.BusinessUnitId;
            CallerOrigin = context.CallerOrigin;
            CorrelationId = context.CorrelationId;
            CorrelationUpdatedTime = context.CorrelationUpdatedTime;
            Depth = context.Depth;
            InitiatingUserId = context.InitiatingUserId;
            InputParameters = context.InputParameters;
            InvocationSource = context.InvocationSource;
            IsExecutingInOfflineMode = context.IsExecutingInOfflineMode;
            MessageName = context.MessageName;
            Mode = context.Mode;
            OrganizationId = context.OrganizationId;
            OrganizationName = context.OrganizationName;
            OutputParameters = context.OutputParameters;
            if (context.ParentContext != null)
            {
                ParentContext = new PluginExecutionContext(context.ParentContext);
            }
            PostEntityImages = context.PostEntityImages;
            PreEntityImages = context.PreEntityImages;
            PrimaryEntityName = context.PrimaryEntityName;
            SecondaryEntityName = context.SecondaryEntityName;
            SharedVariables = context.SharedVariables;
            Stage = context.Stage;
            UserId = context.UserId;
        }
        #region IPluginExecutionContext Members

        public Guid BusinessUnitId;


        public CallerOrigin CallerOrigin;

        public Guid CorrelationId;

        public CrmDateTime CorrelationUpdatedTime;

        public int Depth;


        public Guid InitiatingUserId;

        public PropertyBag InputParameters;

        public int InvocationSource;


        public bool IsExecutingInOfflineMode;

        public string MessageName;


        public int Mode;


        public Guid OrganizationId;

        public string OrganizationName;

        public PropertyBag OutputParameters;


        public PluginExecutionContext ParentContext;


        public PropertyBag PostEntityImages;


        public PropertyBag PreEntityImages;


        public string PrimaryEntityName;


        public string SecondaryEntityName;


        public PropertyBag SharedVariables;


        public int Stage;


        public Guid UserId;


        #endregion
    }

    public class PluginHelper
    {
        /// <summary>
        /// Retrieves EntityId from the Context
        /// Create,Update,Delete,SetState,Assign,DeliverIncoming
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Guid GetEntityId(IPluginExecutionContext context)
        {
            switch (context.MessageName)
            {
                case MessageName.Create:
                case MessageName.DeliverIncoming:
                    if (context.Stage == MessageProcessingStage.BeforeMainOperationOutsideTransaction)
                    {
                        //throw new InvalidPluginExecutionException("EntityId is not available in PreCreate");
                        return Guid.Empty;
                    }
                    else
                    {
                        //CreateResponse r;
                        //r.id;
                        if (context.OutputParameters.Contains(ParameterName.Id))
                        {
                            return (Guid)context.OutputParameters[ParameterName.Id];
                        }

                        //DeliverIncomingEmailResponse r;
                        //r.EmailId;
                        if (context.OutputParameters.Contains(ParameterName.EmailId))
                        {
                            return (Guid)context.OutputParameters[ParameterName.EmailId];
                        }
                    }
                    break;
                case MessageName.Update:
                    //context.InputParameters.Contains(ParameterName.Target)
                    IMetadataService metadataService = context.CreateMetadataService(false);
                    RetrieveEntityRequest rar = new RetrieveEntityRequest();
                    rar.LogicalName = context.PrimaryEntityName;
                    rar.EntityItems = EntityItems.IncludeAttributes;
                    RetrieveEntityResponse resp = (RetrieveEntityResponse)metadataService.Execute(rar);
                    string keyName = resp.EntityMetadata.PrimaryKey;

                    //UpdateRequest u;
                    //TargetUpdateAccount a;
                    //a.Account; // This s Dynamic entity
                    //u.Target = a;

                    // Update
                    if (context.InputParameters[ParameterName.Target] is DynamicEntity)
                    {
                        Key key = (Key)((DynamicEntity)context.InputParameters[ParameterName.Target]).Properties[keyName];
                        return key.Value;
                    }
                    break;
                case MessageName.Delete:
                case MessageName.Assign:
                case MessageName.GrantAccess:
                case MessageName.Handle:
                    if (context.InputParameters[ParameterName.Target] is Moniker)
                    {
                        Moniker monikerId = (Moniker)context.InputParameters[ParameterName.Target];
                        return monikerId.Id;
                    }
                    break;
                case MessageName.SetState:
                case MessageName.SetStateDynamicEntity:
                    //SetStateAccountRequest r;
                    //r.EntityId; // Guid === Moniker 
                    //r.AccountState; // State
                    //r.AccountStatus; // Status
                    return ((Moniker)context.InputParameters.Properties[ParameterName.EntityMoniker]).Id; ;
                default:
                    if (context.InputParameters.Contains(ParameterName.Target) &&
                        (context.InputParameters[ParameterName.Target] is Moniker))
                    {
                        Moniker monikerId = (Moniker)context.InputParameters[ParameterName.Target];
                        return monikerId.Id;
                    }
                    //Try by best route else fail
                    //throw new InvalidPluginExecutionException("GetEntityId could not extract the Guid from Context");
                    return Guid.Empty;
            }
            //throw new InvalidPluginExecutionException("GetEntityId could not extract the Guid from Context");
            return Guid.Empty;
        }

        /// <summary>
        /// Context is serialized to Xml
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetContextXml(IPluginExecutionContext context, out int length)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PluginExecutionContext));
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                PluginExecutionContext c1 = new PluginExecutionContext(context);
                serializer.Serialize(writer, c1);
                StringBuilder sb = writer.GetStringBuilder();
                length = sb.Length;
                if (sb.Length < 10000)
                {
                    return writer.ToString();
                }
                else
                {
                    sb.Insert(0, "Truncated.");
                    return sb.ToString(0, 10000);
                }
            }
        }

        /// <summary>
        /// Custom Entity prepped with the IPluginExecutionContext Information
        /// </summary>
        /// <param name="config"></param>
        /// <param name="secureconfig"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DynamicEntity GetPreparedPluginContextEntity(string config, string secureconfig, IPluginExecutionContext context)
        {
            if (null == config)
            {
                config = "NULL";
            }
            if (null == secureconfig)
            {
                secureconfig = "NULL";
            }
            DynamicEntity pluginContextDynamicEntity = new DynamicEntity("new_plugincontext");
            pluginContextDynamicEntity.Properties.Add(new StringProperty("new_name", String.Format("{0},{1},{2}", context.MessageName, context.PrimaryEntityName, context.Stage)));
            pluginContextDynamicEntity.Properties.Add(new StringProperty("new_config", config));
            pluginContextDynamicEntity.Properties.Add(new StringProperty("new_secureconfig", secureconfig));
            pluginContextDynamicEntity.Properties.Add(new StringProperty("new_messagename", context.MessageName));
            pluginContextDynamicEntity.Properties.Add(new StringProperty("new_stage", context.Stage.ToString()));
            int length;
            pluginContextDynamicEntity.Properties.Add(new StringProperty("new_serializedxml", GetContextXml(context, out length)));
            pluginContextDynamicEntity.Properties.Add(new CrmNumberProperty("new_contextxmllength", new CrmNumber(length)));

            return pluginContextDynamicEntity;
        }

        /*
    /// <summary>
    /// In Child pipeline, you can still use CrmService to connect to CRM but we DONOT Recommend as it is prone to Deadlocks
    /// But if you really want to do it, this is a suggested approach.
    /// </summary>
    /// <param name="endPointUrl"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    
    public static CrmService GetCrmProxyUsingEndpointUrlInChildPipeline(string endPointUrl, IPluginExecutionContext context)
    {
        CrmService childCrmService = new CrmService();
        childCrmService.Url = endPointUrl;
        childCrmService.Credentials = System.Net.CredentialCache.DefaultCredentials;
        childCrmService.CrmAuthenticationTokenValue = new CrmAuthenticationToken();
        childCrmService.CrmAuthenticationTokenValue.AuthenticationType = AuthenticationType.AD;
        childCrmService.CrmAuthenticationTokenValue.OrganizationName = context.OrganizationName;
        childCrmService.CorrelationTokenValue = new CorrelationToken(context.CorrelationId, context.Depth, context.CorrelationUpdatedTime);
        return childCrmService;
    }
     * */

        public static DynamicEntity GetDynamicEntity(IPluginExecutionContext contexto)
        {

            DynamicEntity entity = null;

            // Verifica se os parâmetros de entrada contém um target de criação e que é target do tipo DynamicEntity
            if (contexto.InputParameters.Properties.Contains("Target") && contexto.InputParameters.Properties["Target"] is DynamicEntity)
                // Recupera a entidade de negócio target dos parâmetros de entrada
                entity = (DynamicEntity)contexto.InputParameters.Properties["Target"];
            else
            {
                if (contexto.InputParameters.Properties.Contains("IncidentResolution") && contexto.InputParameters.Properties["IncidentResolution"] is DynamicEntity)
                    entity = (DynamicEntity)contexto.InputParameters.Properties["IncidentResolution"];
                else
                {

                    if (contexto.InputParameters.Properties.Contains("State") && contexto.InputParameters.Properties.Contains("EntityMoniker"))
                    {

                        if (contexto.InputParameters.Properties["EntityMoniker"] is Moniker)
                        {

                            var moniker = (Moniker)contexto.InputParameters.Properties["EntityMoniker"];
                            entity = new DynamicEntity("incident");
                            entity.Properties.Add(new KeyProperty("incidentid", new Key(moniker.Id)));

                            return entity;

                        }

                    }
                    else
                    {

                        // Verifica se os parâmetros de entrada contém um target de criação e que é target do tipo DynamicEntity
                        if (contexto.InputParameters.Properties.Contains("Target") && contexto.InputParameters.Properties["Target"] is Moniker)
                        {

                            var filaOrigemId = (Guid)contexto.InputParameters.Properties["SourceQueueId"];

                            var moniker = (Moniker)contexto.InputParameters.Properties["Target"];

                            entity = new DynamicEntity("incident");
                            entity.Properties.Add(new KeyProperty("incidentid", new Key(moniker.Id)));
                            entity.Properties.Add(new LookupProperty("sourcequeueid", new Lookup("queue", filaOrigemId)));

                            return entity;
                        }

                    }

                }

            }

            return entity;
        }

        public static DynamicEntity GetDynamicEntity(IPluginExecutionContext contexto, string nomeEntidade)
        {
            var entidade = (DynamicEntity)contexto.PreEntityImages[nomeEntidade];

            if (null == entidade)
                entidade = (DynamicEntity)contexto.PostEntityImages[nomeEntidade];

            return entidade;
        }

        public static DynamicEntity GetDynamicEntity(IWorkflowContext contexto)
        {
            DynamicEntity entity = null;

            // Verifica se os parâmetros de entrada contém um target de criação e que é target do tipo DynamicEntity
            if (contexto.InputParameters.Properties.Contains("Target") && contexto.InputParameters.Properties["Target"] is DynamicEntity)
                // Recupera a entidade de negócio target dos parâmetros de entrada
                entity = (DynamicEntity)contexto.InputParameters.Properties["Target"];

            return entity;
        }

        public static DynamicEntity AdicionarPropriedadeEmEntidadeDinamica(DynamicEntity entidade, string propriedade, object valor)
        {
            if (entidade != null && propriedade != null && valor != null)
            {
                if (entidade.Properties.Contains(propriedade))
                    entidade.Properties.Remove(propriedade);

                switch (valor.GetType().Name)
                {
                    case "CrmNumber":
                        entidade.Properties.Add(new CrmNumberProperty(propriedade, valor as CrmNumber));
                        break;
                    case "String":
                        entidade.Properties.Add(new StringProperty(propriedade, valor.ToString()));
                        break;
                    case "CrmDateTime":
                        entidade.Properties.Add(new CrmDateTimeProperty(propriedade, valor as CrmDateTime));
                        break;
                    case "Lookup":
                        entidade.Properties.Add(new LookupProperty(propriedade, valor as Lookup));
                        break;
                    case "Status":
                        entidade.Properties.Add(new StatusProperty(propriedade, valor as Status));
                        break;
                    case "CrmMoney":
                        entidade.Properties.Add(new CrmMoneyProperty(propriedade, valor as CrmMoney));
                        break;

                    default:
                        throw new ArgumentException("Tipo não tratado. PluginHelper.AdicionarPropriedadeEmEntidadeDinamica");
                }
            }

            return entidade;
        }
        
        public static void TratarExcecao(Exception ex, TipoDeLog tipo)
        {
            LogException.Tratar(ex, tipo);
        }

        public static void Log(params object[] mensagem)
        {
            StringBuilder log = new StringBuilder();

            int x = 0;
            foreach (var item in mensagem)
            {
                x++;

                log.Append(item.ToString());

                if (mensagem.Length > x)
                    log.Append(" :: ");
            }
            LogService.GravaLog(log.ToString());
        }

        public static void LogEmArquivo(IPluginExecutionContext context, string status, string dataInicio, string dataFim)
        {
            //Origem: ApplicationOrigin / AsyncServiceOrigin / WebServiceApiOrigin
            //context.Stage 10 Pre - 50 Post
            //context.Mode  0 Sync - 1 Async
            //Criar logs nos plug-ins de ocorrência em arquivo texto para armazenar usuário, evento, entidade, início, termino e origem
            //ID_EXECUCAO;10PRE_50POST;ENTIDADE;PROFUND;EVENTO;0SYNC_1ASYNC;ID_ENTIDADE;ORIGEM;USER_CALL;USER_EXEC;INICIO;FIM;STATUS;
            LogService.GravaLogArquivo(context.CorrelationId.ToString() + ";" + context.Stage + ";" + context.PrimaryEntityName + ";" + context.Depth.ToString() + ";" + context.MessageName + ";" + context.Mode.ToString() + ";" + PluginHelper.GetEntityId(context).ToString() + ";" + context.CallerOrigin.GetType().Name + ";" + context.InitiatingUserId.ToString() + ";" + context.UserId.ToString() + ";" + dataInicio + ";" + dataFim + ";" + status);
        }
    }

    public class LogException
    {
        public static void Tratar(Exception ex, TipoDeLog tipo)
        {
            if (ex.GetType() == typeof(ArgumentException))
                throw new InvalidPluginExecutionException(ex.Message);

            if (ex.GetType() == typeof(ArgumentNullException))
                throw new InvalidPluginExecutionException(ex.Message);

            if (ex.GetType() == typeof(InvalidPluginExecutionException))
                throw ex;

            LogService.GravaLog(ex, tipo);
            throw new InvalidPluginExecutionException(ex.Message);
        }
    }
}
