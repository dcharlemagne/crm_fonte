using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class UnidadeNegocioService
    {
         #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public UnidadeNegocioService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public UnidadeNegocioService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public UnidadeNegocioService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }

        #endregion

        #region Métodos

        public UnidadeNegocio Persistir(UnidadeNegocio ObjUnidadeNegocio)
        {
            UnidadeNegocio TmpUnidadeNegocio = null;

            if (!String.IsNullOrEmpty(ObjUnidadeNegocio.ChaveIntegracao))
            {
                TmpUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(ObjUnidadeNegocio.ChaveIntegracao);
                if (TmpUnidadeNegocio != null)
                {
                    ObjUnidadeNegocio.ID = TmpUnidadeNegocio.ID;
                    ObjUnidadeNegocio.Id = TmpUnidadeNegocio.ID.Value;

                    RepositoryService.UnidadeNegocio.Update(ObjUnidadeNegocio);
                   
                    return TmpUnidadeNegocio;
                }
                else
                {
                    ObjUnidadeNegocio.ID = RepositoryService.UnidadeNegocio.Create(ObjUnidadeNegocio);
                    return ObjUnidadeNegocio;
                }
            }
            else
            {
                return null;
            }
        }


        public UnidadeNegocio BuscaUnidadeNegocio(Guid unidadeNegocioID)
        {
            List<UnidadeNegocio> lstUnidadeNegocio = RepositoryService.UnidadeNegocio.ListarPor(unidadeNegocioID);
            if (lstUnidadeNegocio.Count > 0)
                return lstUnidadeNegocio.First<UnidadeNegocio>();
            return null;
        }

        public UnidadeNegocio BuscaUnidadeNegocioPorNome(String unidadeNegocioNome)
        {
            List<UnidadeNegocio> lstUnidadeNegocio = RepositoryService.UnidadeNegocio.ListarPor(unidadeNegocioNome);
            if (lstUnidadeNegocio.Count > 0)
                return lstUnidadeNegocio.First<UnidadeNegocio>();
            return null;
        }

        public UnidadeNegocio BuscaUnidadeNegocioPorCodigo(String unidadeNegocioCodigo)
        {
            List<UnidadeNegocio> lstUnidadeNegocio = RepositoryService.UnidadeNegocio.ListarPor(unidadeNegocioCodigo);
            if (lstUnidadeNegocio.Count > 0)
                return lstUnidadeNegocio.First<UnidadeNegocio>();
            return null;
        }
        public UnidadeNegocio BuscaUnidadeNegocioPorChaveIntegracao(string unidadeNegocioCodigo)
        {
            UnidadeNegocio objUnidadeNegocio =  RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(unidadeNegocioCodigo);
            if (objUnidadeNegocio != null)
                return objUnidadeNegocio;
            return null;
        }

        public List<UnidadeNegocio> BuscaUnidadeNegocioPorChaveIntegracao(string[] conjCodigosUnidadeNegocio)
        {
            List<UnidadeNegocio> objUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(conjCodigosUnidadeNegocio);
            if (objUnidadeNegocio.Count() > 0)
                return objUnidadeNegocio;
            return null;
        }

        public UnidadeNegocio ExtrairUnidadeNegocioPorChaveIntegracao(string unidadeNegocioCodigo, List<UnidadeNegocio> listaUnidadesNegocio)
        {
            foreach (var unidade in listaUnidadesNegocio)
            {
                if (unidade.ChaveIntegracao.ToLower() == unidadeNegocioCodigo.ToLower())
                    return unidade;
            }

            return null;
        }

        public List<UnidadeNegocio> ListarUnidadeNegocioPorConta(Guid itbc_accountid)
        {
            return RepositoryService.UnidadeNegocio.ListarPorConta(itbc_accountid);
        }

        public List<UnidadeNegocio> ListarTodos()
        {
            return RepositoryService.UnidadeNegocio.ListarTodos();
        }

        public Guid BuscarRelacionamentoUnidadeDeNegocioSolicBeneficio(String chaveIntegracao)
        {
            return RepositoryService.UnidadeNegocio.ObterRelacionamentoUnidadeNegocioBenef(chaveIntegracao);
        }
        
        #endregion
    }
}
