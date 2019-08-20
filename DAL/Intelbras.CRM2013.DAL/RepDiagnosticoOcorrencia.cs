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
using Intelbras.CRM2013.Domain.Enum;
using System.Diagnostics;

namespace Intelbras.CRM2013.DAL
{
    public class RepDiagnosticoOcorrencia<T> : CrmServiceRepository<T>, IDiagnosticoOcorrencia<T>
    {
       public void Cancelar(Guid id)
        {
            SetStateRequest request = new SetStateRequest
            {
                EntityMoniker = new EntityReference("new_defeito_ocorrencia_cliente", id),
                State = new OptionSetValue(0),
                Status = new OptionSetValue((int)StatusDoDiagnostico.Cancelado)
            };
            SetStateResponse resp = (SetStateResponse)this.Execute(request);
        }

        
        public List<T> ListarPor(Ocorrencia ocorrencia)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Orders.Add(new OrderExpression("new_name", OrderType.Ascending));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorrencia.Id));
            //queryHelper.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public void LimparCampoExtratoLogisticaReversa(Extrato extrato)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_extrato_logistica_reversaid", ConditionOperator.Equal, extrato.Id));
            var bec = base.Provider.RetrieveMultiple(queryHelper);
            foreach (Entity item in bec.Entities)
            {
                item["new_extrato_logistica_reversaid"] = null;
                base.Provider.Update(item);
            }
        }

        public Diagnostico BuscarDadosDoServicoPor(Domain.Model.Conta postoDeServico, Product produto, DefeitoOcorrenciaCliente defeito, Solucao solucao)
        {
            Diagnostico servico = new Diagnostico(this.OrganizationName, this.IsOffline);
            //Regra para pegar o valor do Serviço
            //1 - Busca a % de acréscimo ou decréscimo no valor de mão de obra por Posto de Serviço e Linha (Familia Comercial) em new_tipo_posto_linha 
            //2 - Busca o valor da mão de obra pelo Defeito e Serviço
            var queryValorServicoPosto = new QueryExpression("new_valor_servico_posto");
            queryValorServicoPosto.ColumnSet.AddColumns(new string[] { "new_linha_unidade_negocioid", "new_produtoid", "new_valor" });
            queryValorServicoPosto.Criteria.Conditions.Add(new ConditionExpression("new_clienteid", ConditionOperator.Equal, postoDeServico.Id));
            var collectionValorServicoPosto = base.Provider.RetrieveMultiple(queryValorServicoPosto);
            bool seraQueVai = false;
            foreach (Entity servicoCRM in collectionValorServicoPosto.Entities)
            {
                if ((servicoCRM.Attributes.Contains("new_linha_unidade_negocioid")
                    && produto.LinhaComercial != null
                    & ((EntityReference)servicoCRM["new_linha_unidade_negocioid"]).Id == produto.LinhaComercial.Id)
                    || (servicoCRM.Attributes.Contains("new_produtoid") && ((EntityReference)servicoCRM["new_produtoid"]).Id == produto.Id))
                {
                    seraQueVai = true;
                    if (servicoCRM.Attributes.Contains("new_valor") && ((Money)servicoCRM["new_valor"]).Value > 0)
                    {
                        servico.Valor = ((Money)servicoCRM["new_valor"]).Value;
                        break;
                    }
                    else
                        seraQueVai = false;
                }
            }

            string emailAviso = SDKore.Configuration.ConfigurationManager.GetSettingValue("EmailAvisoErrosASTEC");
      
            //verifica se o produto está com as configurações completas
            if (produto == null)
                return servico;
            else if (produto.DadosFamiliaComercial == null)
            {
                EventLog.WriteEntry("CRM ASTEC Produto sem Dados", "Produto " + produto.Codigo + " " + produto.Nome + " não possui Familia Comercial configurada");

                this.EnviarEmail(
                    produto.Id,
                    produto.Nome,
                    "product",
                    "Produto com Configuração Incompleta",
                    "Produto " + produto.Codigo + " " + produto.Nome + " não possui Familia Comercial configurada",
                    this.ObterIdEmailCorporativo("ID_EMAIL_CORPORATIVO"),
                    new List<KeyValuePair<Guid, string>>() { new KeyValuePair<Guid, string>(this.ObterIdEmailCorporativo("ID_EMAIL_AVISO"), "systemuser") }
                    );

                return servico;
            }
            else if (produto.LinhaComercial == null)
            {
                EventLog.WriteEntry("CRM ASTEC Produto sem Dados", "Produto " + produto.Codigo + " " + produto.Nome + " não possui Linha Comercial configurada");

                this.EnviarEmail(
                    produto.Id,
                    produto.Nome,
                    "product",
                    "Produto com Configuração Incompleta",
                    "Produto " + produto.Codigo + " " + produto.Nome + " não possui Linha Comercial configurada",
                    this.ObterIdEmailCorporativo("ID_EMAIL_CORPORATIVO"),
                    new List<KeyValuePair<Guid, string>>() { new KeyValuePair<Guid, string>(this.ObterIdEmailCorporativo("ID_EMAIL_AVISO"), "systemuser") }
                    );

                //try
                //{
                //    System.Web.Mail.SmtpMail.SmtpServer = "imap.intelbras.com.br";
                //    System.Web.Mail.SmtpMail.Send("nao_responda@intelbras.com.br", emailAviso, "Produto com Configuração Incompleta", "Produto " + produto.CodigoEms + " " + produto.Nome + " não possui Linha Comercial configurada");
                //}
                //catch { }
                return servico;
            }

            if (!seraQueVai)
            {
                var queryHelper = new QueryExpression("new_valor_servico");
                queryHelper.ColumnSet.AddColumn("new_valor");
                //Alterado em 01/07/2011 para Linha Comercial (Segmento) a pedido do Jackson Luiz
                //queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_familia_comercialid", ConditionOperator.Equal, familiaComercial.Id);
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_linha_unidade_negocioid", ConditionOperator.Equal, produto.LinhaComercial.Id));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_defeitoid", ConditionOperator.Equal, defeito.Id));
                queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_servicoid", ConditionOperator.Equal, solucao.Id));
                var bec = base.Provider.RetrieveMultiple(queryHelper);

                if (bec.Entities.Count > 0)
                {
                    if (bec[0].Attributes.Contains("new_valor"))
                        servico.Valor = ((Money)bec[0]["new_valor"]).Value;
                }
            }

            //Alterdo em 27/02/2012 por Carlos Roweder Nass 
            //Aplicar o acréscimo e decréscimo em qualquer uma das situações de valores a pedido de Gisele Rocha
            var queryTipoPosto = new QueryExpression("new_tipo_posto");
            queryTipoPosto.ColumnSet.AddColumn("new_percentual_servico");
            queryTipoPosto.Criteria.Conditions.Add(new ConditionExpression("new_percentual_servico", ConditionOperator.NotNull));
            queryTipoPosto.AddLink("new_new_tipo_posto_account", "new_tipo_postoid", "new_tipo_postoid");
            queryTipoPosto.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("accountid", ConditionOperator.Equal, postoDeServico.Id));
            var collectionTipoPosto = base.Provider.RetrieveMultiple(queryTipoPosto);
            if (collectionTipoPosto.Entities.Count > 0)
            {
                if (collectionTipoPosto[0].Attributes.Contains("new_percentual_servico"))
                    servico.Valor = servico.Valor + ((Convert.ToDecimal(collectionTipoPosto[0]["new_percentual_servico.Value"]) / 100) * servico.Valor);
            }

            return servico;
        }

        public void EnviarEmail(Guid referenteAId, string referenteANome, string referenteATipo, string assunto, string mensagem, Guid emailDe, List<KeyValuePair<Guid, string>> destinatarios)
        {
            //if (destinatarios.Count <= 0) return;

            //Email email = new Email(base.OrganizacaoCorrente);
            //email.ReferenteAId = referenteAId;
            //email.ReferenteAType = referenteATipo;
            //email.ReferenteAName = referenteANome;
            //email.Assunto = assunto;
            //email.Mensagem = mensagem;
            //email.De = new List<KeyValuePair<Guid, string>>();
            //email.De.Add(new KeyValuePair<Guid, string>(emailDe, "systemuser"));
            //email.Para = destinatarios;

            //email.Id = EmailRepository.Create(email);
            //EmailRepository.EnviarEmail(email.Id);
        }

        public List<T> ListarPagamentoPorAstec(int dia)
        {
            var diaExtrato = dia > 29 && dia <= 31 ? "ultimo_dia" : "dia_" + dia.ToString("#00");
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AddColumns("new_produtoid", "new_servicoid", "new_valor_mao_obra", "new_ocorrenciaid");
            // OS aprovado
            query.AddLink("incident", "new_ocorrenciaid", "incidentid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, "200042"));
            // cliente ativo
            query.LinkEntities[0].AddLink("account", "new_autorizadaid", "accountid");
            query.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, "1"));
            // dia do extrato de processamento do cliente
            query.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_gera_extrato_" + diaExtrato, ConditionOperator.Equal, "1"));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarDiagnosticosParaExportacaoDePedidosASTEC()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, StatusDoDiagnostico.AguardandoPeca)); 
            query.Criteria.Conditions.Add(new ConditionExpression("new_data_geracao_pedido", ConditionOperator.Null));
            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.NotNull));
            query.Criteria.Conditions.Add(new ConditionExpression("new_geratroca", ConditionOperator.Equal, true));
            query.Criteria.Conditions.Add(new ConditionExpression("new_qtd_solicitada", ConditionOperator.GreaterThan, 0));
            
            // Incident
            query.AddLink("incident", "new_ocorrenciaid", "incidentid");
            //query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.Equal, StatusDaOcorrencia.Aguardando_Peça));

            // Account
            query.LinkEntities[0].AddLink("account", "new_autorizadaid", "accountid");
            query.LinkEntities[0].LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_geracao_pedido_posto", ConditionOperator.NotNull));

            return (List<T>)this.RetrieveMultiple(query).List;
        }

         public Guid ObterIdEmailCorporativo(string identificador)
        {
            var emailDe = SDKore.Configuration.ConfigurationManager.GetSettingValue(identificador);
            return !string.IsNullOrEmpty(emailDe) ? new Guid(emailDe) : Guid.Empty;
        }

        public T ObterDuplicidade(Guid ocorrenciaId, Guid produtoId, Guid servicoId, Guid defeitoId, string notaFiscal, string serieNotaFiscal, Guid diagnosticoid)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorrenciaId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_produtoid", ConditionOperator.Equal, produtoId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_servicoid", ConditionOperator.Equal, servicoId));
            query.Criteria.Conditions.Add(new ConditionExpression("new_defeitoid", ConditionOperator.Equal, defeitoId));
            query.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));

            if (!string.IsNullOrEmpty(notaFiscal))
                query.Criteria.Conditions.Add(new ConditionExpression("new_nota_fiscal", ConditionOperator.Equal, notaFiscal));
            
            if (!string.IsNullOrEmpty(serieNotaFiscal))
                query.Criteria.Conditions.Add(new ConditionExpression("new_serie_nota_fiscal", ConditionOperator.Equal, serieNotaFiscal));
            
            if (diagnosticoid != Guid.Empty)
                query.Criteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.NotEqual, diagnosticoid));

            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public T ObterDiagnosticoPai(Guid id)
        {
            var query = GetQueryExpression<T>(true);
            query.AddLink("new_diagnostico_ocorrencia", "new_diagnostico_ocorrenciaid", "new_diagnostico_paiid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.Equal, id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }
        
        public decimal PrecoParaGerarPedidoAstec(Diagnostico diagnostico, out List<string> mensagemErro)
        {
            decimal preco = 0, ipi = 0, icms = 0;
            BuscarPrecoItemAstec_ttErrosRow[] erros = null;
            mensagemErro = new List<string>();

            Domain.Servicos.HelperWS.IntelbrasService.BuscarPrecoItemAstec(diagnostico.Ocorrencia.Produto.LinhaComercial.Estabelecimento.Codigo.ToString(), int.Parse(diagnostico.Ocorrencia.Autorizada.CodigoMatriz), diagnostico.Produto.Codigo, "LAI02", out preco, out ipi, out icms, out erros);

            if (erros != null)
                foreach (var erro in erros)
                    mensagemErro.Add(string.Format("Estabelecimento: {0} \nCliente: {1} \nProduto: {2} \nErro :{3} \n", diagnostico.Ocorrencia.Produto.LinhaComercial.Estabelecimento.Codigo.ToString(), diagnostico.Ocorrencia.Autorizada.CodigoMatriz, diagnostico.ExportaERP, erro.mensagem));

            if (preco <= 0) mensagemErro.Add("Preço de item precisa ser maior que ZERO. Tabela de preço LAI02 \n");

            return preco;
        }

        public List<T> ListarAtualizadosHoje()
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.Conditions.Add(new ConditionExpression("modifiedon", ConditionOperator.GreaterEqual, DateTime.Now));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public T ObterDiagnosticoSemNotaFical(Ocorrencia ocorencia, int codigoProduto)
        {
            var query = GetQueryExpression<T>(true);
            query.TopCount = 1;
            query.AddLink("product", "new_produtoid", "productid");
            query.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("productnumber", ConditionOperator.Equal, codigoProduto));
            query.Criteria.Conditions.Add(new ConditionExpression("new_nota_fiscal", ConditionOperator.Null));
            query.Criteria.Conditions.Add(new ConditionExpression("new_serie_nota_fiscal", ConditionOperator.Null));
            query.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorencia.Id));
            var colecao = this.RetrieveMultiple(query);
            if (colecao.List.Count == 0)
                return default(T);
            return colecao.List[0];
        }

        public bool DiagnosticoTemNotaFiscal(Guid id)
        {
            var queryHelper = GetQueryExpression<T>(true);
            queryHelper.ColumnSet.AddColumn("new_diagnostico_ocorrenciaid");

            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_nota_fiscal", ConditionOperator.NotNull));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_serie_nota_fiscal", ConditionOperator.NotNull));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.Equal, id));

            EntityCollection colecao = base.Provider.RetrieveMultiple(queryHelper);

            return (colecao.Entities.Count > 0);
        }

        public int QuantidadeProdutosFaturados(Guid id)
        {
            int quantidade = 0;

            QueryExpression query = new QueryExpression();
            query.EntityName = "new_diagnostico_ocorrencia";
            query.ColumnSet = new ColumnSet("new_qtd_faturada");

            ConditionExpression condition1 = new ConditionExpression();
            condition1.AttributeName = "new_diagnostico_ocorrenciaid";
            condition1.Operator = ConditionOperator.Equal;
            condition1.Values.Add(id);

            ConditionExpression condition2 = new ConditionExpression();
            condition2.AttributeName = "new_diagnostico_paiid";
            condition2.Operator = ConditionOperator.Equal;
            condition2.Values.Add(id);

            ConditionExpression condition3 = new ConditionExpression();
            condition3.AttributeName = "statecode";
            condition3.Operator = ConditionOperator.Equal;
            condition3.Values.Add((int)StateCode.Ativo);

            FilterExpression childFilter = new FilterExpression();
            childFilter.FilterOperator = LogicalOperator.Or;
            childFilter.Conditions.Add(condition1);
            childFilter.Conditions.Add(condition2);

            FilterExpression topFilter = new FilterExpression();
            topFilter.FilterOperator = LogicalOperator.And;
            topFilter.Conditions.Add(condition3);
            topFilter.Filters.Add(childFilter);

            query.Criteria = topFilter;

            EntityCollection colecao = base.Provider.RetrieveMultiple(query);

            foreach (Entity servico in colecao.Entities)
                if (servico.Attributes.Contains("new_qtd_faturada")) quantidade += Convert.ToInt32(servico["new_qtd_faturada"]);

            return quantidade;
        }

        public List<T> ListarPorFilhoEPai(Guid id)
        {
            var query = GetQueryExpression<T>(true);
            query.Criteria.FilterOperator = LogicalOperator.Or;
            query.Criteria.Conditions.Add(new ConditionExpression("new_diagnostico_ocorrenciaid", ConditionOperator.Equal, id));
            query.Criteria.Conditions.Add(new ConditionExpression("new_diagnostico_paiid", ConditionOperator.Equal, id));
            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPorFilhoEPai(Guid id, int statuscode, Guid notEqualId)
        {
            QueryExpression query = new QueryExpression();
            query.EntityName = "new_diagnostico_ocorrencia";
            query.Distinct = true;
            query.Criteria = new FilterExpression();
            query.Criteria.FilterOperator = LogicalOperator.And;
            ConditionExpression condition1 = null;
            if (statuscode != int.MinValue)
            {
                condition1 = new ConditionExpression()
                {
                    AttributeName = "statuscode",
                    Operator = ConditionOperator.Equal,
                };
                condition1.Values.Add(statuscode);
            }
            ConditionExpression condition2 = new ConditionExpression();
            condition2.AttributeName = "new_diagnostico_ocorrenciaid";
            condition2.Operator = ConditionOperator.NotEqual;
            condition2.Values.Add(notEqualId);

            query.Criteria.Conditions.Add(condition1);
            query.Criteria.Conditions.Add(condition2);

            var filter1 = new FilterExpression();
            filter1.FilterOperator = LogicalOperator.Or;

            ConditionExpression condition3 = new ConditionExpression();
            condition3.AttributeName = "new_diagnostico_ocorrenciaid";
            condition3.Operator = ConditionOperator.Equal;
            condition3.Values.Add(id);

            ConditionExpression condition4 = new ConditionExpression();
            condition4.AttributeName = "new_diagnostico_paiid";
            condition4.Operator = ConditionOperator.Equal;
            condition4.Values.Add(id);

            filter1.Conditions.Add(condition3);
            filter1.Conditions.Add(condition4);

            query.Criteria.Filters.Add(filter1);

            return (List<T>)this.RetrieveMultiple(query).List;
        }

        public List<T> ListarPor(string numeroNotaFiscal, string numeroSerieNotaFiscal, string codigoEstabelecimento)
        {
            List<Diagnostico> lista = new List<Diagnostico>();

            QueryExpression queryHelper = GetQueryExpression<T>(true);

            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_nota_fiscal", ConditionOperator.Equal, numeroNotaFiscal));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("new_serie_nota_fiscal", ConditionOperator.Equal, numeroSerieNotaFiscal));
            queryHelper.Criteria.Conditions.Add(new ConditionExpression("statecode", ConditionOperator.Equal, (int)StateCode.Ativo));
            queryHelper.AddLink("itbc_estabelecimento", "itbc_estabelecimento", "itbc_estabelecimentoid");

            queryHelper.LinkEntities[0].LinkCriteria.Conditions.Add(new ConditionExpression("itbc_codigo_estabelecimento", ConditionOperator.Equal, codigoEstabelecimento));
            return (List<T>)this.RetrieveMultiple(queryHelper).List;
        }

        public List<int> ObterOsStatusDeDiagnoticoPorOcorrencia(Guid ocorrenciaid)
        {
            var query = GetQueryExpression<T>(true);
            query.ColumnSet.AddColumns("statuscode");

            query.Criteria.Conditions.Add(new ConditionExpression("new_ocorrenciaid", ConditionOperator.Equal, ocorrenciaid));
            query.Criteria.Conditions.Add(new ConditionExpression("statuscode", ConditionOperator.NotIn, new object[] { 2, 7 })); // cancelado, substituido

            query.Orders.Add(new OrderExpression("statuscode", OrderType.Ascending));

            var colecao = base.Provider.RetrieveMultiple(query);

            List<int> status = new List<int>();
            foreach (Entity item in colecao.Entities)
            {
                status.Add(((OptionSetValue)item.Attributes["statuscode"]).Value);
            }

            return status;
        }
    }
}
