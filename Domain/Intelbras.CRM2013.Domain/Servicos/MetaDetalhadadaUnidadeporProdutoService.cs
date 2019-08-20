using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class MetaDetalhadadaUnidadeporProdutoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetaDetalhadadaUnidadeporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetaDetalhadadaUnidadeporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }


        #endregion

        #region Métodos
        public void Criar(Model.MetadaUnidadeporProduto mMetadaUnidadeporProduto)
        {
            Model.MetaDetalhadadaUnidadeporProduto mMetaUnidadeDetalhadoporProduto;

            switch (mMetadaUnidadeporProduto.Trimestre)
            {
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1:
                    #region 1º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1)))
                    {
                        mMetaUnidadeDetalhadoporProduto = new Model.MetaDetalhadadaUnidadeporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaUnidadeDetalhadoporProduto.MetadoProduto = new Lookup(mMetadaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mMetaUnidadeDetalhadoporProduto.Ano = mMetadaUnidadeporProduto.Ano;
                        mMetaUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mMetaUnidadeDetalhadoporProduto.Trimestre = mMetadaUnidadeporProduto.Trimestre;
                        mMetaUnidadeDetalhadoporProduto.Produto = new Lookup(mMetadaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaUnidadeDetalhadoporProduto.Nome = mMetadaUnidadeporProduto.Nome;
                        mMetaUnidadeDetalhadoporProduto.MetaPlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.MetaRealizada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdeRealizada = 0;
                        RepositoryService.MetadaUnidadeDetalhadaProduto.Create(mMetaUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2:
                    #region 2º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2)))
                    {
                        mMetaUnidadeDetalhadoporProduto = new Model.MetaDetalhadadaUnidadeporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaUnidadeDetalhadoporProduto.MetadoProduto = new Lookup(mMetadaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mMetaUnidadeDetalhadoporProduto.Ano = mMetadaUnidadeporProduto.Ano;
                        mMetaUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mMetaUnidadeDetalhadoporProduto.Trimestre = mMetadaUnidadeporProduto.Trimestre;
                        mMetaUnidadeDetalhadoporProduto.Produto = new Lookup(mMetadaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaUnidadeDetalhadoporProduto.Nome = mMetadaUnidadeporProduto.Nome;
                        mMetaUnidadeDetalhadoporProduto.MetaPlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.MetaRealizada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdeRealizada = 0;

                        RepositoryService.MetadaUnidadeDetalhadaProduto.Create(mMetaUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3:
                    #region 3º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3)))
                    {
                        mMetaUnidadeDetalhadoporProduto = new Model.MetaDetalhadadaUnidadeporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaUnidadeDetalhadoporProduto.MetadoProduto = new Lookup(mMetadaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mMetaUnidadeDetalhadoporProduto.Ano = mMetadaUnidadeporProduto.Ano;
                        mMetaUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mMetaUnidadeDetalhadoporProduto.Trimestre = mMetadaUnidadeporProduto.Trimestre;
                        mMetaUnidadeDetalhadoporProduto.Produto = new Lookup(mMetadaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaUnidadeDetalhadoporProduto.Nome = mMetadaUnidadeporProduto.Nome;
                        mMetaUnidadeDetalhadoporProduto.MetaPlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.MetaRealizada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdeRealizada = 0;

                        RepositoryService.MetadaUnidadeDetalhadaProduto.Create(mMetaUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4:
                    #region 4º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4)))
                    {
                        mMetaUnidadeDetalhadoporProduto = new Model.MetaDetalhadadaUnidadeporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mMetaUnidadeDetalhadoporProduto.MetadoProduto = new Lookup(mMetadaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mMetaUnidadeDetalhadoporProduto.Ano = mMetadaUnidadeporProduto.Ano;
                        mMetaUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mMetaUnidadeDetalhadoporProduto.Trimestre = mMetadaUnidadeporProduto.Trimestre;
                        mMetaUnidadeDetalhadoporProduto.Produto = new Lookup(mMetadaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetaUnidadeDetalhadoporProduto.Nome = mMetadaUnidadeporProduto.Nome;
                        mMetaUnidadeDetalhadoporProduto.MetaPlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.MetaRealizada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mMetaUnidadeDetalhadoporProduto.QtdeRealizada = 0;

                        RepositoryService.MetadaUnidadeDetalhadaProduto.Create(mMetaUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
            }

        }

        public void Calcular(MetadaUnidadeporProduto mMetadaUnidadeporProduto, object itens)
        {
            Type myType = itens.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            List<MetaDetalhadadaUnidadeporProduto> lstMetaDetalhadaProdutos = RepositoryService.MetadaUnidadeDetalhadaProduto.Listar(mMetadaUnidadeporProduto.ID.Value);
            foreach (MetaDetalhadadaUnidadeporProduto prod in lstMetaDetalhadaProdutos)
            {
                #region
                if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro)
                {
                    prod.MetaPlanejada = decimal.Parse(props[10].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[11].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro)
                {
                    prod.MetaPlanejada = decimal.Parse(props[12].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[13].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco)
                {
                    prod.MetaPlanejada = decimal.Parse(props[14].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[15].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril)
                {
                    prod.MetaPlanejada = decimal.Parse(props[16].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[17].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio)
                {
                    prod.MetaPlanejada = decimal.Parse(props[18].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[19].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho)
                {
                    prod.MetaPlanejada = decimal.Parse(props[20].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[21].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho)
                {
                    prod.MetaPlanejada = decimal.Parse(props[22].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[23].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto)
                {
                    prod.MetaPlanejada = decimal.Parse(props[24].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[25].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro)
                {
                    prod.MetaPlanejada = decimal.Parse(props[26].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[27].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro)
                {
                    prod.MetaPlanejada = decimal.Parse(props[28].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[29].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro)
                {
                    prod.MetaPlanejada = decimal.Parse(props[30].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[31].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro)
                {
                    prod.MetaPlanejada = decimal.Parse(props[32].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[33].GetValue(itens, null).ToString());
                }

                RepositoryService.MetadaUnidadeDetalhadaProduto.Update(prod);
                #endregion
            }
        }

        public void Calcular(MetadaUnidadeporProduto mMetadaUnidadeporProduto, Trimestre trimestre, ref decimal valor, ref int quantidade)
        {
            decimal vlr = 0;
            int qtde = 0;


            List<MetaDetalhadadaUnidadeporProduto> lstMetaDetalhadaProdutos = RepositoryService.MetadaUnidadeDetalhadaProduto.Obterpor(mMetadaUnidadeporProduto.ID.Value, trimestre.trimestre.Value);
            foreach (MetaDetalhadadaUnidadeporProduto prod in lstMetaDetalhadaProdutos)
            {
                #region
                if (prod.Mes == trimestre.Mes1)
                {
                    prod.MetaPlanejada = trimestre.Mes1Vlr.HasValue ? trimestre.Mes1Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes1Qtde.HasValue ? trimestre.Mes1Qtde.Value : 0;
                }
                else if (prod.Mes == trimestre.Mes2)
                {
                    prod.MetaPlanejada = trimestre.Mes2Vlr.HasValue ? trimestre.Mes2Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes2Qtde.HasValue ? trimestre.Mes2Qtde.Value : 0;
                }
                else if (prod.Mes == trimestre.Mes3)
                {
                    prod.MetaPlanejada = trimestre.Mes3Vlr.HasValue ? trimestre.Mes3Vlr.Value : 0;
                    prod.QtdePlanejada = trimestre.Mes3Qtde.HasValue ? trimestre.Mes3Qtde.Value : 0;
                }

                RepositoryService.MetadaUnidadeDetalhadaProduto.Update(prod);
                vlr += prod.MetaPlanejada.Value;
                qtde += (int)prod.QtdePlanejada;
                #endregion
            }

            valor = vlr;
            quantidade = qtde;

        }

        public void RetornoDWMetaProdutoDetalhado(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoProdDetalhado = RepositoryService.MetadaUnidadeDetalhadaProduto.ListarProdutoDetalhadoDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtOrcamentoProdDetalhado.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());

                if (mUnidadeNegocio != null && mProduto != null)
                {
                    var itemcapa = RepositoryService.MetadaUnidadeDetalhadaProduto.ObterProdutoDetalhado(mUnidadeNegocio.ID.Value, mProduto.ID.Value, Convert.ToInt32(item["cd_ano"].ToString()), (int)item["cd_trimestre"], (int)item["cd_mes"]);
                    if (itemcapa != null)
                    {
                        itemcapa.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                        itemcapa.QtdeRealizada = decimal.Parse(item["Qtde"].ToString());

                        RepositoryService.MetadaUnidadeDetalhadaProduto.Update(itemcapa);
                    }
                }
            }
        }
        #endregion
    }
}