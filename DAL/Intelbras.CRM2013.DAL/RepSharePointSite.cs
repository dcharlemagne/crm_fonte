using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepSharePointSite<T> : CrmServiceRepository<T>, ISharePointSite<T>
    {
        public List<T> ListarPor(Guid sharepointsiteid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("sharepointsiteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, sharepointsiteid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor()
        {
            var query = GetQueryExpression<T>(true);

            //Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("sharepointsiteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, sharepointsiteid);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.StateCode.Ativo));

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            
            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }


        public T ObterPor(Guid id)
        {
            var query = GetQueryExpression<T>(true);

            //Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("sharepointsiteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, sharepointsiteid);
            //query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(String relativeUrl)
        {
            var query = GetQueryExpression<T>(true);

            //Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("relativeurl", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, relativeUrl);
            //query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }


        public Boolean AlterarStatus(Guid sharepointsiteid, int status)
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
                EntityMoniker = new EntityReference("sharepointsite", sharepointsiteid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public void AssociarDiretorioAoRegistroCRM(string urlCompleta, string nomeEntidade, string nomeRegistro, Guid idRegistro)
        {
            Entity sharepoint = new Entity("sharepointdocumentlocation");
            sharepoint.Attributes["name"] = string.Concat("Documento do Site Padrão ", nomeRegistro);
            sharepoint.Attributes["description"] = string.Concat("Documentos relacionados ao registro de ", nomeEntidade);
            sharepoint.Attributes["absoluteurl"] = urlCompleta;
            sharepoint.Attributes["regardingobjectid"] = new EntityReference(nomeEntidade, idRegistro);

            var req = new Microsoft.Xrm.Sdk.Messages.CreateRequest();
            req.Target = sharepoint;

            Microsoft.Xrm.Sdk.Messages.CreateResponse resp = (Microsoft.Xrm.Sdk.Messages.CreateResponse)this.Execute(req);
        }
    }
}
