using System;
using System.Collections.Generic;
using Intelbras.CRM2013.Domain.Model;
using Pollux = Intelbras.Message.Helper;
using System.Data;
using System.Linq;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Integracao
{
    public class MSG0293 : Base, IBase<MSG0293, QuestionarioPergunta>
    {
        #region Propriedades
        //Dictionary que sera enviado como resposta do request
        private Dictionary<string, object> retorno = new Dictionary<string, object>();
        Pollux.Entities.Resultado resultadoPersistencia = new Pollux.Entities.Resultado() { Sucesso = true };

        #endregion

        #region Construtor
        public MSG0293(string org, bool isOffline) : base(org, isOffline)
        {
            this.Organizacao = org;
            this.IsOffline = isOffline;
            SDKore.DomainModel.RepositoryFactory.SetTag(SDKore.Configuration.ConfigurationManager.GetSettingValue("OrganizacaoIntelbras"));
        }
        #endregion

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
            var xml = this.CarregarMensagem<Pollux.MSG0293>(mensagem);
            List<Intelbras.Message.Helper.Entities.PerguntaItem> lstQuestionarioRespostaItem = new List<Pollux.Entities.PerguntaItem>();

            //Consultas
            List<QuestionarioPergunta> lstQuestionarioPergunta = new Servicos.QuestionarioPerguntaServices(this.Organizacao, this.IsOffline).ListarQuestionarioPerguntaPorQuestionario(xml.Questionario);
            List<QuestionarioResposta> lstRespostasConta = new Servicos.QuestionarioRespostaServices(this.Organizacao, this.IsOffline).ObterREspostaConta(xml.Conta, true);

            string strMarcas = "";
            #region Marcas   
            if (xml.Conta != null)
            {
                Conta conta = new Servicos.ContaService(this.Organizacao, this.IsOffline).BuscaConta(new Guid(xml.Conta));
                if (conta.MarcasEnviadas != null)
                    strMarcas = conta.MarcasEnviadas;
            }
            #endregion

            #region Lista
            if (lstQuestionarioPergunta != null && lstQuestionarioPergunta.Count > 0)
            {
                foreach (QuestionarioPergunta crmItem in lstQuestionarioPergunta)
                {
                    Pollux.Entities.PerguntaItem objPollux = new Pollux.Entities.PerguntaItem();
                    objPollux.OpcoesResposta = new List<Pollux.Entities.OpcaoRespostaItem>();                    
                    QuestionarioGrupoPergunta grupo = new Servicos.QuestionarioGrupoPerguntaServices(this.Organizacao, this.IsOffline).ListarGrupoById(crmItem.Grupo.Id.ToString());

                    objPollux.CodigoPergunta = crmItem.ID.ToString();
                    objPollux.NomePergunta = crmItem.Nome;
                    objPollux.TipoPergunta = crmItem.TipoPergunta;
                    objPollux.OrdemPergunta = crmItem.Ordem;
                    objPollux.PesoPergunta = crmItem.Peso;

                    if (grupo != null)
                    {
                        objPollux.NomeGrupo = grupo.Nome;
                        objPollux.GrupoOrdem = grupo.Ordem;
                    }
                    
                    List<QuestionarioOpcao> lstOpcoesResposta = new Servicos.QuestionarioOpcaoServices(this.Organizacao, this.IsOffline).ListarQuestionarioOpcaoPor((Guid)crmItem.ID);

                    foreach (QuestionarioOpcao opcaoItem in lstOpcoesResposta)
                    {
                        Pollux.Entities.OpcaoRespostaItem objPolluxOpcao = new Pollux.Entities.OpcaoRespostaItem();

                        objPolluxOpcao.CodigoOpcao = opcaoItem.ID.ToString();
                        objPolluxOpcao.NomeOpcao = opcaoItem.Nome;
                        objPolluxOpcao.OrdemOpcao = opcaoItem.Ordem;
                        objPolluxOpcao.PontuacaoResposta = opcaoItem.Pontuacao;

                        var selecionada = lstRespostasConta.Where(s => s.QuestionarioOpcao.Id == opcaoItem.ID).FirstOrDefault<QuestionarioResposta>();

                        if (selecionada != null)
                        {
                            objPolluxOpcao.ValorResposta = selecionada.Valor;
                            objPolluxOpcao.Selecionada = true;
                        }
                        else
                        {
                            objPolluxOpcao.ValorResposta = 0;
                            objPolluxOpcao.Selecionada = false;
                        }

                        objPollux.OpcoesResposta.Add(objPolluxOpcao);
                    }

                    lstQuestionarioRespostaItem.Add(objPollux);
                }
            }
            else
            {
                resultadoPersistencia.Sucesso = true;
                resultadoPersistencia.Mensagem = "Não foram encontrados registros que satisfaçam os critérios de pesquisa.";
                retorno.Add("Resultado", resultadoPersistencia);
                return CriarMensagemRetorno<Pollux.MSG0293R1>(numeroMensagem, retorno);
            }

            #endregion
            resultadoPersistencia.Sucesso = true;
            resultadoPersistencia.Mensagem = "Integração ocorrida com sucesso";
            retorno.Add("Marcas", strMarcas);
            retorno.Add("Perguntas", lstQuestionarioRespostaItem);
            retorno.Add("Resultado", resultadoPersistencia);
            return CriarMensagemRetorno<Pollux.MSG0293R1>(numeroMensagem, retorno);
        }
        #endregion

        #region Definir Propriedades
        public QuestionarioPergunta DefinirPropriedades(Intelbras.Message.Helper.MSG0293 xml)
        {
            var crm = new Model.QuestionarioPergunta(this.Organizacao, this.IsOffline);
            return crm;
        }
        #endregion

        #region Métodos Auxiliares

        public string Enviar(QuestionarioPergunta objModel)
        {
            return String.Empty;
        }

        public QuestionarioPergunta DefinirPropriedades(MSG0293 legado)
        {

            throw new NotImplementedException();
        }
        #endregion
    }
}
