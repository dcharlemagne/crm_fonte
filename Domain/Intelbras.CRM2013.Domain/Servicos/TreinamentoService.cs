using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.IO;
using System.Net;
using SDKore.Helper;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class TreinamentoService
    {
        #region Objetos
        private string OrganizationName = SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras");
        private string emailAEnviarLog = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Usuario.EnvioEmail");
        private Boolean isOffline = false;
        #endregion

        public SDKore.Helper.Trace Trace { get; set; }
        private List<string> mensagemLog = new List<string>();

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

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public TreinamentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public TreinamentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public TreinamentoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion


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

        CompromissosDoCanalService _compromissosDoCanal = null;
        private CompromissosDoCanalService CompromissosDoCanal
        {
            get
            {
                if (_compromissosDoCanal == null)
                    _compromissosDoCanal = new CompromissosDoCanalService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _compromissosDoCanal;
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

        ProdutoService _Produto = null;
        private ProdutoService Produto
        {
            get
            {
                if (_Produto == null)
                    _Produto = new ProdutoService(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _Produto;
            }
        }

        private IProdutoTreinamento<ProdutoTreinamento> _produtoTreinamento = null;
        public IProdutoTreinamento<ProdutoTreinamento> ProdutoTreinamento
        {
            get
            {
                if (_produtoTreinamento == null)
                    _produtoTreinamento = RepositoryFactory.GetRepository<IProdutoTreinamento<ProdutoTreinamento>>(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _produtoTreinamento;
            }
        }

        private ITreinamentoCertificacao<TreinamentoCertificacao> _treinamentoCertificacao = null;
        public ITreinamentoCertificacao<TreinamentoCertificacao> TreinamentoCertificacao
        {
            get
            {
                if (_treinamentoCertificacao == null)
                    _treinamentoCertificacao = RepositoryFactory.GetRepository<ITreinamentoCertificacao<TreinamentoCertificacao>>(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _treinamentoCertificacao;
            }
        }

        private ITreinamentoCanal<TreinamentoCanal> _treinamentoCanal = null;
        public ITreinamentoCanal<TreinamentoCanal> TreinamentoCanal
        {
            get
            {
                if (_treinamentoCanal == null)
                    _treinamentoCanal = RepositoryFactory.GetRepository<ITreinamentoCanal<TreinamentoCanal>>(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"), false);

                return _treinamentoCanal;
            }
        }

        public void VerificaCumprimento(TreinamentoCanal treinamentoCanal)
        {
            string statusCompromisso = Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido;

            if (QuantidadeRequerida(treinamentoCanal.Canal.Id, treinamentoCanal.Treinamento.Id))
                statusCompromisso = Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido;

            AtualizaTreinamentoCanal((Guid)treinamentoCanal.ID, statusCompromisso);
        }

        public List<TreinamentoCanal> ListarTreinamentoPorCanal(Guid Canal)
        {
            return RepositoryService.TreinamentoCanal.ListarPor(null, Canal);
        }

        public void VerificaSeContatoEstaNoCanal(Guid Canal, Guid Contato, Guid treinacolab)
        {
            Contato Treinado = RepositoryService.Contato.Retrieve(Contato);
            if (Treinado.AssociadoA == null)
                throw new ArgumentException("Erro: O Colaborador (Contato ["+Contato+"]) não esta relacionado a um Canal. Canal associado ao treinamento: ["+ Canal +"]. Id Treinamento do Colaborador: "+treinacolab);

            if (Treinado.AssociadoA.Id != Canal)
                throw new ArgumentException("Erro: O Colaborador (Contato [" + Contato + "]) deve estar relacionado com o Canal do Treinamento.");
        }

        public void VerificaCumprimento(ColaboradorTreinadoCertificado colaboradorTreiCert)
        {
            if (colaboradorTreiCert.Canal != null && colaboradorTreiCert.Canal.Id != null)
            {
                List<TreinamentoCanal> lstTreinamentosCanal = RepositoryService.TreinamentoCanal.ListarPor(colaboradorTreiCert.TreinamentoCertificado.Id, colaboradorTreiCert.Canal.Id);

                string statusCompromisso = string.Empty;

                foreach (TreinamentoCanal treinamento in lstTreinamentosCanal)
                {
                    statusCompromisso = Domain.Enum.CompromissoCanal.StatusCompromisso.Nao_Cumprido;

                    if (QuantidadeRequerida(colaboradorTreiCert.Canal.Id, colaboradorTreiCert.TreinamentoCertificado.Id))
                        statusCompromisso = Domain.Enum.CompromissoCanal.StatusCompromisso.Cumprido;

                    //AtualizaTreinamentoCanal((Guid)colaboradorTreiCert.TreinamentoCertificado.Id, statusCompromisso);
                    AtualizaTreinamentoCanal(treinamento.ID.Value, statusCompromisso);
                }
            }
        }

        public List<ColaboradorTreinadoCertificado> ListarExpirados()
        {
            return RepositoryService.ColaboradorTreinadoCertificado.ListarExpirados();
        }

        public void Desativar(List<ColaboradorTreinadoCertificado> lstColaboradores)
        {
            foreach (ColaboradorTreinadoCertificado _Colaboradores in lstColaboradores)
            {
                //status 1 = desativar.                
                RepositoryService.ColaboradorTreinadoCertificado.AlterarStatus(_Colaboradores.ID.Value, 1);
            }
        }

        private bool QuantidadeRequerida(Guid canalId, Guid treinamentoId)
        {
            List<ProdutoTreinamento> lstProdTreina = RepositoryService.ProdutoTreinamento.ListarPor(treinamentoId);

            if (lstProdTreina.Count > 0)
            {
                int? qtdRequerida = lstProdTreina.Max(x => x.NroMinimoProf);

                if (qtdRequerida.HasValue)
                {
                    //Consulta na entidade “Colaborador Treinado/Certificado” [itbc_colaboradorestreincert],
                    //quantos colaboradores associados ao canal tem o treinamento (coluna [itbc_treinamcertifid]) dentro da validade,
                    List<ColaboradorTreinadoCertificado> lstColaboradoresTreina = RepositoryService.ColaboradorTreinadoCertificado.ListarPor(canalId, treinamentoId);

                    ////se for igual ou maior atualiza o status [itbc_Status] da entidade [itbc_treinamento_certificacao_canal] com status igual a cumprido,
                    ////caso contrário como não cumprido.
                    if (lstColaboradoresTreina.Count >= qtdRequerida)
                        return true;
                }
            }

            return false;
        }

        public void AtualizaTreinamentoCanal(Guid treinamentoId, string statusCompromisso)
        {
            TreinamentoCanal treinamentoCanal = RepositoryService.TreinamentoCanal.ObterPor(treinamentoId);
            StatusCompromissos status = RepositoryService.StatusCompromissos.ObterPor(statusCompromisso);

            treinamentoCanal.StatusCompromisso = new Lookup((Guid)status.ID, "");

            RepositoryService.TreinamentoCanal.Update(treinamentoCanal);

        }

        // TREINAMENTO
        // CASO DE USO 1 – GERAÇÃO TREINAMENTO E CERTIFICAÇÃO DO CANAL
        public void GeracaoTreinamentoECertificacaoDoCanal()
        {
            Trace = new SDKore.Helper.Trace("GeraTreinamento");
            mensagemLog = new List<string>();

            InserirLog(DateTime.Now + " - Inicio Geração Treinamento e Certificacao do Canal");

            ParametroGlobal paramGlobal = ParametroGlobal.ListarParamGlobalPorTipoParam((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.DatasTrimestre).FirstOrDefault();
            if (paramGlobal == null)
                throw new ApplicationException("A execução do monitoramento foi interrompida, o parâmetro global não foi encontrado ou está preenchido com valores incorretos.");

            List<Conta> lstContas = ContaService.ListarContasParticipantes();

            InserirLog(DateTime.Now + " - Foram encontrados " + lstContas.Count + " participantes do PCI para processar!");


            foreach (Conta canal in lstContas)
            {
                try
                {
                    InserirLog(string.Empty);
                    InserirLog(DateTime.Now + " -- Canal[" + canal.CodigoMatriz + "]");

                    List<TreinamentoCanal> lstTreinamentoCanal = this.ListarTreinamentoPorCanal(canal.ID.Value);

                    InserirLog(DateTime.Now + " --- Foram encontrados " + lstTreinamentoCanal.Count() + " treinamento do canal!");

                    // Inativa status de treinamentos e certificações
                    this.InativaTreinamentosDeCanal(lstTreinamentoCanal);

                    // lista produtos portifolios
                    List<ProdutoPortfolio> lstProdutoPortifolio = new Servicos.ProdutoService(OrganizationName, isOffline).ProdutosPortfolio(canal, canal.Classificacao.Id, null);

                    InserirLog(DateTime.Now + " --- Foram encontrados " + lstProdutoPortifolio.Count + " produtos do portfolio!");

                    // filtra produto por produto que exija treinamento
                    var lstProdPortExigeTrein = lstProdutoPortifolio.Where(x => x.Product.ExigeTreinamento == true);

                    InserirLog(DateTime.Now + " --- Foram encontrados " + lstProdPortExigeTrein.Count() + " produtos do portfolio que exigem treinamento!");

                    // para cada produto portifolio
                    foreach (var prodPort in lstProdPortExigeTrein)
                    {
                        InserirLog(DateTime.Now + " ---- Produto: " + prodPort.Product.Codigo + " - Unidade Negocio: " + prodPort.Product.UnidadeNegocio.Name);

                        List<CompromissosDoCanal> lstCompCanal = new List<CompromissosDoCanal>();
                        // se forma de apuração de beneficio e compromisso do canal for por filial
                        if (canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Por_Filiais
                            || canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Matriz)
                        {
                            // compromisso do canal 33
                            lstCompCanal = CompromissosDoCanal.ListarPorCod33EPorMatriz(canal.ID.Value, prodPort.Product.UnidadeNegocio.Id);
                        }
                        else if (canal.ApuracaoBeneficiosCompromissos.Value == (int)Enum.Conta.ApuracaoDeBeneficiosECompromissos.Centralizada_Na_Matriz
                            && canal.TipoConta == (int)Enum.Conta.MatrizOuFilial.Filial)
                        {
                            lstCompCanal = CompromissosDoCanal.ListarPorCod33EPorMatriz(canal.ContaPrimaria.Id, prodPort.Product.UnidadeNegocio.Id);
                        }

                        InserirLog(DateTime.Now + " ----- Foram encontrados " + lstCompCanal.Count() + " Compromisso do Canal ");

                        if (lstCompCanal.Count() > 0)
                        {
                            // lista de produto treinamento certificação
                            List<ProdutoTreinamento> lstProdTrei = ProdutoTreinamento.ListarPorProduto(prodPort.Produto.Id);

                            InserirLog(DateTime.Now + " ------ Foram encontrados " + lstProdTrei.Count() + " Produtos do Treinamento ");

                            // Cria TReinamento do canal ou ativa se ja existir
                            foreach (var prodTrei in lstProdTrei)
                            {
                                InserirLog(DateTime.Now + " ------- Produto por Treinamento Certificação: " + prodTrei.Treinamento.Name);

                                this.CriaTreinamentoCanalOuAtiva(prodTrei.Treinamento.Id, prodTrei.Treinamento.Name, canal, lstCompCanal[0].ID);
                            }
                        }
                    }
                                    }
                catch (Exception ex)
                {
                    string mensagem = SDKore.Helper.Error.Handler(ex);
                    InserirLog(DateTime.Now + " - ERRO: " + mensagem);
                }
            }

            InserirLog(string.Empty);
            InserirLog(DateTime.Now + " - Fim GeracaoTreinamentoECertificacaoDoCanal");

            this.EnviaEmailDeLog("Geracao Treinamento e Certificação");
        }

        private void CriaTreinamentoCanalOuAtiva(Guid prodTreiId, string nomeTreinamento, Conta canal, Guid? compCanalId)
        {
            List<TreinamentoCanal> lstTreiCanal = TreinamentoCanal.ListarPorInativo(prodTreiId, canal.ID, compCanalId);

            if (lstTreiCanal.Count > 0)
            {
                foreach (var TreiCanal in lstTreiCanal)
                {
                    InserirLog(DateTime.Now + " --------- Ativando Treinamento do Canal: " + TreiCanal.ID.Value);

                    RepositoryService.TreinamentoCanal.AlterarStatus(TreiCanal.ID.Value, 0);
                }
            }
            else
            {
                #region Cria treinamento canal
                
                TreinamentoCanal treinamentocanal = new TreinamentoCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                treinamentocanal.Canal = new Lookup(canal.ID.Value, "account");
                treinamentocanal.Treinamento = new Lookup(prodTreiId, "itbc_treinamento_certificacao_canal");
                treinamentocanal.CompromissoCanal = new Lookup(compCanalId.Value, "itbc_compdocanal");
                string _tempNomeTreinamento = canal.RazaoSocial + " - " + nomeTreinamento;
                treinamentocanal.Nome = (_tempNomeTreinamento.Length < 100) ? _tempNomeTreinamento : _tempNomeTreinamento.Substring(0, 100);
                treinamentocanal.DataLimite = DateTime.Today.AddMonths(1);
                treinamentocanal.StatusCompromisso = new Lookup(StatusCompromissoNaoCumprido.ID.Value, "");

                treinamentocanal.ID = RepositoryService.TreinamentoCanal.Create(treinamentocanal);

                InserirLog(DateTime.Now + " --------- Criar Treinamento do Canal: " + treinamentocanal.ID);
                #endregion
            }
        }
        
        private void InativaTreinamentosDeCanal(List<TreinamentoCanal> lstTreinamentoCanal)
        {
            foreach (var TreiCanal in lstTreinamentoCanal)
                RepositoryService.TreinamentoCanal.AlterarStatus(TreiCanal.ID.Value, 1);
        }

        private void AlteraStatusaTreinamentosDeCanal(List<TreinamentoCanal> lstTreinamentoCanal)
        {
            if (lstTreinamentoCanal.Count > 0)
            {
                foreach (TreinamentoCanal tc in lstTreinamentoCanal)
                    RepositoryService.TreinamentoCanal.AlterarStatus(tc.ID.Value, 1);
            }
        }

        // TREINAMENTO
        // CASO DE USO 2 - VERIFICAÇÃO DO STATUS DOS TREINAMENTOS E CERTIFICAÇÃO DO CANAL
        public void VerificacaoDoStatusTreinamentosECertificacaoCanal()
        {
            Trace = new SDKore.Helper.Trace("MonitoramentoVerificaStatus");
            mensagemLog = new List<string>();

            InserirLog(DateTime.Now + " - Inicio - Verificação do Status dos Treinamentos e Certificados dos Canais!");

            ParametroGlobal paramGlobal = ParametroGlobal.ListarParamGlobalPorTipoParam((int)Intelbras.CRM2013.Domain.Enum.TipoParametroGlobal.DatasTrimestre).FirstOrDefault();
            if (paramGlobal == null)
                throw new ApplicationException("A execução do monitoramento foi interrompida, o parâmetro global não foi encontrado ou está preenchido com valores incorretos.");

            // Obtem lista de canais matriz ou filial // (CASO USO TREINAMENTO 2: STEP 1)
            List<Conta> lstContas = ContaService.ListarContasParticipantes();

            InserirLog(string.Empty);
            InserirLog(DateTime.Now + " - Foram encontrados " + lstContas.Count + " participantes do PCI para processar!");

            foreach (Conta canal in lstContas)
            {
                // (CASO USO TREINAMENTO 2: STEP 3)
                List<TreinamentoCanal> lstTreinamentoCanal = this.ListarTreinamentoPorCanal(canal.ID.Value);

                InserirLog(string.Empty);
                InserirLog(DateTime.Now + " --- Foram encontrados " + lstTreinamentoCanal.Count + " treinamentos do Canal [" + canal.CodigoMatriz + "] para processar!");

                // (CASO USO TREINAMENTO 2: STEP 4)
                foreach (var treinamentoCanal in lstTreinamentoCanal)
                {
                    try
                    {
                        InserirLog(DateTime.Now + " ----- Treinamentos do Canal [" + treinamentoCanal.Nome + "]");
                        

                        // (CASO USO TREINAMENTO 2: STEP 5)
                        TreinamentoCertificacao treinamentoCertficacao = TreinamentoCertificacao.ObterPor(treinamentoCanal.Treinamento.Id);
                        if (treinamentoCanal.CompromissoCanal != null)
                        {
                            CompromissosDoCanal compCanal = CompromissoDoCanal.BuscarPorGuid(treinamentoCanal.CompromissoCanal.Id);

                            // (CASO USO TREINAMENTO 2: STEP 5)
                            if (treinamentoCertficacao.ID != null && compCanal.UnidadeDeNegocio != null)
                            {
                                List<ProdutoTreinamento> lstProdTrei = ProdutoTreinamento.ListarPorTreinamento(treinamentoCertficacao.ID.Value, compCanal.UnidadeDeNegocio.Id);
                                int numMinimoProfissionais = lstProdTrei.Max(x => x.NroMinimoProf.Value);

                                // (CASO USO TREINAMENTO 2: STEP 6)
                                List<ColaboradorTreinadoCertificado> lstColaboradorTreinadoCertificadoDoCanal = RepositoryService.ColaboradorTreinadoCertificado.ListarPor(canal.ID.Value, treinamentoCertficacao.ID.Value);

                                // (CASO USO TREINAMENTO 2: STEP 7)
                                if (lstColaboradorTreinadoCertificadoDoCanal.Count >= numMinimoProfissionais)
                                {
                                    InserirLog(DateTime.Now + " ----- Atualizando para Cumprido");

                                    treinamentoCanal.StatusCompromisso = new Lookup(StatusCompromissoCumprido.ID.Value, "");
                                    RepositoryService.TreinamentoCanal.Update(treinamentoCanal);
                                }
                                else if (treinamentoCanal.DataLimite < DateTime.Today)
                                {
                                    InserirLog(DateTime.Now + " ----- Atualizando para Não Cumprido");
                                    
                                    treinamentoCanal.StatusCompromisso = new Lookup(StatusCompromissoNaoCumprido.ID.Value, "");
                                    RepositoryService.TreinamentoCanal.Update(treinamentoCanal);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string mensagem = SDKore.Helper.Error.Handler(ex);
                        InserirLog(DateTime.Now + " - ERRO: " + mensagem);
                    }
                }
            }


            InserirLog(string.Empty);
            InserirLog(DateTime.Now + " - Fim - Verificação do Status dos Treinamentos e Certificados dos Canais!");

            this.EnviaEmailDeLog("Verificação do status do Treinamento");
        }

        private void InserirLog(string mensagem)
        {
            Trace.Add(mensagem);
            this.mensagemLog.Add(mensagem);
        }

        private void EnviaEmailDeLog(string subject)
        {
            if (mensagemLog.Count == 0)
            {
                return;
            }

            String msg = string.Empty;

            var email = new Intelbras.CRM2013.Domain.Model.Email(OrganizationName, isOffline);
            email.Assunto = "Monitoramento - " + subject;

            foreach (string item in mensagemLog)
            {
                msg += string.Format("<br />{0}", item);
            }

            email.Mensagem = msg;
            email.Para = new Lookup[1];
            email.Para[0] = new Lookup { Id = Guid.Parse(emailAEnviarLog), Type = SDKore.Crm.Util.Utility.GetEntityName<Usuario>() };

            email.Direcao = false;
            email.ID = RepositoryService.Email.Create(email);

            RepositoryService.Email.EnviarEmail(email.ID.Value);
        }
    }
}
