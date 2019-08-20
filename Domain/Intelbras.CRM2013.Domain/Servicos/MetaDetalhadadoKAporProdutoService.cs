using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Collections.Generic;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MetaDetalhadadoKAporProdutoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public MetaDetalhadadoKAporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetaDetalhadadoKAporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public void Criar(Model.PotencialdoKAporProduto mPotencialdoKAporProduto)
        {
            Model.MetaDetalhadadoKAporProduto mMetaDetalhadadoKAporProduto;

            switch (mPotencialdoKAporProduto.Trimestre)
            {
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1:
                    #region 1º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1)))
                    {
                        mMetaDetalhadadoKAporProduto = new Model.MetaDetalhadadoKAporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoKAporProduto.MetadoKAporProduto = new Lookup(mPotencialdoKAporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporProduto>());
                        mMetaDetalhadadoKAporProduto.Ano = mPotencialdoKAporProduto.Ano;
                        mMetaDetalhadadoKAporProduto.KAouRepresentante = new Lookup(mPotencialdoKAporProduto.KAouRepresentante.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());
                        mMetaDetalhadadoKAporProduto.Mes = (int)mes;
                        mMetaDetalhadadoKAporProduto.Trimestre = mPotencialdoKAporProduto.Trimestre;
                        mMetaDetalhadadoKAporProduto.Produto = new Lookup(mPotencialdoKAporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaDetalhadadoKAporProduto.Nome = mPotencialdoKAporProduto.Nome;

                        RepositoryService.MetaDetalhadadoKAporProduto.Create(mMetaDetalhadadoKAporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2:
                    #region 2º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2)))
                    {
                        mMetaDetalhadadoKAporProduto = new Model.MetaDetalhadadoKAporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoKAporProduto.MetadoKAporProduto = new Lookup(mPotencialdoKAporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporProduto>());
                        mMetaDetalhadadoKAporProduto.Ano = mPotencialdoKAporProduto.Ano;
                        mMetaDetalhadadoKAporProduto.KAouRepresentante = new Lookup(mPotencialdoKAporProduto.KAouRepresentante.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());
                        mMetaDetalhadadoKAporProduto.Mes = (int)mes;
                        mMetaDetalhadadoKAporProduto.Trimestre = mPotencialdoKAporProduto.Trimestre;
                        mMetaDetalhadadoKAporProduto.Produto = new Lookup(mPotencialdoKAporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaDetalhadadoKAporProduto.Nome = mPotencialdoKAporProduto.Nome;

                        RepositoryService.MetaDetalhadadoKAporProduto.Create(mMetaDetalhadadoKAporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3:
                    #region 3º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3)))
                    {
                        mMetaDetalhadadoKAporProduto = new Model.MetaDetalhadadoKAporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoKAporProduto.MetadoKAporProduto = new Lookup(mPotencialdoKAporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporProduto>());
                        mMetaDetalhadadoKAporProduto.Ano = mPotencialdoKAporProduto.Ano;
                        mMetaDetalhadadoKAporProduto.Mes = (int)mes;
                        mMetaDetalhadadoKAporProduto.KAouRepresentante = new Lookup(mPotencialdoKAporProduto.KAouRepresentante.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());
                        mMetaDetalhadadoKAporProduto.Trimestre = mPotencialdoKAporProduto.Trimestre;
                        mMetaDetalhadadoKAporProduto.Produto = new Lookup(mPotencialdoKAporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaDetalhadadoKAporProduto.Nome = mPotencialdoKAporProduto.Nome;

                        RepositoryService.MetaDetalhadadoKAporProduto.Create(mMetaDetalhadadoKAporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4:
                    #region 4º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4)))
                    {
                        mMetaDetalhadadoKAporProduto = new Model.MetaDetalhadadoKAporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoKAporProduto.MetadoKAporProduto = new Lookup(mPotencialdoKAporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporProduto>());
                        mMetaDetalhadadoKAporProduto.Ano = mPotencialdoKAporProduto.Ano;
                        mMetaDetalhadadoKAporProduto.Mes = (int)mes;
                        mMetaDetalhadadoKAporProduto.KAouRepresentante = new Lookup(mPotencialdoKAporProduto.KAouRepresentante.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());
                        mMetaDetalhadadoKAporProduto.Trimestre = mPotencialdoKAporProduto.Trimestre;
                        mMetaDetalhadadoKAporProduto.Produto = new Lookup(mPotencialdoKAporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaDetalhadadoKAporProduto.Nome = mPotencialdoKAporProduto.Nome;

                        RepositoryService.MetaDetalhadadoKAporProduto.Create(mMetaDetalhadadoKAporProduto);
                    }
                    #endregion
                    break;
            }

        }

        public void Calcular(PotencialdoKAporProduto mPotencialdoKAporProduto, Trimestre trimestre, ref decimal valor, ref int quantidade)
        {
            decimal vlr = 0;
            int qtde = 0;

            List<MetaDetalhadadoKAporProduto> lstMetaDetalhadadoKAporProduto = RepositoryService.MetaDetalhadadoKAporProduto.ListarPor(mPotencialdoKAporProduto.ID.Value);
            foreach (MetaDetalhadadoKAporProduto prod in lstMetaDetalhadadoKAporProduto)
            {
                #region
                if (prod.Mes == trimestre.Mes1)
                {
                    prod.MetaPlanejada = trimestre.Mes1Vlr.HasValue ? trimestre.Mes1Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes1Qtde.HasValue ? trimestre.Mes1Qtde.Value : 0;
                }
                else if (prod.Mes == trimestre.Mes2)
                {
                    prod.MetaPlanejada = trimestre.Mes2Vlr.HasValue ? trimestre.Mes2Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes2Qtde.HasValue ? trimestre.Mes2Qtde.Value : 0;
                }
                else if (prod.Mes == trimestre.Mes3)
                {
                    prod.MetaPlanejada = trimestre.Mes3Vlr.HasValue ? trimestre.Mes3Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes3Qtde.HasValue ? trimestre.Mes3Qtde.Value : 0;
                }

                RepositoryService.MetaDetalhadadoKAporProduto.Update(prod);
                vlr += prod.MetaPlanejada.Value;
                qtde += (int)prod.QtdePlanejada;
                #endregion
            }

            valor = vlr;
            quantidade = qtde;
        }

        #endregion
    }
}