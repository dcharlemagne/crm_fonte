using Intelbras.CRM2013.Domain.Model;
using Microsoft.Xrm.Sdk;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SolicitacaoBeneficioService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SolicitacaoBeneficioService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public SolicitacaoBeneficioService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public SolicitacaoBeneficioService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        #region Propertys e Objetos

        private ProcessoDeSolicitacoesService _ServiceProcessoDeSolicitacoes = null;
        private ProcessoDeSolicitacoesService ServiceProcessoDeSolicitacoes
        {
            get
            {
                if (_ServiceProcessoDeSolicitacoes == null)
                    _ServiceProcessoDeSolicitacoes = new ProcessoDeSolicitacoesService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

                return _ServiceProcessoDeSolicitacoes;
            }
        }

        private TarefaService _ServiceTarefaService = null;
        private TarefaService ServiceTarefaService
        {
            get
            {
                if (_ServiceTarefaService == null)
                    _ServiceTarefaService = new TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);

                return _ServiceTarefaService;
            }
        }
        #endregion

        public void Atualizar(SolicitacaoBeneficio solicitacao)
        {
            RepositoryService.SolicitacaoBeneficio.Update(solicitacao);
        }

        public bool AtualizarStatus(Guid Solicitacao, int state, int status)
        {
            return RepositoryService.SolicitacaoBeneficio.AlterarStatus(Solicitacao, state, status);
        }

        public SolicitacaoBeneficio ObterPor(Guid solicitacaoId)
        {
            return RepositoryService.SolicitacaoBeneficio.Retrieve(solicitacaoId);
        }

        public List<SolicitacaoBeneficio> ListarPorBeneficioCanal(Guid beneficioCanalId)
        {
            return RepositoryService.SolicitacaoBeneficio.ListarPorBeneficioCanal(beneficioCanalId);
        }

        public List<SolicitacaoBeneficio> ListarPorBeneficioCanalEAjusteSaldo(Guid beneficioCanalId, Boolean ajusteSaldo)
        {
            return RepositoryService.SolicitacaoBeneficio.ListarPorBeneficioCanalEAjusteSaldo(beneficioCanalId, ajusteSaldo);
        }

        public List<SolicitacaoBeneficio> ListarPorBeneficioCanalEStatus(Guid beneficiocanalId, Guid beneficioPrograma, int status)
        {
            return RepositoryService.SolicitacaoBeneficio.ListarPorBeneficioCanalEStatus(beneficiocanalId, beneficioPrograma, status);
        }

        public void GerarTarefaSolicBeneficio(SolicitacaoBeneficio mSolicitacaoBeneficio, Guid usuarioId, int ordem)
        {
            ServiceProcessoDeSolicitacoes.CriarTarefasSolicitacaoBeneficio(mSolicitacaoBeneficio, usuarioId, ordem);
        }

        public void CalculaValorAprovado(SolicitacaoBeneficio mSolBeneficio)
        {
            SolicitacaoBeneficio mSolicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.Retrieve(mSolBeneficio.ID.Value);

            decimal VlrAprovado = 0;
            BeneficioDoCanal mBeneficioDoCanal = RepositoryService.BeneficioDoCanal.Retrieve(mSolicitacaoBeneficio.BeneficioCanal.Id);
            List<SolicitacaoBeneficio> lstSolicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.ListarAprovado(mBeneficioDoCanal.ID.Value, mSolicitacaoBeneficio.Canal.Id, mSolicitacaoBeneficio.UnidadedeNegocio.Id, mSolicitacaoBeneficio.BeneficioPrograma.Id);

            foreach (SolicitacaoBeneficio item in lstSolicitacaoBeneficio)
            {
                VlrAprovado += item.ValorAprovado.HasValue ? item.ValorAprovado.Value : 0;
            }

            mBeneficioDoCanal.TotalSolicitacoesAprovadasNaoPagas = VlrAprovado;
            RepositoryService.BeneficioDoCanal.Update(mBeneficioDoCanal);
        }

        /*
        public bool TemSituacaoInregularVMC(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (solicitacaoBeneficio.BeneficioPrograma != null && !solicitacaoBeneficio.AjusteSaldo.Value)
            {
                var beneficioPrograma = RepositoryService.Beneficio.Retrieve(solicitacaoBeneficio.BeneficioPrograma.Id, "itbc_codigo");

                if (beneficioPrograma.Codigo.Value == (int)Enum.BeneficiodoPrograma.Codigos.VMC)
                {
                    if (!solicitacaoBeneficio.ValorAcao.HasValue)
                    {
                        throw new ArgumentException("(CRM) Valor da Ação não informado.");
                    }

                    if (!solicitacaoBeneficio.ValorSolicitado.HasValue)
                    {
                        throw new ArgumentException("(CRM) Valor da Solicitação não informado.");
                    }

                    int codigoParametroGlobal = (int)Enum.TipoParametroGlobal.PadraoContrapartidaEmAcaoVMC;
                    ParametroGlobal mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal(codigoParametroGlobal);

                    decimal VlrResultado = ((solicitacaoBeneficio.ValorSolicitado.Value / solicitacaoBeneficio.ValorAcao.Value) - 1) * 100;

                    if (VlrResultado < 0)
                    {
                        VlrResultado = VlrResultado * (-1);
                    }

                    return VlrResultado < Convert.ToDecimal(mParametroGlobal.Valor);
                }
            }

            return false;
        }*/

        /// <summary>
        /// Preenche o campo forma pagamento da solicitação de benefício
        /// </summary>
        /// <param name="target">Entidade Target</param>
        /// <param name="imagem">Entidade Imagem</param>
        /// <returns>Entidade modificada</returns>
        public Entity PreencherFormaPagamento(Entity target, Entity imagem)
        {
            Guid? unidNeg = null, benefProg = null;
            List<ParametroGlobal> lstParam = new List<ParametroGlobal>();

            if (imagem == null)
            {
                //Campo obrigatório
                if (!target.Contains("itbc_beneficiodoprograma"))
                    throw new ArgumentException("(CRM) Ocorreu um erro ao obter a forma de pagamento da solicitação de benefício");

                benefProg = ((EntityReference)target.Attributes["itbc_beneficiodoprograma"]).Id;

                //Campo não obrigatório
                if (target.Contains("itbc_businessunitid"))
                    unidNeg = ((EntityReference)target.Attributes["itbc_businessunitid"]).Id;
            }
            else
            {
                if (!imagem.Contains("itbc_beneficiodoprograma"))
                    throw new ArgumentException("(CRM) Ocorreu um erro ao obter a forma de pagamento da solicitação de benefício");

                benefProg = ((EntityReference)imagem.Attributes["itbc_beneficiodoprograma"]).Id;

                if (imagem.Contains("itbc_businessunitid"))
                    unidNeg = ((EntityReference)imagem.Attributes["itbc_businessunitid"]).Id;
            }

            #region Pega o parametro com base no codigo 33 ("Forma de Pagamento de beneficio") + Unidade de negocio + beneficio do programa, caso nao encontre manda sem a unidade de neg
            lstParam = RepositoryService.ParametroGlobal.ListarPor(33, unidNeg.Value, null, null, null, null, benefProg.Value);
            if (lstParam.Count <= 0)
                lstParam = RepositoryService.ParametroGlobal.ListarPor(33, null, null, null, null, null, benefProg.Value);
            #endregion
            Domain.Model.FormaPagamento formatoPgto = null;
            if (lstParam != null && lstParam.Count > 0)
            {
                formatoPgto = RepositoryService.FormaPagamento.ObterPor(lstParam.First().Valor.ToString());
            }
            else
            {
                throw new ArgumentException("(CRM) Não há ParametroGlobal para o Beneficio Id : " + benefProg.Value.ToString());
            }

            if (formatoPgto == null)
                throw new ArgumentException("(CRM) Formato Pagamento não encontrado para o benefício " + ((EntityReference)target.Attributes["itbc_beneficiodoprograma"]).Name);

            if (target.Attributes.Contains("itbc_formapagamentoid"))
            {
                if (target.Attributes["itbc_formapagamentoid"] != null && ((EntityReference)target.Attributes["itbc_formapagamentoid"]).Id != formatoPgto.ID.Value)
                {
                    throw new ArgumentException("(CRM) Forma de pagamento incompatível com regra de negócio. ");
                }
            }
            target.Attributes["itbc_formapagamentoid"] = new EntityReference("", formatoPgto.ID.Value);
            return target;
        }

        public void ValidaValorAcao(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (solicitacaoBeneficio.ValorSolicitado.HasValue && solicitacaoBeneficio.ValorAcao.HasValue)
            {
                if (solicitacaoBeneficio.ValorSolicitado.Value > solicitacaoBeneficio.ValorAcao.Value)
                {
                    throw new ArgumentException("(CRM) O Valor Solicitado não pode ser maior que o Valor da Ação!");
                }
            }
        }

        public void ValidaValorAprovado(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (!solicitacaoBeneficio.ValorAprovado.HasValue)
            {
                throw new ArgumentException("(CRM) O Valor Aprovado é obrigatório!");
            }

            if (solicitacaoBeneficio.ValorSolicitado.HasValue)
            {
                if (solicitacaoBeneficio.ValorAprovado.Value > solicitacaoBeneficio.ValorSolicitado.Value)
                {
                    throw new ArgumentException("(CRM) O Valor Aprovado não pode ser maior que o Valor da Solicitado!");
                }
            }

            if (solicitacaoBeneficio.StatusSolicitacao.HasValue)
            {
                if (solicitacaoBeneficio.ValorAprovado.Value == 0)
                {
                    if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado
                        || solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente
                        || solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada)
                    {
                        throw new ArgumentException("(CRM) A solicitação não pode ser aprovada quando o Valor Aprovado é zero.");
                    }
                }
            }
        }

        public void ValidaDataParametrizadaParaConclusao(SolicitacaoBeneficio mSolicitacaoBeneficio)
        {
            ParametroGlobal mParametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Enum.TipoParametroGlobal.PrazoLimiteSolicitacaoReembolsoVMC);

            int diasParaConclusao = Convert.ToInt32(mParametroGlobal.Valor);
            Guid tipoAtividadeAprovacaoId = Guid.Parse(SDKore.Configuration.ConfigurationManager.GetSettingValue("aprovasolicitacao"));

            List<Tarefa> lstTarefa = RepositoryService.Tarefa.ListarPor(mSolicitacaoBeneficio.ID.Value, tipoAtividadeAprovacaoId, null, null, null);

            foreach (Tarefa item in lstTarefa)
            {
                if (item.Status.Value == (int)Enum.Tarefa.StatusCode.Concluida && item.Resultado.Equals((int)Enum.Tarefa.Resultado.Aprovada))
                {
                    DateTime conclusaoaddparameter = item.TerminoReal.Value.AddDays(diasParaConclusao);

                    if (conclusaoaddparameter.Date <= DateTime.Now.Date)
                    {
                        throw new ArgumentException("(CRM) Data de conclusão menor do que data parametrizada.");
                    }
                }
            }
        }

        public void ValidaPrazoLimiteParaCriacao(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (!solicitacaoBeneficio.AjusteSaldo.Value)
            {
                var lstParam = RepositoryService.ParametroGlobal.ListarPor((int)Enum.TipoParametroGlobal.PrazoLimiteParaSolicitarBeneficio, solicitacaoBeneficio.UnidadedeNegocio.Id, null, null, null, null, solicitacaoBeneficio.BeneficioPrograma.Id);

                if (lstParam.Count == 0)
                {
                    throw new ArgumentException("(CRM) Parâmetro Global [" + (int)Enum.TipoParametroGlobal.PrazoLimiteParaSolicitarBeneficio + "] não localizado.");
                }

                int dias = lstParam[0].GetValue<int>();
                DateTime primeiroDiaDoTrimestre = new SDKore.Helper.DateTimeHelper().PrimeiroDiaDoTrimestre();
                DateTime dataLimite = primeiroDiaDoTrimestre.AddDays(dias);

                if (dataLimite < DateTime.Today)
                {
                    throw new ArgumentException("(CRM) Não é possível cadastrar solicitação. Prazo para cadastro finalizado. A data limite para Solicitação de Benefício nesse trimestre foi " + dataLimite + ".");
                }
            }
        }

        public bool MudarStatusSolicitacao(Guid id, int status, int razao)
        {
            return RepositoryService.SolicitacaoBeneficio.AlterarStatus(id, status, razao);
        }

        public SolicitacaoBeneficio Persistir(SolicitacaoBeneficio objSolicitacaoBeneficio)
        {
            if (objSolicitacaoBeneficio.ID.HasValue)
            {
                if (objSolicitacaoBeneficio.Status.Value == (int)Enum.SolicitacaoBeneficio.RazaoStatusInativo.Cancelada)
                {
                    objSolicitacaoBeneficio.Status = null;
                    objSolicitacaoBeneficio.State = null;
                    objSolicitacaoBeneficio.IntegrarNoPlugin = true;
                    RepositoryService.SolicitacaoBeneficio.Update(objSolicitacaoBeneficio);

                    MudarStatusSolicitacao(objSolicitacaoBeneficio.ID.Value, (int)Enum.SolicitacaoBeneficio.State.Inativo,
                                                (int)Enum.SolicitacaoBeneficio.RazaoStatusInativo.Cancelada);
                }
                else
                {
                    RepositoryService.SolicitacaoBeneficio.Update(objSolicitacaoBeneficio);
                }

                return objSolicitacaoBeneficio;
            }
            else
            {
                objSolicitacaoBeneficio.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada;
                objSolicitacaoBeneficio.ID = RepositoryService.SolicitacaoBeneficio.Create(objSolicitacaoBeneficio);
                return objSolicitacaoBeneficio;
            }
        }

        public Guid PersistirMensagemIntegracao(SolicitacaoBeneficio solicitacaoBeneficio, List<ProdutosdaSolicitacao> listaProdutoSolicitacao)
        {
            listaProdutoSolicitacao.ForEach(x => x.IntegrarNoPlugin = true);

            if (solicitacaoBeneficio.ID.HasValue)
            {
                new ProdutosdaSolicitacaoService(RepositoryService).Persistir(listaProdutoSolicitacao);

                bool temErro = Persistir(solicitacaoBeneficio) == null;
                if (temErro)
                {
                    throw new ArgumentException("(CRM) Erro de Persistência!");
                }

                return solicitacaoBeneficio.ID.Value;
            }
            else
            {
                var solicitacaoBeneficioNova = Persistir(solicitacaoBeneficio);
                if (solicitacaoBeneficioNova == null)
                {
                    throw new ArgumentException("(CRM) Erro de Persistência!");
                }


                try
                {
                    if (listaProdutoSolicitacao.Count > 0)
                    {
                        listaProdutoSolicitacao.ForEach(x => x.SolicitacaoBeneficio = new Lookup(solicitacaoBeneficioNova.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(solicitacaoBeneficioNova)));

                        listaProdutoSolicitacao.LastOrDefault().IntegrarNoPlugin = false;
                        new ProdutosdaSolicitacaoService(RepositoryService).Persistir(listaProdutoSolicitacao);

                        Avanca(solicitacaoBeneficio);
                    }
                }
                catch (Exception ex)
                {
                    Deletar(solicitacaoBeneficioNova.ID.Value);
                    throw ex;
                }

                return solicitacaoBeneficioNova.ID.Value;
            }
        }

        public SolicitacaoBeneficio Persistir1(SolicitacaoBeneficio objSolicitacaoBeneficio)
        {
            SolicitacaoBeneficio TmpSolicitacaoBeneficio = null;
            if (objSolicitacaoBeneficio.ID.HasValue)
            {
                TmpSolicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.ObterPor(objSolicitacaoBeneficio.ID.Value, null);

                if (TmpSolicitacaoBeneficio != null)
                {
                    if (TmpSolicitacaoBeneficio.State.HasValue && TmpSolicitacaoBeneficio.State.Value == (int)Enum.SolicitacaoBeneficio.State.Inativo)
                        return TmpSolicitacaoBeneficio;

                    if (objSolicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                    {
                        Tarefa mTarefa = new Intelbras.CRM2013.Domain.Servicos.TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).ObterPorTarefaAtiva(objSolicitacaoBeneficio.ID.Value);

                        if (mTarefa != null)
                        {
                            objSolicitacaoBeneficio.StatusSolicitacao = null;
                        }

                        RepositoryService.SolicitacaoBeneficio.Update(objSolicitacaoBeneficio);

                        if (mTarefa != null)
                        {
                            //Fecha task
                            mTarefa.Resultado = (int)Enum.Tarefa.Resultado.PagamentoEfetuadoPedidoGerado;
                            mTarefa.State = (int)Enum.Tarefa.StateCode.Fechada;

                            string retorno;
                            mTarefa = new Intelbras.CRM2013.Domain.Servicos.TarefaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).Persistir(mTarefa, out retorno);
                        }

                    }
                    else
                    {
                        int statusTemp = objSolicitacaoBeneficio.Status.Value;
                        objSolicitacaoBeneficio.Status = null;
                        RepositoryService.SolicitacaoBeneficio.Update(objSolicitacaoBeneficio);
                        this.MudarStatusSolicitacao(objSolicitacaoBeneficio.ID.Value, objSolicitacaoBeneficio.State.Value, statusTemp);
                    }
                    return objSolicitacaoBeneficio;
                }
                else
                    return null;
            }
            else
            {
                //Para nova solicitação - Sempre insert como criada
                objSolicitacaoBeneficio.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada;
                objSolicitacaoBeneficio.ID = RepositoryService.SolicitacaoBeneficio.Create(objSolicitacaoBeneficio);
                return objSolicitacaoBeneficio;
            }

        }

        public bool MudarStatus(Guid id, int stateCode, int statusCode)
        {
            return RepositoryService.SolicitacaoBeneficio.AlterarStatus(id, stateCode, statusCode);
        }

        public string IntegracaoBarramento(SolicitacaoBeneficio solBeneficio)
        {
            if (solBeneficio.BeneficioPrograma != null)
            {
                var beneficioDoPrograma = RepositoryService.Beneficio.Retrieve(solBeneficio.BeneficioPrograma.Id);

                if (beneficioDoPrograma.Codigo.HasValue)
                {
                    switch (beneficioDoPrograma.Codigo.Value)
                    {
                        case (int)Domain.Enum.BeneficiodoPrograma.Codigos.VMC:
                            Domain.Integracao.MSG0152 msgVMC = new Domain.Integracao.MSG0152(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            return msgVMC.Enviar(solBeneficio);

                        case (int)Domain.Enum.BeneficiodoPrograma.Codigos.PriceProtection:
                            if (solBeneficio.TipoPriceProtection == (int)Domain.Enum.BeneficiodoPrograma.TipoPriceProtection.Autorizacao)
                            {
                                Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                return msgPriceProtection.Enviar(solBeneficio);
                            }
                            break;

                        case (int)Domain.Enum.BeneficiodoPrograma.Codigos.StockRotation:
                            Domain.Integracao.MSG0156 msgStockRotation = new Domain.Integracao.MSG0156(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            return msgStockRotation.Enviar(solBeneficio);


                        case (int)Domain.Enum.BeneficiodoPrograma.Codigos.Rebate:
                            Domain.Integracao.MSG0154 msgRebate = new Domain.Integracao.MSG0154(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            return msgRebate.Enviar(solBeneficio);

                        case (int)Domain.Enum.BeneficiodoPrograma.Codigos.RebatePosVenda:
                            Domain.Integracao.MSG0173 msgRebatePosVenda = new Domain.Integracao.MSG0173(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            return msgRebatePosVenda.Enviar(solBeneficio);
                    }
                }
            }

            return null;
        }

        public void Deletar(Guid solicitacaoId)
        {
            RepositoryService.SolicitacaoBeneficio.Delete(solicitacaoId);
        }

        public DateTime ObterDataValidadeCriacao(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            DateTime dataValidade = DateTime.MinValue;
            var beneficioPrograma = RepositoryService.Beneficio.Retrieve(solicitacaoBeneficio.BeneficioPrograma.Id);

            if (beneficioPrograma.PossuiControleContaCorrente.Value == (int)Enum.BeneficiodoPrograma.ControleContaCorrente.Nao
                && solicitacaoBeneficio.AjusteSaldo == true)
                throw new ArgumentException("(CRM) Não é possível incluir solicitação de ajuste para benefício que não possui controle de conta corrente.");

            if (solicitacaoBeneficio.BeneficioPrograma == null)
            {
                throw new ArgumentException("(CRM) Benefício do Programa é obrigatório!");
            }

            if (solicitacaoBeneficio.UnidadedeNegocio == null)
            {
                throw new ArgumentException("(CRM) Unidade de Negócio é obrigatório!");
            }

            int diasValidade = 0;
            if (!solicitacaoBeneficio.DataValidade.HasValue)
            {
                List<ParametroGlobal> lstParam = RepositoryService.ParametroGlobal.ListarPor((int)Enum.TipoParametroGlobal.ValidadeDeSolicitacaoDeBeneficio, solicitacaoBeneficio.UnidadedeNegocio.Id, null, null, null, null, solicitacaoBeneficio.BeneficioPrograma.Id);

                if (lstParam.Count <= 0)
                {
                    UnidadeNegocio un = RepositoryService.UnidadeNegocio.ObterPor(solicitacaoBeneficio.UnidadedeNegocio.Id);
                    throw new ArgumentException("(CRM) Parâmetro Global Validade de Solicitação de Benefício não encontrado para o benefício [" + beneficioPrograma.Nome + "] e unidade de negócio [" + un.Nome + "].");
                }



                if (!int.TryParse(lstParam.First().Valor, out diasValidade))
                {
                    UnidadeNegocio un = RepositoryService.UnidadeNegocio.ObterPor(solicitacaoBeneficio.UnidadedeNegocio.Id);
                    throw new ArgumentException("(CRM) Parâmetro Global Validade de Solicitação de Benefício não encontrado para o benefício [" + beneficioPrograma.Nome + "] e unidade de negócio [" + un.Nome + "].");
                }

                dataValidade = new SDKore.Helper.DateTimeHelper().PrimeiroDiaDoTrimestre();


            }
            return dataValidade.AddDays(diasValidade);
        }

        public CondicaoPagamento ObterCondicaoDePagamento(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (solicitacaoBeneficio.FormaPagamento == null)
            {
                throw new ArgumentException("(CRM) Forma de Pagamento é obrigatório!");
            }

            FormaPagamento formaPagto = RepositoryService.FormaPagamento.Retrieve(solicitacaoBeneficio.FormaPagamento.Id);

            if (formaPagto != null && formaPagto.Nome != Enum.SolicitacaoBeneficio.FormaPagamento.Produto)
            {
                return null;
            }

            if (solicitacaoBeneficio.BeneficioPrograma == null)
            {
                throw new ArgumentException("(CRM) Benefício do Programa é obrigatório!");
            }

            if (solicitacaoBeneficio.UnidadedeNegocio == null)
            {
                throw new ArgumentException("(CRM) Unidade de Negócio é obrigatório!");
            }

            List<ParametroGlobal> lstParam = RepositoryService.ParametroGlobal.ListarPor((int)Enum.TipoParametroGlobal.CondicaoPagamento, solicitacaoBeneficio.UnidadedeNegocio.Id, null, null, null, null, solicitacaoBeneficio.BeneficioPrograma.Id);

            if (lstParam.Count <= 0)
            {
                throw new ArgumentException("(CRM) Parâmetro Global [" + (int)Enum.TipoParametroGlobal.CondicaoPagamento + "] de Condição de Pagamento não localizado.");
            }

            CondicaoPagamento condPagto = RepositoryService.CondicaoPagamento.ObterPor(int.Parse(lstParam.First().Valor));

            if (condPagto == null)
            {
                throw new ArgumentException("(CRM) Condição de pagamento [" + lstParam.First().Valor + "] não localizada.");
            }

            return condPagto;
            // solicBeneficio
        }

        public PortfoliodoKeyAccountRepresentantes ObterPortfolioRepresentante(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (solicitacaoBeneficio.UnidadedeNegocio != null)
            {
                if (solicitacaoBeneficio.KaRepresentante == null
                    || solicitacaoBeneficio.Assistente == null
                    || solicitacaoBeneficio.Supervisor == null)
                {
                    string codigodorepresentante = SDKore.Configuration.ConfigurationManager.GetSettingValue("Intelbras.Solicitacao.RepresentantePadrao", true);

                    PortfoliodoKeyAccountRepresentantes portRep = RepositoryService.PortfoliodoKeyAccountRepresentantes
                        .ListarPorCodigoRepresentante(solicitacaoBeneficio.UnidadedeNegocio.Id, codigodorepresentante)
                        .FirstOrDefault();

                    if (portRep == null)
                    {
                        UnidadeNegocio un = RepositoryService.UnidadeNegocio.ObterPor(solicitacaoBeneficio.UnidadedeNegocio.Id);
                        throw new ArgumentException("(CRM) Potifólio do representante 2000 não localizado para a unidade de negócio " + un.Nome);
                    }

                    if (portRep.AssistentedeAdministracaodeVendas == null)
                    {
                        throw new ArgumentException("(CRM) Assistente de Adm. Vendas não encontrado no Portfólio do Representante padrão para a solicitação");
                    }

                    if (portRep.SupervisordeVendas == null)
                    {
                        throw new ArgumentException("(CRM) Supervisor não encontrado no Portfólio do Representante padrão para a solicitação");
                    }

                    if (portRep.KeyAccountRepresentante == null)
                    {
                        throw new ArgumentException("(CRM) Representante não encontrado no Portfólio do Representante padrão para a solicitação");
                    }


                    return portRep;
                }
            }

            return null;
        }

        public string ObterNome(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            decimal valorSolicitado = (solicitacaoBeneficio.ValorSolicitado.HasValue) ? solicitacaoBeneficio.ValorSolicitado.Value : 0m;
            string unidadeNegocioNome = (string.IsNullOrEmpty(solicitacaoBeneficio.UnidadedeNegocio.Name))
                                        ? RepositoryService.UnidadeNegocio.Retrieve(solicitacaoBeneficio.UnidadedeNegocio.Id, "name").Nome
                                        : solicitacaoBeneficio.UnidadedeNegocio.Name;

            string nome = unidadeNegocioNome;
            nome += " / ";
            nome += decimal.Round(valorSolicitado, 2).ToString("N");
            nome += " / ";
            nome += RepositoryService.Conta.Retrieve(solicitacaoBeneficio.Canal.Id, "accountnumber").CodigoMatriz;

            return nome;
        }

        public decimal? ObterValorAbater(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            if (solicitacaoBeneficio.ValorSolicitado.HasValue)
            {
                if (solicitacaoBeneficio.UnidadedeNegocio != null && solicitacaoBeneficio.BeneficioPrograma != null)
                {
                    if (solicitacaoBeneficio.FormaPagamento != null)
                    {
                        int tipoParamGlobal = 0;
                        string tipoParamGlobalNome = "";

                        FormaPagamento formaPagto = RepositoryService.FormaPagamento.Retrieve(solicitacaoBeneficio.FormaPagamento.Id);


                        switch (formaPagto.Nome)
                        {
                            case Enum.SolicitacaoBeneficio.FormaPagamento.Produto:
                                tipoParamGlobal = (int)Enum.TipoParametroGlobal.FatorConversaoValorSolicitadoProduto;
                                tipoParamGlobalNome = Enum.SolicitacaoBeneficio.FormaPagamento.Produto;
                                break;

                            case Enum.SolicitacaoBeneficio.FormaPagamento.DescontoDuplicata:
                                tipoParamGlobal = (int)Enum.TipoParametroGlobal.FatorConversaoValorSolicitadoBeneficioDescontoDuplicata;
                                tipoParamGlobalNome = Enum.SolicitacaoBeneficio.FormaPagamento.DescontoDuplicata;
                                break;

                            case Enum.SolicitacaoBeneficio.FormaPagamento.Dinheiro:
                                tipoParamGlobal = (int)Enum.TipoParametroGlobal.FatorConversaoValorSolicitadoDinheiro;
                                tipoParamGlobalNome = Enum.SolicitacaoBeneficio.FormaPagamento.Dinheiro;
                                break;
                        }

                        Beneficio benProg = RepositoryService.Beneficio.ObterPor(solicitacaoBeneficio.BeneficioPrograma.Id);

                        decimal valorAAbater = this.DefineValorAAbater(tipoParamGlobal, tipoParamGlobalNome, solicitacaoBeneficio.UnidadedeNegocio.Id, solicitacaoBeneficio.UnidadedeNegocio.Name, solicitacaoBeneficio.BeneficioPrograma.Id, benProg.Nome);

                        return solicitacaoBeneficio.ValorSolicitado.Value / valorAAbater;
                    }
                }
            }

            return null;
        }

        private decimal DefineValorAAbater(int tipoParamGlobal, string tipoParamGlobalNome, Guid unidNeg, string nomUn, Guid? benefProgramId, string beneficioProgramaName)
        {
            List<ParametroGlobal> lstParam = RepositoryService.ParametroGlobal.ListarPor(tipoParamGlobal, unidNeg, null, null, null, null, benefProgramId);

            if (lstParam.Count <= 0)
            {
                UnidadeNegocio un = RepositoryService.UnidadeNegocio.ObterPor(unidNeg);
                throw new ArgumentException("(CRM) Parâmetro global [" + tipoParamGlobal + " - " + tipoParamGlobalNome + "] não encontrado para o benefício [" + beneficioProgramaName + "] e unidade de negócio [" + un.Nome + "].");
            }

            return decimal.Parse(lstParam.First().Valor.ToString());
        }

        public string ObterTrimestreCompetencia()
        {
            DateTime ultimoTrimestre = new SDKore.Helper.DateTimeHelper().UltimoDiaDoUltimoTrimestre();
            int quarterNumber = (ultimoTrimestre.Month - 1) / 3 + 1;

            return ultimoTrimestre.Year + "-T" + quarterNumber;
        }

        public void SolicitarBaneficioPostCreate(SolicitacaoBeneficio mSolBeneficio)
        {
            SolicitacaoBeneficio solicitacaoBeneficio = RepositoryService.SolicitacaoBeneficio.Retrieve(mSolBeneficio.ID.Value);

            if (!solicitacaoBeneficio.AjusteSaldo.Value)
            {
                Beneficio beneficio = RepositoryService.Beneficio.Retrieve(solicitacaoBeneficio.BeneficioPrograma.Id);

                //TODO: VALIAR SE vmc E STOCKrOTATION ENTRA NESSA REGRA TAMBEM POIS ATE O MOMENTO SERVIRA PARA REBATE E REBATE POS VENDA
                var formaPagto = RepositoryService.FormaPagamento.Retrieve(mSolBeneficio.FormaPagamento.Id);

                if (formaPagto.Nome != Enum.SolicitacaoBeneficio.FormaPagamento.Produto
               && mSolBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada)
                {
                    switch (beneficio.Codigo)
                    {
                        case (int)Enum.BeneficiodoPrograma.Codigos.Rebate:
                            Domain.Integracao.MSG0154 msgRebate = new Domain.Integracao.MSG0154(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgRebate.Enviar(mSolBeneficio);
                            break;
                        case (int)Enum.BeneficiodoPrograma.Codigos.RebatePosVenda:
                            Domain.Integracao.MSG0173 msgRebatePosVenda = new Domain.Integracao.MSG0173(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgRebatePosVenda.Enviar(mSolBeneficio);
                            break;
                        case (int)Enum.BeneficiodoPrograma.Codigos.VMC:
                            Domain.Integracao.MSG0152 msgVMC = new Domain.Integracao.MSG0152(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgVMC.Enviar(mSolBeneficio);
                            break;
                        case (int)Enum.BeneficiodoPrograma.Codigos.StockRotation:
                            Domain.Integracao.MSG0156 msgStockRotation = new Domain.Integracao.MSG0156(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgStockRotation.Enviar(mSolBeneficio);
                            break;
                        case (int)Enum.BeneficiodoPrograma.Codigos.PriceProtection:
                            if (solicitacaoBeneficio.TipoPriceProtection.HasValue && solicitacaoBeneficio.TipoPriceProtection.Value == (int)Enum.SolicitacaoBeneficio.TipoPriceProtection.Consumo)
                            {
                                Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                msgPriceProtection.Enviar(solicitacaoBeneficio);
                            }

                            break;
                    }
                }
            }
        }

        private bool EnviarMensagemIntegracaoAjuste(SolicitacaoBeneficio solicitacaoBeneficio, Beneficio beneficioPrograma, SolicitacaoBeneficio preSolicitacaoBeneficio)
        {
            if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente)
            {
                switch (beneficioPrograma.Codigo.Value)
                {
                    case (int)Enum.BeneficiodoPrograma.Codigos.Rebate:
                        Domain.Integracao.MSG0154 msgRebate = new Domain.Integracao.MSG0154(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgRebate.Enviar(solicitacaoBeneficio);
                        break;

                    case (int)Enum.BeneficiodoPrograma.Codigos.RebatePosVenda:
                        Domain.Integracao.MSG0173 msgRebatePosVenda = new Domain.Integracao.MSG0173(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgRebatePosVenda.Enviar(solicitacaoBeneficio);
                        break;

                    case (int)Enum.BeneficiodoPrograma.Codigos.VMC:
                        Domain.Integracao.MSG0152 msgVMC = new Domain.Integracao.MSG0152(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgVMC.Enviar(solicitacaoBeneficio);
                        break;

                    case (int)Enum.BeneficiodoPrograma.Codigos.PriceProtection:
                        if (solicitacaoBeneficio.TipoPriceProtection.HasValue && solicitacaoBeneficio.TipoPriceProtection.Value == (int)Enum.SolicitacaoBeneficio.TipoPriceProtection.Autorizacao)
                        {
                            if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente)
                            {
                                Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                msgPriceProtection.Enviar(solicitacaoBeneficio);
                                return true;
                            }
                            if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada)
                            {
                                if (preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                                    && preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise)
                                {
                                    Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                    msgPriceProtection.Enviar(solicitacaoBeneficio);
                                    return true;
                                }
                            }
                        }
                        else
                        {
                            Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgPriceProtection.Enviar(solicitacaoBeneficio);
                            return true;
                        }

                        break;

                    case (int)Enum.BeneficiodoPrograma.Codigos.StockRotation:
                        Domain.Integracao.MSG0156 msgStockRotation = new Domain.Integracao.MSG0156(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgStockRotation.Enviar(solicitacaoBeneficio);
                        break;

                    case (int)Enum.BeneficiodoPrograma.Codigos.Backup:
                        Domain.Integracao.MSG0158 msgBackup = new Domain.Integracao.MSG0158(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgBackup.Enviar(solicitacaoBeneficio);
                        break;

                    case (int)Enum.BeneficiodoPrograma.Codigos.Showroom:
                        Domain.Integracao.MSG0157 msgShowroom = new Domain.Integracao.MSG0157(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgShowroom.Enviar(solicitacaoBeneficio);
                        break;
                }

                return true;
            }

            return false;
        }

        public bool EnviarMensagemIntegracao(SolicitacaoBeneficio solicitacaoBeneficio, Beneficio beneficioPrograma, SolicitacaoBeneficio preSolicitacaoBeneficio)
        {
            if (!solicitacaoBeneficio.IntegrarNoPlugin.HasValue || solicitacaoBeneficio.IntegrarNoPlugin.Value)
            {
                return false;
            }

            if (solicitacaoBeneficio.FormaPagamento.Name == Enum.SolicitacaoBeneficio.FormaPagamento.Produto
               && solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada)
            {
                return false;
            }

            switch (beneficioPrograma.Codigo.Value)
            {
                #region Rebate

                case (int)Enum.BeneficiodoPrograma.Codigos.Rebate:
                    if (solicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                    {
                        Domain.Integracao.MSG0154 msgRebate = new Domain.Integracao.MSG0154(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgRebate.Enviar(solicitacaoBeneficio);
                        return true;
                    }
                    break;

                #endregion

                #region RebatePosVenda

                case (int)Enum.BeneficiodoPrograma.Codigos.RebatePosVenda:
                    if (solicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                    {
                        Domain.Integracao.MSG0173 msgRebatePosVenda = new Domain.Integracao.MSG0173(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgRebatePosVenda.Enviar(solicitacaoBeneficio);
                        return true;
                    }
                    break;

                #endregion

                #region PriceProtection

                case (int)Enum.BeneficiodoPrograma.Codigos.PriceProtection:
                    if (solicitacaoBeneficio.TipoPriceProtection.HasValue && solicitacaoBeneficio.TipoPriceProtection.Value == (int)Enum.SolicitacaoBeneficio.TipoPriceProtection.Autorizacao)
                    {
                        if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente)
                        {
                            Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgPriceProtection.Enviar(solicitacaoBeneficio);
                            return true;
                        }
                        if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada)
                        {
                            if (preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                                && preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise)
                            {
                                Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                                msgPriceProtection.Enviar(solicitacaoBeneficio);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Domain.Integracao.MSG0155 msgPriceProtection = new Domain.Integracao.MSG0155(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgPriceProtection.Enviar(solicitacaoBeneficio);
                        return true;
                    }
                    break;

                #endregion

                #region StockRotation

                case (int)Enum.BeneficiodoPrograma.Codigos.StockRotation:
                    if (solicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                    {
                        Domain.Integracao.MSG0156 msgStockRotation = new Domain.Integracao.MSG0156(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgStockRotation.Enviar(solicitacaoBeneficio);
                        return true;
                    }
                    break;

                #endregion

                #region Backup

                case (int)Enum.BeneficiodoPrograma.Codigos.Backup:
                    if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente)
                    {
                        Domain.Integracao.MSG0158 msgBackup = new Domain.Integracao.MSG0158(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgBackup.Enviar(solicitacaoBeneficio);
                        return true;
                    }

                    if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada)
                    {
                        if (preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                            && preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise
                            && preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada)
                        {
                            Domain.Integracao.MSG0158 msgBackup = new Domain.Integracao.MSG0158(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgBackup.Enviar(solicitacaoBeneficio);
                            return true;
                        }
                    }
                    break;

                #endregion

                #region Showroom

                case (int)Enum.BeneficiodoPrograma.Codigos.Showroom:
                    if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente)
                    {
                        Domain.Integracao.MSG0157 msgShowroom = new Domain.Integracao.MSG0157(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgShowroom.Enviar(solicitacaoBeneficio);
                        return true;
                    }

                    if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada)
                    {
                        if (preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                            && preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise
                            && preSolicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada)
                        {
                            Domain.Integracao.MSG0157 msgShowroom = new Domain.Integracao.MSG0157(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                            msgShowroom.Enviar(solicitacaoBeneficio);
                            return true;
                        }
                    }
                    break;

                #endregion

                #region VMC

                case (int)Enum.BeneficiodoPrograma.Codigos.VMC:
                    if (solicitacaoBeneficio.StatusSolicitacao.Value != (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                    {
                        Domain.Integracao.MSG0152 msgmsgVMC = new Domain.Integracao.MSG0152(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        msgmsgVMC.Enviar(solicitacaoBeneficio);
                        return true;
                    }
                    break;

                    #endregion
            }

            return false;
        }

        public void SolicitarBeneficioPostUpdate(SolicitacaoBeneficio solicitacaoBeneficio, SolicitacaoBeneficio preSolicitacaoBeneficio, Beneficio beneficioPrograma = null)
        {
            if (beneficioPrograma == null)
            {
                beneficioPrograma = RepositoryService.Beneficio.Retrieve(solicitacaoBeneficio.BeneficioPrograma.Id);
            }

            if (!beneficioPrograma.Codigo.HasValue) { return; }

            AcoesPosAteracaoStatus(solicitacaoBeneficio, beneficioPrograma, preSolicitacaoBeneficio);
        }

        private void ConcluirTarefaSolicitacaoBeneficio(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            List<Tarefa> tarefas = RepositoryService.Tarefa.ListarPorReferenteAAtivo(solicitacaoBeneficio.ID.Value);

            foreach (var tarefa in tarefas)
            {
                ServiceTarefaService.MudarStatus(tarefa.ID.Value, (int)Enum.Tarefa.StateCode.Fechada, (int)Enum.Tarefa.StatusCode.Concluida);
            }
        }

        private void AlteraSolicitacaoParaPagametoReembolsado(SolicitacaoBeneficio mSolBeneficio)
        {
            var updateSolicitacaoStockRotation = new SolicitacaoBeneficio(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
            {
                ID = mSolBeneficio.ID,
                StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado,
                Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.Reembolsado,
                IntegrarNoPlugin = true
            };

            RepositoryService.SolicitacaoBeneficio.Update(updateSolicitacaoStockRotation);
        }

        private void AcoesPosAteracaoStatus(SolicitacaoBeneficio solicitacaoBeneficio, Beneficio beneficioPrograma, SolicitacaoBeneficio preSolicitacaoBeneficio)
        {
            if (!solicitacaoBeneficio.AjusteSaldo.Value)
            {
                EnviarMensagemIntegracao(solicitacaoBeneficio, beneficioPrograma, preSolicitacaoBeneficio);

                switch (beneficioPrograma.Codigo.Value)
                {
                    case (int)Enum.BeneficiodoPrograma.Codigos.StockRotation:
                        if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente)
                        {
                            AlteraSolicitacaoParaPagametoReembolsado(solicitacaoBeneficio);
                        }
                        break;

                    case (int)Enum.BeneficiodoPrograma.Codigos.Showroom:
                        if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
                        {
                            new ProcessoDeSolicitacoesService(RepositoryService).CriaTarefaShowRoom(solicitacaoBeneficio);
                        }
                        break;
                }
            }
            else
            {
                EnviarMensagemIntegracaoAjuste(solicitacaoBeneficio, beneficioPrograma, preSolicitacaoBeneficio);
            }

            switch (solicitacaoBeneficio.StatusSolicitacao.Value)
            {
                case (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada:
                    new TarefaService(RepositoryService).CancelarTarefasPorReferenteA(solicitacaoBeneficio.ID.Value);
                    break;

                case (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado:
                    ConcluirTarefaSolicitacaoBeneficio(solicitacaoBeneficio);
                    break;
            }
        }

        public Guid? CriarSolicitacaoComProdutosCancelados(SolicitacaoBeneficio objSolicitacao)
        {
            if (objSolicitacao.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoEfetuado)
            {
                if (objSolicitacao.FormaPagamento.Name == Enum.SolicitacaoBeneficio.FormaPagamento.Produto
                    && objSolicitacao.StatusPagamento == (int)Enum.SolicitacaoBeneficio.StatusPagamento.PagoParcial)
                {
                    if (objSolicitacao != null && objSolicitacao.ID.HasValue)
                    {
                        List<ProdutosdaSolicitacao> lstProdSolic = new ProdutosdaSolicitacaoService(RepositoryService)
                                                                    .ListarPorSolicitacaoAtivos(objSolicitacao.ID.Value);

                        var produtosCancelados = lstProdSolic.FindAll(x => x.QuantidadeCancelada.HasValue && x.QuantidadeCancelada.Value > 0);

                        if (produtosCancelados.Count > 0)
                        {
                            decimal valorAprovado = 0;
                            decimal valorSolicitado = 0;

                            foreach (var produto in lstProdSolic)
                            {
                                produto.QuantidadeSolicitada = produto.QuantidadeCancelada;
                                produto.QuantidadeAprovada = produto.QuantidadeCancelada;
                                produto.QuantidadeCancelada = 0;
                                produto.ValorCancelado = 0;
                                produto.ValorTotal = (produto.ValorUnitario.HasValue) ? produto.ValorUnitario.Value * produto.QuantidadeSolicitada.Value : 0;
                                produto.ValorTotalAprovado = (produto.ValorUnitarioAprovado.HasValue) ? produto.ValorUnitarioAprovado.Value * produto.QuantidadeSolicitada.Value : 0;

                                valorAprovado += produto.ValorTotalAprovado.Value;
                                valorSolicitado += produto.ValorTotal.Value;
                            }

                            objSolicitacao.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente;
                            objSolicitacao.Status = (int)Enum.SolicitacaoBeneficio.RazaoStatusAtivo.ReembolsoPendente;
                            objSolicitacao.StatusPagamento = (int)Enum.SolicitacaoBeneficio.StatusPagamento.NaoPago;
                            objSolicitacao.ValorSolicitado = valorSolicitado;
                            objSolicitacao.ValorAprovado = valorAprovado;
                            objSolicitacao.ValorPago = null;
                            objSolicitacao.ValorAbater = null;
                            objSolicitacao.ValorCancelado = null;
                            objSolicitacao.ID = RepositoryService.SolicitacaoBeneficio.Create(objSolicitacao);

                            foreach (var produto in lstProdSolic)
                            {
                                produto.SolicitacaoBeneficio = new Lookup(objSolicitacao.ID.Value, SDKore.Crm.Util.Utility.GetEntityName(objSolicitacao));
                                RepositoryService.ProdutosdaSolicitacao.Create(produto);
                            }

                            return objSolicitacao.ID;
                        }

                    }
                }
            }

            return null;
        }

        public SolicitacaoBeneficio RecalculaValoresNaAlteracaoDeStatus(SolicitacaoBeneficio solicitacaoBeneficio, SolicitacaoBeneficio preSolicitacaoBeneficio)
        {
            var beneficioPrograma = RepositoryService.Beneficio.Retrieve(solicitacaoBeneficio.BeneficioPrograma.Id);
            var formaPagamento = RepositoryService.FormaPagamento.Retrieve(solicitacaoBeneficio.FormaPagamento.Id, "itbc_name");


            if (solicitacaoBeneficio.StatusSolicitacao.Value == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada
                || solicitacaoBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise
                || solicitacaoBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.ComprovantesValidacao
                || solicitacaoBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.ComprovacaoConcluida
                || solicitacaoBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada)
            {
                if ((formaPagamento.Nome == Enum.SolicitacaoBeneficio.FormaPagamento.Produto && solicitacaoBeneficio.TipoPriceProtection.HasValue && solicitacaoBeneficio.TipoPriceProtection.Value == (int)Enum.SolicitacaoBeneficio.TipoPriceProtection.Autorizacao)
                    || (formaPagamento.Nome == Enum.SolicitacaoBeneficio.FormaPagamento.Produto && !solicitacaoBeneficio.AjusteSaldo.Value))
                {
                    solicitacaoBeneficio.ValorSolicitado = ObtemTotalAprovadosProdutos(solicitacaoBeneficio);
                }

                if (ValorAprovadoDeveSerIgualValorSolicitado(solicitacaoBeneficio, beneficioPrograma))
                {
                    solicitacaoBeneficio.ValorAprovado = solicitacaoBeneficio.ValorSolicitado;
                }

                if (solicitacaoBeneficio.AjusteSaldo.Value)
                {
                    solicitacaoBeneficio.ValorAbater = solicitacaoBeneficio.ValorAprovado;
                }
                else
                {
                    var unidadeNegocio = new UnidadeNegocio(RepositoryService) { ID = solicitacaoBeneficio.UnidadedeNegocio.Id, Nome = solicitacaoBeneficio.UnidadedeNegocio.Name };

                    solicitacaoBeneficio.ValorAbater = ObterValorAbater(solicitacaoBeneficio.ValorAprovado.Value, formaPagamento, unidadeNegocio, beneficioPrograma);
                }
            }

            if (solicitacaoBeneficio.AjusteSaldo.Value)
            {
                if ((solicitacaoBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.PagamentoPendente
                     || solicitacaoBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Aprovada)
                    && preSolicitacaoBeneficio.StatusSolicitacao.Value != solicitacaoBeneficio.StatusSolicitacao.Value)
                {
                    solicitacaoBeneficio.ValorPago = solicitacaoBeneficio.ValorSolicitado;
                    solicitacaoBeneficio.ValorCancelado = 0;
                }
            }

            return solicitacaoBeneficio;
        }

        public SolicitacaoBeneficio RecalcularValoresNaAnaliseParaFormaPagtoIgualAProduto(SolicitacaoBeneficio objSolicitacao)
        {
            if (objSolicitacao == null && !objSolicitacao.ID.HasValue)
                throw new ArgumentException("(CRM) Solicitação Benefício Nula");

            List<ProdutosdaSolicitacao> lstProdSolic = new Domain.Servicos.ProdutosdaSolicitacaoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider).ListarPorSolicitacaoAtivos(objSolicitacao.ID.Value);
            if (lstProdSolic.Count <= 0)
                throw new ArgumentException("(CRM) Não é possível analisar a solicitação pois não foi informado nenhum produto para esta solicitação.");

            decimal valorSolicitado = 0;

            #region CALCULA FILHOS (PRODUTOS SOLICITAÇÕES)
            if (lstProdSolic.Count > 0)
            {

                foreach (ProdutosdaSolicitacao item in lstProdSolic)
                {
                    if (item.ValorTotalAprovado.HasValue)
                        valorSolicitado += item.ValorTotal.Value;
                }
            }
            #endregion

            objSolicitacao.ValorSolicitado = valorSolicitado;


            Beneficio beneficio = RepositoryService.Beneficio.Retrieve(objSolicitacao.BeneficioPrograma.Id);

            int tipoParamGlobal = (int)Enum.TipoParametroGlobal.FatorConversaoValorSolicitadoProduto;
            var lstParam = RepositoryService.ParametroGlobal.ListarPor(tipoParamGlobal, objSolicitacao.UnidadedeNegocio.Id, null, null, null, null, beneficio.ID);

            if (lstParam.Count <= 0)
            {
                throw new ArgumentException("(CRM)Parâmetro Global [Fator Conversão Valor Solicitado Benefício - Produto] não localizado para Unidade de Negócio [" + objSolicitacao.UnidadedeNegocio.Name + "] e Benefício [" + beneficio.Nome + "].");
            }
            else
            {
                objSolicitacao.ValorAbater = objSolicitacao.ValorSolicitado / decimal.Parse(lstParam.First().Valor);
            }

            return objSolicitacao;
        }

        public bool CriadaAposDataLimite(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            var lstParam = RepositoryService.ParametroGlobal.ListarPor((int)Enum.TipoParametroGlobal.PrazoLimiteParaSolicitarBeneficio, solicitacaoBeneficio.UnidadedeNegocio.Id, null, null, null, null, solicitacaoBeneficio.BeneficioPrograma.Id);

            if (lstParam.Count == 0)
            {
                throw new ArgumentException("(CRM) Parâmetro Global [" + (int)Enum.TipoParametroGlobal.PrazoLimiteParaSolicitarBeneficio + "] não localizado.");
            }

            int dias = lstParam[0].GetValue<int>();
            DateTime primeiroDiaDoTrimestre = new SDKore.Helper.DateTimeHelper().PrimeiroDiaDoTrimestre();
            DateTime dataLimite = primeiroDiaDoTrimestre.AddDays(dias);

            return (dataLimite < DateTime.Today); //Trocar >= por <
        }

        private decimal ObtemTotalAprovadosProdutos(SolicitacaoBeneficio solicitacaoBeneficio)
        {
            List<ProdutosdaSolicitacao> lista = new ProdutosdaSolicitacaoService(RepositoryService).ListarPorSolicitacaoAtivos(solicitacaoBeneficio.ID.Value);

            decimal totalAprovado = 0;

            lista.FindAll(x => x.ValorTotalAprovado.HasValue).ForEach(y => totalAprovado += y.ValorTotalAprovado.Value);

            return totalAprovado;
        }

        private decimal ObterValorAbater(decimal valor, FormaPagamento formaPagamento, UnidadeNegocio unidadeNegocio, Beneficio beneficio)
        {
            decimal fator = ObterFatorConversao(formaPagamento, unidadeNegocio, beneficio);

            return valor / fator;
        }

        private decimal ObterFatorConversao(FormaPagamento formaPagamento, UnidadeNegocio unidadeNegocio, Beneficio beneficio)
        {
            int tipoParametroGlobal = 0;

            switch (formaPagamento.Nome)
            {
                case Enum.SolicitacaoBeneficio.FormaPagamento.Produto:
                    tipoParametroGlobal = (int)Enum.TipoParametroGlobal.FatorConversaoValorSolicitadoProduto;
                    break;

                case Enum.SolicitacaoBeneficio.FormaPagamento.DescontoDuplicata:
                    tipoParametroGlobal = (int)Enum.TipoParametroGlobal.FatorConversaoValorSolicitadoBeneficioDescontoDuplicata;
                    break;

                case Enum.SolicitacaoBeneficio.FormaPagamento.Dinheiro:
                    tipoParametroGlobal = (int)Enum.TipoParametroGlobal.FatorConversaoValorSolicitadoDinheiro;
                    break;

                default:
                    throw new ArgumentException("(CRM) A Forma de Pagamento [" + formaPagamento.Nome + "] não está configurada para obter o fator de conversão!");
            }

            var lstParam = RepositoryService.ParametroGlobal.ListarPor(tipoParametroGlobal, unidadeNegocio.ID, null, null, null, null, beneficio.ID);

            if (lstParam.Count <= 0)
            {
                throw new ArgumentException("(CRM) Parâmetro Global [" + tipoParametroGlobal + "] não localizado para Unidade de Negócio [" + unidadeNegocio.Nome + "], Forma de Pagamento [" + formaPagamento.Nome + "] e Benefício [" + beneficio.Nome + "].");
            }

            return Convert.ToDecimal(lstParam.First().Valor);
        }

        private bool ValorAprovadoDeveSerIgualValorSolicitado(SolicitacaoBeneficio solicitacaoBeneficio, Beneficio beneficioPrograma)
        {
            if (!solicitacaoBeneficio.AjusteSaldo.Value)
            {
                if (beneficioPrograma.Codigo.Value == (int)Enum.BeneficiodoPrograma.Codigos.VMC)
                {
                    if (solicitacaoBeneficio.FormaPagamento.Name != Enum.SolicitacaoBeneficio.FormaPagamento.Produto)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void ServicoDiarioSolicitacaoDeBeneficio()
        {
            List<SolicitacaoBeneficio> solicitacoesBen = RepositoryService.SolicitacaoBeneficio.ListarDiferenteDeCanceladaEPagAnteriorAAtual();

            foreach (var solicitacao in solicitacoesBen)
            {
                solicitacao.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Cancelada;
                solicitacao.DescartarVerba = true;
                solicitacao.FormaCancelamento = (int)Enum.SolicitacaoBeneficio.FormaCancelamento.Automatico;
                solicitacao.Descricao = "***Cancelamento de Solicitação de Benefício**** Solicitação de benefício cancelada em [" + DateTime.Now.ToString() + "] devido ao prazo de validade da mesma ter sido ultrapassado. ***Cancelamento de Solicitação de Benefício****";
                Atualizar(solicitacao);
            }
        }

        public void Avanca(SolicitacaoBeneficio objSolicitacaoBeneficio)
        {
            if (objSolicitacaoBeneficio.StatusSolicitacao == (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.Criada)
            {
                if (objSolicitacaoBeneficio.FormaPagamento != null && objSolicitacaoBeneficio.FormaPagamento.Id != Guid.Empty)
                {
                    var formaPagto = RepositoryService.FormaPagamento.Retrieve(objSolicitacaoBeneficio.FormaPagamento.Id);

                    if (formaPagto.Nome == Enum.SolicitacaoBeneficio.FormaPagamento.Produto)
                    {
                        var oobj = new SolicitacaoBeneficio(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        oobj.ID = objSolicitacaoBeneficio.ID;

                        objSolicitacaoBeneficio.StatusSolicitacao = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise;
                        objSolicitacaoBeneficio.Status = (int)Enum.SolicitacaoBeneficio.StatusSolicitacaoBeneficio.EmAnalise;
                        objSolicitacaoBeneficio.IntegrarNoPlugin = false;
                        this.Atualizar(objSolicitacaoBeneficio);
                    }
                }
            }
        }

        public void EndTaskOfWindows()
        {
            List<SolicitacaoBeneficio> lstSolicitacaoBeneficio = new List<SolicitacaoBeneficio>();
            #region Metas
            lstSolicitacaoBeneficio.AddRange(RepositoryService.SolicitacaoBeneficio.ObterPorStatusPrice((int)Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calcular).ToArray());
            lstSolicitacaoBeneficio.AddRange(RepositoryService.SolicitacaoBeneficio.ObterPorStatusPrice((int)Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calculando).ToArray());

            foreach (SolicitacaoBeneficio item in lstSolicitacaoBeneficio)
            {
                item.MensagemErro = "Serviço de Automação de Price Protection foi interrompido.";

                if (item.StatusCalculoPriceProtection == (int)Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calcular)
                    item.StatusCalculoPriceProtection = (int)Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.ErroCalcular;
                else if (item.StatusCalculoPriceProtection == (int)Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.Calculando)
                    item.StatusCalculoPriceProtection = (int)Enum.SolicitacaoBeneficio.StatusCalculoPriceProtection.ErroCalcular;

                RepositoryService.SolicitacaoBeneficio.Update(item);
            }
            #endregion

            lstSolicitacaoBeneficio.Clear();
        }        
    }
}
