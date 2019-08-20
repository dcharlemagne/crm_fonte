using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodoCanalporFamiliaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalporFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodoCanalporFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos

        OrcamentodoCanalporSubFamiliaService _ServiceOrcamentodoCanalporSubFamilia = null;
        private OrcamentodoCanalporSubFamiliaService ServiceOrcamentodoCanalporSubFamilia
        {
            get
            {
                if (_ServiceOrcamentodoCanalporSubFamilia == null)
                    _ServiceOrcamentodoCanalporSubFamilia = new OrcamentodoCanalporSubFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalporSubFamilia;
            }
        }


        #endregion

        #region Método
        public void Criar(OrcamentodoCanalporSegmento mOrcamentodoCanalporSegmento, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid SegmentoId, Guid canalId)
        {
            OrcamentodoCanalporFamilia mOrcamentodoCanalporFamilia;
            var lstorcamentoporsegfamilia = (from x in lstOrcamentoDetalhado
                                             //where x.Segmento.Id == SegmentoId && x.Canal.Id == canalId
                                             group x by string.Format("{0}/{1}", x.Familia.Id, x.Segmento.Id));

            foreach (var OrcaSegFamilia in lstorcamentoporsegfamilia)
            {
                mOrcamentodoCanalporFamilia = RepositoryService.OrcamentodoCanalporFamilia.ObterOrcamentoCanalFamilia(canalId, mOrcamentodoCanalporSegmento.ID.Value, OrcaSegFamilia.First().Segmento.Id, OrcaSegFamilia.First().Familia.Id, mOrcamentodoCanalporSegmento.Trimestre.Value);
                if (mOrcamentodoCanalporFamilia == null)
                {
                    mOrcamentodoCanalporFamilia = new OrcamentodoCanalporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodoCanalporFamilia.ID = Guid.NewGuid();
                    mOrcamentodoCanalporFamilia.Canal = new Lookup(canalId, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mOrcamentodoCanalporFamilia.UnidadedeNegocio = mOrcamentodoCanalporSegmento.UnidadedeNegocio;
                    mOrcamentodoCanalporFamilia.Ano = mOrcamentodoCanalporSegmento.Ano;
                    mOrcamentodoCanalporFamilia.Trimestre = mOrcamentodoCanalporSegmento.Trimestre;
                    mOrcamentodoCanalporFamilia.Segmento = new Lookup(OrcaSegFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodoCanalporFamilia.FamiliadeProduto = new Lookup(OrcaSegFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mOrcamentodoCanalporFamilia.OrcamentodoCanalporSegmento = new Lookup(mOrcamentodoCanalporSegmento.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporSegmento>());

                    mOrcamentodoCanalporFamilia.Nome = (mOrcamentodoCanalporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).ToString().Length > 99 ?
                        (mOrcamentodoCanalporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).Substring(1, 99)
                        : (mOrcamentodoCanalporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name);

                    RepositoryService.OrcamentodoCanalporFamilia.Create(mOrcamentodoCanalporFamilia);
                }
                ServiceOrcamentodoCanalporSubFamilia.Criar(mOrcamentodoCanalporFamilia, OrcaSegFamilia.ToList(), OrcaSegFamilia.First().Familia.Id, canalId);

            }
        }

        public void CalcularOrcamento(Guid orcamentounidadeId)
        {
            List<OrcamentodoCanalporFamilia> lstOrcamentodoCanalporFamilia = RepositoryService.OrcamentodoCanalporFamilia.ListarOrcamentoFamiliaporOrcUnidade(orcamentounidadeId);
            foreach (OrcamentodoCanalporFamilia item in lstOrcamentodoCanalporFamilia)
            {
                item.OrcamentoPlanejado = 0;
                item.OrcamentoParaNovosProdutos = 0;

                List<OrcamentodoCanalporSubFamilia> lstSubFamilia = RepositoryService.OrcamentodoCanalporSubFamilia.ListarOrcamentoSubFamiliapor(item.ID.Value);
                foreach (OrcamentodoCanalporSubFamilia subfamilia in lstSubFamilia)
                {
                    item.OrcamentoPlanejado += subfamilia.OrcamentoPlanejado.HasValue ? subfamilia.OrcamentoPlanejado.Value : 0;
                    item.OrcamentoParaNovosProdutos += subfamilia.OrcamentoParaNovosProdutos.HasValue ? subfamilia.OrcamentoParaNovosProdutos.Value : 0;
                }

                RepositoryService.OrcamentodoCanalporFamilia.Update(item);
            }
        }

        public void RetornoDWCanalFamilia(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoCanalFamilia = RepositoryService.OrcamentodoCanalporFamilia.ListarCanalFamiliaDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoCanalFamilia.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["cd_unidade_negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null && mFamiliaProduto != null && mConta != null)
                {
                    OrcamentodoCanalporFamilia mOrcamentodoCanalporFamilia = RepositoryService.OrcamentodoCanalporFamilia.Obter(mUnidadeNegocio.ID.Value, ano, trimestre, mConta.ID.Value, mSegmento.ID.Value, mFamiliaProduto.ID.Value);

                    if (mOrcamentodoCanalporFamilia != null)
                    {
                        mOrcamentodoCanalporFamilia.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());
                        RepositoryService.OrcamentodoCanalporFamilia.Update(mOrcamentodoCanalporFamilia);
                    }
                }
            }
        }

        #endregion
    }
}

