using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class AcaoSubsidiadaVmcService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public AcaoSubsidiadaVmcService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public AcaoSubsidiadaVmcService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public AcaoSubsidiadaVmc ObterPorCodigoAcao(String codigoAcao)
        {
            return RepositoryService.AcaoSubsidiadaVmc.ObterPorCodigo(codigoAcao);
        }
        public AcaoSubsidiadaVmc ObterPor(Guid acaoId)
        {
            return RepositoryService.AcaoSubsidiadaVmc.ObterPor(acaoId);
        }

        public List<AcaoSubsidiadaVmc> Listar()
        {
            return RepositoryService.AcaoSubsidiadaVmc.Listar();
        }

        #endregion
    }
}
