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
using Intelbras.CRM2013.Domain.ValueObjects;

namespace Intelbras.CRM2013.DAL
{
    public class RepResgateDePremio<T> : CrmServiceRepository<T>, IResgateDePremio<T>
    {
        public List<ResgateDoParticipante> ListarPor(Guid participanteId)
        {
            //Lista de ValueObject
            List<ResgateDoParticipante> listaResgatesValueObject = new List<ResgateDoParticipante>();

            QueryExpression queryHelper = new QueryExpression("new_resgate_premio_fidelidade");
            queryHelper.ColumnSet.AllColumns = true;
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_participanteid", ConditionOperator.Equal, participanteId));

            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

            
            foreach (Entity item in colecao.Entities)
            {
                bool adicionar = true;
                int valor = 0;
                int index = 0;

                for (int i = 0; i < listaResgatesValueObject.Count; i++)
                {
                    if (listaResgatesValueObject[i].ProdutoId == ((EntityReference)item["new_premioid"]).Id)
                    {
                        adicionar = false;
                        valor = Convert.ToInt32(item["new_quantidade_pontos_utilizados"]);
                        index = i;
                        break;
                    }
                }

                if (adicionar)
                {
                    ResgateDoParticipante resgateParticipante = new ResgateDoParticipante();

                    if (item.Attributes.Contains("new_premioid")) resgateParticipante.ProdutoId = ((EntityReference)item["new_premioid"]).Id;
                    if (item.Attributes.Contains("new_premioid")) resgateParticipante.ProdutoNome = ((EntityReference)item["new_premioid"]).Name;
                    if (item.Attributes.Contains("new_quantidade_pontos_utilizados"))
                    {
                        resgateParticipante.PontosUnitario = Convert.ToInt32(item["new_quantidade_pontos_utilizados"]);
                        resgateParticipante.PontosTotal = Convert.ToInt32(item["new_quantidade_pontos_utilizados"]);
                    }
                    resgateParticipante.Quantidade = 1;
                    listaResgatesValueObject.Add(resgateParticipante);
                }
                else
                {
                    listaResgatesValueObject[index].Quantidade++;
                    listaResgatesValueObject[index].PontosTotal += valor;
                }
            }

            return listaResgatesValueObject;
        }
    }
}
