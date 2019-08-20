using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class SeguroContaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public SeguroContaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }

        public SeguroContaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }

        #endregion

        #region Métodos

        public SeguroConta Persistir(Model.SeguroConta ObjSeguroConta)
        {

            SeguroConta TmpSeguroConta = null;
            if (ObjSeguroConta.ID.HasValue)
            {
                TmpSeguroConta = RepositoryService.SeguroConta.ObterPor(ObjSeguroConta.ID.Value);

                if (TmpSeguroConta != null)
                {
                    ObjSeguroConta.ID = TmpSeguroConta.ID;

                    RepositoryService.SeguroConta.Update(ObjSeguroConta);

                    if (!TmpSeguroConta.State.Equals(ObjSeguroConta.State) && ObjSeguroConta.State != null)
                        this.MudarStatus(TmpSeguroConta.ID.Value, ObjSeguroConta.State.Value);

                    return TmpSeguroConta;
                }
                else
                    return null;
            }
            else
            {
                ObjSeguroConta.ID = RepositoryService.SeguroConta.Create(ObjSeguroConta);
                return ObjSeguroConta;
            }
        }

        public SeguroConta BuscaFornecedorCanal(Guid seguroContaID)
        {
            List<SeguroConta> lstSeguroConta = RepositoryService.SeguroConta.ListarPor(seguroContaID);
            if (lstSeguroConta.Count > 0)
                return lstSeguroConta.First<SeguroConta>();
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.SeguroConta.AlterarStatus(id, status);
        }

        #endregion

    }
}
