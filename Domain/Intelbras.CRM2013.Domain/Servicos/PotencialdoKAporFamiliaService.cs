using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;


namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialdoKAporFamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }
        public PotencialdoKAporFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoKAporFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys
        PotencialdoKAporSubfamiliaService _ServicePotencialdoKAporSubFamilia = null;
        PotencialdoKAporSubfamiliaService ServicePotencialdoKAporSubFamilia
        {
            get
            {
                if (_ServicePotencialdoKAporSubFamilia == null)
                    _ServicePotencialdoKAporSubFamilia = new PotencialdoKAporSubfamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoKAporSubFamilia;
            }
        }
        #endregion

        #region Métodos
        public void Criar(PotencialdoKAporSegmento mPotencialdoKAporSegmento, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid representanteId, int numTrimestre)
        {
            PotencialdoKAporFamilia mPotencialdoKAporFamilia = null;
            
            var lstorcamentoporsegfamilia = (from x in lstOrcamentoDetalhado
                                             group x by string.Format("{0}/{1}", x.Familia.Id, x.Segmento.Id));

            foreach (var OrcaSegFamilia in lstorcamentoporsegfamilia)
            {
                
                mPotencialdoKAporSegmento = RepositoryService.PotencialdoKAporSegmento.Retrieve(mPotencialdoKAporSegmento.ID.Value);
                PotencialdoKAporFamilia tempFamilia = RepositoryService.PotencialdoKAporFamilia.ObterPor(OrcaSegFamilia.First().Segmento.Id, OrcaSegFamilia.First().Familia.Id, representanteId, numTrimestre);
                if (tempFamilia == null)
                {
                   
                    mPotencialdoKAporFamilia = new PotencialdoKAporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mPotencialdoKAporFamilia.ID = Guid.NewGuid();
                    mPotencialdoKAporFamilia.KAouRepresentante = new Lookup(representanteId, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());
                    mPotencialdoKAporFamilia.UnidadedeNegocio = mPotencialdoKAporSegmento.UnidadedeNegocio;
                    mPotencialdoKAporFamilia.Ano = mPotencialdoKAporSegmento.Ano;
                    mPotencialdoKAporFamilia.Trimestre = numTrimestre;
                    mPotencialdoKAporFamilia.Segmento = new Lookup(OrcaSegFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mPotencialdoKAporFamilia.FamiliadeProduto = new Lookup(OrcaSegFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mPotencialdoKAporFamilia.PotencialdoKAporSegmento = new Lookup(mPotencialdoKAporSegmento.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporSegmento>());

                    mPotencialdoKAporFamilia.Nome = (mPotencialdoKAporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).ToString().Length > 99 ?
                        (mPotencialdoKAporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).Substring(1, 99)
                        : (mPotencialdoKAporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name);

                    RepositoryService.PotencialdoKAporFamilia.Create(mPotencialdoKAporFamilia);
                }
                if (tempFamilia != null)
                    ServicePotencialdoKAporSubFamilia.Criar(tempFamilia, OrcaSegFamilia.ToList(), representanteId, numTrimestre);
                else
                    ServicePotencialdoKAporSubFamilia.Criar(mPotencialdoKAporFamilia, OrcaSegFamilia.ToList(), representanteId, numTrimestre);
            }
        }

        public void CalcularMetaKA(Guid metaunidadeId)
        {
            List<PotencialdoKAporFamilia> lstPotencialdoKAporFamilia = RepositoryService.PotencialdoKAporFamilia.ObterFamiliaporMeta(metaunidadeId);
            foreach (PotencialdoKAporFamilia item in lstPotencialdoKAporFamilia)
            {
                item.PotencialPlanejado = 0;

                List<PotencialdoKAporSubfamilia> lstSubFamilia = RepositoryService.PotencialdoKAporSubfamilia.ObterSubFamiliaPor(item.ID.Value);
                foreach (PotencialdoKAporSubfamilia subfamilia in lstSubFamilia)
                {
                    item.PotencialPlanejado += subfamilia.PotencialPlanejado.HasValue ? subfamilia.PotencialPlanejado.Value : 0;
                }

                RepositoryService.PotencialdoKAporFamilia.Update(item);
            }
        }

        public void RetornoDWKaFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            DataTable dtMetaCanal = RepositoryService.PotencialdoKAporFamilia.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Contato mContato = RepositoryService.Contato.ObterPor(item["CD_representante"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());

                if (mUnidadeNegocio != null && mContato != null && mSegmento != null && mFamiliaProduto != null)
                {
                    PotencialdoKAporFamilia mPotencialdoKAporFamilia = RepositoryService.PotencialdoKAporFamilia.ObterPor(mUnidadeNegocio.ID.Value, mContato.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value);
                    if (mPotencialdoKAporFamilia != null)
                    {
                        mPotencialdoKAporFamilia.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.PotencialdoKAporFamilia.Update(mPotencialdoKAporFamilia);
                    }
                }
            }

        }
        #endregion

        public void CalcularMeta(Guid metaunidadeId)
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
    }
}

