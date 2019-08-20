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
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.DAL
{
    public class RepAssunto<T> : CrmServiceRepository<T>, IAssunto<T>
    {
        
        #region Metodos

        public bool TemAssuntoFilho(Guid assuntoId)
        {
            var queryHelper = new QueryExpression("subject");
            queryHelper.ColumnSet.AddColumn("subjectid");
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("parentsubject", ConditionOperator.Equal, assuntoId));

            var be = base.Provider.RetrieveMultiple(queryHelper);

            return (be.Entities.Count > 0);
        }

        public TipoDeAssunto BuscaTipoDeRelacaoPor(Assunto assunto)
        {
            TipoDeAssunto tipo = TipoDeAssunto.Vazio;
            var queryHelper = new QueryExpression("new_classificacao_assunto");
            queryHelper.ColumnSet.AddColumn("new_tipo_assunto");
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_assuntoid", ConditionOperator.Equal, assunto.Id));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_tipo_assunto", ConditionOperator.NotNull));

            var be = base.Provider.RetrieveMultiple(queryHelper);

            if (be.Entities.Count > 0)
                tipo = (TipoDeAssunto)((OptionSetValue)be.Entities[0]["new_tipo_assunto"]).Value;

            return tipo;
        }

        #endregion
    }
}
