using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class UnidadeKonvivaService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public UnidadeKonvivaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public UnidadeKonvivaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public UnidadeKonvivaService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        #region Metodos

        public UnidadeKonviva ObterPor(Int32 idInterno)
        {
            return RepositoryService.UnidadeKonviva.ObterPorIdInterno(idInterno);
        }

        public UnidadeKonviva ObterPor(String nome)
        {
            return RepositoryService.UnidadeKonviva.ObterPorNome(nome);
        }

        public UnidadeKonviva ObterPor(Guid idUnidadeKonviva)
        {
            return RepositoryService.UnidadeKonviva.Retrieve(idUnidadeKonviva);
        }
        #endregion



    }
}
