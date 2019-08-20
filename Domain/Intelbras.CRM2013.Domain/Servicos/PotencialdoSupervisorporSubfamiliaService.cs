using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Intelbras.CRM2013.Domain.Servicos
{
    public class PotencialdoSupervisorporSubfamiliaService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }
        public PotencialdoSupervisorporSubfamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoSupervisorporSubfamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys
        PotencialdoSupervisorporProdutoService _ServicePotencialdoSupervisorporProduto = null;
        PotencialdoSupervisorporProdutoService ServicePotencialdoSupervisorporProduto
        {
            get
            {
                if (_ServicePotencialdoSupervisorporProduto == null)
                    _ServicePotencialdoSupervisorporProduto = new PotencialdoSupervisorporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialdoSupervisorporProduto;
            }
        }
        #endregion

        #region Métodos
        public void Criar(PotencialdoSupervisorporFamilia mPotencialdoSupervisorporFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            PotencialdoSupervisorporSubfamilia mPotencialdoSupervisorporSubfamilia;
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id));

            foreach (var OrcaSegSubFamilia in lstOrcamentoporSegSubFamilia)
            {
                mPotencialdoSupervisorporSubfamilia = RepositoryService.PotencialdoSupervisorporSubfamilia.Obterpor(OrcaSegSubFamilia.First().Canal.Id, OrcaSegSubFamilia.First().Familia.Id, mPotencialdoSupervisorporFamilia.ID.Value, OrcaSegSubFamilia.First().Segmento.Id, OrcaSegSubFamilia.First().SubFamilia.Id, mPotencialdoSupervisorporFamilia.Trimestre.Value);
                if (mPotencialdoSupervisorporSubfamilia == null)
                {
                    mPotencialdoSupervisorporSubfamilia = new PotencialdoSupervisorporSubfamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mPotencialdoSupervisorporSubfamilia.ID = Guid.NewGuid();
                    mPotencialdoSupervisorporSubfamilia.UnidadedeNegocio = mPotencialdoSupervisorporFamilia.UnidadedeNegocio;
                    mPotencialdoSupervisorporSubfamilia.Ano = mPotencialdoSupervisorporFamilia.Ano;
                    mPotencialdoSupervisorporSubfamilia.Trimestre = mPotencialdoSupervisorporFamilia.Trimestre;
                    mPotencialdoSupervisorporSubfamilia.Segmento = new Lookup(OrcaSegSubFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mPotencialdoSupervisorporSubfamilia.FamiliadoProduto = new Lookup(OrcaSegSubFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mPotencialdoSupervisorporSubfamilia.SubfamiliadeProduto = new Lookup(OrcaSegSubFamilia.First().SubFamilia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mPotencialdoSupervisorporSubfamilia.PotencialdoSupervisorporSubfamiliaID = new Lookup(mPotencialdoSupervisorporFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisorporFamilia>());
                    mPotencialdoSupervisorporSubfamilia.Nome = (mPotencialdoSupervisorporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Length > 99 ? (mPotencialdoSupervisorporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Substring(1, 99) : (mPotencialdoSupervisorporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name);
                    mPotencialdoSupervisorporSubfamilia.Supervisor = new Lookup(OrcaSegSubFamilia.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());
                    RepositoryService.PotencialdoSupervisorporSubfamilia.Create(mPotencialdoSupervisorporSubfamilia);
                }
                ServicePotencialdoSupervisorporProduto.Criar(mPotencialdoSupervisorporSubfamilia, OrcaSegSubFamilia.ToList());
                
            }
        }

        public void RetornoDWSubFamilia(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialdoSupervisorporSubfamilia.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Usuario mUsuario = RepositoryService.Usuario.ObterPor(item["CD_representante"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());

                if (mUnidadeNegocio != null && mUsuario != null && mSegmento != null && mFamiliaProduto != null && mSubfamiliaProduto != null)
                {
                    PotencialdoSupervisorporSubfamilia mPotencialdoSupervisorporSubfamilia = RepositoryService.PotencialdoSupervisorporSubfamilia
                        .Obter(mUnidadeNegocio.ID.Value, mUsuario.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value, mSubfamiliaProduto.ID.Value);
                    
                    if (mPotencialdoSupervisorporSubfamilia != null)
                    {
                        mPotencialdoSupervisorporSubfamilia.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.PotencialdoSupervisorporSubfamilia.Update(mPotencialdoSupervisorporSubfamilia);
                    }
                }
            }
        }

        #endregion
    }
}

