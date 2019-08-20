using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodoCanalporSegmentoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalporSegmentoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodoCanalporSegmentoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodoCanalporFamiliaService _ServiceOrcamentodoCanalporFamilia = null;
        private OrcamentodoCanalporFamiliaService ServiceOrcamentodoCanalporFamilia
        {
            get
            {
                if (_ServiceOrcamentodoCanalporFamilia == null)
                    _ServiceOrcamentodoCanalporFamilia = new OrcamentodoCanalporFamiliaService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalporFamilia;
            }
        }

        #endregion

        #region Método
        public void OldCriar(Model.OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid canalid, Guid orcamentodocanalId)
        {
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           where x.Canal.Id == canalid
                                           group x by string.Format("{0}", x.Segmento.Id));

            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                OrcamentodoCanalporSegmento mOrcamentodoCanalporSegmento = new OrcamentodoCanalporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                mOrcamentodoCanalporSegmento.ID = Guid.NewGuid();
                mOrcamentodoCanalporSegmento.Nome = mOrcamentodaUnidadeporTrimestre.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                mOrcamentodoCanalporSegmento.UnidadedeNegocio = mOrcamentodaUnidadeporTrimestre.UnidadedeNegocio;
                mOrcamentodoCanalporSegmento.Ano = mOrcamentodaUnidade.Ano;
                mOrcamentodoCanalporSegmento.Canal = new Lookup(OrcaSegmento.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                mOrcamentodoCanalporSegmento.Trimestre = mOrcamentodaUnidadeporTrimestre.Trimestre;
                mOrcamentodoCanalporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                mOrcamentodoCanalporSegmento.OrcamentodoCanal = new Lookup(orcamentodocanalId, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporTrimestre>());

                RepositoryService.OrcamentodoCanalporSegmento.Create(mOrcamentodoCanalporSegmento);
                ServiceOrcamentodoCanalporFamilia.Criar(mOrcamentodoCanalporSegmento, lstOrcamentoDetalhado, OrcaSegmento.First().Segmento.Id, OrcaSegmento.First().Canal.Id);
            }
        }

        public void Criar(OrcamentoPorCanal mOrcamentodoCanal, Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            OrcamentodoCanalporSegmento mOrcamentodoCanalporSegmento;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Segmento.Id));

            foreach (var OrcaSegmento in lstOrcamentoporSegmento)
            {
                mOrcamentodoCanalporSegmento = RepositoryService.OrcamentodoCanalporSegmento.ObterPor(mOrcamentodoCanal.ID.Value, OrcaSegmento.First().Canal.Id, mOrcamentodoCanal.Trimestre.Value, OrcaSegmento.First().Segmento.Id);
                if (mOrcamentodoCanalporSegmento == null)
                {
                    mOrcamentodoCanalporSegmento = new OrcamentodoCanalporSegmento(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                    mOrcamentodoCanalporSegmento.ID = Guid.NewGuid();
                    mOrcamentodoCanalporSegmento.Nome = mOrcamentodoCanal.Nome + " - " + OrcaSegmento.First().Segmento.Name;
                    mOrcamentodoCanalporSegmento.UnidadedeNegocio = mOrcamentodoCanal.UnidadedeNegocio;
                    mOrcamentodoCanalporSegmento.Ano = mOrcamentodaUnidade.Ano;
                    mOrcamentodoCanalporSegmento.Canal = new Lookup(OrcaSegmento.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mOrcamentodoCanalporSegmento.Trimestre = mOrcamentodoCanal.Trimestre;
                    mOrcamentodoCanalporSegmento.Segmento = new Lookup(OrcaSegmento.First().Segmento.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Segmento>());
                    mOrcamentodoCanalporSegmento.OrcamentodoCanal = new Lookup(mOrcamentodoCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentoPorCanal>());

                    RepositoryService.OrcamentodoCanalporSegmento.Create(mOrcamentodoCanalporSegmento);
                }
                ServiceOrcamentodoCanalporFamilia.Criar(mOrcamentodoCanalporSegmento, OrcaSegmento.ToList(), OrcaSegmento.First().Segmento.Id, OrcaSegmento.First().Canal.Id);
            }
        }

        public void CalcularOrcamento(Guid orcamentounidadeId)
        {
            List<OrcamentodoCanalporSegmento> lstOrcamentodoCanalporSegmento = RepositoryService.OrcamentodoCanalporSegmento.ListarSegmentodoOrcamentounidade(orcamentounidadeId);
            foreach (OrcamentodoCanalporSegmento item in lstOrcamentodoCanalporSegmento)
            {
                item.OrcamentoPlanejado = 0;
                item.OrcamentoParaNovosProdutos = 0;

                List<OrcamentodoCanalporFamilia> lstFamilia = RepositoryService.OrcamentodoCanalporFamilia.ListarOrcamentoFamiliapor(item.ID.Value);
                foreach (OrcamentodoCanalporFamilia familia in lstFamilia)
                {
                    item.OrcamentoPlanejado += familia.OrcamentoPlanejado.HasValue ? familia.OrcamentoPlanejado.Value : 0;
                    item.OrcamentoParaNovosProdutos += familia.OrcamentoParaNovosProdutos.HasValue ? familia.OrcamentoParaNovosProdutos.Value : 0;
                }

                RepositoryService.OrcamentodoCanalporSegmento.Update(item);
            }

        }

        public void RetornoDWCanalSegmento()
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(DateTime.Now.Date.Year);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoCanalSeg = RepositoryService.OrcamentodoCanalporSegmento.ListarCanalSegmentoDW(DateTime.Now.Date.Year, Helper.TrimestreAtual()[1], lstOrcamentodaUnidade);

            #region Atualiza Orçamentos Trimestre         
            foreach (DataRow item in dtOrcamentoCanalSeg.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());

                if (mUnidadeNegocio != null && mConta != null && mSegmento != null)
                {
                    OrcamentodoCanalporSegmento mOrcamentodoCanalporSegmento = RepositoryService.OrcamentodoCanalporSegmento.ObterPor(mUnidadeNegocio.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), Helper.TrimestreAtual()[1], mConta.ID.Value, mSegmento.ID.Value);

                    if (mOrcamentodoCanalporSegmento != null)
                    {
                        mOrcamentodoCanalporSegmento.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());

                        RepositoryService.OrcamentodoCanalporSegmento.Update(mOrcamentodoCanalporSegmento);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}

