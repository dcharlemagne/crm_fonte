using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Servicos.GestaoSLA;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class RepositoryService
    {

        #region Construtores

        public RepositoryService() { }

        public RepositoryService(string organizationName)
        {
            NomeDaOrganizacao = organizationName;
        }

        public RepositoryService(string organizationName, bool isOffLine)
        {
            NomeDaOrganizacao = organizationName;
            IsOffline = isOffLine;
        }

        public RepositoryService(string organizationName, bool isOffLine, object provider)
        {
            NomeDaOrganizacao = organizationName;
            IsOffline = isOffLine;
            Provider = provider;
        }

        #endregion

        #region Atributos

        private bool _isOffline = false;
        public bool IsOffline
        {
            get { return _isOffline; }
            set { _isOffline = value; }
        }

        private string _nomeDaOrganizacao = "";
        public string NomeDaOrganizacao
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeDaOrganizacao))
                    _nomeDaOrganizacao = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

                return _nomeDaOrganizacao;
            }
            set { _nomeDaOrganizacao = value; }
        }

        public object Provider { get; set; }

        #endregion

        #region Domínio

        private IImportacaoAssistenciaTecnica<ImportacaoAssistenciaTecnica> _ImportacaoAssistenciaTecnica = null;
        public IImportacaoAssistenciaTecnica<ImportacaoAssistenciaTecnica> ImportacaoAssistenciaTecnica
        {
            get
            {
                if (_ImportacaoAssistenciaTecnica == null)
                    _ImportacaoAssistenciaTecnica = Provider != null ? RepositoryFactory.GetRepository<IImportacaoAssistenciaTecnica<ImportacaoAssistenciaTecnica>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IImportacaoAssistenciaTecnica<ImportacaoAssistenciaTecnica>>(NomeDaOrganizacao, IsOffline);

                return _ImportacaoAssistenciaTecnica;
            }
        }

        private IProdutoAssisteciaTecnica<ProdutoAssisteciaTecnica> _ProdutoAssisteciaTecnica = null;
        public IProdutoAssisteciaTecnica<ProdutoAssisteciaTecnica> ProdutoAssisteciaTecnica
        {
            get
            {
                if (_ProdutoAssisteciaTecnica == null)
                    _ProdutoAssisteciaTecnica = Provider != null ? RepositoryFactory.GetRepository<IProdutoAssisteciaTecnica<ProdutoAssisteciaTecnica>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoAssisteciaTecnica<ProdutoAssisteciaTecnica>>(NomeDaOrganizacao, IsOffline);

                return _ProdutoAssisteciaTecnica;
            }
        }

        private IPrioridadeLigacaoCallCenter<PrioridadeLigacaoCallCenter> _PrioridadeLigacaoCallCenter = null;
        public IPrioridadeLigacaoCallCenter<PrioridadeLigacaoCallCenter> PrioridadeLigacaoCallCenter
        {
            get
            {
                if (_PrioridadeLigacaoCallCenter == null)
                    _PrioridadeLigacaoCallCenter = Provider != null ? RepositoryFactory.GetRepository<IPrioridadeLigacaoCallCenter<PrioridadeLigacaoCallCenter>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPrioridadeLigacaoCallCenter<PrioridadeLigacaoCallCenter>>(NomeDaOrganizacao, IsOffline);

                return _PrioridadeLigacaoCallCenter;
            }
        }

        private IPontuacao<Pontuacao> _FidelidadePontuacao = null;
        public IPontuacao<Pontuacao> FidelidadePontuacao
        {
            get
            {
                if (_FidelidadePontuacao == null)
                    _FidelidadePontuacao = Provider != null ? RepositoryFactory.GetRepository<IPontuacao<Pontuacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPontuacao<Pontuacao>>(NomeDaOrganizacao, IsOffline);

                return _FidelidadePontuacao;
            }
        }

        private IGrupoFidelidade<GrupoFidelidade> _FidelidadeGrupo = null;
        public IGrupoFidelidade<GrupoFidelidade> FidelidadeGrupo
        {
            get
            {
                if (_FidelidadeGrupo == null)
                    _FidelidadeGrupo = Provider != null ? RepositoryFactory.GetRepository<IGrupoFidelidade<GrupoFidelidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IGrupoFidelidade<GrupoFidelidade>>(NomeDaOrganizacao, IsOffline);

                return _FidelidadeGrupo;
            }
        }

        private IExtratoFidelidade<ExtratoFidelidade> _FidelidadeExtrato = null;
        public IExtratoFidelidade<ExtratoFidelidade> FidelidadeExtrato
        {
            get
            {
                if (_FidelidadeExtrato == null)
                    _FidelidadeExtrato = Provider != null ? RepositoryFactory.GetRepository<IExtratoFidelidade<ExtratoFidelidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IExtratoFidelidade<ExtratoFidelidade>>(NomeDaOrganizacao, IsOffline);

                return _FidelidadeExtrato;
            }
        }

        private IPremioFidelidade<PremioFidelidade> _FidelidadePremio = null;
        public IPremioFidelidade<PremioFidelidade> FidelidadePremio
        {
            get
            {
                if (_FidelidadePremio == null)
                    _FidelidadePremio = Provider != null ? RepositoryFactory.GetRepository<IPremioFidelidade<PremioFidelidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPremioFidelidade<PremioFidelidade>>(NomeDaOrganizacao, IsOffline);

                return _FidelidadePremio;
            }
        }

        private IProdutoResgatadoFidelidade<ProdutoResgatadoFidelidade> _FidelidadeProdutoResgatado = null;
        public IProdutoResgatadoFidelidade<ProdutoResgatadoFidelidade> FidelidadeProdutoResgatado
        {
            get
            {
                if (_FidelidadeProdutoResgatado == null)
                    _FidelidadeProdutoResgatado = Provider != null ? RepositoryFactory.GetRepository<IProdutoResgatadoFidelidade<ProdutoResgatadoFidelidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoResgatadoFidelidade<ProdutoResgatadoFidelidade>>(NomeDaOrganizacao, IsOffline);

                return _FidelidadeProdutoResgatado;
            }
        }

        private IArquivoDeEstoqueGiro<ArquivoDeEstoqueGiro> _ArquivoDeEstoqueGiro = null;
        public IArquivoDeEstoqueGiro<ArquivoDeEstoqueGiro> ArquivoDeEstoqueGiro
        {
            get
            {
                if (_ArquivoDeEstoqueGiro == null)
                    _ArquivoDeEstoqueGiro = Provider != null ? RepositoryFactory.GetRepository<IArquivoDeEstoqueGiro<ArquivoDeEstoqueGiro>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IArquivoDeEstoqueGiro<ArquivoDeEstoqueGiro>>(NomeDaOrganizacao, IsOffline);

                return _ArquivoDeEstoqueGiro;
            }
        }

        private IArquivoDeSellOut<ArquivoDeSellOut> _ArquivoDeSellOut = null;
        public IArquivoDeSellOut<ArquivoDeSellOut> ArquivoDeSellOut
        {
            get
            {
                if (_ArquivoDeSellOut == null)
                    _ArquivoDeSellOut = Provider != null ? RepositoryFactory.GetRepository<IArquivoDeSellOut<ArquivoDeSellOut>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IArquivoDeSellOut<ArquivoDeSellOut>>(NomeDaOrganizacao, IsOffline);

                return _ArquivoDeSellOut;
            }
        }

        private IGrupoCliente<GrupoDoCliente> _GrupoDoCliente = null;
        public IGrupoCliente<GrupoDoCliente> GrupoDoCliente
        {
            get
            {
                if (_GrupoDoCliente == null)
                    _GrupoDoCliente = Provider != null ? RepositoryFactory.GetRepository<IGrupoCliente<GrupoDoCliente>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IGrupoCliente<GrupoDoCliente>>(NomeDaOrganizacao, IsOffline);

                return _GrupoDoCliente;
            }
        }

        private IStatusBeneficios<StatusBeneficios> _StatusBeneficios = null;
        public IStatusBeneficios<StatusBeneficios> StatusBeneficios
        {
            get
            {
                if (_StatusBeneficios == null)
                    _StatusBeneficios = Provider != null ? RepositoryFactory.GetRepository<IStatusBeneficios<StatusBeneficios>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IStatusBeneficios<StatusBeneficios>>(NomeDaOrganizacao, IsOffline);

                return _StatusBeneficios;
            }
        }

        private ISharePointDocumentLocation<SharePointDocumentLocation> _SharePointDocumentLocation = null;
        public ISharePointDocumentLocation<SharePointDocumentLocation> SharePointDocumentLocation
        {
            get
            {
                if (_SharePointDocumentLocation == null)
                    _SharePointDocumentLocation = Provider != null ? RepositoryFactory.GetRepository<ISharePointDocumentLocation<SharePointDocumentLocation>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISharePointDocumentLocation<SharePointDocumentLocation>>(NomeDaOrganizacao, IsOffline);

                return _SharePointDocumentLocation;
            }
        }

        private ITabelaFinanciamento<TabelaFinanciamento> _TabelaFinanciamento = null;
        public ITabelaFinanciamento<TabelaFinanciamento> TabelaFinanciamento
        {
            get
            {
                if (_TabelaFinanciamento == null)
                    _TabelaFinanciamento = Provider != null ? RepositoryFactory.GetRepository<ITabelaFinanciamento<TabelaFinanciamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITabelaFinanciamento<TabelaFinanciamento>>(NomeDaOrganizacao, IsOffline);

                return _TabelaFinanciamento;
            }
        }

        private IFormaPagamento<FormaPagamento> _FormaPagamento = null;
        public IFormaPagamento<FormaPagamento> FormaPagamento
        {
            get
            {
                if (_FormaPagamento == null)
                    _FormaPagamento = Provider != null ? RepositoryFactory.GetRepository<IFormaPagamento<FormaPagamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFormaPagamento<FormaPagamento>>(NomeDaOrganizacao, IsOffline);

                return _FormaPagamento;
            }
        }


        private IIndice<Indice> _Indice = null;
        public IIndice<Indice> Indice
        {
            get
            {
                if (_Indice == null)
                    _Indice = Provider != null ? RepositoryFactory.GetRepository<IIndice<Indice>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IIndice<Indice>>(NomeDaOrganizacao, IsOffline);

                return _Indice;
            }
        }

        private IFatura<Fatura> _Fatura = null;
        public IFatura<Fatura> Fatura
        {
            get
            {
                if (_Fatura == null)
                    _Fatura = Provider != null ? RepositoryFactory.GetRepository<IFatura<Fatura>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFatura<Fatura>>(NomeDaOrganizacao, IsOffline);

                return _Fatura;
            }
        }

        private IEndereco<Endereco> _Endereco = null;
        public IEndereco<Endereco> Endereco
        {
            get
            {
                if (_Endereco == null)
                    _Endereco = Provider != null ? RepositoryFactory.GetRepository<IEndereco<Endereco>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IEndereco<Endereco>>(NomeDaOrganizacao, IsOffline);

                return _Endereco;
            }
        }

        private IConfiguracaoBeneficio<ConfiguracaoBeneficio> _ConfiguracaoBeneficio = null;
        public IConfiguracaoBeneficio<ConfiguracaoBeneficio> ConfiguracaoBeneficio
        {
            get
            {
                if (_ConfiguracaoBeneficio == null)
                    _ConfiguracaoBeneficio = Provider != null ? RepositoryFactory.GetRepository<IConfiguracaoBeneficio<ConfiguracaoBeneficio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IConfiguracaoBeneficio<ConfiguracaoBeneficio>>(NomeDaOrganizacao, IsOffline);

                return _ConfiguracaoBeneficio;
            }
        }

        private IBeneficiosCompromissos<BeneficiosCompromissos> _BeneficiosCompromissos = null;
        public IBeneficiosCompromissos<BeneficiosCompromissos> BeneficiosCompromissos
        {
            get
            {
                if (_BeneficiosCompromissos == null)
                    _BeneficiosCompromissos = Provider != null ? RepositoryFactory.GetRepository<IBeneficiosCompromissos<BeneficiosCompromissos>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IBeneficiosCompromissos<BeneficiosCompromissos>>(NomeDaOrganizacao, IsOffline);

                return _BeneficiosCompromissos;
            }
        }


        private ITipoAcessoExtranet<TipoDeAcessoExtranet> _TipoAcessoExtranet = null;
        public ITipoAcessoExtranet<TipoDeAcessoExtranet> TipoAcessoExtranet
        {
            get
            {
                if (_TipoAcessoExtranet == null)
                    _TipoAcessoExtranet = Provider != null ? RepositoryFactory.GetRepository<ITipoAcessoExtranet<TipoDeAcessoExtranet>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITipoAcessoExtranet<TipoDeAcessoExtranet>>(NomeDaOrganizacao, IsOffline);

                return _TipoAcessoExtranet;
            }
        }

        private IAcessoExtranet<AcessoExtranet> _AcessoExtranet = null;
        public IAcessoExtranet<AcessoExtranet> AcessoExtranet
        {
            get
            {
                if (_AcessoExtranet == null)
                    _AcessoExtranet = Provider != null ? RepositoryFactory.GetRepository<IAcessoExtranet<AcessoExtranet>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAcessoExtranet<AcessoExtranet>>(NomeDaOrganizacao, IsOffline);

                return _AcessoExtranet;
            }
        }

        private ISolicitacaoCadastro<SolicitacaoCadastro> _SolicitacaoCadastro = null;
        public ISolicitacaoCadastro<SolicitacaoCadastro> SolicitacaoCadastro
        {
            get
            {
                if (_SolicitacaoCadastro == null)
                    _SolicitacaoCadastro = Provider != null ? RepositoryFactory.GetRepository<ISolicitacaoCadastro<SolicitacaoCadastro>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISolicitacaoCadastro<SolicitacaoCadastro>>(NomeDaOrganizacao, IsOffline);

                return _SolicitacaoCadastro;
            }
        }


        private ITipoDeAtividade<TipoDeAtividade> _TipoDeAtividade = null;
        public ITipoDeAtividade<TipoDeAtividade> TipoDeAtividade
        {
            get
            {
                if (_TipoDeAtividade == null)
                    _TipoDeAtividade = Provider != null ? RepositoryFactory.GetRepository<ITipoDeAtividade<TipoDeAtividade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITipoDeAtividade<TipoDeAtividade>>(NomeDaOrganizacao, IsOffline);

                return _TipoDeAtividade;
            }
        }


        private IEmail<Email> _Email = null;
        public IEmail<Email> Email
        {
            get
            {
                if (_Email == null)
                    _Email = Provider != null ? RepositoryFactory.GetRepository<IEmail<Email>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IEmail<Email>>(NomeDaOrganizacao, IsOffline);

                return _Email;
            }
        }


        private ITransportadora<Transportadora> _Transportadora = null;
        public ITransportadora<Transportadora> Transportadora
        {
            get
            {
                if (_Transportadora == null)
                    _Transportadora = Provider != null ? RepositoryFactory.GetRepository<ITransportadora<Transportadora>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITransportadora<Transportadora>>(NomeDaOrganizacao, IsOffline);

                return _Transportadora;
            }
        }
        private IGrupoEstoque<GrupoEstoque> _GrupoEstoque = null;
        public IGrupoEstoque<GrupoEstoque> GrupoEstoque
        {
            get
            {
                if (_GrupoEstoque == null)
                    _GrupoEstoque = Provider != null ? RepositoryFactory.GetRepository<IGrupoEstoque<GrupoEstoque>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IGrupoEstoque<GrupoEstoque>>(NomeDaOrganizacao, IsOffline);

                return _GrupoEstoque;
            }
        }

        private IBeneficiosCompromissos<BeneficiosCompromissos> _BeneficioCompromisso = null;
        public IBeneficiosCompromissos<BeneficiosCompromissos> BeneficioCompromisso
        {
            get
            {
                if (_BeneficioCompromisso == null)
                    _BeneficioCompromisso = Provider != null ? RepositoryFactory.GetRepository<IBeneficiosCompromissos<BeneficiosCompromissos>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IBeneficiosCompromissos<BeneficiosCompromissos>>(NomeDaOrganizacao, IsOffline);

                return _BeneficioCompromisso;
            }
        }
        private ICompromissosDoPrograma<CompromissosDoPrograma> _CompromissosPrograma = null;
        public ICompromissosDoPrograma<CompromissosDoPrograma> CompromissosPrograma
        {
            get
            {
                if (_CompromissosPrograma == null)
                    _CompromissosPrograma = Provider != null ? RepositoryFactory.GetRepository<ICompromissosDoPrograma<CompromissosDoPrograma>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICompromissosDoPrograma<CompromissosDoPrograma>>(NomeDaOrganizacao, IsOffline);

                return _CompromissosPrograma;
            }
        }

        private ISolicitacaoBeneficio<SolicitacaoBeneficio> _SolicitacaoBeneficio = null;
        public ISolicitacaoBeneficio<SolicitacaoBeneficio> SolicitacaoBeneficio
        {
            get
            {
                if (_SolicitacaoBeneficio == null)
                    _SolicitacaoBeneficio = Provider != null ? RepositoryFactory.GetRepository<ISolicitacaoBeneficio<SolicitacaoBeneficio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISolicitacaoBeneficio<SolicitacaoBeneficio>>(NomeDaOrganizacao, IsOffline);

                return _SolicitacaoBeneficio;
            }
        }

        private ISolicitacaoXUnidades<SolicitacaoXUnidades> _SolicitacaoXUnidades = null;
        public ISolicitacaoXUnidades<SolicitacaoXUnidades> SolicitacaoXUnidades
        {
            get
            {
                if (_SolicitacaoXUnidades == null)
                    _SolicitacaoXUnidades = Provider != null ? RepositoryFactory.GetRepository<ISolicitacaoXUnidades<SolicitacaoXUnidades>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISolicitacaoXUnidades<SolicitacaoXUnidades>>(NomeDaOrganizacao, IsOffline);

                return _SolicitacaoXUnidades;
            }
        }

        private IPais<Pais> _Pais = null;
        public IPais<Pais> Pais
        {
            get
            {
                if (_Pais == null)
                    _Pais = Provider != null ? RepositoryFactory.GetRepository<IPais<Pais>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPais<Pais>>(NomeDaOrganizacao, IsOffline);

                return _Pais;
            }
        }

        private IParticipantesDoProcesso<ParticipantesDoProcesso> _ParticipantesDoProcesso = null;
        public IParticipantesDoProcesso<ParticipantesDoProcesso> ParticipantesDoProcesso
        {
            get
            {
                if (_ParticipantesDoProcesso == null)
                    _ParticipantesDoProcesso = Provider != null ? RepositoryFactory.GetRepository<IParticipantesDoProcesso<ParticipantesDoProcesso>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IParticipantesDoProcesso<ParticipantesDoProcesso>>(NomeDaOrganizacao, IsOffline);

                return _ParticipantesDoProcesso;
            }
        }

        private IProcesso<Processo> _Processo = null;
        public IProcesso<Processo> Processo
        {
            get
            {
                if (_Processo == null)
                    _Processo = Provider != null ? RepositoryFactory.GetRepository<IProcesso<Processo>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProcesso<Processo>>(NomeDaOrganizacao, IsOffline);

                return _Processo;
            }
        }

        private ITarefa<Tarefa> _Tarefa = null;
        public ITarefa<Tarefa> Tarefa
        {
            get
            {
                if (_Tarefa == null)
                    _Tarefa = Provider != null ? RepositoryFactory.GetRepository<ITarefa<Tarefa>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITarefa<Tarefa>>(NomeDaOrganizacao, IsOffline);

                return _Tarefa;
            }
        }

        private IPausaTarefa<PausaTarefa> _PausaTarefa = null;
        public IPausaTarefa<PausaTarefa> PausaTarefa
        {
            get
            {
                if (_PausaTarefa == null)
                    _PausaTarefa = Provider != null ? RepositoryFactory.GetRepository<IPausaTarefa<PausaTarefa>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPausaTarefa<PausaTarefa>>(NomeDaOrganizacao, IsOffline);

                return _PausaTarefa;
            }
        }

        private ICanalVerde<CanalVerde> _CanalVerde = null;
        public ICanalVerde<CanalVerde> CanalVerde
        {
            get
            {
                if (_CanalVerde == null)
                    _CanalVerde = Provider != null ? RepositoryFactory.GetRepository<ICanalVerde<CanalVerde>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICanalVerde<CanalVerde>>(NomeDaOrganizacao, IsOffline);

                return _CanalVerde;
            }
        }


        private IEstado<Estado> _Estado = null;
        public IEstado<Estado> Estado
        {
            get
            {
                if (_Estado == null)
                    _Estado = Provider != null ? RepositoryFactory.GetRepository<IEstado<Estado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IEstado<Estado>>(NomeDaOrganizacao, IsOffline);

                return _Estado;
            }
        }

        private IFila<Fila> _Fila = null;
        public IFila<Fila> Fila
        {
            get
            {
                if (_Fila == null)
                    _Fila = Provider != null ? RepositoryFactory.GetRepository<IFila<Fila>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFila<Fila>>(NomeDaOrganizacao, IsOffline);

                return _Fila;
            }
        }

        private IItemFila<ItemFila> _ItemFila = null;
        public IItemFila<ItemFila> ItemFila
        {
            get
            {
                if (_ItemFila == null)
                    _ItemFila = Provider != null ? RepositoryFactory.GetRepository<IItemFila<ItemFila>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IItemFila<ItemFila>>(NomeDaOrganizacao, IsOffline);

                return _ItemFila;
            }
        }

        private IMunicipio<Municipio> _Municipio = null;
        public IMunicipio<Municipio> Municipio
        {
            get
            {
                if (_Municipio == null)
                    _Municipio = Provider != null ? RepositoryFactory.GetRepository<IMunicipio<Municipio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMunicipio<Municipio>>(NomeDaOrganizacao, IsOffline);

                return _Municipio;
            }
        }

        private IBeneficioDoCanal<BeneficioDoCanal> _BeneficioDoCanal = null;
        public IBeneficioDoCanal<BeneficioDoCanal> BeneficioDoCanal
        {
            get
            {
                if (_BeneficioDoCanal == null)
                    _BeneficioDoCanal = Provider != null ? RepositoryFactory.GetRepository<IBeneficioDoCanal<BeneficioDoCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IBeneficioDoCanal<BeneficioDoCanal>>(NomeDaOrganizacao, IsOffline);

                return _BeneficioDoCanal;
            }
        }

        private IBeneficio<Beneficio> _Beneficio = null;
        public IBeneficio<Beneficio> Beneficio
        {
            get
            {
                if (_Beneficio == null)
                    _Beneficio = Provider != null ? RepositoryFactory.GetRepository<IBeneficio<Beneficio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IBeneficio<Beneficio>>(NomeDaOrganizacao, IsOffline);

                return _Beneficio;
            }
        }


        private IParametroBeneficio<ParametroBeneficio> _ParametroBeneficio = null;
        public IParametroBeneficio<ParametroBeneficio> ParametroBeneficio
        {
            get
            {
                if (_ParametroBeneficio == null)
                    _ParametroBeneficio = Provider != null ? RepositoryFactory.GetRepository<IParametroBeneficio<ParametroBeneficio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IParametroBeneficio<ParametroBeneficio>>(NomeDaOrganizacao, IsOffline);

                return _ParametroBeneficio;
            }
        }

        private ICompromissosDoCanal<CompromissosDoCanal> _CompromissosDoCanal = null;
        public ICompromissosDoCanal<CompromissosDoCanal> CompromissosDoCanal
        {
            get
            {
                if (_CompromissosDoCanal == null)
                    _CompromissosDoCanal = Provider != null ? RepositoryFactory.GetRepository<ICompromissosDoCanal<CompromissosDoCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICompromissosDoCanal<CompromissosDoCanal>>(NomeDaOrganizacao, IsOffline);

                return _CompromissosDoCanal;
            }
        }

        private IPedido<Pedido> _Pedido = null;
        public IPedido<Pedido> Pedido
        {
            get
            {
                if (_Pedido == null)
                    _Pedido = Provider != null ? RepositoryFactory.GetRepository<IPedido<Pedido>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPedido<Pedido>>(NomeDaOrganizacao, IsOffline);

                return _Pedido;
            }
        }




        private IPerfil<Perfil> _Perfil = null;
        public IPerfil<Perfil> Perfil
        {
            get
            {
                if (_Perfil == null)
                    _Perfil = Provider != null ? RepositoryFactory.GetRepository<IPerfil<Perfil>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPerfil<Perfil>>(NomeDaOrganizacao, IsOffline);

                return _Perfil;
            }
        }


        private IConta<Conta> _Conta = null;
        public IConta<Conta> Conta
        {
            get
            {
                if (_Conta == null)
                    _Conta = Provider != null ? RepositoryFactory.GetRepository<IConta<Conta>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IConta<Conta>>(NomeDaOrganizacao, IsOffline);

                return _Conta;
            }
        }

        private IContaSegmento<ContaSegmento> _ContaSegmento = null;
        public IContaSegmento<ContaSegmento> ContaSegmento
        {
            get
            {
                if (_ContaSegmento == null)
                    _ContaSegmento = Provider != null ? RepositoryFactory.GetRepository<IContaSegmento<ContaSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IContaSegmento<ContaSegmento>>(NomeDaOrganizacao, IsOffline);

                return _ContaSegmento;
            }
        }

        private IEquipe<Equipe> _Equipe = null;
        public IEquipe<Equipe> Equipe
        {
            get
            {
                if (_Equipe == null)
                    _Equipe = Provider != null ? RepositoryFactory.GetRepository<IEquipe<Equipe>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IEquipe<Equipe>>(NomeDaOrganizacao, IsOffline);

                return _Equipe;
            }
        }

        private IPoliticaComercial<PoliticaComercial> _PoliticaComercial = null;
        public IPoliticaComercial<PoliticaComercial> PoliticaComercial
        {
            get
            {
                if (_PoliticaComercial == null)
                    _PoliticaComercial = Provider != null ? RepositoryFactory.GetRepository<IPoliticaComercial<PoliticaComercial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPoliticaComercial<PoliticaComercial>>(NomeDaOrganizacao, IsOffline);

                return _PoliticaComercial;
            }
        }

        private IFamiliaPoliticaComercial<FamiliaPoliticaComercial> _FamiliaPoliticaComercial = null;
        public IFamiliaPoliticaComercial<FamiliaPoliticaComercial> FamiliaPoliticaComercial
        {
            get
            {
                if (_FamiliaPoliticaComercial == null)
                    _FamiliaPoliticaComercial = Provider != null ? RepositoryFactory.GetRepository<IFamiliaPoliticaComercial<FamiliaPoliticaComercial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFamiliaPoliticaComercial<FamiliaPoliticaComercial>>(NomeDaOrganizacao, IsOffline);

                return _FamiliaPoliticaComercial;
            }
        }

        private IPoliticaComercialXConta<PoliticaComercialXConta> _PoliticaComercialXConta = null;
        public IPoliticaComercialXConta<PoliticaComercialXConta> PoliticaComercialXConta
        {
            get
            {
                if (_PoliticaComercialXConta == null)
                    _PoliticaComercialXConta = Provider != null ? RepositoryFactory.GetRepository<IPoliticaComercialXConta<PoliticaComercialXConta>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPoliticaComercialXConta<PoliticaComercialXConta>>(NomeDaOrganizacao, IsOffline);

                return _PoliticaComercialXConta;
            }
        }

        private IPoliticaComercialXEstado<PoliticaComercialXEstado> _PoliticaComercialXEstado = null;
        public IPoliticaComercialXEstado<PoliticaComercialXEstado> PoliticaComercialXEstado
        {
            get
            {
                if (_PoliticaComercialXEstado == null)
                    _PoliticaComercialXEstado = Provider != null ? RepositoryFactory.GetRepository<IPoliticaComercialXEstado<PoliticaComercialXEstado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPoliticaComercialXEstado<PoliticaComercialXEstado>>(NomeDaOrganizacao, IsOffline);

                return _PoliticaComercialXEstado;
            }
        }

        private IProdutoPoliticaComercial<ProdutoPoliticaComercial> _produtoPoliticaComercial = null;
        public IProdutoPoliticaComercial<ProdutoPoliticaComercial> ProdutoPoliticaComercial
        {
            get
            {
                if (_produtoPoliticaComercial == null)
                    _produtoPoliticaComercial = Provider != null ? RepositoryFactory.GetRepository<IProdutoPoliticaComercial<ProdutoPoliticaComercial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoPoliticaComercial<ProdutoPoliticaComercial>>(NomeDaOrganizacao, IsOffline);

                return _produtoPoliticaComercial;
            }
        }

        private IClassificacao<Classificacao> _Classificacao = null;
        public IClassificacao<Classificacao> Classificacao
        {
            get
            {
                if (_Classificacao == null)
                    _Classificacao = Provider != null ? RepositoryFactory.GetRepository<IClassificacao<Classificacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IClassificacao<Classificacao>>(NomeDaOrganizacao, IsOffline);

                return _Classificacao;
            }
        }

        private IFamiliaComercial<FamiliaComercial> _FamiliaComercial = null;
        public IFamiliaComercial<FamiliaComercial> FamiliaComercial
        {
            get
            {
                if (_FamiliaComercial == null)
                    _FamiliaComercial = Provider != null ? RepositoryFactory.GetRepository<IFamiliaComercial<FamiliaComercial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFamiliaComercial<FamiliaComercial>>(NomeDaOrganizacao, IsOffline);

                return _FamiliaComercial;
            }
        }


        private IRelacionamentoCanal<RelacionamentoCanal> _RelacionamentoDoCanal = null;
        public IRelacionamentoCanal<RelacionamentoCanal> RelacionamentoDoCanal
        {
            get
            {
                if (_RelacionamentoDoCanal == null)
                    _RelacionamentoDoCanal = Provider != null ? RepositoryFactory.GetRepository<IRelacionamentoCanal<RelacionamentoCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRelacionamentoCanal<RelacionamentoCanal>>(NomeDaOrganizacao, IsOffline);

                return _RelacionamentoDoCanal;
            }
        }

        private IRelacionamentoB2B<RelacionamentoB2B> _RelacionamentoB2B = null;
        public IRelacionamentoB2B<RelacionamentoB2B> RelacionamentoB2B
        {
            get
            {
                if (_RelacionamentoB2B == null)
                    _RelacionamentoB2B = Provider != null ? RepositoryFactory.GetRepository<IRelacionamentoB2B<RelacionamentoB2B>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRelacionamentoB2B<RelacionamentoB2B>>(NomeDaOrganizacao, IsOffline);

                return _RelacionamentoB2B;
            }
        }

        private IHistoricoComprasCanal<HistoricoCompraCanal> _HistoricoCompraCanal = null;
        public IHistoricoComprasCanal<HistoricoCompraCanal> HistoricoCompraCanal
        {
            get
            {
                if (_HistoricoCompraCanal == null)
                    _HistoricoCompraCanal = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasCanal<HistoricoCompraCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasCanal<HistoricoCompraCanal>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoCompraCanal;
            }
        }

        private IHistoricoCompras<HistoricoCompra> _HistoricoCompra = null;
        public IHistoricoCompras<HistoricoCompra> HistoricoCompra
        {
            get
            {
                if (_HistoricoCompra == null)
                    _HistoricoCompra = Provider != null ? RepositoryFactory.GetRepository<IHistoricoCompras<HistoricoCompra>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoCompras<HistoricoCompra>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoCompra;
            }
        }

        private IHistoricoDistribuidor<HistoricoDistribuidor> _HistoricoDistribuidor = null;
        public IHistoricoDistribuidor<HistoricoDistribuidor> HistoricoDistribuidor
        {
            get
            {
                if (_HistoricoDistribuidor == null)
                    _HistoricoDistribuidor = Provider != null ? RepositoryFactory.GetRepository<IHistoricoDistribuidor<HistoricoDistribuidor>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoDistribuidor<HistoricoDistribuidor>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoDistribuidor;
            }
        }



        private ITeamMembership<TeamMembership> _TeamMembership = null;
        public ITeamMembership<TeamMembership> TeamMembership
        {
            get
            {
                if (_TeamMembership == null)
                    _TeamMembership = Provider != null ? RepositoryFactory.GetRepository<ITeamMembership<TeamMembership>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITeamMembership<TeamMembership>>(NomeDaOrganizacao, IsOffline);

                return _TeamMembership;
            }
        }

        private ICNAE<CNAE> _CNAE = null;
        public ICNAE<CNAE> CNAE
        {
            get
            {
                if (_CNAE == null)
                {
                    _CNAE = Provider != null ? RepositoryFactory.GetRepository<ICNAE<CNAE>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICNAE<CNAE>>(NomeDaOrganizacao, IsOffline);
                }
                return _CNAE;
            }
        }

        private ICategoria<Categoria> _Categoria = null;
        public ICategoria<Categoria> Categoria
        {
            get
            {
                if (_Categoria == null)
                    _Categoria = Provider != null ? RepositoryFactory.GetRepository<ICategoria<Categoria>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICategoria<Categoria>>(NomeDaOrganizacao, IsOffline);

                return _Categoria;
            }
        }

        private ICategoriaB2B<CategoriaB2B> _CategoriaB2B = null;
        public ICategoriaB2B<CategoriaB2B> CategoriaB2B
        {
            get
            {
                if (_CategoriaB2B == null)
                    _CategoriaB2B = Provider != null ? RepositoryFactory.GetRepository<ICategoriaB2B<CategoriaB2B>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICategoriaB2B<CategoriaB2B>>(NomeDaOrganizacao, IsOffline);

                return _CategoriaB2B;
            }
        }

        private ICategoriasCanal<CategoriasCanal> _CategoriasCanal = null;
        public ICategoriasCanal<CategoriasCanal> CategoriasCanal
        {
            get
            {
                if (_CategoriasCanal == null)
                    _CategoriasCanal = Provider != null ? RepositoryFactory.GetRepository<ICategoriasCanal<CategoriasCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICategoriasCanal<CategoriasCanal>>(NomeDaOrganizacao, IsOffline);

                return _CategoriasCanal;
            }
        }

        private IProduto<Product> _Produto = null;
        public IProduto<Product> Produto
        {
            get
            {
                if (_Produto == null)
                    _Produto = Provider != null ? RepositoryFactory.GetRepository<IProduto<Product>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProduto<Product>>(NomeDaOrganizacao, IsOffline);

                return _Produto;
            }
        }

        private IProdutoKit<ProdutoKit> _produtoKit = null;
        public IProdutoKit<ProdutoKit> ProdutoKit
        {
            get
            {
                if (_produtoKit == null)
                    _produtoKit = Provider != null ? RepositoryFactory.GetRepository<IProdutoKit<ProdutoKit>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoKit<ProdutoKit>>(NomeDaOrganizacao, IsOffline);

                return _produtoKit;
            }
        }

        private ISegmento<Segmento> _Segmento = null;
        public ISegmento<Segmento> Segmento
        {
            get
            {
                if (_Segmento == null)
                    _Segmento = Provider != null ? RepositoryFactory.GetRepository<ISegmento<Segmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISegmento<Segmento>>(NomeDaOrganizacao, IsOffline);

                return _Segmento;
            }
        }

        private IRegiaoGeografica<Itbc_regiaogeo> _RegiaoGeografica = null;
        public IRegiaoGeografica<Itbc_regiaogeo> RegiaoGeografica
        {
            get
            {
                if (_RegiaoGeografica == null)
                    _RegiaoGeografica = Provider != null ? RepositoryFactory.GetRepository<IRegiaoGeografica<Itbc_regiaogeo>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRegiaoGeografica<Itbc_regiaogeo>>(NomeDaOrganizacao, IsOffline);

                return _RegiaoGeografica;
            }
        }

        private IRegiao<Regiao> _Regiao = null;
        public IRegiao<Regiao> Regiao
        {
            get
            {
                if (_Regiao == null)
                    _Regiao = Provider != null ? RepositoryFactory.GetRepository<IRegiao<Regiao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRegiao<Regiao>>(NomeDaOrganizacao, IsOffline);

                return _Regiao;
            }
        }

        private IFamiliaProduto<FamiliaProduto> _FamiliaProduto = null;
        public IFamiliaProduto<FamiliaProduto> FamiliaProduto
        {
            get
            {
                if (_FamiliaProduto == null)
                    _FamiliaProduto = Provider != null ? RepositoryFactory.GetRepository<IFamiliaProduto<FamiliaProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFamiliaProduto<FamiliaProduto>>(NomeDaOrganizacao, IsOffline);

                return _FamiliaProduto;
            }
        }

        private IProdutoPortfolio<ProdutoPortfolio> _produtoPortfolio = null;
        public IProdutoPortfolio<ProdutoPortfolio> ProdutoPortfolio
        {
            get
            {
                if (_produtoPortfolio == null)
                    _produtoPortfolio = Provider != null ? RepositoryFactory.GetRepository<IProdutoPortfolio<ProdutoPortfolio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoPortfolio<ProdutoPortfolio>>(NomeDaOrganizacao, IsOffline);

                return _produtoPortfolio;
            }
        }

        private IProdutoProjeto<ProdutoProjeto> _produtoProjeto = null;
        public IProdutoProjeto<ProdutoProjeto> ProdutoProjeto
        {
            get
            {
                if (_produtoProjeto == null)
                    _produtoProjeto = Provider != null ? RepositoryFactory.GetRepository<IProdutoProjeto<ProdutoProjeto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoProjeto<ProdutoProjeto>>(NomeDaOrganizacao, IsOffline);

                return _produtoProjeto;
            }
        }

        private IProdutoTreinamento<ProdutoTreinamento> _produtoTreinamento = null;
        public IProdutoTreinamento<ProdutoTreinamento> ProdutoTreinamento
        {
            get
            {
                if (_produtoTreinamento == null)
                    _produtoTreinamento = Provider != null ? RepositoryFactory.GetRepository<IProdutoTreinamento<ProdutoTreinamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoTreinamento<ProdutoTreinamento>>(NomeDaOrganizacao, IsOffline);

                return _produtoTreinamento;
            }
        }

        private ISubfamiliaProduto<SubfamiliaProduto> _SubfamiliaProduto = null;
        public ISubfamiliaProduto<SubfamiliaProduto> SubfamiliaProduto
        {
            get
            {
                if (_SubfamiliaProduto == null)
                    _SubfamiliaProduto = Provider != null ? RepositoryFactory.GetRepository<ISubfamiliaProduto<SubfamiliaProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISubfamiliaProduto<SubfamiliaProduto>>(NomeDaOrganizacao, IsOffline);

                return _SubfamiliaProduto;
            }
        }




        private IOrigem<Origem> _Origem = null;
        public IOrigem<Origem> Origem
        {
            get
            {
                if (_Origem == null)
                    _Origem = Provider != null ? RepositoryFactory.GetRepository<IOrigem<Origem>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrigem<Origem>>(NomeDaOrganizacao, IsOffline);

                return _Origem;
            }
        }

        private IMoeda<Moeda> _Moeda = null;
        public IMoeda<Moeda> Moeda
        {
            get
            {
                if (_Moeda == null)
                    _Moeda = Provider != null ? RepositoryFactory.GetRepository<IMoeda<Moeda>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMoeda<Moeda>>(NomeDaOrganizacao, IsOffline);

                return _Moeda;
            }
        }

        private IListaPreco<ListaPreco> _ListaPreco = null;
        public IListaPreco<ListaPreco> ListaPreco
        {
            get
            {
                if (_ListaPreco == null)
                    _ListaPreco = Provider != null ? RepositoryFactory.GetRepository<IListaPreco<ListaPreco>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IListaPreco<ListaPreco>>(NomeDaOrganizacao, IsOffline);

                return _ListaPreco;
            }
        }

        private IProdutoEstabelecimento<ProdutoEstabelecimento> _produtoEstabelecimento = null;
        public IProdutoEstabelecimento<ProdutoEstabelecimento> ProdutoEstabelecimento
        {
            get
            {
                if (_produtoEstabelecimento == null)
                    _produtoEstabelecimento = Provider != null ? RepositoryFactory.GetRepository<IProdutoEstabelecimento<ProdutoEstabelecimento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoEstabelecimento<ProdutoEstabelecimento>>(NomeDaOrganizacao, IsOffline);

                return _produtoEstabelecimento;
            }
        }

        private IPortfolio<Portfolio> _portfolio = null;
        public IPortfolio<Portfolio> Portfolio
        {
            get
            {
                if (_portfolio == null)
                    _portfolio = Provider != null ? RepositoryFactory.GetRepository<IPortfolio<Portfolio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPortfolio<Portfolio>>(NomeDaOrganizacao, IsOffline);

                return _portfolio;
            }
        }

        private ITreinamentoCanal<TreinamentoCanal> _treinamentoCanal = null;
        public ITreinamentoCanal<TreinamentoCanal> TreinamentoCanal
        {
            get
            {
                if (_treinamentoCanal == null)
                    _treinamentoCanal = Provider != null ? RepositoryFactory.GetRepository<ITreinamentoCanal<TreinamentoCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITreinamentoCanal<TreinamentoCanal>>(NomeDaOrganizacao, IsOffline);

                return _treinamentoCanal;
            }
        }

        private IListaPrecoXEstado<ListaPrecoXEstado> _listaPrecoXEstado = null;
        public IListaPrecoXEstado<ListaPrecoXEstado> ListaPrecoXEstado
        {
            get
            {
                if (_listaPrecoXEstado == null)
                    _listaPrecoXEstado = Provider != null ? RepositoryFactory.GetRepository<IListaPrecoXEstado<ListaPrecoXEstado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IListaPrecoXEstado<ListaPrecoXEstado>>(NomeDaOrganizacao, IsOffline);

                return _listaPrecoXEstado;
            }
        }

        private IItemListaPreco<ItemListaPreco> _itemListaPreco = null;
        public IItemListaPreco<ItemListaPreco> ItemListaPreco
        {
            get
            {
                if (_itemListaPreco == null)
                    _itemListaPreco = Provider != null ? RepositoryFactory.GetRepository<IItemListaPreco<ItemListaPreco>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IItemListaPreco<ItemListaPreco>>(NomeDaOrganizacao, IsOffline);

                return _itemListaPreco;
            }
        }

        private IPortador<Portador> _Portador = null;
        public IPortador<Portador> Portador
        {
            get
            {
                if (_Portador == null)
                    _Portador = Provider != null ? RepositoryFactory.GetRepository<IPortador<Portador>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPortador<Portador>>(NomeDaOrganizacao, IsOffline);

                return _Portador;
            }
        }
        private IAcessoExtranetContatos<AcessoExtranetContato> _AcessoExtranetContato = null;
        public IAcessoExtranetContatos<AcessoExtranetContato> AcessoExtranetContato
        {
            get
            {
                if (_AcessoExtranetContato == null)
                    _AcessoExtranetContato = Provider != null ? RepositoryFactory.GetRepository<IAcessoExtranetContatos<AcessoExtranetContato>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAcessoExtranetContatos<AcessoExtranetContato>>(NomeDaOrganizacao, IsOffline);

                return _AcessoExtranetContato;
            }
        }

        private IAcessoExtranet<AcessoExtranet> _RepositorioAcessoExtranet = null;
        public IAcessoExtranet<AcessoExtranet> RepositorioAcessoExtranet
        {
            get
            {
                if (_RepositorioAcessoExtranet == null)
                    _RepositorioAcessoExtranet = Provider != null ? RepositoryFactory.GetRepository<IAcessoExtranet<AcessoExtranet>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAcessoExtranet<AcessoExtranet>>(NomeDaOrganizacao, IsOffline);

                return _RepositorioAcessoExtranet;
            }
        }

        private IContato<Contato> _Contato = null;
        public IContato<Contato> Contato
        {
            get
            {
                if (_Contato == null)
                    _Contato = Provider != null ? RepositoryFactory.GetRepository<IContato<Contato>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IContato<Contato>>(NomeDaOrganizacao, IsOffline);

                return _Contato;
            }
        }

        private IFornecedorCanal<FornecedorCanal> _FornecedorCanal = null;
        public IFornecedorCanal<FornecedorCanal> FornecedorCanal
        {
            get
            {
                if (_FornecedorCanal == null)
                    _FornecedorCanal = Provider != null ? RepositoryFactory.GetRepository<IFornecedorCanal<FornecedorCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFornecedorCanal<FornecedorCanal>>(NomeDaOrganizacao, IsOffline);

                return _FornecedorCanal;
            }
        }
        private IEmpresasColigadas<EmpresasColigadas> _EmpresasColigadas = null;
        public IEmpresasColigadas<EmpresasColigadas> EmpresasColigadas
        {
            get
            {
                if (_EmpresasColigadas == null)
                    _EmpresasColigadas = Provider != null ? RepositoryFactory.GetRepository<IEmpresasColigadas<EmpresasColigadas>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IEmpresasColigadas<EmpresasColigadas>>(NomeDaOrganizacao, IsOffline);

                return _EmpresasColigadas;
            }
        }


        private ISeguroConta<SeguroConta> _SeguroConta = null;
        public ISeguroConta<SeguroConta> SeguroConta
        {
            get
            {
                if (_SeguroConta == null)
                    _SeguroConta = Provider != null ? RepositoryFactory.GetRepository<ISeguroConta<SeguroConta>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISeguroConta<SeguroConta>>(NomeDaOrganizacao, IsOffline);

                return _SeguroConta;
            }
        }

        private IReferenciasCanal<ReferenciasCanal> _ReferenciasCanal = null;
        public IReferenciasCanal<ReferenciasCanal> ReferenciasCanal
        {
            get
            {
                if (_ReferenciasCanal == null)
                    _ReferenciasCanal = Provider != null ? RepositoryFactory.GetRepository<IReferenciasCanal<ReferenciasCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IReferenciasCanal<ReferenciasCanal>>(NomeDaOrganizacao, IsOffline);

                return _ReferenciasCanal;
            }
        }


        private IEstruturaAtendimento<EstruturaAtendimento> _EstruturaAtendimento = null;
        public IEstruturaAtendimento<EstruturaAtendimento> EstruturaAtendimento
        {
            get
            {
                if (_EstruturaAtendimento == null)
                    _EstruturaAtendimento = Provider != null ? RepositoryFactory.GetRepository<IEstruturaAtendimento<EstruturaAtendimento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IEstruturaAtendimento<EstruturaAtendimento>>(NomeDaOrganizacao, IsOffline);

                return _EstruturaAtendimento;
            }
        }

        private IUnidadeNegocio<UnidadeNegocio> _UnidadeNegocio = null;
        public IUnidadeNegocio<UnidadeNegocio> UnidadeNegocio
        {
            get
            {
                if (_UnidadeNegocio == null)
                    _UnidadeNegocio = Provider != null ? RepositoryFactory.GetRepository<IUnidadeNegocio<UnidadeNegocio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IUnidadeNegocio<UnidadeNegocio>>(NomeDaOrganizacao, IsOffline);

                return _UnidadeNegocio;
            }
        }
        

        private IDenuncia<Denuncia> _Denuncia = null;
        public IDenuncia<Denuncia> Denuncia
        {
            get
            {
                if (_Denuncia == null)
                    _Denuncia = Provider != null ? RepositoryFactory.GetRepository<IDenuncia<Denuncia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDenuncia<Denuncia>>(NomeDaOrganizacao, IsOffline);

                return _Denuncia;
            }
        }

        private ITipoDeDenuncia<TipoDeDenuncia> _TipoDeDenuncia = null;
        public ITipoDeDenuncia<TipoDeDenuncia> TipoDeDenuncia
        {
            get
            {
                if (_TipoDeDenuncia == null)
                    _TipoDeDenuncia = Provider != null ? RepositoryFactory.GetRepository<ITipoDeDenuncia<TipoDeDenuncia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITipoDeDenuncia<TipoDeDenuncia>>(NomeDaOrganizacao, IsOffline);

                return _TipoDeDenuncia;
            }
        }

        private IReceitaPadrao<ReceitaPadrao> _receitaPadrao = null;
        public IReceitaPadrao<ReceitaPadrao> ReceitaPadrao
        {
            get
            {
                if (_receitaPadrao == null)
                    _receitaPadrao = Provider != null ? RepositoryFactory.GetRepository<IReceitaPadrao<ReceitaPadrao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IReceitaPadrao<ReceitaPadrao>>(NomeDaOrganizacao, IsOffline);

                return _receitaPadrao;
            }
        }

        private IBens<Bens> _Bens = null;
        public IBens<Bens> Bens
        {
            get
            {
                if (_Bens == null)
                    _Bens = Provider != null ? RepositoryFactory.GetRepository<IBens<Bens>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IBens<Bens>>(NomeDaOrganizacao, IsOffline);

                return _Bens;
            }
        }

        private ICanaldeVenda<CanaldeVenda> _CanaldeVenda = null;
        public ICanaldeVenda<CanaldeVenda> CanaldeVenda
        {
            get
            {
                if (_CanaldeVenda == null)
                    _CanaldeVenda = Provider != null ? RepositoryFactory.GetRepository<ICanaldeVenda<CanaldeVenda>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICanaldeVenda<CanaldeVenda>>(NomeDaOrganizacao, IsOffline);

                return _CanaldeVenda;
            }
        }

        private IEstabelecimento<Estabelecimento> _Estabelecimento = null;
        public IEstabelecimento<Estabelecimento> Estabelecimento
        {
            get
            {
                if (_Estabelecimento == null)
                    _Estabelecimento = Provider != null ? RepositoryFactory.GetRepository<IEstabelecimento<Estabelecimento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IEstabelecimento<Estabelecimento>>(NomeDaOrganizacao, IsOffline);

                return _Estabelecimento;
            }
        }

        private ICondicaoPagamento<CondicaoPagamento> _CondicaoPagamento = null;
        public ICondicaoPagamento<CondicaoPagamento> CondicaoPagamento
        {
            get
            {
                if (_CondicaoPagamento == null)
                    _CondicaoPagamento = Provider != null ? RepositoryFactory.GetRepository<ICondicaoPagamento<CondicaoPagamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICondicaoPagamento<CondicaoPagamento>>(NomeDaOrganizacao, IsOffline);

                return _CondicaoPagamento;
            }
        }

        private INaturezaOperacao<NaturezaOperacao> _NaturezaOperacao = null;
        public INaturezaOperacao<NaturezaOperacao> NaturezaOperacao
        {
            get
            {
                if (_NaturezaOperacao == null)
                    _NaturezaOperacao = Provider != null ? RepositoryFactory.GetRepository<INaturezaOperacao<NaturezaOperacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<INaturezaOperacao<NaturezaOperacao>>(NomeDaOrganizacao, IsOffline);

                return _NaturezaOperacao;
            }
        }

        private IRota<Rota> _Rota = null;
        public IRota<Rota> Rota
        {
            get
            {
                if (_Rota == null)
                    _Rota = Provider != null ? RepositoryFactory.GetRepository<IRota<Rota>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRota<Rota>>(NomeDaOrganizacao, IsOffline);

                return _Rota;
            }
        }

        private IUsuario<Usuario> _usuario = null;
        public IUsuario<Usuario> Usuario
        {
            get
            {
                if (_usuario == null)
                    _usuario = Provider != null ? RepositoryFactory.GetRepository<IUsuario<Usuario>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IUsuario<Usuario>>(NomeDaOrganizacao, IsOffline);

                return _usuario;
            }
        }

        private ITabelaPreco<TabelaPreco> _TabelaPreco = null;
        public ITabelaPreco<TabelaPreco> TabelaPreco
        {
            get
            {
                if (_TabelaPreco == null)
                    _TabelaPreco = Provider != null ? RepositoryFactory.GetRepository<ITabelaPreco<TabelaPreco>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITabelaPreco<TabelaPreco>>(NomeDaOrganizacao, IsOffline);

                return _TabelaPreco;
            }
        }

        private ITabelaPrecoB2B<TabelaPrecoB2B> _TabelaPrecoB2B = null;
        public ITabelaPrecoB2B<TabelaPrecoB2B> TabelaPrecoB2B
        {
            get
            {
                if (_TabelaPrecoB2B == null)
                    _TabelaPrecoB2B = Provider != null ? RepositoryFactory.GetRepository<ITabelaPrecoB2B<TabelaPrecoB2B>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITabelaPrecoB2B<TabelaPrecoB2B>>(NomeDaOrganizacao, IsOffline);

                return _TabelaPrecoB2B;
            }
        }

        private IItemTabelaPrecoB2B<ItemTabelaPrecoB2B> _ItemTabelaPrecoB2B = null;
        public IItemTabelaPrecoB2B<ItemTabelaPrecoB2B> ItemTabelaPrecoB2B
        {
            get
            {
                if (_ItemTabelaPrecoB2B == null)
                    _ItemTabelaPrecoB2B = Provider != null ? RepositoryFactory.GetRepository<IItemTabelaPrecoB2B<ItemTabelaPrecoB2B>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IItemTabelaPrecoB2B<ItemTabelaPrecoB2B>>(NomeDaOrganizacao, IsOffline);

                return _ItemTabelaPrecoB2B;
            }
        }

        private IMensagem<Mensagem> _Mensagem = null;
        public IMensagem<Mensagem> Mensagem
        {
            get
            {
                if (_Mensagem == null)
                    _Mensagem = Provider != null ? RepositoryFactory.GetRepository<IMensagem<Mensagem>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMensagem<Mensagem>>(NomeDaOrganizacao, IsOffline);

                return _Mensagem;
            }
        }

        private IUnidadeDeMedida<Unidade> _Unidade = null;
        public IUnidadeDeMedida<Unidade> Unidade
        {
            get
            {
                if (_Unidade == null)
                    _Unidade = Provider != null ? RepositoryFactory.GetRepository<IUnidadeDeMedida<Unidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IUnidadeDeMedida<Unidade>>(NomeDaOrganizacao, IsOffline);

                return _Unidade;
            }
        }

        private IProdutoPedido<ProdutoPedido> _ProdutoPedido = null;
        public IProdutoPedido<ProdutoPedido> ProdutoPedido
        {
            get
            {
                if (_ProdutoPedido == null)
                    _ProdutoPedido = Provider != null ? RepositoryFactory.GetRepository<IProdutoPedido<ProdutoPedido>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoPedido<ProdutoPedido>>(NomeDaOrganizacao, IsOffline);

                return _ProdutoPedido;
            }
        }

        private IConexao<Conexao> _conexao = null;
        public IConexao<Conexao> Conexao
        {
            get
            {
                if (_conexao == null)
                    _conexao = Provider != null ? RepositoryFactory.GetRepository<IConexao<Conexao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IConexao<Conexao>>(NomeDaOrganizacao, IsOffline);

                return _conexao;
            }
        }

        private IRegiaoAtuacao<RegiaoAtuacao> _RegiaoAtuacao = null;
        public IRegiaoAtuacao<RegiaoAtuacao> RegiaoAtuacao
        {
            get
            {
                if (_RegiaoAtuacao == null)
                    _RegiaoAtuacao = Provider != null ? RepositoryFactory.GetRepository<IRegiaoAtuacao<RegiaoAtuacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRegiaoAtuacao<RegiaoAtuacao>>(NomeDaOrganizacao, IsOffline);

                return _RegiaoAtuacao;
            }
        }

        private IListaDesconto<ListaDesconto> _ListaDesconto = null;
        public IListaDesconto<ListaDesconto> ListaDesconto
        {
            get
            {
                if (_ListaDesconto == null)
                    _ListaDesconto = Provider != null ? RepositoryFactory.GetRepository<IListaDesconto<ListaDesconto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IListaDesconto<ListaDesconto>>(NomeDaOrganizacao, IsOffline);

                return _ListaDesconto;
            }
        }

        private IDocumentoCanal<DocumentoCanal> _DocumentoCanal = null;
        public IDocumentoCanal<DocumentoCanal> DocumentoCanal
        {
            get
            {
                if (_DocumentoCanal == null)
                    _DocumentoCanal = Provider != null ? RepositoryFactory.GetRepository<IDocumentoCanal<DocumentoCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDocumentoCanal<DocumentoCanal>>(NomeDaOrganizacao, IsOffline);

                return _DocumentoCanal;
            }
        }

        private IDocumentoCanalEstoqueGiro<DocumentoCanalEstoqueGiro> _DocumentoCanalEstoqueGiro = null;
        public IDocumentoCanalEstoqueGiro<DocumentoCanalEstoqueGiro> DocumentoCanalEstoqueGiro
        {
            get
            {
                if (_DocumentoCanalEstoqueGiro == null)
                    _DocumentoCanalEstoqueGiro = Provider != null ? RepositoryFactory.GetRepository<IDocumentoCanalEstoqueGiro<DocumentoCanalEstoqueGiro>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDocumentoCanalEstoqueGiro<DocumentoCanalEstoqueGiro>>(NomeDaOrganizacao, IsOffline);

                return _DocumentoCanalEstoqueGiro;
            }
        }

        private IFuncaoConexao<FuncaoConexao> _funcaoconexao = null;
        public IFuncaoConexao<FuncaoConexao> FuncaoConexao
        {
            get
            {
                if (_funcaoconexao == null)
                    _funcaoconexao = Provider != null ? RepositoryFactory.GetRepository<IFuncaoConexao<FuncaoConexao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFuncaoConexao<FuncaoConexao>>(NomeDaOrganizacao, IsOffline);

                return _funcaoconexao;
            }
        }

        private IFamiliaMaterial<FamiliaMaterial> _FamiliaMaterial = null;
        public IFamiliaMaterial<FamiliaMaterial> FamiliaMaterial
        {
            get
            {
                if (_FamiliaMaterial == null)
                    _FamiliaMaterial = Provider != null ? RepositoryFactory.GetRepository<IFamiliaMaterial<FamiliaMaterial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFamiliaMaterial<FamiliaMaterial>>(NomeDaOrganizacao, IsOffline);

                return _FamiliaMaterial;
            }
        }

        private IParametroGlobal<ParametroGlobal> _parametroGlobal = null;
        public IParametroGlobal<ParametroGlobal> ParametroGlobal
        {
            get
            {
                if (_parametroGlobal == null)
                    _parametroGlobal = Provider != null
                        ? RepositoryFactory.GetRepository<IParametroGlobal<ParametroGlobal>>(NomeDaOrganizacao, IsOffline, Provider)
                        : RepositoryFactory.GetRepository<IParametroGlobal<ParametroGlobal>>(NomeDaOrganizacao, IsOffline);

                return _parametroGlobal;
            }
        }

        private IListaPrecoPSD<ListaPrecoPSDPPPSCF> _ListaPrecoPSD = null;
        public IListaPrecoPSD<ListaPrecoPSDPPPSCF> ListaPrecoPSD
        {
            get
            {
                if (_ListaPrecoPSD == null)
                    _ListaPrecoPSD = Provider != null ? RepositoryFactory.GetRepository<IListaPrecoPSD<ListaPrecoPSDPPPSCF>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IListaPrecoPSD<ListaPrecoPSDPPPSCF>>(NomeDaOrganizacao, IsOffline);

                return _ListaPrecoPSD;
            }
        }

        private IListaPrecoPSDEstado<ListaPrecoPSDEstado> _ListaPrecoPSDEstado = null;
        public IListaPrecoPSDEstado<ListaPrecoPSDEstado> ListaPrecoPSDEstado
        {
            get
            {
                if (_ListaPrecoPSDEstado == null)
                    _ListaPrecoPSDEstado = Provider != null ? RepositoryFactory.GetRepository<IListaPrecoPSDEstado<ListaPrecoPSDEstado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IListaPrecoPSDEstado<ListaPrecoPSDEstado>>(NomeDaOrganizacao, IsOffline);

                return _ListaPrecoPSDEstado;
            }
        }

        private IProdutoListaPSD<ProdutoListaPSDPPPSCF> _produtoListaPSD = null;
        public IProdutoListaPSD<ProdutoListaPSDPPPSCF> ProdutoListaPSD
        {
            get
            {
                if (_produtoListaPSD == null)
                    _produtoListaPSD = Provider != null ? RepositoryFactory.GetRepository<IProdutoListaPSD<ProdutoListaPSDPPPSCF>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoListaPSD<ProdutoListaPSDPPPSCF>>(NomeDaOrganizacao, IsOffline);

                return _produtoListaPSD;
            }
        }

        private ITipoSolicitacao<TipoSolicitacao> _tipoSolicitacao = null;
        public ITipoSolicitacao<TipoSolicitacao> TipoSolicitacao
        {
            get
            {
                if (_tipoSolicitacao == null)
                    _tipoSolicitacao = Provider != null ? RepositoryFactory.GetRepository<ITipoSolicitacao<TipoSolicitacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITipoSolicitacao<TipoSolicitacao>>(NomeDaOrganizacao, IsOffline);

                return _tipoSolicitacao;
            }
        }

        private ILinhaCorteDistribuidor<LinhaCorteDistribuidor> _linhaCorteDist = null;
        public ILinhaCorteDistribuidor<LinhaCorteDistribuidor> LinhaCorteDistribuidor
        {
            get
            {
                if (_linhaCorteDist == null)
                    _linhaCorteDist = Provider != null ? RepositoryFactory.GetRepository<ILinhaCorteDistribuidor<LinhaCorteDistribuidor>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILinhaCorteDistribuidor<LinhaCorteDistribuidor>>(NomeDaOrganizacao, IsOffline);

                return _linhaCorteDist;
            }
        }

        private IOrcamentoPorCanal<OrcamentoPorCanal> _OrcamentoPorCanal = null;
        public IOrcamentoPorCanal<OrcamentoPorCanal> OrcamentoPorCanal
        {
            get
            {
                if (_OrcamentoPorCanal == null)
                    _OrcamentoPorCanal = Provider != null ? RepositoryFactory.GetRepository<IOrcamentoPorCanal<OrcamentoPorCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentoPorCanal<OrcamentoPorCanal>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentoPorCanal;
            }
        }

        private IOrcamentodaUnidade<OrcamentodaUnidade> _OrcamentoUnidade = null;
        public IOrcamentodaUnidade<OrcamentodaUnidade> OrcamentodaUnidade
        {
            get
            {
                if (_OrcamentoUnidade == null)
                    _OrcamentoUnidade = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodaUnidade<OrcamentodaUnidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodaUnidade<OrcamentodaUnidade>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentoUnidade;
            }
        }

        private IMetadaUnidade<MetadaUnidade> _MetasUnidade = null;
        public IMetadaUnidade<MetadaUnidade> MetasUnidade
        {
            get
            {
                if (_MetasUnidade == null)
                    _MetasUnidade = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidade<MetadaUnidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidade<MetadaUnidade>>(NomeDaOrganizacao, IsOffline);

                return _MetasUnidade;
            }
        }

        private IGrupoUnidade<GrupoUnidade> _grupoUM = null;
        public IGrupoUnidade<GrupoUnidade> GrupoUnidadeMedida
        {
            get
            {
                if (_grupoUM == null)
                    _grupoUM = Provider != null ? RepositoryFactory.GetRepository<IGrupoUnidade<GrupoUnidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IGrupoUnidade<GrupoUnidade>>(NomeDaOrganizacao, IsOffline);

                return _grupoUM;
            }
        }

        private IColaboradorTreinadoCertificado<ColaboradorTreinadoCertificado> _colaboradorTreinadoCert = null;
        public IColaboradorTreinadoCertificado<ColaboradorTreinadoCertificado> ColaboradorTreinadoCertificado
        {
            get
            {
                if (_colaboradorTreinadoCert == null)
                    _colaboradorTreinadoCert = Provider != null ? RepositoryFactory.GetRepository<IColaboradorTreinadoCertificado<ColaboradorTreinadoCertificado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IColaboradorTreinadoCertificado<ColaboradorTreinadoCertificado>>(NomeDaOrganizacao, IsOffline);

                return _colaboradorTreinadoCert;
            }
        }

        private IStatusCompromissos<StatusCompromissos> _statusCompromissos = null;
        public IStatusCompromissos<StatusCompromissos> StatusCompromissos
        {
            get
            {
                if (_statusCompromissos == null)
                    _statusCompromissos = Provider != null ? RepositoryFactory.GetRepository<IStatusCompromissos<StatusCompromissos>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IStatusCompromissos<StatusCompromissos>>(NomeDaOrganizacao, IsOffline);

                return _statusCompromissos;
            }
        }

        private IObservacao<Observacao> _Observacao = null;
        public IObservacao<Observacao> Observacao
        {
            get
            {
                if (_Observacao == null)
                    _Observacao = Provider != null ? RepositoryFactory.GetRepository<IObservacao<Observacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IObservacao<Observacao>>(NomeDaOrganizacao, IsOffline);

                return _Observacao;
            }
        }

        private ITipodeParametroGlobal<TipodeParametroGlobal> _TipodeParametroGlobal = null;
        public ITipodeParametroGlobal<TipodeParametroGlobal> TipodeParametroGlobal
        {
            get
            {
                if (_TipodeParametroGlobal == null)
                    _TipodeParametroGlobal = Provider != null ? RepositoryFactory.GetRepository<ITipodeParametroGlobal<TipodeParametroGlobal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITipodeParametroGlobal<TipodeParametroGlobal>>(NomeDaOrganizacao, IsOffline);

                return _TipodeParametroGlobal;
            }
        }

        private IOrcamentodaUnidadeporTrimestre<OrcamentodaUnidadeporTrimestre> _OrcamentodaUnidadeporTrimestre = null;
        public IOrcamentodaUnidadeporTrimestre<OrcamentodaUnidadeporTrimestre> OrcamentodaUnidadeporTrimestre
        {
            get
            {
                if (_OrcamentodaUnidadeporTrimestre == null)
                    _OrcamentodaUnidadeporTrimestre = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodaUnidadeporTrimestre<OrcamentodaUnidadeporTrimestre>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodaUnidadeporTrimestre<OrcamentodaUnidadeporTrimestre>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodaUnidadeporTrimestre;
            }
        }

        private ISubclassificacoes<Subclassificacoes> _Subclassificacoes = null;
        public ISubclassificacoes<Subclassificacoes> Subclassificacoes
        {
            get
            {
                if (_Subclassificacoes == null)
                    _Subclassificacoes = Provider != null ? RepositoryFactory.GetRepository<ISubclassificacoes<Subclassificacoes>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISubclassificacoes<Subclassificacoes>>(NomeDaOrganizacao, IsOffline);

                return _Subclassificacoes;
            }
        }

        private IOrcamentodaUnidadeporSegmento<OrcamentodaUnidadeporSegmento> _OrcamentodaUnidadeporSegmento = null;
        public IOrcamentodaUnidadeporSegmento<OrcamentodaUnidadeporSegmento> OrcamentodaUnidadeporSegmento
        {
            get
            {
                if (_OrcamentodaUnidadeporSegmento == null)
                    _OrcamentodaUnidadeporSegmento = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodaUnidadeporSegmento<OrcamentodaUnidadeporSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodaUnidadeporSegmento<OrcamentodaUnidadeporSegmento>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodaUnidadeporSegmento;
            }
        }

        private IOrcamentodaUnidadeporFamilia<OrcamentodaUnidadeporFamilia> _OrcamentodaUnidadeporFamilia = null;
        public IOrcamentodaUnidadeporFamilia<OrcamentodaUnidadeporFamilia> OrcamentodaUnidadeporFamilia
        {
            get
            {
                if (_OrcamentodaUnidadeporFamilia == null)
                    _OrcamentodaUnidadeporFamilia = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodaUnidadeporFamilia<OrcamentodaUnidadeporFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodaUnidadeporFamilia<OrcamentodaUnidadeporFamilia>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodaUnidadeporFamilia;
            }
        }

        private IOrcamentodaUnidadeporSubFamilia<OrcamentodaUnidadeporSubFamilia> _OrcamentodaUnidadeporSubFamilia = null;
        public IOrcamentodaUnidadeporSubFamilia<OrcamentodaUnidadeporSubFamilia> OrcamentodaUnidadeporSubFamilia
        {
            get
            {
                if (_OrcamentodaUnidadeporSubFamilia == null)
                    _OrcamentodaUnidadeporSubFamilia = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodaUnidadeporSubFamilia<OrcamentodaUnidadeporSubFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodaUnidadeporSubFamilia<OrcamentodaUnidadeporSubFamilia>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodaUnidadeporSubFamilia;
            }
        }

        private IOrcamentodaUnidadeporProduto<OrcamentodaUnidadeporProduto> _OrcamentodaUnidadeporProduto = null;
        public IOrcamentodaUnidadeporProduto<OrcamentodaUnidadeporProduto> OrcamentodaUnidadeporProduto
        {
            get
            {
                if (_OrcamentodaUnidadeporProduto == null)
                    _OrcamentodaUnidadeporProduto = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodaUnidadeporProduto<OrcamentodaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodaUnidadeporProduto<OrcamentodaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodaUnidadeporProduto;
            }
        }


        private IAnotacao<Anotacao> _Anexo = null;
        public IAnotacao<Anotacao> Anexo
        {
            get
            {
                if (_Anexo == null)
                    _Anexo = Provider != null ? RepositoryFactory.GetRepository<IAnotacao<Anotacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAnotacao<Anotacao>>(NomeDaOrganizacao, IsOffline);

                return _Anexo;
            }
        }

        private IOrcamentoPorCanal<OrcamentoPorCanal> _OrcamentodoCanal = null;
        public IOrcamentoPorCanal<OrcamentoPorCanal> OrcamentodoCanal
        {
            get
            {
                if (_OrcamentodoCanal == null)
                    _OrcamentodoCanal = Provider != null ? RepositoryFactory.GetRepository<IOrcamentoPorCanal<OrcamentoPorCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentoPorCanal<OrcamentoPorCanal>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodoCanal;
            }
        }

        private IOrcamentodoCanalporSegmento<OrcamentodoCanalporSegmento> _OrcamentodoCanalporSegmento = null;
        public IOrcamentodoCanalporSegmento<OrcamentodoCanalporSegmento> OrcamentodoCanalporSegmento
        {
            get
            {
                if (_OrcamentodoCanalporSegmento == null)
                    _OrcamentodoCanalporSegmento = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodoCanalporSegmento<OrcamentodoCanalporSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodoCanalporSegmento<OrcamentodoCanalporSegmento>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodoCanalporSegmento;
            }
        }

        private IOrcamentodoCanalporFamilia<OrcamentodoCanalporFamilia> _OrcamentodoCanalporFamilia = null;
        public IOrcamentodoCanalporFamilia<OrcamentodoCanalporFamilia> OrcamentodoCanalporFamilia
        {
            get
            {
                if (_OrcamentodoCanalporFamilia == null)
                    _OrcamentodoCanalporFamilia = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodoCanalporFamilia<OrcamentodoCanalporFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodoCanalporFamilia<OrcamentodoCanalporFamilia>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodoCanalporFamilia;
            }
        }

        private IOrcamentodoCanalporSubFamilia<OrcamentodoCanalporSubFamilia> _OrcamentodoCanalporSubFamilia = null;
        public IOrcamentodoCanalporSubFamilia<OrcamentodoCanalporSubFamilia> OrcamentodoCanalporSubFamilia
        {
            get
            {
                if (_OrcamentodoCanalporSubFamilia == null)
                    _OrcamentodoCanalporSubFamilia = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodoCanalporSubFamilia<OrcamentodoCanalporSubFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodoCanalporSubFamilia<OrcamentodoCanalporSubFamilia>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodoCanalporSubFamilia;
            }
        }

        private IOrcamentodoCanalporProduto<OrcamentodoCanalporProduto> _OrcamentodoCanalporProduto = null;
        public IOrcamentodoCanalporProduto<OrcamentodoCanalporProduto> OrcamentodoCanalporProduto
        {
            get
            {
                if (_OrcamentodoCanalporProduto == null)
                    _OrcamentodoCanalporProduto = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodoCanalporProduto<OrcamentodoCanalporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodoCanalporProduto<OrcamentodoCanalporProduto>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodoCanalporProduto;
            }
        }

        private IOrcamentodaUnidadeDetalhadoporProduto<OrcamentodaUnidadeDetalhadoporProduto> _OrcamentodaUnidadeDetalhadoporProduto = null;
        public IOrcamentodaUnidadeDetalhadoporProduto<OrcamentodaUnidadeDetalhadoporProduto> OrcamentodaUnidadeDetalhadoporProduto
        {
            get
            {
                if (_OrcamentodaUnidadeDetalhadoporProduto == null)
                    _OrcamentodaUnidadeDetalhadoporProduto = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodaUnidadeDetalhadoporProduto<OrcamentodaUnidadeDetalhadoporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodaUnidadeDetalhadoporProduto<OrcamentodaUnidadeDetalhadoporProduto>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodaUnidadeDetalhadoporProduto;
            }
        }

        private IOrcamentodoCanalDetalhadoporProduto<OrcamentodoCanalDetalhadoporProduto> _OrcamentodoCanalDetalhadoporProduto = null;
        public IOrcamentodoCanalDetalhadoporProduto<OrcamentodoCanalDetalhadoporProduto> OrcamentodoCanalDetalhadoporProduto
        {
            get
            {
                if (_OrcamentodoCanalDetalhadoporProduto == null)
                    _OrcamentodoCanalDetalhadoporProduto = Provider != null ? RepositoryFactory.GetRepository<IOrcamentodoCanalDetalhadoporProduto<OrcamentodoCanalDetalhadoporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOrcamentodoCanalDetalhadoporProduto<OrcamentodoCanalDetalhadoporProduto>>(NomeDaOrganizacao, IsOffline);

                return _OrcamentodoCanalDetalhadoporProduto;
            }
        }

        private ILinhaCorteEstado<LinhaCorteEstado> _linhaCorteDistribuidorEstado = null;
        public ILinhaCorteEstado<LinhaCorteEstado> LinhaCorteDistribuidorEstado
        {
            get
            {
                if (_linhaCorteDistribuidorEstado == null)
                    _linhaCorteDistribuidorEstado = Provider != null ? RepositoryFactory.GetRepository<ILinhaCorteEstado<LinhaCorteEstado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILinhaCorteEstado<LinhaCorteEstado>>(NomeDaOrganizacao, IsOffline);

                return _linhaCorteDistribuidorEstado;
            }
        }

        private ILinhaCorteRevenda<LinhaCorteRevenda> _linhaCorteRevenda = null;
        public ILinhaCorteRevenda<LinhaCorteRevenda> LinhaCorteRevenda
        {
            get
            {
                if (_linhaCorteRevenda == null)
                    _linhaCorteRevenda = Provider != null ? RepositoryFactory.GetRepository<ILinhaCorteRevenda<LinhaCorteRevenda>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILinhaCorteRevenda<LinhaCorteRevenda>>(NomeDaOrganizacao, IsOffline);

                return _linhaCorteRevenda;
            }
        }

        private IMetadaUnidadeporTrimestre<MetadaUnidadeporTrimestre> _MetadaUnidadeporTrimestre = null;
        public IMetadaUnidadeporTrimestre<MetadaUnidadeporTrimestre> MetadaUnidadeporTrimestre
        {
            get
            {
                if (_MetadaUnidadeporTrimestre == null)
                    _MetadaUnidadeporTrimestre = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidadeporTrimestre<MetadaUnidadeporTrimestre>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidadeporTrimestre<MetadaUnidadeporTrimestre>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidadeporTrimestre;
            }
        }

        private IMetadaUnidadeporSegmento<MetadaUnidadeporSegmento> _MetadaUnidadeporSegmento = null;
        public IMetadaUnidadeporSegmento<MetadaUnidadeporSegmento> MetadaUnidadeporSegmento
        {
            get
            {
                if (_MetadaUnidadeporSegmento == null)
                    _MetadaUnidadeporSegmento = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidadeporSegmento<MetadaUnidadeporSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidadeporSegmento<MetadaUnidadeporSegmento>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidadeporSegmento;
            }
        }

        private IMetadaUnidadeporFamilia<MetadaUnidadeporFamilia> _MetadaUnidadeporFamilia = null;
        public IMetadaUnidadeporFamilia<MetadaUnidadeporFamilia> MetadaUnidadeporFamilia
        {
            get
            {
                if (_MetadaUnidadeporFamilia == null)
                    _MetadaUnidadeporFamilia = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidadeporFamilia<MetadaUnidadeporFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidadeporFamilia<MetadaUnidadeporFamilia>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidadeporFamilia;
            }
        }

        private IMetadaUnidadeporSubfamilia<MetadaUnidadeporSubfamilia> _MetadaUnidadeporSubfamilia = null;
        public IMetadaUnidadeporSubfamilia<MetadaUnidadeporSubfamilia> MetadaUnidadeporSubfamilia
        {
            get
            {
                if (_MetadaUnidadeporSubfamilia == null)
                    _MetadaUnidadeporSubfamilia = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidadeporSubfamilia<MetadaUnidadeporSubfamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidadeporSubfamilia<MetadaUnidadeporSubfamilia>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidadeporSubfamilia;
            }
        }

        private IMetadaUnidadeporProdutoMes<MetaDetalhadadaUnidadeporProduto> _MetadaUnidadeporProdutoMes = null;
        public IMetadaUnidadeporProdutoMes<MetaDetalhadadaUnidadeporProduto> MetadaUnidadeporProdutoMes
        {
            get
            {
                if (_MetadaUnidadeporProdutoMes == null)
                    _MetadaUnidadeporProdutoMes = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidadeporProdutoMes<MetaDetalhadadaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidadeporProdutoMes<MetaDetalhadadaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidadeporProdutoMes;
            }
        }

        private IMetadaUnidadeporProduto<MetadaUnidadeporProduto> _MetadaUnidadeporProduto = null;
        public IMetadaUnidadeporProduto<MetadaUnidadeporProduto> MetadaUnidadeporProduto
        {
            get
            {
                if (_MetadaUnidadeporProduto == null)
                    _MetadaUnidadeporProduto = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidadeporProduto<MetadaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidadeporProduto<MetadaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidadeporProduto;
            }
        }

        private IMetaDetalhadadaUnidadeporProduto<MetaDetalhadadaUnidadeporProduto> _MetadaUnidadeDetalhadaProduto = null;
        public IMetaDetalhadadaUnidadeporProduto<MetaDetalhadadaUnidadeporProduto> MetadaUnidadeDetalhadaProduto
        {
            get
            {
                if (_MetadaUnidadeDetalhadaProduto == null)
                    _MetadaUnidadeDetalhadaProduto = Provider != null ? RepositoryFactory.GetRepository<IMetaDetalhadadaUnidadeporProduto<MetaDetalhadadaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetaDetalhadadaUnidadeporProduto<MetaDetalhadadaUnidadeporProduto>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidadeDetalhadaProduto;
            }
        }

        private IMetadoCanal<MetadoCanal> _MetadoCanal = null;
        public IMetadoCanal<MetadoCanal> MetadoCanal
        {
            get
            {
                if (_MetadoCanal == null)
                    _MetadoCanal = Provider != null ? RepositoryFactory.GetRepository<IMetadoCanal<MetadoCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadoCanal<MetadoCanal>>(NomeDaOrganizacao, IsOffline);

                return _MetadoCanal;
            }
        }


        private IMetadoCanalPorTrimestre<MetadoCanalporTrimestre> _MetadoCanalPorTrimestre = null;
        public IMetadoCanalPorTrimestre<MetadoCanalporTrimestre> MetadoCanalporTrimestre
        {
            get
            {
                if (_MetadoCanalPorTrimestre == null)
                    _MetadoCanalPorTrimestre = Provider != null
                        ? RepositoryFactory.GetRepository<IMetadoCanalPorTrimestre<MetadoCanalporTrimestre>>(NomeDaOrganizacao, IsOffline, Provider)
                        : RepositoryFactory.GetRepository<IMetadoCanalPorTrimestre<MetadoCanalporTrimestre>>(NomeDaOrganizacao, IsOffline);

                return _MetadoCanalPorTrimestre;
            }
        }

        private IMetadoCanalporSegmento<MetadoCanalporSegmento> _MetadoCanalporSegmento = null;
        public IMetadoCanalporSegmento<MetadoCanalporSegmento> MetadoCanalporSegmento
        {
            get
            {
                if (_MetadoCanalporSegmento == null)
                    _MetadoCanalporSegmento = Provider != null ? RepositoryFactory.GetRepository<IMetadoCanalporSegmento<MetadoCanalporSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadoCanalporSegmento<MetadoCanalporSegmento>>(NomeDaOrganizacao, IsOffline);

                return _MetadoCanalporSegmento;
            }
        }

        private IMetadoCanalporFamilia<MetadoCanalporFamilia> _MetadoCanalporFamilia = null;
        public IMetadoCanalporFamilia<MetadoCanalporFamilia> MetadoCanalporFamilia
        {
            get
            {
                if (_MetadoCanalporFamilia == null)
                    _MetadoCanalporFamilia = Provider != null ? RepositoryFactory.GetRepository<IMetadoCanalporFamilia<MetadoCanalporFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadoCanalporFamilia<MetadoCanalporFamilia>>(NomeDaOrganizacao, IsOffline);

                return _MetadoCanalporFamilia;
            }
        }

        private IMetadoCanalporProduto<MetadoCanalporProduto> _MetadoCanalporProduto = null;
        public IMetadoCanalporProduto<MetadoCanalporProduto> MetadoCanalporProduto
        {
            get
            {
                if (_MetadoCanalporProduto == null)
                    _MetadoCanalporProduto = Provider != null ? RepositoryFactory.GetRepository<IMetadoCanalporProduto<MetadoCanalporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadoCanalporProduto<MetadoCanalporProduto>>(NomeDaOrganizacao, IsOffline);

                return _MetadoCanalporProduto;
            }
        }

        private IMetadoCanalporSubFamilia<MetadoCanalporSubFamilia> _MetadoCanalporSubFamilia = null;
        public IMetadoCanalporSubFamilia<MetadoCanalporSubFamilia> MetadoCanalporSubFamilia
        {
            get
            {
                if (_MetadoCanalporSubFamilia == null)
                    _MetadoCanalporSubFamilia = Provider != null ? RepositoryFactory.GetRepository<IMetadoCanalporSubFamilia<MetadoCanalporSubFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadoCanalporSubFamilia<MetadoCanalporSubFamilia>>(NomeDaOrganizacao, IsOffline);

                return _MetadoCanalporSubFamilia;
            }
        }

        private IMetaDetalhadadoCanalporProduto<MetaDetalhadadoCanalporProduto> _MetaDetalhadadoCanalporProduto = null;
        public IMetaDetalhadadoCanalporProduto<MetaDetalhadadoCanalporProduto> MetaDetalhadadoCanalporProduto
        {
            get
            {
                if (_MetaDetalhadadoCanalporProduto == null)
                    _MetaDetalhadadoCanalporProduto = Provider != null ? RepositoryFactory.GetRepository<IMetaDetalhadadoCanalporProduto<MetaDetalhadadoCanalporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetaDetalhadadoCanalporProduto<MetaDetalhadadoCanalporProduto>>(NomeDaOrganizacao, IsOffline);

                return _MetaDetalhadadoCanalporProduto;
            }
        }

        private IMetadaUnidade<MetadaUnidade> _MetadaUnidade = null;
        public IMetadaUnidade<MetadaUnidade> MetadaUnidade
        {
            get
            {
                if (_MetadaUnidade == null)
                    _MetadaUnidade = Provider != null ? RepositoryFactory.GetRepository<IMetadaUnidade<MetadaUnidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetadaUnidade<MetadaUnidade>>(NomeDaOrganizacao, IsOffline);

                return _MetadaUnidade;
            }
        }

        private ISharePointSite<SharePointSite> _SharePointSite = null;
        public ISharePointSite<SharePointSite> SharePointSite
        {
            get
            {
                if (_SharePointSite == null)
                    _SharePointSite = Provider != null ? RepositoryFactory.GetRepository<ISharePointSite<SharePointSite>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISharePointSite<SharePointSite>>(NomeDaOrganizacao, IsOffline);

                return _SharePointSite;
            }
        }

        private IDocumentoSharePoint<DocumentoSharePoint> _DocumentoSharePoint = null;
        public IDocumentoSharePoint<DocumentoSharePoint> DocumentoSharePoint
        {
            get
            {
                if (_DocumentoSharePoint == null)
                    _DocumentoSharePoint = Provider != null ? RepositoryFactory.GetRepository<IDocumentoSharePoint<DocumentoSharePoint>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDocumentoSharePoint<DocumentoSharePoint>>(NomeDaOrganizacao, IsOffline);

                return _DocumentoSharePoint;
            }
        }

        private IPotencialdoKAporProduto<PotencialdoKAporProduto> _PotencialdoKAporProduto = null;
        public IPotencialdoKAporProduto<PotencialdoKAporProduto> PotencialdoKAporProduto
        {
            get
            {
                if (_PotencialdoKAporProduto == null)
                    _PotencialdoKAporProduto = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoKAporProduto<PotencialdoKAporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoKAporProduto<PotencialdoKAporProduto>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoKAporProduto;
            }
        }

        private IPotencialdoKAporSegmento<PotencialdoKAporSegmento> _PotencialdoKAporSegmento = null;
        public IPotencialdoKAporSegmento<PotencialdoKAporSegmento> PotencialdoKAporSegmento
        {
            get
            {
                if (_PotencialdoKAporSegmento == null)
                    _PotencialdoKAporSegmento = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoKAporSegmento<PotencialdoKAporSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoKAporSegmento<PotencialdoKAporSegmento>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoKAporSegmento;
            }
        }

        private IPotencialdoKAporTrimestre<PotencialdoKAporTrimestre> _PotencialdoKAporTrimestre = null;
        public IPotencialdoKAporTrimestre<PotencialdoKAporTrimestre> PotencialdoKAporTrimestre
        {
            get
            {
                if (_PotencialdoKAporTrimestre == null)
                    _PotencialdoKAporTrimestre = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoKAporTrimestre<PotencialdoKAporTrimestre>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoKAporTrimestre<PotencialdoKAporTrimestre>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoKAporTrimestre;
            }
        }

        private IPotencialdoKAporFamilia<PotencialdoKAporFamilia> _PotencialdoKAporFamilia = null;
        public IPotencialdoKAporFamilia<PotencialdoKAporFamilia> PotencialdoKAporFamilia
        {
            get
            {
                if (_PotencialdoKAporFamilia == null)
                    _PotencialdoKAporFamilia = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoKAporFamilia<PotencialdoKAporFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoKAporFamilia<PotencialdoKAporFamilia>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoKAporFamilia;
            }
        }

        private IPotencialdoKAporSubfamilia<PotencialdoKAporSubfamilia> _PotencialdoKAporSubfamilia = null;
        public IPotencialdoKAporSubfamilia<PotencialdoKAporSubfamilia> PotencialdoKAporSubfamilia
        {
            get
            {
                if (_PotencialdoKAporSubfamilia == null)
                    _PotencialdoKAporSubfamilia = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoKAporSubfamilia<PotencialdoKAporSubfamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoKAporSubfamilia<PotencialdoKAporSubfamilia>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoKAporSubfamilia;
            }
        }

        private IMetaDetalhadadoKAporProduto<MetaDetalhadadoKAporProduto> _MetaDetalhadadoKAporProduto = null;
        public IMetaDetalhadadoKAporProduto<MetaDetalhadadoKAporProduto> MetaDetalhadadoKAporProduto
        {
            get
            {
                if (_MetaDetalhadadoKAporProduto == null)
                    _MetaDetalhadadoKAporProduto = Provider != null ? RepositoryFactory.GetRepository<IMetaDetalhadadoKAporProduto<MetaDetalhadadoKAporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMetaDetalhadadoKAporProduto<MetaDetalhadadoKAporProduto>>(NomeDaOrganizacao, IsOffline);

                return _MetaDetalhadadoKAporProduto;
            }
        }

        private IPotencialdoKARepresentante<PotencialdoKARepresentante> _PotencialdoKARepresentante = null;
        public IPotencialdoKARepresentante<PotencialdoKARepresentante> PotencialdoKARepresentante
        {
            get
            {
                if (_PotencialdoKARepresentante == null)
                    _PotencialdoKARepresentante = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoKARepresentante<PotencialdoKARepresentante>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoKARepresentante<PotencialdoKARepresentante>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoKARepresentante;
            }
        }

        private IPotencialdoSupervisorporProduto<PotencialdoSupervisorporProduto> _PotencialdoSupervisorporProduto = null;
        public IPotencialdoSupervisorporProduto<PotencialdoSupervisorporProduto> PotencialdoSupervisorporProduto
        {
            get
            {
                if (_PotencialdoSupervisorporProduto == null)
                    _PotencialdoSupervisorporProduto = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoSupervisorporProduto<PotencialdoSupervisorporProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoSupervisorporProduto<PotencialdoSupervisorporProduto>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoSupervisorporProduto;
            }
        }

        private IPotencialdoSupervisorporFamilia<PotencialdoSupervisorporFamilia> _PotencialdoSupervisorporFamilia = null;
        public IPotencialdoSupervisorporFamilia<PotencialdoSupervisorporFamilia> PotencialdoSupervisorporFamilia
        {
            get
            {
                if (_PotencialdoSupervisorporFamilia == null)
                    _PotencialdoSupervisorporFamilia = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoSupervisorporFamilia<PotencialdoSupervisorporFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoSupervisorporFamilia<PotencialdoSupervisorporFamilia>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoSupervisorporFamilia;
            }
        }

        private IPotencialdoSupervisorporSubfamilia<PotencialdoSupervisorporSubfamilia> _PotencialdoSupervisorporSubfamilia = null;
        public IPotencialdoSupervisorporSubfamilia<PotencialdoSupervisorporSubfamilia> PotencialdoSupervisorporSubfamilia
        {
            get
            {
                if (_PotencialdoSupervisorporSubfamilia == null)
                    _PotencialdoSupervisorporSubfamilia = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoSupervisorporSubfamilia<PotencialdoSupervisorporSubfamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoSupervisorporSubfamilia<PotencialdoSupervisorporSubfamilia>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoSupervisorporSubfamilia;
            }
        }

        private IPotencialdoSupervisorporTrimestre<PotencialdoSupervisorporTrimestre> _PotencialdoSupervisorporTrimestre = null;
        public IPotencialdoSupervisorporTrimestre<PotencialdoSupervisorporTrimestre> PotencialdoSupervisorporTrimestre
        {
            get
            {
                if (_PotencialdoSupervisorporTrimestre == null)
                    _PotencialdoSupervisorporTrimestre = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoSupervisorporTrimestre<PotencialdoSupervisorporTrimestre>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoSupervisorporTrimestre<PotencialdoSupervisorporTrimestre>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoSupervisorporTrimestre;
            }
        }

        private IPotencialdoSupervisorporSegmento<PotencialdoSupervisorporSegmento> _PotencialdoSupervisorporSegmento = null;
        public IPotencialdoSupervisorporSegmento<PotencialdoSupervisorporSegmento> PotencialdoSupervisorporSegmento
        {
            get
            {
                if (_PotencialdoSupervisorporSegmento == null)
                    _PotencialdoSupervisorporSegmento = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoSupervisorporSegmento<PotencialdoSupervisorporSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoSupervisorporSegmento<PotencialdoSupervisorporSegmento>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoSupervisorporSegmento;
            }
        }

        private IPotencialDetalhadodoSupervisorporProduto<PotencialdoSupervisorporProdutoDetalhado> _PotencialDetalhadodoSupervisorporProduto = null;
        public IPotencialDetalhadodoSupervisorporProduto<PotencialdoSupervisorporProdutoDetalhado> PotencialDetalhadodoSupervisorporProduto
        {
            get
            {
                if (_PotencialDetalhadodoSupervisorporProduto == null)
                    _PotencialDetalhadodoSupervisorporProduto = Provider != null ? RepositoryFactory.GetRepository<IPotencialDetalhadodoSupervisorporProduto<PotencialdoSupervisorporProdutoDetalhado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialDetalhadodoSupervisorporProduto<PotencialdoSupervisorporProdutoDetalhado>>(NomeDaOrganizacao, IsOffline);

                return _PotencialDetalhadodoSupervisorporProduto;
            }
        }

        private IPotencialdoSupervisor<PotencialdoSupervisor> _PotencialdoSupervisor = null;
        public IPotencialdoSupervisor<PotencialdoSupervisor> PotencialdoSupervisor
        {
            get
            {
                if (_PotencialdoSupervisor == null)
                    _PotencialdoSupervisor = Provider != null ? RepositoryFactory.GetRepository<IPotencialdoSupervisor<PotencialdoSupervisor>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPotencialdoSupervisor<PotencialdoSupervisor>>(NomeDaOrganizacao, IsOffline);

                return _PotencialdoSupervisor;
            }
        }


        private IProdutoFatura<ProdutoFatura> _ProdutoFatura = null;
        public IProdutoFatura<ProdutoFatura> ProdutoFatura
        {
            get
            {
                if (_ProdutoFatura == null)
                    _ProdutoFatura = Provider != null ? RepositoryFactory.GetRepository<IProdutoFatura<ProdutoFatura>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoFatura<ProdutoFatura>>(NomeDaOrganizacao, IsOffline);

                return _ProdutoFatura;
            }
        }

        private IPortfoliodoKeyAccountRepresentantes<PortfoliodoKeyAccountRepresentantes> _PortfoliodoKeyAccountRepresentantes = null;
        public IPortfoliodoKeyAccountRepresentantes<PortfoliodoKeyAccountRepresentantes> PortfoliodoKeyAccountRepresentantes
        {
            get
            {
                if (_PortfoliodoKeyAccountRepresentantes == null)
                    _PortfoliodoKeyAccountRepresentantes = Provider != null ? RepositoryFactory.GetRepository<IPortfoliodoKeyAccountRepresentantes<PortfoliodoKeyAccountRepresentantes>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPortfoliodoKeyAccountRepresentantes<PortfoliodoKeyAccountRepresentantes>>(NomeDaOrganizacao, IsOffline);

                return _PortfoliodoKeyAccountRepresentantes;
            }
        }

        private IParecer<Parecer> _Parecer = null;
        public IParecer<Parecer> Parecer
        {
            get
            {
                if (_Parecer == null)
                    _Parecer = Provider != null ? RepositoryFactory.GetRepository<IParecer<Parecer>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IParecer<Parecer>>(NomeDaOrganizacao, IsOffline);

                return _Parecer;
            }
        }

        private IProdutosdaSolicitacao<ProdutosdaSolicitacao> _ProdutosdaSolicitacao = null;
        public IProdutosdaSolicitacao<ProdutosdaSolicitacao> ProdutosdaSolicitacao
        {
            get
            {
                if (_ProdutosdaSolicitacao == null)
                    _ProdutosdaSolicitacao = Provider != null ? RepositoryFactory.GetRepository<IProdutosdaSolicitacao<ProdutosdaSolicitacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutosdaSolicitacao<ProdutosdaSolicitacao>>(NomeDaOrganizacao, IsOffline);

                return _ProdutosdaSolicitacao;
            }
        }

        private IAcaoSubsidiadaVmc<AcaoSubsidiadaVmc> _AcaoSubsidiadaVmc = null;
        public IAcaoSubsidiadaVmc<AcaoSubsidiadaVmc> AcaoSubsidiadaVmc
        {
            get
            {
                if (_AcaoSubsidiadaVmc == null)
                    _AcaoSubsidiadaVmc = Provider != null ? RepositoryFactory.GetRepository<IAcaoSubsidiadaVmc<AcaoSubsidiadaVmc>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAcaoSubsidiadaVmc<AcaoSubsidiadaVmc>>(NomeDaOrganizacao, IsOffline);

                return _AcaoSubsidiadaVmc;
            }
        }

        private IHistoricoComprasTrimestre<HistoricoComprasTrimestre> _HistoricoComprasTrimestre = null;
        public IHistoricoComprasTrimestre<HistoricoComprasTrimestre> HistoricoComprasTrimestre
        {
            get
            {
                if (_HistoricoComprasTrimestre == null)
                    _HistoricoComprasTrimestre = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasTrimestre<HistoricoComprasTrimestre>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasTrimestre<HistoricoComprasTrimestre>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasTrimestre;
            }
        }

        private IHistoricoComprasSubFamilia<HistoricoComprasSubfamilia> _HistoricoComprasSubFamilia = null;
        public IHistoricoComprasSubFamilia<HistoricoComprasSubfamilia> HistoricoComprasSubFamilia
        {
            get
            {
                if (_HistoricoComprasSubFamilia == null)
                    _HistoricoComprasSubFamilia = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasSubFamilia<HistoricoComprasSubfamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasSubFamilia<HistoricoComprasSubfamilia>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasSubFamilia;
            }
        }

        private IHistoricoComprasFamilia<HistoricoComprasFamilia> _HistoricoComprasFamilia = null;
        public IHistoricoComprasFamilia<HistoricoComprasFamilia> HistoricoComprasFamilia
        {
            get
            {
                if (_HistoricoComprasFamilia == null)
                    _HistoricoComprasFamilia = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasFamilia<HistoricoComprasFamilia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasFamilia<HistoricoComprasFamilia>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasFamilia;
            }
        }

        private IHistoricoComprasSegmento<HistoricoComprasSegmento> _HistoricoComprasSegmento = null;
        public IHistoricoComprasSegmento<HistoricoComprasSegmento> HistoricoComprasSegmento
        {
            get
            {
                if (_HistoricoComprasSegmento == null)
                    _HistoricoComprasSegmento = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasSegmento<HistoricoComprasSegmento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasSegmento<HistoricoComprasSegmento>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasSegmento;
            }
        }

        private IHistoricoComprasProduto<HistoricoDeComprasPorProduto> _HistoricoComprasProduto = null;
        public IHistoricoComprasProduto<HistoricoDeComprasPorProduto> HistoricoComprasProduto
        {
            get
            {
                if (_HistoricoComprasProduto == null)
                    _HistoricoComprasProduto = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasProduto<HistoricoDeComprasPorProduto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasProduto<HistoricoDeComprasPorProduto>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasProduto;
            }
        }


        private IHistoricoComprasProdutoMes<HistoricoDeComprasPorProdutoMes> _HistoricoComprasProdutoMes = null;
        public IHistoricoComprasProdutoMes<HistoricoDeComprasPorProdutoMes> HistoricoComprasProdutoMes
        {
            get
            {
                if (_HistoricoComprasProdutoMes == null)
                    _HistoricoComprasProdutoMes = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasProdutoMes<HistoricoDeComprasPorProdutoMes>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasProdutoMes<HistoricoDeComprasPorProdutoMes>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasProdutoMes;
            }
        }


        private IHistoricoComprasCanalProdutoMes<HistoricoComprasCanalProdutoMes> _HistoricoComprasCanalProdutoMes = null;
        public IHistoricoComprasCanalProdutoMes<HistoricoComprasCanalProdutoMes> HistoricoComprasCanalProdutoMes
        {
            get
            {
                if (_HistoricoComprasCanalProdutoMes == null)
                    _HistoricoComprasCanalProdutoMes = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasCanalProdutoMes<HistoricoComprasCanalProdutoMes>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasCanalProdutoMes<HistoricoComprasCanalProdutoMes>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasCanalProdutoMes;
            }
        }

        private IHistoricoComprasCanal<HistoricoCompraCanal> _HistoricoComprasCanal = null;
        public IHistoricoComprasCanal<HistoricoCompraCanal> HistoricoComprasCanal
        {
            get
            {
                if (_HistoricoComprasCanal == null)
                    _HistoricoComprasCanal = Provider != null ? RepositoryFactory.GetRepository<IHistoricoComprasCanal<HistoricoCompraCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoComprasCanal<HistoricoCompraCanal>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoComprasCanal;
            }
        }


        private ITreinamentoCertificacao<TreinamentoCertificacao> _TreinamentoCertificacao = null;
        public ITreinamentoCertificacao<TreinamentoCertificacao> TreinamentoCertificacao
        {
            get
            {
                if (_TreinamentoCertificacao == null)
                    _TreinamentoCertificacao = Provider != null ? RepositoryFactory.GetRepository<ITreinamentoCertificacao<TreinamentoCertificacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITreinamentoCertificacao<TreinamentoCertificacao>>(NomeDaOrganizacao, IsOffline);

                return _TreinamentoCertificacao;
            }
        }

        private ITurmaCanal<TurmaCanal> _TurmaCanal = null;
        public ITurmaCanal<TurmaCanal> TurmaCanal
        {
            get
            {
                if (_TurmaCanal == null)
                    _TurmaCanal = Provider != null ? RepositoryFactory.GetRepository<ITurmaCanal<TurmaCanal>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITurmaCanal<TurmaCanal>>(NomeDaOrganizacao, IsOffline);

                return _TurmaCanal;
            }
        }

        private IUnidadeKonviva<UnidadeKonviva> _UnidadeKonviva = null;
        public IUnidadeKonviva<UnidadeKonviva> UnidadeKonviva
        {
            get
            {
                if (_UnidadeKonviva == null)
                    _UnidadeKonviva = Provider != null ? RepositoryFactory.GetRepository<IUnidadeKonviva<UnidadeKonviva>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IUnidadeKonviva<UnidadeKonviva>>(NomeDaOrganizacao, IsOffline);

                return _UnidadeKonviva;
            }
        }

        private IAcessoKonviva<AcessoKonviva> _AcessoKonviva = null;
        public IAcessoKonviva<AcessoKonviva> AcessoKonviva
        {
            get
            {
                if (_AcessoKonviva == null)
                    _AcessoKonviva = Provider != null ? RepositoryFactory.GetRepository<IAcessoKonviva<AcessoKonviva>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAcessoKonviva<AcessoKonviva>>(NomeDaOrganizacao, IsOffline);

                return _AcessoKonviva;
            }
        }

        private IDeParaDeUnidadeDoKonviva<DeParaDeUnidadeDoKonviva> _DeParaDeUnidadeDoKonviva = null;
        public IDeParaDeUnidadeDoKonviva<DeParaDeUnidadeDoKonviva> DeParaDeUnidadeDoKonviva
        {
            get
            {
                if (_DeParaDeUnidadeDoKonviva == null)
                    _DeParaDeUnidadeDoKonviva = Provider != null ? RepositoryFactory.GetRepository<IDeParaDeUnidadeDoKonviva<DeParaDeUnidadeDoKonviva>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDeParaDeUnidadeDoKonviva<DeParaDeUnidadeDoKonviva>>(NomeDaOrganizacao, IsOffline);

                return _DeParaDeUnidadeDoKonviva;
            }
        }

        private IDocumentoCanaisExtranet<DocumentoCanaisExtranet> _DocumentoCanaisExtranet = null;
        public IDocumentoCanaisExtranet<DocumentoCanaisExtranet> DocumentoCanaisExtranet
        {
            get
            {
                if (_DocumentoCanaisExtranet == null)
                    _DocumentoCanaisExtranet = Provider != null ? RepositoryFactory.GetRepository<IDocumentoCanaisExtranet<DocumentoCanaisExtranet>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDocumentoCanaisExtranet<DocumentoCanaisExtranet>>(NomeDaOrganizacao, IsOffline);

                return _DocumentoCanaisExtranet;
            }
        }

        private IAdvertencia<Advertencia> _Advertencia = null;
        public IAdvertencia<Advertencia> Advertencia
        {
            get
            {
                if (_Advertencia == null)
                    _Advertencia = Provider != null ? RepositoryFactory.GetRepository<IAdvertencia<Advertencia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAdvertencia<Advertencia>>(NomeDaOrganizacao, IsOffline);

                return _Advertencia;
            }
        }

        private IAssunto<Assunto> _Assunto = null;
        public IAssunto<Assunto> Assunto
        {
            get
            {
                if (_Assunto == null)
                    _Assunto = Provider != null ? RepositoryFactory.GetRepository<IAssunto<Assunto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAssunto<Assunto>>(NomeDaOrganizacao, IsOffline);

                return _Assunto;
            }
        }

        private IAuditoria<Auditoria> _Auditoria = null;
        public IAuditoria<Auditoria> Auditoria
        {
            get
            {
                if (_Auditoria == null)
                    _Auditoria = Provider != null ? RepositoryFactory.GetRepository<IAuditoria<Auditoria>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAuditoria<Auditoria>>(NomeDaOrganizacao, IsOffline);

                return _Auditoria;
            }
        }

        private IAutorizacaoDePostagem<AutorizacaoPostagemCorreios> _AutorizacaoDePostagem = null;
        public IAutorizacaoDePostagem<AutorizacaoPostagemCorreios> AutorizacaoDePostagem
        {
            get
            {
                if (_AutorizacaoDePostagem == null)
                    _AutorizacaoDePostagem = Provider != null ? RepositoryFactory.GetRepository<IAutorizacaoDePostagem<AutorizacaoPostagemCorreios>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAutorizacaoDePostagem<AutorizacaoPostagemCorreios>>(NomeDaOrganizacao, IsOffline);

                return _AutorizacaoDePostagem;
            }
        }

        private IClienteParticipanteDoContrato<ClienteParticipanteDoContrato> _ClienteParticipanteDoContrato = null;
        public IClienteParticipanteDoContrato<ClienteParticipanteDoContrato> ClienteParticipanteDoContrato
        {
            get
            {
                if (_ClienteParticipanteDoContrato == null)
                    _ClienteParticipanteDoContrato = Provider != null ? RepositoryFactory.GetRepository<IClienteParticipanteDoContrato<ClienteParticipanteDoContrato>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IClienteParticipanteDoContrato<ClienteParticipanteDoContrato>>(NomeDaOrganizacao, IsOffline);

                return _ClienteParticipanteDoContrato;
            }
        }

        private IClienteParticipanteDoEndereco<ClienteParticipanteEndereco> _ClienteParticipanteDoEndereco = null;
        public IClienteParticipanteDoEndereco<ClienteParticipanteEndereco> ClienteParticipanteDoEndereco
        {
            get
            {
                if (_ClienteParticipanteDoEndereco == null)
                    _ClienteParticipanteDoEndereco = Provider != null ? RepositoryFactory.GetRepository<IClienteParticipanteDoEndereco<ClienteParticipanteEndereco>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IClienteParticipanteDoEndereco<ClienteParticipanteEndereco>>(NomeDaOrganizacao, IsOffline);

                return _ClienteParticipanteDoEndereco;
            }
        }

        private IClienteParticipante<ClienteParticipante> _ClienteParticipante = null;
        public IClienteParticipante<ClienteParticipante> ClienteParticipante
        {
            get
            {
                if (_ClienteParticipante == null)
                    _ClienteParticipante = Provider != null ? RepositoryFactory.GetRepository<IClienteParticipante<ClienteParticipante>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IClienteParticipante<ClienteParticipante>>(NomeDaOrganizacao, IsOffline);

                return _ClienteParticipante;
            }
        }

        private IClientePotencial<ClientePotencial> _ClientePotencial = null;
        public IClientePotencial<ClientePotencial> ClientePotencial
        {
            get
            {
                if (_ClientePotencial == null)
                    _ClientePotencial = Provider != null ? RepositoryFactory.GetRepository<IClientePotencial<ClientePotencial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IClientePotencial<ClientePotencial>>(NomeDaOrganizacao, IsOffline);

                return _ClientePotencial;
            }
        }

        private ICompromisso<Compromisso> _Compromisso = null;
        public ICompromisso<Compromisso> Compromisso
        {
            get
            {
                if (_Compromisso == null)
                    _Compromisso = Provider != null ? RepositoryFactory.GetRepository<ICompromisso<Compromisso>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICompromisso<Compromisso>>(NomeDaOrganizacao, IsOffline);

                return _Compromisso;
            }
        }

        private IContrato<Contrato> _Contrato = null;
        public IContrato<Contrato> Contrato
        {
            get
            {
                if (_Contrato == null)
                    _Contrato = Provider != null ? RepositoryFactory.GetRepository<IContrato<Contrato>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IContrato<Contrato>>(NomeDaOrganizacao, IsOffline);

                return _Contrato;
            }
        }

        private ICreditoFidelidade<CreditoFidelidade> _CreditoFidelidade = null;
        public ICreditoFidelidade<CreditoFidelidade> CreditoFidelidade
        {
            get
            {
                if (_CreditoFidelidade == null)
                    _CreditoFidelidade = Provider != null ? RepositoryFactory.GetRepository<ICreditoFidelidade<CreditoFidelidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICreditoFidelidade<CreditoFidelidade>>(NomeDaOrganizacao, IsOffline);

                return _CreditoFidelidade;
            }
        }

        private IDebitoFidelidade<DebitoFidelidade> _DebitoFidelidade = null;
        public IDebitoFidelidade<DebitoFidelidade> DebitoFidelidade
        {
            get
            {
                if (_DebitoFidelidade == null)
                    _DebitoFidelidade = Provider != null ? RepositoryFactory.GetRepository<IDebitoFidelidade<DebitoFidelidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDebitoFidelidade<DebitoFidelidade>>(NomeDaOrganizacao, IsOffline);

                return _DebitoFidelidade;
            }
        }

        private IDefeito<Defeito> _Defeito = null;
        public IDefeito<Defeito> Defeito
        {
            get
            {
                if (_Defeito == null)
                    _Defeito = Provider != null ? RepositoryFactory.GetRepository<IDefeito<Defeito>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDefeito<Defeito>>(NomeDaOrganizacao, IsOffline);

                return _Defeito;
            }
        }

        private ISolucao<Solucao> _Solucao = null;
        public ISolucao<Solucao> Solucao
        {
            get
            {
                if (_Solucao == null)
                    _Solucao = Provider != null ? RepositoryFactory.GetRepository<ISolucao<Solucao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISolucao<Solucao>>(NomeDaOrganizacao, IsOffline);

                return _Solucao;
            }
        }

        private IDiagnostico<Diagnostico> _Diagnostico = null;
        public IDiagnostico<Diagnostico> Diagnostico
        {
            get
            {
                if (_Diagnostico == null)
                    _Diagnostico = Provider != null ? RepositoryFactory.GetRepository<IDiagnostico<Diagnostico>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDiagnostico<Diagnostico>>(NomeDaOrganizacao, IsOffline);

                return _Diagnostico;
            }
        }

        private IDiagnosticoOcorrencia<DiagnosticoOcorrencia> _DiagnosticoOcorrencia = null;
        public IDiagnosticoOcorrencia<DiagnosticoOcorrencia> DiagnosticoOcorrencia
        {
            get
            {
                if (_DiagnosticoOcorrencia == null)
                    _DiagnosticoOcorrencia = Provider != null ? RepositoryFactory.GetRepository<IDiagnosticoOcorrencia<DiagnosticoOcorrencia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDiagnosticoOcorrencia<DiagnosticoOcorrencia>>(NomeDaOrganizacao, IsOffline);

                return _DiagnosticoOcorrencia;
            }
        }

        private IExtrato<Extrato> _Extrato = null;
        public IExtrato<Extrato> Extrato
        {
            get
            {
                if (_Extrato == null)
                    _Extrato = Provider != null ? RepositoryFactory.GetRepository<IExtrato<Extrato>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IExtrato<Extrato>>(NomeDaOrganizacao, IsOffline);

                return _Extrato;
            }
        }

        private IGrupoPremio<GrupoPremio> _GrupoPremio = null;
        public IGrupoPremio<GrupoPremio> GrupoPremio
        {
            get
            {
                if (_GrupoPremio == null)
                    _GrupoPremio = Provider != null ? RepositoryFactory.GetRepository<IGrupoPremio<GrupoPremio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IGrupoPremio<GrupoPremio>>(NomeDaOrganizacao, IsOffline);

                return _GrupoPremio;
            }
        }

        private IHistoricoDePostagem<HistoricoDePostagem> _HistoricoDePostagem = null;
        public IHistoricoDePostagem<HistoricoDePostagem> HistoricoDePostagem
        {
            get
            {
                if (_HistoricoDePostagem == null)
                    _HistoricoDePostagem = Provider != null ? RepositoryFactory.GetRepository<IHistoricoDePostagem<HistoricoDePostagem>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoDePostagem<HistoricoDePostagem>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoDePostagem;
            }
        }

        private IIntervencao<IntervencaoTecnica> _Intervencao = null;
        public IIntervencao<IntervencaoTecnica> Intervencao
        {
            get
            {
                if (_Intervencao == null)
                    _Intervencao = Provider != null ? RepositoryFactory.GetRepository<IIntervencao<IntervencaoTecnica>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IIntervencao<IntervencaoTecnica>>(NomeDaOrganizacao, IsOffline);

                return _Intervencao;
            }
        }

        private ILancamentoAvulsoDoExtrato<LancamentoAvulsoDoExtrato> _LancamentoAvulsoDoExtrato = null;
        public ILancamentoAvulsoDoExtrato<LancamentoAvulsoDoExtrato> LancamentoAvulsoDoExtrato
        {
            get
            {
                if (_LancamentoAvulsoDoExtrato == null)
                    _LancamentoAvulsoDoExtrato = Provider != null ? RepositoryFactory.GetRepository<ILancamentoAvulsoDoExtrato<LancamentoAvulsoDoExtrato>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILancamentoAvulsoDoExtrato<LancamentoAvulsoDoExtrato>>(NomeDaOrganizacao, IsOffline);

                return _LancamentoAvulsoDoExtrato;
            }
        }

        private ILinhaComercial<LinhaComercial> _LinhaComercial = null;
        public ILinhaComercial<LinhaComercial> LinhaComercial
        {
            get
            {
                if (_LinhaComercial == null)
                    _LinhaComercial = Provider != null ? RepositoryFactory.GetRepository<ILinhaComercial<LinhaComercial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILinhaComercial<LinhaComercial>>(NomeDaOrganizacao, IsOffline);

                return _LinhaComercial;
            }
        }

        private ILinhaDoContrato<LinhaDeContrato> _LinhaDoContrato = null;
        public ILinhaDoContrato<LinhaDeContrato> LinhaDoContrato
        {
            get
            {
                if (_LinhaDoContrato == null)
                    _LinhaDoContrato = Provider != null ? RepositoryFactory.GetRepository<ILinhaDoContrato<LinhaDeContrato>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILinhaDoContrato<LinhaDeContrato>>(NomeDaOrganizacao, IsOffline);

                return _LinhaDoContrato;
            }
        }

        private ILocalidade<Localidade> _Localidade = null;
        public ILocalidade<Localidade> Localidade
        {
            get
            {
                if (_Localidade == null)
                    _Localidade = Provider != null ? RepositoryFactory.GetRepository<ILocalidade<Localidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILocalidade<Localidade>>(NomeDaOrganizacao, IsOffline);

                return _Localidade;
            }
        }

        private ILogisticaReversa<LogisticaReversa> _LogisticaReversa = null;
        public ILogisticaReversa<LogisticaReversa> LogisticaReversa
        {
            get
            {
                if (_LogisticaReversa == null)
                    _LogisticaReversa = Provider != null ? RepositoryFactory.GetRepository<ILogisticaReversa<LogisticaReversa>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILogisticaReversa<LogisticaReversa>>(NomeDaOrganizacao, IsOffline);

                return _LogisticaReversa;
            }
        }

        private ILog<Log> _Log = null;
        public ILog<Log> Log
        {
            get
            {
                if (_Log == null)
                    _Log = Provider != null ? RepositoryFactory.GetRepository<ILog<Log>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILog<Log>>(NomeDaOrganizacao, IsOffline);

                return _Log;
            }
        }

        private IOcorrencia<Ocorrencia> _Ocorrencia = null;
        public IOcorrencia<Ocorrencia> Ocorrencia
        {
            get
            {
                if (_Ocorrencia == null)
                    _Ocorrencia = Provider != null ? RepositoryFactory.GetRepository<IOcorrencia<Ocorrencia>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOcorrencia<Ocorrencia>>(NomeDaOrganizacao, IsOffline);

                return _Ocorrencia;
            }
        }

        private IOcorrenciaBase<OcorrenciaBase> _OcorrenciaBase = null;
        public IOcorrenciaBase<OcorrenciaBase> OcorrenciaBase
        {
            get
            {
                if (_OcorrenciaBase == null)
                    _OcorrenciaBase = Provider != null ? RepositoryFactory.GetRepository<IOcorrenciaBase<OcorrenciaBase>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOcorrenciaBase<OcorrenciaBase>>(NomeDaOrganizacao, IsOffline);

                return _OcorrenciaBase;
            }
        }

        private IPagamentoServico<PagamentoServico> _PagamentoServico = null;
        public IPagamentoServico<PagamentoServico> PagamentoServico
        {
            get
            {
                if (_PagamentoServico == null)
                    _PagamentoServico = Provider != null ? RepositoryFactory.GetRepository<IPagamentoServico<PagamentoServico>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPagamentoServico<PagamentoServico>>(NomeDaOrganizacao, IsOffline);

                return _PagamentoServico;
            }
        }

        private IPontosDoParticipante<PontosDoParticipante> _PontosDoParticipante = null;
        public IPontosDoParticipante<PontosDoParticipante> PontosDoParticipante
        {
            get
            {
                if (_PontosDoParticipante == null)
                    _PontosDoParticipante = Provider != null ? RepositoryFactory.GetRepository<IPontosDoParticipante<PontosDoParticipante>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPontosDoParticipante<PontosDoParticipante>>(NomeDaOrganizacao, IsOffline);

                return _PontosDoParticipante;
            }
        }

        private IPremio<Premio> _Premio = null;
        public IPremio<Premio> Premio
        {
            get
            {
                if (_Premio == null)
                    _Premio = Provider != null ? RepositoryFactory.GetRepository<IPremio<Premio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPremio<Premio>>(NomeDaOrganizacao, IsOffline);

                return _Premio;
            }
        }

        private IPrivilegio<Privilegio> _Privilegio = null;
        public IPrivilegio<Privilegio> Privilegio
        {
            get
            {
                if (_Privilegio == null)
                    _Privilegio = Provider != null ? RepositoryFactory.GetRepository<IPrivilegio<Privilegio>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPrivilegio<Privilegio>>(NomeDaOrganizacao, IsOffline);

                return _Privilegio;
            }
        }

        private IRegional<Regional> _Regional = null;
        public IRegional<Regional> Regional
        {
            get
            {
                if (_Regional == null)
                    _Regional = Provider != null ? RepositoryFactory.GetRepository<IRegional<Regional>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRegional<Regional>>(NomeDaOrganizacao, IsOffline);

                return _Regional;
            }
        }

        private IRelacionamento<Relacionamento> _Relacionamento = null;
        public IRelacionamento<Relacionamento> Relacionamento
        {
            get
            {
                if (_Relacionamento == null)
                    _Relacionamento = Provider != null ? RepositoryFactory.GetRepository<IRelacionamento<Relacionamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRelacionamento<Relacionamento>>(NomeDaOrganizacao, IsOffline);

                return _Relacionamento;
            }
        }

        private IRelacionamentoCliente<RelacionamentoCliente> _RelacionamentoCliente = null;
        public IRelacionamentoCliente<RelacionamentoCliente> RelacionamentoCliente
        {
            get
            {
                if (_RelacionamentoCliente == null)
                    _RelacionamentoCliente = Provider != null ? RepositoryFactory.GetRepository<IRelacionamentoCliente<RelacionamentoCliente>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRelacionamentoCliente<RelacionamentoCliente>>(NomeDaOrganizacao, IsOffline);

                return _RelacionamentoCliente;
            }
        }

        private IFuncaoRelacionamento<FuncaoRelacionamento> _FuncaoRelacionamento = null;
        public IFuncaoRelacionamento<FuncaoRelacionamento> FuncaoRelacionamento
        {
            get
            {
                if (_FuncaoRelacionamento == null)
                    _FuncaoRelacionamento = Provider != null ? RepositoryFactory.GetRepository<IFuncaoRelacionamento<FuncaoRelacionamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFuncaoRelacionamento<FuncaoRelacionamento>>(NomeDaOrganizacao, IsOffline);

                return _FuncaoRelacionamento;
            }
        }

        private IResgateDePremio<ResgateDePremios> _ResgateDePremio = null;
        public IResgateDePremio<ResgateDePremios> ResgateDePremio
        {
            get
            {
                if (_ResgateDePremio == null)
                    _ResgateDePremio = Provider != null ? RepositoryFactory.GetRepository<IResgateDePremio<ResgateDePremios>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IResgateDePremio<ResgateDePremios>>(NomeDaOrganizacao, IsOffline);

                return _ResgateDePremio;
            }
        }

        private IProcessamentoFidelidade<ProcessamentoFidelidade> _FidelidadeProcessamento = null;
        public IProcessamentoFidelidade<ProcessamentoFidelidade> FidelidadeProcessamento
        {
            get
            {
                if (_FidelidadeProcessamento == null)
                    _FidelidadeProcessamento = Provider != null ? RepositoryFactory.GetRepository<IProcessamentoFidelidade<ProcessamentoFidelidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProcessamentoFidelidade<ProcessamentoFidelidade>>(NomeDaOrganizacao, IsOffline);

                return _FidelidadeProcessamento;
            }
        }

        private IResgate<Resgate> _Resgate = null;
        public IResgate<Resgate> Resgate
        {
            get
            {
                if (_Resgate == null)
                    _Resgate = Provider != null ? RepositoryFactory.GetRepository<IResgate<Resgate>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IResgate<Resgate>>(NomeDaOrganizacao, IsOffline);

                return _Resgate;
            }
        }

        private IValorDoServico<ValorDoServico> _ValorDoServico = null;
        public IValorDoServico<ValorDoServico> ValorDoServico
        {
            get
            {
                if (_ValorDoServico == null)
                    _ValorDoServico = Provider != null ? RepositoryFactory.GetRepository<IValorDoServico<ValorDoServico>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IValorDoServico<ValorDoServico>>(NomeDaOrganizacao, IsOffline);

                return _ValorDoServico;
            }
        }

        private IValorDoServicoPorPosto<ValorDoServicoPorPosto> _ValorDoServicoPorPosto = null;
        public IValorDoServicoPorPosto<ValorDoServicoPorPosto> ValorDoServicoPorPosto
        {
            get
            {
                if (_ValorDoServicoPorPosto == null)
                    _ValorDoServicoPorPosto = Provider != null ? RepositoryFactory.GetRepository<IValorDoServicoPorPosto<ValorDoServicoPorPosto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IValorDoServicoPorPosto<ValorDoServicoPorPosto>>(NomeDaOrganizacao, IsOffline);

                return _ValorDoServicoPorPosto;
            }
        }

        private IOportunidade<Oportunidade> _Oportunidade = null;
        public IOportunidade<Oportunidade> Oportunidade
        {
            get
            {
                if (_Oportunidade == null)
                    _Oportunidade = Provider != null ? RepositoryFactory.GetRepository<IOportunidade<Oportunidade>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IOportunidade<Oportunidade>>(NomeDaOrganizacao, IsOffline);

                return _Oportunidade;
            }
        }

        private IDefeitoOcorrenciaCliente<DefeitoOcorrenciaCliente> _DefeitoOcorrenciaCliente = null;
        public IDefeitoOcorrenciaCliente<DefeitoOcorrenciaCliente> DefeitoOcorrenciaCliente
        {
            get
            {
                if (_DefeitoOcorrenciaCliente == null)
                    _DefeitoOcorrenciaCliente = Provider != null ? RepositoryFactory.GetRepository<IDefeitoOcorrenciaCliente<DefeitoOcorrenciaCliente>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IDefeitoOcorrenciaCliente<DefeitoOcorrenciaCliente>>(NomeDaOrganizacao, IsOffline);

                return _DefeitoOcorrenciaCliente;
            }
        }

        private ITipoPosto<TipoPosto> _TipoPosto = null;
        public ITipoPosto<TipoPosto> TipoPosto
        {
            get
            {
                if (_TipoPosto == null)
                    _TipoPosto = Provider != null ? RepositoryFactory.GetRepository<ITipoPosto<TipoPosto>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITipoPosto<TipoPosto>>(NomeDaOrganizacao, IsOffline);

                return _TipoPosto;
            }
        }

        private IFeriado<Feriado> _Feriado = null;
        public IFeriado<Feriado> Feriado
        {
            get
            {
                if (_Feriado == null)
                    _Feriado = Provider != null ? RepositoryFactory.GetRepository<IFeriado<Feriado>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IFeriado<Feriado>>(NomeDaOrganizacao, IsOffline);

                return _Feriado;
            }
        }

        private IPostagem<Postagem> _Postagem = null;
        public IPostagem<Postagem> Postagem
        {
            get
            {
                if (_Postagem == null)
                    _Postagem = Provider != null ? RepositoryFactory.GetRepository<IPostagem<Postagem>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IPostagem<Postagem>>(NomeDaOrganizacao, IsOffline);

                return _Postagem;
            }
        }

        private IMarca<Marca> _Marca = null;
        public IMarca<Marca> Marca
        {
            get
            {
                if (_Marca == null)
                    _Marca = Provider != null ? RepositoryFactory.GetRepository<IMarca<Marca>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMarca<Marca>>(NomeDaOrganizacao, IsOffline);

                return _Marca;
            }
        }

        private IModelo<Modelo> _Modelo = null;
        public IModelo<Modelo> Modelo
        {
            get
            {
                if (_Modelo == null)
                    _Modelo = Provider != null ? RepositoryFactory.GetRepository<IModelo<Modelo>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IModelo<Modelo>>(NomeDaOrganizacao, IsOffline);

                return _Modelo;
            }
        }

        private ITipoPagamento<TipoPagamento> _TipoPagamento = null;
        public ITipoPagamento<TipoPagamento> TipoPagamento
        {
            get
            {
                if (_TipoPagamento == null)
                    _TipoPagamento = Provider != null ? RepositoryFactory.GetRepository<ITipoPagamento<TipoPagamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ITipoPagamento<TipoPagamento>>(NomeDaOrganizacao, IsOffline);

                return _TipoPagamento;
            }
        }

        private IAreaAtuacao<AreaAtuacao> _AreaAtuacao = null;
        public IAreaAtuacao<AreaAtuacao> AreaAtuacao
        {
            get
            {
                if (_AreaAtuacao == null)
                    _AreaAtuacao = Provider != null ? RepositoryFactory.GetRepository<IAreaAtuacao<AreaAtuacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAreaAtuacao<AreaAtuacao>>(NomeDaOrganizacao, IsOffline);

                return _AreaAtuacao;        
            }
        }

        private IUtil<Object> _Util = null;
        public IUtil<Object> Util
        {
            get
            {
                if (_Util == null)
                    _Util = Provider != null ? RepositoryFactory.GetRepository<IUtil<Object>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IUtil<Object>>(NomeDaOrganizacao, IsOffline);

                return _Util;
            }
        }

        private ISegmentoComercial<SegmentoComercial> _SegmentoComercial = null;
        public ISegmentoComercial<SegmentoComercial> SegmentoComercial
        {
            get
            {
                if (_SegmentoComercial == null)
                    _SegmentoComercial = Provider != null ? RepositoryFactory.GetRepository<ISegmentoComercial<SegmentoComercial>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISegmentoComercial<SegmentoComercial>>(NomeDaOrganizacao, IsOffline);

                return _SegmentoComercial;
            }
        }
        private IQuestionarioPergunta<QuestionarioPergunta> _QuestionarioPergunta = null;
        public IQuestionarioPergunta<QuestionarioPergunta> QuestionarioPergunta
        {
            get
            {
                if (_QuestionarioPergunta == null)
                    _QuestionarioPergunta = Provider != null ? RepositoryFactory.GetRepository<IQuestionarioPergunta<QuestionarioPergunta>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IQuestionarioPergunta<QuestionarioPergunta>>(NomeDaOrganizacao, IsOffline);

                return _QuestionarioPergunta;
            }
        }
        private IQuestionarioGrupoPergunta<QuestionarioGrupoPergunta> _QuestionarioGrupoPergunta = null;
        public IQuestionarioGrupoPergunta<QuestionarioGrupoPergunta> QuestionarioGrupoPergunta
        {
            get
            {
                if (_QuestionarioGrupoPergunta == null)
                    _QuestionarioGrupoPergunta = Provider != null ? RepositoryFactory.GetRepository<IQuestionarioGrupoPergunta<QuestionarioGrupoPergunta>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IQuestionarioGrupoPergunta<QuestionarioGrupoPergunta>>(NomeDaOrganizacao, IsOffline);

                return _QuestionarioGrupoPergunta;
            }
        }
        private IQuestionarioOpcao<QuestionarioOpcao> _QuestionarioOpcao = null;
        public IQuestionarioOpcao<QuestionarioOpcao> QuestionarioOpcao
        {
            get
            {
                if (_QuestionarioOpcao == null)
                    _QuestionarioOpcao = Provider != null ? RepositoryFactory.GetRepository<IQuestionarioOpcao<QuestionarioOpcao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IQuestionarioOpcao<QuestionarioOpcao>>(NomeDaOrganizacao, IsOffline);

                return _QuestionarioOpcao;
            }
        }
        private IQuestionarioResposta<QuestionarioResposta> _QuestionarioResposta = null;
        public IQuestionarioResposta<QuestionarioResposta> QuestionarioResposta
        {
            get
            {
                if (_QuestionarioResposta == null)
                    _QuestionarioResposta = Provider != null ? RepositoryFactory.GetRepository<IQuestionarioResposta<QuestionarioResposta>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IQuestionarioResposta<QuestionarioResposta>>(NomeDaOrganizacao, IsOffline);

                return _QuestionarioResposta;
            }
        }
        private ISinonimosMarcas<SinonimosMarcas> _SinonimosMarcas = null;
        public ISinonimosMarcas<SinonimosMarcas> SinonimosMarcas
        {
            get
            {
                if (_SinonimosMarcas == null)
                    _SinonimosMarcas = Provider != null ? RepositoryFactory.GetRepository<ISinonimosMarcas<SinonimosMarcas>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISinonimosMarcas<SinonimosMarcas>>(NomeDaOrganizacao, IsOffline);

                return _SinonimosMarcas;
            }
        }
        private IMarcas<Marcas> _Marcas = null;
        public IMarcas<Marcas> Marcas
        {
            get
            {
                if (_Marcas == null)
                    _Marcas = Provider != null ? RepositoryFactory.GetRepository<IMarcas<Marcas>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IMarcas<Marcas>>(NomeDaOrganizacao, IsOffline);

                return _Marcas;
            }
        }
        private ISegmentoComercialConta<SegmentoComercialConta> _SegmentoComercialConta = null;
        public ISegmentoComercialConta<SegmentoComercialConta> SegmentoComercialConta
        {
            get
            {
                if (_SegmentoComercialConta == null)
                    _SegmentoComercialConta = Provider != null ? RepositoryFactory.GetRepository<ISegmentoComercialConta<SegmentoComercialConta>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ISegmentoComercialConta<SegmentoComercialConta>>(NomeDaOrganizacao, IsOffline);

                return _SegmentoComercialConta;
            }
        }

        private ILiveChatTracking<LiveChatTracking> _LiveChatTracking = null;
        public ILiveChatTracking<LiveChatTracking> LiveChatTracking
        {
            get
            {
                if (_LiveChatTracking == null)
                    _LiveChatTracking = Provider != null ? RepositoryFactory.GetRepository<ILiveChatTracking<LiveChatTracking>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ILiveChatTracking<LiveChatTracking>>(NomeDaOrganizacao, IsOffline);

                return _LiveChatTracking;
            }
        }

        private ICausa<Causa> _Causa = null;
        public ICausa<Causa> Causa
        {
            get
            {
                if (_Causa == null)
                    _Causa = Provider != null ? RepositoryFactory.GetRepository<ICausa<Causa>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<ICausa<Causa>>(NomeDaOrganizacao, IsOffline);

                return _Causa;
            }
        }

        private IAcao<Acao> _Acao = null;
        public IAcao<Acao> Acao
        {
            get
            {
                if (_Acao == null)
                    _Acao = Provider != null ? RepositoryFactory.GetRepository<IAcao<Acao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAcao<Acao>>(NomeDaOrganizacao, IsOffline);

                return _Acao;
            }
        }

        private IProdutoCondicaoPagamento<ProdutoCondicaoPagamento> _ProdutoCondicaoPagamento = null;
        public IProdutoCondicaoPagamento<ProdutoCondicaoPagamento> ProdutoCondicaoPagamento
        {
            get
            {
                if (_ProdutoCondicaoPagamento == null)
                    _ProdutoCondicaoPagamento = Provider != null ? RepositoryFactory.GetRepository<IProdutoCondicaoPagamento<ProdutoCondicaoPagamento>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IProdutoCondicaoPagamento<ProdutoCondicaoPagamento>>(NomeDaOrganizacao, IsOffline);

                return _ProdutoCondicaoPagamento;
            }
        }

        private IVeiculo<Veiculo> _veiculo = null;
        public IVeiculo<Veiculo> Veiculo
        {
            get
            {
                if (_veiculo == null)
                    _veiculo = Provider != null ? RepositoryFactory.GetRepository<IVeiculo<Veiculo>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IVeiculo<Veiculo>>(NomeDaOrganizacao, IsOffline);

                return _veiculo;
            }
        }


        private IRegiaoDeAtuacao<RegiaoDeAtuacao> _RegiaoDeAtuacao = null;
        public IRegiaoDeAtuacao<RegiaoDeAtuacao> RegiaoDeAtuacao
        {
            get
            {
                if (_RegiaoDeAtuacao == null)
                    _RegiaoDeAtuacao = Provider != null ? RepositoryFactory.GetRepository<IRegiaoDeAtuacao<RegiaoDeAtuacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IRegiaoDeAtuacao<RegiaoDeAtuacao>>(NomeDaOrganizacao, IsOffline);

                return _RegiaoDeAtuacao;
            }
        }

        private IHistoricoCategoria<HistoricoCategoria> _HistoricoCategoria = null;
        public IHistoricoCategoria<HistoricoCategoria> HistoricoCategoria
        {
            get
            {
                if (_HistoricoCategoria == null)
                    _HistoricoCategoria = Provider != null ? RepositoryFactory.GetRepository<IHistoricoCategoria<HistoricoCategoria>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IHistoricoCategoria<HistoricoCategoria>>(NomeDaOrganizacao, IsOffline);

                return _HistoricoCategoria;
            }
        }

        private IAnotacao<Anotacao> _Anotacao = null;
        public IAnotacao<Anotacao> Anotacao
        {
            get
            {
                if (_Anotacao == null)
                    _Anotacao = Provider != null ? RepositoryFactory.GetRepository<IAnotacao<Anotacao>>(NomeDaOrganizacao, IsOffline, Provider) : RepositoryFactory.GetRepository<IAnotacao<Anotacao>>(NomeDaOrganizacao, IsOffline);

                return _Anotacao;
            }
        }
        #endregion

    }
}
