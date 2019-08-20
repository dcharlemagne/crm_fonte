using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class PotencialDetalhadodoSupervisorporProdutoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public PotencialDetalhadodoSupervisorporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }
        public PotencialDetalhadodoSupervisorporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Métodos
        public void Criar(Model.PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto)
        {
            Model.PotencialdoSupervisorporProdutoDetalhado mPotencialDetalhadodoSupervisorporProduto;

            switch (mPotencialdoSupervisorporProduto.Trimestre)
            {
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1:
                    #region 1º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1)))
                    {
                        mPotencialDetalhadodoSupervisorporProduto = new Model.PotencialdoSupervisorporProdutoDetalhado(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mPotencialDetalhadodoSupervisorporProduto.PotencialdoSupervisorPorProduto = new Lookup(mPotencialdoSupervisorporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisorporProduto>());
                        mPotencialDetalhadodoSupervisorporProduto.Ano = mPotencialdoSupervisorporProduto.Ano;
                        mPotencialDetalhadodoSupervisorporProduto.Supervisor = new Lookup(mPotencialdoSupervisorporProduto.Supervisor.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());
                        mPotencialDetalhadodoSupervisorporProduto.Mes = (int)mes;
                        mPotencialDetalhadodoSupervisorporProduto.Trimestre = mPotencialdoSupervisorporProduto.Trimestre;
                        mPotencialDetalhadodoSupervisorporProduto.Produto = new Lookup(mPotencialdoSupervisorporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mPotencialDetalhadodoSupervisorporProduto.Nome = mPotencialdoSupervisorporProduto.Nome;

                        RepositoryService.PotencialDetalhadodoSupervisorporProduto.Create(mPotencialDetalhadodoSupervisorporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2:
                    #region 2º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2)))
                    {
                        mPotencialDetalhadodoSupervisorporProduto = new Model.PotencialdoSupervisorporProdutoDetalhado(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mPotencialDetalhadodoSupervisorporProduto.PotencialdoSupervisorPorProduto = new Lookup(mPotencialdoSupervisorporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisorporProduto>());
                        mPotencialDetalhadodoSupervisorporProduto.Ano = mPotencialdoSupervisorporProduto.Ano;
                        mPotencialDetalhadodoSupervisorporProduto.Supervisor = new Lookup(mPotencialdoSupervisorporProduto.Supervisor.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());
                        mPotencialDetalhadodoSupervisorporProduto.Mes = (int)mes;
                        mPotencialDetalhadodoSupervisorporProduto.Trimestre = mPotencialdoSupervisorporProduto.Trimestre;
                        mPotencialDetalhadodoSupervisorporProduto.Produto = new Lookup(mPotencialdoSupervisorporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mPotencialDetalhadodoSupervisorporProduto.Nome = mPotencialdoSupervisorporProduto.Nome;

                        RepositoryService.PotencialDetalhadodoSupervisorporProduto.Create(mPotencialDetalhadodoSupervisorporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3:
                    #region 3º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3)))
                    {
                        mPotencialDetalhadodoSupervisorporProduto = new Model.PotencialdoSupervisorporProdutoDetalhado(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mPotencialDetalhadodoSupervisorporProduto.PotencialdoSupervisorPorProduto = new Lookup(mPotencialdoSupervisorporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisorporProduto>());
                        mPotencialDetalhadodoSupervisorporProduto.Ano = mPotencialdoSupervisorporProduto.Ano;
                        mPotencialDetalhadodoSupervisorporProduto.Mes = (int)mes;
                        mPotencialDetalhadodoSupervisorporProduto.Supervisor = new Lookup(mPotencialdoSupervisorporProduto.Supervisor.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());
                        mPotencialDetalhadodoSupervisorporProduto.Trimestre = mPotencialdoSupervisorporProduto.Trimestre;
                        mPotencialDetalhadodoSupervisorporProduto.Produto = new Lookup(mPotencialdoSupervisorporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mPotencialDetalhadodoSupervisorporProduto.Nome = mPotencialdoSupervisorporProduto.Nome;

                        RepositoryService.PotencialDetalhadodoSupervisorporProduto.Create(mPotencialDetalhadodoSupervisorporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4:
                    #region 4º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4)))
                    {
                        mPotencialDetalhadodoSupervisorporProduto = new Model.PotencialdoSupervisorporProdutoDetalhado(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mPotencialDetalhadodoSupervisorporProduto.PotencialdoSupervisorPorProduto = new Lookup(mPotencialdoSupervisorporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.PotencialdoSupervisorporProduto>());
                        mPotencialDetalhadodoSupervisorporProduto.Ano = mPotencialdoSupervisorporProduto.Ano;
                        mPotencialDetalhadodoSupervisorporProduto.Mes = (int)mes;
                        mPotencialDetalhadodoSupervisorporProduto.Supervisor = new Lookup(mPotencialdoSupervisorporProduto.Supervisor.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Usuario>());
                        mPotencialDetalhadodoSupervisorporProduto.Trimestre = mPotencialdoSupervisorporProduto.Trimestre;
                        mPotencialDetalhadodoSupervisorporProduto.Produto = new Lookup(mPotencialdoSupervisorporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mPotencialDetalhadodoSupervisorporProduto.Nome = mPotencialdoSupervisorporProduto.Nome;

                        RepositoryService.PotencialDetalhadodoSupervisorporProduto.Create(mPotencialDetalhadodoSupervisorporProduto);
                    }
                    #endregion
                    break;
            }

        }

        public void RetornoDWDetalhadoProduto(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtMetaCanal = RepositoryService.PotencialDetalhadodoSupervisorporProduto.ListarMetaTrimestreDW(ano, trimestre, lstMetadaUnidade);

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
                        PotencialdoSupervisorporProdutoDetalhado mPotencialDetalhadodoSupervisorporProduto = RepositoryService.PotencialDetalhadodoSupervisorporProduto
                            .Obter(Convert.ToInt32(item["cd_ano"].ToString()), trimestre, Convert.ToInt32(item["cd_mes"].ToString()), mProduto.ID.Value, mUsuario.ID.Value, mPotencialdoSupervisorporProduto.ID.Value);
                        
                        if (mPotencialDetalhadodoSupervisorporProduto != null)
                        {
                            mPotencialdoSupervisorporProduto.PotencialRealizado = decimal.Parse(item["vlr"].ToString());
                            mPotencialdoSupervisorporProduto.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                            RepositoryService.PotencialdoSupervisorporProduto.Update(mPotencialdoSupervisorporProduto);
                        }
                    }
                }
            }
        }

        public void Calcular(PotencialdoSupervisorporProduto mPotencialdoSupervisorporProduto, Trimestre trimestre, ref decimal valor, ref int quantidade)
        {
            decimal vlr = 0;
            int qtde = 0;

            List<PotencialdoSupervisorporProdutoDetalhado> lstPotencialDetalhadodoSupervisorporProduto = RepositoryService.PotencialDetalhadodoSupervisorporProduto.Listar(mPotencialdoSupervisorporProduto.Supervisor.Id, mPotencialdoSupervisorporProduto.Produto.Id, trimestre.Id.Value);
            foreach (PotencialdoSupervisorporProdutoDetalhado item in lstPotencialDetalhadodoSupervisorporProduto)
            {
                #region
                if (item.Mes == trimestre.Mes1)
                {
                    item.PotencialPlanejado = trimestre.Mes1Vlr.HasValue ? trimestre.Mes1Vlr.Value : 0;
                    item.QtdePlanejada = trimestre.Mes1Qtde.HasValue ? trimestre.Mes1Qtde.Value : 0;
                }
                else if (item.Mes == trimestre.Mes2)
                {
                    item.PotencialPlanejado = trimestre.Mes2Vlr.HasValue ? trimestre.Mes2Vlr.Value : 0;
                    item.QtdePlanejada = trimestre.Mes2Qtde.HasValue ? trimestre.Mes2Qtde.Value : 0;
                }
                else if (item.Mes == trimestre.Mes3)
                {
                    item.PotencialPlanejado = trimestre.Mes3Vlr.HasValue ? trimestre.Mes3Vlr.Value : 0;
                    item.QtdePlanejada = trimestre.Mes3Qtde.HasValue ? trimestre.Mes3Qtde.Value : 0;
                }

                RepositoryService.PotencialDetalhadodoSupervisorporProduto.Update(item);
                vlr += item.PotencialPlanejado.Value;
                qtde += (int)item.QtdePlanejada;
                #endregion
            }

            valor = vlr;
            quantidade = qtde;
        }


        #endregion
    }
}

