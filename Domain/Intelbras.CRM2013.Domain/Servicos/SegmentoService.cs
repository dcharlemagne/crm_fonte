using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SegmentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public SegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        public SegmentoService(RepositoryService repository)
        {
            RepositoryService = new RepositoryService(repository.NomeDaOrganizacao, repository.IsOffline, repository.Provider);
        }
        #endregion

        public Segmento BuscaSegmento(String codigoSegmento)
        {
            List<Segmento> lstSegmento = RepositoryService.Segmento.ListarPor(codigoSegmento);
            if (lstSegmento.Count > 0)
                return lstSegmento.First<Segmento>();
            return null;
        }

        public Segmento Persistir(Segmento ObjSegmento, ref bool MudancaProprietario)
        {
            Segmento TmpSegmento = null;

            if (!String.IsNullOrEmpty(ObjSegmento.CodigoSegmento))
            {
                TmpSegmento = RepositoryService.Segmento.ObterPor(ObjSegmento.CodigoSegmento);
                if (TmpSegmento != null)
                {
                    ObjSegmento.ID = TmpSegmento.ID;

                    RepositoryService.Segmento.Update(ObjSegmento);

                    if (!TmpSegmento.Status.Equals(ObjSegmento.Status) && ObjSegmento.Status != null)
                        this.MudarStatus(TmpSegmento.ID.Value, ObjSegmento.Status.Value);

                    return TmpSegmento;
                }
                else
                {
                    ObjSegmento.ID = RepositoryService.Segmento.Create(ObjSegmento);
                    return ObjSegmento;
                }
            }
            else
            {
                return null;
            }

        }

        public Segmento ObterPor(Guid seguimentoId)
        {
            return RepositoryService.Segmento.ObterPor(seguimentoId);
            
        }
        public List<Segmento> ListarPorUnidadeNegocio(Guid codigoUnidadeNegocio)
        {
            return RepositoryService.Segmento.ListarPor(codigoUnidadeNegocio);

        }
        public List<Segmento> ListarTodos()
        {
            return RepositoryService.Segmento.ListarTodos();

        }

        public List<Segmento> ListarCanaisVerdes()
        {
            return RepositoryService.Segmento.ListarCanaisVerdes();
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Segmento.AlterarStatus(id, status);
        }
    }
}
