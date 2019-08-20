using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;
using System.DirectoryServices.AccountManagement;
using System.ServiceModel.Description;
using Microsoft.Crm.Sdk.Messages;
using Intelbras.CRM2013.Domain.Servicos;

namespace Intelbras.CRM2013.Application.ValidaCNPJRepetidosCRM
{
    class Program
    {
        private static string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private static bool IsOffline = false;
        private static SqlConnection _erpConection;

        protected static SqlConnection ErpConection
        {
            get
            {
                if (_erpConection == null)
                {
                    _erpConection = new SqlConnection();
                    _erpConection.ConnectionString = SDKore.Configuration.ConfigurationManager.GetSettingValue("Sellout.Provider.ConnectionString", false);
                }
                return _erpConection;
            }
        }

        protected static void GravaLog(string log)
        {
            using (StreamWriter w = File.AppendText(@"c:\temp\logrepetidos.txt"))
            {
                w.WriteLine(log);
            }
        }

        static void Main(string[] args)
        {
            ContatoService contatoService = new ContatoService(OrganizationName, IsOffline);
            ContaService contaService = new ContaService(OrganizationName, IsOffline);
            switch (args[0].ToUpper())
            {
                case "COLOCA_MASCARA_CONTATO":
                    contatoService.ColocarMascara();
                    break;
                case "COLOCA_MASCARA_CONTA":
                    contaService.ColocarMascara();
                    break;
                case "CNPJ_REPETIDO":
                    try
                    {
                        var reader = new StreamReader(File.OpenRead(@"c:\contas_mesmo_cnpjDEV2015.csv"));
                        //if (ErpConection.State == ConnectionState.Closed)
                        //    ErpConection.Open();
                        string[] linhas = reader.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        for (int x = 0; x < linhas.Length; x++)
                        {
                            string[] valorAtual = linhas[x].Split(';');
                            string cnpj = valorAtual[0], id = valorAtual[1].ToUpper(), codigo = valorAtual[2];

                            string[] valorProximo = linhas[x + 1].Split(';');
                            string cnpjProx = valorProximo[0], idProx = valorProximo[1].ToUpper(), codigoProx = valorProximo[2];

                            if (cnpj != cnpjProx)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("CNPJ Triplicado: Linha " + x.ToString());
                                //Console.ReadKey();
                                continue;
                                //return x;
                            }
                            //string query = "Select [IdRevendaCRM] FROM Revendas where [cpfcnpj] = '" + cnpj + "'";
                            //SqlCommand cmd = new SqlCommand(query, ErpConection);
                            //SqlDataReader registroSellOut = cmd.ExecuteReader();
                            //if (registroSellOut.HasRows)
                            //{
                            //    registroSellOut.Read();
                            //    if (id == Convert.ToString(registroSellOut.GetValue(0)).ToUpper() && string.IsNullOrEmpty(codigo))
                            //        MergeContaCRM(new Guid(idProx), new Guid(id), false);
                            //    else //if (idProx == registroSellOut.GetString(0) && string.IsNullOrEmpty(codigoProx))
                            //        MergeContaCRM(new Guid(id), new Guid(idProx), false);
                            //}
                            //else
                            //{
                            if (string.IsNullOrEmpty(codigo))
                                MergeContaCRM(new Guid(idProx), new Guid(id), false);
                            else //if (idProx == registroSellOut.GetString(0) && string.IsNullOrEmpty(codigoProx))
                                MergeContaCRM(new Guid(id), new Guid(idProx), true);
                            //}
                            //registroSellOut.Close();
                            //registroSellOut = null;
                            //cmd.Dispose();
                            //cmd = null;
                            x++;

                        }

                    }
                    catch (Exception ex)
                    {
                        string messageError = SDKore.Helper.Error.Handler(ex);
                        Console.WriteLine("Erro : " + messageError);
                        GravaLog(messageError);
                        var hashcode = ex.GetHashCode();
                    }

                    break;
            }
            
        }

