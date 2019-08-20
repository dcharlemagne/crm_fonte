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
    public class RepSegmentoComercial<T> : CrmServiceRepository<T>, ISegmentoComercial<T>
    {
        public T ObterPor(Guid itbc_segmentocomercialid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_segmentocomercialid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_segmentocomercialid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarSegmentoPorConta(string contaId)
        {
            var query = GetQueryExpression<T>(true);

            LinkEntity link = query.AddLink("itbc_itbc_segmentocomercial_account", "itbc_segmentocomercialid", "itbc_segmentocomercialid", JoinOperator.Inner);
            //query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, (int)Domain.Enum.Produto.StatusCode.Ativo);
            link.LinkCriteria.AddCondition(new ConditionExpression("accountid", ConditionOperator.Equal, contaId));

            return (List<T>)this.RetrieveMultiple(query).List;

        }
        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_codigo_site", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public Boolean AlterarStatus(Guid segmentoId, int status)
        {
            int stateCode;
            if (status == 0)
            {
                //Ativar
                stateCode = 0;
                status = 1;
            }
            else
            {
                //Inativar
                stateCode = 1;
                status = 2;
            }

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_segmentocomercial", segmentoId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }
        public T ObterPorCodigo(string codigoSeg)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_codigo_site", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigoSeg);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        public void AssociarSegmentoComercial(List<SegmentoComercial> segmentoComerciais, Guid conta)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (SegmentoComercial segmentoComercial in segmentoComerciais)
            {
                relatedEntities.Add(new EntityReference("itbc_segmentocomercial", (Guid)segmentoComercial.ID));
            }

            this.Provider.Associate(
                   "account",
                   conta,
                   new Relationship("itbc_itbc_segmentocomercial_account"),
                   relatedEntities
               );
        }
        public void DesassociarSegmentoComercial(List<SegmentoComercial> segmentoComerciais, Guid conta)
        {
            EntityReferenceCollection relatedEntities = new EntityReferenceCollection();
            foreach (SegmentoComercial segmentoComercial in segmentoComerciais)
            {
                relatedEntities.Add(new EntityReference("itbc_segmentocomercial", (Guid)segmentoComercial.ID));
            }

            this.Provider.Disassociate(
                   "account",
                   conta,
                   new Relationship("itbc_itbc_segmentocomercial_account"),
                   relatedEntities
               );
        }
    }
}
