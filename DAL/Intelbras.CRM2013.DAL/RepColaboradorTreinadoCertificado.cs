using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepColaboradorTreinadoCertificado<T> : CrmServiceRepository<T>, IColaboradorTreinadoCertificado<T>
    {
        public T ObterPor(Guid colaboradorTreinId)
        {
            var query = GetQueryExpression<T>(true);

            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_colaboradorestreincertid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, colaboradorTreinId);
            query.Criteria.Conditions.Add(cond1);

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);


            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
            throw new NotImplementedException();
        }

        public T ObterPor(Int32 idMatricula)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_idmatricula", ConditionOperator.Equal, idMatricula);
            query.Criteria.Conditions.Add(cond1);

            #endregion

            var colecao = this.RetrieveMultiple(query);

            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        //
        public List<T> ListarPor(Guid contatoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_contactid", ConditionOperator.Equal, contatoId);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(Guid canalId, Guid treinamentoId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            query.Criteria.AddCondition("itbc_validade", ConditionOperator.GreaterEqual, DateTime.Now);
            query.Criteria.AddCondition("itbc_accountid", ConditionOperator.Equal, canalId.ToString());
            query.Criteria.AddCondition("itbc_treinamcertifid", ConditionOperator.Equal, treinamentoId.ToString());
                                         
            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarExpirados()
        {
            var query = GetQueryExpression<T>(true);

            #region Condições


            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_validade", ConditionOperator.LessThan, DateTime.Now);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCanal(Guid accountid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            //query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);
            //query.Criteria.AddCondition("itbc_validade", ConditionOperator.GreaterEqual, DateTime.Now);
            //query.Criteria.AddCondition("itbc_accountid", ConditionOperator.Equal, accountid);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorCanalTreinamentosAprovadosValidos(Guid accountid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("statuscode", ConditionOperator.Equal, 1);
            query.Criteria.AddCondition("itbc_validade", ConditionOperator.GreaterEqual, DateTime.Now);
            query.Criteria.AddCondition("itbc_accountid", ConditionOperator.Equal, accountid);

            //Faz join com itbc_treinamcertif para trazer a modalidade, tem que ser Presencial.
            LinkEntity link = query.AddLink("itbc_treinamcertif", "itbc_treinamcertifid", "itbc_treinamcertifid", JoinOperator.Inner);
            link.EntityAlias = "ct";
            query.Criteria.AddCondition("ct", "itbc_modalidade_curso", ConditionOperator.Equal, "PRESENCIAL");

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Boolean AlterarStatus(Guid treinamentoid, int status)
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
                EntityMoniker = new EntityReference("itbc_colaboradorestreincert", treinamentoid),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);

            if (resp != null)
                return true;

            return false;
        }
    }
}
