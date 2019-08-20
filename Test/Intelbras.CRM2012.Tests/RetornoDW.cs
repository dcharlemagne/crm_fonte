using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Intelbras.CRM2013.Domain.Model;
using Intelbras.CRM2013.Domain.Servicos;
using Microsoft.Crm.Sdk.Query;
using System.IO;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;


namespace Intelbras.CRM2013.Tests
{
    [TestFixture]
    public class RetornoW : Base
    {
        [Test]
        public void RetornoDW()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);
            #region Orcamento da Unidade
            //new OrcamentodaUnidadeService(this.OrganizationName, this.IsOffline).RetornoDWCapaOrcamento();
            //new OrcamentodaUnidadeporTrimestreService(this.OrganizationName, this.IsOffline).RetornoDWTrimestre();
            //new OrcamentodaUnidadeporSegmentoService(this.OrganizationName, this.IsOffline).RetornoDWTrimestreSegmento();
            //new OrcamentodaUnidadeporFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWTrimestreFamilia();
            //new OrcamentodaUnidadeporSubFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWTrimestreSubFamilia();
            //new OrcamentodaUnidadeporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWTrimestreProduto();
            //new OrcamentodaUnidadeDetalhadoporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWTrimestreProdutoMes();
            #endregion

            #region Orcamento do Canal / Orçamento do Canal Manual
            //new OrcamentodoCanalService(this.OrganizationName, this.IsOffline).RetornoDWCanalTrimestre();
            //new OrcamentodoCanalporSegmentoService(this.OrganizationName, this.IsOffline).RetornoDWCanalSegmento();
            //new OrcamentodoCanalporFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWCanalFamilia();
            //new OrcamentodoCanalporSubFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWCanalSubFamilia();
            //new OrcamentodoCanalporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWCanalProduto();
            //new OrcamentodoCanalDetalhadoporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWCanalDetalhadoProduto();
            #endregion

            #region Metas da Unidade
            //new MetadaUnidadeService(this.OrganizationName, this.IsOffline).RetornoDWCapaMeta();
            //new MetadaUnidadeporTrimestreService(this.OrganizationName, this.IsOffline).RetornoDWTrimestre();
            //new MetadaUnidadeporSegmentoService(this.OrganizationName, this.IsOffline).RetornoDWMetaSegmento();
            //new MetadaUnidadeporFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWMetaFamilia();
            //new MetadaUnidadeporSubfamiliaService(this.OrganizationName, this.IsOffline).RetornoDWMetaSubFamilia();
            //new MetadaUnidadeporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWMetaProduto();
            //new MetaDetalhadadaUnidadeporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWMetaProdutoDetalhado();
            #endregion

            #region Metas do Canal
            //new MetadoCanalService(this.OrganizationName, this.IsOffline).RetornoDWMetaCanal();
            //new MetadoCanalporSegmentoService(this.OrganizationName, this.IsOffline).RetornoDWMetaSegmento();
            //new MetadoCanalporFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWMetaCanalFamilia();
            //new MetadoCanalporSubFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWMetaCanalSubFamilia();
            //new MetadoCanalporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWMetaCanalProduto();
            //new MetaDetalhadadoCanalporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWMetaCanalProdutoDetalhado();
            #endregion

            #region Meta Ka/Representante
            //new PotencialdoKARepresentanteService(this.OrganizationName, this.IsOffline).RetornoDWKaRepresentante();
            //new PotencialdoKAporSegmentoService(this.OrganizationName, this.IsOffline).RetornoDWKaSegmento();
            //new PotencialdoKAporFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWKaFamilia();
            //new PotencialdoKAporSubfamiliaService(this.OrganizationName, this.IsOffline).RetornoDWKaSubFamilia();
            //new PotencialdoKAporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWKaProduto();
            //new MetaDetalhadadoKAporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWKaDetalhadaProduto();
            #endregion

