using Intelbras.CRM2013.Domain.Enum;
using Intelbras.CRM2013.Domain.IntelbrasService;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SDKore.Helper;

namespace Intelbras.CRM2013.DAL
{
    public class OcorrenciaBaseRepository<T> : CrmServiceRepository<T>, IOcorrenciaBase<T>
    {
        public T ObterPor(string numeroDaOcorrencia)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("ticketnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, numeroDaOcorrencia));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public List<T> ListarPorStatus(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            var query = GetQueryExpression<OcorrenciaBase>(true);
            //var query = new QueryExpression("incident");
            //query.ColumnSet.AllColumns = true;
            //query.ColumnSet.AddColumns("ticketnumber", "createdon", "statuscode", "statecode", "accountid", "productserialnumber", "productid", "followupby", "new_data_hora_escalacao");
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCpfCnpjCliente(string cpfCnpj, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            List<T> retorno = new List<T>();
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            //Busca pelas contas, depois pelos contatos
            query.AddLink("account", "customerid", "accountid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.InputMask()));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);
            //Busca pelos contatos
            query.LinkEntities.Clear();
            query.AddLink("contact", "customerid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, cpfCnpj.InputMask()));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);

            return retorno;
        }

        public List<T> ListarPorNomeCliente(string nomeCliente, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            List<T> retorno = new List<T>();

            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            //Busca pelas contas, depois pelos contatos
            query.AddLink("account", "customerid", "accountid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("name", ConditionOperator.Equal, nomeCliente));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);
            //Busca pelos contatos
            query.LinkEntities.Clear();
            query.AddLink("contact", "customerid", "contactid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("fullname", ConditionOperator.Equal, nomeCliente));

            foreach (T item in (List<T>)this.RetrieveMultiple(query).List)
                retorno.Add(item);

            return retorno;
        }

        public List<T> ListarPorNumeroSerie(string numeroSerie, Domain.Model.Conta assistenciaTecnica, string[] status, string[] origem)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("productserialnumber", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, numeroSerie));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
        public List<T> ListarPorStatusDiagnostico(DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, string[] statusDiagnostico, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("caseorigincode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, status));

            query.AddLink("new_diagnostico_ocorrencia", "incidentid", "new_ocorrenciaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", ConditionOperator.Equal, statusDiagnostico));

            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));

            query.Distinct = true;
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorNotaFiscalCompra(string notaFical, DateTime? dtAberturaIncio, DateTime? dtAberturaFim, string[] status, Domain.Model.Conta assistenciaTecnica, string[] origem)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new ConditionExpression("caseorigincode", ConditionOperator.In, origem));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.In, status));
            query.Criteria.Conditions.Add(new ConditionExpression("new_numero_nf_consumidor", ConditionOperator.Equal, notaFical));
            if (dtAberturaIncio.HasValue)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_origem", ConditionOperator.GreaterEqual, dtAberturaIncio.Value.Date));
            if (dtAberturaFim.HasValue)
                query.Criteria.Conditions.Add(new ConditionExpression("new_data_origem", ConditionOperator.LessThan, dtAberturaFim.Value.Date));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarOcorrenciaAbertaComIntervencaoTecnica(Domain.Model.Conta assistenciaTecnica)
        {
            var query = GetQueryExpression<T>(true);
            query.Orders.Add(new OrderExpression("new_data_origem", OrderType.Descending));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_autorizadaid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, assistenciaTecnica.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", Microsoft.Xrm.Sdk.Query.ConditionOperator.In, 200035, 200036, 200037, 200038, 200039, 200045));
            query.AddLink("new_intervencao_tecnica", "incidentid", "new_ocorrenciaid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statuscode", ConditionOperator.Equal, 1));
            query.Distinct = true;
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Extrato extrato, Domain.Model.Conta assistenciaTecnica = null)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_extrato_pagamentoid", ConditionOperator.Equal, extrato.Id));
            if (assistenciaTecnica != null)
                query.Criteria.Conditions.Add(new ConditionExpression("new_autorizadaid", ConditionOperator.Equal, assistenciaTecnica.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }
    }
}
