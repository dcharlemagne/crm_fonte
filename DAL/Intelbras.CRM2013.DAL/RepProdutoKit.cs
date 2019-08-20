using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using System;
using System.Collections.Generic;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoKit<T> : CrmServiceRepository<T>, IProdutoKit<T>
    {
        public List<T> ListarPorProdutoPai(Guid produtoPaiId)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.Criteria.AddCondition("itbc_produtopaiid", ConditionOperator.Equal, produtoPaiId.ToString());

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorProdutoPai(string codigoProdutoPai)
        {
            var query = GetQueryExpression<T>(true);

            #region Condições

            query.AddLink("product", "itbc_produtopaiid", "productid")
              .LinkCriteria.AddCondition("productnumber", ConditionOperator.Equal, codigoProdutoPai);

            #endregion

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public Boolean AlterarStatus(Guid produtokitId, int status)
        {
            //int stateCode;
            //if (status == 0)
            //{
            //    //Ativar
            //    stateCode = 0;
            //    status = 1;
            //}
            //else
            //{
            //    //Inativar
            //    stateCode = 1;
            //    status = 2;
            //}

            //SetStateRequest request = new SetStateRequest
            //{
            //    EntityMoniker = new EntityReference("itbc_produtokit", produtokitId),
            //    State = new OptionSetValue(stateCode),
            //    Status = new OptionSetValue(status)
            //};

            //SetStateResponse resp = (SetStateResponse)this.Execute(request);

            //if (resp != null)
            //    return true;

            return false;
        }
    }
}
