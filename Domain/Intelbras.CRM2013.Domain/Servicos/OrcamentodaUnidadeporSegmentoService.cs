using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodaUnidadeporSegmentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeporSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodaUnidadeporSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodaUnidadeporFamiliaService _ServiceOrcamentodaUnidadeporFamilia = null;
        private OrcamentodaUnidadeporFamiliaService ServiceOrcamentodaUnidadeporFamilia
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporFamilia == null)
                    _ServiceOrcamentodaUnidadeporFamilia = new OrcamentodaUnidadeporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporFamilia;
            }
        }

        #endregion

        #region Métodos
        public void OldCriar(OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, bool addLinha)
        {
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Segmento.Id));

            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                OrcamentodaUnidadeporSegmento mOrcamentodaUnidadeporSegmento;
                mOrcamentodaUnidadeporSegmento = RepositoryService.OrcamentodaUnidadeporSegmento.ObterOrcamentoSegmento(OrcaSegmento.First().Segmento.Id, mOrcamentodaUnidadeporTrimestre.ID.Value);
                if (mOrcamentodaUnidadeporSegmento == null)
                {
                    mOrcamentodaUnidadeporSegmento = new OrcamentodaUnidadeporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodaUnidadeporSegmento.ID = Guid.NewGuid();
                    mOrcamentodaUnidadeporSegmento.Nome = mOrcamentodaUnidadeporTrimestre.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                    mOrcamentodaUnidadeporSegmento.UnidadedeNegocio = mOrcamentodaUnidadeporTrimestre.UnidadedeNegocio;
                    mOrcamentodaUnidadeporSegmento.Ano = mOrcamentodaUnidadeporTrimestre.Ano;
                    mOrcamentodaUnidadeporSegmento.Trimestre = mOrcamentodaUnidadeporTrimestre.Trimestre;
                    mOrcamentodaUnidadeporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Segmento>());
                    mOrcamentodaUnidadeporSegmento.OrcamentoporTrimestredaUnidade = new Lookup(mOrcamentodaUnidadeporTrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<OrcamentodaUnidadeporTrimestre>());

                    RepositoryService.OrcamentodaUnidadeporSegmento.Create(mOrcamentodaUnidadeporSegmento);
                }

                ServiceOrcamentodaUnidadeporFamilia.Criar(mOrcamentodaUnidadeporTrimestre.OrcamentoporUnidade.Id, mOrcamentodaUnidadeporSegmento, lstOrcamentoDetalhado, OrcaSegmento.First().Segmento.Id);
            }

        }

        public void Criar(OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Segmento.Id));

            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                OrcamentodaUnidadeporSegmento mOrcamentodaUnidadeporSegmento;
                mOrcamentodaUnidadeporSegmento = RepositoryService.OrcamentodaUnidadeporSegmento.ObterOrcamentoSegmento(OrcaSegmento.First().Segmento.Id, mOrcamentodaUnidadeporTrimestre.ID.Value);
                if (mOrcamentodaUnidadeporSegmento == null)
                {
                    mOrcamentodaUnidadeporSegmento = new OrcamentodaUnidadeporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodaUnidadeporSegmento.ID = Guid.NewGuid();
                    mOrcamentodaUnidadeporSegmento.Nome = mOrcamentodaUnidadeporTrimestre.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                    mOrcamentodaUnidadeporSegmento.UnidadedeNegocio = mOrcamentodaUnidadeporTrimestre.UnidadedeNegocio;
                    mOrcamentodaUnidadeporSegmento.Ano = mOrcamentodaUnidadeporTrimestre.Ano;
                    mOrcamentodaUnidadeporSegmento.Trimestre = mOrcamentodaUnidadeporTrimestre.Trimestre;
                    //mOrcamentodaUnidadeporSegmento.Segmento = new Lookup(item.Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodaUnidadeporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodaUnidadeporSegmento.OrcamentoporTrimestredaUnidade = new Lookup(mOrcamentodaUnidadeporTrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporTrimestre>());

                    RepositoryService.OrcamentodaUnidadeporSegmento.Create(mOrcamentodaUnidadeporSegmento);
                }

                ServiceOrcamentodaUnidadeporFamilia.Criar(mOrcamentodaUnidadeporTrimestre.OrcamentoporUnidade.Id, mOrcamentodaUnidadeporSegmento, OrcaSegmento.ToList(), OrcaSegmento.First().Segmento.Id);
            }
        }

        public void CalcularOrcamento(Guid orcamentounidadeId)
        {
            List<OrcamentodaUnidadeporSegmento> lstOrcamentodaUnidadeporSegmento = RepositoryService.OrcamentodaUnidadeporSegmento.ListarSegmentodoOrcamento(orcamentounidadeId);
            foreach (OrcamentodaUnidadeporSegmento item in lstOrcamentodaUnidadeporSegmento)
            {
                item.OrcamentoNaoAlocado = 0;
                item.OrcamentoParaNovosProdutos = 0;
                item.OrcamentoPlanejado = 0;

                List<OrcamentodaUnidadeporFamilia> lstFamilia = RepositoryService.OrcamentodaUnidadeporFamilia.ObterOrcamentoFamiliaporSegmento(item.ID.Value);
                foreach (OrcamentodaUnidadeporFamilia familia in lstFamilia)
                {
                    item.OrcamentoNaoAlocado += familia.OrcamentoNaoAlocado.HasValue ? familia.OrcamentoNaoAlocado.Value : 0;
                    item.OrcamentoParaNovosProdutos += familia.OrcamentoParaNovosProdutos.HasValue ? familia.OrcamentoParaNovosProdutos.Value : 0;
                    item.OrcamentoPlanejado += familia.OrcamentoPlanejado.HasValue ? familia.OrcamentoPlanejado.Value : 0;
                }

                RepositoryService.OrcamentodaUnidadeporSegmento.Update(item);
            }

        }

        public void AtualizarFaturamentoDoSegmento(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);

            if (lstOrcamentodaUnidade.Count == 0)
            {
                return;
            }

            DataTable dtOrcamentoTrimestreSeg = RepositoryService.OrcamentodaUnidadeporSegmento.ListarOrcamentoSegmentoDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoTrimestreSeg.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());

                if (mUnidadeNegocio != null && mSegmento != null)
                {
                    OrcamentodaUnidadeporSegmento orcamentoUnidadePorSegmento = RepositoryService.OrcamentodaUnidadeporSegmento
                        .ObterOrcamentoSegmento(mUnidadeNegocio.ID.Value, ano, trimestre, mSegmento.ID.Value, "itbc_orcamentoporsegmentoid");

                    if (orcamentoUnidadePorSegmento != null)
                    {
                        orcamentoUnidadePorSegmento.OrcamentoRealizado = item.Field<decimal>("vlr");
                        RepositoryService.OrcamentodaUnidadeporSegmento.Update(orcamentoUnidadePorSegmento);
                    }
                }
            }
        }

        #endregion
    }
}

