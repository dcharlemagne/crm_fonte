using System.ComponentModel;

namespace Intelbras.CRM2013.Domain.Enum
{
    public enum ClassificacaoLog
    {
        PortalAssistenciaTecnica = 5000,
        PortalOcorrenciasIsol = 5001,
        PortalTreinamentoAlunos = 5002,
        PortalTreinamentoInstrutos = 5003,
        PluginNew_diagnostico_ocorrencia = 5050,
        PluginIncident = 5051,
        PluginEmail = 5052,
        PluginContact = 5053,
        PluginNew_lancamento_avulso = 5054,
        PluginNew_calendario_participante = 5055,
        PluginNew_resgate_premio_fidelidade = 5056,
        PluginNew_pontos_participante_fidelidade = 5057,
        PluginIntegradorERP = 5058,
        PluginNew_valor_servico_posto = 5059,
        PluginNew_calendario_custos = 5060,
        PluginNew_extrato_logistica_reversa = 5061,
        PluginCustomeraddress = 5062,
        PluginContractLine = 5063,
        PluginAccount = 5064,
        Plugin = 5065,
        PaginaOcorrencia = 5300,
        WSIntelbras = 5400,
        WSVendasApoio = 5401,
        WSVendasIsol = 5402,
        WSIntegradorASTEC = 5403,
        WSIntegradorERP = 5404,
        WSCardTreinamentoService = 5405,
        WSIntelbras_WSOcorrencia = 5406,
        WSUsersService_UserService = 5407,
        ServicoASTEC = 5500,
        ServicoSiteIntelbras = 5501,
        ServicoCustomCRM = 5502,
        PortalB2B = 5504,
        PortalHelper = 5505,
        PortalFidelidade = 5506,
    }

    public enum IntegrarASTEC
    {
        Sim = 993520000,
        Nao = 993520001
    }


    public class Plugin
    {
        public enum IntelbrasMensagem
        {
            PreEvent = 10,
            PostEvent = 50,

        }

        public enum MessageName
        {
            Create,
            Update,
            Delete,
            Send,
            SetStateDynamicEntity,
            Associate,
            DeliverIncoming,
            AssociateEntities,
            DisassociateEntities
        }

        public enum Stage
        {
            PreEvent = 10,
            PostEvent = 50
        }

        public enum Keys
        {
            Target
        }
    }

    public class Cliente
    {
        public enum Perfil
        {
            //Revenda_Autorizada = 1,
            Revenda_Corporativa = 2,
            ISP = 3,
            Instalador = 4,
            MSP = 5,
            Integrador = 6
        }

        public enum Grupo
        {
            Distribuidor = 5
        }

        public enum IntegracaoRevendaSite
        {
            AtualizarNoSite = 1,
            AtualizadoComSucesso = 2,
            ExcluirNoSite = 3,
            ExcluidoComSucesso = 4,
            ErroParaAtualizar = 5,
            ErroParaExcluir = 6
        }
    }

    public class ImportacaoAssistenciaTecnica
    {
        public enum RazaoStatus
        {
            AProcessar = 1,
            Processado_com_Erro = 3,
            Processado_com_Sucesso = 4
        }
    }

    public class PoliticaComercial
    {
        public enum PoliticaEspecifica
        {
            Sim = 993520000,
            Nao = 993520001
        }

        public enum AplicarPolíticaPara
        {
            PerfilDeCanais = 993520000,
            ConjuntoDeCanais = 993520001
        }

        public enum TipoDePolitica
        {
            Normal = 993520000,
            CrossSelling = 993520001
        }

        public enum Status
        {
            Ativo = 0,
            Inativo = 1
        }
        public enum RazaoStatus
        {
            Rascunho = 993520000
        }
    }
    public class Denuncia
    {
        public enum AcaoTomada
        {
            AlertaCanal = 993520000,
            PerdaBeneficio = 993520001,
            Descredenciamento = 993520002
        }
        public enum StatusDenuncia
        {
            EmAnalise = 993520000,
            AguardandoJustificativa = 993520001,
            JustificativaProvida = 993520002,
            AnaliseJustificativa = 993520003,
            DenunciaProcedente = 993520004,
            DenunciaImprocedente = 993520005,
        }
        public enum TipoDenunciante
        {
            Anonimo = 993520000,
            ColaboradordoCanal = 993520001,
            ColaboradorIntelbras = 993520002,
            KeyAccountRepresentante = 993520003
        }
        public enum Status
        {
            Ativo = 0
        }

    }

    public class TipoDeDenuncia
    {
        public enum status
        {
            Ativo = 0
        }
    }

    public class Contato
    {
        public enum Natureza
        {
            PessoaFisica = 993520003,
            PessoaJuridica = 993520000,
            Estrangeiro = 993520001,
            Trading = 993520002
        }

        public enum Cargo
        {
            AnalistadeMarketing = 993520023,
            Administrador = 993520000,
            Analista = 993520001,
            Assistente = 993520002,
            Atendente = 993520003,
            AuxiliardeProducao = 993520004,
            Comprador = 993520005,
            Consultor = 993520006,
            Contador = 993520007,
            Coordenador = 993520008,
            Diretor = 993520009,
            Engenheiro = 993520010,
            Gerente = 993520011,
            Presidente = 993520012,
            PreVenda = 993520013,
            Promotor = 993520014,
            ProprietarioSocio = 993520015,
            Representante = 993520016,
            Secretaria = 993520017,
            Supervisor = 993520018,
            Tecnico = 993520019,
            Vendedor = 993520020,
            VicePresidente = 993520021,
            AClassificar = 993520022
        }

        public enum TipoRelacao
        {
            KeyAccount = 993520007,
            ColaboradorDoCanal = 993520008,
            ClienteFinal = 1,
            ColaboradorIntelbras = 993520000,
            Outro = 993520006
        }

