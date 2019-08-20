using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ParametroGlobalService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ParametroGlobalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ParametroGlobalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        public ParametroGlobalService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos
        public List<ParametroGlobal> ListarParamGlobalPorTipoParam(int tipoParametro)
        {
            return RepositoryService.ParametroGlobal.ListarPor(tipoParametro, null, null, null, null, null, null);
        }
        
        public ParametroGlobal ObterPor(int tipoParametro,
                                        Guid? unidadeNegocioId = null,
                                        Guid? classificacaoId = null,
                                        Guid? categoriaId = null,
                                        Guid? nivelPosVendaId = null,
                                        Guid? compromissoId = null,
                                        Guid? beneficioId = null,
                                        int? parametrizar = null)
        {

            return RepositoryService.ParametroGlobal.ObterPor(tipoParametro,
                                                              unidadeNegocioId,
                                                              classificacaoId,
                                                              categoriaId,
                                                              nivelPosVendaId,
                                                              compromissoId,
                                                              beneficioId,
                                                              parametrizar);
        }

        public ParametroGlobal ObterFrequenciaAtividadeChecklist(Guid compromissoId)
        {
            ParametroGlobal parametroGlobal = this.ObterPor((int)Domain.Enum.TipoParametroGlobal.FrequenciaChecklist,null,null,null,null, compromissoId,null,null);

            return parametroGlobal;
        }

        public ParametroGlobal ObterEmailContatosAdministrativos()
        {
            return this.RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)Domain.Enum.TipoParametroGlobal.ContatosAdministrativos); // ContatosAdministrativos
        }

        #endregion

    }
}
