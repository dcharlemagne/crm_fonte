using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetadoCanalporSegmentoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadoCanalporSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadoCanalporSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }


        #endregion

        #region Objetos
        MetadoCanalporFamiliaService _ServiceMetadoCanalporFamilia = null;
        MetadoCanalporFamiliaService ServiceMetadoCanalporFamilia
        {
            get
            {
                if (_ServiceMetadoCanalporFamilia == null)
                    _ServiceMetadoCanalporFamilia = new MetadoCanalporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetadoCanalporFamilia;
            }
        }
        #endregion

        #region Método
        public void Criar(Model.MetadaUnidadeporTrimestre mMetadaUnidadeporTrimestre, MetadoCanal mMetadoCanal, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            MetadoCanalporSegmento mMetadoCanalporSegmento;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Segmento.Id));

            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                //mMetadoCanalporSegmento = RepositoryService.MetadoCanalporSegmento.Obter(mMetadaUnidadeporTrimestre.MetadaUnidade.Id, OrcaSegmento.First().Canal.Id, mMetadaUnidadeporTrimestre.Trimestre.Value, OrcaSegmento.First().Segmento.Id);
                mMetadoCanalporSegmento = RepositoryService.MetadoCanalporSegmento.Obter(mMetadaUnidadeporTrimestre.UnidadedeNegocio.Id, OrcaSegmento.First().Canal.Id, mMetadaUnidadeporTrimestre.Ano.Value, mMetadaUnidadeporTrimestre.Trimestre.Value, OrcaSegmento.First().Segmento.Id);
                if (mMetadoCanalporSegmento == null)
                {
                    mMetadoCanalporSegmento = new MetadoCanalporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mMetadoCanalporSegmento.ID = Guid.NewGuid();
                    mMetadoCanalporSegmento.Nome = mMetadaUnidadeporTrimestre.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                    mMetadoCanalporSegmento.UnidadedeNegocio = mMetadaUnidadeporTrimestre.UnidadedeNegocio;
                    mMetadoCanalporSegmento.Ano = mMetadaUnidadeporTrimestre.Ano;// mOrcamentodaUnidade.Ano;
                    mMetadoCanalporSegmento.Canal = new Lookup(OrcaSegmento.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mMetadoCanalporSegmento.Trimestre = mMetadaUnidadeporTrimestre.Trimestre;
                    mMetadoCanalporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    //mMetadoCanalporSegmento.MetadoTrimestreCanal = new Lookup(mMetadoCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.MetadoCanal>());

                    RepositoryService.MetadoCanalporSegmento.Create(mMetadoCanalporSegmento);
                }

                ServiceMetadoCanalporFamilia.Criar(mMetadoCanalporSegmento, lstOrcamentoDetalhado);
            }
        }

        public void CalcularMeta(Guid metaunidadeId)
        {
            List<MetadoCanalporSegmento> lstMetadoCanalporSegmento = RepositoryService.MetadoCanalporSegmento.ListarPor(metaunidadeId);
            foreach (MetadoCanalporSegmento item in lstMetadoCanalporSegmento)
            {
                item.MetaPlanejada = 0;
                //item.OrcamentoParaNovosProdutos = 0;

                List<MetadoCanalporFamilia> lstFamilia = RepositoryService.MetadoCanalporFamilia.ListarPorSegmento(item.ID.Value);
                foreach (MetadoCanalporFamilia familia in lstFamilia)
                {
                    item.MetaPlanejada += familia.MetaPlanejada.HasValue ? familia.MetaPlanejada.Value : 0;
                    //item.OrcamentoParaNovosProdutos += familia.OrcamentoParaNovosProdutos.HasValue ? familia.OrcamentoParaNovosProdutos.Value : 0;
                }

                RepositoryService.MetadoCanalporSegmento.Update(item);
            }

        }

        public void AtualizarFaturamentoDoSegmentoCanal(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
            {
                return;
            }

            DataTable dtMetaCanalSegmento = RepositoryService.MetadoCanalporSegmento.ListarMetaCanalSegmentoDW(ano, trimestre, lstMetadaUnidade);
            
            foreach (DataRow item in dtMetaCanalSegmento.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());

                if (mUnidadeNegocio != null && mConta != null && mSegmento != null)
                {
                    MetadoCanalporSegmento mMetadoCanalporSegmento = RepositoryService.MetadoCanalporSegmento.Obter(mUnidadeNegocio.ID.Value, mConta.ID.Value, ano, trimestre, mSegmento.ID.Value, "itbc_metadocanalporsegmentoid");
                    if (mMetadoCanalporSegmento != null)
                    {
                        mMetadoCanalporSegmento.MetaRealizada = item.Field<decimal>("vlr");
                        RepositoryService.MetadoCanalporSegmento.Update(mMetadoCanalporSegmento);
                    }
                }
            }
        }
        #endregion
    }
}

