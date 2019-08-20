using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetadoCanalporFamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadoCanalporFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadoCanalporFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }


        #endregion

        #region Propertys/Objetos

        MetadoCanalporSubFamiliaService _ServiceMetadoCanalporSubFamilia = null;
        private MetadoCanalporSubFamiliaService ServiceMetadoCanalporSubFamilia
        {
            get
            {
                if (_ServiceMetadoCanalporSubFamilia == null)
                    _ServiceMetadoCanalporSubFamilia = new MetadoCanalporSubFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadoCanalporSubFamilia;
            }
        }


        #endregion

        #region Método
        public void Criar(MetadoCanalporSegmento mMetadoCanalporSegmento, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            MetadoCanalporFamilia mMetadoCanalporFamilia;
            var lstorcamentoporsegfamilia = (from x in lstOrcamentoDetalhado
                                             group x by string.Format("{0}/{1}", x.Familia.Id, x.Segmento.Id));

            foreach (var OrcaSegFamilia in lstorcamentoporsegfamilia)
            {
                //mMetadoCanalporFamilia = RepositoryService.MetadoCanalporFamilia.Obterpor(OrcaSegFamilia.First().Canal.Id, mMetadoCanalporSegmento.ID.Value, OrcaSegFamilia.First().Segmento.Id, OrcaSegFamilia.First().Familia.Id, mMetadoCanalporSegmento.Trimestre.Value);
                mMetadoCanalporFamilia = RepositoryService.MetadoCanalporFamilia.Obterpor(OrcaSegFamilia.First().UnidadeNegocio.Id, OrcaSegFamilia.First().Canal.Id, mMetadoCanalporSegmento.Ano.Value, mMetadoCanalporSegmento.Trimestre.Value, OrcaSegFamilia.First().Segmento.Id, OrcaSegFamilia.First().Familia.Id);
                if (mMetadoCanalporFamilia == null)
                {
                    mMetadoCanalporFamilia = new MetadoCanalporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mMetadoCanalporFamilia.ID = Guid.NewGuid();
                    mMetadoCanalporFamilia.Canal = new Lookup(OrcaSegFamilia.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mMetadoCanalporFamilia.UnidadedeNegocio = mMetadoCanalporSegmento.UnidadedeNegocio;
                    mMetadoCanalporFamilia.Ano = mMetadoCanalporSegmento.Ano;
                    mMetadoCanalporFamilia.Trimestre = mMetadoCanalporSegmento.Trimestre;
                    mMetadoCanalporFamilia.Segmento = new Lookup(OrcaSegFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadoCanalporFamilia.Familia = new Lookup(OrcaSegFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mMetadoCanalporFamilia.MetadoCanalporSegmento = new Lookup(mMetadoCanalporSegmento.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporSegmento>());

                    mMetadoCanalporFamilia.Nome = (mMetadoCanalporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).ToString().Length > 99 ?
                        (mMetadoCanalporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).Substring(1, 99)
                        : (mMetadoCanalporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name);

                    RepositoryService.MetadoCanalporFamilia.Create(mMetadoCanalporFamilia);
                }
                ServiceMetadoCanalporSubFamilia.Criar(mMetadoCanalporFamilia, lstOrcamentoDetalhado);
            }
        }

        public void CalcularMeta(Guid metaunidadeId)
        {
            List<MetadoCanalporFamilia> lstMetadoCanalporFamilia = RepositoryService.MetadoCanalporFamilia.ListarPor(metaunidadeId);
            foreach (MetadoCanalporFamilia item in lstMetadoCanalporFamilia)
            {
                item.MetaPlanejada = 0;
                //item.OrcamentoParaNovosProdutos = 0;

                List<MetadoCanalporSubFamilia> lstSubFamilia = RepositoryService.MetadoCanalporSubFamilia.ListarPor(item.ID.Value);
                foreach (MetadoCanalporSubFamilia subfamilia in lstSubFamilia)
                {
                    item.MetaPlanejada += subfamilia.MetaPlanejada.HasValue ? subfamilia.MetaPlanejada.Value : 0;
                    //item.OrcamentoParaNovosProdutos += subfamilia.OrcamentoParaNovosProdutos.HasValue ? subfamilia.OrcamentoParaNovosProdutos.Value : 0;
                }

                RepositoryService.MetadoCanalporFamilia.Update(item);
            }
        }

        public void RetornoDWMetaCanalFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanalFamilia = RepositoryService.MetadoCanalporFamilia.ListarMetaCanalFamiliaDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanalFamilia.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());

                if (mUnidadeNegocio != null && mConta != null && mSegmento != null && mFamiliaProduto != null)
                {
                    MetadoCanalporFamilia mMetadoCanalporFamilia = RepositoryService.MetadoCanalporFamilia.Obterpor(mUnidadeNegocio.ID.Value, mConta.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value);

                    if (mMetadoCanalporFamilia != null)
                    {
                        mMetadoCanalporFamilia.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.MetadoCanalporFamilia.Update(mMetadoCanalporFamilia);
                    }
                }
            }
        }
        #endregion
    }
}

