using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetadoCanalporSubFamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadoCanalporSubFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadoCanalporSubFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Propertys/Objetos
        MetadoCanalporProdutoService _ServiceMetadoCanalporProduto = null;
        private MetadoCanalporProdutoService ServiceMetadoCanalporProduto
        {
            get
            {
                if (_ServiceMetadoCanalporProduto == null)
                    _ServiceMetadoCanalporProduto = new MetadoCanalporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadoCanalporProduto;
            }
        }

        #endregion

        #region Método
        public void Criar(MetadoCanalporFamilia mMetadoCanalporFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            MetadoCanalporSubFamilia mMetadoCanalporSubFamilia;
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id));

            foreach (var OrcaSegSubFamilia in lstOrcamentoporSegSubFamilia)
            {
                //mMetadoCanalporSubFamilia = RepositoryService.MetadoCanalporSubFamilia.Obterpor(OrcaSegSubFamilia.First().Canal.Id, OrcaSegSubFamilia.First().Familia.Id, mMetadoCanalporFamilia.ID.Value, OrcaSegSubFamilia.First().Segmento.Id, OrcaSegSubFamilia.First().SubFamilia.Id, mMetadoCanalporFamilia.Trimestre.Value);
                mMetadoCanalporSubFamilia = RepositoryService.MetadoCanalporSubFamilia.Obterpor(OrcaSegSubFamilia.First().UnidadeNegocio.Id, OrcaSegSubFamilia.First().Canal.Id, mMetadoCanalporFamilia.Ano.Value, mMetadoCanalporFamilia.Trimestre.Value, OrcaSegSubFamilia.First().Segmento.Id, OrcaSegSubFamilia.First().Familia.Id, OrcaSegSubFamilia.First().SubFamilia.Id);
                if (mMetadoCanalporSubFamilia == null)
                {
                    mMetadoCanalporSubFamilia = new MetadoCanalporSubFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mMetadoCanalporSubFamilia.ID = Guid.NewGuid();
                    mMetadoCanalporSubFamilia.UnidadedeNegocio = mMetadoCanalporFamilia.UnidadedeNegocio;
                    mMetadoCanalporSubFamilia.Ano = mMetadoCanalporFamilia.Ano;
                    mMetadoCanalporSubFamilia.Trimestre = mMetadoCanalporFamilia.Trimestre;
                    mMetadoCanalporSubFamilia.Segmento = new Lookup(OrcaSegSubFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadoCanalporSubFamilia.Familia = new Lookup(OrcaSegSubFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mMetadoCanalporSubFamilia.SubFamilia = new Lookup(OrcaSegSubFamilia.First().SubFamilia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mMetadoCanalporSubFamilia.MetadoCanalporFamilia = new Lookup(mMetadoCanalporFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadoCanalporFamilia>());
                    mMetadoCanalporSubFamilia.Nome = (mMetadoCanalporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Length > 99 ? (mMetadoCanalporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Substring(1, 99) : (mMetadoCanalporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name);
                    mMetadoCanalporSubFamilia.Canal = new Lookup(OrcaSegSubFamilia.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    RepositoryService.MetadoCanalporSubFamilia.Create(mMetadoCanalporSubFamilia);
                }
                //ServiceOrcamentodoCanalporProduto.Criar(mMetadoCanalporSubFamilia, OrcaSegSubFamilia.ToList(), OrcaSegSubFamilia.First().SubFamilia.Id, canalId);
                ServiceMetadoCanalporProduto.Criar(mMetadoCanalporSubFamilia, lstOrcamentoDetalhado);
            }
        }

        public void RetornoDWMetaCanalSubFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanalSubFamilia = RepositoryService.MetadoCanalporSubFamilia.ListarMetaCanalSubFamiliaDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanalSubFamilia.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null && mFamiliaProduto != null && mConta != null && mSubfamiliaProduto != null)
                {
                    MetadoCanalporSubFamilia mMetadoCanalporSubFamilia = RepositoryService.MetadoCanalporSubFamilia
                        .Obterpor(mUnidadeNegocio.ID.Value, mConta.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value, mSubfamiliaProduto.ID.Value);
                    
                    if (mMetadoCanalporSubFamilia != null)
                    {
                        mMetadoCanalporSubFamilia.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.MetadoCanalporSubFamilia.Update(mMetadoCanalporSubFamilia);
                    }
                }
            }
        }

        #endregion
    }
}

