using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.Message.Helper;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class IndiceService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public IndiceService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public IndiceService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion
        //persistir utilizado para nao permitir mudar o proprietario
        public Indice Persistir(Indice ObjIndice)
        {

            Indice TmpIndice = null;

            if (!String.IsNullOrEmpty(ObjIndice.ChaveIntegracao))
            {
                TmpIndice = RepositoryService.Indice.ObterPor(ObjIndice.ChaveIntegracao);

                if (TmpIndice != null)
                {
                    ObjIndice.ID = TmpIndice.ID;

                    RepositoryService.Indice.Update(ObjIndice);

                    if (!TmpIndice.Status.Equals(ObjIndice.Status) && ObjIndice.Status != null)
                        this.MudarStatus(TmpIndice.ID.Value, ObjIndice.Status.Value);

                    return TmpIndice;
                }
                else
                {
                    ObjIndice.ID = RepositoryService.Indice.Create(ObjIndice);
                    return ObjIndice;
                }
            }
            else
            {
                return null;
            }
        }

        public bool MudarStatus(Guid id, int stateCode)
        {
            int statusCode;
            if (stateCode == 0)
            {
                //Ativar
                statusCode = 1;
            }
            else
            {
                //Inativar
                statusCode = 2;
            }

            return RepositoryService.Indice.AlterarStatus(id, stateCode, statusCode);
        }

        public Indice ObterPor(string chaveIntegracao)
        {
            return RepositoryService.Indice.ObterPor(chaveIntegracao);
        }

    }
}
