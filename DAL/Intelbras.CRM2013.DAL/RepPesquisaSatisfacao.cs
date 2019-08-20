using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.SharepointWebService;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.IRepository;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.DAL
{
    public class RepPesquisaSatisfacao<T> : CrmServiceRepository<T>, IRepositoryBase
    {
    }
}
