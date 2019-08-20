using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ContratoService
    {

        private RepositoryService RepositoryService { get; set; }

        public ContratoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ContratoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public List<ClienteParticipante> ObterClientesParticipantesPor(Contrato contrato, Model.Conta cliente)
        {
            return RepositoryService.Contrato.ObterClientesParticipantesPor(contrato, cliente);
        }
        public List<ClienteParticipante> ObterEnderecosClientesParticipantes(Contrato contrato, Model.Conta cliente)
        {
            return RepositoryService.Contrato.ObterEnderecoClientesParticipantesPor(contrato, cliente);
        }

        public Endereco ObterEnderecoClienteParticipante(Contrato contrato, Model.Conta cliente)
        {
            return RepositoryService.Endereco.Retrieve(RepositoryService.Contrato.ObterEnderecoIDPrimeiroClienteParticipante(contrato, cliente));
        }

        public void ExcluirEnderecosSemLocalidadePor(List<ClienteParticipante> clientesSemLocalidade)
        {
            foreach (var item in clientesSemLocalidade)
                RepositoryService.Contrato.ExcluirEnderecosSemLocalidadePor(item);
        }

        public LinhaDeContrato ObterLinhaDeContrato(Guid id)
        {
            return RepositoryService.LinhaDoContrato.Retrieve(id);
        }

        public List<Contrato> ObterContratosProximoVencimento(DateTime dataTermino)
        {
            return RepositoryService.Contrato.ObterContratosProximoVencimento(dataTermino);
        }
    }
}
