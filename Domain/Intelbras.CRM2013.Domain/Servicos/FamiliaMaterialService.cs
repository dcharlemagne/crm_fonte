using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class FamiliaMaterialService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FamiliaMaterialService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public FamiliaMaterialService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion


        public FamiliaMaterial ObterPor(Guid FamiliaMaterialId)
        {
            return RepositoryService.FamiliaMaterial.ObterPor(FamiliaMaterialId);
        }

        public FamiliaMaterial ObterPor(string codigoFamiliaMaterial)
        {
            return RepositoryService.FamiliaMaterial.ObterPor(codigoFamiliaMaterial);
        }

        public FamiliaMaterial Persistir(FamiliaMaterial FamiliaMaterial)
        {
            if (!String.IsNullOrEmpty(FamiliaMaterial.Codigo))
            {
                FamiliaMaterial TmpFamiliaMaterial = RepositoryService.FamiliaMaterial.ObterPor(FamiliaMaterial.Codigo);

                if (TmpFamiliaMaterial != null)
                {
                    FamiliaMaterial.ID = TmpFamiliaMaterial.ID;

                    RepositoryService.FamiliaMaterial.Update(FamiliaMaterial);

                    if (!TmpFamiliaMaterial.Status.Equals(FamiliaMaterial.Status) && FamiliaMaterial.Status != null)
                        this.MudarStatus(TmpFamiliaMaterial.ID.Value, FamiliaMaterial.Status.Value);

                    return FamiliaMaterial;
                }
                else
                {
                    FamiliaMaterial.ID = RepositoryService.FamiliaMaterial.Create(FamiliaMaterial);
                }
            }
            else
                return null;

            return FamiliaMaterial;
        }

        public bool MudarStatus(Guid id, int stateCode)
        {
            return RepositoryService.FamiliaMaterial.AlterarStatus(id, stateCode);
        }

    }
}
