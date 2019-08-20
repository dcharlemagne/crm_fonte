using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intelbras.CRM2013.Domain.IRepository;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class FamiliaComercialService
    {

        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public FamiliaComercialService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public FamiliaComercialService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        public List<Product> ObterPor(string PoliticaComercialId, string codigoInicial, string codigoFinal)
        {
            List<FamiliaProduto> lstFamiliaProdutos = RepositoryService.FamiliaProduto.ListarPor(codigoInicial,codigoFinal);

            List<Guid> FamiliasProduto = new List<Guid>();
            foreach (FamiliaProduto _ProdutosEstab in lstFamiliaProdutos)
            {
                FamiliasProduto.Add(_ProdutosEstab.ID.Value);
            }


            List<ProdutoEstabelecimento> lstProdutosEstab = RepositoryService.ProdutoEstabelecimento.ListarPorEstabelecimento(RepositoryService.PoliticaComercial.Retrieve(new Guid(PoliticaComercialId)).Estabelecimento.Id);
            List<Guid> ProdutosDoEstab = new List<Guid>();
            
            foreach (ProdutoEstabelecimento _ProdutosEstab in lstProdutosEstab)
            {
                ProdutosDoEstab.Add(_ProdutosEstab.Produto.Id);
            }

            
            //List<string> CodigoProdutos = new List<string>();

            //foreach (ProdutoEstabelecimento _ProdutosEstab in lstProdutosEstab)
            //{
            //    CodigoProdutos.Add(_ProdutosEstab.Produto.Id.ToString());
            //}

            //CodigoProdutos.Distinct<string>();
            if (FamiliasProduto.Count == 0 || ProdutosDoEstab.Count == 0)
                return new List<Product>();

            List<Product> produtos = RepositoryService.Produto.ListarPor(FamiliasProduto, ProdutosDoEstab);


            return produtos;

        }

        public FamiliaComercial ObterPor(Guid familiacomercialId)
        {
            return RepositoryService.FamiliaComercial.ObterPor(familiacomercialId);
        }

        public FamiliaComercial ObterPor(string codigoFamiliaComercial)
        {
            return RepositoryService.FamiliaComercial.ObterPor(codigoFamiliaComercial);
        }

        public List<Product> ProdutosPorFamilias(string familiasIds)
        {
            string[] arrIdFamilia = familiasIds.Split('|');
            List<Product> produtos = new List<Product>();

            foreach (string idFamilia in arrIdFamilia)
            {
                if (idFamilia != string.Empty)
                    produtos.AddRange(RepositoryService.Produto.ListarPor(new Guid(idFamilia)));
                    //produtos.AddRange(RepositoryService.Produto.ListarPor(null, null, null, new Guid(idFamilia)));
            }

            return produtos;
        }

        public FamiliaComercial Persistir(FamiliaComercial familiaComercial)
        {

            List<Model.FamiliaComercial> TmpFamiliaComercial = RepositoryService.FamiliaComercial.ListarPor(familiaComercial.Codigo);

            if (TmpFamiliaComercial.Count() > 0)
            {
                familiaComercial.ID = TmpFamiliaComercial.First<FamiliaComercial>().ID;
                RepositoryService.FamiliaComercial.Update(familiaComercial);
                if (!TmpFamiliaComercial.First<FamiliaComercial>().Status.Equals(familiaComercial.Status) && familiaComercial.Status != null)
                    this.MudarStatus(TmpFamiliaComercial.First<FamiliaComercial>().ID.Value, familiaComercial.Status.Value);

                return TmpFamiliaComercial.First<FamiliaComercial>();
            }
            else
                familiaComercial.ID = RepositoryService.FamiliaComercial.Create(familiaComercial);
            return familiaComercial;
        }

        public bool MudarStatus(Guid id, int stateCode)
        {
            return RepositoryService.FamiliaComercial.AlterarStatus(id, stateCode);
        }
    }
}
