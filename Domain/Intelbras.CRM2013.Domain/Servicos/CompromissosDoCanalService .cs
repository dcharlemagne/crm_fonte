using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using System.IO;
using System.Net;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class CompromissosDoCanalService
    {
        #region Objetos
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private string emailAEnviarLog = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail");
        private Boolean isOffline = false;
        #endregion
        public SDKore.Helper.Trace Trace { get; set; }
        private List<string> mensagemLog = new List<string>();

        private StatusCompromissos _statusCompromissoCumprido;
        public StatusCompromissos StatusCompromissoCumprido
        {
            get
            {
                if (_statusCompromissoCumprido == null)
                {
                    _statusCompromissoCumprido = RepositoryService.StatusCompromissos.ObterPor(Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido);
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
                    _statusCompromissoNaoCumprido = RepositoryService.StatusCompromissos.ObterPor(Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido);
                }

                return _statusCompromissoNaoCumprido;
            }
        }

        private StatusCompromissos _statusCompromissoCumpridoForaPrazo;
        public StatusCompromissos StatusCompromissoCumpridoForaPrazo
        {
            get
            {
                if (_statusCompromissoCumpridoForaPrazo == null)
                {
                    _statusCompromissoCumpridoForaPrazo = RepositoryService.StatusCompromissos.ObterPor(Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido_fora_Prazo);
                }

                return _statusCompromissoCumpridoForaPrazo;
            }
        }

        CompromissosDoCanalService _CompromissosDoCanal = null;
        private CompromissosDoCanalService CompromissoDoCanalService
        {
            get
            {
                if (_CompromissosDoCanal == null)
                    _CompromissosDoCanal = new CompromissosDoCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _CompromissosDoCanal;
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

        StatusCompromissoService _statusCompromissosService = null;
        private StatusCompromissoService StatusCompromissosService
        {
            get
            {
                if (_statusCompromissosService == null)
                    _statusCompromissosService = new StatusCompromissoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _statusCompromissosService;

            }
        }

        TarefaService _tarefaService = null;
        private TarefaService TarefaService
        {
            get
            {
                if (_tarefaService == null)
                    _tarefaService = new TarefaService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), isOffline);

                return _tarefaService;

            }
        }

        ParametroGlobalService _ParametroGlobal = null;
        private ParametroGlobalService ParametroGlobal
        {
            get
            {
                if (_ParametroGlobal == null)
                    _ParametroGlobal = new ParametroGlobalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _ParametroGlobal;
            }
        }

        CategoriaCanalService _CategoriaCanal = null;
        private CategoriaCanalService CategoriaCanal
        {
            get
            {
                if (_CategoriaCanal == null)
                    _CategoriaCanal = new CategoriaCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _CategoriaCanal;
            }
        }

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CompromissosDoCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CompromissosDoCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public CompromissosDoCanalService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        public CompromissosDoCanal BuscarCompromissoCanalPorTipoCompromissoECanal(string nomeCompromisso, Guid canal)
        {
            Domain.Model.CompromissosDoPrograma compromissosPrograma = this.BuscarCompromissoDoPrograma(nomeCompromisso);

            return RepositoryService.CompromissosDoCanal.ObterPor(compromissosPrograma.ID.Value, canal);
        }

        public List<CompromissosDoCanal> ListarPorListaCompromissosEcanal(List<Guid> CompromissosProg, Guid Canal, Guid UnidadeNegocios)
        {
            return RepositoryService.CompromissosDoCanal.ListarPor(CompromissosProg, Canal, UnidadeNegocios);
        }
        public List<CompromissosDoCanal> ListarPorCod33EPorMatriz(Guid Canal, Guid UnidadeNegocios)
        {
            return RepositoryService.CompromissosDoCanal.ListarPorCod33EPorMatriz(Canal, UnidadeNegocios);
        }


        public CompromissosDoPrograma BuscarCompromissoDoPrograma(string nomeCompromisso)
        {
            return RepositoryService.CompromissosPrograma.ListarPor(nomeCompromisso).FirstOrDefault();
        }

        public CompromissosDoPrograma BuscarCompromissoDoPrograma(int codigo)
        {
            return RepositoryService.CompromissosPrograma.ObterPor(codigo);
        }

        public CompromissosDoPrograma BuscarCompromissoDoPrograma(Guid compromissoId)
        {
            return RepositoryService.CompromissosPrograma.ObterPor(compromissoId);
        }

        public Guid Persistir(CompromissosDoCanal objCompromissosDoCanal)
        {
            return RepositoryService.CompromissosDoCanal.Create(objCompromissosDoCanal);

        }

        public void VerificarStatusCompromissoAutomatico(CompromissosDoCanal objCompromissoDoCanal)
        {
            if (objCompromissoDoCanal.Compromisso != null)
            {
                CompromissosDoPrograma compromisso = RepositoryService.CompromissosPrograma.Retrieve(objCompromissoDoCanal.Compromisso.Id);
                // Garante que somente pode ser alterado compromissos com monitoramento manual
                if (compromisso != null)
                {
                    if (compromisso.TipoMonitoramento.HasValue
                        && !compromisso.TipoMonitoramento.Value.Equals((int)Enum.CompromissoPrograma.TipoMonitoramento.Manual))
                        throw new ArgumentException("Status do compromisso não pode ser alterado para compromissos do programa com Tipo de Compromisso Automático/PorTarefas.");
                }
                else
                    throw new ArgumentException("Compromisso do Programa não encontrado no Crm.");
            }
        }

        public void Atualizar(CompromissosDoCanal objCompromissosDoCanal)
        {
            RepositoryService.CompromissosDoCanal.Update(objCompromissosDoCanal);
        }

        public CompromissosDoCanal BuscarPorGuid(Guid compromissoCanal)
        {
            return RepositoryService.CompromissosDoCanal.Retrieve(compromissoCanal);
        }

        public void GerarAtividadeChecklist(string nomeAtividade, CompromissosDoCanal ObjCompromisso, Usuario Proprietario)
        {
            Domain.Model.Tarefa tarefa = new Domain.Model.Tarefa(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

            ParametroGlobal paramGlobal = new Domain.Servicos.ParametroGlobalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                                                                    .ObterFrequenciaAtividadeChecklist(ObjCompromisso.Compromisso.Id);

            if (paramGlobal != null && paramGlobal.Valor != null)
                tarefa.Conclusao = DateTime.Now.AddDays(Convert.ToInt32(paramGlobal.Valor));
            else
                tarefa.Conclusao = null;

           // tarefa.Proprietario = new SDKore.DomainModel.Lookup(Proprietario.Id, Proprietario.Name, Proprietario.Type);

            tarefa.Assunto = nomeAtividade;

            Domain.Model.TipoDeAtividade tipoAtividade = new Domain.Servicos.TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).BuscarTipoTarefa("Checklist");
            if (tipoAtividade != null)
            {
                tarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
            }

            tarefa.ReferenteA = new SDKore.DomainModel.Lookup(ObjCompromisso.ID.Value, "itbc_compdocanal");

            tarefa.ID = new Domain.Servicos.TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).Persistir(tarefa);
            if (tarefa.ID.HasValue)
            {
                new Servicos.UtilService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).MudarProprietarioRegistro("systemuser", Proprietario.Id, "task", tarefa.ID.Value);
            }
        }

        public CompromissosDoCanal BuscarCompromissoCanal(Guid compromissoPrograma, Guid unidadeNegocio, Guid canal)
        {
            return RepositoryService.CompromissosDoCanal.ObterPor(compromissoPrograma, unidadeNegocio, canal);
        }

        public List<CompromissosDoCanal> ListaCompromissoCanalPorContaUnidade(Guid canalId, Guid unidadeNegocioId)
        {
            return RepositoryService.CompromissosDoCanal.ListarPorContaUnidade(canalId, unidadeNegocioId);
        }

        public List<CompromissosDoCanal> ListarAtivosVencidosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento tipoMonitoramento)
        {
            return RepositoryService.CompromissosDoCanal.ListarAtivosVencidosCumpridos(tipoMonitoramento, StatusCompromissoCumprido, StatusCompromissoCumpridoForaPrazo);
        }

        public List<CompromissosDoCanal> ListarVencidosManualPorTarefasESolicitacoes(int[] tipoMonitoramento, Guid statusCompromissoId)
        {
            return RepositoryService.CompromissosDoCanal.ListarVencidosManualPorTarefasESolicitacoes(tipoMonitoramento, statusCompromissoId );
        }

        public List<CompromissosDoCanal> ListarAtivosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento tipoMonitoramento)
        {
            return RepositoryService.CompromissosDoCanal.ListarAtivosCumpridos(tipoMonitoramento, StatusCompromissoCumprido, StatusCompromissoCumpridoForaPrazo);
        }

        public List<CompromissosDoCanal> ListarPorCanalCompromisso(Guid canalId, Guid compDoProgId)
        {
            return RepositoryService.CompromissosDoCanal.ListarPorCanalCompromisso(canalId, compDoProgId);
        }

        public List<CompromissosDoCanal> ListaCompromissoCanalPlanilha()
        {
            return RepositoryService.CompromissosDoCanal.ListarPorPlanilha();
        }
        public List<CompromissosDoCanal> ListaCompromissoCanalLote()
        {
            return RepositoryService.CompromissosDoCanal.ListarLote();
        }

        public CompromissosDoCanal BuscarCompromissoCanal(Guid compromissoPrograma, Guid canal)
        {
            return RepositoryService.CompromissosDoCanal.ObterPor(compromissoPrograma, canal);
        }

        public List<CompromissosDoCanal> ListarCompromissoCanalPorConta(Guid canalId)
        {
            return RepositoryService.CompromissosDoCanal.ListarPorCanal(canalId);
        }

        public DateTime? ObterValidade(CompromissosDoCanal CompromissoTarget)
        {
            if (CompromissoTarget.StatusCompromisso.Name == Enum.CompromissoCanal.StatusCompromisso.Cumprido)
            {
                if (CompromissoTarget.Compromisso != null && CompromissoTarget.Compromisso.Id != Guid.Empty)
                {
                    ParametroGlobal paramGlobal = new ParametroGlobalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                                  .ObterFrequenciaAtividadeChecklist(CompromissoTarget.Compromisso.Id);

                    if (paramGlobal == null)
                    {
                        throw new ApplicationException("”(CRM)Não foi possível alterar o status do compromisso devido a falta do parâmetro global "
                            + (int)Enum.TipoParametroGlobal.FrequenciaChecklist + ". Entrar em contato com o suporte");
                    }
                    else
                    {
                        return DateTime.Today.AddDays(paramGlobal.GetValue<int>());
                    }
                }
            }

            return null;
        }

        public void AtualizarBeneficiosECompromissosCascata(CompromissosDoCanal CompromissoTarget)
        {
            Guid? UnidadeNeg = null;
            Guid? Classificacao = null;
            Guid? Categoria = null;
            Boolean? Exclusividade = null;

            if (CompromissoTarget.Canal == null)
                throw new ArgumentException("Campo canal não preenchido");

            Domain.Model.Conta canal = new Domain.Servicos.ContaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                            .BuscaConta(CompromissoTarget.Canal.Id);

            if (canal != null)
            {
                if (CompromissoTarget.UnidadeDeNegocio != null)
                    UnidadeNeg = CompromissoTarget.UnidadeDeNegocio.Id;

                if (canal.Classificacao != null)
                    Classificacao = canal.Classificacao.Id;

                Domain.Model.CategoriasCanal categoriaCanal = new Domain.Servicos.CategoriaCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                            .ListarPor(canal.ID.Value, UnidadeNeg).FirstOrDefault();

                if (categoriaCanal != null && categoriaCanal.Categoria != null)
                    Categoria = categoriaCanal.Categoria.Id;

                if (canal.Exclusividade != null)
                    Exclusividade = canal.Exclusividade.Value;

                Domain.Model.Perfil perfil = new Intelbras.CRM2013.Domain.Servicos.PerfilServices(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).BuscarPerfil(Classificacao, UnidadeNeg, Categoria, Exclusividade);

                if (perfil != null)
                {
                    List<Domain.Model.BeneficiosCompromissos> benefCompr = new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).BuscaBeneficiosCompromissos(perfil.ID.Value, CompromissoTarget.Compromisso.Id, null);

                    if (benefCompr != null && benefCompr.Count > 0)
                    {
                        foreach (Domain.Model.BeneficiosCompromissos item in benefCompr)
                        {
                            bool flagAtualizarBeneficio = true;
                            Lookup statusBenef = (Lookup)new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                            .BuscarBeneficioCorrespondentePorCodigoStatus(item, CompromissoTarget.StatusCompromisso.Id);

                            if (statusBenef != null)
                            {
                                if (statusBenef.Name != Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido)
                                {
                                    //fluxo alternativo 1
                                    List<Domain.Model.BeneficiosCompromissos> benefComprAlternativo = new Intelbras.CRM2013.Domain.Servicos.BeneficiosCompromissosService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).BuscaBeneficiosCompromissos(perfil.ID.Value, null, item.Beneficio.Id);
                                    if (benefComprAlternativo.Count > 0)
                                    {
                                        List<BeneficiosCompromissos> lstDoBeneficio = new List<BeneficiosCompromissos>();// benefComprAlternativo.Where(x => x.Compromisso != CompromissoTarget.Compromisso).ToList<BeneficiosCompromissos>();
                                        foreach (var _benefCompro in benefComprAlternativo)
                                        {
                                            if (_benefCompro.Compromisso != null && _benefCompro.Compromisso.Id != CompromissoTarget.Compromisso.Id)
                                                lstDoBeneficio.Add(_benefCompro);
                                        }

                                        foreach (Domain.Model.BeneficiosCompromissos registro in lstDoBeneficio)
                                        {
                                            if (registro.Compromisso == null)
                                                throw new ArgumentException("Beneficio x Compromisso do Perfil : " + perfil.Nome + " configurado incorretamente , campo compromisso vazio.Operação cancelada.");

                                            Domain.Model.CompromissosDoCanal comproCanal = new Intelbras.CRM2013.Domain.Servicos.CompromissosDoCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                                                            .BuscarCompromissoCanal(registro.Compromisso.Id, UnidadeNeg.Value, canal.ID.Value);

                                            if (comproCanal != null)
                                            {
                                                if (comproCanal.StatusCompromisso != null && comproCanal.StatusCompromisso.Name == Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido)
                                                {
                                                    flagAtualizarBeneficio = false;
                                                    break;
                                                }
                                            }
                                            else
                                                throw new ArgumentException("O compromisso " + registro.Compromisso.Name + " não existe para este Canal");
                                        }
                                    }
                                }
                                if (flagAtualizarBeneficio)
                                {
                                    Domain.Model.BeneficioDoCanal benefCanal = new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                                                            .BuscarBeneficioCanal(item.Beneficio.Id, UnidadeNeg.Value, canal.ID.Value);
                                    benefCanal.StatusBeneficio = statusBenef;
                                    new Intelbras.CRM2013.Domain.Servicos.BeneficioDoCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).AlterarBeneficioCanal(benefCanal);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void InativarCompromissosDoCanal(Conta canal)
        {
            var lstCompromissosDoCanal = this.RepositoryService.CompromissosDoCanal.ListarPorCanal(canal.ID.Value);
            foreach (var compromissoDoCanal in lstCompromissosDoCanal)
            {
                if (compromissoDoCanal.Status == (int)Enum.CompromissoCanal.StateCode.Ativo)
                {
                    this.RepositoryService.CompromissosDoCanal.AtualizarStatus(compromissoDoCanal.ID.Value, (int)Enum.CompromissoCanal.StateCode.Inativo, (int)Enum.CompromissoCanal.Status.Desativado);
                }
            }
        }

        // CASO DE USO 5 – MONITORAMENTO MANUAL 
        public void MonitoramentoManual()
        {
            Trace = new SDKore.Helper.Trace("Monitoramento Manual");

            if (StatusCompromissoNaoCumprido == null)
            {
                throw new ApplicationException("A execução do monitoramento foi interrompida, não foi encontrado o Status do Compromisso: " + Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido);
            }

            // OBTEM COMPROMISSOS DO CANAL (CASO USO 5: STEP 4)
            // VERIFICA VALIDADE (CASO USO 5: STEP 5)
            List<CompromissosDoCanal> lstCompCanal = CompromissoDoCanalService.ListarAtivosVencidosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento.Manual);

            Trace.Add("Foram encontrados {0} para ser processado!", lstCompCanal.Count);
            Trace.SaveClear();

            foreach (var item in lstCompCanal)
            {
                try
                {
                    var compromissoCanalUpdate = new CompromissosDoCanal(item.OrganizationName, item.IsOffline)
                    {
                        ID = item.ID,
                        StatusCompromisso = new Lookup(StatusCompromissoNaoCumprido.ID.Value, "")
                    };

                    Trace.Add("Atualizando o compromisso do Canal [{0}] para não cumprido!", item.ID);
                    Trace.SaveClear();

                    RepositoryService.CompromissosDoCanal.Update(compromissoCanalUpdate);
                }
                catch (Exception ex)
                {
                    SDKore.Helper.Error.Handler(ex);
                }
            }
        }

        public void MonitoramentoPorSolicitacoes()
        {
            Trace = new SDKore.Helper.Trace("Monitoramento por Solicitação");
            mensagemLog = new List<string>();

            List<CompromissosDoCanal> lstCompCanal = CompromissoDoCanalService.ListarAtivosVencidosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento.Solicitacoes);

            foreach (var item in lstCompCanal)
                    {
                List<Tarefa> tarefas = TarefaService.ListarTarefasAtivas(item.ID.Value);

                var compromissoCanalUpdate = new CompromissosDoCanal(item.OrganizationName, isOffline)
                        {
                    ID = item.ID
                };

                            if (tarefas.Count > 0)
                            {
                    compromissoCanalUpdate.StatusCompromisso = new Lookup(StatusCompromissoNaoCumprido.ID.Value, "");
                            }
                            else
                            {
                                ParametroGlobal paramGlobal = new Domain.Servicos.ParametroGlobalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                                                        .ObterFrequenciaAtividadeChecklist(item.Compromisso.Id);

                                if (paramGlobal != null)
                                {
                        compromissoCanalUpdate.Validade = DateTime.Today.AddDays(int.Parse(paramGlobal.Valor));
                                }
                                else
                    {
                        InserirLogFormat("Parametro global {0} não localizado para o compromisso {1}", Enum.TipoParametroGlobal.FrequenciaChecklist, item.Compromisso.Name);
                        continue;
                            }
                        }

                RepositoryService.CompromissosDoCanal.Update(compromissoCanalUpdate);
            }

            this.EnviaEmailDeLog("Monitoramento Por Solicitações");
        }

        private void ExecutaVerificacaoDeValidade_Manual(CompromissosDoCanal compCanal)
        {
            Trace = new SDKore.Helper.Trace("Monitoramento Manual");
            mensagemLog = new List<string>();
            InserirLog("Monitoramento Manual inicio");

                CompromissosDoPrograma compPrograma = RepositoryService.CompromissosPrograma.ObterPorCompCanal(compCanal.ID.Value);

                #region VERIFICA VALIDADE
                // VERIFICA VALIDADE (CASO USO 5: STEP 5)
                if ((compCanal.Validade == null || compCanal.Validade < DateTime.Now) &&
                    (compPrograma.TipoMonitoramento != null && compPrograma.TipoMonitoramento.Value == (int)Enum.CompromissoPrograma.TipoMonitoramento.Manual))
                {
                    compCanal.StatusCompromisso = new Lookup(RepositoryService.StatusCompromissos.ObterPor(Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido).ID.Value, "");
                    RepositoryService.CompromissosDoCanal.Update(compCanal);
                }
                #endregion

            #region MANDA EMAIL DE LOG Monitoramnto Manual
            StringBuilder sb = new StringBuilder();
            foreach (string item in mensagemLog)
                sb.AppendLine(item);

            this.EnviaEmailDeLog("Monitoramento Manual");
            #endregion 
        }

        private void ExecutaVerificacaoDeValidade_Tarefas(CompromissosDoCanal compCanal)
        {
            TarefaService tarefa = new TarefaService(OrganizationName, isOffline);
            //tarefa.MonitoramentoPorTarefas(compCanal); //TODO: GABRIEL
        }

        private void ExecutaVerificacaoDeValidade_Solicitacoes(CompromissosDoCanal compCanal)
        {
            List<Tarefa> tarefas = TarefaService.ListarTarefasAtivas(compCanal.ID.Value);

            CompromissosDoPrograma compPrograma = RepositoryService.CompromissosPrograma.ObterPorCompCanal(compCanal.ID.Value);


            #region VERIFICA VALIDADE
            if ((compCanal.Validade == null || compCanal.Validade < DateTime.Now) &&
                (compPrograma.TipoMonitoramento != null && compPrograma.TipoMonitoramento.Value == (int)Enum.CompromissoPrograma.TipoMonitoramento.Solicitacoes))
            {
                if (tarefas.Count > 0)
                {
                    if (tarefas[0].Conclusao < DateTime.Now)
                    {
                        compCanal.StatusCompromisso = new Lookup(RepositoryService.StatusCompromissos.ObterPor(Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido).ID.Value, "");
                        RepositoryService.CompromissosDoCanal.Update(compCanal);
                    }
                }
                else
                {
                    ParametroGlobal paramGlobal = new Domain.Servicos.ParametroGlobalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
                                                                        .ObterFrequenciaAtividadeChecklist(compCanal.Compromisso.Id);

                    if (paramGlobal != null)
                    {
                        compCanal.Validade = DateTime.Now.AddDays(int.Parse(paramGlobal.Valor));
                        RepositoryService.CompromissosDoCanal.Update(compCanal);
                    }
                    else
                        InserirLog("Parametro global " + Enum.TipoParametroGlobal.FrequenciaChecklist + " não localizado para o compromisso " + compCanal.Compromisso.Name);
                }
            }
            #endregion
        }

        public void MonitoramentoManualEPorTarefasESolicitacoes()
        {
            Trace = new SDKore.Helper.Trace("Monitoramento Manual e por Tarefas e Solicitacoes");
            mensagemLog = new List<string>();
            InserirLog("INICIO - Monitoramento Manual e por Tarefas e Solicitacoes");

            #region parametro de entrada
            int[] tiposMonitoramentoIn = new int[3];
            tiposMonitoramentoIn[0] = (int)Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento.Manual;
            tiposMonitoramentoIn[1] = (int)Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento.PorTarefas;
            tiposMonitoramentoIn[2] = (int)Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento.Solicitacoes;
            #endregion

            List<CompromissosDoCanal> lstCompCanal = CompromissoDoCanalService.ListarVencidosManualPorTarefasESolicitacoes(tiposMonitoramentoIn, 
                                                                                                                    RepositoryService.StatusCompromissos.ObterPor(Enum.CompromissoCanal.StatusCompromisso.Cumprido).ID.Value);

            if (lstCompCanal.Count() > 0)
            {
                if (lstCompCanal != null)
                {
                    foreach (var compCanal in lstCompCanal)
                    {
                        if (compCanal.CompromissosDoPrograma.TipoMonitoramento != null)
                        {
                            switch (compCanal.CompromissosDoPrograma.TipoMonitoramento.Value)
                            {
                                case (int)Enum.CompromissoPrograma.TipoMonitoramento.Manual:
                                    this.ExecutaVerificacaoDeValidade_Manual(compCanal);
                                    break;
                                case (int)Enum.CompromissoPrograma.TipoMonitoramento.Solicitacoes:
                                    this.ExecutaVerificacaoDeValidade_Solicitacoes(compCanal);
                                    break;
                                case (int)Enum.CompromissoPrograma.TipoMonitoramento.PorTarefas:
                                    this.ExecutaVerificacaoDeValidade_Tarefas(compCanal);
                                    break;
                            }
                        }
                    }
                }
            }

            InserirLog("FIM - Monitoramento Manual e por Tarefas e Solicitacoes");

            #region MANDA EMAIL DE LOG MonitoramntoAutomatico
            StringBuilder sb = new StringBuilder();
            foreach (string item in mensagemLog)
                sb.AppendLine(item);

            this.EnviaEmailDeLog("Monitoramento Manual e por Tarefas e Solicitacoes");
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
        private void InserirLogFormat(string mensagem, params object[] args)
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
