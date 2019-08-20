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
    public class RepOportunidade<T> : CrmServiceRepository<T>, IOportunidade<T>
    {

        public void CriarAnotacaoParaUmLead(Guid leadId, Anotacao anotacao)
        {

            //var be = new annotation();
            //be.notetext = "Anexo do Lead";
            //be.subject = "Anexo do Lead";

            //be.objectid = new Lookup();
            //be.objectid.type = "lead";
            //be.objectid.Value = leadId;

            //be.objecttypecode = new EntityNameReference();
            //be.objecttypecode.Value = "lead";

            //var noteId = base.Provider.Create(be);

            //string encodedData = System.Convert.ToBase64String(anotacao.BytesDoArquivo);

            //be = new annotation();
            //be.annotationid = new Key();
            //be.annotationid.Value = noteId;
            //be.documentbody = encodedData;
            //be.filename = anotacao.NomeDoArquivo;
            //be.mimetype = @"application\ms-word";
            //be.isdocument = new CrmBoolean();
            //be.isdocument.Value = true;

            //base.Provider.Update(be);
        }

        public void CriarAnotacaoParaUmaOportunidade(Guid oportunidadeId, Anotacao anotacao)
        {

            //var be = new annotation();
            //be.notetext = "Anexo salvo";
            //be.subject = "Anexo salvo";

            //be.objectid = new Lookup();
            //be.objectid.type = "3";
            //be.objectid.Value = oportunidadeId;

            //be.objecttypecode = new EntityNameReference();
            //be.objecttypecode.Value = "opportunity";

            //string encodedData = System.Convert.ToBase64String(anotacao.BytesDoArquivo);

            ////be = new annotation();
            ////be.annotationid = new Key();
            ////be.annotationid.Value = noteId;
            //be.documentbody = encodedData;
            //be.filename = anotacao.NomeDoArquivo;
            //be.mimetype = @"application\ms-word";
            //be.isdocument = new CrmBoolean();
            //be.isdocument.Value = true;

            //var noteId = base.Provider.Create(be);

        }
        
        public List<T> ListarPor(Revenda revenda)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_revendaid", ConditionOperator.Equal, revenda.Revendaid));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, 0));
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T BuscarPor(ClientePotencial cliente)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new ConditionExpression("originatingleadid", ConditionOperator.Equal, cliente.Id));
            query.Orders.Add(new OrderExpression("name", OrderType.Ascending));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
    }
}
