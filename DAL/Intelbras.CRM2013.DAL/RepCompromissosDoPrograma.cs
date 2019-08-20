using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System.Text;

namespace Intelbras.CRM2013.DAL
{
    public class RepCompromissosDoPrograma<T> : CrmServiceRepository<T>, ICompromissosDoPrograma<T>
    {
        public List<T> ListarPor(string nomeCompromisso)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_name", ConditionOperator.Equal, nomeCompromisso);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid CompromissosDoProgramaId)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_compromissosid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, CompromissosDoProgramaId);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(int codigo)
        {
            var query = GetQueryExpression<T>(true);

            query.Criteria.AddCondition("itbc_codigo", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, codigo.ToString());

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPorCompCanal(Guid CompromissosDoCanalId)
        {
            var query = GetQueryExpression<T>(true);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

           strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>                                             ");                     
           strFetchXml.Append("  <entity name='itbc_compromissos'>                                                                                              ");
           strFetchXml.Append("<attribute name='itbc_compromissosid' />        ");
           strFetchXml.Append("<attribute name='itbc_name' />                  ");
           strFetchXml.Append("<attribute name='createdon' />                  ");
           strFetchXml.Append("<attribute name='itbc_tipodemonitoramento' />   ");
           strFetchXml.Append("<attribute name='statecode' />                  ");
           strFetchXml.Append("<attribute name='overriddencreatedon' />        ");
           strFetchXml.Append("<attribute name='statuscode' />                 ");
           strFetchXml.Append("<attribute name='modifiedonbehalfby' />         ");
           strFetchXml.Append("<attribute name='modifiedby' />                 ");
           strFetchXml.Append("<attribute name='itbc_descricaoid' />           ");
           strFetchXml.Append("<attribute name='modifiedon' />                 ");
           strFetchXml.Append("<attribute name='createdonbehalfby' />          ");
           strFetchXml.Append("<attribute name='createdby' />                  ");
           strFetchXml.Append("<attribute name='itbc_codigo' />                "); 
           strFetchXml.Append("    <order attribute='itbc_name' descending='false' />                                                                           ");
           strFetchXml.Append("    <link-entity name='itbc_compdocanal' from='itbc_compdoprogid' to='itbc_compromissosid' alias='ae'>                           ");
           strFetchXml.Append("      <filter type='and'>                                                                                                        ");
           strFetchXml.AppendFormat("        <condition attribute='itbc_compdocanalid' operator='eq' uitype='itbc_compdocanal' value='{0}' />        ", CompromissosDoCanalId);
           strFetchXml.Append("      </filter>                                                                                                                  ");
           strFetchXml.Append("    </link-entity>                                                                                                               ");
           strFetchXml.Append("  </entity>                                                                                                                      ");
           strFetchXml.Append("</fetch>                                                                                                                         ");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            var colecao = this.RetrieveMultiple(retrieveMultiple.Query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        
    }
}
