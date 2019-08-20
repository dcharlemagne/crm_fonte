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

namespace Intelbras.CRM2013.DAL
{
    public class RepTipoPosto<T> : CrmServiceRepository<T>, ITipoPosto<T>
    {
        public List<T> ListarPor(string login)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.AddLink("new_new_tipo_posto_account", "new_tipo_postoid", "new_tipo_postoid");
            query.LinkEntities[0].AddLink("account", "accountid", "accountid");
            query.LinkEntities[0].LinkEntities[0].AddLink("contact", "accountid", "accountid");
            query.LinkEntities[0].LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_login", ConditionOperator.Equal, login));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public decimal ObterMaiorValorPor(Domain.Model.Conta cliente)
        {
            decimal maiorValor = decimal.MinValue;

            string perfilAsssistencia = "";

            switch (cliente.PerfilAssistenciaTecnica)
            {
                case 993520000:
                    perfilAsssistencia = "Assistência Técnica";
                    break;
                case 993520001:
                    perfilAsssistencia = "LAI";
                    break;
                case 993520002:
                    perfilAsssistencia = "Assistência Premium";
                    break;
            }

            QueryExpression query = new QueryExpression("new_tipo_posto");
            query.Criteria.AddCondition(new ConditionExpression("new_name", ConditionOperator.Equal, perfilAsssistencia));
            query.ColumnSet.AddColumns("new_tipo_postoid", "new_percentual_servico");
            query.AddLink("new_new_tipo_posto_account", "new_tipo_postoid", "new_tipo_postoid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("accountid", ConditionOperator.Equal, cliente.Id));
            query.Orders.Add(new OrderExpression("new_percentual_servico", OrderType.Descending));

            EntityCollection colecao = base.Provider.RetrieveMultiple(query);
            if (colecao.Entities.Count > 0)
            {
                var tipoPosto = colecao.Entities[0];
                if (tipoPosto.Attributes.Contains("new_percentual_servico"))
                    maiorValor = Convert.ToDecimal(tipoPosto["new_percentual_servico"]);
            }

            return maiorValor;
        }
    }
}