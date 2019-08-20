using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialdoKAporSubfamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialdoKAporSubfamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoKAporSubfamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys
        PotencialdoKAporProdutoService _ServicePotencialdoKAporProduto = null;
        PotencialdoKAporProdutoService ServicePotencialdoKAporProduto
        {
            get
            {
                if (_ServicePotencialdoKAporProduto == null)
                    _ServicePotencialdoKAporProduto = new PotencialdoKAporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoKAporProduto;
            }
        }
        #endregion

        #region Métodos
        public void Criar(PotencialdoKAporFamilia mPotencialdoKAporFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid representanteId, int numTrimestre)
        {
            PotencialdoKAporSubfamilia mPotencialdoKAporSubfamilia = null;

            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id));

            foreach (var OrcaSegSubFamilia in lstOrcamentoporSegSubFamilia)
            {
                
                mPotencialdoKAporFamilia = RepositoryService.PotencialdoKAporFamilia.Retrieve(mPotencialdoKAporFamilia.ID.Value);
                PotencialdoKAporSubfamilia tempSubFamilia = RepositoryService.PotencialdoKAporSubfamilia.Obterpor(representanteId, OrcaSegSubFamilia.First().Familia.Id, mPotencialdoKAporFamilia.ID.Value, OrcaSegSubFamilia.First().Segmento.Id, OrcaSegSubFamilia.First().SubFamilia.Id, mPotencialdoKAporFamilia.Trimestre.HasValue ? mPotencialdoKAporFamilia.Trimestre.Value : int.MinValue);
                if (tempSubFamilia == null)
                {
                    mPotencialdoKAporSubfamilia = new PotencialdoKAporSubfamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mPotencialdoKAporSubfamilia.ID = Guid.NewGuid();
                    mPotencialdoKAporSubfamilia.UnidadedeNegocio = mPotencialdoKAporFamilia.UnidadedeNegocio;
                    mPotencialdoKAporSubfamilia.Ano = mPotencialdoKAporFamilia.Ano;
                    mPotencialdoKAporSubfamilia.Trimestre = numTrimestre;
                    mPotencialdoKAporSubfamilia.Segmento = new Lookup(OrcaSegSubFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mPotencialdoKAporSubfamilia.FamiliadeProduto = new Lookup(OrcaSegSubFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mPotencialdoKAporSubfamilia.SubfamiliadeProduto = new Lookup(OrcaSegSubFamilia.First().SubFamilia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mPotencialdoKAporSubfamilia.PotencialdoKAporFamilia = new Lookup(mPotencialdoKAporFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoKAporFamilia>());
                    mPotencialdoKAporSubfamilia.Nome = (mPotencialdoKAporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Length > 99 ? (mPotencialdoKAporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Substring(1, 99) : (mPotencialdoKAporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name);
                    mPotencialdoKAporSubfamilia.KAouRepresentante = new Lookup(representanteId, SDKore.Crm.Util.Utility.GetEntityName<Model.Contato>());
                    RepositoryService.PotencialdoKAporSubfamilia.Create(mPotencialdoKAporSubfamilia);
                }

                if (tempSubFamilia != null)
                    ServicePotencialdoKAporProduto.Criar(tempSubFamilia, OrcaSegSubFamilia.ToList(), representanteId, numTrimestre);
                else
                    ServicePotencialdoKAporProduto.Criar(mPotencialdoKAporSubfamilia, OrcaSegSubFamilia.ToList(), representanteId, numTrimestre);
            }
        }

        public void RetornoDWKaSubFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialdoKAporSubfamilia.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Contato mContato = RepositoryService.Contato.ObterPor(item["CD_representante"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());

                if (mUnidadeNegocio != null && mContato != null && mSegmento != null && mFamiliaProduto != null && mSubfamiliaProduto != null)
                {
                    PotencialdoKAporSubfamilia mPotencialdoKAporSubfamilia = RepositoryService.PotencialdoKAporSubfamilia
                        .Obter(mUnidadeNegocio.ID.Value, mContato.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value, mSubfamiliaProduto.ID.Value);
                    
                    if (mPotencialdoKAporSubfamilia != null)
                    {
                        mPotencialdoKAporSubfamilia.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.PotencialdoKAporSubfamilia.Update(mPotencialdoKAporSubfamilia);
                    }
                }
            }
        }

        #endregion
    }
}