            #region Meta Supervisor / falta criar o código
            //new PotencialdoSupervisorService(this.OrganizationName, this.IsOffline).RetornoDWSupervisor();
            //new PotencialdoSupervisorporSegmentoService(this.OrganizationName, this.IsOffline).RetornoDWSegmento();
            //new PotencialdoSupervisorporFamiliaService(this.OrganizationName, this.IsOffline).RetornoDWFamilia();
            //new PotencialdoSupervisorporSubfamiliaService(this.OrganizationName, this.IsOffline).RetornoDWSubFamilia();
            //new PotencialdoSupervisorporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWProduto();
            //new PotencialDetalhadodoSupervisorporProdutoService(this.OrganizationName, this.IsOffline).RetornoDWDetalhadoProduto();
            #endregion
        }

        [Test]
        public void AltEntidade()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);

            OrcamentodaUnidade mOrcUni = sRepos.OrcamentodaUnidade.Retrieve(Guid.Parse("9D85FD8D-7428-E411-9235-00155D013E44"));
            //mOrcUni.Ano = 2014;
            //mOrcUni.RazaoStatusOramentoManual = (int)Intelbras.CRM2013.Domain.Enum.OrcamentodaUnidade.RazaodoStatusOramentoManual.ImportarPlanilhaOrcamentoCanalManual;
            mOrcUni.NiveldoOrcamento = (int)Intelbras.CRM2013.Domain.Enum.OrcamentodaUnidade.NivelOrcamento.Resumido;
            sRepos.OrcamentodaUnidade.Update(mOrcUni);

        }
        
        [Test]
        public void AlteraData()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);

            Product mProdut0 = sRepos.Produto.Retrieve(Guid.Parse("47887A73-E705-E411-9420-00155D013D39"));

            DateTime DATA = new DateTime(2014, 07, 15);

            mProdut0.DataUltAlteracaoPVC = DATA;
            sRepos.Produto.Update(mProdut0);
        }

        [Test]
        public void AlteraStatus()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            strFetchXml.Append("<entity name='itbc_orcdetalhadoporproduto'>");
            strFetchXml.Append("<all-attributes />");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            List<OrcamentodaUnidadeDetalhadoporProduto> lst = (List<OrcamentodaUnidadeDetalhadoporProduto>)sRepos.OrcamentodaUnidadeDetalhadoporProduto.RetrieveMultiple(retrieveMultiple.Query).List;

            foreach (OrcamentodaUnidadeDetalhadoporProduto item in lst)
            {
                item.Ano = 2014;
                sRepos.OrcamentodaUnidadeDetalhadoporProduto.Update(item);
            }

        }

        [Test]
        public void Delete()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);

            RetrieveMultipleRequest retrieveMultiple;
            StringBuilder strFetchXml = new StringBuilder();

            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            strFetchXml.Append("<entity name='itbc_metadocanalporproduto'>");
            strFetchXml.Append("<all-attributes />");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            List<MetadoCanalporProduto> lst = (List<MetadoCanalporProduto>)sRepos.MetadoCanalporProduto.RetrieveMultiple(retrieveMultiple.Query).List;
            int contador = 0;
            foreach (MetadoCanalporProduto item in lst)
            {
                sRepos.MetadoCanalporProduto.Delete(item.ID.Value);
                contador++;
            }

            strFetchXml.Clear();
            strFetchXml.Append("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>");
            strFetchXml.Append("<entity name='itbc_metadetalhadadocanalporproduto'>");
            strFetchXml.Append("<all-attributes />");
            strFetchXml.Append("</entity>");
            strFetchXml.Append("</fetch>");

            // Build fetch request and obtain results.
            retrieveMultiple = new RetrieveMultipleRequest()
            {
                Query = new FetchExpression(strFetchXml.ToString())
            };

            List<MetaDetalhadadoCanalporProduto> lst1 = (List<MetaDetalhadadoCanalporProduto>)sRepos.MetaDetalhadadoCanalporProduto.RetrieveMultiple(retrieveMultiple.Query).List;
            int contador1 = 0;
            foreach (MetaDetalhadadoCanalporProduto item in lst1)
            {
                sRepos.MetaDetalhadadoCanalporProduto.Delete(item.ID.Value);
                contador1++;
            }

        }

        [Test]
        public void TesteSolicitacaoBenefico()
        {
            RepositoryService sRepos = new RepositoryService(OrganizationName, IsOffline);
            SolicitacaoBeneficio mSolicitacaoBeneficio = sRepos.SolicitacaoBeneficio.Retrieve(Guid.Parse("79607404-1439-E411-803A-00155D013E44"));
            Tarefa mTarefa = sRepos.Tarefa.ObterPor(Guid.Parse("97CC7BCF-1639-E411-803A-00155D013E44"));
            Guid usuarioId = Guid.Parse("4EBA8596-4E17-E411-9233-00155D013E44");

            //new ProcessoDeSolicitacoesService(sRepos.NomeDaOrganizacao, sRepos.IsOffline).ConcluirTarefaSolicitacaoBeneficio(mTarefa, usuarioId);

            new SolicitacaoBeneficioService(sRepos.NomeDaOrganizacao, sRepos.IsOffline).GerarTarefaSolicBeneficio(mSolicitacaoBeneficio, usuarioId, 1);
        }

    }
}
