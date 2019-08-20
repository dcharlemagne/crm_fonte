using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class ProdutoCondicaoPagamentoService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public ProdutoCondicaoPagamentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public ProdutoCondicaoPagamentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        public ProdutoCondicaoPagamento ObterPor(Guid id)
        {
            return RepositoryService.ProdutoCondicaoPagamento.Retrieve(id);
        }

        #endregion

        public ProdutoCondicaoPagamento Persistir(Model.ProdutoCondicaoPagamento objProdCondPag)
        {
            ProdutoCondicaoPagamento TmpProdEsta = null;
            if (objProdCondPag.ID.HasValue)
            {
                TmpProdEsta = RepositoryService.ProdutoCondicaoPagamento.Retrieve(objProdCondPag.ID.Value);

                if (TmpProdEsta != null)
                {
                    objProdCondPag.ID = TmpProdEsta.ID;
                    RepositoryService.ProdutoCondicaoPagamento.Update(objProdCondPag);
                    //Altera Status - Se necessário
                    if (!TmpProdEsta.Status.Equals(objProdCondPag.Status) && objProdCondPag.Status != null)
                        this.MudarStatus(TmpProdEsta.ID.Value, objProdCondPag.Status.Value);
                    return TmpProdEsta;
                }
                else
                    return null;
            }
            else
            {
                objProdCondPag.ID = RepositoryService.ProdutoCondicaoPagamento.Create(objProdCondPag);
                return objProdCondPag;
            }
        }

        public void Excluir(Guid prodEstabId)
        {
            RepositoryService.ProdutoCondicaoPagamento.Delete(prodEstabId);
        }

        public void MudarStatus(Guid id, int status)
        {
            RepositoryService.ProdutoCondicaoPagamento.AlterarStatus(id, status);
        }

        public string IntegracaoBarramento(ProdutoCondicaoPagamento objProdCondPag)
        {
            Domain.Integracao.MSG0287 msgProdEstab = new Domain.Integracao.MSG0287(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            return msgProdEstab.Enviar(objProdCondPag);
        }


    }
}