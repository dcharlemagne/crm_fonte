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
    public class SharePointSiteService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SharePointSiteService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public SharePointSiteService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public SharePointSite Persistir(Model.SharePointSite objSharePointSite)
        {
            SharePointSite TmpSharePointSite = null;
            if (objSharePointSite.ID.HasValue)
            {
                TmpSharePointSite = RepositoryService.SharePointSite.ObterPor(objSharePointSite.ID.Value);

                if (TmpSharePointSite != null)
                {
                    objSharePointSite.ID = TmpSharePointSite.ID;
                    RepositoryService.SharePointSite.Update(objSharePointSite);
                    //Altera Status - Se necessário
                    if (!TmpSharePointSite.Status.Equals(objSharePointSite.Status) && objSharePointSite.Status != null)
                        this.MudarStatus(TmpSharePointSite.ID.Value, objSharePointSite.Status.Value);
                    return TmpSharePointSite;
                }
                else
                    return null;
            }
            else
            {
                objSharePointSite.ID = RepositoryService.SharePointSite.Create(objSharePointSite);
                return objSharePointSite;
            }
        }

        public SharePointSite ObterPorUrlRelativa()
        {
            return RepositoryService.SharePointSite.ObterPor();
        }


        public List<DocumentoSharePoint> ListarPorIdRegistro(Guid ObjetoRelativoId)
        {
            return RepositoryService.DocumentoSharePoint.ListarPorIdRegistro(ObjetoRelativoId);
        }


        public List<DocumentoSharePoint> ListarTodos()
        {
            return RepositoryService.DocumentoSharePoint.ListarTodos();
        }

        public SharePointSite ObterPor(Guid sharePointSiteID)
        {
            return RepositoryService.SharePointSite.ObterPor(sharePointSiteID);
        }



        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.SharePointSite.AlterarStatus(id, status);
        }

        #endregion
    }
}
