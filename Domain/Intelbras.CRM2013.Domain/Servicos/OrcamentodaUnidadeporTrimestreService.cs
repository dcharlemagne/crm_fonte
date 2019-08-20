
using System;
using System.Collections.Generic;
using System.Data;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodaUnidadeporTrimestreService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeporTrimestreService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodaUnidadeporTrimestreService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodaUnidadeporSegmentoService _ServiceOrcamentodaUnidadeporSegmentoService = null;
        private OrcamentodaUnidadeporSegmentoService ServiceOrcamentodaUnidadeporSegmentoService
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeporSegmentoService == null)
                    _ServiceOrcamentodaUnidadeporSegmentoService = new OrcamentodaUnidadeporSegmentoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeporSegmentoService;
            }
        }

        OrcamentodoCanalService _ServiceOrcamentodoCanal = null;
        private OrcamentodoCanalService ServiceOrcamentodoCanal
        {
            get
            {
                if (_ServiceOrcamentodoCanal == null)
                    _ServiceOrcamentodoCanal = new OrcamentodoCanalService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanal;
            }
        }

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

        #region Métodos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mOrcamentodaUnidade"></param>
        /// <param name="lstOrcamentoDetalhado"></param>
        /// <param name="Nome"></param>
        /// <param name="trimestre"></param>
        /// <param name="lstLinhas"></param>
        public void Atualiza(OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            ServiceOrcamentodaUnidadeporSegmentoService.Criar(mOrcamentodaUnidadeporTrimestre, lstOrcamentoDetalhado);

            if (mOrcamentodaUnidade.NiveldoOrcamento.Value == (int)Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Detalhado)
                ServiceOrcamentodoCanal.Criar(mOrcamentodaUnidadeporTrimestre, mOrcamentodaUnidade, lstOrcamentoDetalhado);

        }

        public void Criar(Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, string Nome, int trimestre
            , Guid trimestreId, bool addLinha)
        {
            OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre;

            mOrcamentodaUnidadeporTrimestre = new OrcamentodaUnidadeporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            mOrcamentodaUnidadeporTrimestre.Ano = mOrcamentodaUnidade.Ano;
            mOrcamentodaUnidadeporTrimestre.UnidadedeNegocio = mOrcamentodaUnidade.UnidadedeNegocio;
            mOrcamentodaUnidadeporTrimestre.Trimestre = trimestre;
            mOrcamentodaUnidadeporTrimestre.Nome = mOrcamentodaUnidade.Nome + Nome;
            mOrcamentodaUnidadeporTrimestre.OrcamentoporUnidade = new Lookup(mOrcamentodaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidade>());
            mOrcamentodaUnidadeporTrimestre.ID = trimestreId;

            RepositoryService.OrcamentodaUnidadeporTrimestre.Create(mOrcamentodaUnidadeporTrimestre);
            ServiceOrcamentodaUnidadeporSegmentoService.Criar(mOrcamentodaUnidadeporTrimestre, lstOrcamentoDetalhado);

            if (mOrcamentodaUnidade.NiveldoOrcamento.Value == (int)Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Detalhado)
                ServiceOrcamentodoCanal.Criar(mOrcamentodaUnidadeporTrimestre, mOrcamentodaUnidade, lstOrcamentoDetalhado);

        }


        public void AtualizaManual(OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            ServiceOrcamentodoCanal.CriarManual(mOrcamentodaUnidadeporTrimestre, mOrcamentodaUnidade, lstOrcamentoDetalhado);
        }

        public void CriarManual(Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, string Nome, int trimestre
            , Guid trimestreId, bool addLinha)
        {
            OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre;

            mOrcamentodaUnidadeporTrimestre = new OrcamentodaUnidadeporTrimestre(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
            mOrcamentodaUnidadeporTrimestre.Ano = mOrcamentodaUnidade.Ano;
            mOrcamentodaUnidadeporTrimestre.UnidadedeNegocio = mOrcamentodaUnidade.UnidadedeNegocio;
            mOrcamentodaUnidadeporTrimestre.Trimestre = trimestre;
            mOrcamentodaUnidadeporTrimestre.Nome = mOrcamentodaUnidade.Nome + Nome;
            mOrcamentodaUnidadeporTrimestre.OrcamentoporUnidade = new Lookup(mOrcamentodaUnidade.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidade>());
            mOrcamentodaUnidadeporTrimestre.ID = trimestreId;

            RepositoryService.OrcamentodaUnidadeporTrimestre.Create(mOrcamentodaUnidadeporTrimestre);
            ServiceOrcamentodoCanal.CriarManual(mOrcamentodaUnidadeporTrimestre, mOrcamentodaUnidade, lstOrcamentoDetalhado);
        }

        /// <summary>
        /// obtem todos os segmentos e atualiza o trimestre
        /// </summary>
        /// <param name="orcamentounidadeId"></param>
        public void CalcularOrcamento(Guid orcamentounidadeId)
        {
            List<OrcamentodaUnidadeporTrimestre> lstOrcamentodaUnidadeporTri = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(orcamentounidadeId);
            foreach (OrcamentodaUnidadeporTrimestre item in lstOrcamentodaUnidadeporTri)
            {
                item.OrcamentonaoAlocado = 0;
                item.OrcamentoparanovosProdutos = 0;
                item.OrcamentoPlanejado = 0;

                List<OrcamentodaUnidadeporSegmento> lstSegmento = RepositoryService.OrcamentodaUnidadeporSegmento.ListarSegmentodoOrcamentoproTrimestre(item.ID.Value);
                foreach (OrcamentodaUnidadeporSegmento segmento in lstSegmento)
                {
                    item.OrcamentonaoAlocado += segmento.OrcamentoNaoAlocado.HasValue ? segmento.OrcamentoNaoAlocado.Value : 0;
                    item.OrcamentoparanovosProdutos += segmento.OrcamentoParaNovosProdutos.HasValue ? segmento.OrcamentoParaNovosProdutos.Value : 0;
                    item.OrcamentoPlanejado += segmento.OrcamentoPlanejado.HasValue ? segmento.OrcamentoPlanejado.Value : 0;
                }

                RepositoryService.OrcamentodaUnidadeporTrimestre.Update(item);
            }

        }

        public void AtualizaManual(Model.OrcamentodaUnidade mOrcamentodaUnidade, Trimestre trimestre, Guid canalId)
        {
            OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mOrcamentodaUnidade.ID.Value, trimestre.Id.Value);
            ServiceOrcamentodoCanal.AtualizarManual(mOrcamentodaUnidadeporTrimestre, trimestre, canalId);
        }

        public void RetornoDWTrimestre()
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(DateTime.Now.Date.Year);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoTrimestre = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestreDW(DateTime.Now.Date.Year, Helper.TrimestreAtual()[1], lstOrcamentodaUnidade);

            #region Atualiza Orçamentos Trimestre
            foreach (DataRow item in dtOrcamentoTrimestre.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre = RepositoryService.OrcamentodaUnidadeporTrimestre.ObterOrcamentoTrimestre(mUnidadeNegocio.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), Helper.TrimestreAtual()[1]);

                if (mUnidadeNegocio != null && mOrcamentodaUnidadeporTrimestre != null)
                {
                    mOrcamentodaUnidadeporTrimestre.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());

                    RepositoryService.OrcamentodaUnidadeporTrimestre.Update(mOrcamentodaUnidadeporTrimestre);
                }
            }
            #endregion

        }
        #endregion
    }
}

