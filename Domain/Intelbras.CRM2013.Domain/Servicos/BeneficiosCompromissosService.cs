using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using SDKore.Helper;
using System.Net;
using System.IO;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class BeneficiosCompromissosService
    {

        #region Objetos
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private string emailAEnviarLog = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail");
        private Boolean isOffline = false;

        public SDKore.Helper.Trace Trace { get; set; }
        private List<string> mensagemLog = new List<string>();
        public int cont_ant = 1;


        string usuarioSharePoint = SDKore.Helper.Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("UsuarioSharePoint"));
        string senhaSharePoint = SDKore.Helper.Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("SenhaSharePoint"));
        private string ConfigOrganizacaoIntelbras { get { return SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"); } }

        private StatusCompromissos _statusCompromissoCumprido;
        public StatusCompromissos StatusCompromissoCumprido
        {
            get
            {
                if (_statusCompromissoCumprido == null)
                {
                    _statusCompromissoCumprido = StatusCompromissoService.ObterPorNome(Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido);
                }

                return _statusCompromissoCumprido;
            }
        }

        private StatusCompromissos _statusCompromissoNaoCumprido;
        public StatusCompromissos StatusCompromissoNaoCumprido
        {
            get
            {
                if (_statusCompromissoNaoCumprido == null)
                {
                    _statusCompromissoNaoCumprido = StatusCompromissoService.ObterPorNome(Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido);
                }

                return _statusCompromissoNaoCumprido;
            }
        }

        ContaService _Conta = null;
        private ContaService ContaService
        {
            get
            {
                if (_Conta == null)
                    _Conta = new ContaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _Conta;
            }
        }
        CategoriaCanalService _CategoriaCanal = null;
        private CategoriaCanalService CategoriaCanal
        {
            get
            {
                if (_CategoriaCanal == null)
                    _CategoriaCanal = new CategoriaCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _CategoriaCanal;
            }
        }

        ParametroGlobalService _ParametroGlobal = null;
        private ParametroGlobalService ParametroGlobal
        {
            get
            {
                if (_ParametroGlobal == null)
                    _ParametroGlobal = new ParametroGlobalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _ParametroGlobal;
            }
        }
        HistoricoComprasCanalService _HistoricoComprasCanal = null;
        private HistoricoComprasCanalService HistoricoComprasCanal
        {
            get
            {
                if (_HistoricoComprasCanal == null)
                    _HistoricoComprasCanal = new HistoricoComprasCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _HistoricoComprasCanal;
            }
        }

        MetadoCanalService _MetaDoCanal = null;
        private MetadoCanalService MetaDoCanal
        {
            get
            {
                if (_MetaDoCanal == null)
                    _MetaDoCanal = new MetadoCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _MetaDoCanal;
            }
        }

        CompromissosDoCanalService _CompromissosDoCanal = null;
        private CompromissosDoCanalService CompromissoDoCanal
        {
            get
            {
                if (_CompromissosDoCanal == null)
                    _CompromissosDoCanal = new CompromissosDoCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _CompromissosDoCanal;
            }
        }

        StatusCompromissoService _StatusCompromissos = null;
        private StatusCompromissoService StatusCompromissoService
        {
            get
            {
                if (_StatusCompromissos == null)
                    _StatusCompromissos = new StatusCompromissoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _StatusCompromissos;
            }
        }

        LinhaCorteService _LinhaCorteService = null;
        private LinhaCorteService LinhaCorteService
        {
            get
            {
                if (_LinhaCorteService == null)
                    _LinhaCorteService = new LinhaCorteService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _LinhaCorteService;
            }
        }

        TreinamentoService _TreinamentoService = null;
        private TreinamentoService TreinamentoService
        {
            get
            {
                if (_TreinamentoService == null)
                    _TreinamentoService = new TreinamentoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _TreinamentoService;
            }
        }

        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public BeneficiosCompromissosService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public BeneficiosCompromissosService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Lookup BuscarBeneficioCorrespondentePorCodigoStatus(BeneficiosCompromissos BenefCompro, Guid status)
        {
            if (BenefCompro.Status1Compromisso != null && BenefCompro.Status1Compromisso.Id == status)
            {
                return BenefCompro.Status1Beneficio;
            }
            else if (BenefCompro.Status2Compromisso != null && BenefCompro.Status2Compromisso.Id == status)
            {
                return BenefCompro.Status2Beneficio;
            }
            else if (BenefCompro.Status3Compromisso != null && BenefCompro.Status3Compromisso.Id == status)
            {
                return BenefCompro.Status3Beneficio;
            }
            else if (BenefCompro.Status4Compromisso != null && BenefCompro.Status4Compromisso.Id == status)
            {
                return BenefCompro.Status4Beneficio;
            }

            return null;
        }

        public List<BeneficiosCompromissos> BuscaBeneficiosCompromissos(Guid Perfil, Guid? Compromisso, Guid? Beneficio)
        {
            List<BeneficiosCompromissos> lstBeneficioCompromisso = RepositoryService.BeneficioCompromisso.ListarPor(Perfil, Compromisso, Beneficio);

            if (lstBeneficioCompromisso.Count() == 0)
                return null;
            else
                return lstBeneficioCompromisso;
        }

        #endregion

        public bool HojeDiaExecutarMonitoramentoAutomatico()
        {
            ParametroGlobal paramGlobal = ParametroGlobal.ListarParamGlobalPorTipoParam((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.DatasTrimestre).FirstOrDefault();

            if (paramGlobal == null)
            {
                throw new ApplicationException("A execução do monitoramento foi interrompida, o parâmetro global não foi encontrado ou está preenchido com valores incorretos.");
            }

            var lista = ConverterParametroParaLista(paramGlobal.Valor);

            foreach (var item in lista)
            {
                if (item == DateTime.Today)
                {
                    return true;
                }
            }

            return false;
        }

        private List<DateTime> ConverterParametroParaLista(string listaTrimestres)
        {
            try
            {
                return listaTrimestres.Split(';').ToList<string>().ConvertAll<DateTime>(x => Convert.ToDateTime(x));
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Não foi possível converter em data", ex);
            }
        }
        
        private void AtualizarCompromissoCanalInconsistente(string mensagem, CompromissosDoCanal compCanal, Conta canal)
        {
            compCanal.StatusCompromisso = new Lookup(StatusCompromissoNaoCumprido.ID.Value, "");
            CompromissoDoCanal.Atualizar(compCanal);
            InserirLog(mensagem);
        }

        private List<Guid> RefinaUnidadeNegocio(List<CategoriasCanal> lstCat)
        {
            List<Guid> unidadeNegocio = new List<Guid>();

            foreach (var categoria in lstCat)
            {
                if (!unidadeNegocio.Contains(categoria.UnidadeNegocios.Id))
                    unidadeNegocio.Add(categoria.UnidadeNegocios.Id);
            }
            return unidadeNegocio;
        }

        ///////////////////// CASO DE USO 2 e 3 APURAÇÃO DE BENEFICIO E COMPROMISSO POR FILIAL E POR MATRIZ
        public void MonitoramntoAutomaticoParaApuracaoDeCompromissosEBaneficiosPorFilialEMatriz()
        {
            Trace = new SDKore.Helper.Trace("MonitoramentoAutomatico");
            mensagemLog = new List<string>();
            string dataProc = DateTime.Now.ToString() + " - ";

            #region Validando Parametros

            if (StatusCompromissoCumprido == null)
            {
                throw new ApplicationException("A execução do monitoramento foi interrompida, não foi encontrado o Status do Compromisso: " + Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido);
            }

            if (StatusCompromissoNaoCumprido == null)
            {
                throw new ApplicationException("A execução do monitoramento foi interrompida, não foi encontrado o Status do Compromisso: " + Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido);
            }

            ParametroGlobal dataTimestre = ParametroGlobal.ListarParamGlobalPorTipoParam((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.DatasTrimestre).FirstOrDefault();

            if (dataTimestre == null)
            {
                throw new ApplicationException("A execução do monitoramento foi interrompida, o parâmetro global não foi encontrado ou está preenchido com valores incorretos.");
            }

            CompromissosDoPrograma compProgMeta = CompromissoDoCanal.BuscarCompromissoDoPrograma((int)Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.Codigo.MetaTrimestral);
            if (compProgMeta == null)
                throw new ApplicationException("Não foi possível encontrar o compromisso do programa de MetasTrimestrais");

            CompromissosDoPrograma compromissoTecnicoTreinadoCertificado = CompromissoDoCanal.BuscarCompromissoDoPrograma((int)Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.Codigo.TecnicoTreinadoCertificado);
            if (compromissoTecnicoTreinadoCertificado == null)
                throw new ApplicationException("Compromisso tecnico treinado certificado não encontrado");

            CompromissosDoPrograma compProgLinhaCorte = CompromissoDoCanal.BuscarCompromissoDoPrograma((int)Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.Codigo.LinhaCorteTrimestral);
            if (compProgLinhaCorte == null)
                throw new ApplicationException("Não foi possível encontrar o compromisso do programa de Linhas de cortes");

            #endregion

            DateTime ultimoDiaDoUltimoTrimestre = new SDKore.Helper.DateTimeHelper().UltimoDiaDoUltimoTrimestre();
            int ano = ultimoDiaDoUltimoTrimestre.Year;
            int trimestre = (ultimoDiaDoUltimoTrimestre.Month - 1) / 3 + 1;
            var trimestreOrcamentodaUnidade = Helper.ConverterTrimestreOrcamentoUnidade(trimestre);

            // Obtem lista de canais matriz ou filial // (CASO USO 2: STEP 2)
            List<Conta> lstContas = ContaService.ListarContasParticipantesMAtrizEFilial();

            // (CASO USO 2: STEP 2)
            foreach (Conta canal in lstContas)
            {
                if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial &&
                    canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz)
                    continue;


                // LISTA DE CATEGORIAS DO CANAL // (CASO USO 2: STEP 4)
                List<CategoriasCanal> lstCat = CategoriaCanal.ListarPor(canal.ID, null);

                // caso seja um canal matriz obtem lista de suas filiais // (CASO USO 2: STEP 4)
                List<Conta> listaFiliais = ContaService.ListarContasFiliaisPorMatriz(canal.ID.Value);

                listaFiliais = listaFiliais.Where(x => x.ParticipantePrograma.HasValue 
                                                    && x.ParticipantePrograma.Value == (int)Enum.Conta.ParticipaDoPrograma.Sim).ToList();

                // refina lista de unidade de negocio por categoria // (CASO USO 2: STEP 4)
                List<Guid> unidadesDeNegocio = this.RefinaUnidadeNegocio(lstCat);

                // (CASO USO 2: STEP 5 INICIANDO)
                foreach (var unNeg in unidadesDeNegocio)
                {
                    HistoricoCompraCanal histCanal = new HistoricoCompraCanal(OrganizationName, isOffline);
                    MetadoCanal metaCanal = new MetadoCanal(OrganizationName, isOffline);
                    decimal valorHistorico = 0, metaPlanejada = 0;

                    CompromissosDoCanal compCanal = CompromissoDoCanal.BuscarCompromissoCanal(compProgMeta.ID.Value, unNeg, canal.ID.Value);

                    if (compCanal == null)
                    {
                        continue;
                    }

                    // SE FOR UM CANAL CENTRALIZADO NA MATRI OBTEM SUAS FILIAIS 
                    // ESSA INTERAÇÃO GARANTE QUE NO CASO DE UM CANAL SER UMA MATRIZ, O CASO USO 3: STEP 6,7,8 SERA ORRETAMENTE CONTEMPLADO
                    // VISTO QUE O VALOR DO HISTORICO ESTA SENDO SOMADO CASO HAJA FILIAIS PARA O CANAL CORRENTE

                    foreach (Intelbras.CRM2013.Domain.Model.Conta filial in listaFiliais)
                    {
                        if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Matriz &&
                            canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
                            continue;

                        List<CategoriasCanal> lstCatFiliais = CategoriaCanal.ListarPor(filial.ID, null);
                        foreach (var categoria in lstCatFiliais)
                        {
                            if (categoria.UnidadeNegocios != null && categoria.UnidadeNegocios.Id == unNeg)
                            {
                                histCanal = null;
                                metaCanal = null;

                                // (CASO USO 3: STEP 6) hISTORICO DE COMPRA DAS FILIAIS, CASO CANAL SEJA CENTRALIZADO NA MATRI
                                histCanal = RepositoryService.HistoricoCompraCanal.ObterPor(categoria.UnidadeNegocios.Id, (int)trimestreOrcamentodaUnidade, ano, filial.ID.Value);

                                // so soma se for diferente do proprio canal pois posteriormente é somado o canal corrente
                                if (histCanal != null && histCanal.Valor.HasValue)
                                    valorHistorico += histCanal.Valor.Value;
                            }
                        }
                    }

                    #region VALIDANDO BENEFICIO DE META x HISTORICO
                    // EM CASO DE MATRIZ O VALOR DO HISTORICO É CALCULADO PARA TODAS A FILIAIS
                    // (CASO USO 3: STEP 6)
                    histCanal = RepositoryService.HistoricoCompraCanal.ObterPor(unNeg, (int)trimestreOrcamentodaUnidade, ano, canal.ID.Value);

                    // (CASO USO 2: STEP 6) e // (CASO USO 3: STEP 9)
                    metaCanal = MetaDoCanal.ObterPor(unNeg, (int)trimestreOrcamentodaUnidade, canal.ID.Value, ano);

                    Trace.Add(dataProc+"Obter Meta do Canal - Un [{0}] Trimeste [{1}] Canal [{2}] Ano [{3}] Encontrou? [{4}]", unNeg, trimestre, canal.ID.Value, ano, (metaCanal != null));

                    if (metaCanal == null || !metaCanal.MetaPlanejada.HasValue || metaCanal.MetaPlanejada.Value == 0)
                    {
                        UnidadeNegocio un = RepositoryService.UnidadeNegocio.Retrieve(unNeg);
                        string mensagem = string.Format("Não foi possível obter a Meta do Canal para Canal [{0}] e Unidade de Negócio [{1}]", canal.CodigoMatriz, un.Nome);
                        this.AtualizarCompromissoCanalInconsistente(mensagem, compCanal, canal);
                    }
                    else
                    {
                        if (histCanal != null && histCanal.Valor.HasValue)
                        {
                            valorHistorico += histCanal.Valor.Value;
                        }

                        // (CASO USO 2: STEP 7) ou // (CASO USO 3: STEP 10)
                        #region CALCULO DE HITORICO X META E BAIXA EM COMPROMISSO

                        metaPlanejada = metaCanal.MetaPlanejada.Value;

                        StatusCompromissos statusCompromissoMeta = (valorHistorico >= metaPlanejada)
                            ? StatusCompromissoCumprido
                            : StatusCompromissoNaoCumprido;

                        if (statusCompromissoMeta != null)
                        {
                            compCanal.StatusCompromisso = new Lookup(statusCompromissoMeta.ID.Value, "");
                            compCanal.Validade = DateTime.Now.Date.AddMonths(3);
                            CompromissoDoCanal.Atualizar(compCanal);
                        }

                        #endregion
                    }

                    #endregion

                    #region VALIDANDO BENEFICIO DE LINHA DE CORTE
                    valorHistorico = 0;
                    decimal linhaCorte = 0;
                    Lookup estadoCanal = canal.Endereco1Estadoid;
                    Lookup classCanal = canal.Classificacao;
                    StatusCompromissos statusComp = null;

                    // compromisso do canal para linha de corte
                    CompromissosDoCanal compCanalLinCorte = CompromissoDoCanal.BuscarCompromissoCanal(compProgLinhaCorte.ID.Value, unNeg, canal.ID.Value);

                    #region VALIDAÇÕES PARA LOG
                    if (compCanal == null)
                            InserirLog(dataProc+"Não foi possível encontrar o compromisso do canal de Linha de Corte para o canal : " + canal.ID.Value.ToString());

                    if (canal.Endereco1Estadoid == null)
                            InserirLog(dataProc+"Monitoramento de linhas de corte não realizado para o canal : " + canal.ID.Value.ToString() + " Estado do canal não preenchido.");

                    if (canal.Classificacao == null)
                            InserirLog(dataProc+"Monitoramento de linhas de corte não realizado para o canal : " + canal.ID.Value.ToString() + " Classificação do canal não preenchido.");
                    #endregion

                    #region PARA AS FILIAIS DA MATRIZ
                    // EM CASO DE MATRIZ O VALOR DO HISTORICO X LINHA DE CORTE É CALCULADO PARA TODAS A FILIAIS
                    // (CASO USO 3: STEP 6)

                    foreach (Intelbras.CRM2013.Domain.Model.Conta filial in listaFiliais)
                    {
                        if (canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Matriz &&
                            canal.ApuracaoBeneficiosCompromissos == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais)
                            continue;

                        histCanal = null;
                        CRM2013.Domain.Model.Estado estado = new Intelbras.CRM2013.Domain.Model.Estado(this.OrganizationName, this.isOffline);
                        CRM2013.Domain.Model.Municipio municipio = new Intelbras.CRM2013.Domain.Model.Municipio(this.OrganizationName, this.isOffline);
                        CRM2013.Domain.Model.Categoria _categoriaCanal = new Intelbras.CRM2013.Domain.Model.Categoria(this.OrganizationName, this.isOffline);
                        List<Guid> lstUnidade = new List<Guid>();
                        List<CategoriasCanal> lstCatFiliais = new List<CategoriasCanal>();

                        if (canal.Endereco1Municipioid == null)
                        {
                            InserirLogFormat("O Município do Canal [{0}] não está preenchido.", canal.CodigoMatriz);
                        }

                        // (CASO USO 3: STEP 6)
                        histCanal = RepositoryService.HistoricoCompraCanal.ObterPor(unNeg, (int)trimestreOrcamentodaUnidade, ano, filial.ID.Value);

                        if (histCanal != null && histCanal.Valor.HasValue)
                            valorHistorico += histCanal.Valor.Value;

                        lstCatFiliais = CategoriaCanal.ListarPor(filial.ID, null);
                        foreach (var categoria in lstCatFiliais)
                        {
                            if (categoria.UnidadeNegocios != null && categoria.UnidadeNegocios.Id == unNeg)
                            {
                                // (CASO USO 3: STEP 12)
                                if (classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover ||
                                    classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD)
                                {
                                    lstUnidade = new List<Guid>();
                                    lstUnidade.Add(categoria.UnidadeNegocios.Id);
                                    estado.ID = estadoCanal.Id;

                                    // (CASO USO 3: STEP 11) MUNICIPIO DO CANAL
                                    municipio = new Intelbras.CRM2013.Domain.Servicos.EnderecoServices(this.OrganizationName, this.isOffline).ObterMunicipio(canal.Endereco1Municipioid.Id);

                                    List<LinhaCorteDistribuidor> listaLinhaCorte = LinhaCorteService.ListarLinhadeCorteDistribuidor(lstUnidade, estado, municipio.CapitalOuInterior);

                                    //(CASO USO 3: STEP 14)
                                    if (listaLinhaCorte == null || listaLinhaCorte.Count == 0)
                                    {
                                        InserirLog(dataProc+"Linha de corte não encontrada para a filial: " + filial.CodigoMatriz);
                                        continue;
                                    }

                                    foreach (var item in listaLinhaCorte)
                                    {

                                        //(CASO USO 3: STEP 12)
                                        //Se achar mais de uma linha de corte, verifica se o campo capitalOuInterior é igual ao valor do municipio ligado ao canal
                                        if ((listaLinhaCorte.Count > 1 && item.CapitalOuInterior == municipio.CapitalOuInterior) ||
                                            listaLinhaCorte.Count == 1)
                                            linhaCorte += item.LinhaCorteTrimestral.Value;

                                        //(CASO USO 3: STEP 14)
                                        if (!item.LinhaCorteTrimestral.HasValue)
                                                InserirLog(dataProc+"Linha de corte não encontrada para a filial : " + filial.ID.Value.ToString());
                                        //this.AtualizarCompromissoCanalInconsistente("Linha de corte não possui valor para a filial : : " + filial.ID.Value.ToString() + " do canal : ", compCanal, canal);

                                    }
                                }

                                //SE FOR REVENDA TRANSACIONAL OU RELACIONAL // (CASO USO 3: STEP 17)
                                if (classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Rev_Rel ||
                                    classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Rev_Trans)
                                {
                                    foreach (var item in lstCat)
                                    {
                                        lstUnidade = new List<Guid>();
                                        lstUnidade.Add(unNeg);
                                        _categoriaCanal.ID = item.ID;

                                        List<LinhaCorteRevenda> listaLinhaCorteRevenda = LinhaCorteService.ListarLinhadeCorteRevenda(lstUnidade, _categoriaCanal);
                                        //(CASO USO 3: STEP 14)
                                        if (listaLinhaCorteRevenda == null || listaLinhaCorteRevenda.Count == 0)
                                                InserirLog(dataProc+"Linha de corte não encontrada para a filial : " + filial.ID.Value.ToString());
                                        //this.AtualizarCompromissoCanalInconsistente("Linha de corte não encontrada para a filial : " + filial.ID.Value.ToString() + " do canal : ", compCanal, canal);

                                        foreach (var _linhaCorteRevenda in listaLinhaCorteRevenda)
                                        {
                                            //(CASO USO 3: STEP 19)
                                            if (!_linhaCorteRevenda.LinhaCorteTrimestral.HasValue)
                                                    InserirLog(dataProc+"Linha de corte não encontrada para a filial : " + filial.ID.Value.ToString());
                                            //this.AtualizarCompromissoCanalInconsistente("Linha de corte não possui valor para a filial : : " + filial.ID.Value.ToString() + " do canal : ", compCanal, canal);

                                            linhaCorte += _linhaCorteRevenda.LinhaCorteTrimestral.Value;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    #endregion

                    // OBTEM HISTORICO DE CANAL
                    histCanal = RepositoryService.HistoricoCompraCanal.ObterPor(unNeg, (int)trimestreOrcamentodaUnidade, ano, canal.ID.Value);

                    // SE TIVER HISTORICO SOMA COM VARIAVEL SOMADORA valorHistorico
                    if (histCanal != null && histCanal.Valor.HasValue)
                        valorHistorico += histCanal.Valor.Value;

                    #region BOXMOVER OU VAD
                    //(CASO USO 2: STEP 11)
                    if (classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_BoxMover ||
                            classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Dist_VAD)
                    {
                        LinhaCorteDistribuidor objLinhaCorte = (LinhaCorteDistribuidor)LinhaCorteService.ObterLinhaCorte(unNeg, estadoCanal.Id, null, "distribuidor");
                        if (objLinhaCorte == null || objLinhaCorte.LinhaCorteTrimestral == null)
                                InserirLog(dataProc+"Linha de corte não encontrada para a canal : " + canal.ID.Value.ToString());
                        //this.AtualizarCompromissoCanalInconsistente("Linha de corte não encontrada para o canal : ", compCanal, canal);

                        if (!objLinhaCorte.LinhaCorteTrimestral.HasValue)
                                InserirLog(dataProc+"Linha de corte não encontrada para a canal : " + canal.ID.Value.ToString());
                        //this.AtualizarCompromissoCanalInconsistente("Linha de corte não possui valor para o canal : ", compCanal, canal);

                        linhaCorte += objLinhaCorte.LinhaCorteTrimestral.Value;

                    }
                    #endregion

                    #region RELACIONAL OU TRANSACIONAL
                    //(CASO USO 2: STEP 11)
                    if (classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Rev_Rel ||
                        classCanal.Name == Intelbras.CRM2013.Domain.Enum.Conta.Classificacao.Rev_Trans)
                    {
                        foreach (var item in lstCat)
                        {
                            LinhaCorteRevenda objLinhaCorteRevenda = (LinhaCorteRevenda)LinhaCorteService.ObterLinhaCorte(unNeg, null, item.Categoria.Id, "revenda");
                            if (objLinhaCorteRevenda == null || objLinhaCorteRevenda.LinhaCorteTrimestral == null)
                                    InserirLog(dataProc+"Linha de corte não encontrada para a canal : " + canal.ID.Value.ToString());
                            //this.AtualizarCompromissoCanalInconsistente("Linha de corte não encontrada para o canal : ", compCanal, canal);

                            if (!objLinhaCorteRevenda.LinhaCorteTrimestral.HasValue)
                                    InserirLog(dataProc+"Linha de corte não encontrada para a canal : " + canal.ID.Value.ToString());
                            //this.AtualizarCompromissoCanalInconsistente("Linha de corte não possui valor para o canal : ", compCanal, canal);

                            linhaCorte += objLinhaCorteRevenda.LinhaCorteTrimestral.Value;
                        }

                    }
                    #endregion

                    #region CALCULO DE HISTORICO X LINHA DE CORTE

                    statusComp = (valorHistorico >= linhaCorte) ? StatusCompromissoCumprido : StatusCompromissoNaoCumprido;

                    compCanalLinCorte.StatusCompromisso = new Lookup(statusComp.ID.Value, "");
                    compCanalLinCorte.Validade = DateTime.Now.Date.AddMonths(3);
                    CompromissoDoCanal.Atualizar(compCanalLinCorte);

                    #endregion

                    #endregion

                    #region VALIDANDO TREINAMENTO
                    // ESTE CASO DE USO ATENDE O 2 E O 3 POIS AMBOS IDEPENDEM DE MATRIZ OU FILIAL 
                    // E O MESMO DEVE SER REFERENCIADO PELO CANAL EM QUESTÃO
                    CompromissosDoCanal compTreinamento = CompromissoDoCanal.BuscarCompromissoCanal(compromissoTecnicoTreinadoCertificado.ID.Value, unNeg, canal.ID.Value);

                    if (compTreinamento == null)
                    {
                        continue;
                    }
                    
                    //(CASO USO 2: STEP 12)
                    List<TreinamentoCanal> lstTreinamentoCanal = RepositoryService.TreinamentoCanal.ListarPor(null, null, compTreinamento.ID.Value);

                    StatusCompromissos statusCompromissoTreinamento = StatusCompromissoCumprido;

                    //(CASO USO 2: STEP 13)
                    foreach (TreinamentoCanal _treinamentocanal in lstTreinamentoCanal)
                    {
                        if (_treinamentocanal.StatusCompromisso.Name == Enum.TreinamentoCanal.StatusCompromisso.Nao_Cumprido)
                        {
                            statusCompromissoTreinamento = StatusCompromissoNaoCumprido;
                            break;
                        }
                    }

                    compTreinamento.StatusCompromisso = new Lookup(statusCompromissoTreinamento.ID.Value, "");
                    compTreinamento.Validade = DateTime.Now.Date.AddMonths(3);
                    CompromissoDoCanal.Atualizar(compTreinamento);

                    #endregion

                    Trace.SaveClear();
                }
            }
            #region MANDA EMAIL DE LOG
            StringBuilder sb = new StringBuilder();
            foreach (string item in mensagemLog)
                sb.AppendLine(item);

            EnviaEmailDeLog("Monitoramnto Automatico");
            #endregion
        }

        private void EnviaEmailDeLog(string subject)
        {
            if (mensagemLog.Count == 0)
            {
                return;
            }

            RepositoryService repService = new RepositoryService();

            String msg = string.Empty;

            var email = new Intelbras.CRM2013.Domain.Model.Email(OrganizationName, isOffline);
            email.Assunto = "Log de erro de processamento de Monitoramento - " + subject;

            foreach (string item in mensagemLog)
            {
                msg += string.Format("<br />{0}", item);
            }

            email.Mensagem = msg;
            email.Para = new Lookup[1];
            email.Para[0] = new Lookup { Id = Guid.Parse(emailAEnviarLog), Type = SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>() };

            email.Direcao = false;
            email.ID = repService.Email.Create(email);

            repService.Email.EnviarEmail(email.ID.Value);
        }

        private void InserirLogFormat(string mensagem, params string[] args)
        {
            string msg = string.Format(mensagem, args);
            InserirLog(msg);
        }

        private void InserirLog(string mensagem)
        {
            Trace.Add(mensagem);
            this.mensagemLog.Add(mensagem);
        }
            }
}