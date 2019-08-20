using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using System.Net;
using System.IO;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PostagemService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PostagemService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public PostagemService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public Postagem Persistir(Model.Postagem objPostagem)
        {
            Postagem tmpPostagem = null;
            if (objPostagem.ID.HasValue)
            {
                tmpPostagem = RepositoryService.Postagem.Retrieve(objPostagem.ID.Value);

                if (tmpPostagem != null)
                {
                    objPostagem.ID = tmpPostagem.ID;
                    RepositoryService.Postagem.Update(objPostagem);

                    return objPostagem;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                objPostagem.ID = RepositoryService.Postagem.Create(objPostagem);
                return objPostagem;
            }
        }

        public List<Postagem> ListarPorReferenteA(Guid referenteA)
        {
            return RepositoryService.Postagem.ListarPorReferenteA(referenteA);
        }

        #endregion
    }
}
