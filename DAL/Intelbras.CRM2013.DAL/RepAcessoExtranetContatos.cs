using System;
using System.Collections.Generic;
using System.Linq;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Intelbras.CRM2013.DAL
{
    public class RepAcessoExtranetContatos<T> : CrmServiceRepository<T>, IAcessoExtranetContatos<T>
    {
        public List<T> ListarPor(Guid itbc_acessosextranetcontatosid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_acessosextranetcontatosid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_acessosextranetcontatosid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorContato(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_contactid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contatoId);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }


        public List<T> ListarAcessosAtivosPorContato(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_contactid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contatoId);
            query.Criteria.Conditions.Add(cond1);
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);
            query.Criteria.AddCondition(cond2);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarTodos()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid itbc_acessosextranetcontatosid)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_acessosextranetcontatosid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_acessosextranetcontatosid);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid itbc_portadorid, int status)
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
                EntityMoniker = new EntityReference("itbc_acessosextranetcontatos", itbc_portadorid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }

        public bool MudarProprietario(Guid proprietario, string TipoProprietario)
        {
            Microsoft.Crm.Sdk.Messages.AssignRequest assignRequest = new Microsoft.Crm.Sdk.Messages.AssignRequest()
            {
                Assignee = new Microsoft.Xrm.Sdk.EntityReference
                {
                    LogicalName = TipoProprietario,
                    Id = proprietario
                },

                Target = new Microsoft.Xrm.Sdk.EntityReference("itbc_acessosextranetcontatos", proprietario)
            };


            if (this.Execute(assignRequest).Results.Any())
                return true;
            else
                return false;


        }

        public List<T> ListarPor(Guid canalId, Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            if (canalId != Guid.Empty)
                query.Criteria.AddCondition("itbc_canal", ConditionOperator.Equal, canalId);
            else
                query.Criteria.AddCondition("itbc_canal", ConditionOperator.Null);

            if (contatoId != Guid.Empty)
                query.Criteria.AddCondition("itbc_contactid", ConditionOperator.Equal, contatoId);
            else
                query.Criteria.AddCondition("itbc_contactid", ConditionOperator.Null);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCanal(Guid canalId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_canal", ConditionOperator.Equal, canalId);
            #endregion

            #region Ordenações
            OrderExpression ord1 = new OrderExpression("itbc_name", OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Guid VerificarEmail(string email)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AllColumns = true;
            LinkEntity link = query.AddLink("contact", "itbc_contactid", "contactid", JoinOperator.Inner);
            link.EntityAlias = "ctt";

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("ctt", "emailaddress1", ConditionOperator.Equal, email);

            EntityCollection colecao = base.Provider.RetrieveMultiple(query);
           
            if (colecao.Entities.Count > 0)
                if (colecao.Entities[0].Contains("itbc_contactid"))
                    return ((Microsoft.Xrm.Sdk.EntityReference)(colecao.Entities[0].Attributes["itbc_contactid"])).Id;

            return Guid.Empty;
        }

    }
}
