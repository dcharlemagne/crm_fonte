using SDKore.Crm.Util;
using SDKore.DomainModel;
using SDKore.Helper;
using SDKore.Helper.Cache;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Model
{
    [Serializable]
    public class DomainBase
    {
        public bool Erro { get; set; }
        public string MensagemErro { get; set; }
        public string ExportaERP { get; set; }
        public string CodigoEms { get; set; }
        public List<string> NullableProperties { get; set; }
        public string OrganizationName { get; set; }
        public bool IsOffline { get; set; }
        [LogicalAttribute("id")]
        public Guid Id { get; set; }
        [LogicalAttribute("statuscode")]
        public int? RazaoStatus { get; set; }
        [LogicalAttribute("createdon")]
        public DateTime? CriadoEm { get; set; }
        [LogicalAttribute("createdby")]
        public Lookup CriadoPor { get; set; }
        [LogicalAttribute("modifiedon")]
        public DateTime? ModificadoEm { get; set; }
        [LogicalAttribute("modifiedby")]
        public Lookup ModificadoPor { get; set; }
        [LogicalAttribute("statecode")]
        public int? Status { get; set; }

        public DomainBase()
        {
        
        }

        public DomainBase(string organization, bool isOffline)
        {
            OrganizationName = organization;
            IsOffline = isOffline;
            NullableProperties = new List<string>();

            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        public DomainBase(string organization, bool isOffline, object provider)
        {
            OrganizationName = organization;
            IsOffline = isOffline;
            NullableProperties = new List<string>();

            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }

        protected ICacheManager GerenciadorCache
        {
            get
            {
                return CacheFactory.Instance.Container.Resolve<ICacheManager>();
            }
        }

        public void AddNullProperty(string property)
        {
            NullableProperties.Add(property);
        }

        public void AddNullProperty(params string[] properties)
        {
            for (int index = 0; index < properties.Length; index++)
                this.AddNullProperty(properties[index]);
        }

        public void ClearNullProperty()
        {
            this.NullableProperties = new List<string>();
        }

        public void removeNullProperty(params string[] properties)
        {
            for (int index = 0; index < properties.Length; index++)
                this.NullableProperties.Remove(properties[index]);
        }
    }
}
