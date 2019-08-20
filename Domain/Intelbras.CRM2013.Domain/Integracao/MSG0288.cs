using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using System;
using System.Collections.Generic;
using Pollux = Intelbras.Message.Helper;
using Intelbras.CRM2013.Domain.Exceptions;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0288 : Base, IBase<Message.Helper.MSG0288, Domain.Model.Ocorrencia>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };
        private string tipoProprietario;
        private Domain.Model.Usuario usuarioIntegracao;
        #endregion

        public MSG0288(string org, bool isOffline) : base(org, isOffline) { }
        
        #region trace
        private SDKore.Helper.Trace Trace { get; set; }
        public void DefinirObjetoTrace(SDKore.Helper.Trace trace)
        {
            this.Trace = trace;
        }
        #endregion

        #region Executar

        public string Executar(string mensagem, string numeroMensagem, Usuario usuario)
        {
            usuarioIntegracao = usuario;
            var xml = this.CarregarMensagem<Pollux.MSG0288>(mensagem);
            bool reaberta = false; // Variável de controle de acesso

            List<Intelbras.Message.Helper.Entities.AtendimentoTelefonico> lstAtendimentosTelefonicos = new List<Pollux.Entities.AtendimentoTelefonico>();

            #region Leitura do xml atualização ocorrencia e montagem retorno

            if (xml.AtendimentoTelefonico != null)
            {
                foreach (Pollux.Entities.AtendimentoTelefonico atendimentoTelefonico in xml.AtendimentoTelefonico)
                {
                    if (!string.IsNullOrEmpty(atendimentoTelefonico.ProtocoloTelefonico))
                    {
                        Ocorrencia ocorrencia = new Servicos.RepositoryService(this.Organizacao, this.IsOffline).Ocorrencia.ObterPorProtocoloTelefonico(atendimentoTelefonico.ProtocoloTelefonico);
                        if (ocorrencia != null)
                        {
                            //Reabre a ocorrência, para que salve os dados do protocola de atendimento.
                            bool ocorrenciaEstavaFechada = (ocorrencia.RazaoStatus.Value == (int)StatusDaOcorrencia.Resolvido);
                            if (ocorrenciaEstavaFechada)
                            {
                                (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.ReabrirOcorrencia(ocorrencia);
                                ocorrencia.RazaoStatus = (int)StatusDaOcorrencia.Aberta;
                                ocorrencia.ManterAberto = true;
                                reaberta = true;
                                ocorrencia.Atualizar();
                            }

                            if (atendimentoTelefonico.DuracaoChamada.HasValue)
                            {
                                ocorrencia.DuracaoChamada = atendimentoTelefonico.DuracaoChamada;
                            }

                            if (!string.IsNullOrEmpty(atendimentoTelefonico.NomeQuestionario))
                            {
                                List<QuestionarioPergunta> lstQuestionarioPergunta = new Servicos.QuestionarioPerguntaServices(this.Organizacao, this.IsOffline).ListarQuestionarioPerguntaPorNomeQuestionario(atendimentoTelefonico.NomeQuestionario);

                                if (atendimentoTelefonico.PesquisaSatisfacao != null)
                                {
                                    if (atendimentoTelefonico.PesquisaSatisfacao.Count > 0)
                                    {
                                        foreach (var pesquisaSatisfacao in atendimentoTelefonico.PesquisaSatisfacao)
                                        {
                                            foreach (var pergunta in lstQuestionarioPergunta)
                                            {
                                                if (pesquisaSatisfacao.Questao == pergunta.Ordem)
                                                {
                                                    List<QuestionarioOpcao> lstOpcoesResposta = new Servicos.QuestionarioOpcaoServices(this.Organizacao, this.IsOffline).ListarQuestionarioOpcaoPor((Guid)pergunta.ID);
                                                    foreach (QuestionarioOpcao opcaoItem in lstOpcoesResposta)
                                                    {
                                                        if (opcaoItem.Nome == pesquisaSatisfacao.ValorPesquisa.ToString())
                                                        {

                                                            //Cria o vínculo da Resposta com a ocorrência
                                                            QuestionarioResposta questionarioRespostaCreate = new QuestionarioResposta();
                                                            questionarioRespostaCreate.QuestionarioOpcao = new Lookup(opcaoItem.Id, "itbc_questionarioresposta");
                                                            questionarioRespostaCreate.QuestionarioRespostaOcorrencia = new Lookup(ocorrencia.Id, "incident");
                                                            new RepositoryService(this.Organizacao, this.IsOffline).QuestionarioResposta.Create(questionarioRespostaCreate);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (atendimentoTelefonico.TempoFila.HasValue)
                            {
                                ocorrencia.TempoFila = atendimentoTelefonico.TempoFila;
                            }

                            if (atendimentoTelefonico.TempoWrap.HasValue)
                            {
                                ocorrencia.TempoWrap = atendimentoTelefonico.TempoWrap;
                            }

                            if (!String.IsNullOrEmpty(atendimentoTelefonico.NumeroFilaEntrada))
                            {
                                ocorrencia.NumeroFilaEntrada = atendimentoTelefonico.NumeroFilaEntrada;
                            }

                            if (!String.IsNullOrEmpty(atendimentoTelefonico.CodigoProdutoURA))
                            {
                                ocorrencia.CodigoProdutoURA = atendimentoTelefonico.CodigoProdutoURA;
                            }

                            try
                            {

                                if (reaberta == true) // Entrou no if de cancelada ou resolvida
                                {
                                    ocorrencia.ManterAberto = false;
                                    reaberta = false;
                                    ocorrencia.Atualizar();
                                }
                                else
                                {
                                    ocorrencia.Atualizar();
                                }

                                //Antes de resolver a ocorrência, precisa preencher a resolução
                                SolucaoOcorrencia solucaoOcorrencia = new SolucaoOcorrencia(this.Organizacao, this.IsOffline)
                                {
                                    DataHoraConclusao = DateTime.Now,
                                    Nome = "Rotina Histórico do Protocolo de atendimento",
                                    OcorrenciaId = ocorrencia.Id
                                };

                                //Fecha a ocorrência, caso ela já estivesse fechada.
                                if (ocorrenciaEstavaFechada)
                                    (new CRM2013.Domain.Servicos.RepositoryService()).Ocorrencia.FecharOcorrencia(ocorrencia, solucaoOcorrencia);

                                if (ocorrencia != null)
                                {
                                    Pollux.Entities.AtendimentoTelefonico objPollux = new Pollux.Entities.AtendimentoTelefonico();

                                    objPollux.ProtocoloTelefonico = ocorrencia.ProtocoloTelefonico;
                                    objPollux.NumeroOcorrencia = ocorrencia.Numero;
                                    objPollux.Mensagem = "Ocorrência atualizada com sucesso!";

                                    lstAtendimentosTelefonicos.Add(objPollux);
                                }
                            } catch (Exception e) {
                                if (ocorrencia != null)
                                {
                                    Pollux.Entities.AtendimentoTelefonico objPollux = new Pollux.Entities.AtendimentoTelefonico();

                                    objPollux.ProtocoloTelefonico = ocorrencia.ProtocoloTelefonico;
                                    objPollux.NumeroOcorrencia = ocorrencia.Numero;
                                    objPollux.Mensagem = "Falha na atualização da Ocorrência: '" + e.Message + "'.";

                                    lstAtendimentosTelefonicos.Add(objPollux);
                                }
                            }
                        } else
                        {
                            Pollux.Entities.AtendimentoTelefonico objPollux = new Pollux.Entities.AtendimentoTelefonico();

                            objPollux.ProtocoloTelefonico = atendimentoTelefonico.ProtocoloTelefonico;
                            objPollux.Mensagem = "Não encontrou ocorrência com esse Protocolo.";

                            lstAtendimentosTelefonicos.Add(objPollux);
                        }
                    }
                }
            }
            #endregion
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso!";
                retorno.Add("AtendimentoTelefonicoAtualizado", lstAtendimentosTelefonicos);
                retorno.Add("Resultado", resultadoPersistencia);

            return CriarMensagemRetorno<Pollux.MSG0288R1>(numeroMensagem, retorno);       
        }

        #endregion

        #region Definir Propriedades

        public Ocorrencia DefinirPropriedades(Intelbras.Message.Helper.MSG0288 xml)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Métodos Auxiliares

        public string Enviar(Ocorrencia objModel)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}