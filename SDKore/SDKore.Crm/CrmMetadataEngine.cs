using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using SDKore.Helper.Cache;
using SDKore.Helper;
using Microsoft.Xrm.Sdk.Messages;

namespace SDKore.Crm
{
    public class CrmMetadataEngine<T> : CrmServiceRepository<T>
    {       
        public CrmMetadataEngine()
        {
        }

        public ICacheManager GerenciadorCache
        {
            get
            {
                return CacheFactory.Instance.Container.Resolve<ICacheManager>();
            }
        }

        private List<EntityMetadata> _metadata;
        internal List<EntityMetadata> Metadata
        {
            get
            {
                _metadata = (List<EntityMetadata>)GerenciadorCache.Get(string.Format(Constants.MetadataKeyName, this.OrganizationName));
                if (_metadata == null)
                {
                    var request = new RetrieveAllEntitiesRequest()
                    {
                        RetrieveAsIfPublished = true,
                        EntityFilters = EntityFilters.Attributes
                    };
                    RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)Provider.Execute(request);

                    _metadata = response.EntityMetadata.ToList<EntityMetadata>();
                    GerenciadorCache.Add(string.Format(Constants.MetadataKeyName, this.OrganizationName), _metadata, 24);
                }
                return _metadata;
            }
        }
    }
}
