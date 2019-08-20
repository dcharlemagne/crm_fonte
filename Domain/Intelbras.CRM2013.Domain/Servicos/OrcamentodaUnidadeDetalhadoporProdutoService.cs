using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class OrcamentodaUnidadeDetalhadoporProdutoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeDetalhadoporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public OrcamentodaUnidadeDetalhadoporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Métodos
        public void Criar(Model.OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto)
        {
            Model.OrcamentodaUnidadeDetalhadoporProduto mOrcUnidadeDetalhadoporProduto;

            switch (mOrcamentodaUnidadeporProduto.Trimestre)
            {
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre1:
                    #region 1º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre1)))
                    {
                        mOrcUnidadeDetalhadoporProduto = new Model.OrcamentodaUnidadeDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcUnidadeDetalhadoporProduto.OrcamentodoProduto = new Lookup(mOrcamentodaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mOrcUnidadeDetalhadoporProduto.Ano = mOrcamentodaUnidadeporProduto.Ano;
                        mOrcUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mOrcUnidadeDetalhadoporProduto.Trimestre = mOrcamentodaUnidadeporProduto.Trimestre;
                        mOrcUnidadeDetalhadoporProduto.Produto = new Lookup(mOrcamentodaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcUnidadeDetalhadoporProduto.Nome = mOrcamentodaUnidadeporProduto.Nome;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoPlanejado = 0;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoRealizado = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdeRealizada = 0;
                        //mOrcUnidadeDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodaUnidadeporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.Create(mOrcUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre2:
                    #region 2º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre2)))
                    {
                        mOrcUnidadeDetalhadoporProduto = new Model.OrcamentodaUnidadeDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcUnidadeDetalhadoporProduto.OrcamentodoProduto = new Lookup(mOrcamentodaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mOrcUnidadeDetalhadoporProduto.Ano = mOrcamentodaUnidadeporProduto.Ano;
                        mOrcUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mOrcUnidadeDetalhadoporProduto.Trimestre = mOrcamentodaUnidadeporProduto.Trimestre;
                        mOrcUnidadeDetalhadoporProduto.Produto = new Lookup(mOrcamentodaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcUnidadeDetalhadoporProduto.Nome = mOrcamentodaUnidadeporProduto.Nome;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoPlanejado = 0;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoRealizado = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdeRealizada = 0;
                        //mOrcUnidadeDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodaUnidadeporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.Create(mOrcUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre3:
                    #region 3º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre3)))
                    {
                        mOrcUnidadeDetalhadoporProduto = new Model.OrcamentodaUnidadeDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcUnidadeDetalhadoporProduto.OrcamentodoProduto = new Lookup(mOrcamentodaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mOrcUnidadeDetalhadoporProduto.Ano = mOrcamentodaUnidadeporProduto.Ano;
                        mOrcUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mOrcUnidadeDetalhadoporProduto.Trimestre = mOrcamentodaUnidadeporProduto.Trimestre;
                        mOrcUnidadeDetalhadoporProduto.Produto = new Lookup(mOrcamentodaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcUnidadeDetalhadoporProduto.Nome = mOrcamentodaUnidadeporProduto.Nome;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoPlanejado = 0;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoRealizado = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdeRealizada = 0;
                        //mOrcUnidadeDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodaUnidadeporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.Create(mOrcUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
                case (int)Enum.OrcamentodaUnidade.Trimestres.Trimestre4:
                    #region 4º Trimestre
                    foreach (var mes in System.Enum.GetValues(typeof(Domain.Enum.OrcamentodaUnidadeDetalhadoporProduto.Trimestre4)))
                    {
                        mOrcUnidadeDetalhadoporProduto = new Model.OrcamentodaUnidadeDetalhadoporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);
                        mOrcUnidadeDetalhadoporProduto.OrcamentodoProduto = new Lookup(mOrcamentodaUnidadeporProduto.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporProduto>());
                        mOrcUnidadeDetalhadoporProduto.Ano = mOrcamentodaUnidadeporProduto.Ano;
                        mOrcUnidadeDetalhadoporProduto.Mes = (int)mes;
                        mOrcUnidadeDetalhadoporProduto.Trimestre = mOrcamentodaUnidadeporProduto.Trimestre;
                        mOrcUnidadeDetalhadoporProduto.Produto = new Lookup(mOrcamentodaUnidadeporProduto.Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcUnidadeDetalhadoporProduto.Nome = mOrcamentodaUnidadeporProduto.Nome;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoPlanejado = 0;
                        mOrcUnidadeDetalhadoporProduto.OrcamentoRealizado = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdePlanejada = 0;
                        mOrcUnidadeDetalhadoporProduto.QtdeRealizada = 0;
                        //mOrcUnidadeDetalhadoporProduto.UnidadeNegocio = new Lookup(mOrcamentodaUnidadeporProduto.UnidadeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.Create(mOrcUnidadeDetalhadoporProduto);
                    }
                    #endregion
                    break;
            }

        }

        public void Calcular(OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto, object itens)
        {
            Type myType = itens.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            List<OrcamentodaUnidadeDetalhadoporProduto> lstOrcDetalhadaProdutos = RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.ObterOrcDetalhadoProdutos(mOrcamentodaUnidadeporProduto.ID.Value);
            foreach (OrcamentodaUnidadeDetalhadoporProduto prod in lstOrcDetalhadaProdutos)
            {
                #region
                if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Janeiro)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[10].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[11].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Fevereiro)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[12].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[13].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Marco)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[14].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[15].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Abril)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[16].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[17].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Maio)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[18].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[19].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Junho)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[20].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[21].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Julho)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[22].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[23].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Agosto)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[24].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[25].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Setembro)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[26].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[27].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Outubro)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[28].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[29].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Novembro)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[30].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[31].GetValue(itens, null).ToString());
                }
                else if (prod.Mes == (int)Enum.OrcamentodaUnidadeDetalhadoporProduto.Mes.Dezembro)
                {
                    prod.OrcamentoPlanejado = decimal.Parse(props[32].GetValue(itens, null).ToString());
                    prod.QtdePlanejada = decimal.Parse(props[33].GetValue(itens, null).ToString());
                }

                RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.Update(prod);
                #endregion
            }

        }

        public void Calcular(OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto, Trimestre trimestre, ref decimal valor, ref int quantidade)
        {
            decimal vlr = 0;
            int qtde = 0;

            List<OrcamentodaUnidadeDetalhadoporProduto> lstOrcDetalhadaProdutos = RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.ObterOrcDetalhadoProdutos(mOrcamentodaUnidadeporProduto.ID.Value, trimestre.trimestre.Value);
            foreach (OrcamentodaUnidadeDetalhadoporProduto prod in lstOrcDetalhadaProdutos)
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

                RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.Update(prod);
                vlr += prod.OrcamentoPlanejado.Value;
                qtde += (int)prod.QtdePlanejada;
                #endregion
            }

            valor = vlr;
            quantidade = qtde;

        }

        public void RetornoDWTrimestreProdutoMes(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);
            
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoTrimestreSeg = RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.ListarOrcamentoProdutoMesDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoTrimestreSeg.Rows)
            {
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());

                if (mProduto != null && mUnidadeNegocio != null)
                {
                    OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcamentoporProduto(mProduto.ID.Value,
                        mUnidadeNegocio.ID.Value, ano, trimestre, item["cd_segmento"].ToString(), item["cd_familia"].ToString(), item["cd_subfamilia"].ToString());

                    if (mOrcamentodaUnidadeporProduto != null)
                    {
                        OrcamentodaUnidadeDetalhadoporProduto mOrcamentodaUnidadeDetalhadoporProduto = RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.ObterOrcamentoProdutoDetalhado(mOrcamentodaUnidadeporProduto.ID.Value, Convert.ToInt32(item["cd_mes"].ToString()));

                        if (mOrcamentodaUnidadeDetalhadoporProduto != null)
                        {
                            mOrcamentodaUnidadeDetalhadoporProduto.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());
                            mOrcamentodaUnidadeDetalhadoporProduto.QtdeRealizada = decimal.Parse(item["Qtde"].ToString());

                            RepositoryService.OrcamentodaUnidadeDetalhadoporProduto.Update(mOrcamentodaUnidadeDetalhadoporProduto);
                        }
                    }
                }
            }
        }

        #endregion
    }
}

