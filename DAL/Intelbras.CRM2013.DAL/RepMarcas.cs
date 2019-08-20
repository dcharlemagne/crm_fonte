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
    public class RepMarcas<T> : CrmServiceRepository<T>, IMarcas<T>
    {
        public List<T> ListarMarcasPorConta(string contaId)
        {
            var query = GetQueryExpression<T>(true);

                LinkEntity link = query.AddLink("itbc_itbc_marca_equipamento_account", "itbc_marca_equipamentoid", "itbc_marca_equipamentoid", JoinOperator.Inner);
                //query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StatusCode.Ativo);
                link.LinkCriteria.AddCondition(new ConditionExpression("accountid", ConditionOperator.Equal, contaId));

            return (List<T>)this.RetrieveMultiple(query).List;
        }
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
        public void associarMarcas(List<Marcas> lstMarcas, Guid contaId)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (Marcas marca in lstMarcas)
            {
                relatedEntities.Add(new EntityReference("itbc_marca_equipamento", (Guid)marca.ID));
            }

            this.Provider.Associate(
                   "account",
                   contaId,
                   new Relationship("itbc_itbc_marca_equipamento_account"),
                   relatedEntities
               );
        }
        public void desassociarMarcas(List<Marcas> lstMarcas, Guid contaId)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (Marcas marca in lstMarcas)
            {
                relatedEntities.Add(new EntityReference("itbc_marca_equipamento", (Guid)marca.ID));
            }

            this.Provider.Disassociate(
                   "account",
                   contaId,
                   new Relationship("itbc_itbc_marca_equipamento_account"),
                   relatedEntities
               );
        }
    }
}
