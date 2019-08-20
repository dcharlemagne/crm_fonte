using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;


namespace Intelbras.CRM2013.DAL
{
    public class RepRelacionamentoCanal<T> : CrmServiceRepository<T>, IRelacionamentoCanal<T>
    {
        public List<T> ListarPor(Guid? itbc_relacionamentodocanalid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_relacionamentodocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_relacionamentodocanalid);
            #endregion

            #region Validações

            if (itbc_relacionamentodocanalid.HasValue)
                query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid contaId, Domain.Enum.Conta.StateCode? stateCode = null)
        {
            var query = GetQueryExpression<T>(true);
            if (contaId != Guid.Empty)
                query.Criteria.AddCondition("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, contaId);

            if (stateCode.HasValue)
                query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)stateCode);

            query.Orders.Add(new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPor(Guid RelacionamentoDoCanalID)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_relacionamentodocanalid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, RelacionamentoDoCanalID);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenação
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid guidCanal, Guid guidSupervisor, Guid guidKeyAccount, Guid guidAssistente,DateTime dtInicial, DateTime dtFinal)

        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidCanal);
            query.Criteria.AddCondition("itbc_supervisorid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidSupervisor);
            query.Criteria.AddCondition("itbc_contactid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidKeyAccount);
            query.Criteria.AddCondition("itbc_assistenteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidAssistente);

            query.Criteria.AddCondition("itbc_datafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, dtInicial);
            query.Criteria.AddCondition("itbc_datainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan, dtFinal);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public List<T> ListarPor(Guid guidCanal, Guid guidSupervisor, Guid guidKeyAccount, Guid guidAssistente, DateTime dtInicial, DateTime dtFinal)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidCanal);
            query.Criteria.AddCondition("itbc_supervisorid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidSupervisor);
            query.Criteria.AddCondition("itbc_contactid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidKeyAccount);
            query.Criteria.AddCondition("itbc_assistenteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidAssistente);

            query.Criteria.AddCondition("itbc_datafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, dtInicial);
            query.Criteria.AddCondition("itbc_datainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan, dtFinal);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Intelbras.CRM2013.Domain.Enum.StateCode.Ativo);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorKeyAccount(Guid guidCanal,Guid guidKeyAccount, DateTime dtAtual)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            query.Criteria.AddCondition("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidCanal);
            query.Criteria.AddCondition("itbc_contactid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, guidKeyAccount);

            query.Criteria.AddCondition("itbc_datafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, dtAtual);
            query.Criteria.AddCondition("itbc_datainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessThan, dtAtual);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, 0);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterPorSupervisor(Guid Canal, Guid Supervisor)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_supervisorid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Supervisor);
            query.Criteria.Conditions.Add(cond1);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Canal);
            query.Criteria.Conditions.Add(cond3);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_datainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.Conditions.Add(cond4);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond5 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_datafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, DateTime.Today);
            query.Criteria.Conditions.Add(cond5);

            #endregion


            var colecao = this.RetrieveMultiple(query);



            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }
        
        public T ObterPorAssistente(Guid Canal, Guid Assistente)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições


            Microsoft.Xrm.Sdk.Query.ConditionExpression cond2 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_assistenteid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Assistente);
            query.Criteria.Conditions.Add(cond2);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond3 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_accountid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, Canal);
            query.Criteria.Conditions.Add(cond3);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond4 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_datainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.Conditions.Add(cond4);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond5 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_datafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, DateTime.Today);
            query.Criteria.Conditions.Add(cond5);

            #endregion        
           

            var colecao = this.RetrieveMultiple(query);

            

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public void AdicionarEquipe(Guid Equipe, Guid Supervisor)
        {
            Microsoft.Crm.Sdk.Messages.AddMembersTeamRequest req = new Microsoft.Crm.Sdk.Messages.AddMembersTeamRequest();
            req.TeamId = Equipe;
            Guid[] arrMembers = new Guid[] { Supervisor };
            req.MemberIds = arrMembers;
            var resp = (Microsoft.Crm.Sdk.Messages.AddMembersTeamResponse)this.Execute(req);  

        }

        public void RemoverEquipe(Guid Equipe, Guid Supervisor)
        {
            Microsoft.Crm.Sdk.Messages.RemoveMembersTeamRequest req = new Microsoft.Crm.Sdk.Messages.RemoveMembersTeamRequest();
            req.TeamId = Equipe;
            Guid[] arrMembers = new Guid[] { Supervisor };
            req.MemberIds = arrMembers;
            var resp = (Microsoft.Crm.Sdk.Messages.RemoveMembersTeamResponse)this.Execute(req);

        }

        public Boolean AlterarStatus(Guid id, int stateCode, int statusCode)
        {
            SetStateResponse resp = null;

            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("itbc_relacionamentodocanal", id),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(statusCode)
            };

            resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }
    }
}
