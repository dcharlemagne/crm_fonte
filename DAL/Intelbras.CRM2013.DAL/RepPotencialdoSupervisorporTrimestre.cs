using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepPotencialdoSupervisorporTrimestre<T> : CrmServiceRepository<T>, IPotencialdoSupervisorporTrimestre<T>
    {
        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_unidadedenegocioid", ConditionOperator.Equal, unidadeNegocioId);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);

            if (trimestre.HasValue)
            {
                query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre.Value);
            }

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<PotencialdoSupervisorporTrimestre> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres? trimestre = null)
        {
            var lista = new List<PotencialdoSupervisorporTrimestre>();

            string fetch = @"<fetch aggregate='true' no-lock='true'>
                              <entity name='itbc_metadokaporsegmento' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <attribute name='itbc_supervisorid' alias='supervisor' groupby='true' />
                                <attribute name='itbc_trimestre' alias='trimestre' groupby='true' />
                                <filter type='and'>
                                  <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  <condition attribute='itbc_unidadedenegocioid' operator='eq' value='{1}' />
                                  {2}
                                </filter>
                              </entity>
                            </fetch>";
            

            string filterTrimestre = string.Empty;
            if (trimestre.HasValue)
            {
                filterTrimestre = string.Format(@"<condition attribute='itbc_trimestre' operator='eq' value='{0}' />", (int)trimestre.Value);
            }

            fetch = string.Format(fetch, ano, unidadeNegocioId, filterTrimestre);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;


            foreach (var item in collection.Entities)
            {
                var supervisor = ((EntityReference)((AliasedValue)item.Attributes["supervisor"]).Value);

                var potencial = new PotencialdoSupervisorporTrimestre(OrganizationName, IsOffline, Provider)
                {
                    PotencialPlanejado = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    PotencialRealizado = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value,
                    Supervisor = new SDKore.DomainModel.Lookup(supervisor.Id, supervisor.Name, supervisor.LogicalName),
                    Trimestre = ((OptionSetValue)((AliasedValue)item.Attributes["trimestre"]).Value).Value
                };

                lista.Add(potencial);
            }

            return lista;
        }
    }
}