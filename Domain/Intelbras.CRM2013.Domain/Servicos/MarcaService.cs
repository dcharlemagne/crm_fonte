using System;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MarcaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MarcaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public MarcaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public string IntegracaoBarramento(Marca marca)
        {
            Domain.Integracao.MSG0280 msgMarca = new Domain.Integracao.MSG0280(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgMarca.Enviar(marca);
        }

        public Marca ObterPor(Guid marcaId)
        {
            return RepositoryService.Marca.ObterPor(marcaId);
        }

        public List<Marca> ListarPorContato(Guid contatoId)
        {
            return RepositoryService.Marca.ListarPorContato(contatoId);
        }
        #endregion
    }
}