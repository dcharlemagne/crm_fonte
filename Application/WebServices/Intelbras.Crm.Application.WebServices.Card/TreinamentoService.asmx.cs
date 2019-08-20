using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Text;
using System.Configuration;
using System.Web.Services.Protocols;
using Intelbras.Crm.Infrastructure.Dal;
using Intelbras.Crm.Domain.Model;
using Intelbras.Crm.Domain.Model.Enum;
using Intelbras.Crm.Domain.Services;
using Intelbras.Crm.Domain.Services.Docs;
using Intelbras.Crm.Domain.Repository;
using Intelbras.Crm.CrossCutting.Base;
using Tridea.Framework.DomainModel;

namespace Intelbras.Crm.Application.WebServices.Card
{
    /// <summary>
    /// Summary description for TreinamentoService
    /// </summary>
    [WebService(Namespace = "http://schemas.microsoft.com/crm/2006/WebServices")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class TreinamentoService : WebServiceBase
    {
        //[WebMethod]
        //public List<Agenda> ListarAgendasPor(Regional regional, Cliente cliente, Organizacao organizacao)
        //{
        //    var agendaRepository = RepositoryFactory.GetRepository<IAgendaRepository>();
        //    agendaRepository.Organizacao = organizacao;

        //    return agendaRepository.ListarPor(regional, cliente, true);
        //}

        //[WebMethod]
        //public List<Agenda> ListarAgendas(string organizacao)
        //{
        //    var agendaRepository = RepositoryFactory.GetRepository<IAgendaRepository>();
        //    agendaRepository.Organizacao = new Organizacao(organizacao);

        //    return (List<Agenda>)agendaRepository.RetrieveAll();
        //}

        [WebMethod]
        public XmlDocument ObterAgendaPor(string agendaId, string organizacao)
        {
            try
            {
                Agenda agenda = DomainService.RepositoryAgenda.Retrieve(new Guid(agendaId));

                base.Mensageiro.AdicionarTopico("StatusAgenda", agenda.Status);
                base.Mensageiro.AdicionarTopico("Sucesso", true);
            }
            catch (Exception ex)
            {
                base.Mensageiro.AdicionarTopico("Sucesso", false);
                base.Mensageiro.AdicionarTopico("MensagemDeErro", "Não foi possível obter a agenda");
                LogService.GravaLog(ex, TipoDeLog.WSCardTreinamentoService, "XmlDocument ObterAgendaPor(string agendaId, string organizacao)");
            }
            return base.Mensageiro.Mensagem;
        }

        // Retirado em 27/09/2012 - Caso não apresente erro deve ser removido.
        //[WebMethod]
        //public Agenda ObterAgenda(string agendaId, string organizacao)
        //{
        //    var agendaRepository = RepositoryFactory.GetRepository<IAgendaRepository>();
        //    agendaRepository.Organizacao = new Organizacao(organizacao);

        //    return agendaRepository.Retrieve(new Guid(agendaId));
        //}
        
        //[WebMethod]
        //public List<Agenda> ListarOsMeusTreinamentos(string usuarioId, string organizacaoName)
        //{
        //    if (usuarioId == string.Empty) throw new ArgumentNullException("O parametro 'usuarioId' não pode ser vazio");
        //    if (organizacaoName == string.Empty) throw new ArgumentNullException("O parametro 'organizacaoName' não pode ser vazio");

        //    Contato contato = new Contato() { Id = new Guid(usuarioId) };

        //    var agendaRepository = RepositoryFactory.GetRepository<IAgendaRepository>();
        //    agendaRepository.Organizacao = new Organizacao(organizacaoName);

        //    return agendaRepository.ListarPor(contato);
        //}

        //[WebMethod]
        //public List<Agenda> ListarOsMeusTreinamentosFiltradosPor(string usuarioId, string status, string organizacaoName)
        //{
        //    if (usuarioId == string.Empty) throw new ArgumentNullException("O parametro 'usuarioId' não pode ser vazio");

        //    var contato = new Contato() { Id = new Guid(usuarioId) };

        //    var agendaRepository = RepositoryFactory.GetRepository<IAgendaRepository>();
        //    agendaRepository.Organizacao = new Organizacao(organizacaoName);

        //    return agendaRepository.ListarPor(contato, int.Parse(status));
        //}

        [WebMethod]
        public string BuscarParametrosDoWebConfig(string parametro)
        {
           return ConfigurationManager.AppSettings[parametro].ToString(); 
        }

        //[WebMethod]
        //public void ConcluirEmissaoDoCertificado(string agendaId, string participanteId, string organizacao)
        //{
        //    try
        //    {
        //        if (participanteId == string.Empty)
        //            throw new ArgumentNullException("O parametro 'usuarioId' não pode ser vazio");
        //        if (agendaId == string.Empty)
        //            throw new ArgumentNullException("O parametro 'agendaId' não pode ser vazio");

        //        var org = new Organizacao(organizacao);

        //        var participante = this.ObterParticipanteDoTreinamento(agendaId, participanteId, org);

        //        if (participante == null)
        //            throw new ArgumentException("O participante não foi encontrado.");

        //        participante.DataEmissaoCertificado = DateTime.Now;

        //        var participanteTreinamentoRepository = RepositoryFactory.GetRepository<IParticipanteTreinamentoRepository>();
        //        participanteTreinamentoRepository.Organizacao = org;

        //        participanteTreinamentoRepository.Update(participante);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //[WebMethod]
        //public void ConfirmarParticipacao(bool confirmado, string agendaId, string participanteId, string organizacao)
        //{
        //    if (participanteId == string.Empty)
        //        throw new ArgumentNullException("O parametro 'usuarioId' não pode ser vazio");
        //    if (agendaId == string.Empty)
        //        throw new ArgumentNullException("O parametro 'agendaId' não pode ser vazio");

        //    var org = new Organizacao(organizacao);

        //    var participante = this.ObterParticipanteDoTreinamento(agendaId, participanteId, org);
        //    if (participante == null)
        //        throw new ArgumentException("O participante não foi encontrado.");

        //    participante.Confirmacao = confirmado ? 1 : 2;

        //    var participanteTreinamentoRepository = RepositoryFactory.GetRepository<IParticipanteTreinamentoRepository>();
        //    participanteTreinamentoRepository.Organizacao = new Organizacao(organizacao);

        //    participanteTreinamentoRepository.Update(participante);
        //}
        

        #region internos

        //private ParticipanteTreinamento ObterParticipanteDoTreinamento(string agendaId, string participanteId, Organizacao org)
        //{
        //    var contato = new Contato(org) { Id = new Guid(participanteId) };
        //    var agenda = new Agenda(org) { Id = new Guid(agendaId) };

        //    var participante = DomainService.RepositoryParticipanteTreinamento.ObterParticipantePor(agenda, contato);

        //    return participante;
        //}

        #endregion


    }
}
