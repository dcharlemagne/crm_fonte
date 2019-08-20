using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SDKore.Crm;
using SDKore.Crm.Util;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intelbras.CRM2013.DAL
{
    public class RepAtualizacaoCRM<T> : CrmServiceRepository<T>, IRepositoryBase
    {

        public void AtualizaFamiliaComercial()
        {
            var query = new QueryExpression("itbc_familiacomercial");
            query.AddLink("new_familiacomercial", "itbc_codigo_familia_comercial", "new_codigo_familia_comercial");
            var colecao = base.Provider.RetrieveMultiple(query);

            foreach (var item in colecao.Entities)
            {
                var familiaComercial = new FamiliaComercial();
                familiaComercial.Id = item.Id;

                var ent = EntityConvert.Convert(familiaComercial, this.OrganizationName, this.IsOffline);
                this.Provider.Update(ent);
            }
        }

        public void AtualizaLinhaUnidadeNegocio()
        {
            var query = new QueryExpression("new_linha_unidade_negocio");
            //query.ColumnSet.AddColumns(new string[] { "itbc_estabelecimentoid", "itbc_canaldevendaid" });

            var filter = new FilterExpression(LogicalOperator.Or);
            filter.Conditions.Add(new ConditionExpression("new_estabelecimentoid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_canal_vendaid", ConditionOperator.NotNull));
            query.Criteria.AddFilter(filter);

            var q1 = query.AddLink("new_estabelecimento", "new_estabelecimentoid", "new_estabelecimentoid",JoinOperator.LeftOuter);            
            var q2 = q1.AddLink("itbc_estabelecimento", "new_codigo_estabelecimento", "itbc_codigo_estabelecimento", JoinOperator.LeftOuter);
            q2.Columns.AddColumn("itbc_estabelecimentoid");
            q2.EntityAlias = "e";

            var q3 = query.AddLink("new_canal_venda", "new_canal_vendaid", "new_canal_vendaid", JoinOperator.LeftOuter);            
            var q4 = q3.AddLink("itbc_canaldevenda", "new_codigo_canal_venda", "itbc_codigo_venda", JoinOperator.LeftOuter);
            q4.EntityAlias = "c";
            q4.Columns.AddColumn("itbc_canaldevendaid");

            var colecao = base.Provider.RetrieveMultiple(query);

            foreach (var item in colecao.Entities)
            {
                if (item.Contains("e.itbc_estabelecimentoid") || item.Contains("c.itbc_canaldevendaid"))
                {
                    var linha = new LinhaComercial();
                    linha.Id = item.Id;
                    if (item.Contains("e.itbc_estabelecimentoid"))
                        linha.EstabelecimentoId = new Lookup(new Guid(((AliasedValue)item["e.itbc_estabelecimentoid"]).Value.ToString()), "itbc_estabelecimento");
                    if (item.Contains("c.itbc_canaldevendaid"))
                        linha.CanalDeVendaId = new Lookup(new Guid(((AliasedValue)item["c.itbc_canaldevendaid"]).Value.ToString()), "itbc_estabelecimento");

                    var ent = EntityConvert.Convert(linha, this.OrganizationName, this.IsOffline);
                    this.Provider.Update(ent);
                }
            }
        }

        public void AtualizaPermissaoUsuarioB2B()
        {
            var query = new QueryExpression("new_permissao_usuario_b2b");
            //query.ColumnSet.AddColumns(new string[] { "itbc_estabelecimentoid", "itbc_canaldevendaid" });

            var q1 = query.AddLink("new_representante", "new_representanteid", "new_representanteid", JoinOperator.Natural);            
            q1.Columns.AddColumn("new_codigo_representante");
            q1.EntityAlias = "r";

            var colecao = base.Provider.RetrieveMultiple(query);

            foreach (var item in colecao.Entities)
            {
                var ent = new Entity("new_permissao_usuario_b2b");
                ent.Id = item.Id;
                ent.Attributes.Add("itbc_codigo_representante", Convert.ToInt32(((AliasedValue)item.Attributes["r.new_codigo_representante"]).Value));

                this.Provider.Update(ent);
            }
        }

        public void AtualizaCategoriaB2B()
        {
            var query = new QueryExpression("new_categoria");
            query.ColumnSet.AddColumns(new string[] { "new_name", "new_codigo_categoria" });

            var colecao = base.Provider.RetrieveMultiple(query);

            foreach (var item in colecao.Entities)
            {
                var ent = new Entity("itbc_categoriadob2b");
                //ent.Id = item.Id;
                ent.Attributes.Add("itbc_name", item.Attributes["new_name"]);
                ent.Attributes.Add("itbc_codigocategoriab2b", Convert.ToInt32(item.Attributes["new_codigo_categoria"]));

                this.Provider.Create(ent);
            }
        }

        public void AtualizaRelacionamentoB2B()
        {
            var query = new QueryExpression("new_relacionamento");
            query.PageInfo = new PagingInfo();
            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;

            query.ColumnSet.AddColumns(new string[] { "new_clienteid", "new_data_final","new_data_inicial", "new_mensagem","new_name","new_sequencia", "new_unidadedenegociosid", "new_chaveintegracao" });

            var q1 = query.AddLink("new_categoria", "new_categoriaid", "new_categoriaid", JoinOperator.LeftOuter);
            var q2 = q1.AddLink("itbc_categoriadob2b", "new_codigo_categoria", "itbc_codigocategoriab2b", JoinOperator.LeftOuter);
            q2.Columns.AddColumn("itbc_categoriadob2bid");
            q2.EntityAlias = "a";

            var q3 = query.AddLink("new_representante", "new_representanteid", "new_representanteid", JoinOperator.LeftOuter);            
            q3.EntityAlias = "b";
            q3.Columns.AddColumn("new_codigo_representante");

            var q4 = query.AddLink("businessunit", "new_unidadedenegociosid", "businessunitid", JoinOperator.LeftOuter);
            q4.EntityAlias = "c";
            q4.Columns.AddColumn("name");

            
            var colecao = base.Provider.RetrieveMultiple(query);

            while (true)
            {

                foreach (var item in colecao.Entities)
                {
                    var obj = new RelacionamentoB2B(this.OrganizationName, this.IsOffline);
                    obj.CodigoRepresentante = Convert.ToInt32(((AliasedValue)item.Attributes["b.new_codigo_representante"]).Value);
                    obj.CategoriaB2B = new Lookup(new Guid(((AliasedValue)item["a.itbc_categoriadob2bid"]).Value.ToString()), "itbc_categoriadob2b");
                    obj.Canal = new Lookup(((EntityReference)item["new_clienteid"]).Id, "account");
                    if (item.Contains("new_unidadedenegociosid"))
                        obj.UnidadeNegocio = new Lookup(((EntityReference)item["new_unidadedenegociosid"]).Id, "businessunit");
                    obj.DataInicial = (DateTime)item["new_data_inicial"];
                    if (item.Contains("new_data_final"))
                        obj.DataFinal = (DateTime)item["new_data_final"];
                    obj.NomeRelacionamento = (string)item["new_name"];
                    if (item.Contains("new_mensagem"))
                        obj.Mensagem = (string)item["new_mensagem"];
                    if (item.Contains("c.name"))
                        obj.NomeUnidadeNegocio = ((AliasedValue)item["c.name"]).Value.ToString();
                    if (item.Contains("new_sequencia"))
                        obj.Sequencia = Convert.ToInt32(item["new_sequencia"]);
                    obj.CodigoRelacionamentoB2B = (string)item["new_chaveintegracao"];
                    var ent = EntityConvert.Convert(obj, this.OrganizationName, this.IsOffline);

                    this.Provider.Create(ent);
                }
                if (colecao.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = colecao.PagingCookie;
                }
                else
                {
                    break;
                }

            }
        }

        public void AtualizaEstabelecimento()
        {
            var query = new QueryExpression("new_estabelecimento");
            query.ColumnSet.AddColumns(new string[] { "new_exibir_b2b"});

            var q1 = query.AddLink("itbc_estabelecimento", "new_codigo_estabelecimento", "itbc_codigo_estabelecimento", JoinOperator.Inner);
            q1.Columns.AddColumn("itbc_estabelecimentoid");
            q1.EntityAlias = "a";

            var colecao = base.Provider.RetrieveMultiple(query);

            foreach (var item in colecao.Entities)
            {
                
                var ent = new Entity("itbc_estabelecimento");
                ent.Id = new Guid(((AliasedValue)item["a.itbc_estabelecimentoid"]).Value.ToString());
                if (item.Contains("new_exibir_b2b"))
                    ent.Attributes.Add("itbc_exibir_b2b", item.Attributes["new_exibir_b2b"]);
                else
                    ent.Attributes.Add("itbc_exibir_b2b", false);
                this.Provider.Update(ent);
            }
        }


        public void AtualizarTabelaPrecoB2B()
        {
            var query = new QueryExpression("new_tabela_preco");
            query.ColumnSet.AddColumns(new string[] { "new_codigo_tabela","new_data_final","new_data_inicial","new_moedaid","new_name","new_transportadoraid" });

            var q1 = query.AddLink("transactioncurrency", "new_moedaid", "transactioncurrencyid", JoinOperator.Inner);
            q1.Columns.AddColumn("currencyname");
            q1.EntityAlias = "a";

            var colecao = base.Provider.RetrieveMultiple(query);

            foreach (var item in colecao.Entities)
            {

                var obj = new TabelaPrecoB2B(this.OrganizationName, this.IsOffline);
                obj.Nome = (string)item["new_name"];
                obj.NomeMoeda = ((AliasedValue)item["a.currencyname"]).Value.ToString();
                obj.CodigoTabelaPrecoEMS = (string)item["new_codigo_tabela"];
                obj.DataInicial = (DateTime)item["new_data_inicial"];
                obj.DataFinal = (DateTime)item["new_data_final"];    
                var ent = EntityConvert.Convert(obj, this.OrganizationName, this.IsOffline);

                this.Provider.Create(ent);
            }

            var q2 = new QueryExpression("new_item_tabela");
            q2.PageInfo = new PagingInfo();
            q2.PageInfo.PageNumber = 1;
            q2.PageInfo.Count = 5000;
            q2.PageInfo.PagingCookie = null;

            q2.ColumnSet.AddColumns(new string[] { "new_chaveintegracao", "new_name", "new_pma","new_pmd","new_preco_fob","new_preco_min_cif","new_preco_min_fob","new_preco_unico","new_preco_venda","new_produtoid","new_quantidade_minima" });

            var q3 = q2.AddLink("new_tabela_preco", "new_tabela_precoid", "new_tabela_precoid", JoinOperator.Inner);
            var q4 = q3.AddLink("itbc_tabelaprecob2b", "new_codigo_tabela", "itbc_tabelaprecoems", JoinOperator.Inner);
            q4.Columns.AddColumn("itbc_tabelaprecob2bid");
            q4.EntityAlias = "b";


            while (true)
            {
                var colecao2 = base.Provider.RetrieveMultiple(q2);

                foreach (var item in colecao2.Entities)
                {
                    if (item.Contains("new_produtoid"))
                    {
                        var obj = new ItemTabelaPrecoB2B(this.OrganizationName, this.IsOffline);
                        obj.PrecoFOB = ((Money)item["new_preco_fob"]).Value;
                        obj.PrecoMinimoCIF = ((Money)(item["new_preco_min_cif"])).Value;
                        obj.PrecoMinimoFOB = ((Money)(item["new_preco_min_fob"])).Value;
                        obj.PrecoUnico = ((Money)item["new_preco_unico"]).Value;
                        obj.PrecoVenda = ((Money)item["new_preco_venda"]).Value;
                        obj.QuantidadeMinima = Convert.ToDecimal(item["new_quantidade_minima"]);
                        obj.Produto = new Lookup(((EntityReference)item["new_produtoid"]).Id, "product");
                        obj.TabelaPreco = new Lookup(new Guid(((AliasedValue)item["b.itbc_tabelaprecob2bid"]).Value.ToString()), "itbc_tabelaprecob2b");
                        obj.CodigoItemPreco = (string)item["new_chaveintegracao"];

                        var ent = EntityConvert.Convert(obj, this.OrganizationName, this.IsOffline);

                        this.Provider.Create(ent);
                    }
                }
                if (colecao2.MoreRecords)
                {
                    q2.PageInfo.PageNumber++;
                    q2.PageInfo.PagingCookie = colecao2.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        public void AtualizarConta()
        {
            var query = new QueryExpression("account");
            query.PageInfo = new PagingInfo();
            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;

            query.ColumnSet.AddColumns(new string[] { "new_agencia", "new_agente_retencao", "new_banco", "new_calcula_multa", "new_canal_vendaid", "new_cnpj", "new_codigo_suframa",
                "new_condicao_pagamentoid", "new_contacorrente", "new_contribuinte_icms", "new_cpf", "new_data_implantacao", "new_data_limite_credito", "new_data_vencimento_concessao",
                "new_desconto_cat", "new_dispositivo_legal", "new_embarque_via", "new_emite_bloqueto", "new_forma_tributacao_manaus", "new_gera_aviso_credito", "new_grupo_clienteid",
                "new_identificacao", "new_incoterm", "new_inscricaoestadual", "new_inscricaomunicipal", "new_isnc_subs_trib", "new_local_embarque", "new_natureza", "new_nome_abreviado_erp",
                "new_nome_fantasia", "new_numero_endereco_cobranca", "new_numero_endereco_principal", "new_numero_passaporte", "new_observacao_pedido", "new_optante_ipi", "new_portadorid",
                "new_ramal_fax", "new_ramal1", "new_ramal2", "new_recebe_informacao_sci", "new_recebe_nfe", "new_receita_padraoid", "new_representanteid", "new_rg", "new_saldo_credito",
                "new_tipo_embalagem", "new_transp_assistencia_tecnica", "new_transportadora_redespachoid", "new_transportadoraid", "new_vendas_alc", "address1_line1", "address1_line2",
                "address1_line3", "address1_stateorprovince", "address1_city", "address1_country","address2_line1", "address2_line2", "address2_line3", "address2_stateorprovince", "address2_city", "address2_country" });

            var filter = new FilterExpression(LogicalOperator.Or);
            filter.Conditions.Add(new ConditionExpression("new_tipo_embalagem", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_transp_assistencia_tecnica", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_transportadora_redespachoid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_transportadoraid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_vendas_alc", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_agencia", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_agente_retencao", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_banco", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_calcula_multa", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_canal_vendaid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_cnpj", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_codigo_suframa", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_condicao_pagamentoid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_contacorrente", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_contribuinte_icms", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_cpf", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_data_implantacao", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_data_limite_credito", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_data_vencimento_concessao", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_desconto_cat", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_dispositivo_legal", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_embarque_via", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_emite_bloqueto", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_forma_tributacao_manaus", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_gera_aviso_credito", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_grupo_clienteid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_identificacao", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_incoterm", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_inscricaoestadual", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_inscricaomunicipal", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_isnc_subs_trib", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_local_embarque", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_natureza", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_nome_abreviado_erp", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_nome_fantasia", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_numero_endereco_cobranca", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_numero_endereco_principal", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_numero_passaporte", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_observacao_pedido", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_optante_ipi", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_portadorid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_ramal_fax", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_ramal1", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_ramal2", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_recebe_informacao_sci", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_recebe_nfe", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_receita_padraoid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_representanteid", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_rg", ConditionOperator.NotNull));
            filter.Conditions.Add(new ConditionExpression("new_saldo_credito", ConditionOperator.NotNull));
            
            query.Criteria.AddFilter(filter);

            var q1 = query.AddLink("new_canal_venda", "new_canal_vendaid", "new_canal_vendaid", JoinOperator.LeftOuter);
            var q2 = q1.AddLink("itbc_canaldevenda", "new_codigo_canal_venda", "itbc_codigo_venda", JoinOperator.LeftOuter);
            q2.Columns.AddColumn("itbc_codigo_venda");
            q2.EntityAlias = "a";

            var q3 = query.AddLink("new_condicao_pagamento", "new_condicao_pagamentoid", "new_condicao_pagamentoid", JoinOperator.LeftOuter);
            var q4 = q3.AddLink("itbc_condicao_pagamento", "new_codicao_pagamento", "itbc_condicao_pagamento", JoinOperator.LeftOuter);
            q4.Columns.AddColumn("itbc_condicao_pagamentoid");
            q4.EntityAlias = "b";

            var q5 = query.AddLink("new_portador", "new_portadorid", "new_portadorid", JoinOperator.LeftOuter);
            var q6 = q5.AddLink("itbc_portador", "new_cod_portador", "itbc_codigo_portador", JoinOperator.LeftOuter);
            q6.Columns.AddColumn("itbc_portadorid");
            q6.EntityAlias = "c";

            var q7 = query.AddLink("new_receita_padrao", "new_receita_padraoid", "new_receita_padraoid", JoinOperator.LeftOuter);
            var q8 = q7.AddLink("itbc_receitapadrao", "new_codigo_receita_padrao", "itbc_codigoreceitapadrao", JoinOperator.LeftOuter);
            q8.Columns.AddColumn("itbc_receitapadraoid");
            q8.EntityAlias = "d";

            var q9 = query.AddLink("new_representante", "new_representanteid", "new_representanteid", JoinOperator.LeftOuter);
            var q10 = q9.AddLink("contact", "new_codigo_representante", "itbc_codigodorepresentante", JoinOperator.LeftOuter);
            q10.Columns.AddColumn("contactid");
            q10.EntityAlias = "e";


            base.Provider.Timeout = new TimeSpan(0, 20, 0); //20 minutos

            while (true)
            {


                var colecao = base.Provider.RetrieveMultiple(query);

                foreach (var item in colecao.Entities)
                {
                    var obj = new Conta(this.OrganizationName, this.IsOffline);
                    obj.Id = item.Id;
                    if (item.Contains("new_agencia"))
                        obj.Agencia = (string)item["new_agencia"];
                    if (item.Contains("new_agente_retencao"))
                        obj.AgenteRetencao = (bool)item["new_agente_retencao"];
                    if (item.Contains("new_banco"))
                        obj.Banco = (string)item["new_banco"];
                    if (item.Contains("new_calcula_multa"))
                        obj.CalculaMulta = (bool)item["new_calcula_multa"];
                    if (item.Contains("new_cnpj"))
                        obj.CpfCnpj = (string)item["new_cnpj"];
                    if (item.Contains("new_codigo_suframa"))
                        obj.CodigoSuframa = (string)item["new_codigo_suframa "];
                    if (item.Contains("new_contacorrente"))
                        obj.ContaCorrente = (string)item["new_contacorrente"];
                    if (item.Contains("new_contribuinte_icms"))
                        obj.ContribuinteICMS = (bool)item["new_contribuinte_icms"];
                    if (item.Contains("new_cpf"))
                        obj.CpfCnpj = (string)item["new_cpf"];
                    if (item.Contains("new_data_implantacao"))
                        obj.DataImplantacao = (DateTime)item["new_data_implantacao"];
                    if (item.Contains("new_data_limite_credito"))
                        obj.DataLimiteCredito = (DateTime)item["new_data_limite_credito"];
                    if (item.Contains("new_data_vencimento_concessao"))
                        obj.DataVenctoConcessao = (DateTime)item["new_data_vencimento_concessao"];
                    if (item.Contains("new_desconto_cat"))
                        obj.DescontoCAT = Convert.ToDecimal(item["new_desconto_cat"]);
                    if (item.Contains("new_dispositico_legal"))
                        obj.ObservacoesNF = (string)item["new_dispositivo_legal"];
                    if (item.Contains("new_embarque_via"))
                        obj.EmbarqueVia = (string)item["new_embarque_via"];
                    if (item.Contains("new_emite_bloqueto"))
                        obj.EmiteBloqueto = (bool)item["new_emite_bloqueto"];
                    if (item.Contains("new_gera_aviso_credito"))
                        obj.GeraAvisoCredito = (bool)item["new_gera_aviso_credito"];
                    if (item.Contains("new_incoterm"))
                        obj.Incoterm = (string)item["new_incoterm"];
                    if (item.Contains("new_inscricaoestadual"))
                        obj.InscricaoEstadual = (string)item["new_inscricaoestadual"];
                    if (item.Contains("new_incricaomunicipal"))
                        obj.InscricaoMunicipal = (string)item["new_inscricaomunicipal"];
                    if (item.Contains("new_isnc_subs_trib"))
                        obj.SubstituicaoTributaria = (string)item["new_isnc_subs_trib"];
                    if (item.Contains("new_local_embarque"))
                        obj.LocalEmbarque = (string)item["new_local_embarque"];
                    if (item.Contains("new_nome_abreviado_erp"))
                        obj.NomeAbreviado = (string)item["new_nome_abreviado_erp"];
                    if (item.Contains("new_nome_fantasia"))
                        obj.NomeFantasia = (string)item["new_nome_fantasia"];
                    if (item.Contains("new_numero_passaporte"))
                        obj.NumeroPassaporte = (string)item["new_numero_passaporte"];
                    if (item.Contains("new_observacao_pedido"))
                        obj.ObservacoesPedido = (string)item["new_observacao_pedido"];
                    if (item.Contains("new_optante_ipi"))
                        obj.OptanteSuspensaoIPI = (bool)item["new_optante_ipi"];
                    if (item.Contains("new_ramal_fax"))
                        obj.RamalFax = (string)item["new_ramal_fax"];
                    if (item.Contains("new_ramal1"))
                        obj.RamalTelefonePrincipal = (string)item["new_ramal1"];
                    if (item.Contains("new_ramal2"))
                        obj.RamalOutroTelefone = (string)item["new_ramal2"];
                    if (item.Contains("new_recebe_informacao_sci"))
                        obj.RecebeInformacaoSCI = (bool)item["new_recebe_informacao_sci"];
                    if (item.Contains("new_recebe_nfe"))
                        obj.RecebeNFE = (bool)item["new_recebe_nfe"];
                    if (item.Contains("new_rg"))
                        obj.DocIdentidade = (string)item["new_rg"];
                    if (item.Contains("new_saldo_credito"))
                        obj.SaldoCredito = Convert.ToDecimal(item["new_saldo_credito"]);
                    if (item.Contains("new_tipo_embalagem"))
                        obj.TipoEmbalagem = (string)item["new_tipo_embalagem"];
                    if (item.Contains("new_vendas_alc"))
                        obj.VendasParaAtacadistaVarejista = (bool)item["new_vendas_alc"];
                    if (item.Contains("new_numero_endereco_principal"))
                    {
                        var s = (string)item["new_numero_endereco_principal"];
                        obj.Endereco1Numero = s.Length > 5 ? s.Substring(0, 5) : s;
                    }
                    if (item.Contains("address1_line1"))
                    {
                        var s = (string)item["address1_line1"];
                        obj.Endereco1Rua = s.Length > 35 ? s.Substring(0, 35) : s;
                    }
                    if (item.Contains("address1_line2"))
                    {
                        var s = (string)item["address1_line2"];
                        obj.Endereco2Complemento = s.Length > 40 ? s.Substring(0, 40) : s;
                    }
                    if (item.Contains("address1_line3"))
                    {
                        var s = (string)item["address1_line3"];
                        obj.Endereco2Bairro = s.Length > 30 ? s.Substring(0, 30) : s;
                    }
                    if (item.Contains("address1_city") && item.Contains("address1_stateorprovince") && item.Contains("address1_country"))
                    {
                        var qp = new QueryExpression("itbc_pais");
                        qp.TopCount = 1;
                        qp.Criteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, (string)item["address1_country"]));
                        var c = base.Provider.RetrieveMultiple(qp);
                        if (c.Entities.Count > 0)
                        {
                            var pais = c.Entities.First();
                            var qe = new QueryExpression("itbc_estado");
                            qe.TopCount = 1;
                            var f = new FilterExpression();
                            f.FilterOperator = LogicalOperator.Or;
                            f.Conditions.Add(new ConditionExpression("itbc_siglauf", ConditionOperator.Equal, (string)item["address1_stateorprovince"]));
                            f.Conditions.Add(new ConditionExpression("itbc_name", ConditionOperator.Equal, (string)item["address1_stateorprovince"]));
                            qe.Criteria.Filters.Add(f);
                            qe.Criteria.AddCondition(new ConditionExpression("itbc_pais", ConditionOperator.Equal, pais.Id));
                            var c2 = base.Provider.RetrieveMultiple(qe);
                            if (c2.Entities.Count > 0)
                            {
                                var estado = c2.Entities.First();
                                var qm = new QueryExpression("itbc_municipios");
                                qm.TopCount = 1;
                                qm.Criteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, (string)item["address1_city"]));
                                qm.Criteria.AddCondition(new ConditionExpression("itbc_estadoid", ConditionOperator.Equal, estado.Id));
                                var c3 = base.Provider.RetrieveMultiple(qm);
                                if (c3.Entities.Count > 0)
                                {
                                    var cidade = c3.Entities.First();
                                    obj.Endereco1Municipioid = new Lookup(cidade.Id, "itbc_municipios");
                                }
                                obj.Endereco1Estadoid = new Lookup(estado.Id, "itbc_estado");
                            }
                            obj.Endereco1Pais = new Lookup(pais.Id, "itbc_pais");
                        }
                    }
                    if (item.Contains("new_numero_endereco_cobranca"))
                    {
                        var s = (string)item["new_numero_endereco_cobranca"];
                        obj.Endereco2Numero = s.Length > 5 ? s.Substring(0, 5) : s;
                    }
                    if (item.Contains("address2_line1"))
                    {
                        var s = (string)item["address2_line1"];
                        obj.Endereco2Rua = s.Length > 35 ? s.Substring(0, 35) : s;

                    }
                    if (item.Contains("address2_line2"))
                    {
                        var s = (string)item["address2_line2"];
                        obj.Endereco2Complemento = s.Length > 40 ? s.Substring(0, 40) : s;
                    }
                    if (item.Contains("address2_line3"))
                    {
                        var s = (string)item["address2_line3"];
                        obj.Endereco2Bairro = s.Length > 30 ? s.Substring(0, 30) : s;
                    }
                    if (item.Contains("address2_city") && item.Contains("address2_stateorprovince") && item.Contains("address2_country"))
                    {
                        var qp = new QueryExpression("itbc_pais");
                        qp.TopCount = 1;
                        qp.Criteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, (string)item["address2_country"]));
                        var c = base.Provider.RetrieveMultiple(qp);
                        if (c.Entities.Count > 0)
                        {
                            var pais = c.Entities.First();
                            var qe = new QueryExpression("itbc_estado");
                            qe.TopCount = 1;
                            var f = new FilterExpression();
                            f.FilterOperator = LogicalOperator.Or;
                            f.Conditions.Add(new ConditionExpression("itbc_siglauf", ConditionOperator.Equal, (string)item["address2_stateorprovince"]));
                            f.Conditions.Add(new ConditionExpression("itbc_name", ConditionOperator.Equal, (string)item["address2_stateorprovince"]));
                            qe.Criteria.Filters.Add(f);
                            qe.Criteria.AddCondition(new ConditionExpression("itbc_pais", ConditionOperator.Equal, pais.Id));
                            var c2 = base.Provider.RetrieveMultiple(qe);
                            if (c2.Entities.Count > 0)
                            {
                                var estado = c2.Entities.First();
                                var qm = new QueryExpression("itbc_municipios");
                                qm.TopCount = 1;
                                qm.Criteria.AddCondition(new ConditionExpression("itbc_name", ConditionOperator.Equal, (string)item["address2_city"]));
                                qm.Criteria.AddCondition(new ConditionExpression("itbc_estadoid", ConditionOperator.Equal, estado.Id));
                                var c3 = base.Provider.RetrieveMultiple(qm);
                                if (c3.Entities.Count > 0)
                                {
                                    var cidade = c3.Entities.First();
                                    obj.Endereco2Municipioid = new Lookup(cidade.Id, "itbc_municipios");
                                }
                                obj.Endereco2Estadoid = new Lookup(estado.Id, "itbc_estado");
                            }
                            obj.Endereco2Pais = new Lookup(pais.Id, "itbc_pais");
                        }
                    }
                    if (item.Contains("a.itbc_codigo_venda"))
                    {
                        obj.CanaldeVenda = Convert.ToInt32(item["a.itbc_codigo_venda"]);
                    }
                    if (item.Contains("b.itbc_condicao_pagamentoid"))
                    {
                        obj.CondicaoPagamento = new Lookup(new Guid(((AliasedValue)item["b.itbc_condicao_pagamentoid"]).Value.ToString()), "itbc_condicao_pagamento");
                    }
                    if (item.Contains("c.itbc_portadorid"))
                    {
                        obj.Portador = new Lookup(new Guid(((AliasedValue)item["c.itbc_portadorid"]).Value.ToString()), "itbc_portador");
                    }
                    if (item.Contains("d.itbc_receitapadraoid"))
                    {
                        obj.ReceitaPadrao = new Lookup(new Guid(((AliasedValue)item["d.itbc_receitapadraoid"]).Value.ToString()), "itbc_receitapadrao");
                    }
                    if (item.Contains("e.contactid"))
                    {
                        obj.Representante = new Lookup(new Guid(((AliasedValue)item["e.contactid"]).Value.ToString()), "contact");
                    }
                    if (item.Contains("new_transp_assistencia_tecnica"))
                    {
                        var q = new QueryExpression("new_transportadora");
                        q.Criteria.AddCondition(new ConditionExpression("new_transportadoraid", ConditionOperator.Equal, ((EntityReference)item["new_transp_assistencia_tecnica"]).Id.ToString()));
                        var q12 = q.AddLink("itbc_transportadora", "new_codigo_transportadora", "itbc_codigodatransportadora", JoinOperator.Natural);
                        q12.Columns.AddColumn("itbc_transportadoraid");
                        q12.EntityAlias = "f";

                        var c = base.Provider.RetrieveMultiple(q);
                        if (c.Entities.Count > 0)
                        {
                            var t = c.Entities.First();
                            obj.TransportadoraASTEC = new Lookup(new Guid(((AliasedValue)t["f.itbc_transportadoraid"]).Value.ToString()), "itbc_transportadora");
                        }
                    }
                    if (item.Contains("new_transportadora_redespachoid"))
                    {
                        var q = new QueryExpression("new_transportadora");
                        q.Criteria.AddCondition(new ConditionExpression("new_transportadoraid", ConditionOperator.Equal, ((EntityReference)item["new_transportadora_redespachoid"]).Id.ToString()));
                        var q12 = q.AddLink("itbc_transportadora", "new_codigo_transportadora", "itbc_codigodatransportadora", JoinOperator.Natural);
                        q12.Columns.AddColumn("itbc_transportadoraid");
                        q12.EntityAlias = "f";

                        var c = base.Provider.RetrieveMultiple(q);
                        if (c.Entities.Count > 0)
                        {
                            var t = c.Entities.First();
                            obj.TransportadoraRedespacho = new Lookup(new Guid(((AliasedValue)item["f.itbc_transportadoraid"]).Value.ToString()), "itbc_transportadora");
                        }
                    }
                    if (item.Contains("new_transportadoraid"))
                    {
                        var q = new QueryExpression("new_transportadora");
                        q.Criteria.AddCondition(new ConditionExpression("new_transportadoraid", ConditionOperator.Equal, ((EntityReference)item["new_transportadoraid"]).Id.ToString()));
                        var q12 = q.AddLink("itbc_transportadora", "new_codigo_transportadora", "itbc_codigodatransportadora", JoinOperator.Natural);
                        q12.Columns.AddColumn("itbc_transportadoraid");
                        q12.EntityAlias = "f";

                        var c = base.Provider.RetrieveMultiple(q);
                        if (c.Entities.Count > 0)
                        {
                            var t = c.Entities.First();
                            obj.Transportadora = new Lookup(new Guid(((AliasedValue)t["f.itbc_transportadoraid"]).Value.ToString()), "itbc_transportadora");
                        }
                    }
                    if (item.Contains("new_identificacao"))
                    {
                        obj.IdentificacaoConta = ((OptionSetValue)item["new_identificacao"]).Value;
                    }
                    if (item.Contains("new_forma_tributacao_manaus"))
                    {
                        obj.FormaTributacao = 993520000 + ((OptionSetValue)item["new_forma_tributacao_manaus"]).Value - 1;
                    }
                    if (item.Contains("new_modalidade"))
                    {
                        obj.Modalidade = 993520000 + ((OptionSetValue)item["new_modalidade"]).Value - 1;
                    }
                    if (item.Contains("new_natureza"))
                    {
                        int natrueza = ((OptionSetValue)item["new_natureza"]).Value;
                        switch (natrueza)
                        {
                            case 1: obj.Natureza = 993520003; obj.TipoConstituicao = 993520000; break;
                            case 2: obj.Natureza = 993520000; obj.TipoConstituicao = 993520001; break;
                            case 3: obj.Natureza = 993520001; obj.TipoConstituicao = 993520002; break;
                            case 4: obj.Natureza = 993520002; obj.TipoConstituicao = 993520002; break;
                        }
                    }

                    if (item.Contains("new_grupo_clienteid"))
                    {
                        var idGrupoCliente = ((EntityReference)item["new_grupo_clienteid"]).Id.ToString().ToUpper();
                        switch (idGrupoCliente)
                        {
                            case "57A51937-963E-E411-A122-00155D013D4A":
                                obj.Classificacao = new Lookup(new Guid("07DD5E73-6DD9-E511-8C4B-0050568D7C5E"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("E10BC3EC-6EED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("1F886648-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "CC4F48D1-3CF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("13A432BB-6DED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("BEC315D0-6EED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("68C05902-99EE-E311-940A-00155D013D3B"), "itbc_categoria");
                                break;
                            case "3295D721-3DF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("E11378C7-6DED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("962D89DF-6EED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("68C05902-99EE-E311-940A-00155D013D3B"), "itbc_categoria");
                                break;
                            case "3395D721-3DF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("46C0815A-6DD9-E511-8C4B-0050568D7C5E"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("A64C08E7-6DD9-E511-8C4B-0050568D7C5E"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("0802D40D-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "3495D721-3DF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("46C0815A-6DD9-E511-8C4B-0050568D7C5E"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("A64C08E7-6DD9-E511-8C4B-0050568D7C5E"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("0802D40D-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "F3DD085B-3DF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("026E4C24-6BED-E311-9420-00155D013D39"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("C6E1494F-6BED-E311-9420-00155D013D39"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("1F886648-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "046AEAA7-3DF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("11D697AB-6DED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("FACEA983-6FED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("76FD363B-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "056AEAA7-3DF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("0C3BB3EB-6DED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("5D56C688-6EED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("E0AEB351-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "066AEAA7-3DF2-E311-8FE0-00155D013E46":
                                obj.Classificacao = new Lookup(new Guid("F1C90BB4-6FED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("6E87ABCA-6FED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("848EFB66-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "F2C4BE9F-E350-E511-9C67-00155D014419":
                                obj.Classificacao = new Lookup(new Guid("026E4C24-6BED-E311-9420-00155D013D39"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("C6E1494F-6BED-E311-9420-00155D013D39"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("1F886648-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "9DEA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("0C3BB3EB-6DED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("5D56C688-6EED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("E0AEB351-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "9EEA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("0C3BB3EB-6DED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("5D56C688-6EED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("E0AEB351-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "A0EA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("F1C90BB4-6FED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("6E87ABCA-6FED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("848EFB66-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "A2EA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("46C0815A-6DD9-E511-8C4B-0050568D7C5E"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("A64C08E7-6DD9-E511-8C4B-0050568D7C5E"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("0802D40D-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "A7EA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("C40668BE-B7D1-E511-9423-0050568D1C61"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("DCFCDDE7-B5D1-E511-9423-0050568D1C61"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("1F886648-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "ACEA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("0C3BB3EB-6DED-E311-9407-00155D013D38"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("5D56C688-6EED-E311-9407-00155D013D38"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("E0AEB351-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "B6EA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("3F40D16A-7B38-E611-9433-0050568D63AB"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("FC6ED777-7B38-E611-9433-0050568D63AB"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("1F886648-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "BBEA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("026E4C24-6BED-E311-9420-00155D013D39"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("C6E1494F-6BED-E311-9420-00155D013D39"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("1F886648-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "D1EA0FF1-F8B9-DF11-8266-00155DA09700":
                                obj.Classificacao = new Lookup(new Guid("026E4C24-6BED-E311-9420-00155D013D39"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("C6E1494F-6BED-E311-9420-00155D013D39"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("1F886648-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                            case "F6FE25D4-C240-E311-8AFA-00155DA7056E":
                                obj.Classificacao = new Lookup(new Guid("3A664E7C-9E59-E611-9435-0050568D63AB"), "itbc_classificacao");
                                obj.Subclassificacao = new Lookup(new Guid("6B20928A-9E59-E611-9435-0050568D63AB"), "itbc_subclassificacoes");
                                obj.Categoria = new Lookup(new Guid("E0AEB351-6EED-E311-9407-00155D013D38"), "itbc_categoria");
                                break;
                        }
                    }
                    obj.NivelPosVendas = new Lookup(new Guid("37E3A262-75ED-E311-9407-00155D013D38"), "itbc_posvenda");
                    obj.OrigemConta = 993520005;
                    obj.TipoRelacao = 12;
                    obj.TipoConta = 993520000;
                    obj.ParticipantePrograma = 993520000;
                    obj.StatusIntegracaoSefaz = 993520000;

                    obj.IntegrarNoPlugin = true;

                    var ent = EntityConvert.Convert(obj, this.OrganizationName, this.IsOffline);
                    this.Provider.Update(ent);
                }
                if (colecao.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = colecao.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        public void AtualizarContato()
        {
            var query = new QueryExpression("contact");
            query.PageInfo = new PagingInfo();
            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;

            query.ColumnSet.AddColumns(new string[] { "new_area", "new_cargo", "new_cpf_sem_mascara", "new_escolaridade",
                "new_lojaid", "new_numero_endereco_principal", "new_observacao", "new_paisid",
                "new_ramal_comercial2", "new_ramal_fax", "new_ramal_telefone", "new_rg", "new_ufid", "address1_line1", "address1_line2",
                "address1_line3", "fullname", "customertypecode"});

            var q1 = query.AddLink("new_cidade", "new_cidadeid", "new_cidadeid", JoinOperator.LeftOuter);
            var q2 = q1.AddLink("itbc_municipios", "new_codigo_ibge", "itbc_codigo_ibge", JoinOperator.LeftOuter);
            q2.Columns.AddColumn("itbc_municipiosid");
            q2.EntityAlias = "a";
            var q3 = q2.AddLink("itbc_estado", "itbc_estadoid", "itbc_estadoid", JoinOperator.LeftOuter);
            q3.Columns.AddColumn("itbc_estadoid");
            q3.EntityAlias = "b";
            var q4 = q3.AddLink("itbc_pais", "itbc_pais", "itbc_paisid", JoinOperator.LeftOuter);
            q4.Columns.AddColumn("itbc_paisid");
            q4.EntityAlias = "c";

            while (true)
            {               

                var colecao = base.Provider.RetrieveMultiple(query);

                foreach (var item in colecao.Entities)
                {
                    var obj = new Contato(this.OrganizationName, this.IsOffline);
                    obj.Id = item.Id;                    
                    if (item.Contains("new_area"))
                    {
                        switch (((OptionSetValue)item["new_area"]).Value)
                        {
                            case 1: obj.Area = 993520000; break;
                            case 2: obj.Area = 993520001; break;
                            case 3: obj.Area = 993520002; break;
                            case 4: obj.Area = 993520003; break;
                            case 5: obj.Area = 993520004; break;
                            case 6: obj.Area = 993520005; break;
                            case 7: obj.Area = 993520006; break;
                            case 8: obj.Area = 993520007; break;
                            case 9: obj.Area = 993520010; break;
                            case 10: obj.Area = 993520011; break;
                            case 11: obj.Area = 993520012; break;
                            case 12: obj.Area = 993520008; break;
                            case 13: obj.Area = 993520009; break;
                            case 14: obj.Area = 993520013; break;
                            case 15: obj.Area = 993520015; break;
                            case 99: obj.Area = 993520016; break;
                        }
                    }
                    if (item.Contains("new_cargo"))
                    {
                        switch (((OptionSetValue)item["new_cargo"]).Value)
                        {
                            case 1: obj.Cargo = 993520000; break;
                            case 2: obj.Cargo = 993520001; break;
                            case 3: obj.Cargo = 993520002; break;
                            case 4: obj.Cargo = 993520003; break;
                            case 5: obj.Cargo = 993520004; break;
                            case 6: obj.Cargo = 993520005; break;
                            case 7: obj.Cargo = 993520006; break;
                            case 8: obj.Cargo = 993520007; break;
                            case 9: obj.Cargo = 993520008; break;
                            case 10: obj.Cargo = 993520009; break;
                            case 11: obj.Cargo = 993520010; break;
                            case 12: obj.Cargo = 993520011; break;
                            case 13: obj.Cargo = 993520012; break;
                            case 14: obj.Cargo = 993520014; break;
                            case 15: obj.Cargo = 993520015; break;
                            case 16: obj.Cargo = 993520016; break;
                            case 17: obj.Cargo = 993520017; break;
                            case 18: obj.Cargo = 993520018; break;
                            case 19: obj.Cargo = 993520019; break;
                            case 20: obj.Cargo = 993520020; break;
                            case 21: obj.Cargo = 993520021; break;
                            case 22: obj.Cargo = 993520023; break;
                            case 23: obj.Cargo = 993520013; break;
                            case 99: obj.Cargo = 993520022; break;

                        }
                    }
                    if (item.Contains("new_cpf_sem_mascara"))
                        obj.CpfCnpj = (string)item["new_cpf_sem_mascara"];
                    if (item.Contains("new_escolaridade"))
                    {
                        switch (((OptionSetValue)item["new_escolaridade"]).Value)
                        {
                            case 1: obj.Escolaridade = 993520000; break;
                            case 2: obj.Escolaridade = 993520001; break;
                            case 3: obj.Escolaridade = 993520002; break;
                            case 4: obj.Escolaridade = 993520003; break;
                            case 5: obj.Escolaridade = 993520004; break;
                            case 6: obj.Escolaridade = 993520005; break;
                            case 7: obj.Escolaridade = 993520006; break;
                            case 8: obj.Escolaridade = 993520007; break;
                            case 9: obj.Escolaridade = 993520008; break;
                            case 10: obj.Escolaridade = 993520009; break;
                        }
                    }
                    if (item.Contains("new_lojaid"))
                        obj.Loja = new Lookup(new Guid(((AliasedValue)item["new_lojaid"]).Value.ToString()), "account");
                    if (item.Contains("new_numero_endereco_principal"))
                        obj.Endereco1Numero = (string)item["new_numero_endereco_principal"];
                    if (item.Contains("new_observacao"))
                        obj.Descricao = (string)item["new_observacao"];
                    if (item.Contains("new_ramal_comercial2"))
                        obj.Ramal2 = (string)item["new_ramal_comercial2"];
                    if (item.Contains("new_ramal_fax"))
                        obj.RamalFax = (string)item["new_ramal_fax"];
                    if (item.Contains("new_ramal_telefone"))
                        obj.Ramal1 = (string)item["new_ramal_telefone"];
                    if (item.Contains("new_rg"))
                        obj.DocIdentidade = (string)item["new_rg"];
                    if (item.Contains("address1_line1"))
                        obj.Endereco1Rua = (string)item["address1_line1"];
                    if (item.Contains("address1_line2"))
                        obj.Endereco1Complemento = (string)item["address1_line2"];
                    if (item.Contains("address1_line3"))
                        obj.Endereco1Bairro = (string)item["address1_line3"];
                    if (item.Contains("a.itbc_municipiosid"))
                    {
                        obj.Endereco1Municipioid = new Lookup(new Guid(((AliasedValue)item["a.itbc_municipiosid"]).Value.ToString()), "itbc_municipios");
                    }
                    if (item.Contains("b.itbc_estadoid"))
                    {
                        obj.Endereco1Estadoid = new Lookup(new Guid(((AliasedValue)item["b.itbc_estadoid"]).Value.ToString()), "itbc_estado");
                    }
                    if (item.Contains("c.itbc_paisid"))
                    {
                        obj.Endereco1Pais = new Lookup(new Guid(((AliasedValue)item["c.itbc_paisid"]).Value.ToString()), "itbc_pais");
                    }
                    if (item.Contains("fullname") && ((string)item["fullname"]).Contains("NFE"))
                    {
                        obj.ContatoNFE = 993520000;
                    }
                    else
                    {
                        obj.ContatoNFE = 993520001;
                    }
                    if(!item.Contains("customertypecode"))
                        obj.TipoRelacao = 993520006;

                    obj.IntegrarNoPlugin = false;


                    var ent = EntityConvert.Convert(obj, this.OrganizationName, this.IsOffline);
                    this.Provider.Update(ent);
                }
                if (colecao.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = colecao.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        public void AtualizarEndereco()
        {
            var query = new QueryExpression("customeraddress");
            query.PageInfo = new PagingInfo();
            query.PageInfo.PageNumber = 1;
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;

            query.ColumnSet.AddColumns(new string[] { "line2", "line3", "new_numero_endereco","objecttypecode", "new_numero_endereco" });
            query.Criteria.AddCondition(new ConditionExpression("line2", ConditionOperator.NotNull));
            query.Criteria.AddCondition(new ConditionExpression("line3", ConditionOperator.NotNull));

            while (true)
            {
                var colecao = base.Provider.RetrieveMultiple(query);
                foreach (var item in colecao.Entities)
                {
                    if ((string)item["objecttypecode"] == "contact" || ((string)item["objecttypecode"] == "account" && item.Contains("new_numero_endereco")))
                    {
                        var obj = new Endereco(this.OrganizationName, this.IsOffline);
                        obj.Id = item.Id;
                        obj.Complemento = (string)item["line2"];
                        obj.Bairro = (string)item["line3"];

                        var ent = EntityConvert.Convert(obj, this.OrganizationName, this.IsOffline);
                        this.Provider.Update(ent);
                    }
                }
                if (colecao.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = colecao.PagingCookie;
                }
                else
                {
                    break;
                }
            }
        }

        public void AtualizarProduto()
        {
            var query = new QueryExpression("customeraddress");
            query.ColumnSet.AddColumns(new string[] { "line2", "line3", "new_numero_endereco", "objecttypecode", "new_numero_endereco" });
            query.Criteria.AddCondition(new ConditionExpression("line2", ConditionOperator.NotNull));
            query.Criteria.AddCondition(new ConditionExpression("line3", ConditionOperator.NotNull));

            var colecao = base.Provider.RetrieveMultiple(query);
            var c = colecao.Entities.Count;

            foreach (var item in colecao.Entities)
            {
                if ((string)item["objecttypecode"] == "contact" || ((string)item["objecttypecode"] == "account" && item.Contains("new_numero_endereco")))
                {
                    var obj = new Endereco(this.OrganizationName, this.IsOffline);
                    obj.Id = item.Id;
                    obj.Complemento = (string)item["line2"];
                    obj.Bairro = (string)item["line3"];

                    var ent = EntityConvert.Convert(obj, this.OrganizationName, this.IsOffline);
                    this.Provider.Update(ent);
                }
            }
        }


    }
}
