using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class DocumentoCanalEstoqueGiroService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public DocumentoCanalEstoqueGiroService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public DocumentoCanalEstoqueGiroService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Guid Persistir(DocumentoCanalEstoqueGiro ObjDocumentoCanalEstoqueGiro)
        {
            DocumentoCanalEstoqueGiro tmpDocumentoCanalEstoqueGiro = null;

            if(ObjDocumentoCanalEstoqueGiro.ID.HasValue)
                tmpDocumentoCanalEstoqueGiro = RepositoryService.DocumentoCanalEstoqueGiro.ObterPor((Guid)ObjDocumentoCanalEstoqueGiro.ID);

            if (tmpDocumentoCanalEstoqueGiro != null)
            {
                ObjDocumentoCanalEstoqueGiro.ID = tmpDocumentoCanalEstoqueGiro.ID;

                RepositoryService.DocumentoCanalEstoqueGiro.Update(ObjDocumentoCanalEstoqueGiro);

                return tmpDocumentoCanalEstoqueGiro.ID.Value;
            }
            else
                return RepositoryService.DocumentoCanalEstoqueGiro.Create(ObjDocumentoCanalEstoqueGiro);
        }


        public DocumentoCanalEstoqueGiro buscaDocumentoCanalEstoqueGiro(Guid documentoCanalEstoqueGiro)
        {
            DocumentoCanalEstoqueGiro TmpDocumentoCanalEstoqueGiro = null;

            TmpDocumentoCanalEstoqueGiro = RepositoryService.DocumentoCanalEstoqueGiro.ObterPor(documentoCanalEstoqueGiro);

            if (TmpDocumentoCanalEstoqueGiro != null)
            {
                return TmpDocumentoCanalEstoqueGiro;
            }

            return null;
        }

        public List<DocumentoCanalEstoqueGiro> ListarDocumentoCanalEstoqueGiro(Guid? accountid, DateTime? DataCriacao, string URL, bool? SomenteArquivosNovos, DateTime? DataInicial, DateTime? DataFinal)
        {
            return RepositoryService.DocumentoCanalEstoqueGiro.ListarPor(accountid, DataCriacao, URL, SomenteArquivosNovos, DataInicial, DataFinal);
        }

        #endregion



    }
}