        public enum Escolaridade
        {
            Primeiro_Grau_Incompleto = 993520000,
            Primeiro_Grau_Completo = 993520001,
            Segundo_Grau_Incompleto = 993520002,
            Segundo_Grau_Completo = 993520003,
            Superior_Incompleto = 993520004,
            Superior_Completo = 993520005,
            MBA = 993520006,
            Mestrado = 993520007,
            Doutorado = 993520008,
            PhD = 993520009
        }

        public enum PapelNoCanal
        {
            TecnicoAutorizado = 993520000,
            TecnicoAutonomo = 993520001,
            TecnicoLAI = 993520002,
            TecnicoDistribuidor = 993520003,
            Outros = 993520004,
            RH = 993520005,
            Trade = 993520006,
            PosVendas = 993520007,
            Instrutores = 993520008,
            Representante = 993520009,
            Revenda = 993520010,
            TecnicoTI = 993520011,
            ProvedorInternet = 993520012,
            TecnicoEstGov = 993520013

        }

        public enum AreaAtuacao
        {
            Seguranca = 993520000,
            Telecom = 993520001,
            ControleAcesso = 993520002,
            IncendioIluminacao = 993520003,
            Redes = 993520004,
            Outros = 993520005

        }

        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }

        public enum TipoAcesso
        {
            Distribuidor, Revenda, Vendedor, NaoDefinido
        }
        public enum Sexo
        {
            Masculino = 1,
            Feminino = 2
        }

    }

    public class ExtratoFidelidade
    {
        public enum RazaoDoStatus
        {
            Ativo = 1,
            Inativo = 2,
            Cancelado = 3,
            Expirado = 4
        }
    }

    public class Resgate
    {
        public enum RazaoDoStatus
        {
            Finalizado = 1,
            Inativo = 2,
            Andamento = 3,
            Pendente = 4,
            Cancelado = 5
        }

        public enum Tipo
        {
            Intelbras,
            Walmart,
            Sistema,
            NaoInformado
        }
    }

    public class ProdutoResgatadoFidelidade
    {
        public enum RazaoDoStatus
        {
            Ativo = 1,
            CanceladoPeloSistema = 2
        }
    }

    public class Tarefa
    {
        public enum Resultado
        {
            Favoravel = 993520000,
            Desfavoravel = 993520001,
            Aprovada = 993520002,
            Reprovada = 993520003,
            PagamentoEfetuadoPedidoGerado = 993520004,
            PagamentoNaoEfetuadoNaoSeraGeradoPedido = 993520005,
            ComprovantesValidados = 993520006,
            RetornoFinanceiroValidado = 993520007,
            PagamentoAutorizado = 993520008,
            PagamentoNaoAutorizado = 993520009
        }

        public enum StateCode
        {
            Ativo = 0,
            Fechada = 1,
            Cancelada = 2
        }

        public enum StatusCode
        {
            NaoIniciada = 2,
            EmProgresso = 3,
            Aguardando = 4,
            Concluida = 5,
            Cancelada = 6,
            Diferido = 7
        }

        public enum Prioridade
        {
            Baixa = 0,
            Alta = 2,
            Normal = 1
        }

    }


    public class ProdutoPedido
    {
        public enum Status
        {
            Aberto = 993520000,
            AtendidoParcial = 993520001,
            AtendidoTotal = 993520002,
            Pendente = 993520003,
            Suspenso = 993520004,
            Cancelado = 993520005,
            FaturaBalcao = 993520006
        }
    }
    #region Parecer
    public class Parecer
    {
        public string pegarNomeEnum(int codigo)
        {
            if (codigo == 993520000)
                return "Sim";
            else if (codigo == 993520001)
                return "Não";
            else
                return "N/A";
        }
        public enum DispoeSuporteTecnico
        {
            Sim = 993520000,
            Nao = 993520001
        }

        public enum AprovadoRedir
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum AtuaVendasClientesFinais
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum CompetenciaTecnica
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum EnviaSellOut
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum CondicoesAtendemPadraoMinimo
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum PossuiSistemaGestaoFinanc
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum EnviouDocCompleta
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum ExclusivoProdutosIntelbras
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum ParecerKARepresentante
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum AprovadoComite
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum FichaAvalicaoDistribuidor
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum DispostoAtuarNovasPraticas
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum ParecerSetorFinanc
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum ParecerGerenteNacVendas
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum DistribuidorAprovado
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum VendaSemNotaFiscal
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum DistribuidorAdequado
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum Experiencia5AnosDistri
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum TipoParecer
        {
            KeyAccountouRepresentante = 993520000,
            GerentedeVendas = 993520001,
            Financeiro = 993520002,
            ComitedeCanaiseREDIR = 993520003,
            Normal = 993520004
        }
        public string TipoParecerNome(int tipoParecer)
        {
            switch (tipoParecer)
            {
                case 993520000:
                    return "Key account ou Representante";
                case 993520001:
                    return "Gerente de Vendas";
                case 993520002:
                    return "Financeiro";
                case 993520003:
                    return "Comitê de Canais e REDIR";
                default:
                    return "N/A";
            }
        }
    }
    #endregion

    public class Pedido
    {
        public enum OrigemPedido
        {
            Normal = 993520010,
            TelePedido = 993520011,
            Configurado = 993520012,
            Batch = 993520013,
            Exportacao = 993520014,
            EDI = 993520015,
            Multiplanta = 993520000,
            Cotacao = 993520001,
            Bonificacao = 993520002,
            PortalDatasul = 993520003,
            SFA = 993520004,
            Web = 993520005,
            B2B = 993520006,
            CRM = 993520007,
            CRMModificadoERP = 993520008,
            EAI = 993520009
        }

        public enum TipoPreco
        {

            Informado = 993520000,
            TabelaImplantacao = 993520001,
            TabelaFaturamento = 993520002
        }

        public enum TipoRelacao
        {
            KeyAccount = 993520007,
            ColaboradorDoCanal = 993520008,
            ClienteFinal = 1,
            ColaboradorIntelbras = 993520000,
            Outro = 993520006
        }

        public enum CondicoesFrete
        {
            FOB = 1,
            FreteGratuito = 2
        }

        public enum DestinoMercadoria
        {
            ComercioIndustria = 993520000,
            ConsProprioAtivo = 993520001
        }

        public enum SituacaoAlocacao
        {
            NaoAlocado = 993520000,
            AlocadoParcial = 993520001,
            AlocadoTotal = 993520002
        }

        public enum Modalidade
        {
            CbSimples = 993520000,
            Desconto = 993520001,
            Caucao = 993520002,
            Judicial = 993520003,
            Repres = 993520004,
            Carteira = 993520005,
            Vendor = 993520006,
            Cheque = 993520007,
            NotaPromissoria = 993520008
        }

        public enum SituacaoAvaliacao
        {
            NaoAvaliado = 993520000,
            Avaliado = 993520001,
            Aprovado = 993520002,
            NaoAprovado = 993520003,
            PendenteInformacao = 993520004
        }

        public enum RazaoStatus
        {
            Aberto = 1,
            Pendente = 2,
            Suspenso = 993520000,
            AtendidoParcial = 993520001,
            AtendidoTotal = 993520002,
            Cancelado = 4,
            AtendidoTotalCumprido = 100001
        }

        public enum StateCode
        {
            Ativa = 0,
            Submitted = 1,
            Cancelada = 2,
            Cumprido = 3,
            Faturado = 4
        }

        public enum TipoUsuarioCriacao
        {
            KeyAccountRepresentante = 993520000,
            Canal = 993520001,
            Totvs = 993520002,
            B2B = 993520003,
            Outros = 993520004
        }
    }


    public class Pefil
    {
        public enum Status
        {
            Pendente = 993520000,
            Configurado = 993520001
        }
    }
    public class Conta
    {
        public enum OrigemConta
        {
            Extranet = 993520000,
            SellOut = 993520001,
            ITEC = 993520002,
            CallCenter = 993520003,
            Canais = 993520004,
            Outros = 993520005
        }

        public enum ApuracaoDeBeneficiosECompromissos
        {
            Centralizada_Na_Matriz = 993520000,
            Por_Filiais = 993520001
        }

        public enum CapitalOuInterior
        {
            Capital = 993520000,
            Interior = 993520001
        }
        public enum MatrizOuFilial
        {
            Matriz = 993520000,
            Filial = 993520001
        }
        public enum PossuiFiliais
        {
            Sim = 993520000,
            Nao = 993520001
        }
        public enum TipoConstituicao
        {
            Cpf = 993520000,
            Cnpj = 993520001,
            Estrangeiro = 993520002
        }
        public enum ParticipaDoPrograma
        {
            [DescriptionAttribute("Sim")]
            Sim = 993520001,

            [DescriptionAttribute("Não")]
            Nao = 993520000,

            [DescriptionAttribute("Descredenciado")]
            Descredenciado = 993520002,

            [DescriptionAttribute("Pendente")]
            Pendente = 993520003
        }

        public enum StatusEnvioVMC
        {
            ErroAoEnviar = 993520000,
            Enviado = 993520001,
            AguardandoEnvio = 993520002,
            Enviando = 993520003
        }

        public class Classificacao
        {
            public const string ContaNomeada = "Contas Nomeadas";
            public const string Dist_BoxMover = "Distribuidor Box Mover";
            public const string Dist_VAD = "Distribuidor VAD";
            public const string Rev_Trans = "Revenda Transacional";
            public const string Rev_Rel = "Revenda Relacional";
            public const string Revendas = "Revendas";
            public const string Atac_Dist = "Atacado Distribuidor";
            public const string Provedores = "Provedores";
            public const string Rev_Sol = "Revenda Soluções";
        }

        public class SubClassificacao
        {
            public const string Rev_Integrador = "Integrador";
            public const string Rev_Bancaria = "Revenda Bancária";
            public const string Rev_Incendio = "Revenda Incêndio";
            public const string Rev_Monitoramento = "Revenda Monitoramento";
            public const string Rev_Telecom = "Revenda Telecom";
            public const string Rev_Revendas = "Revendas";
            public const string Atac_Distribuidor = "Atacado Distribuidor";
            public const string Provedores = "Provedores";
        }

        public class CategoriaConta
        {
            public const string Completo = "Completo";
            public const string Rev_Sol = "Revenda Soluções";
            public const string Provedores = "Provedores";
            public const string Ouro = "Ouro";
            public const string Prata = "Prata";
            public const string Bronze = "Bronze";
        }

        public enum Tipoendereco2
        {
            Primario = 1,
            Cobranca = 993520000

        }
        public enum Tipoendereco
        {
            Fatura = 1,
            Remessa = 2,
            Primario = 3,
            Outro = 4
        }

        public enum StatusIntegracaoSefaz
        {
            NaoValidado = 993520000,
            Validado = 993520001,
            IntegracaoIndisponivel = 993520002,
            CnpjNaoEncontrado = 993520003,
            ValidadoManualmente = 993520004
        }

        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }

        public enum StatusCode
        {
            Ativo = 1,
            Inativo = 2
        }

        public enum TipoRelacao
        {
            Canal = 993520000,
            EmpresaRepresentante = 993520002,
            ClienteFinal = 993520001,
            ContaNomeada = 993520003,
            Fornecedor = 993520004,
            Outro = 12
        }

        public enum Categorizacao
        {
            NaoCategorizada = 993520000,
            Categorizar = 993520001,
            Recategorizar = 993520002,
            Categorizada = 993520003,
            RecategorizarMensal = 993520004,
        }

        public enum PerfilASTEC
        {
            LAI = 993520001,
            AssistenciaPremium = 993520002
        }
    }

    public class Fatura
    {
        public enum RazaoStatus
        {
            EntregaRealizadaNormalmente = 1,
            EntregaForaDaDataProgramada = 2,
            DevolucaoNaoAutorizadaPeloCliente = 4,
            MercadoriaDevolvidaAoClienteOrigem = 5,
            ClienteRetiraMercadoriaNaTransportadora = 6,
            EntregaComIndenizacaoEfetuada = 993520000,
            EntregaRetiradaDiasNaoUteis = 993520001,
            AvariaTotal = 993520002,
            ExtravioTotal = 993520003,
            RouboCarga = 993520004,
            Cancelada = 100003
        }

        public enum Status
        {
            Ativa = 0,
            Cancelada = 3
        }

        public enum TipoNotaFiscal
        {
            Saida = 993520000,
            Entrada = 993520001,
            Servicos = 993520002
        }
    }

    public class Produto
    {
        public enum TipoProduto
        {
            Servicos = 3,
            Diversos = 9,
            Produtos = 11,
            Taxa = 12
        }

        public enum NaturezaProduto
        {
            Comprado = 993520000,
            Fabricado = 993520001
        }

        public enum StateCode
        {
            ativo = 0,
            inativo = 1
        }

        public enum StatusCode
        {
            [DescriptionAttribute("Ativo")]
            Ativo = 1,

            [DescriptionAttribute("Inativo")]
            Inativo = 2,

            [DescriptionAttribute("Descontinuado")]
            Descontinuado = 993520000
        }
    }

    public class Segmento
    {
        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class FamiliaProduto
    {
        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class SubFamiliaProduto
    {
        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class ParticipanteDoCanal
    {
        public enum PapelNoCanal
        {
            SupervisorDeVendas = 993520001,
            KeyAccountRepresentante = 993520000,
            GerenteNacionalGerenteDeDistribuicao = 993520002,
            DiretorDeUnidade = 993520003
        }
    }

    public class PortfolioKa
    {
        public enum State
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class ProdutoPoliticaComercial
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2,
            Descontinuado = 100000000
        }
    }

    public class ProdutoPortfolio
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2,
            Descontinuado = 100000000
        }
        public enum Tipo
        {
            BoxMover = 993520000,
            VAD = 993520001,
            Exclusivo = 993520002,
            CrossSelling = 993520003,
        }

        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class ProdutoSolicitacao
    {
        public enum Status
        {
            Ativo = 0,
            Inativo = 1
        }
        public enum StatusCode
        {
            Ativo = 1,
            Inativo = 2
        }
    }


    public class ProdutoTreinamento
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2,
            Descontinuado = 100000000
        }
        public enum BloqueiaPortfolio
        {
            Sim = 993520000,
            Nao = 993520001
        }
    }

    public class Portfolio
    {
        public enum Tipo
        {
            Normal = 993520005,
            BoxMover = 993520000,
            VAD = 993520001,
            Exclusivo = 993520002,
            CrossSelling = 993520003,
            Solucao = 993520004
        }

        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
    }
    public class SolicitacaoCadastro
    {
        public enum StatusSolicitacao
        {
            CanceladaOuReprovada = 993520003,
            Criada = 993520001,
            EmAnalise = 1,
            Aprovada = 993520000,
            Executada = 993520002,
            Inativo = 2
        }

        public enum Status
        {
            Ativo = 0,
            Inativo = 1
        }
    }
    public class SolicitacaoBeneficio
    {
        public enum StatusSolicitacaoBeneficio
        {
            Criada = 993520005,
            EmAnalise = 993520000,
            Aprovada = 993520001,
            NaoAprovada = 993520002,
            AguardandoComprovantes = 993520008,
            ComprovantesValidacao = 993520011,
            ComprovacaoConcluida = 993520010,
            AnaliseDeReembolso = 993520007,
            PagamentoPendente = 993520003,
            PagamentoEfetuado = 993520004,
            Cancelada = 993520006,
            AguardandoRetornoFinanceiro = 993520010,
            RetornoFinanceiroValidado = 993520009,
            AguardandoFinanceiroValidacao = 993520012
        }

        public enum State
        {
            Ativo = 0,
            Inativo = 1
        }

        public enum StatusCalculoPriceProtection
        {
            NaoCalculado = 993520000,
            Calcular = 993520001,
            Calculando = 993520002,
            ErroCalcular = 993520003,
            Calculado = 993520004
        }

        public enum RazaoStatusAtivo
        {
            EmAberto = 1,
            EmAnalise = 993520000,
            AprovadaParaReembolso = 993520001,
            ReembolsoPendente = 993520002,
            ReembolsoAutorizado = 993520003,
            Reembolsado = 993520004,
            NaoAprovada = 993520005
        }
        public enum RazaoStatusInativo
        {
            Cancelada = 2
        }

        public enum TipoPriceProtection
        {
            Autorizacao = 993520000,
            Consumo = 993520001
        }
        public const string StockRotation = "Stock Rotation";


        public enum StatusPagamento
        {
            NaoPago = 993520000,
            PagoParcial = 993520001,
            PagoTotal = 993520002
        }

        public enum FormaCancelamento
        {
            Automatico = 993520000
        }

        public class FormaPagamento
        {
            public const string Produto = "Produto";
            public const string DescontoDuplicata = "Desconto em Duplicata";
            public const string Dinheiro = "Dinheiro";
        }
    }
    public class ListaPsd
    {
        public enum State
        {
            Ativo = 0,
            Inativo = 1
        }
    }
    public class CompromissoPrograma
    {
        public enum Codigo
        {
            AbrirFiliaisComAprovacaoIntelbras = 1,
            AcordarMetasTrimestrais = 2,
            AderenciaPoliticaPosVenda = 3,
            AderenciaPSDCondicoesPagamento = 4,
            AderenciaRegrasUtilizacaoIdentidadevisual = 5,
            Adimplencia = 6,
            AtendimentoAdequadoIndicacaoLead = 7,
            AtendimentoAdequadoRegistroOportunidades = 8,
            ComercializarProdutosExigemCertificacaoParceirosCertificados = 9,
            EnvioDocumentacao = 10,
            Estoque45Dias = 11,
            EstruturaFisicaSuporte = 12,
            ForcaVendasCertificada = 13,
            InformarIntelbrasComercilizacaoNovoProdutoConcorrente = 14,
            InformarMudancasComposicaoSocietariaDaEmpresaDoParceiro = 15,
            LinhaCorteTrimestral = 16,
            MetaTrimestral = 17,
            NaoAtenderAContaNomeadaSemAvisoPrevio = 18,
            NaoComercializarProdutosCorporativosParaVarejo = 19,
            NaoComercializarProdutosParaConsumidorFinal = 20,
            NaoDirecionamentoOportunidadesConcorrentes = 21,
            NaoPraticarPrecoPredatorio = 22,
            NaoRepassarMercadoriasATerceirosNaoCredenciadosSubdistribuicao = 23,
            NaoTerSiteParaConsumidorFinal = 24,
            ParticipacaoCampanhasMotivacionaisEPremiacao = 25,
            PromoverEventosRelacionamento = 26,
            PromoverTreinamentosEstruturaMinima = 27,
            RelatorioPrevisaoVendasTrimestral = 28,
            RelatorioSellOutEstoque = 29,
            Showroom = 30,
            SociosDistribuidoraNaoPodemTerOutraEmpresaComoRevenda = 31,
            SuporteTecnicoCertificadoSuporteEProdutos = 32,
            TecnicoTreinadoCertificado = 33
        }

        public enum TipoMonitoramento
        {
            Automatico = 993520000,
            PorTarefas = 993520001,
            Manual = 993520002,
            Solicitacoes = 993520003
        }
    }
    public class CategoriaCanal
    {
        public enum StateCode
        {
            Ativado = 0,
            Desativado = 1
        }

        public enum StatusCode
        {
            Ativo = 1,
            Inativo = 2
        }
    }
    public class CompromissoCanal
    {
        public class StatusCompromisso
        {
            public const string Cumprido = "Cumprido";
            public const string Nao_Cumprido = "Não Cumprido";
            public const string Cumprido_fora_Prazo = "Cumprido fora do prazo";
            public const string Nao_Cumprido_dentro_Prazo = "Não Cumprido mas dentro do prazo de carência";
        }

        public class Compromisso
        {
            public const string RenovacaoContatos = "Renovação de Contatos";
            public const string Sellout = "Sell out";
            public const string Documentacao = "Envio de Documentação";
            public const string EnvioShowroom = "Envio de evidências de Showroom";
            public const string Showroom = "Showroom";

        }

        public enum Status
        {
            Ativo = 1,
            Desativado = 2
        }

        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class ListaPreco
    {
        public enum Tipo
        {
            PMA = 993520000,
            PrecoBase = 993520001
        }

        public enum State
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class ListaPrecoPSDPPPSCF
    {
        public enum StatusIntegracao
        {
            NaoIntegrado = 1,
            IntegracaoPendente = 993520000,
            Integrado = 993520001
        }
    }

    public class ListaProdutos
    {
        public enum Tipo
        {
            PMA = 1,
            Showroom = 2
        }
    }

    public class ReceitaPadrao
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2
        }
    }

    public class CondicaoPagamento
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2
        }
    }

    public class CategoriaB2B
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2
        }
    }

    public class RelacionamentoB2B
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2
        }
    }

    public class Endereco
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2
        }
    }

    public enum StateCode
    {
        Ativo = 0,
        Inativo = 1
    }

    public enum Status
    {
        Ativo = 1,
        Inativo = 2
    }
    public class Conexao
    {
        public enum Status
        {
            Ativo = 1,
            Inativo = 2
        }
    }
    public class ConfiguracaoBeneficio
    {
        public enum State
        {
            Inativo = 1,
            Ativo = 0
        }
    }
    public class FuncaoConexao
    {
        public enum Categoria
        {
            Comercial = 1,
            Familia = 2,
            Capital_Social = 3,
            Vendas = 4,
            Outro = 5,
            Participante = 1000,
            Equipe_Vendas = 1001,
            Servico = 1002
        }
    }

    public class HistoricoDistribuidor
    {
        public enum Statecode
        {
            Ativo = 0,
            Inativo = 1
        }

        public enum Statuscode
        {
            Ativo = 1,
            FluxoConluido = 993520000,
            Inativo = 2
        }
    }

    public class ColaboradorTreinamentoCertificado
    {
        public enum Status
        {
            Aprovado = 1,
            Reprovado = 993520000,
            Pendente = 993520001
        }
    }
    public class TreinamentoCanal
    {
        public enum State
        {
            Inativo = 1,
            Ativo = 0
        }

        public class StatusCompromisso
        {
            public const string Cumprido = "Cumprido";
            public const string Nao_Cumprido = "Não Cumprido";
            public const string Cumprido_fora_Prazo = "Cumprido fora do prazo";
        }
    }

    public class PotencialdoSupervisor
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoSupervisorporTrimestre
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoSupervisorporSegmento
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoSupervisorporFamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoSupervisorporSubFamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoSupervisorporProduto
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoSupervisorporProdutoMes
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoKaRepresentante
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoKaRepresentanteporTrimestre
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoKaRepresentanteporSegmento
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoKaRepresentanteporFamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoKaRepresentanteporSubfamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoKaRepresentanteporProduto
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class PotencialdoKaRepresentanteporProdutoMes
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetadoCanal
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetadoCanalporTrimestre
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetadoCanalporSegmento
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetadoCanalporFamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetadoCanalporSubfamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetadoCanalporProduto
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetadoCanalporProdutoMes
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetaUnidadeporTrimestre
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetaUnidadeporSegmento
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetaUnidadeporFamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetaUnidadeporSubfamilia
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetaUnidadeporProduto
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetaUnidadeporProdutoMes
    {
        public enum StatusCode
        {
            Ativa = 1,
            Rascunho = 993520000,
            EmProcessamento = 993520001,
            ErroProcessamento = 993520002
        }
    }

    public class MetaUnidade
    {
        public enum NivelMeta
        {
            [DescriptionAttribute("Resumido")]
            Resumido = 993520001,
            [DescriptionAttribute("Detalhado")]
            Detalhado = 993520000
        }

        public enum StatusMetaUnidade
        {
            [DescriptionAttribute("Gerar Modelo de Meta")]
            GerarModelodeMeta = 993520000,
            [DescriptionAttribute("Gerando Modelo de Meta")]
            GerandoModelodeMeta = 993520001,
            [DescriptionAttribute("Modelo de Meta Gerado com Sucesso")]
            ModelodeMetaGeradocomSucesso = 993520003,
            [DescriptionAttribute("Erro na Geração de Modelo de Meta")]
            ErroGeracaoModeloMeta = 993520002,
            [DescriptionAttribute("Ativo")]
            Ativo = 1,
            [DescriptionAttribute("Importar Planilha de Meta")]
            ImportarPlanilhaMeta = 993520004,
            [DescriptionAttribute("Importando Planilha de Meta")]
            ImportandoPlanilhaMeta = 993520006,
            [DescriptionAttribute("Planilha de Meta Importada com Sucesso")]
            PlanilhaMetaImportadaSucesso = 993520005,
            [DescriptionAttribute("Erro ao Importar Planilha de Meta")]
            ErroImportarPlanilhaMeta = 993520007
        }

        public enum RazaodoStatusMetaManual
        {
            [DescriptionAttribute("Gerar Meta do Canal Manual")]
            GerarMetaCanalManual = 993520000,
            [DescriptionAttribute("Gerando Meta do Canal Manual")]
            GerandoMetaCanalManual = 993520001,
            [DescriptionAttribute("Meta do Canal Manual Gerado com Sucesso")]
            MetaCanalManualGeradoSucesso = 993520002,
            [DescriptionAttribute("Erro ao Gerar Meta do Canal Manual")]
            ErroGerarMetaCanalManual = 993520003,
            [DescriptionAttribute("Importar Planilha Meta do Canal Manual")]
            ImportarPlanilhaMetaCanalManual = 993520004,
            [DescriptionAttribute("Importando Planilha Meta do Canal Manual")]
            ImportandoPlanilhaMetaCanalManual = 993520005,
            [DescriptionAttribute("Planilha Meta do Canal Manual Importada com Sucesso")]
            PlanilhaMetaCanalManualImportadaSucesso = 993520006,
            [DescriptionAttribute("Erro ao Importar Meta do Canal Manual")]
            ErroImportarMetaCanalManual = 993520007
        }

        public enum RazaodoStatusMetaKARepresentante
        {
            [DescriptionAttribute("Gerar Meta do KA/Representante")]
            GerarMetaKARepresentante = 993520000,
            [DescriptionAttribute("Gerando Meta do KA/Representante")]
            GerandoMetaKARepresentante = 993520001,
            [DescriptionAttribute("Meta do KA/Representante Gerado com Sucesso")]
            MetaKARepresentanteGeradoSucesso = 993520002,
            [DescriptionAttribute("Erro ao Gerar Meta do KA/Representante")]
            ErroGerarMetaKARepresentante = 993520003,
            [DescriptionAttribute("Importar Planilha Meta do KA/Representante")]
            ImportarPlanilhaMetaKARepresentante = 993520004,
            [DescriptionAttribute("Importando Planilha Meta do KA/Representante")]
            ImportandoPlanilhaMetaKARepresentante = 993520005,
            [DescriptionAttribute("Planilha Meta do KA/Representante Importada com Sucesso")]
            PlanilhaMetaKARepresentanteImportadaSucesso = 993520006,
            [DescriptionAttribute("Erro ao Importar Meta do KA/Representante")]
            ErroImportarMetaKARepresentante = 993520007
        }

        public enum RazaodoStatusMetaSupervisor
        {
            [DescriptionAttribute("Gerar Meta do Supervisor")]
            GerarMetaSupervisor = 993520000,
            [DescriptionAttribute("Gerando Meta do Supervisor")]
            GerandoMetaSupervisor = 993520001,
            [DescriptionAttribute("Meta do Supervisor Gerado com Sucesso")]
            MetaSupervisorGeradoSucesso = 993520002,
            [DescriptionAttribute("Erro ao Gerar Meta do Supervisor")]
            ErroGerarMetaSupervisor = 993520003,
            [DescriptionAttribute("Importar Planilha Meta do Supervisor")]
            ImportarPlanilhaMetaSupervisor = 993520004,
            [DescriptionAttribute("Importando Planilha Meta do Supervisor")]
            ImportandoPlanilhaMetaSupervisor = 993520005,
            [DescriptionAttribute("Planilha Meta do Supervisor Importada com Sucesso")]
            PlanilhaMetaSupervisorImportadaSucesso = 993520006,
            [DescriptionAttribute("Erro ao Importar Meta do Supervisor")]
            ErroImportarMetaSupervisor = 993520007
        }
    }
    public class ParametroGlobal
    {
        public enum Parametrizar
        {
            VisitaComercial = 993520007,
            GruposEstoqueGeracaoOrcamentosMeta = 42
        }
    }

    public enum TipoParametroGlobal
    {
        PadraoContrapartidaEmAcaoVMC = 4,
        QuantidadeKitsShowroomPorSegmento = 12,
        PrazoPermitidoNovaCompraShowroom = 13,
        PrazoLimiteSolicitacaoReembolsoVMC = 23,
        DataEnvioRegistrosSelloutAtacadoFielo = 36,
        EmailContratoVencimento = 39,
        ModeloPlanilhaMetaDetalhada = 40,
        ModeloPlanilhaMetaResumida = 45,
        ModeloPlanilhaMetaManualCanal = 46,
        ModeloPlanilhaMetaManualRepresentante = 47,
        ModeloPlanilhaMetaManualSupervisorVendas = 48,
        NumeroDiasCumprimentoCompromissosDenuncia = 49,
        DataEnvioRegistroSellinProvedoresSolucoesFielo = 61,
        AtividadesChecklist = 63,
        FrequenciaChecklist = 64,
        DatasTrimestre = 65,
        FatorConversaoValorSolicitadoDinheiro = 68,
        FatorConversaoValorSolicitadoProduto = 69,
        FatorConversaoValorSolicitadoBeneficioDescontoDuplicata = 70,
        CondicaoPagamentoTabelaPreco = 71,
        DataProximaReCategorizacaoMensalRevenda = 72,
        ContatosAdministrativos = 74,
        ValidadeDeSolicitacaoDeBeneficio = 75,
        PrazoLimiteParaSolicitarBeneficio = 76,
        CondicaoPagamento = 77,
        PercentualDescontoShowRoom = 78,
        PercentualDescontoBackup = 79,
        QuatidadeEvidenciaShowRoom = 80,
        CategoriaParaAdesaoRevendas = 82,
        DataPróximaCaterizaçãoRevenda = 84,
        DataPróximaReCaterizaçãoRevenda = 85,
        DataEnvioRegistroSelloutFielo = 86,
        MesesAEnviarParaFielo = 87,
        ValorVerbaVMCEnviadaParaFielo = 88,
        PrazoEmdiasResgatarVerbaFielo = 89,
        //ModeloPlanilhaMarcaRevendaRecategorizar = 86,
        DataHistoricoDistribuidorRevenda = 90,
        NumeroDiasParaCumprimento = 91,
        DataEnvioRegistroSellinFielo = 92,
        Feriados = 93,
        DataExecucaoValorPSD = 94,
        EmailGrupoIC = 95,
        EmailGrupoRotinasPCI = 96,
        QuantidadeDistribuidorPref = 97,
        EmailGrupoCRM = 98,
        NumeroMinimoColaboradoresTreinadosRevendaOuro = 99,
    }
    public class OrcamentodaUnidadeDetalhadoporProduto
    {
        public enum Trimestre1
        {
            #region valores
            [DescriptionAttribute("Janeiro")]
            Janeiro = 993520000,
            [DescriptionAttribute("Fevereiro")]
            Fevereiro = 993520001,
            [DescriptionAttribute("Março")]
            Marco = 993520002
            #endregion
        }

        public enum Trimestre2
        {
            #region valores
            [DescriptionAttribute("Abril")]
            Abril = 993520003,
            [DescriptionAttribute("Maio")]
            Maio = 993520004,
            [DescriptionAttribute("Junho")]
            Junho = 993520005
            #endregion
        }

        public enum Trimestre3
        {
            #region valores
            [DescriptionAttribute("Julho")]
            Julho = 993520006,
            [DescriptionAttribute("Agosto")]
            Agosto = 993520007,
            [DescriptionAttribute("Setembro")]
            Setembro = 993520008
            #endregion
        }

        public enum Trimestre4
        {
            #region valores
            [DescriptionAttribute("Outubro")]
            Outubro = 993520009,
            [DescriptionAttribute("Novembro")]
            Novembro = 993520010,
            [DescriptionAttribute("Dezembro")]
            Dezembro = 993520011
            #endregion
        }

        public enum Mes
        {
            [DescriptionAttribute("Janeiro")]
            Janeiro = 993520000,

            [DescriptionAttribute("Fevereiro")]
            Fevereiro = 993520001,

            [DescriptionAttribute("Março")]
            Marco = 993520002,

            [DescriptionAttribute("Abril")]
            Abril = 993520003,

            [DescriptionAttribute("Maio")]
            Maio = 993520004,

            [DescriptionAttribute("Junho")]
            Junho = 993520005,

            [DescriptionAttribute("Julho")]
            Julho = 993520006,

            [DescriptionAttribute("Agosto")]
            Agosto = 993520007,

            [DescriptionAttribute("Setembro")]
            Setembro = 993520008,
            [DescriptionAttribute("Outubro")]
            Outubro = 993520009,

            [DescriptionAttribute("Novembro")]
            Novembro = 993520010,

            [DescriptionAttribute("Dezembro")]
            Dezembro = 993520011
        }
    }

    public class OrcamentodaUnidade
    {
        public enum NivelOrcamento
        {
            [DescriptionAttribute("Resumido")]
            Resumido = 993520001,
            [DescriptionAttribute("Detalhado")]
            Detalhado = 993520000
        }

        public enum Trimestres
        {
            [DescriptionAttribute("1o Trimestre")]
            Trimestre1 = 993520000,
            [DescriptionAttribute("2o Trimestre")]
            Trimestre2 = 993520001,
            [DescriptionAttribute("3o Trimestre")]
            Trimestre3 = 993520002,
            [DescriptionAttribute("4o Trimestre")]
            Trimestre4 = 993520003
        }

        public enum StatusCodeOrcamento
        {
            [DescriptionAttribute("Gerar Modelo de Orçamento")]
            GerarModelodeOrcamento = 993520000,
            [DescriptionAttribute("Gerando Modelo de Orçamento")]
            GerandoModelodeOrcamento = 993520001,
            [DescriptionAttribute("Modelo de Orçamento Gerado com Sucesso")]
            ModelodeOrcamentoGeradocomSucesso = 993520003,
            [DescriptionAttribute("Erro na Geração de Modelo de Orçamento")]
            ErroGeracaoModeloOrcamento = 993520002,
            [DescriptionAttribute("Ativo")]
            Ativo = 1,
            [DescriptionAttribute("Importar Planilha de Orçamento")]
            ImportarPlanilhaOrcamento = 993520004,
            [DescriptionAttribute("Importando Planilha de Orçamento")]
            ImportandoPlanilhaOrcamento = 993520006,
            [DescriptionAttribute("Planilha de Orçamento Importada com Sucesso")]
            PlanilhaOrcamentoImportadaSucesso = 993520005,
            [DescriptionAttribute("Erro ao Importar Planilha de Orçamento")]
            ErroImportarPlanilhaOrcamento = 993520007
        }

        public enum RazaodoStatusOramentoManual
        {
            [DescriptionAttribute("Gerar Orçamento do Canal Manual")]
            GerarOrcamentoCanalManual = 993520000,
            [DescriptionAttribute("Gerando Orçamento do Canal Manual")]
            GerandoOrcamentoCanalManual = 993520001,
            [DescriptionAttribute("Orçamento do Canal Manual Gerado com Sucesso")]
            OrcamentoCanalManualGeradoSucesso = 993520002,
            [DescriptionAttribute("Erro ao Gerar Orçamento do Canal Manual")]
            ErroGerarOrcamentoCanalManual = 993520003,
            [DescriptionAttribute("Importar Planilha Orçamento do Canal Manual")]
            ImportarPlanilhaOrcamentoCanalManual = 993520004,
            [DescriptionAttribute("Importando Planilha Orçamento do Canal Manual")]
            ImportandoPlanilhaOrcamentoCanalManual = 993520005,
            [DescriptionAttribute("Planilha Orçamento do Canal Manual Importada com Sucesso")]
            PlanilhaOrcamentoCanalManualImportadaSucesso = 993520006,
            [DescriptionAttribute("Erro ao Importar Orçamento do Canal Manual")]
            ErroImportarOrcamentoCanalManual = 993520007
        }
    }
    public class ArquivoSellOut
    {
        public enum RazaoStatus
        {
            NaoProcessado = 1,
            Processando = 993520002,
            ProcessadoComplato = 993520000,
            ArquivoInvalido = 993520001,
            ProcessadoParcial = 993520003
        }
    }

    public class AcessoKonviva
    {
        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
        public enum StatusCode
        {
            Ativo = 1,
            Inativo = 2
        }
    }

    public class AcessoExtranetContatos
    {
        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
        public enum StatusCode
        {
            Ativo = 1,
            Inativo = 2
        }
    }

    public class BeneficioDoCanal
    {
        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
        public enum StatusCode
        {
            Ativo = 1,
            Inativo = 2
        }
    }
    public class BeneficiodoPrograma
    {
        public enum BeneficioAtivo
        {
            [DescriptionAttribute("Sim")]
            sim = 993520000,
            [DescriptionAttribute("Não")]
            Nao = 993520001
        }

        public enum ControleContaCorrente
        {
            [DescriptionAttribute("Sim")]
            Sim = 993520000,
            [DescriptionAttribute("Não")]
            Nao = 993520001

        }

        public enum TipoPriceProtection
        {
            Autorizacao = 993520000
        }

        public enum Codigos
        {
            ProgramaDeCertificacaoProfissionalIntelbras = 1,
            AcessoPortalRelacionamento = 2,
            AtendimentoDistânciaIntelbras = 0,
            AtendimentoKeyAccount = 3,
            Backup = 4,
            CertificacaoDeSuporteDistribuidorAutorizadoIntelbras = 0,
            IndicacaoDistribuidorAutorizadoIntelbras = 5,
            FerramentasCustomizaveisComunicacao = 6,
            FidelidadeIntelbras = 7,
            //Hotline = 8,
            IndicacaoClientePotencial = 0,
            IndicacaoLeads = 9,
            LogoDistribuidorAutorizadoIntelbras = 10,
            MateriaisInteligenciaCompetitiva = 11,
            ParticiparProgramaCanaisIntelbras = 0,
            PrazoPagamento = 12,
            PriceProtection = 8,
            RamalVoIPSuporte = 0,
            Rebate = 37,
            RebatePosVenda = 66,
            RegistroOportunidade = 0,
            Showroom = 15,
            StockRotation = 22,
            TreinamentosForcaVendas = 0,
            VMC = 21
        }
    }

    public class ProdutoKit
    {
        public enum StateCode
        {
            Ativo = 0,
            Inativo = 1
        }
    }

    public class Email
    {
        public enum StatusEmail
        {
            Rascunho = 1,
            Concluida = 2,
            Enviado = 3,
            Recebido = 4,
            Cancelada = 5,
            EnvioPendente = 6,
            Enviando = 7,
            Falha = 8
        }

        public enum StateCode
        {
            Aberto = 0,
            Concluido = 1,
            Cancelado = 2
        }
    }

    public class Transportadora
    {
        public enum CodigoViaTransportadora
        {
            Rodoviario = 993520001,
            Aeroviário = 993520002,
            Marítimo = 993520003,
            Ferroviário = 993520004,
            Rodoferroviário = 993520005,
            Rodofluvial = 993520006,
            Rodoaeroviário = 993520007,
            Outros = 993520008
        }
    }

    public class DeParaDeUnidadeDoKonviva
    {
        public enum TipoDeDePara
        {
            Canal = 993520000,
            ContaAdesaoNomeada = 993520001,
            Fornecedor = 993520002,
            ColaboradorIntelbras = 993520003,
            ClienteFinal = 993520004
        }

        public enum TipoDeRelacao
        {
            ColaboradorIntelbras = 993520000,
            KeyAccountRepresentante = 993520001
        }

        public enum PapelNoCanalIntelbras
        {
            RH = 993520005,
            Trade = 993520006,
            Instrutores = 993520007,
            Representantes = 993520008,
            PosVendas = 993520009
        }
    }

    public class DocumentoCanaisExtranet
    {
        public enum RazaoStatus
        {
            Rascunho = 993520000,
            PendenteAprovacao = 993520001,
            Aprovado = 993520002
        }
    }
}
