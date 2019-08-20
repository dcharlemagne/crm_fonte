using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetadaUnidadeporFamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadaUnidadeporFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadaUnidadeporFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Propertys/Objetos
        MetadaUnidadeporSubfamiliaService _ServiceMetadaUnidadeporSubfamilia = null;
        MetadaUnidadeporSubfamiliaService ServiceMetadaUnidadeporSubfamilia
        {
            get
            {
                if (_ServiceMetadaUnidadeporSubfamilia == null)
                    _ServiceMetadaUnidadeporSubfamilia = new MetadaUnidadeporSubfamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadaUnidadeporSubfamilia;
            }

        }

        #endregion

        #region Métodos
        public void Criar(Guid metaunidadeId, MetadaUnidadeporSegmento mMetadaUnidadeporSegmento, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid SegmentoId)
        {
            var lstorcamentoporsegfamilia = (from x in lstOrcamentoDetalhado
                                             group x by string.Format("{0}/{1}", x.Segmento.Id, x.Familia.Id));

            foreach (var OrcaSegFamilia in lstorcamentoporsegfamilia)
            {
                MetadaUnidadeporFamilia mMetadaUnidadeporFamilia;
                mMetadaUnidadeporFamilia = RepositoryService.MetadaUnidadeporFamilia.Obter(OrcaSegFamilia.First().Familia.Id, mMetadaUnidadeporSegmento.ID.Value);
                if (mMetadaUnidadeporFamilia == null)
                {
                    mMetadaUnidadeporFamilia = new MetadaUnidadeporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mMetadaUnidadeporFamilia.ID = Guid.NewGuid();
                    mMetadaUnidadeporFamilia.UnidadedeNegocio = mMetadaUnidadeporSegmento.UnidadedeNegocios;
                    mMetadaUnidadeporFamilia.Ano = mMetadaUnidadeporSegmento.Ano;
                    mMetadaUnidadeporFamilia.Trimestre = mMetadaUnidadeporSegmento.Trimestre;
                    mMetadaUnidadeporFamilia.Segmento = new Lookup(OrcaSegFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadaUnidadeporFamilia.Familia = new Lookup(OrcaSegFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mMetadaUnidadeporFamilia.MetadoSegmento = new Lookup(mMetadaUnidadeporSegmento.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporSegmento>());
                    mMetadaUnidadeporFamilia.Nome = (mMetadaUnidadeporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).ToString().Length > 99 ?
                        (mMetadaUnidadeporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).Substring(1, 99)
                        : (mMetadaUnidadeporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name);

                    RepositoryService.MetadaUnidadeporFamilia.Create(mMetadaUnidadeporFamilia);
                }

                ServiceMetadaUnidadeporSubfamilia.Criar(metaunidadeId, mMetadaUnidadeporFamilia, OrcaSegFamilia.ToList(), OrcaSegFamilia.First().Familia.Id);
            }

        }

        public void     CalcularMeta(Guid metaunidadeId)
        {
            List<MetadaUnidadeporFamilia> lstMetadaUnidadeporFamilia = RepositoryService.MetadaUnidadeporFamilia.ObterFamiliaporMeta(metaunidadeId);
            foreach (MetadaUnidadeporFamilia item in lstMetadaUnidadeporFamilia)
            {
                item.MetaPlanejada = 0;

                List<MetadaUnidadeporSubfamilia> lstSubFamilia = RepositoryService.MetadaUnidadeporSubfamilia.ObterSubFamiliaPor(item.ID.Value);
                foreach (MetadaUnidadeporSubfamilia subfamilia in lstSubFamilia)
                {
                    item.MetaPlanejada += subfamilia.MetaPlanejada.HasValue ? subfamilia.MetaPlanejada.Value : 0;
                }

                RepositoryService.MetadaUnidadeporFamilia.Update(item);
            }
        }

        public void RetornoDWMetaFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoFamilia = RepositoryService.MetadaUnidadeporFamilia.ListarMetaFamiliaDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtOrcamentoFamilia.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null && mFamiliaProduto != null)
                {
                    var itemcapa = RepositoryService.MetadaUnidadeporFamilia.ObterMetaFamilia(mUnidadeNegocio.ID.Value, mSegmento.ID.Value, mFamiliaProduto.ID.Value, ano, trimestre);
                    if (itemcapa != null)
                    {
                        itemcapa.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.MetadaUnidadeporFamilia.Update(itemcapa);
                    }
                }
            }
        }
        #endregion
    }
}

