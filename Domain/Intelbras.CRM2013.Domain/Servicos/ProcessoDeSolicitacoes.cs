using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class ProcessoDeSolicitacoesService
    {

        #region Atributos

        private static bool _isOffline = false;
        public static bool IsOffline
        {
            get { return _isOffline; }
            set { _isOffline = value; }
        }

        private static string _nomeDaOrganizacao = "";
        public static string NomeDaOrganizacao
        {
            get
            {
                if (String.IsNullOrEmpty(_nomeDaOrganizacao))
                    _nomeDaOrganizacao = ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");

                return _nomeDaOrganizacao;
            }
            set { _nomeDaOrganizacao = value; }
        }

        public static object Provider { get; set; }

        #endregion

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProcessoDeSolicitacoesService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public ProcessoDeSolicitacoesService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        public ProcessoDeSolicitacoesService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        
        #endregion

        #region Metodos

        public string OrdemMaior1(Guid activityId)
        {
            //Busca os pareceres anteriores.
            string ParecerAnterior = String.Empty;
            List<Tarefa> lstTarefas = RepositoryService.Tarefa.ListarPorReferenteA(activityId);
            foreach (Tarefa _Tarefas in lstTarefas)
                ParecerAnterior += "Ordem: " + _Tarefas.Ordem.Value.ToString() + " - Parecer do Usuário: " + _Tarefas.Descricao + "\r\n";

            return ParecerAnterior;
        }

        public void CriarEmail(Lookup referenteA, ParticipantesDoProcesso PartProcesso)
        {
            Guid fromUserId;

            if (!Guid.TryParse(SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail", true), out fromUserId))
            {
                throw new ApplicationException("O parâmetro 'Intelbras.Usuario.EnvioEmail' não foi possível converte em GUID.");
            }

            SolicitacaoBeneficio Solicitacao = RepositoryService.SolicitacaoBeneficio.Retrieve(referenteA.Id);

            String msg = string.Empty;

            var email = new Domain.Model.Email(NomeDaOrganizacao, _isOffline);
            email.Assunto = Solicitacao.Canal.Name + " - " + Solicitacao.Nome + " - " + Solicitacao.TipoSolicitacao.Name + " - " + Solicitacao.DataCriacao.Value;


            msg = "Canal: " + Solicitacao.Canal.Name;
            msg += "</br>Nome da Solicitação: " + Solicitacao.Nome;
            msg += "</brTipo da Solicitação: " + Solicitacao.TipoSolicitacao.Name;
            msg += "</brValor: " + Solicitacao.ValorSolicitado.Value;
            msg += "</brDescrição: " + Solicitacao.Descricao;
            msg += "</brStatus da Solicitação : " + Solicitacao.StatusSolicitacao.Value;

            email.Mensagem = msg;
            email.De = new Lookup[1];
            email.De[0] = new Lookup { Id = fromUserId, Type = SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.Usuario>() };
            email.ReferenteA = new Lookup { Id = referenteA.Id, Type = referenteA.Type };

            if (PartProcesso.Equipe != null)
            {
                List<TeamMembership> lstTeam = RepositoryService.TeamMembership.ListarPor(PartProcesso.Equipe.Id);
                email.Para = new Lookup[lstTeam.Count()];
                int i = 0;

                foreach (TeamMembership team in lstTeam)
                {
                    email.Para[i] = new Lookup { Id = team.Usuario, Type = SDKore.Crm.Util.Utility.GetEntityName<Domain.Model.Usuario>() };
                    i++;
                }
            }

            if (PartProcesso.Usuario != null)
            {
                email.Para = new Lookup[1];
                email.Para[0] = new Lookup { Id = PartProcesso.Usuario.Id, Type = PartProcesso.Usuario.Type };
            }

            if (PartProcesso.Contato != null)
            {
                email.Para = new Lookup[1];
                email.Para[0] = new Lookup { Id = PartProcesso.Contato.Id, Type = PartProcesso.Contato.Type };
            }

            email.Direcao = false;
            email.ID = RepositoryService.Email.Create(email);

            RepositoryService.Email.EnviarEmail(email.ID.Value);
        }

        public bool CriarTarefa(Lookup referenteA, string tiposolicitacaonome, string descricao, string ParecerAnterior, ParticipantesDoProcesso PartProcesso)
        {
            Guid idProprietario = Guid.Empty;
            string tipoProprietario = "";
            Model.Tarefa task = new Model.Tarefa(NomeDaOrganizacao, _isOffline);
            Model.SolicitacaoBeneficio tmpSolBen = new SolicitacaoBeneficio(NomeDaOrganizacao, _isOffline);
            Model.SolicitacaoCadastro tmpSolCad = new SolicitacaoCadastro(NomeDaOrganizacao, _isOffline);

            List<RelacionamentoCanal> lstRelacionamento = new List<RelacionamentoCanal>();

            task.Assunto = "Plugin - " + referenteA.Name;
            task.ReferenteA = new Lookup(referenteA.Id, referenteA.Type);
            task.Assunto = PartProcesso.Papel.Name + " - " + tiposolicitacaonome;
            task.Ordem = PartProcesso.Ordem;
            task.PareceresAnteriores = ParecerAnterior;
            task.DescricaoSolicitacao = descricao;
            task.Conclusao = DateTime.Now.AddDays(1).AddHours(3);

            TipoDeAtividade tmpTipoDeAtividade = RepositoryService.TipoDeAtividade.ObterPorPapel(PartProcesso.Papel.Id);

            if (tmpTipoDeAtividade == null)
                throw new ArgumentException("Tipo de Atividade do Participante Não Encontrado! : " + PartProcesso.Papel.Name + " : " + PartProcesso.Papel.Id.ToString());

            if (tmpTipoDeAtividade.ID.HasValue)
                task.TipoDeAtividade = new Lookup(tmpTipoDeAtividade.ID.Value, "itbc_tipoatividade");

            if (PartProcesso.Equipe != null)
            {
                idProprietario = PartProcesso.Equipe.Id;
                tipoProprietario = PartProcesso.Equipe.Type;
            }

            if (PartProcesso.Usuario != null)
            {
                idProprietario = PartProcesso.Usuario.Id;
                tipoProprietario = PartProcesso.Usuario.Type;
            }

            if (PartProcesso.Contato != null)
            {
                idProprietario = PartProcesso.Contato.Id;
                tipoProprietario = PartProcesso.Contato.Type;
            }

            if (PartProcesso.PapelNoCanal.HasValue)
            {
                if (referenteA.Type.ToLower().Equals(SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoCadastro>().ToLower()))
                {
                    tmpSolCad = RepositoryService.SolicitacaoCadastro.Retrieve(referenteA.Id);
                    lstRelacionamento = RepositoryService.RelacionamentoDoCanal.ListarPor(tmpSolCad.Canal.Id);

                }
                if (referenteA.Type.ToLower().Equals(SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoBeneficio>().ToLower()))
                {
                    tmpSolBen = RepositoryService.SolicitacaoBeneficio.Retrieve(referenteA.Id);
                    lstRelacionamento = RepositoryService.RelacionamentoDoCanal.ListarPor(tmpSolBen.Canal.Id);
                }


                if (PartProcesso.PapelNoCanal.Value == (int)Domain.Enum.ParticipanteDoCanal.PapelNoCanal.DiretorDeUnidade)
                {
                    if (lstRelacionamento.First<RelacionamentoCanal>().Supervisor == null)
                        throw new ArgumentException("Diretor da Unidade Não encontrado. Não é possivel criar o próximo passo de Tarefa!");

                    Usuario _usuario = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).ObterPor(lstRelacionamento.First<RelacionamentoCanal>().Supervisor.Id);

                    if (_usuario == null)
                        throw new ArgumentException("Não foi encontrado o usuário.");

                    if (_usuario.Gerente == null)
                        throw new ArgumentException("Gerente do Supervisor não confiurado.Operação cancelada.");

                    //Descemos mais um nivel para pegar o gerente do gerente
                    _usuario = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).ObterPor(_usuario.Gerente.Id);

                    if (_usuario == null)
                        throw new ArgumentException("Não foi encontrado o usuário.");

                    if (_usuario.Gerente == null)
                        throw new ArgumentException("Gerente do Supervisor não confiurado.Operação cancelada.");

                    idProprietario = _usuario.Gerente.Id;
                    tipoProprietario = _usuario.Gerente.Type;
                }

                if (PartProcesso.PapelNoCanal.Value == (int)Domain.Enum.ParticipanteDoCanal.PapelNoCanal.GerenteNacionalGerenteDeDistribuicao)
                {
                    if (lstRelacionamento.First<RelacionamentoCanal>().Supervisor == null)
                        throw new ArgumentException("Gerente Nacional Gerente De Distribuicao Não encontrado. Não é possivel criar o próximo passo de Tarefa!");

                    Usuario _usuario = new Intelbras.CRM2013.Domain.Servicos.UsuarioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).ObterPor(lstRelacionamento.First<RelacionamentoCanal>().Supervisor.Id);

                    if (_usuario == null)
                        throw new ArgumentException("Não foi encontrado o usuário.");

                    if (_usuario.Gerente == null)
                        throw new ArgumentException("Gerente do Supervisor não confiurado.Operação cancelada.");

                    idProprietario = _usuario.Gerente.Id;
                    tipoProprietario = _usuario.Gerente.Type;
                }

                if (PartProcesso.PapelNoCanal.Value == (int)Domain.Enum.ParticipanteDoCanal.PapelNoCanal.KeyAccountRepresentante)
                {

                    if (lstRelacionamento.First<RelacionamentoCanal>().Supervisor != null)
                    {
                        idProprietario = lstRelacionamento.First<RelacionamentoCanal>().Supervisor.Id;
                        tipoProprietario = lstRelacionamento.First<RelacionamentoCanal>().Supervisor.Type;
                    }
                    else
                        throw new ArgumentException("Key Account Representante Não encontrado. Não é possivel criar o próximo passo de Tarefa!");
                }

                if (PartProcesso.PapelNoCanal.Value == (int)Domain.Enum.ParticipanteDoCanal.PapelNoCanal.SupervisorDeVendas)
                {

                    if (lstRelacionamento.First<RelacionamentoCanal>().Supervisor != null)
                    {
                        idProprietario = lstRelacionamento.First<RelacionamentoCanal>().Supervisor.Id;
                        tipoProprietario = lstRelacionamento.First<RelacionamentoCanal>().Supervisor.Type;
                    }
                    else
                        throw new ArgumentException("Supervisor De Vendas Não encontrado. Não é possivel criar o próximo passo de Tarefa!");
                }
            }

            Guid idTask = RepositoryService.Tarefa.Create(task);
            if (idTask != Guid.Empty)
            {
                new Domain.Servicos.UtilService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).MudarProprietarioRegistro(tipoProprietario, idProprietario, "task", idTask);
            }
            return true;
        }

        #region Solicitacao de Beneficio
        public void CriarTarefasSolicitacaoBeneficio(SolicitacaoBeneficio mSolicitacaoBeneficio, Guid usuarioId, int Ordem)
        {
            Guid? beneficioPrograma = null;

            if (mSolicitacaoBeneficio.BeneficioPrograma != null)
                beneficioPrograma = mSolicitacaoBeneficio.BeneficioPrograma.Id;

            Model.Processo tmpProcesso = RepositoryService.Processo.ObterPorTipoDeSolicitacao(mSolicitacaoBeneficio.TipoSolicitacao.Id, beneficioPrograma);

            if (tmpProcesso == null)
                return;

            string ParecerAnterior = String.Empty;

            if (Ordem > 1)
                ParecerAnterior = OrdemMaior1(mSolicitacaoBeneficio.ID.Value);


            List<ParticipantesDoProcesso> lstParticipanteDoProcesso = RepositoryService.ParticipantesDoProcesso.ListarPor(tmpProcesso.ID.Value, Ordem);
            bool CriouTarefa = false;

            foreach (ParticipantesDoProcesso PartProcesso in lstParticipanteDoProcesso.GroupBy(x => x.Ordem).Select(grp => grp.First()))
            {
                Lookup referenteA = new Lookup(mSolicitacaoBeneficio.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoBeneficio>());
                CriouTarefa = false;
                if (PartProcesso.Papel.Name == "Informado")
                {
                    //TODO: KSYS 45784 - Precisa corrigir o problema para não parar o fluxo
                    this.CriarEmail(referenteA, PartProcesso);
                }
                else
                {
                    CriouTarefa = this.CriarTarefa(referenteA, mSolicitacaoBeneficio.TipoSolicitacao.Name, mSolicitacaoBeneficio.Descricao, ParecerAnterior, PartProcesso);
                }

                if (!CriouTarefa)
                    CriarTarefasSolicitacaoBeneficio(mSolicitacaoBeneficio, usuarioId, Ordem + 1);
            }
        }

        public void EnvioDeEvidenciaShowRoom(Tarefa Task)
        {
            SolicitacaoBeneficio solBen = RepositoryService.SolicitacaoBeneficio.Retrieve(Task.ReferenteA.Id);

            CompromissosDoCanal compromissoCanal = RepositoryService.CompromissosDoCanal.ObterPorNome(Enum.CompromissoCanal.Compromisso.EnvioShowroom.ToString(), solBen.Canal.Id, solBen.UnidadedeNegocio.Id);

            if (compromissoCanal != null)
            {

                if (compromissoCanal.Compromisso.Name == Enum.CompromissoCanal.Compromisso.EnvioShowroom)
                {
                    if (!Task.ReferenteA.Type.ToLower().Equals(SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoBeneficio>().ToLower()))
                        return;

                    compromissoCanal.StatusCompromisso = new Lookup(RepositoryService.StatusCompromissos.ObterPor(Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido).ID.Value, "");
                    RepositoryService.CompromissosDoCanal.Update(compromissoCanal);

                    SolicitacaoBeneficio mSolicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.Retrieve(Task.ReferenteA.Id);

                    if (mSolicitacaoBeneficio == null)
                        throw new ArgumentException("Solicitação de benefício no campo 'Referente a' não encontrada ou Desativada.");

                    ParametroGlobal parametroAtividadeQtdeEvidencia = RepositoryService.ParametroGlobal.ObterPor((int)Enum.TipoParametroGlobal.QuatidadeEvidenciaShowRoom, mSolicitacaoBeneficio.UnidadedeNegocio.Id, null, null, null, compromissoCanal.Compromisso.Id, null, null);
                    if (parametroAtividadeQtdeEvidencia == null)
                        throw new ArgumentException("(CRM)  Parâmetro Global Quantidade de Evidências Show Room não encontrado para o Compromisso Envio de evidências de Showroom e Unidade de Negócio [" + mSolicitacaoBeneficio.UnidadedeNegocio.Name + "].");

                    ParametroGlobal parametroAtividadeChacklist = RepositoryService.ParametroGlobal.ObterPor((int)Enum.TipoParametroGlobal.AtividadesChecklist, mSolicitacaoBeneficio.UnidadedeNegocio.Id, null, null, null, compromissoCanal.Compromisso.Id, null, null);
                    if (parametroAtividadeChacklist == null)
                        throw new ArgumentException("(CRM) Parâmetro Global Atividade de Checklist não encontrado para o Compromisso Envio de evidências de Showroom e Unidade de Negócio [" + mSolicitacaoBeneficio.UnidadedeNegocio.Name + "].");

                    ParametroGlobal parametroFrequenciaChecklist = RepositoryService.ParametroGlobal.ObterPor((int)Enum.TipoParametroGlobal.FrequenciaChecklist, mSolicitacaoBeneficio.UnidadedeNegocio.Id, null, null, null, compromissoCanal.Compromisso.Id, null, null);
                    if (parametroFrequenciaChecklist == null)
                        throw new ArgumentException("(CRM) Parâmetro Global Frequencia de Checklist não encontrado para o Compromisso Envio de evidências de Showroom e Unidade de Negócio [" + mSolicitacaoBeneficio.UnidadedeNegocio.Name + "].");

                    Tarefa task = new Tarefa(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                    task.ReferenteA = new Lookup(compromissoCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<CompromissosDoPrograma>());
                    task.Assunto = parametroAtividadeChacklist.GetValue<string>();
                    task.Ordem = Task.Ordem + 1;
                    task.Conclusao = DateTime.Now.AddDays(parametroFrequenciaChecklist.GetValue<int>());

                    var tipoTarefa = new TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).BuscarTipoTarefa("Checklist");

                    if (tipoTarefa != null)
                        task.TipoDeAtividade = new Lookup(tipoTarefa.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(tipoTarefa));

                    Conta canal = RepositoryService.Conta.Retrieve(mSolicitacaoBeneficio.Canal.Id, "ownerid");

                    Guid idTarefa = RepositoryService.Tarefa.Create(task);
                    if (idTarefa != Guid.Empty)
                    {
                        if (canal != null)
                        {
                            Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).BuscarProprietario("account", "accountid", canal.Id);

                            if (proprietario != null)
                            {
                                new Domain.Servicos.UtilService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).MudarProprietarioRegistro("systemuser", proprietario.ID.Value, "task", idTarefa);
                            }
                        }
                    }
                }
            }
        }

        public void CriarTarefaParaChecklistCompromissoCanal(int ordem, CompromissosDoCanal compromissoDoCanal)
        {
            ParametroGlobal parametroFrequenciaChecklist = RepositoryService.ParametroGlobal
                .ObterPor((int)Enum.TipoParametroGlobal.FrequenciaChecklist, compromissoDoCanal.UnidadeDeNegocio.Id, null, null, null, compromissoDoCanal.Compromisso.Id, null, null);

            if (parametroFrequenciaChecklist == null)
            {
                throw new ArgumentException("(CRM) Parâmetro Global Frequencia de Checklist não encontrado para o Compromisso Envio de evidências de Showroom e Unidade de Negócio [" + compromissoDoCanal.UnidadeDeNegocio.Name + "].");
            }

            var compromissoPrograma = new CompromissosDoPrograma(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
            {
                ID = compromissoDoCanal.CompromissosDoPrograma.ID,
                Nome = compromissoDoCanal.CompromissosDoPrograma.Nome
            };

            TarefaService ServiceTarefas = new TarefaService(RepositoryService);
            string tituloAtividadeChecklist = ServiceTarefas.ObterProximaAtividadeCheckup(ordem + 1, compromissoPrograma);


            Tarefa task = new Tarefa(RepositoryService);

            task.ReferenteA = new Lookup(compromissoDoCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(compromissoDoCanal));
            task.Assunto = tituloAtividadeChecklist;
            task.Ordem = ordem;
            task.Conclusao = DateTime.Now.AddDays(parametroFrequenciaChecklist.GetValue<int>());

            var tipoTarefa = new TarefaService(RepositoryService).BuscarTipoTarefa("Checklist");

            if (tipoTarefa != null)
            {
                task.TipoDeAtividade = new Lookup(tipoTarefa.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(tipoTarefa));
            }

            Guid idTarefa = RepositoryService.Tarefa.Create(task);
            if (idTarefa != Guid.Empty)
            {
                Domain.Model.Usuario proprietario = new Domain.Servicos.UsuarioService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).BuscarProprietario("itbc_compdocanal", "itbc_compdocanalid", compromissoDoCanal.Id);
                if (proprietario != null)
                {
                    new Domain.Servicos.UtilService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline).MudarProprietarioRegistro("systemuser", proprietario.ID.Value, "task", idTarefa);
                }
            }
        }

        public void CriaTarefaShowRoom(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (solicitacaoBeneficio != null && solicitacaoBeneficio.UnidadedeNegocio != null && solicitacaoBeneficio.Canal != null)
            {
                CompromissosDoCanal compromissoCanal = RepositoryService.CompromissosDoCanal
                    .ObterPor(solicitacaoBeneficio.UnidadedeNegocio.Id, solicitacaoBeneficio.Canal.Id, (int)Enum.CompromissoPrograma.Codigo.Showroom);

                if (compromissoCanal == null)
                {
                    throw new ArgumentException("(CRM) Compromisso do canal Envio de evidências de Showroom não localizado, entre em contato com o suporte.");
                }

                CriarTarefaParaChecklistCompromissoCanal(1, compromissoCanal);
            }
        }

        public void ConcluirTarefaCompromissoCanal(Tarefa tarefa)
        {
            if (tarefa.Resultado.HasValue)
            {
                var compromissoDoCanal = RepositoryService.CompromissosDoCanal.Retrieve(tarefa.ReferenteA.Id);
                var compromissoPrograma = RepositoryService.CompromissosPrograma.Retrieve(compromissoDoCanal.CompromissosDoPrograma.ID.Value, "itbc_codigo", "itbc_tipodemonitoramento");

                if (!compromissoPrograma.TipoMonitoramento.HasValue || compromissoPrograma.TipoMonitoramento.Value != (int)Enum.CompromissoPrograma.TipoMonitoramento.PorTarefas)
                {
                    return;
                }

                if (tarefa.Resultado.Value == (int)Enum.Tarefa.Resultado.Reprovada)
                {
                    var statusCompromissoNaoCumprido = RepositoryService.StatusCompromissos.ObterPor(Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido);

                    RepositoryService.CompromissosDoCanal.Update(new CompromissosDoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                    {
                        ID = compromissoDoCanal.ID,
                        StatusCompromisso = new Lookup(statusCompromissoNaoCumprido.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(statusCompromissoNaoCumprido), statusCompromissoNaoCumprido.Nome)
                    });
                }
                else if (tarefa.Resultado.Value == (int)Enum.Tarefa.Resultado.Aprovada)
                {
                    int ordem = (tarefa.Ordem.HasValue) ? tarefa.Ordem.Value : 1;
                    var statusCompromissoCumprido = RepositoryService.StatusCompromissos.ObterPor(Enum.CompromissoCanal.StatusCompromisso.Cumprido);

                    var compromissoCanalService = new CompromissosDoCanalService(RepositoryService);
                    DateTime? validadeCompromissoCanal = compromissoCanalService.ObterValidade(compromissoDoCanal);

                    var compromissoCanalUpdate = new CompromissosDoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                    {
                        ID = compromissoDoCanal.ID,
                        Validade = validadeCompromissoCanal,
                        StatusCompromisso = new Lookup(statusCompromissoCumprido.ID.Value, statusCompromissoCumprido.Nome, SDKore.Crm.Util.Utility.GetEntityName(statusCompromissoCumprido))
                    };

                    RepositoryService.CompromissosDoCanal.Update(compromissoCanalUpdate);

                    if (compromissoPrograma.Codigo.Value == (int)Enum.CompromissoPrograma.Codigo.Showroom)
                    {
                        ParametroGlobal parametroQuantidadeKitsShowroom = RepositoryService.ParametroGlobal.ObterPor((int)Enum.TipoParametroGlobal.QuantidadeKitsShowroomPorSegmento, compromissoDoCanal.UnidadeDeNegocio.Id, null, null, null, compromissoDoCanal.Compromisso.Id, null, null);
                        if (parametroQuantidadeKitsShowroom == null)
                        {
                            throw new ArgumentException("(CRM) Parâmetro Global Quantidade de Evidências Show Room não encontrado para o Compromisso Envio de evidências de Showroom e Unidade de Negócio [" + compromissoDoCanal.UnidadeDeNegocio.Name + "].");
                        }

                        if (parametroQuantidadeKitsShowroom.GetValue<int>() <= ordem)
                        {
                            return;
                        }
                    }

                    CriarTarefaParaChecklistCompromissoCanal(ordem + 1, compromissoDoCanal);
                }
            }
        }

        public void ConcluirTarefaSolicitacaoBeneficio(Tarefa Task, Guid UsuarioId)
        {
            if (Task.State.Value != (int)Enum.Tarefa.StateCode.Cancelada)
            {
                SolicitacaoBeneficio solicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.Retrieve(Task.ReferenteA.Id);

                if (solicitacaoBeneficio == null)
                {
                    throw new ArgumentException("(CRM) Solicitação de benefício no campo 'Referente a' não encontrada ou Desativada.");
                }

                #region Bloco para ajuste manual

                Guid tipoAtividadeExecucao;

                if (!Guid.TryParse(ConfigurationManager.GetSettingValue("TipoAtividadeExecucao"), out tipoAtividadeExecucao))
                    throw new ArgumentException("(CRM) Faltando parâmetro TipoAtividadeExecucao no SDKore");

                if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                    Task.Resultado = (int)Enum.Tarefa.Resultado.PagamentoEfetuadoPedidoGerado;


                if (!Task.Resultado.HasValue && solicitacaoBeneficio.TipoSolicitacao != null 
                    && solicitacaoBeneficio.AjusteSaldo.Value 
                    && Task.TipoDeAtividade != null 
                    && Task.TipoDeAtividade.Id == tipoAtividadeExecucao)
                {
                    Task.Resultado = (int)Enum.Tarefa.Resultado.PagamentoEfetuadoPedidoGerado;
                }

                #endregion

                solicitacaoBeneficio.IntegrarNoPlugin = false;

                switch ((Enum.Tarefa.Resultado)Task.Resultado.Value)
                {
                    case Domain.Enum.Tarefa.Resultado.Reprovada:
                        solicitacaoBeneficio.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada;
                        solicitacaoBeneficio.Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.NaoAprovada;
                        RepositoryService.SolicitacaoBeneficio.Update(solicitacaoBeneficio);
                        RepositoryService.SolicitacaoBeneficio.AlterarStatus(solicitacaoBeneficio.ID.Value, solicitacaoBeneficio.State.Value, solicitacaoBeneficio.Status.Value);
                        return;

                    case Enum.Tarefa.Resultado.Aprovada:

                        //Após análise com o José Luiz, foi identificado que essa rotina não faz sentido - 13/09/2016 - Robson Bertolli
                        //EnvioDeEvidenciaShowRoom(Task);

                        var solicitacaoAprovada = new SolicitacaoBeneficio(solicitacaoBeneficio.OrganizationName, solicitacaoBeneficio.IsOffline)
                        {
                            ID = solicitacaoBeneficio.ID,
                            StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada,
                            Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.AprovadaParaReembolso,
                            DataAprovacao = DateTime.Now,
                            IntegrarNoPlugin = false
                        };
                        RepositoryService.SolicitacaoBeneficio.Update(solicitacaoAprovada);
                        break;

                    case Enum.Tarefa.Resultado.ComprovantesValidados:
                        solicitacaoBeneficio.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.AguardandoRetornoFinanceiro;
                        solicitacaoBeneficio.Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.AprovadaParaReembolso;
                        solicitacaoBeneficio.State = (int)Enum.SolicitacaoBeneficio.State.Ativo;
                        RepositoryService.SolicitacaoBeneficio.Update(solicitacaoBeneficio);
                        RepositoryService.SolicitacaoBeneficio.AlterarStatus(solicitacaoBeneficio.ID.Value, solicitacaoBeneficio.State.Value, solicitacaoBeneficio.Status.Value);
                        break;

                    case Enum.Tarefa.Resultado.RetornoFinanceiroValidado:
                        solicitacaoBeneficio.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.RetornoFinanceiroValidado;
                        solicitacaoBeneficio.Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.AprovadaParaReembolso;
                        solicitacaoBeneficio.State = (int)Enum.SolicitacaoBeneficio.State.Ativo;
                        RepositoryService.SolicitacaoBeneficio.Update(solicitacaoBeneficio);
                        RepositoryService.SolicitacaoBeneficio.AlterarStatus(solicitacaoBeneficio.ID.Value, solicitacaoBeneficio.State.Value, solicitacaoBeneficio.Status.Value);
                        break;

                    case Enum.Tarefa.Resultado.PagamentoAutorizado:
                        var solicitacaoPagamentoAutorizado = new SolicitacaoBeneficio(solicitacaoBeneficio.OrganizationName, solicitacaoBeneficio.IsOffline)
                        {
                            ID = solicitacaoBeneficio.ID,
                            StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente,
                            Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.ReembolsoPendente,
                            IntegrarNoPlugin = false
                        };
                        RepositoryService.SolicitacaoBeneficio.Update(solicitacaoPagamentoAutorizado);
                        break;

                    case Enum.Tarefa.Resultado.PagamentoEfetuadoPedidoGerado:
                        if (solicitacaoBeneficio.StatusSolicitacao != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                        {
                            var solicitacaoPagamentoEfetuado = new SolicitacaoBeneficio(solicitacaoBeneficio.OrganizationName, solicitacaoBeneficio.IsOffline)
                            {
                                ID = solicitacaoBeneficio.ID,
                                StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado,
                                Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.Reembolsado,
                                IntegrarNoPlugin = true
                            };
                            RepositoryService.SolicitacaoBeneficio.Update(solicitacaoPagamentoEfetuado);
                        }
                        break;

                    case Enum.Tarefa.Resultado.PagamentoNaoAutorizado:
                        solicitacaoBeneficio.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada;
                        solicitacaoBeneficio.Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.NaoAprovada;
                        solicitacaoBeneficio.State = (int)Enum.SolicitacaoBeneficio.State.Ativo;
                        RepositoryService.SolicitacaoBeneficio.Update(solicitacaoBeneficio);
                        RepositoryService.SolicitacaoBeneficio.AlterarStatus(solicitacaoBeneficio.ID.Value, solicitacaoBeneficio.State.Value, solicitacaoBeneficio.Status.Value);
                        return;

                    case Enum.Tarefa.Resultado.Favoravel:
                        var solicitacaoBeneficioFavoravel = new SolicitacaoBeneficio(solicitacaoBeneficio.OrganizationName, solicitacaoBeneficio.IsOffline)
                        {
                            ID = solicitacaoBeneficio.ID,
                            StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise,
                            Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.EmAnalise
                        };

                        RepositoryService.SolicitacaoBeneficio.Update(solicitacaoBeneficioFavoravel);
                        break;
                }



                CriarTarefasSolicitacaoBeneficio(solicitacaoBeneficio, UsuarioId, Task.Ordem.Value + 1);
            }
        }

        #endregion

        #region Solicitacao de Cadastro
        public void CriacaoDeTarefasSolicitacaoDeCadastro(SolicitacaoCadastro SolCadastro, Guid usuarioId, int Ordem)
        {
            Model.Processo tmpProcesso = RepositoryService.Processo.ObterPorTipoDeSolicitacao(SolCadastro.TipoDeSolicitacao.Id, null);

            if (tmpProcesso == null)
                return;

            string ParecerAnterior = String.Empty;

            if (Ordem > 1)
            {
                //Busca os pareceres anteriores.
                //SolCadastro.ID.Value
                ParecerAnterior = OrdemMaior1(SolCadastro.ID.Value);
            }

            List<ParticipantesDoProcesso> lstParticipanteDoProcesso = RepositoryService.ParticipantesDoProcesso.ListarPor(tmpProcesso.ID.Value, Ordem);
            bool CriouTarefa = false;

            foreach (ParticipantesDoProcesso PartProcesso in lstParticipanteDoProcesso)
            {
                Lookup referenteA = new Lookup(SolCadastro.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<SolicitacaoCadastro>());
                CriouTarefa = false;
                if (PartProcesso.Papel.Name == "Informado")
                {
                    this.CriarEmail(referenteA, PartProcesso);

                    #region código comentado, foi isolado um método generico citado acima
                    //var email = new Domain.Model.Email(NomeDaOrganizacao, _isOffline);
                    //email.Assunto = "a definir";
                    //email.Mensagem = "a definir";
                    //email.De = new Lookup[1];
                    //email.De[0] = new Lookup { Id = SolCadastro.Proprietario.Id, Type = SolCadastro.Proprietario.Type };
                    //email.RefenteA = new Lookup { Id = SolCadastro.ID.Value, Type = "itbc_solicitacaodecadastro" };

                    //if (PartProcesso.Equipe != null)
                    //{
                    //    List<TeamMembership> lstTeam = RepositoryService.TeamMembership.ListarPor(PartProcesso.Equipe.Id);
                    //    email.Para = new Lookup[lstTeam.Count()];
                    //    int i = 0;

                    //    foreach (TeamMembership team in lstTeam)
                    //    {
                    //        email.Para[i] = new Lookup { Id = team.Usuario, Type = "systemuser" };
                    //        i++;
                    //    }
                    //}

                    //if (PartProcesso.Usuario != null)
                    //{
                    //    email.Para = new Lookup[1];
                    //    email.Para[0] = new Lookup { Id = PartProcesso.Usuario.Id, Type = PartProcesso.Usuario.Type };
                    //}

                    //if (PartProcesso.Contato != null)
                    //{
                    //    email.Para = new Lookup[1];
                    //    email.Para[0] = new Lookup { Id = PartProcesso.Contato.Id, Type = PartProcesso.Contato.Type };
                    //}

                    //email.Direcao = false;
                    //email.ID = RepositoryService.Email.Create(email);

                    ////RepositoryService.RepositorioEmail.EnviarEmail(email.ID.Value);
                    #endregion
                }
                else
                {
                    CriouTarefa = this.CriarTarefa(referenteA, SolCadastro.TipoDeSolicitacao.Name, SolCadastro.Descricao, ParecerAnterior, PartProcesso);

                    #region código comentado, foi isolado um método generico citado acima
                    //Model.Tarefa task = new Model.Tarefa(NomeDaOrganizacao, _isOffline);
                    //task.Assunto = "Plugin - " + SolCadastro.Nome;
                    //task.ReferenteA = new Lookup(SolCadastro.ID.Value, "itbc_solicitacaodecadastro");
                    //task.Assunto = PartProcesso.Papel.Name + " - " + SolCadastro.TipoDeSolicitacao.Name;
                    //task.Ordem = PartProcesso.Ordem;
                    //task.PareceresAnteriores = ParecerAnterior;
                    //task.DescricaoSolicitacao = SolCadastro.Descricao;
                    //task.Conclusao = DateTime.Now.AddDays(1).AddHours(3);


                    //TipoDeAtividade tmpTipoDeAtividade = RepositoryService.TipoDeAtividade.ObterPorPapel(PartProcesso.Papel.Id);

                    //if (tmpTipoDeAtividade == null)
                    //    throw new ArgumentException("Tipo de Atividade do Participante Não Encontrado!");

                    //if (tmpTipoDeAtividade.ID.HasValue)
                    //    task.TipoDeAtividade = new Lookup(tmpTipoDeAtividade.ID.Value, "itbc_tipoatividade");

                    //if (PartProcesso.Equipe != null)
                    //    task.Proprietario = new Lookup(PartProcesso.Equipe.Id, PartProcesso.Equipe.Type);

                    //if (PartProcesso.Usuario != null)
                    //    task.Proprietario = new Lookup(PartProcesso.Usuario.Id, PartProcesso.Usuario.Type);

                    //if (PartProcesso.Contato != null)
                    //    task.Proprietario = new Lookup(PartProcesso.Contato.Id, PartProcesso.Contato.Type);

                    //RepositoryService.Tarefa.Create(task);
                    //CriouTarefa = true;
                    #endregion
                }

                if (!CriouTarefa)
                    CriacaoDeTarefasSolicitacaoDeCadastro(SolCadastro, usuarioId, Ordem + 1);
            }

        }

        public void ConcluirTarefaSolicitacaoDeCadastro(Tarefa Task, Guid UsuarioId)
        {
            if (Task.ReferenteA.Type != "itbc_solicitacaodecadastro")
                return;

            Model.SolicitacaoCadastro SolCadastro = RepositoryService.SolicitacaoCadastro.ObterPor(Task.ReferenteA.Id);
            // Model.ParticipantesDoProcesso PartProcesso = RepositoryService.ParticipantesDoProcesso.ListarPor(tmpProcesso.ID.Value, Ordem);


            if (Task.Resultado.Value == (int)Domain.Enum.Tarefa.Resultado.Reprovada)
            {
                ////var email = new Domain.Model.Email(NomeDaOrganizacao, _isOffline);
                ////email.Assunto = "a definir - Tarefa Reprovada";
                ////email.Mensagem = "a definir - Tarefa Reprovada";
                ////email.De = new Lookup[1];
                ////email.De[0] = new Lookup { Id = SolCadastro.Proprietario.Id, Type = SolCadastro.Proprietario.Type };
                ////email.RefenteA = new Lookup { Id = SolCadastro.ID.Value, Type = "itbc_solicitacaodecadastro" };

                ////if (PartProcesso.Usuario != null)
                ////{
                ////    email.Para = new Lookup[1];
                ////    email.Para[0] = new Lookup { Id = PartProcesso.Usuario.Id, Type = PartProcesso.Usuario.Type };
                ////}


                ////email.Direcao = false;
                ////email.ID = RepositoryService.RepositorioEmail.Create(email);

                return;
            }
            else
            {
                CriacaoDeTarefasSolicitacaoDeCadastro(SolCadastro, UsuarioId, Task.Ordem.Value + 1);
            }
        }

        public ParticipantesDoProcesso BuscarParticipanteProcesso(int ordem, Guid tipoSolicitacao)
        {
            return RepositoryService.ParticipantesDoProcesso.ObterPor(ordem, tipoSolicitacao);
        }
        #endregion

        #endregion

    }
}
