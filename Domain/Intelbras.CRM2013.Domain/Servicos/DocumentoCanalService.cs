using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class DocumentoCanalService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DocumentoCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public DocumentoCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Guid Persistir(DocumentoCanal ObjDocumentoCanal)
        {
            DocumentoCanal tmpDocumentoCanal = null;

            if(ObjDocumentoCanal.ID.HasValue)
                tmpDocumentoCanal = RepositoryService.DocumentoCanal.ObterPor((Guid)ObjDocumentoCanal.ID);

            if (tmpDocumentoCanal !=null)
            {
                ObjDocumentoCanal.ID = tmpDocumentoCanal.ID;

                RepositoryService.DocumentoCanal.Update(ObjDocumentoCanal);

                return tmpDocumentoCanal.ID.Value;
            }
            else
                return RepositoryService.DocumentoCanal.Create(ObjDocumentoCanal);
        }


        public DocumentoCanal buscaDocumentoCanal(Guid documentoCanal)
        {
            DocumentoCanal TmpDocumentoCanal = null;

            TmpDocumentoCanal = RepositoryService.DocumentoCanal.ObterPor(documentoCanal);

            if (TmpDocumentoCanal != null)
            {
                return TmpDocumentoCanal;
            }

            return null;
        }

        public List<DocumentoCanal> ListarDocumentoCanal(Guid accountId)
        {
            return RepositoryService.DocumentoCanal.ListarPor(accountId);
        }

        #endregion



    }
}


