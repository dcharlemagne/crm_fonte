using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class OrcamentodaUnidadeporSubFamiliaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeporSubFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodaUnidadeporSubFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodaUnidadeporProdutoService _ServiceOrcamentodaUnidadeporProduto = null;
        private OrcamentodaUnidadeporProdutoService ServiceOrcamentodaUnidadeporProduto
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporProduto == null)
                    _ServiceOrcamentodaUnidadeporProduto = new OrcamentodaUnidadeporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporProduto;
            }
        }

        #endregion

        #region Método
        public void OldCriar(Guid orcamentounidadeId, OrcamentodaUnidadeporFamilia mOrcamentodaUnidadeporSegFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid familiaId, bool addLinha, ref int Linha)
        {
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                where x.Familia.Id == familiaId
                                                group x by string.Format("{0}/{1}/{2}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id));

            foreach (var OrcaSegSubFamilia in lstOrcamentoporSegSubFamilia)
            {
                OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia;
                mOrcamentodaUnidadeporSubFamilia = RepositoryService.OrcamentodaUnidadeporSubFamilia.ObterOrcamentoSubFamilia(OrcaSegSubFamilia.First().SubFamilia.Id, mOrcamentodaUnidadeporSegFamilia.ID.Value);
                if (mOrcamentodaUnidadeporSubFamilia == null)
                {
                    mOrcamentodaUnidadeporSubFamilia = new OrcamentodaUnidadeporSubFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodaUnidadeporSubFamilia.ID = Guid.NewGuid();
                    mOrcamentodaUnidadeporSubFamilia.UnidadedeNegocio = mOrcamentodaUnidadeporSegFamilia.UnidadedeNegocio;
                    mOrcamentodaUnidadeporSubFamilia.Ano = mOrcamentodaUnidadeporSegFamilia.Ano;
                    mOrcamentodaUnidadeporSubFamilia.Trimestre = mOrcamentodaUnidadeporSegFamilia.Trimestre;
                    mOrcamentodaUnidadeporSubFamilia.Segmento = new Lookup(OrcaSegSubFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodaUnidadeporSubFamilia.FamiliadeProduto = new Lookup(OrcaSegSubFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mOrcamentodaUnidadeporSubFamilia.SubFamilia = new Lookup(OrcaSegSubFamilia.First().SubFamilia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mOrcamentodaUnidadeporSubFamilia.OrcamentoporFamilia = new Lookup(mOrcamentodaUnidadeporSegFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporFamilia>());
                    mOrcamentodaUnidadeporSubFamilia.Nome = (mOrcamentodaUnidadeporSegFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Length > 99 ? (mOrcamentodaUnidadeporSegFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Substring(1, 99) : (mOrcamentodaUnidadeporSegFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name);

                    RepositoryService.OrcamentodaUnidadeporSubFamilia.Create(mOrcamentodaUnidadeporSubFamilia);
                }
                ServiceOrcamentodaUnidadeporProduto.Criar(orcamentounidadeId, mOrcamentodaUnidadeporSubFamilia, lstOrcamentoDetalhado, OrcaSegSubFamilia.First().SubFamilia.Id, mOrcamentodaUnidadeporSegFamilia.OrcamentoporSegmento.Id);

            }
        }

        public void Criar(Guid orcamentounidadeId, OrcamentodaUnidadeporFamilia mOrcamentodaUnidadeporSegFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid familiaId)
        {
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id));

            foreach (var OrcaSegSubFamilia in lstOrcamentoporSegSubFamilia)
            {
                OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia;
                mOrcamentodaUnidadeporSubFamilia = RepositoryService.OrcamentodaUnidadeporSubFamilia.ObterOrcamentoSubFamilia(OrcaSegSubFamilia.First().SubFamilia.Id, mOrcamentodaUnidadeporSegFamilia.ID.Value);
                if (mOrcamentodaUnidadeporSubFamilia == null)
                {
                    mOrcamentodaUnidadeporSubFamilia = new OrcamentodaUnidadeporSubFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodaUnidadeporSubFamilia.ID = Guid.NewGuid();
                    mOrcamentodaUnidadeporSubFamilia.UnidadedeNegocio = mOrcamentodaUnidadeporSegFamilia.UnidadedeNegocio;
                    mOrcamentodaUnidadeporSubFamilia.Ano = mOrcamentodaUnidadeporSegFamilia.Ano;
                    mOrcamentodaUnidadeporSubFamilia.Trimestre = mOrcamentodaUnidadeporSegFamilia.Trimestre;
                    mOrcamentodaUnidadeporSubFamilia.Segmento = new Lookup(OrcaSegSubFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodaUnidadeporSubFamilia.FamiliadeProduto = new Lookup(OrcaSegSubFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mOrcamentodaUnidadeporSubFamilia.SubFamilia = new Lookup(OrcaSegSubFamilia.First().SubFamilia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mOrcamentodaUnidadeporSubFamilia.OrcamentoporFamilia = new Lookup(mOrcamentodaUnidadeporSegFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporFamilia>());
                    mOrcamentodaUnidadeporSubFamilia.Nome = (mOrcamentodaUnidadeporSegFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Length > 99 ? (mOrcamentodaUnidadeporSegFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Substring(1, 99) : (mOrcamentodaUnidadeporSegFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name);

                    RepositoryService.OrcamentodaUnidadeporSubFamilia.Create(mOrcamentodaUnidadeporSubFamilia);
                }
                ServiceOrcamentodaUnidadeporProduto.Criar(orcamentounidadeId, mOrcamentodaUnidadeporSubFamilia, OrcaSegSubFamilia.ToList(), OrcaSegSubFamilia.First().SubFamilia.Id, mOrcamentodaUnidadeporSegFamilia.OrcamentoporSegmento.Id);

            }
        }

        public void Calcular(OrcamentodaUnidadeporFamilia mOrcamentodaUnidadeporFamilia, OrcamentoDetalhado mOrcamentoDetalhado)
        {
            OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia = RepositoryService.OrcamentodaUnidadeporSubFamilia.ObterOrcamentoSubFamilia(mOrcamentoDetalhado.SubFamiliaID.Value, mOrcamentodaUnidadeporFamilia.ID.Value);
            ServiceOrcamentodaUnidadeporProduto.Calcular(mOrcamentodaUnidadeporSubFamilia, mOrcamentoDetalhado);


        }

        public void RetornoDWTrimestreSubFamilia(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoTrimestreSeg = RepositoryService.OrcamentodaUnidadeporSubFamilia.ListarOrcamentoSubFamiliaDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoTrimestreSeg.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null && mFamiliaProduto != null && mSubfamiliaProduto != null)
                {
                    OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia = RepositoryService.OrcamentodaUnidadeporSubFamilia
                        .ObterPorSubFamiliaTrimestreUnidade(mUnidadeNegocio.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value, mSubfamiliaProduto.ID.Value);

                    if (mOrcamentodaUnidadeporSubFamilia != null)
                    {
                        mOrcamentodaUnidadeporSubFamilia.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.OrcamentodaUnidadeporSubFamilia.Update(mOrcamentodaUnidadeporSubFamilia);
                    }
                }
            }
        }

        #endregion

    }
}

