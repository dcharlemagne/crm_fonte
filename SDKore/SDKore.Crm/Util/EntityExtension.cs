using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SDKore.Crm.Util;
using SDKore.Crm;
using Microsoft.Xrm.Sdk.Metadata;

namespace Microsoft.Xrm.Sdk
{
    public static class EntityExtension
    {

        /// <summary>
        /// Efetua o parse entre Microsoft.Xrm.Sdk.Entity para T, onde T é a classe de domínio.
        /// </summary>
        /// <typeparam name="T">Classe de domínio</typeparam>
        /// <param name="organization">Organização CRM 2011</param>
        /// <returns>Retorna uma instância de T preenchida.</returns>
        public static T Parse<T>(this Entity entity, string organization, bool offline)
        {
            return Parse<T>(entity, organization, offline, null);
        }


        /// <summary>
        /// Efetua o parse entre Microsoft.Xrm.Sdk.Entity para T, onde T é a classe de domínio.
        /// </summary>
        /// <typeparam name="T">Classe de domínio</typeparam>
        /// <param name="organization">Organização CRM 2011</param>
        /// <returns>Retorna uma instância de T preenchida.</returns>
        public static T Parse<T>(this Entity entity, string organization, bool offline, object provider)
        {
            string campoCRM="";
            string entidadeCRM = "";
            try
            {
                T obj;

                if (provider == null)
                {
                    obj = (T)Activator.CreateInstance(typeof(T), organization, offline);
                }
                else
                {
                    obj = (T)Activator.CreateInstance(typeof(T), organization, offline, provider);
                }
                entidadeCRM = Utility.GetEntityName<T>();
                var listLogicalAttribute = obj.GetType().GetProperties().GetNameProperties();

                var entidadesRetiradas = new string[] { "annotation", "systemuser", "businessunit", "uom", "productpricelevel", "subject", "customeraddress", "salesorderdetail", "invoicedetail" };
                var camposSituacao   = new string[] { "statuscode", "statecode" };
                var camposDomainBase = new string[] { "statuscode", "statecode", "createdon", "createdby", "modifiedon", "modifiedby" };
                var entityName = entidadeCRM.ToLower();

                for (int index = 0; index < listLogicalAttribute.Count; index++)
                {
                    var item = listLogicalAttribute[index];
                    var logicalAttribute = Utility.GetLogicalAttribute<T>(item);
                    if (logicalAttribute == null)
                        continue;
                    campoCRM = logicalAttribute.Name.ToLower();

                    var lowerEntity = entidadeCRM.ToLower();

                    if (entidadesRetiradas.Contains(lowerEntity) && camposSituacao.Contains(campoCRM)) continue;
                    if (lowerEntity.StartsWith("itbc_itbc_") && camposDomainBase.Contains(campoCRM)) continue;

                    // Retirado campos padrões da DomainBase que não existem em determinadas entidades
                    //if (entidadeCRM.ToLower() == "annotation" && camposSituacao.Contains(campoCRM)) continue;
                    //if (entidadeCRM.ToLower() == "systemuser" && camposSituacao.Contains(campoCRM)) continue;
                    //if (entidadeCRM.ToLower() == "businessunit" && camposSituacao.Contains(campoCRM)) continue;
                    //if (entidadeCRM.ToLower() == "uom" && camposSituacao.Contains(campoCRM)) continue;
                    //if (entidadeCRM.ToLower() == "productpricelevel" && camposSituacao.Contains(campoCRM)) continue;
                    //if (entidadeCRM.ToLower().StartsWith("itbc_itbc_") && camposDomainBase.Contains(campoCRM)) continue;

                    if (campoCRM == "id")
                    {
                        if(lowerEntity == "task" || lowerEntity == "email")
                        {
                            campoCRM = "activityid";
                        }
                        else
                        {
                            campoCRM = entidadeCRM + "id";
                        }                        
                    }
                        
                    var value = entity.TryGetValue(campoCRM);
                    if (value == null)
                        continue;

                    PropertyInfo propTmp = obj.GetType().GetProperty(item, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    //propTmp.SetValue(obj, ConfigureProperty(Utility.GetTypeAttibute(organization, Utility.GetEntityName<T>(), logicalAttribute.Name, offline), value), null);

                    //Obter tipo de objeto para o CRM.
                    var typeXrmSdk = Utility.GetTypeAttribute(organization, offline, Utility.GetEntityName<T>(), propTmp, value);
                    var crmValue = ConfigureProperty(typeXrmSdk, value);
                    if (Utility.IsEnum(propTmp.PropertyType))
                    {
                        Type enumType = Utility.GetType(propTmp);
                        propTmp.SetValue(obj, System.Enum.Parse(enumType, crmValue.ToString()), null);
                    }
                    else
                    {
                        propTmp.SetValue(obj, crmValue, null);
                    }
                }
                return obj;
            }catch(Exception ex)
            {
                SDKore.Helper.Error.Create(new Exception (ex.Message + " Campo CRM: "+ campoCRM + " Entidade: " +entidadeCRM), System.Diagnostics.EventLogEntryType.Error);
                throw new ArgumentException(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeXrmSdk"></param>
        /// <param name="valueCRM"></param>
        /// <returns></returns>
        private static object ConfigureProperty(SDKore.Crm.Enum.TypeXrmSdk typeXrmSdk, object valueCRM)
        {
            switch (typeXrmSdk)
            {
                case SDKore.Crm.Enum.TypeXrmSdk.PartyList:
                    var entities = (EntityCollection)valueCRM;
                    SDKore.DomainModel.Lookup[] temp = new SDKore.DomainModel.Lookup[entities.Entities.Count];
                    for (int index = 0; index < temp.Length; index++)
                    {
                        EntityReference partyId = entities.Entities[index].GetAttributeValue<EntityReference>("partyid");

                        temp[index] = new SDKore.DomainModel.Lookup
                        {
                            Id = partyId == null ? Guid.Empty : entities.Entities[index].GetAttributeValue<EntityReference>("partyid").Id,
                            Type = partyId == null ? null : entities.Entities[index].GetAttributeValue<EntityReference>("partyid").LogicalName,
                            Name = partyId == null ? entities.Entities[index].GetAttributeValue<string>("addressused").ToString() : entities.Entities[index].GetAttributeValue<EntityReference>("partyid").Name
                        };
                    }
                    return temp;

                case SDKore.Crm.Enum.TypeXrmSdk.EntityReference:
                    return new SDKore.DomainModel.Lookup()
                    {
                        Id = ((EntityReference)valueCRM).Id,
                        Name = ((EntityReference)valueCRM).Name,
                        Type = ((EntityReference)valueCRM).LogicalName
                    };
                case SDKore.Crm.Enum.TypeXrmSdk.OptionSetValue:
                    return ((OptionSetValue)valueCRM).Value;
                case SDKore.Crm.Enum.TypeXrmSdk.Money:
                    return ((Microsoft.Xrm.Sdk.Money)valueCRM).Value;
                case SDKore.Crm.Enum.TypeXrmSdk.Datetime:
                    return ((DateTime)valueCRM);
                    //return ((DateTime)valueCRM).ToLocalTime();
                    //Atenção, usar o LocalTime no Framework altera o comportamento de hora de todo o sistema, e pode ser perigoso para tratamento de horas do banco de dados e do plugin
                    //Foi decidido usar o LocalTime() somente na chamada das horas, quando necessário
                default:
                    return valueCRM;
            }
        }

        /// <summary>
        /// Recupera valor do atributo da Entidade.
        /// </summary>
        /// <param name="entity">Entidade do CRM</param>
        /// <param name="attribute">Atributo para recuperar valor.</param>
        /// <returns>Valor do atributo</returns>
        private static object TryGetValue(this Entity entity, string attribute)
        {
            object obj = null;
            try
            { obj = entity.GetAttributeValue<object>(attribute); }
            catch
            { }
            return obj;
        }
    }
}
