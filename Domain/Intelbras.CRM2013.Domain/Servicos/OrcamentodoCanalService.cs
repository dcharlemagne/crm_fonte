using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class OrcamentodoCanalService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public OrcamentodoCanalService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodoCanalporSegmentoService _ServiceOrcamentodoCanalporSegmento = null;
        private OrcamentodoCanalporSegmentoService ServiceOrcamentodoCanalporSegmento
        {
            get
            {
                if (_ServiceOrcamentodoCanalporSegmento == null)
                    _ServiceOrcamentodoCanalporSegmento = new OrcamentodoCanalporSegmentoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalporSegmento;
            }
        }

        OrcamentodoCanalDetalhadoporProdutoService _ServiceOrcamentodoCanalDetalhadoporProduto = null;
        private OrcamentodoCanalDetalhadoporProdutoService ServiceOrcamentodoCanalDetalhadoporProduto
        {
            get
            {
                if (_ServiceOrcamentodoCanalDetalhadoporProduto == null)
                    _ServiceOrcamentodoCanalDetalhadoporProduto = new OrcamentodoCanalDetalhadoporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodoCanalDetalhadoporProduto;
            }
        }
        #endregion

        #region Método

        public OrcamentoPorCanal getOrcamentoCanal(Guid orcamentoporcanalId)
        {

            OrcamentoPorCanal orcamentocanal = RepositoryService.OrcamentoPorCanal.ObterPor(orcamentoporcanalId);
            if (orcamentocanal != null)
                return orcamentocanal;

            return null;


        }

        public void Criar(Model.OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            OrcamentoPorCanal mOrcamentodoCanal;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Canal.Id));

            foreach (var OrcaCanal in lstOrcamentoporSegmento)
            {
                mOrcamentodoCanal = RepositoryService.OrcamentoPorCanal.ObterPor(mOrcamentodaUnidadeporTrimestre.ID.Value, OrcaCanal.First().Canal.Id, mOrcamentodaUnidadeporTrimestre.Trimestre.Value);
                if (mOrcamentodoCanal == null)
                {
                    mOrcamentodoCanal = new OrcamentoPorCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                    mOrcamentodoCanal.Ano = mOrcamentodaUnidade.Ano;
                    mOrcamentodoCanal.UnidadedeNegocio = mOrcamentodaUnidade.UnidadedeNegocio;
                    mOrcamentodoCanal.Trimestre = mOrcamentodaUnidadeporTrimestre.Trimestre;
                    mOrcamentodoCanal.OrcamentoporTrimestredaUnidade = new Lookup(mOrcamentodaUnidadeporTrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporTrimestre>());
                    mOrcamentodoCanal.Nome = mOrcamentodaUnidadeporTrimestre.Nome;
                    mOrcamentodoCanal.Canal = new Lookup(OrcaCanal.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mOrcamentodoCanal.ID = Guid.NewGuid();

                    RepositoryService.OrcamentodoCanal.Create(mOrcamentodoCanal);
                }
                ServiceOrcamentodoCanalporSegmento.Criar(mOrcamentodoCanal, mOrcamentodaUnidade, OrcaCanal.ToList());
            }
        }

        public void CriarManual(Model.OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, Model.OrcamentodaUnidade mOrcamentodaUnidade, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            OrcamentoPorCanal mOrcamentodoCanal;
            var lstOrcamentoporSegmento = (from x in lstOrcamentoDetalhado
                                           group x by string.Format("{0}", x.Canal.Id));

            foreach (var OrcaCanal in lstOrcamentoporSegmento)
            {
                mOrcamentodoCanal = RepositoryService.OrcamentoPorCanal.ObterPor(mOrcamentodaUnidadeporTrimestre.ID.Value, OrcaCanal.First().Canal.Id, mOrcamentodaUnidadeporTrimestre.Trimestre.Value);
                if (mOrcamentodoCanal == null)
                {
                    mOrcamentodoCanal = new OrcamentoPorCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                    mOrcamentodoCanal.Ano = mOrcamentodaUnidade.Ano;
                    mOrcamentodoCanal.UnidadedeNegocio = mOrcamentodaUnidade.UnidadedeNegocio;
                    mOrcamentodoCanal.Trimestre = mOrcamentodaUnidadeporTrimestre.Trimestre;
                    mOrcamentodoCanal.OrcamentoporTrimestredaUnidade = new Lookup(mOrcamentodaUnidadeporTrimestre.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporTrimestre>());
                    mOrcamentodoCanal.Nome = mOrcamentodaUnidadeporTrimestre.Nome;
                    mOrcamentodoCanal.Canal = new Lookup(OrcaCanal.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                    mOrcamentodoCanal.ID = Guid.NewGuid();

                    RepositoryService.OrcamentodoCanal.Create(mOrcamentodoCanal);
                }

                Trimestre trimestre = new Trimestre();
                #region
                if (mOrcamentodaUnidadeporTrimestre.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1)
                {
                    trimestre.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    trimestre.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Janeiro;
                    trimestre.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Fevereiro;
                    trimestre.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1.Marco;
                }
                else if (mOrcamentodaUnidadeporTrimestre.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2)
                {
                    trimestre.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                    trimestre.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Abril;
                    trimestre.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Maio;
                    trimestre.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2.Junho;
                }
                else if (mOrcamentodaUnidadeporTrimestre.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3)
                {
                    trimestre.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                    trimestre.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Julho;
                    trimestre.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Agosto;
                    trimestre.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3.Setembro;
                }
                else if (mOrcamentodaUnidadeporTrimestre.Trimestre.Value == (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4)
                {
                    trimestre.trimestre = (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                    trimestre.Mes1 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Outubro;
                    trimestre.Mes2 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Novembro;
                    trimestre.Mes3 = (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4.Dezembro;
                }
                #endregion

                //so cria no retorno da planilha
                ServiceOrcamentodoCanalDetalhadoporProduto.CriarManual(mOrcamentodoCanal, trimestre);
            }
        }

        public void CalcularOrcamento(Guid orcamentounidadeId)
        {
            List<OrcamentoPorCanal> lstOrcamentodoCanal = RepositoryService.OrcamentoPorCanal.ListarPorOrcamentoUnidade(orcamentounidadeId);
            foreach (OrcamentoPorCanal item in lstOrcamentodoCanal)
            {
                item.OrcamentoParaNovosProdutos = 0;
                item.OrcamentoPlanejado = 0;

                List<OrcamentodoCanalporSegmento> lstSegmento = RepositoryService.OrcamentodoCanalporSegmento.ListarSegmentodoOrcamentocanal(item.ID.Value);
                foreach (OrcamentodoCanalporSegmento segmento in lstSegmento)
                {
                    item.OrcamentoParaNovosProdutos += segmento.OrcamentoParaNovosProdutos.HasValue ? segmento.OrcamentoParaNovosProdutos.Value : 0;
                    item.OrcamentoPlanejado += segmento.OrcamentoPlanejado.HasValue ? segmento.OrcamentoPlanejado.Value : 0;
                }

                RepositoryService.OrcamentodoCanal.Update(item);
            }

        }

        public void AtualizarManual(OrcamentodaUnidadeporTrimestre mOrcamentodaUnidadeporTrimestre, Trimestre trimestre, Guid canalId)
        {
            OrcamentoPorCanal mOrcamentoPorCanal = RepositoryService.OrcamentoPorCanal.ObterPor(mOrcamentodaUnidadeporTrimestre.ID.Value, canalId, trimestre.trimestre.Value);
            mOrcamentoPorCanal.OrcamentoPlanejado = mOrcamentoPorCanal.OrcamentoPlanejado.HasValue ? mOrcamentoPorCanal.OrcamentoPlanejado.Value : 0;
            mOrcamentoPorCanal.OrcamentoPlanejado += trimestre.Mes1Vlr + trimestre.Mes2Vlr + trimestre.Mes3Vlr;

            ServiceOrcamentodoCanalDetalhadoporProduto.CriarManual(mOrcamentoPorCanal, trimestre);

            RepositoryService.OrcamentoPorCanal.Update(mOrcamentoPorCanal);
        }

        public void RetornoDWCanalTrimestre(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);

            if (lstOrcamentodaUnidade.Count == 0)
            {
                return;
            }

            DataTable dtOrcamentoTrimestre = RepositoryService.OrcamentoPorCanal.ListarCanalDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoTrimestre.Rows)
            {
                if (item.IsNull("CD_Unidade_Negocio") || item.IsNull("CD_Emitente"))
                {
                    continue;
                }

                UnidadeNegocio unidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item.Field<string>("CD_Unidade_Negocio"));
                Conta conta = RepositoryService.Conta.ObterCanal(item.Field<string>("CD_Emitente"));

                if (unidadeNegocio != null && conta != null)
                {
                    OrcamentoPorCanal mOrcamentoCanalTrimetre = RepositoryService.OrcamentoPorCanal.ObterPor(unidadeNegocio.ID.Value, ano, trimestre, conta.ID.Value);

                    if (mOrcamentoCanalTrimetre != null)
                    {
                        var orcamentoPorCanalUpdate = new OrcamentoPorCanal(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline)
                        {
                            ID = mOrcamentoCanalTrimetre.ID,
                            OrcamentoRealizado = item.Field<decimal>("vlr")
                        };

                        RepositoryService.OrcamentoPorCanal.Update(orcamentoPorCanalUpdate);
                    }
                }
            }
        }

        #endregion

    }
}

