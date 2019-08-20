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
    class ItemFilaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ItemFilaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ItemFilaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public ItemFila Persistir(Model.ItemFila ObjItemFila)
        {

            Model.ItemFila TmpItemFila = null;

            if (ObjItemFila.ID.HasValue)
                TmpItemFila = RepositoryService.ItemFila.ObterPor(ObjItemFila.ID.Value);

            if (TmpItemFila != null)
            {
                ObjItemFila.ID = TmpItemFila.ID;


                RepositoryService.ItemFila.Update(ObjItemFila);

                return ObjItemFila;
            }
            else
                ObjItemFila.ID = RepositoryService.ItemFila.Create(ObjItemFila);
            return ObjItemFila;
        }

        public Fila BuscaFilaPorId(Guid FilaId)
        {
            return RepositoryService.Fila.ObterPor(FilaId);
        }
        #endregion  
    }
}
