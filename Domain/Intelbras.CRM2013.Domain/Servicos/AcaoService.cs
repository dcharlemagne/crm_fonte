using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class AcaoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public AcaoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public AcaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Métodos
        public string IntegracaoBarramento(Acao objModel)
        {
            Domain.Integracao.MSG0299 msg = new Domain.Integracao.MSG0299(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msg.Enviar(objModel);
        }

        public Acao ObterPor(Guid acaoId)
        {
            return RepositoryService.Acao.ObterPor(acaoId);
        }
        #endregion
    }
}
