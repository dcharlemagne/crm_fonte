using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MetadaUnidadeporSegmentoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadaUnidadeporSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadaUnidadeporSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region objetos/propertys
        MetadaUnidadeporFamiliaService _ServiceMetadaUnidadeporFamilia = null;
        MetadaUnidadeporFamiliaService ServiceMetadaUnidadeporFamilia
        {
            get
            {
                if (_ServiceMetadaUnidadeporFamilia == null)
                    _ServiceMetadaUnidadeporFamilia = new MetadaUnidadeporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadaUnidadeporFamilia;
            }
        }
        #endregion

        #region Métodos
        public void Criar(MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Segmento.Id));

            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                MetadaUnidadeporSegmento mMetadaUnidadeporSegmento;
                mMetadaUnidadeporSegmento = RepositoryService.MetadaUnidadeporSegmento.Obter(OrcaSegmento.First().Segmento.Id, mMetadaUnidadeporTrimestre.ID.Value);
                if (mMetadaUnidadeporSegmento == null)
                {
                    mMetadaUnidadeporSegmento = new MetadaUnidadeporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mMetadaUnidadeporSegmento.ID = Guid.NewGuid();
                    mMetadaUnidadeporSegmento.Nome = mMetadaUnidadeporTrimestre.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                    mMetadaUnidadeporSegmento.UnidadedeNegocios = mMetadaUnidadeporTrimestre.UnidadedeNegocio;
                    mMetadaUnidadeporSegmento.Ano = mMetadaUnidadeporTrimestre.Ano;
                    mMetadaUnidadeporSegmento.Trimestre = mMetadaUnidadeporTrimestre.Trimestre;
                    //mMetadaUnidadeporSegmento.Segmento = new Lookup(item.Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadaUnidadeporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadaUnidadeporSegmento.MetaporTrimestre = new Lookup(mMetadaUnidadeporTrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporTrimestre>());

                    RepositoryService.MetadaUnidadeporSegmento.Create(mMetadaUnidadeporSegmento);
                }

                ServiceMetadaUnidadeporFamilia.Criar(mMetadaUnidadeporTrimestre.MetadaUnidade.Id, mMetadaUnidadeporSegmento, OrcaSegmento.ToList(), OrcaSegmento.First().Segmento.Id);
            }
        }

        public void CalcularMeta(Guid metaunidadeId)
        {
            List<MetadaUnidadeporSegmento> lstOrcamentodaUnidadeporSegmento = RepositoryService.MetadaUnidadeporSegmento.Obter(metaunidadeId);
            foreach (MetadaUnidadeporSegmento item in lstOrcamentodaUnidadeporSegmento)
            {
                //item.OrcamentoNaoAlocado = 0;
                //item.OrcamentoParaNovosProdutos = 0;
                item.MetaPlanejada = 0;

                List<MetadaUnidadeporFamilia> lstFamilia = RepositoryService.MetadaUnidadeporFamilia.ObterFamiliaporSegmento(item.ID.Value);
                foreach (MetadaUnidadeporFamilia familia in lstFamilia)
                {
                    //item.OrcamentoNaoAlocado += familia.OrcamentoNaoAlocado.HasValue ? familia.OrcamentoNaoAlocado.Value : 0;
                    //item.OrcamentoParaNovosProdutos += familia.OrcamentoParaNovosProdutos.HasValue ? familia.OrcamentoParaNovosProdutos.Value : 0;
                    item.MetaPlanejada += familia.MetaPlanejada.HasValue ? familia.MetaPlanejada.Value : 0;
                }

                RepositoryService.MetadaUnidadeporSegmento.Update(item);
            }

        }

        public void OldCriarKARepresentante(MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Segmento.Id));

            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                MetadaUnidadeporSegmento mMetadaUnidadeporSegmento;
                mMetadaUnidadeporSegmento = RepositoryService.MetadaUnidadeporSegmento.Obter(OrcaSegmento.First().Segmento.Id, mMetadaUnidadeporTrimestre.ID.Value);
                if (mMetadaUnidadeporSegmento == null)
                {
                    mMetadaUnidadeporSegmento = new MetadaUnidadeporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mMetadaUnidadeporSegmento.ID = Guid.NewGuid();
                    mMetadaUnidadeporSegmento.Nome = mMetadaUnidadeporTrimestre.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                    mMetadaUnidadeporSegmento.UnidadedeNegocios = mMetadaUnidadeporTrimestre.UnidadedeNegocio;
                    mMetadaUnidadeporSegmento.Ano = mMetadaUnidadeporTrimestre.Ano;
                    mMetadaUnidadeporSegmento.Trimestre = mMetadaUnidadeporTrimestre.Trimestre;
                    //mMetadaUnidadeporSegmento.Segmento = new Lookup(item.Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadaUnidadeporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mMetadaUnidadeporSegmento.MetaporTrimestre = new Lookup(mMetadaUnidadeporTrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporTrimestre>());

                    RepositoryService.MetadaUnidadeporSegmento.Create(mMetadaUnidadeporSegmento);
                }

                ServiceMetadaUnidadeporFamilia.Criar(mMetadaUnidadeporTrimestre.MetadaUnidade.Id, mMetadaUnidadeporSegmento, OrcaSegmento.ToList(), OrcaSegmento.First().Segmento.Id);
            }
        }

        public void AtualizarFaturamentoDoSegmento(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetas = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetas.Count != 0)
            {
                DataTable dtMetasSegmento = RepositoryService.MetadaUnidadeporSegmento.ListarMetasSegmentoDW(ano, trimestre, lstMetas);

                foreach (DataRow item in dtMetasSegmento.Rows)
                {
                    UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                    Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());

                    if (mUnidadeNegocio != null && mSegmento != null)
                    {
                        MetadaUnidadeporSegmento itemcapa = RepositoryService.MetadaUnidadeporSegmento.ObterMetasSegmento(mUnidadeNegocio.ID.Value, mSegmento.ID.Value, ano, trimestre, "itbc_metaporsegmentoid");
                        if (itemcapa != null)
                        {
                            itemcapa.MetaRealizada = item.Field<decimal>("vlr");
                            RepositoryService.MetadaUnidadeporSegmento.Update(itemcapa);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

