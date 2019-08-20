using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.Helper;
using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Intelbras.CRM2013.Domain.ValueObjects;
using System.Xml;
using Intelbras.CRM2013.Domain.Enum;
using Microsoft.Xrm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepGrupoFidelidade<T> : CrmServiceRepository<T>, Domain.IRepository.IGrupoFidelidade<T>
    {
        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
