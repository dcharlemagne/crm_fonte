using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class NivelPosVendaService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public NivelPosVendaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public NivelPosVendaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public string IntegracaoBarramento(NivelPosVenda nivelPosVenda)
        {
            Domain.Integracao.MSG0132 msgNivelPosVenda = new Domain.Integracao.MSG0132(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            return msgNivelPosVenda.Enviar(nivelPosVenda);
        }
    }
}