using System;
using System.Collections.Generic;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

namespace Intelbras.CRM2013.DAL
{
    public class RepConfiguracaoBeneficio<T> : CrmServiceRepository<T>, IConfiguracaoBeneficio<T>
    {

        public T ObterPorProduto(Guid? produto)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_produtoid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, produto);

            query.Criteria.AddCondition("statecode", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, (int)Domain.Enum.ConfiguracaoBeneficio.State.Ativo);
            query.Criteria.AddCondition("itbc_vigenciainicial", Microsoft.Xrm.Sdk.Query.ConditionOperator.LessEqual, DateTime.Today);
            query.Criteria.AddCondition("itbc_vigenciafinal", Microsoft.Xrm.Sdk.Query.ConditionOperator.GreaterEqual, DateTime.Today);

            #endregion

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public T ObterPor(Guid itbc_configuracaodebeneficioid)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições
            Microsoft.Xrm.Sdk.Query.ConditionExpression cond1 = new Microsoft.Xrm.Sdk.Query.ConditionExpression("itbc_configuracaodebeneficioid", Microsoft.Xrm.Sdk.Query.ConditionOperator.Equal, itbc_configuracaodebeneficioid);
            query.Criteria.Conditions.Add(cond1);
            #endregion

            #region Ordenações
            Microsoft.Xrm.Sdk.Query.OrderExpression ord1 = new Microsoft.Xrm.Sdk.Query.OrderExpression("itbc_name", Microsoft.Xrm.Sdk.Query.OrderType.Ascending);
            query.Orders.Add(ord1);
            #endregion

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);

            return colecao.List[0];
        }

        public Boolean AlterarStatus(Guid ConfiguracaoBeneficioid, int status)
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
                EntityMoniker = new EntityReference("itbc_configuracaodebeneficioid", ConfiguracaoBeneficioid),
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
