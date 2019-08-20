using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SDKore.Crm.Util;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace SDKore.Crm.Util
{
    public class Utility
    {
        private static CrmMetadataEngine<int> _metadataEngine;
        public static CrmMetadataEngine<int> MetadataEngine
        {
            get
            {
                if (_metadataEngine == null)
                {
                    _metadataEngine = new CrmMetadataEngine<int>();
                }
                return _metadataEngine;
            }
        }


        public Utility()
        { }

        /// <summary>
        /// Obtém o nome lógico da entidade
        /// </summary>
        /// <param name="t">Classe de domínio</param>
        /// <returns></returns>
        public static string GetEntityName<T>()
        {
            var info = typeof(T).GetCustomAttributes(typeof(LogicalEntity), true);
            if (info == null || info.Length == 0)
                throw new ArgumentException("Classe não decorada com o atributo LogicalEntity.");

            return ((LogicalEntity)info[0]).Name;
        }

        /// <summary>
        /// Obtém o nome lógico da entidade. Este nome deve estar informado no Atributo LogicalEntity que decora a classe
        /// </summary>
        /// <param name="entity">Classe de domínio que se deseja recuprar o nome lógico.</param>
        /// <returns></returns>
        public static string GetEntityName(object entity)
        {
            if (null == entity)
                throw new ArgumentNullException("Entity", "A entidade informada é nula ou inválida.");

            var type = entity.GetType();

            var info = type.GetCustomAttributes(typeof(LogicalEntity), true);
            if (info == null || info.Length == 0)
                throw new ArgumentException("Classe não decorada com o atributo LogicalEntity.");

            return ((LogicalEntity)info[0]).Name;
        }

        /// <summary>
        /// Obtém o nome lógico do atributo. 
        /// <para>As propriedades devem estar decoradas com o Attribute [LogicalAttribute].</para>
        /// </summary>
        /// <param name="t">Classe de domínio</param>
        /// <param name="name">Nome do Atributo.</param>
        /// <returns></returns>
        public static LogicalAttribute GetLogicalAttribute<T>(string name)
        {
            PropertyInfo propr = typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            return GetLogicalAttribute(propr);
            //var info = propr.GetCustomAttributes(typeof(LogicalAttribute), false);
            //if (info == null || info.Length == 0)
            //    return null;

            //return ((LogicalAttribute)info[0]);
        }

        private static LogicalAttribute GetLogicalAttribute(PropertyInfo property)
        {
            var info = property.GetCustomAttributes(typeof(LogicalAttribute), false);
            if (info == null || info.Length == 0)
                return null;

            return ((LogicalAttribute)info[0]);
        }

        /// <summary>
        /// Obtém o valor do atributo
        /// </summary>
        /// <param name="t">Classe de domínio</param>
        /// <param name="name">Nome do atributo.</param>
        /// <returns></returns>
        public static object GetValue<T>(T t, string name)
        {
            PropertyInfo propr = t.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            return propr.GetValue(t, null);
        }

        public static SDKore.Crm.Enum.TypeXrmSdk GetTypeAttribute(string organization, bool offline, string entityName, PropertyInfo property, object value)
        {
            Enum.TypeXrmSdk typeXrmSdk = Enum.TypeXrmSdk.None;
            string fullName = GetFullName(property.PropertyType);

            //Se for um Enum, obrigatoriamente deve ser um OptionSet no CRM
            if (IsEnum(property.PropertyType))
            {
                return typeXrmSdk = Enum.TypeXrmSdk.OptionSetValue;
            }
            else if (typeof(Int16).FullName == fullName || typeof(Int32).FullName == fullName)
            {
                //Se inteiro, pode ser um OptionSetValue.
                if (IsOptionSetValue(organization, offline, entityName, Utility.GetLogicalAttribute(property).Name))
                    typeXrmSdk = Enum.TypeXrmSdk.OptionSetValue;
            }
            else if (typeof(Decimal).FullName == fullName)
            {
                if (IsMoney(organization, offline, entityName, Utility.GetLogicalAttribute(property).Name))
                    typeXrmSdk = Enum.TypeXrmSdk.Money;

            }
            else if (fullName == typeof(SDKore.DomainModel.Lookup).FullName)
            {
                typeXrmSdk = SDKore.Crm.Enum.TypeXrmSdk.EntityReference;
            }
            else if (fullName == typeof(SDKore.DomainModel.Lookup[]).FullName)
            {
                typeXrmSdk = SDKore.Crm.Enum.TypeXrmSdk.PartyList;
            }
            else if (fullName == typeof(DateTime).FullName)
            {
                typeXrmSdk = SDKore.Crm.Enum.TypeXrmSdk.Datetime;
            }

            return typeXrmSdk;
        }

        private static string GetFullName(Type type)
        {
            string fullName = string.Empty;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                fullName = type.GetGenericArguments()[0].FullName;
            }
            else
                fullName = type.FullName;

            return fullName;
        }

        public static bool IsEnum(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0].IsEnum;
            }
            else
            {
                return type.IsEnum;
            }
        }

        public static Type GetType(PropertyInfo info)
        {
            if (info.PropertyType.IsGenericType && info.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return info.PropertyType.GetGenericArguments()[0];
            }
            else
            {
                return info.PropertyType;
            }
        }

        private static bool IsOptionSetValue(string organization, bool offline, string entityName, string attributeName)
        {
            try
            {
                var att = GetAttributeMetadata(organization, offline, entityName, attributeName);
                return (att.AttributeType.Value == AttributeTypeCode.Picklist || att.AttributeType.Value == AttributeTypeCode.State || att.AttributeType.Value == AttributeTypeCode.Status);
            }
            catch
            { return false; }

            //MetadataEngine.SetOrganization(organization);
            //MetadataEngine.SetIsOffline(offline);

            //string key = string.Format("{0}_{1}_{2}_{3}", organization, offline.ToString(), entityName, attributeName);
            //var isOption = (bool?)MetadataEngine.GerenciadorCache.Get(key);
            //if (!isOption.HasValue)
            //{
            //    var request = new RetrieveAttributeRequest()
            //    {
            //        EntityLogicalName = entityName,
            //        LogicalName = attributeName
            //    };
            //    isOption = false;
            //    RetrieveAttributeResponse response = (RetrieveAttributeResponse)MetadataEngine.Execute(request);
            //    if (response.Results.Count > 0)
            //    {
            //        try
            //        {
            //            var picklist = new PicklistAttributeMetadata();
            //            picklist.OptionSet = ((EnumAttributeMetadata)response.Results.Values.ElementAt(0)).OptionSet;
            //            isOption = true;
            //        }
            //        catch { }
            //    }
            //    MetadataEngine.GerenciadorCache.Add(key, isOption, 24);
            //    return isOption.Value;

            //}
            //else
            //    return isOption.Value;
        }

        private static bool IsMoney(string organization, bool offline, string entityName, string attributeName)
        {
            try
            {
                var att = GetAttributeMetadata(organization, offline, entityName, attributeName);
                return (att.AttributeType.Value == AttributeTypeCode.Money);
            }
            catch
            { return false; }
        }

        private static AttributeMetadata GetAttributeMetadata(string organization, bool offline, string entityName, string attributeName)
        {
            MetadataEngine.SetOrganization(organization);
            MetadataEngine.SetIsOffline(offline);

            string key = string.Format("{0}_{1}_{2}_{3}", organization, offline.ToString(), entityName, attributeName);
            AttributeMetadata att = (AttributeMetadata)MetadataEngine.GerenciadorCache.Get(key);
            if (att == null)
            {
                var request = new RetrieveAttributeRequest()
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName
                };

                RetrieveAttributeResponse response = (RetrieveAttributeResponse)MetadataEngine.Execute(request);
                if (response.Results.Count > 0)
                {
                    att = (Microsoft.Xrm.Sdk.Metadata.AttributeMetadata)response.Results.Values.ElementAt(0);
                    MetadataEngine.GerenciadorCache.Add(key, att, 24);
                }
            }

            return att;
        }

        public static object GetCrmAttributeValueForSearches(object value)
        {
            switch (Utility.ToEnum(value.GetType()))
            {
                case Enum.TypeXrmSdk.EntityReference:
                    return ((EntityReference)value).Id;
                case Enum.TypeXrmSdk.OptionSetValue:
                    return ((OptionSetValue)value).Value;
                case Enum.TypeXrmSdk.Money:
                    return ((Money)value).Value;
                default:
                    return value;
            }
        }

        private static Enum.TypeXrmSdk ToEnum(Type tipo)
        {
            if (tipo == typeof(EntityReference))
                return Enum.TypeXrmSdk.EntityReference;
            if (tipo == typeof(OptionSetValue))
                return Enum.TypeXrmSdk.OptionSetValue;
            if (tipo == typeof(Money))
                return Enum.TypeXrmSdk.Money;
            else
                return Enum.TypeXrmSdk.None;
        }

        [Obsolete]
        public static SDKore.Crm.Enum.TypeXrmSdk GetTypeAttibute(string organization, string entity, string attribute, bool offline)
        {
            SDKore.Crm.Enum.TypeXrmSdk typeXrmSdk = SDKore.Crm.Enum.TypeXrmSdk.None;
            MetadataEngine.SetOrganization(organization);
            MetadataEngine.SetIsOffline(offline);

            var metaEntity = MetadataEngine.Metadata.Find(delegate(EntityMetadata e) { return e.LogicalName.ToLower() == entity.ToLower(); });
            if (metaEntity != null)
            {
                var metaAttribute = metaEntity.Attributes.FirstOrDefault(delegate(AttributeMetadata a) { return a.LogicalName.ToLower() == attribute.ToLower(); });
                if (metaAttribute != null)
                {
                    switch (metaAttribute.AttributeType.Value)
                    {
                        case AttributeTypeCode.PartyList:
                            typeXrmSdk = Enum.TypeXrmSdk.PartyList;
                            break;
                        case AttributeTypeCode.Money:
                            typeXrmSdk = SDKore.Crm.Enum.TypeXrmSdk.Money;
                            break;
                        case AttributeTypeCode.Lookup:
                        case AttributeTypeCode.Owner:
                        case AttributeTypeCode.Customer:
                            typeXrmSdk = SDKore.Crm.Enum.TypeXrmSdk.EntityReference;
                            break;
                        case AttributeTypeCode.Picklist:
                        case AttributeTypeCode.Status:
                        case AttributeTypeCode.State:
                            typeXrmSdk = SDKore.Crm.Enum.TypeXrmSdk.OptionSetValue;
                            break;

                    }
                }
            }

            return typeXrmSdk;
        }

    }
}
