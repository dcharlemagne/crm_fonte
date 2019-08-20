using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Intelbras.Crm.Domain.Model;

namespace Intelbras.Crm.Application.WebServices.Card
{
    /// <summary>
    /// Summary description for AvaliacaoService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AvaliacaoService : System.Web.Services.WebService
    {

        [WebMethod]
        public List<Avaliacao> ListarAvaliacoesPor(string treinamentoId, string organizacao)
        {
            try
            {
                if (treinamentoId == string.Empty)
                    throw new ArgumentNullException("O ID do treinameto está em branco.");

                var _organizacao = new Organizacao(organizacao);

                var treinamento = new Treinamento(_organizacao) { Id = new Guid(treinamentoId) };

                return Intelbras.Crm.Domain.Services.AvaliacoesService.ListarAvaliacoesPor(treinamento);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public Avaliacao ListarAvaliacaoPor(string avaliacaoId, string organizacao)
        {
            try
            {
                var avaliacao = new Avaliacao(new Organizacao(organizacao));
                var id = new Guid(avaliacaoId);
                avaliacao = avaliacao.ObterAvaliacaoPor(id);

                return avaliacao;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [WebMethod]
        public void SalvarResposta(string agendaId, string avaliacaoId, string participanteId, string perguntaId, string respostaId, string textoDissertativo, string organizacao)
        {
            if (agendaId == string.Empty)
                throw new ArgumentNullException("O ID da Agenda do Treinamento está em branco");
            if (avaliacaoId == string.Empty)
                throw new ArgumentNullException("O ID da Avaliação está em branco.");
            if (participanteId == string.Empty)
                throw new ArgumentNullException("O ID do Participante está em branco.");
            if (perguntaId == string.Empty)
                throw new ArgumentNullException("O ID da Pergunta está em branco.");

            var agenda = new Guid(agendaId);
            var participante = new Guid(participanteId);
            var pergunta = new Guid(perguntaId);

            Guid? resposta = null;
            if (respostaId != string.Empty)
                resposta = new Guid(respostaId);

            var avaliacao = new Avaliacao(new Organizacao(organizacao)) { Id = new Guid(avaliacaoId) };
            avaliacao.SalvarAvaliacaoDoParticipante(agenda, participante, pergunta, resposta, textoDissertativo);
        }
    }
}
