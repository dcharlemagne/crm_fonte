using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetadaUnidadeporSubfamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadaUnidadeporSubfamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadaUnidadeporSubfamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        MetadaUnidadeporProdutoService _ServiceMetadaUnidadeporProduto = null;
        MetadaUnidadeporProdutoService ServiceMetadaUnidadeporProduto
        {
            get
            {
                if (_ServiceMetadaUnidadeporProduto == null)
                    _ServiceMetadaUnidadeporProduto = new MetadaUnidadeporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadaUnidadeporProduto;
            }
        }

        #endregion

        #region Métodos
        public void Criar(Guid metaunidadeId, MetadaUnidadeporFamilia mMetadaUnidadeporFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid familiaId)
        {
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id));

            foreach (var OrcaSegSubFamilia in lstOrcamentoporSegSubFamilia)
            {
                MetadaUnidadeporSubfamilia mMetadaUnidadeporSubfamilia;
                mMetadaUnidadeporSubfamilia = RepositoryService.MetadaUnidadeporSubfamilia.Obter(OrcaSegSubFamilia.First().SubFamilia.Id, mMetadaUnidadeporFamilia.ID.Value);
                if (mMetadaUnidadeporSubfamilia == null)
                {
                    mMetadaUnidadeporSubfamilia = new MetadaUnidadeporSubfamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mMetadaUnidadeporSubfamilia.ID = Guid.NewGuid();
                    mMetadaUnidadeporSubfamilia.UnidadedeNegocio = mMetadaUnidadeporFamilia.UnidadedeNegocio;
                    mMetadaUnidadeporSubfamilia.Ano = mMetadaUnidadeporFamilia.Ano;
                    mMetadaUnidadeporSubfamilia.Trimestre = mMetadaUnidadeporFamilia.Trimestre;
                    mMetadaUnidadeporSubfamilia.Segmento = new Lookup(OrcaSegSubFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadaUnidadeporSubfamilia.Familia = new Lookup(OrcaSegSubFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mMetadaUnidadeporSubfamilia.Subfamilia = new Lookup(OrcaSegSubFamilia.First().SubFamilia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mMetadaUnidadeporSubfamilia.MetadaFamilia = new Lookup(mMetadaUnidadeporFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporFamilia>());
                    mMetadaUnidadeporSubfamilia.Nome = (mMetadaUnidadeporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Length > 99 ? (mMetadaUnidadeporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Substring(1, 99) : (mMetadaUnidadeporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name);

                    RepositoryService.MetadaUnidadeporSubfamilia.Create(mMetadaUnidadeporSubfamilia);
                }

                ServiceMetadaUnidadeporProduto.Criar(metaunidadeId, mMetadaUnidadeporSubfamilia, OrcaSegSubFamilia.ToList(), OrcaSegSubFamilia.First().SubFamilia.Id, mMetadaUnidadeporFamilia.MetadoSegmento.Id);

            }
        }

        public void RetornoDWMetaSubFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoSubFamilia = RepositoryService.MetadaUnidadeporSubfamilia.ListarMetaSubFamiliaDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtOrcamentoSubFamilia.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null && mFamiliaProduto != null && mSubfamiliaProduto != null)
                {
                    var itemcapa = RepositoryService.MetadaUnidadeporSubfamilia
                        .ObterMetaSubFamilia(mUnidadeNegocio.ID.Value, mSegmento.ID.Value, mFamiliaProduto.ID.Value, mSubfamiliaProduto.ID.Value, ano, trimestre);
                    
                    if (itemcapa != null)
                    {
                        itemcapa.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.MetadaUnidadeporSubfamilia.Update(itemcapa);
                    }
                }
            }
        }

        #endregion
    }
}

