using System;
using System.Collections.Generic;
using System.Linq;
using SDKore.Crm;
using Intelbras.CRM2013.Domain.IRepository;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.DAL
{
    public class RepProdutoCondicaoPagamento<T> : CrmServiceRepository<T>, IProdutoCondicaoPagamento<T>
    {
        //void AlterarStatus(Guid ProdutoCondicaoPagamentoId, int status);
        public void AlterarStatus(Guid ProdutoCondicaoPagamentoId, int status)
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
                EntityMoniker = new EntityReference("itbc_produtodacondiodepagamento", ProdutoCondicaoPagamentoId),
                State = new OptionSetValue(stateCode),
                Status = new OptionSetValue(status)
            };

            SetStateResponse resp = (SetStateResponse)this.Execute(request);


        }
    }
}
