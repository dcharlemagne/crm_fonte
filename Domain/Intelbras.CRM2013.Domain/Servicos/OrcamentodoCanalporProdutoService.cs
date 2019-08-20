using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodoCanalporProdutoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodoCanalporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
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

        #region Métodos
        public void Criar(OrcamentodoCanalporSubFamilia mOrcamentodoCanalporSubFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid subfamiliaId, Guid canalId)
        {
            OrcamentodoCanalporProduto mOrcamentodaUnidadeporProduto;
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}/{3}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id, x.Produto.Id));

            foreach (var OrcaProduto in lstOrcamentoporSegSubFamilia)
            {
                if (OrcaProduto.First().Produto.Id != Guid.Empty)
                {
                    mOrcamentodaUnidadeporProduto = RepositoryService.OrcamentodoCanalporProduto.ObterOrcCanalProduto(canalId, OrcaProduto.First().Produto.Id, mOrcamentodoCanalporSubFamilia.ID.Value, (int)mOrcamentodoCanalporSubFamilia.Trimestre);

                    if (mOrcamentodaUnidadeporProduto == null)
                    {
                        mOrcamentodaUnidadeporProduto = new OrcamentodoCanalporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        mOrcamentodaUnidadeporProduto.ID = Guid.NewGuid();
                        mOrcamentodaUnidadeporProduto.Ano = mOrcamentodoCanalporSubFamilia.Ano;
                        //mOrcamentodaUnidadeporProduto.UnidadeNegocio = new Lookup(OrcaProduto.First().UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<UnidadeNegocio>());
                        mOrcamentodaUnidadeporProduto.Trimestre = mOrcamentodoCanalporSubFamilia.Trimestre;
                        mOrcamentodaUnidadeporProduto.Produto = new Lookup(OrcaProduto.First().Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcamentodaUnidadeporProduto.OrcamentodoCanalporSubFamilia = new Lookup(mOrcamentodoCanalporSubFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporSubFamilia>());
                        mOrcamentodaUnidadeporProduto.Nome = OrcaProduto.First().Produto.Name;
                        mOrcamentodaUnidadeporProduto.Canal = new Lookup(OrcaProduto.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());

                        RepositoryService.OrcamentodoCanalporProduto.Create(mOrcamentodaUnidadeporProduto);
                        ServiceOrcamentodoCanalDetalhadoporProduto.Criar(mOrcamentodaUnidadeporProduto, canalId);
                    }
                }
            }
        }

        public void Atualizar(OrcamentoDetalhado mOrcamentoDetalhado, OrcamentodoCanalporSubFamilia mOrcCanalporSubFamilia, Trimestre trimestre)
        {
            OrcamentodoCanalporProduto mOrcamentodoCanalporProduto = RepositoryService.OrcamentodoCanalporProduto.ObterOrcamentoCanalporProduto(mOrcamentoDetalhado.ProdutoID.Value, mOrcCanalporSubFamilia.ID.Value);
            mOrcamentodoCanalporProduto.OrcamentoPlanejado += trimestre.Mes1Vlr.Value + trimestre.Mes2Vlr.Value + trimestre.Mes3Vlr.Value;
            mOrcamentodoCanalporProduto.QtdePlanejada += trimestre.Mes1Qtde.Value + trimestre.Mes2Qtde.Value + trimestre.Mes3Qtde.Value;

            ServiceOrcamentodoCanalDetalhadoporProduto.Calcular(mOrcamentodoCanalporProduto, trimestre);
            RepositoryService.OrcamentodoCanalporProduto.Update(mOrcamentodoCanalporProduto);
        }

        public void Atualizar(OrcamentoDetalhado mOrcamentoDetalhado)
        {
            decimal valor = 0;
            int quantidade = 0;
            OrcamentodoCanalporProduto mOrcamentodoCanalporProduto1;
            OrcamentodoCanalporProduto mOrcamentodoCanalporProduto2;
            OrcamentodoCanalporProduto mOrcamentodoCanalporProduto3;
            OrcamentodoCanalporProduto mOrcamentodoCanalporProduto4;

            if (mOrcamentoDetalhado.AtualizarTrimestre1)
            {
                mOrcamentodoCanalporProduto1 = RepositoryService.OrcamentodoCanalporProduto.ObterOrcCanalporProduto(mOrcamentoDetalhado.CanalID.Value, mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre1.Id.Value);
                mOrcamentodoCanalporProduto1.Trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                mOrcamentoDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                ServiceOrcamentodoCanalDetalhadoporProduto.Calcular(mOrcamentodoCanalporProduto1, mOrcamentoDetalhado.Trimestre1, ref valor, ref quantidade);
                mOrcamentodoCanalporProduto1.OrcamentoPlanejado = valor;
                mOrcamentodoCanalporProduto1.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodoCanalporProduto.Update(mOrcamentodoCanalporProduto1);
            }

            if (mOrcamentoDetalhado.AtualizarTrimestre2)
            {
                mOrcamentodoCanalporProduto2 = RepositoryService.OrcamentodoCanalporProduto.ObterOrcCanalporProduto(mOrcamentoDetalhado.CanalID.Value, mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre2.Id.Value);
                mOrcamentodoCanalporProduto2.Trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                mOrcamentoDetalhado.Trimestre2.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                ServiceOrcamentodoCanalDetalhadoporProduto.Calcular(mOrcamentodoCanalporProduto2, mOrcamentoDetalhado.Trimestre2, ref valor, ref quantidade);
                mOrcamentodoCanalporProduto2.OrcamentoPlanejado = valor;
                mOrcamentodoCanalporProduto2.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodoCanalporProduto.Update(mOrcamentodoCanalporProduto2);
            }

            if (mOrcamentoDetalhado.AtualizarTrimestre3)
            {
                mOrcamentodoCanalporProduto3 = RepositoryService.OrcamentodoCanalporProduto.ObterOrcCanalporProduto(mOrcamentoDetalhado.CanalID.Value, mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre3.Id.Value);
                mOrcamentoDetalhado.Trimestre3.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                mOrcamentodoCanalporProduto3.Trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                ServiceOrcamentodoCanalDetalhadoporProduto.Calcular(mOrcamentodoCanalporProduto3, mOrcamentoDetalhado.Trimestre3, ref valor, ref quantidade);
                mOrcamentodoCanalporProduto3.OrcamentoPlanejado = valor;
                mOrcamentodoCanalporProduto3.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodoCanalporProduto.Update(mOrcamentodoCanalporProduto3);
            }

            if (mOrcamentoDetalhado.AtualizarTrimestre4)
            {
                mOrcamentodoCanalporProduto4 = RepositoryService.OrcamentodoCanalporProduto.ObterOrcCanalporProduto(mOrcamentoDetalhado.CanalID.Value, mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre4.Id.Value);
                mOrcamentoDetalhado.Trimestre4.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                mOrcamentodoCanalporProduto4.Trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                ServiceOrcamentodoCanalDetalhadoporProduto.Calcular(mOrcamentodoCanalporProduto4, mOrcamentoDetalhado.Trimestre4, ref valor, ref quantidade);
                mOrcamentodoCanalporProduto4.OrcamentoPlanejado = valor;
                mOrcamentodoCanalporProduto4.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodoCanalporProduto.Update(mOrcamentodoCanalporProduto4);
            }
        }

        public void RetornoDWCanalProduto(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);

            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoCanalProduto = RepositoryService.OrcamentodoCanalporProduto.ListarCanalProdutoDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoCanalProduto.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["cd_unidade_negocio"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());

                if (mUnidadeNegocio != null && mConta != null && mProduto != null)
                {
                    OrcamentodoCanalporProduto mOrcamentodoCanalporProduto = RepositoryService.OrcamentodoCanalporProduto
                        .ObterOrcCanalProduto(mUnidadeNegocio.ID.Value, ano, trimestre, mConta.ID.Value, mProduto.ID.Value);
                    
                    if (mOrcamentodoCanalporProduto != null)
                    {
                        mOrcamentodoCanalporProduto.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());
                        mOrcamentodoCanalporProduto.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                        RepositoryService.OrcamentodoCanalporProduto.Update(mOrcamentodoCanalporProduto);
                    }
                }
            }
        }

        #endregion
    }
}

