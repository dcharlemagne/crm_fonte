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
    public class FamiliaProdutoService
    {
         
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FamiliaProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public FamiliaProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public FamiliaProduto BuscaFamiliaProduto(String codigoFamiliaProduto)
        {
            List<FamiliaProduto> lstFamiliaProduto = RepositoryService.FamiliaProduto.ListarPor(codigoFamiliaProduto);
            if (lstFamiliaProduto.Count > 0)
                return lstFamiliaProduto.First<FamiliaProduto>();
            return null;
        }

        public FamiliaProduto Persistir(FamiliaProduto familiaProduto)
        {
            List<FamiliaProduto> TmpFamiliaProduto = RepositoryService.FamiliaProduto.ListarPor(familiaProduto.Codigo);

            if (TmpFamiliaProduto.Count() > 0)
            {
                familiaProduto.ID = TmpFamiliaProduto.First<FamiliaProduto>().ID;
                RepositoryService.FamiliaProduto.Update(familiaProduto);

                if (!TmpFamiliaProduto.First<FamiliaProduto>().Status.Equals(familiaProduto.Status) && familiaProduto.Status != null)
                    this.MudarStatus(TmpFamiliaProduto.First<FamiliaProduto>().ID.Value, familiaProduto.Status.Value);

                return TmpFamiliaProduto.First<FamiliaProduto>();
            }
            else
                familiaProduto.ID = RepositoryService.FamiliaProduto.Create(familiaProduto);
            return familiaProduto;
        }

        public FamiliaProduto ObterPor(Guid familiaProdId)
        {
            return RepositoryService.FamiliaProduto.ObterPor(familiaProdId);
        }

        public List<FamiliaProduto> ListarPorSegmento(Guid segmentoId, bool filtrarCanaisVerdes, Guid? canalId, string[] notInList)
        {
            return RepositoryService.FamiliaProduto.ListarPorSegmento(segmentoId, filtrarCanaisVerdes, canalId, notInList);
        }

        public bool MudarStatus(Guid id, int status)
        {
            return RepositoryService.FamiliaProduto.AlterarStatus(id, status);
        }

    }
}
