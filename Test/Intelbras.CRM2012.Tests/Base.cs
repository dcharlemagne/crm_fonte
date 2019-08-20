using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDKore.Helper.Cache;
using SDKore.Helper;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Tests
{
    public class Base
    {
        public string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        public bool IsOffline = false;

        public string usuario = "barramento";
        //public string usuario = "1DyE1FbyrhoFdY5rI1tZnTGGjNyHAh9kDWAuDElJ7H4=";
        public string senha = "vCha6rGxXmWHeu8YKWfeXW2E"; //- para o server 171
        //public string senha = "xZIpp/z5k5gYPQSHd5r3OuISxMmLdkcygkMo2cvSNxM=";
    }
}
