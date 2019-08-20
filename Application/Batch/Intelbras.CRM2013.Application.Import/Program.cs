using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.Application.Import
{
    class Program
    {

        private static OrganizationServiceProxy ServiceCRM
        {
            get
            {
                //Authenticate using credentials of the logged in user;
                string UserName = "crm2015admin";
                string Password = "mAHhPZPWUYPPtq5z8MMWvvFr";
                string Dominio = "INTELBRAS";
                UserName = Dominio + @"\" + UserName;
                ClientCredentials Credentials = new ClientCredentials();
                Credentials.UserName.UserName = UserName;
                Credentials.UserName.Password = Password;
                //Uri OrganizationUri = new Uri("https://crm2015.intelbras.com.br/IntelbrasCRM/XRMServices/2011/Organization.svc");
                Uri OrganizationUri = new Uri("https://internalcrm2015.intelbras.com.br/crm2015/XRMServices/2011/Organization.svc");
                OrganizationServiceProxy _Service = new OrganizationServiceProxy(OrganizationUri, null, Credentials, null);
                IOrganizationService crmConnection = (IOrganizationService)_Service;
                _Service.Timeout = new TimeSpan(0, 20, 0);
                return _Service;
            }
        }

        private static SqlConnection _erpConection;
        protected static SqlConnection ErpConection
        {
            get
            {
                if (_erpConection == null)
                {
                    _erpConection = new SqlConnection();
                    //_erpConection.ConnectionString = "Server=sjo-dbms-09\\crm2011;Database=Intelbras_MSCRM;User Id=connector;Password=connector;";
                    _erpConection.ConnectionString = "Server=SQLCRM\\SQLCRM;Database=Intelbras_MSCRM;User Id=connector;Password=connector;";
                }
                return _erpConection;
            }
        }

        protected static void GravaLog(string log)
        {
            using (StreamWriter w = File.AppendText(@"c:\temp\logimport.txt"))
                w.WriteLine(log);
        }

        static int Main(string[] args)
        {

            var systemUsers = new Dictionary<string, string>();
            string qbs = "SELECT (SELECT top 1 BUDe.SystemUserId FROM [SystemuserBase] BUDe WHERE LOWER(BUDe.DomainName) = LOWER(BUPara.DomainName)) AS DE, BUPara.SystemUserId AS PARA, BUPara.DomainName AS DomainName FROM[SQLCRM2015\\SQLCRM2015].[CRM2015_MSCRM].[dbo].[SystemUserBase] BUPara";
            if (ErpConection.State == System.Data.ConnectionState.Closed)
                ErpConection.Open();
            SqlCommand command1 = new SqlCommand(qbs, ErpConection);
            SqlDataReader reader1 = command1.ExecuteReader();

            while (reader1.Read())
            {
                if(reader1["DE"].ToString() != "")
                {
                    systemUsers[reader1["DE"].ToString()] = reader1["PARA"].ToString();
                }
            }
            reader1.Close();

            var businessUnits = new Dictionary<string, string>();
            string qbb = "SELECT (SELECT top 1 BUDe.BusinessUnitId FROM [BusinessUnitBase] BUDe WHERE BUDe.Name = BUPara.Name) AS DE, BUPara.BusinessUnitId AS PARA, BUPara.Name AS Name FROM [SQLCRM2015\\SQLCRM2015].[CRM2015_MSCRM].[dbo].[BusinessUnitBase] BUPara";
            if (ErpConection.State == System.Data.ConnectionState.Closed)
                ErpConection.Open();
            SqlCommand command2 = new SqlCommand(qbb, ErpConection);
            SqlDataReader reader2 = command2.ExecuteReader();

            while (reader2.Read())
            {
                if (reader2["DE"].ToString() != "")
                {
                    businessUnits[reader2["DE"].ToString()] = reader2["PARA"].ToString();
                }
            }
            reader2.Close();

            var products = new Dictionary<string, string>();
            string qbp = "SELECT (SELECT top 1 BUDe.ProductId FROM [ProductBase] BUDe WHERE BUDe.ProductNumber = BUPara.ProductNumber) AS DE, BUPara.ProductId AS PARA, BUPara.ProductNumber AS ProductNumber FROM[SQLCRM2015\\SQLCRM2015].[CRM2015_MSCRM].[dbo].[ProductBase] BUPara";
            if (ErpConection.State == System.Data.ConnectionState.Closed)
                ErpConection.Open();
            SqlCommand command3 = new SqlCommand(qbp, ErpConection);
            SqlDataReader reader3 = command3.ExecuteReader();

            while (reader3.Read())
            {
                if (reader3["DE"].ToString() != "")
                {
                    products[reader3["DE"].ToString()] = reader3["PARA"].ToString();
                }
            }
            reader3.Close();
            var moedas = new Dictionary<string, string>();
            moedas.Add("1B81AD2E-D0BC-DF11-8266-00155DA09700", "038B6824-09BF-E611-80BD-0050568DA30E");
            moedas.Add("AC66F5A2-87B7-DF11-91C9-00155DA09700", "24B5CE59-90C5-E311-93FD-00155D013E56");

            var unidades = new Dictionary<string, string>();
            string qbu = "SELECT (SELECT top 1 BUDe.UoMId FROM [UoMBase] BUDe WHERE BUDe.Name = BUPara.Name) AS DE, BUPara.UoMId AS PARA, BUPara.Name AS Name FROM[SQLCRM2015\\SQLCRM2015].[CRM2015_MSCRM].[dbo].[UoMBase] BUPara";
            if (ErpConection.State == System.Data.ConnectionState.Closed)
                ErpConection.Open();
            SqlCommand command4 = new SqlCommand(qbu, ErpConection);
            SqlDataReader reader4 = command4.ExecuteReader();

            while (reader4.Read())
            {
                if (reader4["DE"].ToString() != "")
                {
                    unidades[reader4["DE"].ToString()] = reader4["PARA"].ToString();
                }
            }
            reader4.Close();


            try
            {
                string[] entidades = {
//"Subject", IMPORTADO
//"New_acao", IMPORTADO
//"New_pais", IMPORTADO
//"New_autorizcao_postagem_eletronica_correios", IMPORTADO
//"New_canal_venda", IMPORTADO
//"New_categoria", IMPORTADO
//"New_causa", IMPORTADO
//"New_uf", IMPORTADO
//"Territory", IMPORTADO
//"New_regional", IMPORTADO
//"New_cidade", IMPORTADO
//"New_classificacao_assunto", IMPORTADO
//"New_cliente_Linha", IMPORTADO
//"New_cupomdesconto", IMPORTADO
//"New_tipo_cobranca", IMPORTADO
//"New_historico_postagem", IMPORTADO
//"New_promocao_pontuacao_fidelidade", IMPORTADO
//"New_tipo_posto_linha", IMPORTADO
//"New_problema", IMPORTADO
//"New_criterio_sla_assistencia_tecnica", IMPORTADO
//"New_desconto_pedido_programado", IMPORTADO
//"New_pedido_programado", IMPORTADO
//"New_item_pedido_programado", IMPORTADO
//"New_titulo", NÃO IMPORTADO
//"New_valor_servico_posto", IMPORTADO
//"New_condicao_pagamento", IMPORTADO
//"New_config_sistema", IMPORTADO
//"New_localidade", IMPORTADO
//"New_estabelecimento", IMPORTADO
//"New_defeito", IMPORTADO
//"New_dia", IMPORTADO
//"New_familia_material", IMPORTADO
//"New_tipo_motivo_auditoria", IMPORTADO
//"New_tipo_posto", IMPORTADO
//"New_natureza_operacao", IMPORTADO
//"New_rota", IMPORTADO
//"New_representante", IMPORTADO
//"New_grupo_premio_fidelidade", IMPORTADO
//"New_grupo_estoque", IMPORTADO
//"New_feriado_municipal_estadual", IMPORTADO
//"New_grupo_cliente", IMPORTADO
//"New_unidade_familia", IMPORTADO
//"New_linha_unidade_negocio", IMPORTADO
//"New_familiacomercial", IMPORTADO
//"New_Importacao_Assistencia_tecnica", IMPORTADO
//"New_receita_padrao", IMPORTADO
//"New_mensagem", IMPORTADO
//"New_transportadora", IMPORTADO
//"New_unxcategoria", IMPORTADO
//"New_segmento", IMPORTADO
//"New_familia", IMPORTADO
//"New_subfamilia", IMPORTADO
//"New_origem", IMPORTADO
//"New_tabela_financiamento", IMPORTADO
//"New_indice", IMPORTADO
//"New_tabela_preco", IMPORTADO
//"New_prioridade_ligacao_callcenter", IMPORTADO
//"New_processamentofidelidade", IMPORTADO
//"New_portador", IMPORTADO
//"New_conformidade", IMPORTADO

//"Account", IMPORTADO
//"Contact",
//"CustomerAddress",
//"Contract",

//"Opportunity",
//"New_auditoria_ocorrencia",

//Não importar os dados de Treinamento
//"New_avaliacao_pergunta", NÃO IMPORTAR
//"New_avaliacao_treinamento", NÃO IMPORTAR
//"New_cadastro_custo", NÃO IMPORTAR
//"New_calendario_custos", NÃO IMPORTAR
//"New_calendario_participante", NÃO IMPORTAR
//"New_calendario_treinamento", NÃO IMPORTAR
//"New_publicoalvo", NÃO IMPORTAR
//"New_lista_presenca", NÃO IMPORTAR
//"New_meta_treinamento_instrutor", NÃO IMPORTAR
//"New_perguntas", NÃO IMPORTAR
//"New_relacionamento_treinamento_avaliacao", NÃO IMPORTAR
//"New_respostas", NÃO IMPORTAR
//"New_respostas_avaliacao", NÃO IMPORTAR
//"New_respostas_participante", NÃO IMPORTAR
//"New_treinamento", NÃO IMPORTAR
//"New_treinamento_assunto", NÃO IMPORTAR

//"New_cliente_participante_contrato",
//"New_cliente_participante_endereco",
//"New_comunicado_advertencia",
//"New_defeito_ocorrencia_cliente",
//"New_diagnostico_ocorrencia",
//"New_duplicata", NÃO IMPORTAR
//"New_estrutura_produto",
//"New_extrato_fidelidade",
//"New_extrato_logistica_reversa",
//"New_extrato_pagamento_ocorrencia",
//"New_intervencao_tecnica",
//"New_item_extrato_logistica_reversa",
//"New_item_tabela", IMPORTANDO (JOSE)
//"New_lancamento_avulso",
//"New_linha_posto_servico", //IMPORTANDO (JOSE)
//"New_nota_ocorrncia",
//"New_pagamento_servico",
//"New_permissao_usuario_b2b",
//"New_permissao_usuario_servico",
//"New_pontuacao",
//"New_pre_requisito",
//"New_premio_fidelidade",
//"New_produto_assistecia_tecica", //IMPORTANDO (JOSE)
//"New_produto_contrato",
//"New_produto_resgatado_fidelidade",
//"New_registro_materiais",
"New_relacionamento", //IMPORTANDO (JOSE)
//"New_resgate_premio_fidelidade",
//"New_servico_assistencia_tecnica",
//"New_sla",
//"New_valor_servico",

//"ActivityMimeAttachment",
//"Annotation",
//"Email",

//"ActivityParty",
//"ActivityPointer", NÃO IMPORTAR
//"AnnualFiscalCalendar", NÃO IMPORTAR
//"ApplicationFile", NÃO IMPORTAR
//"AsyncOperation", NÃO IMPORTAR
//"BusinessTask", IMPORTADO
//"BusinessUnitNewsArticle",
//"Calendar", IMPORTADO
//"CalendarRule" IMPORTADO
//"Campaign" IMPORTADO
//"CampaignActivity", IMPORTADO
//"Commitment",
//"Appointment",
//"Competitor", IMPORTADO
//"ConstraintBasedGroup", NÃO IMPORTAR
//"ContractDetail",
//"ContractTemplate", IMPORTADO
//"CustomerOpportunityRole",
//"CustomerRelationship",
//"Discount", IMPORTADO
//"DiscountType", IMPORTADO
//"DisplayString", NÃO IMPORTAR
//"DocumentIndex",
//"DuplicateRecord", NÃO IMPORTAR
//"DuplicateRule", NÃO IMPORTAR
//"DuplicateRuleCondition", NÃO IMPORTAR
//"EmailHash", NÃO IMPORTAR
//"Equipment", NÃO IMPORTAR
//"Fax", NÃO IMPORTAR
//"FixedMonthlyFiscalCalendar", NÃO IMPORTAR
//"Incident",
//"IncidentResolution",
//"KbArticleTemplate", IMPORTADO
//"KbArticle",
//"KbArticleComment",
//"Lead",
//"LeadAddress",
//"List", IMPORTADO
//"MailMergeTemplate", IMPORTADO
//"MonthlyFiscalCalendar", NÃO IMPORTAR
//"OpportunityClose",
//"OpportunityProduct",
//"OrderClose",
//"Organization", NÃO IMPORTAR
//"OrganizationStatistic", NÃO IMPORTAR
//"OwnerMapping", IMPORTADO

//Entidades com problemas no CRM 4
//"PhoneCallBase", DEIXAR PARA O FINAL PQ TEM O RegardingObjectId que pode vior de várias entidades
//"LetterBase", DEIXAR PARA O FINAL PQ 
//"ListMemberBase", EntityId não sei de onde pegar
//"CampaignResponseBase" - tem que rodar o Base pra pegar o ObjectRegardingTypeCode

//"PickListMapping", NÃO IMPORTAR
//"Product", IMPORTANDO (JOSE)
//"PriceLevel", IMPORTADO
//"ProductPriceLevel", --IMPORTAR DEPOIS POIS SAO 125 mil itens
//"QuarterlyFiscalCalendar", NÃO IMPORTAR
//"Queue", IMPORTADO
//"QueueItem",// NÃO IMPORTAR campos não encontrados no CRM 4
//"Quote", NÃO IMPORTAR
//"QuoteClose", NÃO IMPORTAR
//"QuoteDetail", NÃO IMPORTAR
//"RelationshipRole", NÃO IMPORTAR
//"Report", NÃO IMPORTAR
//"ReportCategory", NÃO IMPORTAR
//"ReportEntity", NÃO IMPORTAR
//"ReportLink", NÃO IMPORTAR
//"ReportVisibility", NÃO IMPORTAR
//"Resource", NÃO IMPORTAR
//"ResourceSpec", NÃO IMPORTAR
//"Role", NÃO IMPORTAR
//"SemiAnnualFiscalCalendar", NÃO IMPORTAR
//"Service", IMPORTADO
//"ServiceAppointment", IMPORTADO
//"Site", IMPORTADO
//"Task", IMPORTADO
//"Team", NÃO IMPORTAR
//"Template", NÃO IMPORTAR
//"TimeZoneDefinition", NÃO IMPORTAR
//"TimeZoneLocalizedName", NÃO IMPORTAR
//"TimeZoneRule", NÃO IMPORTAR
//"TransactionCurrency", NÃO IMPORTAR
//"TransformationMapping", NÃO IMPORTAR
//"TransformationParameterMapping", NÃO IMPORTAR
//"UserFiscalCalendar", NÃO IMPORTAR
//"UserSettings", NÃO IMPORTAR
//"WebWizard", NÃO IMPORTAR
//"WizardAccessPrivilege", NÃO IMPORTAR
//"WizardPage", NÃO IMPORTAR
//"Workflow", NÃO IMPORTAR
//"WorkflowDependency", NÃO IMPORTAR
//"WorkflowLog" NÃO IMPORTAR
//"SavedQuery", NÃO IMPORTAR
//"UserQuery", NÃO IMPORTAR
//"Invoice", NÃO IMPORTAR
//"InvoiceDetail", NÃO IMPORTAR
//"SalesLiterature", NÃO IMPORTAR
//"SalesLiteratureItem", NÃO IMPORTAR
//"SalesOrder", NÃO IMPORTAR
//"SalesOrderDetail", NÃO IMPORTAR
//"ClientUpdate", NÃO IMPORTAR
//"Import", NÃO IMPORTAR
//"ImportData", NÃO IMPORTAR
//"ImportFile", NÃO IMPORTAR
//"ImportJob", NÃO IMPORTAR
//"ImportLog", NÃO IMPORTAR
//"ImportMap", NÃO IMPORTAR
//"LookUpMapping", NÃO IMPORTAR
//"AttributeMap", NÃO IMPORTAR
//"BulkDeleteFailure", NÃO IMPORTAR
//"BulkDeleteOperation", NÃO IMPORTAR
//"BulkOperation", NÃO IMPORTAR
//"BulkOperationLog", NÃO IMPORTAR
//"IsvConfig", NÃO IMPORTAR
//"SystemUser", NÃO IMPORTAR
//"UoM", NÃO IMPORTAR
//"UoMSchedule", NÃO IMPORTAR
//"BusinessUnit", NÃO IMPORTAR
//"codek_answer", NÃO IMPORTAR
//"codek_answer_list", NÃO IMPORTAR
//"codek_answer_type", NÃO IMPORTAR
//"codek_EventField", NÃO IMPORTAR

//"Codek_livechat_client_style", IMPORTADO
//"Codek_livechat_config", IMPORTADO
//"Codek_livechat_default_answers", IMPORTADO
//"Codek_livechat_event_log", IMPORTADO
//"CodeK_livechat_form_visitor_config", IMPORTADO
//"Codek_livechat_interval_reason", IMPORTADO
//"Codek_livechat_operator", IMPORTADO
//"Codek_livechat_operator_log", IMPORTADO
//"CodeK_livechat_search_visitor_config", IMPORTADO
//"Codek_livechat_subject", IMPORTADO
//"CodeK_livechat_subject_operator", IMPORTADO
//"CodeK_livechat_survey", IMPORTADO
//"CodeK_livechat_survey_answer", IMPORTADO
//"CodeK_livechat_survey_question", IMPORTADO
//"Codek_livechat_tracking", IMPORTADO
//"Codek_livechat_tracking_detail", MUDOU ESTRUTURA, NÃO TEM COMO IMPORTAR

//"codek_option_answer", NÃO IMPORTAR
//"codek_question", NÃO IMPORTAR
//"codek_search", NÃO IMPORTAR
//"codek_search_message", NÃO IMPORTAR
//"CodeK_sequential_number", NÃO IMPORTAR
//"codek_workgroup", NÃO IMPORTAR
//"ColumnMapping", NÃO IMPORTAR
//"EntityMap", NÃO IMPORTAR
//"New_log_diagnostico", NÃO IMPORTAR
//"New_log_email", NÃO IMPORTAR
//"New_log_ocorrencia", NÃO IMPORTAR
//"New_registro_log", NÃO IMPORTAR

//"New_capturadehyperlinks", NÃO IMPORTAR
//"New_categoriadohyperlink", NÃO IMPORTAR
//"New_dicionriodehyperlinks", NÃO IMPORTAR

//"PluginAssembly", NÃO IMPORTAR
//"PluginType", NÃO IMPORTAR
//"SdkMessage", NÃO IMPORTAR
//"SdkMessagePair", NÃO IMPORTAR
//"SdkMessageProcessingStep", NÃO IMPORTAR
//"SdkMessageProcessingStepImage", NÃO IMPORTAR
//"SdkMessageRequestField", NÃO IMPORTAR
//"SdkMessageRequestInput", NÃO IMPORTAR
//"SdkMessageResponse", NÃO IMPORTAR

 };

                foreach (string entidade in entidades)
                {
                    Console.WriteLine(entidade + " Inicio " + DateTime.Now.ToString());
                    //faz um select de mil em mil registros
                    for (Int64 x = 1000; x < 9999999999; x=x+1000)
                    {
                        string query = "SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY CreatedOn desc) AS RowNum, * FROM " + entidade + " (nolock) "+
                            "WHERE "+ entidade + "id not in (Select TT." + entidade + "id from [SQLCRM2015\\SQLCRM2015].[CRM2015_MSCRM].[dbo]." + entidade + " TT) )" +
                            "AS RowConstrainedResult WHERE RowNum >=" + (x-999).ToString() + " AND RowNum <= " + x.ToString() +
                            " ORDER BY CreatedOn desc";                        
                        
                        if (ErpConection.State == System.Data.ConnectionState.Closed)
                            ErpConection.Open();
                        SqlCommand cmd = new SqlCommand(query, ErpConection);
                        SqlDataReader registros = cmd.ExecuteReader();
                        if (registros.HasRows)
                        {
                            List<RetrieveAttributeResponse> ColunasDaEntidade = new List<RetrieveAttributeResponse>();
                            for (int i = 1; i < registros.FieldCount; i++) //pode começar o i com 1 para tirar a primira coluna que é RowNum
                            {
                                string coluna = registros.GetName(i).ToLower();
                                if(coluna.Contains("customer"))
                                {

                                }
                                if (coluna == "versionnumber" || coluna == "deletionstatecode") continue;                                
                                try
                                {
                                    RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
                                    {
                                        EntityLogicalName = entidade.ToLower(),
                                        LogicalName = coluna,
                                        RetrieveAsIfPublished = true
                                    };
                                    ColunasDaEntidade.Add((RetrieveAttributeResponse)ServiceCRM.Execute(attributeRequest));
                                }
                                catch (Exception ex)
                                {
                                    //pode não achar a coluna conforme o select
                                }
                            }
                            while (registros.Read())
                            {
                                Entity registroCRM = new Entity(entidade.ToLower());
                                //for (int i = 1; i < registros.FieldCount; i++) //pode começar o i com 1 para tirar a primira coluna que é RowNum
                                foreach (var DadosDaColuna in ColunasDaEntidade)
                                {
                                    string coluna = DadosDaColuna.AttributeMetadata.LogicalName.ToLower();
                                    /*
                                    -BigInt A big integer attribute. Value = 0x12.
                                    -Boolean	A Boolean attribute. Value = 0.
                                    CalendarRules An attribute that contains calendar rules. Value = 0x10.
                                    Customer An attribute that represents a customer. Value = 1.
                                    -DateTime A date/time attribute. Value = 2.
                                    -Decimal A decimal attribute. Value = 3.
                                    -Double A double attribute. Value = 4.
                                    EntityName An entity name attribute. Value = 20.
                                    -Integer An integer attribute. Value = 5.
                                    -Lookup A lookup attribute. Value = 6.
                                    ManagedProperty A managed property attribute. Value = 0x13.
                                    -Memo A memo attribute. Value = 7.
                                    -Money A money attribute. Value = 8.
                                    -Owner An owner attribute. Value = 9.
                                    -PartyList A partylist attribute. Value = 10.
                                    -Picklist A picklist attribute. Value = 11.
                                    -State A state attribute. Value = 12.
                                    -Status A status attribute. Value = 13.
                                    -String A string attribute. Value = 14.
                                    -Uniqueidentifier An attribute that is an ID. Value = 15.
                                    Virtual An attribute that is created by the system at run time. Value = 0x11.
                                    */
                                    if (coluna == "parentcustomeridtype" || coluna == "customeridtype" || coluna == "billingcustomeridtype")
                                    {
                                        registroCRM.Attributes.Add(coluna, registros[coluna]);                                        
                                        continue;
                                    }
                                    else if(coluna == "defaultuomscheduleid")
                                    {
                                        registroCRM.Attributes.Add(coluna, new EntityReference("uomschedule", new Guid("B24B8C73-D569-4C6D-BC13-7A4A0AEBA7CE")));
                                        continue;
                                    } else if (coluna == "uomid" || coluna == "defaultuomid")
                                    {
                                        var valor = registros[coluna].ToString();
                                        if (unidades.ContainsKey(valor))
                                        {
                                            registroCRM.Attributes.Add(coluna, new EntityReference("uom", new Guid(unidades[valor])));
                                            continue;
                                        }else
                                        {
                                            registroCRM.Attributes.Add(coluna, new EntityReference("uom", new Guid("861702E6-A3E9-E311-940A-00155D013D3B")));
                                            continue;
                                        }
                                    }
                                    else if (coluna == "transactioncurrencyid")
                                    {
                                        var valor = registros[coluna].ToString().ToUpper();
                                        if (moedas.ContainsKey(valor))
                                        {
                                            registroCRM.Attributes.Add(coluna, new EntityReference("transactioncurrency", new Guid(moedas[valor])));
                                            continue;
                                        }
                                    }
                                    else if (new string[] { "new_produtoid", "new_produto_substitutoid", "new_produtosid", "productid" }.Contains(coluna))
                                    {
                                        var valor = registros[coluna].ToString();
                                        if (products.ContainsKey(valor))
                                        {
                                            registroCRM.Attributes.Add(coluna, new EntityReference("product", new Guid(products[valor])));
                                            continue;
                                        }
                                    }
                                    else if (new string[] { "new_unidade_negocioid", "businessunitid", "owningbusinessunit", "new_unidadedenegociosid", "new_unidadeid", "new_unidade_negocioid", "new_unidadenegocioid", "new_unidadedenegcioid", "parentbusinessunitid", "new_unidade_negocio_astec" }.Contains(coluna))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                        {
                                            if (businessUnits.ContainsKey(registros[coluna].ToString()))
                                            {
                                                registroCRM.Attributes.Add(coluna, new EntityReference("businessunit", new Guid(businessUnits[registros[coluna].ToString()])));
                                                continue;
                                            }
                                        }                                            
                                    }
                                    else if (new string[] { "primaryuserid", "systemuserid", "createdby", "ownerid", "modifiedby", "preferredsystemuserid", "new_responsavel_icomp_id", "new_responsavel_icon_id", "new_responsavel_icorp_id", "new_responsavel_imax_id", "new_responsavel_inet_id", "new_responsavel_isec_id", "new_usuario_conclusao_ocorrencia", "new_recebido_porid" }.Contains(coluna))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                        {
                                            var guid = new Guid("289737F9-2BBE-E611-80BD-0050568DA30E");
                                            if (systemUsers.ContainsKey(registros[coluna].ToString()))
                                            {
                                                guid = new Guid(systemUsers[registros[coluna].ToString()]);
                                                if (guid == new Guid("829ff124-ecbe-e611-80bd-0050568da30e"))
                                                {
                                                    guid = new Guid("289737F9-2BBE-E611-80BD-0050568DA30E");
                                                }                                                
                                            }
                                            registroCRM.Attributes.Add(coluna, new EntityReference("systemuser", guid));
                                            continue;                                           
                                        }
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.BigInt, AttributeTypeCode.Integer}).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, (int)(registros[coluna]));
                                    }else if ((new List<AttributeTypeCode> { AttributeTypeCode.Picklist, AttributeTypeCode.State, AttributeTypeCode.Status }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, new OptionSetValue((int)(registros[coluna])));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Money }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, new Money(Convert.ToDecimal(registros[coluna])));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Decimal}).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, Convert.ToDecimal(registros[coluna]));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Double }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, Convert.ToDouble(registros[coluna]));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.DateTime }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, Convert.ToDateTime(registros[coluna]));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Boolean }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, Convert.ToBoolean(registros[coluna]));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Uniqueidentifier }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, new Guid(Convert.ToString(registros[coluna])));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Lookup }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, new EntityReference("", new Guid(Convert.ToString(registros[coluna]))));
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Customer }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                        {
                                            var type = registros[coluna + "type"].ToString() == "1" ? "account" : "contact";
                                            registroCRM.Attributes.Add(coluna, new EntityReference(type, new Guid(Convert.ToString(registros[coluna]))));
                                        }
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.Owner }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                        {
                                            /*var valor = registros[coluna].ToString();
                                            if(systemUsers.ContainsKey(valor))
                                                registroCRM.Attributes.Add(coluna, new EntityReference("systemuser", new Guid(systemUsers[valor])));
                                            else*/
                                                registroCRM.Attributes.Add(coluna, new EntityReference("systemuser", new Guid("289737F9-2BBE-E611-80BD-0050568DA30E")));
                                        }
                                    }
                                    else if ((new List<AttributeTypeCode> { AttributeTypeCode.PartyList }).Contains(DadosDaColuna.AttributeMetadata.AttributeType.Value))
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                        {
                                            var temp = (Lookup[])registros[coluna];
                                            var entities = new EntityCollection();
                                            entities.EntityName = "activityparty";
                                            for (int index = 0; index < temp.Length; index++)
                                            {
                                                var entityTmp = new Entity("activityparty");
                                                entityTmp.Attributes["partyid"] = new EntityReference(temp[index].Type, temp[index].Id);
                                                entities.Entities.Add(entityTmp);
                                            }
                                            registroCRM.Attributes.Add(coluna, entities);
                                        }
                                    }
                                    else
                                    {
                                        if (Convert.ToString(registros[coluna]) != string.Empty && Convert.ToString(registros[coluna]) != "NULL")
                                            registroCRM.Attributes.Add(coluna, registros[coluna]);
                                    }
                                    //Console.WriteLine(attributeResponse.AttributeMetadata.AttributeTypeName.Value);
                                    //Console.WriteLine(attributeResponse.AttributeMetadata.SchemaName);
                                    //registroCRM.Attributes.Add(coluna, registros[coluna]);
                                }
                                if (registroCRM.Attributes.Count > 0)
                                    SalvarRegistroCRM(registroCRM);
                            }
                            registros.Close();
                        }
                        else
                        {
                            break;
                        }
                    }

                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro : " + ex.Message);
                GravaLog(ex.ToString());
                return ex.GetHashCode();
            }
        }

        private static bool SalvarRegistroCRM(Entity registro)
        {
            IOrganizationService crmConnection = (IOrganizationService)ServiceCRM;
            try
            {
                try
                {
                    if (registro.LogicalName == "contact")
                    {
                        TrataCamposContato(registro);
                    }else if (registro.LogicalName == "product")
                    {
                        TrataCamposProduto(registro);
                    }
                    else
                    {
                        crmConnection.Create(registro);
                    }
                    Console.Write("+");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("is not a valid status code for state code "))
                    {
                        var stateCode = (OptionSetValue) registro.Attributes["statecode"];
                        var statusCode = (OptionSetValue)registro.Attributes["statuscode"];
                        
                        registro.Attributes.Remove("statuscode");
                        var id = crmConnection.Create(registro);

                        SetStateRequest request = new SetStateRequest
                        {
                            EntityMoniker = new EntityReference(registro.LogicalName, id),
                            State = stateCode,
                            Status = statusCode
                        };

                        crmConnection.Execute(request);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Cannot insert duplicate key."))
                    Console.Write("-");
                else
                {
                   Console.Write("!");
                    GravaLog(ex.ToString());
                }
            }
            return true;
        }        

        public static void TrataCamposContato(Entity registro)
        {
            IOrganizationService crmConnection = (IOrganizationService)ServiceCRM;

            var e = new Entity("contact");
            var existente = false;

            if (registro.Contains("new_cpf") || registro.Contains("emailaddress1"))
            {
                var q = new QueryExpression("contact");
                q.ColumnSet.AllColumns = true;
                var f = new FilterExpression(LogicalOperator.Or);
                if (registro.Contains("new_cpf"))
                {
                    f.AddCondition(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, registro.Attributes["new_cpf"].ToString()));
                    f.AddCondition(new ConditionExpression("itbc_cpfoucnpj", ConditionOperator.Equal, registro.Attributes["new_cpf_sem_mascara"].ToString()));
                }
                if (registro.Contains("emailaddress1"))
                    f.AddCondition(new ConditionExpression("emailaddress1", ConditionOperator.Equal, registro.Attributes["emailaddress1"].ToString()));
                q.Criteria.AddFilter(f);
                var r = crmConnection.RetrieveMultiple(q);
                if (r.Entities != null && r.Entities.Count > 0)
                {
                    existente = true;
                    e = r.Entities.First();
                }
            }

            if (registro.Attributes.Contains("new_mensagem"))
                registro.Attributes["new_mensagem"] = "";
            else
                registro.Attributes.Add("new_mensagem", "");

            foreach (var atributo in registro.Attributes)
            {
                if (!e.Contains(atributo.Key))
                {
                    e.Attributes.Add(atributo.Key, atributo.Value);
                }
                if (atributo.Key == "new_loja")
                {
                    e.Attributes.Add("itbc_loja_cnpj", ConsultaCnpjConta(((EntityReference)atributo.Value).Id.ToString()));
                    e.Attributes.Remove("new_loja");
                    //e.Attributes.Add("new_chave_integracao",ConsultaCnpjConta((((EntityReference)atributo.Value).Id.ToString())));
                }
                else if (atributo.Key == "parentcustomerid")
                {
                    e.Attributes["new_mensagem"] = ConsultaCnpjConta(((EntityReference)atributo.Value).Id.ToString()) + "|" + registro.Attributes["parentcustomeridtype"].ToString();
                    e.Attributes.Remove("parentcustomerid");                    
                    //e.Attributes.Add("", ConsultaCnpjConta((((EntityReference)atributo.Value).Id.ToString())));
                    //e.Attributes.Add("", e.Attributes["parentcustomeridtype"].ToString());
                }
                else if (atributo.Key == "new_area" && !e.Attributes.Contains("itbc_area"))
                {
                    var area = 993520000;
                    switch (((OptionSetValue)atributo.Value).Value)
                    {
                        case 1: area = 993520000; break;
                        case 2: area = 993520001; break;
                        case 3: area = 993520002; break;
                        case 4: area = 993520003; break;
                        case 5: area = 993520004; break;
                        case 6: area = 993520005; break;
                        case 7: area = 993520006; break;
                        case 8: area = 993520007; break;
                        case 9: area = 993520010; break;
                        case 10: area = 993520011; break;
                        case 11: area = 993520012; break;
                        case 12: area = 993520008; break;
                        case 13: area = 993520009; break;
                        case 14: area = 993520013; break;
                        case 15: area = 993520015; break;
                        case 99: area = 993520016; break;
                    }
                    e.Attributes.Add("itbc_area", new OptionSetValue(area));
                }
                else if (atributo.Key == "new_cargo" && !e.Attributes.Contains("itbc_cargo"))
                {
                    var cargo = 993520000;
                    switch (((OptionSetValue)atributo.Value).Value)
                    {
                        case 1: cargo = 993520000; break;
                        case 2: cargo = 993520001; break;
                        case 3: cargo = 993520002; break;
                        case 4: cargo = 993520003; break;
                        case 5: cargo = 993520004; break;
                        case 6: cargo = 993520005; break;
                        case 7: cargo = 993520006; break;
                        case 8: cargo = 993520007; break;
                        case 9: cargo = 993520008; break;
                        case 10: cargo = 993520009; break;
                        case 11: cargo = 993520010; break;
                        case 12: cargo = 993520011; break;
                        case 13: cargo = 993520012; break;
                        case 14: cargo = 993520014; break;
                        case 15: cargo = 993520015; break;
                        case 16: cargo = 993520016; break;
                        case 17: cargo = 993520017; break;
                        case 18: cargo = 993520018; break;
                        case 19: cargo = 993520019; break;
                        case 20: cargo = 993520020; break;
                        case 21: cargo = 993520021; break;
                        case 22: cargo = 993520023; break;
                        case 23: cargo = 993520013; break;
                        case 99: cargo = 993520022; break;
                    }
                    e.Attributes.Add("itbc_cargo", new OptionSetValue(cargo));
                }
                else if (atributo.Key == "new_cpf_sem_mascara" && !e.Attributes.Contains("itbc_cpfoucnpj"))
                    e.Attributes.Add("itbc_cpfoucnpj", (string)atributo.Value);
                else if (atributo.Key == "new_escolaridade" && !e.Attributes.Contains("itbc_escolaridade"))
                {
                    var escolaridade = 993520000;
                    switch (((OptionSetValue)atributo.Value).Value)
                    {
                        case 1: escolaridade = 993520000; break;
                        case 2: escolaridade = 993520001; break;
                        case 3: escolaridade = 993520002; break;
                        case 4: escolaridade = 993520003; break;
                        case 5: escolaridade = 993520004; break;
                        case 6: escolaridade = 993520005; break;
                        case 7: escolaridade = 993520006; break;
                        case 8: escolaridade = 993520007; break;
                        case 9: escolaridade = 993520008; break;
                        case 10: escolaridade = 993520009; break;
                    }
                    e.Attributes.Add("itbc_escolaridade", new OptionSetValue(escolaridade));
                }
                else if (atributo.Key == "new_numero_endereco_principal" && !e.Attributes.Contains("itbc_address1_number"))
                    e.Attributes.Add("itbc_address1_number", atributo.Value);
                else if (atributo.Key == "new_observacao" && !e.Attributes.Contains("description"))
                    e.Attributes.Add("description", atributo.Value);
                else if (atributo.Key == "new_ramal_comercial2" && !e.Attributes.Contains("itbc_ramal_telefone2"))
                    e.Attributes.Add("itbc_ramal_telefone2", atributo.Value);
                else if (atributo.Key == "new_ramal_fax" && !e.Attributes.Contains("itbc_ramal_fax"))
                    e.Attributes.Add("itbc_ramal_fax", atributo.Value);
                else if (atributo.Key == "new_ramal_telefone" && !e.Attributes.Contains("itbc_ramal_telefone1"))
                    e.Attributes.Add("itbc_ramal_telefone1", atributo.Value);
                else if (atributo.Key == "new_rg" && !e.Attributes.Contains("itbc_docidentidade"))
                    e.Attributes.Add("itbc_docidentidade", atributo.Value);
                else if (atributo.Key == "address1_line1" && !e.Attributes.Contains("itbc_address1_street"))
                    e.Attributes.Add("itbc_address1_street", atributo.Value);
                else if (atributo.Key == "address1_line2")
                {
                    if(!e.Attributes.Contains("address1_line3"))
                        e.Attributes.Add("address1_line3", atributo.Value);
                    else
                        e.Attributes["address1_line3"] = atributo.Value;
                }
                else if (atributo.Key == "address1_line3")
                {
                    if (!e.Attributes.Contains("address1_line2"))
                        e.Attributes.Add("address1_line2", atributo.Value);
                    else
                        e.Attributes["address1_line2"] = atributo.Value;
                }                
            }

            if(e.Attributes.Contains("new_cidadeidname") && e.Attributes.Contains("new_ufidname") && e.Attributes.Contains("new_paisidname"))
            {
                var query = new QueryExpression("itbc_pais");
                query.Criteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, e.Attributes["new_paisidname"].ToString()));
                var q1 = query.AddLink("itbc_estado", "itbc_paisid", "itbc_pais");
                q1.LinkCriteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, e.Attributes["new_ufidname"].ToString()));
                q1.Columns.AddColumn("itbc_estadoid");
                q1.EntityAlias = "a";
                var q2 = q1.AddLink("itbc_municipios", "itbc_estadoid", "itbc_estadoid");
                q2.LinkCriteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, e.Attributes["new_cidadeidname"].ToString()));
                q2.Columns.AddColumn("itbc_municipiosid");
                q2.EntityAlias = "b";

                var r = crmConnection.RetrieveMultiple(query);
                if(r.Entities != null && r.Entities.Count > 0)
                {
                    var o = r.Entities.First();
                    e.Attributes.Add("itbc_address1_city", new EntityReference("itbc_municipios", new Guid(((AliasedValue)o.Attributes["b.itbc_municipiosid"]).Value.ToString())));
                    e.Attributes.Add("itbc_address1_stateorprovince", new EntityReference("itbc_estado", new Guid(((AliasedValue)o.Attributes["a.itbc_estadoid"]).Value.ToString())));
                    e.Attributes.Add("itbc_address1_country", new EntityReference("itbc_pais", o.Id));
                }
            }

            if(!e.Attributes.Contains("customertypecode"))
                e.Attributes.Add("customertypecode", new OptionSetValue(993520006));

            if (existente)
            {
                crmConnection.Update(e);
            }else
            {
                crmConnection.Create(e);
            }
            
        }


        public static void TrataCamposProduto(Entity registro)
        {
            IOrganizationService crmConnection = (IOrganizationService)ServiceCRM;

            var q = new QueryExpression("product");
            q.Criteria.AddCondition(new ConditionExpression("productnumber", ConditionOperator.Equal, registro.Attributes["productnumber"].ToString()));
            var r = crmConnection.RetrieveMultiple(q);            

            if (r.Entities != null && r.Entities.Count > 0)
            {
                var ent = r.Entities.First();
                if(registro.Contains("new_call_center"))
                    ent.Attributes.Add("new_call_center", registro.Attributes["new_call_center"]);
                if (registro.Contains("new_data_intervencao_tecnica"))
                    ent.Attributes.Add("new_data_intervencao_tecnica", registro.Attributes["new_data_intervencao_tecnica"]);
                if (registro.Contains("new_descricao_intervencao_tecnica"))
                    ent.Attributes.Add("new_descricao_intervencao_tecnica", registro.Attributes["new_descricao_intervencao_tecnica"]);
                if (registro.Contains("new_intervencao_tecnica"))
                    ent.Attributes.Add("new_intervencao_tecnica", registro.Attributes["new_intervencao_tecnica"]);
                if (registro.Contains("new_logistica_reversa"))
                    ent.Attributes.Add("new_logistica_reversa", registro.Attributes["new_logistica_reversa"]);
                if (registro.Contains("new_motivo_intervencao_tecnica"))
                    ent.Attributes.Add("new_motivo_intervencao_tecnica", registro.Attributes["new_motivo_intervencao_tecnica"]);
                if (registro.Contains("new_participado_fidelidade"))
                    ent.Attributes.Add("new_participado_fidelidade", registro.Attributes["new_participado_fidelidade"]);
                if (registro.Contains("new_pontos_fidelidades"))
                    ent.Attributes.Add("new_pontos_fidelidades", registro.Attributes["new_pontos_fidelidades"]);
                if (registro.Contains("new_valor_mao_de_obra"))
                    ent.Attributes.Add("new_valor_mao_de_obra", registro.Attributes["new_valor_mao_de_obra"]);

                crmConnection.Update(ent);
            }else
            {
                if (registro.Contains("new_complemento"))
                {
                    registro.Attributes.Add("itbc_complemento", registro.Attributes["new_complemento"]);
                }
                if (registro.Contains("new_familiacomercialid"))
                {
                    var query = new QueryExpression("new_familiacomercial");
                    query.Criteria.AddCondition(new ConditionExpression("new_familiacomercialid", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_familiacomercialid"]).Id.ToString()));
                    var q1 = query.AddLink("itbc_familiacomercial", "new_codigo_familia_comercial", "itbc_codigo_familia_comercial");
                    q1.Columns.AddColumn("itbc_familiacomercialid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.itbc_familiacomercialid"]).Value.ToString();
                        registro.Attributes.Add("itbc_familiacomercial", new EntityReference("itbc_familiacomercial", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_familiaid"))
                {
                    var query = new QueryExpression("new_familia");
                    query.Criteria.AddCondition(new ConditionExpression("new_familia", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_familiaid"]).Id.ToString()));
                    var q1 = query.AddLink("itbc_familDeprod", "new_codigo", "itbc_codigo_familia");
                    q1.Columns.AddColumn("itbc_familDeprodid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.itbc_familDeprodid"]).Value.ToString();
                        registro.Attributes.Add("itbc_famildeprod", new EntityReference("itbc_familDeprod", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_familia_materialid"))
                {
                    var query = new QueryExpression("new_familia_material");
                    query.Criteria.AddCondition(new ConditionExpression("new_familia_material", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_familia_materialid"]).Id.ToString()));
                    var q1 = query.AddLink("itbc_familia_material", "new_codigo_familia_material", "itbc_codigo_familia_material");
                    q1.Columns.AddColumn("itbc_familia_materialid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.itbc_familia_materialid"]).Value.ToString();
                        registro.Attributes.Add("itbc_familia_material", new EntityReference("itbc_familia_material", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_grupo_estoqueid"))
                {
                    var query = new QueryExpression("new_grupo_estoque");
                    query.Criteria.AddCondition(new ConditionExpression("new_grupo_estoqueid", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_grupo_estoqueid"]).Id.ToString()));
                    var q1 = query.AddLink("itbc_grupodeestoque", "new_codigo_grupo_estoque", "itbc_codigo_grupo_estoque");
                    q1.Columns.AddColumn("itbc_grupodeestoqueid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.itbc_grupodeestoqueid"]).Value.ToString();
                        registro.Attributes.Add("itbc_grupodeestoque", new EntityReference("itbc_grupodeestoque", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_origemid"))
                {
                    var query = new QueryExpression("new_origem");
                    query.Criteria.AddCondition(new ConditionExpression("new_origemid", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_origemid"]).Id.ToString()));
                    var q1 = query.AddLink("itbc_origem", "new_codigo", "itbc_codigo_origem");
                    q1.Columns.AddColumn("itbc_origemid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.itbc_origemid"]).Value.ToString();
                        registro.Attributes.Add("itbc_origem", new EntityReference("itbc_origem", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_segmentoid"))
                {
                    var query = new QueryExpression("new_segmento");
                    query.Criteria.AddCondition(new ConditionExpression("new_segmentoid", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_segmentoid"]).Id.ToString()));
                    var q1 = query.AddLink("itbc_segmento", "new_codigo", "itbc_codigo_segmento");
                    q1.Columns.AddColumn("itbc_segmentoid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.itbc_segmentoid"]).Value.ToString();
                        registro.Attributes.Add("itbc_segmento", new EntityReference("itbc_segmento", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_subfamiliaid"))
                {
                    var query = new QueryExpression("new_subfamilia");
                    query.Criteria.AddCondition(new ConditionExpression("new_subfamiliaid", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_subfamiliaid"]).Id.ToString()));
                    var q1 = query.AddLink("itbc_subfamiliadeproduto", "new_codigo", "itbc_codigo_subfamilia");
                    q1.Columns.AddColumn("itbc_subfamiliadeprodutoid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.itbc_subfamiliadeprodutoid"]).Value.ToString();
                        registro.Attributes.Add("itbc_subfamiliadeproduto", new EntityReference("itbc_subfamiliadeproduto", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_unidadefamiliaid"))
                {
                    var query = new QueryExpression("new_unidadefamilia");
                    query.Criteria.AddCondition(new ConditionExpression("new_unidadefamiliaid", ConditionOperator.Equal, ((EntityReference)registro.Attributes["new_unidadefamiliaid"]).Id.ToString()));
                    var q1 = query.AddLink("businessunit", "new_name", "name");
                    q1.Columns.AddColumn("businessunitid");
                    q1.EntityAlias = "a";
                    var res = crmConnection.RetrieveMultiple(query);

                    if (res.Entities != null && res.Entities.Count > 0)
                    {
                        var famid = ((AliasedValue)res.Entities.First().Attributes["a.businessunitid"]).Value.ToString();
                        registro.Attributes.Add("itbc_businessunitid", new EntityReference("businessunit", new Guid(famid)));
                    }
                }
                if (registro.Contains("new_tipo_produto"))
                {
                    var valor = ((OptionSetValue)registro.Attributes["new_tipo_produto"]).Value;
                    registro.Attributes.Add("itbc_tipodeproduto", valor == 0 ? false : true);
                }
                if (registro.Contains("new_politica_reparo"))
                {
                    int p = 993520000;
                    switch (((OptionSetValue)registro.Attributes["new_politica_reparo"]).Value)
                    {
                        case 1: p = 993520005; break;
                        case 2: p = 993520006; break;
                        case 3: p = 993520002; break;
                        case 4: p = 993520000; break;
                        case 5: p = 993520003; break;
                        case 6: p = 993520001; break;
                        case 7: p = 993520004; break;
                    }
                    registro.Attributes.Add("itbc_politicadeposvenda", new OptionSetValue(p));
                }

                crmConnection.Create(registro);
            }
        }

        public static string ConsultaCnpjConta(string id)
        {
            string cnpj = "";
            string qbs = "select new_cpf,new_cnpj from account where accountid = '"+id+"'";
            if (ErpConection.State == System.Data.ConnectionState.Closed)
                ErpConection.Open();
            SqlCommand command1 = new SqlCommand(qbs, ErpConection);
            SqlDataReader reader1 = command1.ExecuteReader();
            if(reader1.Read())
            {
                if (reader1["new_cpf"] != null && reader1["new_cpf"].ToString() != "")
                    cnpj = reader1["new_cpf"].ToString();
                if (reader1["new_cnpj"] != null && reader1["new_cnpj"].ToString() != "")
                    cnpj = reader1["new_cnpj"].ToString();
            }
            reader1.Close();
            return cnpj;
        }
    }
}
