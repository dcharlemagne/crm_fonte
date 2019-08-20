using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Intelbras.Crm.Domain.Model;
using System.Web.Services.Protocols;
using Tridea.Framework.DomainModel;
using Crm.Entities.CrmSdk;
using System.Diagnostics;
using Intelbras.Crm.Domain.Repository;

namespace Intelbras.Crm.Application.WebServices.Card
{
    /// <summary>
    /// Summary description for InstrutorService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class InstrutorService : System.Web.Services.WebService
    {

        [WebMethod]
        public Guid SalvarAgenda(bool disponivelNoPortal, string clienteId, string treinamentoId, string local, string cidade, DateTime dataInicial, DateTime dataFinal, int numeroDeVagas, string instrutorId, string horaInicial, string horaFinal, string organizacao)
        {
            Guid agendaId = new Guid();
            try
            {
                if (treinamentoId == string.Empty) throw new Exception("O parametro 'treinamentoId' está em branco ou nulo");
                if (local == string.Empty) throw new Exception("O parametro 'local' está em branco ou nulo");
                if (cidade == string.Empty) throw new Exception("O parametro 'cidade' está em branco ou nulo");
                if (instrutorId == string.Empty) throw new Exception("O parametro 'instrutorId' está em branco ou nulo");
                if (horaInicial == string.Empty) throw new Exception("O parametro 'horaInicial' está em branco ou nulo");
                if (horaFinal == string.Empty) throw new Exception("O parametro 'horaFinal'está em branco ou nulo");

                var org = new Organizacao(organizacao);
                var agenda = new Agenda(org);
                var cid = new Cidade(org);

                if (cidade != null)
                {                 
                    agenda.Cidade = cid.PesquisarPor(new Guid(cidade));
                    agenda.Regional = agenda.Cidade.Regional;       
                }
                
                agenda.Fim = dataFinal;
                agenda.Inicio = dataInicial;
                agenda.DisponivelnoPortal = disponivelNoPortal;
                agenda.Instrutor = new Usuario(org) { Id = new Guid(instrutorId) };
                //agenda.Proprietario = agenda.Instrutor;
                agenda.Local = local;
                agenda.Treinamento = new Treinamento(org) { Id = new Guid(treinamentoId) };
                agenda.Vagas = numeroDeVagas;
                agenda.HoraInicial = new TimeSpan(int.Parse(horaInicial.Split(':')[0]), int.Parse(horaInicial.Split(':')[1]), 00);
                agenda.HoraFinal = new TimeSpan(int.Parse(horaFinal.Split(':')[0]), int.Parse(horaFinal.Split(':')[1]), 00);
                if (!string.IsNullOrEmpty(clienteId))
                    agenda.Cliente = new Cliente(org) { Id = new Guid(clienteId) };

                var agendaRepository = RepositoryFactory.GetRepository<IAgendaRepository>();
                agendaRepository.Organizacao = org;

                agendaId = agendaRepository.Create(agenda);
            }
            catch (SoapException soapEx)
            {
                EventLog.WriteEntry("CRM Salvar Agenda", string.Format("SOAPEXCEPTION: <strong>{0}</strong><br />{1}", soapEx.Message + soapEx.Detail.InnerXml, soapEx.Detail.InnerText));
                }
            catch (Exception ex)
            {
                EventLog.WriteEntry("CRM Salvar Agenda", string.Format("<strong>{0}</strong><br />{1}", ex.Message, ex.StackTrace));
               }

            return agendaId;
        }

        [WebMethod]
        public Guid? SalvarPresencaDoParticipante(bool presenca, string agendaId, string participanteId, DateTime data, string organizacao, string presencaId)
        {
            //try
            //{
            Guid? listaId = null;
            Intelbras.Crm.Domain.Services.TreinamentoService.Organizacao = new Organizacao(organizacao);
            if (participanteId == string.Empty)
                throw new Exception("O parametro do participante não pode ser vazio ou nulo");
            if (agendaId == string.Empty)
                throw new Exception("O parametro da agenda não pode ser vazio ou nulo");
            Intelbras.Crm.Domain.Services.TreinamentoService.Organizacao = new Organizacao(organizacao);
            var contato = new Contato(new Organizacao(organizacao)) { Id = new Guid(participanteId) };
            var agenda = new Agenda(new Organizacao(organizacao)) { Id = new Guid(agendaId) };
            var participante = Intelbras.Crm.Domain.Services.TreinamentoService.ObterParticipantePor(agenda, contato);
            if (participante != null)
            {
                var treinamento = new Treinamento(new Organizacao(organizacao));

                var lista = new ListaDePresenca(new Organizacao(organizacao));
                lista.Participante = participante;
                lista.Presenca = presenca;
                lista.Data = data;
                if (presencaId == string.Empty)
                    treinamento.SalvarPresencaDoParticipante(lista);
                else
                {
                    lista.Id = new Guid(presencaId);
                    listaId = treinamento.AtualizaPresencaDoParticipante(lista);
                }
                return listaId;
            }
            else
                throw new Exception("O participante não foi encontrado. ID Participante: " + participanteId.ToString() + " :: ID Agenda: " + agendaId.ToString());
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        [WebMethod]
        public ListaDePresenca ObterPresencaDoParticipante(string agendaId, string participanteId, DateTime data, string organizacao)
        {
            try
            {
                ListaDePresenca presenca = null;
                if (participanteId == string.Empty)
                    throw new Exception("O parametro do participante não pode ser vazio ou nulo");
                if (agendaId == string.Empty)
                    throw new Exception("O parametro da agenda não pode ser vazio ou nulo");

                var _organizacao = new Organizacao(organizacao);
                var treinamento = new Treinamento(_organizacao);
                var contato = new Contato(new Organizacao(organizacao)) { Id = new Guid(participanteId) };
                var agenda = new Agenda(new Organizacao(organizacao)) { Id = new Guid(agendaId) };
                Intelbras.Crm.Domain.Services.TreinamentoService.Organizacao = new Organizacao(organizacao);
                var participante = Intelbras.Crm.Domain.Services.TreinamentoService.ObterParticipantePor(agenda, contato);
                if (participante != null)
                {
                    var lista = new ListaDePresenca(_organizacao);
                    lista.Data = data;
                    lista.Participante = participante;

                    presenca = treinamento.ObterListaDePresencaPor(lista);
                }

                return presenca;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public List<ListaDePresenca> ListarHistoricoDePresenca(string agendaId, string participanteId, string organizacao)
        {
            try
            {
                if (agendaId == string.Empty)
                    throw new Exception("O parametro 'agendaId' não pode ser vazio ou nulo.");
                if (participanteId == string.Empty)
                    throw new Exception("O parametro 'participanteId' não pode ser vazio ou nulo.");

                var agenda = new Agenda(new Organizacao(organizacao)) { Id = new Guid(agendaId) };
                var participante = new Contato(new Organizacao(organizacao)) { Id = new Guid(participanteId) };
                var treinamento = new Treinamento(new Organizacao(organizacao));

                return treinamento.ObterListaDePresencaPor(agenda);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public void CalculaFrequenciaDePresenca(string participanteId, string agendaId, string organizacao)
        {
            try
            {
                if (String.IsNullOrEmpty(participanteId))
                    throw new Exception("O parametro 'participanteId' está vazio ou é inválido");
                if (String.IsNullOrEmpty(agendaId))
                    throw new Exception("o parametro 'agendaId' está vazio ou é inválido");

                var agenda = new Agenda(new Organizacao(organizacao)) { Id = new Guid(agendaId) };
                var participante = new Contato(new Organizacao(organizacao)) { Id = new Guid(participanteId) };



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public void ExcluirAgenda(string agendaId, bool excluirCompromissos, string organizacao)
        {
            if (string.IsNullOrEmpty(agendaId))
                throw new Exception("O parametro 'agenda' está em branco ou é inválido.");

            var _organizacao = new Organizacao(organizacao);
            var agenda = new Agenda(_organizacao) { Id = new Guid(agendaId) };
            agenda.ExcluirAgenda();
        }

        [WebMethod]
        public Agenda ObterAgendaPor(string agendaId, string organizacao)
        {
            var agendaRepository = RepositoryFactory.GetRepository<IAgendaRepository>();
            agendaRepository.Organizacao = new Organizacao(organizacao);

            return agendaRepository.Retrieve(new Guid(agendaId));
        }

        // 08/11/2012
        //[WebMethod]
        //public List<Contato> ListarContatosPor(Contato contato, string organizacao)
        //{
        //    var todosOsContatos = RepositoryFactory.GetRepository<IContatoRepository>();
        //    todosOsContatos.Organizacao = new Organizacao(organizacao);
        //    return todosOsContatos.ListarContatosPor(contato);
        //}

    }
}
