using Intelbras.CRM2013.Domain.Model;
using System;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ObservacaoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ObservacaoService(string organizacao, bool isOffline) : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ObservacaoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ObservacaoService(RepositoryService repositoryService)
        {
            RepositoryService = repositoryService;
        }

        #endregion

        public Observacao ObterObservacao(Enum.TipoParametroGlobal tipoParametroGlobal)
        {
            var parametroGlobal = RepositoryService.ParametroGlobal.ObterPorCodigoTipoParametroGlobal((int)tipoParametroGlobal);

            if (parametroGlobal == null)
            {
                throw new ArgumentException("(CRM) Não foi Encontrado Parametro Global para este nivel de Orçamento.");
            }

            return RepositoryService.Observacao.ObterPorParametrosGlobais(parametroGlobal.ID.Value);
        }
    }
}