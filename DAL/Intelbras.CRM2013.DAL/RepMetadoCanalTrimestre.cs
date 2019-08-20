using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
   public class RepMetadoCanalPorTrimestre<T> : CrmServiceRepository<T>, IMetadoCanalPorTrimestre<T>
    {
        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_unidadenegocioid", ConditionOperator.Equal, unidadeNegocioId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorUnidadeNegocio(Guid unidadeNegocioId, int ano, Domain.Enum.OrcamentodaUnidade.Trimestres trimestre)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_ano", ConditionOperator.Equal, ano);
            query.Criteria.AddCondition("itbc_trimestre", ConditionOperator.Equal, (int)trimestre);
            query.Criteria.AddCondition("itbc_unidadenegocioid", ConditionOperator.Equal, unidadeNegocioId);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<MetadoCanalporTrimestre> ListarValoresPorUnidadeNegocio(Guid unidadeNegocioId, int ano)
        {
            var lista = new List<MetadoCanalporTrimestre>();

            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_metadocanalporsegmento' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <link-entity name='itbc_metatrimestrecanal' from='itbc_metatrimestrecanalid' to='itbc_metatrimestrecanalid' >
                                  <attribute name='itbc_metatrimestrecanalid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_unidadenegocioid' operator='eq' value='{1}' />
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";

            fetch = string.Format(fetch, ano, unidadeNegocioId);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var meta = new MetadoCanalporTrimestre(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(meta);
            }

            return lista;
        }

        public List<MetadoCanalporTrimestre> ListarValoresPorUnidadeNegocioResunmida(Guid unidadeNegocioId, int ano)
        {
            var lista = new List<MetadoCanalporTrimestre>();

            string fetch = @"<fetch aggregate='true' no-lock='true' >
                              <entity name='itbc_metadetalhadadocanalporproduto' >
                                <attribute name='itbc_metaplanejada' alias='valor_planejado' aggregate='sum' />
                                <attribute name='itbc_metarealizada' alias='valor_realizado' aggregate='sum' />
                                <link-entity name='itbc_metatrimestrecanal' from='itbc_metatrimestrecanalid' to='itbc_meta_canal_trimestreid' >
                                  <attribute name='itbc_metatrimestrecanalid' alias='id' groupby='true' />
                                  <filter type='and' >
                                    <condition attribute='itbc_ano' operator='eq' value='{0}' />
                                    <condition attribute='itbc_unidadenegocioid' operator='eq' value='{1}' />
                                  </filter>
                                </link-entity>
                              </entity>
                            </fetch>";
            
            fetch = string.Format(fetch, ano, unidadeNegocioId);

            var retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(fetch)
            };

            EntityCollection collection = ((RetrieveMultipleResponse)this.Execute(retrieveMultiple)).EntityCollection;

            foreach (var item in collection.Entities)
            {
                var meta = new MetadoCanalporTrimestre(OrganizationName, IsOffline, Provider)
                {
                    ID = (Guid)((AliasedValue)(item.Attributes["id"])).Value,
                    MetaPlanejada = ((Money)((AliasedValue)item.Attributes["valor_planejado"]).Value).Value,
                    MetaRealizada = ((Money)((AliasedValue)item.Attributes["valor_realizado"]).Value).Value
                };

                lista.Add(meta);
            }

            return lista;
        }
    }
}