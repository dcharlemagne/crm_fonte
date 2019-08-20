using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class FornecedorCanalService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FornecedorCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public FornecedorCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public FornecedorCanal Persistir(Model.FornecedorCanal ObjFornCanal)
        {
            FornecedorCanal TmpFornCanal = null;
            if (ObjFornCanal.ID.HasValue)
            {
                TmpFornCanal = RepositoryService.FornecedorCanal.ObterPor(ObjFornCanal.ID.Value);

                if (TmpFornCanal != null)
                {
                    ObjFornCanal.ID = TmpFornCanal.ID;
                    RepositoryService.FornecedorCanal.Update(ObjFornCanal);
                    //Altera Status - Se necessário
                    if (!TmpFornCanal.State.Equals(ObjFornCanal.State) && ObjFornCanal.State != null)
                        this.MudarStatus(TmpFornCanal.ID.Value, ObjFornCanal.State.Value);
                    return TmpFornCanal;
                }
                else
                    return null;
            }
            else
            {
                ObjFornCanal.ID = RepositoryService.FornecedorCanal.Create(ObjFornCanal);
                return ObjFornCanal;
            }
        }

        public FornecedorCanal BuscaFornecedorCanal(Guid fornecedorCanal)
        {
            List<FornecedorCanal> lstFornecedorCanal = RepositoryService.FornecedorCanal.ListarPor(fornecedorCanal);
            if (lstFornecedorCanal.Count > 0)
                return lstFornecedorCanal.First<FornecedorCanal>();
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.FornecedorCanal.AlterarStatus(id, status);
        }

        #endregion

    }
}
