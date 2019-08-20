using System;
using System.Collections.Generic;
using SDKore.DomainModel;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Enum;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ClienteParticipanteService
    {
        #region Atributos
        private RepositoryService RepositoryService { get; set; }

        public ClienteParticipanteService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ClienteParticipanteService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion


        public List<ClienteParticipante> ObterEnderecosParticipantes(Guid clienteId)
        {
            return RepositoryService.ClienteParticipante.ListarPor(clienteId, Guid.Empty);
        }
    }
}
