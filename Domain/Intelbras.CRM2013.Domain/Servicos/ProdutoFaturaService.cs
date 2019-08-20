using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutoFaturaService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoFaturaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public ProdutoFaturaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion


        public ProdutoFatura Atualizar(ProdutoFatura ObjProdutoFatura, ref bool MudancaProprietario)
        {
            ProdutoFatura TmpProdutoFatura = null;

            if (!String.IsNullOrEmpty(ObjProdutoFatura.ChaveIntegracao))//ID.HasValue)
            {
                TmpProdutoFatura = RepositoryService.ProdutoFatura.ObterPorChaveIntegracao(ObjProdutoFatura.ChaveIntegracao);//ObterPor(ObjProdutoFatura.ID.Value);

                if (TmpProdutoFatura != null)
                {
                    ObjProdutoFatura.ID = TmpProdutoFatura.ID;
                    RepositoryService.ProdutoFatura.Update(ObjProdutoFatura);

                    return ObjProdutoFatura;
                }
                else
                {
                    ObjProdutoFatura.ID = RepositoryService.ProdutoFatura.Create(ObjProdutoFatura);
                    return ObjProdutoFatura;
                }
            }
            else
            {
                ObjProdutoFatura.ID = RepositoryService.ProdutoFatura.Create(ObjProdutoFatura);
                return ObjProdutoFatura;
            }
        }

        public void Deletar(ProdutoFatura ObjProdutoFatura)
        {
            RepositoryService.ProdutoFatura.Delete(ObjProdutoFatura.ID.Value);
        }

        public bool MudarProprietario(Guid proprietario, string TipoProprietario, Guid produtoFaturaId)
        {
            return RepositoryService.ProdutoFatura.AlterarProprietario(proprietario, TipoProprietario, produtoFaturaId);
        }

        public ProdutoFatura ObterPorProdutoEfatura(Guid productid, Guid invoiceid)
        {
            return RepositoryService.ProdutoFatura.ObterPor(productid, invoiceid);
        }

    }
}


