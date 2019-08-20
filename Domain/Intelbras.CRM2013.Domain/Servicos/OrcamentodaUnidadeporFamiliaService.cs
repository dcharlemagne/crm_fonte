using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodaUnidadeporFamiliaService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeporFamiliaService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodaUnidadeporFamiliaService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodaUnidadeporSubFamiliaService _ServiceOrcamentodaUnidadeporSubFamilia = null;
        private OrcamentodaUnidadeporSubFamiliaService ServiceOrcamentodaUnidadeporSubFamilia
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporSubFamilia == null)
                    _ServiceOrcamentodaUnidadeporSubFamilia = new OrcamentodaUnidadeporSubFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporSubFamilia;
            }
        }

        #endregion

        #region Método
        public void Criar(Guid orcamentounidadeId, OrcamentodaUnidadeporSegmento mOrcamentodaUnidadeporSegmento, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid SegmentoId)
        {
            var lstorcamentoporsegfamilia = (from x in lstOrcamentoDetalhado
                                             group x by string.Format("{0}/{1}", x.Segmento.Id, x.Familia.Id));

            foreach (var OrcaSegFamilia in lstorcamentoporsegfamilia)
            {
                OrcamentodaUnidadeporFamilia mOrcamentodaUnidadeporSegFamilia;
                mOrcamentodaUnidadeporSegFamilia = RepositoryService.OrcamentodaUnidadeporFamilia.ObterOrcamentoFamilia(OrcaSegFamilia.First().Familia.Id, mOrcamentodaUnidadeporSegmento.ID.Value);
                if (mOrcamentodaUnidadeporSegFamilia == null)
                {
                    mOrcamentodaUnidadeporSegFamilia = new OrcamentodaUnidadeporFamilia(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodaUnidadeporSegFamilia.ID = Guid.NewGuid();
                    mOrcamentodaUnidadeporSegFamilia.UnidadedeNegocio = mOrcamentodaUnidadeporSegmento.UnidadedeNegocio;
                    mOrcamentodaUnidadeporSegFamilia.Ano = mOrcamentodaUnidadeporSegmento.Ano;
                    mOrcamentodaUnidadeporSegFamilia.Trimestre = mOrcamentodaUnidadeporSegmento.Trimestre;
                    mOrcamentodaUnidadeporSegFamilia.Segmento = new Lookup(OrcaSegFamilia.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodaUnidadeporSegFamilia.FamiliadeProduto = new Lookup(OrcaSegFamilia.First().Familia.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.FamiliaProduto>());
                    mOrcamentodaUnidadeporSegFamilia.OrcamentoporSegmento = new Lookup(mOrcamentodaUnidadeporSegmento.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporSegmento>());
                    mOrcamentodaUnidadeporSegFamilia.Nome = (mOrcamentodaUnidadeporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).ToString().Length > 99 ?
                        (mOrcamentodaUnidadeporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name).Substring(1, 99)
                        : (mOrcamentodaUnidadeporSegmento.Nome + " - " + OrcaSegFamilia.First().Familia.Name);

                    RepositoryService.OrcamentodaUnidadeporFamilia.Create(mOrcamentodaUnidadeporSegFamilia);
                }

                ServiceOrcamentodaUnidadeporSubFamilia.Criar(orcamentounidadeId, mOrcamentodaUnidadeporSegFamilia, OrcaSegFamilia.ToList(), OrcaSegFamilia.First().Familia.Id);
            }

        }

        public void CalcularOrcamento(Guid orcamentounidadeId)
        {
            List<OrcamentodaUnidadeporFamilia> lstOrcamentodaUnidadeporFamilia = RepositoryService.OrcamentodaUnidadeporFamilia.ObterOrcamentoFamiliaporOrcUnidade(orcamentounidadeId);
            foreach (OrcamentodaUnidadeporFamilia item in lstOrcamentodaUnidadeporFamilia)
            {
                item.OrcamentoNaoAlocado = 0;
                item.OrcamentoParaNovosProdutos = 0;
                item.OrcamentoPlanejado = 0;

                List<OrcamentodaUnidadeporSubFamilia> lstSubFamilia = RepositoryService.OrcamentodaUnidadeporSubFamilia.ListarSubFamiliaPor(item.ID.Value);
                foreach (OrcamentodaUnidadeporSubFamilia subfamilia in lstSubFamilia)
                {
                    item.OrcamentoNaoAlocado += subfamilia.OrcamentoNaoAlocado.HasValue ? subfamilia.OrcamentoNaoAlocado.Value : 0;
                    item.OrcamentoParaNovosProdutos += subfamilia.OrcamentoParaNovosProdutos.HasValue ? subfamilia.OrcamentoParaNovosProdutos.Value : 0;
                    item.OrcamentoPlanejado += subfamilia.OrcamentoPlanejado.HasValue ? subfamilia.OrcamentoPlanejado.Value : 0;
                }

                RepositoryService.OrcamentodaUnidadeporFamilia.Update(item);
            }
        }

        public void RetornoDWTrimestreFamilia(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoTrimestreSeg = RepositoryService.OrcamentodaUnidadeporFamilia.ListarOrcamentoFamiliaDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoTrimestreSeg.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null && mFamiliaProduto != null)
                {
                    OrcamentodaUnidadeporFamilia mOrcamentoCanalTrimetre = RepositoryService.OrcamentodaUnidadeporFamilia.ObterOrcamentoFamilia(mUnidadeNegocio.ID.Value, ano, trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value);

                    if (mOrcamentoCanalTrimetre != null)
                    {
                        mOrcamentoCanalTrimetre.OrcamentoRealizado = item.Field<decimal>("vlr");
                        RepositoryService.OrcamentodaUnidadeporFamilia.Update(mOrcamentoCanalTrimetre);
                    }
                }
            }
        }
        #endregion
    }
}

