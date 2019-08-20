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
    public class RepValorDoServico<T> : CrmServiceRepository<T>, IValorDoServico<T>
    {
        public decimal ObterMaiorValorPor(Diagnostico servico)
        {
            decimal maiorValor = decimal.MinValue;
            if (servico.Ocorrencia == null && servico.OcorrenciaId != null)
                servico.Ocorrencia = new Domain.Servicos.RepositoryService().Ocorrencia.Retrieve(servico.OcorrenciaId.Id);
            if (servico != null
                && servico.Defeito != null
                && servico.Solucao != null
                && servico.Ocorrencia != null
                && servico.Ocorrencia.Produto != null
                && servico.Ocorrencia.Produto.DadosFamiliaComercial != null
                && servico.Ocorrencia.Produto.LinhaComercial != null)
            {
                QueryExpression query = new QueryExpression("new_valor_servico");
                query.ColumnSet.AddColumns("new_valor_servicoid", "new_valor");
                query.Criteria.Conditions.Add(new ConditionExpression("new_defeitoid", ConditionOperator.Equal, servico.Defeito.Id));
                query.Criteria.Conditions.Add(new ConditionExpression("new_servicoid", ConditionOperator.Equal, servico.Solucao.Id));
                query.Criteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, servico.Ocorrencia.Produto.LinhaComercial.Id));
                query.Orders.Add(new OrderExpression("new_valor", OrderType.Descending));

                EntityCollection colecao = base.Provider.RetrieveMultiple(query);
                if (colecao.Entities.Count > 0)
                {
                    if (colecao.Entities[0].Attributes.Contains("new_valor"))
                        maiorValor = ((Money)colecao.Entities[0]["new_valor"]).Value;
                }
            }
            return maiorValor;
        }

        public decimal ObterMaiorValorPor(Ocorrencia ocorrencia)
        {
            decimal maiorValor = decimal.MinValue;
            List<Diagnostico> servicos = (new Domain.Servicos.RepositoryService()).Diagnostico.ListarDiagnosticoPortalPor(ocorrencia);
            foreach (Diagnostico servico in servicos)
            {
                decimal maiorValorDoServico = this.ObterMaiorValorPor(servico);
                if (maiorValorDoServico > maiorValor)
                    maiorValor = maiorValorDoServico;
            }
            return maiorValor;
        }
    }
}