        private static bool MergeContaCRM(Guid masterAccountId, Guid subOrdinateAccountId, bool ValidacaoAccountNumber)
        {
            try
            {
                Guid ContaId = masterAccountId;
                Guid ContaSubId = subOrdinateAccountId;
                //Authenticate using credentials of the logged in user;
                string UserName = "crm2015admin";
                string Password = "mAHhPZPWUYPPtq5z8MMWvvFr";
                string Dominio = "INTELBRAS";
                UserName = Dominio + @"\" + UserName;
                ClientCredentials Credentials = new ClientCredentials();
                Credentials.UserName.UserName = UserName;
                Credentials.UserName.Password = Password;
                Uri OrganizationUri = new Uri("https://crm2015dev.intelbras.com.br/IntelbrasCRM/XRMServices/2011/Organization.svc");
                using (OrganizationServiceProxy _Service = new OrganizationServiceProxy(OrganizationUri, null, Credentials, null))
                {
                    IOrganizationService crmConnection = (IOrganizationService)_Service;
                    _Service.Timeout = new TimeSpan(0, 20, 0);
                    
                    var cols = new ColumnSet(new[] { "statecode", "primarycontactid", "websiteurl", "telephone1", "fax", "emailaddress1", "accountnumber", "name",
                    "revenue", "numberofemployees", "description", "sic", "ownershipcode", "marketcap", "sharesoutstanding", "tickersymbol", "stockexchange",
                    "ftpsiteurl", "emailaddress2", "emailaddress3", "donotphone", "donotfax", "telephone1", "donotemail", "telephone2", "telephone3", "donotpostalmail",
                    "donotbulkemail", "donotbulkpostalmail", "creditlimit", "creditonhold", "isprivate", "donotsendmm", "itbc_address1_number", "itbc_address1_street",
                    "itbc_address2_number", "itbc_address2_street", "itbc_agencia", "itbc_agenteretencao", "itbc_apuracaodebeneficiosecompromissos", "itbc_atacadistavarejista",
                    "itbc_atividadeeconmicaramodeatividade", "itbc_banco", "itbc_calcula_multa", "itbc_coberturageografica", "itbc_codigosuframa", "itbc_conta_corrente",
                    "itbc_contribuinteicms", "itbc_cpfoucnpj", "itbc_dataadesao", "itbc_datadeconstituio", "itbc_datadeimplantacao", "itbc_datadevencimentoconcessao",
                    "itbc_datalimitedecredito", "itbc_descontocat", "itbc_diasdeatraso", "itbc_distfontereceita", "itbc_docidentidade", "itbc_embarquevia",
                    "itbc_emissordocidentidade", "itbc_emitebloqueto", "itbc_espaco_fisico_qualificado", "itbc_exclusividade", "itbc_formadetributacao",
                    "itbc_gera_aviso_credito", "itbc_guidcrm40", "itbc_historico", "itbc_incoterm", "itbc_indicada_icon", "itbc_indicada_icorp", "itbc_indicada_inet",
                    "itbc_indicada_isec", "itbc_inscricaoestadual", "itbc_inscricaomunicipal", "itbc_integradopor", "itbc_intencaoapoio", "itbc_localembarque",
                    "itbc_matrizoufilial", "itbc_metodo_comercializacao_produtos", "itbc_modalidade", "itbc_modelooperacaofiliais", "itbc_natureza", "itbc_nomeabreviado",
                    "itbc_nomefantasia", "itbc_numdecolaboradores", "itbc_numdevendedores", "itbc_numerorevendasativas", "itbc_numerorevendasinativas",
                    "itbc_numtecnicossuporte", "itbc_obsnf", "itbc_obspedido", "itbc_optantesuspensaoipi", "itbc_outrafontereceita", "itbc_participa_do_programa",
                    "itbc_perfilrevendasdodistribuidor", "itbc_piscofinsporunidade", "itbc_possuiestruturacompleta", "itbc_possuifiliais", "itbc_prazomediocompras",
                    "itbc_prazomediovendas", "itbc_quantasfiliais", "itbc_ramal_fax", "itbc_ramaloutrotelefone", "itbc_ramaltelefoneprincipal", "itbc_recebe_informacao_sci",
                    "itbc_recebenfe", "itbc_saldodecredito", "itbc_softwaredenegocios", "itbc_substituicaotributaria", "itbc_tipodeconstituicao", "itbc_tipodeembalagem",
                    "itbc_ult_atualizacao_integracao", "itbc_usuariointegracao", "itbc_valormediocomprasmensais", "itbc_valormediocomprasmensais_base", "itbc_valormediovendasmensais",
                    "itbc_valormediovendasmensais_base", "itbc_distribuidor_principal", "itbc_classificacaoid", "itbc_condicao_pagamento", "itbc_portador", "itbc_posvendaid",
                    "itbc_receitapadraoid", "itbc_subclassificacaoid", "itbc_transportadora", "itbc_transportadoraredespacho", "itbc_databaixacontribuinte",
                    "itbc_datahoraintegracaosefaz", "itbc_numeropassaporte", "itbc_origemconta", "itbc_regimeapuracao", "itbc_statusintegracaosefaz", "itbc_isastec",
                    "itbc_nomeabrevmatrizeconomica", "itbc_perfilastec", "itbc_tabelaprecoastec", "itbc_ultprocesssellout", "itbc_cnaeid", "itbc_participapcimotivo",
                    "itbc_adesaopcirealizadapor", "itbc_escolheudistrforasellout", "itbc_dataultimosellout", "itbc_figuranosite",
                    "new_agencia",
"new_agente_retencao",
"new_altera_endereco_padrao",
"new_banco",
"new_calcula_multa",
"new_canal_vendaid",
"new_condicao_pagamentoid",
"new_grupo_clienteid",
"new_portadorid",
"new_receita_padraoid",
"new_cnpj",
"new_codigo_suframa",
"new_contacorrente",
"new_contribuinte_icms",
"new_cpf",
"new_crm2013",
"new_data_credenciamento",
"new_data_implantacao",
"new_data_limite_credito",
"new_data_ultimo_pedido_posto_servico",
"new_data_vencimento_concessao",
"new_desconto_cat",
"new_dispositivo_legal",
"new_divulgada_site",
"new_embarque_via",
"new_emite_bloqueto",
"new_envioparaerp",
"new_exporta_erp",
"new_fidelidade",
"new_forma_tributacao_manaus",
"new_gera_aviso_credito",
"new_gera_extrato_dia_01",
"new_gera_extrato_dia_02",
"new_gera_extrato_dia_03",
"new_gera_extrato_dia_04",
"new_gera_extrato_dia_05",
"new_gera_extrato_dia_06",
"new_gera_extrato_dia_07",
"new_gera_extrato_dia_08",
"new_gera_extrato_dia_09",
"new_gera_extrato_dia_10",
"new_gera_extrato_dia_11",
"new_gera_extrato_dia_12",
"new_gera_extrato_dia_13",
"new_gera_extrato_dia_14",
"new_gera_extrato_dia_15",
"new_gera_extrato_dia_16",
"new_gera_extrato_dia_17",
"new_gera_extrato_dia_18",
"new_gera_extrato_dia_19",
"new_gera_extrato_dia_20",
"new_gera_extrato_dia_21",
"new_gera_extrato_dia_22",
"new_gera_extrato_dia_23",
"new_gera_extrato_dia_24",
"new_gera_extrato_dia_25",
"new_gera_extrato_dia_26",
"new_gera_extrato_dia_27",
"new_gera_extrato_dia_28",
"new_gera_extrato_dia_29",
"new_gera_extrato_ultimo_dia",
"new_geracao_pedido_posto",
"new_identificacao",
"new_incoterm",
"new_inscricaoestadual",
"new_inscricaomunicipal",
"new_integracao_revenda_site",
"new_intelbras_clube",
"new_isnc_subs_trib",
"new_local_embarque",
"new_mensagem",
"new_mercado_atuacao_redes",
"new_mercado_atuacao_seguranca",
"new_mercado_atuacao_telecom",
"new_modalidade",
"new_natureza",
"new_nome_abreviado_erp",
"new_nome_fantasia",
"new_numero_endereco_cobranca",
"new_numero_endereco_principal",
"new_numero_passaporte",
"new_observacao_pedido",
"new_optante_ipi",
"new_parametro_pedido_posto_servico",
"new_perfil_empresa_fidelidade",
"new_pis_cofins_unidade",
"new_posto_servico",
"new_prestador_servico_isol",
"new_ramal_fax",
"new_ramal1",
"new_ramal2",
"new_recebe_informacao_sci",
"new_recebe_nfe",
"new_representanteid",
"new_responsavel_icomp_id",
"new_responsavel_icon_id",
"new_responsavel_icorp_id",
"new_responsavel_imax_id",
"new_responsavel_inet_id",
"new_responsavel_isec_id",
"new_rg",
"new_saldo_credito",
"new_saldo_credito_base",
"new_sem_masc_cnpj_cpf",
"new_status_cadastro",
"new_status_integracao",
"new_tipo_embalagem",
"new_transp_assistencia_tecnica",
"new_transportadora_redespachoid",
"new_transportadoraid",
"new_vendas_alc"

                    });

                    //"itbc_address1_stateorprovince", "itbc_address2_stateorprovince", "itbc_address1_city", "itbc_address2_city", "itbc_address1_country", "itbc_address2_country", 

                    var cols2 = new ColumnSet();
                    cols2.AllColumns = true;
                    var masterAccount = crmConnection.Retrieve("account", ContaId, cols2);
                    if (masterAccount == null)
                        return true;
                    if (((OptionSetValue)masterAccount.Attributes["statecode"]).Value == 1) //se tiver sido excl
                    {
                        crmConnection.Delete("account", ContaId);
                        return true;
                    }
                    //Testa se o CNPJ veio formatado, deve ter prioridade pois veio do Konviva, o não formatado veio do SellOut e pode ser excluido
                    if (!Convert.ToString(masterAccount.Attributes["itbc_cpfoucnpj"]).Contains(".") && !ValidacaoAccountNumber)
                    {
                        //Invertemos o Principal para o Secundário
                        ContaId = subOrdinateAccountId;
                        ContaSubId = masterAccountId;
                        //Testa de novo a conta
                        masterAccount = null;
                        masterAccount = crmConnection.Retrieve("account", ContaId, cols2);
                        if (masterAccount == null)
                            return true;
                        if (((OptionSetValue)masterAccount.Attributes["statecode"]).Value == 1)
                        {
                            crmConnection.Delete("account", ContaId);
                            return true;
                        }
                    }

                    //Get Subordinate Account Primary Contact,Website,Phone,Fax,Email
                    var subOrdinateAccount = crmConnection.Retrieve("account", ContaSubId, cols2);
                    if (subOrdinateAccount == null)
                        return true;
                    if (((OptionSetValue)subOrdinateAccount.Attributes["statecode"]).Value == 1)
                    {
                        crmConnection.Delete("account", ContaSubId);
                        return true;
                    }
                    Entity updateContent = new Entity("account");
                    EntityReference target = new EntityReference();
                    target.Id = ContaId;
                    target.LogicalName = "account";

                    for (int x = 0; x < cols.Columns.Count; x++)
                    {
                        if (!masterAccount.Contains(cols.Columns[x]) && subOrdinateAccount.Contains(cols.Columns[x]) && subOrdinateAccount.Attributes[cols.Columns[x]] != null)
                        {
                            if (updateContent.Attributes.Contains(cols.Columns[x]))
                                updateContent.Attributes.Remove(cols.Columns[x]);
                            if (typeof(EntityReference) == subOrdinateAccount.Attributes[cols.Columns[x]].GetType())
                                updateContent.Attributes.Add(cols.Columns[x], new EntityReference(subOrdinateAccount.GetAttributeValue<EntityReference>(cols.Columns[x]).LogicalName, subOrdinateAccount.GetAttributeValue<EntityReference>(cols.Columns[x]).Id));
                            else
                                updateContent.Attributes.Add(cols.Columns[x], subOrdinateAccount.Attributes[cols.Columns[x]]);
                        }
                    }
                    MergeRequest merge = new MergeRequest();
                    merge.SubordinateId = ContaSubId;
                    merge.Target = target;
                    merge.PerformParentingChecks = false;
                    merge.UpdateContent = updateContent;
                    
                    MergeResponse mergeRes = (MergeResponse)crmConnection.Execute(merge);
                    GravaLog(masterAccountId.ToString() + " : Sucesso");
                    Console.Write("+");
                }
            }
            catch (Exception ex)
            {
                Console.Write("E");
                GravaLog(masterAccountId.ToString() + " : " + ex.Message);
            }
            return true;
        }
    }
}
