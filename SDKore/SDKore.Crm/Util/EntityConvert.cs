using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Reflection;

namespace SDKore.Crm.Util
{
    public class EntityConvert
    {
        /// <summary>
        /// Efetua a conversão entre Domínio para Microsoft.Xrm.Sdk.Entity
        /// </summary>
        /// <param name="entityDomain">Classe de domínio</param>
        /// <param name="organization">Organização em execução</param>
        /// <returns>Entity preenchida com valores das propriedades de domínio.</returns>
        public static Entity Convert<T>(T entityDomain, string organization)
        {
            return Convert<T>(entityDomain, organization, false);
        }

        /// <summary>
        /// Efetua a conversão entre Domínio para Microsoft.Xrm.Sdk.Entity
        /// </summary>
        /// <param name="entityDomain">Classe de domínio</param>
        /// <param name="offline">Informa se está executando Online ou Offline</param>
        /// <param name="organization">Organização em execução</param>
        /// <returns>Entity preenchida com valores das propriedades de domínio.</returns>
        public static Entity Convert<T>(T entityDomain, string organization, bool offline)
        {
            string nameEntity = Utility.GetEntityName<T>();
            Entity entity = new Entity(nameEntity);

            var listLogicalAttribute = entityDomain.GetType().GetProperties().GetNameProperties();
            for (int index = 0; index < listLogicalAttribute.Count; index++)
            {
                string item = listLogicalAttribute[index];
                var logicalAttribute = Utility.GetLogicalAttribute<T>(item);
                if (logicalAttribute == null) continue;
                var value = Utility.GetValue<T>(entityDomain, item);
                if (value == null) continue;
                if (value.GetType() == typeof(String))
                    if (string.IsNullOrEmpty((string)value)) continue;
                PropertyInfo propTmp = entityDomain.GetType().GetProperty(item, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var typeXrmSdk = Utility.GetTypeAttribute(organization, offline, nameEntity, propTmp, value);
                //entity[logicalAttribute.Name] = ResolveProperty(typeXrmSdk, value);
                object valor = ResolveProperty(typeXrmSdk, value);
                //quando não usado DateTime? na criação do atributo, a data fica inválida para o CRM
                if (valor.GetType() == typeof(DateTime) && System.Convert.ToDateTime(valor) < System.Convert.ToDateTime("01/01/1900 00:00:00"))
                    continue;
                    //valor = null;

                Guid _guid = Guid.Empty;
                if (valor.GetType() == typeof(Guid) && !Guid.TryParse(System.Convert.ToString(valor), out _guid))
                {
                    //Se o Guid não for resolvido, não atribui o valor (erro no create quando incluia um Guid.Empty)
                }
                if (valor.GetType() == typeof(Guid) && _guid == Guid.Empty && (item.ToLower() == nameEntity.ToLower() + "id" || item.ToLower() == "id"))
                {
                    //Valida o ID de cada model que deve ser removido posteriormente
                    if (entity.Attributes.Contains(nameEntity.ToLower() + "id") && !Guid.TryParse(System.Convert.ToString(entity.Attributes[nameEntity.ToLower() + "id"]), out _guid))
                        entity.Attributes.Remove(nameEntity.ToLower() + "id");
                }
                else
                {
                    if (logicalAttribute.Name.ToLower() == "id" && nameEntity.ToLower() == "task")
                    {
                        entity["activityid"] = valor;
                        entity.Id = new Guid(value.ToString());
                    }
                    else
                    {
                        entity[(logicalAttribute.Name.ToLower() == "id" ? nameEntity.ToLower() + "id" : logicalAttribute.Name.ToLower())] = valor;
                        if (item.ToLower() == nameEntity.ToLower() + "id")
                            entity.Id = new Guid(value.ToString());
                    }                    
                }
            }
            List<string> itens = (List<string>)Utility.GetValue<T>(entityDomain, "NullableProperties");
            if (itens != null)
            {
                foreach (var item in itens)
                {
                    var attInfo = Utility.GetLogicalAttribute<T>(item);
                    if (attInfo == null) continue;
                    entity[attInfo.Name] = null;
                }
            }
            return entity;
        }

        private static object ResolveProperty(Crm.Enum.TypeXrmSdk typeXrmSdk, object valueDomain)
        {
            switch (typeXrmSdk)
            {
                case Enum.TypeXrmSdk.PartyList:
                    var temp = (SDKore.DomainModel.Lookup[])valueDomain;
                    var entities = new EntityCollection();
                    entities.EntityName = "activityparty";
                    for (int index = 0; index < temp.Length; index++)
                    {
                        var entityTmp = new Entity("activityparty");
                        entityTmp.Attributes["partyid"] = new EntityReference(temp[index].Type, temp[index].Id);
                        entities.Entities.Add(entityTmp);
                    }
                    return entities;

                case Enum.TypeXrmSdk.EntityReference:
                    return new EntityReference(((SDKore.DomainModel.Lookup)valueDomain).Type, ((SDKore.DomainModel.Lookup)valueDomain).Id);
                case Enum.TypeXrmSdk.OptionSetValue:
                    return new OptionSetValue((int)valueDomain);
                case Enum.TypeXrmSdk.Money:
                    return new Money((decimal)valueDomain);
                default:
                    return valueDomain;
            }
        }
    }
}
