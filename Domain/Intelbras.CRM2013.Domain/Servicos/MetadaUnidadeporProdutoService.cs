using Intelbras.CRM2013.Domain.Model;
using SDKore.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Intelbras.CRM2013.Domain.Servicos
{

    public class MetadaUnidadeporProdutoService
    {
        #region Construtores
        private RepositoryService RepositoryService { get; set; }

        public MetadaUnidadeporProdutoService(string organizacao, bool isOffline)
            : this(organizacao, isOffline, null)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline);
        }

        public MetadaUnidadeporProdutoService(string organizacao, bool isOffline, object provider)
        {
            RepositoryService = new RepositoryService(organizacao, isOffline, provider);
        }
        #endregion

        #region Objetos/propertys

        MetaDetalhadadaUnidadeporProdutoService _ServiceMetaDetalhadadaUnidadeporProduto = null;
        private MetaDetalhadadaUnidadeporProdutoService ServiceMetaDetalhadadaUnidadeporProduto
        {
            get
            {
                if (_ServiceMetaDetalhadadaUnidadeporProduto == null)
                    _ServiceMetaDetalhadadaUnidadeporProduto = new MetaDetalhadadaUnidadeporProdutoService(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline);

                return _ServiceMetaDetalhadadaUnidadeporProduto;
            }
        }

        #endregion

        #region Métodos
        public void Criar(Guid orcamentounidadeId, MetadaUnidadeporSubfamilia mMetadaUnidadeporSubfamilia, List<Model.OrcamentoDetalhado> lstOrcamentoDetalhado, Guid subfamiliaId, Guid metadosegmentoId)
        {
            MetadaUnidadeporProduto mMetadaUnidadeporProduto = null;
            MetadaUnidadeporProduto mMetaUnidadeProduto = null;

            var lstOrcamentoporSegSubFamilia = (from x in lstOrcamentoDetalhado
                                                group x by string.Format("{0}/{1}/{2}/{3}", x.Segmento.Id, x.Familia.Id, x.SubFamilia.Id, x.Produto.Id));

            foreach (var OrcaProduto in lstOrcamentoporSegSubFamilia)
            {
                if (OrcaProduto.First().Produto.Id != Guid.Empty)
                {
                    mMetaUnidadeProduto = RepositoryService.MetadaUnidadeporProduto.Obter(OrcaProduto.First().Produto.Id, mMetadaUnidadeporSubfamilia.ID.Value);
                    if (mMetaUnidadeProduto == null)
                    {
                        #region
                        mMetadaUnidadeporProduto = new MetadaUnidadeporProduto(RepositoryService.NomeDaOrganizacao, RepositoryService.IsOffline, RepositoryService.Provider);
                        mMetadaUnidadeporProduto.ID = Guid.NewGuid();
                        mMetadaUnidadeporProduto.Ano = mMetadaUnidadeporSubfamilia.Ano;
                        mMetadaUnidadeporProduto.Trimestre = mMetadaUnidadeporSubfamilia.Trimestre;
                        mMetadaUnidadeporProduto.Produto = new Lookup(OrcaProduto.First().Produto.Id, SDKore.Crm.Util.Utility.GetEntityName<Model.Product>());
                        mMetadaUnidadeporProduto.MetadaSubfamilia = new Lookup(mMetadaUnidadeporSubfamilia.ID.Value, SDKore.Crm.Util.Utility.GetEntityName<Model.OrcamentodaUnidadeporSubFamilia>());
                        mMetadaUnidadeporProduto.Nome = OrcaProduto.First().Produto.Name;

                        RepositoryService.MetadaUnidadeporProduto.Create(mMetadaUnidadeporProduto);
                        ServiceMetaDetalhadadaUnidadeporProduto.Criar(mMetadaUnidadeporProduto);
                        #endregion
                    }
                }
            }
        }

        public void Atualizar(object mMetaDetalhado)
        {
            #region variable and object
            MetadaUnidadeporProduto mMetadaUnidadeporProduto1;
            MetadaUnidadeporProduto mMetadaUnidadeporProduto2;
            MetadaUnidadeporProduto mMetadaUnidadeporProduto3;
            MetadaUnidadeporProduto mMetadaUnidadeporProduto4;
            #endregion

            Type myType = mMetaDetalhado.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            //trimestre1
            if (bool.Parse(props[4].GetValue(mMetaDetalhado, null).ToString()))
            {
                mMetadaUnidadeporProduto1 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(Guid.Parse(props[8].GetValue(mMetaDetalhado, null).ToString()), Guid.Parse(props[0].GetValue(mMetaDetalhado, null).ToString()));
                if (mMetadaUnidadeporProduto1 != null)
                {
                    //mMetaDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto1, mMetaDetalhado);
                    mMetadaUnidadeporProduto1.MetaPlanejada = decimal.Parse(props[10].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[12].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[14].GetValue(mMetaDetalhado, null).ToString());
                    mMetadaUnidadeporProduto1.QtdePlanejada = decimal.Parse(props[11].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[13].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[15].GetValue(mMetaDetalhado, null).ToString());
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto1);
                }
            }

            //trimestre2
            if (bool.Parse(props[5].GetValue(mMetaDetalhado, null).ToString()))
            {
                mMetadaUnidadeporProduto2 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(Guid.Parse(props[8].GetValue(mMetaDetalhado, null).ToString()), Guid.Parse(props[1].GetValue(mMetaDetalhado, null).ToString()));
                if (mMetadaUnidadeporProduto2 != null)
                {
                    //mMetaDetalhado.Trimestre2.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto2, mMetaDetalhado);
                    mMetadaUnidadeporProduto2.MetaPlanejada = decimal.Parse(props[16].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[18].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[20].GetValue(mMetaDetalhado, null).ToString());
                    mMetadaUnidadeporProduto2.QtdePlanejada = decimal.Parse(props[17].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[19].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[21].GetValue(mMetaDetalhado, null).ToString());
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto2);
                }
            }

            //trimestre3
            if (bool.Parse(props[6].GetValue(mMetaDetalhado, null).ToString()))
            {
                mMetadaUnidadeporProduto3 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(Guid.Parse(props[8].GetValue(mMetaDetalhado, null).ToString()), Guid.Parse(props[2].GetValue(mMetaDetalhado, null).ToString()));
                if (mMetadaUnidadeporProduto3 != null)
                {
                    //mMetaDetalhado.Trimestre3.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto3, mMetaDetalhado);
                    mMetadaUnidadeporProduto3.MetaPlanejada = decimal.Parse(props[22].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[24].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[26].GetValue(mMetaDetalhado, null).ToString());
                    mMetadaUnidadeporProduto3.QtdePlanejada = decimal.Parse(props[23].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[25].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[27].GetValue(mMetaDetalhado, null).ToString());
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto3);
                }
            }

            //trimestre4
            if (bool.Parse(props[7].GetValue(mMetaDetalhado, null).ToString()))
            {
                mMetadaUnidadeporProduto4 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(Guid.Parse(props[8].GetValue(mMetaDetalhado, null).ToString()), Guid.Parse(props[3].GetValue(mMetaDetalhado, null).ToString()));
                if (mMetadaUnidadeporProduto4 != null)
                {
                    //mMetaDetalhado.Trimestre4.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto4, mMetaDetalhado);                    
                    mMetadaUnidadeporProduto4.MetaPlanejada = decimal.Parse(props[28].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[30].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[32].GetValue(mMetaDetalhado, null).ToString());
                    mMetadaUnidadeporProduto4.QtdePlanejada = decimal.Parse(props[29].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[31].GetValue(mMetaDetalhado, null).ToString()) + decimal.Parse(props[33].GetValue(mMetaDetalhado, null).ToString());
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto4);
                }
            }
        }

        public void Atualizar(OrcamentoDetalhado mMetaDetalhado)
        {
            #region variable and object
            decimal valor = 0;
            int quantidade = 0;

            MetadaUnidadeporProduto mMetadaUnidadeporProduto1;
            MetadaUnidadeporProduto mMetadaUnidadeporProduto2;
            MetadaUnidadeporProduto mMetadaUnidadeporProduto3;
            MetadaUnidadeporProduto mMetadaUnidadeporProduto4;
            #endregion

            if (mMetaDetalhado.AtualizarTrimestre1)
            {
                mMetadaUnidadeporProduto1 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre1.Id.Value);
                if (mMetadaUnidadeporProduto1 != null)
                {
                    mMetaDetalhado.Trimestre1.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre1;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto1, mMetaDetalhado.Trimestre1, ref valor, ref quantidade);
                    mMetadaUnidadeporProduto1.MetaPlanejada = valor;
                    mMetadaUnidadeporProduto1.QtdePlanejada = quantidade;
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto1);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre2)
            {
                mMetadaUnidadeporProduto2 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre2.Id.Value);
                if (mMetadaUnidadeporProduto2 != null)
                {
                    mMetaDetalhado.Trimestre2.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre2;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto2, mMetaDetalhado.Trimestre2, ref valor, ref quantidade);
                    mMetadaUnidadeporProduto2.MetaPlanejada = valor;
                    mMetadaUnidadeporProduto2.QtdePlanejada = quantidade;
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto2);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre3)
            {
                mMetadaUnidadeporProduto3 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre3.Id.Value);
                if (mMetadaUnidadeporProduto3 != null)
                {
                    mMetaDetalhado.Trimestre3.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre3;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto3, mMetaDetalhado.Trimestre3, ref valor, ref quantidade);
                    mMetadaUnidadeporProduto3.MetaPlanejada = valor;
                    mMetadaUnidadeporProduto3.QtdePlanejada = quantidade;
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto3);
                }
            }

            if (mMetaDetalhado.AtualizarTrimestre4)
            {
                mMetadaUnidadeporProduto4 = RepositoryService.MetadaUnidadeporProduto.ObterporTrimestre(mMetaDetalhado.ProdutoID.Value, mMetaDetalhado.Trimestre4.Id.Value);
                if (mMetadaUnidadeporProduto4 != null)
                {
                    mMetaDetalhado.Trimestre4.trimestre = (int)Domain.Enum.OrcamentodaUnidade.Trimestres.Trimestre4;
                    ServiceMetaDetalhadadaUnidadeporProduto.Calcular(mMetadaUnidadeporProduto4, mMetaDetalhado.Trimestre4, ref valor, ref quantidade);
                    mMetadaUnidadeporProduto4.MetaPlanejada = valor;
                    mMetadaUnidadeporProduto4.QtdePlanejada = quantidade;
                    RepositoryService.MetadaUnidadeporProduto.Update(mMetadaUnidadeporProduto4);
                }
            }
        }

        public void RetornoDWMetaProduto(int ano, int trimestre)
        {
            List<MetadaUnidade> lstMetadaUnidade = RepositoryService.MetadaUnidade.ListarMetas(ano);
            if (lstMetadaUnidade.Count == 0)
                return;

            DataTable dtOrcamentoProduto = RepositoryService.MetadaUnidadeporProduto.ListarMetaProdutoDW(ano, trimestre, lstMetadaUnidade);

            foreach (DataRow item in dtOrcamentoProduto.Rows)
            {
                UnidadeNegocio mUnidadeNegocio = RepositoryService.UnidadeNegocio.ObterPorChaveIntegracao(item["CD_Unidade_Negocio"].ToString());
                Product mProduto = RepositoryService.Produto.ObterPor(item["cd_item"].ToString());

                if (mUnidadeNegocio != null && mProduto != null)
                {
                    var itemcapa = RepositoryService.MetadaUnidadeporProduto.ObterMetaProduto(mUnidadeNegocio.ID.Value, mProduto.ID.Value, ano, trimestre);

                    if (itemcapa != null)
                    {
                        itemcapa.MetaRealizada = decimal.Parse(item["vlr"].ToString());
                        itemcapa.QtdeRealizada = decimal.Parse(item["qtde"].ToString());

                        RepositoryService.MetadaUnidadeporProduto.Update(itemcapa);
                    }
                }
            }
        }

        #endregion
    }
}

