using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetaDetalhadadoCanalporProdutoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetaDetalhadadoCanalporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetaDetalhadadoCanalporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }


        #endregion

        #region Métodos
        public void Criar(Model.MetadoCanalporProduto mMetadoCanalporProduto)
        {
            Model.MetaDetalhadadoCanalporProduto mMetaDetalhadadoCanalporProduto;

            switch (mMetadoCanalporProduto.Trimestre)
            {
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1:
                    #region 1º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1)))
                    {
                        mMetaDetalhadadoCanalporProduto = new MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoCanalporProduto.MetadoCanalporProduto = new Lookup(mMetadoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadoCanalporProduto>());
                        mMetaDetalhadadoCanalporProduto.Ano = mMetadoCanalporProduto.Ano;
                        mMetaDetalhadadoCanalporProduto.Canal = new Lookup(mMetadoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
                        mMetaDetalhadadoCanalporProduto.Mes = (int)mes;
                        mMetaDetalhadadoCanalporProduto.Trimestre = mMetadoCanalporProduto.Trimestre;
                        mMetaDetalhadadoCanalporProduto.Produto = new Lookup(mMetadoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Product>());
                        mMetaDetalhadadoCanalporProduto.Nome = mMetadoCanalporProduto.Nome;

                        RepositoryService.MetaDetalhadadoCanalporProduto.Create(mMetaDetalhadadoCanalporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2:
                    #region 2º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2)))
                    {
                        mMetaDetalhadadoCanalporProduto = new Model.MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoCanalporProduto.MetadoCanalporProduto = new Lookup(mMetadoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporProduto>());
                        mMetaDetalhadadoCanalporProduto.Ano = mMetadoCanalporProduto.Ano;
                        mMetaDetalhadadoCanalporProduto.Canal = new Lookup(mMetadoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mMetaDetalhadadoCanalporProduto.Mes = (int)mes;
                        mMetaDetalhadadoCanalporProduto.Trimestre = mMetadoCanalporProduto.Trimestre;
                        mMetaDetalhadadoCanalporProduto.Produto = new Lookup(mMetadoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaDetalhadadoCanalporProduto.Nome = mMetadoCanalporProduto.Nome;

                        RepositoryService.MetaDetalhadadoCanalporProduto.Create(mMetaDetalhadadoCanalporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3:
                    #region 3º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3)))
                    {
                        mMetaDetalhadadoCanalporProduto = new Model.MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoCanalporProduto.MetadoCanalporProduto = new Lookup(mMetadoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporProduto>());
                        mMetaDetalhadadoCanalporProduto.Ano = mMetadoCanalporProduto.Ano;
                        mMetaDetalhadadoCanalporProduto.Mes = (int)mes;
                        mMetaDetalhadadoCanalporProduto.Canal = new Lookup(mMetadoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mMetaDetalhadadoCanalporProduto.Trimestre = mMetadoCanalporProduto.Trimestre;
                        mMetaDetalhadadoCanalporProduto.Produto = new Lookup(mMetadoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaDetalhadadoCanalporProduto.Nome = mMetadoCanalporProduto.Nome;

                        RepositoryService.MetaDetalhadadoCanalporProduto.Create(mMetaDetalhadadoCanalporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4:
                    #region 4º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4)))
                    {
                        mMetaDetalhadadoCanalporProduto = new Model.MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaDetalhadadoCanalporProduto.MetadoCanalporProduto = new Lookup(mMetadoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporProduto>());
                        mMetaDetalhadadoCanalporProduto.Ano = mMetadoCanalporProduto.Ano;
                        mMetaDetalhadadoCanalporProduto.Mes = (int)mes;
                        mMetaDetalhadadoCanalporProduto.Canal = new Lookup(mMetadoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mMetaDetalhadadoCanalporProduto.Trimestre = mMetadoCanalporProduto.Trimestre;
                        mMetaDetalhadadoCanalporProduto.Produto = new Lookup(mMetadoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaDetalhadadoCanalporProduto.Nome = mMetadoCanalporProduto.Nome;

                        RepositoryService.MetaDetalhadadoCanalporProduto.Create(mMetaDetalhadadoCanalporProduto);
                    }
                    #endregion
                    break;
            }

        }

        public void Calcular(MetadoCanalporProduto mMetadoCanalporProduto, Trimestre trimestre, ref decimal valor, ref int quantidade)
        {
            decimal vlr = 0;
            int qtde = 0;

            List<MetaDetalhadadoCanalporProduto> lstMetaDetalhadadoCanalporProduto = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPor(mMetadoCanalporProduto.Canal.Id, mMetadoCanalporProduto.Produto.Id, trimestre.Id.Value);
            foreach (MetaDetalhadadoCanalporProduto prod in lstMetaDetalhadadoCanalporProduto)
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

                RepositoryService.MetaDetalhadadoCanalporProduto.Update(prod);
                vlr += prod.MetaPlanejada.Value;
                qtde += (int)prod.QtdePlanejada;
                #endregion
            }

            valor = vlr;
            quantidade = qtde;
        }

        public void CriarManual(MetadoCanal mMetadoCanal, Trimestre trimestre)
        {
            try
            {
                MetaDetalhadadoCanalporProduto mMetaDetalhadadoCanalporProduto;

                mMetaDetalhadadoCanalporProduto = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorManual(mMetadoCanal.Canal.Id, (int)trimestre.Mes1);

                if (mMetaDetalhadadoCanalporProduto == null)
                {
                    mMetaDetalhadadoCanalporProduto = new MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                }

                mMetaDetalhadadoCanalporProduto.Ano = mMetadoCanal.Ano;
                mMetaDetalhadadoCanalporProduto.Trimestre = (int)trimestre.trimestre;
                mMetaDetalhadadoCanalporProduto.Mes = (int)trimestre.Mes1;
                mMetaDetalhadadoCanalporProduto.MetaPlanejada = trimestre.Mes1Vlr.HasValue ? trimestre.Mes1Vlr.Value : 0;
                mMetaDetalhadadoCanalporProduto.Canal = new Lookup(mMetadoCanal.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
                mMetaDetalhadadoCanalporProduto.MetadoCanal = new Lookup(mMetadoCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<MetadoCanal>());
                mMetaDetalhadadoCanalporProduto.MetadoCanal = mMetadoCanal.UnidadedeNegocio;



                if (!mMetaDetalhadadoCanalporProduto.ID.HasValue)
                    RepositoryService.MetaDetalhadadoCanalporProduto.Create(mMetaDetalhadadoCanalporProduto);
                else
                    RepositoryService.MetaDetalhadadoCanalporProduto.Update(mMetaDetalhadadoCanalporProduto);

                mMetaDetalhadadoCanalporProduto = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorManual(mMetadoCanal.Canal.Id, (int)trimestre.Mes2);
                if (mMetaDetalhadadoCanalporProduto == null)
                    mMetaDetalhadadoCanalporProduto = new MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                mMetaDetalhadadoCanalporProduto.Ano = mMetadoCanal.Ano;
                mMetaDetalhadadoCanalporProduto.Trimestre = (int)trimestre.trimestre;
                mMetaDetalhadadoCanalporProduto.Mes = (int)trimestre.Mes2;
                mMetaDetalhadadoCanalporProduto.MetaPlanejada = trimestre.Mes2Vlr.HasValue ? trimestre.Mes2Vlr : 0;
                mMetaDetalhadadoCanalporProduto.Canal = new Lookup(mMetadoCanal.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
                mMetaDetalhadadoCanalporProduto.MetadoCanal = new Lookup(mMetadoCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<MetadoCanal>());
                mMetaDetalhadadoCanalporProduto.MetadoCanal = mMetadoCanal.UnidadedeNegocio;

                if (!mMetaDetalhadadoCanalporProduto.ID.HasValue)
                    RepositoryService.MetaDetalhadadoCanalporProduto.Create(mMetaDetalhadadoCanalporProduto);
                else
                    RepositoryService.MetaDetalhadadoCanalporProduto.Update(mMetaDetalhadadoCanalporProduto);

                mMetaDetalhadadoCanalporProduto = RepositoryService.MetaDetalhadadoCanalporProduto.ListarPorManual(mMetadoCanal.Canal.Id, (int)trimestre.Mes3);
                if (mMetaDetalhadadoCanalporProduto == null)
                    mMetaDetalhadadoCanalporProduto = new MetaDetalhadadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                mMetaDetalhadadoCanalporProduto.Ano = mMetadoCanal.Ano;
                mMetaDetalhadadoCanalporProduto.Trimestre = (int)trimestre.trimestre;
                mMetaDetalhadadoCanalporProduto.Mes = (int)trimestre.Mes3;
                mMetaDetalhadadoCanalporProduto.MetaPlanejada = trimestre.Mes3Vlr.HasValue ? trimestre.Mes3Vlr : 0;
                mMetaDetalhadadoCanalporProduto.Canal = new Lookup(mMetadoCanal.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
                mMetaDetalhadadoCanalporProduto.MetadoCanal = new Lookup(mMetadoCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<MetadoCanal>());
                mMetaDetalhadadoCanalporProduto.MetadoCanal = mMetadoCanal.UnidadedeNegocio;

                if (!mMetaDetalhadadoCanalporProduto.ID.HasValue)
                    RepositoryService.MetaDetalhadadoCanalporProduto.Create(mMetaDetalhadadoCanalporProduto);
                else
                    RepositoryService.MetaDetalhadadoCanalporProduto.Update(mMetaDetalhadadoCanalporProduto);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("(CRM) Ocorreu erro ao gerar Detalhe meta manual, contate o administrator.", ex);
            }
        }

        public void RetornoDWMetaCanalProdutoDetalhado(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanalProdDetalhado = RepositoryService.MetaDetalhadadoCanalporProduto.ListarMetaCanalDetalhadoProdutoDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanalProdDetalhado.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());
                Product mProduto = RepositoryService.Produto.ObterPor(item["CD_Item"].ToString());

                if (mUnidadeNegocio != null && mConta != null && mSubfamiliaProduto != null && mProduto != null)
                {
                    MetadoCanalporProduto mMetadoCanalporProduto = RepositoryService.MetadoCanalporProduto
                        .Obterpor(mUnidadeNegocio.ID.Value, mConta.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), trimestre, mSubfamiliaProduto.ID.Value, mProduto.ID.Value);

                    if (mMetadoCanalporProduto != null)
                    {
                        MetaDetalhadadoCanalporProduto mMetaDetalhadadoCanalporProduto = RepositoryService.MetaDetalhadadoCanalporProduto.Obter(Convert.ToInt32(item["cd_ano"].ToString()), Helper.TrimestreAtual()[1], Convert.ToInt32(item["cd_mes"].ToString()), mConta.ID.Value, mMetadoCanalporProduto.ID.Value, mProduto.ID.Value);

                        if (mMetaDetalhadadoCanalporProduto != null)
                        {
                            mMetaDetalhadadoCanalporProduto.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                            mMetaDetalhadadoCanalporProduto.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                            RepositoryService.MetaDetalhadadoCanalporProduto.Update(mMetaDetalhadadoCanalporProduto);
                        }
                    }
                }
            }
        }

        public void RetornoDWMetaCanalManualProdutoDetalhado(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanalProdDetalhado = RepositoryService.MetaDetalhadadoCanalporProduto.ListarMetaCanalManualDetalhadoProdutoDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanalProdDetalhado.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());

                if (mUnidadeNegocio != null && mConta != null)
                {
                    MetadoCanal mMetadoCanal = RepositoryService.MetadoCanal.ObterPor(mUnidadeNegocio.ID.Value, trimestre, mConta.ID.Value, ano);

                    if (mMetadoCanal != null)
                    {
                        MetaDetalhadadoCanalporProduto mMetaDetalhadadoCanalporProduto = RepositoryService.MetaDetalhadadoCanalporProduto
                            .ObterManual(trimestre, ano, mConta.ID.Value, mMetadoCanal.ID.Value, Convert.ToInt32(item["cd_mes"].ToString()), mUnidadeNegocio.ID.Value);

                        if (mMetaDetalhadadoCanalporProduto != null)
                        {
                            mMetaDetalhadadoCanalporProduto.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                            mMetaDetalhadadoCanalporProduto.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                            RepositoryService.MetaDetalhadadoCanalporProduto.Update(mMetaDetalhadadoCanalporProduto);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

