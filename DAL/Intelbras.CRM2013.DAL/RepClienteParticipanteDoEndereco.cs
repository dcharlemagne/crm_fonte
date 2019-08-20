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
    public class RepClienteParticipanteDoEndereco<T> : CrmServiceRepository<T>, IClienteParticipanteDoEndereco<T>
    {
 
        #region Metodos
        public List<T> ListarPor(Domain.Model.Conta cliente, Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, cliente.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Domain.Model.Conta cliente, Contrato contrato, string endereco)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            if (!String.IsNullOrEmpty(endereco))
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_rua", ConditionOperator.Like, "%" + endereco + "%"));
            if (cliente.Id != Guid.Empty)
                query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_clienteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, cliente.Id));
            else
            {
                query.AddLink("account", "new_clienteid", "accountid");
                if (!String.IsNullOrEmpty(cliente.CpfCnpj))
                    query.LinkEntities[0].LinkCriteria.AddCondition("itbc_cpfoucnpj", ConditionOperator.Like, "%" + cliente.CpfCnpj + "%");
                if (!String.IsNullOrEmpty(cliente.NomeFantasia))
                    query.LinkEntities[0].LinkCriteria.AddCondition("itbc_nomefantasia", ConditionOperator.Like, "%" + cliente.NomeFantasia + "%");
                if (!String.IsNullOrEmpty(cliente.Nome))
                    query.LinkEntities[0].LinkCriteria.AddCondition("name", ConditionOperator.Like, "%" + cliente.Nome + "%");
            }

            var list = this.RetrieveMultiple(query);

            return (List<T>) list.List;
        }

        public List<T> PesquisarPor(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> PesquisarPor(ClienteParticipante clienteParticipante)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_cliente_participanteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, clienteParticipante.Id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(ClienteParticipanteDoContrato clienteParticipanteDoContrato)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_cliente_participanteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, clienteParticipanteDoContrato.Id));
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0)); //busca os endereços ativos
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public void DeleteAll(Contrato contrato)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            query.Criteria.Conditions.Add(new Microsoft.Xrm.Sdk.Query.ConditionExpression("new_contratoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contrato.Id));
            DataCollection<Entity> result = base.Provider.RetrieveMultiple(query).Entities;
            
            foreach (var item in result)
                this.Delete(item.Id);
        }

        #endregion
    }
}
