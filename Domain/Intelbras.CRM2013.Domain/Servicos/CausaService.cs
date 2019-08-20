using System;
using Intelbras.CRM2013.Domain.Model;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class CausaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CausaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CausaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public string IntegracaoBarramento(Causa objModelCausa)
        {
            Domain.Integracao.MSG0298 msg = new Domain.Integracao.MSG0298(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msg.Enviar(objModelCausa);
        }

        public Causa ObterPor(Guid causaId)
        {
            return RepositoryService.Causa.ObterPor(causaId);
        }
        #endregion
    }
}