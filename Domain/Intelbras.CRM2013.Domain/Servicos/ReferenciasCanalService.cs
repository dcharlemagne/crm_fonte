using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ReferenciasCanalService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ReferenciasCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }


        public ReferenciasCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos

        public ReferenciasCanal Persistir(Model.ReferenciasCanal ObjReferenciasCanal)
        {
            ReferenciasCanal TmpReferenciasCanal = null;
            if (ObjReferenciasCanal.ID.HasValue)
            {
                TmpReferenciasCanal = RepositoryService.ReferenciasCanal.ObterPor(ObjReferenciasCanal.ID.Value);

                if (TmpReferenciasCanal != null)
                {
                    ObjReferenciasCanal.ID = TmpReferenciasCanal.ID;

                    RepositoryService.ReferenciasCanal.Update(ObjReferenciasCanal);

                    if (!TmpReferenciasCanal.State.Equals(ObjReferenciasCanal.State) && ObjReferenciasCanal.State != null)
                        this.MudarStatus(TmpReferenciasCanal.ID.Value, ObjReferenciasCanal.State.Value);

                    return TmpReferenciasCanal;
                }
                else
                    return null; 
            }
            else
            {
                ObjReferenciasCanal.ID = RepositoryService.ReferenciasCanal.Create(ObjReferenciasCanal);
                return ObjReferenciasCanal;
            }
        }

        public ReferenciasCanal BuscaReferenciasCanal(Guid referenciasCanalID)
        {
            List<ReferenciasCanal> lstReferenciasCanal = RepositoryService.ReferenciasCanal.ListarPor(referenciasCanalID);
            if (lstReferenciasCanal.Count > 0)
                return lstReferenciasCanal.First<ReferenciasCanal>();
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.ReferenciasCanal.AlterarStatus(id, status);
        }

        #endregion
    }
}
