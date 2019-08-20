using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDKore.Helper.Cache;
using SDKore.Helper;
using SDKore.DomainModel;
using SDKore.Crm.Util;

namespace SDKore.Crm.Base
{
    public class SDKoreBase
    {
        #region Contrutores

        public SDKoreBase(string organization, bool isOffline, object provider)
        {
            this.OrganizationName = organization;
            this.IsOffline = isOffline;
            if (provider != null) this.Provider = provider;

            this.NullableProperties = new List<string>();
            RepositoryFactory.SetTag(organization);
        }

        public SDKoreBase(string organization, bool isOffline)
        {
            this.OrganizationName = organization;
            this.IsOffline = isOffline;

            this.NullableProperties = new List<string>();
            RepositoryFactory.SetTag(organization);
        }

        #endregion

        #region Atributos

        public List<string> NullableProperties { get; set; }
        public string OrganizationName { get; set; }
        public bool IsOffline { get; set; }
        public object Provider { get; set; }

        [LogicalAttribute("createdon")]
        public DateTime? CriadoEm { get; set; }

        [LogicalAttribute("modifiedon")]
        public DateTime? ModificadoEm { get; set; }

        protected ICacheManager GerenciadorCache
        {
            get
            {
                return CacheFactory.Instance.Container.Resolve<ICacheManager>();
            }
        }

        #endregion

        #region Métodos

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

        #endregion
    }
}
