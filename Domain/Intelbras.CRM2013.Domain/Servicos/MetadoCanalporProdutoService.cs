using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetadoCanalporProdutoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadoCanalporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadoCanalporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }


        #endregion

        #region Propertys/Objetos
        MetaDetalhadadoCanalporProdutoService _ServiceMetaDetalhadadoCanalporProduto = null;
        private MetaDetalhadadoCanalporProdutoService ServiceMetaDetalhadadoCanalporProduto
        {
            get
            {
                if (_ServiceMetaDetalhadadoCanalporProduto == null)
                    _ServiceMetaDetalhadadoCanalporProduto = new MetaDetalhadadoCanalporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetaDetalhadadoCanalporProduto;
            }
        }
        #endregion

        #region Métodos
        public void Criar(MetadoCanalporSubFamilia mMetadoCanalporSubFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            MetadoCanalporProduto mMetadoCanalporProduto;
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}/{3}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id, x.Produto.Id));

            foreach (var OrcaProduto in lstOrcamentoporSegSubFamilia)
            {
                if (OrcaProduto.First().Produto.Id != Guid.Empty)
                {
                    mMetadoCanalporProduto = RepositoryService.MetadoCanalporProduto.Obter(OrcaProduto.First().Canal.Id, OrcaProduto.First().Produto.Id, OrcaProduto.First().SubFamilia.Id, mMetadoCanalporSubFamilia.ID.Value, (int)mMetadoCanalporSubFamilia.Ano.Value, (int)mMetadoCanalporSubFamilia.Trimestre);

                    if (mMetadoCanalporProduto == null)
                    {
                        mMetadoCanalporProduto = new MetadoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        mMetadoCanalporProduto.ID = Guid.NewGuid();
                        mMetadoCanalporProduto.Ano = mMetadoCanalporSubFamilia.Ano;
                        mMetadoCanalporProduto.Trimestre = mMetadoCanalporSubFamilia.Trimestre;
                        mMetadoCanalporProduto.Produto = new Lookup(OrcaProduto.First().Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetadoCanalporProduto.MetadoCanalporSubFamilia = new Lookup(mMetadoCanalporSubFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadoCanalporSubFamilia>());
                        mMetadoCanalporProduto.Nome = OrcaProduto.First().Produto.Name;
                        mMetadoCanalporProduto.Canal = new Lookup(OrcaProduto.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());

                        RepositoryService.MetadoCanalporProduto.Create(mMetadoCanalporProduto);
                        ServiceMetaDetalhadadoCanalporProduto.Criar(mMetadoCanalporProduto);
                    }
                }
            }
        }

        public void Atualizar(OrcamentoDetalhado mMetaDetalhado)
        {
            decimal valor = 0;
            int quantidade = 0;
            MetadoCanalporProduto mMetadoCanalporProduto1;
            MetadoCanalporProduto mMetadoCanalporProduto2;
            MetadoCanalporProduto mMetadoCanalporProduto3;
            MetadoCanalporProduto mMetadoCanalporProduto4;

            if (mMetaDetalhado.AtualizarTrimestre1)
            {
                mMetadoCanalporProduto1 = RepositoryService.MetadoCanalporProduto.Obterpor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre1.Id.Value);
                if (mMetadoCanalporProduto1 != null)
                {
                    mMetaDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceMetaDetalhadadoCanalporProduto.Calcular(mMetadoCanalporProduto1, mMetaDetalhado.Trimestre1, ref valor, ref quantidade);
                    mMetadoCanalporProduto1.MetaPlanejada = valor;
                    RepositoryService.MetadoCanalporProduto.Update(mMetadoCanalporProduto1);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre2)
            {
                mMetadoCanalporProduto2 = RepositoryService.MetadoCanalporProduto.Obterpor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre2.Id.Value);
                if (mMetadoCanalporProduto2 != null)
                {
                    mMetaDetalhado.Trimestre2.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                    ServiceMetaDetalhadadoCanalporProduto.Calcular(mMetadoCanalporProduto2, mMetaDetalhado.Trimestre2, ref valor, ref quantidade);
                    mMetadoCanalporProduto2.MetaPlanejada = valor;
                    RepositoryService.MetadoCanalporProduto.Update(mMetadoCanalporProduto2);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre3)
            {
                mMetadoCanalporProduto3 = RepositoryService.MetadoCanalporProduto.Obterpor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre3.Id.Value);
                if (mMetadoCanalporProduto3 != null)
                {
                    mMetaDetalhado.Trimestre3.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                    ServiceMetaDetalhadadoCanalporProduto.Calcular(mMetadoCanalporProduto3, mMetaDetalhado.Trimestre3, ref valor, ref quantidade);
                    mMetadoCanalporProduto3.MetaPlanejada = valor;
                    RepositoryService.MetadoCanalporProduto.Update(mMetadoCanalporProduto3);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre4)
            {
                mMetadoCanalporProduto4 = RepositoryService.MetadoCanalporProduto.Obterpor(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre4.Id.Value);
                if (mMetadoCanalporProduto4 != null)
                {
                    mMetaDetalhado.Trimestre4.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                    ServiceMetaDetalhadadoCanalporProduto.Calcular(mMetadoCanalporProduto4, mMetaDetalhado.Trimestre4, ref valor, ref quantidade);
                    mMetadoCanalporProduto4.MetaPlanejada = valor;
                    RepositoryService.MetadoCanalporProduto.Update(mMetadoCanalporProduto4);
                }
            }
        }

        public void RetornoDWMetaCanalProduto(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanalProduto = RepositoryService.MetadoCanalporProduto.ListarMetaCanalProdutoDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanalProduto.Rows)
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
                        mMetadoCanalporProduto.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                                                RepositoryService.MetadoCanalporProduto.Update(mMetadoCanalporProduto);
                    }
                }
            }
        }

        #endregion
    }
}