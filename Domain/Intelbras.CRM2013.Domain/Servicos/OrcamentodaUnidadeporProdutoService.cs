using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Intelbras.CRM2013.Domain.Servicos
{
    public class OrcamentodaUnidadeporProdutoService
    {
        #region Construtores

        private RepositoryService RepositoryService { get; set; }

        public OrcamentodaUnidadeporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }


        public OrcamentodaUnidadeporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }

        #endregion

        #region Propertys/Objetos
        OrcamentodaUnidadeDetalhadoporProdutoService _ServiceOrcamentodaUnidadeDetalhadoporProduto = null;
        private OrcamentodaUnidadeDetalhadoporProdutoService ServiceOrcamentodaUnidadeDetalhadoporProduto
        {
            get
            {
                if (_ServiceOrcamentodaUnidadeDetalhadoporProduto == null)
                    _ServiceOrcamentodaUnidadeDetalhadoporProduto = new OrcamentodaUnidadeDetalhadoporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceOrcamentodaUnidadeDetalhadoporProduto;
            }
        }
        #endregion

        #region Método
        public void Criar(Guid orcamentounidadeId, OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid subfamiliaId, Guid orcamentosegmentoid)
        {
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto = null;
            OrcamentodaUnidadeporProduto mOrcUnidadeProduto = null;

            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}/{3}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id, x.Produto.Id));

            foreach (var OrcaProduto in lstOrcamentoporSegSubFamilia)
            {
                if (OrcaProduto.First().Produto.Id != Guid.Empty)
                {
                    mOrcUnidadeProduto = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcamentoporProduto(OrcaProduto.First().Produto.Id, mOrcamentodaUnidadeporSubFamilia.ID.Value);
                    if (mOrcUnidadeProduto == null)
                    {
                        #region
                        mOrcamentodaUnidadeporProduto = new OrcamentodaUnidadeporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        mOrcamentodaUnidadeporProduto.ID = Guid.NewGuid();
                        mOrcamentodaUnidadeporProduto.Ano = mOrcamentodaUnidadeporSubFamilia.Ano;
                        mOrcamentodaUnidadeporProduto.Trimestre = mOrcamentodaUnidadeporSubFamilia.Trimestre;
                        mOrcamentodaUnidadeporProduto.Produto = new Lookup(OrcaProduto.First().Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mOrcamentodaUnidadeporProduto.OrcamentoporSubFamilia = new Lookup(mOrcamentodaUnidadeporSubFamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporSubFamilia>());
                        mOrcamentodaUnidadeporProduto.Nome = OrcaProduto.First().Produto.Name;
                        //mOrcamentodaUnidadeporProduto.UnidadeNegocio = new Lookup(mOrcamentodaUnidadeporSubFamilia.UnidadedeNegocio.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.UnidadeNegocio>());

                        RepositoryService.OrcamentodaUnidadeporProduto.Create(mOrcamentodaUnidadeporProduto);
                        ServiceOrcamentodaUnidadeDetalhadoporProduto.Criar(mOrcamentodaUnidadeporProduto);
                        #endregion
                    }
                }
            }
        }

        public void Atualizar(OrcamentoDetalhado mOrcamentoDetalhado, OrcamentodaUnidadeporSubFamilia mOrcUnidporSubFamilia, Trimestre trimestre)
        {
            decimal valor = 0;
            int quantidade = 0;

            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcamentoporProduto(mOrcamentoDetalhado.ProdutoID.Value, mOrcUnidporSubFamilia.ID.Value);
            mOrcamentodaUnidadeporProduto.OrcamentoPlanejado = trimestre.Mes1Vlr.Value + trimestre.Mes2Vlr.Value + trimestre.Mes3Vlr.Value;
            mOrcamentodaUnidadeporProduto.QtdePlanejada = trimestre.Mes1Qtde.Value + trimestre.Mes2Qtde.Value + trimestre.Mes3Qtde.Value;

            ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto, trimestre, ref valor, ref quantidade);
            RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto);
        }

        public void Calcular(OrcamentodaUnidadeporSubFamilia mOrcamentodaUnidadeporSubFamilia, OrcamentoDetalhado mOrcamentoDetalhado)
        {
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcamentoporProduto(mOrcamentoDetalhado.ProdutoID.Value, mOrcamentodaUnidadeporSubFamilia.ID.Value);
            mOrcamentodaUnidadeporProduto.OrcamentoPlanejado = mOrcamentoDetalhado.Trimestre1.Mes1Vlr.Value + mOrcamentoDetalhado.Trimestre1.Mes2Vlr.Value + mOrcamentoDetalhado.Trimestre1.Mes3Vlr.Value;
            mOrcamentodaUnidadeporProduto.QtdePlanejada = mOrcamentoDetalhado.Trimestre1.Mes1Qtde.Value + mOrcamentoDetalhado.Trimestre1.Mes2Qtde.Value + mOrcamentoDetalhado.Trimestre1.Mes3Qtde.Value;
            //RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto);


        }

        public void Atualizar(object mOrcamentoDetalhado)
        {
            #region variable and object
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto1;
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto2;
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto3;
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto4;
            #endregion

            Type myType = mOrcamentoDetalhado.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            //foreach (PropertyInfo prop in props)
            //{
            //    object propValue = prop.GetValue(mOrcamentoDetalhado, null);
            //}

            //trimestre1
            if (bool.Parse(props[4].GetValue(mOrcamentoDetalhado, null).ToString()))
            {
                mOrcamentodaUnidadeporProduto1 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(Guid.Parse(props[8].GetValue(mOrcamentoDetalhado, null).ToString()), Guid.Parse(props[0].GetValue(mOrcamentoDetalhado, null).ToString()));
                if (mOrcamentodaUnidadeporProduto1 != null)
                {
                    //mOrcamentoDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto1, mOrcamentoDetalhado);
                    mOrcamentodaUnidadeporProduto1.OrcamentoPlanejado = decimal.Parse(props[10].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[12].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[14].GetValue(mOrcamentoDetalhado, null).ToString());
                    mOrcamentodaUnidadeporProduto1.QtdePlanejada = decimal.Parse(props[11].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[13].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[15].GetValue(mOrcamentoDetalhado, null).ToString());
                    RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto1);
                }
            }

            //trimestre2
            if (bool.Parse(props[5].GetValue(mOrcamentoDetalhado, null).ToString()))
            {
                mOrcamentodaUnidadeporProduto2 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(Guid.Parse(props[8].GetValue(mOrcamentoDetalhado, null).ToString()), Guid.Parse(props[1].GetValue(mOrcamentoDetalhado, null).ToString()));
                if (mOrcamentodaUnidadeporProduto2 != null)
                {
                    //mOrcamentoDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto2, mOrcamentoDetalhado);
                    mOrcamentodaUnidadeporProduto2.OrcamentoPlanejado = decimal.Parse(props[16].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[18].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[20].GetValue(mOrcamentoDetalhado, null).ToString());
                    mOrcamentodaUnidadeporProduto2.QtdePlanejada = decimal.Parse(props[17].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[19].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[21].GetValue(mOrcamentoDetalhado, null).ToString());
                    RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto2);
                }
            }

            //trimestre3
            if (bool.Parse(props[6].GetValue(mOrcamentoDetalhado, null).ToString()))
            {
                mOrcamentodaUnidadeporProduto3 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(Guid.Parse(props[8].GetValue(mOrcamentoDetalhado, null).ToString()), Guid.Parse(props[2].GetValue(mOrcamentoDetalhado, null).ToString()));
                if (mOrcamentodaUnidadeporProduto3 != null)
                {
                    //mOrcamentoDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto3, mOrcamentoDetalhado);
                    mOrcamentodaUnidadeporProduto3.OrcamentoPlanejado = decimal.Parse(props[22].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[24].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[26].GetValue(mOrcamentoDetalhado, null).ToString());
                    mOrcamentodaUnidadeporProduto3.QtdePlanejada = decimal.Parse(props[23].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[25].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[27].GetValue(mOrcamentoDetalhado, null).ToString());
                    RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto3);
                }
            }

            //trimestre4
            if (bool.Parse(props[7].GetValue(mOrcamentoDetalhado, null).ToString()))
            {
                mOrcamentodaUnidadeporProduto4 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(Guid.Parse(props[8].GetValue(mOrcamentoDetalhado, null).ToString()), Guid.Parse(props[3].GetValue(mOrcamentoDetalhado, null).ToString()));
                if (mOrcamentodaUnidadeporProduto4 != null)
                {
                    //mOrcamentoDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto4, mOrcamentoDetalhado);
                    mOrcamentodaUnidadeporProduto4.OrcamentoPlanejado = decimal.Parse(props[28].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[30].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[32].GetValue(mOrcamentoDetalhado, null).ToString());
                    mOrcamentodaUnidadeporProduto4.QtdePlanejada = decimal.Parse(props[29].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[31].GetValue(mOrcamentoDetalhado, null).ToString()) + decimal.Parse(props[33].GetValue(mOrcamentoDetalhado, null).ToString());
                    RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto4);
                }
            }
        }

        public void Atualizar(OrcamentoDetalhado mOrcamentoDetalhado)
        {
            #region variable and object
            decimal valor = 0;
            int quantidade = 0;

            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto1;
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto2;
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto3;
            OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto4;
            #endregion

            if (mOrcamentoDetalhado.AtualizarTrimestre1)
            {
                mOrcamentodaUnidadeporProduto1 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre1.Id.Value);
                mOrcamentoDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto1, mOrcamentoDetalhado.Trimestre1, ref valor, ref quantidade);
                mOrcamentodaUnidadeporProduto1.OrcamentoPlanejado = valor;
                mOrcamentodaUnidadeporProduto1.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto1);
            }

            if (mOrcamentoDetalhado.AtualizarTrimestre2)
            {
                mOrcamentodaUnidadeporProduto2 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre2.Id.Value);
                mOrcamentoDetalhado.Trimestre2.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto2, mOrcamentoDetalhado.Trimestre2, ref valor, ref quantidade);
                mOrcamentodaUnidadeporProduto2.OrcamentoPlanejado = valor;
                mOrcamentodaUnidadeporProduto2.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto2);
            }

            if (mOrcamentoDetalhado.AtualizarTrimestre3)
            {
                mOrcamentodaUnidadeporProduto3 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre3.Id.Value);
                mOrcamentoDetalhado.Trimestre3.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto3, mOrcamentoDetalhado.Trimestre3, ref valor, ref quantidade);
                mOrcamentodaUnidadeporProduto3.OrcamentoPlanejado = valor;
                mOrcamentodaUnidadeporProduto3.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto3);
            }

            if (mOrcamentoDetalhado.AtualizarTrimestre4)
            {
                mOrcamentodaUnidadeporProduto4 = RepositoryService.OrcamentodaUnidadeporProduto.ObterOrcUnidadeProduto(mOrcamentoDetalhado.ProdutoID.Value, mOrcamentoDetalhado.Trimestre4.Id.Value);
                mOrcamentoDetalhado.Trimestre4.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                ServiceOrcamentodaUnidadeDetalhadoporProduto.Calcular(mOrcamentodaUnidadeporProduto4, mOrcamentoDetalhado.Trimestre4, ref valor, ref quantidade);
                mOrcamentodaUnidadeporProduto4.OrcamentoPlanejado = valor;
                mOrcamentodaUnidadeporProduto4.QtdePlanejada = quantidade;
                RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto4);
            }
        }

        public void RetornoDWTrimestreProduto(int ano, int trimestre)
        {
            List<OrcamentodaUnidade> lstOrcamentodaUnidade = RepositoryService.OrcamentodaUnidade.ListarOrcamentos(ano);
            if (lstOrcamentodaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoTrimestreSeg = RepositoryService.OrcamentodaUnidadeporProduto.ListarOrcamentoProdutoDW(ano, trimestre, lstOrcamentodaUnidade);

            foreach (DataRow item in dtOrcamentoTrimestreSeg.Rows)
            {
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());

                if (mProduto != null & mUnidadeNegocio != null)
                {
                    OrcamentodaUnidadeporProduto mOrcamentodaUnidadeporProduto = RepositoryService.OrcamentodaUnidadeporProduto
                        .ObterOrcamentoporProduto(mProduto.ID.Value, mUnidadeNegocio.ID.Value, ano, trimestre, item["cd_segmento"].ToString(), item["cd_familia"].ToString(), item["cd_subfamilia"].ToString());

                    if (mOrcamentodaUnidadeporProduto != null)
                    {
                        mOrcamentodaUnidadeporProduto.OrcamentoRealizado = decimal.Parse(item["vlr"].ToString());
                        mOrcamentodaUnidadeporProduto.QtdeRealizada = decimal.Parse(item["Qtde"].ToString());

                        RepositoryService.OrcamentodaUnidadeporProduto.Update(mOrcamentodaUnidadeporProduto);
                    }
                }
            }
        }

        #endregion
    }
}

