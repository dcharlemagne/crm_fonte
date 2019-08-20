using System;
using System.Data;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.DAL
{
    public class RepSinonimosMarcas<T> : CrmServiceRepository<T>, ISinonimosMarcas<T>
    {
        public T obterPorNome(string nome)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_name", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, nome);
            query.Criteria.Conditions.Add(cond1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public void associarSinonimosMarcas(List<SinonimosMarcas> lstSinonimosMarcas, Guid contaId)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (SinonimosMarcas sinonimosMarcas in lstSinonimosMarcas)
            {
                if (sinonimosMarcas != null)
                    relatedEntities.Add(new EntityReference("itbc_sinonimos_marcasid", (Guid)sinonimosMarcas.ID));
            }

            this.Provider.Associate(
                   "account",
                   contaId,
                   new Relationship("itbc_itbc_sinonimos_marcas_account"),
                   relatedEntities
               );
        }
    }
}
