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
    public class RotaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public RotaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline); 
        }

        public RotaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider); 
        }




        #endregion

        #region Métodos

        public Rota Persistir(Rota objRota)
        {
            Rota TmpRota = null;

            if (!String.IsNullOrEmpty(objRota.CodigoRota))
            {
                TmpRota = RepositoryService.Rota.ObterPor(objRota.CodigoRota);

                if (TmpRota != null)
                {
                    objRota.ID = TmpRota.ID;

                    RepositoryService.Rota.Update(objRota);

                    if (!TmpRota.State.Equals(objRota.State) && objRota.State != null)
                        this.MudarStatus(TmpRota.ID.Value, objRota.State.Value);

                    return TmpRota;
                }
                else
                {
                    objRota.ID = RepositoryService.Rota.Create(objRota);
                    return objRota;
                }
            }
            else
            {
                return null;
            }
        }

        public Rota BuscaRota(Guid itbc_rotaid)
        {
            Rota Rota = RepositoryService.Rota.ObterPor(itbc_rotaid);
            if (Rota != null)
                return Rota;
            return null;
        }

        public Rota BuscaRotaPorCodigo(String itbc_codigo_rota)
        {
            Rota Rota = RepositoryService.Rota.ObterPor(itbc_codigo_rota);
            if (Rota != null)
                return Rota;
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.Rota.AlterarStatus(id, status);
        }

        #endregion

    }
}
