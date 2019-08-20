using System;
using System.Collections.Generic;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialdoKAporSegmentoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }
        public PotencialdoKAporSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoKAporSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys
        PotencialdoKAporFamiliaService _ServicePotencialdoKAporFamilia = null;
        PotencialdoKAporFamiliaService ServicePotencialdoKAporFamilia
        {
            get
            {
                if (_ServicePotencialdoKAporFamilia == null)
                    _ServicePotencialdoKAporFamilia = new PotencialdoKAporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoKAporFamilia;
            }
        }
        #endregion

        #region Métodos
        public void Criar(PotencialdoKAporTrimestre mPotencialdoKATrimestre, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid representanteId, int numTrimestre)
        {
            PotencialdoKAporSegmento mPotencialdoKAporSegmento = null;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Familia.Id));



            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                mPotencialdoKATrimestre = RepositoryService.PotencialdoKAporTrimestre.Retrieve(mPotencialdoKATrimestre.ID.Value);
                List<PotencialdoKAporSegmento> tempSegmento = RepositoryService.PotencialdoKAporSegmento.__Obter(mPotencialdoKATrimestre.UnidadedeNegocio.Id, representanteId, numTrimestre, mPotencialdoKATrimestre.ID.Value, OrcaSegmento.First().Segmento.Id);
                if (tempSegmento.Count == 0)
                {
                    PortfoliodoKeyAccountRepresentantes portfolioKA = RepositoryService.PortfoliodoKeyAccountRepresentantes.ObterPor(mPotencialdoKATrimestre.UnidadedeNegocio.Id, representanteId);

                    if (portfolioKA.Segmento != null)
                    {
                        if (OrcaSegmento.First().Segmento.Id != portfolioKA.Segmento.Id)
                            continue;
                    }

                    mPotencialdoKAporSegmento = new PotencialdoKAporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mPotencialdoKAporSegmento.ID = Guid.NewGuid();
                    mPotencialdoKAporSegmento.Nome = mPotencialdoKATrimestre.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                    mPotencialdoKAporSegmento.UnidadedeNegocio = mPotencialdoKATrimestre.UnidadedeNegocio;
                    mPotencialdoKAporSegmento.Ano = mPotencialdoKATrimestre.Ano;
                    mPotencialdoKAporSegmento.KAouRepresentante = new Lookup(representanteId, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());
                    mPotencialdoKAporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mPotencialdoKAporSegmento.Trimestre = numTrimestre;
                    mPotencialdoKAporSegmento.PotencialKaTrimestre = new Lookup(mPotencialdoKATrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporTrimestre>());

                    RepositoryService.PotencialdoKAporSegmento.Create(mPotencialdoKAporSegmento);

                    ServicePotencialdoKAporFamilia.Criar(mPotencialdoKAporSegmento, OrcaSegmento.ToList(), representanteId, numTrimestre);
                }
                else
                {
                    ServicePotencialdoKAporFamilia.Criar(tempSegmento[0], OrcaSegmento.ToList(), representanteId, numTrimestre);
                }
            }
        }

        public void CalcularMetaKa(MetadaUnidade mMetadaUnidade, List<PortfoliodoKeyAccountRepresentantes> mPortifolioKA)
        {
            foreach (var port in mPortifolioKA)
            {
                List<PotencialdoKAporSegmento> lstPotencialdoKAporSegmento = RepositoryService.PotencialdoKAporSegmento.ListarSegmentos(mMetadaUnidade, port.KeyAccountRepresentante.Id);
                if (lstPotencialdoKAporSegmento.Count > 0)
                {
                    foreach (PotencialdoKAporSegmento PotSeg in lstPotencialdoKAporSegmento)
                    {
                        PotSeg.PotencialPlanejado = 0;

                        List<PotencialdoKAporFamilia> lstFamilia = RepositoryService.PotencialdoKAporFamilia.ObterFamiliaporSegmento(PotSeg.ID.Value);
                        foreach (PotencialdoKAporFamilia familia in lstFamilia)
                        {
                            PotSeg.PotencialPlanejado += familia.PotencialPlanejado.HasValue ? familia.PotencialPlanejado.Value : 0;
                        }

                        RepositoryService.PotencialdoKAporSegmento.Update(PotSeg);
                    }
                }
            }
        }

        public void AtualizarFaturamentoDoSegmentoRepresentante(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialdoKAporSegmento.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            List<UnidadeNegocio> unidadeNegocioAll = RepositoryService.UnidadeNegocio.ListarTodosChaveIntegracao();
            Dictionary<string, UnidadeNegocio> unidadeNegocioMap = new Dictionary<string, UnidadeNegocio>();

            foreach (var tmpVar in unidadeNegocioAll)
            {
                if (tmpVar.ChaveIntegracao != null)
                {
                    unidadeNegocioMap.Add(tmpVar.ChaveIntegracao, tmpVar);
                }
            }

            List<Contato> representatesAll = RepositoryService.Contato.ListarKaRepresentantes(null);
            Dictionary<string, Contato> representatesMap = new Dictionary<string, Contato>();

            foreach (var tmpVar in representatesAll)
            {
                if (tmpVar.CodigoRepresentante != null)
                {
                    representatesMap.Add(tmpVar.CodigoRepresentante, tmpVar);
                }
            }

            List<Segmento> segmentosAll = RepositoryService.Segmento.ListarTodos();
            Dictionary<string, Segmento> segmentosMap = new Dictionary<string, Segmento>();

            foreach (var tmpVar in segmentosAll)
            {
                if (tmpVar.CodigoSegmento != null)
                {
                    segmentosMap.Add(tmpVar.CodigoSegmento, tmpVar);
                }
            }


            foreach (DataRow item in dtMetaCanal.Rows)
            {
                if (item.IsNull("CD_Unidade_Negocio") || item.IsNull("CD_representante"))
                {
                    continue;
                }

                if (unidadeNegocioMap.ContainsKey(item.Field<string>("CD_Unidade_Negocio")) 
                    && representatesMap.ContainsKey(item["CD_representante"].ToString()) 
                    && segmentosMap.ContainsKey(item["cd_segmento"].ToString()))
                {
                    UnidadeNegocio mUnidadeNegocio = unidadeNegocioMap[item.Field<string>("CD_Unidade_Negocio")];
                    Contato mContato = representatesMap[item["CD_representante"].ToString()];
                    Segmento mSeg = segmentosMap[item["cd_segmento"].ToString()];

                    PotencialdoKAporSegmento mPotencialdoKAporSegmento = RepositoryService.PotencialdoKAporSegmento.Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, ano, trimestre, mSeg.ID.Value);
                    if (mPotencialdoKAporSegmento != null)
                    {
                        mPotencialdoKAporSegmento.PotencialRealizado = item.Field<decimal>("vlr");
                        RepositoryService.PotencialdoKAporSegmento.Update(mPotencialdoKAporSegmento);
                    }
                }
            }

        }
        #endregion

        //public void CalcularMeta(List<OrcamentoDetalhado> lstOrcamentoDetalhado)
        //{
        //    foreach (MetadaUnidadeporSegmento item in lstOrcamentoDetalhado)
        //    {
        //        //item.OrcamentoNaoAlocado = 0;
        //        //item.OrcamentoParaNovosProdutos = 0;
        //        item.MetaPlanejada = 0;

        //        List<MetadaUnidadeporFamilia> lstFamilia = RepositoryService.MetadaUnidadeporFamilia.ObterFamiliaporSegmento(item.ID.Value);
        //        foreach (MetadaUnidadeporFamilia familia in lstFamilia)
        //        {
        //            //item.OrcamentoNaoAlocado += familia.OrcamentoNaoAlocado.HasValue ? familia.OrcamentoNaoAlocado.Value : 0;
        //            //item.OrcamentoParaNovosProdutos += familia.OrcamentoParaNovosProdutos.HasValue ? familia.OrcamentoParaNovosProdutos.Value : 0;
        //            item.MetaPlanejada += familia.MetaPlanejada.HasValue ? familia.MetaPlanejada.Value : 0;
        //        }

        //        RepositoryService.MetadaUnidadeporSegmento.Update(item);
        //    }

        //}
    }
}
