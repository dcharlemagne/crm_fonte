using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class OrcamentodoCanalDetalhadoporProdutoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodoCanalDetalhadoporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public OrcamentodoCanalDetalhadoporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public void Criar(Model.OrcamentodoCanalporProduto mOrcamentodoCanalporProduto, Guid canalId)
        {
            Model.OrcamentodoCanalDetalhadoporProduto mOrcCanalDetalhadoporProduto;

            switch (mOrcamentodoCanalporProduto.Trimestre)
            {
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1:
                    #region 1º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1)))
                    {
                        mOrcCanalDetalhadoporProduto = new Model.OrcamentodoCanalDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcCanalDetalhadoporProduto.OrcamentodoCanalporProduto = new Lookup(mOrcamentodoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporProduto>());
                        mOrcCanalDetalhadoporProduto.Ano = mOrcamentodoCanalporProduto.Ano;
                        mOrcCanalDetalhadoporProduto.Canal = new Lookup(mOrcamentodoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mOrcCanalDetalhadoporProduto.Mes = (int)mes;
                        mOrcCanalDetalhadoporProduto.Trimestre = mOrcamentodoCanalporProduto.Trimestre;
                        mOrcCanalDetalhadoporProduto.Produto = new Lookup(mOrcamentodoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcCanalDetalhadoporProduto.Nome = mOrcamentodoCanalporProduto.Nome;
                        //mOrcCanalDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodoCanalporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodoCanalDetalhadoporProduto.Create(mOrcCanalDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2:
                    #region 2º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2)))
                    {
                        mOrcCanalDetalhadoporProduto = new Model.OrcamentodoCanalDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcCanalDetalhadoporProduto.OrcamentodoCanalporProduto = new Lookup(mOrcamentodoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporProduto>());
                        mOrcCanalDetalhadoporProduto.Ano = mOrcamentodoCanalporProduto.Ano;
                        mOrcCanalDetalhadoporProduto.Canal = new Lookup(mOrcamentodoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mOrcCanalDetalhadoporProduto.Mes = (int)mes;
                        mOrcCanalDetalhadoporProduto.Trimestre = mOrcamentodoCanalporProduto.Trimestre;
                        mOrcCanalDetalhadoporProduto.Produto = new Lookup(mOrcamentodoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcCanalDetalhadoporProduto.Nome = mOrcamentodoCanalporProduto.Nome;
                        //mOrcCanalDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodoCanalporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodoCanalDetalhadoporProduto.Create(mOrcCanalDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3:
                    #region 3º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3)))
                    {
                        mOrcCanalDetalhadoporProduto = new Model.OrcamentodoCanalDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcCanalDetalhadoporProduto.OrcamentodoCanalporProduto = new Lookup(mOrcamentodoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporProduto>());
                        mOrcCanalDetalhadoporProduto.Ano = mOrcamentodoCanalporProduto.Ano;
                        mOrcCanalDetalhadoporProduto.Mes = (int)mes;
                        mOrcCanalDetalhadoporProduto.Canal = new Lookup(mOrcamentodoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mOrcCanalDetalhadoporProduto.Trimestre = mOrcamentodoCanalporProduto.Trimestre;
                        mOrcCanalDetalhadoporProduto.Produto = new Lookup(mOrcamentodoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcCanalDetalhadoporProduto.Nome = mOrcamentodoCanalporProduto.Nome;
                        //mOrcCanalDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodoCanalporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodoCanalDetalhadoporProduto.Create(mOrcCanalDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4:
                    #region 4º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4)))
                    {
                        mOrcCanalDetalhadoporProduto = new Model.OrcamentodoCanalDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcCanalDetalhadoporProduto.OrcamentodoCanalporProduto = new Lookup(mOrcamentodoCanalporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodoCanalporProduto>());
                        mOrcCanalDetalhadoporProduto.Ano = mOrcamentodoCanalporProduto.Ano;
                        mOrcCanalDetalhadoporProduto.Mes = (int)mes;
                        mOrcCanalDetalhadoporProduto.Canal = new Lookup(mOrcamentodoCanalporProduto.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Conta>());
                        mOrcCanalDetalhadoporProduto.Trimestre = mOrcamentodoCanalporProduto.Trimestre;
                        mOrcCanalDetalhadoporProduto.Produto = new Lookup(mOrcamentodoCanalporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcCanalDetalhadoporProduto.Nome = mOrcamentodoCanalporProduto.Nome;
                        //mOrcCanalDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodoCanalporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodoCanalDetalhadoporProduto.Create(mOrcCanalDetalhadoporProduto);
                    }
                    #endregion
                    break;
            }

        }

        public void Calcular(OrcamentodoCanalporProduto mOrcamentodoCanalporProduto, Trimestre trimestre)
        {
            List<OrcamentodoCanalDetalhadoporProduto> lstOrcCanalDetalhadaProdutos = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ObterOrcCanalDetalhadoProdutos(mOrcamentodoCanalporProduto.ID.Value, trimestre.trimestre.Value);
            foreach (OrcamentodoCanalDetalhadoporProduto prod in lstOrcCanalDetalhadaProdutos)
            {
                #region
                if (prod.Mes == trimestre.Mes1)
                {
                    prod.OrcamentoPlanejado = trimestre.Mes1Vlr.Value;
                    prod.QtdePlanejada = trimestre.Mes1Qtde.Value;
                }
                else if (prod.Mes == trimestre.Mes2)
                {
                    prod.OrcamentoPlanejado = trimestre.Mes2Vlr.Value;
                    prod.QtdePlanejada = trimestre.Mes2Qtde.Value;
                }
                else if (prod.Mes == trimestre.Mes3)
                {
                    prod.OrcamentoPlanejado = trimestre.Mes3Vlr.Value;
                    prod.QtdePlanejada = trimestre.Mes3Qtde.Value;
                }

                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Update(prod);
                #endregion
            }


        }

        public void Calcular(OrcamentodoCanalporProduto mOrcamentodoCanalporProduto, Trimestre trimestre, ref decimal valor, ref int quantidade)
        {
            decimal vlr = 0;
            int qtde = 0;

            List<OrcamentodoCanalDetalhadoporProduto> lstOrcCanalDetalhadaProdutos = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ObterOrcCanalDetalhadoProdutos(mOrcamentodoCanalporProduto.ID.Value, trimestre.trimestre.Value);
            foreach (OrcamentodoCanalDetalhadoporProduto prod in lstOrcCanalDetalhadaProdutos)
            {
                #region
                if (prod.Mes == trimestre.Mes1)
                {
                    prod.OrcamentoPlanejado = trimestre.Mes1Vlr.HasValue ? trimestre.Mes1Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes1Qtde.HasValue ? trimestre.Mes1Qtde.Value : 0;
                }
                else if (prod.Mes == trimestre.Mes2)
                {
                    prod.OrcamentoPlanejado = trimestre.Mes2Vlr.HasValue ? trimestre.Mes2Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes2Qtde.HasValue ? trimestre.Mes2Qtde.Value : 0;
                }
                else if (prod.Mes == trimestre.Mes3)
                {
                    prod.OrcamentoPlanejado = trimestre.Mes3Vlr.HasValue ? trimestre.Mes3Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes3Qtde.HasValue ? trimestre.Mes3Qtde.Value : 0;
                }

                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Update(prod);
                vlr += prod.OrcamentoPlanejado.Value;
                qtde += (int)prod.QtdePlanejada;
                #endregion
            }

            valor = vlr;
            quantidade = qtde;
        }

        public void CriarManual(OrcamentoPorCanal mOrcamentoPorCanal, Trimestre trimestre)
        {
            Model.OrcamentodoCanalDetalhadoporProduto mOrcCanalDetalhadoporProduto;

            #region mes 1
            mOrcCanalDetalhadoporProduto = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ObterOrcamentoProdutoDetalhadoManual(mOrcamentoPorCanal.ID.Value, mOrcamentoPorCanal.UnidadedeNegocio.Id, mOrcamentoPorCanal.Canal.Id, mOrcamentoPorCanal.Ano.Value, trimestre.trimestre.Value, trimestre.Mes1.Value);
            if (mOrcCanalDetalhadoporProduto == null)
                mOrcCanalDetalhadoporProduto = new OrcamentodoCanalDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            mOrcCanalDetalhadoporProduto.Ano = mOrcamentoPorCanal.Ano;
            mOrcCanalDetalhadoporProduto.Trimestre = (int)trimestre.trimestre;
            mOrcCanalDetalhadoporProduto.Mes = (int)trimestre.Mes1;
            mOrcCanalDetalhadoporProduto.OrcamentoPlanejado = trimestre.Mes1Vlr;
            mOrcCanalDetalhadoporProduto.Canal = new Lookup(mOrcamentoPorCanal.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
            mOrcCanalDetalhadoporProduto.OrcamentodoCanal = new Lookup(mOrcamentoPorCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<OrcamentoPorCanal>());
            //mOrcCanalDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentoPorCanal.UnidadedeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

            if (mOrcCanalDetalhadoporProduto.ID.HasValue)
                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Update(mOrcCanalDetalhadoporProduto);
            else
                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Create(mOrcCanalDetalhadoporProduto);
            #endregion

            #region mes 2
            mOrcCanalDetalhadoporProduto = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ObterOrcamentoProdutoDetalhadoManual(mOrcamentoPorCanal.ID.Value, mOrcamentoPorCanal.UnidadedeNegocio.Id, mOrcamentoPorCanal.Canal.Id, mOrcamentoPorCanal.Ano.Value, trimestre.trimestre.Value, trimestre.Mes2.Value);
            if (mOrcCanalDetalhadoporProduto == null)
                mOrcCanalDetalhadoporProduto = new OrcamentodoCanalDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

            mOrcCanalDetalhadoporProduto.Ano = mOrcamentoPorCanal.Ano;
            mOrcCanalDetalhadoporProduto.Trimestre = (int)trimestre.trimestre;
            mOrcCanalDetalhadoporProduto.Mes = (int)trimestre.Mes2;
            mOrcCanalDetalhadoporProduto.OrcamentoPlanejado = trimestre.Mes2Vlr;
            mOrcCanalDetalhadoporProduto.Canal = new Lookup(mOrcamentoPorCanal.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
            mOrcCanalDetalhadoporProduto.OrcamentodoCanal = new Lookup(mOrcamentoPorCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<OrcamentoPorCanal>());
            //mOrcCanalDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentoPorCanal.UnidadedeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

            if (mOrcCanalDetalhadoporProduto.ID.HasValue)
                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Update(mOrcCanalDetalhadoporProduto);
            else
                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Create(mOrcCanalDetalhadoporProduto);
            #endregion

            #region mes 3
            mOrcCanalDetalhadoporProduto = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ObterOrcamentoProdutoDetalhadoManual(mOrcamentoPorCanal.ID.Value, mOrcamentoPorCanal.UnidadedeNegocio.Id, mOrcamentoPorCanal.Canal.Id, mOrcamentoPorCanal.Ano.Value, trimestre.trimestre.Value, trimestre.Mes3.Value);
            if (mOrcCanalDetalhadoporProduto == null)
                mOrcCanalDetalhadoporProduto = new OrcamentodoCanalDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
            mOrcCanalDetalhadoporProduto.Ano = mOrcamentoPorCanal.Ano;
            mOrcCanalDetalhadoporProduto.Trimestre = (int)trimestre.trimestre;
            mOrcCanalDetalhadoporProduto.Mes = (int)trimestre.Mes3;
            mOrcCanalDetalhadoporProduto.OrcamentoPlanejado = trimestre.Mes3Vlr;
            mOrcCanalDetalhadoporProduto.Canal = new Lookup(mOrcamentoPorCanal.Canal.Id, SDKore.Crm.Util.Utility.GetEntityName<Conta>());
            mOrcCanalDetalhadoporProduto.OrcamentodoCanal = new Lookup(mOrcamentoPorCanal.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<OrcamentoPorCanal>());
            //mOrcCanalDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentoPorCanal.UnidadedeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

            if (mOrcCanalDetalhadoporProduto.ID.HasValue)
                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Update(mOrcCanalDetalhadoporProduto);
            else
                RepositoryService.OrcamentodoCanalDetalhadoporProduto.Create(mOrcCanalDetalhadoporProduto);
            #endregion
        }

        public void RetornoDWCanalDetalhadoProduto(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoCanalProdutoMes = RepositoryService.OrcamentodoCanalDetalhadoporProduto.ListarProdutoDetalhadoCanalDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoCanalProdutoMes.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["cd_unidade_negocio"].ToString());
                Conta mConta = RepositoryService.Conta.ObterCanal(item["CD_Emitente"].ToString());
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());

                if (mUnidadeNegocio != null && mConta != null && mProduto != null)
                {
                    var itemTrimestre = RepositoryService.OrcamentodoCanalDetalhadoporProduto
                        .ObterOrcamentoProdutoDetalhado(mUnidadeNegocio.ID.Value, mConta.ID.Value, mProduto.ID.Value, ano, trimestre, (int)item["cd_mes"]);

                    if (itemTrimestre != null)
                    {
                        itemTrimestre.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());
                        itemTrimestre.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                        RepositoryService.OrcamentodoCanalDetalhadoporProduto.Update(itemTrimestre);
                    }
                }
            }
        }

        #endregion
    }
}