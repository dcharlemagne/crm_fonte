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
    public class CanalDeVendaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public CanalDeVendaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public CanalDeVendaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos

        public CanaldeVenda Persistir(Model.CanaldeVenda objCanaldeVenda, ref bool mudancaProprietario)
        {
            CanaldeVenda TmpCanaldeVenda = null;
            if (objCanaldeVenda.CodigoVenda.HasValue)
            {
                TmpCanaldeVenda = RepositoryService.CanaldeVenda.ObterPor(objCanaldeVenda.CodigoVenda.Value);

                if (TmpCanaldeVenda != null)
                {
                    objCanaldeVenda.ID = TmpCanaldeVenda.ID;

                    RepositoryService.CanaldeVenda.Update(objCanaldeVenda);

                    //Altera Status - Se necessário
                    if (objCanaldeVenda.Status.HasValue && !TmpCanaldeVenda.Status.Equals(objCanaldeVenda.Status))
                        this.MudarStatus(TmpCanaldeVenda.ID.Value, objCanaldeVenda.Status.Value);

                    return TmpCanaldeVenda;
                }
                else
                {
                    objCanaldeVenda.ID = RepositoryService.CanaldeVenda.Create(objCanaldeVenda);
                    return objCanaldeVenda;
                }
            }
            else
            {
                return null;
            }
        }

        public CanaldeVenda BuscaCanalDeVenda(Guid itbc_canaldevendaid)
        {
            List<CanaldeVenda> lstCanaldeVenda = RepositoryService.CanaldeVenda.ListarPor(itbc_canaldevendaid);
            if (lstCanaldeVenda.Count > 0)
                return lstCanaldeVenda.First<CanaldeVenda>();
            return null;
        }

        public CanaldeVenda BuscaCanalDeVendaPorCodigoVenda(int itbc_codigo_venda)
        {
            CanaldeVenda canaldeVenda = RepositoryService.CanaldeVenda.ObterPor(itbc_codigo_venda);
            if (canaldeVenda != null)
                return canaldeVenda;
            return null;
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.CanaldeVenda.AlterarStatus(id, status);
        }

        #endregion
    }
}
