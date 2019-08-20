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
    public class BensService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public BensService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public BensService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public Bens Persistir(Model.Bens objBens)
        {
            Bens TmpBens = null;
            if (objBens.ID.HasValue)
            {
                TmpBens = RepositoryService.Bens.ObterPor(objBens.ID.Value);

                if (TmpBens != null)
                {
                    objBens.ID = TmpBens.ID;
                    RepositoryService.Bens.Update(objBens);
                    //Altera Status - Se necessário
                    if (objBens.Status.HasValue && !TmpBens.Status.Equals(objBens.Status))
                        this.MudarStatus(TmpBens.ID.Value, objBens.Status.Value);
                    return TmpBens;
                }
                else
                    return null;
            }
            else
            {
                objBens.ID = RepositoryService.Bens.Create(objBens);
                return objBens;
            }
        }

        public Bens BuscaBens(Guid bensID)
        {
            List<Bens> lstBens = RepositoryService.Bens.ListarPor(bensID);
            if (lstBens.Count > 0)
                return lstBens.First<Bens>();
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Bens.AlterarStatus(id, status);
        }

        #endregion
    }
}
