using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialdoSupervisorporProdutoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public PotencialdoSupervisorporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialdoSupervisorporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys
        PotencialDetalhadodoSupervisorporProdutoService _ServicePotencialDetalhadodoSupervisorporProduto = null;
        PotencialDetalhadodoSupervisorporProdutoService ServicePotencialDetalhadodoSupervisorporProduto
        {
            get
            {
                if (_ServicePotencialDetalhadodoSupervisorporProduto == null)
                    _ServicePotencialDetalhadodoSupervisorporProduto = new PotencialDetalhadodoSupervisorporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServicePotencialDetalhadodoSupervisorporProduto;
            }
        }
        #endregion

        #region Método
        public void Criar(PotencialdoSupervisorporSubfamilia mPotencialdoSupervisorporSubfamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado)
        {
            PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto;
            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}/{3}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id, x.Produto.Id));

            foreach (var OrcaProduto in lstOrcamentoporSegSubFamilia)
            {
                if (OrcaProduto.First().Produto.Id != Guid.Empty)
                {
                    //mPotencialdoSupervisorporProduto = RepositoryService.PotencialdoSupervisorporProduto.Obter(OrcaProduto.First().Canal.Id, OrcaProduto.First().Produto.Id, OrcaProduto.First().SubFamilia.Id, mPotencialdoSupervisorporSubfamilia.ID.Value, (int)mPotencialdoSupervisorporSubfamilia.Trimestre);
                    mPotencialdoSupervisorporProduto = RepositoryService.PotencialdoSupervisorporProduto.Obter(OrcaProduto.First().Canal.Id, OrcaProduto.First().Produto.Id, mPotencialdoSupervisorporSubfamilia.ID.Value, (int)mPotencialdoSupervisorporSubfamilia.Trimestre);

                    if (mPotencialdoSupervisorporProduto == null)
                    {
                        mPotencialdoSupervisorporProduto = new PotencialdoSupervisorporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        mPotencialdoSupervisorporProduto.ID = Guid.NewGuid();
                        mPotencialdoSupervisorporProduto.Ano = mPotencialdoSupervisorporSubfamilia.Ano;
                        mPotencialdoSupervisorporProduto.Trimestre = mPotencialdoSupervisorporSubfamilia.Trimestre;
                        mPotencialdoSupervisorporProduto.Produto = new Lookup(OrcaProduto.First().Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mPotencialdoSupervisorporProduto.PotencialdoSupervisorPorProduto = new Lookup(mPotencialdoSupervisorporSubfamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisorporSubfamilia>());
                        mPotencialdoSupervisorporProduto.Nome = OrcaProduto.First().Produto.Name;
                        mPotencialdoSupervisorporProduto.Supervisor = new Lookup(OrcaProduto.First().Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());

                        RepositoryService.PotencialdoSupervisorporProduto.Create(mPotencialdoSupervisorporProduto);

                        ServicePotencialDetalhadodoSupervisorporProduto.Criar(mPotencialdoSupervisorporProduto);
                    }
                }
            }
        }

        public void Atualizar(OrcamentoDetalhado mMetaDetalhado)
        {
            decimal valor = 0;
            int quantidade = 0;
            PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto1;
            PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto2;
            PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto3;
            PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto4;

            if (mMetaDetalhado.AtualizarTrimestre1)
            {
                mPotencialdoSupervisorporProduto1 = RepositoryService.PotencialdoSupervisorporProduto.Obter(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre1.Id.Value);
                if (mPotencialdoSupervisorporProduto1 != null)
                {
                    mMetaDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    //ServiceMetaDetalhadadoCanalporProduto.Calcular(mPotencialdoSupervisorporProduto1, mMetaDetalhado.Trimestre1, ref valor, ref quantidade);
                    ServicePotencialDetalhadodoSupervisorporProduto.Calcular(mPotencialdoSupervisorporProduto1, mMetaDetalhado.Trimestre1, ref valor, ref quantidade);
                    mPotencialdoSupervisorporProduto1.PotencialPlanejado = valor;
                    mPotencialdoSupervisorporProduto1.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoSupervisorporProduto.Update(mPotencialdoSupervisorporProduto1);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre2)
            {
                mPotencialdoSupervisorporProduto2 = RepositoryService.PotencialdoSupervisorporProduto.Obter(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre2.Id.Value);
                if (mPotencialdoSupervisorporProduto2 != null)
                {
                    mMetaDetalhado.Trimestre2.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                    //ServiceMetaDetalhadadoCanalporProduto.Calcular(mPotencialdoSupervisorporProduto2, mMetaDetalhado.Trimestre2, ref valor, ref quantidade);
                    ServicePotencialDetalhadodoSupervisorporProduto.Calcular(mPotencialdoSupervisorporProduto2, mMetaDetalhado.Trimestre2, ref valor, ref quantidade);
                    mPotencialdoSupervisorporProduto2.PotencialPlanejado = valor;
                    mPotencialdoSupervisorporProduto2.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoSupervisorporProduto.Update(mPotencialdoSupervisorporProduto2);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre3)
            {
                mPotencialdoSupervisorporProduto3 = RepositoryService.PotencialdoSupervisorporProduto.Obter(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre3.Id.Value);
                if (mPotencialdoSupervisorporProduto3 != null)
                {
                    mMetaDetalhado.Trimestre3.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                    //ServiceMetaDetalhadadoCanalporProduto.Calcular(mPotencialdoSupervisorporProduto3, mMetaDetalhado.Trimestre3, ref valor, ref quantidade);
                    ServicePotencialDetalhadodoSupervisorporProduto.Calcular(mPotencialdoSupervisorporProduto3, mMetaDetalhado.Trimestre3, ref valor, ref quantidade);
                    mPotencialdoSupervisorporProduto3.PotencialPlanejado = valor;
                    mPotencialdoSupervisorporProduto3.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoSupervisorporProduto.Update(mPotencialdoSupervisorporProduto3);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre4)
            {
                mPotencialdoSupervisorporProduto4 = RepositoryService.PotencialdoSupervisorporProduto.Obter(mMetaDetalhado.CanalID.Value, mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre4.Id.Value);
                if (mPotencialdoSupervisorporProduto4 != null)
                {
                    mMetaDetalhado.Trimestre4.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                    //ServiceMetaDetalhadadoCanalporProduto.Calcular(mPotencialdoSupervisorporProduto4, mMetaDetalhado.Trimestre4, ref valor, ref quantidade);
                    ServicePotencialDetalhadodoSupervisorporProduto.Calcular(mPotencialdoSupervisorporProduto4, mMetaDetalhado.Trimestre4, ref valor, ref quantidade);
                    mPotencialdoSupervisorporProduto4.PotencialPlanejado = valor;
                    mPotencialdoSupervisorporProduto4.QtdePlanejada = quantidade;
                    RepositoryService.PotencialdoSupervisorporProduto.Update(mPotencialdoSupervisorporProduto4);
                }
            }
        }

        public void RetornoDWProduto(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);

            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialdoSupervisorporProduto.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtMetaCanal.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Usuario mUsuario = RepositoryService.Usuario.ObterPor(item["CD_representante"].ToString());
                Segmento mSegmento = RepositoryService.Segmento.ObterPor(item["cd_segmento"].ToString());
                FamiliaProduto mFamiliaProduto = RepositoryService.FamiliaProduto.ObterPor(item["CD_familia"].ToString());
                SubfamiliaProduto mSubfamiliaProduto = RepositoryService.SubfamiliaProduto.ObterPor(item["CD_subfamilia"].ToString());
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());

                if (mUnidadeNegocio != null && mUsuario != null && mSegmento != null && mFamiliaProduto != null && mSubfamiliaProduto != null && mProduto != null)
                {
                    PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto = RepositoryService.PotencialdoSupervisorporProduto
                        .Obter(mUnidadeNegocio.ID.Value, mUsuario.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), trimestre, mSegmento.ID.Value, mFamiliaProduto.ID.Value, mSubfamiliaProduto.ID.Value, mProduto.ID.Value);

                    if (mPotencialdoSupervisorporProduto != null)
                    {
                        mPotencialdoSupervisorporProduto.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                        mPotencialdoSupervisorporProduto.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                        RepositoryService.PotencialdoSupervisorporProduto.Update(mPotencialdoSupervisorporProduto);
                    }
                }
            }

        }

        #endregion
    }
}