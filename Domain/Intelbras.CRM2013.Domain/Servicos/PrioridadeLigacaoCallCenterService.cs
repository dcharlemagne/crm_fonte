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
    public class PrioridadeLigacaoCallCenterService
    {
        private readonly string _filaPadra = "0";

        private RepositoryService RepositoryService { get; set; }

        public PrioridadeLigacaoCallCenterService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PrioridadeLigacaoCallCenterService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        public PrioridadeLigacaoCallCenterService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        public int? ObterPrioridade(string cpfCnpj, string fila)
        {
            string[] filas = new string[] { fila, _filaPadra };

            var listaPrioridade = RepositoryService.PrioridadeLigacaoCallCenter.ListarPorCpfCnpjNomeFila(cpfCnpj, filas);
            
            var prioridade = listaPrioridade.FirstOrDefault(x => x.NomeFila == fila);

            if (prioridade != null)
            {
                return prioridade.Prioridade;
            }

            var prioridadePadrao = listaPrioridade.FirstOrDefault(x => x.NomeFila == _filaPadra);

            if (prioridadePadrao != null)
            {
                return prioridadePadrao.Prioridade;
            }

            return null;
        }
    }
}
