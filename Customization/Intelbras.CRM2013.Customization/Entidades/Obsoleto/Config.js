if (typeof (Config) == "undefined") { Config = {}; }

//Configuração da url do serviço
var url = window.location.host;
var URLServico = "";

if (url == "crm2015dev.intelbras.com.br")
    URLServico = "https://integracrmd.intelbras.com.br/";

else if (url == "crm2015h.intelbras.com.br")
    URLServico = "https://integracrmh.intelbras.com.br/";

else if (url == "crm2015.intelbras.com.br")
    URLServico = "https://integracrm.intelbras.com.br/";

Config = {

    Entidade: {

        BeneficioPrograma: {
            VMC: { id: 0, name: "VMC", code: "21" },
            ShowRoom: { id: 1, name: "Show Room", code: "15" },
            PriceProtection: { id: 2, name: "Price Protection", code: "8" },
            Backup: { id: 3, name: "Backup", code: "4" }
        },

        FormaPagamento: {
            Dinheiro: { id: 0, name: "Dinheiro", code: "" },
            Produto: { id: 1, name: "Produto", code: "" },
            DescontoDuplicata: { id: 2, name: "Desconto em Duplicata", code: "" }
        },

        SolicitacaoBeneficio: {
            Status: {
                Criada: 993520005,
                EmAnalise: 993520000,
                Aprovada: 993520001,
                ComprovantesEmValidacao: 993520011,
                ComprovacaoConcluida: 993520010
            },
            TipoPriceProtection: {
                Autorizacao: 993520000,
                Consumo: 993520001
            },
            StatusCalculoPriceProtection: {
                NaoCalculado: 993520000,
                Calcular: 993520001,
                Calculando: 993520002,
                ErroCalcular: 993520003,
                Calculado: 993520004
            }
        },

        PapelNoProcesso: {
            Solicitante: { id: 0, name: "Solicitante", code: "1" },
            Parecer: { id: 1, name: "Parecer", code: "2" },
            Informado: { id: 2, name: "Informado", code: "3" },
            Aprovador: { id: 3, name: "Aprovador", code: "4" },
            ResponsavelPeloPagamento: { id: 4, name: "Responsável pelo Pagamento", code: "5" },
            ResponsavelPelaExecucao: { id: 5, name: "Responsável pela Execução", code: "6" }
        },

        TipoDeAtividade: {
            ParecerSolicitacoes: { id: 0, name: "Parecer de Solicitações", code: "1" },
            ExecucaoSolicitacoes: { id: 1, name: "Execução de Solicitação", code: "2" },
            Checklist: { id: 2, name: "Checklist", code: "3" },
            AprovacaoSolicitacao: { id: 3, name: "Aprovação de Solicitações", code: "4" },
            PendenciaDoCanal: { id: 4, name: "Pendências do Canal", code: "5" },
            AutorizarPagamento: { id: 5, name: "Autorizar Pagamento", code: "6" },
            AtuacaoOcorrencia: { id: 7, name: "Atuação na Ocorrência", code: "7" }
        },

        TipoAcessoExtranet: {
            Canal: { id: "4C77DB98-70ED-E311-9407-00155D013D38", name: "Canal", code: "" }
        },

        Tarefa: {
            Resultado: {
                Favoravel: { name: "Favorável", code: "993520000" },
                Desfavoravel: { name: "Desfavorável", code: "993520001" },
                Aprovada: { name: "Aprovada", code: "993520002" },
                Reprovada: { name: "Reprovada", code: "993520003" },
                PagamentoEfetuadoPedidoGeradoSolicitacaoAtendida: { name: "Pagamento Efetuado/Pedido Gerado/Solicitação Atendida", code: "993520004" },
                PagamentoNaoEfetuadoNaoSeraGeradoPedido: { name: "Pagamento Não Efetuado/Não será gerado Pedido", code: "993520005" },
                ComprovantesValidados: { name: "Comprovantes Validados", code: "993520006" },
                RetornoFinanceiroValidado: { name: "Retorno Financeiro Validado", code: "993520007" },
                PagamentoAutorizado: { name: "Pagamento Autorizado", code: "993520008" }
            }
        }
    },

    ParametroGlobal: {
        FatorConversaoValorSolicitacaoBeneficioProduto: { id: 0, name: "Fator Conversão Valor Solicitado Benefício - Produto", code: 68 },
        FatorConversaoValorSolicitacaoBeneficioDinheiro: { id: 0, name: "Fator Conversão Valor Solicitado Benefício - Dinheiro", code: 68 },
        FatorConversaoValorSolicitacaoBeneficioDescontoEmDuplicata: { id: 0, name: "Fator Conversão Valor Solicitado Benefício - Desconto em Duplicata", code: 70 },
        IntegrationWS: {
            CRM: {
                CrmWebServices: URLServico + "CRM/CrmWebServices.asmx",
                CrmWebApoioFormulario: URLServico + "CRM/CrmWebApoioFormulario.asmx"
            },
            IntegradorASTEC: URLServico + "IntegradorASTEC/IntegradorASTEC.asmx",
            IntegradorERP: {
                ERPService: URLServico + "IntegradorERP/ERPService.asmx"
            },
            Intelbras: {
                Intelbras: URLServico + "Intelbras/Intelbras.asmx",
                WSOcorrencia: URLServico + "Intelbras/WSOcorrencia.asmx"
            },
            PortalB2BEMS: {
                Cliente: URLServico + "PortalB2BEMS/Cliente.asmx",
                Cota: URLServico + "PortalB2BEMS/Cota.asmx",
                CurvaABC: URLServico + "PortalB2BEMS/CurvaABC.asmx",
                Devolucao: URLServico + "PortalB2BEMS/Devolucao.asmx",
                Faturamento: URLServico + "PortalB2BEMS/Faturamento.asmx",
                NotaFiscal: URLServico + "PortalB2BEMS/NotaFiscal.asmx",
                Pedido: URLServico + "PortalB2BEMS/Pedido.asmx",
                Preco: URLServico + "PortalB2BEMS/Preco.asmx",
                Ranking: URLServico + "PortalB2BEMS/Ranking.asmx",
                Titulo: URLServico + "PortalB2BEMS/Titulo.asmx"
            },
            User: {
                ConfigSistemas: URLServico + "User/ConfigSistemas.asmx",
                UserService: URLServico + "User/UserService.asmx"
            },
            Vendas: {
                Apoio: URLServico + "Vendas/Apoio.asmx",
                Isolservice: URLServico + "Vendas/isolservice.asmx"
            }
        }
    }
}