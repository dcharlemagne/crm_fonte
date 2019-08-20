using System;
using Intelbras.CRM2013.Domain.Model;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ModeloService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ModeloService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ModeloService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public string IntegracaoBarramento(Modelo modelo)
        {
            Domain.Integracao.MSG0281 msgModelo = new Domain.Integracao.MSG0281(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgModelo.Enviar(modelo);
        }

        public Categoria ObterPor(Guid modeloId)
        {
            return RepositoryService.Categoria.ObterPor(modeloId);
        }
        
        #endregion
    }
}