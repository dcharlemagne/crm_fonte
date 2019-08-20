using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodoCanalporSubFamiliaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalporSubFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodoCanalporSubFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodoCanalporProdutoService _ServiceOrcamentodoCanalporProduto = null;
        private OrcamentodoCanalporProdutoService ServiceOrcamentodoCanalporProduto
        {
            get
            {
                if (_ServiceOrcamentodoCanalporProduto == null)
                    _ServiceOrcamentodoCanalporProduto = new OrcamentodoCanalporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalporProduto;
            }
        }

        #endregion

        #region Método
        public void Criar(OrcamentodoCanalporFamilia mOrcamentodoCanalporFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid familiaId, Guid canalId)
        {
            OrcamentodoCanalporSubFamilia mOrcamentodoCanalporSubFamilia;
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                //where x.Familia.Id == familiaId && x.Canal.Id == canalId
                                                group x by string.Format("{0}/{1}/{2}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id));

            foreach (var OrcaSegSubFamilia in lstOrcamentoporSegSubFamilia)
            {
                mOrcamentodoCanalporSubFamilia = RepositoryService.OrcamentodoCanalporSubFamilia.ObterPorSubFamiliaTrimestreCanal(OrcaSegSubFamilia.First().Canal.Id, OrcaSegSubFamilia.First().Familia.Id, mOrcamentodoCanalporFamilia.ID.Value, OrcaSegSubFamilia.First().Segmento.Id, OrcaSegSubFamilia.First().SubFamilia.Id, mOrcamentodoCanalporFamilia.Trimestre.Value);
                if (mOrcamentodoCanalporSubFamilia == null)
                {
                    mOrcamentodoCanalporSubFamilia = new OrcamentodoCanalporSubFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodoCanalporSubFamilia.ID = Guid.NewGuid();
                    mOrcamentodoCanalporSubFamilia.UnidadedeNegocio = mOrcamentodoCanalporFamilia.UnidadedeNegocio;
                    mOrcamentodoCanalporSubFamilia.Ano = mOrcamentodoCanalporFamilia.Ano;
                    mOrcamentodoCanalporSubFamilia.Trimestre = mOrcamentodoCanalporFamilia.Trimestre;
                    mOrcamentodoCanalporSubFamilia.Segmento = new Lookup(OrcaSegSubFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodoCanalporSubFamilia.FamiliadeProduto = new Lookup(OrcaSegSubFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mOrcamentodoCanalporSubFamilia.SubFamilia = new Lookup(OrcaSegSubFamilia.First().SubFamilia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.SubfamiliaProduto>());
                    mOrcamentodoCanalporSubFamilia.OrcamentodoCanalporFamilia = new Lookup(mOrcamentodoCanalporFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporFamilia>());
                    mOrcamentodoCanalporSubFamilia.Nome = (mOrcamentodoCanalporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Length > 99 ? (mOrcamentodoCanalporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name).Substring(1, 99) : (mOrcamentodoCanalporFamilia.Nome + " - " + OrcaSegSubFamilia.First().SubFamilia.Name);
                    mOrcamentodoCanalporSubFamilia.Canal = new Lookup(canalId, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    RepositoryService.OrcamentodoCanalporSubFamilia.Create(mOrcamentodoCanalporSubFamilia);
                }
                ServiceOrcamentodoCanalporProduto.Criar(mOrcamentodoCanalporSubFamilia, OrcaSegSubFamilia.ToList(), OrcaSegSubFamilia.First().SubFamilia.Id, canalId);

            }
        }

        public void RetornoDWCanalSubFamilia(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoSubFamilia = RepositoryService.OrcamentodoCanalporSubFamilia.ListarCanalSubFamiliaDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoSubFamilia.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null && mFamiliaProduto != null && mConta != null && mSubfamiliaProduto != null)
                {
                    OrcamentodoCanalporSubFamilia  mOrcamentodoCanalporSubFamilia = RepositoryService.OrcamentodoCanalporSubFamilia
                        .Obter(mUnidadeNegocio.ID.Value, ano, trimestre, mConta.ID.Value, mSegmento.ID.Value, mFamiliaProduto.ID.Value, mSubfamiliaProduto.ID.Value);

                    if (mOrcamentodoCanalporSubFamilia != null)
                    {
                        mOrcamentodoCanalporSubFamilia.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.OrcamentodoCanalporSubFamilia.Update(mOrcamentodoCanalporSubFamilia);
                    }
                }
            }
        }
        #endregion
    }
}

