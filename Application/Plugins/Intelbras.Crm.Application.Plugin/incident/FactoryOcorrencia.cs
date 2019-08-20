using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.Crm.Domain.Model;
using Microsoft.Crm.Sdk;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.ValueObjects;

namespace Intelbras.Crm.Application.Plugin.incident
{
    public class FactoryOcorrencia
    {
        Organizacao Organizacao = null;
        Guid Id = Guid.Empty;

        public FactoryOcorrencia(string organizacao)
        {
            this.Organizacao = new Organizacao(organizacao);
        }

        public FactoryOcorrencia(string organizacao, Guid id)
        {
            this.Organizacao = new Organizacao(organizacao);
            this.Id = id;
        }

        public Ocorrencia CriarOcorrencia(params  DynamicEntity[] entidades)
        {
            Ocorrencia ocorrencia = new Ocorrencia(this.Organizacao);
            ocorrencia.Id = this.Id;
            ocorrencia.DataDeConclusao = InstanciarData("new_data_hora_conclusao", entidades);

            foreach (var entidade in entidades)
            {
                ocorrencia = this.CriarOcorrencia(ocorrencia, entidade);
            }
            
            return ocorrencia;
        }

        private Ocorrencia CriarOcorrencia(Ocorrencia ocorrencia, DynamicEntity entidade)
        {
            if (entidade.Properties.Contains("new_autorizadaid") && ocorrencia.Autorizada == null)
                ocorrencia.Autorizada = new Cliente
                {
                    Id = ((Lookup)entidade.Properties["new_autorizadaid"]).Value
                };

            if (entidade.Properties.Contains("productid") && ocorrencia.Produto == null)
                ocorrencia.Produto = new LookupVO
                {
                    Id = ((Lookup)entidade.Properties["productid"]).Value
                };

            if (entidade.Properties.Contains("customerid") && ocorrencia.Cliente == null)
            {
                var customerid = ((Customer)entidade.Properties["customerid"]);
                ocorrencia.Cliente = new LookupVO
                {
                    Id = customerid.Value,
                    Nome = customerid.name,
                    Tipo = customerid.type
                };
            }
            if (entidade.Properties.Contains("productserialnumber") && ocorrencia.ProdutosDoCliente == null)
                ocorrencia.ProdutosDoCliente = entidade.Properties["productserialnumber"].ToString();

            if (entidade.Properties.Contains("new_numero_nf_consumidor") && ocorrencia.NotaFiscalFatura == null)
                ocorrencia.NotaFiscalFatura = new NotaFiscal
                {
                    Codigo = entidade.Properties["new_numero_nf_consumidor"].ToString()
                };

            if (entidade.Properties.Contains("caseorigincode") && ocorrencia.Origem == null)
                ocorrencia.Origem = ((Picklist)entidade.Properties["caseorigincode"]).Value;

            if (entidade.Properties.Contains("new_reincidenteid") && ocorrencia.OcorrenciaPai == null)
                ocorrencia.OcorrenciaPai = new Ocorrencia
                {
                    Id = ((Lookup)entidade.Properties["new_reincidenteid"]).Value
                };

            if (entidade.Properties.Contains("new_numero_nota_fiscal") && ocorrencia.NumeroNotaFiscal == null)
                ocorrencia.NumeroNotaFiscal = entidade.Properties["new_numero_nota_fiscal"].ToString();

            if (entidade.Properties.Contains("statuscode"))
            {
                var statuscode = entidade.Properties["statuscode"] as Status;

                if (ocorrencia.StatusDaOcorrencia == Intelbras.Crm.Domain.Model.Enum.StatusDaOcorrencia.Vazio)
                    ocorrencia.StatusDaOcorrencia = (StatusDaOcorrencia)statuscode.Value;

                if (ocorrencia.Status == null)
                    ocorrencia.Status = new PicklistVO(statuscode.Value, statuscode.name);
            }

            #region CrmDateTime

            if (entidade.Properties.Contains("new_data_origem") && (ocorrencia.DataOrigem == null || ocorrencia.DataOrigem == DateTime.MinValue))
                ocorrencia.DataOrigem = Convert.ToDateTime(((CrmDateTime)entidade.Properties["new_data_origem"]).Value);

            if (entidade.Properties.Contains("new_data_ajuste_posto") && ocorrencia.DataAjustePosto == null)
                ocorrencia.DataAjustePosto = Convert.ToDateTime(((CrmDateTime)entidade.Properties["new_data_ajuste_posto"]).Value);

            if (entidade.Properties.Contains("new_data_envio_ajuste_posto") && ocorrencia.DataEnvioAjustePosto == null)
                ocorrencia.DataEnvioAjustePosto = Convert.ToDateTime(((CrmDateTime)entidade.Properties["new_data_envio_ajuste_posto"]).Value);

            if (entidade.Properties.Contains("new_data_entrega_cliente") && ocorrencia.DataDeConsertoInformada == null)
                ocorrencia.DataDeConsertoInformada = Convert.ToDateTime(((CrmDateTime)entidade.Properties["new_data_entrega_cliente"]).Value);

            if (entidade.Properties.Contains("new_data_entrega_cliente_real") && ocorrencia.DataDeEntregaClienteDigitada == null)
                ocorrencia.DataDeEntregaClienteDigitada = Convert.ToDateTime(((CrmDateTime)entidade.Properties["new_data_entrega_cliente_real"]).Value);

            if (entidade.Properties.Contains("new_data_hora_escalacao") && ocorrencia.DataEscalacao == null)
                ocorrencia.DataEscalacao = Convert.ToDateTime(((CrmDateTime)entidade.Properties["new_data_hora_escalacao"]).Value);

            if (entidade.Properties.Contains("followupby") && ocorrencia.DataSLA == null)
                ocorrencia.DataSLA = Convert.ToDateTime(((CrmDateTime)entidade.Properties["followupby"]).Value);

            #endregion

            if (entidade.Properties.Contains("contractid") && ocorrencia._contrato == null)
                ocorrencia._contrato = new Contrato { Id = ((Lookup)entidade.Properties["contractid"]).Value };

            if (entidade.Properties.Contains("casetypecode") && ocorrencia.TipoDeOcorrencia == null)
                ocorrencia.TipoDeOcorrencia = (TipoDeOcorrencia)((Picklist)entidade.Properties["casetypecode"]).Value;

            if (entidade.Properties.Contains("prioritycode") && ocorrencia.Prioridade == null)
                ocorrencia.Prioridade = (TipoDePrioridade)((Picklist)entidade.Properties["prioritycode"]).Value;

            if (entidade.Properties.Contains("new_localidadeid") && ocorrencia.Localidade == null)
                ocorrencia.Localidade = new LookupVO() { Id = ((Lookup)entidade.Properties["new_localidadeid"]).Value, Nome = ((Lookup)entidade.Properties["new_localidadeid"]).name };

            if (entidade.Properties.Contains("new_valor_servico") && ocorrencia.ValorServico == null)
                ocorrencia.ValorServico = ((CrmMoney)entidade.Properties["new_valor_servico"]).Value;

            if (entidade.Properties.Contains("subjectid") && ocorrencia.Assunto == null)
                ocorrencia.Assunto = ((Lookup)entidade.Properties["subjectid"]).Value;

            if (entidade.Properties.Contains("new_cidade") && ocorrencia.Cidade == null)
                ocorrencia.Cidade = entidade.Properties["new_cidade"].ToString();

            if (entidade.Properties.Contains("new_uf") && ocorrencia.Estado == null)
                ocorrencia.Estado = entidade.Properties["new_uf"].ToString();


            if (entidade.Properties.Contains("new_acao_final2") && ocorrencia.AcaoFinal == null)
                ocorrencia.AcaoFinal = new LookupVO() { Id = ((Lookup)entidade.Properties["new_acao_final2"]).Value, Nome = ((Lookup)entidade.Properties["new_acao_final2"]).name };

            if (entidade.Properties.Contains("new_solucao_asstec") && ocorrencia.TipoDeOcorrencia == null)
                ocorrencia.new_solucao_asstec = (AcaoAssistencia)((Picklist)entidade.Properties["new_solucao_asstec"]).Value;

            if (entidade.Properties.Contains("new_gera_atividade"))
                ocorrencia.ManterAberto = Convert.ToBoolean(((CrmBoolean)entidade.Properties["new_gera_atividade"]).Value);


            return ocorrencia;
        }

        private DateTime? InstanciarData(string columns, DynamicEntity[] entidades)
        {
            foreach (var entidade in entidades)
            {
                if (entidade.Properties.Contains(columns))
                {
                    var new_data_hora_conclusao = entidade.Properties[columns] as CrmDateTime;

                    if (new_data_hora_conclusao.IsNull)
                        return null;
                    else
                        return Convert.ToDateTime(new_data_hora_conclusao.Value);
                }
            }

            return null;
        }
    }
}
