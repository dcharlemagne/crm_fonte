using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.Configuration;
using SDKore.DomainModel;
using Microsoft.Xrm.Sdk;
using Intelbras.CRM2013.Domain.ViewModels;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class FaturaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FaturaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public FaturaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos Fatura

        /// <summary>
        /// Verifica se o status e state code existem
        /// </summary>
        /// <param name="status">Status</param>
        /// <param name="stateCode">Razão Status</param>
        /// <returns>Bool</returns>
        public bool verificarStatusStateCode(int status, int stateCode)
        {

            if (System.Enum.IsDefined(typeof(Enum.Fatura.Status), status))
            {
                return System.Enum.IsDefined(typeof(Enum.Fatura.RazaoStatus), stateCode);
            }

            return false;
        }

        public Fatura ObterPor(Guid FaturaId)
        {
            return RepositoryService.Fatura.Retrieve(FaturaId);
        }


        public bool MudarStatus(Guid id, int stateCode, int statusCode)
        {
            if (System.Enum.IsDefined(typeof(Enum.Fatura.Status), stateCode) && (System.Enum.IsDefined(typeof(Enum.Fatura.RazaoStatus), statusCode)))
                return RepositoryService.Fatura.AlterarStatus(id, stateCode, statusCode);

            return false;
        }

        public bool MudarProprietario(Guid proprietario, string TipoProprietario, Guid Fatura)
        {
            return RepositoryService.Fatura.AlterarProprietario(proprietario, TipoProprietario, Fatura);
        }

        public Fatura ObterFaturaPorPedidoEMS(string pedidoEMS)
        {
            return RepositoryService.Fatura.ObterPorPedidoEMS(pedidoEMS);
        }

        public Fatura ObterPorChaveIntergacao(String chaveIntegracao)
        {
            return RepositoryService.Fatura.ObterPorChaveIntegracao(chaveIntegracao);
        }

        public List<ProdutoFaturaViewModel> listarContagemVendaPrice(Guid canalId, List<Guid?> lstProdutos, List<Product> lstProductRef)
        {
            var listProdFat = RepositoryService.ProdutoFatura.listarContagemVendaPrice(canalId, lstProdutos);

            var listRet = new List<ProdutoFaturaViewModel>();
            foreach (var prodFat in listProdFat)
            {
                var viewModelRet = new ProdutoFaturaViewModel();
                var prodTmp = lstProductRef.Find(x => x.ID == prodFat.Produto.Id);

                viewModelRet.PrecoFatura = (decimal)prodFat.ValorLiquido;
                viewModelRet.QtdFatura = (decimal)prodFat.Quantidade;

                viewModelRet.CodigoProduto = prodTmp.Codigo;
                viewModelRet.DescProduto = prodTmp.Nome;

                var faturaTmp = RepositoryService.Fatura.ObterPor(prodFat.Fatura.Id);
                viewModelRet.CodigoFaturaEMS = faturaTmp.NumeroNF;
                viewModelRet.DataEmissaoFatura = (DateTime)faturaTmp.DataEmissao;

                listRet.Add(viewModelRet);
            }

            return listRet;
        }

        public Fatura Persistir(Fatura objFatura, ref bool alteraStatus)
        {
            Fatura TmpFatura = null;
            if (objFatura.ID.HasValue)
                TmpFatura = RepositoryService.Fatura.ObterPor(objFatura.ID.Value);
            //alterado pesquisa pela chave de integração - Sol. José - 14/08/2014
            if (TmpFatura == null && !String.IsNullOrEmpty(objFatura.ChaveIntegracao))
            {
                TmpFatura = RepositoryService.Fatura.ObterPorChaveIntegracao(objFatura.ChaveIntegracao);
            }
            if (TmpFatura != null)
            {
                //Para poder atualizar state posteriormente
                int? stateUpdate = objFatura.Status;
                int? razaoStatusUpdate = objFatura.RazaoStatus;
                objFatura.Status = TmpFatura.Status;
                objFatura.RazaoStatus = TmpFatura.RazaoStatus;

                objFatura.ID = TmpFatura.ID;
                RepositoryService.Fatura.Update(objFatura);

                //Retorna o state e razao do update
                objFatura.Status = stateUpdate;
                objFatura.RazaoStatus = razaoStatusUpdate;

                //Se statusCode for diferente do atual altera
                if (!TmpFatura.RazaoStatus.Equals(objFatura.RazaoStatus) && objFatura.RazaoStatus != null)
                    alteraStatus = true;
                return TmpFatura;
            }
            else
            {

                if (objFatura.Status == (int?)Enum.Fatura.Status.Cancelada)
                {
                    objFatura.Status = (int?)Enum.Fatura.Status.Ativa;
                    objFatura.RazaoStatus = (int?)Enum.Fatura.RazaoStatus.EntregaRealizadaNormalmente;
                    //Insere nova fatura com status ativo - caso seja enviada pra insert como cancelada.
                    objFatura.ID = RepositoryService.Fatura.Create(objFatura);
                    //Retorna para cancelado e solicita alteração de status
                    objFatura.Status = (int?)Enum.Fatura.Status.Cancelada;
                    objFatura.RazaoStatus = (int?)Enum.Fatura.RazaoStatus.Cancelada;
                    alteraStatus = true;
                }
                else
                    objFatura.ID = RepositoryService.Fatura.Create(objFatura);
                return objFatura;
            }
        }

        #endregion

    }
}
