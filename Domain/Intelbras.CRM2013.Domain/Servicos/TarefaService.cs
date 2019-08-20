using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using System.Net;
using System.IO;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TarefaService
    {
        #region Objetos
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private string emailAEnviarLog = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail");
        private Boolean isOffline = false;

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

        string usuarioSharePoint = SDKore.Helper.Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("UsuarioSharePoint"));
        string senhaSharePoint = SDKore.Helper.Cryptography.Decrypt(SDKore.Configuration.ConfigurationManager.GetSettingValue("SenhaSharePoint"));


        public SDKore.Helper.Trace Trace { get; set; }
        private List<string> mensagemLog = new List<string>();

        #endregion


        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TarefaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
            Trace = new SDKore.Helper.Trace(organizacao);
        }

        public TarefaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
            Trace = new SDKore.Helper.Trace(organizacao);
        }

        public TarefaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
            Trace = new SDKore.Helper.Trace(string.Empty);
        }

        #endregion

        #region Métodos
        public Guid Persistir(Model.Tarefa objTarefa)
        {
            if (objTarefa.ID != null && objTarefa.ID.Value != Guid.Empty) {
                RepositoryService.Tarefa.Update(objTarefa);
                return objTarefa.ID.Value;
            }else
                return RepositoryService.Tarefa.Create(objTarefa);
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

        public Tarefa Persistir(Model.Tarefa objTarefa, out string retorno)
        {
            //Somente Update
            retorno = string.Empty;
            Tarefa TmpTarefa = null;

            Trace.Add("Tarefa.Persistir IsUpdate: {0}", objTarefa.ID.HasValue);

            if (objTarefa.ID.HasValue)
            {
                TmpTarefa = RepositoryService.Tarefa.ObterPor(objTarefa.ID.Value);

                if (TmpTarefa != null)
                {
                    objTarefa.ID = TmpTarefa.ID;
                    RepositoryService.Tarefa.Update(objTarefa);

                    Trace.Add("Atualizando tarefa ID: {0}", objTarefa.ID.Value);

                    //Altera Status - Se necessário
                    if (!TmpTarefa.State.Equals(objTarefa.State) && objTarefa.State != null)
                    {
                        Trace.Add("Atualizando Status para: {0}", objTarefa.State.Value);
                        this.MudarStatus(TmpTarefa.ID.Value, objTarefa.State.Value, null);
                    }

                    return TmpTarefa;
                }
                else
                {
                    retorno = "Tarefa - " + objTarefa.ID.Value.ToString() + " não cadastrada no Crm, ação de insert não permitida para entidade 'Tarefa'.";
                    Trace.Add(retorno);

                    return null;
                }

            }

            return null;
        }
        public void AtualizarTarefaNaoAjusteManual(SolicitacaoBeneficio SolBenef)
        {
            Guid tipoAtividadeExecucao;

            if (!Guid.TryParse(ConfigurationManager.GetSettingValue("TipoAtividadeExecucao"), out tipoAtividadeExecucao))
                throw new ArgumentException("(CRM) Faltando parâmetro TipoAtividadeExecucao no SDKore");

            if (SolBenef.TipoSolicitacao != null && !SolBenef.AjusteSaldo.Value)
            {
                var tarefaService = new TarefaService(RepositoryService);

                Lookup referenteA = new Lookup(SolBenef.ID.Value, "");
                Lookup tipoAtividade = new Lookup(tipoAtividadeExecucao, "");
                var tarefa = tarefaService.ObterPor(referenteA.Id, tipoAtividade.Id, (int)Enum.Tarefa.StateCode.Ativo);

                if (tarefa != null)
                {
                    tarefa.Resultado = (int)Enum.Tarefa.Resultado.PagamentoEfetuadoPedidoGerado;
                    tarefa.PareceresAnteriores = "Validada/Aprovada";
                    tarefa.Status = 5;
                    tarefa.State = 1;

                    string retorno;
                    tarefaService.Persistir(tarefa, out retorno);
                }
            }
        }
        
        public TipoDeAtividade BuscarTipoTarefa(string nome)
        {
            return RepositoryService.TipoDeAtividade.ListarPorNome(nome).FirstOrDefault();
        }
        public Tarefa BuscaTarefa(Guid TarefaID)
        {
            List<Tarefa> lstTarefa = RepositoryService.Tarefa.ListarPor(TarefaID);
            if (lstTarefa.Count > 0)
                return lstTarefa.First<Tarefa>();
            return null;
        }

        public Tarefa BuscarTarefaPorReferenteA(Guid referenteA)
        {
            return RepositoryService.Tarefa.ObterPorReferenteA(referenteA);
        }

        public Tarefa ObterPorTarefaAtiva(Guid referenteA)
        {
            return RepositoryService.Tarefa.ListarPorReferenteAAtivo(referenteA).FirstOrDefault();
        }

        public List<Tarefa> ListarTarefasAtivas(Guid referenteA)
        {
            return RepositoryService.Tarefa.ListarPorReferenteAAtivo(referenteA);
        }

        public List<Tarefa> ListarTarefas(Tarefa tarefa, DateTime? dtInicio, DateTime? dtFim)
        {
            return RepositoryService.Tarefa.ListarPor(tarefa.ReferenteA.Id, tarefa.TipoDeAtividade.Id, dtInicio, dtFim, tarefa.State);
        }

        public Tarefa ObterPor(Guid referenteA, Guid tipoAtividade, int situacao)
        {
            return RepositoryService.Tarefa.ListarPor(referenteA, tipoAtividade, null, null, situacao).FirstOrDefault();
        }

        public bool MudarStatus(Guid id, int status, int? statuscode)
        {
            return RepositoryService.Tarefa.AlterarStatus(id, status, statuscode);
        }

        public string ObterProximaAtividadeCheckup(List<string> ListaAtividadeCheckup, string AtividadeAtual)
        {
            //se parametro vier vazio ele retorna a primeira atividade
            if (string.IsNullOrEmpty(AtividadeAtual))
            {
                return ListaAtividadeCheckup.First<string>();
            }
            else
            {
                if (ListaAtividadeCheckup.Contains(AtividadeAtual) && ListaAtividadeCheckup.IndexOf(AtividadeAtual) + 1 < ListaAtividadeCheckup.Count)
                {
                    return ListaAtividadeCheckup[ListaAtividadeCheckup.IndexOf(AtividadeAtual) + 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public string ObterProximaAtividadeCheckup(int proximaOrdem, CompromissosDoPrograma compromissoPrograma)
        {
            var lista = ListarAtividadesCheckup(compromissoPrograma.ID.Value);

            if (lista.Count == 0)
            {
                return null;
            }
            else if (lista.Count() >= proximaOrdem)
            {
                return lista[proximaOrdem - 1];
            }
            else
            {
                return lista[0];
            }
        }

        public List<string> ListarAtividadesCheckup(Guid compromissoId)
        {
            List<string> lstTarefas = new List<string>();

            ParametroGlobal parametroGlobal = new ParametroGlobalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider)
            .ObterPor((int)Enum.TipoParametroGlobal.AtividadesChecklist, null, null, null, null, compromissoId, null, null);

            if (parametroGlobal != null && !string.IsNullOrEmpty(parametroGlobal.Valor))
            {
                return ConverterParametroParaLista(parametroGlobal.Valor);
            }
            else
                return new List<string>();

        }

        public List<string> ConverterParametroParaLista(string listaTarefas)
        {
            char delimitador = ';';
            return listaTarefas.Split(delimitador).ToList<string>();
        }

        public void CriarTarefaChecklist(Tarefa tarefa)
        {
            if (tarefa.ReferenteA != null && tarefa.ReferenteA.Type.Equals("account"))
            {
                return;
            }

            var conta = RepositoryService.Conta.Retrieve(tarefa.ReferenteA.Id, "itbc_classificacaoid", "name");

            if (conta == null || conta.Classificacao == null)
            {
                return;
            }

            var parametroGlobalService = new Servicos.ParametroGlobalService(RepositoryService);
            var pAtividadeCheckList = parametroGlobalService.ObterPor((int)Domain.Enum.TipoParametroGlobal.FrequenciaChecklist, null, conta.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);
            var pListaAtividades = parametroGlobalService.ObterPor((int)Domain.Enum.TipoParametroGlobal.AtividadesChecklist, null, conta.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);

            if (pAtividadeCheckList == null || String.IsNullOrEmpty(pAtividadeCheckList.Valor))
            {
                throw new ArgumentException("(CRM) Operação Cancelada. O parâmetro global de Frequência de Checklist (64) para Visita Comercial de canais com classificação : " + conta.Classificacao.Name + " não foi configurado. Para resolver o problema você deve criar esse parâmetro global.");
            }

            if (pListaAtividades == null || String.IsNullOrEmpty(pListaAtividades.Valor))
            {
                throw new ArgumentException("(CRM) Operação Cancelada. O parâmetro global de Atividade  de Checklist (63) para Visita Comercial de canais com classificação :  " + conta.Classificacao.Name + " não foi configurado. Para resolver o problema você deve criar esse parâmetro global.");
            }

            var tarefaService = new Domain.Servicos.TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            var tipoAtividade = tarefaService.BuscarTipoTarefa("Checklist");

            var novaTarefa = new Domain.Model.Tarefa(RepositoryService);
            novaTarefa.Assunto = "Checklist - " + conta.RazaoSocial;
            novaTarefa.Conclusao = DateTime.Now.AddDays(Convert.ToInt16(pAtividadeCheckList.Valor));
            novaTarefa.ReferenteA = new SDKore.DomainModel.Lookup(conta.ID.Value, "account");
            novaTarefa.Descricao = pListaAtividades.Valor;

            if (tipoAtividade != null)
            {
                novaTarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
            }

            tarefaService.Persistir(novaTarefa);
            Guid idTarefa = tarefaService.Persistir(tarefa);
            if (idTarefa != Guid.Empty)
            {
                Usuario proprietario = new Domain.Servicos.UsuarioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).BuscarProprietario("task", "activityid", tarefa.Id);
                if (proprietario != null)
                    new Domain.Servicos.UtilService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).MudarProprietarioRegistro("systemuser", proprietario.Id, "task", idTarefa);
            }
        }

        public void CriarTarefaChecklist(RelacionamentoCanal relacionamentoCanal)
        {
            if (relacionamentoCanal.Canal == null)
            {
                throw new ApplicationException("Canal precisa estar preenchido para criação das atividades de checklist!");
            }

            if (relacionamentoCanal.Supervisor == null)
            {
                throw new ApplicationException("Supervisor precisa estar preenchido para criação das atividades de checklist!");
            }

            var conta = RepositoryService.Conta.Retrieve(relacionamentoCanal.Canal.Id, "itbc_classificacaoid", "name", "itbc_participa_do_programa");

            if (!conta.ParticipantePrograma.HasValue || conta.ParticipantePrograma.Value != (int)Domain.Enum.Conta.ParticipaDoPrograma.Sim)
            {
                return;
            }

            if (conta == null || conta.Classificacao == null)
            {
                throw new ApplicationException("Canal precisa estar classificado para criação das atividades de checklist!");
            }

            var parametroGlobalService = new Servicos.ParametroGlobalService(RepositoryService);
            var pAtividadeCheckList = parametroGlobalService.ObterPor((int)Domain.Enum.TipoParametroGlobal.FrequenciaChecklist, null, conta.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);
            var pListaAtividades = parametroGlobalService.ObterPor((int)Domain.Enum.TipoParametroGlobal.AtividadesChecklist, null, conta.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);

            if (pAtividadeCheckList == null || String.IsNullOrEmpty(pAtividadeCheckList.Valor))
            {
                throw new ApplicationException("Operação Cancelada. O parâmetro global de Frequência de Checklist (64) para Visita Comercial de canais com classificação : " + conta.Classificacao.Name + " não foi configurado. Para resolver o problema você deve criar esse parâmetro global.");
            }

            if (pListaAtividades == null || String.IsNullOrEmpty(pListaAtividades.Valor))
            {
                throw new ApplicationException("Operação Cancelada. O parâmetro global de Atividade  de Checklist (63) para Visita Comercial de canais com classificação :  " + conta.Classificacao.Name + " não foi configurado. Para resolver o problema você deve criar esse parâmetro global.");
            }

            var tarefaService = new Domain.Servicos.TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            var tipoAtividade = tarefaService.BuscarTipoTarefa("Checklist");

            var novaTarefa = new Domain.Model.Tarefa(RepositoryService);
            novaTarefa.Assunto = "Checklist - " + conta.RazaoSocial;
            novaTarefa.Conclusao = DateTime.Now.AddDays(Convert.ToInt16(pAtividadeCheckList.Valor));
            novaTarefa.ReferenteA = new SDKore.DomainModel.Lookup(conta.ID.Value, "account");
            novaTarefa.Descricao = pListaAtividades.Valor;

            if (tipoAtividade != null)
            {
                novaTarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
            }

            novaTarefa.ID = tarefaService.Persistir(novaTarefa);
            if (novaTarefa.ID.HasValue)
            {
                Usuario proprietario = new Domain.Servicos.UsuarioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).BuscarProprietario("task", "activityid", relacionamentoCanal.Supervisor.Id);
                if (proprietario != null)
                {
                    new Domain.Servicos.UtilService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).MudarProprietarioRegistro("systemuser", proprietario.ID.Value, "task", novaTarefa.ID.Value);
                }
            }
        }

        public void GerarAtividadesVisitaComercial(Conta conta)
        {
            List<Model.RelacionamentoCanal> lstRelacionamento = new Domain.Servicos.RelacionamentoCanalService(RepositoryService).ListarAtivosPorCanal(conta.ID.Value);

            if (lstRelacionamento.Count > 0)
            {
                var parametroGlobalService = new Servicos.ParametroGlobalService(RepositoryService);
                var pAtividadeCheckList = parametroGlobalService.ObterPor((int)Domain.Enum.TipoParametroGlobal.FrequenciaChecklist, null, conta.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);
                var pListaAtividades = parametroGlobalService.ObterPor((int)Domain.Enum.TipoParametroGlobal.AtividadesChecklist, null, conta.Classificacao.Id, null, null, null, null, (int)Domain.Enum.ParametroGlobal.Parametrizar.VisitaComercial);

                if (pAtividadeCheckList == null || String.IsNullOrEmpty(pAtividadeCheckList.Valor))
                {
                    throw new ArgumentException("(CRM) Operação Cancelada.O parâmetro global de Frequência de Checklist (64) para Visita Comercial de canais com classificação : " + conta.Classificacao.Name + " não foi configurado. Para resolver o problema você deve criar esse parâmetro global.");
                }

                if (pListaAtividades == null || String.IsNullOrEmpty(pListaAtividades.Valor))
                {
                    throw new ArgumentException("(CRM) Operação Cancelada.O parâmetro global de Atividade  de Checklist (63) para Visita Comercial de canais com classificação :  " + conta.Classificacao.Name + " não foi configurado. Para resolver o problema você deve criar esse parâmetro global.");
                }

                var tarefaService = new Domain.Servicos.TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                var tipoAtividade = tarefaService.BuscarTipoTarefa("Checklist");

                foreach (RelacionamentoCanal registro in lstRelacionamento)
                {
                    if (registro.Supervisor != null)
                    {
                        var tarefa = new Domain.Model.Tarefa(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

                        if (tipoAtividade != null)
                        {
                            tarefa.TipoDeAtividade = new SDKore.DomainModel.Lookup(tipoAtividade.ID.Value, tipoAtividade.Nome, "");
                        }

                        tarefa.Assunto = "Checklist - " + conta.RazaoSocial;
                        tarefa.Conclusao = DateTime.Now.AddDays(Convert.ToInt16(pAtividadeCheckList.Valor));
                        tarefa.Descricao = pListaAtividades.Valor;
                        tarefa.ReferenteA = new SDKore.DomainModel.Lookup(conta.ID.Value, "account");

                        Guid idTarefa = tarefaService.Persistir(tarefa);
                        if (idTarefa != Guid.Empty)
                        {
                            new Domain.Servicos.UtilService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).MudarProprietarioRegistro("systemuser", registro.Supervisor.Id, "task", idTarefa);
                        }
                    }
                }
            }
        }

        public void CriarParecerParaSolicitacao(Tarefa mTarefa)
        {
            if (mTarefa.TipoDeAtividade != null && mTarefa.TipoDeAtividade.Name.Contains("Parecer"))
            {
                if (mTarefa.ReferenteA == null)
                    throw new ArgumentException("(CRM) Campo 'Referente a' obrigatório. Operação cancelada.");

                //Só executa se referenteA for solicitacao Cadastro
                if (mTarefa.ReferenteA.Type.ToLower().Equals(SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoCadastro>().ToLower()))
                {
                    if (!mTarefa.Ordem.HasValue)
                        throw new ArgumentException("(CRM) Campo 'Ordem' obrigatório.Operação cancelada.");

                    Domain.Model.SolicitacaoCadastro solicCadastro = new Intelbras.CRM2013.Domain.Servicos.SolicitacaoCadastroService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).ObterPor(mTarefa.ReferenteA.Id);

                    if (solicCadastro == null)
                        throw new ArgumentException("(CRM) Solicitação de Cadastro não encontrada.Operação cancelada.");

                    if (solicCadastro.Canal == null)
                        throw new ArgumentException("(CRM) Canal da Solicitação de Cadastro não informado.Operação cancelada.");

                    if (solicCadastro.Representante == null)
                        throw new ArgumentException("(CRM) Key Account/Representante não informado.Operação cancelada.");

                    if (solicCadastro.TipoDeSolicitacao == null)
                        throw new ArgumentException("(CRM) Tipo de Solicitação da solicitação de Cadastro relacionada não preenchido.Operação cancelada.");

                    ParticipantesDoProcesso partProc = new Intelbras.CRM2013.Domain.Servicos.ProcessoDeSolicitacoesService(RepositoryService).BuscarParticipanteProcesso(mTarefa.Ordem.Value, solicCadastro.TipoDeSolicitacao.Id);

                    if (partProc == null)
                        throw new ArgumentException("(CRM) Participante do Processo não encontrado.Operação cancelada.");

                    if (!partProc.TipoDoParecer.HasValue)
                        throw new ArgumentException("(CRM) Tipo do Parecer do Participante do processo não preenchido.Operação cancelada.");

                    Domain.Model.Parecer mParecer = new Parecer(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mParecer.Tarefa = new Lookup(mTarefa.ID.Value, "");
                    mParecer.Canal = solicCadastro.Canal;
                    mParecer.KeyAccountouRepresentante = solicCadastro.Representante;
                    mParecer.TipodoParecer = partProc.TipoDoParecer;

                    mParecer = new Intelbras.CRM2013.Domain.Servicos.ParecerService(RepositoryService).Persistir(mParecer);

                    if (!mParecer.ID.HasValue)
                    {
                        throw new ArgumentException("(CRM) Não foi possível criar o parecer para esta atividade.Operação cancelada.");
                    }
                }
            }
        }

        public void CancelarTarefasPorReferenteA(Guid referenteA)
        {
            var lstTarefas = this.ListarTarefasAtivas(referenteA);

            foreach (var tarefa in lstTarefas)
            {
                MudarStatus(tarefa.ID.Value, (int)Enum.Tarefa.StateCode.Cancelada, null);
            }
        }

        #endregion

        // CASO DE USO 4 – MONITORAMENTO POR TAREFAS 
        public void MonitoramentoPorTarefas()
        {
            Trace = new SDKore.Helper.Trace("MonitoramentoPorTarefa");
            mensagemLog = new List<string>();

            if (StatusCompromissoCumprido == null)
            {
                throw new ApplicationException("(CRM) A execução do monitoramento foi interrompida, não foi encontrado o Status do Compromisso: " + Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido);
            }

            if (StatusCompromissoNaoCumprido == null)
            {
                throw new ApplicationException("(CRM) A execução do monitoramento foi interrompida, não foi encontrado o Status do Compromisso: " + Intelbras.CRM2013.Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido);
            }

            List<CompromissosDoCanal> compromissosCanalVencidos = CompromissoDoCanalService.ListarAtivosVencidosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento.PorTarefas);

            Trace.Add("Foram encontrados {0} vencidos para ser processado!", compromissosCanalVencidos.Count);
            Trace.SaveClear();

            foreach (var item in compromissosCanalVencidos)
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

            List<CompromissosDoCanal> compromissosCanalCumpridos = CompromissoDoCanalService.ListarAtivosCumpridos(Intelbras.CRM2013.Domain.Enum.CompromissoPrograma.TipoMonitoramento.PorTarefas);

            Trace.Add("Foram encontrados {0} Cumpridos para ser processado!", compromissosCanalCumpridos.Count);
            Trace.SaveClear();

            foreach (var item in compromissosCanalCumpridos)
            {
                List<Tarefa> tarefas = RepositoryService.Tarefa.ListarPorReferenteAAtivo(item.ID.Value);

                // CRIA TAREFA CASO NAO EXISTA (CASO USO 4: STEP 6)
                if (tarefas.Count == 0)
                {
                    try
                    {
                        #region Obtem Parametros Global
                        if (item.UnidadeDeNegocio == null || item.Compromisso == null)
                        {
                            throw new ArgumentException("(CRM) O Comromisso do Canal a seguir não tem Unidade de Negócio ou Compromisso preenchido. " + item.ID.Value);
                        }

                        ParametroGlobal frequenciaChecklist = ParametroGlobal.ObterPor((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.FrequenciaChecklist,
                                                                                null, null, null, null, item.Compromisso.Id, null, null);

                        ParametroGlobal atividadesChecklist = ParametroGlobal.ObterPor((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.AtividadesChecklist,
                                                                               null, null, null, null, item.Compromisso.Id, null, null);

                        if (frequenciaChecklist == null)
                        {
                            InserirLogFormat("Parametro Global {0} não foi localizado.", Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.FrequenciaChecklist);
                            continue;
                        }

                        if (atividadesChecklist == null)
                        {
                            InserirLogFormat("Parametro Global {0} não foi localizado.", Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.AtividadesChecklist);
                            continue;
                        }

                        #endregion

                        #region Criando Tarefa

                        var task = new Model.Tarefa(OrganizationName, isOffline);
                        task.Assunto = string.Format("Atividades Checklist - {0} - {1}", item.UnidadeDeNegocio.Name, item.Compromisso.Name);
                        task.ReferenteA = new Lookup(item.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(item));
                        task.Descricao = atividadesChecklist.Valor;

                        task.DataInicial = DateTime.Now;
                        task.Conclusao = DateTime.Today.AddDays(int.Parse(frequenciaChecklist.Valor));
                        task.Prioridade = (int)Enum.Tarefa.Prioridade.Normal;
                        task.Status = (int)Enum.Tarefa.StatusCode.NaoIniciada;

                        Trace.Add("Criando Tarefa para Compromisso do Canal [{0}]", item.ID.Value);
                        Trace.Add("Atividades Checklist [{0}]", atividadesChecklist.Valor);
                        Trace.Add("Frequencia Checklist [{0}]", frequenciaChecklist.Valor);

                        RepositoryService.Tarefa.Create(task);

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Trace.Add(ex);
                        InserirLog(SDKore.Helper.Error.Handler(ex));
                    }
                    finally
                    {
                        Trace.SaveClear();
                    }
                }

            }

            this.EnviaEmailDeLog("Monitoramento Por Tarefas");
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

        public void CriarArquivoLog(List<string> mensagem, string url, string usuario, string senha)
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Trace.Add("CriarArquivoLog - URL {0}  USERNAME: {1}", url, userName);
            Trace.Add("MensagemArquivoLog - {0}  ", mensagem);

            StringBuilder sb = new StringBuilder();
            foreach (string item in mensagem)
            {
                sb.AppendLine(item);
            }

            Uri destUri = new Uri(url);

            using (MemoryStream inStream = new System.IO.MemoryStream(Encoding.Default.GetBytes(sb.ToString())))
            {
                WebRequest request = WebRequest.Create(url);
                request.Credentials = new NetworkCredential(usuario, senha);
                request.Method = "PUT";
                request.Timeout = 10000;
                request.ContentType = "text/plain;charset=utf-8";
                using (Stream outStream = request.GetRequestStream())
                {
                    inStream.CopyTo(outStream);
                }
                WebResponse res = request.GetResponse();
            }
        }

        public List<Tarefa> ListarPorReferenteA(Guid referenteA)
        {
            return RepositoryService.Tarefa.ListarPorReferenteA(referenteA);
        }
    }
}
